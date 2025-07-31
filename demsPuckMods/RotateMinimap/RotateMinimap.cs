using HarmonyLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UIElements;

namespace RotateMinimap
{
    //[HarmonyPatch(typeof(UIMinimap), "AddPlayerBody")]
    //public static class MinimapAddPlayerPatch
    //{
    //    [HarmonyPostfix]
    //    public static void Postfix(UIMinimap __instance, PlayerBodyV2 playerBody)
    //    {
    //        var playerMap = Traverse.Create(__instance)
    //            .Field("playerBodyVisualElementMap")
    //            .GetValue<Dictionary<PlayerBodyV2, VisualElement>>();

    //        if (playerMap.TryGetValue(playerBody, out var icon))
    //        {
    //            // Apply the current scale immediately so new icons match existing ones
    //            icon.style.scale = new Scale(new Vector3(MinimapValues.CurrentScale * 1.25f, MinimapValues.CurrentScale * 1.25f, 1f));
    //        }
    //    }
    //}
    //[HarmonyPatch(typeof(UIMinimap), "SetScale")]
    //public static class MinimapScalePatch
    //{
    //    [HarmonyPostfix]
    //    public static void Postfix(UIMinimap __instance, float scale)
    //    {
    //        MinimapValues.CurrentScale = scale;

    //        var playerMap = Traverse.Create(__instance)
    //            .Field("playerBodyVisualElementMap")
    //            .GetValue<Dictionary<PlayerBodyV2, VisualElement>>();

    //        if (playerMap == null) return;

    //        foreach (var kvp in playerMap)
    //        {
    //            var icon = kvp.Value;
    //            if (icon != null)
    //            {
    //                icon.style.scale = new Scale(new Vector3(scale * 1.25f, scale * 1.25f, 1f));
    //                icon.style.unityBackgroundImageTintColor = new StyleColor(Color.yellow);
    //            }

    //        }
    //    }
    //}
    //[HarmonyPatch(typeof(UIMinimap), "AddPlayerBody")]
    //public static class MinimapPlayerIconPatch
    //{
    //    [HarmonyPostfix]
    //    public static void Postfix(UIMinimap __instance, PlayerBodyV2 playerBody)
    //    {
    //        if (!playerBody) return;

    //        var playerMap = Traverse.Create(__instance)
    //            .Field("playerBodyVisualElementMap")
    //            .GetValue<Dictionary<PlayerBodyV2, VisualElement>>();

    //        if (playerMap.TryGetValue(playerBody, out var icon))
    //        {
    //            // Make icon larger
    //            //icon.style.scale = new Scale(new Vector3(1.5f, 1.5f, 1f));

    //            // Optional: glow (yellow for demonstration)
    //            icon.style.unityBackgroundImageTintColor = new StyleColor(Color.yellow);
    //        }
    //    }
    //}
    [HarmonyPatch(typeof(UIMinimap), "RemovePlayerBody")]
    public static class MinimapPinnedIconCleanup
    {
        [HarmonyPrefix]
        public static bool Prefix(UIMinimap __instance, PlayerBodyV2 playerBody)
        {
            if (playerBody == null)
                return true;

            var playerMap = Traverse.Create(__instance)
                .Field("playerBodyVisualElementMap")
                .GetValue<Dictionary<PlayerBodyV2, VisualElement>>();

            if (!playerMap.TryGetValue(playerBody, out var icon) || icon == null)
                return true;

            var minimapRoot = Traverse.Create(__instance)
                .Field("minimapVisualElement").GetValue<VisualElement>();

            if (icon.parent == minimapRoot)
            {
                // Safely unpin and remove it
                minimapRoot.Remove(icon);
                playerMap.Remove(playerBody);
                //Debug.Log("[PinnedIconCleanup] Removed pinned local icon safely.");
                return false; // Skip default removal to avoid double-remove errors
            }

            return true; // Normal behavior for unpinned icons
        }
    }
    [HarmonyPatch(typeof(UIMinimap), "AddPlayerBody")]
    public static class PinLocalPlayerIconPatch
    {
        static VisualElement pinnedLocalIcon;
        [HarmonyPostfix]
        public static void Postfix(UIMinimap __instance, PlayerBodyV2 playerBody)
        {
            if (!playerBody || !playerBody.Player || !playerBody.Player.IsLocalPlayer || !ConfigData.Instance.CenterOnPlayer)
                return;

            // Get the player's icon from the minimap
            var playerMap = Traverse.Create(__instance)
                .Field("playerBodyVisualElementMap")
                .GetValue<Dictionary<PlayerBodyV2, VisualElement>>();

            if (!playerMap.TryGetValue(playerBody, out var icon) || icon == null)
                return;

            // Remove the old pinned icon (if one exists)
            if (pinnedLocalIcon != null && pinnedLocalIcon != icon)
            {
                pinnedLocalIcon.RemoveFromHierarchy();  // Detaches it entirely
                pinnedLocalIcon = null;
            }

            // Get the containers
            var minimapRoot = Traverse.Create(__instance)
                .Field("minimapVisualElement").GetValue<VisualElement>();

            var minimapLayer = Traverse.Create(__instance)
                .Field("minimapMarkingsVisualElement").GetValue<VisualElement>();

            // Detach from the moving layer (the rink + other icons)
            minimapLayer.Remove(icon);

            // Attach to the static root container
            minimapRoot.Add(icon);

            // Ensure it renders above other elements
            icon.BringToFront();

            // Lock it visually at the center of the minimap (0,0 relative)
            icon.style.translate = new Translate(0, 0);

            pinnedLocalIcon = icon;
            //Debug.Log(ConfigData.Instance.CenterOnPlayer.ToString());
            //Debug.Log("[PinLocalPlayerIconPatch] Local player icon pinned to minimap center.");
        }
    }
    [HarmonyPatch(typeof(UIMinimap), "Update")]
    public static class RotateMinimapPatch
    {
        // Create a new instance of Harmony

        static bool initialized = false;
        // Let's patch the Server_ProcessPlayerChatMessage function within UIChat
        [HarmonyPostfix]
        public static void Postfix(UIMinimap __instance)
        {
            if (!initialized)
            {
                if (__instance.Team == PlayerTeam.Blue)
                {
                    VisualElement minimapVisualElement = Traverse.Create(__instance).Field("minimapVisualElement").GetValue<VisualElement>();
                    minimapVisualElement.style.rotate = new Rotate(new Angle(180f, AngleUnit.Degree));
                }
                initialized = true;
                //Debug.Log("Running my injected Start() logic on first Update");
            }
            Dictionary<PlayerBodyV2, VisualElement> playerBodyVisualElementMap = Traverse.Create(__instance).Field("playerBodyVisualElementMap").GetValue<Dictionary<PlayerBodyV2, VisualElement>>();
            foreach (KeyValuePair<PlayerBodyV2, VisualElement> keyValuePair in playerBodyVisualElementMap)
            {
                PlayerBodyV2 key = keyValuePair.Key;
                VisualElement value = keyValuePair.Value;
                if (key)
                {
                    float value2 = (__instance.Team == PlayerTeam.Blue) ? key.transform.rotation.eulerAngles.y : (key.transform.rotation.eulerAngles.y + 180f);

                    VisualElement e = playerBodyVisualElementMap[key];
                    if (!key.Player)
                    {
                        return;
                    }
                    Vector2 centerOffset = Vector2.zero;
                    VisualElement minimapVisualElement = Traverse.Create(__instance).Field("minimapVisualElement").GetValue<VisualElement>();
                    Vector3 position = (__instance.Team == PlayerTeam.Blue) ? key.transform.position : (-key.transform.position);
                    Vector2 vector = WorldPositionToMinimapPosition(position, NetworkBehaviourSingleton<LevelManager>.Instance.IceBounds, __instance);
                    if (key.Player.IsLocalPlayer)
                    {
                        centerOffset = vector;
                        VisualElement minimapMarkingsVisualElement = Traverse.Create(__instance).Field("minimapMarkingsVisualElement").GetValue<VisualElement>();
                        VisualElement minimapBackgroundVisualElement = Traverse.Create(__instance).Field("minimapBackgroundVisualElement").GetValue<VisualElement>();
                        if (ConfigData.Instance.CenterOnPlayer)
                        {
                            minimapMarkingsVisualElement.style.translate = new Translate(centerOffset.x, -centerOffset.y);
                            minimapBackgroundVisualElement.style.translate = new Translate(centerOffset.x, -centerOffset.y);
                            value.style.translate = new Translate(0, 0);
                        }
                        else
                        {
                            //value.style.translate = new Translate(-vector.x, vector.y);
                        }
                        
                        ////Debug.Log($"Vector X: {vector.x.ToString()}, Vector Y: {vector.y.ToString()}");
                        minimapVisualElement.style.rotate = new Rotate(-value2 + 180f);
                        foreach (KeyValuePair<PlayerBodyV2, VisualElement> keyValuePair2 in playerBodyVisualElementMap)
                        {
                            PlayerBodyV2 key2 = keyValuePair2.Key;
                            VisualElement value3 = keyValuePair2.Value;
                            if (value3 == null || key2.Player.IsLocalPlayer) continue;

                            Label numberLabel2 = value3.Query<Label>("Number");
                            if (numberLabel2 != null)
                            {
                                // Cancel the minimap rotation so text is always upright
                                numberLabel2.style.rotate = new Rotate(value2 + 180f);
                            }
                        }
                            //minimapVisualElement.style.translate = new Translate(-vector.x, vector.y);
                            //value.style.translate = new Translate(vector.x, -vector.y);
                        }
                    else
                    {
                        if (ConfigData.Instance.CenterOnPlayer)
                        {
                            Vector2 adjusted = vector - centerOffset;
                            value.style.translate = new Translate(-adjusted.x, adjusted.y);
                        }   
                    }
                }
            }
        }
        private static Vector2 WorldPositionToMinimapPosition(Vector3 position, Bounds bounds, UIMinimap __instance)
        {
            VisualElement minimapMarkingsVisualElement = Traverse.Create(__instance).Field("minimapMarkingsVisualElement").GetValue<VisualElement>();
            Vector2 vector = new Vector2((position.x + bounds.center.x) / bounds.size.x, (position.z + bounds.center.z) / bounds.size.z);
            Vector2 vector2 = new Vector2(minimapMarkingsVisualElement.resolvedStyle.width, minimapMarkingsVisualElement.resolvedStyle.height);
            return new Vector2(vector2.x * vector.x, vector2.y * vector.y);
        }
    }
    public class RotateMinimap : IPuckMod
    {
        static readonly Harmony harmony = new Harmony("GAFURIX.RotateMinimap");
        
        public bool OnEnable()
        {
            Debug.Log("Rotate Minimap Enabled");
            ModConfig.Initialize();   // Ensures config file exists in <Puck>/config/
            ConfigData.Load();        // Loads JSON into the singleton (ConfigData.Instance)
            try
            {
                // Patched all functions we have defined to be patched
                harmony.PatchAll();
            }
            catch (Exception e)
            {
                //Debug.LogError($"Harmony patch failed: {e.Message}");

                return false;
            }

            return true;
        }

        public bool OnDisable()
        {
            try
            {
                // Reverts our patches, essentially returning the game to a vanilla state
                harmony.UnpatchSelf();
            }
            catch (Exception e)
            {
                //Debug.LogError($"Harmony unpatch failed: {e.Message}");

                return false;
            }

            return true;
        }
    }
}