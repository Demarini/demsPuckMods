using UnityEngine;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace ModifyMinimapIcons
{
    public class ConfigData
    {
        private static ConfigData _instance;

        public static ConfigData Instance
        {
            get
            {
                if (_instance == null)
                    Load();
                return _instance;
            }
        }

        private static string ConfigPath => ModConfig.ConfigPath;

        public static void Load()
        {
            if (!File.Exists(ConfigPath))
            {
                // If missing, initialize (will copy from defaults)
                ModConfig.Initialize();
            }

            try
            {
                _instance = JsonConvert.DeserializeObject<ConfigData>(File.ReadAllText(ConfigPath));
                Debug.Log($"[Config] Loaded config successfully from: {ConfigPath}");
            }
            catch
            {
                Debug.LogWarning($"[Config] Config at {ConfigPath} is corrupted. Resetting to defaults.");
                _instance = new ConfigData();
                Save();
            }
        }

        public static void Save()
        {
            if (_instance == null)
                return;

            string json = JsonConvert.SerializeObject(_instance, Formatting.Indented);
            Debug.Log($"[Config] Saving config to: {ConfigPath}");
            File.WriteAllText(ConfigPath, json);
        }

        // --- Config Properties (matches JSON) ---
        public bool usePlayerColor { get; set; } = true;
        public string playerColor { get; set; } = "#FF00FF";

        public bool useTeamColor { get; set; } = false;
        public string teamColor { get; set; } = "#0000FF";

        public bool useOpponentColor { get; set; } = false;
        public string opponentColor { get; set; } = "#FF0000";

        // Fallback colors for spectators or invalid configs
        public string redTeamColor { get; set; } = "#FF0000";
        public string blueTeamColor { get; set; } = "#0000FF";

        public float playerScalingFactor { get; set; } = 1.25f;
        public float teamScalingFactor { get; set; } = 1.0f;
        public float opponentScalingFactor { get; set; } = 1.0f;

        public bool scaleWithMinimap { get; set; } = true;

        public bool pulsePlayerIcon { get; set; } = true;
        public float playerPulseSpeed { get; set; } = 4.0f;
        public float playerPulseStrength { get; set; } = 0.2f;

        public bool pulseTeamIcon { get; set; } = false;
        public float teamPulseSpeed { get; set; } = 4.0f;
        public float teamPulseStrength { get; set; } = 0.2f;

        public bool pulseOpponentIcon { get; set; } = false;
        public float opponentPulseSpeed { get; set; } = 4.0f;
        public float opponentPulseStrength { get; set; } = 0.2f;
        public bool highlightOpenTeammates { get; set; } = true;
        public string openHighlightColor { get; set; } = "#FFFF00";
        public float passBlockRadius { get; set; } = 2.5f;
        public float passBlockDistanceLimit { get; set; } = 25f;
        public bool scalePassDetectionWithMinimap { get; set; } = true;

        // --- HELPER FUNCTIONS ---
        /// <summary>
        /// Converts a hex string (e.g., "#FF00FF") to a Unity Color.
        /// Returns white if parsing fails.
        /// </summary>
        public static Color HexToColor(string hex)
        {
            if (ColorUtility.TryParseHtmlString(hex, out var color))
                return color;
            Debug.LogWarning($"[Config] Failed to parse color from hex: {hex}. Defaulting to white.");
            return Color.white;
        }
    }

    public static class ModConfig
    {
        public static string ConfigPath { get; private set; }

        public static void Initialize()
        {
            string modFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string workshopConfig = Path.Combine(modFolder, "ModifyMinimapIconsConfig.json");

            // Locate <SteamLibrary>/common/Puck
            string steamLibrary = Path.GetFullPath(Path.Combine(modFolder, @"..\..\..\.."));
            string gameDir = Path.Combine(steamLibrary, "common", "Puck");

            // Plugins folder (<Puck>/Plugins/ModifyMinimapIcons)
            string pluginsFolder = Path.Combine(gameDir, "Plugins", "ModifyMinimapIcons");
            string pluginsConfig = Path.Combine(pluginsFolder, "ModifyMinimapIconsConfig.json");

            // Final config directory (<Puck>/config)
            string configDir = Path.Combine(gameDir, "config");
            if (!Directory.Exists(configDir))
            {
                Debug.Log($"[Config] Creating config directory: {configDir}");
                Directory.CreateDirectory(configDir);
            }

            // Use the new filename everywhere
            ConfigPath = Path.Combine(configDir, "ModifyMinimapIconsConfig.json");

            Debug.Log($"[Config] Workshop folder: {modFolder}");
            Debug.Log($"[Config] Workshop config path: {workshopConfig} (exists: {File.Exists(workshopConfig)})");
            Debug.Log($"[Config] Plugins config path: {pluginsConfig} (exists: {File.Exists(pluginsConfig)})");
            Debug.Log($"[Config] Target config path: {ConfigPath}");

            // Copy config if it doesn’t already exist in <Puck>/config
            if (!File.Exists(ConfigPath))
            {
                if (File.Exists(workshopConfig))
                {
                    Debug.Log("[Config] Copying config from Workshop folder to Puck/config...");
                    File.Copy(workshopConfig, ConfigPath);
                }
                else if (File.Exists(pluginsConfig))
                {
                    Debug.Log("[Config] Copying config from Plugins folder to Puck/config...");
                    File.Copy(pluginsConfig, ConfigPath);
                }
                else
                {
                    Debug.LogWarning("[Config] No default config found. Creating a new default config.");
                    File.WriteAllText(ConfigPath, "{ \"centerOnPlayer\": true }");
                }
            }
            else
            {
                Debug.Log("[Config] Config already exists in Puck/config. Not overwriting.");
            }
        }

        public static string LoadConfig()
        {
            if (!File.Exists(ConfigPath))
                Initialize();

            Debug.Log($"[Config] Loading config from: {ConfigPath}");
            return File.ReadAllText(ConfigPath);
        }

        public static void SaveConfig(string json)
        {
            Debug.Log($"[Config] Saving config to: {ConfigPath}");
            File.WriteAllText(ConfigPath, json);
        }
    }
}
