using HarmonyLib;
using Newtonsoft.Json.Linq;
using PuckAIPractice.AI;
using PuckAIPractice.GameModes;
using PuckAIPractice.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics.Geometry;
using Unity.Netcode;
using UnityEngine;

namespace PuckAIPractice.Patches
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
            ulong clientId = (ulong)message["clientId"];
            string[] parsedCommand = (string[])message["args"];
            Player playerByClientId = NetworkBehaviourSingleton<PlayerManager>.Instance.GetPlayerByClientId(clientId);
            VoteChatCommandHelper.VotesNeeded = Mathf.RoundToInt((float)NetworkBehaviourSingleton<PlayerManager>.Instance.GetPlayers(false).Count / 2f + 0.5f);

            if (command == "/goalies" && parsedCommand.Count() == 1)
            {
                VoteManager voteManager = (VoteManager)Traverse.Create(__instance).Field("voteManager").GetValue();
                var vm = Traverse.Create(voteManager);
                Player player = NetworkBehaviourSingleton<PlayerManager>.Instance.GetPlayerByUsername("dem", false);
                List<string> goalieData = new List<string>() { "both", parsedCommand[0] };
                //if (!vm.Method("Server_IsVoteStarted", new object[] { CustomVoteTypes.GoalieDifficulty }).GetValue<bool>())
                //{
                //    vm.Method("Server_CreateVote", new object[] { CustomVoteTypes.GoalieDifficulty, VoteChatCommandHelper.VotesNeeded, clientId, goalieData /* your data */ })
                //      .GetValue();
                //}
                //vm.Method("Server_SubmitVote", new object[] { CustomVoteTypes.GoalieDifficulty, clientId }).GetValue();
                Player sender = NetworkBehaviourSingleton<PlayerManager>.Instance.GetPlayerByClientId(clientId);
                if (sender != null)
                {
                    return SpawnGoaliesBasedOffCommand(parsedCommand[0], GoalieSession.Both);
                }
            }
            else if (command == "/goalie" && parsedCommand.Count() == 2) 
            {
                //Debug.Log("[GOALIES] Custom command triggered!");
                //Debug.Log(parsedCommand.Count());
                // You can extract the caller ID if needed
                Player sender = NetworkBehaviourSingleton<PlayerManager>.Instance.GetPlayerByClientId(clientId);
                //Debug.Log($"[GOALIES] Command issued by {sender.Username.Value}");
                if (sender != null)
                {
                    GoalieSession type = GoalieSession.Red;
                    if (parsedCommand[1].ToLower() == "red")
                    {
                        type = GoalieSession.Red;
                    }
                    else if (parsedCommand[1].ToLower() == "blue")
                    {
                        type = GoalieSession.Blue;
                    }
                    else
                    {
                        return true;
                    }
                    return SpawnGoaliesBasedOffCommand(parsedCommand[0], type);
                }
            }
            else if (command == "/endgoaliesession")
            {
                //Debug.Log("End Goalie Session");
                Goalies.EndGoalieSession(GoalieSession.Both);
            }
            else
            {
                return true;
            }
                 // Not our command, continue with original method
            return false; // Skip original voting logic
        }
        public static void ApplyGoalieSettings(string difficulty, GoalieSession type)
        {
            if (difficulty.ToLower() == "easy")
            {
                if (type == GoalieSession.Red)
                {
                    GoalieSettings.InstanceRed.ApplyDifficulty(GoalieDifficulty.Easy);
                }
                else if (type == GoalieSession.Blue)
                {
                    GoalieSettings.InstanceBlue.ApplyDifficulty(GoalieDifficulty.Easy);
                }
                else
                {
                    GoalieSettings.InstanceRed.ApplyDifficulty(GoalieDifficulty.Easy);
                    GoalieSettings.InstanceBlue.ApplyDifficulty(GoalieDifficulty.Easy);
                }
            }
            else if (difficulty.ToLower() == "normal")
            {
                if (type == GoalieSession.Red)
                {
                    GoalieSettings.InstanceRed.ApplyDifficulty(GoalieDifficulty.Normal);
                }
                else if (type == GoalieSession.Blue)
                {
                    GoalieSettings.InstanceBlue.ApplyDifficulty(GoalieDifficulty.Normal);
                }
                else
                {
                    GoalieSettings.InstanceRed.ApplyDifficulty(GoalieDifficulty.Normal);
                    GoalieSettings.InstanceBlue.ApplyDifficulty(GoalieDifficulty.Normal);
                }
            }
            else if (difficulty.ToLower() == "hard")
            {
                if (type == GoalieSession.Red)
                {
                    GoalieSettings.InstanceRed.ApplyDifficulty(GoalieDifficulty.Hard);
                }
                else if (type == GoalieSession.Blue)
                {
                    GoalieSettings.InstanceBlue.ApplyDifficulty(GoalieDifficulty.Hard);
                }
                else
                {
                    GoalieSettings.InstanceRed.ApplyDifficulty(GoalieDifficulty.Hard);
                    GoalieSettings.InstanceBlue.ApplyDifficulty(GoalieDifficulty.Hard);
                }
            }
        }
        public static bool SpawnGoaliesBasedOffCommand(string difficulty, GoalieSession type)
        {
            
            if (!Goalies.GoaliesAreRunning)
            {
                ApplyGoalieSettings(difficulty, type);
                if (difficulty.ToLower() == "easy")
                {
                    Goalies.GoaliesAreRunning = true;
                    //Goalies.StartGoalieSessionViaCoroutine(type);
                }
                else if (difficulty.ToLower() == "normal")
                {
                    Goalies.GoaliesAreRunning = true;
                    //Goalies.StartGoalieSessionViaCoroutine(type);
                }
                else if (difficulty.ToLower() == "hard")
                {
                    Goalies.GoaliesAreRunning = true;
                    //Goalies.StartGoalieSessionViaCoroutine(type);
                }
                else
                {
                    return true;
                }
            }
            else
            {
                if (difficulty.ToLower() != "end")
                {
                    Goalies.GoaliesAreRunning = false;
                    ApplyGoalieSettings(difficulty, type);
                    if (difficulty.ToLower() == "easy")
                    {
                        //BotSpawning.DespawnBots(type);
                        Goalies.EndGoalieSession(type);
                        //Goalies.StartGoalieSessionViaCoroutine(type);
                        Goalies.GoaliesAreRunning = true;
                    }
                    else if (difficulty.ToLower() == "normal")
                    {
                        //BotSpawning.DespawnBots(type);
                        Goalies.EndGoalieSession(type);
                        //Goalies.StartGoalieSessionViaCoroutine(type)
                        //Goalies.GoaliesAreRunning = true;;
                    }
                    else if (difficulty.ToLower() == "hard")
                    {
                        //BotSpawning.DespawnBots(type);
                        Goalies.EndGoalieSession(type);
                        //Goalies.StartGoalieSessionViaCoroutine(type);
                        Goalies.GoaliesAreRunning = true;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    //Goalies.EndGoalieSession(type);
                }
            }
            return false;
        }
    }
}