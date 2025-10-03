using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SceneryChanger.Helpers
{
    public static class GetAudio
    {
        public static AudioSource FindAudio(string sourceName)
        {
            // Look for HockeyArenaRoot -> GoalCrowdNoise
            var root = UnityEngine.Object.FindObjectsOfType<Transform>(true)
                             .FirstOrDefault(t => t.name.Contains("HockeyArenaRoot"));
            if (root == null)
            {
                Debug.Log("Could not find arena root");
                return null;
            }

            var audio = root.GetComponentsInChildren<Transform>(true)
                            .FirstOrDefault(t => t.name == sourceName);
            if (audio == null)
            {
                Debug.Log("Could not find arena");
                return null;
            }
            return audio.GetComponent<AudioSource>();
        }
    }
}