using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SceneryChanger.Services
{
    public static class KeyParser
    {
        public static bool TryParseKey(string key, out byte[] key32)
        {
            key32 = null;
            if (string.IsNullOrWhiteSpace(key)) return false;

            var s = key.Trim();

            // HEX? (64 hex chars)
            if (s.Length == 64 && IsHex(s))
            {
                key32 = HexToBytes(s);
                return key32 != null && key32.Length == 32;
            }

            // Base64?
            try
            {
                key32 = Convert.FromBase64String(s);
                return key32.Length == 32;
            }
            catch { return false; }
        }

        static bool IsHex(string s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                bool hex = (c >= '0' && c <= '9') ||
                           (c >= 'a' && c <= 'f') ||
                           (c >= 'A' && c <= 'F');
                if (!hex) return false;
            }
            return true;
        }

        static byte[] HexToBytes(string hex)
        {
            if (hex.Length % 2 != 0) return null;
            var bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
                bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            return bytes;
        }
    }
}
