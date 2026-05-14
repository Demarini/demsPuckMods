using HarmonyLib;
using PuckAIPractice.Patches;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PuckAIPractice.Chaser
{
    [HarmonyPatch(typeof(VoteManagerController), "Event_Server_OnChatCommand")]
    public static class ChaserCommandPatch
    {
        private static readonly HashSet<string> AllowedPositions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "LW", "C", "RW", "LD", "RD"
        };

        [HarmonyPrefix]
        public static bool Prefix(Dictionary<string, object> message)
        {
            var command = (string)message["command"];
            if (command != "/chaser") return true;

            Debug.Log($"[Chaser] /chaser received (practiceMode={PracticeModeDetector.IsPracticeMode})");

            var args = (string[])message["args"];
            if (args == null || args.Length < 1)
            {
                Debug.Log("[Chaser] usage: /chaser <LW|C|RW|LD|RD> | /chaser clear [position]");
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
                        Debug.Log($"[Chaser] invalid position '{args[1]}'. Allowed: LW, C, RW, LD, RD");
                        return false;
                    }
                    ChaserSpawner.DespawnAt(targetPos);
                }
                else
                {
                    ChaserSpawner.DespawnAll();
                }
                return false;
            }

            if (!AllowedPositions.Contains(first))
            {
                Debug.Log($"[Chaser] invalid position '{args[0]}'. Allowed: LW, C, RW, LD, RD");
                return false;
            }

            var callerId = (ulong)message["clientId"];
            var caller = PlayerManager.Instance.GetPlayerByClientId(callerId);
            var callerTeam = caller != null ? caller.Team : PlayerTeam.Blue;
            var botTeam = (callerTeam == PlayerTeam.Red) ? PlayerTeam.Blue : PlayerTeam.Red;

            ChaserSpawner.Spawn(botTeam, first, callerId);
            return false;
        }
    }
}
