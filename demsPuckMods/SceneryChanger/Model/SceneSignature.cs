using SceneryChanger.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SceneryChanger.Model
{
    public sealed class SceneSignature : IEquatable<SceneSignature>
    {
        public readonly string Bundle;     // logical bundle name, lower
        public readonly string Prefab;     // exact prefab logical name
        public readonly string Skybox;     // exact skybox material logical name
        public readonly string KeySig;     // short hash of key (or "" if none)

        public SceneSignature(SceneInformation si)
        {
            Bundle = (si?.bundleName ?? "").Trim().ToLowerInvariant();
            Prefab = (si?.prefabName ?? "").Trim();
            Skybox = (si?.skyboxName ?? "").Trim();
            KeySig = ShortHash(si?.contentKey64); // hex of SHA256(key bytes), 8–12 chars
        }

        static string ShortHash(string key64)
        {
            if (string.IsNullOrWhiteSpace(key64)) return "";
            try
            {
                // your existing KeyParser.TryParseKey(...) returns 32 bytes
                if (!KeyParser.TryParseKey(key64, out var key) || key == null) return "badkey";
                using (var sha = System.Security.Cryptography.SHA256.Create())
                {
                    var h = sha.ComputeHash(key);
                    var sb = new System.Text.StringBuilder(16);
                    for (int i = 0; i < 8; i++) sb.Append(h[i].ToString("x2"));
                    return sb.ToString();
                }
            }
            catch { return "badkey"; }
        }

        public bool Equals(SceneSignature o)
            => o != null && Bundle == o.Bundle && Prefab == o.Prefab && Skybox == o.Skybox && KeySig == o.KeySig;

        public override bool Equals(object obj) => Equals(obj as SceneSignature);
        public override int GetHashCode() => (Bundle, Prefab, Skybox, KeySig).GetHashCode();
        public override string ToString() => $"{Bundle}|{Prefab}|{Skybox}|{KeySig}";
    }
}
