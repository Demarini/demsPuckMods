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
        static bool _arenaLightsSearched;
        static AudioSource _cachedCrowdSource;
        static bool _crowdSourceSearched;

        static Type _gameStateType;
        static MethodInfo _phaseGetter;
        static bool _reflectionInit;

        static MethodBase TargetMethod()
        {
            var type = AccessTools.TypeByName("LevelController");
            return type == null ? null : AccessTools.Method(type, "Event_Everyone_OnGameStateChanged");
        }

        static void InitReflection()
        {
            if (_reflectionInit) return;
            _reflectionInit = true;
            _gameStateType = AccessTools.TypeByName("GameState");
            if (_gameStateType != null)
                _phaseGetter = AccessTools.PropertyGetter(_gameStateType, "Phase");
        }

        static void Postfix(object __instance, Dictionary<string, object> eventParams)
        {
            var nm = NetworkManager.Singleton;
            if (nm != null && !nm.IsClient) return;

            if (eventParams == null || !eventParams.TryGetValue("newGameState", out var raw))
            {
                Debug.LogWarning("[Patch] newGameState missing");
                return;
            }

            InitReflection();
            if (_gameStateType == null || _phaseGetter == null) return;

            if (raw == null || !_gameStateType.IsInstanceOfType(raw))
            {
                Debug.LogWarning($"[Patch] newGameState unexpected type: {raw?.GetType()}");
                return;
            }

            GamePhase gamePhase = (GamePhase)_phaseGetter.Invoke(raw, null);

            bool isCheering = gamePhase == GamePhase.BlueScore
                           || gamePhase == GamePhase.RedScore
                           || gamePhase == GamePhase.GameOver;

            ToggleCheer(isCheering);

            bool turnOn = gamePhase == GamePhase.BlueScore
                       || gamePhase == GamePhase.RedScore
                       || gamePhase == GamePhase.Warmup
                       || gamePhase == GamePhase.FaceOff;

            if (_lastOn.HasValue && _lastOn.Value == turnOn) return;
            _lastOn = turnOn;

            if (_arenaLights == null)
            {
                if (_arenaLightsSearched) return;
                _arenaLightsSearched = true;
                _arenaLights = ArenaLightUtil.FindArenaLights();
                if (_arenaLights != null)
                    ResetCrowdSourceCache();
            }

            if (_arenaLights == null) return;

            if (_arenaLights.activeSelf != turnOn)
                _arenaLights.SetActive(turnOn);
        }
        public static AudioSource FindGoalCrowdSource()
        {
            if (_cachedCrowdSource != null) return _cachedCrowdSource;
            if (_crowdSourceSearched) return null;
            _crowdSourceSearched = true;

            var root = UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                             .FirstOrDefault(t => t.name.Contains("HockeyArenaRoot"));
            if (root == null) return null;

            var noise = root.GetComponentsInChildren<Transform>(true)
                            .FirstOrDefault(t => t.name == "GoalCrowdNoise");
            if (noise == null) return null;

            _cachedCrowdSource = noise.GetComponent<AudioSource>();
            return _cachedCrowdSource;
        }

        public static void ResetCaches()
        {
            _cachedCrowdSource = null;
            _crowdSourceSearched = false;
            _arenaLights = null;
            _arenaLightsSearched = false;
            _lastOn = null;
        }

        public static void ResetCrowdSourceCache()
        {
            _cachedCrowdSource = null;
            _crowdSourceSearched = false;
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
