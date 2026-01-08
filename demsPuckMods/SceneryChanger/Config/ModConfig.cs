using Newtonsoft.Json;
using SceneryLoader.Singletons;
using System;
using System.IO;
using System.Reflection;
using Unity.Netcode;
using UnityEngine;

namespace SceneryLoader.Config
{
    public static class ModConfig
    {
        public static string ConfigPath { get; private set; }

        private const string PrimaryName = "SceneryLoaderConfig.json";
        private const string LocalName = "SceneryLoaderConfig.local.json";
        private const string PathName = "SceneryLoaderConfig.path.json";

        private sealed class ConfigPathData
        {
            public string configDirectory;
        }

        /// <summary>
        /// Original behavior: computes ConfigPath and seeds the .local if missing.
        /// </summary>
        public static void Initialize()
        {
            Refresh(seedIfMissing: true);
        }

        /// <summary>
        /// Re-reads the .path file and updates ConfigPath accordingly.
        /// Safe to call frequently (e.g., before each important event).
        /// </summary>
        public static void Refresh(bool seedIfMissing)
        {
            try
            {
                string assemblyPath = Assembly.GetExecutingAssembly().Location;
                string modDir = Path.GetDirectoryName(assemblyPath) ?? ".";
                Debug.Log($"[SceneryLoader] Mod assembly path: {assemblyPath}");
                Debug.Log($"[SceneryLoader] Using mod dir: {modDir}");

                string primaryInMod = Path.Combine(modDir, PrimaryName);

                // Determine stable root (server or local user)
                string serverRoot = ResolveServerRoot(modDir);
                Debug.Log($"[SceneryLoader] Server root: {serverRoot}");

                string defaultConfigDir = Path.Combine(serverRoot, "Config", "SceneryLoader");
                Directory.CreateDirectory(defaultConfigDir);

                string pathFile = Path.Combine(defaultConfigDir, PathName);

                // IMPORTANT: Always re-read pathFile to get the current directory
                string resolvedConfigDir = ResolveConfigDirectory(pathFile, defaultConfigDir, serverRoot);

                Directory.CreateDirectory(resolvedConfigDir);
                string localConfigPath = Path.Combine(resolvedConfigDir, LocalName);

                if (!string.Equals(ConfigPath, localConfigPath, StringComparison.OrdinalIgnoreCase))
                {
                    Debug.Log($"[SceneryLoader] ConfigPath updated: '{ConfigPath}' -> '{localConfigPath}'");
                }

                ConfigPath = localConfigPath;

                Debug.Log($"[SceneryLoader] Pointer file: {pathFile}");
                Debug.Log($"[SceneryLoader] Resolved config dir: {resolvedConfigDir}");
                Debug.Log($"[SceneryLoader] Effective ConfigPath (local): {ConfigPath}");
                Debug.Log($"[SceneryLoader] Local exists before seed? {File.Exists(localConfigPath)}");

                if (seedIfMissing && !File.Exists(localConfigPath))
                {
                    if (File.Exists(primaryInMod))
                    {
                        Debug.Log("[SceneryLoader] Seeding local config from mod-folder SceneryLoaderConfig.json.");
                        File.Copy(primaryInMod, localConfigPath, overwrite: false);
                    }
                    else
                    {
                        Debug.LogWarning("[SceneryLoader] No SceneryLoaderConfig.json next to DLL; writing defaults to local config.");
                        var defaultJson = JsonConvert.SerializeObject(new ConfigData(), Formatting.Indented);
                        File.WriteAllText(localConfigPath, defaultJson);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SceneryLoader] Error during ModConfig.Refresh: {ex}");
                ConfigPath = string.Empty;
            }
        }

        private static string ResolveServerRoot(string modDir)
        {
            // 1) Most specific: "<root>/Plugins"
            var pluginsRoot = FindParentOfDirectoryNamed(modDir, "Plugins");
            if (!string.IsNullOrEmpty(pluginsRoot))
                return pluginsRoot;

            // 2) If not under steamapps, just use modDir
            var steamappsOwner = FindParentOfDirectoryNamed(modDir, "steamapps");
            if (string.IsNullOrEmpty(steamappsOwner))
                return modDir;

            // 3) We're under steamapps — decide intent
            bool isServer = NetworkManager.Singleton?.IsServer == true;
            bool isClient = NetworkManager.Singleton?.IsClient == true;

            bool dedicatedServer = isServer && !isClient;

            // Dedicated server: use the server instance root
            if (dedicatedServer)
                return steamappsOwner;

            // Client, host, practice mode: use actual running game folder
            try
            {
                var dataPath = Application.dataPath;
                var gameRoot = Directory.GetParent(dataPath)?.FullName;
                if (!string.IsNullOrEmpty(gameRoot))
                    return gameRoot;
            }
            catch { }

            // Fallback
            return steamappsOwner;
        }

        private static string ResolveConfigDirectory(string pathFile, string defaultConfigDir, string serverRoot)
        {
            ConfigPathData data = null;

            if (File.Exists(pathFile))
            {
                try
                {
                    data = JsonConvert.DeserializeObject<ConfigPathData>(File.ReadAllText(pathFile));
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[SceneryLoader] Failed to parse {pathFile}, will rewrite default. {e.Message}");
                }
            }

            if (data == null)
            {
                // Blank = "use defaultConfigDir"
                data = new ConfigPathData { configDirectory = "" };
                File.WriteAllText(pathFile, JsonConvert.SerializeObject(data, Formatting.Indented));
            }

            var raw = data.configDirectory?.Trim();

            if (string.IsNullOrEmpty(raw))
                return defaultConfigDir;

            // Relative paths are relative to serverRoot for portability
            if (!Path.IsPathRooted(raw))
                return Path.GetFullPath(Path.Combine(serverRoot, raw));

            return raw;
        }

        private static string FindParentOfDirectoryNamed(string startDir, string targetName)
        {
            try
            {
                var dir = new DirectoryInfo(startDir);
                while (dir != null)
                {
                    if (dir.Name.Equals(targetName, StringComparison.OrdinalIgnoreCase))
                        return dir.Parent?.FullName;

                    dir = dir.Parent;
                }
            }
            catch { }

            return null;
        }
    }
}
