using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

namespace SceneryChanger.Behaviors
{
    public class DetectGameState : MonoBehaviour
    {
        public float interval = 1f;

        // NEW: only search for arena lights for this long after install (seconds)
        public float findWindowSeconds = 10f;

        float _timer;
        float _findElapsed;
        bool _findExpired;

        static DetectGameState _instance;
        static GameObject _arenaLights; // cache
        static bool? _lastOn;           // last applied state

        // Reflection cache: avoids direct GameManager.Instance reference which triggers
        // MonoBehaviourSingleton<T> generic inflation TLE on Mono.
        static System.Type _gmType;
        static System.Reflection.MethodInfo _phaseGetter;
        static bool _reflectionInit;
        static Object _cachedGM;

        public static void Install()
        {
            if (_instance) return;
            var go = new GameObject("~DetectGameState");
            DontDestroyOnLoad(go);
            _instance = go.AddComponent<DetectGameState>();
        }

        public static void Uninstall()
        {
            if (!_instance) return;
            Destroy(_instance.gameObject);
            _instance = null;
            _arenaLights = null;
            _lastOn = null;
            _cachedGM = null;
        }

        // OPTIONAL: call this when you know a new rink/scene was loaded
        // to allow searching again for 10 seconds.
        public static void ResetArenaLightSearch()
        {
            if (_instance == null) return;
            _arenaLights = null;
            _lastOn = null;
            _cachedGM = null;
            _instance._findElapsed = 0f;
            _instance._findExpired = false;
        }

        static void InitReflection()
        {
            if (_reflectionInit) return;
            _reflectionInit = true;
            _gmType = AccessTools.TypeByName("GameManager");
            if (_gmType != null)
                _phaseGetter = AccessTools.PropertyGetter(_gmType, "Phase");
        }

        void Update()
        {
            var nm = NetworkManager.Singleton;
            if (nm == null || !nm.IsClient) return;

            _timer += Time.deltaTime;
            if (_timer < interval) return;
            _timer = 0f;

            InitReflection();
            if (_gmType == null || _phaseGetter == null) return;

            if (!_cachedGM)
                _cachedGM = Object.FindFirstObjectByType(_gmType);
            if (!_cachedGM) return;

            var phase = (GamePhase)_phaseGetter.Invoke(_cachedGM, null);

            bool desiredOn =
                   phase == GamePhase.BlueScore
                || phase == GamePhase.RedScore
                || phase == GamePhase.Warmup
                || phase == GamePhase.FaceOff;

            // If we don't have it yet, only attempt for the first findWindowSeconds.
            if (_arenaLights == null)
            {
                if (!_findExpired)
                {
                    _findElapsed += interval;
                    if (_findElapsed <= findWindowSeconds)
                    {
                        Debug.Log("Trying to Find Lights...");
                        _arenaLights = SceneryChanger.Helpers.ArenaLightUtil.FindArenaLights();
                    }
                    else
                    {
                        _findExpired = true; // stop searching forever (until reset)
                    }
                }

                if (_arenaLights == null) return; // still not found (or expired)

                // First time found -> force initial sync immediately
                if (_arenaLights.activeSelf != desiredOn)
                    _arenaLights.SetActive(desiredOn);

                _lastOn = desiredOn;
                return;
            }

            // We have the GO: only apply when desired state changes
            if (!_lastOn.HasValue || _lastOn.Value != desiredOn)
            {
                if (_arenaLights.activeSelf != desiredOn)
                    _arenaLights.SetActive(desiredOn);

                _lastOn = desiredOn;
            }
        }
    }
}
