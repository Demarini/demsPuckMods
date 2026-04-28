using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace RotateMinimap
{
    static class MinimapFields
    {
        static bool _resolved;
        static FieldInfo _boundsField;

        public static void EnsureResolved(UIMinimap instance)
        {
            if (_resolved) return;
            _resolved = true;
            _boundsField = typeof(UIMinimap).GetField("Bounds", BindingFlags.Public | BindingFlags.Instance)
                        ?? typeof(UIMinimap).GetField("Bounds", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public static VisualElement GetMinimap(UIMinimap inst)    => Traverse.Create(inst).Field("minimap").GetValue<VisualElement>();
        public static VisualElement GetForeground(UIMinimap inst) => Traverse.Create(inst).Field("foreground").GetValue<VisualElement>();
        public static VisualElement GetBackground(UIMinimap inst) => Traverse.Create(inst).Field("background").GetValue<VisualElement>();
        public static VisualElement GetContent(UIMinimap inst)    => Traverse.Create(inst).Field("content").GetValue<VisualElement>();

        public static Bounds? GetBounds(UIMinimap inst)
        {
            if (_boundsField == null) return null;
            return (Bounds)_boundsField.GetValue(inst);
        }
    }

    [HarmonyPatch(typeof(UIMinimap), "RemovePlayerBody")]
    public static class MinimapPinnedIconCleanup
    {
        [HarmonyPrefix]
        public static bool Prefix(UIMinimap __instance, object playerBody)
        {
            if (playerBody == null) return true;

            var playerMap = Traverse.Create(__instance).Field("playerBodyVisualElementMap").GetValue<object>() as IDictionary;
            if (playerMap == null || !playerMap.Contains(playerBody)) return true;

            var icon = playerMap[playerBody] as VisualElement;
            if (icon == null) return true;

            MinimapFields.EnsureResolved(__instance);
            var minimapRoot = MinimapFields.GetMinimap(__instance);

            if (minimapRoot != null && icon.parent == minimapRoot)
            {
                minimapRoot.Remove(icon);
                playerMap.Remove(playerBody);
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(UIMinimap), "AddPlayerBody")]
    public static class PinLocalPlayerIconPatch
    {
        static VisualElement pinnedLocalIcon;

        [HarmonyPostfix]
        public static void Postfix(UIMinimap __instance, object playerBody)
        {
            if (playerBody == null) return;
            var player = Traverse.Create(playerBody).Property("Player").GetValue<Player>();
            if (player == null || !player.IsLocalPlayer || !ConfigData.Instance.CenterOnPlayer) return;

            var playerMap = Traverse.Create(__instance).Field("playerBodyVisualElementMap").GetValue<object>() as IDictionary;
            if (playerMap == null || !playerMap.Contains(playerBody)) return;

            var icon = playerMap[playerBody] as VisualElement;
            if (icon == null) return;

            if (pinnedLocalIcon != null && pinnedLocalIcon != icon)
            {
                pinnedLocalIcon.RemoveFromHierarchy();
                pinnedLocalIcon = null;
            }

            MinimapFields.EnsureResolved(__instance);
            var minimapRoot = MinimapFields.GetMinimap(__instance);

            if (minimapRoot != null)
            {
                icon.RemoveFromHierarchy();
                minimapRoot.Add(icon);
                icon.BringToFront();
                icon.style.translate = new Translate(0, 0);
                pinnedLocalIcon = icon;
            }
        }
    }

    [HarmonyPatch(typeof(UIMinimap), "Update")]
    public static class RotateMinimapPatch
    {
        static bool initialized = false;

        [HarmonyPostfix]
        public static void Postfix(UIMinimap __instance)
        {
            MinimapFields.EnsureResolved(__instance);

            if (!initialized)
            {
                if (__instance.Team == PlayerTeam.Blue)
                {
                    var minimapVE = MinimapFields.GetMinimap(__instance);
                    if (minimapVE != null)
                        minimapVE.style.rotate = new Rotate(new Angle(180f, AngleUnit.Degree));
                }
                initialized = true;
            }

            var bounds = MinimapFields.GetBounds(__instance);
            if (bounds == null) return;

            var playerMap = Traverse.Create(__instance).Field("playerBodyVisualElementMap").GetValue<object>() as IDictionary;
            if (playerMap == null) return;

            Vector2 centerOffset = Vector2.zero;

            foreach (DictionaryEntry entry in playerMap)
            {
                var key     = entry.Key;
                var value   = entry.Value as VisualElement;
                var keyComp = key as Component;

                if (keyComp == null || value == null) continue;

                var keyPlayer = Traverse.Create(key).Property("Player").GetValue<Player>();
                if (keyPlayer == null) return;

                float rotY = keyComp.transform.rotation.eulerAngles.y;
                float value2 = (__instance.Team == PlayerTeam.Blue) ? rotY : (rotY + 180f);

                var minimapVE = MinimapFields.GetMinimap(__instance);
                Vector3 position = (__instance.Team == PlayerTeam.Blue)
                    ? keyComp.transform.position
                    : -keyComp.transform.position;

                Vector2 vector = WorldPositionToMinimapPosition(position, bounds.Value, __instance);

                if (keyPlayer.IsLocalPlayer)
                {
                    centerOffset = vector;
                    var contentVE = MinimapFields.GetContent(__instance);
                    var bgVE     = MinimapFields.GetBackground(__instance);

                    if (ConfigData.Instance.CenterOnPlayer)
                    {
                        if (contentVE != null) contentVE.style.translate = new Translate(centerOffset.x, -centerOffset.y);
                        if (bgVE != null) bgVE.style.translate = new Translate(centerOffset.x, -centerOffset.y);
                        value.style.translate = new Translate(0, 0);
                    }

                    if (minimapVE != null)
                        minimapVE.style.rotate = new Rotate(-value2 + 180f);

                    foreach (DictionaryEntry entry2 in playerMap)
                    {
                        var key2     = entry2.Key;
                        var value3   = entry2.Value as VisualElement;
                        if (value3 == null) continue;

                        var key2Player = Traverse.Create(key2).Property("Player").GetValue<Player>();
                        if (key2Player == null || key2Player.IsLocalPlayer) continue;

                        Label numberLabel = value3.Query<Label>("NumberLabel").First();
                        if (numberLabel == null)
                            numberLabel = value3.Query<Label>("Number").First();
                        if (numberLabel != null)
                            numberLabel.style.rotate = new Rotate(value2 + 180f);
                    }
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

        private static Vector2 WorldPositionToMinimapPosition(Vector3 position, Bounds bounds, UIMinimap __instance)
        {
            var contentVE = MinimapFields.GetContent(__instance);
            Vector2 vector = new Vector2(
                (position.x + bounds.center.x) / bounds.size.x,
                (position.z + bounds.center.z) / bounds.size.z);
            Vector2 vector2 = new Vector2(
                contentVE?.resolvedStyle.width ?? 0,
                contentVE?.resolvedStyle.height ?? 0);
            return new Vector2(vector2.x * vector.x, vector2.y * vector.y);
        }
    }

    public class RotateMinimap : IPuckMod
    {
        static readonly Harmony harmony = new Harmony("GAFURIX.RotateMinimap");

        public bool OnEnable()
        {
            Debug.Log("Rotate Minimap Enabled");
            ModConfig.Initialize();
            ConfigData.Load();
            try { harmony.PatchAll(); }
            catch (Exception e) { return false; }
            return true;
        }

        public bool OnDisable()
        {
            try { harmony.UnpatchSelf(); }
            catch (Exception e) { return false; }
            return true;
        }
    }
}
