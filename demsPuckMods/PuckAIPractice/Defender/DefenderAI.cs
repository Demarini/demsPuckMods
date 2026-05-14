using UnityEngine;

namespace PuckAIPractice.Defender
{
    public class DefenderAI : MonoBehaviour
    {
        public Player ControlledPlayer;
        public ulong TargetClientId;

        // Set by DefenderSpawner immediately after AddComponent.
        // HomePosition is pulled back from spawn toward own net and tucked inward
        // toward the rink centerline (defensive zone position). PatrolAxis is the
        // lateral direction along which the bot oscillates.
        public Vector3 HomePosition;
        public Vector3 PatrolAxis;

        [Header("Zone Anchor")]
        // How far back from the spawn (faceoff) position toward own net the home is.
        public float HomePullBackDistance = 13f;
        // How far inward (toward rink centerline) the home is. C will collapse to ~0
        // since it spawns near center; wings and defensemen get pulled in.
        public float HomeTuckInDistance = 10f;

        [Header("Patrol")]
        // Half-width of the lateral oscillation around HomePosition.
        public float PatrolAmplitude = 3f;
        // Seconds for one full back-and-forth cycle.
        public float PatrolPeriod = 4f;
        // Distance threshold for considering the bot "at" its patrol target.
        public float PatrolArrivalDeadzone = 0.5f;

        [Header("Patrol Slide Pulse")]
        // Pulse the slide (crouch) input on a periodic cycle while patrolling. Crouch
        // turns much tighter than a regular turn, so brief pulses keep the bot pivot-
        // ready and prevent it from drifting wide on each end of the stroke. Without
        // this the patrol arc gets so wide it goes off-zone.
        public float PatrolSlidePulsePeriod = 0.6f;
        // How long the slide is held within each pulse period.
        public float PatrolSlidePulseDuration = 0.18f;

        [Header("Patrol Movement")]
        public float PatrolTurnDeadzoneDeg = 5f;
        public float PatrolTurnFullInputAngle = 30f;
        public float PatrolForwardFullAngle = 15f;
        public float PatrolForwardCutoffAngle = 90f;
        public float PatrolThrottleStartDistance = 3f;
        public float PatrolThrottleMinForward = 0.3f;

        [Header("Zone")]
        // Pursuit is zone-based: the trigger is the target entering the radius around
        // HomePosition, NOT the target being close to the bot. A good defender plays
        // their zone, not their personal space. Radius is sized so adjacent positions'
        // zones overlap with a decent margin — coverage is continuous across the
        // defensive zone, no gaps for an attacker to walk through.
        public float ZoneRadius = 12f;
        // Hysteresis on zone exit — target must travel ZoneRadius + this far from home
        // before the bot returns to patrol. Prevents flicker when the target hovers
        // exactly at the zone boundary.
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

        // Sprint state for chase mode
        private bool isSprinting;

        // Patrol oscillation phase
        private float patrolTime;

        // Patrol slide pulse state
        private float patrolSlidePulseTime;
        private bool isPatrolSliding;

        void Update()
        {
            if (ControlledPlayer == null) return;
            var input = ControlledPlayer.PlayerInput;
            var body = ControlledPlayer.PlayerBody;
            if (input == null || body == null) return;

            var target = PlayerManager.Instance.GetPlayerByClientId(TargetClientId);

            // State transitions — zone-based.
            if (state == State.Patrolling && ShouldChase(target))
            {
                state = State.Chasing;
                ReleasePatrolSlide(input);
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

        // Zone-based chase trigger. The bot promotes from Patrol → Chase when the
        // target is inside the zone radius around HomePosition — independent of how
        // close the target is to the bot itself.
        private bool ShouldChase(Player target)
        {
            if (target == null || target.PlayerBody == null) return false;
            Vector3 targetPos = target.PlayerBody.transform.position;
            float distFromHome = Vector3.Distance(targetPos, HomePosition);
            return distFromHome < ZoneRadius;
        }

        private bool ShouldReturnToPatrol(Player target)
        {
            if (target == null || target.PlayerBody == null) return true;
            Vector3 targetPos = target.PlayerBody.transform.position;
            float distFromHome = Vector3.Distance(targetPos, HomePosition);
            return distFromHome > (ZoneRadius + ZoneReturnHysteresis);
        }

        private void DoPatrol(PlayerInput input, PlayerBody body)
        {
            patrolTime += Time.deltaTime;
            float phase = (patrolTime / Mathf.Max(0.01f, PatrolPeriod)) * Mathf.PI * 2f;
            Vector3 patrolOffset = PatrolAxis * (Mathf.Sin(phase) * PatrolAmplitude);
            Vector3 patrolTarget = HomePosition + patrolOffset;

            Vector3 botPos = body.transform.position;
            Vector3 toTarget = patrolTarget - botPos;
            toTarget.y = 0f;
            float dist = toTarget.magnitude;

            if (dist < PatrolArrivalDeadzone)
            {
                input.MoveInput.ServerValue = Vector2.zero;
            }
            else
            {
                WriteMoveInput(
                    input, body, toTarget, dist,
                    PatrolTurnDeadzoneDeg, PatrolTurnFullInputAngle,
                    PatrolForwardFullAngle, PatrolForwardCutoffAngle,
                    PatrolThrottleStartDistance, PatrolThrottleMinForward);
            }

            UpdatePatrolSlidePulse(input);
        }

        // Pulse the slide input on a periodic schedule. Within each PatrolSlidePulsePeriod
        // window, the slide is held for the first PatrolSlidePulseDuration seconds and
        // released for the rest. Keeps the bot's turn radius small without bleeding all
        // its speed.
        private void UpdatePatrolSlidePulse(PlayerInput input)
        {
            patrolSlidePulseTime += Time.deltaTime;
            float period = Mathf.Max(0.05f, PatrolSlidePulsePeriod);
            float phase = patrolSlidePulseTime % period;
            bool slideThisFrame = phase < PatrolSlidePulseDuration;

            if (slideThisFrame != isPatrolSliding)
            {
                isPatrolSliding = slideThisFrame;
                input.SlideInput.ServerValue = slideThisFrame;
            }
        }

        private void ReleasePatrolSlide(PlayerInput input)
        {
            if (isPatrolSliding)
            {
                isPatrolSliding = false;
                input.SlideInput.ServerValue = false;
            }
            patrolSlidePulseTime = 0f;
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

        // Shared move-toward-aim computation. Picks turn input from signed angle and
        // forward input from absolute angle, with an optional close-range throttle.
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

        // Same stick-aim implementation as ChaserAI — see CHASER_HANDOFF.md §9 for
        // the full explanation of the inverse math and on-ice aim-clamp strategy.
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
