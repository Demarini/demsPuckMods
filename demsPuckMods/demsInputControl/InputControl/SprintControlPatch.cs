using demsInputControl.Singletons;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace demsInputControl.InputControl
{
    [HarmonyPatch(typeof(PlayerInput), "Update")]
    public static class SprintVelocityCheckPatch
    {
        public static void Postfix(PlayerInput __instance)
        {
            var player = __instance.Player?.PlayerBody;

            var localVelocityZ = LocalVelocityTracker.LastLocalZVelocity;

            float minSpeed = ConfigData.Instance.ModifySprintControl.MinimumSpeedToSprint;
            SprintControl.IsSprintingBlockedByVelocity = localVelocityZ < minSpeed;

            // Debug
            //if (SprintControl.IsSprintingBlockedByVelocity)
                //Debug.Log($"[SprintControl] Blocking sprint - LocalZ: {localVelocity.z:F2}");
        }
    }
    public static class SprintControl
    {
        public static bool IsSprintingBlockedByVelocity = false;
    }
}
