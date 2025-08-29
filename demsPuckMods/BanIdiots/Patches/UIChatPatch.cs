using DG.Tweening.Plugins.Core.PathCore;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BanIdiots.Patches
{
    [HarmonyPatch(typeof(UIChat), "Server_ProcessPlayerChatMessage")]
    public static class UIChat_Server_ProcessPlayerChatMessage_Prefix
    {
        // You can mutate 'message' and/or return false to skip the original.
        static bool Prefix(UIChat __instance, Player player, ref string message, ulong clientId, bool useTeamChat, bool isMuted)
        {
            try
            {
                // 0) Sanity
                if (player == null || string.IsNullOrEmpty(message))
                    return true; // let original handle its own null checks

                // 1) Example: add your own command(s) and consume them
                //    /motd   -> server-only handling (don’t let the base method run)
                if (message.StartsWith("/motd", StringComparison.OrdinalIgnoreCase))
                {
                    // Do whatever your mod needs (trigger your own event, queue an RPC your mod owns, etc.)
                    UnityEngine.Debug.Log($"[YourMod] {player.Username?.Value} requested MOTD");
                    return false; // swallow: don't run the game's handler
                }

                // 2) Example: rewrite message text before the game processes it
                // Trim, normalize whitespace, etc.
                message = message.Trim();
                ConfigData.Load();
                if(ConfigData.Instance.bannedWordsTextLocation != "")
                {
                    try
                    {
                        List<string> bannedWords = File.ReadAllLines(ConfigData.Instance.bannedWordsTextLocation).ToList();
                        foreach (string bannedWord in bannedWords)
                        {
                            if (message.Contains(bannedWord))
                            {
                                UIChat.Instance.Server_SendSystemChatMessage(string.Format("<b><color=orange>ADMIN</color></b> banned {0}.", player.Username.Value));
                                string originalMessage = message;
                                message = $"I'm the dumbest person alive and got banned for being racist.";
                                string logMessage = $"[{DateTime.Now.ToString("MM:dd:yyyy HH:mm:ss")}] Player {player.Username.Value} with Steam ID {player.SteamId.Value.ToString()} banned.\nMessage Sent: {originalMessage}";
                                ServerManager.Instance.Server_BanPlayer(player);
                                Debug.Log(logMessage);
                                using (StreamWriter sw = File.AppendText(ConfigData.Instance.logOutputLocation))
                                {
                                    sw.WriteLine(logMessage);
                                }
                            }
                        }
                    }
                    catch
                    {

                    }
                }
                
                // Example: block messages that are only punctuation
                
                if(message.ToUpper() == "TEST")
                {
                    
                    //
                }
                

                // 3) Example: soft-filter (replace banned words, etc.)
                // message = SoftFilter(message);

                // 4) Example: add a prefix for admins (purely demonstrative)
                // if (YourAdminCheck(player)) message = "[ADMIN] " + message;

                // Fall through: run original method with (possibly) modified message
                return true;
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"[YourMod] Chat Prefix failed: {ex}");
                // Fail open so you don't break chat if your mod throws
                return true;
            }
        }

        private static bool AllPunctuation(string s)
        {
            foreach (var ch in s)
                if (!char.IsPunctuation(ch) && !char.IsWhiteSpace(ch))
                    return false;
            return true;
        }
    }
}
