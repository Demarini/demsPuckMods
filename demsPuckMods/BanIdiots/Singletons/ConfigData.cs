using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BanIdiots.Config;
namespace BanIdiots
{
    public class ConfigData
    {
        private static ConfigData _instance;
        private static string ConfigPath => ModConfig.ConfigPath;

        public static ConfigData Instance
        {
            get { if (_instance == null) Load(); return _instance; }
        }

        public static void Load()
        {
            try
            {
                Debug.Log("[MOTD] Loading config...");

                if (string.IsNullOrEmpty(ConfigPath))
                {
                    Debug.LogWarning("[MOTD] No ConfigPath available; using in-memory defaults.");
                    _instance = new ConfigData();
                    return;
                }

                if (!File.Exists(ConfigPath))
                {
                    Debug.Log($"[MOTD] Local config missing at {ConfigPath}. Initializing...");
                    ModConfig.Initialize();

                    if (string.IsNullOrEmpty(ConfigPath) || !File.Exists(ConfigPath))
                    {
                        Debug.LogWarning("[MOTD] Still no config file; using in-memory defaults.");
                        _instance = new ConfigData();
                        return;
                    }
                }

                string rawJson = File.ReadAllText(ConfigPath);
                Debug.Log($"[MOTD] Raw config contents: {rawJson}");
                _instance = JsonConvert.DeserializeObject<ConfigData>(rawJson) ?? new ConfigData();
                Debug.Log("[MOTD] Config loaded successfully.");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[MOTD] Failed to load config: {ex}");
                _instance = new ConfigData();
            }
        }

        public static void Save()
        {
            try
            {
                if (_instance == null)
                {
                    Debug.LogWarning("[MOTD] Save called but _instance is null.");
                    return;
                }
                if (string.IsNullOrEmpty(ConfigPath))
                {
                    Debug.LogWarning("[MOTD] No ConfigPath set; skipping save.");
                    return;
                }
                // Do not create directories; only write if the file's directory already exists (it does: the mod folder).
                string json = JsonConvert.SerializeObject(_instance, Formatting.Indented);
                File.WriteAllText(ConfigPath, json);
                Debug.Log($"[MOTD] Saved config to {ConfigPath}.");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[MOTD] Failed to save config: {ex}");
            }
        }

        public string bannedWordsTextLocation { get; set; }
        public string logOutputLocation { get; set; }
    }
}
