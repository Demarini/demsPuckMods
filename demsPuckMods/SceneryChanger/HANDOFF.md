# SceneryLoader Handoff Document

Last updated: 2026-04-30

## Overview

SceneryLoader is a Puck mod that replaces the default hangar arena with custom scenes built from Unity asset bundles. It consists of two projects:

1. **Unity Editor project** (`D:\UnityProjects\PuckAssetBundles`) — tools for building, encrypting, and configuring asset bundles
2. **C# mod project** (`C:\Users\Tom\source\repos\Demarini\demsPuckMods\demsPuckMods\SceneryChanger`) — the runtime mod that loads bundles into the game

The mod ships as `SceneryChanger.dll` and gets deployed to `<PuckInstall>\Plugins\SceneryLoader\`.

---

## Project Structure

### Mod Project (SceneryChanger)

```
SceneryChanger/
  Initialization.cs              — IPuckMod entry point, installs Harmony patches and behaviors
  Behaviors/
    CoroutineRunner.cs            — DontDestroyOnLoad MonoBehaviour for running coroutines
    DetectGameState.cs            — Detects game state transitions
    MuteAmbience.cs               — Legacy ambient muting
    RinkOnlyPruner.cs             — Strips default hangar, loads encrypted bundles, manages scene lifecycle
    UpdateAudioSources.cs         — Continuously adjusts StadiumNoise and GoalCrowdNoise volumes
  Config/
    ModConfig.cs                  — Resolves config file paths (pointer file, local config)
  Helpers/
    ArenaLightUtil.cs             — Light discovery utilities
    GetAudio.cs                   — Finds AudioSources by name in the scene
  Model/
    AssetInformation.cs           — Per-bundle settings (glass, ambient, music, audio, goal noise)
    BundleResolveResult.cs        — Return type from BundleResolver
    SceneInformation.cs           — Which bundle/prefab/skybox to load (stored in config, sent over network)
    SceneSignature.cs             — Deduplication key for scene loads
  Patches/
    LevelManagerPatch.cs          — Hooks into level management
    PracticeModeDetector.cs       — Detects practice mode vs multiplayer
    SceneryCommandPatch.cs        — /sl chat commands for runtime volume control
    UIChatControllerPatch.cs      — Intercepts !LoadMap chat messages, syncs scenes to connecting clients
  Services/
    AbxCacheDecryptor.cs          — Decrypts .abx bundles to a temp cache
    AbxLoader.cs                  — Loads .abx encrypted bundles
    AudioTweaks.cs                — Disables default ambient audio
    BundleLoader.cs               — Loads and instantiates prefabs from asset bundles
    BundleResolver.cs             — Finds bundles on disk, loads AssetInformation.json
    KeyParser.cs                  — Parses encryption keys
    LightingNullifier.cs          — Kills scene lighting
    MessageObfuscation.cs         — Base64 encoding for chat payloads
    ReflectionKiller.cs           — Disables all reflection probes, applies black cubemap
    ReflectionProbeKiller.cs      — Additional reflection probe removal
    RemoveArena.cs                — Hides/removes default arena geometry and glass
    RinkSceneLoader.cs            — Core scene loading pipeline (orchestrates everything)
    SceneDumper.cs                — Debug utility to dump scene hierarchy
    SceneLoadCoordinator.cs       — Coordinates server vs local scene selection with timeout window
    SceneryAudioState.cs          — Static state holding runtime audio source refs and volumes
    SkyboxLoader.cs               — Loads skybox materials from bundles
  Singletons/
    ConfigData.cs                 — Loads/saves SceneryLoaderConfig.local.json
```

### Unity Editor Project (PuckAssetBundles)

```
Assets/Editor/
  SceneryBundleBuilder.cs         — Main build window (Tools > Bundles > Build Selected Bundle)
  SceneryPackBuilderWindow.cs     — Legacy "build all" window, also defines Unity-side AssetInformation class
  BuildBundles.cs                 — Batch bundle building
  EditorAbx.cs                    — Editor-side .abx encryption
  AbxManifestIO.cs                — Manages encryption key manifest
  FixURPMaterialTextures.cs       — Fixes missing base textures after Standard-to-URP conversion
```

---

## How It Works

### Scene Loading Pipeline

1. **Config loads** on game start (`ConfigData.Load()`) from `SceneryLoaderConfig.local.json`
2. When `level_default` scene loads, `RinkOnlyPruner.OnSceneLoaded` fires
3. `SceneLoadCoordinator.OnSceneLoaded` decides whether to use local config or wait for a server directive
   - In practice mode: loads immediately from local config
   - In multiplayer: waits up to 3 seconds for a `!LoadMap` message from the server
4. `RinkSceneLoader.LoadSceneAsync` runs the full pipeline:
   - `RinkOnlyPruner.RemoveHangar()` strips the default arena (geometry, lights, reflections, skybox)
   - `BundleResolver.Resolve()` finds the bundle file and loads `AssetInformation.json`
   - Glass is shown/hidden based on `AssetInformation.useGlass`
   - Ambient light is multiplied if `useCustomAmbient` is true
   - `BundleLoader.InstantiatePrefabAsync` loads and instantiates the prefab
   - Shaders are rebound to the game's runtime URP shaders
   - Music and ambient audio are set up from mp3 files in the bundle folder
   - Goal crowd noise volume is applied
   - Skybox is loaded from the bundle
   - Spectator positions are auto-detected and tagged

### Multiplayer Sync

- Server sends `!LoadMap <base64-encoded SceneInformation>` to each client via `UIChatControllerPatch.SyncComplete_Postfix`
- Client's `UIChat_AddChatMessage_LoadMapPatch` intercepts the message, decodes it, and triggers `SceneLoadCoordinator.OnServerSceneDirective`
- A follow-up message tells the client about `/sl help`

### Chat Commands

`SceneryCommandPatch` patches `VoteManagerController.Event_Server_OnChatCommand`:
- `/sl` or `/sl help` — lists commands with current volume levels
- `/slMusicVolume <0-100>` — adjusts music volume via `SceneryAudioState`
- `/slAmbientVolume <0-100>` — adjusts ambient audio volume
- `/slGoalCrowdNoise <0-100>` — adjusts goal crowd noise (read by `UpdateAudioSources` each tick)

Responses are sent back to the sender via `ChatManager.Server_SendChatMessageToClients`.

---

## Configuration Files

### SceneryLoaderConfig.local.json
Located at `<PuckInstall>\Config\SceneryLoader\`. Specifies which bundle to load:
```json
{
  "sceneInformation": {
    "bundleName": "danceclub",
    "prefabName": "danceclub",
    "skyboxName": "FS013_Rainy",
    "useSceneLocally": true,
    "contentKey64": ""
  }
}
```

### AssetInformation.json
Lives in the `AssetBundles` folder next to the bundle file. Per-bundle visual and audio settings:
```json
{
  "useGlass": false,
  "useCustomAmbient": true,
  "ambientMultiplier": 0.35,
  "musicEnabled": true,
  "musicPath": "2012clubremix.mp3",
  "musicVolume": 0.056,
  "ambientAudioEnabled": false,
  "ambientAudioPath": "",
  "ambientAudioVolume": 0.3,
  "goalCrowdNoiseVolume": 0.37
}
```

All fields have defaults so old JSON files with only `useGlass` still work. The class must have `[Serializable]` for the `JsonUtility.FromJson` fallback path to work (see Known Issues).

---

## Runtime File Layout

```
<PuckInstall>/
  Plugins/SceneryLoader/
    SceneryChanger.dll
    SceneryLoaderConfig.json          — seed config (copied to Config dir on first run)
    AssetBundles/
      danceclub                       — raw asset bundle
      danceclub.abx                   — encrypted bundle (optional)
      AssetInformation.json           — per-bundle settings
      2012clubremix.mp3               — music file referenced by AssetInformation
  Config/SceneryLoader/
    SceneryLoaderConfig.local.json    — active config
    SceneryLoaderConfig.path.json     — pointer to config directory
```

---

## Key Technical Constraints

### URP Rendering
- Puck uses Universal Render Pipeline. All materials in bundles must use URP shaders.
- Shaders are rebound at load time (`RebindShadersToGameRuntime`) because bundle-compiled shaders don't match runtime shader objects.
- Player models are on layer 8 ("Player") and are only affected by **ambient light** and the **main directional light**. Point/spot lights do not illuminate players.
- Additional lights require **Per Pixel** rendering mode in the URP pipeline asset (`m_AdditionalLightsRenderingMode: 2`). The game's runtime pipeline may still use Per Vertex, which limits spot/point light quality.

### Asset Bundle Limitations
- **No custom MonoBehaviours.** Scripts included in bundles fail to deserialize at runtime. All effects must use Unity animation, particle systems, or built-in components.
- **No Post Processing Stack v2.** `UnityEngine.Rendering.PostProcessing.PostProcessVolume` is not available in Puck's URP runtime. Delete any PostProcessing GameObjects from prefabs before building.
- Baked lights (`m_Lightmapping: 4`) don't respond to runtime changes. Use Realtime (`m_Lightmapping: 1`) for animated lights.

### Deserialization
- `BundleResolver.TryLoadAssetInformation` tries Newtonsoft.Json first, falls back to `JsonUtility.FromJson`.
- The Newtonsoft fallback can fail silently if the DLL has issues at runtime. The `JsonUtility` path requires `[Serializable]` on `AssetInformation` or it returns an object with all default values.
- Diagnostic logging was added to show which deserializer succeeded and what values were read.

### Reflection-Heavy Code
- The mod avoids direct references to game types like `SpectatorManager`, `SettingsManager`, `SpectatorPosition`, `ChatManager`, and `SceneManager` (the game's, not Unity's) to prevent `TypeLoadException` from `MonoBehaviourSingleton<T>.Instance` generic inflation.
- All access is via `AccessTools.TypeByName`, `FindFirstObjectByType`, and reflection. This is fragile across game updates.

### Audio System
- `UpdateAudioSources` runs every second, finding `StadiumNoise` and `GoalCrowdNoise` AudioSources by name and scaling their volume by `GlobalVolume * AmbientVolume * multiplier`.
- Music and ambient audio are loaded from mp3 files via `UnityWebRequestMultimedia.GetAudioClip` at runtime.
- Audio file paths in `AssetInformation.json` are relative to the bundle folder. Absolute paths also work.
- `SceneryAudioState` holds live references to the Music and AmbientAudio AudioSources so chat commands can adjust them without re-finding them.

---

## Building a Bundle

1. Open the Unity project at `D:\UnityProjects\PuckAssetBundles`
2. Assign assets to an asset bundle name in the Inspector
3. Open **Tools > Bundles > Build Selected Bundle**
4. Select the bundle, root prefab, and configure settings (glass, ambient, music, audio, goal noise)
5. Browse for mp3 files if using music or ambient audio
6. Click **Build Selected Bundle**
7. Output goes to `Build/AssetBundles/`: the bundle file, `AssetInformation.json`, and any audio files
8. Copy the contents to `<PuckInstall>\Plugins\SceneryLoader\AssetBundles\`
9. Update `SceneryLoaderConfig.local.json` with the bundle/prefab/skybox names

### Fixing URP Textures
After converting materials from Standard to URP Lit, base textures may be lost. Use **Tools > Fix URP Material Textures** to re-link them by naming convention.

---

## Known Issues / Watch Out For

1. **Newtonsoft.Json can fail silently** — if it throws, the catch falls through to JsonUtility. Without `[Serializable]`, all fields get defaults. The diagnostic logging added to `BundleResolver` will show which path was taken.

2. **Prefab AudioSources with playOnAwake** — if a prefab has an AudioSource with `playOnAwake=true` and a clip assigned, it will start playing immediately on instantiation, before `SetupMusic` can configure it. The code now always calls `Stop()` / `clip = null` / `playOnAwake = false` before doing anything else.

3. **Game updates can break reflection** — any rename of `SpectatorManager`, `ChatManager`, `SettingsManager`, `VoteManagerController`, or their methods will break the mod. The decompiled game code lives at `C:\Users\Tom\source\repos\Demarini\demsPuckMods\demsPuckMods\PuckNew` for reference.

4. **Chat commands run server-side** — `/sl` volume commands execute in `Event_Server_OnChatCommand`, which runs on the server. In practice mode (server = client) this works fine. In multiplayer, only the server host's audio is affected. Client-side volume control would require sending a directive message back to the client, similar to the `!LoadMap` pattern.

5. **Spectator detection is name-based** — the mod searches for transforms starting with "Spectator" (excluding "Column", "Row", "SpectatorList", "SpectatorLocations") and adds `SpectatorPosition` components. If a bundle uses different naming, spectators won't spawn.

6. **Skybox name must match exactly** — the skybox material name in `SceneInformation.skyboxName` must match a material in the bundle. The log will show `Couldn't find Material 'X' in bundle` if it doesn't match.

7. **The two AssetInformation classes must stay in sync** — one is in the Unity Editor project (`SceneryPackBuilderWindow.cs`), the other in the mod (`Model/AssetInformation.cs`). Adding a field to one without the other will cause settings to be silently ignored.

---

## Companion Mod: demsInputControl

Located at `C:\Users\Tom\source\repos\Demarini\demsPuckMods\demsPuckMods\demsInputControl`. Provides the `/delay` chat command for input latency adjustment. The `SceneryCommandPatch` follows the same pattern (Harmony prefix on `VoteManagerController.Event_Server_OnChatCommand`, reading `message["command"]`, `message["args"]`, and `message["clientId"]`).
