# Camera System

## CameraManager

Static class — selects and activates the appropriate camera for the current game context.

**Access:** `CameraManager` (static — no `Instance` property)

## BaseCamera / BaseCameraController

`NetworkBehaviour` / `MonoBehaviour` — base class for all camera types. Provides common follow/targeting logic.

## Camera Types

| View Class | Controller | Base | Description |
|---|---|---|---|
| `PlayerCamera` | `PlayerCameraController` | `BaseCamera` | First-person player view |
| `SpectatorCamera` | `SpectatorCameraController` | `BaseCamera` | Free-roam spectator view |
| `ReplayCamera` | `ReplayCameraController` | `BaseCamera` | Automated replay playback camera |
| `LockerRoomCamera` | `LockerRoomCameraController` | `BaseCamera` | Locker room / lobby view |

## CameraType Enum

| Value | Description |
|---|---|
| `Player` | Active player first-person camera |
| `Spectator` | Spectator free camera |
| `Replay` | Replay playback camera |
| `LockerRoom` | Locker room lobby camera |

## Modding Notes

### Switching cameras

Patch `CameraManager` to intercept or override camera selection:

```csharp
[HarmonyPatch(typeof(CameraManager), "SetCamera")]  // verify method name
class Patch_CameraManager
{
    static void Prefix(ref CameraType type)
    {
        // redirect to a different camera type
    }
}
```

### Modifying PlayerCamera behavior

`PlayerCamera` is the most relevant target for first-person view mods (FOV changes, camera shake, head bob):

```csharp
[HarmonyPatch(typeof(PlayerCamera), "SomeMethod")]
class Patch_PlayerCamera
{
    static void Postfix(PlayerCamera __instance)
    {
        // adjust FOV or transform after normal update
    }
}
```

### Replay camera

`ReplayCamera` and `ReplayCameraController` run during `GamePhase.Replay`. If you want a custom replay angle or to skip replays, patch these classes or intercept the phase change in `GameManager`.
