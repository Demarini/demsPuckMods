using UnityEngine;

namespace PuckAIPractice.Utilities
{
    // Keeps a bot from entering the "fallen on the ice" state, while still
    // letting it lean and crouch normally for slide-turn physics.
    //
    // Why surgical: an earlier version forced the body upright every frame
    // and zeroed X/Z angular velocity. That worked for "never topple", but
    // also killed the body lean that ChaserAI relies on for tight slide
    // turns — bots stopped being able to crouch-cut and became easy to
    // sidestep. The fix: clamp the binary fall/slip flags and KeepUpright
    // balance every frame so the game-state never says "this bot is down",
    // but leave the rigidbody alone so physics-driven leans still happen.
    //
    // Emergency upright-lock: if the body somehow ends up more than ~70°
    // off vertical (genuinely flopped sideways), only then do we slam the
    // rotation back. That threshold is well past any normal lean.
    //
    // Reusable for the future goalie rewrite; goalies barely lean so the
    // emergency threshold rarely fires for them either.
    public class BotTopplePreventer : MonoBehaviour
    {
        // Cosine of the maximum tilt we accept before forcing upright.
        // dot(body.up, world.up) below this triggers the emergency lock.
        // 0.34 ≈ cos(70°) — body is more than 70° off vertical.
        private const float TiltLockDot = 0.34f;

        // Minimum KeepUpright.Balance — high enough that hover physics
        // doesn't collapse (causing the pogo-stick glitch) but low enough
        // that slide-cuts still get lean.
        private const float BalanceFloor = 1f;

        private PlayerBody _body;

        public void Bind(PlayerBody body)
        {
            _body = body;
        }

        void LateUpdate()
        {
            if (_body == null) return;

            // Clear the binary state every frame — even if the body briefly
            // gets knocked sideways, the game never registers it as fallen,
            // so the long ragdoll-pose recovery animation doesn't kick in.
            _body.HasFallen = false;
            _body.HasSlipped = false;

            // Clamp KeepUpright.Balance to a floor instead of slamming to 1.
            // Slide-cuts work by letting balance drop briefly (slip → lean
            // → sharp turn), so we let it dip — but if it gets too low the
            // hover physics collapse and the body pogo-glitches when
            // partially fallen. 0.5 is the sweet spot: enough lean room
            // for sharper slide turns, enough upright force to never let
            // the bot end up flopped on the ice.
            if (_body.KeepUpright != null && _body.KeepUpright.Balance < BalanceFloor)
            {
                _body.KeepUpright.Balance = BalanceFloor;
            }

            // Emergency lock only — let normal lean happen.
            if (Vector3.Dot(_body.transform.up, Vector3.up) < TiltLockDot)
            {
                var fwd = _body.transform.forward;
                fwd.y = 0f;
                if (fwd.sqrMagnitude < 0.0001f) fwd = Vector3.forward;
                var upright = Quaternion.LookRotation(fwd.normalized, Vector3.up);
                _body.transform.rotation = upright;
                if (_body.Rigidbody != null)
                {
                    _body.Rigidbody.MoveRotation(upright);
                }
            }
        }
    }
}
