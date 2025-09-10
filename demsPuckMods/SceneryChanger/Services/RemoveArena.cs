using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneryChanger.Services
{
    public class RemoveArena
    {
        //Toaster da legend found these object names! Going through that scene hierarchy would have been a pain, ty!
        private static List<string> hangarObjectNames = new List<string>()
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
        private static List<string> glassObjectNames = new List<string>()
        {
        "Pillars",
        "Glass",
        };

        private static List<string> scoreboardObjectNames = new List<string>()
        {
        "scoreboard",
        "Scoreboard",
        "Scoreboard (1)"
        };

        private static List<string> spectatorObjectNames = new List<string>()
        {
        "Spectator",
        "Spectator(Clone)",
        "spectator_booth",
        "Spectator Booth 1",
        "Spectator Booth 2",
        "Spectator Booth 3",
        "Spectator Booth 4",
        "Spectator Booth 5",
        "Spectator Booth 6",
        "Spectator Booth 7",
        "Spectator Booth 8",
        "Spectator Booth 9",
        "Spectator Booth 10"
        };
        static readonly Regex[] Keep = new[]
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
        private static List<GameObject> hangarObjects = new List<GameObject>();
        private static List<GameObject> spectatorObjects = new List<GameObject>();
        private static List<GameObject> scoreboardObjects = new List<GameObject>();
        private static List<GameObject> glassObjects = new List<GameObject>();
        private static List<GameObject> totalObjectList = new List<GameObject>();

        public static void PopulateGameObjectLists()
        {
            hangarObjects.Clear();
            spectatorObjects.Clear();
            scoreboardObjects.Clear();
            glassObjects.Clear();
            totalObjectList.Clear();
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
                if (hangarObjectNames.Contains(gameObject.name))
                {
                    hangarObjects.Add(gameObject);
                }
                if (glassObjectNames.Contains(gameObject.name))
                {
                    glassObjects.Add(gameObject);
                }
                if (scoreboardObjectNames.Contains(gameObject.name))
                {
                    scoreboardObjects.Add(gameObject);
                }
                if (spectatorObjectNames.Contains(gameObject.name))
                {
                    spectatorObjects.Add(gameObject);
                }
            }
            totalObjectList.AddRange(hangarObjects); 
            //totalObjectList.AddRange(glassObjects); 
            totalObjectList.AddRange(scoreboardObjects);
            totalObjectList.AddRange(spectatorObjects);
        }
        public static void HideEverythingExceptRink(Scene scene)
        {
            // Grab all scene objects (not assets, not DontDestroyOnLoad)
            var all = UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsSortMode.None);
            int affected = 0;

            foreach (var tr in all)
            {
                if (!tr || tr.gameObject.scene != scene) continue;

                // Skip anything in keep subtrees (object or any parent matches)
                if (IsInKeepSubtree(tr)) continue;

                var root = tr.root;
                if (root && root.gameObject.activeSelf)
                {
                    root.gameObject.SetActive(false);
                    affected++;
                    // optional: log a little, not too spammy
                    // Debug.Log($"[RinkExcluder] Deactivated root: {root.name}");
                }
            }

            Debug.Log($"[RinkExcluder] Excluded non-rink content. Objects affected (approx): {affected}");
        }
        public  static void TryPruneScene(Scene scene, string reason)
        {
            if (!scene.IsValid() || !scene.isLoaded) return;

            // Check name match
            //if (!string.Equals(scene.name, targetScene, System.StringComparison.OrdinalIgnoreCase))
            //{
            //    Debug.Log($"[RinkOnlyPruner] Skip '{scene.name}' (target '{targetScene}')");
            //    return;
            //}

            Debug.Log($"[RinkOnlyPruner] Pruning scene '{scene.name}' due to {reason}.");
            GameObject[] roots = scene.GetRootGameObjects();

            foreach (GameObject go in roots)
            {
                string name = go.name;

                if (Keep.Any(rx => rx.IsMatch(name)))
                {
                    Debug.Log($"[RinkOnlyPruner] KEEP: {name}");
                    continue;
                }

                bool explicitNuke = NukePatterns.Any(rx => rx.IsMatch(name));
                Debug.Log($"[RinkOnlyPruner] NUKE ROOT: {name}");
                go.SetActive(false);
            }
        }
        void UndoPruneScene(Scene scene, string reason)
        {
            if (!scene.IsValid() || !scene.isLoaded) return;

            // Check name match
            //if (!string.Equals(scene.name, targetScene, System.StringComparison.OrdinalIgnoreCase))
            //{
            //    Debug.Log($"[RinkOnlyPruner] Skip '{scene.name}' (target '{targetScene}')");
            //    return;
            //}

            Debug.Log($"[RinkOnlyPruner] Pruning scene '{scene.name}' due to {reason}.");
            GameObject[] roots = scene.GetRootGameObjects();

            foreach (GameObject go in roots)
            {
                string name = go.name;

                if (Keep.Any(rx => rx.IsMatch(name)))
                {
                    Debug.Log($"[RinkOnlyPruner] KEEP: {name}");
                    continue;
                }

                bool explicitNuke = NukePatterns.Any(rx => rx.IsMatch(name));
                Debug.Log($"[RinkOnlyPruner] NUKE ROOT: {name}");
                go.SetActive(true);
            }
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
        public static void HideArena()
        {
            HideArenaObjects(totalObjectList);
            HideMeshRenderers(totalObjectList);
        }
        public static void ShowArena()
        {
            ShowArenaObjects(totalObjectList);
            ShowMeshRenderers(totalObjectList);
        }
        public static void HideGlass()
        {
            HideArenaObjects(glassObjects);
            HideMeshRenderers(glassObjects);
        }
        public static void ShowGlass()
        {
            ShowArenaObjects(glassObjects);
            ShowMeshRenderers(glassObjects);
        }
        private static void HideArenaObjects(List<GameObject> objectToHide) => objectToHide.ForEach(go => go.SetActive(false));
        private static void HideMeshRenderers(List<GameObject> objectToHide) =>
    objectToHide.ForEach(go => { var mr = go.GetComponent<MeshRenderer>(); if (mr != null) mr.enabled = false; });
        private static void ShowArenaObjects(List<GameObject> objectToHide) => objectToHide.ForEach(go => go.SetActive(true));
        private static void ShowMeshRenderers(List<GameObject> objectToHide) =>
    objectToHide.ForEach(go => { var mr = go.GetComponent<MeshRenderer>(); if (mr != null) mr.enabled = true; });
    }
}
