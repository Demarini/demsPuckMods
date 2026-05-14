using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Rendering;
using UnityEngine;

namespace SceneryLoader.Services
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

        // Smart cleanup called AFTER the bundle is instantiated. Disables the game's now-stale
        // hangar reflection probes (they were sampling geometry we just destroyed) while keeping
        // any reflection probes the bundle authored alive. Also restores the global default
        // reflection so glossy surfaces (ice especially) reflect their environment instead of
        // a black cubemap. SkyboxLoader runs after this and may override defaultReflectionMode
        // again if the bundle ships a custom skybox.
        public static void ApplyReflectionPolicy(GameObject stagedRoot, bool keepAllReflections)
        {
            try
            {
                int disabled = 0, kept = 0;
                foreach (var rp in UnityEngine.Object.FindObjectsByType<ReflectionProbe>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                {
                    if (rp == null) continue;
                    bool inStaged = stagedRoot != null && rp.transform.IsChildOf(stagedRoot.transform);
                    if (keepAllReflections || inStaged)
                    {
                        rp.enabled = true;
                        kept++;
                    }
                    else
                    {
                        rp.enabled = false;
                        disabled++;
                    }
                }

                // Undo any black-cube override we may have left around from older flows so URP can
                // sample a real environment (skybox or fallback) for glossy surfaces.
                if (RenderSettings.defaultReflectionMode == DefaultReflectionMode.Custom)
                {
                    RenderSettings.defaultReflectionMode = DefaultReflectionMode.Skybox;
                    RenderSettings.customReflection = null;
                }

                DynamicGI.UpdateEnvironment();
                Debug.Log($"[ReflectionKiller] Policy: keepAll={keepAllReflections} disabled={disabled} kept={kept} (bundle/+game) defaultRefl={RenderSettings.defaultReflectionMode}");
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[ReflectionKiller] ApplyReflectionPolicy failed: {e.Message}");
            }
        }
    }
}
