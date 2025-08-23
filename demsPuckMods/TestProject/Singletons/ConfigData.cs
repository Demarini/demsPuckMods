using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using MOTD.Config;
namespace MOTD.Singletons
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
                Debug.Log("[InputControl] Loading config...");

                if (!File.Exists(ConfigPath))
                {
                    Debug.Log($"[InputControl] Config not found at {ConfigPath}, initializing...");
                    ModConfig.Initialize();
                }

                string rawJson = File.ReadAllText(ConfigPath);
                Debug.Log($"[InputControl] Raw config contents: {rawJson}");

                _instance = JsonConvert.DeserializeObject<ConfigData>(rawJson);

                if (_instance == null)
                {
                    Debug.LogWarning("[InputControl] Config deserialized to null, using defaults.");
                    _instance = new ConfigData();
                }

                Debug.Log("[InputControl] Config loaded successfully.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[InputControl] Failed to load config: {ex}");
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
                    Debug.LogWarning("[InputControl] Save called but _instance is null.");
                    return;
                }

                string json = JsonConvert.SerializeObject(_instance, Formatting.Indented);
                File.WriteAllText(ConfigPath, json);
                Debug.Log($"[InputControl] Saved config to {ConfigPath}.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[InputControl] Failed to save config: {ex}");
            }
        }
        public string JsonFileLocation { get; set; }
    }
}