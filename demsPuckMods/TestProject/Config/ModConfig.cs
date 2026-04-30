using MOTD.Singletons;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace MOTD.Config
{
    public static class MOTDConfig
    {
        public static string ConfigPath { get; private set; } // <moddir>/MOTDConfig.local.json

        private const string PrimaryName = "MOTDConfig.json";       // workshop-managed (may be overwritten)
        private const string LocalName = "MOTDConfig.local.json"; // our persistent copy (we use this)

        public static void Initialize()
        {
            try
            {
                string assemblyPath = Assembly.GetExecutingAssembly().Location;
                string modDir = Path.GetDirectoryName(assemblyPath) ?? ".";
                Debug.Log($"[MOTD] Mod assembly path: {assemblyPath}");
                Debug.Log($"[MOTD] Using mod dir: {modDir}");

                string primaryInMod = Path.Combine(modDir, PrimaryName);
                string localInMod = Path.Combine(modDir, LocalName);

                // Always use the local file inside the SAME workshop/mod folder.
                ConfigPath = localInMod;
                Debug.Log($"[MOTD] Effective ConfigPath (local): {ConfigPath}");

                // One-time seed: if local missing, copy from workshop file if present; else write defaults.
                if (!File.Exists(localInMod))
                {
                    if (File.Exists(primaryInMod))
                    {
                        Debug.Log("[MOTD] Seeding local config from mod-folder MOTDConfig.json.");
                        File.Copy(primaryInMod, localInMod, overwrite: false);
                    }
                    else
                    {
                        Debug.LogWarning("[MOTD] No MOTDConfig.json next to DLL; writing defaults to local config.");
                        var defaultJson = JsonConvert.SerializeObject(new ConfigData(), Formatting.Indented);
                        File.WriteAllText(localInMod, defaultJson); // writes only; never creates directories
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[MOTD] Error during ModConfig.Initialize: {ex}");
                ConfigPath = string.Empty; // signal to skip file I/O
            }
        }
    }
}
