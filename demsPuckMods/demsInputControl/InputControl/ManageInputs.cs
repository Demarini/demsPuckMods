using demsInputControl.InputControl;
using demsInputControl.Logging;
using demsInputControl.Singletons;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Windows;

namespace demsInputControl
{
    [HarmonyPatch]
    public static class NetworkedInputClientTickLogger
    {
        static MethodBase TargetMethod()
        {
            var genericType = typeof(NetworkedInput<bool>);
            return genericType.GetMethod("ClientTick", BindingFlags.Public | BindingFlags.Instance);
        }

        static void Postfix(object __instance)
        {
            Type type = __instance.GetType();

            var clientVal = (bool)type.GetField("ClientValue").GetValue(__instance);
            var lastSentVal = (bool)type.GetField("LastSentValue").GetValue(__instance);
            var time = (double)type.GetField("LastSentTime").GetValue(__instance);

            InputControlLogger.Log(LogCategory.RPC, "[Harmony][ClientTick] Called on NetworkedInput<bool>");
            InputControlLogger.Log(LogCategory.RPC, $"  ClientValue: {clientVal}");
            InputControlLogger.Log(LogCategory.RPC, $"  LastSentValue: {lastSentVal}");
            InputControlLogger.Log(LogCategory.RPC, $"  LastSentTime: {time:F3}");

            var hasChangedProp = type.GetProperty("HasChanged");
            if (hasChangedProp != null)
            {
                bool hasChanged = (bool)hasChangedProp.GetValue(__instance);
                InputControlLogger.Log(LogCategory.RPC, $"  HasChanged (after ClientTick): {hasChanged}");
            }
        }
    }
    [HarmonyPatch(typeof(PlayerInput))]
    public static class PlayerInputLatencyPatch
    {
        static float jitter = UnityEngine.Random.Range(-ConfigData.Instance.DelayInputs.JitterMs, ConfigData.Instance.DelayInputs.JitterMs);
        static float delay = (ConfigData.Instance.DelayInputs.ArtificialLatencyMs + jitter) / 1000f;
        private static readonly Queue<BufferedInput> inputQueue = new Queue<BufferedInput>();

        private class BufferedInput
        {
            public Action Apply;
            public float TimeToApply;
        }

        private static float lastServerValueLogTime = 0f;

        [HarmonyPatch(typeof(PlayerInput), "ClientTick")]
        [HarmonyPrefix]
        private static bool Prefix(PlayerInput __instance)
        {
            float now = Time.time;
            if (now - lastServerValueLogTime >= 0.5f)
            {
                InputControlLogger.Log(LogCategory.StopControl, $"Server Value (StopInput): {__instance.StopInput.ServerValue}");
                InputControlLogger.Log(LogCategory.StopControl, $"Client Value (StopInput): {__instance.StopInput.ClientValue}");
                InputControlLogger.Log(LogCategory.SprintControl, $"Server Value (SprintInput): {__instance.SprintInput.ServerValue}");
                InputControlLogger.Log(LogCategory.SprintControl, $"Client Value (SprintInput): {__instance.SprintInput.ClientValue}");
                lastServerValueLogTime = now;
            }

            QueueAllInputs(__instance);
            ProcessQueue();

            return false;
        }

        private static readonly MethodInfo handleInputsMethod = typeof(PlayerBodyV2)
            .GetMethod("HandleInputs", BindingFlags.NonPublic | BindingFlags.Instance);

        private static void QueueAllInputs(PlayerInput input)
        {
            bool shouldSimulateLatency = delay > 0f && (!ConfigData.Instance.DelayInputs.OnlyInPracticeMode || PracticeModeDetector.IsPracticeMode);

            void EnqueueOrApply(Action rpc, Action tick)
            {
                InputControlLogger.Log(LogCategory.PracticeModeDetection, $"Is Practice Mode: {PracticeModeDetector.IsPracticeMode}");

                if (!shouldSimulateLatency)
                {
                    InputControlLogger.Log(LogCategory.PracticeModeDetection, "No Latency");
                    rpc();
                    tick();
                }
                else
                {
                    InputControlLogger.Log(LogCategory.PracticeModeDetection, "Latency");
                    inputQueue.Enqueue(new BufferedInput
                    {
                        TimeToApply = Time.time + delay,
                        Apply = () => { rpc(); tick(); }
                    });
                }
            }
            if (input.MoveInput.HasChanged)
            {
                short x = (short)(input.MoveInput.ClientValue.x * 32767f);
                short y = (short)(input.MoveInput.ClientValue.y * 32767f);

                EnqueueOrApply(() =>
                {
                    input.Client_MoveInputRpc(x, y);
                    input.MoveInput.ClientTick();

                    var body = input.GetComponent<PlayerBodyV2>();
                    var movement = input.GetComponent<Movement>();
                    if (body != null && movement != null && movement.Rigidbody != null && handleInputsMethod != null)
                    {
                        Vector3 localVel = movement.MovementDirection.InverseTransformVector(movement.Rigidbody.linearVelocity);
                        if (Mathf.Abs(localVel.z) < 0.05f) // Only invoke when movement has stopped
                        {
                            //Debug.Log("[InputControl] Velocity near zero, calling HandleInputs()");
                            handleInputsMethod.Invoke(body, null);
                        }
                        else
                        {
                            //Debug.Log("[InputControl] Skipping HandleInputs() due to velocity: " + localVel.z.ToString("F3"));
                        }
                    }
                },
                () => { });
            }
            if (input.StickRaycastOriginAngleInput.HasChanged)
            {
                short x = (short)(input.StickRaycastOriginAngleInput.ClientValue.x / 360f * 32767f);
                short y = (short)(input.StickRaycastOriginAngleInput.ClientValue.y / 360f * 32767f);
                EnqueueOrApply(() => input.Client_RaycastOriginAngleInputRpc(x, y), () => input.StickRaycastOriginAngleInput.ClientTick());
            }

            if (input.LookAngleInput.HasChanged)
            {
                short x = (short)(input.LookAngleInput.ClientValue.x / 360f * 32767f);
                short y = (short)(input.LookAngleInput.ClientValue.y / 360f * 32767f);
                EnqueueOrApply(() => input.Client_LookAngleInputRpc(x, y), () => input.LookAngleInput.ClientTick());
            }

            if (input.BladeAngleInput.HasChanged)
            {
                var val = input.BladeAngleInput.ClientValue;
                EnqueueOrApply(() => input.Client_BladeAngleInputRpc(val), () => input.BladeAngleInput.ClientTick());
            }

            if (input.SlideInput.HasChanged)
            {
                var val = input.SlideInput.ClientValue;
                EnqueueOrApply(() => input.Client_SlideInputRpc(val), () => input.SlideInput.ClientTick());
            }

            if (input.SprintInput.HasChanged && (!SprintControl.IsSprintingBlockedByVelocity || !ConfigData.Instance.ModifySprintControl.AllowModifySprintControl))
            {
                InputControlLogger.Log(LogCategory.SprintControl, "Sprint Input Changed");
                var val = input.SprintInput.ClientValue;
                EnqueueOrApply(() => input.Client_SprintInputRpc(val), () => input.SprintInput.ClientTick());
            }

            if (input.TrackInput.HasChanged)
            {
                var val = input.TrackInput.ClientValue;
                EnqueueOrApply(() => input.Client_TrackInputRpc(val), () => input.TrackInput.ClientTick());
            }

            if (input.LookInput.HasChanged)
            {
                var val = input.LookInput.ClientValue;
                EnqueueOrApply(() => input.Client_LookInputRpc(val), () => input.LookInput.ClientTick());
            }

            if (input.JumpInput.HasChanged)
            {
                EnqueueOrApply(() => input.Client_JumpInputRpc(), () => input.JumpInput.ClientTick());
            }
            InputControlLogger.Log(LogCategory.StopControl, $"Stop Has Changed {input.StopInput.HasChanged}");
            InputControlLogger.Log(LogCategory.StopControl, $"Client Value {input.StopInput.ClientValue}");
            InputControlLogger.Log(LogCategory.StopControl, $"Last Value {input.StopInput.LastSentValue}");

            if (input.StopInput.HasChanged && !StopOverridePatch.ShouldForceStopInputChanged)
            {
                InputControlLogger.Log(LogCategory.StopControl, "Queuing manual StopInput change");
                EnqueueOrApply(() => input.Client_StopInputRpc(input.StopInput.ClientValue), () => input.StopInput.ClientTick());
            }

            if (StopOverridePatch.ShouldForceStopInputChanged)
            {
                InputControlLogger.Log(LogCategory.StopControl, "Stop Input Has Changed");
                StopOverridePatch.LastForceStopInput = StopOverridePatch.ShouldForceStopInput;
                EnqueueOrApply(() => input.Client_StopInputRpc(StopOverridePatch.ShouldForceStopInput), () => { });
                InputControlLogger.Log(LogCategory.StopControl, $"Sending Stop Input {StopOverridePatch.ShouldForceStopInput}");
                InputControlLogger.Log(LogCategory.StopControl, $"Server Value {input.StopInput.ServerValue}");
            }
            if (input.JumpInput.HasChanged)
            {
                EnqueueOrApply(() => input.Client_JumpInputRpc(), () => input.JumpInput.ClientTick());
            }
            if (input.TwistLeftInput.HasChanged)
            {
                EnqueueOrApply(() => input.Client_TwistLeftInputRpc(), () => input.TwistLeftInput.ClientTick());
            }

            if (input.TwistRightInput.HasChanged)
            {
                EnqueueOrApply(() => input.Client_TwistRightInputRpc(), () => input.TwistRightInput.ClientTick());
            }

            if (input.DashLeftInput.HasChanged)
            {
                EnqueueOrApply(() => input.Client_DashLeftInputRpc(), () => input.DashLeftInput.ClientTick());
            }

            if (input.DashRightInput.HasChanged)
            {
                EnqueueOrApply(() => input.Client_DashRightInputRpc(), () => input.DashRightInput.ClientTick());
            }

            if (input.ExtendLeftInput.HasChanged)
            {
                var val = input.ExtendLeftInput.ClientValue;
                EnqueueOrApply(() => input.Client_ExtendLeftInputRpc(val), () => input.ExtendLeftInput.ClientTick());
            }

            if (input.ExtendRightInput.HasChanged)
            {
                var val = input.ExtendRightInput.ClientValue;
                EnqueueOrApply(() => input.Client_ExtendRightInputRpc(val), () => input.ExtendRightInput.ClientTick());
            }

            if (input.LateralLeftInput.HasChanged)
            {
                var val = input.LateralLeftInput.ClientValue;
                EnqueueOrApply(() => input.Client_LateralLeftInputRpc(val), () => input.LateralLeftInput.ClientTick());
            }

            if (input.LateralRightInput.HasChanged)
            {
                var val = input.LateralRightInput.ClientValue;
                EnqueueOrApply(() => input.Client_LateralRightInputRpc(val), () => input.LateralRightInput.ClientTick());
            }

            if (input.TalkInput.HasChanged)
            {
                var val = input.TalkInput.ClientValue;
                EnqueueOrApply(() => input.Client_TalkInputRpc(val), () => input.TalkInput.ClientTick());
            }

            if (input.SleepInput.HasChanged)
            {
                var val = input.SleepInput.ClientValue;
                EnqueueOrApply(() => input.Client_SleepInputRpc(val), () => input.SleepInput.ClientTick());
            }
        }
        public static void EnqueueCustomInput(PlayerInput input, Action rpc)
        {
            float jitter = UnityEngine.Random.Range(-ConfigData.Instance.DelayInputs.JitterMs, ConfigData.Instance.DelayInputs.JitterMs);
            float delay = (ConfigData.Instance.DelayInputs.ArtificialLatencyMs + jitter) / 1000f;

            inputQueue.Enqueue(new BufferedInput
            {
                TimeToApply = Time.time + delay,
                Apply = rpc
            });
        }
        private static void ProcessQueue()
        {
            while (inputQueue.Count > 0 && Time.time >= inputQueue.Peek().TimeToApply)
            {
                inputQueue.Dequeue().Apply?.Invoke();
            }
        }
    }
    [HarmonyPatch(typeof(Movement), "FixedUpdate")]
    public static class MovementFixedUpdateLogger
    {
        private static readonly ConditionalWeakTable<Movement, MovementStateCache> StateCache = new ConditionalWeakTable<Movement, MovementStateCache>();

        private class MovementStateCache
        {
            public bool LastForward;
            public bool LastBackward;
        }

        static void Prefix(Movement __instance)
        {
            if (!StateCache.TryGetValue(__instance, out var cache))
            {
                cache = new MovementStateCache();
                StateCache.Add(__instance, cache);
            }

            if (__instance == null || __instance.Rigidbody == null)
                return;

            Vector3 worldVel = __instance.Rigidbody.linearVelocity;
            Vector3 localVel = __instance.MovementDirection.InverseTransformVector(worldVel);
            InputControlLogger.Log(LogCategory.Velocity, $"LocalVelocity.z: {localVel.z:F3}, WorldVelocity: {worldVel}");

            InputControlLogger.Log(LogCategory.Velocity, $"MoveForwards: {__instance.MoveForwards}, MoveBackwards: {__instance.MoveBackwards}");

            var flags = BindingFlags.NonPublic | BindingFlags.Instance;
            float currentMaxSpeed = __instance.GetType().GetField("currentMaxSpeed", flags)?.GetValue(__instance) as float? ?? -1f;
            float currentAcceleration = __instance.GetType().GetField("currentAcceleration", flags)?.GetValue(__instance) as float? ?? -1f;
            InputControlLogger.Log(LogCategory.Velocity, $"Speed: {__instance.Speed:F3}, currentMaxSpeed: {currentMaxSpeed:F3}, currentAcceleration: {currentAcceleration:F3}");

            Vector3 forward = __instance.MovementDirection.forward;
            Vector3 velocityDir = worldVel.normalized;
            float dot = Vector3.Dot(forward, velocityDir);

            Debug.DrawRay(__instance.transform.position, forward * 3f, Color.green);
            Debug.DrawRay(__instance.transform.position, velocityDir * 3f, Color.red);
            InputControlLogger.Log(LogCategory.Velocity, $"forward: {forward}");
            InputControlLogger.Log(LogCategory.Velocity, $"Dot(forward, velocity): {dot:F3}");

            if (dot < 0f)
            {
                InputControlLogger.LogWarning(LogCategory.Velocity, "\uD83D\uDEA8 Velocity is opposite of forward direction — may be canceling out!");
            }

            bool currentForward = __instance.IsMovingForwards;
            bool currentBackward = __instance.IsMovingBackwards;

            if (currentForward != cache.LastForward)
            {
                InputControlLogger.Log(LogCategory.Velocity, $"IsMovingForwards changed to {currentForward}");
                cache.LastForward = currentForward;
            }

            if (currentBackward != cache.LastBackward)
            {
                InputControlLogger.Log(LogCategory.Velocity, $"IsMovingBackwards changed to {currentBackward}");
                cache.LastBackward = currentBackward;
            }
        }
    }
}