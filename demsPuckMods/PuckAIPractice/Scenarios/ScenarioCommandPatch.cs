using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace PuckAIPractice.Scenarios
{
    [HarmonyPatch(typeof(VoteManagerController), "Event_Server_OnChatCommand")]
    public static class ScenarioCommandPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(Dictionary<string, object> message)
        {
            var command = (string)message["command"];
            var callerId = (ulong)message["clientId"];

            if (command == "/sr")
            {
                ScenarioManager.Restart(callerId);
                return false;
            }

            if (command != "/scenario") return true;

            var args = (string[])message["args"];
            if (args == null || args.Length < 1)
            {
                Debug.Log("[Scenario] usage: /scenario <name> | /scenario stop");
                return false;
            }

            var first = args[0].ToUpperInvariant();
            if (first == "STOP")
            {
                ScenarioManager.Stop();
                return false;
            }

            var rest = new string[args.Length - 1];
            System.Array.Copy(args, 1, rest, 0, rest.Length);
            ScenarioManager.StartByName(first, callerId, rest);
            return false;
        }
    }
}
