using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using SceneryLoader.Singletons;
namespace SceneryLoader.Config
{
    public static class ModConfig
    {
        public static string ConfigPath { get; private set; } // <moddir>/SceneryLoaderConfig.local.json

        private const string PrimaryName = "SceneryLoaderConfig.json";       // workshop-managed (may be overwritten)
        private const string LocalName = "SceneryLoaderConfig.local.json"; // our persistent copy (we use this)

        public static void Initialize()
        {
            try
            {
                string assemblyPath = Assembly.GetExecutingAssembly().Location;
                string modDir = Path.GetDirectoryName(assemblyPath) ?? ".";
                Debug.Log($"[SceneryLoader] Mod assembly path: {assemblyPath}");
                Debug.Log($"[SceneryLoader] Using mod dir: {modDir}");

                string primaryInMod = Path.Combine(modDir, PrimaryName);
                string localInMod = Path.Combine(modDir, LocalName);
                // Always use the local file inside the SAME workshop/mod folder.
                ConfigPath = localInMod;
                Debug.Log($"[SceneryLoader] Effective ConfigPath (local): {ConfigPath}");

                // One-time seed: if local missing, copy from workshop file if present; else write defaults.
                if (!File.Exists(localInMod))
                {
                    if (File.Exists(primaryInMod))
                    {
                        Debug.Log("[SceneryLoader] Seeding local config from mod-folder SceneryLoaderConfig.json.");
                        File.Copy(primaryInMod, localInMod, overwrite: false);
                    }
                    else
                    {
                        Debug.LogWarning("[SceneryLoader] No SceneryLoaderConfig.json next to DLL; writing defaults to local config.");
                        var defaultJson = JsonConvert.SerializeObject(new ConfigData(), Formatting.Indented);
                        File.WriteAllText(localInMod, defaultJson); // writes only; never creates directories
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SceneryLoader] Error during ModConfig.Initialize: {ex}");
                ConfigPath = string.Empty; // signal to skip file I/O
            }
        }
    }
}
