using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace demsInputControl.UnityPlugins
{
    public class PluginBehaviour : MonoBehaviour
    {
        private static PluginBehaviour _instance;
        public static PluginBehaviour Instance => _instance;

        public static void Initialize()
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("InputControlPlugin");
                DontDestroyOnLoad(go);
                _instance = go.AddComponent<PluginBehaviour>();
            }
        }
    }
}
