# PuckAIPractice — Goalie Duplication Bug & Refactor Handoff

## Status
Research complete. Root cause identified. Fix not yet implemented.

---

## The Bug: Duplicate Goalie During Replay

### Symptom
When a goal is scored and the replay plays, sometimes the AI goalie appears
twice — once at its pre-replay position and once being moved by the replay
system. After the replay ends, the duplicate persists in the middle of the ice.

### Root Cause

The game uses an event-driven system to capture spawns into the replay recording.
`ReplayRecorderController` (game source: `PuckNew/ReplayRecorderController.cs`)
listens to `Event_Everyone_OnPlayerSpawned` and `Event_Everyone_OnPlayerBodySpawned`.
When these events fire, it calls `replayRecorder.Server_AddPlayerSpawnedEvent(player)`.

When `BotSpawning.SpawnFakePlayer` runs:
1. `netObj.SpawnWithOwnership(fakeClientId, true)` fires `Event_Everyone_OnPlayerSpawned`
2. `playerObj.Server_SpawnCharacter(...)` fires `Event_Everyone_OnPlayerBodySpawned`

Both events are intercepted by `ReplayRecorderController` **before** the mod's
subsequent `PlayerManager.RemovePlayer(player)` call. If the recorder is already
active at spawn time, `PlayerSpawned` and `PlayerBodySpawned` events for the bot
get written into the recording.

During replay (`ReplayPlayer.Server_ReplayEvent`):
- `PlayerSpawned` → `PlayerManager.Server_SpawnPlayer(..., isReplay: true)` →
  a **new replay copy** of the bot is created
- `PlayerBodySpawned` → a body is spawned for the replay copy
- `PlayerBodyMove` events (from `ReplayRecorder_Server_Tick_Postfix`) → replay
  copy moves correctly

The **original real bot** is still alive on the ice at the same time.
Two goalies are on the ice. When `Server_DisposeReplayObjects()` runs after the
replay, the replay copy is cleaned up — but it may not be found in
`PlayerManager.GetReplayPlayers()` because the bot's `OwnerClientId` (7777778/9)
was removed from `PlayerManager` via `RemovePlayer()`. If cleanup misses it,
the replay copy persists.

### Why "sometimes"
- If the bot was spawned **before** the recording started: `Server_StartRecording`
  calls `GetPlayers(false)`, which the `ExcludeFakePlayersFromGetPlayers` patch
  filters. No spawn event recorded → no replay copy → no duplication.
- If the bot was spawned (or respawned) **while** recording is active (e.g. the
  user issued `/goalies` mid-period): the event fires and is captured → duplication.

---

## Current Workaround
Goalies are hidden during the `GamePhase.Replay` phase entirely. This avoids
the visual duplication but means you can't see the goalie in legitimate replays.
The hiding code was not found in the current source (may have been removed, or
lives in a Harmony patch that was since commented out).

---

## Fix Plan

### Step 1 — Block fake player events from the recorder
Patch `ReplayRecorderController` to skip fake players:

```csharp
[HarmonyPatch(typeof(ReplayRecorderController), "Event_Everyone_OnPlayerSpawned")]
static class BlockFakeBotPlayerSpawned
{
    static bool Prefix(Dictionary<string, object> message)
    {
        var player = (Player)message["player"];
        return !FakePlayerRegistry.IsFake(player); // skip if fake
    }
}

// Same pattern for Event_Everyone_OnPlayerBodySpawned
// Same pattern for Event_Everyone_OnStickSpawned
```

This stops the duplication. Bots will be invisible in replays (same as current
workaround) but without the duplication risk.

### Step 2 — Make bots visible in replays (optional, full fix)
To properly show bots in replays, manually inject their spawn events at
recording-start time (when `Server_StartRecording` runs), so they're in the
initial snapshot instead of mid-recording:

```csharp
[HarmonyPatch(typeof(ReplayRecorder), "Server_StartRecording")]
static class InjectFakePlayerSpawnEvents
{
    static void Postfix(ReplayRecorder __instance)
    {
        if (!NetworkManager.Singleton.IsServer) return;
        foreach (Player bot in FakePlayerRegistry.All)
        {
            __instance.Server_AddPlayerSpawnedEvent(bot);
            if (bot.PlayerBody)
                __instance.Server_AddPlayerBodySpawnedEvent(bot.PlayerBody);
            if (bot.Stick)
                __instance.Server_AddStickSpawnedEvent(bot.Stick);
        }
    }
}
```

With Steps 1+2:
- Spawn events exist in the initial snapshot (not mid-recording)
- Movement events (already recorded by `ReplayRecorder_Server_Tick_Postfix`) are
  played back against the replay copy
- Replay copy is cleaned up by `Server_DisposeReplayObjects()` via
  `GetReplayPlayers()` — this should work since `Server_SpawnPlayer` with
  `isReplay=true` registers the replay copy properly

### Step 3 — Pause/hide the real bot during replay
The original real bot must not run its AI or be visible while the replay copy
plays. Hook into `GameManager.OnGameStateChanged` in the existing `PhaseChangePatch`:

```csharp
// In PhaseChangePatch.Postfix:
if (newGameState.Phase == GamePhase.Replay)
{
    foreach (Player bot in FakePlayerRegistry.All)
    {
        var ai = bot.NetworkObject.GetComponent<GoalieAI>();
        if (ai != null) ai.enabled = false;
        // Optionally hide renderer too
    }
}
else if (oldGameState.Phase == GamePhase.Replay)
{
    foreach (Player bot in FakePlayerRegistry.All)
    {
        var ai = bot.NetworkObject.GetComponent<GoalieAI>();
        if (ai != null) ai.enabled = true;
    }
}
```

---

## Files to Touch

| File | Change |
|------|--------|
| `Patches/ReplayRecorderPatch.cs` | Add Prefix patches for `Event_Everyone_OnPlayerSpawned`, `Event_Everyone_OnPlayerBodySpawned`, `Event_Everyone_OnStickSpawned`; add Postfix on `Server_StartRecording` to inject fake player spawn events |
| `InitializePuckAI.cs` | Add replay phase detection in `PhaseChangePatch.Postfix` to pause/resume GoalieAI |
| `Utilities/BotSpawning.cs` | General cleanup (dead code, commented-out blocks); `SpawnChaser` and `SpawnFakePlayer` have very similar code that can share a helper |
| `GameModes/Goalies.cs` | Minor cleanup only |
| `Patches/ProcessChatCommands.cs` | Logic is tangled; `SpawnGoaliesBasedOffCommand` has inconsistent `GoaliesAreRunning` toggling |

---

## Key Classes & Methods (game source)

| Symbol | Location | Role |
|--------|----------|------|
| `ReplayRecorderController.Event_Everyone_OnPlayerSpawned` | `PuckNew/ReplayRecorderController.cs:42` | Captures player spawns into recording — the entry point for the bug |
| `ReplayRecorder.Server_StartRecording` | `PuckNew/ReplayRecorder.cs:33` | Takes initial player snapshot; fake players excluded by `GetPlayers` patch |
| `ReplayRecorder.Server_AddPlayerSpawnedEvent` | `PuckNew/ReplayRecorder.cs:154` | Writes a `PlayerSpawned` event at the current tick |
| `ReplayPlayer.Server_ReplayEvent` (case `"PlayerSpawned"`) | `PuckNew/ReplayPlayer.cs:443` | Calls `Server_SpawnPlayer(..., isReplay: true)` — this is what creates the duplicate |
| `ReplayPlayer.Server_DisposeReplayObjects` | `PuckNew/ReplayPlayer.cs:533` | Iterates `GetReplayPlayers()` and despawns them — cleanup step |
| `FakePlayerRegistry` | `Utilities/FakePlayerRegistry.cs` | Single source of truth for which players are bots |
| `BotSpawning.SpawnFakePlayer` | `Utilities/BotSpawning.cs:120` | Spawns the network object — triggers the recording events |

---

## Cleanup Notes (separate from bug fix)

The code has a lot of technical debt to address alongside the fix:

1. **Dead commented-out code** — `PlayerPatch.cs` is 100% commented out and can
   be deleted. `ReplayRecorderPatch.cs` has large commented-out blocks to remove.
   Same in `BotInputPatch.cs`.

2. **`SpawnChaser` vs `SpawnFakePlayer`** — 80% identical code. Extract shared
   skin/setup logic into a private helper.

3. **Inconsistent `GoaliesAreRunning` state** — it's set to `true` in
   `InitializePuckAI.OnEnable()` before any bots are spawned, making it unreliable
   as a "bots are actually running" guard. `ProcessChatCommands` reads this flag
   to decide whether to start or stop, so stale state causes wrong behavior.
   Fix: only set `GoaliesAreRunning = true` after at least one bot is successfully
   spawned; set `false` after all bots are confirmed despawned.

4. **`PhaseChangePatch` does nothing** — currently just logs and returns. Either
   give it real work (replay handling from Step 3 above) or remove it.

5. **`BotSpawning.redGoalie` / `blueGoalie` / `blueGoalieSpawned` / `redGoalieSpawned`**
   — declared but never used. Remove them.

6. **Early-return bug in `SpawnFakePlayer`** — the `IsCharacterPartiallySpawned`
   branch at line 195 returns early before registering the player in
   `FakePlayerRegistry` or setting up `GoalieAI`. If that branch ever triggers
   you get a bot with no AI. Verify this code path is actually reachable and fix.
