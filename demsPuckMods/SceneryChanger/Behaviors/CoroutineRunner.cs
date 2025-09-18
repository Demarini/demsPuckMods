using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SceneryChanger.Behaviors
{
    public sealed class CoroutineRunner : MonoBehaviour
    {
        static CoroutineRunner _instance;
        public static CoroutineRunner Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject("CoroutineRunner");
                    UnityEngine.Object.DontDestroyOnLoad(go);
                    _instance = go.AddComponent<CoroutineRunner>();
                }
                return _instance;
            }
        }
        public static void Install()
        {
            if (_instance) return;
            var go = new GameObject("~CoroutineRunner");
            DontDestroyOnLoad(go);
            _instance = go.AddComponent<CoroutineRunner>();
        }

        public static void Uninstall()
        {
            if (!_instance) return;
            Destroy(_instance.gameObject);
            _instance = null;
        }
    }
}
