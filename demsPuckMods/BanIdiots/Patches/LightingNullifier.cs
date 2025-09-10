using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace BanIdiots.Patches
{
    public static class LightingNullifier
    {
        public static void KillAllSceneLighting(Scene scene)
        {
            // 1) No ambient, no skybox, no reflections
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = Color.black;
            RenderSettings.skybox = null;
            RenderSettings.defaultReflectionMode = DefaultReflectionMode.Custom;
            RenderSettings.customReflection = null;

            // 2) Clear baked lightmaps entirely
            LightmapSettings.lightmaps = new LightmapData[0];
            LightmapSettings.lightmapsMode = LightmapsMode.NonDirectional;

            // 3) Strip per-renderer baked indices / probes / reflections / emission
            foreach (var go in scene.GetRootGameObjects())
            {
                StripRendererLighting(go.transform);
            }

            // 4) Flush GI
            DynamicGI.UpdateEnvironment();
        }

        static void StripRendererLighting(Transform root)
        {
            // MeshRenderer
            foreach (var r in root.GetComponentsInChildren<MeshRenderer>(true))
            {
                r.lightmapIndex = -1;
                r.lightmapScaleOffset = Vector4.zero;
                r.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
                r.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;

                // kill emission if any
                var mats = r.sharedMaterials;
                for (int i = 0; i < mats.Length; i++)
                {
                    var m = mats[i];
                    if (m == null) continue;
                    if (m.HasProperty("_EmissionColor"))
                    {
                        m.DisableKeyword("_EMISSION");
                        m.SetColor("_EmissionColor", Color.black);
                    }
                    if (m.HasProperty("_EmissionStrength")) m.SetFloat("_EmissionStrength", 0f); // some URP packs
                }
            }

            // SkinnedMeshRenderer
            foreach (var r in root.GetComponentsInChildren<SkinnedMeshRenderer>(true))
            {
                r.lightmapIndex = -1;
                r.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
                r.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;

                var mats = r.sharedMaterials;
                for (int i = 0; i < mats.Length; i++)
                {
                    var m = mats[i];
                    if (m == null) continue;
                    if (m.HasProperty("_EmissionColor"))
                    {
                        m.DisableKeyword("_EMISSION");
                        m.SetColor("_EmissionColor", Color.black);
                    }
                    if (m.HasProperty("_EmissionStrength")) m.SetFloat("_EmissionStrength", 0f);
                }
            }
        }
    }
}
