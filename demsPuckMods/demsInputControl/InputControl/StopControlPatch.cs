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
    public static class LocalPlayerLocator
    {
        public static PlayerBodyV2 Cached { get; private set; }

        public static bool TryFind()
        {
            foreach (var pb in UnityEngine.Object.FindObjectsOfType<PlayerBodyV2>())
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
        // Block sprint visuals when moving backwards
        //[HarmonyPatch(typeof(PlayerBodyV2), "OnIsSprintingChanged")]
        //[HarmonyPrefix]
        //private static bool BlockSprintingBasedOnVelocity(PlayerBodyV2 __instance)
        //{
        //    if (__instance.Rigidbody == null) return true;

        //    var localVelocity = __instance.transform.InverseTransformDirection(__instance.Rigidbody.velocity);

        //    if (localVelocity.z < ConfigData.Instance.ModifySprintControl.MinimumSpeedToSprint)
        //    {
        //        //InputControlLogger.Log(LogCategory.StopControl, "[InputControl] Preventing sprint animation due to reverse speed");
        //        return false; // Skip OnIsSprintingChanged
        //    }

        //    return true;
        //}


        //Auto-stop if trying to move forward while moving backward
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
        [HarmonyPatch(typeof(PlayerBodyV2), "FixedUpdate")]
        public static class PlayerBodyVelocityTrackerPatch
        {
            static int UpdatePerFrame = 0;
            static int FixedUpdateCount = 0;
            static float TimePassed = 0;
            [HarmonyPostfix]
            public static void Postfix(PlayerBodyV2 __instance)
            {
                TimePassed += Time.fixedDeltaTime;
                FixedUpdateCount++;
                if(FixedUpdateCount > UpdatePerFrame)
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
                                    InputControlLogger.Log(LogCategory.StopControl, $"Rigid Body Velo: {__instance.Rigidbody.velocity}");
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
                {
                    clearStop = true;
                }
                if (!holdingForward && !holdingBackward && almostStopped)
                {
                    clearStop = true;
                }
                if (clearStop)
                {
                    //InputControlLogger.Log(LogCategory.StopControl, "[InputControl] Auto-stop cleared via Update movement check.");
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
            //var player = __instance.Player.PlayerBody;
            //if (player == null || player.Rigidbody == null) return true;
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
            {
                shouldStop = false; // Cancel auto-stop if speed is too low
            }
            if (shouldStop)
            {
                if (now - lastStopTime >= tickInterval)
                {
                    //InputControlLogger.Log(LogCategory.StopControl, "[InputControl] Auto-stopping due to direction conflict.");
                    ShouldForceStopInput = true;
                    InputControlLogger.Log(LogCategory.StopControl, "ShoudlForceStopInput True");
                    if (movingBackwards && pressingForward)
                        CurrentStopDirection = StopDirection.FromBackwardToForward;
                    else if (movingForwards && pressingBackward)
                        CurrentStopDirection = StopDirection.FromForwardToBackward;
                    else
                        CurrentStopDirection = StopDirection.None;

                    //__instance.StopInput.LastSentValue = false;
                    __instance.StopInput.ClientValue = true;
                    //__instance.StopInput.ClientTick();

                    //InputControlLogger.Log(LogCategory.StopControl, $"[InputControl] Set StopInput.ClientValue={__instance.StopInput.ClientValue}, LastSent={__instance.StopInput.LastSentValue}, HasChanged={__instance.StopInput.HasChanged}");
                    lastStopTime = now;
                    IsAutoStopping = true;
                }
            }
            else
            {
                if (IsAutoStopping)
                {
                    //InputControlLogger.Log(LogCategory.StopControl, "[InputControl] Ending auto-stop override.");
                    //ShouldForceStopInput = false;
                    IsAutoStopping = false;
                    ShouldForceStopInput = false;
                    CurrentStopDirection = StopDirection.None;
                    __instance.StopInput.ClientValue = false;
                    //__instance.StopInput.ClientTick();
                }
            }

            return true;
        }
        [HarmonyPatch(typeof(PlayerInput), "Client_StopInputRpc")]
        [HarmonyPrefix]
        public static bool Prefix(PlayerInput __instance, bool value)
        {
            //InputControlLogger.Log(LogCategory.StopControl, $"[Harmony][StopInputRpc] Intercepted RPC with value: {value}");

            var flags = BindingFlags.NonPublic | BindingFlags.Instance;
            var execStageField = typeof(NetworkBehaviour).GetField("__rpc_exec_stage", flags);

            if (execStageField != null)
            {
                object execStage = execStageField.GetValue(__instance);
                //InputControlLogger.Log(LogCategory.StopControl, $"[Harmony][StopInputRpc] ExecStage via reflection: {execStage}");
            }
            else
            {
                //Debug.LogWarning("[Harmony][StopInputRpc] Could not access __rpc_exec_stage field!");
            }

            return true; // Let the original method run
        }
        //[HarmonyPatch(typeof(PlayerInput), "FixedUpdate")]
        //public static class PlayerInputFixedUpdatePatch
        //{
        //    public static void Prefix(PlayerInput __instance)
        //    {
        //        if (NetworkManager.Singleton.IsServer)
        //            return;

        //        var movement = __instance.GetComponent<Movement>();
        //        if (movement == null || movement.Rigidbody == null || movement.MovementDirection == null)
        //            return;

        //        Vector3 localVel = movement.MovementDirection.InverseTransformVector(movement.Rigidbody.linearVelocity);
        //        bool holdingForward = __instance.MoveInput.ClientValue.y > 0.15f;
        //        bool movingForwardEnough = localVel.z > 0.5f;

        //        if (__instance.StopInput.ClientValue && holdingForward && movingForwardEnough)
        //        {
        //            //InputControlLogger.Log(LogCategory.StopControl, "[Harmony][PlayerInput] Auto-clearing StopInput.Value due to resumed forward movement");
        //            __instance.StopInput.ClientValue = false;
        //            ShouldForceStopInput = false;
        //        }
        //    }
        //}
    }
    [HarmonyPatch(typeof(PlayerBodyV2), "HandleInputs")]
    public static class HandleInputsLoggerPatch
    {
        [HarmonyPostfix]
        private static void Postfix(PlayerBodyV2 __instance)
        {
            var movement = __instance.Movement;
            if (movement == null)
            {
                //Debug.LogWarning("[Harmony][HandleInputs] No Movement component found.");
                return;
            }

            Vector3 velocity = movement.Rigidbody?.velocity ?? Vector3.zero;
            Vector3 localVelocity = movement.MovementDirection != null
                ? movement.MovementDirection.InverseTransformVector(velocity)
                : velocity;

            //InputControlLogger.Log(LogCategory.StopControl, $"[Harmony][HandleInputs] Called at Time.time={Time.time:F2}");
            //InputControlLogger.Log(LogCategory.StopControl, $"  MoveForwards: {movement.MoveForwards}");
            //InputControlLogger.Log(LogCategory.StopControl, $"  MoveBackwards: {movement.MoveBackwards}");
            //InputControlLogger.Log(LogCategory.StopControl, $"  TurnLeft: {movement.TurnLeft}");
            //InputControlLogger.Log(LogCategory.StopControl, $"  TurnRight: {movement.TurnRight}");
            //InputControlLogger.Log(LogCategory.StopControl, $"  Velocity (world): {velocity}");
            //InputControlLogger.Log(LogCategory.StopControl, $"  Velocity (local z): {localVelocity.z:F3}");
            //InputControlLogger.Log(LogCategory.StopControl, $"  StopInput.ServerValue: {__instance.Player.PlayerInput.StopInput.ServerValue}");
            //InputControlLogger.Log(LogCategory.StopControl, $"  SlideInput.ServerValue: {__instance.Player.PlayerInput.SlideInput.ServerValue}");
        }
    }
    public enum StopDirection
    {
        None,
        FromBackwardToForward,
        FromForwardToBackward
    }
}