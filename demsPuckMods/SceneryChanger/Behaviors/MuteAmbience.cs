using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine;
using SM = UnityEngine.SceneManagement;
using SceneryLoader.Behaviors;
namespace SceneryChanger.Behaviors
{
    public class DisableAmbientCrowd : MonoBehaviour
    {
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) => TryDisable();
        public static DisableAmbientCrowd _instance;
        public static void Install()
        {
            if (_instance != null) return;
            var go = new GameObject("~RinkOnlyPruner(Auto)");
            _instance = go.AddComponent<DisableAmbientCrowd>();
            UnityEngine.Object.DontDestroyOnLoad(go);
            SM.SceneManager.sceneLoaded += _instance.OnSceneLoaded;
        }
        public static void Uninstall()
        {
            SM.SceneManager.sceneLoaded -= _instance.OnSceneLoaded;
            _instance = null;
        }
        private static void TryDisable()
        {
            
        }
    }
}
