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
using UnityEngine;

namespace PuckAIPractice.Patches
{
    
    [HarmonyPatch(typeof(VoteManagerController), "Event_Server_OnChatCommand")]
    public static class GoaliesCommandPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(Dictionary<string, object> message)
        {
            string command = (string)message["command"];
            Debug.Log(command);
            string[] parsedCommand = (string[])message["args"];
            Debug.Log(parsedCommand[0]);
            if (command == "/goalies" && parsedCommand.Count() == 1)
            {
                // Execute your goalie spawning logic here
                Debug.Log("[GOALIES] Custom command triggered!");
                Debug.Log(parsedCommand.Count());
                // You can extract the caller ID if needed
                ulong clientId = (ulong)message["clientId"];
                Player sender = NetworkBehaviourSingleton<PlayerManager>.Instance.GetPlayerByClientId(clientId);
                Debug.Log($"[GOALIES] Command issued by {sender.Username.Value}");
                if (sender != null)
                {
                    return SpawnGoaliesBasedOffCommand(parsedCommand[0], GoalieSession.Both);
                }
            }
            else if (command == "/goalie" && parsedCommand.Count() == 2) 
            {
                Debug.Log("[GOALIES] Custom command triggered!");
                Debug.Log(parsedCommand.Count());
                // You can extract the caller ID if needed
                ulong clientId = (ulong)message["clientId"];
                Player sender = NetworkBehaviourSingleton<PlayerManager>.Instance.GetPlayerByClientId(clientId);
                Debug.Log($"[GOALIES] Command issued by {sender.Username.Value}");
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
                    GoalieSettings.InstanceRed.ApplyDifficulty(GoalieDifficulty.Easy);
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
                    GoalieSettings.InstanceRed.ApplyDifficulty(GoalieDifficulty.Normal);
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
                    GoalieSettings.InstanceRed.ApplyDifficulty(GoalieDifficulty.Hard);
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
                    Goalies.StartGoalieSessionViaCoroutine(type);
                }
                else if (difficulty.ToLower() == "normal")
                {
                    Goalies.StartGoalieSessionViaCoroutine(type);
                }
                else if (difficulty.ToLower() == "hard")
                {
                    Goalies.StartGoalieSessionViaCoroutine(type);
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
                    ApplyGoalieSettings(difficulty, type);
                    if (difficulty.ToLower() == "easy")
                    {
                        Goalies.EndGoalieSession(type);
                        Goalies.StartGoalieSessionViaCoroutine(type);
                    }
                    else if (difficulty.ToLower() == "normal")
                    {
                        Goalies.EndGoalieSession(type);
                        Goalies.StartGoalieSessionViaCoroutine(type);
                    }
                    else if (difficulty.ToLower() == "hard")
                    {
                        Goalies.EndGoalieSession(type);
                        Goalies.StartGoalieSessionViaCoroutine(type);
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    Goalies.EndGoalieSession(type);
                }
            }
            return false;
        }
    }
}