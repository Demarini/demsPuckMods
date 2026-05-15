using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace PuckAIPractice.Scenarios.Environment
{
    // Hides rink scenery and builds a long narrow corridor for the gauntlet:
    // floor + 4 walls (20m wide, 2000m long, 4m tall). Floor and walls reuse
    // Hangar materials so the player has visual reference for stickhandling,
    // with texture tiling scaled to the cube dimensions.
    public class RinkVoid
    {
        private static readonly Regex[] DisablePatterns =
        {
            new Regex(@"^Rink$",                               RegexOptions.IgnoreCase),
            new Regex(@"^Hangar$",                             RegexOptions.IgnoreCase),
            new Regex(@"^Goal\s+(Blue|Red)$",                  RegexOptions.IgnoreCase),
            new Regex(@"^Goal\s+Lights$",                      RegexOptions.IgnoreCase),
            new Regex(@"^Sounds$",                             RegexOptions.IgnoreCase),
            new Regex(@"^Reflection\s+Probe$",                 RegexOptions.IgnoreCase),
            new Regex(@"^Spectator\s+Camera\s+Spline$",        RegexOptions.IgnoreCase),
            new Regex(@"^Spectator\s+Booth(\s*\d+)?$",         RegexOptions.IgnoreCase),
            new Regex(@"^Spectator(\(Clone\))?$",              RegexOptions.IgnoreCase),
            new Regex(@"^spectator_booth.*",                   RegexOptions.IgnoreCase),
            new Regex(@"^Scoreboard(\s+(Blue|Red))?(\s*\(\d+\))?$", RegexOptions.IgnoreCase),
            new Regex(@"^scoreboard$",                         RegexOptions.IgnoreCase),
            new Regex(@"^Crowd.*",                             RegexOptions.IgnoreCase),
        };

        private static readonly Regex[] HardKeepPatterns =
        {
            new Regex(@"^Lights$",                             RegexOptions.IgnoreCase),
            new Regex(@".*Camera.*",                           RegexOptions.IgnoreCase),
            new Regex(@".*Post\s+Processing.*",                RegexOptions.IgnoreCase),
        };

        private static readonly Regex[] ExternalDisablePatterns =
        {
            new Regex(@"^Spectator\s*Manager$",                RegexOptions.IgnoreCase),
        };

        // Arena dimensions. Player spawns at (0, iceY, 0) facing +Z.
        // Floor extends from Z = -BackPadding to Z = ForwardLength.
        public const float ArenaWidth = 20f;
        public const float ForwardLength = 1950f;
        public const float BackPadding = 50f;
        private const float FloorThickness = 1f;
        private const float WallHeight = 4f;
        private const float WallThickness = 1f;
        // Invisible ceiling-height collider extensions sitting in the same
        // X/Z slots as the visible walls so the puck can't escape over the
        // top on hard shots or deflections.
        private const float ContainmentHeight = 200f;
        // Real-world meters per one repeat of the source texture. Smaller =
        // denser tiling, more reference detail. The floor uses a coarser
        // tile so the long lines don't read as a busy carpet at high speed.
        private const float WallTileMeters = 4f;
        private const float FloorTileMeters = 4f;

        private readonly List<GameObject> _disabled = new List<GameObject>();
        private readonly List<GameObject> _created = new List<GameObject>();
        private bool _active;

        public bool IsActive => _active;

        public void Enter(float iceY)
        {
            if (_active)
            {
                Debug.LogWarning("[RinkVoid] Enter called while already active; ignoring");
                return;
            }

            // Sample hangar materials BEFORE disabling — we want the donor
            // material's _BaseMap texture, then we clone the material and only
            // override tiling so the original asset isn't mutated. The 2nd
            // sampled material reads better as the floor (the 1st's repeating
            // long lines were disorienting on the floor), so we swap them.
            var (firstMat, secondMat) = SampleHangarMaterials();
            var floorMat = secondMat;
            var wallMat = firstMat;

            var levelRoots = FindLevelRoots();
            int matched = 0;
            foreach (var root in levelRoots)
            {
                var descendants = root.GetComponentsInChildren<Transform>(true);
                foreach (var tr in descendants)
                {
                    if (tr == null || tr.gameObject == null) continue;
                    if (tr.gameObject == root) continue;
                    if (!tr.gameObject.activeSelf) continue;
                    if (!DisablePatterns.Any(rx => rx.IsMatch(tr.name))) continue;
                    if (HasHardKeepAncestor(tr)) continue;

                    matched++;
                    tr.gameObject.SetActive(false);
                    _disabled.Add(tr.gameObject);
                }
            }

            // Second pass: scenery managers outside Level Default (e.g.
            // "Spectator Manager" spawns Spectator(Clone) skater meshes at
            // runtime). Narrow exact-name match to avoid catching Player/Puck.
            var allTransforms = Object.FindObjectsOfType<Transform>();
            foreach (var tr in allTransforms)
            {
                if (tr == null || tr.gameObject == null) continue;
                if (!tr.gameObject.activeSelf) continue;
                if (!ExternalDisablePatterns.Any(rx => rx.IsMatch(tr.name))) continue;

                matched++;
                tr.gameObject.SetActive(false);
                _disabled.Add(tr.gameObject);
            }

            BuildArena(iceY, floorMat, wallMat);
            _active = true;
            Debug.Log($"[RinkVoid] Active. disabled={_disabled.Count} created={_created.Count} floorY={iceY:F3}");
        }

        public void Exit()
        {
            if (!_active) return;

            foreach (var go in _created)
            {
                if (go != null) UnityEngine.Object.Destroy(go);
            }
            _created.Clear();

            foreach (var go in _disabled)
            {
                if (go != null) go.SetActive(true);
            }
            _disabled.Clear();
            _active = false;
            Debug.Log("[RinkVoid] Exited");
        }

        private void BuildArena(float iceY, Material floorMat, Material wallMat)
        {
            int iceLayer = LayerMask.NameToLayer("Ice");
            if (iceLayer < 0)
            {
                Debug.LogWarning("[RinkVoid] Layer 'Ice' not found; arena pieces stay on Default and may not collide correctly");
            }

            float totalLength = ForwardLength + BackPadding;
            float floorCenterZ = (ForwardLength - BackPadding) * 0.5f; // center between -BackPadding and +ForwardLength

            // Floor: 20m wide × totalLength long × 1m thick. Top surface at iceY.
            BuildSlab(
                "PuckAIPractice_GauntletFloor",
                new Vector3(0f, iceY - FloorThickness * 0.5f, floorCenterZ),
                new Vector3(ArenaWidth, FloorThickness, totalLength),
                new Vector2(ArenaWidth / FloorTileMeters, totalLength / FloorTileMeters),
                floorMat,
                iceLayer);

            float wallCenterY = iceY + WallHeight * 0.5f;
            float halfWidth = ArenaWidth * 0.5f;

            // Left wall along -X, runs the full length.
            BuildSlab(
                "PuckAIPractice_GauntletWallLeft",
                new Vector3(-halfWidth - WallThickness * 0.5f, wallCenterY, floorCenterZ),
                new Vector3(WallThickness, WallHeight, totalLength),
                new Vector2(totalLength / WallTileMeters, WallHeight / WallTileMeters),
                wallMat,
                iceLayer);

            // Right wall along +X.
            BuildSlab(
                "PuckAIPractice_GauntletWallRight",
                new Vector3(halfWidth + WallThickness * 0.5f, wallCenterY, floorCenterZ),
                new Vector3(WallThickness, WallHeight, totalLength),
                new Vector2(totalLength / WallTileMeters, WallHeight / WallTileMeters),
                wallMat,
                iceLayer);

            // Back wall at -Z end.
            BuildSlab(
                "PuckAIPractice_GauntletWallBack",
                new Vector3(0f, wallCenterY, -BackPadding - WallThickness * 0.5f),
                new Vector3(ArenaWidth + WallThickness * 2f, WallHeight, WallThickness),
                new Vector2(ArenaWidth / WallTileMeters, WallHeight / WallTileMeters),
                wallMat,
                iceLayer);

            // Front wall at +Z end.
            BuildSlab(
                "PuckAIPractice_GauntletWallFront",
                new Vector3(0f, wallCenterY, ForwardLength + WallThickness * 0.5f),
                new Vector3(ArenaWidth + WallThickness * 2f, WallHeight, WallThickness),
                new Vector2(ArenaWidth / WallTileMeters, WallHeight / WallTileMeters),
                wallMat,
                iceLayer);

            // Invisible tall colliders in the same X/Z positions as the visible
            // walls. The visible walls only collide up to 4m; these extend
            // collision up to ContainmentHeight so a deflected puck can't fly
            // over them. Overlap with the visible walls is fine.
            float containmentCenterY = iceY + ContainmentHeight * 0.5f;

            BuildInvisibleSlab(
                "PuckAIPractice_GauntletContainmentLeft",
                new Vector3(-halfWidth - WallThickness * 0.5f, containmentCenterY, floorCenterZ),
                new Vector3(WallThickness, ContainmentHeight, totalLength),
                iceLayer);

            BuildInvisibleSlab(
                "PuckAIPractice_GauntletContainmentRight",
                new Vector3(halfWidth + WallThickness * 0.5f, containmentCenterY, floorCenterZ),
                new Vector3(WallThickness, ContainmentHeight, totalLength),
                iceLayer);

            BuildInvisibleSlab(
                "PuckAIPractice_GauntletContainmentBack",
                new Vector3(0f, containmentCenterY, -BackPadding - WallThickness * 0.5f),
                new Vector3(ArenaWidth + WallThickness * 2f, ContainmentHeight, WallThickness),
                iceLayer);

            BuildInvisibleSlab(
                "PuckAIPractice_GauntletContainmentFront",
                new Vector3(0f, containmentCenterY, ForwardLength + WallThickness * 0.5f),
                new Vector3(ArenaWidth + WallThickness * 2f, ContainmentHeight, WallThickness),
                iceLayer);
        }

        private GameObject BuildInvisibleSlab(string name, Vector3 center, Vector3 scale, int layer)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.position = center;
            go.transform.localScale = scale;
            if (layer >= 0) go.layer = layer;
            var r = go.GetComponent<MeshRenderer>();
            if (r != null) r.enabled = false;
            _created.Add(go);
            return go;
        }

        private GameObject BuildSlab(string name, Vector3 center, Vector3 scale, Vector2 tiling, Material srcMat, int layer)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.position = center;
            go.transform.localScale = scale;
            if (layer >= 0) go.layer = layer;
            _created.Add(go);

            var renderer = go.GetComponent<MeshRenderer>();
            if (renderer == null) return go;

            Material mat = null;
            if (srcMat != null)
            {
                mat = new Material(srcMat);
            }
            else
            {
                var shader = Shader.Find("Universal Render Pipeline/Lit")
                    ?? Shader.Find("Universal Render Pipeline/Simple Lit");
                if (shader != null)
                {
                    mat = new Material(shader);
                    var gray = new Color(0.55f, 0.58f, 0.62f);
                    if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", gray);
                    if (mat.HasProperty("_Color")) mat.SetColor("_Color", gray);
                }
            }

            if (mat == null)
            {
                Debug.LogWarning($"[RinkVoid] No usable material for '{name}'; hiding renderer (collider still active)");
                renderer.enabled = false;
                return go;
            }

            if (mat.HasProperty("_BaseMap")) mat.SetTextureScale("_BaseMap", tiling);
            if (mat.HasProperty("_MainTex")) mat.SetTextureScale("_MainTex", tiling);
            renderer.material = mat;
            return go;
        }

        // Walk the Hangar subtree for two distinct URP materials with an
        // assigned _BaseMap. Returns (floor, wall). Sampling before the
        // disable pass keeps both renderer instance and material asset alive.
        private static (Material floor, Material wall) SampleHangarMaterials()
        {
            Material first = null, second = null;
            var hangars = Object.FindObjectsOfType<Transform>()
                .Where(t => t != null && t.name.Equals("Hangar", System.StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var hangar in hangars)
            {
                var renderers = hangar.GetComponentsInChildren<MeshRenderer>(true);
                foreach (var r in renderers)
                {
                    if (r == null || r.sharedMaterial == null) continue;
                    var src = r.sharedMaterial;
                    var shader = src.shader;
                    if (shader == null) continue;
                    if (!shader.name.StartsWith("Universal Render Pipeline", System.StringComparison.OrdinalIgnoreCase)) continue;

                    // Prefer materials that actually have a texture — otherwise
                    // we'll get a flat color which is no better than gray.
                    Texture baseMap = src.HasProperty("_BaseMap") ? src.GetTexture("_BaseMap") : null;
                    if (baseMap == null && src.HasProperty("_MainTex")) baseMap = src.GetTexture("_MainTex");
                    if (baseMap == null) continue;

                    Debug.Log($"[RinkVoid] Hangar material '{src.name}' shader='{shader.name}' on '{BuildPath(r.transform)}'");

                    if (first == null) { first = src; continue; }
                    if (second == null && src != first) { second = src; }
                    if (first != null && second != null) break;
                }
                if (first != null && second != null) break;
            }

            if (first == null) Debug.LogWarning("[RinkVoid] No hangar URP material with a base texture found; falling back to flat gray");
            return (first, second ?? first);
        }

        private static List<GameObject> FindLevelRoots()
        {
            var result = new List<GameObject>();
            int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCount;
            for (int i = 0; i < sceneCount; i++)
            {
                var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                if (!scene.IsValid() || !scene.isLoaded) continue;
                foreach (var go in scene.GetRootGameObjects())
                {
                    if (go == null) continue;
                    if (!go.name.StartsWith("Level", System.StringComparison.OrdinalIgnoreCase)) continue;
                    if (go.name.IndexOf("Post Processing", System.StringComparison.OrdinalIgnoreCase) >= 0) continue;
                    result.Add(go);
                }
            }
            return result;
        }

        private static bool HasHardKeepAncestor(Transform tr)
        {
            var t = tr;
            while (t != null)
            {
                if (HardKeepPatterns.Any(rx => rx.IsMatch(t.name))) return true;
                t = t.parent;
            }
            return false;
        }

        private static string BuildPath(Transform tr)
        {
            var parts = new List<string>();
            var t = tr;
            while (t != null)
            {
                parts.Add(t.name);
                t = t.parent;
            }
            parts.Reverse();
            return string.Join("/", parts);
        }
    }
}
