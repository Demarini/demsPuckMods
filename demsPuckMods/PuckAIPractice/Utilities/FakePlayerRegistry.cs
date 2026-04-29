using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PuckAIPractice.Utilities
{
    public static class FakePlayerRegistry
    {
        private static readonly HashSet<Player> fakePlayers = new HashSet<Player>();
        private static readonly Dictionary<Player, PlayerTeam> fakePlayerTeams = new Dictionary<Player, PlayerTeam>();

        public static void Register(Player player, PlayerTeam team)
        {
            if (player == null) return;
            CleanupDestroyed();
            if (!fakePlayers.Any(p => p != null && p.OwnerClientId == player.OwnerClientId))
            {
                fakePlayers.Add(player);
                fakePlayerTeams[player] = team;
            }
        }

        public static void Unregister(Player player)
        {
            fakePlayers.Remove(player);
            fakePlayerTeams.Remove(player);
        }

        public static void CleanupDestroyed()
        {
            fakePlayers.RemoveWhere(p => p == null);
            var staleKeys = new List<Player>();
            foreach (var kvp in fakePlayerTeams)
            {
                if (kvp.Key == null)
                    staleKeys.Add(kvp.Key);
            }
            foreach (var key in staleKeys)
                fakePlayerTeams.Remove(key);
        }

        public static void Clear()
        {
            fakePlayers.Clear();
            fakePlayerTeams.Clear();
        }

        public static bool IsFake(Player player)
        {
            return player != null && fakePlayers.Contains(player);
        }

        public static PlayerTeam GetTeam(Player player)
        {
            if (player == null) return PlayerTeam.None;
            return fakePlayerTeams.TryGetValue(player, out var team) ? team : PlayerTeam.None;
        }

        public static bool HasBotForTeam(PlayerTeam team)
        {
            return fakePlayerTeams.ContainsValue(team);
        }

        public static IEnumerable<Player> All => fakePlayers;
    }
}