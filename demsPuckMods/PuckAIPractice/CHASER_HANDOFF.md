# Chaser Bot — Handoff

## Status (2026-05-14)

`/chaser <pos>` spawns a fully physics-driven skater bot at the named PlayerPosition on the team opposite the caller. The bot then chases the caller and uses its stick to swat at whichever puck the caller is handling. Real defender behavior (gap control, body-between-puck-and-net) is a separate AI yet to be built — see "Next: Defender AI."

Branch: `tm/defenderBot` (legacy name from when this was scoped as a defender). Built and deployed locally; not pushed.

**Renamed from "Defender" → "Chaser"** (2026-05-14). The original `/defender` command and `Defender/*` files are now `/chaser` and `Chaser/*`. The "Defender" name is reserved for the upcoming patrol-and-react AI.

## Chat commands

| Command | Effect |
|---|---|
| `/chaser LW` (or `C`, `RW`, `LD`, `RD`) | Spawn one bot at that named PlayerPosition on the opposite team. |
| `/chaser clear` | Despawn all chaser bots. |
| `/chaser clear LW` | Despawn the bot at that named position (any team). |

The chat command lives in `Chaser/ChaserCommandPatch.cs` as a Harmony prefix on `VoteManagerController.Event_Server_OnChatCommand` — composes cleanly alongside the existing goalie `/goalies` patch on the same method.

**Practice-mode gate was removed during testing** (commit history). The bot works in any session because `PracticeModeDetector.IsPracticeMode` is only true in single-player practice sessions, and we wanted to test on shared sessions. If we want to lock this down, re-add the gate or admin-check.

## File layout

All under `demsPuckMods/PuckAIPractice/Chaser/`:

| File | Role |
|---|---|
| `ChaserRegistry.cs` | Single source of truth for which Players are chaser bots. ClientId range 8_000_000+ to stay clear of goalie bots (7777777/7777778) and replay-copy offset (real id + 1337). |
| `ChaserSpawner.cs` | Server-side spawn/despawn. Instantiates `playerPrefab` (Traverse'd off `PlayerManager`), `SpawnWithOwnership`, claims the `PlayerPosition`, `Server_SpawnCharacter`. Despawn unclaims the position and removes from `PlayerManager`. |
| `ChaserAI.cs` | MonoBehaviour attached to the spawned bot. The whole AI brain lives here (chase + stick aim). |
| `ChaserCommandPatch.cs` | Harmony prefix on `VoteManagerController.Event_Server_OnChatCommand` parsing `/chaser`. |

Csproj entries added for all four files.

## Critical architectural choice: separate registry

Chaser bots are **NOT** registered in the existing `FakePlayerRegistry`. Reasons:

- `MovementFixedUpdatePatch` / `PlayerBody_FixedUpdate_Patch` / `DashLeft|RightSimPatch` (all in `Patches/BotInputPatch.cs`) skip or replace normal physics for `FakePlayerRegistry.IsFake` players. We want chaser bots to use **real** physics so they can collide with the player and feel like a real skater.
- `ExcludeFakePlayersFromGetPlayers` in `Patches/PlayerSearch.cs` filters bots out of `PlayerManager.GetPlayers`. We want chaser bots to appear there ("as if they were a player").
- `PlayerManager_AddPlayer_SkipFakePlayers` in `Patches/PlayerPatch.cs` blocks AddPlayer for fake clientIds. We want our bot added normally.

So `ChaserRegistry` is intentionally a completely separate, parallel registry. The two don't share state.

**Trade-off**: chaser bots won't get the existing replay-recorder bot-aware patches (Block + Inject in `Patches/ReplayRecorderPatch.cs` only check `IsFakeClientId` from `FakePlayerRegistry`). If a chaser bot is on the ice during a goal recording, the recorder will record its events with a clientId no real client owns. Behavior is untested. Watch for replay weirdness if chaser bots are alive during goals.

## Behavior model (ChaserAI)

The AI is a single `Update()` loop, layered as follows. All inputs are written server-side directly to `ServerValue` on the NetworkedInput fields — no RPC round-trip.

### 1. Target acquisition

Currently the target is fixed at spawn time to the **client who called `/chaser`** (`TargetClientId`). Looked up each frame via `PlayerManager.GetPlayerByClientId`. If the target is null/unspawned, bot zeros its input and idles.

### 2. Velocity tracking

Two EMAs of the target's `Rigidbody.velocity`:
- `recentTargetVel` (lerp factor 15/s, time constant ~67ms) — used for cut detection and aim.
- `oldTargetVel` (lerp factor 3/s, time constant ~330ms) — used to detect direction change vs `recentTargetVel`.

When the dot product between normalized recent and old vels drops below `CutDotThreshold = 0.3` (with both moving above `CutDetectMinSpeed = 2 m/s`), a cut is registered. Rate-limited by `CutReactionCooldown = 0.5s` to prevent re-firing.

### 3. Facing anticipation (predictedTargetVel)

The player's `transform.forward` rotates instantly when they turn the body; velocity lags behind by hundreds of ms due to skater mass. We blend the two:

```
predictedDir = Slerp(velocityDir, facingDir, FacingBias)   // default 0.6
predictedTargetVel = predictedDir * |velocity|
```

This is what's used for the lead point. Result: bot starts adjusting heading as soon as the player rotates, even before velocity catches up. The default 0.6 is a meaningful anticipation without being clairvoyant.

### 4. Lead point with magnitude clamp

```
leadTime = clamp(dist / LeadDistanceScale, 0, MaxLeadTime)    // 5, 1.2
if dist < CommitDistance: leadTime *= dist / CommitDistance   // 3.5 — fade lead at close range
leadVec = predictedTargetVel * leadTime
leadVec = ClampLead(leadVec, dist)                            // critical: see below
aimPoint = targetPos + leadVec
```

**`ClampLead` is the load-bearing fix for the 180° pivot bug.** Without it, when the target moves head-on at the bot at speed, the lead vector projects the aim point *past the bot's own position*, and the bot dutifully rotates 180° to face the phantom point behind it. The clamp caps lead magnitude to `dist × MaxLeadAsDistRatio` (default 0.5). Aim is therefore never further than half the gap behind the player.

### 5. Engagement state machine

When `dist < EngageDistance (6m)`: enter engagement, pick a mode once via `ChooseEngagementMode`:
- **Pursuit** (50%): aim = `targetPos + leadVec`. Just track them.
- **Mirror** (30%): currently identical to Pursuit. Reserved as a separate mode for future per-mode tuning.
- **Predict** (20%): pick a random side (±1). Aim = `targetPos + leadVec + lateralAxis × predictSide × PredictLateralOffset`. Bot commits to that side, betting the player will go that way.

When `dist > DisengageDistance (9m)`: exit engagement, reset to Pursuit. Hysteresis (6m vs 9m) prevents flicker.

When entering Predict mode, a one-shot slide commit is scheduled (`predictCommitPending = true`) — see slide section.

### 6. Movement input

```
toAim = aimPoint - botPos
signedAngle = SignedAngle(botForward, toAim, up)
absAngle = |signedAngle|

turn = (absAngle > TurnDeadzoneDeg) ? clamp(signedAngle / TurnFullInputAngle, -1, 1) : 0
fwd  = piecewise based on absAngle:
       1.0                                   if absAngle <= ForwardFullAngle (15°)
       1 - lerp(15, 90, absAngle)            if between
       0                                     if absAngle >= ForwardCutoffAngle (90°)

if dist < ThrottleStartDistance (8m):
    fwd *= lerp(ThrottleMinForward, 1.0, dist / 8)    // brake into engagement
```

MoveInput.ServerValue = (turn, fwd).

### 7. Slide (crouch)

Slide fires for **only two reasons**:
- A cut was detected this frame (`cutDetected` — force, bypasses cooldown).
- A Predict mode commit just started (`predictCommitPending`, respects cooldown).

**Deliberately does NOT fire on `absAngle > threshold`.** That was the original behavior and caused panic crouches at close range due to geometry (small lateral offsets between bot and player blow up the aim angle when distance is tiny). The user verified that with the geometric trigger removed, the bot no longer slides when you go straight at it.

Active slide releases on either `SlideMaxDuration (0.35s)` elapsed or `absAngle < SlideReleaseAngle (20°)`. Cooldown `SlideCooldown (1.2s)` after release.

### 8. Sprint

```
isSprinting starts when: stamina > 0.35, fwd >= 0.5, !isSliding, dist > 8m (or post-slide window)
isSprinting stops when:  stamina < 0.15, OR fwd < 0.5, OR slide starts, OR dist < 6m
```

Post-slide sprint burst: for `PostSlideSprintWindow (0.6s)` after a slide releases, the distance gate is bypassed. Lets the bot recover speed after a reactive slide-cut even at close range. Pairs well with the slide-on-cut behavior.

### 9. Stick aim

Stick aim runs after movement each frame in `AimStickAtPuck`. The chaser tracks whichever puck the **target player** (the one being chased) is closest to — `PickFocusPuck(targetPos)` scans all live pucks and picks the nearest to the target. This switches focus automatically when the player swaps pucks; no event hooks needed.

The aim point is computed on the ice along the bot→puck horizontal axis:

```
aimDist = min(horizDist + StickAimForwardExtension, StickMaxAimDistance)
aimPoint = botPos + horizDir * aimDist
aimPoint.y = puckPos.y                       // ice level
```

The forward extension (default 0.3m) pushes the aim point past the puck so the game's raycast clears over the puck and lands on the ice past it — without this, the ray hits puck-top and the blade hovers at puck-top height. The max-distance cap (default 1.8m) keeps the aim within the stick's physical ice-reach window (~2.3m horizontal at chest height with maxReach 2.5m), so the ray always hits the ice; otherwise it runs out in midair and the blade hovers.

The aim point is converted to local space and inverted back into `(pitch, yaw)`:
- `dirLocal = StickPositioner.transform.InverseTransformDirection(toAim.normalized)`
- `pitch = asin(-dirLocal.y) × Rad2Deg`
- `yaw = atan2(dirLocal.x, dirLocal.z) × Rad2Deg`
- Written to `input.StickRaycastOriginAngleInput.ServerValue`.

The game's PID smooths to the target — that lag is what makes the bot look like a human aiming rather than a perfect snap-to-puck robot. No Harmony patches required.

**Stick tuning knobs** (on `ChaserAI`):
- `StickAimForwardExtension` (0.3m) — push aim past the puck so blade lands on ice next to it, not on top.
- `StickMaxAimDistance` (1.8m) — clamp aim distance so the raycast always lands on the ice; lower = stick stays lower / closer to the bot.

## Tuning knobs (all on the ChaserAI component)

| Header | Field | Default | Effect |
|---|---|---|---|
| Turn | TurnDeadzoneDeg | 5° | Below this, no turn input |
| Turn | TurnFullInputAngle | 30° | At this angle, turn input maxes (±1) |
| Forward | ForwardFullAngle | 15° | Within this absAngle, full forward |
| Forward | ForwardCutoffAngle | 90° | Past this, no forward |
| Slide | SlideReleaseAngle | 20° | Release ongoing slide when re-aligned |
| Slide | SlideMaxDuration | 0.35s | Hard cap on slide burst |
| Slide | SlideCooldown | 1.2s | Min gap between slides |
| Pursuit / Lead | MaxLeadTime | 1.2s | Cap on lead horizon |
| Pursuit / Lead | LeadDistanceScale | 5 | distance / this = leadTime |
| Pursuit / Lead | CommitDistance | 3.5m | Lead fades to zero inside this |
| Pursuit / Lead | MaxLeadAsDistRatio | 0.5 | **Lead magnitude cap. Critical.** |
| Facing Anticipation | FacingBias | 0.6 | 0 = velocity-only, 1 = facing-only |
| Cut Reaction | CutDotThreshold | 0.3 | dot(recentVel, oldVel) below this = cut |
| Cut Reaction | CutDetectMinSpeed | 2 m/s | Ignore cuts when slow |
| Cut Reaction | CutReactionCooldown | 0.5s | Min gap between cut detections |
| Engagement | EngageDistance | 6m | Enter engagement inside |
| Engagement | DisengageDistance | 9m | Exit engagement past |
| Engagement | MirrorChance | 0.3 | P(Mirror) on entering engagement |
| Engagement | PredictChance | 0.2 | P(Predict). Pursuit = remainder |
| Engagement | PredictLateralOffset | 1.8m | Side commit in Predict mode |
| Sprint | SprintMinForward | 0.5 | Need this fwd input to sprint |
| Sprint | SprintMinStamina | 0.15 | Stop sprint below this |
| Sprint | SprintResumeStamina | 0.35 | Restart sprint above this |
| Sprint | SprintEngageDistance | 8m | Sprint only when farther than this |
| Sprint | SprintReleaseDistance | 6m | Drop sprint when closer than this |
| Sprint | PostSlideSprintWindow | 0.6s | Bypass distance gate this long after slide |
| Approach Throttle | ThrottleStartDistance | 8m | Begin damping forward thrust inside |
| Approach Throttle | ThrottleMinForward | 0.25 | Min thrust at zero distance |
| Stick Aim | StickAimForwardExtension | 0.3m | Push aim past puck so blade hits side, not top |
| Stick Aim | StickMaxAimDistance | 1.8m | Cap aim distance so blade stays on ice |

## Bugs we hit and the fix for each

1. **Patch never fired on `/chaser` chat command** (was `/defender` at the time). Cause: `PracticeModeDetector.IsPracticeMode` was false in test session — same gate as `/goalies`, so existing commands also wouldn't have worked. Fix: removed the practice-mode gate; the prefix runs whenever the chat command arrives.
2. **Bot looped past the player on every overshoot.** Cause: no lead, pure pursuit. Fix: added lead pursuit (leadTime = dist / LeadDistanceScale).
3. **Bot didn't react to late cuts.** Cause: bot was reading velocity, which lags player rotation by hundreds of ms due to mass. Fix: blend `transform.forward` (facing) into the velocity used for lead. `FacingBias = 0.6`.
4. **Bot panic-crouched at close range even when player went straight.** Cause: `SlideEngageAngle` fired on geometric angle, which spikes at close range due to small lateral offsets. Fix: removed the angular trigger entirely. Slide now only fires on detected cuts or Predict mode commits.
5. **Bot pivoted 180° when player closed head-on at speed.** Cause: lead vector pointed toward the bot (player moving at bot), and `leadVec.magnitude > dist`, so `aimPoint` ended up *behind* the bot's own position. Bot rotated to face the phantom point. Fix: `ClampLead` caps the lead magnitude at `dist × MaxLeadAsDistRatio` so the aim never flies past the bot.
6. **Stick locked onto the wrong puck with multiple pucks on ice.** Cause: `PuckManager.GetPuck()` returns the first puck in its list — arbitrary. Fix: `PickFocusPuck` selects the puck closest to the target player (the one being chased), so focus follows the player's handling.
7. **Stick hovered above the puck, barely grazing the top.** Cause: aim was at puck position → game's raycast hit puck-top → blade target landed at puck-top height. Fix: aim past the puck along bot→puck axis so the ray clears the puck and hits the ice past it; the blade then presses against the puck's side at ice level.
8. **Stick was elevated when bot was far from puck.** Cause: at long range, the ray going forward over the puck ran out at maxReach (2.5m) before hitting ice, so the blade target landed in midair. Fix: cap aim distance at `StickMaxAimDistance = 1.8m` so the ray always lands on the ice within reach. Stick stays at ice level at all times.

## Insight discovered while tuning

The chaser AI fundamentally cannot solve the **180° player turn**. A player at full speed who simply turns around (no juke needed) is uncatchable by a chaser because:
- A crouch turn by the bot kills its speed too much — player skates by.
- A regular turn has too large a radius — bot is already past the cut point when it turns.

This is a structural limit of "chase" as a strategy, not a bug. The fix is to build a different AI (real Defender, below) that never commits to a chase and only denies space.

## Next: Defender AI (patrol + react)

**User-stated goal**: a bot that patrols a zone, only switches to chase when a threat enters their zone heading in their direction. Solves the "chaser is uncatchable on 180° turns" problem because the defender never over-commits.

**Patrol pattern**: side-to-side / back-and-forth oscillation while facing the threat zone. Considered figure-8 but rejected — figure-8 has the defender facing different directions through the loop, half the time body is angled wrong for a fast pivot to chase. Side-to-side keeps them pivot-ready every frame and is closer to how real defenders skate.

**Trigger to switch to chase**: player gets close to defender's zone AND heading toward it. Probably a combo of distance threshold and velocity dot product with bot→player axis.

**Trigger to return to patrol**: player exits zone or velocity points away.

**Likely shape**:
- New `Defender/DefenderAI.cs` with patrol behavior + a "promote to chase" state.
- Could reuse `ChaserAI`'s movement primitives if extracted into a shared helper.
- New `/defender <pos>` command parallel to `/chaser <pos>`.
- New `DefenderRegistry` parallel to `ChaserRegistry`, same clientId base range strategy.

**Long-term vision** (out of scope for first cut): a full 5-bot lineup. LW/C/RW patrol near the blue line; LD/RD deeper near the slot. Entering the zone gets you collapsed on by 2 forwards, followed up by defenders. A `/lineup` or similar command would spawn the whole defensive set.

## Future stick work (deferred)

Phase 1 (track + swat at the closest puck to target) is done and working. Phases below are still future work:

- **Phase 2 — lead the puck motion.** User judgment after Phase 1: probably not needed. Current behavior gives "enough buffer for the player to juke, but not too much that it's super easy."
- **Phase 3 — discrete poke actions** (twist, extend, lateral RPCs) instead of just blade positioning. Adds reactive bursts.
- **Phase 4 — last-touched-by-player puck tracking** via an `Event_*OnPuckTouched`-style event. The current "closest to target" heuristic approximates this and may be good enough.

## Future work beyond

- **Multi-bot lane scenarios**: spawn 2-3 staggered bots, each in their own lane. Player has to beat them serially. Requires lane assignment + per-bot zone constraints so they don't all collapse on the puck.
- **Reset to position command**: `/chaser reset` / `/defender reset` to send all bots back to their spawn positions. Useful between practice reps.
- **Replay compatibility**: bots aren't covered by the existing replay-recorder patches. If they're alive during a goal, behavior is unverified.
