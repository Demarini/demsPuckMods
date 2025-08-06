using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PuckAIPractice.GameModes
{
    public class GoalieRunner : MonoBehaviour
    {
        public static GoalieRunner Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this);
            }
        }

        public static void Initialize()
        {
            if (Instance == null)
            {
                var go = new GameObject("GoalieRunner");
                go.hideFlags = HideFlags.HideAndDontSave;
                go.AddComponent<GoalieRunner>();
            }
        }
    }
}
