using HarmonyLib;
using SceneryChanger.Model;
using SceneryChanger.Patches;
using SceneryChanger.Services;
using SceneryLoader.Services;
using SceneryLoader.Singletons;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Splines;
using SM = UnityEngine.SceneManagement;
namespace SceneryLoader.Behaviors
{
    [DefaultExecutionOrder(-10000)]
    public class RinkOnlyPruner : MonoBehaviour
    {
        public static bool SceneIsLoaded { get; set; }
        public static bool ReapplySpectators { get; set; }
        public static Scene scene { get; set; }
        private static string[] namesOfOutdoorObjects = new[]
    {
        "hangar",
        "Rafter",
        "Rafter Edge",

        "Doors",
         "Light Row",
         "Light Row.001",
         "Light Row.002",
         "Light Row.003",
        "Small Roof Rafters",
        "Small Side Rafters",
        "Window Borders",
        "Windows",

        "Side Rafter Ties",
        "Hangar"
    };
        [Tooltip("Set false to only disable visuals/colliders/audio (no destroy).")]
        public bool hardNuke = true;

        [Tooltip("Scene name to target.")]
        public string targetScene = "level_1";

        public int passes = 6;
        public float passInterval = 0.2f;

        static readonly Regex[] KeepPatterns = new[]
        {
            new Regex(@"^Level$", RegexOptions.IgnoreCase),
            new Regex(@"^Rink$", RegexOptions.IgnoreCase),
            new Regex(@"^Goal\s+(Blue|Red)$", RegexOptions.IgnoreCase),
            new Regex(@"^Team\s+(Blue|Red)\s+Positions$", RegexOptions.IgnoreCase),
            new Regex(@"^Puck\s+Shooter$", RegexOptions.IgnoreCase),
            new Regex(@"^Puck\s+Position$", RegexOptions.IgnoreCase),
            new Regex(@"^Warmup\s+Puck\s+Positions$", RegexOptions.IgnoreCase),
            new Regex(@".*Camera$", RegexOptions.IgnoreCase), // keep all cameras
            new Regex(@"^Puck", RegexOptions.IgnoreCase),
            new Regex(@"^Puck(\(Clone\))?$", RegexOptions.IgnoreCase),
        };

        static readonly Regex[] NukePatterns = new[]
        {
            new Regex(@"^Hangar$", RegexOptions.IgnoreCase),
            new Regex(@"^Sounds$", RegexOptions.IgnoreCase),
            new Regex(@"^Lights$", RegexOptions.IgnoreCase),
            new Regex(@"^Goal\s+Lights$", RegexOptions.IgnoreCase),
            new Regex(@"^Reflection\s+Probe$", RegexOptions.IgnoreCase),
            new Regex(@"^Spectator\s+Camera\s+Spline$", RegexOptions.IgnoreCase),
            new Regex(@"^Spectator\s+Booth\s+\d+$", RegexOptions.IgnoreCase),
            new Regex(@"^Scoreboard(\s*\(\d+\))?$", RegexOptions.IgnoreCase),
        };

        static RinkOnlyPruner _instance;

        // --- Self-install even if no GameObject exists in scene ---
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void Install()
        {
            if (_instance != null) return;
            var go = new GameObject("~RinkOnlyPruner(Auto)");
            _instance = go.AddComponent<RinkOnlyPruner>();
            UnityEngine.Object.DontDestroyOnLoad(go);

            // Subscribe using fully qualified Unity SceneManager (aliased as SM)
            SM.SceneManager.sceneLoaded -= _instance.OnSceneLoaded;
            SM.SceneManager.sceneLoaded += _instance.OnSceneLoaded;
            SM.SceneManager.activeSceneChanged -= _instance.OnActiveSceneChanged;
            SM.SceneManager.activeSceneChanged += _instance.OnActiveSceneChanged;

            // Try pruning the currently active scene immediately
            //_instance.TryPruneAllLoadedScenes("Boot");
            Debug.Log("[RinkOnlyPruner] Installed; initial prune attempted.");
        }
        public static void Uninstall()
        {
            // Subscribe using fully qualified Unity SceneManager (aliased as SM)
            SM.SceneManager.sceneLoaded -= _instance.OnSceneLoaded;
            SM.SceneManager.activeSceneChanged -= _instance.OnActiveSceneChanged;
            _instance = null;
        }
        void OnDestroy()
        {
            if (_instance == this)
            {
                SM.SceneManager.sceneLoaded -= OnSceneLoaded;
                SM.SceneManager.activeSceneChanged -= OnActiveSceneChanged;
            }
        }

        // Extra safety: if this script is manually added to a GO in-scene
        void Awake()
        {
            if (_instance == null) _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // Hotkey to manually test
        void Update()
        {
        }
        public static IEnumerator LoadEncryptedBundleCoroutine(string abxPath, string contentKey64, Action<AssetBundle> onReady)
        {
            if (string.IsNullOrWhiteSpace(contentKey64)) throw new ArgumentException("contentKey64 missing");
            byte[] key32;
            if (!KeyParser.TryParseKey(contentKey64, out key32))
            {
                Debug.LogError("[ABX] Bad key format");
                onReady?.Invoke(null);
                yield break;
            }

            // run decrypt + cache off the main thread
            var task = AbxCacheDecryptor.EnsureDecryptedBundleAsync(abxPath, key32);
            while (!task.IsCompleted) yield return null;
            if (task.IsFaulted) { Debug.LogException(task.Exception); onReady?.Invoke(null); yield break; }

            string cachePath = task.Result;

            // Now stream in on main thread (non-blocking) 
            var req = AssetBundle.LoadFromFileAsync(cachePath);
            yield return req;
            onReady?.Invoke(req.assetBundle);
        }
        // --- Event handlers (Unity's, fully qualified via alias) ---
        void OnSceneLoaded(SM.Scene scene, SM.LoadSceneMode mode)
        {
            Debug.Log("Scene Loaded!");

            if (scene.name.Equals("level_1", StringComparison.OrdinalIgnoreCase))
            {
                //SceneDumper.DumpActiveSceneHierarchy(scene);
                ConfigData.Load();
                SceneLoadCoordinator.OnSceneLoaded(scene, PracticeModeDetector.IsPracticeMode, ConfigData.Instance.sceneInformation);
            }
            else
            {
                SceneIsLoaded = false;
                UIChatControllerPatch.sceneInformation = null;
            }
        }
        public static IEnumerator SpawnSpectatorsWhenLocationsReady(float timeoutSec = 10f)
        {
            float t0 = Time.realtimeSinceStartup;
            GameObject locations = null;

            // If your Unity version supports it and you have a SpectatorLocations component, prefer that:
            // var marker = UnityEngine.Object.FindFirstObjectByType<SpectatorLocations>(FindObjectsInactive.Include);
            // while (marker == null && Time.realtimeSinceStartup - t0 < timeoutSec) { yield return null; marker = ...; }

            // Name-based fallback (works across versions)
            while (locations == null && Time.realtimeSinceStartup - t0 < timeoutSec)
            {
                // include inactive objects if needed:
                locations = GameObject.Find("SpectatorLocations");
                yield return null; // give a frame for Awake/Start on the loaded root
            }

            if (locations == null)
            {
                Debug.LogWarning("[SceneLoader] Timed out waiting for 'SpectatorLocations'. Skipping spectator spawn.");
                yield break;
            }

            // One more frame so children under SpectatorLocations finish activating
            yield return null;

            var mgr = SpectatorManager.Instance;
            if (mgr == null)
            {
                Debug.LogWarning("[SceneLoader] SpectatorManager.Instance not found. Skipping spawn.");
                yield break;
            }

            try
            {
                mgr.SpawnSpectators();
                Debug.Log("[SceneLoader] Spectators spawned.");
            }
            catch (Exception e)
            {
                Debug.LogError($"[SceneLoader] SpawnSpectators threw: {e}");
            }
        }
        public static void DestroyByName(string name)
        {
            var all = Resources.FindObjectsOfTypeAll<GameObject>(); // includes inactive
            foreach (var go in all)
                if (go && go.name == name && go.scene.IsValid())
                    UnityEngine.Object.Destroy(go);
        }
        public static void RemoveHangar()
        {
            RemoveArena.PopulateGameObjectLists();
            RemoveArena.HideArena();
            RemoveArena.HideEverythingExceptRink(scene);
            RemoveArena.TryPruneScene(scene, "SCENE LOADED");
            ReflectionKiller.NukeAllReflections();
            //RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            //RenderSettings.ambientLight = Color.black;
            RenderSettings.skybox = null;   // optional: kills visible skybox if it’s contributing
                                            //LightingNullifier.KillAllSceneLighting(scene);
            foreach (var light in GameObject.FindObjectsOfType<Light>(true))
            {
                if (light.enabled)
                {
                    //      Debug.Log($"[LightKiller] Disabling light {light.name} on {light.transform.parent?.name}");
                    //      Debug.Log($"[LightKiller] {light.transform} " +
                    //$"type={light.type}, intensity={light.intensity}, range={light.range}, shadows={light.shadows}");
                    //light.enabled = false;
                    light.intensity = light.intensity;
                }
            }
        }
        //public static void LoadScene(SM.Scene scene, string assetBundle, string prefab, string skyBox, string contentKey64)
        //{
        //    Debug.Log($"[RinkOnlyPruner] sceneLoaded '{scene.name}' index={scene.buildIndex}");
        //    ReapplySpectators = true;
        //    RemoveArena.PopulateGameObjectLists();
        //    RemoveArena.HideArena();
        //    RemoveArena.HideEverythingExceptRink(scene);
        //    RemoveArena.TryPruneScene(scene, "SCENE LOADED");
        //    ReflectionKiller.NukeAllReflections();
        //    //RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        //    //RenderSettings.ambientLight = Color.black;
        //    RenderSettings.skybox = null;   // optional: kills visible skybox if it’s contributing
        //                                    //LightingNullifier.KillAllSceneLighting(scene);
        //    foreach (var light in GameObject.FindObjectsOfType<Light>(true))
        //    {
        //        if (light.enabled)
        //        {
        //            Debug.Log($"[LightKiller] Disabling light {light.name} on {light.transform.parent?.name}");
        //            Debug.Log($"[LightKiller] {light.transform} " +
        //      $"type={light.type}, intensity={light.intensity}, range={light.range}, shadows={light.shadows}");
        //            //light.enabled = false;
        //        }
        //    }
        //    //LightmapSettings.lightmaps = new LightmapData[0];
        //    //LightmapSettings.lightProbes = null;
        //    //ReflectionProbeKiller.RemoveReflectionProbes(scene);
        //    BundleLoader.InstantiatePrefab(assetBundle, prefab, contentKey64);
        //    SkyboxLoader.ApplySkyboxFromBundle(assetBundle, skyBox, contentKey64);
        //    SpectatorManager.Instance.SpawnSpectators();
        //    AudioTweaks.MuteAmbient(scene);
        //    foreach (var light in GameObject.FindObjectsOfType<Light>(true))
        //    {
        //        if (light.enabled)
        //        {
        //            Debug.Log($"[LightKiller] Disabling light {light.name} on {light.transform.parent?.name}");
        //            Debug.Log($"[LightKiller] {light.transform} " +
        //      $"type={light.type}, intensity={light.intensity}, range={light.range}, shadows={light.shadows}");
        //            //light.enabled = false;
        //        }
        //    }
        //    
        //}
        void OnActiveSceneChanged(SM.Scene from, SM.Scene to)
        {
            Debug.Log($"[RinkOnlyPruner] activeSceneChanged: {from.name} -> {to.name}");
            //TryPruneScene(to, "activeSceneChanged");
        }

        IEnumerator PassRunner(SM.Scene scene)
        {
            for (int i = 0; i < Mathf.Max(1, passes); i++)
            {
                TryPruneScene(scene, $"pass {i + 1}/{passes}");
                if (i < passes - 1) yield return new WaitForSeconds(passInterval);
            }
        }

        void TryPruneAllLoadedScenes(string reason)
        {
            for (int i = 0; i < SM.SceneManager.sceneCount; i++)
            {
                var sc = SM.SceneManager.GetSceneAt(i);
                TryPruneScene(sc, reason + " (all)");
            }
        }

        void TryPruneScene(SM.Scene scene, string reason)
        {
            if (!scene.IsValid() || !scene.isLoaded) return;

            // Check name match
            if (!string.Equals(scene.name, targetScene, System.StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log($"[RinkOnlyPruner] Skip '{scene.name}' (target '{targetScene}')");
                return;
            }

            Debug.Log($"[RinkOnlyPruner] Pruning scene '{scene.name}' due to {reason}.");
            var roots = scene.GetRootGameObjects();

            foreach (var go in roots)
            {
                string name = go.name;

                if (KeepPatterns.Any(rx => rx.IsMatch(name)))
                {
                    Debug.Log($"[RinkOnlyPruner] KEEP: {name}");
                    continue;
                }

                bool explicitNuke = NukePatterns.Any(rx => rx.IsMatch(name));
                if (explicitNuke || hardNuke)
                {
                    Debug.Log($"[RinkOnlyPruner] NUKE ROOT: {name}");
                    Destroy(go);
                }
                else
                {
                    Debug.Log($"[RinkOnlyPruner] STRIP: {name}");
                    StripVisualsAndColliders(go.transform);
                }
            }
        }

        static void StripVisualsAndColliders(Transform root)
        {
            foreach (var r in root.GetComponentsInChildren<Renderer>(true)) r.enabled = false;
            foreach (var c in root.GetComponentsInChildren<Collider>(true)) c.enabled = false;
            foreach (var l in root.GetComponentsInChildren<Light>(true)) l.enabled = false;
            foreach (var a in root.GetComponentsInChildren<AudioSource>(true)) a.enabled = false;
            foreach (var rp in root.GetComponentsInChildren<ReflectionProbe>(true)) rp.enabled = false;
            foreach (var cloth in root.GetComponentsInChildren<Cloth>(true)) cloth.enabled = false;
            foreach (var ps in root.GetComponentsInChildren<ParticleSystem>(true)) ps.gameObject.SetActive(false);
        }
    }
}
