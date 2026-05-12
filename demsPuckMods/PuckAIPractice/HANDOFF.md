# PuckAIPractice — Handoff

## Status

**Duplicate-goalie bug:** Fixed and shipped 2026-05-12.
**Bot visibility in replay:** Trade-off accepted — bot is invisible in subsequent replays after the first one. Documented below as the next thing to fix when someone has time.

---

## What was wrong (the duplicate-goalie bug)

When a goal was scored, sometimes a duplicate goalie appeared mid-ice during the replay and persisted afterward. Root cause was a race in `Patches/ReplayRecorderPatch.cs`:

The 3 prefix patches that block bot spawn events from being captured into the replay recording (`BlockFakeBotPlayerSpawned` etc.) checked `FakePlayerRegistry.IsFake(player)` — which looks up the **Player object** in the registered set. But `BotSpawning.SpawnFakePlayer` calls `netObj.SpawnWithOwnership(fakeClientId, true)` (which fires the spawn event synchronously) *before* it calls `FakePlayerRegistry.Register(player, team)`. So at event time the Player wasn't yet registered → check returned false → event wasn't blocked → recorder captured the spawn → during replay, `Server_ReplayEvent("PlayerSpawned")` instantiated a duplicate bot.

## Fix that shipped

Switched the prefix patches to `FakePlayerRegistry.IsFakeClientId(player.OwnerClientId)`. The fake client IDs (`7777777`, `7777778`) are reserved via `ReserveFakeClientId` *before* the spawn happens, so the check correctly identifies bots at event time. Three patches updated:
- `BlockFakeBotPlayerSpawned`
- `BlockFakeBotPlayerBodySpawned`
- `BlockFakeBotStickSpawned`

All also got null-defensive guards (`player == null`, `playerBody.Player == null`, `stick.Player == null`).

Verified live on server3 (with workshop mod disabled, local DLL deployed). Duplicate no longer appears.

---

## Known remaining issue: invisible goalie in subsequent replays

### Symptom

First goal of a session: replay shows the goalie correctly.
Every subsequent goal's replay: no goalie visible at all (bot still functions in live gameplay between goals — just absent from the replay viewer).

### Root cause hypothesis

`ReplayRecorder_Server_StartRecording_Postfix` (Step 2 of the original fix plan) injects bot spawn events into the recording at start. But it conditionally injects body/stick:

```csharp
__instance.Server_AddPlayerSpawnedEvent(player);   // always
if (player.PlayerBody)                              // conditional
    __instance.Server_AddPlayerBodySpawnedEvent(player.PlayerBody);
if (player.Stick)                                   // conditional
    __instance.Server_AddStickSpawnedEvent(player.Stick);
```

Logs show `[ReplayRecorder] Replay recording started` and `Goalie AI Started` (from `PhaseChangePatch` re-enabling the AI) firing in the **same millisecond** at the boundary between replay-end and next-recording-start. If `PlayerBody` or `Stick` references are transiently null during that AI re-enable tick, only the bare `Player` event is injected — replay system has nothing to render, goalie appears missing.

First replay works because game start gives the bots a long warmup period before any replay happens, so the first recording-start catches them fully-formed.

### Suggested fix

Either:

**A) Defer injection until parts are ready.** Schedule a coroutine from the postfix that polls `player.PlayerBody`/`player.Stick` for a few ticks and injects events as they become non-null:

```csharp
static IEnumerator DeferredInject(ReplayRecorder rec, Player player)
{
    int ticks = 0;
    while (ticks++ < 30) // ~half second budget
    {
        if (player.PlayerBody && player.Stick)
        {
            rec.Server_AddPlayerBodySpawnedEvent(player.PlayerBody);
            rec.Server_AddStickSpawnedEvent(player.Stick);
            yield break;
        }
        yield return null;
    }
}
```

Drawback: needs a MonoBehaviour to host the coroutine, and "events injected late into a recording" may have its own ordering issues with the replay player.

**B) Hook PlayerBody/Stick spawn events for fake bots.** Since the BlockFake* prefixes already see every player/body/stick spawn event, add a *parallel* postfix that re-injects them when the spawn is for a fake bot AND a recording is active:

```csharp
[HarmonyPatch(typeof(ReplayRecorderController), "Event_Everyone_OnPlayerBodySpawned")]
static class InjectFakeBotPlayerBodySpawned
{
    static void Postfix(Dictionary<string, object> message)
    {
        var pb = (PlayerBody)message["playerBody"];
        if (pb == null || pb.Player == null) return;
        if (!FakePlayerRegistry.IsFakeClientId(pb.Player.OwnerClientId)) return;
        var rec = ReplayRecorder.Instance; // or however the recorder is reached
        if (rec != null && rec.IsRecording) rec.Server_AddPlayerBodySpawnedEvent(pb);
    }
}
```

Cleaner conceptually — "if a fake bot's body spawns while a recording is active, manually inject the event since our prefix blocked the controller from doing it." Same pattern for stick.

Option B is the recommendation. Avoids coroutines, uses the same hook points as the existing fix, and is symmetric with the prefix block patches.

### Trade-off if not fixed

Replays look like the goalie disappeared. Live gameplay is unaffected. Most replay viewers track the puck/scorer, not the goalie, so it's a minor cosmetic bug.

---

## Cleanup punch-list (separate from bug fixes)

Not blockers, but accumulated tech debt worth addressing on a slow day:

1. **Dead commented-out code.** `Patches/PlayerPatch.cs` is 100% commented and can be deleted. `InitializePuckAI.cs` has ~120 lines of commented Harmony patches (the SettingsManager skin-change loggers). `BotInputPatch.cs` likely has similar.

2. **`SpawnChaser` vs `SpawnFakePlayer`** in `Utilities/BotSpawning.cs` — ~80% identical. Extract shared mesh/skin/setup into a private helper.

3. **Stale `GoaliesAreRunning` flag.** Set to `true` in `InitializePuckAI.OnEnable()` before any bots are spawned, so it's an unreliable "are bots actually running" signal. `ProcessChatCommands` reads it to decide what to do — stale state causes wrong behavior. Fix: only set true after a successful spawn, set false after confirmed despawn.

4. **`PhaseChangePatch` does the right thing now (Step 3 fix)** — previously did nothing. Just confirming it's no longer dead code.

5. **Dead fields in `BotSpawning`:** `redGoalie`, `blueGoalie`, `blueGoalieSpawned`, `redGoalieSpawned` — declared, mostly unused. Compiler warned about these in the build output. Remove.

6. **Early-return bug in `SpawnFakePlayer`.** The `IsCharacterPartiallySpawned` branch (~line 195 in old version, may have moved) returns early before registering the player in `FakePlayerRegistry` or attaching `GoalieAI`. If reachable, you'd get a bot with no AI. Verify reachability and fix.

7. **`FakePlayerRegistry.CleanupDestroyed()` doesn't prune `fakeClientIds`** — only `fakePlayers` and `fakePlayerTeams`. Stale IDs accumulate. Tiny issue since IDs are fixed (7777777/7777778) and reused, but inconsistent.

---

## Key files

| File | Role |
|---|---|
| `InitializePuckAI.cs` | Mod entry point + `PhaseChangePatch` (Step 3 — pause AI during Replay) |
| `Patches/ReplayRecorderPatch.cs` | All 3 replay-related patches (block prefixes + start-recording postfix + per-tick movement record) |
| `Utilities/BotSpawning.cs` | `SpawnFakePlayer`, `SpawnChaser`, `Despawn`, `DetectOpenGoalAndSpawnBot` |
| `Utilities/FakePlayerRegistry.cs` | Single source of truth for which players are bots (object set + reserved client ID set) |
| `AI/GoalieAI.cs` | The actual goalie behavior MonoBehaviour |

## Game-side classes referenced

| Symbol | Source location | Role |
|---|---|---|
| `ReplayRecorderController.Event_Everyone_OnPlayerSpawned` | `PuckNew/ReplayRecorderController.cs` | Captures player spawns into the recording |
| `ReplayRecorder.Server_StartRecording` | `PuckNew/ReplayRecorder.cs` | Starts a new recording, takes initial snapshot |
| `ReplayRecorder.Server_AddPlayerSpawnedEvent` etc. | `PuckNew/ReplayRecorder.cs` | Manually inject events into the active recording |
| `ReplayPlayer.Server_ReplayEvent` (case `"PlayerSpawned"`) | `PuckNew/ReplayPlayer.cs` | Spawns replay copies based on recorded events — was the source of the duplicate |
| `ReplayPlayer.Server_DisposeReplayObjects` | `PuckNew/ReplayPlayer.cs` | Cleanup pass after replay ends |
