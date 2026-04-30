using HarmonyLib;
using SceneryChanger.Helpers;
using SceneryChanger.Services;
using UnityEngine;

namespace SceneryChanger.Behaviors
{
    public class UpdateAudioSources : MonoBehaviour
    {
        static UpdateAudioSources _instance;

        [SerializeField] float updateInterval = 1f;   // run once per second
        [SerializeField] float retryInterval = 5f;   // retry missing sources every 5s
        [SerializeField] int maxAttempts = 10;   // stop after 10 failed attempts per source

        float defaultGoal = .37f;
        float defaultAmbient = .027f;

        AudioSource ambientSource;
        AudioSource goalNoise;

        float _nextUpdateAt;
        float _nextAmbientRetryAt;
        float _nextGoalRetryAt;

        int _ambientAttempts;
        int _goalAttempts;
        bool _ambientGaveUp;
        bool _goalGaveUp;

        static System.Type _smType;
        static System.Reflection.PropertyInfo _globalVolProp;
        static System.Reflection.PropertyInfo _ambientVolProp;
        static bool _smReflectionInit;
        Object _cachedSM;

        public static void Install()
        {
            if (_instance) return;
            var go = new GameObject("~UpdateAudioSources");
            Object.DontDestroyOnLoad(go);
            _instance = go.AddComponent<UpdateAudioSources>();
        }

        public static void Uninstall()
        {
            if (!_instance) return;
            Object.Destroy(_instance.gameObject);
            _instance = null;
        }

        void OnEnable()
        {
            float now = Time.time;
            _nextUpdateAt = now;
            _nextAmbientRetryAt = now;
            _nextGoalRetryAt = now;
            _ambientAttempts = 0;
            _goalAttempts = 0;
            _ambientGaveUp = false;
            _goalGaveUp = false;
        }

        void Update()
        {
            var nm = Unity.Netcode.NetworkManager.Singleton;
            if (nm == null || !nm.IsClient) return;

            float now = Time.time;
            if (now < _nextUpdateAt) return;
            _nextUpdateAt = now + updateInterval;

            // Look up missing sources (throttled + capped attempts)
            if (ambientSource == null && !_ambientGaveUp && now >= _nextAmbientRetryAt)
            {
                ambientSource = GetAudio.FindAudio("StadiumNoise");
                if (ambientSource == null)
                {
                    _ambientAttempts++;
                    if (_ambientAttempts >= maxAttempts)
                        _ambientGaveUp = true;
                    else
                        _nextAmbientRetryAt = now + retryInterval;
                }
                else
                {
                    _ambientAttempts = 0; // found; reset counter
                }
            }

            if (goalNoise == null && !_goalGaveUp && now >= _nextGoalRetryAt)
            {
                goalNoise = GetAudio.FindAudio("GoalCrowdNoise");
                if (goalNoise == null)
                {
                    _goalAttempts++;
                    if (_goalAttempts >= maxAttempts)
                        _goalGaveUp = true;
                    else
                        _nextGoalRetryAt = now + retryInterval;
                }
                else
                {
                    _goalAttempts = 0; // found; reset counter
                }
            }

            // Apply volumes only if the sources exist
            // Use reflection to avoid MonoBehaviourSingleton<SettingsManager> generic inflation TLE
            if (!_smReflectionInit)
            {
                _smReflectionInit = true;
                _smType = AccessTools.TypeByName("SettingsManager");
                if (_smType != null)
                {
                    _globalVolProp = AccessTools.Property(_smType, "GlobalVolume");
                    _ambientVolProp = AccessTools.Property(_smType, "AmbientVolume");
                }
            }
            if (_smType == null || _globalVolProp == null || _ambientVolProp == null) return;
            if (!_cachedSM)
                _cachedSM = Object.FindFirstObjectByType(_smType);
            if (!_cachedSM) return;

            float globalVol = (float)_globalVolProp.GetValue(_cachedSM, null);
            float ambientVol = (float)_ambientVolProp.GetValue(_cachedSM, null);
            float audioMultiplier = globalVol * ambientVol;
            if (ambientSource != null)
                ambientSource.volume = audioMultiplier * defaultAmbient;
            if (goalNoise != null)
                goalNoise.volume = audioMultiplier * SceneryAudioState.GoalCrowdNoiseVolume;

            // If sources get destroyed later, we'll resume lookup unless we've given up.
            if (ambientSource == null && _ambientGaveUp) { /* intentionally do nothing */ }
            if (goalNoise == null && _goalGaveUp) { /* intentionally do nothing */ }
        }

        // Optional: call this if you want to try again after giving up (e.g., after a scene loads).
        public static void ResetLookups()
        {
            if (_instance == null) return;
            _instance._ambientAttempts = 0;
            _instance._goalAttempts = 0;
            _instance._ambientGaveUp = false;
            _instance._goalGaveUp = false;
            float now = Time.time;
            _instance._nextAmbientRetryAt = now;
            _instance._nextGoalRetryAt = now;
        }
    }
}
