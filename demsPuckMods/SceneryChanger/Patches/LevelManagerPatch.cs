using HarmonyLib;
using SceneryChanger.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace SceneryChanger.Patches
{
    // PuckNew renamed LevelManagerController → LevelController and changed the event from
    // Event_OnGamePhaseChanged (raw GamePhase) to Event_Everyone_OnGameStateChanged (GameState objects).
    // TargetMethod() resolves the target at runtime so this compiles against old libs too;
    // if LevelController doesn't exist the patch silently skips.
    [HarmonyPatch]
    class Patch_LevelController
    {
        static bool? _lastOn;
        static GameObject _arenaLights;

        static MethodBase TargetMethod()
        {
            var type = AccessTools.TypeByName("LevelController");
            return type == null ? null : AccessTools.Method(type, "Event_Everyone_OnGameStateChanged");
        }

        static void Postfix(object __instance, Dictionary<string, object> eventParams)
        {
            var nm = NetworkManager.Singleton;
            Debug.Log($"[Patch] GAME STATE CHANGED (IsClient={nm?.IsClient}, IsServer={nm?.IsServer}, IsHost={nm?.IsHost})");

            if (nm != null && !nm.IsClient) return;

            if (eventParams == null || !eventParams.TryGetValue("newGameState", out var raw))
            {
                Debug.LogWarning("[Patch] newGameState missing");
                return;
            }

            if (!(raw is GameState newState))
            {
                Debug.LogWarning($"[Patch] newGameState unexpected type: {raw?.GetType()}");
                return;
            }

            GamePhase gamePhase = newState.Phase;

            Debug.Log("[Patch] GAME STATE CHANGED 2");
            bool isCheering = gamePhase == GamePhase.BlueScore
                           || gamePhase == GamePhase.RedScore
                           || gamePhase == GamePhase.GameOver;

            if (isCheering)
                Debug.Log("[Patch] SOMEONE SCORED A GOAL");

            ToggleCheer(isCheering);

            bool turnOn = gamePhase == GamePhase.BlueScore
                       || gamePhase == GamePhase.RedScore
                       || gamePhase == GamePhase.Warmup
                       || gamePhase == GamePhase.FaceOff;

            if (_lastOn.HasValue && _lastOn.Value == turnOn) return;
            _lastOn = turnOn;

            if (_arenaLights == null)
                _arenaLights = ArenaLightUtil.FindArenaLights();

            if (_arenaLights == null) return;

            if (_arenaLights.activeSelf != turnOn)
                _arenaLights.SetActive(turnOn);
        }
        public static AudioSource FindGoalCrowdSource()
        {
            // Look for HockeyArenaRoot -> GoalCrowdNoise
            var root = UnityEngine.Object.FindObjectsOfType<Transform>(true)
                             .FirstOrDefault(t => t.name.Contains("HockeyArenaRoot"));
            if (root == null)
            {
                Debug.Log("Could not find arena root");
                return null;
            }

            var noise = root.GetComponentsInChildren<Transform>(true)
                            .FirstOrDefault(t => t.name == "GoalCrowdNoise");
            if (noise == null)
            {
                Debug.Log("Could not find crowd noise");
                return null;
            }
            return noise.GetComponent<AudioSource>();
        }
        
        public static void ToggleCheer(bool on)
        {
            var src = FindGoalCrowdSource();
            if (src == null) return;

            // If you literally want to flip the GameObject:
            // src.gameObject.SetActive(on);

            // Or just control the AudioSource itself:
            if (on)
            {
                src.gameObject.SetActive(true);
                src.enabled = true;
                src.Play();
            }
            else
            {
                src.Stop();
                src.gameObject.SetActive(false);
                src.enabled = false;
            }
        }
    }

}
