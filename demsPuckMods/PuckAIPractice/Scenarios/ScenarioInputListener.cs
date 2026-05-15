using PuckAIPractice.Singletons;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace PuckAIPractice.Scenarios
{
    public class ScenarioInputListener : MonoBehaviour
    {
        private bool _wasServer;

        void Awake()
        {
            UnityEngine.SceneManagement.SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        void OnDestroy()
        {
            UnityEngine.SceneManagement.SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        void Update()
        {
            var nm = NetworkManager.Singleton;
            bool isServer = nm != null && nm.IsServer;

            // If we just lost server status (e.g. host disconnected), tear
            // down any active scenario before its state leaks into a future
            // session. Without this the pressure wall keeps ticking after
            // disconnect and catches the player as soon as they re-enter
            // practice mode.
            if (_wasServer && !isServer && ScenarioManager.Active != null)
            {
                ScenarioManager.Stop();
            }
            _wasServer = isServer;

            if (!isServer) return;

            ScenarioManager.Tick(Time.deltaTime);

            var key = ConfigData.Instance.ScenarioRestartKey;
            if (key == Key.None) return;

            var keyboard = Keyboard.current;
            if (keyboard == null) return;

            if (IsChatFocused()) return;

            var control = keyboard[key];
            if (control != null && control.wasPressedThisFrame)
            {
                ScenarioManager.Restart(nm.LocalClientId);
            }
        }

        private void OnSceneUnloaded(Scene scene)
        {
            // Belt-and-suspenders: if a scene unloads while a scenario is
            // active, stop it. The IsServer transition above catches normal
            // disconnect; this catches any path that unloads the scene
            // without flipping IsServer first.
            if (ScenarioManager.Active != null)
            {
                ScenarioManager.Stop();
            }
        }

        private static bool IsChatFocused()
        {
            try
            {
                var ui = UIManager.Instance;
                if (ui == null) return false;
                var chat = ui.Chat;
                if (chat == null) return false;
                return chat.IsFocused;
            }
            catch
            {
                return false;
            }
        }

        public static ScenarioInputListener Create()
        {
            var go = new GameObject("ScenarioInputListener");
            DontDestroyOnLoad(go);
            return go.AddComponent<ScenarioInputListener>();
        }
    }
}
