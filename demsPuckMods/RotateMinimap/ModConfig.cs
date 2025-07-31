
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace RotateMinimap
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
                // If missing, let ModConfig handle initialization
                ModConfig.Initialize();
            }

            try
            {
                _instance = JsonConvert.DeserializeObject<ConfigData>(File.ReadAllText(ConfigPath));
            }
            catch
            {
                // If JSON is corrupted, reset to defaults
                _instance = new ConfigData();
                Save();
            }
        }
        public static void Save()
        {
            if (_instance == null)
                return;

            string json = JsonConvert.SerializeObject(_instance, Formatting.Indented);
            File.WriteAllText(ConfigPath, json);
        }
        public bool CenterOnPlayer { get; set; }
    }

    public static class ModConfig
    {
        public static string ConfigPath { get; private set; }

        public static void Initialize()
        {
            // Hardcode the game directory (Steam always puts it here for this title)
            string steamLibrary = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"..\..\..\.."));
            string gameDir = Path.Combine(steamLibrary, "common", "Puck");

            // Make sure the config directory exists
            string configDir = Path.Combine(gameDir, "config");
            if (!Directory.Exists(configDir))
                Directory.CreateDirectory(configDir);

            // The final config path where we will load/save settings
            ConfigPath = Path.Combine(configDir, "RotateMinimapConfig.json");

            // Default config shipped with the mod (inside Workshop folder, next to DLL)
            string defaultConfig = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "RotateMinimapConfig.json");

            // If the config doesn't exist in <Puck>/config, copy the default one
            if (!File.Exists(ConfigPath))
            {
                if (File.Exists(defaultConfig))
                {
                    File.Copy(defaultConfig, ConfigPath);
                }
                else
                {
                    // Generate a basic one if no default is present
                    File.WriteAllText(ConfigPath, "{ \"centerOnPlayer\": true }");
                }
            }
        }

        public static string LoadConfig()
        {
            if (!File.Exists(ConfigPath))
                Initialize();

            return File.ReadAllText(ConfigPath);
        }

        public static void SaveConfig(string json)
        {
            File.WriteAllText(ConfigPath, json);
        }
    }
}
