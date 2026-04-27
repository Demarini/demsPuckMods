# Player System

## Player

`NetworkBehaviour` — the base networked entity for every player in the game. One instance per connected client.

### State Properties

| Property | Type | Description |
|---|---|---|
| `Phase` | `PlayerPhase` | Current phase for this player |
| `Team` | `PlayerTeam` | Which team the player is on |
| `Role` | `PlayerRole` | Attacker or Goalie |
| `ChatTickets` | int | Remaining chat messages allowed |
| `IsChatAvailable` | bool | Whether player can currently chat |

### Customization Properties

| Property | Description |
|---|---|
| `Flags` | Flag/country selection |
| `Headgear` | Helmet/headgear selection per team/role |
| `Jersey` | Jersey selection per team |
| `StickSkin` | Stick skin per team |
| `Tape` | Tape color selection |

### Key Methods

Methods exist for managing team assignment, role, and position — typically called server-side or via RPC.

### Harmony Patch Example

```csharp
[HarmonyPatch(typeof(Player), nameof(Player.Phase), MethodType.Setter)]
class Patch_Player_PhaseChanged
{
    static void Postfix(Player __instance, PlayerPhase value)
    {
        // react to player phase change
    }
}
```

## PlayerManager

`MonoBehaviourSingleton<PlayerManager>` — tracks all active player instances.

**Access:** `PlayerManager.Instance`

### Key Methods

| Method | Description |
|---|---|
| `GetPlayerById(id)` | Fetch a player by their client ID |
| `GetPlayersByTeam(team)` | All players on a given team |

## PlayerInput

`NetworkBehaviour` — processes and synchronizes player input. One of the most frequently patched classes for gameplay mods.

### Key Areas

- **Raycasting** — determines where the stick blade is pointed
- **Look angles** — head/camera orientation
- **Blade angles** — stick blade rotation
- **Input constraints** — clamping/limiting input ranges

### Harmony Patch Pattern

```csharp
[HarmonyPatch(typeof(PlayerInput), "SomeMethod")]
class Patch_PlayerInput
{
    static bool Prefix(PlayerInput __instance)
    {
        // return false to suppress original, true to allow
        return true;
    }
}
```

## PlayerController

`NetworkBehaviour` — higher-level player control logic built on top of `PlayerInput`.

## PlayerBody

`NetworkBehaviour` — represents the physical body of a player in the world. Handles body physics and collision.

## PlayerBodyController

`NetworkBehaviour` — controller paired with `PlayerBody`.

## PlayerPosition / PlayerPositionController

`NetworkBehaviour` — tracks and synchronizes the player's world position.

## Body Part Components

Individual `NetworkBehaviour` components for body segments:

| Class | Description |
|---|---|
| `PlayerHead` | Head/helmet |
| `PlayerTorso` | Torso/chest |
| `PlayerGroin` | Groin/lower body |
| `PlayerLegPad` | Leg pads (goalie) |
| `PlayerMesh` | Visual mesh wrapper |

## Enums

### PlayerTeam

| Value | Description |
|---|---|
| `None` | No team assigned |
| `Blue` | Blue team |
| `Red` | Red team |

### PlayerRole

| Value | Description |
|---|---|
| `Attacker` | Forward/skater position |
| `Goalie` | Goaltender position |

### PlayerPhase

| Value | Description |
|---|---|
| `None` | No phase |
| `TeamSelect` | Choosing a team |
| `PositionSelect` | Choosing attacker/goalie |
| `Play` | Active in game |
| `Replay` | Watching replay |
| `Spectate` | Spectating |

### PlayerHandedness

| Value | Description |
|---|---|
| `Left` | Left-handed |
| `Right` | Right-handed |
| `Ambidextrous` | Both hands |

### PlayerLegPadState

States for goalie leg pad positioning.

### HeadgearRole

Enum used to select correct headgear model per player role.

## Data Structures

### PlayerGameState

`INetworkSerializable` struct — compact serialized snapshot of a player's game-relevant state.

```csharp
struct PlayerGameState : INetworkSerializable
{
    PlayerPhase Phase;
    PlayerTeam  Team;
    PlayerRole  Role;
}
```

### PlayerCustomizationState

`INetworkSerializable` struct — full appearance state sent during connection.

```csharp
struct PlayerCustomizationState : INetworkSerializable
{
    string   FlagID;
    string[] HeadgearIDs;    // indexed by HeadgearRole
    string   MustacheID;
    string   BeardID;
    string[] JerseyIDs;      // indexed by JerseyTeam
    string[] StickSkinIDs;   // indexed by StickSkinTeam
    string[] TapeIDs;
}
```

### PlayerStatistics / MatchPlayer / PlayerMatchData

Classes for tracking in-match and historical stats per player. Used by `PlayerManagerStatistics`.

### PlayerCooldown / PlayerMute

Tracking classes for chat cooldowns and voice/chat mutes.

### PlayerGroupData / PlayerPartyData

Group/party data structures used by the matchmaking and lobby systems.
