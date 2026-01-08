using HarmonyLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Numerics;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

namespace ModifyMinimapIcons
{
    //[HarmonyPatch(typeof(UIMinimap), "Update")]
    //public static class MinimapPinnedIconHandler
    //{
    //    private static VisualElement pinnedIcon;

    //    [HarmonyPostfix]
    //    public static void Postfix(UIMinimap __instance)
    //    {
    //        var playerMap = Traverse.Create(__instance)
    //            .Field("playerBodyVisualElementMap")
    //            .GetValue<Dictionary<PlayerBodyV2, VisualElement>>();

    //        if (playerMap == null) return;

    //        foreach (var kvp in playerMap)
    //        {
    //            var playerBody = kvp.Key;
    //            var icon = kvp.Value;
    //            if (playerBody?.Player == null || !playerBody.Player.IsLocalPlayer)
    //                continue;

    //            var minimapRoot = Traverse.Create(__instance)
    //                .Field("minimapVisualElement").GetValue<VisualElement>();

    //            var minimapLayer = Traverse.Create(__instance)
    //                .Field("minimapMarkingsVisualElement").GetValue<VisualElement>();

    //            // If pinned already, just style it
    //            if (icon.parent == minimapRoot)
    //            {
    //                pinnedIcon = icon;
    //            }
    //            else if (true)
    //            {
    //                // Move it to root if the setting is on
    //                minimapLayer.Remove(icon);
    //                minimapRoot.Add(icon);
    //                icon.style.translate = new Translate(0, 0);
    //                pinnedIcon = icon;
    //                //Debug.Log("[PinnedIconHandler] Local icon pinned.");
    //            }
    //        }

    //        if (pinnedIcon != null)
    //        {
    //            // Apply any color/scale logic here for the pinned icon
    //            VisualElement body = pinnedIcon.Query<VisualElement>("Body");
    //            if (body != null)
    //            {
    //                body.style.unityBackgroundImageTintColor =
    //                    new StyleColor(ConfigData.HexToColor(ConfigData.Instance.playerColor));
    //            }
    //        }
    //    }
    //}
    [HarmonyPatch(typeof(UIMinimap), "SetScale")]
    public static class MinimapScalePatch
    {
        [HarmonyPostfix]
        public static void Postfix(float scale)
        {
            MinimapValues.CurrentScale = scale;
            //Debug.Log($"[Minimap Debug] Minimap scale set to {scale}");
        }
    }

    [HarmonyPatch(typeof(UIMinimap), "Update")]
    public static class MinimapPlayerIconPatch
    {
        private static int lastMinimapId = -1;
        private static PlayerBodyV2 CachedLocalPlayer;
        private static float lastLogTime = 0f;

        // Delay counters for safe cleanup
        private static readonly Dictionary<ulong, int> pendingRemovals = new Dictionary<ulong, int>();

        public static void ResetCache()
        {
            CachedLocalPlayer = null;
            pendingRemovals.Clear();
        }

        private static PlayerBodyV2 FindLocalPlayer(Dictionary<PlayerBodyV2, VisualElement> playerMap)
        {
            ulong localId = NetworkManager.Singleton.LocalClientId;

            // If we have a cached player but it no longer matches the ID or is null, clear it
            if (CachedLocalPlayer == null ||
                CachedLocalPlayer.Player == null ||
                CachedLocalPlayer.Player.OwnerClientId != localId)
            {
                CachedLocalPlayer = null;
            }

            // Always scan to find a fresh match (even if CachedLocalPlayer was set before)
            foreach (var kvp in playerMap)
            {
                if (kvp.Key?.Player != null && kvp.Key.Player.OwnerClientId == localId)
                {
                    if (CachedLocalPlayer != kvp.Key)
                    {
                        CachedLocalPlayer = kvp.Key;
                        ////Debug.Log($"[Minimap Debug] Local player reassigned: ID={localId}, Team={kvp.Key.Player.Team.Value}");
                    }
                    return CachedLocalPlayer;
                }
            }

            // If no match, clear cache so we try again next frame
            CachedLocalPlayer = null;
            //Debug.LogWarning("[Minimap Debug] Local player not found this frame.");
            return null;
        }

        private static float DistancePointToLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
        {
            Vector3 lineDir = lineEnd - lineStart;
            Vector3 pointDir = point - lineStart;
            float t = Mathf.Clamp01(Vector3.Dot(pointDir, lineDir.normalized) / lineDir.magnitude);
            Vector3 projection = lineStart + lineDir * t;
            return Vector3.Distance(point, projection);
        }

        [HarmonyPostfix]
        public static void Postfix(UIMinimap __instance)
        {
            if (__instance.GetInstanceID() != lastMinimapId)
            {
                lastMinimapId = __instance.GetInstanceID();
                CachedLocalPlayer = null;
                pendingRemovals.Clear();
                //Debug.Log($"[Minimap Debug] New minimap detected (simulated Start). ID={lastMinimapId}");
            }

            var cfg = ConfigData.Instance;

            var playerMap = Traverse.Create(__instance)
                .Field("playerBodyVisualElementMap")
                .GetValue<Dictionary<PlayerBodyV2, VisualElement>>();

            var puckMap = Traverse.Create(__instance)
                .Field("puckVisualElementMap")
                .GetValue<Dictionary<Puck, VisualElement>>();

            if (playerMap == null || playerMap.Count == 0)
            {
                CachedLocalPlayer = null;
                pendingRemovals.Clear();
                return;
            }

            // Update pending removals for entries that look broken
            foreach (var kvp in playerMap)
            {
                var pb = kvp.Key;
                if (pb == null || pb.Player == null || pb.transform == null || kvp.Value == null || kvp.Value.panel == null)
                {
                    ulong id = pb?.Player?.OwnerClientId ?? 0;
                    if (!pendingRemovals.ContainsKey(id))
                        pendingRemovals[id] = 0;

                    pendingRemovals[id]++;
                    if (pendingRemovals[id] == 5)
                    {
                        //Debug.Log($"[Minimap Debug] Stale entry for {id} flagged after 5 frames (waiting for UIMinimap cleanup).");
                    }

                }
                else
                {
                    ulong id = pb.Player.OwnerClientId;
                    if (pendingRemovals.ContainsKey(id))
                        pendingRemovals.Remove(id);
                }
            }

            PlayerBodyV2 localPlayer = FindLocalPlayer(playerMap);
            if (localPlayer?.Player == null || NetworkManager.Singleton == null)
            {
                CachedLocalPlayer = null;
                return;
            }

            ulong localId = NetworkManager.Singleton.LocalClientId;
            bool isSpectator = (__instance.Team != PlayerTeam.Red && __instance.Team != PlayerTeam.Blue);

            // Debug state once per second
            if (Time.time - lastLogTime >= 1f)
            {
                lastLogTime = Time.time;
                //Debug.Log($"[Minimap Debug] --- State Dump ---");
                //Debug.Log($"Local PlayerID={localId}, Found={localPlayer != null}, Team={(localPlayer?.Player?.Team.Value.ToString() ?? "None")}");
                //Debug.Log($"Spectator Mode: {isSpectator}");
                foreach (var kvp in playerMap)
                {
                    var pb = kvp.Key;
                    if (pb?.Player == null) continue;
                    //Debug.Log($"PlayerID={pb.Player.OwnerClientId}, Team={pb.Player.Team.Value}, IsLocal={(pb == localPlayer)}");
                }
                //Debug.Log($"[Minimap Debug] --- End Dump ---");
            }

            // Apply colors/scaling for valid players only (skip flagged ones)
            foreach (var kvp in playerMap)
            {
                PlayerBodyV2 playerBody = kvp.Key;
                VisualElement icon = kvp.Value;

                if (playerBody?.Player == null || icon == null || icon.panel == null)
                    continue; // Skip — let UIMinimap handle its cleanup

                VisualElement body = icon.Query<VisualElement>("Body");
                if (body == null || body.panel == null)
                    continue;

                var texture = body.resolvedStyle.backgroundImage.texture;
                if (texture != null)
                    body.style.backgroundImage = new StyleBackground(texture);

                bool isLocal = (playerBody == localPlayer);
                PlayerTeam targetTeam = playerBody.Player.Team.Value;
                float finalScale = 1f;

                // --- Color/scale logic (unchanged) ---
                if (isLocal)
                {
                    if (cfg.usePlayerColor)
                        body.style.unityBackgroundImageTintColor = new StyleColor(ConfigData.HexToColor(cfg.playerColor));
                    finalScale = cfg.playerScalingFactor;
                    if (cfg.pulsePlayerIcon)
                    {
                        float pulse = (Mathf.Sin(Time.time * cfg.playerPulseSpeed) + 1f) * 0.5f;
                        finalScale += pulse * cfg.playerPulseStrength;
                    }
                }
                else if (isSpectator)
                {
                    Color teamColor = (targetTeam == PlayerTeam.Red)
                        ? ConfigData.HexToColor(cfg.redTeamColor)
                        : ConfigData.HexToColor(cfg.blueTeamColor);
                    body.style.unityBackgroundImageTintColor = new StyleColor(teamColor);
                    finalScale = (targetTeam == PlayerTeam.Red ? cfg.teamScalingFactor : cfg.opponentScalingFactor);
                }
                else
                {
                    if (targetTeam == __instance.Team)
                    {
                        if (cfg.useTeamColor)
                            body.style.unityBackgroundImageTintColor = new StyleColor(ConfigData.HexToColor(cfg.teamColor));
                        else
                        {
                            Color fallback = (__instance.Team == PlayerTeam.Red)
                                ? ConfigData.HexToColor(cfg.redTeamColor)
                                : ConfigData.HexToColor(cfg.blueTeamColor);
                            body.style.unityBackgroundImageTintColor = new StyleColor(fallback);
                        }

                        finalScale = cfg.teamScalingFactor;
                        if (cfg.pulseTeamIcon)
                        {
                            float pulse = (Mathf.Sin(Time.time * cfg.teamPulseSpeed) + 1f) * 0.5f;
                            finalScale += pulse * cfg.teamPulseStrength;
                        }
                    }
                    else
                    {
                        if (cfg.useOpponentColor)
                            body.style.unityBackgroundImageTintColor = new StyleColor(ConfigData.HexToColor(cfg.opponentColor));
                        else
                        {
                            Color fallback = (targetTeam == PlayerTeam.Red)
                                ? ConfigData.HexToColor(cfg.redTeamColor)
                                : ConfigData.HexToColor(cfg.blueTeamColor);
                            body.style.unityBackgroundImageTintColor = new StyleColor(fallback);
                        }

                        finalScale = cfg.opponentScalingFactor;
                        if (cfg.pulseOpponentIcon)
                        {
                            float pulse = (Mathf.Sin(Time.time * cfg.opponentPulseSpeed) + 1f) * 0.5f;
                            finalScale += pulse * cfg.opponentPulseStrength;
                        }
                    }
                }

                // Apply scaling relative to minimap zoom
                float minimapScale = MinimapValues.CurrentScale;
                body.transform.scale = new Vector3(finalScale * minimapScale, finalScale * minimapScale, 1f);
            }
            foreach (KeyValuePair<Puck, VisualElement> keyValuePair2 in puckMap)
            {
                Puck key2 = keyValuePair2.Key;
                VisualElement value2 = keyValuePair2.Value;
                if (key2)
                {
                    //test
                    float minimapScale = MinimapValues.CurrentScale;
                    value2.transform.scale = new Vector3(cfg.puckScale * .9f * minimapScale, cfg.puckScale * minimapScale, 1f);
                    value2.style.backgroundColor = new StyleColor(ConfigData.HexToColor(cfg.puckColor));
                }
            }
        }
    }

    public class ModifyMinimapIcons : IPuckMod
    {
        static readonly Harmony harmony = new Harmony("GAFURIX.ModifyMinimapIcons");

        public bool OnEnable()
        {
            Debug.Log("Modify Minimap Enabled");
            ModConfig.Initialize();
            ConfigData.Load();
            try
            {
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
