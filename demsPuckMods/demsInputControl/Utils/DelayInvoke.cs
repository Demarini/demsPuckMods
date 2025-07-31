using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace demsInputControl.Utils
{
    public class DelayedInvoke : MonoBehaviour
    {
        private class DelayedCondition
        {
            public Func<bool> Condition;
            public Action Callback;
        }

        private static readonly List<DelayedCondition> pendingChecks = new List<DelayedCondition>();
        private static DelayedInvoke instance;

        public static void Until(Func<bool> condition, Action callback)
        {
            if (instance == null)
            {
                var go = new GameObject("DelayedInvoke_Helper");
                go.hideFlags = HideFlags.HideAndDontSave;
                instance = go.AddComponent<DelayedInvoke>();
                DontDestroyOnLoad(go);
            }

            pendingChecks.Add(new DelayedCondition
            {
                Condition = condition,
                Callback = callback
            });
        }

        private void Update()
        {
            for (int i = pendingChecks.Count - 1; i >= 0; i--)
            {
                var item = pendingChecks[i];
                if (item.Condition())
                {
                    item.Callback?.Invoke();
                    pendingChecks.RemoveAt(i);
                }
            }
        }
    }
}
