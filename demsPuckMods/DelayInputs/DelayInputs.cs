using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Netcode;
using UnityEngine;

namespace ArtificialInputDelay
{
    public class DelayInputs : IPuckPlugin
    {
        private static readonly Harmony harmony = new Harmony("GAFURIX.DelayInputs");

        public bool OnEnable()
        {
            if (Application.isBatchMode)
            {
                Debug.Log("[DelayInputs] Dedicated server detected — skipping patches");
                return true;
            }

            Debug.Log("[DelayInputs] Mod enabled");
            ModConfig.Initialize();
            ConfigData.Load();

            try
            {
                // Before patching, enumerate all NetworkedInput<> variants for visibility
                foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (var type in asm.GetTypes().Where(t => t.Name.StartsWith("NetworkedInput")))
                    {
                        Debug.Log($"[DelayInputs] Found input type: {type.FullName}");
                        foreach (var f in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
                        {
                            Debug.Log($"[DelayInputs]   Field: {f.Name} ({f.FieldType})");
                        }
                    }
                }

                harmony.PatchAll();
                Debug.Log("[DelayInputs] Harmony patches applied");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[DelayInputs] Harmony patch failed: {e.Message}");
                return false;
            }

            return true;
        }

        public bool OnDisable()
        {
            try
            {
                harmony.UnpatchSelf();
                Debug.Log("[DelayInputs] Mod disabled");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[DelayInputs] Harmony unpatch failed: {e.Message}");
                return false;
            }

            return true;
        }
    }
    public static class PracticeModeDetector
    {
        // Practice mode = the local player is hosting a server named "PRACTICE".
        // Computed from live state every read so it can't get stuck after mode transitions.
        public static bool IsPracticeMode
        {
            get
            {
                try
                {
                    var nm = NetworkManager.Singleton;
                    if (nm == null || !nm.IsHost) return false;

                    var sm = ServerManager.Instance;
                    if (sm == null) return false;

                    return sm.Server.Name.ToString() == "PRACTICE";
                }
                catch
                {
                    return false;
                }
            }
        }
    }
    [HarmonyPatch(typeof(PlayerInput))]
    public static class PlayerInputLatencyPatch
    {
        static float jitter = UnityEngine.Random.Range(-ConfigData.Instance.JitterMs, ConfigData.Instance.JitterMs);
        static float delay = (ConfigData.Instance.ArtificialLatencyMs + jitter) / 1000f;
        private static readonly Queue<BufferedInput> inputQueue = new Queue<BufferedInput>();

        private class BufferedInput
        {
            public Action Apply;
            public float TimeToApply;
        }

        [HarmonyPrefix]
        [HarmonyPatch("ClientTick")]
        private static bool Prefix(PlayerInput __instance)
        {
            // If latency is disabled, run original ClientTick
            if (delay <= 0f ||
            (ConfigData.Instance.OnlyInPracticeMode && !PracticeModeDetector.IsPracticeMode))
            {
                return true; // run original ClientTick with no modification
            }

            QueueAllInputs(__instance);
            ProcessQueue();

            return false; // Skip original ClientTick, we manually replay everything
        }

        private static void QueueAllInputs(PlayerInput input)
        {
            // Each block matches ClientTick, just wraps the RPC into a queued action
            if (input.MoveInput.HasChanged)
            {
                short x = (short)(input.MoveInput.ClientValue.x * 32767f);
                short y = (short)(input.MoveInput.ClientValue.y * 32767f);
                input.MoveInput.ClientTick();

                inputQueue.Enqueue(new BufferedInput
                {
                    TimeToApply = Time.time + delay,
                    Apply = () => input.Client_MoveInputRpc(x, y)
                });
            }

            if (input.StickRaycastOriginAngleInput.HasChanged)
            {
                short x = (short)(input.StickRaycastOriginAngleInput.ClientValue.x / 360f * 32767f);
                short y = (short)(input.StickRaycastOriginAngleInput.ClientValue.y / 360f * 32767f);
                input.StickRaycastOriginAngleInput.ClientTick();

                inputQueue.Enqueue(new BufferedInput
                {
                    TimeToApply = Time.time + delay,
                    Apply = () => input.Client_RaycastOriginAngleInputRpc(x, y)
                });
            }

            if (input.LookAngleInput.HasChanged)
            {
                short x = (short)(input.LookAngleInput.ClientValue.x / 360f * 32767f);
                short y = (short)(input.LookAngleInput.ClientValue.y / 360f * 32767f);
                input.LookAngleInput.ClientTick();

                inputQueue.Enqueue(new BufferedInput
                {
                    TimeToApply = Time.time + delay,
                    Apply = () => input.Client_LookAngleInputRpc(x, y)
                });
            }

            if (input.BladeAngleInput.HasChanged)
            {
                var val = input.BladeAngleInput.ClientValue;
                input.BladeAngleInput.ClientTick();

                inputQueue.Enqueue(new BufferedInput
                {
                    TimeToApply = Time.time + delay,
                    Apply = () => input.Client_BladeAngleInputRpc(val)
                });
            }

            if (input.SlideInput.HasChanged)
            {
                var val = input.SlideInput.ClientValue;
                input.SlideInput.ClientTick();

                inputQueue.Enqueue(new BufferedInput
                {
                    TimeToApply = Time.time + delay,
                    Apply = () => input.Client_SlideInputRpc(val)
                });
            }

            if (input.SprintInput.HasChanged)
            {
                var val = input.SprintInput.ClientValue;
                input.SprintInput.ClientTick();

                inputQueue.Enqueue(new BufferedInput
                {
                    TimeToApply = Time.time + delay,
                    Apply = () => input.Client_SprintInputRpc(val)
                });
            }

            if (input.TrackInput.HasChanged)
            {
                var val = input.TrackInput.ClientValue;
                input.TrackInput.ClientTick();

                inputQueue.Enqueue(new BufferedInput
                {
                    TimeToApply = Time.time + delay,
                    Apply = () => input.Client_TrackInputRpc(val)
                });
            }

            if (input.LookInput.HasChanged)
            {
                var val = input.LookInput.ClientValue;
                input.LookInput.ClientTick();

                inputQueue.Enqueue(new BufferedInput
                {
                    TimeToApply = Time.time + delay,
                    Apply = () => input.Client_LookInputRpc(val)
                });
            }

            if (input.JumpInput.HasChanged)
            {
                input.JumpInput.ClientTick();

                inputQueue.Enqueue(new BufferedInput
                {
                    TimeToApply = Time.time + delay,
                    Apply = () => input.Client_JumpInputRpc()
                });
            }

            if (input.StopInput.HasChanged)
            {
                var val = input.StopInput.ClientValue;
                input.StopInput.ClientTick();

                inputQueue.Enqueue(new BufferedInput
                {
                    TimeToApply = Time.time + delay,
                    Apply = () => input.Client_StopInputRpc(val)
                });
            }

            if (input.TwistLeftInput.HasChanged)
            {
                input.TwistLeftInput.ClientTick();

                inputQueue.Enqueue(new BufferedInput
                {
                    TimeToApply = Time.time + delay,
                    Apply = () => input.Client_TwistLeftInputRpc()
                });
            }

            if (input.TwistRightInput.HasChanged)
            {
                input.TwistRightInput.ClientTick();

                inputQueue.Enqueue(new BufferedInput
                {
                    TimeToApply = Time.time + delay,
                    Apply = () => input.Client_TwistRightInputRpc()
                });
            }

            if (input.DashLeftInput.HasChanged)
            {
                input.DashLeftInput.ClientTick();

                inputQueue.Enqueue(new BufferedInput
                {
                    TimeToApply = Time.time + delay,
                    Apply = () => input.Client_DashLeftInputRpc()
                });
            }

            if (input.DashRightInput.HasChanged)
            {
                input.DashRightInput.ClientTick();

                inputQueue.Enqueue(new BufferedInput
                {
                    TimeToApply = Time.time + delay,
                    Apply = () => input.Client_DashRightInputRpc()
                });
            }

            if (input.ExtendLeftInput.HasChanged)
            {
                var val = input.ExtendLeftInput.ClientValue;
                input.ExtendLeftInput.ClientTick();

                inputQueue.Enqueue(new BufferedInput
                {
                    TimeToApply = Time.time + delay,
                    Apply = () => input.Client_ExtendLeftInputRpc(val)
                });
            }

            if (input.ExtendRightInput.HasChanged)
            {
                var val = input.ExtendRightInput.ClientValue;
                input.ExtendRightInput.ClientTick();

                inputQueue.Enqueue(new BufferedInput
                {
                    TimeToApply = Time.time + delay,
                    Apply = () => input.Client_ExtendRightInputRpc(val)
                });
            }

            if (input.LateralLeftInput.HasChanged)
            {
                var val = input.LateralLeftInput.ClientValue;
                input.LateralLeftInput.ClientTick();

                inputQueue.Enqueue(new BufferedInput
                {
                    TimeToApply = Time.time + delay,
                    Apply = () => input.Client_LateralLeftInputRpc(val)
                });
            }

            if (input.LateralRightInput.HasChanged)
            {
                var val = input.LateralRightInput.ClientValue;
                input.LateralRightInput.ClientTick();

                inputQueue.Enqueue(new BufferedInput
                {
                    TimeToApply = Time.time + delay,
                    Apply = () => input.Client_LateralRightInputRpc(val)
                });
            }

            if (input.TalkInput.HasChanged)
            {
                var val = input.TalkInput.ClientValue;
                input.TalkInput.ClientTick();

                inputQueue.Enqueue(new BufferedInput
                {
                    TimeToApply = Time.time + delay,
                    Apply = () => input.Client_TalkInputRpc(val)
                });
            }
        }

        private static void ProcessQueue()
        {
            while (inputQueue.Count > 0 && Time.time >= inputQueue.Peek().TimeToApply)
            {
                inputQueue.Dequeue().Apply?.Invoke();
            }
        }
    }
}
