using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace SceneryLoader.Services
{
    public static class ReflectionProbeKiller
    {
        public static void RemoveReflectionProbes(Scene scene, bool destroyGameObjects = false)
        {
            foreach (var root in scene.GetRootGameObjects())
            {
                foreach (var probe in root.GetComponentsInChildren<ReflectionProbe>(true))
                {
                    if (destroyGameObjects)
                    {
                        UnityEngine.Object.Destroy(probe.gameObject);
                        Debug.Log($"[RP] Destroyed probe GO: {probe.name}");
                    }
                    else
                    {
                        probe.enabled = false;
                        Debug.Log($"[RP] Disabled probe: {probe.name}");
                    }
                }
            }

            // Renderers can still *sample* probes even if none are active. Turn that off:
            foreach (var r in UnityEngine.Object.FindObjectsOfType<Renderer>(true))
            {
                r.reflectionProbeUsage = ReflectionProbeUsage.Off;
            }

            // Also nuke the *default* environment reflection (skybox cubemap) intensity so you
            // don’t get shiny highlights that look like “phantom lights”.
            RenderSettings.reflectionIntensity = 0f;   // 0..1 scale
                                                       // Optional: force a custom (null) default reflection instead of skybox:
                                                       // RenderSettings.defaultReflectionMode = DefaultReflectionMode.Custom;
                                                       // RenderSettings.customReflection = null;

            DynamicGI.UpdateEnvironment();
        }
    }
}