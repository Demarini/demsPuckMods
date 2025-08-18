using demsInputControl.InputControl;
using HarmonyLib;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics.Geometry;
using Unity.Netcode;
using UnityEngine;
using demsInputControl.Singletons;
namespace demsInputControl.Patches
{
    public static class VoteChatCommandHelper
    {
        public static bool IsGoalieVoteStarted { get; set; } = false;
        public static int TotalVotes { get; set; } = 0;
        public static int VotesNeeded { get; set; } = 0;
    }
    [HarmonyPatch(typeof(VoteManagerController), "Event_Server_OnChatCommand")]
    public static class GoaliesCommandPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(VoteManagerController __instance, Dictionary<string, object> message)
        {
            if (!PracticeModeDetector.IsPracticeMode)
            {
                return true;
            }
            string command = (string)message["command"];
            string[] parsedCommand = (string[])message["args"];
            if (command == "/delay" && parsedCommand.Count() == 1)
            {
                Debug.Log("Command Received: " + parsedCommand[0]);
                int delay = 0;
                if (int.TryParse(parsedCommand[0], out delay))
                {
                    ConfigData.Instance.DelayInputs.ArtificialLatencyMs = delay;
                }
            } 
            else
            {
                return true;
            }     // Not our command, continue with original method
            return false; // Skip original voting logic
        }
    }
}