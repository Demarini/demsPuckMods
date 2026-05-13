using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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
        const string ChunkPrefix = "!MOTDC:";
        const int MaxPayloadBytes = 450;

        // Per-join the original re-read MOTD JSON from disk + parsed it + chunked it. With many
        // joiners this stalled the server tick; cache the prebuilt outgoing messages keyed on the
        // file's last-write-time so admins can still edit MOTD.json live and have it picked up.
        static string _cachedJsonPath;
        static DateTime _cachedJsonMtime;
        static string[] _cachedMessages;

        // Reflection lookups are stable for the lifetime of the server; cache the type + method
        // info and the ChatManager singleton to avoid the AccessTools work per join.
        static Type _chatManagerType;
        static MethodInfo _chatManagerInstanceGetter;
        static MethodInfo _sendChatMethod;
        static object _chatManagerInstance;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(global::SceneManager), "Server_OnClientSceneSynchronizeComplete")]
        static void SyncComplete_Postfix(ulong clientId)
        {
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
                return;

            try
            {
                // ConfigData.Instance lazy-loads once; do NOT call Load() per join — it does
                // synchronous disk I/O + JSON parse on the main thread.
                var path = ConfigData.Instance.JsonFileLocation;
                if (string.IsNullOrEmpty(path)) return;

                var messages = GetCachedMessages(path);
                if (messages == null) return;

                EnsureChatReflection();
                if (_sendChatMethod == null) return;

                if (_chatManagerInstance == null)
                    _chatManagerInstance = _chatManagerInstanceGetter?.Invoke(null, null);
                if (_chatManagerInstance == null) { Debug.LogWarning("[MOTD] ChatManager.Instance is null"); return; }

                var targets = new ulong[] { clientId };
                for (int i = 0; i < messages.Length; i++)
                    _sendChatMethod.Invoke(_chatManagerInstance, new object[] { messages[i], targets });
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[MOTD] SyncComplete_Postfix error: {e}");
            }
        }

        // Returns the cached chunked MOTD messages, rebuilding only if the source file changed
        // since last call. Returns null if the file is missing or the doc fails to parse.
        static string[] GetCachedMessages(string path)
        {
            if (!File.Exists(path)) return null;
            var mtime = File.GetLastWriteTimeUtc(path);

            if (_cachedMessages != null && _cachedJsonPath == path && _cachedJsonMtime == mtime)
                return _cachedMessages;

            string json = File.ReadAllText(path);

            string error = "";
            MOTDSettings doc;
            ModalDocIO.TryLoad(json, out doc, out error);
            if (doc == null)
            {
                Debug.Log($"[MOTD] Could not load MOTD doc: {error}");
                _cachedMessages = null;
                _cachedJsonPath = path;
                _cachedJsonMtime = mtime;
                return null;
            }

            string fullMessage = MOTDCommand + json;
            string[] messages;
            if (Encoding.UTF8.GetByteCount(fullMessage) <= MaxPayloadBytes)
            {
                messages = new[] { fullMessage };
            }
            else
            {
                var chunks = ChunkString(json, MaxPayloadBytes - 16); // reserve bytes for "!MOTDC:XX:XX "
                messages = new string[chunks.Count];
                for (int i = 0; i < chunks.Count; i++)
                    messages[i] = $"{ChunkPrefix}{i + 1}:{chunks.Count} {chunks[i]}";
                Debug.Log($"[MOTD] Cached {chunks.Count} MOTD chunks for {path}");
            }

            _cachedMessages = messages;
            _cachedJsonPath = path;
            _cachedJsonMtime = mtime;
            return messages;
        }

        static void EnsureChatReflection()
        {
            if (_chatManagerType != null) return;
            _chatManagerType = AccessTools.TypeByName("ChatManager");
            if (_chatManagerType == null) { Debug.LogWarning("[MOTD] ChatManager not found"); return; }
            _chatManagerInstanceGetter = AccessTools.PropertyGetter(_chatManagerType.BaseType, "Instance");
            _sendChatMethod = AccessTools.Method(_chatManagerType, "Server_SendChatMessageToClients", new Type[] { typeof(string), typeof(ulong[]) });
            if (_sendChatMethod == null) Debug.LogWarning("[MOTD] Server_SendChatMessageToClients not found");
        }

        static List<string> ChunkString(string s, int maxBytesPerChunk)
        {
            var chunks = new List<string>();
            int start = 0;
            while (start < s.Length)
            {
                int len = Math.Min(s.Length - start, maxBytesPerChunk);
                while (len > 0 && Encoding.UTF8.GetByteCount(s, start, len) > maxBytesPerChunk)
                    len--;
                chunks.Add(s.Substring(start, len));
                start += len;
            }
            return chunks;
        }

        [HarmonyPatch]
        public static class UIChat_AddChatMessage_MotdPatch
        {
            static string _chunkBuffer = "";
            static int _expectedChunks = 0;
            static int _receivedChunks = 0;

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

                // Handle chunked MOTD messages
                if (text.StartsWith(ChunkPrefix))
                {
                    HandleChunk(text);
                    return false;
                }

                // Handle single (small) MOTD messages
                if (!text.StartsWith(MOTDCommand, StringComparison.OrdinalIgnoreCase)) return true;

                string json = text.Substring(MOTDCommand.Length);
                ProcessMotdJson(json);
                return false;
            }

            static void HandleChunk(string text)
            {
                try
                {
                    // Parse "!MOTDC:1:3 <data>"
                    int afterPrefix = ChunkPrefix.Length;
                    int secondColon = text.IndexOf(':', afterPrefix);
                    int space = text.IndexOf(' ', secondColon);
                    if (secondColon < 0 || space < 0) return;

                    int chunkNum = int.Parse(text.Substring(afterPrefix, secondColon - afterPrefix));
                    int totalChunks = int.Parse(text.Substring(secondColon + 1, space - secondColon - 1));
                    string data = text.Substring(space + 1);

                    if (chunkNum == 1)
                    {
                        _chunkBuffer = "";
                        _expectedChunks = totalChunks;
                        _receivedChunks = 0;
                    }

                    _chunkBuffer += data;
                    _receivedChunks++;

                    Debug.Log($"[MOTD] Received chunk {chunkNum}/{totalChunks}");

                    if (_receivedChunks >= _expectedChunks)
                    {
                        Debug.Log($"[MOTD] All {_expectedChunks} chunks received, processing MOTD");
                        ProcessMotdJson(_chunkBuffer);
                        _chunkBuffer = "";
                        _expectedChunks = 0;
                        _receivedChunks = 0;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[MOTD] HandleChunk error: {e}");
                }
            }

            static void ProcessMotdJson(string json)
            {
                string error = "";
                MOTDSettings doc;
                ModalDocIO.TryLoad(json, out doc, out error);
                if (error == null && doc != null)
                {
                    SimpleModal.ApplyTheme(ThemeMapper.ToTheme(doc.Theme, SimpleModal.DarkDefault));
                    SimpleModal.Show(
                        title: doc.ModalDoc.title,
                        richText: ModalDocIO.MdToUnity(doc.ModalDoc.richText),
                        dontShowKey: BuildDismissKey(),
                        bannerUrl: doc.ModalDoc.bannerImageUrl,
                        panelBgUrl: doc.ModalDoc.panelImageUrl,
                        height: doc.ModalDoc.panelHeightPercent,
                        width: doc.ModalDoc.panelWidthPercent,
                        theme: doc.Theme,
                        doc: doc.ModalDoc,
                        version: doc.Version);
                }
                else
                {
                    Debug.Log($"[MOTD] Failed to parse MOTD: {error}");
                }
            }

            // Per-server dismissal key. Reads ConnectionManager.UnityTransport.ConnectionData
            // via reflection so we don't bind to a specific transport assembly. Falls back to
            // the generic "MOTD" key if anything is unavailable, preserving original behavior.
            static string BuildDismissKey()
            {
                try
                {
                    var connMgrType = AccessTools.TypeByName("ConnectionManager");
                    if (connMgrType == null) return "MOTD";
                    var instance = AccessTools.PropertyGetter(connMgrType.BaseType, "Instance")?.Invoke(null, null);
                    if (instance == null) return "MOTD";

                    var transport = Traverse.Create(instance).Field("UnityTransport").GetValue();
                    if (transport == null) return "MOTD";

                    var connData = Traverse.Create(transport).Property("ConnectionData").GetValue();
                    if (connData == null) return "MOTD";

                    string address = Traverse.Create(connData).Field("Address").GetValue<string>();
                    object portObj = Traverse.Create(connData).Field("Port").GetValue();
                    if (string.IsNullOrEmpty(address) || portObj == null) return "MOTD";

                    return $"MOTD@{address}:{portObj}";
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[MOTD] BuildDismissKey fallback to generic: {e.Message}");
                    return "MOTD";
                }
            }

            static bool IsMotdCommand(string s) => s.ToUpper().Contains("!MOTD");
        }

        static string GetMessageText(ChatMessage chatMessage)
        {
            var v = Traverse.Create(chatMessage).Field("Content").GetValue<object>()
                 ?? Traverse.Create(chatMessage).Field("Message").GetValue<object>();
            return (v?.ToString() ?? string.Empty).Trim();
        }
    }
}
