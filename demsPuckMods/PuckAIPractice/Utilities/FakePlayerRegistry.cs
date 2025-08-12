using System.Collections.Generic;
using UnityEngine;

namespace PuckAIPractice.Utilities
{
    public static class FakePlayerRegistry
    {
        private static readonly HashSet<Player> fakePlayers = new HashSet<Player>();
        private static readonly HashSet<Player> existingBots = new HashSet<Player>();
        public static void Register(Player player)
        {
            if (player != null)
            {
                if (!fakePlayers.Contains(player)) fakePlayers.Add(player);
                if (!existingBots.Contains(player)) existingBots.Add(player);
                //Debug.Log($"[FakeRegistry] Registered {player.Username?.Value} (OwnerClientId: {player.OwnerClientId})");
            }
        }

        public static void Unregister(Player player)
        {
            if (player != null)
            {
                fakePlayers.Remove(player);
                //Debug.Log($"[FakeRegistry] Unregistered {player.Username?.Value} (OwnerClientId: {player.OwnerClientId})");
            }
        }

        public static bool IsFake(Player player)
        {
            bool result = player != null && fakePlayers.Contains(player);
            // Debug.Log($"[FakeRegistry] IsFake({player?.Username?.Value}) = {result}");
            return result;
        }

        public static IEnumerable<Player> All => fakePlayers;
        public static IEnumerable<Player> AllExisting => existingBots;
    }
}