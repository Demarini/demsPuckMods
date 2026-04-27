# Replay System

The replay system records a full snapshot of all moving objects every tick during `GamePhase.Play`, then plays them back during `GamePhase.Replay` after a goal.

## ReplayManager

`MonoBehaviourSingleton<ReplayManager>` — orchestrates replay recording and playback.

**Access:** `ReplayManager.Instance`

## ReplayRecorder / ReplayRecorderController

`MonoBehaviour` — records each tick's state into replay data structures. Writes `ReplayPlayerMove`, `ReplayPuckMove`, `ReplayStickMove`, etc. every frame.

## ReplayPlayer

`MonoBehaviour` — reads recorded replay data and drives object positions/animations during playback.

## Data Structures

### Player Events

| Struct | Description |
|---|---|
| `ReplayPlayerSpawned` | Records when a player entered the game (tick, team, role, customization) |
| `ReplayPlayerDespawned` | Records when a player left |
| `ReplayPlayerMove` | Per-tick position/rotation snapshot for a player |
| `ReplayPlayerInput` | Per-tick input state snapshot |

### Player Body Events

| Struct | Description |
|---|---|
| `ReplayPlayerBodySpawned` | Body spawn event |
| `ReplayPlayerBodyDespawned` | Body despawn event |
| `ReplayPlayerBodyMove` | Per-tick body transform snapshot |

### Puck Events

| Struct | Description |
|---|---|
| `ReplayPuckSpawned` | Puck spawn event |
| `ReplayPuckDespawned` | Puck despawn event |
| `ReplayPuckMove` | Per-tick puck position/velocity snapshot |

### Stick Events

| Struct | Description |
|---|---|
| `ReplayStickSpawned` | Stick spawn event |
| `ReplayStickDespawned` | Stick despawn event |
| `ReplayStickMove` | Per-tick stick position/rotation snapshot |

## SynchronizedObjectsSnapshot

Used internally by `SynchronizedObjectManager` — a full-frame snapshot of all `SynchronizedObject` states at a given tick. Feeds into replay recording.

## Replay Lifecycle

```
Play phase (every tick)
  → ReplayRecorder captures:
      ReplayPlayerMove, ReplayStickMove, ReplayPuckMove, etc.
      SynchronizedObjectsSnapshot

Goal scored (Phase → BlueScore or RedScore)
  → Phase transitions to Replay

Replay phase
  → ReplayPlayer reads stored structs
  → Drives objects back through recorded positions
  → ReplayCamera follows the action

Replay ends
  → Phase transitions to FaceOff
```

## Modding Notes

- **Skip replays:** Patch `ReplayManager` or intercept `GamePhase.Replay` in `GameManager.OnGameStateChanged()` and immediately advance the phase
- **Extend replay duration:** Patch `ReplayPlayer` and delay its completion callback
- **React to replay start/end:** Hook `GameManager.OnGameStateChanged()` and check for `GamePhase.Replay` transitions
- **Add data to replay:** The most robust approach is patching `ReplayRecorder` to capture additional state alongside the built-in structs
