# Input System

## InputManager

Static class that owns all `InputAction` definitions and registers custom interactions. Uses Unity's `UnityEngine.InputSystem`.

**Access:** `InputManager` (static — no `Instance` property)

### Registered Actions (30+)

| Action Name | Description |
|---|---|
| `Move` | WASD / left stick movement |
| `Stick` | Stick direction input |
| `Blade` | Blade angle adjustment |
| `Slide` | Sliding/gliding |
| `Sprint` | Sprint modifier |
| `Track` | Puck tracking behavior |
| `Look` | Camera/look direction |
| `Jump` | Jump input |
| `Chat` | Open chat |
| *(additional actions)* | Various UI and gameplay inputs |

### Custom Interactions

`InputManager` registers two custom interaction types with Unity's InputSystem on startup:

| Class | Description |
|---|---|
| `DoublePressInteraction` | Detects a double-tap on a binding |
| `ToggleInteraction` | Toggles on/off with each press |

These are registered globally so they can be used in any `InputAction` binding.

## InputManagerController

Static controller that initializes `InputManager`.

## KeyBind

Configuration class representing a player-remappable key binding.

| Member | Type | Description |
|---|---|---|
| Action | string | The InputAction name this binds |
| Path | string | The binding path (e.g. `<Keyboard>/w`) |
| Interaction | `KeyBindInteraction` | Interaction type (normal, double-press, toggle) |

## KeyBindField

`VisualElement` — the UI control rendered in the Settings screen for a single key binding. Handles rebinding UX.

## Enums

### KeyBindInteraction

Enum for the interaction mode of a key binding.

| Value | Description |
|---|---|
| `Normal` | Standard press/release |
| `DoublePress` | Requires double-tap (`DoublePressInteraction`) |
| `Toggle` | Toggles state each press (`ToggleInteraction`) |

### KeyBindInteractionType

Secondary enum for categorizing interaction types — used in UI filtering or display.

## Harmony Patch Notes

`PlayerInput` (not `InputManager`) is the primary Harmony target for intercepting player controls at the per-player level. `InputManager` is better patched if you want to globally change how bindings are resolved or inject new input actions.

```csharp
// Intercept all movement input
[HarmonyPatch(typeof(PlayerInput), "SomeUpdateMethod")]
class Patch_PlayerInput_Move
{
    static void Prefix(PlayerInput __instance, ref Vector2 __result)
    {
        // modify or suppress movement
    }
}
```

## Input System Integration

The game uses `UnityEngine.InputSystem` (the new Input System package), not the legacy `Input` class. Subdirectories in `PuckNew/UnityEngine/InputSystem/` contain:

- `UnityEngine/InputSystem/` — core input system files
- `UnityEngine/InputSystem/Composites/` — composite binding definitions (e.g. WASD composite)

Custom interactions (`DoublePressInteraction`, `ToggleInteraction`) are registered at startup via `InputManager` before any input actions are created.
