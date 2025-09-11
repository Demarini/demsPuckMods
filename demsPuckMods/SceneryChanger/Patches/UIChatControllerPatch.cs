using HarmonyLib;
using Newtonsoft.Json;
using SceneryChanger.Model;
using SceneryChanger.Services;
using SceneryLoader.Behaviors;
using SceneryLoader.Services;
using SceneryLoader.Singletons;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneryChanger.Patches
{
    [HarmonyPatch]
    public static class UIChatControllerPatch
    {
        // Access the private field UIChatController.uiChat
        private static readonly AccessTools.FieldRef<UIChatController, UIChat>
            _uiChatRef = AccessTools.FieldRefAccess<UIChatController, UIChat>("uiChat");

        // ---- knobs you can change at runtime if you want
        public static bool ReplaceWelcome = true;  // true = replace original; false = let original run
        public static string MOTDCommand =
            "!MOTD ";

        public static SceneInformation sceneInformation = null;
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

            UnityEngine.Debug.Log("Sending loadmap command");
            var uiChat = _uiChatRef(__instance);
            if (uiChat == null) return;

            ulong clientId = 0;
            try { clientId = Convert.ToUInt64(message?["clientId"]); } catch { }
            UnityEngine.Debug.Log(ConfigData.Instance.sceneInformation);
            //ConfigData.Load();
            uiChat.Server_SendSystemChatMessage("!LoadMap " + MessageObfuscation.Encode(JsonConvert.SerializeObject(ConfigData.Instance.sceneInformation)), clientId);
        }
        [HarmonyPatch(typeof(UIChat))]
        public static class UIChat_AddChatMessage_MotdPatch
        {
            [HarmonyPrefix]
            [HarmonyPatch(nameof(UIChat.AddChatMessage), new Type[] { typeof(string) })]
            static bool Prefix(UIChat __instance, ref string message)
            {
                //UnityEngine.Debug.Log("Received Message");
                //UnityEngine.Debug.Log(message);
                if (string.IsNullOrWhiteSpace(message)) return true;

                var m = message.Trim();
                const string cmd = "!LoadMap ";

                if (!m.StartsWith(cmd)) return true;

                // grab the encoded payload after the first space
                var encoded = m.Substring(cmd.Length).Trim();

                // if your transport ever turns '+' into spaces, fix it up:
                if (encoded.IndexOf(' ') >= 0 && encoded.IndexOf('+') < 0)
                    encoded = encoded.Replace(' ', '+');

                string json;
                try
                {
                    json = MessageObfuscation.Decode(encoded);
                    //UnityEngine.Debug.Log(json);
                }
                catch (FormatException fe)
                {
                    UnityEngine.Debug.LogWarning($"[SceneLoader] Bad Base64 in !LoadMap payload: {fe.Message}");
                    return true;
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogWarning($"[SceneLoader] Decode failed: {e}");
                    return true;
                }

                SceneInformation si;
                try
                {
                    si = JsonConvert.DeserializeObject<SceneInformation>(json);
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogWarning($"[SceneLoader] JSON parse failed: {e}");
                    return true;
                }

                // optional: store it globally
                sceneInformation = si;

                if(RinkOnlyPruner.SceneIsLoaded && !PracticeModeDetector.IsPracticeMode)
{
                    UnityEngine.Debug.Log("[SceneLoader] LoadMap directive received; swapping scene.");
                    RinkSceneLoader.LoadSceneAsync(RinkOnlyPruner.scene, si);
                    return false; // swallow chat
                }

                return false; // let chat render if we didn't act
            }

            static bool IsLoadMapCommand(string s)
            {
                // Accept "!motd" or "/motd" (case-insensitive), optionally with trailing spaces
                if (s.ToUpper().Contains("!LOADMAP"))
                {
                    return true;
                }
                else { return false; }
            }
        }
    }
}