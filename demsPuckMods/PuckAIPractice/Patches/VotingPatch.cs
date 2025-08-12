//using HarmonyLib;
//using PuckAIPractice.AI;
//using PuckAIPractice.GameModes;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;
//using Unity.Collections;
//using UnityEngine;

//namespace PuckAIPractice.Patches
//{
//    [HarmonyPatch(typeof(UIChatController))]
//    public static class UIChatController_Event_Server_OnVoteStarted_Prefix
//    {
//        Target the private method: private void Event_Server_OnVoteStarted(Dictionary<string, object> message)
//        static MethodBase TargetMethod() =>
//            AccessTools.Method(typeof(UIChatController), "Event_Server_OnVoteStarted",
//                new[] { typeof(Dictionary<string, object>) });

//        Return false = skip original(we fully handle it)
//        static bool Prefix(UIChatController __instance, Dictionary<string, object> message)
//        {
//            try
//            {
//                Debug.Log("No way is this hit...");
//                object voteObj;
//                if (message == null || !message.TryGetValue("vote", out voteObj) || !(voteObj is Vote))
//                    return true; // let original run
//                var vote = (Vote)voteObj;

//                Player startedBy = vote.StartedBy;
//                if (!startedBy)
//                    return false; // matches original early-return, but we swallow it

//                uiChat is a private field on UIChatController
//               var uiChatObj = Traverse.Create(__instance).Field("uiChat").GetValue();
//                if (uiChatObj == null)
//                {
//                    Debug.LogWarning("[Patch] UIChatController.uiChat was null; deferring to original.");
//                    return true; // fallback: let original try
//                }

//    var chatTr = Traverse.Create(uiChatObj);

//    uiChat.WrapPlayerUsername(startedBy)
//                string wrappedName = chatTr.Method("WrapPlayerUsername", new object[] { startedBy })
//                                       .GetValue<string>();

//                switch ((int) vote.Type)
//                {
//                    case 777:
//                        {
//                            var pm = NetworkBehaviourSingleton<PlayerManager>.Instance;
//                            if (pm == null) return true;

//                            FixedString32Bytes steamId = (FixedString32Bytes)vote.Data;
//    Player target = pm.GetPlayerBySteamId(steamId);
//                            if (!target)
//                                return false; // original would return; swallow

//                            string text = string.Format(
//                                "{0} has started a vote to change goalie difficulty to #{1} {2}. (1/{3})",
//                                wrappedName,
//                                target.Number.Value,
//                                target.Username.Value,
//                                vote.VotesNeeded);
//    chatTr.Method("Server_SendSystemChatMessage", new object[] { text
//}).GetValue();
//return false;
//                        }
//                    default:
//                        return false; // original default: return;
//                }
//            }
//            catch (Exception ex)
//            {
//                Debug.LogError($"[Patch] Event_Server_OnVoteStarted Prefix error: {ex}");
//If anything goes wrong, let the original run to avoid breaking chat.
//                return true;
//            }
//        }
//    }
//    [HarmonyPatch(typeof(UIChatController))]
//static class UIChatController_OnVoteProgress_Prefix
//{
//    private void Event_Server_OnVoteProgress(Dictionary<string, object> message)
//        static MethodBase TargetMethod() =>
//            AccessTools.Method(typeof(UIChatController), "Event_Server_OnVoteProgress",
//                new[] { typeof(Dictionary<string, object>) });

//    static bool Prefix(UIChatController __instance, Dictionary<string, object> message)
//    {
//        try
//        {
//            if (message == null) return true;

//            object voteObj;
//            if (!message.TryGetValue("vote", out voteObj) || !(voteObj is Vote)) return true;
//            var vote = (Vote)voteObj;

//            Only handle our custom type; let stock code handle others
//                if ((int)vote.Type != 777) return true;

//            object voterObj;
//            if (!message.TryGetValue("voter", out voterObj) || !(voterObj is Player)) return true;
//            var voter = (Player)voterObj;
//            if (!voter) return false; // matches original early-return (swallow)

//            Get private uiChat field
//                var uiChat = Traverse.Create(__instance).Field("uiChat").GetValue();
//                if (uiChat == null) return true;
//                var chatTr = Traverse.Create(uiChat);

//    Wrap name using existing UI formatting
//                string wrappedName = chatTr.Method("WrapPlayerUsername", new object[] { voter }).GetValue<string>();

//Pull desired difficulty from vote.Data (support int/string/FixedString32Bytes)
//                int diff = 1;
//if (vote.Data is int) diff = (int)vote.Data;
//else if (vote.Data is string) diff = CustomVotes.ParseDifficulty((string)vote.Data);
//else if (vote.Data is FixedString32Bytes) diff = CustomVotes.ParseDifficulty(((FixedString32Bytes)vote.Data).ToString());

//string msg = string.Format("{0} voted to set goalie difficulty to {1}. ({2}/{3})",
//                           wrappedName, "test", vote.Votes, vote.VotesNeeded);

//chatTr.Method("Server_SendSystemChatMessage", new object[] { msg }).GetValue();

//return false; // handled; skip original switch
//            }
//            catch (Exception ex)
//            {
//                Debug.LogError("[CustomVotes] OnVoteProgress Prefix error: " + ex);
//return true; // fail-safe
//            }
//        }
//    }
//    public static class CustomVoteTypes
//{
//    public const int GoalieDifficultyValue = 777;
//    public static readonly VoteType GoalieDifficulty = (VoteType)GoalieDifficultyValue;

//    public static bool IsGoalieDifficulty(VoteType t) { return (int)t == GoalieDifficultyValue; }
//}
//}