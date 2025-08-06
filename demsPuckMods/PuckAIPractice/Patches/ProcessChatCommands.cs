using HarmonyLib;
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
            if (command != "/goalies")
                return true; // Not our command, continue with original method

            // Execute your goalie spawning logic here
            Debug.Log("[GOALIES] Custom command triggered!");

            // You can extract the caller ID if needed
            ulong clientId = (ulong)message["clientId"];
            Player sender = NetworkBehaviourSingleton<PlayerManager>.Instance.GetPlayerByClientId(clientId);

            
            if (sender != null)
            {
                Debug.Log($"[GOALIES] Command issued by {sender.Username.Value}");
                if (!Goalies.GoaliesAreRunning)
                {
                    Goalies.StartGoalieSessionViaCoroutine();
                }
                else
                {
                    Goalies.EndGoalieSession();
                }

            }

            return false; // Skip original voting logic
        }
    }
}