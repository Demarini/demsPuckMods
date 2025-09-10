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
        static CoroutineRunner _inst;
        public static CoroutineRunner Instance
        {
            get
            {
                if (_inst == null)
                {
                    var go = new GameObject("CoroutineRunner");
                    UnityEngine.Object.DontDestroyOnLoad(go);
                    _inst = go.AddComponent<CoroutineRunner>();
                }
                return _inst;
            }
        }
    }
}
