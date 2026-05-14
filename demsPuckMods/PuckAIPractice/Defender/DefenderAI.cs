using UnityEngine;

namespace PuckAIPractice.Defender
{
    public class DefenderAI : MonoBehaviour
    {
        public Player ControlledPlayer;
        public ulong TargetClientId;

        // Set by DefenderSpawner.
        //
        // StaticHome is the immutable anchor — the defensive-zone position the bot
        // would patrol if no player were on the ice.
        //
        // HomePosition is the current (dynamic) patrol center, recomputed each frame
        // as StaticHome + currentShift. The shift slides all defenders laterally
        // toward the puck-carrying player so the defensive unit moves as a whole.
        //
        // PatrolAxis is the direction the bot oscillates along — angled inward+forward
        // to funnel attackers toward center. ThreatDirection is the world direction
        // toward the attacking zone — used to bias U-turn rotation so the bot's body
        // never points at its own net.
        public Vector3 StaticHome;
        public Vector3 HomePosition;
        public Vector3 PatrolAxis;
        public Vector3 ThreatDirection;

        [Header("Zone Anchor")]
        public float HomePullBackDistance = 13f;
        public float HomeTuckInDistance = 10f;

        [Header("Patrol Orbit")]
        // The bot orbits HomePosition in a tight circle, managing speed and pulsing
        // crouch for sharp turns. The "aim point" each frame sits on the circle
        // ahead of the bot's current angular position (by CircleLeadDeg). Because
        // the aim is always on the circle, the bot is geometrically pulled toward
        // the correct radius — drift is self-correcting.
        public float CircleRadius = 1.5f;
        // Degrees ahead of the bot's current angle on the orbit. Larger = more
        // tangential motion (faster orbit); smaller = aim is more radial (orbits
        // tighter, slower). Sign chooses orbit direction (CCW for positive).
        public float CircleLeadDeg = 60f;
        // Speed hysteresis (m/s). Below min, forward = 1 until reaching max.
        // Above max, forward = 0 until dropping to min. ~2 m/s ≈ 4.5 mph.
        public float CircleMinSpeed = 1.6f;
        public float CircleMaxSpeed = 2.4f;
        // Distance threshold to enter orbit mode. Inside this radius, the tight
        // pulse-crouch orbit takes over.
        public float OrbitEngageDistance = 1.5f;
        // Distance at which the bot transitions from full-speed transit into the
        // brake-approach phase. Between BrakeStartDistance and OrbitEngageDistance,
        // the bot slams StopInput (the alt-key brake) to kill momentum before
        // entering the orbit — otherwise it sails through the orbit zone at full
        // skating speed and oscillates back and forth across home.
        public float BrakeStartDistance = 5f;
        // Speed (m/s) below which the brake releases and the bot resumes normal
        // approach. Once speed is this low, the remaining 1–3m of approach can be
        // done without overshooting.
        public float BrakeReleaseSpeed = 2f;

        [Header("Zone Shift")]
        // The defender's home position shifts toward the target player's lateral
        // position so the defensive shape moves with the puck. Static defenders are
        // easy to walk around; shifting defenders close the gap. All defenders share
        // the same signal (player's world X), so the whole unit slides together.
        //
        // shift.x = clamp(player.x * LateralShiftFactor, -MaxLateralShift, +MaxLateralShift)
        // HomePosition = StaticHome + shift (smoothed via ShiftSmoothing)
        public float LateralShiftFactor = 0.4f;
        public float MaxLateralShift = 5f;
        // How fast the shift converges toward target. Higher = snappier (less lag).
        public float ShiftSmoothing = 3f;

        [Header("Debug")]
        // Spawn a non-collider visual marker at HomePosition (tracks the dynamic
        // shifted home each frame). Useful while tuning shift/anchor offsets.
        public bool ShowDebugMarker = true;
        public Color DebugMarkerColor = Color.yellow;

        [Header("Patrol Movement")]
        public float PatrolTurnDeadzoneDeg = 5f;
        public float PatrolTurnFullInputAngle = 30f;
        // Pulse the slide (crouch) input on a fixed schedule for tight turns.
        // Held crouch slides too far and murders speed; brief pulses tighten the
        // turn radius without bleeding everything.
        public float PulsePeriod = 0.4f;
        public float PulseDuration = 0.15f;

        [Header("Zone")]
        // Pursuit is zone-based: trigger is the target entering the radius around
        // HomePosition. Adjacent positions' zones should overlap meaningfully so
        // there are no walkthrough gaps. Larger radius also gives the bot more
        // runway to close on a fast attacker — Puck's acceleration is slow enough
        // that a small chase trigger leaves the bot perpetually behind.
        public float ZoneRadius = 16f;
        public float ZoneReturnHysteresis = 4f;

        [Header("Chase Movement")]
        public float ChaseTurnDeadzoneDeg = 5f;
        public float ChaseTurnFullInputAngle = 30f;
        public float ChaseForwardFullAngle = 15f;
        public float ChaseForwardCutoffAngle = 90f;
        public float ChaseSprintMinStamina = 0.2f;
        public float ChaseSprintResumeStamina = 0.4f;
        public float ChaseSprintEngageDistance = 5f;
        public float ChaseSprintReleaseDistance = 3f;

        [Header("Chase Cut Reaction")]
        // dot(recentVel, oldVel) below this = the target just changed direction
        // hard. Triggers a slide burst so the bot can pivot tightly to follow.
        // Same machinery as ChaserAI — without it the bot's wide turn radius
        // gets beaten by a cutting attacker.
        public float ChaseCutDotThreshold = 0.3f;
        // Ignore cuts when target is barely moving.
        public float ChaseCutDetectMinSpeed = 2f;
        // Minimum gap between cut-triggered slide bursts. Effectively the
        // rate-limit on cut reactions since cut detection itself is gated here.
        public float ChaseCutReactionCooldown = 0.5f;

        [Header("Chase Slide")]
        // Hard cap on the slide burst duration. Released earlier if the bot
        // re-aligns with the target inside ChaseSlideReleaseAngle.
        public float ChaseSlideMaxDuration = 0.35f;
        // Cooldown after a chase slide ends before another can start from a
        // non-cut trigger. Cuts bypass this — see UpdateChaseSlide.
        public float ChaseSlideCooldown = 1.2f;
        // Release angle for an active chase slide. Once the bot's facing is
        // within this of the target direction, the slide ends.
        public float ChaseSlideReleaseAngle = 20f;

        [Header("Stick Aim")]
        public float StickAimForwardExtension = 0.3f;
        public float StickMaxAimDistance = 1.8f;

        private enum State { Patrolling, Chasing }
        private State state = State.Patrolling;

        private bool isSprinting;
        private bool isPatrolSliding;
        private bool isBraking;                 // StopInput latch for brake-approach
        private bool circleHoldingForward;     // hysteresis latch for orbit speed
        private float patrolPulseTime;          // accumulator for crouch pulse phase
        private GameObject debugMarker;
        private Vector3 currentShift;

        // Cut-detection state. EMAs of the target's planar velocity are updated
        // every frame regardless of state so they're warm whenever chase begins.
        private Vector3 recentTargetVel;
        private Vector3 oldTargetVel;
        private float lastCutDetectedTime = -1000f;

        // Chase slide state. Distinct from isPatrolSliding (which is the orbit
        // pulse-crouch) so the two slide systems don't fight over SlideInput.
        private bool isChaseSliding;
        private float chaseSlideStartTime = -1000f;
        private float lastChaseSlideEndTime = -1000f;

        void Start()
        {
            // Initialize the dynamic home to the static anchor so the first frame's
            // patrol/chase logic and the debug marker have a sane value before the
            // first UpdateZoneShift call.
            HomePosition = StaticHome;
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

        private void UpdateDebugMarkerPosition()
        {
            if (debugMarker == null) return;
            debugMarker.transform.position = HomePosition + Vector3.up * 2f;
        }

        // Slide HomePosition laterally toward the target player so the defensive
        // formation moves with the puck. Each defender independently computes the
        // shift, but because the signal is just the player's world X, all defenders
        // get the same shift and the unit translates as a whole. The shift is
        // smoothed (exponential lerp) so the formation glides instead of snapping.
        private void UpdateZoneShift(Player target)
        {
            Vector3 desiredShift = Vector3.zero;
            if (target != null && target.PlayerBody != null)
            {
                float playerX = target.PlayerBody.transform.position.x;
                float shiftX = Mathf.Clamp(
                    playerX * LateralShiftFactor,
                    -MaxLateralShift,
                    MaxLateralShift);
                desiredShift = new Vector3(shiftX, 0f, 0f);
            }

            currentShift = Vector3.Lerp(currentShift, desiredShift, Time.deltaTime * ShiftSmoothing);
            HomePosition = StaticHome + currentShift;
        }

        void Update()
        {
            if (ControlledPlayer == null) return;
            var input = ControlledPlayer.PlayerInput;
            var body = ControlledPlayer.PlayerBody;
            if (input == null || body == null) return;

            var target = PlayerManager.Instance.GetPlayerByClientId(TargetClientId);

            // Recompute the dynamic home (StaticHome + lateral shift toward player)
            // before any patrol/chase/zone logic reads HomePosition.
            UpdateZoneShift(target);
            UpdateDebugMarkerPosition();

            // Track target velocity continuously so cut detection has warm data
            // the moment chase mode engages. Dual EMAs at the same rates as
            // ChaserAI — recent (~67ms time constant) vs old (~330ms).
            if (target != null && target.PlayerBody != null)
            {
                Vector3 rawTargetVel = target.PlayerBody.Rigidbody.velocity;
                rawTargetVel.y = 0f;
                recentTargetVel = Vector3.Lerp(recentTargetVel, rawTargetVel, Time.deltaTime * 15f);
                oldTargetVel = Vector3.Lerp(oldTargetVel, rawTargetVel, Time.deltaTime * 3f);
            }

            if (state == State.Patrolling && ShouldChase(target))
            {
                state = State.Chasing;
                if (isPatrolSliding)
                {
                    isPatrolSliding = false;
                    input.SlideInput.ServerValue = false;
                }
                if (isBraking)
                {
                    isBraking = false;
                    input.StopInput.ServerValue = false;
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
                // Release any active chase slide so patrol's pulse-crouch isn't
                // fighting against a still-held SlideInput from chase.
                if (isChaseSliding)
                {
                    isChaseSliding = false;
                    lastChaseSlideEndTime = Time.time;
                    input.SlideInput.ServerValue = false;
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

        // Patrol has three sub-modes based on distance to home:
        //   * Transit (dist > BrakeStartDistance): full-speed straight skating.
        //   * BrakeApproach (OrbitEngageDistance < dist <= BrakeStartDistance): slam
        //     StopInput while steering toward home until speed bleeds off, then
        //     coast in. Prevents the overshoot/oscillation around home.
        //   * Orbit (dist <= OrbitEngageDistance): tight pulse-crouch orbit.
        private void DoPatrol(PlayerInput input, PlayerBody body)
        {
            Vector3 botPos = body.transform.position;
            float distFromHome = Vector3.Distance(botPos, HomePosition);

            if (distFromHome > BrakeStartDistance)
            {
                DoTransitToHome(input, body, botPos);
            }
            else if (distFromHome > OrbitEngageDistance)
            {
                DoBrakeApproach(input, body, botPos);
            }
            else
            {
                DoOrbit(input, body, botPos);
            }
        }

        // Brake-approach: bot is close enough to home that further full-speed transit
        // would overshoot, but not yet in the orbit zone. While speed is above
        // BrakeReleaseSpeed, slam StopInput (alt-key brake) to kill linear momentum;
        // turn input still works so the bot keeps facing toward home while braking.
        // Once speed has dropped, release the brake and resume normal forward approach
        // (which will now arrive at low speed and not overshoot).
        private void DoBrakeApproach(PlayerInput input, PlayerBody body, Vector3 botPos)
        {
            float currentSpeed = body.Rigidbody.velocity.magnitude;

            if (currentSpeed > BrakeReleaseSpeed)
            {
                if (!isBraking)
                {
                    isBraking = true;
                    input.StopInput.ServerValue = true;
                }

                Vector3 toHome = HomePosition - botPos;
                toHome.y = 0f;

                Vector3 forwardDir = body.transform.forward;
                forwardDir.y = 0f;
                float signedAngle = Vector3.SignedAngle(forwardDir, toHome, Vector3.up);
                float turn = (Mathf.Abs(signedAngle) > PatrolTurnDeadzoneDeg)
                    ? Mathf.Clamp(signedAngle / PatrolTurnFullInputAngle, -1f, 1f)
                    : 0f;

                input.MoveInput.ServerValue = new Vector2(turn, 0f);
            }
            else
            {
                if (isBraking)
                {
                    isBraking = false;
                    input.StopInput.ServerValue = false;
                }
                DoTransitToHome(input, body, botPos);
            }

            // No crouch pulse while braking — releases any lingering pulse-on.
            if (isPatrolSliding)
            {
                isPatrolSliding = false;
                input.SlideInput.ServerValue = false;
            }
        }

        // Full-speed straight skating toward HomePosition. Used when the bot is far
        // enough from its patrol point that orbiting (slow + crouching) would take
        // forever. Releases any active patrol slide so the bot isn't crouched while
        // sprinting across the ice.
        private void DoTransitToHome(PlayerInput input, PlayerBody body, Vector3 botPos)
        {
            Vector3 toHome = HomePosition - botPos;
            toHome.y = 0f;

            Vector3 forwardDir = body.transform.forward;
            forwardDir.y = 0f;
            float signedAngle = Vector3.SignedAngle(forwardDir, toHome, Vector3.up);
            float absAngle = Mathf.Abs(signedAngle);

            float turn = (absAngle > PatrolTurnDeadzoneDeg)
                ? Mathf.Clamp(signedAngle / PatrolTurnFullInputAngle, -1f, 1f)
                : 0f;

            float fwd;
            if (absAngle <= 15f) fwd = 1f;
            else if (absAngle >= 90f) fwd = 0f;
            else fwd = 1f - Mathf.InverseLerp(15f, 90f, absAngle);

            input.MoveInput.ServerValue = new Vector2(turn, fwd);

            if (isPatrolSliding)
            {
                isPatrolSliding = false;
                input.SlideInput.ServerValue = false;
            }
            if (isBraking)
            {
                isBraking = false;
                input.StopInput.ServerValue = false;
            }
        }

        // Tight orbit around HomePosition. Each frame:
        //   1. Compute bot's current angle around the pole (atan2 of bot-pole offset).
        //   2. Aim point = a fixed lead-angle ahead of the bot on the desired circle.
        //      Because the aim sits on the circle at fixed radius, drift is
        //      self-correcting — the bot is geometrically pulled toward the right
        //      radius no matter where it is.
        //   3. Forward input is hysteresis-latched on speed (below min → 1, above
        //      max → 0). Bot coasts most of the time, pushes only when it slows.
        //   4. Slide (crouch) is pulsed on a fixed schedule for tight turning
        //      without bleeding all the speed.
        private void DoOrbit(PlayerInput input, PlayerBody body, Vector3 botPos)
        {
            Vector3 botRelative = botPos - HomePosition;
            botRelative.y = 0f;

            // If bot is essentially AT the pole, kick out in the threat direction
            // so we have a defined angle to orbit from.
            if (botRelative.sqrMagnitude < 0.04f)
            {
                botRelative = ThreatDirection * CircleRadius;
            }

            float botAngle = Mathf.Atan2(botRelative.z, botRelative.x);
            float aimAngle = botAngle + CircleLeadDeg * Mathf.Deg2Rad;
            Vector3 aimOffset = new Vector3(
                Mathf.Cos(aimAngle) * CircleRadius,
                0f,
                Mathf.Sin(aimAngle) * CircleRadius);
            Vector3 aimPoint = HomePosition + aimOffset;

            Vector3 toAim = aimPoint - botPos;
            toAim.y = 0f;

            Vector3 forwardDir = body.transform.forward;
            forwardDir.y = 0f;
            float signedAngle = Vector3.SignedAngle(forwardDir, toAim, Vector3.up);
            float absAngle = Mathf.Abs(signedAngle);
            float turn = (absAngle > PatrolTurnDeadzoneDeg)
                ? Mathf.Clamp(signedAngle / PatrolTurnFullInputAngle, -1f, 1f)
                : 0f;

            float currentSpeed = body.Rigidbody.velocity.magnitude;
            if (currentSpeed < CircleMinSpeed) circleHoldingForward = true;
            else if (currentSpeed > CircleMaxSpeed) circleHoldingForward = false;
            float fwd = circleHoldingForward ? 1f : 0f;

            input.MoveInput.ServerValue = new Vector2(turn, fwd);

            if (isBraking)
            {
                isBraking = false;
                input.StopInput.ServerValue = false;
            }

            UpdatePatrolPulse(input);
        }

        // Crouch pulse on a fixed schedule — first PulseDuration seconds of each
        // PulsePeriod window the slide is on, then off. Tight turn radius without
        // killing all the bot's speed.
        private void UpdatePatrolPulse(PlayerInput input)
        {
            patrolPulseTime += Time.deltaTime;
            float period = Mathf.Max(0.05f, PulsePeriod);
            float phase = patrolPulseTime % period;
            bool slideThisFrame = phase < PulseDuration;

            if (slideThisFrame != isPatrolSliding)
            {
                isPatrolSliding = slideThisFrame;
                input.SlideInput.ServerValue = slideThisFrame;
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
                // Don't leave SlideInput hot if we ended up sitting on the target.
                if (isChaseSliding)
                {
                    isChaseSliding = false;
                    lastChaseSlideEndTime = Time.time;
                    input.SlideInput.ServerValue = false;
                }
                return;
            }

            // Detect a hard direction change on the target before writing inputs,
            // so the slide burst this frame can start in lockstep with the cut.
            bool cutDetected = DetectChaseCut();

            WriteMoveInput(
                input, body, toTarget, dist,
                ChaseTurnDeadzoneDeg, ChaseTurnFullInputAngle,
                ChaseForwardFullAngle, ChaseForwardCutoffAngle,
                throttleStartDist: 0f, throttleMinForward: 1f);

            // Recompute the bot→target signed angle so UpdateChaseSlide can
            // release the slide once the bot has re-aligned with the target.
            Vector3 forwardDir = body.transform.forward;
            forwardDir.y = 0f;
            float signedAngle = Vector3.SignedAngle(forwardDir, toTarget, Vector3.up);
            float absAngle = Mathf.Abs(signedAngle);

            UpdateChaseSlide(input, absAngle, cutDetected);

            float stamina = body.Stamina.Value;
            if (isSprinting)
            {
                // Sprint during a slide is wasted — sliding kills traction.
                if (stamina < ChaseSprintMinStamina || dist < ChaseSprintReleaseDistance || isChaseSliding)
                {
                    isSprinting = false;
                    input.SprintInput.ServerValue = false;
                }
            }
            else
            {
                if (stamina > ChaseSprintResumeStamina && dist > ChaseSprintEngageDistance && !isChaseSliding)
                {
                    isSprinting = true;
                    input.SprintInput.ServerValue = true;
                }
            }
        }

        // Fires once when the target's recent velocity direction has diverged
        // sharply from its older velocity direction — i.e. the target just cut.
        // Self-cooldowns via lastCutDetectedTime so a sustained turn doesn't
        // fire every frame.
        private bool DetectChaseCut()
        {
            float now = Time.time;
            if ((now - lastCutDetectedTime) < ChaseCutReactionCooldown) return false;
            if (recentTargetVel.magnitude < ChaseCutDetectMinSpeed) return false;
            if (oldTargetVel.magnitude < ChaseCutDetectMinSpeed) return false;

            float align = Vector3.Dot(recentTargetVel.normalized, oldTargetVel.normalized);
            if (align < ChaseCutDotThreshold)
            {
                lastCutDetectedTime = now;
                return true;
            }
            return false;
        }

        // Manages the chase-mode slide burst. A detected cut starts a slide
        // (bypassing ChaseSlideCooldown — the cut detector's own cooldown is
        // the real rate-limit). The slide ends when the duration expires OR
        // the bot's facing has re-aligned with the target inside
        // ChaseSlideReleaseAngle.
        private void UpdateChaseSlide(PlayerInput input, float absAngle, bool cutDetected)
        {
            float now = Time.time;

            if (isChaseSliding)
            {
                bool durationExpired = (now - chaseSlideStartTime) > ChaseSlideMaxDuration;
                bool aligned = absAngle < ChaseSlideReleaseAngle;
                if (durationExpired || aligned)
                {
                    isChaseSliding = false;
                    lastChaseSlideEndTime = now;
                    input.SlideInput.ServerValue = false;
                }
                return;
            }

            if (cutDetected)
            {
                isChaseSliding = true;
                chaseSlideStartTime = now;
                input.SlideInput.ServerValue = true;
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
