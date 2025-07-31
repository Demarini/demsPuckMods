using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using UnityEngine;

namespace ArtificialInputDelay
{
    public class ConfigData
    {
        private static ConfigData _instance;
        private static string ConfigPath => ModConfig.ConfigPath;

        public static ConfigData Instance
        {
            get
            {
                if (_instance == null)
                    Load();
                return _instance;
            }
        }

        public static void Load()
        {
            try
            {
                Debug.Log("[DelayInputs] Loading config...");

                if (!File.Exists(ConfigPath))
                {
                    Debug.Log($"[DelayInputs] Config not found at {ConfigPath}, initializing...");
                    ModConfig.Initialize();
                }

                string rawJson = File.ReadAllText(ConfigPath);
                Debug.Log($"[DelayInputs] Raw config contents: {rawJson}");

                _instance = JsonConvert.DeserializeObject<ConfigData>(rawJson);

                if (_instance == null)
                {
                    Debug.LogWarning("[DelayInputs] Config deserialized to null, using defaults.");
                    _instance = new ConfigData();
                }

                Debug.Log("[DelayInputs] Config loaded successfully.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[DelayInputs] Failed to load config: {ex}");
                _instance = new ConfigData();
                Save();
            }
        }

        public static void Save()
        {
            try
            {
                if (_instance == null)
                {
                    Debug.LogWarning("[DelayInputs] Save called but _instance is null.");
                    return;
                }

                string json = JsonConvert.SerializeObject(_instance, Formatting.Indented);
                File.WriteAllText(ConfigPath, json);
                Debug.Log($"[DelayInputs] Saved config to {ConfigPath}.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[DelayInputs] Failed to save config: {ex}");
            }
        }

        // === Configurable Settings ===
        public float ArtificialLatencyMs { get; set; } = 150f;
        public float JitterMs { get; set; } = 20f;
        public bool OnlyInPracticeMode { get; set; } = true;
    }

    public static class ModConfig
    {
        public static string ConfigPath { get; private set; }

        public static void Initialize()
        {
            try
            {
                string assemblyPath = Assembly.GetExecutingAssembly().Location;
                Debug.Log($"[DelayInputs] Mod assembly path: {assemblyPath}");
                Debug.Log("-----");
                string steamLibrary = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(assemblyPath), @"..\..\..\.."));
                string gameDir = Path.Combine(steamLibrary, "common", "Puck");
                Debug.Log($"[DelayInputs] Resolved gameDir: {gameDir}");

                string configDir = Path.Combine(gameDir, "config");
                if (!Directory.Exists(configDir))
                {
                    Debug.Log("[DelayInputs] Creating config directory...");
                    Directory.CreateDirectory(configDir);
                }

                ConfigPath = Path.Combine(configDir, "DelayInputsConfig.json");
                Debug.Log($"[DelayInputs] Final ConfigPath: {ConfigPath}");

                string defaultConfig = Path.Combine(Path.GetDirectoryName(assemblyPath), "DelayInputsConfig.json");
                Debug.Log($"[DelayInputs] Looking for default config at: {defaultConfig}");

                if (!File.Exists(ConfigPath))
                {
                    if (File.Exists(defaultConfig))
                    {
                        Debug.Log("[DelayInputs] Copying default config to game config folder...");
                        File.Copy(defaultConfig, ConfigPath);
                    }
                    else
                    {
                        Debug.LogWarning("[DelayInputs] No default config found, generating new one...");
                        var defaultJson = JsonConvert.SerializeObject(new ConfigData(), Formatting.Indented);
                        File.WriteAllText(ConfigPath, defaultJson);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[DelayInputs] Error during ModConfig.Initialize: {ex}");
            }
        }
    }
}
