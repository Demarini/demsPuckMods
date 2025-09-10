using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Rendering;
using UnityEngine;

namespace BanIdiots.Patches
{
    public static class ReflectionKiller
    {
        static Cubemap _blackCube;

        public static void NukeAllReflections()
        {
            // 1) Disable all ReflectionProbes in the scene
            foreach (var rp in UnityEngine.Object.FindObjectsOfType<ReflectionProbe>(true))
                rp.enabled = false;

            // 2) Create (once) a tiny black cubemap and use it as the custom reflection
            if (_blackCube == null)
            {
                const int size = 16;
                _blackCube = new Cubemap(size, TextureFormat.RGBA32, /*mipChain*/ false)
                {
                    name = "BlackReflection"
                };

                var black = Enumerable.Repeat(Color.black, size * size).ToArray();
                foreach (CubemapFace face in System.Enum.GetValues(typeof(CubemapFace)))
                {
                    if (face == CubemapFace.Unknown) continue;
                    _blackCube.SetPixels(black, face);
                }
                _blackCube.Apply(false);
            }

            RenderSettings.defaultReflectionMode = DefaultReflectionMode.Custom;
            RenderSettings.customReflection = _blackCube;   // global specular reflection = black
                                                            // (Optional) also kill the skybox so there’s nothing to sample at all:
                                                            // RenderSettings.skybox = null;

            DynamicGI.UpdateEnvironment();
            Debug.Log("[ReflectionKiller] Probes disabled and custom black reflection applied.");
        }

        // Optional: disable only glass reflections instead of global nuke
        public static void DisableGlassReflections(string nameContains = "glass")
        {
            var lower = nameContains.ToLowerInvariant();
            foreach (var r in UnityEngine.Object.FindObjectsOfType<MeshRenderer>(true))
            {
                if (r.name.ToLowerInvariant().Contains(lower))
                    r.reflectionProbeUsage = ReflectionProbeUsage.Off;
            }
            Debug.Log("[ReflectionKiller] Disabled reflectionProbeUsage on glass-like renderers.");
        }

        // Optional: restore default behavior
        public static void RestoreReflections()
        {
            foreach (var rp in UnityEngine.Object.FindObjectsOfType<ReflectionProbe>(true))
                rp.enabled = true;

            RenderSettings.defaultReflectionMode = DefaultReflectionMode.Skybox;
            RenderSettings.customReflection = null;
            DynamicGI.UpdateEnvironment();
            Debug.Log("[ReflectionKiller] Restored probes and skybox reflections.");
        }
    }
}
