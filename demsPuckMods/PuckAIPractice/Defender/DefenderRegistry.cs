using System.Collections.Generic;

namespace PuckAIPractice.Defender
{
    public static class DefenderRegistry
    {
        public class Entry
        {
            public Player Player;
            public PlayerTeam Team;
            public string PositionName;
            public ulong ClientId;
        }

        // Clear of the goalie-bot range (7777777/7777778) and clear of the
        // replay-copy offset (real clientId + 1337). 8_000_000+ is safe.
        private const ulong ClientIdBase = 8000000UL;
        private static ulong nextClientId = ClientIdBase;

        private static readonly Dictionary<ulong, Entry> byClientId = new Dictionary<ulong, Entry>();

        public static ulong AllocateClientId() => nextClientId++;

        public static void Register(Entry entry) => byClientId[entry.ClientId] = entry;

        public static void Unregister(ulong clientId) => byClientId.Remove(clientId);

        public static bool IsDefenderClientId(ulong clientId) => byClientId.ContainsKey(clientId);

        public static Entry GetByClientId(ulong clientId) =>
            byClientId.TryGetValue(clientId, out var e) ? e : null;

        public static IEnumerable<Entry> All => byClientId.Values;

        public static void CleanupDestroyed()
        {
            var stale = new List<ulong>();
            foreach (var kvp in byClientId)
            {
                if (kvp.Value.Player == null) stale.Add(kvp.Key);
            }
            foreach (var id in stale) byClientId.Remove(id);
        }
    }
}
