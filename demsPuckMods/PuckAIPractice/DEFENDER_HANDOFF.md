# Defender Bot — Handoff

## Status (2026-05-14)

`/defender <pos>` spawns a fully physics-driven skater bot at the named PlayerPosition on the team opposite the caller. Unlike the chaser, the defender plays a **zone**: it patrols a fixed anchor position (the defensive zone equivalent of its faceoff spot) and only chases when the target enters the zone. The whole defensive formation also **shifts laterally** with the player's position so the unit moves as a group.

**Latest (2026-05-14, second pass):** Two efficacy improvements landed.
1. `ZoneRadius` is now per-position. LD/RD bumped to **20m** so the corner defenders are already engaging when the attacker reaches the C's zone — no more "beat the forward, then face the D one at a time." LW/RW/C kept at 16m.
2. Chase now has **cut detection + a chase-slide burst** ported from `ChaserAI`. A hard direction change on the target triggers a brief slide so the bot pivots tightly to follow. Distinct from the patrol pulse-crouch (`isPatrolSliding`) — separate state, separate fields. Sprint is suppressed while the chase slide is active.

Branch: `tm/defenderBot`. Built and deployed locally; not pushed.

Paired with `CHASER_HANDOFF.md` (the constant-pursuit bot). Chaser and Defender can coexist on the ice — separate registries, separate clientId ranges.

## Chat commands

| Command | Effect |
|---|---|
| `/defender LW` (or `C`, `RW`, `LD`, `RD`) | Spawn one defender bot at that position on the team opposite the caller. |
| `/defender all` | Spawn defender bots at every unclaimed position on the team opposite the caller. Already-claimed slots (real players or existing bots) are skipped. |
| `/defender clear` | Despawn all defender bots. |
| `/defender clear LW` | Despawn the defender at that position. |

Practice-mode gate is removed (works in any session), same as chaser.

## File layout

All under `demsPuckMods/PuckAIPractice/Defender/`:

| File | Role |
|---|---|
| `DefenderRegistry.cs` | ClientId range **9_000_000+** to stay clear of chaser bots (8M+), goalie bots (7777777/7777778), and replay-copy offset (real id + 1337). |
| `DefenderSpawner.cs` | Server-side spawn/despawn. Sets per-position defaults, computes the patrol anchor and rink-aligned axes, attaches `DefenderAI`. |
| `DefenderAI.cs` | The brain — patrol state machine, zone shifting, chase, stick aim. |
| `DefenderCommandPatch.cs` | Harmony prefix on `VoteManagerController.Event_Server_OnChatCommand` parsing `/defender`. |

Csproj entries added for all four files.

## Architectural choices

### Separate registry from FakePlayerRegistry

Same reasoning as the chaser — defender bots need to use real physics (so they can collide with you) and appear in `PlayerManager.GetPlayers` as if they were real players. `DefenderRegistry` is a parallel registry with a distinct clientId range (9M+). It does NOT share state with `FakePlayerRegistry` or `ChaserRegistry`.

**Trade-off** (carried over): defender bots aren't covered by the existing replay-recorder bot-aware patches. If a defender is on the ice during a goal, the recorder will record events with a clientId no real client owns. Behavior is untested.

### Rink-axis assumptions

The defender AI assumes Puck's rink is aligned with world X (lateral / width) and Z (length), with rink center at world origin. Each team spawns on its own side of the rink along the Z axis, so `sign(spawnPos.z)` tells us which Z direction is "toward own goal" (= backward direction for that team).

**Critical: `position.transform.forward` is NOT rink-aligned.** Faceoff transforms point toward the faceoff dot at an angle (e.g. Red LD at (-10, 0, -13.25) has forward = (0.57, 0, 0.82) — pointing into +X and +Z toward the central dot). Using `transform.forward` for "pull bot back into defensive zone" dragged bots diagonally outward toward the boards. The current code derives all rink-axis directions from `spawnPos.z` (backward = same Z sign as spawn) and `spawnPos.x` (inward = opposite sign to spawn X).

### Spawn coordinates observed in-game

| Position | X (lateral) | Z (length) |
|---|---|---|
| LD/RD | ±~10–12 | ±~13 (blue line area) |
| LW/RW | ±~14–17 | ±~3 (near center ice line) |
| C | 0 | ~0 |

Rink ~91.5m × 45m. Blue lines ~Z=±15.

## Behavior model

The AI is a two-state machine (Patrolling ↔ Chasing) with three sub-phases under Patrol.

### State transitions (zone-based)

```
Patrol → Chase:  target inside ZoneRadius of dynamic HomePosition
Chase  → Patrol: target beyond ZoneRadius + ZoneReturnHysteresis (4m) of dynamic HomePosition
```

`ZoneRadius` is per-position (LD/RD = 20m, LW/RW/C = 16m — see "Per-position defaults"). The "dynamic HomePosition" matters — see Zone Shifting below.

**Velocity-direction check was deliberately removed.** First-pass attempted "trigger chase when target is close AND moving toward bot." That made pursuit unpredictable and missed obvious threats (stationary attackers next to the zone, attackers cutting laterally inside the zone). Pure distance-from-zone-center is the trigger now.

### Zone shifting

The whole defensive formation slides laterally with the target player's X position. Each defender independently computes the same shift signal, so the unit translates as a group.

```
desiredShift.x = clamp(target.x * LateralShiftFactor, -MaxLateralShift, +MaxLateralShift)
currentShift = lerp(currentShift, desiredShift, dt * ShiftSmoothing)
HomePosition = StaticHome + currentShift
```

`StaticHome` is the immutable anchor (set at spawn). `HomePosition` is recomputed every frame and is what all other logic reads — patrol target, zone-check, debug marker. Default knobs: `LateralShiftFactor = 0.4`, `MaxLateralShift = 5m`, `ShiftSmoothing = 3`.

### Patrol — three sub-phases

Based on `distFromHome`:

#### 1. Transit (`dist > BrakeStartDistance`, default 5m)

Full-speed straight skating toward home. No crouch pulse, no speed cap. Used when the bot just spawned far away, just dropped chase, or the zone shifted faster than the bot could orbit-track.

#### 2. BrakeApproach (`OrbitEngageDistance < dist ≤ BrakeStartDistance`)

The bot would otherwise sail through the orbit zone at full skating speed and oscillate back and forth across home. To kill momentum, slam `StopInput` (the alt-key brake — `NetworkedInput<bool>` on `PlayerInput`). Still steers toward home so the bot's body ends up facing the right way when the brake releases. Once `Rigidbody.velocity.magnitude < BrakeReleaseSpeed` (default 2 m/s), the brake releases and `DoTransitToHome` takes over for the final glide-in.

#### 3. Orbit (`dist ≤ OrbitEngageDistance`, default 1.5m)

A tight circle around the pole — radius 1.5m, low speed (1.6–2.4 m/s), continuous pulse crouch for sharp turning.

**Orbit mechanics**:
```
botAngle = atan2(bot.z - home.z, bot.x - home.x)       // bot's angle around home
aimAngle = botAngle + CircleLeadDeg * Deg2Rad           // lead by 60° in orbit direction
aimPoint = home + CircleRadius * (cos(aimAngle), 0, sin(aimAngle))
```

The aim point sits on the desired circle, slightly ahead of the bot's current angular position. Because the aim is **always** on the circle at fixed radius, drift is self-correcting — the bot is geometrically pulled toward the right radius from wherever it is.

**Speed management** is hysteresis-latched:
- Below `CircleMinSpeed` (1.6 m/s): forward = 1, latch on.
- Above `CircleMaxSpeed` (2.4 m/s): forward = 0, latch off.
- Latch holds between frames so the bot doesn't toggle every frame around the boundary.

**Crouch pulse**: `PulsePeriod = 0.4s`, `PulseDuration = 0.15s` (held on the first 0.15s of each 0.4s window, then off). Tight turn radius without bleeding all the speed.

### Chase

Straight-line "go toward target" — turn + forward + sprint. Still simpler than `ChaserAI` (no lead point, no Mirror/Predict modes), but **does now react to cuts**:

- Velocity EMAs (`recentTargetVel` at ~67ms time constant, `oldTargetVel` at ~330ms) run continuously regardless of state, so the data is warm the moment chase engages.
- `DetectChaseCut` fires once when `dot(recentVel, oldVel) < ChaseCutDotThreshold` (with both above `ChaseCutDetectMinSpeed`). Self-cooldowns via `ChaseCutReactionCooldown` so a sustained turn doesn't re-trigger every frame.
- On a detected cut, `UpdateChaseSlide` starts a slide burst (`SlideInput = true`). Released when either `ChaseSlideMaxDuration (0.35s)` elapses OR the bot's facing has re-aligned with the target inside `ChaseSlideReleaseAngle (20°)`.
- Cut-triggered slides bypass `ChaseSlideCooldown` — the cut detector's own cooldown is the real rate-limit. The slide cooldown only matters for future non-cut triggers (none today).
- A separate state flag (`isChaseSliding`) keeps this distinct from the patrol pulse-crouch (`isPatrolSliding`) so neither system steps on the other's `SlideInput` writes. The state-machine transitions release each cleanly when crossing the chase/patrol boundary.

Sprint engages when stamina > 0.4 and `dist > ChaseSprintEngageDistance` (5m); releases when stamina < 0.2, `dist < ChaseSprintReleaseDistance` (3m), **or `isChaseSliding == true`** (sprint during a slide wastes stamina — sliding kills traction).

### Stick aim

Identical to `ChaserAI` — see `CHASER_HANDOFF.md` §9 for the full inverse-math explanation. Picks the puck closest to the target player and aims at a point on the ice past the puck along the bot→puck axis, with the aim distance capped so the raycast always lands on the ice.

## Per-position defaults

The spawner overrides three AI fields based on position name:

| Position | Pull-back | Tuck-in | ZoneRadius |
|---|---|---|---|
| LD/RD | 18m | 4m | **20m** |
| LW/RW | 15m | 4m | 16m |
| C | 15m | 0m | 16m |

Pull-back and tuck-in were arrived at empirically by visually confirming the debug pole's location matched what a defensive setup should look like. For LD at world spawn (-10, 0, -13.25), the static home ends up around (-6, 0, -31.25) — deep slot. C (which has no inward direction since spawn X ≈ 0) just gets the longitudinal pull-back.

**Why LD/RD have a wider zone:** the corner defenders sit ~18m behind their spawn point near the slot. With a 16m zone, they only engaged once an attacker had already beaten the forward line and was deep in the zone. At 20m, their zone reaches the blue line — they're chasing before the attacker reaches the C, so the attacker faces multiple defenders converging instead of one at a time. Forwards keep 16m because their zones are already up at the blue line where they're meant to engage early.

## Tuning knobs (all on the DefenderAI component)

| Header | Field | Default | Effect |
|---|---|---|---|
| Zone Anchor | HomePullBackDistance | per-position | Backward distance from spawn toward own goal |
| Zone Anchor | HomeTuckInDistance | per-position | Lateral distance from spawn toward centerline |
| Patrol Orbit | CircleRadius | 1.5m | Tight orbit radius |
| Patrol Orbit | CircleLeadDeg | 60° | Aim-ahead angle (sign chooses CW/CCW) |
| Patrol Orbit | CircleMinSpeed | 1.6 m/s | Below this, forward = 1 |
| Patrol Orbit | CircleMaxSpeed | 2.4 m/s | Above this, forward = 0 |
| Patrol Orbit | OrbitEngageDistance | 1.5m | Inside this radius → orbit mode |
| Patrol Orbit | BrakeStartDistance | 5m | Begin brake-approach phase |
| Patrol Orbit | BrakeReleaseSpeed | 2 m/s | Brake releases below this speed |
| Patrol Movement | PatrolTurnDeadzoneDeg | 5° | Below this angle, no turn input |
| Patrol Movement | PatrolTurnFullInputAngle | 30° | At this angle, full turn input |
| Patrol Movement | PulsePeriod | 0.4s | Crouch pulse window |
| Patrol Movement | PulseDuration | 0.15s | Crouch held within each window |
| Zone | ZoneRadius | per-position | Target inside → chase. LD/RD = 20m, others = 16m (set by spawner) |
| Zone | ZoneReturnHysteresis | 4m | Target outside (radius+this) → return to patrol |
| Zone Shift | LateralShiftFactor | 0.4 | `shift.x = factor × target.x` |
| Zone Shift | MaxLateralShift | 5m | Cap on shift magnitude |
| Zone Shift | ShiftSmoothing | 3 | Lerp rate toward desired shift |
| Chase Movement | ChaseTurnDeadzoneDeg | 5° | Below this angle, no turn |
| Chase Movement | ChaseTurnFullInputAngle | 30° | At this angle, full turn |
| Chase Movement | ChaseForwardFullAngle | 15° | Within this angle, fwd = 1 |
| Chase Movement | ChaseForwardCutoffAngle | 90° | Past this angle, fwd = 0 |
| Chase Movement | ChaseSprintMinStamina | 0.2 | Stop sprinting below this |
| Chase Movement | ChaseSprintResumeStamina | 0.4 | Resume sprinting above this |
| Chase Movement | ChaseSprintEngageDistance | 5m | Min distance to start sprinting |
| Chase Movement | ChaseSprintReleaseDistance | 3m | Close inside this → drop sprint |
| Chase Cut Reaction | ChaseCutDotThreshold | 0.3 | dot(recentVel, oldVel) below this = cut detected |
| Chase Cut Reaction | ChaseCutDetectMinSpeed | 2 m/s | Ignore cuts when target is barely moving |
| Chase Cut Reaction | ChaseCutReactionCooldown | 0.5s | Min gap between cut-triggered slides (real rate-limit) |
| Chase Slide | ChaseSlideMaxDuration | 0.35s | Hard cap on chase-slide burst length |
| Chase Slide | ChaseSlideCooldown | 1.2s | Min gap after slide end (bypassed by cuts) |
| Chase Slide | ChaseSlideReleaseAngle | 20° | Release ongoing slide when re-aligned with target |
| Stick Aim | StickAimForwardExtension | 0.3m | Aim past puck so blade hits side not top |
| Stick Aim | StickMaxAimDistance | 1.8m | Cap aim distance so blade stays on ice |
| Debug | ShowDebugMarker | true | Spawn yellow pole at HomePosition |
| Debug | DebugMarkerColor | yellow | Pole color |

## Bugs we hit and the fix for each

1. **`PlayerRole.Defender` doesn't exist.** Only None/Attacker/Goalie in the enum. Defender bots get `PlayerRole.Attacker` like the chaser does — they're skaters, not goalies.
2. **Pursuit never triggered.** First attempt used "target close AND moving toward bot" via a velocity dot product. Failed on stationary targets and lateral cuts. **Fix:** zone-based trigger — pure distance from `HomePosition`, no velocity check.
3. **Patrol anchor was offset in the wrong direction (all wings and defenders).** First attempt computed "inward direction" from `C.transform.position - spawnPos`. The "C" PlayerPosition in Puck isn't necessarily at world center (likely a faceoff-dot transform somewhere off-center), so `toCenter` sometimes pointed *outward*. **Fix:** derive inward direction from `spawnPos.x` directly — assume rink centerline is at world X=0, so inward = `-sign(spawnPos.x) * X`. Direction-correct regardless of where Puck's PlayerPosition objects happen to live.
4. **Pull-back dragged bots diagonally outward.** `position.transform.forward` is angled toward the faceoff dot, not aligned with the rink length axis (Red LD's forward = (0.57, 0, 0.82) — 35° off pure +Z). Pulling back along `-spawnForward` had a strong lateral component that pushed the bot toward the boards. **Fix:** use `sign(spawnPos.z)` to determine the rink-axis backward direction directly. Bots pull back along pure ±Z, no lateral drag.
5. **Sine-wave patrol overshot endpoints, looped wide.** Bot couldn't decelerate sharply enough at the ends of the stroke; ended up tracing wide arcs around the home position. **First fix:** state machine with explicit U-turn phases. **Second fix (final):** ditched the line-and-U-turn pattern entirely for a tight orbit.
6. **U-turn rotation went through facing-own-goal.** At the "outer-back" endpoint, the shortest rotation from facing -PatrolAxis to facing +PatrolAxis crossed through facing-south (toward own net). **Fix:** force turn direction with `sign(SignedAngle(currentAxis, ThreatDirection))` so the body always rotated through facing-threat. (No longer relevant in the orbit-based patrol.)
7. **Full-hold crouch during U-turn slid too far and killed speed.** Was a real-hockey-feel problem the user identified. **Fix:** replaced with pulse crouch on a fixed schedule. (Then later: orbit replaced the U-turn pattern entirely, pulse crouch carried over.)
8. **Patrol straights blasted top speed, hard to control.** **Fix:** switched from line+U-turn to tight orbit with speed hysteresis (1.6–2.4 m/s) — bot coasts most of the time, only pushes when it slows below the floor.
9. **Bot oscillated around home (overshoot/loop/overshoot).** Bot full-speed transit→sailed past home→looped wide→full-speed transit again. **Fix:** added a `BrakeApproach` phase between transit and orbit that slams `StopInput` (the alt-key brake) until speed drops below `BrakeReleaseSpeed`. Bot arrives at home near-stationary and slides cleanly into the orbit.
10. **Spawn-in took forever.** Bot was using orbit-speed patrol the entire way from spawn to home (8+ meters at 2 m/s with crouch pulses = many seconds). **Fix:** Transit phase — full-speed straight skating when bot is far from home, no speed cap, no pulse.

## Insight worth keeping

The defender's bigger structural advantage over the chaser is the **zone shift**: because all defenders share the same shift signal, the formation moves as a group. A player cutting laterally pulls the whole defensive shape with them, not just one bot. Even without sophisticated per-bot reactions, the formation behaves like a connected unit.

The single-knob signal (target's world X) is intentionally simple. Per-bot independent shifts would create gaps between defenders; uniform shift maintains relative spacing.

## Next: increasing defender efficacy

**The current problem (user's words):** "they're still kind of too easy. Let's say one is facing the right way, the center, but one guy is easy to beat, so you beat him, but the LD and RD are backwards, just so happen to be… they have no chance."

It comes down to luck on facing direction. If the engaged defender happens to be on the wrong side of their orbit when you arrive, you can outmaneuver them before they pivot. And by that point the next-line defender hasn't started moving yet because you haven't entered *their* zone.

### The real-hockey model

In a real game, you can't stickhandle through an entire team because **the next defender is already in motion when you make your move**. When you beat the first guy, the second guy isn't reacting — they were already in pursuit, just from a different angle. You force one defender to commit, and the second is positioned to meet you wherever you go.

### Proposed approach

**Bump zone radius to ~20m so adjacent defenders engage simultaneously.** Right now each defender has a 16m zone — neighbors overlap by ~10m, but the corner defenders only engage once you're well into their zone. At 20m radius, the LD and RD are already in chase mode the moment you enter the C's zone. They're coming at you from different angles before you make your first move.

With multiple defenders converging at once, the player's options narrow:
- Beat C with a right cut → LD is already in motion and adjusts to meet you on the right
- Beat C with a left cut → RD adjusts to meet you on the left
- Only way through: a really clean move, or continue away from the net laterally (= no shot)

### What needs to change for this to work

1. **`ZoneRadius` 16 → 20.** ✅ Done (2026-05-14) — LD/RD only, per-position default in spawner. Forwards stay at 16m. Adjacent zones now overlap enough that an attacker entering the C's zone is also inside LD's and RD's, so all three engage simultaneously.
2. **Chase has to be reactive to cuts.** ✅ Done (2026-05-14) — `DetectChaseCut` runs on the same dual-EMA + dot-product machinery as `ChaserAI`. On detect, `UpdateChaseSlide` fires a slide burst (released on duration OR re-alignment). State is fully separate from the patrol pulse-crouch.
3. **Predictive aim during chase.** Still open. Lead the target's velocity slightly with a magnitude cap (`MaxLeadAsDistRatio` trick from `ChaserAI`) to avoid the 180° pivot bug. The defender currently aims at the target's *current* position — adding lead would let the bot meet the cut instead of trailing it. Pair well with the cut-slide that's now in place.
4. **Different chase angles per defender.** Deferred — user flagged as potentially overkill. The "team brain" idea: when multiple defenders engage one target, the lead defender on the player's cut side anticipates the cut while the trailing defender stays passive / covers the cut-back. Promising but adds cross-bot coordination state.
5. **Faster initial chase acceleration.** Still open. Options unchanged from the original writeup: one-shot velocity nudge at chase-start, higher patrol min speed, or rely on the new cut-slide to handle the worst case.

### Suggested order (revised)

1. ✅ Bump LD/RD `ZoneRadius` to 20m.
2. ✅ Add cut detection + slide-pivot to chase.
3. **Playtest.** Pure-knob and pure-port changes; the right next move depends on how the bots feel now. If they're still trailing on cuts, add predictive lead (item 3). If they're still losing the first race off the line, add chase-start nudge (item 5). If single defenders are still beatable in isolation, revisit the team-brain coordination idea (item 4).

## Future work beyond efficacy

- **Reset to position**: `/defender reset` to send all bots back to their static home (regardless of current state).
- **Multi-bot lineup command**: `/defenders` (plural) spawns all 5 positions at once.
- **Replay compatibility**: same gap as chaser — bots not covered by replay-recorder bot-aware patches.
- **Real defender + chaser combo**: at full lineup, 3 forwards as chasers (forecheck) and 2 defenders as zone defenders (containment) approximates a real team. `/lineup` style command.
