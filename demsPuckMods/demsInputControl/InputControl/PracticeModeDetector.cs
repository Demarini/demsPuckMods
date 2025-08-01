using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace demsInputControl.InputControl
{
    [HarmonyPatch(typeof(ConnectionManager))]
    public static class ConnectionManagerPracticePatches
    {
        //[HarmonyPostfix]
        //[HarmonyPatch("Client_StartClient")]
        //private static void AfterStartClient(ConnectionManager __instance, string ipAddress, ushort port, string password)
        //{
        //    // Extract the ConnectionData JSON from NetworkConfig (it holds PRACTICE flag in logs)
        //    string json = null;
        //    if (__instance != null && NetworkManager.Singleton != null)
        //    {
        //        json = System.Text.Encoding.ASCII.GetString(NetworkManager.Singleton.NetworkConfig.ConnectionData);
        //    }

        //    PracticeModeDetector.OnClientStart(ipAddress, port, password, json);
        //}

        //[HarmonyPostfix]
        //[HarmonyPatch("Client_Disconnect")]
        //private static void AfterDisconnect()
        //{
        //    PracticeModeDetector.OnClientDisconnect();
        //}
    }
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

                    Debug.Log($"[InputControl] Practice Mode Detected: {IsPracticeMode}");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[InputControl] Practice detection failed: {ex.Message}");
            }
        }
        public static void OnClientStart(string ip, ushort port, string password, string connectionDataJson)
        {
            // Default to not practice
            IsPracticeMode = false;

            try
            {
                // If the JSON contains "PRACTICE" as the server name, it's practice mode
                if (connectionDataJson != null && connectionDataJson.Contains("\"name\":\"PRACTICE\""))
                {
                    IsPracticeMode = true;
                    Debug.Log("[InputControl] Practice Mode Detected via ConnectionManager: True");
                }
            }
            catch { }
        }

        // Call this from a Harmony patch on ConnectionManager.Client_Disconnect
        public static void OnClientDisconnect()
        {
            if (IsPracticeMode)
                Debug.Log("[InputControl] Left Practice Mode (Disconnect)");
            IsPracticeMode = false;
        }
        [HarmonyPatch(typeof(ConnectionManager))]
        public static class ConnectionManagerPracticePatches
        {
            [HarmonyPostfix]
            [HarmonyPatch("Client_StartClient")]
            private static void AfterStartClient(ConnectionManager __instance, string ipAddress, ushort port, string password)
            {
                // Extract the ConnectionData JSON from NetworkConfig (it holds PRACTICE flag in logs)
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
