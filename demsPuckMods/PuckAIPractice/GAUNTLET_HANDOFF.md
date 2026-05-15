# Gauntlet Scenario — Handoff

## Status (2026-05-15)

Two scenarios share the same code: `/scenario gauntlet [rate]` for casual practice and `/scenario gauntletChallenge [rate]` for run-or-die mode. Both hide the rink and drop the player into a 20m × 2000m walled corridor with a fresh puck; the challenge variant adds a red pressure wall behind the player and ends the run on contact.

Branch: `tm/defenderBot`. Built and deployed locally; not pushed.

Paired with `CHASER_HANDOFF.md` (the chaser AI is the bot type used). `SCENARIOS_HANDOFF.md` covers the scenario framework, restart keybind, and bot-appearance pipeline — all of which gauntlet inherits.

## Chat commands

| Command | Effect |
|---|---|
| `/scenario gauntlet` | Empty corridor, no bots, no wall. Free stickhandle practice. |
| `/scenario gauntlet <rate>` | Chaser spawns every `<rate>` meters of player forward Z. No wall — casual. |
| `/scenario gauntletChallenge <rate>` | Same bot cadence + red pressure wall closing in from behind. Contact with the wall (player or puck) ends the run. |
| `/scenario stop` | Restores the rink, despawns bots, hides HUD. |
| `/sr` or `G` (default key) | Restart whichever variant was last started. Honors the original rate. |

`rate = 0` (or omitted) in either variant means no bots and no wall — same as `/scenario gauntlet` plain.

## File layout

All under `demsPuckMods/PuckAIPractice/`:

| File | Role |
|---|---|
| `Scenarios/GauntletScenario.cs` | The scenario itself. Manages bot pool, pressure wall, loss handling, recycle math. |
| `Scenarios/GauntletHud.cs` | `MonoBehaviour` with `OnGUI` for the top-left "Passed / Wall" stats and the centered "CAUGHT! Final: N" banner. Created in `InitializePuckAI.OnEnable`. |
| `Scenarios/Environment/RinkVoid.cs` | Hides the rink hierarchy and builds the 20m × 2000m × 4m corridor (floor, 4 visible walls, 4 invisible tall containment walls). Samples Hangar materials for ground/wall textures. |
| `Utilities/BotTopplePreventer.cs` | Component attached to spawned chasers. Clears `HasFallen`/`HasSlipped` state, clamps `KeepUpright.Balance` to a floor, emergency rotation lock past ~70° tilt. |
| `Chaser/ChaserSpawner.cs` | Added `SpawnAtWorld(...)` — chaser spawn at arbitrary world position, bypasses PlayerPosition claiming. |
| `Scenarios/IScenario.cs` | Added `Tick(float dt)`. |
| `Scenarios/ScenarioManager.cs` | Added `Tick(dt)` dispatch and the `GAUNTLETCHALLENGE` case. |
| `Scenarios/ScenarioInputListener.cs` | Now drives `ScenarioManager.Tick` every frame. Disconnect detection (IsServer transition + `sceneUnloaded`) stops any active scenario so state can't leak across reconnect. |
| `Utilities/BotSpawning.cs` | Added `SuppressAutoGoalieSpawn` flag so the every-10-frame `DetectOpenGoalAndSpawnBot` respawner can't refill the goalies during gauntlet. |

## Architecture

### Scenario lifecycle: Tick

`IScenario.Start` runs once at command time; `Tick(dt)` runs every frame on the server while the scenario is active; `Stop` runs when the scenario ends (manual, replaced, or reset). `ScenarioInputListener.Update` is the ticker host — same `MonoBehaviour` that handles the `G` keybind.

The listener also watches `NetworkManager.IsServer` for a true→false transition and subscribes to `SceneManager.sceneUnloaded`. Either path calls `ScenarioManager.Stop()`, so when the host disconnects from a multiplayer practice and reconnects, the wall doesn't keep ticking in the background and catch the player at origin a few seconds in.

### RinkVoid: the corridor

Three things happen at scenario start:

1. **Sample two Hangar materials before disabling anything.** Walk `Hangar`'s descendant renderers, take the first two URP materials that have a `_BaseMap` texture. Cloned and reused on the corridor pieces with per-slab tiling so they don't stretch. Sampling order is `(first=walls, second=floor)` — the first material's long parallel lines were disorienting on the floor.

2. **Disable scenery.** Two passes:
   - Inside `Level Default`: name-match against `Rink`, `Hangar`, `Goal Blue|Red`, `Sounds`, `Reflection Probe`, `Spectator Booth N`, `Scoreboard *`, etc. `HardKeepPatterns` (`^Lights$`, `Camera`, `Post Processing`) prevent collateral damage if a name accidentally matches.
   - Outside `Level Default`: exact-match for known runtime scenery roots, currently just `Spectator Manager` — that root spawns `Spectator(Clone)` NPCs at runtime; killing the root inactivates them all.

   Disabled GameObjects are tracked in `_disabled` so `Exit()` re-enables exactly the ones we touched (anything that was already inactive when we got there stays inactive).

3. **Build the arena.** A `BuildSlab` helper creates each cube primitive with cloned material + per-slab tiling. The arena:

   | Piece | Position | Scale | Notes |
   |---|---|---|---|
   | Floor | center Z=950, y=iceY−0.5 | 20 × 1 × 2000 | Top face at `iceY`. Layer `Ice`. |
   | Left/Right walls | X=±10.5, full Z | 1 × 4 × 2000 | 4m visible. Layer `Ice`. |
   | Back/Front walls | Z=−50.5 / Z=1950.5 | 22 × 4 × 1 | Cap the corridor. |
   | Containment ×4 | same X/Z as visible walls | thickness × 200 × length | Invisible (renderer disabled). Stops the puck from arcing over the 4m visible walls on deflections. |

   Floor texture tiles every 4m; walls also every 4m. The `FloorTileMeters` / `WallTileMeters` consts are separated so they can be tuned independently — earlier the floor was 16m before we settled on 4m.

### Bot spawning & recycling

`ChaserSpawner.SpawnAtWorld(team, pos, rot, targetClientId, label)` bypasses the PlayerPosition claim that `ChaserSpawner.Spawn` does, registering the bot under the synthetic label (`Gauntlet1`, `Gauntlet2`, …) so the existing despawn path still works.

Gauntlet maintains a `BotEntry[]` pool with `Active` flags. Per tick:

- **Pass detection.** When a bot's `Z + 5m < playerZ`, send it to standby and increment `PassedBots`. In challenge mode the wall snaps to `playerZ − 10m` on any pass — that's the breathing room each pass buys.
- **Wall sweep.** Any active bot whose `Z < wallFrontZ` gets sent to standby with no pass credit. Without this, the wall pushes a passed-but-not-yet-recycled bot forward and they end up perpetually sitting on the player's back.
- **Spawn.** When `playerZ >= _nextSpawnZ`, pick a spawn X (see below), reactivate a standby bot if one exists, otherwise create a fresh one via `SpawnAtWorld`. Bot spawns 50m ahead so it has runway to build chase momentum.

**Standby position** is `(varied X, y=500, z=ForwardLength+200)` — far past the front wall, way up in the air. The bot falls forever (no floor out there) with no traction, so even though its AI keeps ticking it can't affect play. `Server_Teleport` zeros velocities each time, and `PlayerBody.OnStandUp()` is called on every reactivate to clear any `HasFallen` carried over from contact in the previous active cycle.

**Spawn X bias.** The bot lands toward whichever side the player has more open lane, with a minimum 3m horizontal separation from the previous spawn to break "all bots in one column" cheese:

| Player X | Bot spawn X |
|---|---|
| Left of center | `playerX + Random(2..6)` (to the right) |
| Right of center | `playerX − Random(2..6)` (to the left) |
| Roughly centered | `Random(−8..+8)` |

Final X is clamped to `[−8, +8]` (2m margin off each wall).

### Pressure wall + loss

Challenge mode only. Wall starts 20m behind the player, full corridor width, 200m tall (so puck can't sail over it either), bright red. Advances at `5 m/s`. On any bot pass it snaps to `playerZ − 7m`.

**Loss check** uses the wall's front face: `wallFrontZ = _wallZ + WallThickness/2`. If `wallFrontZ ≥ playerZ − 0.5m` OR any active puck's Z is within that same range, `OnLoss` fires. Center-of-wall was the obvious comparison but it never tripped because the wall physically pushes the player forward, keeping `playerZ` ~1m ahead of `_wallZ` indefinitely — only the front face actually closes on the player.

**OnLoss** captures `LastFinalScore = PassedBots` for the HUD banner, then resets: counter to 0, all active bots to standby, player teleported to start, puck respawned, wall to start position. The HUD shows "CAUGHT!" + "Final: N" for 2 seconds (`LossBannerSeconds`).

### BotTopplePreventer

Component attached automatically by `SpawnAtWorld`. Every `LateUpdate`:

1. **`HasFallen = false`, `HasSlipped = false`** — clears the binary fallen state every frame. Even if a collision briefly flips these, they're cleared before the game's "lying on the ice" animation can latch.
2. **`KeepUpright.Balance` floored at `BalanceFloor` (1.0 currently)** — keeps the upright force at maximum. The earlier 0.5 floor allowed slide-lean room but also let the body hover-collapse and pogo on hard hits. With Balance=1 the bots still slide fine (defenders run the game's default which is also ~1).
3. **Emergency rotation lock past ~70° off vertical** — if `dot(body.up, world.up) < 0.34`, slam rotation to upright (preserving yaw). Rarely fires in practice with Balance=1, but catches the edge case where something inverts the body hard.

Earlier this loop also forced rotation upright every frame and zeroed X/Z angular velocity. That worked for topple-immunity but killed the body lean that powers slide-cuts — bots became easy to dodge. Removing those two operations (keeping only state clamps + emergency lock) restored slide-cut sharpness without re-introducing flops.

Reusable for the future goalie rewrite — attach the component, call `Bind(playerBody)`. Goalies barely lean so the existing settings should fit them too.

### HUD

`GauntletHud` is a `DontDestroyOnLoad` `MonoBehaviour` with `OnGUI`. Reads three static fields from `GauntletScenario`:

- `ShowHud` — top-left labels render only when this is true. Set by `Start` when wall is enabled, cleared by `Stop`.
- `PassedBots`, `WallDistance` — top-left two lines: "Passed: N" / "Wall: Nm back".
- `LossBannerSecondsRemaining`, `LastFinalScore` — centered banner during the post-loss window.

Host-only by design. A non-host client's local statics stay at default (`ShowHud = false`), so they render nothing. Cross-client sync via RPCs would be needed for multiplayer practice; skipped for now.

## Tuning knobs

All in `Scenarios/GauntletScenario.cs` unless noted.

| Const | Default | Effect |
|---|---|---|
| `PuckSpawnAheadOfPlayer` | 1.5m | Puck offset from player at start / on loss reset. |
| `BotSpawnAheadDistance` | 50m | Bot's Z relative to player at spawn — runway for chase momentum. |
| `BotPassedThreshold` | 5m | Bot Z this far behind player → recycle to standby. |
| `SpawnXWallMargin` | 2m | Bot spawn X never within this of either wall. |
| `MinSideOffset` / `MaxSideOffset` | 2 / 6m | Sidestep range from player's X when biasing the open side. |
| `MinSpawnXSeparation` | 3m | No consecutive spawn within this X of the previous. |
| `StandbyZ` / `StandbyY` | ForwardLength + 200 / 500 | Off-arena park for inactive pool bots. |
| `WallStartBehindPlayer` | 20m | Wall's opening cushion at start / after a loss. |
| `WallApproachSpeed` | 6 m/s | Constant forward speed of the wall. Skating tops near 8 m/s. |
| `WallTeleportBehindOnPass` | 7m | Wall snaps to `playerZ − this` after each bot pass. |
| `WallLossMargin` | 0.5m | Slack on the wall-front-touches-player loss check. |
| `WallHeight` / `WallThickness` | 200 / 2m | Pressure wall dimensions. Tall enough that puck can't sail over. |
| `LossBannerSeconds` | 2s | "CAUGHT!" banner duration. |
| `RinkVoid.ArenaWidth` | 20m | Public; gauntlet uses for spawn-X clamp. |
| `RinkVoid.ForwardLength` / `BackPadding` | 1950 / 50m | Floor extends from `−BackPadding` to `+ForwardLength`. |
| `RinkVoid.FloorTileMeters` / `WallTileMeters` | 4 / 4m | Real-world meters per source-texture repeat. |
| `BotTopplePreventer.BalanceFloor` | 1.0 | Minimum `KeepUpright.Balance`. Lower = more slide lean / more hover instability. |
| `BotTopplePreventer.TiltLockDot` | 0.34 (cos 70°) | Emergency rotation reset threshold. |

## Bugs we hit and the fix for each

1. **Scenery roots not found by initial name match.** First version walked top-level scene roots looking for `^Rink$`, `^Hangar$`, etc. The scene only has 16 roots and the rink hierarchy lives under a single `Level Default` root. **Fix:** restrict the walk to `Level Default`'s descendants and match anywhere in that subtree.
2. **Skating sound silenced.** Initial broader walk matched `^Sounds$` and `^Ice$` against descendants of every root — including the `Player Body/Sounds/Ice` audio source on the player itself. **Fix:** restrict to the `Level Default` subtree explicitly (no global walk).
3. **Spectators survived the first sweep.** They aren't under `Level Default` at all — they're spawned at runtime under a separate `Spectator Manager` root. **Fix:** added an `ExternalDisablePatterns` exact-match pass for known runtime scenery roots.
4. **Cube rendered as pink/black.** Unity primitive material uses Standard shader; Puck is URP. Sampling an existing rink material gave us black (donor's shader needed texture mappings the cube UVs didn't satisfy). **Fix:** build a fresh URP/Lit material with an explicit base color; later replaced with sampled Hangar materials + per-slab tiling so the player has visual reference for stickhandling.
5. **Player fell through the floor.** Unity primitive defaults to layer 0 (Default), which isn't in the player's grounded-raycast mask. **Fix:** set arena pieces to the `Ice` layer. No `Boards` or `Wall` layer exists — the real rink uses Ice for everything physical.
6. **`G` keybind fired while typing in chat.** `Keyboard.current[Key.G].wasPressedThisFrame` doesn't know about chat focus. **Fix:** gate on `UIManager.Instance.Chat.IsFocused == false` in the listener.
7. **AI goalies kept respawning during gauntlet.** `DetectPositions.Update` calls `DetectOpenGoalAndSpawnBot` every 10 frames. **Fix:** added `BotSpawning.SuppressAutoGoalieSpawn`; scenario sets it on Start, clears on Stop.
8. **Wall pushed the player forward but never registered a loss.** Loss check used `_wallZ` (center), but the wall physically shoves the player ahead, keeping `playerZ ≈ _wallZ + WallThickness/2 + ε`. The center comparison can never close. **Fix:** compare against `_wallZ + WallThickness/2` (the front face).
9. **Bots spawned looking knocked over.** Bots got toppled during contact in their previous active cycle, were recycled to standby with `HasFallen=true` still set, and read as flopped when reactivated. **Fix:** call `PlayerBody.OnStandUp()` after every `Server_Teleport` (both initial spawn and reactivate).
10. **Topple-prevention killed slide-cut sharpness.** First version of `BotTopplePreventer` forced rotation upright every frame and zeroed X/Z angular velocity — eating the body lean ChaserAI relies on for tight slides. Bots became easy to sidestep. **Fix:** drop the per-frame rotation force and the angular-velocity zeroing; keep `HasFallen` / `HasSlipped` state clamps + `Balance` floor + emergency lock only past 70° tilt.
11. **Bots pogo-glitched on hard hits when `Balance` had no floor.** With no minimum, `Balance` tweened toward 0 after a slip; hover physics rely on `Balance × hoverDistance` and collapse near 0, causing the body to bounce against the floor. **Fix:** floor `Balance` at 1.0 (defenders run the game default which also stays near 1, and they slide-cut fine — so this floor doesn't actually cost us anything).
12. **Wall mechanic leaked across disconnect.** Scenario state persisted in a static, and on reconnect the wall kept ticking and caught the player at origin a few seconds in. **Fix:** `ScenarioInputListener` watches `IsServer` true→false transitions and subscribes to `sceneUnloaded`; either path calls `ScenarioManager.Stop()`.
13. **Puck could escape with no loss.** Wall collision only checked the player. **Fix:** `PuckHitWall(wallFrontZ)` scans `PuckManager.GetPucks()` each tick and triggers `OnLoss` if any puck is at or past the wall front.

## Future work

- **Defender-type gauntlet variant.** Currently chaser-only. Defenders patrol a zone — different feel, would be a meaningful second mode if exposed via a third command name.
- **Difficulty scaling.** Spawn rate could shrink over time (every minute, rate cuts in half), or wall speed could ramp. The rate-fixed mode is the right starting point; this is an obvious next knob.
- **Persistent high score.** `LastFinalScore` is in-memory only. A small `ConfigData.GauntletHighScore` field would survive restarts and feed a "Best: N" line on the HUD.
- **RPC the HUD state to remote clients.** Currently host-only because the HUD reads server-side statics directly. A small NetworkVariable sync would make this work for non-hosts in multiplayer practice.
- **Sound for the pressure wall.** A low rumble that gets louder as the wall approaches would help the player gauge danger without having to look back. Same pipeline: gated on `ShowHud`, attenuated by `WallDistance`.
- **Goalie AI rewrite using `BotTopplePreventer`.** The component is already designed for reuse — goalies need permanent topple immunity. Same `Bind(playerBody)` call on spawn; might want `BalanceFloor` exposed as a per-instance field if goalie tuning differs.
