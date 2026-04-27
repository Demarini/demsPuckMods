# Gameplay Mechanics

## Stick

`NetworkBehaviour` — the hockey stick. Synchronized via `SynchronizedObject` (same as Puck). One instance per player.

### Key Notes

- Stick physics are network-synced via the `SynchronizedObject` base class
- Stick-puck collision is a frequent interaction point for gameplay mods
- Visual appearance is controlled by `StickMesh`, `StickSkin`, and `StickTape`

### Related Classes

| Class | Base | Description |
|---|---|---|
| `StickController` | `NetworkBehaviour` | Control logic for the stick |
| `StickPositioner` | `NetworkBehaviour` | Calculates and sets stick world position |
| `StickPositionerController` | `NetworkBehaviour` | Controller for `StickPositioner` |
| `StickMesh` | `MonoBehaviour` | Visual mesh for the stick model |
| `StickSkin` | — | Stick appearance asset |
| `StickTape` | — | Tape color/pattern on the stick |

### Enums

| Enum | Values | Description |
|---|---|---|
| `StickSkinTeam` | Blue, Red | Which team variant of a stick skin |

### Harmony Patch Example

```csharp
// Intercept stick positioning
[HarmonyPatch(typeof(StickPositioner), "SomeUpdateMethod")]
class Patch_StickPositioner
{
    static void Postfix(StickPositioner __instance)
    {
        // modify stick position after the game sets it
    }
}
```

## Goal

`MonoBehaviour` — the goal object (net). One per end of the rink.

### Related Classes

| Class | Base | Description |
|---|---|---|
| `GoalController` | `MonoBehaviour` | Logic controller for the goal |
| `GoalTrigger` | `MonoBehaviour` | Trigger zone that detects puck entry |
| `GoalNetCollider` | `MonoBehaviour` | The physical net collider |

### Harmony Patch Example — Goal Detection

```csharp
[HarmonyPatch(typeof(GoalTrigger), "OnTriggerEnter")]
class Patch_GoalTrigger
{
    static void Prefix(GoalTrigger __instance, Collider other)
    {
        // fires just before a goal is registered
    }
}
```

## Level

`NetworkBehaviour` — the arena/rink level object. Likely holds references to ice, boards, lights, and environmental anchors.

### Related Class

| Class | Base | Description |
|---|---|---|
| `LevelController` | `MonoBehaviour` | Controller for level events |

## Movement

`MonoBehaviour` — general entity movement controller. Used for player skating locomotion.

## Skate

`MonoBehaviour` — skating-specific movement logic. Handles ice friction, edge control, and skating speed calculations.

## MoveAlongSpline

`MonoBehaviour` — moves an entity along a spline curve. Used for cinematic cameras or automated path following.

## Equipment & Cosmetics

### Headgear

`MonoBehaviour` — helmet and headgear management component. Attached to the player head transform.

### Jersey

`MonoBehaviour` — jersey visual component. Uses `JerseyTeam` to select the correct team texture.

| Enum | Description |
|---|---|
| `JerseyTeam` | Blue or Red — selects which jersey variant is loaded |

### Beard / Mustache

`MonoBehaviour` — facial hair visual components. Cosmetic only.

## SoftCollider

`MonoBehaviour` — a "soft" trigger/collider that produces dampened physics responses rather than hard bounces. Used to smooth player-to-player contact.

## GameResult

Data structure representing the final match result (winner, scores, period data). Populated at `GamePhase.GameOver` and passed to post-game screens.

## Modding Notes

- **Stick radius / hit zone mods:** Patch `StickPositioner` or the stick's collider configuration
- **Goal detection hooks:** Patch `GoalTrigger.OnTriggerEnter` or `GoalController` methods
- **Skating feel mods:** Patch `Skate` or `Movement`
- **Arena/scenery mods:** Target `Level` and `LevelController`, or use the `SceneryChanger` mod pattern already in this solution
