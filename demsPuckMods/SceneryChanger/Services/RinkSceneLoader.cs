using SceneryChanger.Behaviors;
using SceneryChanger.Model;
using SceneryLoader.Behaviors;
using SceneryLoader.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using SceneryLoader.Singletons;
namespace SceneryChanger.Services
{
    public sealed class SceneContentRoot : MonoBehaviour { } // tiny marker

    public static class RinkSceneState
    {
        public static GameObject Container;     // parent for all mod content
        public static GameObject ActiveRoot;    // currently shown root
        public static int CurrentLoadToken;     // monotonically increasing version
    }
    public static class RinkSceneLoader
    {
        static GameObject EnsureContainer()
        {
            if (RinkSceneState.Container) return RinkSceneState.Container;
            var go = new GameObject("SceneryLoaderContent");
            return RinkSceneState.Container = go;
        }
        // Call this from anywhere. It does everything async.
        public static void LoadSceneAsync(Scene scene, SceneInformation si)
        {
            int token = ++RinkSceneState.CurrentLoadToken;
            Debug.Log($"[SceneLoader] LoadSceneAsync token={token} bundle='{si?.bundleName}' prefab='{si?.prefabName}' skybox='{si?.skyboxName}' enc={!string.IsNullOrWhiteSpace(si?.contentKey64)}");
            CoroutineRunner.Instance.StartCoroutine(LoadSceneFlow(si, token));
        }
        static IEnumerator LoadSceneFlow(SceneInformation si, int token)
        {
            // --- Stage the new content (do NOT clear old yet) ---
            GameObject stagedRoot = null;
            bool got = false;
            RinkOnlyPruner.RemoveHangar();
            string dllDir = Path.GetDirectoryName(typeof(BundleLoader).Assembly.Location);
            bool enc = !string.IsNullOrWhiteSpace(si.contentKey64);
            var resolved = BundleResolver.Resolve(si.bundleName, dllDir, preferEncrypted: enc);
            Debug.Log($"[SceneLoader] BundleResolver: dllDir='{dllDir}' bundle='{si.bundleName}' enc={enc} -> Exists={resolved?.Exists} Path='{resolved?.BundlePath}'");
            if (resolved == null || !resolved.Exists)
            {
                Debug.LogError($"[SceneLoader] Bundle '{si.bundleName}' not found under '{dllDir}\\AssetBundles' (or sibling mod folders). Aborting load.");
                yield break;
            }
            var info = resolved != null ? resolved.Info : null;
            if (info != null)
            {
                if (info.useGlass) RemoveArena.ShowGlass();
                else RemoveArena.HideGlass();
            }
            else
            {
                // default if no file present
                RemoveArena.ShowGlass();
            }
            AudioTweaks.TryDisableAmbientAudio();
            yield return BundleLoader.InstantiatePrefabAsync(si.bundleName, si.prefabName, si.contentKey64, go =>
            {
                stagedRoot = go; got = true;
            });

            if (!got || !stagedRoot)
            {
                if (token == RinkSceneState.CurrentLoadToken)
                    Debug.LogError($"[SceneLoader] Instantiate failed for '{si.prefabName}' from '{si.bundleName}'.");
                yield break;
            }

            // If this load was superseded while we were loading, just discard it
            if (token != RinkSceneState.CurrentLoadToken)
            {
                UnityEngine.Object.Destroy(stagedRoot);
                yield break;
            }

            // Parent staged under our container
            var container = EnsureContainer();
            stagedRoot.transform.SetParent(container.transform, true);

            DumpStagedRoot(stagedRoot);

            // --- Apply skybox for *this token* only ---
            if (!string.IsNullOrEmpty(si.skyboxName))
            {
                // You can just yield this; it's quick
                bool skyDone = false;
                CoroutineRunner.Instance.StartCoroutine(SkyboxLoader.ApplySkyboxFromBundleAsync(si.bundleName, si.skyboxName, si.contentKey64));
                // If you want to hard-block until applied, convert to a yield pattern instead.
            }
            foreach (var cam in UnityEngine.Object.FindObjectsByType<Camera>(FindObjectsSortMode.None))
                cam.clearFlags = CameraClearFlags.Skybox;
            DynamicGI.UpdateEnvironment();

            // Final token check before we swap visible content
            if (token != RinkSceneState.CurrentLoadToken)
            {
                UnityEngine.Object.Destroy(stagedRoot);
                yield break;
            }

            // --- Swap: destroy previous ActiveRoot, then make staged the ActiveRoot ---
            var old = RinkSceneState.ActiveRoot;
            RinkSceneState.ActiveRoot = stagedRoot;

            if (old) UnityEngine.Object.Destroy(old); // destroy AFTER new is ready to avoid blank frames
            yield return null; // let Destroy settle

            // --- Spectators: wait for locations then spawn (guarded by token) ---
            yield return SpawnSpectatorsWhenLocationsReady(token, 8f);
            
            // Optional cleanup of stray legacy objects AFTER swap
            //ClearLegacyOutsideContainer(); // see helper below (non-blocking)
        }
        static void DumpStagedRoot(GameObject root)
        {
            try
            {
                var t = root.transform;
                var renderers = root.GetComponentsInChildren<Renderer>(true);
                int enabledRenderers = 0;
                Bounds combined = default;
                bool first = true;
                int magentaShaderCount = 0;
                int nullMatCount = 0;
                foreach (var r in renderers)
                {
                    if (r.enabled && r.gameObject.activeInHierarchy) enabledRenderers++;
                    if (first) { combined = r.bounds; first = false; }
                    else combined.Encapsulate(r.bounds);
                    foreach (var m in r.sharedMaterials)
                    {
                        if (m == null) { nullMatCount++; continue; }
                        var sh = m.shader;
                        if (sh == null || sh.name == "Hidden/InternalErrorShader" || sh.name.StartsWith("Hidden/"))
                            magentaShaderCount++;
                    }
                }
                Debug.Log($"[SceneLoader] Staged root '{root.name}' active={root.activeSelf}/{root.activeInHierarchy} pos={t.position} scale={t.lossyScale} renderers={renderers.Length} enabled={enabledRenderers} bounds={(first ? "n/a" : combined.ToString())} nullMats={nullMatCount} brokenShaders={magentaShaderCount}");
                Debug.Log($"[SceneLoader] Container '{root.transform.parent?.name}' active={root.transform.parent?.gameObject.activeInHierarchy} scene='{root.scene.name}'");

                // dump the first few children for shape
                int childDump = 0;
                foreach (Transform c in t)
                {
                    if (childDump++ >= 8) break;
                    var rends = c.GetComponentsInChildren<Renderer>(true).Length;
                    Debug.Log($"[SceneLoader]   child[{childDump - 1}] '{c.name}' active={c.gameObject.activeInHierarchy} pos={c.position} scale={c.lossyScale} rendersInTree={rends}");
                }

                // sample one material's shader so we can tell what to compare against
                var sample = renderers.FirstOrDefault(r => r.sharedMaterial != null);
                if (sample != null)
                {
                    var m = sample.sharedMaterial;
                    Debug.Log($"[SceneLoader] Sample material on '{sample.name}': mat='{m.name}' shader='{m.shader?.name}' supported={(m.shader != null && m.shader.isSupported)}");
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[SceneLoader] DumpStagedRoot failed: {e.Message}");
            }
        }

        static IEnumerator SpawnSpectatorsWhenLocationsReady(int token, float timeoutSec)
        {
            float t0 = Time.realtimeSinceStartup;
            Transform locations = null;

            while (token == RinkSceneState.CurrentLoadToken &&
                   locations == null &&
                   Time.realtimeSinceStartup - t0 < timeoutSec)
            {
                // robust search including inactive:
                var all = Resources.FindObjectsOfTypeAll<Transform>();
                locations = all.FirstOrDefault(t => t && t.name == "SpectatorLocations");
                if (!locations) yield return null;
            }

            if (token != RinkSceneState.CurrentLoadToken) yield break; // superseded
            if (!locations)
            {
                Debug.LogWarning("[SceneLoader] Timed out waiting for SpectatorLocations; skipping spectators.");
                yield break;
            }

            yield return null; // let children finish Awake/Start

            // Trigger your Harmony prefix path safely
            RinkOnlyPruner.ReapplySpectators = true;
            var mgr = SpectatorManager.Instance;
            if (!mgr) { Debug.LogWarning("[SceneLoader] SpectatorManager missing."); yield break; }

            try
            {
                // Another guard just before spawning
                Debug.Log("Trying to spawn custom specatators");
                if (token != RinkSceneState.CurrentLoadToken) yield break;
                mgr.SpawnSpectators(); // Prefix will do the custom placement
            }
            finally
            {
                RinkOnlyPruner.ReapplySpectators = false;
            }
        }
        static void ClearLegacyOutsideContainer()
        {
            DestroyIfExistsByName("SpectatorList");
            DestroyIfExistsByName("SpectatorLocations"); // default markers                                                   // add others you know about (e.g., “HangarRoot”) if RemoveHangar doesn’t catch them
        }

        static void DestroyIfExistsByName(string name)
        {
            var all = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (var go in all)
                if (go && go.name == name && go.scene.IsValid())
                    UnityEngine.Object.Destroy(go);
        }
    }
}