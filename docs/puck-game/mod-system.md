# Mod System

## IPuckMod Interface

Every mod must implement `IPuckMod`. The game's `ModManagerV2` uses reflection to find the first implementing class in the loaded assembly.

```csharp
public interface IPuckMod
{
    bool OnEnable();   // called when mod loads
    bool OnDisable();  // called when mod unloads
}
```

### Standard Harmony Pattern

```csharp
public class MyMod : IPuckMod
{
    private Harmony _harmony;

    public bool OnEnable()
    {
        _harmony = new Harmony("com.author.ModName");
        _harmony.PatchAll();
        return true;
    }

    public bool OnDisable()
    {
        _harmony.UnpatchSelf();
        return true;
    }
}
```

## ModManager

`MonoBehaviourSingleton<ModManager>` — manages the full lifecycle of all installed mods.

**Access:** `ModManager.Instance`

### Key Properties

| Property | Type | Description |
|---|---|---|
| `InstalledModIds` | collection | All mod IDs found in Plugins directory |
| `EnabledModIds` | collection | Currently active mod IDs |
| `DisabledModIds` | collection | Installed but inactive mod IDs |
| `PendingModIds` | collection | Mods queued for enable/disable |

### Key Methods

| Method | Description |
|---|---|
| `LoadPlugins()` | Scans Plugins directory and loads assemblies |
| `AddMod(mod)` | Registers a new mod |
| `RemoveMod(mod)` | Removes a mod from tracking |
| `EnableMod(id)` | Enables a mod by ID |
| `DisableMod(id)` | Disables a mod by ID |

### State Persistence

`ModManager` uses `SaveManager` to persist which mods are enabled/disabled across sessions.

## Mod Class

`Mod` (implements `INotifyPropertyChanged`) — wraps a single mod's assembly and `IPuckMod` instance.

### Key Members

| Member | Type | Description |
|---|---|---|
| `InstalledItem` | `InstalledItem` | Metadata: ID, path, Steam details |
| `IsEnabled` | `bool` | Current enabled state |
| `IsAssemblyMod` | `bool` | True if loaded from a DLL assembly |
| `IsPlugin` | `bool` | True if a game plugin (vs workshop item) |
| `PreviewTexture` | `Texture2D` | Workshop preview image |

### Assembly Loading

`Mod` dynamically loads the assembly at the path specified by `InstalledItem.Path`, then uses reflection to find and instantiate the `IPuckMod` implementor.

## InstalledItem

Data class (implements `INotifyPropertyChanged`) representing a mod on disk or from Steam Workshop.

| Property | Type | Description |
|---|---|---|
| `Id` | string | Unique mod identifier (Steam Workshop ID or local) |
| `Path` | string | Filesystem path to the mod DLL |
| `ItemDetails` | `ItemDetails` | Steam Workshop metadata (title, description, preview URL) |

## ModConfig

Configuration data class for a specific mod — not to be confused with the per-mod `ModConfig` JSON pattern used in individual mods.

## Deployment Path

Mods are placed in: `<GameInstallDir>/Plugins/<ModName>/<ModName>.dll`

The game scans all subdirectories of `Plugins/` on startup.
