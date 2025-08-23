using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestProject.Behaviors;
using TestProject.Models;
using TestProject.Utilities;
using UnityEngine;

namespace TestProject.Patches
{
    [HarmonyPatch]
    static class UIChat_WelcomePatch
    {
        // Access the private field UIChatController.uiChat
        private static readonly AccessTools.FieldRef<UIChatController, UIChat>
            _uiChatRef = AccessTools.FieldRefAccess<UIChatController, UIChat>("uiChat");

        // ---- knobs you can change at runtime if you want
        public static bool ReplaceWelcome = true;  // true = replace original; false = let original run
        public static string CustomWelcome =
            "!MOTD ";

        // Prefix: run before original
        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(UIChatController),
        //    "Event_Server_OnSynchronizeComplete",
        //    new Type[] { typeof(Dictionary<string, object>) })]
        //static bool Prefix(UIChatController __instance, Dictionary<string, object> message)
        //{
        //    if (!ReplaceWelcome)
        //        return true; // let the game send its own message

        //    var uiChat = _uiChatRef(__instance);
        //    if (uiChat == null) return false; // skip original anyway

        //    // message["clientId"] can be boxed as int/long/ulong; be tolerant
        //    ulong clientId = 0;
        //    try { clientId = Convert.ToUInt64(message?["clientId"]); } catch { /* ignore */ }

        //    uiChat.Server_SendSystemChatMessage(CustomWelcome, clientId);

        //    return false; // skip original welcome
        //}

        // --- If instead you want to APPEND your own message AFTER the original,
        // comment out the Prefix above and use this Postfix:

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIChatController),
            "Event_Server_OnSynchronizeComplete",
            new Type[] { typeof(Dictionary<string, object>) })]
        static void Postfix(UIChatController __instance, Dictionary<string, object> message)
        {
            var uiChat = _uiChatRef(__instance);
            if (uiChat == null) return;

            ulong clientId = 0;
            try { clientId = Convert.ToUInt64(message?["clientId"]); } catch { }
            string test = File.ReadAllText(@"C:\Users\Tom\Documents\TestFilesForPuck\MOTD.json");
            uiChat.Server_SendSystemChatMessage(CustomWelcome + test, clientId);
        }
        [HarmonyPatch(typeof(UIChat))]
        public static class UIChat_AddChatMessage_MotdPatch
        {
            [HarmonyPrefix]
            [HarmonyPatch(nameof(UIChat.AddChatMessage), new Type[] { typeof(string) })]
            static bool Prefix(UIChat __instance, ref string message)
            {
                // (Optional) ensure this is a client/host
                var nm = __instance?.NetworkManager;
                if (nm != null && !nm.IsClient) return true; // not a client -> let original run

                // Normalize the text a little, then check for the command
                var text = (message ?? string.Empty).Trim();
                if (!IsMotdCommand(text)) return true; // not MOTD -> allow original

                Debug.Log("Is MOTD Command");
                string error = "";
                ModalDoc doc;
                ModalDocIO.TryLoad(message, out doc, out error);
                if (error == "")
                {
                    SimpleModal.Show(
    title: doc.title,
    richText: ModalDocIO.MdToUnity(doc.richText),
    dontShowKey: doc.dontShowKey
);
                }
                return false; // <-- skip original AddChatMessage (do not print "!MOTD")
            }

            static bool IsMotdCommand(string s)
            {
                // Accept "!motd" or "/motd" (case-insensitive), optionally with trailing spaces
                if (s.ToUpper().Contains("!MOTD"))
                {
                    return true;
                }
                else { return false; }
            }
        }
    }
}
