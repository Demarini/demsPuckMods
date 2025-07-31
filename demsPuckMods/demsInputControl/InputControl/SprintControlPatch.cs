using demsInputControl.Singletons;
using demsInputControl.Utils;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Windows;

namespace demsInputControl.InputControl
{
    [HarmonyPatch]
    public static class SprintAndStopOverridePatch
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
        //        Debug.Log("[InputControl] Preventing sprint animation due to reverse speed");
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
        [HarmonyPatch(typeof(PlayerInput), "Update")]
        public static class PlayerInputUpdatePatch
        {
            public static void Postfix(PlayerInput __instance)
            {
                if (!SprintAndStopOverridePatch.IsAutoStopping)
                    return;

                var player = __instance.Player?.PlayerBody;
                if (player == null || player.Rigidbody == null) return;

                Vector3 localVel = player.transform.InverseTransformDirection(player.Rigidbody.velocity);

                bool clearStop = false;

                switch (CurrentStopDirection)
                {
                    case StopDirection.FromBackwardToForward:
                        clearStop = localVel.z > 0.5f;
                        break;
                    case StopDirection.FromForwardToBackward:
                        clearStop = localVel.z < -0.5f;
                        break;
                }

                if (clearStop)
                {
                    Debug.Log("[InputControl] Auto-stop cleared via Update movement check.");
                    IsAutoStopping = false;
                    ShouldForceStopInput = false;
                    CurrentStopDirection = StopDirection.None;

                    __instance.StopInput.LastSentValue = true;
                    __instance.StopInput.ClientValue = false;
                    __instance.StopInput.ClientTick();
                }
            }
        }
        [HarmonyPatch(typeof(PlayerInput), "Client_MoveInputRpc")]
        [HarmonyPrefix]
        private static bool AutoStopWhenReversing(PlayerInput __instance, short x, short y)
        {
            Debug.Log("[InputControl] MoveInputRPC");
            var forceStopConfig = ConfigData.Instance.ForceStopWhenChangingDirection;
            if (forceStopConfig == null) return true;
            var player = __instance.Player.PlayerBody;
            if (player == null || player.Rigidbody == null) return true;

            Vector3 localVelocity = player.transform.InverseTransformDirection(player.Rigidbody.velocity);
            Vector2 inputDir = new Vector2(x / 32767f, y / 32767f);

            float tickRate = __instance.TickRate > 0 ? __instance.TickRate : 200;
            float tickInterval = 1f / tickRate;
            float now = Time.time;
            bool pressingForward = inputDir.y > 0.15f;
            bool pressingBackward = inputDir.y < -0.15f;
            bool movingBackwards = localVelocity.z < -1.5f;
            bool movingForwards = localVelocity.z > 1.5f;
            bool shouldStop =
    (ConfigData.Instance.ForceStopWhenChangingDirection.ForceStopWhenBackwardsToForwards && movingBackwards && pressingForward) ||
    (ConfigData.Instance.ForceStopWhenChangingDirection.ForceStopWhenForwardsToBackwards && movingForwards && pressingBackward);


            if (shouldStop)
            {
                if (now - lastStopTime >= tickInterval)
                {
                    Debug.Log("[InputControl] Auto-stopping due to direction conflict.");
                    ShouldForceStopInput = true;

                    if (movingBackwards && pressingForward)
                        CurrentStopDirection = StopDirection.FromBackwardToForward;
                    else if (movingForwards && pressingBackward)
                        CurrentStopDirection = StopDirection.FromForwardToBackward;

                    __instance.StopInput.LastSentValue = false;
                    __instance.StopInput.ClientValue = true;
                    __instance.StopInput.ClientTick();

                    Debug.Log($"[InputControl] Set StopInput.ClientValue={__instance.StopInput.ClientValue}, LastSent={__instance.StopInput.LastSentValue}, HasChanged={__instance.StopInput.HasChanged}");
                    lastStopTime = now;
                    IsAutoStopping = true;
                }
            }
            else
            {
                if (IsAutoStopping)
                {
                    Debug.Log("[InputControl] Ending auto-stop override.");
                    ShouldForceStopInput = false;
                    IsAutoStopping = false;

                    __instance.StopInput.LastSentValue = true;   // <-- Set this FIRST
                    __instance.StopInput.ClientValue = false;
                    __instance.StopInput.ClientTick();
                }
            }

            return true;
        }
        [HarmonyPatch(typeof(PlayerInput), "Client_StopInputRpc")]
        [HarmonyPrefix]
        public static bool Prefix(PlayerInput __instance, bool value)
        {
            Debug.Log($"[Harmony][StopInputRpc] Intercepted RPC with value: {value}");

            var flags = BindingFlags.NonPublic | BindingFlags.Instance;
            var execStageField = typeof(NetworkBehaviour).GetField("__rpc_exec_stage", flags);

            if (execStageField != null)
            {
                object execStage = execStageField.GetValue(__instance);
                Debug.Log($"[Harmony][StopInputRpc] ExecStage via reflection: {execStage}");
            }
            else
            {
                Debug.LogWarning("[Harmony][StopInputRpc] Could not access __rpc_exec_stage field!");
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
        //            Debug.Log("[Harmony][PlayerInput] Auto-clearing StopInput.Value due to resumed forward movement");
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
                Debug.LogWarning("[Harmony][HandleInputs] No Movement component found.");
                return;
            }

            Vector3 velocity = movement.Rigidbody?.velocity ?? Vector3.zero;
            Vector3 localVelocity = movement.MovementDirection != null
                ? movement.MovementDirection.InverseTransformVector(velocity)
                : velocity;

            Debug.Log($"[Harmony][HandleInputs] Called at Time.time={Time.time:F2}");
            Debug.Log($"  MoveForwards: {movement.MoveForwards}");
            Debug.Log($"  MoveBackwards: {movement.MoveBackwards}");
            Debug.Log($"  TurnLeft: {movement.TurnLeft}");
            Debug.Log($"  TurnRight: {movement.TurnRight}");
            Debug.Log($"  Velocity (world): {velocity}");
            Debug.Log($"  Velocity (local z): {localVelocity.z:F3}");
            Debug.Log($"  StopInput.ServerValue: {__instance.Player.PlayerInput.StopInput.ServerValue}");
            Debug.Log($"  SlideInput.ServerValue: {__instance.Player.PlayerInput.SlideInput.ServerValue}");
        }
    }
    public enum StopDirection
    {
        None,
        FromBackwardToForward,
        FromForwardToBackward
    }
}