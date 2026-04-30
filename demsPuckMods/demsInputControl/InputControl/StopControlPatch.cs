using demsInputControl.Singletons;
using demsInputControl.Utils;
using demsInputControl.Logging;
using HarmonyLib;
using System.Reflection;
using Unity.Netcode;
using UnityEngine;

namespace demsInputControl.InputControl
{
    public static class LocalPlayerLocator
    {
        public static PlayerBody Cached { get; private set; }

        public static bool TryFind()
        {
            foreach (var pb in UnityEngine.Object.FindObjectsOfType<PlayerBody>())
            {
                if (pb?.Player != null && pb.Player.IsLocalPlayer)
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

        [HarmonyPatch(typeof(PlayerBody), "FixedUpdate")]
        public static class PlayerBodyVelocityTrackerPatch
        {
            static int UpdatePerFrame = 0;
            static int FixedUpdateCount = 0;
            static float TimePassed = 0;

            [HarmonyPostfix]
            public static void Postfix(PlayerBody __instance)
            {
                TimePassed += Time.fixedDeltaTime;
                FixedUpdateCount++;
                if (FixedUpdateCount > UpdatePerFrame)
                {
                    FixedUpdateCount = 0;
                    if (__instance != null)
                    {
                        InputControlLogger.Log(LogCategory.StopControl, "Instance isn't null");
                        if (__instance.Player != null)
                        {
                            if (__instance.Player.IsLocalPlayer)
                            {
                                LocalPlayerLocator.TryFind();
                                if (__instance.Rigidbody == null)
                                {
                                    InputControlLogger.Log(LogCategory.StopControl, "Rigid Body Null");
                                }
                                else
                                {
                                    InputControlLogger.Log(LogCategory.StopControl, "Rigid Body Not Null");
                                    InputControlLogger.Log(LogCategory.StopControl, $"Rigid Body Velo: {__instance.Rigidbody.linearVelocity}");
                                }
                                LocalVelocityTracker.Update(__instance);
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

                        LocalVelocityTracker.Update(__instance);
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
            Vector2 inputDir = new Vector2(
                NetworkingUtils.DecompressShortToFloat(x, -1f, 1f),
                NetworkingUtils.DecompressShortToFloat(y, -1f, 1f));

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
            return true;
        }
    }

    [HarmonyPatch(typeof(PlayerBody), "HandleInputs")]
    public static class HandleInputsLoggerPatch
    {
        [HarmonyPostfix]
        private static void Postfix(PlayerBody __instance)
        {
            var movement = __instance.Movement;
            if (movement == null) return;

            Vector3 velocity = movement.Rigidbody?.linearVelocity ?? Vector3.zero;
        }
    }

    public enum StopDirection
    {
        None,
        FromBackwardToForward,
        FromForwardToBackward
    }
}
