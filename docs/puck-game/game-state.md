# Game State

## GameManager

`NetworkBehaviourSingleton<GameManager>` — the central authority for all game state. Lives on the server; state syncs to clients via `NetworkVariable<GameState>`.

**Access:** `GameManager.Instance`

### Synced State Properties

These read directly from the `NetworkVariable<GameState>` and are available on all clients:

| Property | Type | Description |
|---|---|---|
| `Phase` | `GamePhase` | Current game phase |
| `Tick` | int | Server tick count |
| `Period` | int | Current period number |
| `BlueScore` | int | Blue team score |
| `RedScore` | int | Red team score |
| `IsOvertime` | bool | Whether game is in overtime |

### Key Methods

| Method | Description |
|---|---|
| `OnGameStateChanged()` | Fired when `NetworkVariable<GameState>` changes — primary Harmony patch target for reacting to state transitions |

### Harmony Patch Target

```csharp
[HarmonyPatch(typeof(GameManager), "OnGameStateChanged")]
class Patch_GameManager_OnGameStateChanged
{
    static void Postfix(GameManager __instance)
    {
        var phase = __instance.Phase;
        var blueScore = __instance.BlueScore;
    }
}
```

## GameState Struct

`INetworkSerializable`, `IEquatable<GameState>` — the serialized snapshot sent over the network.

```csharp
struct GameState : INetworkSerializable, IEquatable<GameState>
{
    GamePhase Phase;
    int       Tick;
    int       Period;
    int       BlueScore;
    int       RedScore;
    bool      IsOvertime;
}
```

## GamePhase Enum

Controls the flow of a match. Transitions happen server-side; clients react via `OnGameStateChanged()`.

| Value | Description |
|---|---|
| `None` | No active game state |
| `Warmup` | Pre-match warmup period |
| `PreGame` | Just before game starts |
| `FaceOff` | Face-off at center ice |
| `Play` | Active gameplay |
| `BlueScore` | Blue team just scored |
| `RedScore` | Red team just scored |
| `Replay` | Goal replay playing |
| `Intermission` | Between periods |
| `GameOver` | Match ended |
| `PostGame` | Post-game results/lobby |

### Typical Phase Flow

```
Warmup → PreGame → FaceOff → Play → [BlueScore|RedScore] → Replay → FaceOff
                                   → Intermission → FaceOff
                                   → GameOver → PostGame
```

## GlobalStateManager

Static — manages high-level application state independent of a specific game session.

| Method | Description |
|---|---|
| `SetConnectionState(state)` | Updates the global connection state |
| `SetGlobalState(state)` | Transitions the global application state |

## GameModeManager

`MonoBehaviourSingleton<GameModeManager>` — owns which game mode is active and switches between them (Standard, Competitive, Public).

**Access:** `GameModeManager.Instance`

## GameResult

Data structure representing the final outcome of a completed match — see also [game-modes.md](game-modes.md).
