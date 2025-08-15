using HarmonyLib;
using PuckAIPractice.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace PuckAIPractice.Patches
{
    public static class PlayerPatch
    {
        //    [HarmonyPatch(typeof(Player), "Server_RespawnCharacter")]
        //    public class Server_RespawnCharacter_Prefix
        //    {
        //        public static bool Prefix(Player __instance, Vector3 position, Quaternion rotation, PlayerRole role)
        //        {
        //            if (!PracticeModeDetector.IsPracticeMode && !NetworkManager.Singleton.IsServer)
        //            {
        //                return true;
        //            }
        //            // Custom logic before the original method runs
        //            //Debug.Log($"[Harmony Prefix] Respawning character for {__instance.Username.Value} at position {position} with role {role}");
        //            List<Player> players = PlayerManager.Instance.GetPlayers(false);
        //            bool hasBlueGoalie = false;
        //            bool hasRedGoalie = false;
        //            bool hasRedBot = false;
        //            bool hasBlueBot = false;
        //            Player blueBot = null;
        //            Player redBot = null;
        //            List<Player> bots = FakePlayerRegistry.All.ToList();
        //            List<Player> existingBots = FakePlayerRegistry.AllExisting.ToList();
        //            //Debug.Log($"Player Count: {players.Count()}");
        //            foreach (Player b in bots)
        //            {
        //                if (b.Team.Value == PlayerTeam.Blue)
        //                {
        //                    //Debug.Log("Has Blue Bot: " + b.Username.Value);
        //                    hasBlueBot = true;
        //                    blueBot = b;
        //                }
        //                else
        //                {
        //                    //Debug.Log("Has Red Bot: " + b.Username.Value);
        //                    hasRedBot = true;
        //                    redBot = b;
        //                }
        //            }
        //            foreach (Player p in players)
        //            {

        //                if (existingBots.Contains(p)) continue;
        //                if (p.Role.Value == PlayerRole.Goalie)
        //                {
        //                    if (p.Team.Value == PlayerTeam.Red)
        //                    {
        //                        //Debug.Log("Has Red Goalie: " + p.Username.Value);
        //                        hasRedGoalie = true;
        //                    }
        //                    else
        //                    {
        //                        //Debug.Log("Has Blue Goalie: " + p.Username.Value);
        //                        hasBlueGoalie = true;
        //                    }
        //                }
        //            }
        //            if (hasBlueGoalie)
        //            {
        //                if (hasBlueBot)
        //                {
        //                    //Debug.Log("Despawning Blue Bot");
        //                    //BotSpawning.Despawn(blueBot);
        //                }
        //            }
        //            else
        //            {
        //                if (!hasBlueBot)
        //                {
        //                    //Debug.Log("Spawning Blue Bot");
        //                    //BotSpawning.SpawnFakePlayer(0, PlayerRole.Goalie, PlayerTeam.Blue);
        //                }
        //            }
        //            if (hasRedGoalie)
        //            {
        //                if (hasRedBot)
        //                {
        //                    //BotSpawning.Despawn(redBot);
        //                }
        //            }
        //            else
        //            {
        //                if (!hasRedBot)
        //                {
        //                    //BotSpawning.SpawnFakePlayer(1, PlayerRole.Goalie, PlayerTeam.Red);
        //                }
        //            }
        //            // Optionally, you can prevent the original method from running by returning false
        //            // return false;

        //            // Return true to allow the original method to execute after the prefix
        //            return true;
        //        }
        //    }
        //}
        //[HarmonyPatch(typeof(PlayerBodyV2), nameof(PlayerBodyV2.UpdateMesh))]
        //public static class PlayerBodyV2_UpdateMesh_Patch
        //{
        //    // Return false to skip the original UpdateMesh entirely
        //    static bool Prefix(PlayerBodyV2 __instance)
        //    {
        //        if (!PracticeModeDetector.IsPracticeMode && !NetworkManager.Singleton.IsServer)
        //        {
        //            return true;
        //        }
        //        var player = __instance.Player;
        //        if (player == null)
        //            return false;

        //        var instTr = Traverse.Create(__instance);

        //        // Get the private field: playerMesh
        //        var playerMeshObj = instTr.Field("playerMesh").GetValue();
        //        if (playerMeshObj == null)
        //        {
        //            //Debug.LogWarning("[Patch] PlayerBodyV2.UpdateMesh: playerMesh was null");
        //            return false;
        //        }

        //        var pmTr = Traverse.Create(playerMeshObj);

        //        // Mirror original behavior via reflection-safe calls
        //        // These Method(...) calls infer overloads from the argument objects you pass.
        //        //Debug.Log("Player: " + player.Username.Value.ToString());
        //        //Debug.Log("Player Team: " + player.Team.Value.ToString());
        //        pmTr.Method("SetUsername", new object[] { player.Username.Value.ToString() }).GetValue();
        //        pmTr.Method("SetNumber", new object[] { player.Number.Value.ToString() }).GetValue();
        //        //Debug.Log("Set Jersey");
        //        //Debug.Log(player.GetPlayerJerseySkin());
        //        pmTr.Method("SetJersey", new object[] { player.Team.Value, player.GetPlayerJerseySkin() }).GetValue();
        //        //pmTr.Method("SetJersey", new object[] { player.Team.Value, player.GetPlayerJerseySkin().ToString() }).GetValue();
        //        //Debug.Log("Set Role");
        //        pmTr.Method("SetRole", new object[] { player.Role.Value }).GetValue();

        //        // Access PlayerHead (likely a property) and set the sub-fields
        //        var headObj = pmTr.Property("PlayerHead").GetValue();
        //        if (headObj != null)
        //        {
        //            var headTr = Traverse.Create(headObj);
        //            headTr.Method("SetHelmetFlag", new object[] { player.Country.Value.ToString() }).GetValue();
        //            headTr.Method("SetHelmetVisor", new object[] { player.GetPlayerVisorSkin().ToString() }).GetValue();
        //            headTr.Method("SetMustache", new object[] { player.Mustache.Value.ToString() }).GetValue();
        //            headTr.Method("SetBeard", new object[] { player.Beard.Value.ToString() }).GetValue();
        //        }
        //        else
        //        {
        //            //Debug.LogWarning("[Patch] PlayerBodyV2.UpdateMesh: PlayerHead was null");
        //        }
        //        //Debug.Log($"Player Count: {PlayerManager.Instance.GetPlayers(false).Count}");
        //        UIScoreboard.Instance.UpdateServer(NetworkBehaviourSingleton<ServerManager>.Instance.Server, NetworkBehaviourSingleton<PlayerManager>.Instance.GetPlayers(false).Count);
        //        // We've fully handled the method.
        //        return false;
        //    }
        //}
        //[HarmonyPatch(typeof(Player), nameof(Player.GetPlayerJerseySkin))]
        //public static class Player_GetPlayerJerseySkin_Patch
        //{
        //    // Return false to skip the original method.
        //    static bool Prefix(Player __instance, ref FixedString32Bytes __result)
        //    {
        //        if (!PracticeModeDetector.IsPracticeMode && !NetworkManager.Singleton.IsServer)
        //        {
        //            return true;
        //        }
        //        var team = __instance.Team.Value;
        //        var role = __instance.Role.Value;
        //        //Debug.Log(team.ToString());
        //        //Debug.Log(role.ToString()); 
        //        if (team == PlayerTeam.Blue)
        //        {
        //            __result = (role == PlayerRole.Attacker)
        //                ? __instance.JerseyAttackerBlueSkin.Value
        //                : __instance.JerseyGoalieBlueSkin.Value;
        //            return false;
        //        }

        //        if (team == PlayerTeam.Red)
        //        {
        //            __result = (role == PlayerRole.Attacker)
        //                ? __instance.JerseyAttackerRedSkin.Value
        //                : __instance.JerseyGoalieRedSkin.Value;
        //            return false;
        //        }

        //        __result = default; // matches original for non-Blue/Red
        //        return false;
        //    }
        //}
    }
}