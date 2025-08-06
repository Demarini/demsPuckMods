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
            if (parsedCommand[0].ToLower() != "/goalies" && parsedCommand.Count() != 1)
                return true; // Not our command, continue with original method

            // Execute your goalie spawning logic here
            Debug.Log("[GOALIES] Custom command triggered!");
            Debug.Log(parsedCommand.Count());
            // You can extract the caller ID if needed
            ulong clientId = (ulong)message["clientId"];
            Player sender = NetworkBehaviourSingleton<PlayerManager>.Instance.GetPlayerByClientId(clientId);
            
            if (sender != null)
            {
                Debug.Log($"[GOALIES] Command issued by {sender.Username.Value}");
                if (!Goalies.GoaliesAreRunning)
                {
                    if(parsedCommand[0].ToLower() == "easy")
                    {
                        GoalieSettings.Instance.ApplyDifficulty(GoalieDifficulty.Easy);
                        Goalies.StartGoalieSessionViaCoroutine();
                    }
                    else if (parsedCommand[0].ToLower() == "normal")
                    {
                        GoalieSettings.Instance.ApplyDifficulty(GoalieDifficulty.Normal);
                        Goalies.StartGoalieSessionViaCoroutine();
                    }
                    else if (parsedCommand[0].ToLower() == "hard")
                    {
                        GoalieSettings.Instance.ApplyDifficulty(GoalieDifficulty.Hard);
                        Goalies.StartGoalieSessionViaCoroutine();
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    ;
                    if (parsedCommand[0].ToLower() != "end")
                    {
                        if (parsedCommand[0].ToLower() == "easy")
                        {
                            Goalies.EndGoalieSession();
                            GoalieSettings.Instance.ApplyDifficulty(GoalieDifficulty.Easy);
                            Goalies.StartGoalieSessionViaCoroutine();
                        }
                        else if (parsedCommand[0].ToLower() == "normal")
                        {
                            Goalies.EndGoalieSession();
                            GoalieSettings.Instance.ApplyDifficulty(GoalieDifficulty.Normal);
                            Goalies.StartGoalieSessionViaCoroutine();
                        }
                        else if (parsedCommand[0].ToLower() == "hard")
                        {
                            Goalies.EndGoalieSession();
                            GoalieSettings.Instance.ApplyDifficulty(GoalieDifficulty.Hard);
                            Goalies.StartGoalieSessionViaCoroutine();
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        Goalies.EndGoalieSession();
                    }
                }

            }

            return false; // Skip original voting logic
        }
    }
}