using HarmonyLib;
using PuckAIPractice.GameModes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuckAIPractice.Patches
{
    [HarmonyPatch(typeof(ServerManagerController), "Event_OnClientDisconnected")]
    [HarmonyPatch(new Type[] { typeof(Dictionary<string, object>) })]
    public static class OnClientDisconnectedPatch
    {
        public static void Postfix(Dictionary<string, object> message)
        {
            //DebugGUI.Log("Disconnected Goalie Session");
            //if (Goalies.GoaliesAreRunning)
            //{
            //    Goalies.EndGoalieSession();
            //}
            //if (message.TryGetValue("clientId", out object clientIdObj) && clientIdObj is ulong clientId)
            //{
            //    //Debug.Log($"[Postfix] Client {clientId} disconnected. (Postfix triggered!)");
                
            //    // You can do any custom logic here
            //    // For example: track rejoin attempts, log, cleanup mod-specific data, etc.
            //}
            //else
            //{
            //    //Debug.LogWarning("[Postfix] Could not extract clientId from message.");
            //}
        }
    }
    [HarmonyPatch(typeof(ConnectionManagerController), "Event_Client_OnPauseMenuClickDisconnect")]
    [HarmonyPatch(new Type[] { typeof(Dictionary<string, object>) })]
    public static class OnPauseMenuClickDisconnectdPatch
    {
        public static void Postfix(Dictionary<string, object> message)
        {
            DebugGUI.Log("Disconnected Goalie Session");
            if (Goalies.GoaliesAreRunning)
            {
                Goalies.EndGoalieSession(GoalieSession.Both);
            }
            if (message.TryGetValue("clientId", out object clientIdObj) && clientIdObj is ulong clientId)
            {
                //Debug.Log($"[Postfix] Client {clientId} disconnected. (Postfix triggered!)");

                // You can do any custom logic here
                // For example: track rejoin attempts, log, cleanup mod-specific data, etc.
            }
            else
            {
                //Debug.LogWarning("[Postfix] Could not extract clientId from message.");
            }
        }
    }
}
