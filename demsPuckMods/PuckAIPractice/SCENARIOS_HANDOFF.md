# Scenarios — Handoff

## Status (2026-05-14)

The scenario system is built and working. First concrete scenario `rush` ships. Bot appearance infrastructure (jersey inheritance + randomization + client-side robot look) is in place across all spawners (defender, chaser, goalie). Keybind restart works via the Input System.

Branch: `tm/defenderBot`. Built and deployed locally; not pushed.

**Next:** Gauntlet — the original SCENARIOS doc's pitch (remove the arena, bots fly down at you in sequence) is unbuilt and is the user's stated next task.

## Chat commands and keybinds

| Trigger | Effect |
|---|---|
| `/scenario rush` | Start the rush scenario for the caller (1v5 from own goalie) |
| `/scenario stop` | Tear down the active scenario (despawns scenario-owned bots) |
| `/sr` | Restart the last scenario for the caller |
| `G` key (default) | Same as `/sr` — host-side only. Configurable via `ScenarioRestartKey`. |

## File layout

All under `demsPuckMods/PuckAIPractice/`:

| File | Role |
|---|---|
| `Scenarios/IScenario.cs` | Lifecycle interface: `Name`, `Start(callerClientId)`, `Stop()` |
| `Scenarios/ScenarioManager.cs` | Static singleton. Owns the active `IScenario`, remembers `LastScenarioName` + `LastCallerClientId` for `/sr`. `StartByName`, `Restart`, `Stop`. |
| `Scenarios/ScenarioCommandPatch.cs` | Harmony prefix on `VoteManagerController.Event_Server_OnChatCommand` parsing `/scenario` and `/sr`. |
| `Scenarios/RushScenario.cs` | First concrete scenario. |
| `Scenarios/ScenarioInputListener.cs` | Persistent MonoBehaviour created in `InitializePuckAI.OnEnable`. Polls `Keyboard.current[key].wasPressedThisFrame` and fires `Restart`. Gated on `IsServer`. |
| `Utilities/BotCustomization.cs` | `BuildFromSettings()` + `BuildRandom(Player)`. Used by all three spawner paths. |
| `Patches/BotRobotLookPatch.cs` | Harmony postfix on `PlayerBody.ApplyCustomizations`. Client-side robot look for any player with a bot clientId. |
| `Singletons/ConfigData.cs` | Schema migration in `Load()` + new fields `ScenarioRestartKey` and `RandomizeBotAppearance`. |

Csproj entries added for everything new.

## Architecture: scenario lifecycle

```
/scenario rush
        |
        v
ScenarioCommandPatch.Prefix
        |
        v
ScenarioManager.StartByName("rush", callerId)
        | - Stop() any active scenario
        | - new RushScenario()
        | - LastScenarioName = "rush"; LastCallerClientId = callerId
        v
RushScenario.Start(callerId)
        | - validate caller (team must be Red or Blue, must be spawned)
        | - clear chaser + defender bots (scenario owns the population)
        | - despawn all pucks
        | - find caller's team goalie (PlayerPosition.Role == Goalie)
        | - compute attack direction (sign(goalieZ) → backward → -that is forward)
        | - teleport caller via PlayerBody.Server_Teleport
        | - spawn puck 1.5m in front
        | - spawn 5 defenders on opposite team
        | - teleport each defender to its DefenderAI.StaticHome facing ThreatDirection
```

`Stop()` despawns scenario-owned bots. Pucks and the player's last position are left as-is (scenario doesn't try to undo state — the next scenario start or a real game phase resets things).

### Why one active scenario, not composable

Simpler. `/scenario rush` while a different scenario is running cleanly stops the old one first. The "5 defenders own the bot population" model assumes one set of scripted opponents at a time. Revisit if multi-scenario composition becomes a real need.

### Restart semantics

`/sr` (and `G`) re-runs the last scenario with **whoever issued the restart** as the new caller — not the original caller. That way any host can pick up the active scenario without state being locked to one Steam ID. Per-player "last scenario" memory would matter only in multi-host shared sessions; not the typical practice-mode case.

## RushScenario behavior

| Step | Constant | Value | Why |
|---|---|---|---|
| Player teleport offset from goalie | `PlayerSpawnAheadOfGoalie` | 2.5m | Just enough room to settle before the first defender's zone |
| Puck offset from player | `PuckSpawnAheadOfPlayer` | 1.5m | At stick reach when player rolls forward |
| Defender positions | — | LD, RD, C, LW, RW | Full defensive shell |

The defenders are spawned via the existing `DefenderSpawner.Spawn` (so they inherit per-position `ZoneRadius`, pull-back, tuck-in defaults from `DefenderSpawner`). After spawn, each is teleported to its own `DefenderAI.StaticHome` so the formation reads from the moment scenario starts — no walk-out from the faceoff spot.

Rink-axis assumption is shared with `DefenderSpawner`: world X is lateral, world Z is rink length, centerline at 0. `sign(goalieWorld.z)` gives the caller's team's "back" direction; attack direction is the negation.

## Bot appearance pipeline

Three independent pieces, applied in order:

### 1. `BotCustomization.BuildFromSettings()` (pre-spawn)

Replaces the spawner's old `new PlayerCustomizationState()` (which is an all-zero struct → `Player.GetPlayerJerseyID()` returns 0 → `SetJerseyID(0, team)` paints a blank jersey). Now populates from `SettingsManager.*` (the host's customization choices), the same shape `ServerManager.cs:209` and `ConnectionManager.cs:67` use for connecting clients. So bots show colored jerseys even without randomization.

Applied to all three spawn paths: `DefenderSpawner`, `ChaserSpawner`, `BotSpawning` (goalie).

### 2. `BotCustomization.BuildRandom(Player)` (post-spawn, after `Server_SpawnCharacter`)

Reads the available option lists off the bot's already-spawned mesh and rolls a random ID per category. Gated on `ConfigData.RandomizeBotAppearance` (default true).

Categories enumerated:

| Category | Source list | Filter |
|---|---|---|
| Headgear (incl. visor) | `PlayerMesh.PlayerHead.headgear` (private, Traverse) | `IsForRole(role)` via `AccessTools.Method` |
| Flag | `PlayerMesh.PlayerHead.flags` | none |
| Mustache | `PlayerMesh.PlayerHead.mustaches` | none |
| Beard | `PlayerMesh.PlayerHead.beards` | none |
| Jersey | `PlayerMesh.PlayerTorso.jerseys` | `IsForTeam(team)` |
| Stick skin | `Stick.StickMesh.skins` | `IsForTeam(team)` |
| Shaft tape | `Stick.StickMesh.shaftTapes` | none |
| Blade tape | `Stick.StickMesh.bladeTapes` | none |

Writes the random ID into the bot's actual team+role slot in `PlayerCustomizationState` (e.g. only `JerseyIDBlueAttacker` is set for a Blue Attacker bot — the other three jersey slots stay at the SettingsManager defaults since they're never read by `Player.GetPlayerJerseyID()`).

**Each bot rolls independently.** Per-roll variance is the point — a row of 5 defenders should read as 5 distinct skaters.

### 3. `BotRobotLookPatch` (client-side, after `PlayerBody.ApplyCustomizations`)

Harmony postfix on `PlayerBody.ApplyCustomizations`. For any `PlayerBody` whose `Player.OwnerClientId` is in the bot range (7M–10M), walks the full `PlayerMesh` renderer hierarchy and pattern-matches material names:

- `"eye"` → red `(0.95, 0.08, 0.08)`, also sets `_EmissionColor` + enables `_EMISSION` keyword if the shader supports it
- `"skin"`/`"face"`/`"head"`/`"body"` with a denylist (`hair`/`beard`/`jersey`/`stick`/`tape`/`helmet`/`hat`/`visor`/`eye`) → grey `(0.62, 0.64, 0.68)`

**No network sync.** Each client that has the mod installed applies the look locally. Vanilla clients see whatever jersey/skin the bot inherited from the host's `SettingsManager`. The robot identity is a local visual cue, not a synced game-state attribute.

#### Detection

ClientId range, not name. Real Steam clientIds are 64-bit (~7.6e16). Our bot IDs sit at 7_777_777 (goalies), 8_000_000+ (chasers), 9_000_000+ (defenders). `clientId >= 7_000_000 && clientId < 10_000_000` covers all three families with no risk of collision.

#### Discovery logging

Every distinct material name seen on a bot's body is logged once via `Debug.Log("[BotRobotLook] discovery: material '<name>' on '<renderer>'")`. If a particular skin or eye renderer is missed by the pattern match (e.g. the actual material name is `M_PlayerHead_Skin_01` and our pattern needs adjustment), the log lines tell us exactly which names to add.

If we ever want exact-name matching instead of pattern matching, replace the `Contains` checks with a `HashSet<string>` populated from those discovered names.

## Config schema and migration

`ConfigData.Load()` deserializes the JSON, then runs `MigrateSchemaIfNeeded` which:

1. Parses the raw JSON as a `JObject`
2. Compares its top-level keys to the public instance properties of `ConfigData` via reflection
3. If any are missing, calls `Save()` — which re-serializes `_instance` (already filled with the C# property defaults for the missing fields)

**Cost**: comments in the existing config file are lost on rewrite. Newtonsoft.Json doesn't preserve `//` comments through a parse/serialize round-trip. If schema-stability becomes more important than comment preservation, that's a trade-off worth revisiting (e.g. text-level insertion only of missing keys).

New fields this session:

| Property | Default | Notes |
|---|---|---|
| `ScenarioRestartKey` | `Key.G` | `UnityEngine.InputSystem.Key`, JSON-serialized as enum name ("G", "Escape", etc.) |
| `RandomizeBotAppearance` | `true` | False makes bots inherit host SettingsManager unchanged |

## Input System gotcha (load-bearing for future keybinds)

**Puck runs `UnityEngine.InputSystem` exclusively.** Legacy `UnityEngine.Input.GetKeyDown` returns false always — and per the comment in `ServerBrowserInGame/PauseMenuServerBrowserInjector.cs:89-91`, it floods logs when called.

Correct pattern for any future keybind:

```csharp
using UnityEngine.InputSystem;

var keyboard = Keyboard.current;
if (keyboard == null) return;
var control = keyboard[key];  // key is UnityEngine.InputSystem.Key, NOT UnityEngine.KeyCode
if (control != null && control.wasPressedThisFrame) { /* fire */ }
```

Store keybind config as `UnityEngine.InputSystem.Key`, not `KeyCode`. JSON serialization with `StringEnumConverter` works the same way for both since the named members overlap (`G`, `Escape`, `F1`, etc.).

## Bugs we hit this session

1. **`G` key did nothing.** First implementation used `Input.GetKeyDown(KeyCode.G)`. Puck has the new Input System package only — legacy Input is dead in this process. Fix: switched to `Keyboard.current[Key].wasPressedThisFrame` and changed the config field's type from `KeyCode` to `UnityEngine.InputSystem.Key`.

2. **Defender bots had blank/white jerseys.** Initial hypothesis was the `ApplyDefaultAppearance` block (which set every ID to `0`). Removing it didn't fix the bots — they were still blank. The actual root cause was the spawner doing `player.CustomizationState.Value = new PlayerCustomizationState();` which initializes the struct to all-zero ints. `Player.GetPlayerJerseyID()` then returns 0, and `PlayerBody.ApplyCustomizations` (the game's own pipeline, line 401) paints `SetJerseyID(0, team)` = blank. The manual `SetJerseyID(0, ...)` in `ApplyDefaultAppearance` was just a redundant duplicate of what the game was already doing. Fix: populate `PlayerCustomizationState` from `SettingsManager` (`BotCustomization.BuildFromSettings`) before the body spawns, so the game's normal pipeline applies real IDs. The old `ApplyDefaultAppearance` blocks were deleted from `DefenderSpawner`, `ChaserSpawner`, and `BotSpawning` (both paths) — they were either redundant or actively wrong.

3. **Bot appearance reset on customization events.** `PlayerBody.ApplyCustomizations` is called from `PlayerBodyController` event subscriptions on every team/role/username/number/CustomizationState change. Our `BotRobotLookPatch` runs as a Postfix on each, re-applying the robot look so it survives changes. No caching, intentional — the events are rare and the cost is small.

4. **Newcomer joining mid-scenario sees vanilla bots momentarily.** The robot look is applied on the joining client when `PlayerBody.OnPlayerReferenceChanged` fires (which calls ApplyCustomizations). For an already-spawned bot, this fires when the joining client's network layer instantiates the remote body. So the bot will be vanilla for a few frames during spawn, then robot. Acceptable.

## Insight worth keeping

**The bot population is identifiable by clientId range, not by name.** This becomes load-bearing the moment you want any client-side patch that targets bots: `BotRobotLookPatch` is the first user, but any future "label bots in scoreboard," "show debug overlay above bots," etc. should use the same detection. Centralize the range check if a second user appears.

The three registries (`FakePlayerRegistry` for goalies at 7777777/8, `ChaserRegistry` at 8M+, `DefenderRegistry` at 9M+) intentionally sit in distinct decimal-million bands so cross-cutting filters can be range-based without consulting any registry. Keep the bands clear if you add more bot types.

## Next: Gauntlet scenario

The user's stated next task. Original pitch (from the pre-build SCENARIOS doc): simulate end-to-end skating against successive defenders. Rink is cleared down to a long flat sheet; player skates from one end; bots spawn ahead in sequence and despawn behind.

### Player experience
1. Player runs `/scenario gauntlet` (probably).
2. Rink walls / faceoff structure / blue lines get cleared. (Exact form TBD — see "Open questions.")
3. Player gets a puck and starts at one end.
4. One bot spawns some distance ahead, facing them.
5. Player skates past the bot. Once the bot is N meters behind, despawn it.
6. After M meters of forward progress, spawn the next bot ahead.
7. Continues until `/scenario stop` or some end condition.

### Knobs (initial guesses)

| Knob | Initial guess | Notes |
|---|---|---|
| Bot spawn distance ahead of player | ~12m | Far enough to see them coming |
| Forward-progress threshold per new spawn | ~8m | One beat ≈ one rep |
| "Passed" threshold | ~3m behind player | When bot is this far behind, count beaten and despawn |
| Max simultaneous bots | 1 (initially) → 2-3 (advanced) | Start simple |
| Bot type | Chaser (probably) | Built to engage a moving target |

### What now exists that the original plan assumed unbuilt

The original SCENARIOS doc was written when no scenario infrastructure existed. Now:

- **`IScenario` + `ScenarioManager`** are in place. Gauntlet just implements `IScenario`. Add a case to `ScenarioManager.Create("GAUNTLET")`.
- **Bot spawn/despawn cadence** is proven — `ChaserSpawner.Spawn` and `DespawnAt` work cleanly. The "every few seconds" worry from the original plan is no longer hypothetical; the chaser bot has been spawn/despawned in shared sessions without leaks.
- **Player position tracking** — `PlayerManager.Instance.GetPlayerByClientId(callerId).PlayerBody.transform.position` per frame. Trivial.
- **Bot appearance** — defender/chaser bots are already styled (jersey, randomized, robot look client-side). Gauntlet bots will inherit all of this for free.
- **Restart pattern** — `/sr` + `G` already works. A new spawned bot list inside gauntlet just needs to be cleaned up in `Stop()` and re-created on `Restart()`.

What gauntlet needs that doesn't exist yet:

- **Environment modification.** The big unknown. Need to find the GameObject hierarchy of the rink boards, faceoff dots, blue/red lines and figure out what's safe to `SetActive(false)`. Specifically: does puck physics still work with no boards? Where does the puck fall — into a void, or onto the ice surface plane?
- **A `Tick(dt)` on `IScenario`.** Gauntlet is the first scenario that isn't a one-shot setup — it has continuous per-frame logic (track player position, check "passed" threshold, spawn next bot at progress milestones). Add `void Tick(float dt)` to the interface and have `ScenarioManager` call it from a MonoBehaviour `Update()`. The existing `ScenarioInputListener` MonoBehaviour could grow this responsibility, or add a separate `ScenarioTicker`.
- **Forward-axis convention.** Player picks an axis to skate along (likely +Z from one end). Need to pick: fixed +Z, or detect from player facing at scenario start? `RushScenario`'s convention (attack direction from `sign(goalieZ)`) is a reasonable starting point.

### Likely shape

```
Scenarios/
  IScenario.cs                — add void Tick(float dt) { } default
  ScenarioManager.cs          — add a ticker MonoBehaviour, call Active?.Tick
  GauntletScenario.cs         — new
  Environment/
    RinkModifier.cs           — helpers to disable/enable rink elements safely
```

### Open exploration before writing code

1. Identify the rink GameObject hierarchy (boards, ice surface, lines, faceoff dots, nets). Note what can be disabled without breaking physics. The decompiled `Puck/` and `PuckNew/` projects don't show prefab hierarchies — this is a runtime discovery task. A diagnostic patch that logs the scene root's GameObject tree at game start would help.
2. Decide on the play space. Likely simplest: don't modify the rink at all, just spawn bots on a forward line within the existing ice surface. Sidesteps the "what breaks if we hide the boards" problem.
3. Decide on bot AI for the gauntlet. Chaser is the obvious starting point — built to engage a moving target — but a new "intercept-on-track" AI might be cleaner since the player has a fixed direction. Start with chaser and tune from there.

## Future work beyond gauntlet

- **Reset to start positions**: `/scenario reset` to reset all bots and player without changing the scenario type. Useful for quick repetitions inside `rush`.
- **`/scenario list`**: enumerate available scenarios for the user. Trivial — `ScenarioManager.Create` returns null for unknown names today; add a static list.
- **Difficulty scaling**: per-scenario knobs (defender ZoneRadius scaling, chaser PursuitSpeed, etc.) loaded from config.
- **Replay compatibility**: same gap as the rest of the bot work — scenarios' bots aren't covered by the existing replay-recorder bot-aware filters.
- **Skin tone / eye color customization for the human player.** Investigated this session and deferred. Not in `PlayerCustomizationState`, not in `AppearanceSubcategory`. Would need a new server-to-client sync (custom NetworkVariable + a client-side material setter patch) plus our own palette. Distinct from the bot-only robot look (which is client-side-only).
