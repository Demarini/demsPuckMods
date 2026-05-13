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
    //
    // CRITICAL: StandardGameMode.OnFaceOffStarted calls Server_StartRecording every
    // FaceOff phase entry, but Server_StopRecording only fires on goal score-end.
    // So period transitions (Play -> Intermission -> FaceOff -> Play) re-invoke
    // Server_StartRecording while a recording is already active. The original
    // short-circuits at `if (this.IsRecording) return;` but Harmony postfixes still
    // run -- without guarding, this injects another full set of bot spawn events
    // into the ONGOING recording at the current tick. Each spurious injection adds
    // one more replay-copy of every fake bot during the next replay -- this is the
    // "duplicate goalie in replay" bug.
    //
    // Fix: capture IsRecording in a prefix BEFORE the original runs, and skip the
    // postfix injection when the original was going to short-circuit (i.e. it was
    // already recording).
    [HarmonyPatch(typeof(ReplayRecorder), "Server_StartRecording")]
    public static class ReplayRecorder_Server_StartRecording_Patch
    {
        static void Prefix(ReplayRecorder __instance, out bool __state)
        {
            __state = __instance.IsRecording;
        }

        static void Postfix(ReplayRecorder __instance, bool __state)
        {
            if (__state) return; // original short-circuited; do not inject into ongoing recording

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

    // Step 4: Inject bot spawn events when the bot spawns DURING an active recording.
    //
    // Server_StartRecording_Patch (above) only injects events for bots that are in the
    // registry at the moment a recording starts. But after every goal, DetectPositions
    // despawns bots on Replay phase entry, and the next recording starts at FaceOff
    // BEFORE DetectOpenGoalAndSpawnBot has a chance to respawn them (~10 frames later).
    // Result: the new recording's Server_StartRecording_Patch sees an empty registry,
    // injects nothing, and the replay has no PlayerSpawned event for the bot --> the
    // goalie is invisible in every replay after the first one of each game.
    //
    // The Block* prefixes above stop the controller from auto-capturing fake-bot spawn
    // events into the recording (which would have caused duplicates before today's
    // Server_StartRecording guard). Postfixes still run even when their matching prefix
    // returned false, so these postfixes are the natural place to inject the spawn event
    // ourselves -- gated on "recording is active" so we only fire mid-recording.
    [HarmonyPatch(typeof(ReplayRecorderController), "Event_Everyone_OnPlayerSpawned")]
    static class InjectFakeBotPlayerSpawned
    {
        static void Postfix(ReplayRecorderController __instance, Dictionary<string, object> message)
        {
            try
            {
                var player = (Player)message["player"];
                if (player == null) return;
                if (!FakePlayerRegistry.IsFakeClientId(player.OwnerClientId)) return;
                var rec = __instance.GetComponent<ReplayRecorder>();
                if (rec != null && rec.IsRecording)
                    rec.Server_AddPlayerSpawnedEvent(player);
            }
            catch (Exception e)
            {
                Debug.LogError($"[ReplayPatch] InjectFakeBotPlayerSpawned error: {e}");
            }
        }
    }

    [HarmonyPatch(typeof(ReplayRecorderController), "Event_Everyone_OnPlayerBodySpawned")]
    static class InjectFakeBotPlayerBodySpawned
    {
        static void Postfix(ReplayRecorderController __instance, Dictionary<string, object> message)
        {
            try
            {
                var pb = (PlayerBody)message["playerBody"];
                if (pb == null || pb.Player == null) return;
                if (!FakePlayerRegistry.IsFakeClientId(pb.Player.OwnerClientId)) return;
                var rec = __instance.GetComponent<ReplayRecorder>();
                if (rec != null && rec.IsRecording)
                    rec.Server_AddPlayerBodySpawnedEvent(pb);
            }
            catch (Exception e)
            {
                Debug.LogError($"[ReplayPatch] InjectFakeBotPlayerBodySpawned error: {e}");
            }
        }
    }

    [HarmonyPatch(typeof(ReplayRecorderController), "Event_Everyone_OnStickSpawned")]
    static class InjectFakeBotStickSpawned
    {
        static void Postfix(ReplayRecorderController __instance, Dictionary<string, object> message)
        {
            try
            {
                var stick = (Stick)message["stick"];
                if (stick == null || stick.Player == null) return;
                if (!FakePlayerRegistry.IsFakeClientId(stick.Player.OwnerClientId)) return;
                var rec = __instance.GetComponent<ReplayRecorder>();
                if (rec != null && rec.IsRecording)
                    rec.Server_AddStickSpawnedEvent(stick);
            }
            catch (Exception e)
            {
                Debug.LogError($"[ReplayPatch] InjectFakeBotStickSpawned error: {e}");
            }
        }
    }

    // Diagnostic logging — kept live while we investigate the rare "duplicate goalie
    // in replay" symptom. Each call to Server_AddPlayerSpawnedEvent /
    // Server_AddPlayerBodySpawnedEvent gets logged with its caller, so a recording
    // that ends up with 2 spawn events for a fake clientId will show up clearly
    // in journalctl. Remove once the bug is confirmed gone in production.
    [HarmonyPatch(typeof(ReplayRecorder), nameof(ReplayRecorder.Server_AddPlayerSpawnedEvent))]
    static class DiagLogPlayerSpawnedEvent
    {
        static void Prefix(ReplayRecorder __instance, Player player)
        {
            if (player == null) return;
            bool fake = FakePlayerRegistry.IsFakeClientId(player.OwnerClientId);
            string tag = fake ? "FAKE" : "real";
            Debug.Log($"[DIAG] AddPlayerSpawnedEvent clientId={player.OwnerClientId} ({tag}) tick={__instance.Tick} recording={__instance.IsRecording} caller={new System.Diagnostics.StackTrace(1, false).GetFrame(1)?.GetMethod()?.DeclaringType?.Name}.{new System.Diagnostics.StackTrace(1, false).GetFrame(1)?.GetMethod()?.Name}");
        }
    }

    [HarmonyPatch(typeof(ReplayRecorder), nameof(ReplayRecorder.Server_AddPlayerBodySpawnedEvent))]
    static class DiagLogPlayerBodySpawnedEvent
    {
        static void Prefix(ReplayRecorder __instance, PlayerBody playerBody)
        {
            if (playerBody == null || playerBody.Player == null) return;
            bool fake = FakePlayerRegistry.IsFakeClientId(playerBody.Player.OwnerClientId);
            string tag = fake ? "FAKE" : "real";
            Debug.Log($"[DIAG] AddPlayerBodySpawnedEvent clientId={playerBody.Player.OwnerClientId} ({tag}) tick={__instance.Tick} recording={__instance.IsRecording} caller={new System.Diagnostics.StackTrace(1, false).GetFrame(1)?.GetMethod()?.DeclaringType?.Name}.{new System.Diagnostics.StackTrace(1, false).GetFrame(1)?.GetMethod()?.Name}");
        }
    }
}
