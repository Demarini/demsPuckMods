using HarmonyLib;
using PuckAIPractice.Utilities;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace PuckAIPractice
{
    // Records movement/input for all registered bots on every recorder tick.
    [HarmonyPatch(typeof(ReplayRecorder), "Server_Tick")]
    public static class ReplayRecorder_Server_Tick_Postfix
    {
        static void Postfix(ReplayRecorder __instance)
        {
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
                return;

            try
            {
                foreach (Player player in FakePlayerRegistry.All)
                {
                    __instance.Server_AddReplayEvent("PlayerBodyMove", new ReplayPlayerBodyMove
                    {
                        OwnerClientId = player.OwnerClientId,
                        Position = player.PlayerBody.transform.position,
                        Rotation = player.PlayerBody.transform.rotation,
                        Stamina = player.PlayerBody.Stamina.Value,
                        Speed = player.PlayerBody.Speed.Value,
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
                Debug.LogError($"[ReplayPatch] Server_Tick Postfix error: {e}");
            }
        }
    }

    // Step 2: Inject bot spawn events into the initial recording snapshot so the
    // replay system knows about them from tick 0 (not mid-recording).
    [HarmonyPatch(typeof(ReplayRecorder), "Server_StartRecording")]
    public static class ReplayRecorder_Server_StartRecording_Postfix
    {
        static void Postfix(ReplayRecorder __instance)
        {
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
                return;

            try
            {
                foreach (Player player in FakePlayerRegistry.All)
                {
                    __instance.Server_AddPlayerSpawnedEvent(player);
                    if (player.PlayerBody)
                        __instance.Server_AddPlayerBodySpawnedEvent(player.PlayerBody);
                    if (player.Stick)
                        __instance.Server_AddStickSpawnedEvent(player.Stick);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[ReplayPatch] Server_StartRecording Postfix error: {e}");
            }
        }
    }

    // Step 1: Block bot player-spawned events from being written mid-recording.
    // Bots are injected at recording-start instead (see above), so mid-recording
    // events would create a second entry and cause the duplicate-goalie bug.
    //
    // Check by OwnerClientId, not by the Player object: SpawnWithOwnership fires
    // PlayerSpawned synchronously before BotSpawning.SpawnFakePlayer has a chance
    // to call FakePlayerRegistry.Register(player). The client ID is reserved before
    // the spawn (ReserveFakeClientId), so it's the only reliable signal at this point.
    [HarmonyPatch(typeof(ReplayRecorderController), "Event_Everyone_OnPlayerSpawned")]
    static class BlockFakeBotPlayerSpawned
    {
        static bool Prefix(Dictionary<string, object> message)
        {
            var player = (Player)message["player"];
            return player == null || !FakePlayerRegistry.IsFakeClientId(player.OwnerClientId);
        }
    }

    [HarmonyPatch(typeof(ReplayRecorderController), "Event_Everyone_OnPlayerBodySpawned")]
    static class BlockFakeBotPlayerBodySpawned
    {
        static bool Prefix(Dictionary<string, object> message)
        {
            var playerBody = (PlayerBody)message["playerBody"];
            return playerBody == null || playerBody.Player == null
                || !FakePlayerRegistry.IsFakeClientId(playerBody.Player.OwnerClientId);
        }
    }

    [HarmonyPatch(typeof(ReplayRecorderController), "Event_Everyone_OnStickSpawned")]
    static class BlockFakeBotStickSpawned
    {
        static bool Prefix(Dictionary<string, object> message)
        {
            var stick = (Stick)message["stick"];
            return stick == null || stick.Player == null
                || !FakePlayerRegistry.IsFakeClientId(stick.Player.OwnerClientId);
        }
    }
}
