using SceneryChanger.Behaviors;
using SceneryChanger.Services;
using SceneryLoader.Behaviors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SceneryLoader.Services
{
    public static class BundleLoader
    {
        private static readonly Dictionary<string, AssetBundle> _bundles = new Dictionary<string, AssetBundle>();

        // EXISTING sync GetOrLoad kept as-is…

        // NEW: async/coroutine version (decrypt off main thread, then LoadFromFileAsync)
        public static IEnumerator GetOrLoadAsync(string bundleName, string contentKey64, Action<AssetBundle> onReady)
        {
            // --------- resolve + version key (no yields) ----------
            string dllDir = Path.GetDirectoryName(typeof(BundleLoader).Assembly.Location);
            bool enc = !string.IsNullOrWhiteSpace(contentKey64);
            var resolved = BundleResolver.Resolve(bundleName, dllDir, preferEncrypted: enc);
            if (!resolved.Exists)
            {
                CompleteInflightEarly($"{bundleName.ToLowerInvariant()}|{(enc ? "enc" : "raw")}|missing", null, onReady);
                yield break;
            }

            byte[] key32 = null;
            if (enc)
            {
                if (!KeyParser.TryParseKey(contentKey64, out key32) || key32.Length != 32)
                { CompleteInflightEarly($"{bundleName.ToLowerInvariant()}|enc|badkey", null, onReady); yield break; }
                if (!resolved.BundlePath.EndsWith(".abx", StringComparison.OrdinalIgnoreCase))
                { CompleteInflightEarly($"{bundleName.ToLowerInvariant()}|enc|wrongfile", null, onReady); yield break; }
            }

            string version = enc
                ? FileVersionSig(resolved.BundlePath) + "|" + KeySig(key32)
                : FileVersionSig(resolved.BundlePath);

            string cacheKey = $"{bundleName.ToLowerInvariant()}|{(enc ? "enc" : "raw")}|{version}";

            // --------- inflight gate (no yields) ----------
            if (TryServeCached(cacheKey, onReady)) yield break;
            if (EnqueueIfInflight(cacheKey, onReady)) yield break; // someone else is loading this exact version

            // purge older versions of this bundle from our dict
            PurgeOurOldVersions(bundleName.ToLowerInvariant(), enc);

            // --------- now we yield; do NOT hold locks beyond here ----------
            // unload Unity’s global instance (+ deps) before loading
            //yield return ForceUnloadUnityBundle(bundleName.ToLowerInvariant(), Path.GetDirectoryName(resolved.BundlePath));

            AssetBundle ab = null;

            if (!enc)
            {
                Debug.Log($"[BundleLoader] RAW -> {resolved.BundlePath} (v={version})");
                // (optional) wait-for-stable if your source can be touched by other processes
                yield return LoadWithRetryFromFile(resolved.BundlePath, loaded => ab = loaded, attempts: 2);
            }
            else
            {
                Debug.Log($"[BundleLoader] ENC -> {resolved.BundlePath} (v={version})");
                // Prefer RAM to avoid file races
                var bytesTask = AbxCacheDecryptor.DecryptAbxToBytesOffThread(resolved.BundlePath, key32);
                while (!bytesTask.IsCompleted) yield return null;
                if (bytesTask.IsFaulted)
                {
                    Fail(cacheKey, $"Decrypt failed: {bytesTask.Exception.GetBaseException().Message}");
                    yield break;
                }

                // final safety unload (deps too), then load from RAM
                //yield return ForceUnloadUnityBundle(bundleName.ToLowerInvariant(), Path.GetDirectoryName(resolved.BundlePath));

                var req = AssetBundle.LoadFromMemoryAsync(bytesTask.Result);
                yield return req;
                ab = req.assetBundle;
            }

            if (ab == null)
            {
                Fail(cacheKey, $"Load failed for {(enc ? "ENC" : "RAW")} '{resolved.BundlePath}'");
                yield break;
            }

            // success
            _bundles[cacheKey] = ab;
            CompleteInflight(cacheKey, ab);
        }
        static bool TryServeCached(string cacheKey, Action<AssetBundle> onReady)
        {
            if (_bundles.TryGetValue(cacheKey, out var cached))
            { onReady?.Invoke(cached); return true; }
            return false;
        }

        static void PurgeOurOldVersions(string nameLower, bool enc)
        {
            var prefix = nameLower + "|" + (enc ? "enc" : "raw") + "|";
            var stale = _bundles.Keys.Where(k => k.StartsWith(prefix, StringComparison.Ordinal)).ToList();
            foreach (var k in stale) { try { _bundles[k]?.Unload(false); } catch { } _bundles.Remove(k); }
        }

        // inflight
        static readonly Dictionary<string, List<Action<AssetBundle>>> _inflight = new Dictionary<string, List<Action<AssetBundle>>>(StringComparer.Ordinal);
        static bool EnqueueIfInflight(string key, Action<AssetBundle> cb)
        {
            lock (_inflight)
            {
                List<Action<AssetBundle>> waiters;
                if (_inflight.TryGetValue(key, out waiters)) { waiters.Add(cb); return true; }
                _inflight[key] = new List<Action<AssetBundle>> { cb };
                return false;
            }
        }
        static void CompleteInflight(string key, AssetBundle ab)
        {
            List<Action<AssetBundle>> waiters;
            lock (_inflight)
            {
                if (!_inflight.TryGetValue(key, out waiters)) return;
                _inflight.Remove(key);
            }
            foreach (var w in waiters) w?.Invoke(ab);
        }
        static void CompleteInflightEarly(string key, AssetBundle ab, Action<AssetBundle> immediate)
        {
            // for early failures before we put it in inflight; just call through
            immediate?.Invoke(ab);
        }
        static void Fail(string cacheKey, string msg)
        {
            Debug.LogError("[BundleLoader] " + msg);
            CompleteInflight(cacheKey, null);
        }
        static IEnumerator WaitForStableFile(string path, float timeoutSec = 2f)
        {
            var sw = Time.realtimeSinceStartup;
            long lastLen = -1;
            int sameCount = 0;

            while (Time.realtimeSinceStartup - sw < timeoutSec)
            {
                if (!File.Exists(path)) { yield return null; continue; }
                long len = 0;
                try { len = new FileInfo(path).Length; } catch { }
                if (len > 0 && len == lastLen) { sameCount++; if (sameCount >= 2) yield break; }
                else { sameCount = 0; lastLen = len; }
                yield return null;
            }
        }

        static IEnumerator LoadWithRetryFromFile(string plainPath, Action<AssetBundle> onReady, int attempts = 3)
        {
            for (int i = 0; i < attempts; i++)
            {
                yield return WaitForStableFile(plainPath, 2f);
                var req = AssetBundle.LoadFromFileAsync(plainPath);
                yield return req;
                var ab = req.assetBundle;
                if (ab) { onReady(ab); yield break; }
                yield return null; // small backoff
            }
            onReady(null);
        }
        static string ShortHex(byte[] bytes, int take = 8)
        {
            var sb = new StringBuilder(take * 2);
            for (int i = 0; i < take && i < bytes.Length; i++) sb.Append(bytes[i].ToString("x2"));
            return sb.ToString();
        }

        static string FileVersionSig(string path)
        {
            var fi = new FileInfo(path);
            if (!fi.Exists) return "missing";
            return $"{fi.Length:x}-{fi.LastWriteTimeUtc.Ticks:x}";
        }

        static string KeySig(byte[] key32)
        {
            using (var sha = SHA256.Create())
                return ShortHex(sha.ComputeHash(key32), 8); // 16 hex chars
        }
        // NEW: async helpers using GetOrLoadAsync
        public static IEnumerator PrefabExistsAsync(string bundleName, string prefabName, string contentKey64, Action<bool> onReady)
        {
            yield return GetOrLoadAsync(bundleName, contentKey64, ab =>
            {
                if (ab == null) { onReady?.Invoke(false); return; }
                // Non-blocking asset query
                var req = ab.LoadAssetAsync<GameObject>(prefabName);
                CoroutineRunner.Instance.StartCoroutine(FinishCheck(req, onReady));

                IEnumerator FinishCheck(AssetBundleRequest r, Action<bool> cb)
                {
                    yield return r;
                    cb?.Invoke(r.asset != null);
                }
            });
        }

        public static IEnumerator InstantiatePrefabAsync(string bundleName, string prefabName, string contentKey64, Action<GameObject> onReady)
        {
            AssetBundle ab = null;
            bool got = false;

            // Wait here until the bundle is loaded (raw or enc)
            yield return GetOrLoadAsync(bundleName, contentKey64, _ab => { ab = _ab; got = true; });
            if (!got || ab == null) { onReady?.Invoke(null); yield break; }

            // Now load the prefab and wait for it in THIS coroutine
            var req = ab.LoadAssetAsync<GameObject>(prefabName);
            yield return req;

            var prefab = req.asset as GameObject;
            if (prefab == null)
            {
                Debug.LogError($"[BundleLoader] Prefab '{prefabName}' not found in bundle '{bundleName}'.");
                onReady?.Invoke(null);
                yield break;
            }

            onReady?.Invoke(UnityEngine.Object.Instantiate(prefab));
        }
        static IEnumerator ForceUnloadUnityBundle(string bundleNameLower, string bundlesFolder = null)
        {
            // 0) If we know deps via manifest, compute the closure to unload
            var namesToUnload = new HashSet<string>(StringComparer.Ordinal);
            namesToUnload.Add(bundleNameLower);

            var manifest = bundlesFolder != null ? TryLoadManifest(bundlesFolder) : null;
            if (manifest)
            {
                var stack = new Stack<string>();
                stack.Push(bundleNameLower);
                while (stack.Count > 0)
                {
                    var n = stack.Pop();
                    var deps = manifest.GetAllDependencies(n); // lowercased names
                    foreach (var d in deps)
                        if (namesToUnload.Add(d)) stack.Push(d);
                }
            }

            // 1) Unload from our dictionary (all versions)
            foreach (var name in namesToUnload)
            {
                var prRaw = name + "|raw|";
                var prEnc = name + "|enc|";
                var toRemove = _bundles.Keys.Where(k => k.StartsWith(prRaw) || k.StartsWith(prEnc)).ToList();
                foreach (var k in toRemove)
                {
                    try { _bundles[k]?.Unload(false); } catch { }
                    _bundles.Remove(k);
                }
            }

            // 2) Unload from Unity’s global cache
            var loaded = AssetBundle.GetAllLoadedAssetBundles().ToList();
            foreach (var ab in loaded)
            {
                if (ab == null) continue;
                var n = ab.name; // lowercased by Unity
                if (namesToUnload.Contains(n))
                {
                    try { ab.Unload(false); } catch { }
                }
            }

            // 3) Let Unity release memory; loop a couple frames until the target name disappears
            for (int i = 0; i < 2; i++) yield return null;
            yield return Resources.UnloadUnusedAssets();

            // 4) Assert it’s gone (optional but helpful)
            var still = AssetBundle.GetAllLoadedAssetBundles().Any(ab => ab != null && namesToUnload.Contains(ab.name));
            if (still) Debug.LogWarning("[BundleLoader] Some bundles remained loaded after unload attempt: " +
                                        string.Join(", ", AssetBundle.GetAllLoadedAssetBundles().Select(a => a.name)));
        }
        static readonly Dictionary<string, AssetBundleManifest> _manifestByFolder =
    new Dictionary<string, AssetBundleManifest>(StringComparer.OrdinalIgnoreCase);

        static AssetBundleManifest TryLoadManifest(string folder)
        {
            if (string.IsNullOrEmpty(folder)) return null;
            if (_manifestByFolder.TryGetValue(folder, out var mf)) return mf;

            var mfPath = Path.Combine(folder, Path.GetFileName(folder)); // Unity convention
            if (!File.Exists(mfPath)) return null;

            var ab = AssetBundle.LoadFromFile(mfPath);
            if (!ab) return null;
            mf = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            ab.Unload(false);

            _manifestByFolder[folder] = mf;
            return mf;
        }
    }
}
