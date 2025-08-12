using HarmonyLib;
using PuckAIPractice.AI;
using PuckAIPractice.GameModes;
using PuckAIPractice.Singletons;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace PuckAIPractice.Patches
{
    [HarmonyPatch(typeof(WebSocketManager))]
    public static class PracticeModeDetector
    {
        public static bool IsPracticeMode { get; set; } = false;

        [HarmonyPostfix]
        [HarmonyPatch("Emit")]
        private static void Emit_Postfix(string messageName, Dictionary<string, object> data, string responseMessageName)
        {
            try
            {
                //Debug.Log("Client Start");
                if (messageName == "serverAuthenticateRequest" && data != null && data.ContainsKey("name"))
                {
                    //Debug.Log("Setting Practice Mode");
                    string name = data["name"]?.ToString() ?? "";
                    //Debug.Log(name.ToUpperInvariant());
                    IsPracticeMode = name.ToUpperInvariant() == "PRACTICE";

                    if (ConfigData.Instance.StartWithBlueGoalie)
                    {
                        //Debug.Log("starting with blue goalie");
                        //Debug.Log(ConfigData.Instance.BlueGoalieDefaultDifficulty.ToString());
                        //GoalieSettings.InstanceBlue.ApplyDifficulty(ConfigData.Instance.BlueGoalieDefaultDifficulty);
                        //Goalies.StartGoalieSessionViaCoroutine(GoalieSession.Blue);
                    }
                    if (ConfigData.Instance.StartWithRedGoalie)
                    {
                        //GoalieSettings.InstanceRed.ApplyDifficulty(ConfigData.Instance.RedGoalieDefaultDifficulty);
                        //Goalies.StartGoalieSessionViaCoroutine(GoalieSession.Red);
                    }
                    //InputControlLogger.Log(LogCategory.PracticeModeDetection, $"Practice Mode Detected: {IsPracticeMode}");
                }
            }
            catch (Exception ex)
            {
                //InputControlLogger.LogError(LogCategory.PracticeModeDetection, $"Practice detection failed: {ex.Message}");
            }
        }

        public static void OnClientStart(string ip, ushort port, string password, string connectionDataJson)
        {
            //Debug.Log("Client Start");
            IsPracticeMode = false;

            try
            {
                if (connectionDataJson != null && connectionDataJson.Contains("\"name\":\"PRACTICE\""))
                {
                    //Debug.Log("Practice Mode");
                    //InputControlLogger.Log(LogCategory.PracticeModeDetection, "Practice Mode Detected via ConnectionManager: True");
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
                //InputControlLogger.Log(LogCategory.PracticeModeDetection, "Left Practice Mode (Disconnect)");

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
