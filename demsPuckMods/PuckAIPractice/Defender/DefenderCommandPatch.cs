using HarmonyLib;
using PuckAIPractice.Patches;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PuckAIPractice.Defender
{
    [HarmonyPatch(typeof(VoteManagerController), "Event_Server_OnChatCommand")]
    public static class DefenderCommandPatch
    {
        private static readonly HashSet<string> AllowedPositions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "LW", "C", "RW", "LD", "RD"
        };

        [HarmonyPrefix]
        public static bool Prefix(Dictionary<string, object> message)
        {
            var command = (string)message["command"];
            if (command != "/defender") return true;

            Debug.Log($"[Defender] /defender received (practiceMode={PracticeModeDetector.IsPracticeMode})");

            var args = (string[])message["args"];
            if (args == null || args.Length < 1)
            {
                Debug.Log("[Defender] usage: /defender <LW|C|RW|LD|RD> | /defender all | /defender clear [position]");
                return false;
            }

            var first = args[0].ToUpperInvariant();

            if (first == "CLEAR")
            {
                if (args.Length >= 2)
                {
                    var targetPos = args[1].ToUpperInvariant();
                    if (!AllowedPositions.Contains(targetPos))
                    {
                        Debug.Log($"[Defender] invalid position '{args[1]}'. Allowed: LW, C, RW, LD, RD");
                        return false;
                    }
                    DefenderSpawner.DespawnAt(targetPos);
                }
                else
                {
                    DefenderSpawner.DespawnAll();
                }
                return false;
            }

            // Resolve the bot team once from the caller (used by both single-pos
            // and "all" branches).
            var callerId = (ulong)message["clientId"];
            var caller = PlayerManager.Instance.GetPlayerByClientId(callerId);
            var callerTeam = caller != null ? caller.Team : PlayerTeam.Blue;
            var botTeam = (callerTeam == PlayerTeam.Red) ? PlayerTeam.Blue : PlayerTeam.Red;

            if (first == "ALL")
            {
                // Defensemen first so they claim their wider zone before the
                // forwards arrive — purely cosmetic since the spawner just
                // skips already-claimed positions.
                var positions = new[] { "LD", "RD", "C", "LW", "RW" };
                int spawned = 0;
                foreach (var pos in positions)
                {
                    if (DefenderSpawner.Spawn(botTeam, pos, callerId) != null) spawned++;
                }
                Debug.Log($"[Defender] /defender all: spawned {spawned} of {positions.Length} on team {botTeam}");
                return false;
            }

            if (!AllowedPositions.Contains(first))
            {
                Debug.Log($"[Defender] invalid position '{args[0]}'. Allowed: LW, C, RW, LD, RD, or 'all'");
                return false;
            }

            DefenderSpawner.Spawn(botTeam, first, callerId);
            return false;
        }
    }
}
