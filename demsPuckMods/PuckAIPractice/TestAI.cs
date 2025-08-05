using DG.Tweening.Core.Easing;
using HarmonyLib;
using PuckAIPractice.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;

namespace PuckAIPractice
{
    public class TestAI : IPuckMod
    {
        static readonly Harmony harmony = new Harmony("GAFURIX.PuckAIPractice");

        public bool OnEnable()
        {
            Debug.Log("[PuckAIPractice] Enabled");
            try
            {
                harmony.PatchAll();
                //HarmonyLogger.PatchSpecificMethods(harmony, typeof(Player), new List<string>() { "OnNetworkSpawn", "OnNetworkPostSpawn", "Client_SetPlayerStateRpc", "Server_RespawnCharacter", "Server_DespawnCharacter" });
                //HarmonyLogger.PatchAllMethods(harmony, typeof(UIManager));
            }
            catch (Exception e)
            {
                Debug.LogError($"[PuckAIPractice] Harmony patch failed: {e}");
                return false;
            }

            return true;
        }

        public bool OnDisable()
        {
            try
            {
                harmony.UnpatchSelf();
            }
            catch (Exception e)
            {
                Debug.LogError($"[PuckAIPractice] Harmony unpatch failed: {e}");
                return false;
            }

            return true;
        }
    }
    [HarmonyPatch(typeof(GameManager), "OnGameStateChanged")]
    public static class PhaseChangePatch
    {
        [HarmonyPostfix]
        public static void Postfix(GameManager __instance, GameState oldGameState, GameState newGameState)
        {
            if (oldGameState.Phase == newGameState.Phase)
                return;

            if (newGameState.Phase != GamePhase.Warmup && newGameState.Phase != GamePhase.Playing)
            {
                Debug.Log($"[FAKE_SPAWN] Skipped spawn — new state is {newGameState}");
                return;
            }

            Debug.Log($"[FAKE_SPAWN] State changed from {oldGameState} to {newGameState}. Injecting fake players...");

            //for (int i = 0; i < 9; i++)
            //    SpawnFakePlayer(i, PlayerRole.Attacker, (i % 2 == 0) ? PlayerTeam.Blue : PlayerTeam.Red);

            //BotSpawning.SpawnFakePlayer(69, PlayerRole.Goalie, PlayerTeam.Blue);

            //BotSpawning.SpawnFakePlayer(6969, PlayerRole.Goalie, PlayerTeam.Red);
        } 
    }
    //[HarmonyPatch(typeof(SettingsManager), "UpdateJerseySkin")]
    //public static class LogJerseySkinChange
    //{
    //    [HarmonyPostfix]
    //    public static void Postfix(PlayerTeam team, PlayerRole role, string value)
    //    {
    //        if (team == PlayerTeam.Blue)
    //        {
    //            Debug.Log($"[SkinChange] Jersey ({role}): {value}");
    //        }
    //    }
    //}
    //[HarmonyPatch(typeof(SettingsManager), "UpdateStickSkin")]
    //public static class LogStickSkinChange
    //{
    //    [HarmonyPostfix]
    //    public static void Postfix(PlayerTeam team, PlayerRole role, string value)
    //    {
    //        if (team == PlayerTeam.Blue)
    //        {
    //            Debug.Log($"[SkinChange] Stick ({role}): {value}");
    //        }
    //    }
    //}
    //[HarmonyPatch(typeof(SettingsManager), "UpdateVisorSkin")]
    //public static class LogVisorSkinChange
    //{
    //    [HarmonyPostfix]
    //    public static void Postfix(PlayerTeam team, PlayerRole role, string value)
    //    {
    //        if (team == PlayerTeam.Blue)
    //        {
    //            Debug.Log($"[SkinChange] Visor ({role}): {value}");
    //        }
    //    }
    //}
    //[HarmonyPatch(typeof(SettingsManager), "UpdateBeard")]
    //public static class LogBeardSkinChange
    //{
    //    [HarmonyPostfix]
    //    public static void Postfix(string value)
    //    {
    //        Debug.Log($"[SkinChange] Beard: {value}");
    //    }
    //}
    //[HarmonyPatch(typeof(SettingsManager), "UpdateMustache")]
    //public static class LogMustacheSkinChange
    //{
    //    [HarmonyPostfix]
    //    public static void Postfix(string value)
    //    {
    //        Debug.Log($"[SkinChange] Mustache: {value}");
    //    }
    //}
    //[HarmonyPatch(typeof(SettingsManager), "UpdateStickBladeSkin")]
    //public static class LogSitckBladeTapeSkinChange
    //{
    //    [HarmonyPostfix]
    //    public static void Postfix(PlayerTeam team, PlayerRole role, string value)
    //    {
    //        if (team == PlayerTeam.Blue)
    //        {
    //            Debug.Log($"[SkinChange] SitckBladeTape ({role}): {value}");
    //        }
    //    }
    //}
    //[HarmonyPatch(typeof(SettingsManager), "UpdateStickShaftSkin")]
    //public static class LogSitckShaftTapeSkinChange
    //{
    //    [HarmonyPostfix]
    //    public static void Postfix(PlayerTeam team, PlayerRole role, string value)
    //    {
    //        if (team == PlayerTeam.Blue)
    //        {
    //            Debug.Log($"[SkinChange] SitckShaftTape ({role}): {value}");
    //        }
    //    }
    //}
    //[HarmonyPatch(typeof(SettingsManager), "UpdateCountry")]
    //public static class LogCountrySkinChange
    //{
    //    [HarmonyPostfix]
    //    public static void Postfix(string value)
    //    {
    //        Debug.Log($"[SkinChange] Country: {value}");
    //    }
    //}
    //[HarmonyPatch(typeof(SettingsManagerController), "Event_Client_OnAppearanceStickSkinChanged")]
    //public static class OverrideAppearanceStickSkinPreview
    //{
    //    [HarmonyPrefix]
    //    public static bool Prefix(object __instance, Dictionary<string, object> message)
    //    {
    //        var team = (PlayerTeam)message["team"];
    //        var role = (PlayerRole)message["role"];
    //        var value = (string)message["value"];

    //        // Custom override: convert camouflage to digital for goalies
    //        if (value == "camouflage_attacker")
    //        {
    //            Debug.Log("[SkinOverride] Replacing 'camouflage_attacker' with 'digital_goalie' for Goalie");
    //            value = "digital_goalie";
    //        }

    //        // Use Traverse to get the private settingsManager field
    //        var settingsManager = Traverse.Create(__instance).Field("settingsManager").GetValue() as SettingsManager;

    //        if (settingsManager != null)
    //        {
    //            settingsManager.UpdateStickSkin(team, role, value);
    //        }
    //        else
    //        {
    //            Debug.LogWarning("[SkinOverride] Could not access settingsManager");
    //        }

    //        return false; // Skip original method
    //    }
    //}
}