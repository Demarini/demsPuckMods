using HarmonyLib;
using System.Reflection;
using System;
using UnityEngine;

namespace FixChat
{
    public class FixChat : IPuckMod
    {
        private static readonly Harmony harmony = new Harmony("GAFURIX.FixChat");

        public bool OnEnable()
        {
            Debug.Log("[FixChat] Mod enabled");
            //HarmonyFileLog.Enabled = true;
            try
            {
                harmony.PatchAll();
                Debug.Log("[FixChat] Harmony patches applied");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[FixChat] Harmony patch failed: {e.Message}");
                return false;
            }

            return true;
        }

        public bool OnDisable()
        {
            try
            {
                harmony.UnpatchSelf();
                Debug.Log("[FixChat] Mod disabled");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[FixChat] Harmony unpatch failed: {e.Message}");
                return false;
            }

            return true;
        }
    }
    [HarmonyPatch(typeof(UIChat), "Update")]
    public static class UIChat_UpdatePatch
    {
        private static float nextRemoteMessageTime = 0f;
        private static int remoteMessageIndex = 0;
        private static readonly string[] remoteNames = { "TrashMan", "Goblin", "Zoomer99", "Bot_David", "Greg" };

        public static void Postfix(UIChat __instance)
        {
            // Check if it's time to send another spoofed message
            if (Time.time >= nextRemoteMessageTime)
            {
                string sender = remoteNames[UnityEngine.Random.Range(0, remoteNames.Length)];
                string message = $"{sender}: Message #{remoteMessageIndex++}";

                // Try to call AddChatMessage(string)
                var addMethod = AccessTools.Method(typeof(UIChat), "AddChatMessage", new[] { typeof(string) });
                if (addMethod != null)
                {
                    addMethod.Invoke(__instance, new object[] { message });
                }
                else
                {
                    Debug.Log("[SpoofChat] Couldn't find AddChatMessage(string) method.");
                }

                nextRemoteMessageTime = Time.time + 5f;
            }
        }
    }
}
