using HarmonyLib;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using MOTD.Behaviors;
using MOTD.Config;
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
        // Protocol: each non-final MOTD chunk goes out as "!MOTD <data>"; the final (or
        // only) chunk goes out as "!ENDMOTD <data>". Receivers append "!MOTD" chunks to a
        // buffer and process+clear on "!ENDMOTD". This keeps the stream identifiable even
        // when other mods' chat messages interleave between our chunks.
        public const string MotdPrefix = "!MOTD ";
        public const string EndMotdPrefix = "!ENDMOTD ";

        // Server caps chat at 512 bytes per message; stay well under that to allow for
        // any wrapping the chat pipeline adds and any UTF-8 multi-byte payload chars.
        const int MaxMessageBytes = 450;

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
                var path = MOTDConfig.ResolveMotdJsonPath();
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

        // Returns the cached MOTD messages, rebuilding only if the source file changed
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

            // Minify before chunking. Pretty-printed JSON contains \n/\r which trips
            // any server-side chat patch that splits multi-line system messages by
            // newline (e.g. the nametag mod's ChatManagerSendSystemStringToClientsPatch),
            // shredding each chunk into many partial messages.
            string compactJson;
            try { compactJson = JObject.Parse(json).ToString(Newtonsoft.Json.Formatting.None); }
            catch (Exception ex)
            {
                Debug.LogWarning($"[MOTD] Failed to minify JSON, falling back to raw: {ex.Message}");
                compactJson = json;
            }

            string[] messages = BuildMessages(compactJson);
            Debug.Log($"[MOTD] Cached {messages.Length} MOTD message(s) for {path}");

            _cachedMessages = messages;
            _cachedJsonPath = path;
            _cachedJsonMtime = mtime;
            return messages;
        }

        // Splits json into one or more wire messages using the !MOTD / !ENDMOTD scheme.
        // Single-fit MOTDs ship as one "!ENDMOTD <json>" message — receivers treat that
        // as both first and last, processing immediately.
        static string[] BuildMessages(string json)
        {
            string single = EndMotdPrefix + json;
            if (Encoding.UTF8.GetByteCount(single) <= MaxMessageBytes)
                return new[] { single };

            int endPrefixBytes = Encoding.UTF8.GetByteCount(EndMotdPrefix);
            int dataBudget = MaxMessageBytes - endPrefixBytes;

            var chunks = ChunkString(json, dataBudget);
            var messages = new string[chunks.Count];
            for (int i = 0; i < chunks.Count - 1; i++)
                messages[i] = MotdPrefix + chunks[i];
            messages[chunks.Count - 1] = EndMotdPrefix + chunks[chunks.Count - 1];
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
            static readonly StringBuilder _buffer = new StringBuilder();

            static MethodBase TargetMethod()
            {
                var type = AccessTools.TypeByName("UIChat");
                if (type == null) return null;
                return type.GetMethods()
                    .FirstOrDefault(m => m.Name == "AddChatMessage" && m.GetParameters().Length == 3);
            }

            // Priority.First ensures we observe the raw ChatMessage text before any other
            // chat-patching mod (e.g. nametag/colorizer mods) can mutate chatMessage.Content
            // and prepend wrappers like "<color=...>Server</color>: " that would break our
            // exact-prefix check.
            [HarmonyPrefix]
            [HarmonyPriority(Priority.First)]
            static bool Prefix(UIChat __instance, ChatMessage chatMessage)
            {
                if (chatMessage == null) return true;

                var text = GetMessageText(chatMessage);

                // Final (or only) chunk — append and process.
                if (text.StartsWith(EndMotdPrefix, StringComparison.Ordinal))
                {
                    _buffer.Append(text.Substring(EndMotdPrefix.Length));
                    string json = _buffer.ToString();
                    _buffer.Length = 0;
                    ProcessMotdJson(json);
                    return false;
                }

                // Intermediate chunk — append and swallow.
                if (text.StartsWith(MotdPrefix, StringComparison.Ordinal))
                {
                    _buffer.Append(text.Substring(MotdPrefix.Length));
                    return false;
                }

                return true;
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
        }

        static string GetMessageText(ChatMessage chatMessage)
        {
            var v = Traverse.Create(chatMessage).Field("Content").GetValue<object>()
                 ?? Traverse.Create(chatMessage).Field("Message").GetValue<object>();
            return (v?.ToString() ?? string.Empty).Trim();
        }
    }
}
