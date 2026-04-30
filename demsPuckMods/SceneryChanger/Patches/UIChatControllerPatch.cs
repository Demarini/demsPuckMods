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

        static MethodBase TargetMethod()
        {
            // Use reflection to find the game's SceneManager (global namespace),
            // not Unity's UnityEngine.SceneManagement.SceneManager.
            var type = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => { try { return a.GetTypes(); } catch { return Type.EmptyTypes; } })
                .FirstOrDefault(t => t.Name == "SceneManager" && (t.Namespace == null || t.Namespace == ""));
            return type == null ? null : AccessTools.Method(type, "Server_OnClientSceneSynchronizeComplete");
        }

        [HarmonyPostfix]
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
                SendChatToClient("Type /sl help for scenery loader commands.", clientId);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[SceneryChanger] SyncComplete_Postfix error: {e}");
            }
        }

        internal static void SendChatToClient(string message, ulong clientId)
        {
            var chatManagerType = AccessTools.TypeByName("ChatManager");
            if (chatManagerType == null) { Debug.LogWarning("[SceneryChanger] ChatManager not found"); return; }
            var instance = AccessTools.PropertyGetter(chatManagerType.BaseType, "Instance")?.Invoke(null, null);
            if (instance == null) { Debug.LogWarning("[SceneryChanger] ChatManager.Instance is null"); return; }
            AccessTools.Method(chatManagerType, "Server_SendChatMessageToClients", new Type[] { typeof(string), typeof(ulong[]) })
                ?.Invoke(instance, new object[] { message, new ulong[] { clientId } });
        }

        [HarmonyPatch]
        public static class UIChat_AddChatMessage_LoadMapPatch
        {
            static MethodBase TargetMethod()
            {
                var type = AccessTools.TypeByName("UIChat");
                if (type == null) return null;
                return type.GetMethods()
                    .FirstOrDefault(m => m.Name == "AddChatMessage" && m.GetParameters().Length == 3);
            }

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

                return false;
            }
        }

        static string GetMessageText(ChatMessage chatMessage)
        {
            var v = Traverse.Create(chatMessage).Field("Content").GetValue<object>()
                 ?? Traverse.Create(chatMessage).Field("Message").GetValue<object>();
            return (v?.ToString() ?? string.Empty).Trim();
        }
    }
}
