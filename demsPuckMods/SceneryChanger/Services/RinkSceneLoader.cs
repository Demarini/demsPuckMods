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
            }
            else
            {
                // default if no file present
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
            
            // Optional cleanup of stray legacy objects AFTER swap
            //ClearLegacyOutsideContainer(); // see helper below (non-blocking)
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