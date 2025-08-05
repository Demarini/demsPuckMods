using DG.Tweening;
using HarmonyLib;
using PuckAIPractice.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        }
        public static bool SimulateDash(PlayerBodyV2 __instance, Vector3 dashDir)
        {
            var state = __instance.GetComponent<SimulateDashState>() ??
                        __instance.gameObject.AddComponent<SimulateDashState>();
            var traverse = Traverse.Create(__instance);

            if (!traverse.Field("canDash").GetValue<bool>() || !__instance.IsSliding.Value)
                return false;

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
            state.MoveTween = rb.DOMove(dashTarget, dashDragTime).SetEase(Ease.OutQuad);
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
            if (!NetworkManager.Singleton.IsServer || !FakePlayerRegistry.IsFake(__instance.Player))
                return true;

            return !SimulateDashHelper.SimulateDash(__instance, __instance.transform.right);
        }
    }
    [HarmonyPatch(typeof(PlayerBodyV2), "DashLeft")]
    public static class DashLeftSimPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(PlayerBodyV2 __instance)
        {
            if (!NetworkManager.Singleton.IsServer || !FakePlayerRegistry.IsFake(__instance.Player))
                return true;

            return !SimulateDashHelper.SimulateDash(__instance, -__instance.transform.right);
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
                return true; // run original for real players

            Debug.Log("[CancelDash] Cancelling dash and killing tweens");
            var state = __instance.GetComponent<SimulateDashState>();
            // Kill and null move tween
            state.MoveTween?.Kill();
            Debug.Log($"[CancelDash] dashMoveTween isActive: {state.MoveTween?.active}, isPlaying: {state.MoveTween?.IsPlaying()}");
            state.MoveTween = null;

            // Kill and null drag tween
            state.DragTween?.Kill();
            Debug.Log($"[CancelDash] dashDragTween isActive: {state.DragTween?.active}, isPlaying: {state.DragTween?.IsPlaying()}");
            state.DragTween = null;

            // Kill and null leg pad tween
            state.LegTween?.Kill();
            Debug.Log($"[CancelDash] dashLegPadTween isActive: {state.LegTween?.active}, isPlaying: {state.LegTween?.IsPlaying()}");
            state.LegTween = null;

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
                return $"<b><color=#00AAFF>DONOR</color></b>";
            }
            else
            {
                return $"<b><color=#00AAFF>BOT ROLE</color></b>";
            }
        }
    }
}