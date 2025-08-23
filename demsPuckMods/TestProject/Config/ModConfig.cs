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
                string assemblyPath = Assembly.GetExecutingAssembly().Location;
                Debug.Log($"[MOTD] Mod assembly path: {assemblyPath}");
                Debug.Log("-----");
                string steamLibrary = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(assemblyPath), @"..\..\..\.."));
                string gameDir = Path.Combine(steamLibrary, "common", "Puck");
                Debug.Log($"[MOTD] Resolved gameDir: {gameDir}");

                string configDir = Path.Combine(gameDir, "config");
                if (!Directory.Exists(configDir))
                {
                    Debug.Log("[MOTD] Creating config directory...");
                    Directory.CreateDirectory(configDir);
                }

                ConfigPath = Path.Combine(configDir, "MOTDConfig.json");
                Debug.Log($"[MOTD] Final ConfigPath: {ConfigPath}");

                string defaultConfig = Path.Combine(Path.GetDirectoryName(assemblyPath), "MOTDConfig.json");
                Debug.Log($"[MOTD] Looking for default config at: {defaultConfig}");

                if (!File.Exists(ConfigPath))
                {
                    if (File.Exists(defaultConfig))
                    {
                        Debug.Log("[MOTD] Copying default config to game config folder...");
                        File.Copy(defaultConfig, ConfigPath);
                    }
                    else
                    {
                        Debug.LogWarning("[MOTD] No default config found, generating new one...");
                        var defaultJson = JsonConvert.SerializeObject(new ConfigData(), Formatting.Indented);
                        File.WriteAllText(ConfigPath, defaultJson);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[MOTD] Error during ModConfig.Initialize: {ex}");
            }
        }
    }
}
