using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PuckAIPractice.Patches
{
    // Local visual override: any player whose OwnerClientId falls in this mod's
    // bot ranges (goalies 7777777/7777778, chasers 8M+, defenders 9M+) gets
    // their exposed skin recolored grey and their eyes recolored red. Runs
    // client-side only — there's no NetworkVariable for skin/eye color, so
    // we don't try to sync. Mod-installed users see robot bots; vanilla
    // clients see the bots' inherited jersey/skin as normal players. Real
    // Steam clientIds are 64-bit and start around 7.6e16, so the < 10M ceiling
    // is well clear of any real player.
    [HarmonyPatch(typeof(PlayerBody), "ApplyCustomizations")]
    public static class BotRobotLookPatch
    {
        private const ulong BotClientIdMin = 7_000_000UL;
        private const ulong BotClientIdMax = 10_000_000UL;

        private static readonly Color RobotSkinColor = new Color(0.62f, 0.64f, 0.68f);
        private static readonly Color RobotEyeColor = new Color(0.95f, 0.08f, 0.08f);

        // Material-name pattern match. The PlayerHead prefab's exact material
        // names aren't visible in the decompiled source, so we go off common
        // Unity character-asset conventions. The HashSet is for one-time
        // discovery logging — we log every distinct material name we see on
        // a bot's body once so we can tune the patterns if defaults miss.
        private static readonly HashSet<string> SeenMaterialNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        [HarmonyPostfix]
        public static void Postfix(PlayerBody __instance)
        {
            try
            {
                if (__instance == null || __instance.Player == null) return;
                ulong clientId = __instance.Player.OwnerClientId;
                if (clientId < BotClientIdMin || clientId >= BotClientIdMax) return;

                ApplyRobotLook(__instance);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[BotRobotLook] failed: {ex.Message}");
            }
        }

        private static void ApplyRobotLook(PlayerBody body)
        {
            var mesh = body.PlayerMesh;
            if (mesh == null) return;

            // Scan the whole PlayerMesh hierarchy (head + torso + groin) — body
            // skin can share a material with the head, so a head-only scan would
            // miss exposed arms/neck and leave the bot half-grey.
            var renderers = mesh.GetComponentsInChildren<Renderer>(true);
            foreach (var rend in renderers)
            {
                if (rend == null) continue;
                var mats = rend.materials; // intentional clone; modifies per-instance only
                bool changed = false;
                for (int i = 0; i < mats.Length; i++)
                {
                    var m = mats[i];
                    if (m == null) continue;

                    string raw = m.name ?? string.Empty;
                    if (SeenMaterialNames.Add(raw))
                    {
                        Debug.Log($"[BotRobotLook] discovery: material '{raw}' on '{rend.name}'");
                    }
                    string n = raw.ToLowerInvariant();

                    if (n.Contains("eye"))
                    {
                        m.color = RobotEyeColor;
                        if (m.HasProperty("_EmissionColor"))
                        {
                            m.SetColor("_EmissionColor", RobotEyeColor * 1.5f);
                            m.EnableKeyword("_EMISSION");
                        }
                        changed = true;
                    }
                    else if ((n.Contains("skin") || n.Contains("face") || n.Contains("head") || n.Contains("body"))
                             && !n.Contains("hair") && !n.Contains("beard") && !n.Contains("mustache")
                             && !n.Contains("jersey") && !n.Contains("stick") && !n.Contains("tape")
                             && !n.Contains("helmet") && !n.Contains("hat") && !n.Contains("visor")
                             && !n.Contains("eye"))
                    {
                        m.color = RobotSkinColor;
                        changed = true;
                    }
                }
                if (changed) rend.materials = mats;
            }
        }
    }
}
