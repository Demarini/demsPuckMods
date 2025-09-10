using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using SceneryLoader.Config;
using SceneryChanger.Model;
namespace SceneryLoader.Singletons
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
                Debug.Log("[SceneryLoader] Loading config...");

                if (string.IsNullOrEmpty(ConfigPath))
                {
                    Debug.LogWarning("[SceneryLoader] No ConfigPath available; using in-memory defaults.");
                    _instance = new ConfigData();
                    return;
                }

                if (!File.Exists(ConfigPath))
                {
                    Debug.Log($"[SceneryLoader] Local config missing at {ConfigPath}. Initializing...");
                    ModConfig.Initialize();

                    if (string.IsNullOrEmpty(ConfigPath) || !File.Exists(ConfigPath))
                    {
                        Debug.LogWarning("[SceneryLoader] Still no config file; using in-memory defaults.");
                        _instance = new ConfigData();
                        return;
                    }
                }

                string rawJson = File.ReadAllText(ConfigPath);
                Debug.Log($"[SceneryLoader] Raw config contents: {rawJson}");
                _instance = JsonConvert.DeserializeObject<ConfigData>(rawJson) ?? new ConfigData();
                Debug.Log("[SceneryLoader] Config loaded successfully.");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[SceneryLoader] Failed to load config: {ex}");
                _instance = new ConfigData();
            }
        }

        public static void Save()
        {
            try
            {
                if (_instance == null)
                {
                    Debug.LogWarning("[SceneryLoader] Save called but _instance is null.");
                    return;
                }
                if (string.IsNullOrEmpty(ConfigPath))
                {
                    Debug.LogWarning("[SceneryLoader] No ConfigPath set; skipping save.");
                    return;
                }
                // Do not create directories; only write if the file's directory already exists (it does: the mod folder).
                string json = JsonConvert.SerializeObject(_instance, Formatting.Indented);
                File.WriteAllText(ConfigPath, json);
                Debug.Log($"[SceneryLoader] Saved config to {ConfigPath}.");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[SceneryLoader] Failed to save config: {ex}");
            }
        }
        public SceneInformation sceneInformation;
    }
}
