using HarmonyLib;
using SceneryLoader.Behaviors;
using SceneryLoader.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SceneryChanger.Patches
{
    public class SpectatorManagerPatch
    {
        [HarmonyPatch(typeof(SpectatorManager), nameof(SpectatorManager.SpawnSpectators))]
        public static class SpectatorManager_SpawnSpectators_Patch
        {
            static bool Prefix(SpectatorManager __instance)
            {
                if (!RinkOnlyPruner.ReapplySpectators)
                    return true; // let original run normally

                // We are handling spawning ourselves → skip original
                Debug.Log("[SpectatorManager] Reapplying spectators (custom).");

                var prefab = Traverse.Create(__instance).Field("spectatorPrefab").GetValue<Spectator>();
                var spectators = Traverse.Create(__instance).Field("spectators").GetValue<List<Spectator>>();

                // Clear existing instances safely
                if (spectators != null)
                {
                    foreach (var s in spectators)
                    {
                        if (s != null && s.gameObject != null)
                            UnityEngine.Object.Destroy(s.gameObject);
                    }
                    spectators.Clear();
                }

                if (prefab == null)
                {
                    Debug.LogWarning("[SpectatorManager] spectatorPrefab is null; aborting.");
                    return false; // skip original
                }

                var spectatorList = GameObject.Find("SpectatorList");
                if (spectatorList == null)
                {
                    Debug.LogWarning("[SpectatorRemover] Could not find 'SpectatorList'.");
                    return false;
                }

                var spectatorLocations = GameObject.Find("SpectatorLocations");
                if (spectatorLocations == null)
                {
                    Debug.LogWarning("[SpectatorRemover] Could not find 'SpectatorLocations'.");
                    return false;
                }
                List<Transform> spectatorSpots = new List<Transform>();

                foreach (Transform child in spectatorLocations.GetComponentsInChildren<Transform>(true))
                {
                    // Skip root, and skip organizational parents
                    if (child == spectatorLocations.transform) continue;

                    if (child.name.StartsWith("Spectator") &&
                        !child.name.Contains("Column") &&
                        !child.name.Contains("Row"))
                    {
                        spectatorSpots.Add(child);
                    }
                }
                foreach (Transform child in spectatorSpots)
                {
                    //Debug.Log("Adding spectator");
                    // remove optional plane
                    var plane = child.Find("Plane");
                    if (plane != null)
                        UnityEngine.Object.Destroy(plane.gameObject);

                    var item = UnityEngine.Object.Instantiate(prefab, child.position, child.rotation, spectatorList.transform);
                    spectators.Add(item);
                }

                // We handled it; prevent original from running
                RinkOnlyPruner.ReapplySpectators = false; // guard against re-entry/double spawn
                return false;
            }
        }
    }
}
