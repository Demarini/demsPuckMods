using UnityEngine;

namespace PuckAIPractice.Defender
{
    public class DefenderAI : MonoBehaviour
    {
        public Player ControlledPlayer;
        public ulong TargetClientId;

        [Header("Turn")]
        public float TurnDeadzoneDeg = 5f;
        public float TurnFullInputAngle = 30f;

        [Header("Forward")]
        // Skate full speed when we're within this many degrees of facing target.
        public float ForwardFullAngle = 15f;
        // Above this, no forward input — just turn in place.
        public float ForwardCutoffAngle = 90f;

        [Header("Slide")]
        // Engage slide for sharper turning when off-axis by at least this much.
        public float SlideEngageAngle = 55f;
        // Release slide once we're back within this — preserves speed.
        public float SlideReleaseAngle = 20f;
        // Hard cap on slide burst length, even if still off-axis.
        public float SlideMaxDuration = 0.35f;
        // Time after a slide ends before another can start.
        public float SlideCooldown = 1.2f;

        private bool isSliding;
        private float slideStartTime = -1000f;
        private float lastSlideEndTime = -1000f;

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
            var dest = target.PlayerBody.transform.position;
            var toTarget = dest - origin;
            toTarget.y = 0f;
            if (toTarget.sqrMagnitude < 0.01f)
            {
                input.MoveInput.ServerValue = Vector2.zero;
                return;
            }

            var forward = body.transform.forward;
            forward.y = 0f;

            float signedAngle = Vector3.SignedAngle(forward, toTarget, Vector3.up);
            float absAngle = Mathf.Abs(signedAngle);

            float turn = (absAngle > TurnDeadzoneDeg)
                ? Mathf.Clamp(signedAngle / TurnFullInputAngle, -1f, 1f)
                : 0f;

            float fwd;
            if (absAngle <= ForwardFullAngle) fwd = 1f;
            else if (absAngle >= ForwardCutoffAngle) fwd = 0f;
            else fwd = 1f - Mathf.InverseLerp(ForwardFullAngle, ForwardCutoffAngle, absAngle);

            input.MoveInput.ServerValue = new Vector2(turn, fwd);

            UpdateSlide(input, absAngle);
        }

        private void UpdateSlide(PlayerInput input, float absAngle)
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
            }
            else
            {
                bool needSharperTurn = absAngle > SlideEngageAngle;
                bool offCooldown = (now - lastSlideEndTime) > SlideCooldown;
                if (needSharperTurn && offCooldown)
                {
                    isSliding = true;
                    slideStartTime = now;
                    input.SlideInput.ServerValue = true;
                }
            }
        }
    }
}
