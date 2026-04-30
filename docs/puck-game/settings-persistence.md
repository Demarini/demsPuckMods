# Settings & Persistence

## SettingsManager

Static class — owns all user-facing settings (audio volumes, graphics, controls, appearance). Settings are stored via `SaveManager` and applied at startup.

**Access:** `SettingsManager` (static — no `Instance` property)

### Known Setting Fields

| Field / Property | Description |
|---|---|
| Handedness | `PlayerHandedness` — left, right, or ambidextrous |
| Flag | Selected flag/country ID |
| Headgear IDs | Headgear selections indexed by team and role |
| Jersey IDs | Jersey selections indexed by `JerseyTeam` |
| Stick Skin IDs | Stick skin selections indexed by `StickSkinTeam` |
| Tape IDs | Tape color/pattern selections |

Audio volumes are set via `AudioManager` — `SettingsManager` stores the preference and calls `AudioManager.Set*Volume()` on load.

## SettingsManagerController

Static controller that initializes `SettingsManager`.

## SaveManager

Static class — thin persistence layer over Unity `PlayerPrefs` or a JSON file. Provides typed get/set operations.

**Access:** `SaveManager` (static)

### Key Methods

| Method | Description |
|---|---|
| `GetString(key)` | Read a persisted string value |
| `SetString(key, value)` | Write a string value |
| *(additional typed overloads)* | Likely `GetInt`, `GetFloat`, `GetBool` — verify in source |

### Usage in Mods

`SaveManager` is used by `ModManager` itself to persist the enabled/disabled mod list. Your mod's own config should use the `ModConfig` JSON pattern (config at `<GameDir>/config/<ModName>Config.json`) rather than `SaveManager`, to avoid key collisions.

## ApplicationManager

Static class — manages application-level settings separate from gameplay settings (window mode, resolution, frame rate cap, quality level).

**Access:** `ApplicationManager` (static)

### ApplicationQuality Enum

| Value | Description |
|---|---|
| *(values to verify)* | Low, Medium, High, Ultra — or similar |

## ApplicationManagerController

Static controller that initializes `ApplicationManager`.

## Mod Config Pattern

The conventional pattern for mod persistence (not using `SaveManager`):

```
<GameInstallDir>/config/<ModName>Config.json
<GameInstallDir>/config/<ModName>Config.local.json   ← survives workshop updates
```

```csharp
// ModConfig static class pattern used across mods
public static class ModConfig
{
    private static readonly string _configDir = Path.Combine(
        Application.dataPath, "..", "config");
    private static readonly string _configPath = Path.Combine(
        _configDir, "MyModConfig.json");

    public static ConfigData Data { get; private set; } = new ConfigData();

    public static void Load()
    {
        if (!File.Exists(_configPath)) { Save(); return; }
        Data = JsonConvert.DeserializeObject<ConfigData>(File.ReadAllText(_configPath));
    }

    public static void Save()
    {
        Directory.CreateDirectory(_configDir);
        File.WriteAllText(_configPath, JsonConvert.SerializeObject(Data, Formatting.Indented));
    }
}

public class ConfigData
{
    public bool SomeOption { get; set; } = true;
    public float SomeValue { get; set; } = 1.0f;
}
```

The `.local.json` variant is loaded after the main config, with its values overwriting the main config. This lets server operators or players keep personal overrides that survive mod updates pushing a new default config.
