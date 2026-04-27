using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MOTD.Behaviors;
using MOTD.Models;
using TestProject.Utilities;
using UnityEngine;
using MOTD.Singletons;
using TestProject.Singletons;
using demsMOTD.MOTD;
using Unity.Netcode;
namespace MOTD.Patches
{
    [HarmonyPatch]
    static class UIChat_WelcomePatch
    {
        public static string MOTDCommand = "!MOTD ";

        // PuckNew: UIChatController.Event_Server_OnSynchronizeComplete no longer exists.
        // The equivalent server hook is SceneManager.Server_OnClientSceneSynchronizeComplete.
        // Chat delivery now goes through ChatManager.Server_SendChatMessageToClients.
        [HarmonyPostfix]
        [HarmonyPatch(typeof(global::SceneManager), "Server_OnClientSceneSynchronizeComplete")]
        static void SyncComplete_Postfix(ulong clientId)
        {
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
                return;

            try
            {
                ConfigData.Load();
                if (string.IsNullOrEmpty(ConfigData.Instance.JsonFileLocation)) return;

                string json = File.ReadAllText(ConfigData.Instance.JsonFileLocation);
                string error = "";
                MOTDSettings doc;
                ModalDocIO.TryLoad(json, out doc, out error);

                if (error != null || doc == null)
                {
                    Debug.Log($"[MOTD] Could not load MOTD doc: {error}");
                    return;
                }

                Debug.Log("[MOTD] Sending MOTD to client " + clientId);
                SendChatToClient(MOTDCommand + json, clientId);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[MOTD] SyncComplete_Postfix error: {e}");
            }
        }

        // PuckNew: ChatManager replaces UIChat.Server_SendSystemChatMessage.
        // Resolved via reflection so this compiles against old libs without ChatManager.
        static void SendChatToClient(string message, ulong clientId)
        {
            var chatManagerType = AccessTools.TypeByName("ChatManager");
            if (chatManagerType == null) { Debug.LogWarning("[MOTD] ChatManager not found"); return; }
            var singletonType = typeof(NetworkBehaviourSingleton<>).MakeGenericType(chatManagerType);
            var instance = AccessTools.PropertyGetter(singletonType, "Instance")?.Invoke(null, null);
            if (instance == null) { Debug.LogWarning("[MOTD] ChatManager.Instance is null"); return; }
            AccessTools.Method(chatManagerType, "Server_SendChatMessageToClients")
                ?.Invoke(instance, new object[] { message, new ulong[] { clientId } });
        }

        // PuckNew: UIChat.AddChatMessage now takes (ChatMessage, Units, bool) instead of (string).
        // TargetMethod resolves the overload at runtime so this compiles against old libs too.
        [HarmonyPatch]
        public static class UIChat_AddChatMessage_MotdPatch
        {
            static MethodBase TargetMethod() =>
                typeof(UIChat).GetMethods()
                    .FirstOrDefault(m => m.Name == "AddChatMessage" && m.GetParameters().Length == 3);

            [HarmonyPrefix]
            static bool Prefix(UIChat __instance, ChatMessage chatMessage)
            {
                var nm = __instance?.NetworkManager;
                if (nm != null && !nm.IsClient) return true;

                if (chatMessage == null) return true;

                var text = GetMessageText(chatMessage);
                if (!IsMotdCommand(text)) return true;

                string error = "";
                MOTDSettings doc;
                ModalDocIO.TryLoad(text, out doc, out error);
                if (error == null && doc != null)
                {
                    SimpleModal.ApplyTheme(ThemeMapper.ToTheme(doc.Theme, SimpleModal.DarkDefault));
                    SimpleModal.Show(
                        title: doc.ModalDoc.title,
                        richText: ModalDocIO.MdToUnity(doc.ModalDoc.richText),
                        dontShowKey: "",
                        bannerUrl: doc.ModalDoc.bannerImageUrl,
                        panelBgUrl: doc.ModalDoc.panelImageUrl,
                        height: doc.ModalDoc.panelHeightPercent,
                        width: doc.ModalDoc.panelWidthPercent,
                        theme: doc.Theme,
                        doc: doc.ModalDoc);
                }
                else
                {
                    Debug.Log(error);
                }
                return false; // swallow raw !MOTD message from chat display
            }

            static bool IsMotdCommand(string s) => s.ToUpper().Contains("!MOTD");
        }

        // PuckNew: Content (FixedString); old Puck: Message (string). Resolved at runtime.
        static string GetMessageText(ChatMessage chatMessage)
        {
            var v = Traverse.Create(chatMessage).Property("Content").GetValue<object>()
                 ?? Traverse.Create(chatMessage).Property("Message").GetValue<object>();
            return (v?.ToString() ?? string.Empty).Trim();
        }
    }
}
