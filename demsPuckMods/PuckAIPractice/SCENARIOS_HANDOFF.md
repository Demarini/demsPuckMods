# Scenarios — Handoff (next task)

## Status (2026-05-14)

Bot AI work is wrapped for now:
- **Chaser bot** — `/chaser <pos>`, lead pursuit with cut detection, full tuning passed. See `CHASER_HANDOFF.md`.
- **Defender bot** — `/defender <pos>` and `/defender all`, zone-based with facing-expanded entry, latched exit, pivot phase, lead pursuit, gated stick aim, dynamic debug cross. See `DEFENDER_HANDOFF.md`.

Both are battle-tested in shared sessions, deployed locally, not pushed. Branch: `tm/defenderBot`.

**Next:** scenario system. The chaser and defender are *components*. Scenarios are *compositions* of those components plus environmental and progression rules.

## What is a "scenario"

A scenario is a scripted training/practice setup that goes beyond "spawn a bot here." A scenario can:

- **Modify the environment** — disable parts of the rink (boards, blue lines, faceoff dots), replace with a different play space, change boundaries.
- **Spawn bots dynamically** — in response to player position, time, or other triggers. Not a one-shot place-and-leave.
- **React to player progress** — track distance traveled, bots beaten, time elapsed, etc.
- **Have a lifecycle** — start, tick, end. Cleans up its own state when stopped.

Think of `/chaser LW` as a tool you pick up; `/scenario gauntlet` as a full level you load.

## First scenario: Gauntlet

**Pitch:** simulate skating end-to-end while stickhandling around opponents in succession.

### Player experience
1. Player runs `/scenario gauntlet`.
2. Rink (the "hangar") gets cleared down to a long flat sheet — no boards interrupting the line, no faceoff structure mid-ice. (Exact form TBD — see "Open questions.")
3. Player starts at one end with a puck.
4. A single bot spawns some distance ahead, facing them. Acts as a defender/chaser hybrid — tries to body up and steal.
5. Player skates by the bot (gets *past* it in the forward direction).
6. As soon as the bot is behind the player by some threshold, it despawns and queues for respawn.
7. Every N meters of forward progress, a new bot spawns ahead of the player.
8. Continues until the player turns around / runs `/scenario stop` / hits some end condition.

**Why this matters:** practices the specific in-game skill of beating defenders consecutively. The chaser alone gives you one rep at a time; the gauntlet stacks reps.

### Knobs (to expose, tune empirically)
| Knob | Initial guess | Notes |
|---|---|---|
| Bot spawn distance ahead of player | ~12m | Far enough that the player sees them coming, close enough that one beat doesn't last forever |
| Forward-progress threshold per new spawn | ~8m | After beating one, next one shows up after ~8m of further skating |
| "Passed" threshold | ~3m behind player | When bot is this far behind, count as beaten and despawn |
| Max simultaneous bots | 1 (initially) → 2-3 (advanced) | Single bot at first; revisit when basic works |
| Bot type | Defender (zone-style) or Chaser (pursuit) | Probably chaser-style — they're built to engage a moving target |

### Open questions for the gauntlet
- **Bot AI**: chaser-style pursuit, defender-style zone, or a new "intercept-the-player-on-a-track" AI? My guess: start with chaser (it's purpose-built to engage a moving target), see how it feels.
- **Player movement constraint**: do we force the player into a corridor, or let them roam on a flat sheet? Corridor is simpler; flat sheet is more like real ice.
- **Direction of play**: assume one fixed direction (e.g., +Z) or detect player's intended direction at scenario start?
- **End condition**: time-based, distance-based, manual stop, or "first time you lose the puck"?

## Architectural sketch

Proposed shape — open to revision:

```
PuckAIPractice/
  Scenarios/
    IScenario.cs               — Start() / Stop() / Tick(dt) lifecycle
    ScenarioManager.cs         — Singleton; owns the active IScenario
    ScenarioCommandPatch.cs    — Harmony prefix on Event_Server_OnChatCommand for /scenario
    GauntletScenario.cs        — First concrete scenario
    Environment/
      RinkModifier.cs          — Helpers to disable/enable rink elements
```

### Key design decisions to make early
- **One active scenario at a time, or composable?** Easier to start with one. Composable is overkill for v1.
- **How scenarios interact with existing bots** — should `/scenario gauntlet` clear out any `/defender`/`/chaser` bots first? Probably yes; scenarios "own" the bot population while active.
- **Server-only vs client visibility** — scenarios mutate the server world. Clients see it via the existing networking. No client-side scenario code required (consistent with how the bots work).
- **Reusable building blocks** — eventually a "Wave" helper (spawn bot every N meters), a "Boundary" helper (clamp player position to a region), a "PuckSpawner" helper. v1 of gauntlet can be monolithic and we extract building blocks once a second scenario reveals what's common.

### Lifecycle sketch
```csharp
public interface IScenario
{
    string Name { get; }
    void Start(ulong callerClientId);  // claim env, spawn initial state
    void Tick(float dt);               // per-frame logic (called by ScenarioManager.Update)
    void Stop();                       // restore env, despawn everything
}
```

`ScenarioManager` would be a `MonoBehaviour` attached to a persistent GameObject (created on first `/scenario` command), holding the active `IScenario` and ticking it in `Update`. `Stop()` runs on `/scenario stop` and also on `OnDisable` of the mod itself.

## Open exploration needed (do this first)

These should be the first session's work, before writing any scenario code:

1. **Find what makes up the "hangar."** In the decompiled `Puck` project, identify the GameObject hierarchy of the rink: boards, ice surface, blue/red/center lines, faceoff dots, nets. Note which can be disabled (SetActive(false)) without breaking the game's networking / physics. Specifically check whether the puck physics still work with no boards (puck falls into the void?).
2. **Decide on the play space.** Long corridor on the existing ice? Whole new procedurally-generated sheet? Or just "ignore boards, let player skate wherever, spawn bots on a line"? Probably the last option — simplest, sidesteps the "modify rink" problem entirely.
3. **Test bot spawn-and-despawn cadence.** The chaser/defender spawners do server-side instantiation cleanly. Confirm that respawning a bot at high frequency (one every few seconds) doesn't accumulate NetworkObject leaks or client-side glitches.
4. **Player position tracking.** What player do we follow as "the player"? The caller of `/scenario gauntlet` is the obvious answer (same pattern as `/chaser`). Confirm we can read their position each frame to drive "every N meters of progress."
5. **"Passed" detection.** Simplest: compare bot position to player position projected onto a chosen forward axis. If bot's projected position is behind the player's by N, count as passed. Fits a corridor model well.

## Reference points in the existing code

| What | Where | Why useful |
|---|---|---|
| Server-only chat command hook | `Defender/DefenderCommandPatch.cs`, `Chaser/ChaserCommandPatch.cs` | Template for `/scenario` parsing |
| Bot spawn pattern | `Defender/DefenderSpawner.cs` | The whole `Spawn / SpawnWithOwnership / Server_SpawnCharacter` ritual |
| Separate registry pattern | `Defender/DefenderRegistry.cs` | ClientId-range strategy. Scenario bots probably want their own range (10M+?) so they don't collide with defender/chaser bots |
| Per-frame AI loop | `Defender/DefenderAI.cs` `Update()` | The MonoBehaviour-on-bot pattern. Scenario AI might reuse `ChaserAI` directly or compose a new one |
| `PracticeModeDetector` | `Patches/PracticeModeDetector.cs` | Currently bypassed for /chaser and /defender. Scenarios may want to require practice mode or be admin-gated |
| Puck rink dims (from memory) | rink ~91.5m × 45m | Helps size the corridor / spawn distances |

## Carryover considerations

- **Replay recording is still a known gap.** None of the bot work is covered by `ReplayRecorderPatch`'s bot-aware filters (those only check `FakePlayerRegistry.IsFakeClientId`). If scenarios run during goal-recordable play, the recorder will write events with clientIds no real client owns. Behavior unverified. Likely fine since scenarios are practice-mode-adjacent, but worth noting.
- **Steam Workshop publishing**: the existing `publish.ps1` flow doesn't need any scenario-specific work — scenarios are just more code in `PuckAIPractice`. Same DLL, same workshop entry.

## What "done" looks like for v1 of Gauntlet

- `/scenario gauntlet` starts a session for the caller.
- `/scenario stop` cleanly tears down (no leaked bots, no leftover environment changes).
- One bot at a time, spawning ahead, despawning when passed, new one after N meters of forward progress.
- Player can run end-to-end of a long corridor and face ~5-10 bots in a single session.
- All knobs exposed on the scenario component for in-Inspector / config-file tuning later.

Stretch (deferred):
- Multiple bot types mixed (some chaser, some defender behavior).
- Difficulty progression (later bots are faster / better-tuned).
- Score/stats overlay.
- `/scenario list` and other scenario types (1v1 face-off drill, defender-line breakthrough, etc.).
