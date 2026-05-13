using HarmonyLib;
using SceneryChanger.Behaviors;
using SceneryChanger.Model;
using SceneryLoader.Behaviors;
using SceneryLoader.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using SceneryLoader.Singletons;
namespace SceneryChanger.Services
{
    public sealed class SceneContentRoot : MonoBehaviour { } // tiny marker

    public static class RinkSceneState
    {
        public static GameObject Container;     // parent for all mod content
        public static GameObject ActiveRoot;    // currently shown root
        public static int CurrentLoadToken;     // monotonically increasing version
    }
    public static class RinkSceneLoader
    {
        static GameObject EnsureContainer()
        {
            if (RinkSceneState.Container) return RinkSceneState.Container;
            var go = new GameObject("SceneryLoaderContent");
            return RinkSceneState.Container = go;
        }
        // Call this from anywhere. It does everything async.
        public static void LoadSceneAsync(Scene scene, SceneInformation si)
        {
            int token = ++RinkSceneState.CurrentLoadToken;
            Debug.Log($"[SceneLoader] LoadSceneAsync token={token} bundle='{si?.bundleName}' prefab='{si?.prefabName}' skybox='{si?.skyboxName}' enc={!string.IsNullOrWhiteSpace(si?.contentKey64)}");
            CoroutineRunner.Instance.StartCoroutine(LoadSceneFlow(si, token));
        }
        static IEnumerator LoadSceneFlow(SceneInformation si, int token)
        {
            // --- Stage the new content (do NOT clear old yet) ---
            GameObject stagedRoot = null;
            bool got = false;
            RinkOnlyPruner.RemoveHangar();
            string dllDir = Path.GetDirectoryName(typeof(BundleLoader).Assembly.Location);
            bool enc = !string.IsNullOrWhiteSpace(si.contentKey64);
            var resolved = BundleResolver.Resolve(si.bundleName, dllDir, preferEncrypted: enc);
            Debug.Log($"[SceneLoader] BundleResolver: dllDir='{dllDir}' bundle='{si.bundleName}' enc={enc} -> Exists={resolved?.Exists} Path='{resolved?.BundlePath}'");
            if (resolved == null || !resolved.Exists)
            {
                Debug.LogError($"[SceneLoader] Bundle '{si.bundleName}' not found under '{dllDir}\\AssetBundles' (or sibling mod folders). Aborting load.");
                yield break;
            }
            var info = resolved != null ? resolved.Info : null;
            if (info != null)
            {
                if (info.useGlass) RemoveArena.ShowGlass();
                else RemoveArena.HideGlass();

                if (info.useCustomAmbient)
                {
                    RenderSettings.ambientMode = AmbientMode.Flat;
                    RenderSettings.ambientLight = RenderSettings.ambientLight * info.ambientMultiplier;
                    Debug.Log($"[SceneLoader] Ambient multiplier={info.ambientMultiplier} applied");
                }
            }
            else
            {
                RemoveArena.ShowGlass();
            }
            AudioTweaks.TryDisableAmbientAudio();
            yield return BundleLoader.InstantiatePrefabAsync(si.bundleName, si.prefabName, si.contentKey64, go =>
            {
                stagedRoot = go; got = true;
            });

            if (!got || !stagedRoot)
            {
                if (token == RinkSceneState.CurrentLoadToken)
                    Debug.LogError($"[SceneLoader] Instantiate failed for '{si.prefabName}' from '{si.bundleName}'.");
                yield break;
            }

            // If this load was superseded while we were loading, just discard it
            if (token != RinkSceneState.CurrentLoadToken)
            {
                UnityEngine.Object.Destroy(stagedRoot);
                yield break;
            }

            // Parent staged under our container
            var container = EnsureContainer();
            stagedRoot.transform.SetParent(container.transform, true);

            RebindShadersToGameRuntime(stagedRoot);
            SetupMusic(stagedRoot, info, resolved?.FolderPath);
            SetupAmbientAudio(stagedRoot, info, resolved?.FolderPath);
            if (info != null)
                SceneryAudioState.GoalCrowdNoiseVolume = info.goalCrowdNoiseVolume;
            DumpStagedRoot(stagedRoot);

            // --- Apply skybox for *this token* only ---
            if (!string.IsNullOrEmpty(si.skyboxName))
            {
                // You can just yield this; it's quick
                bool skyDone = false;
                CoroutineRunner.Instance.StartCoroutine(SkyboxLoader.ApplySkyboxFromBundleAsync(si.bundleName, si.skyboxName, si.contentKey64));
                // If you want to hard-block until applied, convert to a yield pattern instead.
            }
            foreach (var cam in UnityEngine.Object.FindObjectsByType<Camera>(FindObjectsSortMode.None))
                cam.clearFlags = CameraClearFlags.Skybox;
            DynamicGI.UpdateEnvironment();

            // Final token check before we swap visible content
            if (token != RinkSceneState.CurrentLoadToken)
            {
                UnityEngine.Object.Destroy(stagedRoot);
                yield break;
            }

            // --- Swap: destroy previous ActiveRoot, then make staged the ActiveRoot ---
            var old = RinkSceneState.ActiveRoot;
            RinkSceneState.ActiveRoot = stagedRoot;

            if (old) UnityEngine.Object.Destroy(old); // destroy AFTER new is ready to avoid blank frames
            yield return null; // let Destroy settle

            // --- Spectators: search inside the staged prefab and tag positions ---
            yield return SpawnSpectatorsFromStagedRoot(stagedRoot, token);

            // --- Diagnostic: dump lighting/shadow state after everything settles ---
            CoroutineRunner.Instance.StartCoroutine(DumpLightingStateDelayed(stagedRoot, 1.5f));

            // Optional cleanup of stray legacy objects AFTER swap
            //ClearLegacyOutsideContainer(); // see helper below (non-blocking)
        }

        static IEnumerator DumpLightingStateDelayed(GameObject stagedRoot, float delaySec)
        {
            yield return new WaitForSeconds(delaySec);
            DumpLightingState(stagedRoot);
        }

        static void DumpLightingState(GameObject stagedRoot)
        {
            try
            {
                // ===== Render pipeline & quality =====
                var pipeline = GraphicsSettings.currentRenderPipeline;
                Debug.Log($"[SceneLoader][Light] Pipeline='{pipeline?.GetType().FullName ?? "BuiltIn"}' " +
                          $"qShadows={QualitySettings.shadows} shadowDistance={QualitySettings.shadowDistance} " +
                          $"cascades={QualitySettings.shadowCascades} shadowRes={QualitySettings.shadowResolution} " +
                          $"projection={QualitySettings.shadowProjection} shadowmaskMode={QualitySettings.shadowmaskMode}");

                // Probe URP/HDRP asset for shadow toggles via reflection (works without compile-time URP dep)
                if (pipeline != null)
                {
                    var pType = pipeline.GetType();
                    string[] interesting = {
                        "supportsMainLightShadows", "mainLightShadowmapResolution",
                        "supportsAdditionalLightShadows", "additionalLightsShadowmapResolution",
                        "supportsSoftShadows", "shadowDistance", "shadowCascadeCount",
                        "mainLightRenderingMode", "additionalLightsRenderingMode",
                        "supportsHDR", "useSRPBatcher"
                    };
                    var bf = System.Reflection.BindingFlags.Instance |
                             System.Reflection.BindingFlags.NonPublic |
                             System.Reflection.BindingFlags.Public;
                    foreach (var name in interesting)
                    {
                        try
                        {
                            var prop = pType.GetProperty(name, bf);
                            if (prop != null) { Debug.Log($"[SceneLoader][Light] Pipeline.{name}={prop.GetValue(pipeline)}"); continue; }
                            var field = pType.GetField(name, bf);
                            if (field != null) Debug.Log($"[SceneLoader][Light] Pipeline.{name}={field.GetValue(pipeline)}");
                        }
                        catch (Exception e) { Debug.Log($"[SceneLoader][Light] Pipeline.{name} read failed: {e.Message}"); }
                    }
                }

                // ===== RenderSettings =====
                Debug.Log($"[SceneLoader][Light] RenderSettings ambientMode={RenderSettings.ambientMode} " +
                          $"ambientLight={RenderSettings.ambientLight} ambientIntensity={RenderSettings.ambientIntensity:F2} " +
                          $"skybox='{(RenderSettings.skybox != null ? RenderSettings.skybox.name : "<null>")}' " +
                          $"defaultReflectionMode={RenderSettings.defaultReflectionMode} " +
                          $"reflectionIntensity={RenderSettings.reflectionIntensity:F2} " +
                          $"sun='{(RenderSettings.sun != null ? RenderSettings.sun.name : "<null>")}'");

                // ===== All Light components in the scene =====
                var allLights = UnityEngine.Object.FindObjectsByType<Light>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                int dirCount = 0, enabledActive = 0, shadowCasters = 0;
                Debug.Log($"[SceneLoader][Light] Total Light components in scene: {allLights.Length}");
                foreach (var l in allLights)
                {
                    if (l == null) continue;
                    bool inStaged = stagedRoot != null && l.transform.IsChildOf(stagedRoot.transform);
                    bool active = l.gameObject.activeInHierarchy;
                    if (l.enabled && active) enabledActive++;
                    if (l.type == LightType.Directional) dirCount++;
                    if (l.enabled && active && l.shadows != LightShadows.None) shadowCasters++;

                    // NOTE: Light.lightmapBakeType is editor-only; at runtime we only see bakingOutput.
                    // bakingOutput.isBaked==false means the scene wasn't baked (the common case for
                    // dynamically-loaded bundles). If isBaked==true and lightmapBakeType==Baked, that
                    // means runtime shadows on dynamic meshes are impossible — light only baked.
                    var bo = l.bakingOutput;
                    Debug.Log($"[SceneLoader][Light]  - '{l.name}' parent='{l.transform.parent?.name}' inStaged={inStaged} " +
                              $"type={l.type} enabled={l.enabled} active={active} " +
                              $"bakeIsBaked={bo.isBaked} bakeType={bo.lightmapBakeType} mixedMode={bo.mixedLightingMode} " +
                              $"shadows={l.shadows} shadowStrength={l.shadowStrength:F2} " +
                              $"shadowRes={l.shadowResolution} shadowBias={l.shadowBias:F3} normalBias={l.shadowNormalBias:F3} " +
                              $"intensity={l.intensity:F2} bounce={l.bounceIntensity:F2} range={l.range:F1} " +
                              $"renderMode={l.renderMode} cullingMask=0x{l.cullingMask:X8}");
                }
                Debug.Log($"[SceneLoader][Light] Summary: directional={dirCount} enabled&active={enabledActive} shadowCasters={shadowCasters}");

                // ===== Player / stick renderers (the things we want shadows ON) =====
                var allRenderers = UnityEngine.Object.FindObjectsByType<Renderer>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                var playerLike = allRenderers
                    .Where(r => r != null && !r.transform.IsChildOf(stagedRoot != null ? stagedRoot.transform : null) &&
                                (r.name.IndexOf("stick", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                 r.name.IndexOf("player", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                 r.name.IndexOf("puck", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                 r.name.IndexOf("body", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                 r.name.IndexOf("skater", StringComparison.OrdinalIgnoreCase) >= 0))
                    .Take(15).ToList();
                Debug.Log($"[SceneLoader][Light] Player-like renderers sampled: {playerLike.Count}");
                foreach (var r in playerLike)
                {
                    Debug.Log($"[SceneLoader][Light]  P '{r.name}' parent='{r.transform.parent?.name}' " +
                              $"layer={r.gameObject.layer}({LayerMask.LayerToName(r.gameObject.layer)}) " +
                              $"shadowCastingMode={r.shadowCastingMode} receiveShadows={r.receiveShadows} " +
                              $"enabled={r.enabled} active={r.gameObject.activeInHierarchy} " +
                              $"shader='{(r.sharedMaterial != null && r.sharedMaterial.shader != null ? r.sharedMaterial.shader.name : "<null>")}'");
                }

                // ===== Ice / rink renderers (the things we want shadows ON) =====
                var iceLike = allRenderers
                    .Where(r => r != null &&
                                (r.name.IndexOf("ice", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                 r.name.IndexOf("rink", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                 r.name.IndexOf("floor", StringComparison.OrdinalIgnoreCase) >= 0))
                    .Take(10).ToList();
                Debug.Log($"[SceneLoader][Light] Ice/rink-like renderers sampled: {iceLike.Count}");
                foreach (var r in iceLike)
                {
                    Debug.Log($"[SceneLoader][Light]  I '{r.name}' parent='{r.transform.parent?.name}' " +
                              $"layer={r.gameObject.layer}({LayerMask.LayerToName(r.gameObject.layer)}) " +
                              $"shadowCastingMode={r.shadowCastingMode} receiveShadows={r.receiveShadows} " +
                              $"shader='{(r.sharedMaterial != null && r.sharedMaterial.shader != null ? r.sharedMaterial.shader.name : "<null>")}'");
                }

                // ===== Layers a directional light culls in vs renderer layers in use =====
                var dirLights = allLights.Where(l => l != null && l.type == LightType.Directional && l.enabled && l.gameObject.activeInHierarchy).ToList();
                if (dirLights.Count > 0)
                {
                    var rendererLayers = new HashSet<int>();
                    foreach (var r in allRenderers)
                        if (r != null && r.enabled && r.gameObject.activeInHierarchy)
                            rendererLayers.Add(r.gameObject.layer);
                    foreach (var dl in dirLights)
                    {
                        var missed = rendererLayers.Where(layer => (dl.cullingMask & (1 << layer)) == 0).ToList();
                        if (missed.Count > 0)
                        {
                            var missedNames = string.Join(", ", missed.Select(l => $"{l}({LayerMask.LayerToName(l)})"));
                            Debug.LogWarning($"[SceneLoader][Light] Directional light '{dl.name}' does NOT illuminate layers: {missedNames}");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[SceneLoader][Light] DumpLightingState failed: {e}");
            }
        }
        static void SetupMusic(GameObject root, AssetInformation info, string bundleFolder)
        {
            var musicTransform = root.transform.Find("Music");
            if (musicTransform == null)
            {
                foreach (Transform child in root.transform)
                {
                    musicTransform = child.Find("Music");
                    if (musicTransform != null) break;
                }
            }
            if (musicTransform == null)
            {
                Debug.Log("[SceneLoader] No 'Music' GameObject found in prefab, skipping music setup");
                return;
            }

            var audioSource = musicTransform.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = musicTransform.gameObject.AddComponent<AudioSource>();
                audioSource.loop = true;
                audioSource.spatialBlend = 0f;
            }

            audioSource.Stop();
            audioSource.clip = null;
            audioSource.playOnAwake = false;
            SceneryAudioState.MusicSource = audioSource;

            bool enabled = info != null && info.musicEnabled;
            if (!enabled)
            {
                audioSource.enabled = false;
                Debug.Log("[SceneLoader] Music disabled via AssetInformation");
                return;
            }

            string musicFile = ResolveAudioPath(info.musicPath, bundleFolder);
            if (string.IsNullOrEmpty(musicFile) || !File.Exists(musicFile))
            {
                Debug.LogWarning($"[SceneLoader] Music file not found: '{musicFile}'");
                audioSource.enabled = false;
                return;
            }

            audioSource.volume = info.musicVolume;
            SceneryAudioState.MusicVolume = info.musicVolume;
            Debug.Log($"[SceneLoader] Loading music: '{musicFile}' volume={info.musicVolume:F2}");
            CoroutineRunner.Instance.StartCoroutine(LoadAndPlayAudio(audioSource, musicFile, "music"));
        }

        static string ResolveAudioPath(string audioPath, string bundleFolder)
        {
            if (string.IsNullOrWhiteSpace(audioPath)) return null;

            if (Path.IsPathRooted(audioPath) && File.Exists(audioPath))
                return audioPath;

            if (!string.IsNullOrEmpty(bundleFolder))
            {
                var relative = Path.Combine(bundleFolder, audioPath);
                if (File.Exists(relative)) return relative;
            }

            return audioPath;
        }

        static IEnumerator LoadAndPlayAudio(AudioSource source, string filePath, string label)
        {
            var uri = "file:///" + filePath.Replace('\\', '/');
            using (var www = UnityEngine.Networking.UnityWebRequestMultimedia.GetAudioClip(uri, AudioType.MPEG))
            {
                yield return www.SendWebRequest();
                if (www.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"[SceneLoader] Failed to load {label} '{filePath}': {www.error}");
                    yield break;
                }
                var clip = UnityEngine.Networking.DownloadHandlerAudioClip.GetContent(www);
                clip.name = Path.GetFileNameWithoutExtension(filePath);
                source.clip = clip;
                source.Play();
                Debug.Log($"[SceneLoader] Playing {label}: {clip.name} (volume={source.volume:F2})");
            }
        }

        static void SetupAmbientAudio(GameObject root, AssetInformation info, string bundleFolder)
        {
            bool enabled = info != null && info.ambientAudioEnabled;

            var audioTransform = root.transform.Find("AmbientAudio");
            if (audioTransform == null)
            {
                foreach (Transform child in root.transform)
                {
                    audioTransform = child.Find("AmbientAudio");
                    if (audioTransform != null) break;
                }
            }

            if (audioTransform == null && !enabled)
            {
                SceneryAudioState.AmbientAudioSource = null;
                return;
            }

            if (audioTransform == null)
            {
                var go = new GameObject("AmbientAudio");
                go.transform.SetParent(root.transform, false);
                audioTransform = go.transform;
            }

            var audioSource = audioTransform.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = audioTransform.gameObject.AddComponent<AudioSource>();
                audioSource.loop = true;
                audioSource.spatialBlend = 0f;
            }

            audioSource.Stop();
            audioSource.clip = null;
            audioSource.playOnAwake = false;
            SceneryAudioState.AmbientAudioSource = audioSource;

            if (!enabled)
            {
                audioSource.enabled = false;
                Debug.Log("[SceneLoader] Ambient audio disabled via AssetInformation");
                return;
            }

            string audioFile = ResolveAudioPath(info.ambientAudioPath, bundleFolder);
            if (string.IsNullOrEmpty(audioFile) || !File.Exists(audioFile))
            {
                Debug.LogWarning($"[SceneLoader] Ambient audio file not found: '{audioFile}'");
                audioSource.enabled = false;
                return;
            }

            audioSource.volume = info.ambientAudioVolume;
            SceneryAudioState.AmbientAudioVolume = info.ambientAudioVolume;
            Debug.Log($"[SceneLoader] Loading ambient audio: '{audioFile}' volume={info.ambientAudioVolume:F2}");
            CoroutineRunner.Instance.StartCoroutine(LoadAndPlayAudio(audioSource, audioFile, "ambient audio"));
        }

        static void RebindShadersToGameRuntime(GameObject root)
        {
            try
            {
                int rebound = 0, missed = 0, kept = 0;
                var seen = new HashSet<Material>();
                var missedShaders = new HashSet<string>();
                foreach (var r in root.GetComponentsInChildren<Renderer>(true))
                {
                    foreach (var m in r.sharedMaterials)
                    {
                        if (m == null || !seen.Add(m)) continue;
                        if (m.shader == null) continue;
                        var name = m.shader.name;
                        var fresh = Shader.Find(name);
                        if (fresh == null) { missed++; missedShaders.Add(name); continue; }
                        if (fresh == m.shader) { kept++; continue; }
                        m.shader = fresh;
                        rebound++;
                    }
                }
                Debug.Log($"[SceneLoader] Shader rebind: rebound={rebound} keptSame={kept} missed={missed} missedNames=[{string.Join(", ", missedShaders)}]");
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[SceneLoader] Shader rebind failed: {e.Message}");
            }
        }

        static void DumpStagedRoot(GameObject root)
        {
            try
            {
                var t = root.transform;
                var renderers = root.GetComponentsInChildren<Renderer>(true);
                int enabledRenderers = 0;
                Bounds combined = default;
                bool first = true;
                int magentaShaderCount = 0;
                int nullMatCount = 0;
                var layerHist = new Dictionary<int, int>();
                var shaderHist = new Dictionary<string, int>();
                foreach (var r in renderers)
                {
                    if (r.enabled && r.gameObject.activeInHierarchy) enabledRenderers++;
                    if (first) { combined = r.bounds; first = false; }
                    else combined.Encapsulate(r.bounds);

                    int layer = r.gameObject.layer;
                    if (!layerHist.ContainsKey(layer)) layerHist[layer] = 0;
                    layerHist[layer]++;

                    foreach (var m in r.sharedMaterials)
                    {
                        if (m == null) { nullMatCount++; continue; }
                        var sh = m.shader;
                        var sname = sh?.name ?? "<null>";
                        if (!shaderHist.ContainsKey(sname)) shaderHist[sname] = 0;
                        shaderHist[sname]++;
                        if (sh == null || sname == "Hidden/InternalErrorShader" || sname.StartsWith("Hidden/"))
                            magentaShaderCount++;
                    }
                }
                Debug.Log($"[SceneLoader] Staged root '{root.name}' active={root.activeSelf}/{root.activeInHierarchy} pos={t.position} scale={t.lossyScale} renderers={renderers.Length} enabled={enabledRenderers} bounds={(first ? "n/a" : combined.ToString())} nullMats={nullMatCount} brokenShaders={magentaShaderCount}");
                Debug.Log($"[SceneLoader] Container '{root.transform.parent?.name}' active={root.transform.parent?.gameObject.activeInHierarchy} scene='{root.scene.name}'");

                // Layer histogram + names
                var layerStr = string.Join(", ", layerHist.OrderByDescending(kv => kv.Value).Select(kv => $"{kv.Key}({LayerMask.LayerToName(kv.Key)})={kv.Value}"));
                Debug.Log($"[SceneLoader] Renderer layer histogram: {layerStr}");

                // Shader histogram
                var shaderStr = string.Join(", ", shaderHist.OrderByDescending(kv => kv.Value).Take(8).Select(kv => $"'{kv.Key}'={kv.Value}"));
                Debug.Log($"[SceneLoader] Top shaders in use: {shaderStr}");

                // Cameras: which layers do they render? Frustum?
                foreach (var cam in UnityEngine.Object.FindObjectsByType<Camera>(FindObjectsSortMode.None))
                {
                    Debug.Log($"[SceneLoader] Camera '{cam.name}' enabled={cam.enabled} cullingMask=0x{cam.cullingMask:X8} far={cam.farClipPlane} pos={cam.transform.position}");
                    // For each layer the renderers use, say whether this camera renders it
                    foreach (var kv in layerHist)
                    {
                        bool sees = (cam.cullingMask & (1 << kv.Key)) != 0;
                        if (!sees)
                            Debug.LogWarning($"[SceneLoader]   Camera '{cam.name}' does NOT render layer {kv.Key} ({LayerMask.LayerToName(kv.Key)}) — {kv.Value} renderers there");
                    }
                }

                // First few children
                int childDump = 0;
                foreach (Transform c in t)
                {
                    if (childDump++ >= 8) break;
                    var rends = c.GetComponentsInChildren<Renderer>(true).Length;
                    Debug.Log($"[SceneLoader]   child[{childDump - 1}] '{c.name}' active={c.gameObject.activeInHierarchy} layer={c.gameObject.layer}({LayerMask.LayerToName(c.gameObject.layer)}) pos={c.position} scale={c.lossyScale} rendersInTree={rends}");
                }

                // ShadowCastingMode + forceRenderingOff histograms
                var shadowHist = new Dictionary<UnityEngine.Rendering.ShadowCastingMode, int>();
                int forceOffCount = 0;
                int nullMeshCount = 0;
                int zeroVertCount = 0;
                int meshFilterCount = 0;
                foreach (var r in renderers)
                {
                    var mode = r.shadowCastingMode;
                    if (!shadowHist.ContainsKey(mode)) shadowHist[mode] = 0;
                    shadowHist[mode]++;
                    if (r.forceRenderingOff) forceOffCount++;

                    var mf = r.GetComponent<MeshFilter>();
                    if (mf != null)
                    {
                        meshFilterCount++;
                        if (mf.sharedMesh == null) nullMeshCount++;
                        else if (mf.sharedMesh.vertexCount == 0) zeroVertCount++;
                    }
                }
                var shadowStr = string.Join(", ", shadowHist.Select(kv => $"{kv.Key}={kv.Value}"));
                Debug.Log($"[SceneLoader] ShadowCastingMode histogram: {shadowStr}; forceRenderingOff={forceOffCount}; meshFilters={meshFilterCount} nullMesh={nullMeshCount} zeroVerts={zeroVertCount}");

                // Sample a few individual mesh renderers at world position so we can see WHERE they are
                int sampleN = Math.Min(5, renderers.Length);
                for (int i = 0; i < sampleN; i++)
                {
                    var r = renderers[i];
                    var mf = r.GetComponent<MeshFilter>();
                    var mesh = mf != null ? mf.sharedMesh : null;
                    var matName = r.sharedMaterial != null ? r.sharedMaterial.name : "<null>";
                    var shaderName = r.sharedMaterial != null && r.sharedMaterial.shader != null ? r.sharedMaterial.shader.name : "<null>";
                    Debug.Log($"[SceneLoader] Sample[{i}] '{r.name}' worldPos={r.transform.position} bounds={r.bounds} mesh='{(mesh != null ? mesh.name : "<null>")}' verts={(mesh != null ? mesh.vertexCount : 0)} mat='{matName}' shader='{shaderName}' enabled={r.enabled} shadowMode={r.shadowCastingMode}");
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[SceneLoader] DumpStagedRoot failed: {e.Message}");
            }
        }

        static IEnumerator SpawnSpectatorsFromStagedRoot(GameObject stagedRoot, int token)
        {
            Debug.Log($"[SceneLoader] Spectators: entry (token={token}, currentToken={RinkSceneState.CurrentLoadToken}, stagedRoot={(stagedRoot != null ? stagedRoot.name : "<null>")})");
            if (token != RinkSceneState.CurrentLoadToken) yield break;
            if (stagedRoot == null) { Debug.LogWarning("[SceneLoader] Spectators: staged root is null"); yield break; }

            yield return null; // let one frame settle so children are awake

            var spectatorPositionType = AccessTools.TypeByName("SpectatorPosition");
            if (spectatorPositionType == null)
            {
                Debug.LogWarning("[SceneLoader] Type 'SpectatorPosition' not found at runtime; skipping spectators.");
                yield break;
            }

            // Try to bump spectator density via reflection (don't touch SpectatorManager.Instance directly —
            // MonoBehaviourSingleton<T>.Instance has been seen to throw TypeLoadException at runtime).
            try
            {
                var mgrType = AccessTools.TypeByName("SpectatorManager");
                if (mgrType != null)
                {
                    var mgrObj = UnityEngine.Object.FindFirstObjectByType(mgrType);
                    if (mgrObj != null)
                        Traverse.Create(mgrObj).Field("spectatorDensity").SetValue(1.0f);
                }
            }
            catch (Exception e) { Debug.LogWarning($"[SceneLoader] Could not set spectatorDensity: {e.Message}"); }

            // Walk the entire staged prefab and find any transform whose name starts with "Spectator"
            // (excluding organizational parents named with Column/Row).
            var allTransforms = stagedRoot.GetComponentsInChildren<Transform>(true);
            var spots = new List<Transform>();
            var nameSamples = new List<string>();
            foreach (var tr in allTransforms)
            {
                if (tr.name.StartsWith("Spectator", StringComparison.OrdinalIgnoreCase) &&
                    !tr.name.Contains("Column") && !tr.name.Contains("Row") &&
                    !tr.name.Contains("SpectatorList", StringComparison.OrdinalIgnoreCase) &&
                    !tr.name.Contains("SpectatorLocations", StringComparison.OrdinalIgnoreCase))
                {
                    spots.Add(tr);
                }
                else if (tr.name.IndexOf("spec", StringComparison.OrdinalIgnoreCase) >= 0 ||
                         tr.name.IndexOf("audience", StringComparison.OrdinalIgnoreCase) >= 0 ||
                         tr.name.IndexOf("fan", StringComparison.OrdinalIgnoreCase) >= 0 ||
                         tr.name.IndexOf("seat", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    if (nameSamples.Count < 20) nameSamples.Add(tr.name);
                }
            }

            Debug.Log($"[SceneLoader] Spectators: scanned {allTransforms.Length} transforms, found {spots.Count} 'Spectator*' spots");
            if (spots.Count == 0)
            {
                if (nameSamples.Count > 0)
                    Debug.LogWarning($"[SceneLoader] No 'Spectator*' spots — possible alternate names in bundle: [{string.Join(", ", nameSamples)}]");
                else
                    Debug.LogWarning("[SceneLoader] No spectator-like transforms found in bundle. Bundle may not contain spectator markers.");
                yield break;
            }

            int tagged = 0, skipped = 0;
            foreach (var child in spots)
            {
                if (child.GetComponent(spectatorPositionType) != null) { skipped++; continue; }

                var plane = child.Find("Plane");
                if (plane != null) UnityEngine.Object.Destroy(plane.gameObject);

                child.gameObject.AddComponent(spectatorPositionType);
                tagged++;
            }
            Debug.Log($"[SceneLoader] Spectators: tagged={tagged} alreadyTagged={skipped}");
        }

        static IEnumerator SpawnSpectatorsWhenLocationsReady(int token, float timeoutSec)
        {
            float t0 = Time.realtimeSinceStartup;
            Transform locations = null;

            while (token == RinkSceneState.CurrentLoadToken &&
                   locations == null &&
                   Time.realtimeSinceStartup - t0 < timeoutSec)
            {
                // robust search including inactive:
                var all = Resources.FindObjectsOfTypeAll<Transform>();
                locations = all.FirstOrDefault(t => t && t.name == "SpectatorLocations");
                if (!locations) yield return null;
            }

            if (token != RinkSceneState.CurrentLoadToken) yield break; // superseded
            if (!locations)
            {
                Debug.LogWarning("[SceneLoader] Timed out waiting for SpectatorLocations; skipping spectators.");
                yield break;
            }

            yield return null; // let children finish Awake/Start

            // Use reflection to avoid MonoBehaviourSingleton<T>.Instance generic inflation TLE.
            var smType = AccessTools.TypeByName("SpectatorManager");
            var mgr = smType != null ? UnityEngine.Object.FindFirstObjectByType(smType) : null;
            if (!mgr) { Debug.LogWarning("[SceneLoader] SpectatorManager missing."); yield break; }

            // Bump density so most positions actually get a spectator (default is 0.25)
            try { Traverse.Create(mgr).Field("spectatorDensity").SetValue(1.0f); }
            catch (Exception e) { Debug.LogWarning($"[SceneLoader] Could not set spectatorDensity: {e.Message}"); }

            // PuckNew: spectators register themselves via SpectatorPosition.Start() ->
            // Event_OnSpectatorPositionSpawned -> SpectatorManager.RegisterSpectatorPosition.
            // The bundle's spots are plain Transforms named "Spectator*"; tag each with a
            // SpectatorPosition component and the game does the rest.
            // Resolve via reflection because SpectatorPosition isn't visible at compile time.
            var spectatorPositionType = AccessTools.TypeByName("SpectatorPosition");
            if (spectatorPositionType == null)
            {
                Debug.LogWarning("[SceneLoader] Type 'SpectatorPosition' not found at runtime; skipping spectators.");
                yield break;
            }

            int tagged = 0, skipped = 0;
            foreach (var child in locations.GetComponentsInChildren<Transform>(true))
            {
                if (child == locations) continue;
                if (!child.name.StartsWith("Spectator")) continue;
                if (child.name.Contains("Column") || child.name.Contains("Row")) continue;
                if (child.GetComponent(spectatorPositionType) != null) { skipped++; continue; }

                var plane = child.Find("Plane");
                if (plane != null) UnityEngine.Object.Destroy(plane.gameObject);

                child.gameObject.AddComponent(spectatorPositionType);
                tagged++;
            }
            Debug.Log($"[SceneLoader] Spectators: tagged={tagged} alreadyTagged={skipped}");
        }
        static void ClearLegacyOutsideContainer()
        {
            DestroyIfExistsByName("SpectatorList");
            DestroyIfExistsByName("SpectatorLocations"); // default markers                                                   // add others you know about (e.g., “HangarRoot”) if RemoveHangar doesn’t catch them
        }

        static void DestroyIfExistsByName(string name)
        {
            var all = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (var go in all)
                if (go && go.name == name && go.scene.IsValid())
                    UnityEngine.Object.Destroy(go);
        }
    }
}