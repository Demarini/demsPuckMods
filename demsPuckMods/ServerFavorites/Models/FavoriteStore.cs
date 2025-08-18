using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ServerBrowserInGame.Models
{
    public static class FavoriteStore
    {
        // In-memory sets you already use
        public static readonly HashSet<string> IpPort = new HashSet<string>(); // "ip:port"
        public static readonly HashSet<string> Names = new HashSet<string>(); // optional fallback
        // NEW: persisted UI state
        public static bool FavOnly { get; private set; } = false;
        // Where we store the JSON file
        public static string FilePath =>
            Path.Combine(Application.persistentDataPath, "puck.favorites.json");

        [Serializable]
        private class FavoritesDto
        {
            public int version = 2;
            public List<string> ipPorts = new List<string>();
            public List<string> names = new List<string>();
            public bool favOnly = false; // NEW
        }

        public static string Key(ServerBrowserServer s)
            => (s != null && !string.IsNullOrEmpty(s.ipAddress)) ? $"{s.ipAddress}:{s.pingPort}" : null;

        public static bool IsFavorite(ServerBrowserServer s)
        {
            if (s == null) return false;
            var k = Key(s);
            return (k != null && IpPort.Contains(k)) ||
                   (!string.IsNullOrEmpty(s.name) && Names.Contains(s.name));
        }

        public static void InitLoad()
        {
            try
            {
                IpPort.Clear(); Names.Clear();
                FavOnly = false;

                if (!File.Exists(FilePath)) return;



                var raw = File.ReadAllText(FilePath, Encoding.UTF8);
                string json;
                try { json = CryptoLite.DecryptAuto(raw); }
                catch { json = raw; } // legacy plaintext
                var dto = JsonUtility.FromJson<FavoritesDto>(json) ?? new FavoritesDto();

                foreach (var s in dto.ipPorts ?? Enumerable.Empty<string>())
                    if (!string.IsNullOrWhiteSpace(s)) IpPort.Add(s.Trim());

                foreach (var s in dto.names ?? Enumerable.Empty<string>())
                    if (!string.IsNullOrWhiteSpace(s)) Names.Add(s.Trim());

                FavOnly = dto.favOnly;                 // <-- you were missing this
            }
            catch (Exception ex)
            {
                Debug.LogError($"[YourMod] Failed to load favorites: {ex}");
            }
        }
        //testcomment
        public static void SaveNow()
        {
            try
            {
                var dto = new FavoritesDto
                {
                    version = 2,
                    ipPorts = IpPort.ToList(),
                    names = Names.ToList(),
                    favOnly = FavOnly
                };
                var json = JsonUtility.ToJson(dto, prettyPrint: true);
                var enc = CryptoLite.EncryptToBase64(json);

                var dir = Path.GetDirectoryName(FilePath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);

                File.WriteAllText(FilePath, enc, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[YourMod] Failed to save favorites: {ex}");
            }
        }
        // NEW: helper to set + save the UI flag
        public static void SetFavOnly(bool on)
        {
            FavOnly = on;
            SaveNow();
        }
        /// <summary>
        /// Toggles favorite status for this server; returns the new favorite state.
        /// </summary>
        public static bool Toggle(ServerBrowserServer s)
        {
            if (s == null) return false;

            var k = Key(s);
            var name = s.name ?? string.Empty;

            bool nowFav;
            if ((k != null && IpPort.Contains(k)) || Names.Contains(name))
            {
                if (k != null) IpPort.Remove(k);
                if (!string.IsNullOrEmpty(name)) Names.Remove(name);
                nowFav = false;
            }
            else
            {
                if (k != null) IpPort.Add(k);
                if (!string.IsNullOrEmpty(name)) Names.Add(name);   // keep as fallback
                nowFav = true;
            }

            SaveNow();                 // persist the change immediately
            return nowFav;
        }
    }
}