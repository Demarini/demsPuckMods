# PuckAIPractice — Handoff

## Status (2026-05-12, end of day)

Two long-standing replay bugs are both fixed and shipped to Steam Workshop (ID `3543744568`):

- **Duplicate goalie in replays** ("multiple goalies stacked, no AI, standing in net") — fixed.
- **Invisible goalie in subsequent replays** ("first goal of period shows goalie, every subsequent goal doesn't") — fixed.

Workshop DLL hash: `5839a105d5e7fd24782bc88f814bf7e601a05690c8b2b904003a46ea6ed199c1`. Deployed live on server1 (`/srv/puck/server1`), server2, and server3.

Diagnostic logging (`[DIAG]` lines on `Server_AddPlayerSpawnedEvent` / `Server_AddPlayerBodySpawnedEvent`) is **intentionally still live** so we can capture data if anything regresses. Strip it only after a few days of clean production. See "Verifying the fix from logs" below.

---

## Root cause of the duplicate-goalie bug

`StandardGameMode.OnFaceOffStarted` (PuckNew/StandardGameMode.cs:235) calls `ReplayManager.Server_StartRecording()` at **every FaceOff phase entry**. But `Server_StopRecording` only fires from `OnBlueScoreEnded` / `OnRedScoreEnded` — i.e. **only at goals, not at period boundaries**.

So when a period transitions (Play → Intermission → FaceOff → Play, no goal), the FaceOff phase re-invokes `Server_StartRecording` while a recording is *already active*. The original method short-circuits at:

```csharp
public void Server_StartRecording(int tickRate)
{
    if (this.IsRecording) return;   // ← early return, original does nothing
    ...
}
```

But **Harmony postfixes still run when the original short-circuits**. Our old `Server_StartRecording_Postfix` had no guard, so it ran on every period-transition FaceOff and injected another full set of bot spawn events (`PlayerSpawned + PlayerBodySpawned + StickSpawned` per bot) into the *ongoing* recording at the current tick (not tick 0).

When that recording was eventually stopped at the next goal and replayed, the replay system processed every `PlayerSpawned` event it found — spawning one replay-copy per event. So a recording that survived two period transitions would have **three** `PlayerSpawned` events per bot, producing three replay-copies during the replay. Two of them never got matched `PlayerBodyMove` updates and just stood at default spawn position with no AI — that's exactly the "extra goalies in the net, no AI" symptom users reported.

### Why server3 didn't reproduce it during testing

Server3 had been configured with `Play: 60` (1-minute periods) for testing. Goals happened fast enough that most recordings ended before any period boundary, so the spurious postfix call never fired. Server1/server2 have `Play: 300` (5-min periods); recordings frequently span period boundaries, so the bug shows up constantly under real traffic. The "infrequent on test, constant on prod" signature was the tell.

### The fix

`ReplayRecorderPatch.cs` — convert the standalone `Server_StartRecording` postfix into a prefix+postfix patch sharing `__state`:

```csharp
[HarmonyPatch(typeof(ReplayRecorder), "Server_StartRecording")]
public static class ReplayRecorder_Server_StartRecording_Patch
{
    static void Prefix(ReplayRecorder __instance, out bool __state)
    {
        __state = __instance.IsRecording;   // capture BEFORE original runs
    }

    static void Postfix(ReplayRecorder __instance, bool __state)
    {
        if (__state) return;                // original short-circuited; do not inject
        ...inject events for fake bots in FakePlayerRegistry.All...
    }
}
```

Now the postfix only injects at *legitimate* recording starts (when `IsRecording` was false before the call). Period-transition FaceOff calls are harmless no-ops.

---

## Root cause of the invisible-goalie bug

After every goal, `DetectPositions.Update()` calls `BotSpawning.DespawnBots(Both)` on Replay phase entry — the bot is removed from `FakePlayerRegistry` so it doesn't appear alongside the replay copies. After the replay ends, the next FaceOff fires and our `Server_StartRecording_Patch` postfix runs to inject bot spawn events into the new recording. **But the bot hasn't respawned yet** at that moment — `DetectOpenGoalAndSpawnBot` only runs every 10 frames (~167ms) inside `DetectPositions.Update()`, and the FaceOff phase change beats it.

So the postfix iterates an empty `FakePlayerRegistry.All`, injects nothing, and the new recording has zero `PlayerSpawned` events for the bot. When the bot respawns a fraction of a second later via `SpawnFakePlayer`, its `PlayerSpawned` event is correctly blocked by `BlockFakeBotPlayerSpawned` (so it doesn't get duplicated into the recording) — but nothing adds it back either. The recording proceeds with `PlayerBodyMove` events from `Server_Tick_Postfix` but no spawn event to anchor them, and at replay time `ReplayPlayer` has nothing to instantiate → invisible goalie.

The first goal of each game *did* work, because that recording started at the initial game-start FaceOff when the bot was already alive and registered.

### The fix

`ReplayRecorderPatch.cs` — add `Inject*` postfixes to the `ReplayRecorderController.Event_Everyone_On*Spawned` handlers (which the existing `BlockFakeBot*` prefixes already gate). Harmony postfixes run regardless of prefix return value, so the postfix sees every spawn event including the ones the prefix blocked.

For each fake-bot spawn event, if a recording is active, manually call the corresponding `Server_Add*SpawnedEvent` on the recorder. This means bots respawned mid-recording get their spawn events injected exactly once — late by a few ticks, but enough for the replay to instantiate a copy.

The previous duplicate-bug-source (period-transition postfix re-injection) is closed by the `__state` guard above, so re-enabling per-spawn injection cannot reintroduce duplicates.

---

## All the patches now living in `ReplayRecorderPatch.cs`

1. **`ReplayRecorder_Server_Tick_Postfix`** — every recorder tick, write a `PlayerBodyMove + StickMove + PlayerInput` event for each fake bot. Needed because fake bots are filtered out of `PlayerManager.GetSpawnedPlayers`, so the game's native per-tick loop skips them.
2. **`ReplayRecorder_Server_StartRecording_Patch`** (prefix + postfix) — at legitimate recording start, inject `PlayerSpawned + PlayerBodySpawned + StickSpawned` for each bot in `FakePlayerRegistry.All`. Prefix-state guard prevents firing on period-transition FaceOff short-circuits.
3. **`BlockFakeBotPlayerSpawned` / `BlockFakeBotPlayerBodySpawned` / `BlockFakeBotStickSpawned`** — prefix patches on `ReplayRecorderController.Event_Everyone_On*Spawned` that suppress the controller's auto-add for fake-bot clientIds. Without these, mid-recording bot spawns would also flow through the controller route, causing duplicates again.
4. **`InjectFakeBotPlayerSpawned` / `InjectFakeBotPlayerBodySpawned` / `InjectFakeBotStickSpawned`** — postfixes on the same controller handlers. When a fake-bot spawn event fires and a recording is active, manually inject the event. This covers bots that respawn mid-recording (the invisible-goalie case).
5. **`DiagLogPlayerSpawnedEvent` / `DiagLogPlayerBodySpawnedEvent`** — diagnostic logging on `Server_AddPlayerSpawnedEvent` / `Server_AddPlayerBodySpawnedEvent`. Logs every event added to a recording with clientId, tick, recording state, and caller. Kept live to catch regressions. Strip after a few days of clean production.

---

## Other patches added in this session

- **`BotSpawning.SpawnFakePlayer` / `SpawnChaser` pre-check** — early-return if `FakePlayerRegistry.HasBotForTeam(team)` is already true. Closes a race where two concurrent `/vs` chat commands could each call `SpawnFakePlayer` before `Register` fired, creating two NetworkObjects with the same fake clientId. Real race that existed independently of the postfix bug; keep the guard.
- **`Patches/CharacterSpawnPatch.cs`** — Harmony prefix on `Player.Server_SpawnCharacter` that returns false (skip) for fake-bot Players that already have a character spawned. Originally added on the theory that the game's `StandardGameMode.OnPlayerPhaseChanged` was re-spawning characters on fake bots across period boundaries; the *real* bug turned out to be elsewhere (recorder side), but this guard is harmless and defensive — keep it.

---

## Investigation history (lessons learned)

Three failed hypotheses before finding the real cause, all worth documenting so we don't repeat the mistakes:

1. **First (yesterday morning): "BlockFake prefixes check the wrong field."** Switched from `IsFake(player)` to `IsFakeClientId(player.OwnerClientId)` because `Register` is called *after* `SpawnWithOwnership` fires the PlayerSpawned event. This was a real timing issue but only a contributing factor — the duplicates kept appearing in different shapes after this fix. *Lesson: a fix that "should work" but doesn't fully resolve the symptom isn't the root cause, even if it's clean code.*
2. **Second (yesterday afternoon): "Concurrent /vs commands race."** Added the `HasBotForTeam` pre-check to `SpawnFakePlayer` / `SpawnChaser`. This *is* a real race and the pre-check is worth keeping, but it didn't explain the period-transition duplicate pattern. *Lesson: closing one race doesn't mean you've found all of them.*
3. **Third (yesterday night): "Game's `Server_SpawnCharacter` is re-attaching characters across phase transitions."** Added `CharacterSpawnPatch` based on misreading log lines: I saw two consecutive `[Player] Despawning character (7777777)` lines and concluded there were two character NetworkObjects. **Wrong.** `Server_DespawnCharacter` (`PuckNew/Player.cs:1309`) logs unconditionally even when there's nothing to despawn — both child despawns are no-ops if `PlayerBody == null`. The "double despawn" log pattern was just `BotSpawning.Despawn` calling `Server_DespawnCharacter` and then `Server_SetGameState(team: None)` indirectly re-invoking it. There was only ever one character per bot. *Lesson: when interpreting verbose game logs, read the source of every log line before drawing conclusions. Game-side code logs at the entry of methods, not at the actual work, and many methods are defensive no-ops.*

The real cause was only found this morning by **observing the data, not theorizing**:
- Added `[DIAG]` Harmony patches to `Server_AddPlayerSpawnedEvent` / `Server_AddPlayerBodySpawnedEvent` that log every call with tick + caller.
- Deployed to server1 under real traffic.
- Within an hour, the diagnostic showed events being added at `tick=20292` (mid-recording!) by `ReplayRecorder_Server_StartRecording_Postfix.Postfix`. The expectation was `tick=0` at recording start. That non-zero tick was the conclusive evidence — and tracing what could cause it pointed straight at `OnFaceOffStarted` firing during period transitions.

**Diagnostic logging on production code was what finally cracked it.** Before this, we kept iterating on increasingly-defensive code changes based on theories. The DIAG patches added almost no overhead but gave us perfect ground truth.

User preference saved as memory feedback: don't strip diagnostic logging before publishing if the bug being investigated is rare and not yet definitively fixed — production needs the instruments live.

---

## Verifying the fix from logs

Count of `PlayerSpawned` events added per recording per fake clientId — should be **exactly 1** per recording per bot:

```bash
sudo journalctl -u puck@server1 --since '1 hour ago' | \
  grep -E '\[DIAG\].*AddPlayerSpawnedEvent.*FAKE' | wc -l
```

Compare to the count of recording starts in the same window — they should match (1 event per bot per recording, so 2 bots × N recordings = 2N events).

If the per-recording count of `PlayerSpawned` events for one fake clientId rises above 1, the duplicate bug is back. The current `[DIAG]` logging captures the caller, so you'll see immediately which patch added the extra.

For the invisible-goalie health check:

```bash
sudo journalctl -u puck@server1 --since '30 min ago' | \
  grep -E 'Replay recording started|InjectFakeBot.*Postfix'
```

After each recording-start that *follows* a goal replay, you should see `InjectFakeBot*` postfix entries fire as the bot respawns (which means the bot's spawn events are landing in the new recording). If those entries are missing, the invisible-goalie bug would reappear.

---

## Key files

| File | Role |
|---|---|
| `InitializePuckAI.cs` | Mod entry point + `PhaseChangePatch` (pause AI during Replay) |
| `Patches/ReplayRecorderPatch.cs` | All replay-related patches (Block + Inject + StartRecording + Tick + DIAG) |
| `Patches/CharacterSpawnPatch.cs` | Blocks `Server_SpawnCharacter` for fake bots with existing character (defensive) |
| `Patches/PlayerPatch.cs` | Blocks `PlayerManager.AddPlayer` for fake bots |
| `Patches/PlayerSearch.cs` | Filters fake bots out of `PlayerManager.GetPlayers` results |
| `Utilities/BotSpawning.cs` | `SpawnFakePlayer`, `SpawnChaser`, `Despawn`, `DetectOpenGoalAndSpawnBot` |
| `Utilities/FakePlayerRegistry.cs` | Single source of truth for which players are bots |
| `Utilities/DetectPositions.cs` | Periodic loop: `DespawnBots` on Replay phase; `DetectOpenGoalAndSpawnBot` otherwise |
| `AI/GoalieAI.cs` | The actual goalie behavior MonoBehaviour |

---

## Game-side classes referenced

| Symbol | Source | Role |
|---|---|---|
| `StandardGameMode.OnFaceOffStarted` | `PuckNew/StandardGameMode.cs:235` | Calls `ReplayManager.Server_StartRecording()` — fires on every FaceOff including period transitions |
| `StandardGameMode.OnBlueScoreEnded` / `OnRedScoreEnded` | `PuckNew/StandardGameMode.cs:316,324` | Only places `Server_StopRecording` is called |
| `ReplayRecorder.Server_StartRecording` | `PuckNew/ReplayRecorder.cs:33` | Short-circuits if `IsRecording == true`; clears EventMap, resets Tick |
| `ReplayRecorder.Server_AddPlayerSpawnedEvent` | `PuckNew/ReplayRecorder.cs:154` | Adds event to recording's EventMap at current Tick |
| `ReplayRecorderController.Event_Everyone_OnPlayerSpawned` | `PuckNew/ReplayRecorderController.cs:42` | Auto-captures spawns into recording (Block prefixes suppress for fake bots) |
| `ReplayPlayer.Server_ReplayEvent` | `PuckNew/ReplayPlayer.cs:268` | Spawns replay copies based on recorded events |
| `Player.Server_DespawnCharacter` | `PuckNew/Player.cs:1309` | Logs unconditionally even when no-op — caused the misread log pattern |
| `PlayerManager.Server_SpawnPlayer` | `PuckNew/PlayerManager.cs:145` | Spawns replay copies with `OwnerClientId = originalClientId + 1337` |

---

## Cleanup punch-list (separate from bug fixes)

Not blockers, but accumulated tech debt worth addressing on a slow day:

1. **Strip `[DIAG]` logging from `ReplayRecorderPatch.cs`** after a few days of clean production (no reports of duplicates or invisible goalies). The two `DiagLog*` classes can be deleted wholesale.
2. **Dead commented-out code.** `Patches/PlayerPatch.cs` has a large commented block. `InitializePuckAI.cs` has ~120 lines of commented Harmony patches (SettingsManager skin-change loggers). `BotInputPatch.cs` likely similar. Delete.
3. **`SpawnChaser` vs `SpawnFakePlayer`** in `Utilities/BotSpawning.cs` — ~80% identical. Extract shared mesh/skin/setup into a private helper.
4. **Stale `GoaliesAreRunning` flag.** Set to `true` in `InitializePuckAI.OnEnable()` before any bots are spawned, so it's an unreliable "are bots actually running" signal. `ProcessChatCommands` reads it to decide what to do — stale state causes wrong behavior. Fix: only set true after a successful spawn, set false after confirmed despawn.
5. **Dead fields in `BotSpawning`:** `redGoalie`, `blueGoalie`, `blueGoalieSpawned`, `redGoalieSpawned` — declared, mostly unused. Compiler warned about these. Remove.
6. **Early-return bug in `SpawnFakePlayer`.** The `IsCharacterPartiallySpawned` branch in older versions returned early before registering the player in `FakePlayerRegistry` or attaching `GoalieAI`. Verify it's not still reachable.
7. **`FakePlayerRegistry.CleanupDestroyed()` doesn't prune `fakeClientIds`** — only `fakePlayers` and `fakePlayerTeams`. Stale IDs accumulate. Tiny issue since IDs are fixed (7777777/7777778) and reused, but inconsistent.
8. **`PuckAIPractice` path resolution uses Windows separators on Linux.** Logs show `Resolved gameDir: /srv/puck/serverN/Plugins/PuckAIPractice/..\..\..\../common/Puck` with literal `\` characters. Creates a real directory named `..\..\..\..` inside the plugin folder. Works but ugly. Search source for hardcoded `\..\..\..\..` and replace with `Path.Combine` or `/`.

---

## Other work done in this session (broader than PuckAIPractice)

- **Steam Workshop publish flow for scenery maps.** New `scenery_maps` section in `workshop.json`; new `map` verb in `publish.ps1` (`map list`, `map <Name> "note"`, `map all "note"`, `map toggle <Name>`) with first-time create flow (writes title/description/visibility=2 VDF, parses new `PublishedFileId` from SteamCMD output, writes it back to workshop.json). Each map (e.g. DanceClub) publishes as its own Workshop item that the SceneryLoader's `BundleResolver` finds via sibling workshop-content folders.
- **`.gitignore` cleanup.** Replaced verbose per-mod `bin/`/`obj/`/`.vs/` entries with globbed patterns. Added `UpgradeLog*.htm`. Kept `AssetInformation.json` exceptions for `SceneryChanger/AssetBundles/` and `SceneryMaps/*/AssetBundles/`.
- **All 3 servers' bot difficulty set to Hard** (`RedGoalieDefaultDifficulty: 2` / `BlueGoalieDefaultDifficulty: 2`) — applied to live config and seed config files so workshop updates don't revert.
- **Server3 period length set to 60s** for testing (`Play: 60` in `/srv/puck/server3/public_game_mode_config.json`). Revert with: `sudo sed -i 's/"Play": 60/"Play": 300/' /srv/puck/server3/public_game_mode_config.json && sudo systemctl restart puck@server3`.
- **Branch `tm/updateModsForNewUpdate`** committed (217 files): all pre-release game-update fixes, new `AutoJoin` mod source, `UPDATE_PLAYBOOK.md`, `SceneryMaps/DanceClub/` registration. Two commits ahead of `origin/tm/updateModsForNewUpdate` and not yet pushed.
