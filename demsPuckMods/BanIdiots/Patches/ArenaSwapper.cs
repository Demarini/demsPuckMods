using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
namespace BanIdiots.Patches
{
    public static class ArenaSwapper
    {
        public static bool hardMode = true;
        private static List<GameObject> hiddenOutdoorObjects = new List<GameObject>();
        private static List<GameObject> hiddenCrowdObjects = new List<GameObject>();
        private static List<GameObject> hiddenScoreboardObjects = new List<GameObject>();
        private static List<GameObject> hiddenGlassObjects = new List<GameObject>();
        static readonly Regex[] Keep = new[]
    {
        new Regex(@"^Level$", RegexOptions.IgnoreCase),
        new Regex(@"^Rink$", RegexOptions.IgnoreCase),
        new Regex(@"^Goal\s+(Blue|Red)$", RegexOptions.IgnoreCase),
        new Regex(@"^Team\s+(Blue|Red)\s+Positions$", RegexOptions.IgnoreCase),
        new Regex(@"^Puck\s+Shooter$", RegexOptions.IgnoreCase),
        new Regex(@"^Puck\s+Position$", RegexOptions.IgnoreCase),
        new Regex(@"^Warmup\s+Puck\s+Positions$", RegexOptions.IgnoreCase),
        new Regex(@".*Camera$", RegexOptions.IgnoreCase),
    };
        static readonly List<GameObject> _hiddenRoots = new List<GameObject>();

        public static void HideEverythingExceptRink(Scene scene, bool hardMode = true)
        {
            // Grab all scene objects (not assets, not DontDestroyOnLoad)
            var all = UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsSortMode.None);
            int affected = 0;

            foreach (var tr in all)
            {
                if (!tr || tr.gameObject.scene != scene) continue;

                // Skip anything in keep subtrees (object or any parent matches)
                if (IsInKeepSubtree(tr)) continue;

                if (hardMode)
                {
                    // deactivate the *root* of this foreign subtree once
                    var root = tr.root;
                    if (root && root.gameObject.activeSelf)
                    {
                        root.gameObject.SetActive(false);
                        affected++;
                        // optional: log a little, not too spammy
                        // Debug.Log($"[RinkExcluder] Deactivated root: {root.name}");
                    }
                }
                else
                {
                    StripVisualsAndAudio(tr);
                    affected++;
                }
            }

            Debug.Log($"[RinkExcluder] Excluded non-rink content. Objects affected (approx): {affected}");
        }

        static bool IsInKeepSubtree(Transform tr)
        {
            var p = tr;
            while (p != null)
            {
                if (Keep.Any(rx => rx.IsMatch(p.name)))
                    return true;
                p = p.parent;
            }
            return false;
        }

        static void StripVisualsAndAudio(Transform t)
        {
            foreach (var r in t.GetComponentsInChildren<Renderer>(true)) r.enabled = false;
            foreach (var c in t.GetComponentsInChildren<Collider>(true)) c.enabled = false;
            foreach (var l in t.GetComponentsInChildren<Light>(true)) l.enabled = false;
            foreach (var a in t.GetComponentsInChildren<AudioSource>(true)) a.enabled = false;
            foreach (var rp in t.GetComponentsInChildren<ReflectionProbe>(true)) rp.enabled = false;
            foreach (var ps in t.GetComponentsInChildren<ParticleSystem>(true)) ps.gameObject.SetActive(false);
        }
        public static void UpdateCrowdState()
        {
            try
            {
                HideCrowdObjects();
            }
            catch (Exception e)
            {
                //Plugin.LogError($"Error when updating crowd state: {e.Message}");
            }
        }

        public static void UpdateHangarState()
        {
            try
            {
                HideOutdoorObjects();
            }
            catch (Exception e)
            {
                //Plugin.LogError($"Error when updating hanger state: {e.Message}");
            }
        }

        public static void UpdateScoreboardState()
        {
            try
            {
                HideScoreboardObjects();
            }
            catch (Exception e)
            {
                //Plugin.LogError($"Error when updating scoreboard state: {e.Message}");
            }
        }
        public static void HideGlassObjects()
        {
            // Find all GameObjects in the scene
            UnityEngine.Object[] allObjects =
                UnityEngine.Object.FindObjectsByType(typeof(GameObject), FindObjectsSortMode.None);

            // Iterate through all objects
            foreach (UnityEngine.Object obj in allObjects)
            {
                // Try to cast the object to a GameObject
                GameObject gameObject = (GameObject)obj;
                if (gameObject == null || gameObject.transform == null)
                {
                    continue;
                }

                if (namesOfGlassObjects.Contains(gameObject.name))
                {
                    hiddenGlassObjects.Add(gameObject);
                    gameObject.SetActive(false);

                    MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
                    if (mr != null)
                    {
                        mr.enabled = false;
                    }
                }
            }
        }
        public static void UpdateGlassState()
        {
            try
            {
                HideGlassObjects();
            }
            catch (Exception e)
            {
                //Plugin.LogError($"Error when updating glass state: {e.Message}");
            }
        }

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

        private static string[] namesOfGlassObjects = new[]
        {
        "Pillars",
        "Glass",
    };

        private static string[] namesOfScoreboardObjects = new[]
        {
        "scoreboard",
        "Scoreboard",
        "Scoreboard (1)"
    };

        private static string[] namesOfCrowdObjects = new[]
        {
        "Spectator",
        "Spectator(Clone)",
        "spectator_booth"
    };

        private static void HideCrowdObjects()
        {
            // Find all GameObjects in the scene
            UnityEngine.Object[] allObjects =
                UnityEngine.Object.FindObjectsByType(typeof(GameObject), FindObjectsSortMode.None);

            // Iterate through all objects
            foreach (UnityEngine.Object obj in allObjects)
            {
                // Try to cast the object to a GameObject
                GameObject gameObject = (GameObject)obj;
                if (gameObject == null || gameObject.transform == null)
                {
                    continue;
                }

                if (namesOfCrowdObjects.Contains(gameObject.name))
                {
                    hiddenCrowdObjects.Add(gameObject);
                    gameObject.SetActive(false);
                }
            }
        }

        private static void ShowCrowdObjects()
        {
            foreach (GameObject obj in hiddenCrowdObjects)
            {
                obj.SetActive(true);
            }

            hiddenCrowdObjects.Clear();
        }

        public static void HideOutdoorObjects()
        {
            // Find all GameObjects in the scene
            UnityEngine.Object[] allObjects =
                UnityEngine.Object.FindObjectsByType(typeof(GameObject), FindObjectsSortMode.None);

            // Iterate through all objects
            foreach (UnityEngine.Object obj in allObjects)
            {
                // Try to cast the object to a GameObject
                GameObject gameObject = (GameObject)obj;
                if (gameObject == null || gameObject.transform == null)
                {
                    continue;
                }

                if (namesOfOutdoorObjects.Contains(gameObject.name))
                {
                    hiddenOutdoorObjects.Add(gameObject);
                    gameObject.SetActive(false);

                    MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
                    if (mr != null)
                    {
                        mr.enabled = false;
                    }
                }
            }
        }

        public static void ShowOutdoorObjects()
        {
            foreach (GameObject obj in hiddenOutdoorObjects)
            {
                if (obj == null || obj.transform == null) continue;
                obj.SetActive(true);
                MeshRenderer mr = obj.GetComponent<MeshRenderer>();
                if (mr != null)
                {
                    mr.enabled = true;
                }
            }

            hiddenOutdoorObjects.Clear();
        }

        public static void HideScoreboardObjects()
        {
            // Find all GameObjects in the scene
            UnityEngine.Object[] allObjects =
                UnityEngine.Object.FindObjectsByType(typeof(GameObject), FindObjectsSortMode.None);

            // Iterate through all objects
            foreach (UnityEngine.Object obj in allObjects)
            {
                // Try to cast the object to a GameObject
                GameObject gameObject = (GameObject)obj;
                if (gameObject == null || gameObject.transform == null)
                {
                    continue;
                }

                if (namesOfScoreboardObjects.Contains(gameObject.name))
                {
                    if (!hiddenScoreboardObjects.Contains(gameObject))
                        hiddenScoreboardObjects.Add(gameObject);
                    if (gameObject.GetComponent<Scoreboard>() != null)
                    {
                        // Plugin.Log($"turning off scoreboard {gameObject.name}");
                        Scoreboard scoreboard = gameObject.GetComponent<Scoreboard>();
                        scoreboard.TurnOff();
                    }
                    gameObject.SetActive(false);

                    MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
                    if (mr != null)
                    {
                        mr.enabled = false;
                    }
                }
            }
        }
        [HarmonyPatch(typeof(SpectatorManager), nameof(SpectatorManager.SpawnSpectators))]
        public static class SpectatorManagerSpawnSpectators
        {
            [HarmonyPostfix]
            public static void Postfix(SpectatorManager __instance)
            {
                UpdateCrowdState();
            }
        }
    }
}
