using UnityEngine;

namespace PuckAIPractice.Defender
{
    public class DefenderAI : MonoBehaviour
    {
        public Player ControlledPlayer;
        public ulong TargetClientId;

        // Set by DefenderSpawner. HomePosition is the patrol center. PatrolAxis is the
        // direction the bot oscillates along — angled inward+forward to funnel attackers
        // toward center. ThreatDirection is the world direction toward the attacking
        // zone — used to bias U-turn rotation so the bot's body never points at its
        // own net.
        public Vector3 HomePosition;
        public Vector3 PatrolAxis;
        public Vector3 ThreatDirection;

        [Header("Zone Anchor")]
        public float HomePullBackDistance = 13f;
        public float HomeTuckInDistance = 10f;

        [Header("Patrol")]
        // Half the length of the patrol stroke. The bot oscillates between
        // HomePosition ± PatrolAxis × this value. Keep short — real defenders make
        // small adjustments, they don't traverse the whole zone.
        public float PatrolHalfLength = 1.5f;
        // Distance threshold for "arrived at endpoint" → switch to U-turn phase.
        public float PatrolArrivalDeadzone = 0.6f;
        // body.forward · oppositeAxis must exceed this to exit the U-turn phase and
        // start stroking the other way. 0.8 ≈ within ~37° of opposite axis.
        public float UTurnExitDot = 0.8f;

        [Header("Debug")]
        // Spawn a non-collider visual marker at HomePosition so the patrol center can
        // be confirmed visually in-game. Useful while tuning per-position offsets.
        public bool ShowDebugMarker = true;
        public Color DebugMarkerColor = Color.yellow;

        [Header("Patrol Movement")]
        public float PatrolTurnDeadzoneDeg = 5f;
        public float PatrolTurnFullInputAngle = 30f;
        public float PatrolForwardFullAngle = 15f;
        public float PatrolForwardCutoffAngle = 90f;
        public float PatrolThrottleStartDistance = 3f;
        public float PatrolThrottleMinForward = 0.3f;

        [Header("Zone")]
        // Pursuit is zone-based: trigger is the target entering the radius around
        // HomePosition. Adjacent positions' zones should overlap meaningfully so there
        // are no walkthrough gaps.
        public float ZoneRadius = 12f;
        public float ZoneReturnHysteresis = 3f;

        [Header("Chase Movement")]
        public float ChaseTurnDeadzoneDeg = 5f;
        public float ChaseTurnFullInputAngle = 30f;
        public float ChaseForwardFullAngle = 15f;
        public float ChaseForwardCutoffAngle = 90f;
        public float ChaseSprintMinStamina = 0.2f;
        public float ChaseSprintResumeStamina = 0.4f;
        public float ChaseSprintEngageDistance = 5f;
        public float ChaseSprintReleaseDistance = 3f;

        [Header("Stick Aim")]
        public float StickAimForwardExtension = 0.3f;
        public float StickMaxAimDistance = 1.8f;

        private enum State { Patrolling, Chasing }
        private State state = State.Patrolling;

        private enum PatrolPhase
        {
            StrokingPositive,
            UTurningAtPositive,
            StrokingNegative,
            UTurningAtNegative,
        }
        private PatrolPhase patrolPhase = PatrolPhase.StrokingPositive;

        private bool isSprinting;
        private bool isPatrolSliding;
        private GameObject debugMarker;

        void Start()
        {
            if (ShowDebugMarker)
            {
                CreateDebugMarker();
            }
        }

        void OnDestroy()
        {
            if (debugMarker != null) Destroy(debugMarker);
        }

        private void CreateDebugMarker()
        {
            debugMarker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            debugMarker.name = $"DefenderHomeMarker_{(ControlledPlayer != null ? ControlledPlayer.Username.Value.ToString() : "unknown")}";
            var col = debugMarker.GetComponent<Collider>();
            if (col != null) Destroy(col);
            // Default cylinder is 2 units tall, radius 0.5. Scale: thin pole 4m tall.
            debugMarker.transform.localScale = new Vector3(0.2f, 2f, 0.2f);
            debugMarker.transform.position = HomePosition + Vector3.up * 2f;
            var renderer = debugMarker.GetComponent<Renderer>();
            if (renderer != null) renderer.material.color = DebugMarkerColor;
        }

        void Update()
        {
            if (ControlledPlayer == null) return;
            var input = ControlledPlayer.PlayerInput;
            var body = ControlledPlayer.PlayerBody;
            if (input == null || body == null) return;

            var target = PlayerManager.Instance.GetPlayerByClientId(TargetClientId);

            if (state == State.Patrolling && ShouldChase(target))
            {
                state = State.Chasing;
                if (isPatrolSliding)
                {
                    isPatrolSliding = false;
                    input.SlideInput.ServerValue = false;
                }
            }
            else if (state == State.Chasing && ShouldReturnToPatrol(target))
            {
                state = State.Patrolling;
                if (isSprinting)
                {
                    isSprinting = false;
                    input.SprintInput.ServerValue = false;
                }
            }

            if (state == State.Patrolling)
            {
                DoPatrol(input, body);
            }
            else
            {
                DoChase(input, body, target);
            }

            if (target != null && target.PlayerBody != null)
            {
                AimStickAtPuck(input, target.PlayerBody.transform.position);
            }
        }

        private bool ShouldChase(Player target)
        {
            if (target == null || target.PlayerBody == null) return false;
            float distFromHome = Vector3.Distance(target.PlayerBody.transform.position, HomePosition);
            return distFromHome < ZoneRadius;
        }

        private bool ShouldReturnToPatrol(Player target)
        {
            if (target == null || target.PlayerBody == null) return true;
            float distFromHome = Vector3.Distance(target.PlayerBody.transform.position, HomePosition);
            return distFromHome > (ZoneRadius + ZoneReturnHysteresis);
        }

        // Patrol is a 4-phase state machine, not a sine wave:
        //   Stroking{Positive,Negative} — skating toward a patrol endpoint
        //   UTurningAt{Positive,Negative} — pivoting in place at an endpoint
        //
        // Critically, U-turn rotation direction is FORCED so the body passes through
        // facing-threat (toward attackers), never through facing-own-goal. We do this
        // by directly overriding turn input with sign(SignedAngle(currentAxisFacing,
        // ThreatDirection)). Without the override the bot picks the shortest rotation,
        // which at the "outer-back" endpoint goes through facing-own-goal — exactly
        // the failure the user identified ("they turn too slow when they're facing the
        // wrong way and pursuit kicks in").
        private void DoPatrol(PlayerInput input, PlayerBody body)
        {
            Vector3 botPos = body.transform.position;
            Vector3 forwardDir = body.transform.forward;
            forwardDir.y = 0f;
            if (forwardDir.sqrMagnitude > 0.0001f) forwardDir.Normalize();
            else forwardDir = Vector3.forward;

            Vector3 endpointPositive = HomePosition + PatrolAxis * PatrolHalfLength;
            Vector3 endpointNegative = HomePosition - PatrolAxis * PatrolHalfLength;

            switch (patrolPhase)
            {
                case PatrolPhase.StrokingPositive:
                    StrokeToward(input, body, botPos, endpointPositive,
                        switchToPhase: PatrolPhase.UTurningAtPositive);
                    break;

                case PatrolPhase.UTurningAtPositive:
                    UTurnPivot(input, forwardDir,
                        fromAxis: PatrolAxis,
                        exitAxis: -PatrolAxis,
                        nextPhase: PatrolPhase.StrokingNegative);
                    break;

                case PatrolPhase.StrokingNegative:
                    StrokeToward(input, body, botPos, endpointNegative,
                        switchToPhase: PatrolPhase.UTurningAtNegative);
                    break;

                case PatrolPhase.UTurningAtNegative:
                    UTurnPivot(input, forwardDir,
                        fromAxis: -PatrolAxis,
                        exitAxis: PatrolAxis,
                        nextPhase: PatrolPhase.StrokingPositive);
                    break;
            }

            // Hard slide (crouch) only during the U-turn phases — held throughout the
            // entire pivot, not pulsed. Sharp turn at endpoints, bleeds speed as the
            // cost of a tight 180°. Released during strokes so the bot can accelerate
            // on the straightaway.
            bool shouldSlide = (patrolPhase == PatrolPhase.UTurningAtPositive
                             || patrolPhase == PatrolPhase.UTurningAtNegative);
            if (shouldSlide != isPatrolSliding)
            {
                isPatrolSliding = shouldSlide;
                input.SlideInput.ServerValue = shouldSlide;
            }
        }

        private void StrokeToward(
            PlayerInput input, PlayerBody body, Vector3 botPos, Vector3 endpoint,
            PatrolPhase switchToPhase)
        {
            Vector3 toEndpoint = endpoint - botPos;
            toEndpoint.y = 0f;
            float dist = toEndpoint.magnitude;

            if (dist < PatrolArrivalDeadzone)
            {
                input.MoveInput.ServerValue = Vector2.zero;
                patrolPhase = switchToPhase;
                return;
            }

            WriteMoveInput(
                input, body, toEndpoint, dist,
                PatrolTurnDeadzoneDeg, PatrolTurnFullInputAngle,
                PatrolForwardFullAngle, PatrolForwardCutoffAngle,
                PatrolThrottleStartDistance, PatrolThrottleMinForward);
        }

        private void UTurnPivot(
            PlayerInput input, Vector3 forwardDir,
            Vector3 fromAxis, Vector3 exitAxis, PatrolPhase nextPhase)
        {
            // Force rotation in the direction that goes through facing-threat.
            // SignedAngle(fromAxis, ThreatDirection) tells us which side the threat
            // is on relative to the axis the bot was just stroking. We push turn
            // input in that direction and continue until the body has rotated all
            // the way through to the opposite axis.
            float turnDir = Mathf.Sign(Vector3.SignedAngle(fromAxis, ThreatDirection, Vector3.up));
            if (Mathf.Approximately(turnDir, 0f)) turnDir = 1f;

            input.MoveInput.ServerValue = new Vector2(turnDir, 0f);

            if (Vector3.Dot(forwardDir, exitAxis) > UTurnExitDot)
            {
                patrolPhase = nextPhase;
            }
        }

        private void DoChase(PlayerInput input, PlayerBody body, Player target)
        {
            if (target == null || target.PlayerBody == null)
            {
                input.MoveInput.ServerValue = Vector2.zero;
                return;
            }

            Vector3 botPos = body.transform.position;
            Vector3 targetPos = target.PlayerBody.transform.position;
            Vector3 toTarget = targetPos - botPos;
            toTarget.y = 0f;
            float dist = toTarget.magnitude;

            if (dist < 0.1f)
            {
                input.MoveInput.ServerValue = Vector2.zero;
                return;
            }

            WriteMoveInput(
                input, body, toTarget, dist,
                ChaseTurnDeadzoneDeg, ChaseTurnFullInputAngle,
                ChaseForwardFullAngle, ChaseForwardCutoffAngle,
                throttleStartDist: 0f, throttleMinForward: 1f);

            float stamina = body.Stamina.Value;
            if (isSprinting)
            {
                if (stamina < ChaseSprintMinStamina || dist < ChaseSprintReleaseDistance)
                {
                    isSprinting = false;
                    input.SprintInput.ServerValue = false;
                }
            }
            else
            {
                if (stamina > ChaseSprintResumeStamina && dist > ChaseSprintEngageDistance)
                {
                    isSprinting = true;
                    input.SprintInput.ServerValue = true;
                }
            }
        }

        private void WriteMoveInput(
            PlayerInput input, PlayerBody body, Vector3 toAim, float dist,
            float turnDeadzoneDeg, float turnFullInputAngle,
            float forwardFullAngle, float forwardCutoffAngle,
            float throttleStartDist, float throttleMinForward)
        {
            Vector3 forwardDir = body.transform.forward;
            forwardDir.y = 0f;
            float signedAngle = Vector3.SignedAngle(forwardDir, toAim, Vector3.up);
            float absAngle = Mathf.Abs(signedAngle);

            float turn = (absAngle > turnDeadzoneDeg)
                ? Mathf.Clamp(signedAngle / turnFullInputAngle, -1f, 1f)
                : 0f;

            float fwd;
            if (absAngle <= forwardFullAngle) fwd = 1f;
            else if (absAngle >= forwardCutoffAngle) fwd = 0f;
            else fwd = 1f - Mathf.InverseLerp(forwardFullAngle, forwardCutoffAngle, absAngle);

            if (throttleStartDist > 0f && dist < throttleStartDist)
            {
                float t = Mathf.InverseLerp(0f, throttleStartDist, dist);
                fwd *= Mathf.Lerp(throttleMinForward, 1f, t);
            }

            input.MoveInput.ServerValue = new Vector2(turn, fwd);
        }

        // See CHASER_HANDOFF.md §9 for the stick-aim inverse math.
        private void AimStickAtPuck(PlayerInput input, Vector3 targetPos)
        {
            var positioner = ControlledPlayer.StickPositioner;
            if (positioner == null) return;

            var puck = PickFocusPuck(targetPos);
            if (puck == null) return;

            Vector3 originPos = positioner.RaycastOriginPosition;
            Vector3 puckPos = puck.transform.position;
            Vector3 botPos = ControlledPlayer.PlayerBody.transform.position;

            Vector3 horizToPuck = puckPos - botPos;
            horizToPuck.y = 0f;
            float horizDist = horizToPuck.magnitude;
            Vector3 horizDir = horizDist > 0.0001f
                ? horizToPuck / horizDist
                : ControlledPlayer.PlayerBody.transform.forward;
            horizDir.y = 0f;
            if (horizDir.sqrMagnitude < 0.0001f) return;
            horizDir.Normalize();

            float aimDist = Mathf.Min(horizDist + StickAimForwardExtension, StickMaxAimDistance);
            Vector3 aimPoint = botPos + horizDir * aimDist;
            aimPoint.y = puckPos.y;

            Vector3 toAim = aimPoint - originPos;
            if (toAim.sqrMagnitude < 0.0001f) return;

            Vector3 dirLocal = positioner.transform.InverseTransformDirection(toAim.normalized);
            float clampedY = Mathf.Clamp(dirLocal.y, -1f, 1f);
            float pitch = Mathf.Asin(-clampedY) * Mathf.Rad2Deg;
            float yaw = Mathf.Atan2(dirLocal.x, dirLocal.z) * Mathf.Rad2Deg;

            input.StickRaycastOriginAngleInput.ServerValue = new Vector2(pitch, yaw);
        }

        private Puck PickFocusPuck(Vector3 targetPos)
        {
            if (PuckManager.Instance == null) return null;
            var pucks = PuckManager.Instance.GetPucks(false);
            if (pucks == null || pucks.Count == 0) return null;

            Puck best = null;
            float bestSqr = float.MaxValue;
            for (int i = 0; i < pucks.Count; i++)
            {
                var p = pucks[i];
                if (p == null) continue;
                float sqr = (p.transform.position - targetPos).sqrMagnitude;
                if (sqr < bestSqr)
                {
                    bestSqr = sqr;
                    best = p;
                }
            }
            return best;
        }
    }
}
