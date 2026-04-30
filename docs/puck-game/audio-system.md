# Audio System

## AudioManager

`MonoBehaviourSingleton<AudioManager>` — controls all audio output via an `AudioMixer` with five named channels.

**Access:** `AudioManager.Instance`

### Volume Channels

| Channel | Method | Description |
|---|---|---|
| Global | `SetGlobalVolume(float)` | Master volume multiplier |
| Ambient | `SetAmbientVolume(float)` | Background ambient sounds |
| Game | `SetGameVolume(float)` | In-game sound effects (puck, skates, crowd) |
| Voice | `SetVoiceVolume(float)` | Player voice chat |
| UI | `SetUIVolume(float)` | Menu and UI click sounds |

Volume parameters are float values — check the `AudioMixer` asset for the exact dB range expected.

### Harmony Patch Example

```csharp
// Intercept ambient muting (e.g. mute ambient when scoring)
[HarmonyPatch(typeof(AudioManager), nameof(AudioManager.SetAmbientVolume))]
class Patch_AudioManager_Ambient
{
    static void Prefix(ref float volume)
    {
        // override or log volume changes
    }
}
```

## AudioManagerController

`MonoBehaviour` — initializes `AudioManager`. No mod-relevant surface area; exists purely to attach the manager to the scene.

## SynchronizedAudio

`NetworkBehaviour` — plays audio clips in a network-synchronized way so all clients hear sounds at the same logical time.

### Usage

Audio events that need to be consistent across all clients (goal horns, face-off whistles, etc.) are triggered through `SynchronizedAudio` rather than a plain `AudioSource`. Harmony-patching this class lets you intercept networked audio events.

## SynchronizedAudioController

`NetworkBehaviour` — controller paired with `SynchronizedAudio`.

## Mod Patterns

### Muting a channel

```csharp
// In OnEnable
AudioManager.Instance.SetAmbientVolume(0f);

// In OnDisable — restore saved value
AudioManager.Instance.SetAmbientVolume(_savedAmbientVolume);
```

### Reacting to game audio events

Patch `SynchronizedAudio` if you need to react to specific sound events (e.g. suppress or replace the goal horn):

```csharp
[HarmonyPatch(typeof(SynchronizedAudio), "PlayClip")]  // verify method name in decompiled source
class Patch_SynchronizedAudio_Play
{
    static bool Prefix(SynchronizedAudio __instance)
    {
        // return false to suppress, true to allow
        return true;
    }
}
```

## Notes

- The `AudioManager` settings are separate from the per-mod config JSON — they represent the player's global preferences and should be saved/restored if a mod changes them temporarily.
- The `SettingsManager` stores the player's preferred volumes; `AudioManager` applies them. When in doubt, read the user's preference from `SettingsManager` rather than hardcoding a value.
