using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using UnityEngine;

namespace PlayerStickRadius
{
    public class Class1 : IPuckMod
    {
        static readonly Harmony harmony = new Harmony("GAFURIX.PlayerStickRadius");

        public bool OnEnable()
        {
            //ModConfig.Initialize();   // Ensures config file exists in <Puck>/config/
            //ConfigData.Load();        // Loads JSON into the singleton (ConfigData.Instance)
            try
            {
                // Patched all functions we have defined to be patched
                harmony.PatchAll();
            }
            catch (Exception e)
            {
                Debug.LogError($"Harmony patch failed: {e.Message}");

                return false;
            }

            return true;
        }

        public bool OnDisable()
        {
            try
            {
                // Reverts our patches, essentially returning the game to a vanilla state
                harmony.UnpatchSelf();
            }
            catch (Exception e)
            {
                Debug.LogError($"Harmony unpatch failed: {e.Message}");

                return false;
            }

            return true;
        }
    }
    [HarmonyPatch]
    public static class PlayerGroundIndicatorManager
    {
        public static Dictionary<ulong, GameObject> playerIndicators = new Dictionary<ulong, GameObject>();
        private static Mesh discMesh;

        public static readonly float DiscRadius = 2.5f;
        public static readonly int DiscSegments = 32;
        public static readonly float DiscHeight = 0.03f; // Locks just above ice

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerBodyV2), "OnNetworkSpawn")]
        public static void OnPlayerSpawn(PlayerBodyV2 __instance)
        {
            if (!__instance || playerIndicators.ContainsKey(__instance.OwnerClientId))
                return;

            if (discMesh == null)
                discMesh = CreateDiscMeshXZ(DiscRadius, DiscSegments);

            var indicator = new GameObject($"PlayerIndicator_{__instance.OwnerClientId}");
            indicator.transform.position = new Vector3(
                __instance.transform.position.x,
                DiscHeight,
                __instance.transform.position.z
            );
            indicator.transform.rotation = Quaternion.identity;
            indicator.transform.SetParent(null, true); // not parented to player

            var mf = indicator.AddComponent<MeshFilter>();
            mf.mesh = discMesh;

            var mr = indicator.AddComponent<MeshRenderer>();

            var mat = new Material(Shader.Find("Sprites/Default"));
            mat.color = new Color(1f, 0f, 0f, 0.5f); // semi-transparent red
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always); // ignore floor depth
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

            mr.material = mat;

            playerIndicators[__instance.OwnerClientId] = indicator;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerBodyV2), "OnNetworkDespawn")]
        public static void OnPlayerDespawn(PlayerBodyV2 __instance)
        {
            ulong id = __instance.OwnerClientId;
            if (playerIndicators.TryGetValue(id, out var indicator))
            {
                UnityEngine.Object.Destroy(indicator);
                playerIndicators.Remove(id);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerBodyV2), "FixedUpdate")]
        public static void OnPlayerFixedUpdate(PlayerBodyV2 __instance)
        {
            if (!playerIndicators.TryGetValue(__instance.OwnerClientId, out var disc))
                return;

            // Follow player X/Z, but lock to ice height
            Vector3 pos = __instance.transform.position;
            pos.y = DiscHeight;
            disc.transform.position = pos;

            // Always flat, no tilt with player lean
            disc.transform.rotation = Quaternion.identity;
        }

        private static Mesh CreateDiscMeshXZ(float radius, int segments)
        {
            Mesh mesh = new Mesh();

            Vector3[] vertices = new Vector3[segments + 1];
            int[] triangles = new int[segments * 3];

            vertices[0] = Vector3.zero;

            for (int i = 0; i < segments; i++)
            {
                float angle = (float)i / segments * Mathf.PI * 2f;
                vertices[i + 1] = new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
            }

            for (int i = 0; i < segments; i++)
            {
                int current = i + 1;
                int next = (i + 1) % segments + 1;

                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = next;
                triangles[i * 3 + 2] = current;
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            return mesh;
        }
    }  
}
