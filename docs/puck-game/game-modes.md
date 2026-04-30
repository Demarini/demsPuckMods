# Game Modes

## IGameMode

Internal interface — all game mode implementations must satisfy this contract.

```csharp
interface IGameMode
{
    bool IsInitialized { get; }
    void Initialize();
    void Dispose();
}
```

## Class Hierarchy

```
IGameMode
└── BaseGameMode<TConfig>
    └── StandardGameMode<TConfig>
        ├── MatchableGameMode<TConfig>      ← can participate in matchmaking
        │   └── CompetitiveGameMode<TConfig>  ← ranked mode
        └── PublicGameMode<TConfig>           ← casual/public servers
```

## Concrete Mode Classes

| Class | Description |
|---|---|
| `BaseGameMode<TConfig>` | Abstract base — game loop logic, phase transitions, scoring |
| `StandardGameMode<TConfig>` | Full game rules (periods, overtime, faceoffs) |
| `MatchableGameMode<TConfig>` | Adds matchmaking queue integration |
| `CompetitiveGameMode<TConfig>` | Ranked — MMR changes, stricter rules |
| `PublicGameMode<TConfig>` | Casual/open servers — relaxed rules, no MMR |

## Configuration Classes

Each mode has a typed config class:

| Class | Description |
|---|---|
| `BaseGameModeConfig` | Common settings (period length, score limit, overtime rules) |
| `StandardGameModeConfig` | Standard-specific settings |
| `CompetitiveGameModeConfig` | Competitive-specific settings (MMR deltas, queue requirements) |
| `PublicGameModeConfig` | Public-specific settings (max players, password) |

## GameModeManager

`MonoBehaviourSingleton<GameModeManager>` — holds the active `IGameMode` instance and switches modes when a new session starts.

**Access:** `GameModeManager.Instance`

## GameResult

Data class populated at the end of a game. Contains:

- Winner team
- Final score (blue / red)
- Period breakdown
- `PlayerResult` per player (goals, assists, saves, etc.)

## Modding Notes

### Hooking game mode events

Game mode phase transitions go through `GameManager` — patch `OnGameStateChanged()` rather than patching inside the game mode class directly, since the game mode class is generic and harder to target precisely.

### Knowing which mode is active

```csharp
// Access the active game mode through GameModeManager
var mode = GameModeManager.Instance;
// Check the concrete type or config to determine mode
```

### Competitive vs Public

Mods that affect match balance (score manipulation, player advantages) should check whether the current session is `CompetitiveGameMode` and disable themselves if so, to avoid affecting ranked play unfairly.
