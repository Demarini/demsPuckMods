using HarmonyLib;
using Newtonsoft.Json;
using SceneryChanger.Model;
using SceneryChanger.Services;
using SceneryLoader.Behaviors;
using SceneryLoader.Services;
using SceneryLoader.Singletons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Netcode;
using UnityEngine;

namespace SceneryChanger.Patches
{
    [HarmonyPatch]
    public static class UIChatControllerPatch
    {
        public static SceneInformation sceneInformation = null;

        // PuckNew: the old UIChatController.Event_Server_OnSynchronizeComplete no longer exists.
        // The equivalent hook is SceneManager.Server_OnClientSceneSynchronizeComplete (private static).
        // Chat delivery now goes through ChatManager, not UIChat.Server_SendSystemChatMessage.
        [HarmonyPostfix]
        [HarmonyPatch(typeof(global::SceneManager), "Server_OnClientSceneSynchronizeComplete")]
        static void SyncComplete_Postfix(ulong clientId)
        {
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
                return;

            try
            {
                ConfigData.Load();
                var si = ConfigData.Instance.sceneInformation;
                if (si == null) return;

                string payload = "!LoadMap " + MessageObfuscation.Encode(JsonConvert.SerializeObject(si));
                SendChatToClient(payload, clientId);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[SceneryChanger] SyncComplete_Postfix error: {e}");
            }
        }

        // PuckNew: ChatManager replaces UIChat.Server_SendSystemChatMessage.
        // Resolved via reflection so this compiles against old libs without ChatManager.
        static void SendChatToClient(string message, ulong clientId)
        {
            var chatManagerType = AccessTools.TypeByName("ChatManager");
            if (chatManagerType == null) { Debug.LogWarning("[SceneryChanger] ChatManager not found"); return; }
            var singletonType = typeof(NetworkBehaviourSingleton<>).MakeGenericType(chatManagerType);
            var instance = AccessTools.PropertyGetter(singletonType, "Instance")?.Invoke(null, null);
            if (instance == null) { Debug.LogWarning("[SceneryChanger] ChatManager.Instance is null"); return; }
            AccessTools.Method(chatManagerType, "Server_SendChatMessageToClients")
                ?.Invoke(instance, new object[] { message, new ulong[] { clientId } });
        }

        // PuckNew: UIChat.AddChatMessage now takes (ChatMessage, Units, bool) instead of (string).
        // TargetMethod resolves the overload at runtime so this compiles against old libs too.
        [HarmonyPatch]
        public static class UIChat_AddChatMessage_LoadMapPatch
        {
            static MethodBase TargetMethod() =>
                typeof(UIChat).GetMethods()
                    .FirstOrDefault(m => m.Name == "AddChatMessage" && m.GetParameters().Length == 3);

            [HarmonyPrefix]
            static bool Prefix(UIChat __instance, ChatMessage chatMessage)
            {
                if (chatMessage == null) return true;

                var text = GetMessageText(chatMessage);
                const string cmd = "!LoadMap ";

                if (!text.StartsWith(cmd)) return true;

                var encoded = text.Substring(cmd.Length).Trim();
                if (encoded.IndexOf(' ') >= 0 && encoded.IndexOf('+') < 0)
                    encoded = encoded.Replace(' ', '+');

                string json;
                try
                {
                    json = MessageObfuscation.Decode(encoded);
                }
                catch (FormatException fe)
                {
                    Debug.LogWarning($"[SceneLoader] Bad Base64 in !LoadMap payload: {fe.Message}");
                    return true;
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[SceneLoader] Decode failed: {e}");
                    return true;
                }

                SceneInformation si;
                try
                {
                    si = JsonConvert.DeserializeObject<SceneInformation>(json);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[SceneLoader] JSON parse failed: {e}");
                    return true;
                }

                sceneInformation = si;

                if (!PracticeModeDetector.IsPracticeMode)
                {
                    Debug.Log("[SceneLoader] LoadMap directive received; swapping scene.");
                    SceneLoadCoordinator.OnServerSceneDirective(si);
                }

                return false; // swallow the raw !LoadMap message from chat
            }
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
