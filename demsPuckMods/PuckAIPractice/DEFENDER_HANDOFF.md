# Defender Bot — Handoff

## Status (2026-05-14)

`/defender <pos>` spawns a fully physics-driven skater bot at the named PlayerPosition on the team opposite the caller. The bot then chases the caller as a "chaser" AI — currently the only behavior, despite the command name. Real defender (gap control, body-between-puck-and-net) is intentionally not built yet.

Branch: `tm/defenderBot`. Built and deployed locally; not pushed.

## Chat commands

| Command | Effect |
|---|---|
| `/defender LW` (or `C`, `RW`, `LD`, `RD`) | Spawn one bot at that named PlayerPosition on the opposite team. |
| `/defender clear` | Despawn all defender bots. |
| `/defender clear LW` | Despawn the bot at that named position (any team). |

The chat command lives in `Defender/DefenderCommandPatch.cs` as a Harmony prefix on `VoteManagerController.Event_Server_OnChatCommand` — composes cleanly alongside the existing goalie `/goalies` patch on the same method.

**Practice-mode gate was removed during testing** (commit history). The bot works in any session because `PracticeModeDetector.IsPracticeMode` is only true in single-player practice sessions, and we wanted to test on shared sessions. If we want to lock this down, re-add the gate or admin-check.

## File layout

All under `demsPuckMods/PuckAIPractice/Defender/`:

| File | Role |
|---|---|
| `DefenderRegistry.cs` | Single source of truth for which Players are defender bots. ClientId range 8_000_000+ to stay clear of goalie bots (7777777/7777778) and replay-copy offset (real id + 1337). |
| `DefenderSpawner.cs` | Server-side spawn/despawn. Instantiates `playerPrefab` (Traverse'd off `PlayerManager`), `SpawnWithOwnership`, claims the `PlayerPosition`, `Server_SpawnCharacter`. Despawn unclaims the position and removes from `PlayerManager`. |
| `DefenderAI.cs` | MonoBehaviour attached to the spawned bot. The whole AI brain lives here. |
| `DefenderCommandPatch.cs` | Harmony prefix on `VoteManagerController.Event_Server_OnChatCommand` parsing `/defender`. |

Csproj entries added for all four files.

## Critical architectural choice: separate registry

Defender bots are **NOT** registered in the existing `FakePlayerRegistry`. Reasons:

- `MovementFixedUpdatePatch` / `PlayerBody_FixedUpdate_Patch` / `DashLeft|RightSimPatch` (all in `Patches/BotInputPatch.cs`) skip or replace normal physics for `FakePlayerRegistry.IsFake` players. We want defender bots to use **real** physics so they can collide with the player and feel like a real skater.
- `ExcludeFakePlayersFromGetPlayers` in `Patches/PlayerSearch.cs` filters bots out of `PlayerManager.GetPlayers`. We want defender bots to appear there ("as if they were a player").
- `PlayerManager_AddPlayer_SkipFakePlayers` in `Patches/PlayerPatch.cs` blocks AddPlayer for fake clientIds. We want our bot added normally.

So `DefenderRegistry` is intentionally a completely separate, parallel registry. The two don't share state.

**Trade-off**: defender bots won't get the existing replay-recorder bot-aware patches (Block + Inject in `Patches/ReplayRecorderPatch.cs` only check `IsFakeClientId` from `FakePlayerRegistry`). If a defender bot is on the ice during a goal recording, the recorder will record its events with a clientId no real client owns. Behavior is untested. Watch for replay weirdness if defender bots are alive during goals.

## Behavior model (DefenderAI)

The AI is a single `Update()` loop, layered as follows. All inputs are written server-side directly to `ServerValue` on the NetworkedInput fields — no RPC round-trip.

### 1. Target acquisition

Currently the target is fixed at spawn time to the **client who called `/defender`** (`TargetClientId`). Looked up each frame via `PlayerManager.GetPlayerByClientId`. If the target is null/unspawned, bot zeros its input and idles.

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

## Tuning knobs (all on the DefenderAI component)

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

## Bugs we hit and the fix for each

1. **Patch never fired on `/defender` chat command.** Cause: `PracticeModeDetector.IsPracticeMode` was false in test session — same gate as `/goalies`, so existing commands also wouldn't have worked. Fix: removed the practice-mode gate; the prefix runs whenever the chat command arrives.
2. **Bot looped past the player on every overshoot.** Cause: no lead, pure pursuit. Fix: added lead pursuit (leadTime = dist / LeadDistanceScale).
3. **Bot didn't react to late cuts.** Cause: bot was reading velocity, which lags player rotation by hundreds of ms due to mass. Fix: blend `transform.forward` (facing) into the velocity used for lead. `FacingBias = 0.6`.
4. **Bot panic-crouched at close range even when player went straight.** Cause: `SlideEngageAngle` fired on geometric angle, which spikes at close range due to small lateral offsets. Fix: removed the angular trigger entirely. Slide now only fires on detected cuts or Predict mode commits.
5. **Bot pivoted 180° when player closed head-on at speed.** Cause: lead vector pointed toward the bot (player moving at bot), and `leadVec.magnitude > dist`, so `aimPoint` ended up *behind* the bot's own position. Bot rotated to face the phantom point. Fix: `ClampLead` caps the lead magnitude at `dist × MaxLeadAsDistRatio` so the aim never flies past the bot.

## Next: stick work (research notes)

User-stated goal: the bot uses its stick blade to swat at the puck. Last-touched-by-player puck becomes the focus. Bot anticipates puck motion and positions the blade to intercept.

### What's known after a quick read

- `Stick.BladeHandlePosition` (PuckNew/Stick.cs:95) — world position of the blade. Read-only; driven by physics.
- `StickPositioner.BladeTargetPosition` — the position the stick wants the blade at. PID controller (PuckNew/Stick.cs:217) drives the blade toward this target each FixedUpdate.
- The stick input flow is: `StickRaycastOriginAngleInput` (a Vector2 angular input, range `minimumStickRaycastOriginAngle` to `maximumStickRaycastOriginAngle`) → game raycasts from camera/origin at that angle → hit point becomes `BladeTargetPosition` → PID pulls the blade there.
- Setting the bot's stick from server-side is the same pattern as MoveInput: `input.StickRaycastOriginAngleInput.ServerValue = new Vector2(x, y)`. The values are in radians (or some angular units — verify) and get clamped.
- Additional stick inputs available: `BladeAngleInput` (sbyte, blade rotation), `Client_TwistLeftInputRpc` / `TwistRightInputRpc` (instant twist actions), `ExtendLeft|RightInputRpc(bool)`, `LateralLeft|RightInputRpc(bool)`.
- The bot's PlayerCamera **is** spawned (`Server_SpawnPlayerCamera` runs in `Server_SpawnCharacter`). We can read its transform or potentially move it directly. The raycast origin for the stick almost certainly involves that camera.

### Where the complexity is

The hard part is the **inverse mapping**: given a desired world position for the blade, compute the `(X, Y)` angle input that produces it. The forward mapping (input → raycast → target) is what the game does. We'd need to either:

- (a) read the `StickPositioner` source carefully, derive the inverse analytically (likely some atan2-style math from camera position + facing).
- (b) treat it as a black box and tune the angles empirically — track desired vs actual blade position, integrate the error into the next frame's input. Like a PID on top of the game's PID. Slow but robust.
- (c) bypass the input system entirely on the server and write directly to `StickPositioner.BladeTargetPosition` (if it's writable and the PID still runs from there). Cleanest if it works; might break netcode replication.

### Difficulty estimate

- **Phase 1 — get the stick to track the puck approximately, no swat:** half a day if approach (c) works, 1-2 days for (a), 2-3 days for (b).
- **Phase 2 — anticipate puck motion (same pattern as the player lead with magnitude clamp).** Half a day on top.
- **Phase 3 — decide when to *poke* (one of the discrete actions: twist, extend, lateral) vs just position.** 1-2 days. Includes some game-feel tuning.
- **Phase 4 — last-touched-by-player puck tracking.** Listen for an event like `Event_Server_OnPuckTouched` (or whatever Puck calls it). Half a day of research + plumbing.

Total: realistically a week of focused work for a stick that *feels* like a defender's stick. The hard knob is the same one we hit with movement — it's easy to make it cheaty (perfect reads, always intercepts), takes iteration to make it readable and beatable.

## Future work beyond stick

- **Real defender mode**: gap control, stay between player and own net, only commit when puck-carrier crosses some line. The current "chaser" mode is the wrong primary AI for this; needs a separate `DefenderAI` (real one) with different aim logic.
- **Multi-bot lane scenarios**: spawn 2-3 staggered defenders, each in their own lane. Player has to beat them serially. Requires lane assignment + per-bot zone constraints so they don't all collapse on the puck.
- **Reset to position command**: `/defender reset` to send all bots back to their spawn positions. Useful between practice reps.
- **Replay compatibility**: defender bots aren't covered by the existing replay-recorder patches. If they're alive during a goal, behavior is unverified.
