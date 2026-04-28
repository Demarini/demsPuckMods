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

    [HarmonyPatch(typeof(UIMinimap), "Update")]
    public static class RotateMinimapPatch
    {
        static float _nextLogTime;

        [HarmonyPostfix]
        public static void Postfix(UIMinimap __instance)
        {
            MinimapFields.EnsureResolved(__instance);

            var bounds = MinimapFields.GetBounds(__instance);
            if (bounds == null) return;

            var playerMap = Traverse.Create(__instance).Field("playerBodyVisualElementMap").GetValue<object>() as IDictionary;
            if (playerMap == null) return;

            var minimapVE = MinimapFields.GetMinimap(__instance);
            var contentVE = MinimapFields.GetContent(__instance);
            var bgVE = MinimapFields.GetBackground(__instance);
            var fgVE = MinimapFields.GetForeground(__instance);

            float localRotation = 0f;
            Vector2 localMinimapPos = Vector2.zero;
            Vector3 localWorldPos = Vector3.zero;
            bool foundLocal = false;

            foreach (DictionaryEntry entry in playerMap)
            {
                var comp = entry.Key as Component;
                if (comp == null) continue;
                var player = Traverse.Create(entry.Key).Property("Player").GetValue<Player>();
                if (player == null || !player.IsLocalPlayer) continue;

                float eulerY = comp.transform.rotation.eulerAngles.y;
                localRotation = (__instance.Team == PlayerTeam.Blue) ? eulerY : (eulerY + 180f);

                localWorldPos = comp.transform.position;
                Vector3 position = (__instance.Team == PlayerTeam.Blue)
                    ? comp.transform.position
                    : -comp.transform.position;
                localMinimapPos = WorldPositionToMinimapPosition(position, bounds.Value, __instance);
                foundLocal = true;
                break;
            }

            if (!foundLocal) return;

            float mapRotation = -localRotation + 180f;
            if (minimapVE != null)
                minimapVE.style.rotate = new Rotate(mapRotation);

            if (ConfigData.Instance.CenterOnPlayer)
            {
                var shift = new Translate(localMinimapPos.x, -localMinimapPos.y);
                if (contentVE != null) contentVE.style.translate = shift;
                if (bgVE != null) bgVE.style.translate = shift;
                if (fgVE != null) fgVE.style.translate = shift;
            }

            foreach (DictionaryEntry entry in playerMap)
            {
                var ve = entry.Value as VisualElement;
                if (ve == null) continue;
                var player = Traverse.Create(entry.Key).Property("Player").GetValue<Player>();
                if (player == null || player.IsLocalPlayer) continue;

                Label numberLabel = ve.Query<Label>("NumberLabel").First();
                if (numberLabel != null)
                    numberLabel.style.rotate = new Rotate(localRotation - 180f);
            }

            if (Time.time >= _nextLogTime)
            {
                _nextLogTime = Time.time + 2f;
                Debug.Log($"[RotateMinimap] Team={__instance.Team} CenterOnPlayer={ConfigData.Instance.CenterOnPlayer}");
                Debug.Log($"[RotateMinimap] WorldPos={localWorldPos} MinimapPos={localMinimapPos} EulerY={localRotation:F1} MapRot={mapRotation:F1}");
                if (contentVE != null)
                    Debug.Log($"[RotateMinimap] ContentSize=({contentVE.resolvedStyle.width:F0},{contentVE.resolvedStyle.height:F0}) ContentTranslate=({localMinimapPos.x:F1},{(-localMinimapPos.y):F1})");
                if (minimapVE != null)
                    Debug.Log($"[RotateMinimap] MinimapSize=({minimapVE.resolvedStyle.width:F0},{minimapVE.resolvedStyle.height:F0})");
                Debug.Log($"[RotateMinimap] Bounds center={bounds.Value.center} size={bounds.Value.size}");
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
