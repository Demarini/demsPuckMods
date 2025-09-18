using HarmonyLib;
using SceneryChanger.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace SceneryChanger.Patches
{
    [HarmonyPatch(typeof(LevelManagerController))]
    class Patch_LevelManagerController
    {
        static bool? _lastOn;
        static GameObject _arenaLights;
        [HarmonyPostfix]
        [HarmonyPatch(typeof(LevelManagerController), "Event_OnGamePhaseChanged")]
        static void Postfix(LevelManagerController __instance, Dictionary<string, object> message)
        {
            var nm = NetworkManager.Singleton;
            Debug.Log($"[Patch] PHASE CHANGED (IsClient={nm?.IsClient}, IsServer={nm?.IsServer}, IsHost={nm?.IsHost})");

            // --- choose ONE of these guards ---

            // A) Run on any client *including host* (good for local testing)
            if (nm != null && !nm.IsClient) return;

            // B) If you truly want *pure clients only*, use this instead:
            // if (nm != null && (!nm.IsClient || nm.IsServer)) return;

            if (message == null || !message.TryGetValue("newGamePhase", out var raw))
            {
                Debug.LogWarning("[Patch] newGamePhase missing");
                return;
            }

            GamePhase gamePhase;
            if (raw is GamePhase gp) gamePhase = gp;
            else if (raw is int i) gamePhase = (GamePhase)i;
            else if (raw is byte b) gamePhase = (GamePhase)b;
            else
            {
                Debug.LogWarning($"[Patch] newGamePhase unexpected type: {raw.GetType()}");
                return;
            }

            Debug.Log("[Patch] PHASE CHANGED 2");

            switch (gamePhase)
            {
                case GamePhase.BlueScore:
                case GamePhase.RedScore:
                case GamePhase.GameOver:
                    Debug.Log("[Patch] SOMEONE SCORED A GOAL");
                    ToggleCheer(true);
                    break;

                case GamePhase.Warmup:
                case GamePhase.PeriodOver:
                    // ToggleCheer(false);
                    break;
            }
            bool turnOn = gamePhase == GamePhase.BlueScore
                       || gamePhase == GamePhase.RedScore
                       || gamePhase == GamePhase.Warmup
                       || gamePhase == GamePhase.FaceOff;

            // Only do work if the desired state changed
            if (_lastOn.HasValue && _lastOn.Value == turnOn) return;
            _lastOn = turnOn;

            // Find (or reuse cached) ArenaLights
            if (_arenaLights == null)
                _arenaLights = SceneryChanger.Helpers.ArenaLightUtil.FindArenaLights();

            if (_arenaLights == null)
            {
                // Not in scene yet — skip quietly
                // Debug.Log("[DetectGameState] ArenaLights not found yet.");
                return;
            }

            if (_arenaLights.activeSelf != turnOn)
            {
                _arenaLights.SetActive(turnOn);
                // Debug.Log($"[DetectGameState] ArenaLights -> {(turnOn ? "ON" : "OFF")}");
            }
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
