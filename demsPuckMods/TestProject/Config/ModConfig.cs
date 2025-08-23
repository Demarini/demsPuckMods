using MOTD.Singletons;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MOTD.Config
{
    public static class ModConfig
    {
        public static string ConfigPath { get; private set; }

        public static void Initialize()
        {
            try
            {
                var dllDir = GetDllDirectory();
                Debug.Log($"[MOTD] DLL directory: {dllDir}");

                ConfigPath = Path.Combine(dllDir, "MOTDConfig.json");
                Debug.Log($"[MOTD] Using config at: {ConfigPath}");

                // Create a default config next to the DLL if missing
                if (!File.Exists(ConfigPath))
                {
                    Debug.Log("[MOTD] No config found next to DLL. Creating default MOTDConfig.json...");
                    var defaultJson = JsonConvert.SerializeObject(new ConfigData(), Formatting.Indented);
                    File.WriteAllText(ConfigPath, defaultJson);
                }

                // From here on, just read ConfigPath whenever you need it
                // var cfg = JsonConvert.DeserializeObject<ConfigData>(File.ReadAllText(ConfigPath));
            }
            catch (Exception ex)
            {
                Debug.LogError($"[MOTD] Initialize failed: {ex}");
            }
        }

        private static string GetDllDirectory()
        {
            try
            {
                var asm = typeof(ModConfig).Assembly;               // any type from your mod
                var loc = asm.Location;
                if (!string.IsNullOrEmpty(loc))
                    return Path.GetDirectoryName(loc);

                // Fallback (Location can be empty on IL2CPP)
#pragma warning disable SYSLIB0012
                var codeBase = asm.CodeBase;
#pragma warning restore SYSLIB0012
                if (!string.IsNullOrEmpty(codeBase))
                    return Path.GetDirectoryName(new Uri(codeBase).LocalPath);
            }
            catch { /* fall through */ }

            // Last-ditch fallback: game base directory (may not be the plugin folder)
            var baseDir = AppContext.BaseDirectory ?? AppDomain.CurrentDomain.BaseDirectory;
            return baseDir?.TrimEnd(Path.DirectorySeparatorChar) ?? Directory.GetCurrentDirectory();
        }
    }
}
