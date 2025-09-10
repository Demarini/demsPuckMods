using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine;
using SceneryChanger.Behaviors;
using System.Collections;

namespace SceneryLoader.Services
{
    public static class SkyboxLoader
    {
        public static IEnumerator ApplySkyboxFromBundleAsync(string bundleName, string skyboxMaterialName, string contentKey64)
        {
            bool done = false; Material sky = null; AssetBundle abHold = null;

            yield return BundleLoader.GetOrLoadAsync(bundleName, contentKey64, ab =>
            {
                abHold = ab;
                if (ab == null) { done = true; return; }
                var req = ab.LoadAssetAsync<Material>(skyboxMaterialName);
                CoroutineRunner.Instance.StartCoroutine(Finish(req));

                IEnumerator Finish(AssetBundleRequest r)
                {
                    yield return r;
                    sky = r.asset as Material;
                    done = true;
                }
            });

            while (!done) yield return null;

            if (abHold == null)
            {
                Debug.LogError($"[SkyboxLoader] Failed to load bundle '{bundleName}'");
                yield break;
            }
            if (sky == null)
            {
                Debug.LogError($"[SkyboxLoader] Couldn’t find Material '{skyboxMaterialName}' in bundle.");
                yield break;
            }

            RenderSettings.skybox = sky;
            RenderSettings.defaultReflectionMode = UnityEngine.Rendering.DefaultReflectionMode.Skybox;
            RenderSettings.customReflection = null;
            RenderSettings.reflectionIntensity = 1f;
            DynamicGI.UpdateEnvironment();

            foreach (var cam in UnityEngine.Object.FindObjectsByType<Camera>(FindObjectsSortMode.None))
                cam.clearFlags = CameraClearFlags.Skybox;
        }
    }
}