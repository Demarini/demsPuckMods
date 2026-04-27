# Puck & Physics

## Puck

`NetworkBehaviour` — the hockey puck. Synchronized via `SynchronizedObject`.

### Key Properties

| Property | Type | Description |
|---|---|---|
| `PredictedSpeed` | float | Client-side predicted linear speed |
| `PredictedAngularSpeed` | float | Client-side predicted spin rate |
| `ShotSpeed` | float | Speed at which the puck was last shot |
| `IsGrounded` | bool | Whether the puck is on the ice surface |

### Colliders

| Property | Type | Description |
|---|---|---|
| `NetSphereCollider` | Collider | Collider for goal net detection |
| `StickCollider` | Collider | Collider for stick interactions |
| `IceCollider` | Collider | Collider for ice surface |

### Patch Target Notes

- Patch `Puck` methods to intercept shots, deflections, or puck-net collisions
- `ShotSpeed` is a good read target for detecting hard shots
- `IsGrounded` can distinguish aerial passes from ice-level play

## PuckManager

`MonoBehaviourSingleton<PuckManager>` — manages the single puck instance and its physics state.

**Access:** `PuckManager.Instance`

## PuckController

`MonoBehaviour` — control logic layer on top of `Puck`.

## PuckPosition / PuckPositionController

`NetworkBehaviour` — position synchronization for the puck.

## PuckElevationIndicator / PuckElevationIndicatorController

`MonoBehaviour` — the in-game UI indicator showing puck height above ice. Useful target if you want to modify or suppress the elevation display.

## PuckCollisionDetectionModeSwitcher

`MonoBehaviour` — dynamically switches between Unity collision detection modes (Discrete/Continuous) based on puck speed. Prevents tunneling at high velocities.

## SynchronizedObject

`NetworkBehaviour` — base class for all physics objects that need network synchronization (Puck, Stick). Manages prediction and snapshot interpolation.

### Key Properties

| Property | Type | Description |
|---|---|---|
| `PredictedLinearVelocity` | `Vector3` | Current predicted velocity |
| `PredictedAngularVelocity` | `Vector3` | Current predicted angular velocity |

Snapshot data is stored internally for replay and interpolation.

## SynchronizedObjectManager

`NetworkBehaviourSingleton<SynchronizedObjectManager>` — server-side manager for all `SynchronizedObject` instances. Coordinates snapshot collection for replay.

**Access:** `SynchronizedObjectManager.Instance`

## SynchronizedObjectData

`INetworkSerializable` struct — compact snapshot of a synchronized object's physical state at a given tick.

## SynchronizedObjectsSnapshot

Struct holding a full frame snapshot of all synchronized objects — used by the replay system.

## PhysicsManager

`MonoBehaviourSingleton<PhysicsManager>` — configures global Unity physics settings (gravity, fixed timestep, layer collision matrix, etc.).

**Access:** `PhysicsManager.Instance`

## CollisionRecorder / NetworkObjectCollisionRecorder

Tracks which objects collided with what, used internally for replay data and potentially for scoring logic.

## Physics Utility Components

### PIDController / Vector3PIDController

PID (Proportional-Integral-Derivative) controller implementations for smooth physics control. Used for stick and puck stabilization.

### SmoothPositioner

`MonoBehaviour` — smoothly interpolates a transform toward a target position. Used on player bodies and equipment.

### KeepUpright

`MonoBehaviour` — applies torque to keep a Rigidbody upright. Used on player bodies to prevent tipping over.

### VelocityLean

`MonoBehaviour` — tilts an object based on its velocity direction and magnitude. Used on the puck for visual spin effect.

### ExponentialMovingAverage

`struct` — utility for computing EMA of a float value. Used internally for smoothing speed/velocity readings.

## Harmony Patch Examples

### Intercept puck shots

```csharp
[HarmonyPatch(typeof(Puck), nameof(Puck.ShotSpeed), MethodType.Setter)]
class Patch_Puck_ShotSpeed
{
    static void Postfix(Puck __instance, float value)
    {
        if (value > 30f)
            Debug.Log($"Hard shot: {value} m/s");
    }
}
```

### React to puck grounding

```csharp
[HarmonyPatch(typeof(Puck), nameof(Puck.IsGrounded), MethodType.Setter)]
class Patch_Puck_IsGrounded
{
    static void Postfix(Puck __instance, bool value)
    {
        // value just became true: puck landed on ice
    }
}
```
