using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using PuckAIPractice.Singletons;
namespace PuckAIPractice.Config
{
    public static class ModConfig
    {
        public static string ConfigPath { get; private set; }

        public static void Initialize()
        {
            try
            {
                string assemblyPath = Assembly.GetExecutingAssembly().Location;
                Debug.Log($"[PuckAIPractice] Mod assembly path: {assemblyPath}");
                Debug.Log("-----");
                string steamLibrary = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(assemblyPath), @"..\..\..\.."));
                string gameDir = Path.Combine(steamLibrary, "common", "Puck");
                Debug.Log($"[PuckAIPractice] Resolved gameDir: {gameDir}");

                string configDir = Path.Combine(gameDir, "config");
                if (!Directory.Exists(configDir))
                {
                    Debug.Log("[PuckAIPractice] Creating config directory...");
                    Directory.CreateDirectory(configDir);
                }

                ConfigPath = Path.Combine(configDir, "PuckAIPracticeConfig.json");
                Debug.Log($"[PuckAIPractice] Final ConfigPath: {ConfigPath}");

                string defaultConfig = Path.Combine(Path.GetDirectoryName(assemblyPath), "PuckAIPracticeConfig.json");
                Debug.Log($"[PuckAIPractice] Looking for default config at: {defaultConfig}");

                if (!File.Exists(ConfigPath))
                {
                    if (File.Exists(defaultConfig))
                    {
                        Debug.Log("[PuckAIPractice] Copying default config to game config folder...");
                        File.Copy(defaultConfig, ConfigPath);
                    }
                    else
                    {
                        Debug.LogWarning("[PuckAIPractice] No default config found, generating new one...");
                        var defaultJson = JsonConvert.SerializeObject(new ConfigData(), Formatting.Indented);
                        File.WriteAllText(ConfigPath, defaultJson);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[PuckAIPractice] Error during ModConfig.Initialize: {ex}");
            }
        }
    }
}
