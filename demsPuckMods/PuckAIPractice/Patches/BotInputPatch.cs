using DG.Tweening;
using HarmonyLib;
using PuckAIPractice.AI;
using PuckAIPractice.GameModes;
using PuckAIPractice.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;
using static PuckAIPractice.Patches.SimulateDashHelper;

namespace PuckAIPractice.Patches
{
    public static class SimulateDashHelper
    {
        public class SimulateDashState : MonoBehaviour
        {
            public Tween MoveTween;
            public Tween DragTween;
            public Tween LegTween;

            public bool IsDashing;
        }
        public static bool SimulateDash(PlayerBodyV2 __instance, Vector3 dashDir)
        {
            var state = __instance.GetComponent<SimulateDashState>() ??
                        __instance.gameObject.AddComponent<SimulateDashState>();
            var traverse = Traverse.Create(__instance);

            
            //debug.log($"[SimulateDash] Player {__instance.Player.Username.Value} using SimulateDashState instance: {state.GetInstanceID()}");

            if (!traverse.Field("canDash").GetValue<bool>() || !__instance.IsSliding.Value)
            {
                //debug.log("CAN NOT DASH");
                return false;
            }

            state.IsDashing = true;
            float stamina = __instance.Stamina;
            float dashStaminaDrain = traverse.Field("dashStaminaDrain").GetValue<float>();

            Rigidbody rb = __instance.Rigidbody;
            float dashVelocity = traverse.Field("dashVelocity").GetValue<float>();
            float dashDragTime = traverse.Field("dashDragTime").GetValue<float>();
            Vector3 dashTarget = rb.position + dashDir * dashVelocity;

            // Kill old tweens before creating new ones
            state.MoveTween?.Kill();
            state.DragTween?.Kill();
            state.LegTween?.Kill();

            // Create and store new tweens
            //debug.log("DO MOVE");
            //debug.log("Dash Target" + dashTarget);
            //debug.log("Player Position" + rb.position);
            //debug.log("Is Kinematic" + rb.isKinematic);
            //state.MoveTween = rb.DOMove(dashTarget, dashDragTime).SetEase(Ease.OutQuad);
            __instance.StartCoroutine(MoveFakePlayer(__instance, dashTarget, dashDragTime));
            traverse.Field("dashMoveTween").SetValue(state.MoveTween);

            state.DragTween = DOTween.To(() => __instance.Movement.AmbientDrag,
                                         v => __instance.Movement.AmbientDrag = v,
                                         0f, dashDragTime)
                                    .OnComplete(() => __instance.HasDashed = false)
                                    .SetEase(Ease.Linear);
            traverse.Field("dashDragTween").SetValue(state.DragTween);
            
            state.LegTween = DOVirtual.DelayedCall(dashDragTime / 4f, () =>
            {
                __instance.HasDashExtended = false;
            }, true);
            traverse.Field("dashLegPadTween").SetValue(state.LegTween);

            //__instance.Stamina = Mathf.Max(0f, stamina - dashStaminaDrain);
            __instance.HasDashed = true;
            __instance.Movement.AmbientDrag = traverse.Field("dashDrag").GetValue<float>();
            __instance.HasDashExtended = true;
            __instance.IsExtendedLeft.Value = dashDir.x < 0;
            __instance.IsExtendedRight.Value = dashDir.x > 0;

            return true;
        }
        private static readonly MethodInfo updateAudioMethod = typeof(PlayerBodyV2)
    .GetMethod("Server_UpdateAudio", BindingFlags.Instance | BindingFlags.NonPublic);
        static Vector3 redNetCenter = new Vector3(0.0f, 0.8f, -40.23f);
        static Vector3 blueNetCenter = new Vector3(0.0f, 0.8f, 40.23f);
        static float overshootDistanceThreshold = 2f;
        public static bool IsBehindNetRed;
        public static float SignedLateralOffsetRed;
        public static bool IsBehindNetBlue;
        public static float SignedLateralOffsetBlue;
        public static Vector3 ProjectedPointRed;
        public static Vector3 ProjectedPointBlue;
        private static IEnumerator MoveFakePlayer(PlayerBodyV2 body, Vector3 target, float duration)
        {
            Debug.Log("Starting Move Faker Player");
            const float slideTurnMultiplier = 2f;
            const float jumpTurnMultiplier = 5f;
            const float fallenDrag = 0.2f;
            const float stopDrag = 2.5f;
            const float slideDrag = 0.2f;
            const float hoverDistance = 1.2f;
            const float slideHoverDistance = 0.8f;
            float elapsed = 0f;
            Vector3 start = body.transform.position;
            List<Vector3> positionHistory = new List<Vector3>();
            const int maxHistoryFrames = 5; // Tune as needed
            var state = body.GetComponent<SimulateDashState>() ??
                        body.gameObject.AddComponent<SimulateDashState>();
            state.IsDashing = true;

            // Start dash state
            body.IsSliding.Value = true;
            body.HasSlipped = true;
            body.HasFallen = false;
            //body.Rigidbody.velocity = (target - start).normalized * 5f;
            body.Speed = body.Movement.Speed; // Set manually if Movement.Speed is spoofed

            body.Movement.Sprint = body.IsSprinting.Value;
            body.Movement.TurnMultiplier = body.IsSliding.Value ? slideTurnMultiplier :
                                            body.IsJumping ? jumpTurnMultiplier : 1f;

            body.Movement.AmbientDrag = body.HasFallen ? fallenDrag :
                                         body.HasDashed ? body.Movement.AmbientDrag :
                                         body.IsStopping.Value ? stopDrag :
                                         body.IsSliding.Value ? slideDrag : 0f;

            body.Hover.TargetDistance = body.IsSliding.Value ? slideHoverDistance :
                                        body.KeepUpright.Balance * hoverDistance;

            body.Skate.Intensity = (body.IsSliding.Value || body.IsStopping.Value || !body.IsGrounded) ?
                                    0f : body.KeepUpright.Balance;

            body.VelocityLean.AngularIntensity = Mathf.Max(0.1f, body.Movement.NormalizedMaximumSpeed) /
                                                 (body.IsSliding.Value ? 2f : (body.IsJumping ? 2f : 1f));

            body.VelocityLean.Inverted = !body.IsJumping && !body.IsSliding.Value && body.Movement.IsMovingBackwards;
            body.VelocityLean.UseWorldLinearVelocity = body.IsJumping || body.IsSliding.Value;
            // 🔊 Start audio
            updateAudioMethod?.Invoke(body, null);
            GoalieAI goalieAI = body.GetComponent<GoalieAI>();
            Vector3 lastPosition = start;
            Debug.Log("Target Position: " + (body.Player.Team.Value == PlayerTeam.Red ? ProjectedPointRed : ProjectedPointBlue));
            Debug.Log("Last Position: " + lastPosition);
            while (elapsed < duration && state.IsDashing)
            {
                Vector3 netPos = body.Player.Team.Value == PlayerTeam.Red ? redNetCenter : blueNetCenter;
                Vector3 pos = body.transform.position;
                positionHistory.Add(pos);
                if (positionHistory.Count > maxHistoryFrames)
                    positionHistory.RemoveAt(0);

                // Only calculate if we have enough samples
                Vector3 historyMoveDir = Vector3.zero;
                if (positionHistory.Count >= 2)
                {
                    historyMoveDir = (pos - positionHistory[0]).normalized;
                }
                float prevDist = Vector3.Distance(start, (body.Player.Team.Value == PlayerTeam.Red ? ProjectedPointRed : ProjectedPointBlue));
                pos.y = 0f;
                Debug.Log("Current Position: " + pos);
                if(body.Player.Team.Value == PlayerTeam.Red)
                {
                    ProjectedPointRed.y = 0f;
                }
                else
                {
                    ProjectedPointBlue.y = 0f;
                }
                

                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration); // ⬅️ this must come *before* using `t`

                Vector3 toTarget = ((body.Player.Team.Value == PlayerTeam.Red ? ProjectedPointRed : ProjectedPointBlue) - pos).normalized;
                float alignment = Vector3.Dot(historyMoveDir, toTarget);
                //debug.log($"[MoveFakePlayer] isBehindNet = {(body.Player.Team.Value == PlayerTeam.Red ? IsBehindNetRed: IsBehindNetBlue)}, signedOffset = {(body.Player.Team.Value == PlayerTeam.Red ? SignedLateralOffsetRed : SignedLateralOffsetBlue)}, moveDir = {moveDir}");
                if ((body.Player.Team.Value == PlayerTeam.Red ? IsBehindNetRed : IsBehindNetBlue))
                {
                    Vector3 goalRight = body.Player.Team.Value == PlayerTeam.Red ? Vector3.left : Vector3.right;
                    float signedOffset = body.Player.Team.Value == PlayerTeam.Red ? SignedLateralOffsetRed : SignedLateralOffsetBlue;

                    // Only run if we're actually moving
                    if (historyMoveDir.sqrMagnitude > 0.001f)
                    {
                        float directionalAlignment = Vector3.Dot(historyMoveDir, goalRight * Mathf.Sign(signedOffset));
                        Debug.Log($"[MoveFakePlayer] Directional alignment behind net = {directionalAlignment:F3}");

                        // Now break ONLY if they’re moving the wrong way
                        if (directionalAlignment < -0.5f)
                        {
                            Debug.Log("[MoveFakePlayer] Behind net — moving wrong direction!");
                            break;
                        }
                    }
                }
                if (t > 0.05f && alignment < -0.5f)
                {
                    Debug.Log($"🚨 Moving away from target! Dot: {alignment:F3}, t: {t:F2}");
                    break;
                }
                body.transform.position = Vector3.Lerp(start, target, EaseOutQuad(t));
                updateAudioMethod?.Invoke(body, null);

                yield return null;
            }

            // Snap to target if dash finished normally
            //if (state.IsDashing)
            //body.transform.position = target;
            //debug.log("Ending Dash!");
            // 🛑 Clear dash state
            state.IsDashing = false;
            body.IsSliding.Value = false;
            body.HasSlipped = false;
            //body.Rigidbody.velocity = Vector3.zero;
            // 🔇 One final audio update to cut the sliding
            updateAudioMethod?.Invoke(body, null);
        }
        private static float EaseOutQuad(float t)
        {
            return 1 - (1 - t) * (1 - t);
        }
    }
        
    [HarmonyPatch(typeof(Movement), "FixedUpdate")]
    public class MovementFixedUpdatePatch
    {
        static bool Prefix(Movement __instance)
        {
            var playerBody = __instance.GetComponent<PlayerBodyV2>();
            if (playerBody != null && FakePlayerRegistry.IsFake(__instance.PlayerBody.Player))
            {
                return false; // skip FixedUpdate entirely
            }
            return true; // continue original method
        }
    }
    [HarmonyPatch(typeof(PlayerBodyV2), "DashRight")]
    public static class DashRightSimPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(PlayerBodyV2 __instance)
        {
            //debug.log(__instance.Player.OwnerClientId);
            if (!NetworkManager.Singleton.IsServer || !FakePlayerRegistry.IsFake(__instance.Player))
            {
                //debug.log("DASH RIGHT NOT FAKE PLAYER");
                return true;
            }
            else
            {
                //debug.log("DASH RIGHT FAKE PLAYER");
            }

            //debug.log("Simulate Dash Right" + __instance.Player.Username + __instance.Player.OwnerClientId);
            return !SimulateDash(__instance, __instance.transform.right);
        }
    }
    [HarmonyPatch(typeof(PlayerBodyV2), "DashLeft")]
    public static class DashLeftSimPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(PlayerBodyV2 __instance)
        {
            if (!NetworkManager.Singleton.IsServer || !FakePlayerRegistry.IsFake(__instance.Player))
            {
                //debug.log("DASH LEFT NOT FAKE PLAYER");
                return true;
            }
            else
            {
                //debug.log("DASH LEFT FAKE PLAYER");
            }
                

            //debug.log("Simulate Dash Left" + __instance.Player.Username + __instance.Player.OwnerClientId);
            return !SimulateDash(__instance, -__instance.transform.right);
        }
    }
    [HarmonyPatch(typeof(PlayerBodyV2), "CancelDash")]
    public static class CancelDashSimPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(PlayerBodyV2 __instance)
        {
            if (!NetworkManager.Singleton.IsServer)
                return false;

            // Only override for fake players
            if (!FakePlayerRegistry.IsFake(__instance.Player))
            {
                //debug.log(__instance.Player.OwnerClientId.ToString());
                //debug.log(__instance.Player.Username.Value.ToString());
                //debug.log("NOT FAKE PLAYER");
                return true;
            }
            else
            {
                //debug.log("FAKE PLAYER");
            }
                // run original for real players

            //debug.log("[CancelDash] Cancelling dash and killing tweens");
            var state = __instance.GetComponent<SimulateDashState>();
            if (state == null)
            {
                //debug.logWarning($"[CancelDash] SimulateDashState was missing on {__instance.name}");
                return false; // or true, depending on whether you want to run original
            }
            // Kill and null move tween
            state.MoveTween?.Kill();
            //debug.log($"[CancelDash] dashMoveTween isActive: {state.MoveTween?.active}, isPlaying: {state.MoveTween?.IsPlaying()}");
            state.MoveTween = null;

            // Kill and null drag tween
            state.DragTween?.Kill();
            //debug.log($"[CancelDash] dashDragTween isActive: {state.DragTween?.active}, isPlaying: {state.DragTween?.IsPlaying()}");
            state.DragTween = null;

            // Kill and null leg pad tween
            state.LegTween?.Kill();
            //debug.log($"[CancelDash] dashLegPadTween isActive: {state.LegTween?.active}, isPlaying: {state.LegTween?.IsPlaying()}");
            state.LegTween = null;

            state.IsDashing = false;
            // Reset state
            __instance.HasDashed = false;
            __instance.HasDashExtended = false;
            __instance.IsExtendedLeft.Value = false;
            __instance.IsExtendedRight.Value = false;
            __instance.Movement.AmbientDrag = 0f;

            return false; // skip original
        }
    }
    [HarmonyPatch(typeof(UIScoreboard), "UpdatePlayer")]
    public static class UIScoreboard_UpdatePlayer_Patch
    {
        public static void Postfix(UIScoreboard __instance, Player player)
        {
            if (__instance == null || player == null)
                return;

            // Grab the visual element associated with this player
            VisualElement visualElement;
            if (!Traverse.Create(__instance)
                         .Field("playerVisualElementMap")
                         .GetValue<Dictionary<Player, VisualElement>>()
                         .TryGetValue(player, out visualElement))
                return;

            // Rebuild label text with our own admin prefix logic
            Label usernameLabel = visualElement.Query<Label>("UsernameLabel");
            if (usernameLabel == null)
                return;

            string prefix = GetCustomPrefix(player);
            usernameLabel.text = string.Format("{0}<noparse>#{1} {2}</noparse>", prefix, player.Number.Value, player.Username.Value);
        }
        static List<string> donorList = new List<string>() { "76561197994406332" };
        private static string GetCustomPrefix(Player player)
        {
            
            if (donorList.Contains(player.SteamId.Value.ToString()))
            {
                return $"<b><color=#00AAFF>DA_ROBO_MAN</color></b>";
            }
            else
            {
                return $"<b><color=#00AAFF>SAVE_BOT_3000</color></b>";
            }
        }
    }
    [HarmonyPatch(typeof(PlayerBodyV2), "FixedUpdate")]
    public static class PlayerBodyV2_FixedUpdate_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(PlayerBodyV2 __instance)
        {
            if (FakePlayerRegistry.IsFake(__instance.Player))
            {
                RunCustomFixedUpdate(__instance);
                return false; // skip original
            }

            return true; // allow original to run
        }

        private static void RunCustomFixedUpdate(PlayerBodyV2 __instance)
        {
            if (!__instance.Player)
                return;

            var playerInput = __instance.Player.PlayerInput;
            if (!playerInput)
                return;
            var traverse = Traverse.Create(__instance);
            //try
            //{
            //    var playerMeshObj = traverse.Field("playerMesh").GetValue();
            //    Transform playerMesh = Traverse.Create(playerMeshObj).Property("transform").GetValue<Transform>();
            //    if (playerInput.LookInput.ServerValue || playerInput.TrackInput.ServerValue)
            //    {
            //        if (__instance.PlayerCamera)
            //        {
            //            Vector3 lookTarget = __instance.PlayerCamera.transform.position + __instance.PlayerCamera.transform.forward * 10f;
            //            playerMesh.LookAt(lookTarget);
            //        }
            //    }
            //    else if (__instance.Stick)
            //    {
            //        playerMesh.LookAt(__instance.Stick.BladeHandlePosition);
            //    }
            //}
            //catch
            //{
            //    //debug.log("Player Mesh Busted");
            //}


            //    float b = __instance.IsJumping ? 1.05f : (__instance.IsSliding.Value ? 0.95f : 1f);

            //    var stretchSpeed = Traverse.Create(__instance).Field("stretchSpeed").GetValue<float>();
            //    __instance.PlayerMesh.Stretch = Mathf.Lerp(__instance.PlayerMesh.Stretch, b, Time.fixedDeltaTime * stretchSpeed);

            //    if (!NetworkManager.Singleton.IsServer)
            //        return;

            if (!__instance.Player.IsReplay.Value)
            {
                try
                {
                    var handleInputs = traverse.Method("HandleInputs", new Type[] { typeof(PlayerInput) });
                    __instance.GetType()
        .GetMethod("HandleInputs", BindingFlags.Instance | BindingFlags.NonPublic)
        ?.Invoke(__instance, new object[] { playerInput });
                    ////debug.log("Finished Handle Inputs");
                }
                catch
                {
                    //debug.log("YO HANDLE INPUTS WASSUP");
                }

            }

            //    __instance.Speed = __instance.Movement.Speed;

            //    var sprintDrain = Traverse.Create(__instance).Field("sprintStaminaDrainRate").GetValue<float>();
            //    if (__instance.IsSprinting.Value)
            //    {
            //        __instance.Stamina -= Time.deltaTime / sprintDrain;
            //    }
            //    else if (__instance.Stamina < 1f)
            //    {
            //        var regenRate = Traverse.Create(__instance).Field("staminaRegenerationRate").GetValue<float>();
            //        __instance.Stamina = Mathf.Clamp(__instance.Stamina + Time.fixedDeltaTime / regenRate, 0f, 1f);
            //    }

            //    if (__instance.IsUpright)
            //    {
            //        float gravity = -Physics.gravity.y;
            //        float gravityMultiplier = Traverse.Create(__instance).Field("gravityMultiplier").GetValue<float>();
            //        //__instance.Rigidbody.AddForce(Vector3.up * gravity, ForceMode.Acceleration);
            //        //__instance.Rigidbody.AddForce(Vector3.down * gravity * gravityMultiplier, ForceMode.Acceleration);
            //    }

            //    __instance.MovementDirection.localRotation = Quaternion.FromToRotation(
            //        __instance.transform.forward,
            //        Utils.Vector3Slerp3(-__instance.transform.right, __instance.transform.forward, __instance.transform.right, __instance.Laterality));

            //    __instance.Movement.Sprint = __instance.IsSprinting.Value;
            //    __instance.Movement.TurnMultiplier = (__instance.IsSliding.Value ?
            //        Traverse.Create(__instance).Field("slideTurnMultiplier").GetValue<float>() :
            //        (__instance.IsJumping ? Traverse.Create(__instance).Field("jumpTurnMultiplier").GetValue<float>() : 1f));

            //    var fallenDrag = Traverse.Create(__instance).Field("fallenDrag").GetValue<float>();
            //    var stopDrag = Traverse.Create(__instance).Field("stopDrag").GetValue<float>();
            //    var slideDrag = Traverse.Create(__instance).Field("slideDrag").GetValue<float>();

            //    __instance.Movement.AmbientDrag = __instance.HasFallen ?
            //        fallenDrag : (__instance.HasDashed ?
            //        __instance.Movement.AmbientDrag : (__instance.IsStopping.Value ? stopDrag : (__instance.IsSliding.Value ? slideDrag : 0f)));

            //    float slideHover = Traverse.Create(__instance).Field("slideHoverDistance").GetValue<float>();
            //    float hoverDistance = Traverse.Create(__instance).Field("hoverDistance").GetValue<float>();

            //    __instance.Hover.TargetDistance = __instance.IsSliding.Value ? slideHover : (__instance.KeepUpright.Balance * hoverDistance);

            //    __instance.Skate.Intensity = (__instance.IsSliding.Value || __instance.IsStopping.Value || !__instance.IsGrounded) ? 0f : __instance.KeepUpright.Balance;

            //    __instance.VelocityLean.AngularIntensity = Mathf.Max(0.1f, __instance.Movement.NormalizedMaximumSpeed) /
            //        (__instance.IsSliding.Value ? 2f : (__instance.IsJumping ? 2f : 1f));

            //    __instance.VelocityLean.Inverted = !__instance.IsJumping && !__instance.IsSliding.Value && __instance.Movement.IsMovingBackwards;
            //    __instance.VelocityLean.UseWorldLinearVelocity = __instance.IsJumping || __instance.IsSliding.Value;

            //    if (!__instance.HasSlipped && !__instance.HasFallen && __instance.IsSlipping)
            //    {
            //        __instance.OnSlip();
            //    }
            //    else if (__instance.HasSlipped && !__instance.HasFallen && __instance.IsSideways)
            //    {
            //        __instance.OnFall();
            //    }
            //    else if (__instance.HasFallen && !__instance.HasSlipped && __instance.IsUpright)
            //    {
            //        __instance.OnStandUp();
            //    }
            //    __instance.IsSliding.Value = true;
            //    __instance.HasSlipped = true;
            //    __instance.HasFallen = false;
            // private method, invoke manually
            var audioMethod = typeof(PlayerBodyV2).GetMethod("Server_UpdateAudio", BindingFlags.Instance | BindingFlags.NonPublic);
            audioMethod?.Invoke(__instance, null);
        }
    }
}