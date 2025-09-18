using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SceneryChanger.Helpers
{
    public static class ArenaLightUtil
    {
        public static GameObject FindArenaLights()
        {
            // Look for HockeyArenaRoot -> GoalCrowdNoise
            var root = UnityEngine.Object.FindObjectsOfType<Transform>(true)
                             .FirstOrDefault(t => t.name.Contains("HockeyArenaRoot"));
            if (root == null)
            {
                Debug.Log("Could not find arena root");
                return null;
            }

            var arena = root.GetComponentsInChildren<Transform>(true)
                            .FirstOrDefault(t => t.name == "HockeyArena");
            if (arena == null)
            {
                Debug.Log("Could not find arena");
                return null;
            }

            var light = arena.GetComponentsInChildren<Transform>(true)
                            .FirstOrDefault(t => t.name == "Lights");
            if (light == null)
            {
                Debug.Log("Could not find light");
                return null;
            }

            var arenaLights = light.GetComponentsInChildren<Transform>(true)
                            .FirstOrDefault(t => t.name == "ArenaLights");
            if (light == null)
            {
                Debug.Log("Could not find arena light");
                return null;
            }
            return arenaLights.gameObject;
        }
    }
}
