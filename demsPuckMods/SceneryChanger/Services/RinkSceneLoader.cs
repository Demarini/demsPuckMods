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