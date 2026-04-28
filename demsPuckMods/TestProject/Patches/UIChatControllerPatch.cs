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
                SendMotdToClient(json, clientId);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[MOTD] SyncComplete_Postfix error: {e}");
            }
        }

        static void SendMotdToClient(string json, ulong clientId)
        {
            var chatManagerType = AccessTools.TypeByName("ChatManager");
            if (chatManagerType == null) { Debug.LogWarning("[MOTD] ChatManager not found"); return; }
            var instance = AccessTools.PropertyGetter(chatManagerType.BaseType, "Instance")?.Invoke(null, null);
            if (instance == null) { Debug.LogWarning("[MOTD] ChatManager.Instance is null"); return; }
            var sendMethod = AccessTools.Method(chatManagerType, "Server_SendChatMessageToClients", new Type[] { typeof(string), typeof(ulong[]) });
            if (sendMethod == null) { Debug.LogWarning("[MOTD] Server_SendChatMessageToClients not found"); return; }

            var targets = new ulong[] { clientId };
            string fullMessage = MOTDCommand + json;

            if (Encoding.UTF8.GetByteCount(fullMessage) <= MaxPayloadBytes)
            {
                sendMethod.Invoke(instance, new object[] { fullMessage, targets });
                return;
            }

            // Message too large for single FixedString512Bytes — split into chunks
            var chunks = ChunkString(json, MaxPayloadBytes - 16); // reserve bytes for "!MOTDC:XX:XX "
            Debug.Log($"[MOTD] Sending {chunks.Count} chunks to client {clientId}");
            for (int i = 0; i < chunks.Count; i++)
            {
                string msg = $"{ChunkPrefix}{i + 1}:{chunks.Count} {chunks[i]}";
                sendMethod.Invoke(instance, new object[] { msg, targets });
            }
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
                    Debug.Log($"[MOTD] Failed to parse MOTD: {error}");
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
