using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BanIdiots.Patches
{
    public static class BundleLoader
    {
        private static AssetBundle _bundle;

        public static AssetBundle GetOrLoad(string bundleName)
        {
            // If we already cached it, return immediately
            if (_bundle != null) return _bundle;

            // Check if it's already loaded by Unity
            var existing = AssetBundle.GetAllLoadedAssetBundles()
                                      .FirstOrDefault(ab => ab.name == bundleName.ToLowerInvariant());
            if (existing != null)
            {
                Debug.Log($"[BundleLoader] Using already loaded bundle: {existing.name}");
                _bundle = existing;
                return _bundle;
            }

            // Otherwise load it from disk
            string dllDir = Path.GetDirectoryName(typeof(BundleLoader).Assembly.Location);
            string path = Path.Combine(dllDir, "AssetBundles", bundleName);

            Debug.Log($"[BundleLoader] Loading from {path}");
            _bundle = AssetBundle.LoadFromFile(path);

            if (_bundle == null)
                Debug.LogError($"[BundleLoader] Failed to load AssetBundle at {path}");
            else
                Debug.Log($"[BundleLoader] Loaded {_bundle.name}");

            return _bundle;
        }

        public static GameObject InstantiatePrefab(string bundleName, string prefabName)
        {
            var ab = GetOrLoad(bundleName);
            if (ab == null) return null;

            var prefab = ab.LoadAsset<GameObject>(prefabName);
            if (prefab == null)
            {
                Debug.LogError($"[BundleLoader] Prefab '{prefabName}' not found in bundle '{bundleName}'");
                return null;
            }

            return UnityEngine.Object.Instantiate(prefab);
        }
    }  
}
