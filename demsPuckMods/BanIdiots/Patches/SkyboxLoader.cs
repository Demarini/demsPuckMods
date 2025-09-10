using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine;

namespace BanIdiots.Patches
{
    public static class SkyboxLoader
    {
        public static void ApplySkyboxFromBundle(string bundlePath, string skyboxMaterialName)
        {
            var bundle = BundleLoader.GetOrLoad(bundlePath);
            if (!bundle)
            {
                Debug.LogError($"[SkyboxLoader] Failed to load bundle at {bundlePath}");
                return;
            }

            var skyMat = bundle.LoadAsset<Material>(skyboxMaterialName);
            if (!skyMat)
            {
                Debug.LogError($"[SkyboxLoader] Couldn’t find Material '{skyboxMaterialName}' in bundle.");
                return;
            }

            // Assign skybox
            RenderSettings.skybox = skyMat;

            // Make sure reflections come from the skybox (not a stale custom cubemap)
            RenderSettings.defaultReflectionMode = DefaultReflectionMode.Skybox;
            RenderSettings.customReflection = null;
            RenderSettings.reflectionIntensity = 1f; // tweak if you want subtler reflections

            // Force the environment reflection to refresh
            DynamicGI.UpdateEnvironment();

            // Ensure all cameras actually render the skybox
            foreach (var cam in UnityEngine.Object.FindObjectsByType<Camera>(FindObjectsSortMode.None))
            {
                cam.clearFlags = CameraClearFlags.Skybox;

                // URP cameras sometimes get set to “Solid Color” via additional data; ensure skybox background
                if (cam.TryGetComponent<UniversalAdditionalCameraData>(out var urpCam))
                {
                    // Background type is tied to clearFlags, so this is usually enough;
                    // but if you use camera stacks, ensure base camera is Skybox too.
                }
            }

            // Optional: don't unload(true) or you’ll destroy the material instance.
            // You can safely do:
            // bundle.Unload(false);
        }
    }
}