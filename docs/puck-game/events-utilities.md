# Events & Utilities

## EventManager

Static class — decoupled publish/subscribe event bus. Supports both local events and network-routed events.

**Access:** `EventManager` (static)

### Key Methods

| Method | Description |
|---|---|
| `AddEventListener<T>(handler)` | Subscribe to events of type `T` |
| `RemoveEventListener<T>(handler)` | Unsubscribe |
| `TriggerEvent<T>(eventData)` | Fire an event to all subscribers |

### Network Event Naming Convention

Event classes prefixed with routing intent:

| Prefix | Routing |
|---|---|
| `Event_Server_` | Only handled on server |
| `Event_Client_` | Only handled on clients |
| `Event_Everyone_` | Handled on all peers |

### Built-in Event Classes

All extend `EventBase<T>`:

| Class | Description |
|---|---|
| `ChildAddedEvent` | A child was added to a hierarchy node |
| `ChildRemovedEvent` | A child was removed |
| `BeforeChildRemovedEvent` | Fires just before removal |
| `HierarchyChangedEvent` | General hierarchy structure change |
| `RenderingToggledEvent` | A renderer was enabled or disabled |

### HierarchyChangeType Enum

| Value | Description |
|---|---|
| *(values to verify)* | ChildAdded, ChildRemoved, etc. |

### Usage in Mods

```csharp
// Subscribe in OnEnable
EventManager.AddEventListener<SomeGameEvent>(OnSomeEvent);

// Unsubscribe in OnDisable (always clean up!)
EventManager.RemoveEventListener<SomeGameEvent>(OnSomeEvent);

private void OnSomeEvent(SomeGameEvent e)
{
    // react to event
}
```

## Lifecycle & Threading

### LifecycleManager

Static — manages ordered initialization and teardown of game systems. Less relevant for mods; Harmony patches fire independently of this.

### ThreadManager

`MonoBehaviourSingleton<ThreadManager>` — dispatches work to background threads and marshals callbacks back to the Unity main thread.

**Access:** `ThreadManager.Instance`

Useful in mods that do async I/O (config reloading, WebSocket connections, etc.) — use `ThreadManager` to safely call Unity APIs from a callback.

### TimeoutManager

`MonoBehaviourSingleton<TimeoutManager>` — tracks countdown timers and fires callbacks on expiry. Used by `ServerManager` for client idle timeouts.

**Access:** `TimeoutManager.Instance`

## Logging

### LogManager

Static — game's internal logging wrapper. Less relevant for mods; `Debug.Log` / `Debug.LogWarning` / `Debug.LogError` are standard mod logging.

**Access:** `LogManager` (static)

## Scene Management

### SceneManager / SceneManagerController

Static — wraps Unity's scene loading. Used to transition between menus and game arena scenes.

### PatchManager

Static — manages game version patches/updates (not Harmony patches). Applies data migration or config updates when the game version changes.

## Utility Classes

| Class | Description |
|---|---|
| `Utils` | General-purpose static helpers (math, string, collection) |
| `UIUtils` | UI-specific helpers (VisualElement queries, formatting) |
| `ConfigUtils` | Helpers for reading/writing config files |
| `CountryUtils` | Country name/code lookup |
| `Constants` | Game-wide constant values (field dimensions, timing, etc.) |

`Constants` is particularly useful in mods — check it before hardcoding magic numbers that the game itself defines (rink dimensions, max player count, tick rate, etc.).

## Beacon

`MonoBehaviour` — likely a positional anchor or waypoint marker component. Verify usage in decompiled source.

## Singleton Base Classes

The two singleton patterns used throughout the codebase:

```csharp
// Standard Unity singleton (accessible on client and server)
class SomeManager : MonoBehaviourSingleton<SomeManager>
{
    // Accessible via SomeManager.Instance
}

// Network-aware singleton (server-authoritative)
class SomeNetManager : NetworkBehaviourSingleton<SomeNetManager>
{
    // Accessible via SomeNetManager.Instance
    // Only fully functional when IsServer is true
}
```

## PostProcessing / PostProcessingController

`MonoBehaviour` — Unity post-processing volume control. Controls visual effects like bloom, depth of field, color grading.

Patch `PostProcessing` if you want to add/modify visual effects (e.g. a "night mode" or color-blind assist mod).

## MatchData / PoolStatistics / ServerManagerStatistics / MatchmakingManagerStatistics

Data structures for telemetry and display:

| Class | Description |
|---|---|
| `MatchData` | Details of a completed or in-progress match |
| `PoolStatistics` | Matchmaking pool data (queue size, wait times) |
| `ServerManagerStatistics` | Server performance/connection stats |
| `MatchmakingManagerStatistics` | Matchmaking system stats |
