using Newtonsoft.Json;
using SceneryChanger.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SceneryChanger.Services
{
    public static class BundleResolver
    {
        public static BundleResolveResult Resolve(string bundleName, string dllDir, bool preferEncrypted)
        {
            var res = new BundleResolveResult();
            if (string.IsNullOrWhiteSpace(bundleName) || string.IsNullOrWhiteSpace(dllDir)) return res;

            // Folders to search:
            // 1) <dllDir>/AssetBundles
            // 2) <dllDir>/../*/AssetBundles
            var folders = new List<string>();
            var localAB = Path.Combine(dllDir, "AssetBundles");
            if (Directory.Exists(localAB)) folders.Add(localAB);

            var parent = Directory.GetParent(dllDir)?.FullName;
            if (!string.IsNullOrEmpty(parent) && Directory.Exists(parent))
            {
                foreach (var child in SafeGetDirectories(parent))
                {
                    var abFolder = Path.Combine(child, "AssetBundles");
                    if (Directory.Exists(abFolder)) folders.Add(abFolder);
                }
            }

            // Build candidate names in *preference order*
            string baseName = Path.GetFileNameWithoutExtension(bundleName);
            bool hasExt = !string.IsNullOrEmpty(Path.GetExtension(bundleName));

            string[] prefList;
            if (hasExt)
            {
                // Exact name only (caller provided extension)
                prefList = new[] { bundleName };
            }
            else if (preferEncrypted)
            {
                prefList = new[] { baseName + ".abx", baseName + ".unity3d", baseName };
            }
            else
            {
                prefList = new[] { baseName, baseName + ".unity3d", baseName + ".abx" };
            }

            // Pass 1: honor preference strictly (e.g., find .abx when preferEncrypted)
            foreach (var folder in folders)
            {
                foreach (var name in prefList)
                {
                    var full = Path.Combine(folder, name);
                    if (File.Exists(full))
                    {
                        // If preferEncrypted and we matched raw, skip here (strict pass)
                        if (preferEncrypted && !full.EndsWith(".abx", StringComparison.OrdinalIgnoreCase))
                            continue;

                        res.Exists = true;
                        res.FolderPath = folder;
                        res.BundlePath = full;
                        res.BundleFileName = Path.GetFileName(full);
                        res.Info = TryLoadAssetInformation(folder);
                        return res;
                    }
                }
            }

            // Pass 2 (optional fallback): if we *didn’t* find encrypted but you still want *any* match
            // -> Return first available (raw). For ENC mode we usually don't want this fallback; comment out if undesired.
            if (!preferEncrypted)
            {
                foreach (var folder in folders)
                {
                    foreach (var name in prefList)
                    {
                        var full = Path.Combine(folder, name);
                        if (File.Exists(full))
                        {
                            res.Exists = true;
                            res.FolderPath = folder;
                            res.BundlePath = full;
                            res.BundleFileName = Path.GetFileName(full);
                            res.Info = TryLoadAssetInformation(folder);
                            return res;
                        }
                    }
                }
            }

            return res;
        }

        static AssetInformation TryLoadAssetInformation(string folder)
        {
            try
            {
                var jsonPath = Path.Combine(folder, "AssetInformation.json");
                if (!File.Exists(jsonPath)) return null;
                var json = File.ReadAllText(jsonPath);
                if (string.IsNullOrWhiteSpace(json)) return null;

                try { return Newtonsoft.Json.JsonConvert.DeserializeObject<AssetInformation>(json); }
                catch { return JsonUtility.FromJson<AssetInformation>(json); }
            }
            catch { return null; }
        }

        static string[] SafeGetDirectories(string dir) { try { return Directory.GetDirectories(dir); } catch { return Array.Empty<string>(); } }
    }
}