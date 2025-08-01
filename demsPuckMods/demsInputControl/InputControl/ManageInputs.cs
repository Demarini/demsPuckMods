using demsInputControl.InputControl;
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
            // Get NetworkedInput<bool>.ClientTick
            var genericType = typeof(NetworkedInput<bool>);
            return genericType.GetMethod("ClientTick", BindingFlags.Public | BindingFlags.Instance);
        }

        static void Postfix(object __instance)
        {
            // Extract fields via reflection
            Type type = __instance.GetType();

            var clientVal = (bool)type.GetField("ClientValue").GetValue(__instance);
            var lastSentVal = (bool)type.GetField("LastSentValue").GetValue(__instance);
            var time = (double)type.GetField("LastSentTime").GetValue(__instance);

            //Debug.Log($"[Harmony][ClientTick] Called on NetworkedInput<bool>");
            //Debug.Log($"  ClientValue: {clientVal}");
            //Debug.Log($"  LastSentValue: {lastSentVal}");
            //Debug.Log($"  LastSentTime: {time:F3}");

            var hasChangedProp = type.GetProperty("HasChanged");
            if (hasChangedProp != null)
            {
                bool hasChanged = (bool)hasChangedProp.GetValue(__instance);
                //Debug.Log($"  HasChanged (after ClientTick): {hasChanged}");
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
            // If latency is disabled, run original ClientTick
            float now = Time.time;
            if (now - lastServerValueLogTime >= 0.5f)
            {
                //Debug.Log($"[InputControl] Server Value (StopInput): {__instance.StopInput.ServerValue}");
                //Debug.Log($"[InputControl] Client Value (StopInput): {__instance.StopInput.ClientValue}");
                lastServerValueLogTime = now;
            }

            QueueAllInputs(__instance);
            ProcessQueue();

            return false; // Skip original ClientTick, we manually replay everything
        }
        private static readonly MethodInfo handleInputsMethod = typeof(PlayerBodyV2)
    .GetMethod("HandleInputs", BindingFlags.NonPublic | BindingFlags.Instance);
        private static void QueueAllInputs(PlayerInput input)
        {
            bool shouldSimulateLatency = delay > 0f &&
    (!ConfigData.Instance.DelayInputs.OnlyInPracticeMode || PracticeModeDetector.IsPracticeMode);

            void EnqueueOrApply(Action rpc, Action tick)
            {
                Debug.Log($"Is Practice Mode: {PracticeModeDetector.IsPracticeMode}");
                if (!shouldSimulateLatency)
                {
                    Debug.Log("No Latency");
                    rpc();
                    tick();
                }
                else
                {
                    Debug.Log("Latency");
                    inputQueue.Enqueue(new BufferedInput
                    {
                        TimeToApply = Time.time + delay,
                        Apply = () =>
                        {
                            rpc();
                            tick();
                        }
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

            if (input.SprintInput.HasChanged && !SprintControl.IsSprintingBlockedByVelocity)
            {
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
            //Debug.Log($"Stop Has Changed {input.StopInput.HasChanged}");
            //Debug.Log($"Client Value {input.StopInput.ClientValue}");
            //Debug.Log($"Last Value {input.StopInput.LastSentValue}");
            if (input.StopInput.HasChanged && !SprintAndStopOverridePatch.ShouldForceStopInputChanged)
            {
                Debug.Log("[InputControl] Queuing manual StopInput change");
                EnqueueOrApply(() => input.Client_StopInputRpc(input.StopInput.ClientValue), () => input.StopInput.ClientTick());
            }
            if (SprintAndStopOverridePatch.ShouldForceStopInputChanged)
            {
                Debug.Log("Stop Input Has Changed");
                SprintAndStopOverridePatch.LastForceStopInput = SprintAndStopOverridePatch.ShouldForceStopInput;
                EnqueueOrApply(() => input.Client_StopInputRpc(SprintAndStopOverridePatch.ShouldForceStopInput), () => { });
                //Debug.Log($"Sending Stop Input {SprintAndStopOverridePatch.ShouldForceStopInput}");
                //Debug.Log($"Server Value {input.StopInput.ServerValue}");
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

            var movement = __instance;

            // Log raw velocity in local and world space
            Vector3 worldVel = movement.Rigidbody.linearVelocity;
            Vector3 localVel = movement.MovementDirection.InverseTransformVector(worldVel);
            //Debug.Log($"[Harmony][Velocity] LocalVelocity.z: {localVel.z:F3}, WorldVelocity: {worldVel}");

            // Input flags
            //Debug.Log($"[Harmony][InputFlags] MoveForwards: {movement.MoveForwards}, MoveBackwards: {movement.MoveBackwards}");

            // Grab private fields
            var flags = BindingFlags.NonPublic | BindingFlags.Instance;
            float currentMaxSpeed = movement.GetType().GetField("currentMaxSpeed", flags)?.GetValue(movement) as float? ?? -1f;
            float currentAcceleration = movement.GetType().GetField("currentAcceleration", flags)?.GetValue(movement) as float? ?? -1f;
            //Debug.Log($"[Harmony][Speed] Speed: {movement.Speed:F3}, currentMaxSpeed: {currentMaxSpeed:F3}, currentAcceleration: {currentAcceleration:F3}");

            // Direction vectors and diagnostics
            Vector3 forward = movement.MovementDirection.forward;
            Vector3 velocityDir = worldVel.normalized;
            float dot = Vector3.Dot(forward, velocityDir);

            Debug.DrawRay(movement.transform.position, forward * 3f, Color.green); // Forward direction
            Debug.DrawRay(movement.transform.position, velocityDir * 3f, Color.red); // Velocity direction
            //Debug.Log($"[Harmony][Move] forward: {forward}");
            //Debug.Log($"[Harmony][Move] Dot(forward, velocity): {dot:F3}");
            if (dot < 0f)
            {
                //Debug.LogWarning("[Harmony][Move] 🚨 Velocity is opposite of forward direction — may be canceling out!");
            }

            // Detect IsMoving* changes
            bool currentForward = movement.IsMovingForwards;
            bool currentBackward = movement.IsMovingBackwards;

            if (currentForward != cache.LastForward)
            {
                //Debug.Log($"[Harmony][Movement] IsMovingForwards changed to {currentForward}");
                cache.LastForward = currentForward;
            }

            if (currentBackward != cache.LastBackward)
            {
                //Debug.Log($"[Harmony][Movement] IsMovingBackwards changed to {currentBackward}");
                cache.LastBackward = currentBackward;
            }
        }
    }
}