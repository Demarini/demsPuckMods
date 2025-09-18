using Unity.Netcode;
using UnityEngine;

namespace SceneryChanger.Behaviors
{
    public class DetectGameState : MonoBehaviour
    {
        public float interval = 1f;
        float _timer;
        static DetectGameState _instance;
        static GameObject _arenaLights;      // cache
        static bool? _lastOn;                // last applied state

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
        }

        void Update()
        {
            var nm = NetworkManager.Singleton;
            if (nm == null || !nm.IsClient) return;

            _timer += Time.deltaTime;
            if (_timer < interval) return;
            _timer = 0f;

            if (GameManager.Instance == null) return;
            var phase = GameManager.Instance.Phase;

            // Desired state from phase
            bool desiredOn =
                   phase == GamePhase.BlueScore
                || phase == GamePhase.RedScore
                || phase == GamePhase.Warmup
                || phase == GamePhase.FaceOff;

            // If we don't have the GO yet, try to find it and, if found, FORCE initial sync.
            if (_arenaLights == null)
            {
                _arenaLights = SceneryChanger.Helpers.ArenaLightUtil.FindArenaLights();
                if (_arenaLights == null) return; // still not ready; don't change _lastOn yet

                // First time we see it -> force to desired state immediately
                if (_arenaLights.activeSelf != desiredOn)
                    _arenaLights.SetActive(desiredOn);

                _lastOn = desiredOn; // cache only after we actually applied
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
