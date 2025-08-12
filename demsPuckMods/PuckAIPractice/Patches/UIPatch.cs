using HarmonyLib;
using PuckAIPractice.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace PuckAIPractice.Patches
{
    [HarmonyPatch(typeof(UIScoreboard), nameof(UIScoreboard.AddPlayer))]
    public static class ScoreboardUI_AddPlayer_FilterPatch
    {
        // Return false => skip original AddPlayer
        static bool Prefix(UIScoreboard __instance, Player player)
        {
            // Optional: still allow the original early return for nulls
            //if (player == null) return false;
            //if (FakePlayerRegistry.All.Contains(player) || player.OwnerClientId == 7777778 || player.OwnerClientId == 7777777)
            //{
            //    Debug.Log("Detected Bot Don't Add to Scoreboard.");
            //    return false;
            //}
            //else
            //{
            //    Debug.Log($"Adding {player.OwnerClientId} to the UI.");
            //}
                

            // Not excluded — let the original AddPlayer run
            return true;
        }
    }
    //[HarmonyPatch(typeof(UIScoreboard), "UpdatePlayer")]
    //public static class UIScoreboard_UpdatePlayer_Patch
    //{
    //    public static void Postfix(UIScoreboard __instance, Player player)
    //    {
    //        if (__instance == null || player == null)
    //            return;

    //        // Grab the visual element associated with this player
    //        VisualElement visualElement;
    //        if (!Traverse.Create(__instance)
    //                     .Field("playerVisualElementMap")
    //                     .GetValue<Dictionary<Player, VisualElement>>()
    //                     .TryGetValue(player, out visualElement))
    //            return;

    //        // Rebuild label text with our own admin prefix logic
    //        Label label6 = visualElement.Query<Label>("PositionLabel");
    //        Label usernameLabel = visualElement.Query<Label>("Username");
    //        if (FakePlayerRegistry.All.Contains(player))
    //        {
    //            label6.text = "G";
    //            usernameLabel.text = string.Format("{0}<noparse>#{1} {2}</noparse>", "<b><color=#992d22>BOT</color></b>", player.Number.Value, player.Username.Value);
    //        }
            
    //    }
    //    static List<string> donorList = new List<string>() { "76561197994406332" };
    //    private static string GetCustomPrefix(Player player)
    //    {

    //        if (donorList.Contains(player.SteamId.Value.ToString()))
    //        {
    //            return $"<b><color=#00aaff>DEM :o</color></b>";
    //        }
    //        else
    //        {
    //            return $"<b><color=#00aaff>[demBot3000]</color></b>";
    //        }
    //    }
    //}
}