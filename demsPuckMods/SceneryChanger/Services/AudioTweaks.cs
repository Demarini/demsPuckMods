using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace SceneryChanger.Services
{
    public static class AudioTweaks
    {
        public static void MuteAmbient(Scene scene)
        {
            var sounds = scene.GetRootGameObjects().FirstOrDefault(go => go.name == "Sounds");
            if (!sounds) return;

            var ambient = sounds.transform.Find("Ambient Crowd");
            if (!ambient) return;

            var src = ambient.GetComponent<AudioSource>();
            if (src) src.volume = 0f;            // or src.mute = true;

            // If a sync script meddles with volume, just disable it:
            foreach (var c in ambient.GetComponents<Behaviour>())
                if (c.GetType().Name.Contains("SynchronizedAudio")) c.enabled = false;
        }
    }
}
