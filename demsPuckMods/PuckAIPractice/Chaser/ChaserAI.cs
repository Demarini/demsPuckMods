using UnityEngine;

namespace PuckAIPractice.Chaser
{
    public class ChaserAI : MonoBehaviour
    {
        public Player ControlledPlayer;
        public ulong TargetClientId;

        [Header("Turn")]
        public float TurnDeadzoneDeg = 5f;
        public float TurnFullInputAngle = 30f;

        [Header("Forward")]
        public float ForwardFullAngle = 15f;
        public float ForwardCutoffAngle = 90f;

        [Header("Slide")]
        // Slide only fires when reacting to a player cut or committing to a Predict.
        // The bot does not slide for geometric angle alone — going straight at it
        // should not trigger a panic crouch.
        public float SlideReleaseAngle = 20f;
        public float SlideMaxDuration = 0.35f;
        public float SlideCooldown = 1.2f;

        [Header("Pursuit / Lead")]
        // Time horizon to lead the target by (seconds), scaled by distance.
        public float MaxLeadTime = 1.2f;
        // distance / this = leadTime (up to MaxLeadTime). Smaller = more lead per meter.
        public float LeadDistanceScale = 5f;
        // Inside this radius the lead linearly fades to zero — bot goes to the spot,
        // not the anticipated point. The physical advantage handles cuts here.
        public float CommitDistance = 3.5f;
        // Lead vector magnitude is capped to (dist × this). Critical: prevents the
        // aim point from flying past the bot when the target is closing head-on,
        // which would otherwise make the bot pivot 180° to face a phantom point.
        public float MaxLeadAsDistRatio = 0.5f;

        [Header("Facing Anticipation")]
        // 0 = pure velocity (lags player turns), 1 = pure facing (reacts to body rotation
        // instantly, even before velocity catches up). Mid values blend the two.
        [Range(0f, 1f)] public float FacingBias = 0.6f;

        [Header("Cut Reaction")]
        // dot(recentVel, oldVel) below this = the target just changed direction hard.
        public float CutDotThreshold = 0.3f;
        // Ignore cuts when target is barely moving.
        public float CutDetectMinSpeed = 2f;
        // Minimum gap between cut-triggered slide bursts.
        public float CutReactionCooldown = 0.5f;

        [Header("Engagement")]
        public float EngageDistance = 6f;
        public float DisengageDistance = 9f;   // hysteresis so we don't flicker
        [Range(0f, 1f)] public float MirrorChance = 0.3f;
        [Range(0f, 1f)] public float PredictChance = 0.2f;   // pursuit = remainder
        // Lateral offset (m) for predict — bot commits to one side of target.
        public float PredictLateralOffset = 1.8f;

        [Header("Sprint")]
        // Need this much forward thrust to consider sprint useful.
        public float SprintMinForward = 0.5f;
        // Stop sprinting when stamina drops below this.
        public float SprintMinStamina = 0.15f;
        // Re-enable sprint only after stamina recovers past this.
        public float SprintResumeStamina = 0.35f;
        // Don't start sprinting unless target is at least this far away.
        public float SprintEngageDistance = 8f;
        // Stop sprinting once we close inside this — hysteresis with engage.
        public float SprintReleaseDistance = 6f;
        // After a slide ends, allow sprint regardless of distance for this long —
        // recovers the speed the slide bled off, helps catch back up after a cut.
        public float PostSlideSprintWindow = 0.6f;

        [Header("Approach Throttle")]
        // Inside this radius, start damping forward thrust to control closing speed.
        public float ThrottleStartDistance = 8f;
        // Forward input never drops below this (so the bot still glides toward target).
        public float ThrottleMinForward = 0.25f;

        [Header("Stick Aim")]
        // Aim past the puck along the bot→puck axis. Without this, the raycast hits the
        // top of the puck and the blade target lands at puck-top height (hovering).
        // Shifting the aim point past the puck makes the ray clear over the puck and
        // land on the ice past it, putting the blade at ice level next to the puck's
        // side instead of on top of it.
        public float StickAimForwardExtension = 0.3f;
        // Hard cap on the aim distance from the bot. Beyond this the raycast can't
        // reach the ice within max stick reach (~2.5m) and the blade target ends up
        // hovering in midair. Clamping the aim within reach keeps the blade on the ice
        // at all times — when the puck is far, the aim point is "stick-reach forward
        // toward the puck on the ice," not at the puck itself.
        public float StickMaxAimDistance = 1.8f;

        private enum EngagementMode { Pursuit, Mirror, Predict }

        // Slide state
        private bool isSliding;
        private float slideStartTime = -1000f;
        private float lastSlideEndTime = -1000f;

        // Velocity tracking
        private Vector3 recentTargetVel;
        private Vector3 oldTargetVel;
        // Velocity used for lead/aim — blended with target's facing direction so the bot
        // reacts to turning before momentum follows.
        private Vector3 predictedTargetVel;
        private float lastCutDetectedTime = -1000f;

        // Engagement state
        private bool isEngaged;
        private EngagementMode mode = EngagementMode.Pursuit;
        private float predictSide;
        // Set on entering Predict mode; consumed by UpdateSlide for the commit burst.
        private bool predictCommitPending;

        // Sprint state
        private bool isSprinting;

        void Update()
        {
            if (ControlledPlayer == null) return;
            var input = ControlledPlayer.PlayerInput;
            var body = ControlledPlayer.PlayerBody;
            if (input == null || body == null) return;

            var target = PlayerManager.Instance.GetPlayerByClientId(TargetClientId);
            if (target == null || target.PlayerBody == null)
            {
                input.MoveInput.ServerValue = Vector2.zero;
                return;
            }

            var origin = body.transform.position;
            var targetPos = target.PlayerBody.transform.position;

            var rawTargetVel = target.PlayerBody.Rigidbody.velocity;
            rawTargetVel.y = 0f;

            // Dual EMAs: recent (fast) vs old (slow). Divergence = direction change.
            recentTargetVel = Vector3.Lerp(recentTargetVel, rawTargetVel, Time.deltaTime * 15f);
            oldTargetVel = Vector3.Lerp(oldTargetVel, rawTargetVel, Time.deltaTime * 3f);

            // Predicted velocity = current velocity rotated toward where the target's
            // body is facing. Catches the turn-before-momentum-shift case.
            predictedTargetVel = ComputePredictedTargetVel(target.PlayerBody);

            bool cutDetected = DetectCut();

            // Engagement state machine with hysteresis
            float dist = Vector3.Distance(origin, targetPos);
            if (!isEngaged && dist < EngageDistance)
            {
                isEngaged = true;
                mode = ChooseEngagementMode();
                if (mode == EngagementMode.Predict) predictCommitPending = true;
            }
            else if (isEngaged && dist > DisengageDistance)
            {
                isEngaged = false;
                mode = EngagementMode.Pursuit;
            }

            Vector3 aimPoint = ComputeAimPoint(origin, targetPos, dist);

            var toAim = aimPoint - origin;
            toAim.y = 0f;
            if (toAim.sqrMagnitude < 0.01f)
            {
                input.MoveInput.ServerValue = Vector2.zero;
                return;
            }

            var forward = body.transform.forward;
            forward.y = 0f;

            float signedAngle = Vector3.SignedAngle(forward, toAim, Vector3.up);
            float absAngle = Mathf.Abs(signedAngle);

            float turn = (absAngle > TurnDeadzoneDeg)
                ? Mathf.Clamp(signedAngle / TurnFullInputAngle, -1f, 1f)
                : 0f;

            float fwd;
            if (absAngle <= ForwardFullAngle) fwd = 1f;
            else if (absAngle >= ForwardCutoffAngle) fwd = 0f;
            else fwd = 1f - Mathf.InverseLerp(ForwardFullAngle, ForwardCutoffAngle, absAngle);

            // Approach throttle — decelerate as we close so we don't overshoot.
            if (dist < ThrottleStartDistance)
            {
                float t = Mathf.InverseLerp(0f, ThrottleStartDistance, dist);
                fwd *= Mathf.Lerp(ThrottleMinForward, 1f, t);
            }

            input.MoveInput.ServerValue = new Vector2(turn, fwd);

            UpdateSlide(input, absAngle, cutDetected);
            UpdateSprint(input, body, fwd, dist);
            AimStickAtPuck(input, targetPos);
        }

        // Aim the stick blade at the live puck by inverting the game's raycast.
        // The game does: raycastOrigin.localRotation = Quaternion.Euler(angle.x, angle.y, 0),
        // then shoots a ray along raycastOrigin.forward, and places the blade at the hit.
        // For a desired direction d (in StickPositioner-local space), the forward vector
        // from Euler(pitch, yaw, 0) is (sin(yaw)cos(pitch), -sin(pitch), cos(yaw)cos(pitch)).
        // Inverse: pitch = asin(-d.y), yaw = atan2(d.x, d.z). The game clamps to the
        // player's angle range and PIDs toward it — that smoothing is what makes the
        // bot look human instead of telekinetic.
        private void AimStickAtPuck(PlayerInput input, Vector3 targetPos)
        {
            var positioner = ControlledPlayer.StickPositioner;
            if (positioner == null) return;

            var puck = PickFocusPuck(targetPos);
            if (puck == null) return;

            Vector3 originPos = positioner.RaycastOriginPosition;
            Vector3 puckPos = puck.transform.position;
            Vector3 botPos = ControlledPlayer.PlayerBody.transform.position;

            // Aim along the bot→puck horizontal axis, on the ice. Cap the forward
            // distance at StickMaxAimDistance so the raycast always lands on the ice
            // within max stick reach — otherwise the ray runs out in midair and the
            // blade hovers. When in close range, extend past the puck so the blade
            // contacts its side, not its top.
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
            // Puck rests on the ice, so its Y is a stable proxy for ice surface height.
            aimPoint.y = puckPos.y;

            Vector3 toAim = aimPoint - originPos;
            if (toAim.sqrMagnitude < 0.0001f) return;

            Vector3 dirLocal = positioner.transform.InverseTransformDirection(toAim.normalized);
            float clampedY = Mathf.Clamp(dirLocal.y, -1f, 1f);
            float pitch = Mathf.Asin(-clampedY) * Mathf.Rad2Deg;
            float yaw = Mathf.Atan2(dirLocal.x, dirLocal.z) * Mathf.Rad2Deg;

            input.StickRaycastOriginAngleInput.ServerValue = new Vector2(pitch, yaw);
        }

        private void UpdateSprint(PlayerInput input, PlayerBody body, float fwd, float dist)
        {
            float stamina = body.Stamina.Value;
            if (isSprinting)
            {
                bool drained = stamina < SprintMinStamina;
                bool notPushing = fwd < SprintMinForward;
                bool tooClose = dist < SprintReleaseDistance;
                if (drained || notPushing || isSliding || tooClose)
                {
                    isSprinting = false;
                    input.SprintInput.ServerValue = false;
                }
            }
            else
            {
                bool replenished = stamina > SprintResumeStamina;
                bool pushingHard = fwd >= SprintMinForward;
                bool farEnough = dist > SprintEngageDistance;
                // Allow a sprint burst right after a slide ends to recover speed,
                // even at close range — pairs with the slide-on-cut reaction.
                bool postSlideBurst = (Time.time - lastSlideEndTime) < PostSlideSprintWindow;
                if (replenished && pushingHard && !isSliding && (farEnough || postSlideBurst))
                {
                    isSprinting = true;
                    input.SprintInput.ServerValue = true;
                }
            }
        }

        private Vector3 ComputePredictedTargetVel(PlayerBody targetBody)
        {
            Vector3 facingDir = targetBody.transform.forward;
            facingDir.y = 0f;
            if (facingDir.sqrMagnitude < 0.0001f) return recentTargetVel;
            facingDir.Normalize();

            float speed = recentTargetVel.magnitude;
            if (speed < 0.1f) return Vector3.zero;

            Vector3 velDir = recentTargetVel / speed;
            Vector3 blendedDir = Vector3.Slerp(velDir, facingDir, FacingBias);
            return blendedDir * speed;
        }

        private bool DetectCut()
        {
            float now = Time.time;
            if ((now - lastCutDetectedTime) < CutReactionCooldown) return false;
            if (recentTargetVel.magnitude < CutDetectMinSpeed) return false;
            if (oldTargetVel.magnitude < CutDetectMinSpeed) return false;

            float align = Vector3.Dot(recentTargetVel.normalized, oldTargetVel.normalized);
            if (align < CutDotThreshold)
            {
                lastCutDetectedTime = now;
                return true;
            }
            return false;
        }

        private EngagementMode ChooseEngagementMode()
        {
            float roll = Random.value;
            if (roll < MirrorChance) return EngagementMode.Mirror;
            if (roll < MirrorChance + PredictChance)
            {
                predictSide = (Random.value < 0.5f) ? -1f : 1f;
                return EngagementMode.Predict;
            }
            return EngagementMode.Pursuit;
        }

        private Vector3 ComputeAimPoint(Vector3 origin, Vector3 targetPos, float dist)
        {
            float leadTime = Mathf.Clamp(dist / LeadDistanceScale, 0f, MaxLeadTime);

            // Commit-to-spot fade: near the target, drop lead toward zero so the bot
            // doesn't telegraph and isn't fooled by a late cut — it just bodies up.
            if (dist < CommitDistance)
            {
                leadTime *= Mathf.InverseLerp(0f, CommitDistance, dist);
            }

            Vector3 leadVec = ClampLead(predictedTargetVel * leadTime, dist);

            if (!isEngaged || mode != EngagementMode.Predict)
            {
                // Pursuit / Mirror — same formula. Mirror as a separate mode is reserved
                // for future per-mode tuning; behavior currently identical to Pursuit.
                return targetPos + leadVec;
            }

            // Predict: commit to a lateral offset on one side and beat them to it.
            var toTarget = targetPos - origin;
            toTarget.y = 0f;
            if (toTarget.sqrMagnitude < 0.01f) return targetPos + leadVec;
            var lateral = Vector3.Cross(Vector3.up, toTarget.normalized);
            // Scale lateral commit so it can't exceed the room we have.
            float effectiveLateral = Mathf.Min(PredictLateralOffset, dist * 0.7f);
            return targetPos
                + leadVec
                + lateral * (predictSide * effectiveLateral);
        }

        private Vector3 ClampLead(Vector3 leadVec, float dist)
        {
            float maxMag = dist * MaxLeadAsDistRatio;
            if (leadVec.sqrMagnitude > maxMag * maxMag)
            {
                return leadVec.normalized * maxMag;
            }
            return leadVec;
        }

        private void UpdateSlide(PlayerInput input, float absAngle, bool cutDetected)
        {
            float now = Time.time;

            if (isSliding)
            {
                bool durationExpired = (now - slideStartTime) > SlideMaxDuration;
                bool aligned = absAngle < SlideReleaseAngle;
                if (durationExpired || aligned)
                {
                    isSliding = false;
                    lastSlideEndTime = now;
                    input.SlideInput.ServerValue = false;
                }
                return;
            }

            bool offCooldown = (now - lastSlideEndTime) > SlideCooldown;
            // Two valid reasons to slide: reacting to a detected cut (force, bypass
            // cooldown), or committing to a Predict gamble (respect cooldown).
            bool shouldStart = cutDetected || (predictCommitPending && offCooldown);

            if (shouldStart)
            {
                isSliding = true;
                slideStartTime = now;
                input.SlideInput.ServerValue = true;
                predictCommitPending = false;
            }
        }

        // Pick the puck the target player is most likely handling — i.e. closest to them.
        // Approximates "puck the player is engaged with" without needing a touch event hook.
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
