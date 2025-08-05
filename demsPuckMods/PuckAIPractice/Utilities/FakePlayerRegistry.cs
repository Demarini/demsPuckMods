using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuckAIPractice.Utilities
{
    public static class FakePlayerRegistry
    {
        private static readonly HashSet<Player> fakePlayers = new HashSet<Player>();

        public static void Register(Player player)
        {
            if (player != null) fakePlayers.Add(player);
        }

        public static void Unregister(Player player)
        {
            if (player != null) fakePlayers.Remove(player);
        }

        public static bool IsFake(Player player)
        {
            return player != null && fakePlayers.Contains(player);
        }

        public static IEnumerable<Player> All => fakePlayers;
    }
}
