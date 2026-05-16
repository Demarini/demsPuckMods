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
        public static string ModDir { get; private set; }     // directory containing this mod's DLL

        private const string PrimaryName = "MOTDConfig.json";       // workshop-managed (may be overwritten)
        private const string LocalName = "MOTDConfig.local.json"; // our persistent copy (we use this)
        private const string BundledMotdName = "MOTD.json";       // bundled default MOTD content

        // Resolves the MOTD.json path the server should send. Prefers the admin-configured
        // JsonFileLocation; falls back to the bundled MOTD.json next to the DLL so the mod
        // works out of the box on a fresh install.
        public static string ResolveMotdJsonPath()
        {
            try
            {
                var configured = ConfigData.Instance?.JsonFileLocation;
                if (!string.IsNullOrEmpty(configured) && File.Exists(configured))
                    return configured;

                if (!string.IsNullOrEmpty(ModDir))
                {
                    string bundled = Path.Combine(ModDir, BundledMotdName);
                    if (File.Exists(bundled)) return bundled;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[MOTD] ResolveMotdJsonPath error: {ex.Message}");
            }
            return null;
        }

        public static void Initialize()
        {
            try
            {
                string assemblyPath = Assembly.GetExecutingAssembly().Location;
                string modDir = Path.GetDirectoryName(assemblyPath) ?? ".";
                ModDir = modDir;
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
