using demsInputControl.Singletons;
using demsInputControl.Utils;
using demsInputControl.Logging;
using HarmonyLib;
using UnityEngine;

namespace demsInputControl.InputControl
{
    [HarmonyPatch(typeof(PlayerInput), "Update")]
    public static class SprintVelocityCheckPatch
    {
        public static void Postfix(PlayerInput __instance)
        {
            bool holdingBackward = __instance.MoveInput.ClientValue.y < -0.15f;
            var localVelocityZ = LocalVelocityTracker.LastLocalZVelocity;

            float minSpeed = ConfigData.Instance.ModifySprintControl.MinimumSpeedToSprint;
            bool allowMod = ConfigData.Instance.ModifySprintControl.AllowModifySprintControl;
            bool allowSprintBackwards = ConfigData.Instance.ModifySprintControl.AllowSprintWhileMovingBackwards;
            if (!ConfigData.Instance.ModifySprintControl.AllowModifySprintControl)
            {
                SprintControl.IsSprintingBlockedByVelocity = false;
            }
            else
            {
                bool holdingBackwards = __instance.MoveInput.ClientValue.y < -0.15f;
                bool allowSprintWhileHoldingBack = ConfigData.Instance.ModifySprintControl.AllowSprintWhileMovingBackwards;
                bool belowMinSpeed = localVelocityZ < ConfigData.Instance.ModifySprintControl.MinimumSpeedToSprint;

                SprintControl.IsSprintingBlockedByVelocity =
                    allowSprintWhileHoldingBack
                        ? !holdingBackwards && belowMinSpeed
                        : belowMinSpeed;
            }
        }
    }
    public static class SprintControl
    {
        public static bool IsSprintingBlockedByVelocity = false;
    }
}
