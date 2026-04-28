using demsInputControl.Singletons;
using demsInputControl.Utils;
using demsInputControl.Logging;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;

namespace demsInputControl.InputControl
{
    // PuckNew: PlayerBodyV2 renamed to PlayerBody — store as Component for transform access
    public static class LocalPlayerLocator
    {
        public static Component Cached { get; private set; }

        public static bool TryFind()
        {
            var type = AccessTools.TypeByName("PlayerBody") ?? AccessTools.TypeByName("PlayerBodyV2");
            if (type == null) return false;

            foreach (Component pb in UnityEngine.Object.FindObjectsOfType(type))
            {
                var player = Traverse.Create(pb).Property("Player").GetValue<Player>();
                if (player != null && player.IsLocalPlayer)
                {
                    Cached = pb;
                    InputControlLogger.Log(LogCategory.StopControl, $"[InputControl] Found local player: {pb.name}");
                    return true;
                }
            }
            return false;
        }
    }
    [HarmonyPatch]
    public static class StopOverridePatch
    {
        private static float lastStopTime = 0f;
        public static bool IsAutoStopping = false;
        public static bool LastForceStopInput = false;
        public static bool ShouldForceStopInput = false;
        public static StopDirection CurrentStopDirection = StopDirection.None;
        public static bool ShouldForceStopInputChanged
        {
            get
            {
                return LastForceStopInput != ShouldForceStopInput;
            }
        }

        // PuckNew: PlayerBodyV2 → PlayerBody (TargetMethod resolves at runtime)
        [HarmonyPatch]
        public static class PlayerBodyVelocityTrackerPatch
        {
            static int UpdatePerFrame = 0;
            static int FixedUpdateCount = 0;
            static float TimePassed = 0;

            static MethodBase TargetMethod()
            {
                var type = AccessTools.TypeByName("PlayerBody") ?? AccessTools.TypeByName("PlayerBodyV2");
                return type == null ? null : AccessTools.Method(type, "FixedUpdate");
            }

            [HarmonyPostfix]
            public static void Postfix(object __instance)
            {
                TimePassed += Time.fixedDeltaTime;
                FixedUpdateCount++;
                if (FixedUpdateCount > UpdatePerFrame)
                {
                    FixedUpdateCount = 0;
                    if (__instance != null)
                    {
                        InputControlLogger.Log(LogCategory.StopControl, "Instance isn't null");
                        var player = Traverse.Create(__instance).Property("Player").GetValue<Player>();
                        if (player != null)
                        {
                            if (player.IsLocalPlayer)
                            {
                                LocalPlayerLocator.TryFind();
                                var rigidbody = Traverse.Create(__instance).Property("Rigidbody").GetValue<Rigidbody>();
                                if (rigidbody == null)
                                {
                                    InputControlLogger.Log(LogCategory.StopControl, "Rigid Body Null");
                                }
                                else
                                {
                                    InputControlLogger.Log(LogCategory.StopControl, "Rigid Body Not Null");
                                    InputControlLogger.Log(LogCategory.StopControl, $"Rigid Body Velo: {rigidbody.linearVelocity}");
                                }
                                LocalVelocityTracker.Update(__instance as Component);
                                TimePassed = 0;
                            }
                            else
                            {
                                InputControlLogger.Log(LogCategory.StopControl, "Not Local Player");
                            }
                        }
                        else
                        {
                            InputControlLogger.Log(LogCategory.StopControl, "Player Is Null");
                        }
                    }
                    else
                    {
                        if (LocalPlayerLocator.Cached == null)
                            LocalPlayerLocator.TryFind();

                        LocalVelocityTracker.Update(__instance as Component);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(PlayerInput), "Update")]
        public static class PlayerInputUpdatePatch
        {
            public static void Postfix(PlayerInput __instance)
            {
                if (!StopOverridePatch.IsAutoStopping)
                    return;

                float localZVel = LocalVelocityTracker.LastLocalZVelocity;
                float veloBeforeStopping = 0.05f;
                bool clearStop = false;
                bool holdingForward = __instance.MoveInput.ClientValue.y > 0.15f;
                bool holdingBackward = __instance.MoveInput.ClientValue.y < -0.15f;
                bool almostStopped = Mathf.Abs(localZVel) < veloBeforeStopping;
                switch (CurrentStopDirection)
                {
                    case StopDirection.FromBackwardToForward:
                        clearStop = (localZVel > veloBeforeStopping && !holdingBackward) || (almostStopped && holdingForward);
                        break;

                    case StopDirection.FromForwardToBackward:
                        clearStop = (localZVel < veloBeforeStopping * -1 && !holdingForward) || (almostStopped && holdingBackward);
                        break;
                }
                if (almostStopped && (holdingForward || holdingBackward))
                    clearStop = true;
                if (!holdingForward && !holdingBackward && almostStopped)
                    clearStop = true;
                if (clearStop)
                {
                    IsAutoStopping = false;
                    ShouldForceStopInput = false;
                    CurrentStopDirection = StopDirection.None;
                    __instance.StopInput.ClientValue = false;
                }
            }
        }

        [HarmonyPatch(typeof(PlayerInput), "Client_MoveInputRpc")]
        [HarmonyPrefix]
        private static bool AutoStopWhenReversing(PlayerInput __instance, short x, short y)
        {
            InputControlLogger.Log(LogCategory.StopControl, "[InputControl] MoveInputRPC");
            var forceStopConfig = ConfigData.Instance.ForceStopWhenChangingDirection;
            if (forceStopConfig == null) return true;

            InputControlLogger.Log(LogCategory.StopControl, $"Player Local Velocity Z {LocalVelocityTracker.LastLocalZVelocity}");
            Vector2 inputDir = new Vector2(x / 32767f, y / 32767f);

            float tickRate = __instance.TickRate > 0 ? __instance.TickRate : 200;
            float tickInterval = 1f / tickRate;
            float now = Time.time;
            bool pressingForward = inputDir.y > 0.15f;
            bool pressingBackward = inputDir.y < -0.15f;
            float localZVelocity = LocalVelocityTracker.LastLocalZVelocity;
            bool movingBackwards = localZVelocity < -0.15f;
            bool movingForwards = localZVelocity > 0.15f;
            bool shouldStop =
                (ConfigData.Instance.ForceStopWhenChangingDirection.ForceStopWhenBackwardsToForwards && movingBackwards && pressingForward) ||
                (ConfigData.Instance.ForceStopWhenChangingDirection.ForceStopWhenForwardsToBackwards && movingForwards && pressingBackward);

            InputControlLogger.Log(LogCategory.StopControl, $"Player Input Dir Y {inputDir.y}");
            InputControlLogger.Log(LogCategory.StopControl, $"Should Stop {shouldStop}");
            const float AutoStopTriggerMinSpeed = 1.0f;
            if (Mathf.Abs(localZVelocity) < AutoStopTriggerMinSpeed)
                shouldStop = false;

            if (shouldStop)
            {
                if (now - lastStopTime >= tickInterval)
                {
                    ShouldForceStopInput = true;
                    InputControlLogger.Log(LogCategory.StopControl, "ShoudlForceStopInput True");
                    if (movingBackwards && pressingForward)
                        CurrentStopDirection = StopDirection.FromBackwardToForward;
                    else if (movingForwards && pressingBackward)
                        CurrentStopDirection = StopDirection.FromForwardToBackward;
                    else
                        CurrentStopDirection = StopDirection.None;

                    __instance.StopInput.ClientValue = true;
                    lastStopTime = now;
                    IsAutoStopping = true;
                }
            }
            else
            {
                if (IsAutoStopping)
                {
                    IsAutoStopping = false;
                    ShouldForceStopInput = false;
                    CurrentStopDirection = StopDirection.None;
                    __instance.StopInput.ClientValue = false;
                }
            }

            return true;
        }

        [HarmonyPatch(typeof(PlayerInput), "Client_StopInputRpc")]
        [HarmonyPrefix]
        public static bool Prefix(PlayerInput __instance, bool value)
        {
            var flags = BindingFlags.NonPublic | BindingFlags.Instance;
            var execStageField = typeof(NetworkBehaviour).GetField("__rpc_exec_stage", flags);
            // execStage accessed via reflection — logging commented out in original
            return true;
        }
    }

    // PuckNew: PlayerBodyV2 → PlayerBody (TargetMethod resolves at runtime)
    [HarmonyPatch]
    public static class HandleInputsLoggerPatch
    {
        static MethodBase TargetMethod()
        {
            var type = AccessTools.TypeByName("PlayerBody") ?? AccessTools.TypeByName("PlayerBodyV2");
            return type == null ? null
                : type.GetMethod("HandleInputs", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        [HarmonyPostfix]
        private static void Postfix(object __instance)
        {
            var movement = Traverse.Create(__instance).Property("Movement").GetValue<Movement>();
            if (movement == null) return;

            Vector3 velocity = movement.Rigidbody?.linearVelocity ?? Vector3.zero;
            // Remaining log calls commented out in original — preserved
        }
    }

    public enum StopDirection
    {
        None,
        FromBackwardToForward,
        FromForwardToBackward
    }
}
