# Enums Reference

All 29 enums found in the PuckNew decompiled source. Values marked *(verify)* are inferred from context — confirm against the decompiled file before patching.

---

## AppearanceCategory

Categories in the cosmetic appearance UI.

```csharp
enum AppearanceCategory
{
    // verify: Headgear, Jersey, Stick, Flag, Tape, ...
}
```

## AppearanceSubcategory

Sub-categories within each `AppearanceCategory` (e.g. team-specific variants).

```csharp
enum AppearanceSubcategory { /* verify */ }
```

## ApplicationQuality

Graphics quality preset.

```csharp
enum ApplicationQuality
{
    // verify: Low, Medium, High, Ultra
}
```

## AuthenticationPhase

Stages of the client authentication handshake.

```csharp
enum AuthenticationPhase
{
    // verify: Connecting, Authenticating, Approved, Rejected
}
```

## AvatarSize

Resolution for avatar/flag image requests.

```csharp
enum AvatarSize
{
    // verify: Small, Medium, Large
}
```

## CameraType

Which camera is active.

```csharp
enum CameraType
{
    Player,
    Spectator,
    Replay,
    LockerRoom,
}
```

## ConnectionRejectionCode

Why a client connection was rejected.

```csharp
enum ConnectionRejectionCode
{
    // verify: WrongPassword, ServerFull, Banned, MissingRequiredMods, ...
}
```

## DerivativeMeasurement

Used by PID controllers — which derivative measurement mode to use.

```csharp
enum DerivativeMeasurement
{
    // verify: Velocity, ErrorRateOfChange
}
```

## DisconnectionCode

Why a client was disconnected mid-session.

```csharp
enum DisconnectionCode
{
    // verify: Timeout, Kicked, Banned, ServerClosed, ...
}
```

## EdgegapDependency

Dependency tags for Edgegap cloud server configuration.

```csharp
enum EdgegapDependency { /* verify */ }
```

## GamePhase

Current phase of the game. Primary driver of match flow.

```csharp
enum GamePhase
{
    None,
    Warmup,
    PreGame,
    FaceOff,
    Play,
    BlueScore,
    RedScore,
    Replay,
    Intermission,
    GameOver,
    PostGame,
}
```

## HeadgearRole

Which role's headgear variant to load (attacker helmet vs goalie mask).

```csharp
enum HeadgearRole
{
    // verify: Attacker, Goalie
}
```

## HierarchyChangeType

Type of change that triggered a `HierarchyChangedEvent`.

```csharp
enum HierarchyChangeType
{
    // verify: ChildAdded, ChildRemoved
}
```

## JerseyTeam

Which team's jersey variant to load.

```csharp
enum JerseyTeam
{
    Blue,
    Red,
}
```

## KeyBindInteraction

Interaction type for a remappable key binding.

```csharp
enum KeyBindInteraction
{
    Normal,
    DoublePress,
    Toggle,
}
```

## KeyBindInteractionType

Secondary categorization of key bind interaction — used in UI filtering.

```csharp
enum KeyBindInteractionType { /* verify */ }
```

## PlayerHandedness

Player's preferred stick hand.

```csharp
enum PlayerHandedness
{
    Left,
    Right,
    Ambidextrous,
}
```

## PlayerLegPadState

Current state of a goalie's leg pads.

```csharp
enum PlayerLegPadState { /* verify: Standing, Butterfly, ... */ }
```

## PlayerPhase

Current lifecycle phase of an individual player.

```csharp
enum PlayerPhase
{
    None,
    TeamSelect,
    PositionSelect,
    Play,
    Replay,
    Spectate,
}
```

## PlayerRole

Role in the current game session.

```csharp
enum PlayerRole
{
    Attacker,
    Goalie,
}
```

## PlayerTeam

Team assignment.

```csharp
enum PlayerTeam
{
    None,
    Blue,
    Red,
}
```

## QuickChatCategory

Category for a quick-chat message shortcut.

```csharp
enum QuickChatCategory { /* verify: Callouts, Reactions, Team, ... */ }
```

## SpinnerDirection

Which way the loading spinner spins.

```csharp
enum SpinnerDirection
{
    Clockwise,
    CounterClockwise,
}
```

## StickSkinTeam

Which team's stick skin variant to load.

```csharp
enum StickSkinTeam
{
    Blue,
    Red,
}
```

## TCPServerMessageType

Type tag for TCP messages between server and client (server browser data).

```csharp
enum TCPServerMessageType
{
    // verify: PreviewRequest, PreviewResponse
}
```

## TransactionPhase

Phase of a shop purchase transaction.

```csharp
enum TransactionPhase
{
    // verify: Idle, Pending, Complete, Failed
}
```

## UIPhase

Top-level UI state.

```csharp
enum UIPhase { /* verify: MainMenu, InGame, Lobby, ... */ }
```

## Units

Measurement units — used by PID controller configuration.

```csharp
enum Units { /* verify: Degrees, Radians */ }
```

## VoteType

Type of player-initiated vote.

```csharp
enum VoteType { /* verify: Kick, ChangeMap, ... */ }
```
