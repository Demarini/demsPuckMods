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

        // Cached !LoadMap payload, keyed on the SceneInformation reference. ConfigData.Load()
        // builds a new ConfigData (and therefore a new sceneInformation reference) each time,
        // so reference inequality is a reliable invalidation signal across scenery reloads.
        static SceneInformation _cachedSi;
        static string _cachedPayload;

        // Cached reflection for SendChatToClient — these were looked up per-call, hit on every
        // client join via SyncComplete_Postfix.
        static Type _chatManagerType;
        static MethodInfo _chatManagerInstanceGetter;
        static MethodInfo _sendChatMethod;
        static object _chatManagerInstance;

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
                // ConfigData.Instance lazy-loads once; do NOT call Load() here — it does
                // synchronous disk I/O + JSON parse on the main thread per join, and the
                // server-side scenery only changes on scene reload (which calls Load itself).
                var si = ConfigData.Instance.sceneInformation;
                if (si == null) return;

                if (!ReferenceEquals(si, _cachedSi))
                {
                    _cachedSi = si;
                    _cachedPayload = "!LoadMap " + MessageObfuscation.Encode(JsonConvert.SerializeObject(si));
                }

                SendChatToClient(_cachedPayload, clientId);
                SendChatToClient("Type /sl help for scenery loader commands.", clientId);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[SceneryChanger] SyncComplete_Postfix error: {e}");
            }
        }

        internal static void SendChatToClient(string message, ulong clientId)
        {
            if (_chatManagerType == null)
            {
                _chatManagerType = AccessTools.TypeByName("ChatManager");
                if (_chatManagerType == null) { Debug.LogWarning("[SceneryChanger] ChatManager not found"); return; }
                _chatManagerInstanceGetter = AccessTools.PropertyGetter(_chatManagerType.BaseType, "Instance");
                _sendChatMethod = AccessTools.Method(_chatManagerType, "Server_SendChatMessageToClients", new Type[] { typeof(string), typeof(ulong[]) });
            }

            if (_chatManagerInstance == null)
                _chatManagerInstance = _chatManagerInstanceGetter?.Invoke(null, null);
            if (_chatManagerInstance == null) { Debug.LogWarning("[SceneryChanger] ChatManager.Instance is null"); return; }

            _sendChatMethod?.Invoke(_chatManagerInstance, new object[] { message, new ulong[] { clientId } });
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
