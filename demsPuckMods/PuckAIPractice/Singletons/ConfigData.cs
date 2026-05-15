using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using PuckAIPractice.Config;
using PuckAIPractice.AI;

namespace PuckAIPractice.Singletons
{
    public class ConfigData
    {
        private static ConfigData _instance;
        private static string ConfigPath => PuckAIPractice.Config.ModConfig.ConfigPath;

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
                    PuckAIPractice.Config.ModConfig.Initialize();
                }

                string rawJson = File.ReadAllText(ConfigPath);
                Debug.Log($"[InputControl] Raw config contents: {rawJson}");

                _instance = JsonConvert.DeserializeObject<ConfigData>(rawJson);

                if (_instance == null)
                {
                    Debug.LogWarning("[InputControl] Config deserialized to null, using defaults.");
                    _instance = new ConfigData();
                }

                MigrateSchemaIfNeeded(rawJson);

                Debug.Log("[InputControl] Config loaded successfully.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[InputControl] Failed to load config: {ex}");
                _instance = new ConfigData();
                Save();
            }
        }

        // Re-saves the config if the on-disk JSON is missing any property
        // defined on ConfigData. Newtonsoft fills missing values from the C#
        // property initializers on load, but the file itself doesn't get the
        // new keys until something writes it back. Without this, users on an
        // older config wouldn't see new options like ScenarioRestartKey.
        //
        // Note: comments in the on-disk JSON are lost on rewrite, since JSON.NET
        // doesn't preserve them through a parse/serialize round-trip.
        private static void MigrateSchemaIfNeeded(string rawJson)
        {
            try
            {
                JObject loaded;
                try { loaded = JObject.Parse(rawJson); }
                catch (JsonReaderException) { return; } // malformed; leave it alone

                var existing = new HashSet<string>(
                    loaded.Properties().Select(p => p.Name),
                    StringComparer.OrdinalIgnoreCase);

                var expected = typeof(ConfigData)
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Select(p => p.Name);

                var missing = expected.Where(name => !existing.Contains(name)).ToList();
                if (missing.Count == 0) return;

                Debug.Log($"[InputControl] Config schema updated; adding {missing.Count} field(s): {string.Join(", ", missing)}");
                Save();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[InputControl] Schema migration failed: {ex}");
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
        public bool StartWithBlueGoalie { get; set; } = false;
        public bool StartWithRedGoalie { get; set; } = false;
        public GoalieDifficulty RedGoalieDefaultDifficulty { get; set; } = GoalieDifficulty.Normal;
        public GoalieDifficulty BlueGoalieDefaultDifficulty { get; set; } = GoalieDifficulty.Normal;
        public bool IsServer { get; set; } = true;

        // Uses UnityEngine.InputSystem.Key (not KeyCode) because Puck runs the
        // new Input System exclusively — legacy Input.GetKeyDown is dead in this
        // process. JSON still serializes as the enum name ("G", "Escape", etc.).
        [JsonConverter(typeof(StringEnumConverter))]
        public UnityEngine.InputSystem.Key ScenarioRestartKey { get; set; } = UnityEngine.InputSystem.Key.G;

        public bool RandomizeBotAppearance { get; set; } = true;
    }
}