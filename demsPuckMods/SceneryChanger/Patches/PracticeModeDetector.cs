using HarmonyLib;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace SceneryChanger.Patches
{
    [HarmonyPatch(typeof(WebSocketManager))]
    public static class PracticeModeDetector
    {
        public static bool IsPracticeMode { get; private set; } = false;

        [HarmonyPostfix]
        [HarmonyPatch("Emit")]
        private static void Emit_Postfix(string messageName, Dictionary<string, object> data, string responseMessageName)
        {
            try
            {
                if (messageName == "serverAuthenticateRequest" && data != null && data.ContainsKey("name"))
                {
                    string name = data["name"]?.ToString() ?? "";
                    IsPracticeMode = name.ToUpperInvariant() == "PRACTICE";

                }
            }
            catch (Exception ex)
            {

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
                    //MOTDLogger.Log(LogCategory.PracticeModeDetection, "Practice Mode Detected via ConnectionManager: True");
                }
            }
            catch
            {
                // Swallow silently (no logging here)
            }
        }

        public static void OnClientDisconnect()
        {
            if (IsPracticeMode)
                //MOTDLogger.Log(LogCategory.PracticeModeDetection, "Left Practice Mode (Disconnect)");

            IsPracticeMode = false;
        }

        [HarmonyPatch(typeof(ConnectionManager))]
        public static class ConnectionManagerPracticePatches
        {
            [HarmonyPostfix]
            [HarmonyPatch("Client_StartClient")]
            private static void AfterStartClient(ConnectionManager __instance, string ipAddress, ushort port, string password)
            {
                string json = null;
                if (__instance != null && NetworkManager.Singleton != null)
                {
                    json = System.Text.Encoding.ASCII.GetString(NetworkManager.Singleton.NetworkConfig.ConnectionData);
                }

                PracticeModeDetector.OnClientStart(ipAddress, port, password, json);
            }

            [HarmonyPostfix]
            [HarmonyPatch("Client_Disconnect")]
            private static void AfterDisconnect()
            {
                PracticeModeDetector.OnClientDisconnect();
            }
        }
    }
}
