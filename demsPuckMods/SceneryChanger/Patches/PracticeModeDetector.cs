using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Netcode;
using UnityEngine;

namespace SceneryChanger.Patches
{
    public static class PracticeModeDetector
    {
        public static bool IsPracticeMode { get; private set; } = false;

        [HarmonyPatch]
        public static class Patch_Emit
        {
            static MethodBase TargetMethod()
            {
                var type = AccessTools.TypeByName("WebSocketManager");
                return type == null ? null : AccessTools.Method(type, "Emit");
            }

            [HarmonyPostfix]
            static void Postfix(string messageName, Dictionary<string, object> data, string responseMessageName)
            {
                try
                {
                    if (messageName == "serverAuthenticateRequest" && data != null && data.ContainsKey("name"))
                    {
                        string name = data["name"]?.ToString() ?? "";
                        IsPracticeMode = name.ToUpperInvariant() == "PRACTICE";
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        public static void OnClientStart(string ip, ushort port, string password, string connectionDataJson)
        {
            IsPracticeMode = false;

            try
            {
                if (connectionDataJson != null && connectionDataJson.Contains("\"name\":\"PRACTICE\""))
                {
                    IsPracticeMode = true;
                }
            }
            catch
            {
            }
        }

        public static void OnClientDisconnect()
        {
            IsPracticeMode = false;
        }

        [HarmonyPatch]
        public static class Patch_ClientStartClient
        {
            static MethodBase TargetMethod()
            {
                var type = AccessTools.TypeByName("ConnectionManager");
                return type == null ? null : AccessTools.Method(type, "Client_StartClient");
            }

            [HarmonyPostfix]
            static void Postfix(object __instance, string ipAddress, ushort port, string password)
            {
                string json = null;
                if (__instance != null && NetworkManager.Singleton != null)
                {
                    json = System.Text.Encoding.ASCII.GetString(NetworkManager.Singleton.NetworkConfig.ConnectionData);
                }

                OnClientStart(ipAddress, port, password, json);
            }
        }

        [HarmonyPatch]
        public static class Patch_ClientDisconnect
        {
            static MethodBase TargetMethod()
            {
                var type = AccessTools.TypeByName("ConnectionManager");
                return type == null ? null : AccessTools.Method(type, "Client_Disconnect");
            }

            [HarmonyPostfix]
            static void Postfix()
            {
                OnClientDisconnect();
            }
        }
    }
}
