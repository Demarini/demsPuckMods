using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuckAIPractice
{
    using HarmonyLib;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEngine;

    public static class HarmonyLogger
    {
        public static void PatchSpecificMethods(Harmony harmony, Type targetType, List<string> methodNames)
        {
            var methods = targetType
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                .Where(m =>
                    !m.IsSpecialName &&
                    !m.IsConstructor &&
                    methodNames.Contains(m.Name));

            foreach (var method in methods)
            {
                try
                {
                    var postfix = typeof(HarmonyLogger).GetMethod(nameof(LogMethodPostfix), BindingFlags.Static | BindingFlags.NonPublic);
                    var patch = new HarmonyMethod(postfix);
                    harmony.Patch(method, postfix: patch);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"[HarmonyLogger] Failed to patch {method.DeclaringType?.Name}.{method.Name}: {ex.Message}");
                }
            }
        }

        private static void LogMethodPostfix(MethodBase __originalMethod)
        {
            Debug.Log($"[Logger] {__originalMethod.DeclaringType?.Name}.{__originalMethod.Name} called");
        }
    }
}
