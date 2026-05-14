using HarmonyLib;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

namespace ModifyMinimapIcons
{
    [HarmonyPatch(typeof(UIMinimap), "SetScale")]
    public static class MinimapScalePatch
    {
        [HarmonyPostfix]
        public static void Postfix(float scale)
        {
            MinimapValues.CurrentScale = scale;
        }
    }

    [HarmonyPatch(typeof(UIMinimap), "Update")]
    public static class MinimapPlayerIconPatch
    {
        private static int lastMinimapId = -1;
        // PuckNew: PlayerBodyV2 → PlayerBody — store as object, access via Traverse
        private static object CachedLocalPlayer;
        private static float lastLogTime = 0f;
        private static readonly Dictionary<ulong, int> pendingRemovals = new Dictionary<ulong, int>();

        public static void ResetCache()
        {
            CachedLocalPlayer = null;
            pendingRemovals.Clear();
        }

        // Returns the IDictionary entry key (PlayerBody object) matching local client
        private static object FindLocalPlayer(IDictionary playerMap)
        {
            ulong localId = NetworkManager.Singleton.LocalClientId;

            var cachedPlayer = CachedLocalPlayer != null
                ? Traverse.Create(CachedLocalPlayer).Property("Player").GetValue<Player>()
                : null;
            if (cachedPlayer == null || cachedPlayer.OwnerClientId != localId)
                CachedLocalPlayer = null;

            foreach (DictionaryEntry entry in playerMap)
            {
                var player = Traverse.Create(entry.Key).Property("Player").GetValue<Player>();
                if (player != null && player.OwnerClientId == localId)
                {
                    if (CachedLocalPlayer != entry.Key)
                        CachedLocalPlayer = entry.Key;
                    return CachedLocalPlayer;
                }
            }

            CachedLocalPlayer = null;
            return null;
        }

        [HarmonyPostfix]
        public static void Postfix(UIMinimap __instance)
        {
            if (__instance.GetInstanceID() != lastMinimapId)
            {
                lastMinimapId = __instance.GetInstanceID();
                CachedLocalPlayer = null;
                pendingRemovals.Clear();
            }

            var cfg = ConfigData.Instance;

            // PuckNew: Dictionary<PlayerBody, VisualElement> — access via IDictionary to avoid
            // compile-time dependency on PlayerBody type (not in old libs)
            var playerMap = Traverse.Create(__instance).Field("playerBodyVisualElementMap").GetValue<object>() as IDictionary;
            var puckMap   = Traverse.Create(__instance).Field("puckVisualElementMap").GetValue<object>() as IDictionary;

            if (playerMap == null || playerMap.Count == 0)
            {
                CachedLocalPlayer = null;
                pendingRemovals.Clear();
                return;
            }

            foreach (DictionaryEntry entry in playerMap)
            {
                var playerBodyObj = entry.Key;
                var icon = entry.Value as VisualElement;
                var player = Traverse.Create(playerBodyObj).Property("Player").GetValue<Player>();
                var pbComp = playerBodyObj as Component;

                if (player == null || pbComp?.transform == null || icon == null || icon.panel == null)
                {
                    ulong id = player?.OwnerClientId ?? 0;
                    if (!pendingRemovals.ContainsKey(id)) pendingRemovals[id] = 0;
                    pendingRemovals[id]++;
                }
                else
                {
                    ulong id = player.OwnerClientId;
                    if (pendingRemovals.ContainsKey(id)) pendingRemovals.Remove(id);
                }
            }

            object localPlayerObj = FindLocalPlayer(playerMap);
            if (localPlayerObj == null || NetworkManager.Singleton == null)
            {
                CachedLocalPlayer = null;
                return;
            }
            var localPlayerComp = Traverse.Create(localPlayerObj).Property("Player").GetValue<Player>();
            if (localPlayerComp == null) { CachedLocalPlayer = null; return; }

            bool isSpectator = (__instance.Team != PlayerTeam.Red && __instance.Team != PlayerTeam.Blue);

            foreach (DictionaryEntry entry in playerMap)
            {
                var playerBodyObj = entry.Key;
                var icon = entry.Value as VisualElement;
                var player = Traverse.Create(playerBodyObj).Property("Player").GetValue<Player>();

                if (player == null || icon == null || icon.panel == null) continue;

                VisualElement body = icon.Query<VisualElement>("Body");
                if (body == null || body.panel == null) continue;

                var texture = body.resolvedStyle.backgroundImage.texture;
                if (texture != null) body.style.backgroundImage = new StyleBackground(texture);

                bool isLocal = (playerBodyObj == localPlayerObj);
                PlayerTeam targetTeam = player.Team;
                float finalScale = 1f;

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

                float minimapScale = MinimapValues.CurrentScale;
                body.transform.scale = new Vector3(finalScale * minimapScale, finalScale * minimapScale, 1f);
            }

            if (puckMap != null)
            {
                foreach (DictionaryEntry entry in puckMap)
                {
                    var puck = entry.Key as Puck;
                    var value = entry.Value as VisualElement;
                    if (puck == null || value == null) continue;
                    float minimapScale = MinimapValues.CurrentScale;
                    value.transform.scale = new Vector3(cfg.puckScale * .9f * minimapScale, cfg.puckScale * minimapScale, 1f);
                    value.style.backgroundColor = new StyleColor(ConfigData.HexToColor(cfg.puckColor));
                }
            }
        }
    }

    public class ModifyMinimapIcons : IPuckPlugin
    {
        static readonly Harmony harmony = new Harmony("GAFURIX.ModifyMinimapIcons");

        public bool OnEnable()
        {
            Debug.Log("Modify Minimap Enabled");
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
