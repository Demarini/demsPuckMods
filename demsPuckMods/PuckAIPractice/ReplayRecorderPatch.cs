using HarmonyLib;
using PuckAIPractice.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace PuckAIPractice
{
    [HarmonyPatch(typeof(ReplayRecorder), "Server_Tick")]
    public static class ReplayRecorder_Server_Tick_Postfix
    {
        // Cache a fast delegate to ReplayRecorder.Server_AddReplayEvent(string, object)
        private static readonly Action<ReplayRecorder, string, object> _addReplayEvent;

        // Simple payload example; replace with your own serializable type

        static ReplayRecorder_Server_Tick_Postfix()
        {
            var mi = AccessTools.Method(
                typeof(ReplayRecorder),
                "Server_AddReplayEvent",
                new[] { typeof(string), typeof(object) });

            if (mi != null)
            {
                _addReplayEvent =
                    AccessTools.MethodDelegate<Action<ReplayRecorder, string, object>>(mi);
            }
        }

        // Runs after the original Server_Tick has completed
        static void Postfix(ReplayRecorder __instance)
        {
            // If the original early-returned because we're not server, mirror that here
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
                return;

            // Add whatever you need after the recorder did its normal work
            try
            {

                foreach (Player player in FakePlayerRegistry.All)
                {
                    //Debug.Log("Adding Replay For " + player.Username.Value.ToString());
                    __instance.Server_AddReplayEvent("PlayerBodyMove", new ReplayPlayerBodyMove
                    {
                        OwnerClientId = player.OwnerClientId,
                        Position = player.PlayerBody.transform.position,
                        Rotation = player.PlayerBody.transform.rotation,
                        Stamina = player.PlayerBody.StaminaCompressed.Value,
                        Speed = player.PlayerBody.StaminaCompressed.Value,
                        IsSprinting = player.PlayerBody.IsSprinting.Value,
                        IsSliding = player.PlayerBody.IsSliding.Value,
                        IsStopping = player.PlayerBody.IsStopping.Value,
                        IsExtendedLeft = player.PlayerBody.IsExtendedLeft.Value,
                        IsExtendedRight = player.PlayerBody.IsExtendedRight.Value
                    });
                    __instance.Server_AddReplayEvent("StickMove", new ReplayStickMove
                    {
                        OwnerClientId = player.OwnerClientId,
                        Position = player.Stick.transform.position,
                        Rotation = player.Stick.transform.rotation
                    });
                    __instance.Server_AddReplayEvent("PlayerInput", new ReplayPlayerInput
                    {
                        OwnerClientId = player.OwnerClientId,
                        LookAngleInput = player.PlayerInput.LookAngleInput.ServerValue,
                        BladeAngleInput = player.PlayerInput.BladeAngleInput.ServerValue,
                        TrackInput = player.PlayerInput.TrackInput.ServerValue,
                        LookInput = player.PlayerInput.LookInput.ServerValue
                    });
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[YourMod] Postfix failed: {e}");
            }
        }
    }
    [HarmonyPatch(typeof(ReplayRecorder), "Server_StartRecording")]
    public static class ReplayRecorder_Server_StartRecording_Postfix
    {
        // Runs after the original Server_Tick has completed
        static void Postfix(ReplayRecorder __instance)
        {
            // If the original early-returned because we're not server, mirror that here
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
                return;

            // Add whatever you need after the recorder did its normal work
            Debug.Log("Server Start Recording");
            try
            {
                if(FakePlayerRegistry.All.Count() == 0)
                {
                    Debug.Log("Detecting Bots Before Recording");
                    //BotSpawning.DetectOpenGoalAndSpawnBot();
                }
                foreach (Player player in FakePlayerRegistry.AllExisting)
                {
                    __instance.Server_AddPlayerSpawnedEvent(player);
                    if (player.PlayerBody)
                    {
                        Debug.Log("Goalie Skins");
                        Debug.Log(player.PlayerBody.Player.JerseyGoalieRedSkin.Value.ToString());
                        Debug.Log(player.PlayerBody.Player.JerseyGoalieBlueSkin.Value.ToString());
                        __instance.Server_AddPlayerBodySpawnedEvent(player.PlayerBody);
                    }
                    if (player.Stick)
                    {
                        __instance.Server_AddStickSpawnedEvent(player.Stick);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[YourMod] Postfix failed: {e}");
            }
        }
    }
    //[HarmonyPatch(typeof(ReplayPlayer), "Server_StopReplay")]
    //static class Event_OnPlayerSpawned_Prefix
    //{
    //    static bool Prefix(ReplayPlayer __instance)
    //    {
    //        try
    //        {
    //            if (!NetworkManager.Singleton.IsServer)
    //            {
    //                return false;
    //            }
    //            BotSpawning.DetectOpenGoalAndSpawnBot();
    //            return true;
    //        }
    //        catch (Exception e)
    //        {
    //            Debug.LogError($"[ReplayPatch] Prefix(Event_OnPlayerSpawned) error: {e}");
    //            return false;
    //        }
    //    }
    //}
}
