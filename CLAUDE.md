# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a C# mod collection for the Unity game **Puck** (a multiplayer hockey game). Mods are distributed via Steam Workshop and loaded at runtime by the game's built-in mod manager. The `Puck` project contains the full decompiled game source and serves as the reference for patching targets.

## Build & Deploy

Build the solution with MSBuild or Visual Studio — there are no custom build scripts:

```bash
dotnet build demsPuckMods.sln
# or open demsPuckMods.sln in Visual Studio and build normally
```

Target framework: **.NET Framework 4.8**. All projects produce class library `.dll` outputs.

**Deployment:** Every mod's `.csproj` has a `PostBuildMoveDll` target that copies the compiled DLL to `$(PuckInstallDir)\Plugins\<ModName>\`. `$(PuckInstallDir)` is defined once in `demsPuckMods/Directory.Build.props` (currently `E:\SteamLibrary\steamapps\common\Puck`) — change it there to relocate every deploy at once. When publishing to Steam Workshop, `publish.ps1` overrides this property to a staging folder so live game install isn't touched.

**Steam Workshop publishing:** Use `publish.ps1` at the repo root. It reads `workshop.json` (the mod registry) and invokes SteamCMD. Common commands: `.\publish.ps1 list`, `.\publish.ps1 all "<changenote>"`, `.\publish.ps1 <ModName> "<changenote>"`. See `UPDATE_PLAYBOOK.md` for full workflow.

There are no tests or linting tools configured.

## Architecture

### Mod Lifecycle — `IPuckMod`

Every mod implements the `IPuckMod` interface with two methods:

```csharp
public bool OnEnable();   // called when the mod loads
public bool OnDisable();  // called when the mod unloads
```

The game's `ModManagerV2` uses reflection to find the first class in a loaded assembly that implements `IPuckMod`, then invokes these methods through the interface. Mods self-register by placing their DLL in the appropriate Plugins subdirectory.

### Runtime Patching — HarmonyLib

Mods modify game behavior by monkey-patching Unity/game methods at runtime using HarmonyLib. The standard pattern across all mods:

```csharp
// OnEnable
private Harmony _harmony = new Harmony("com.author.ModName");
_harmony.PatchAll();

// OnDisable
_harmony.UnpatchSelf();
```

Patch classes use `[HarmonyPatch(typeof(TargetClass), "MethodName")]` attributes with `Prefix`, `Postfix`, or `Transpiler` inner methods. The decompiled `Puck` project is the source of truth for class/method names to target.

### Configuration

Mods that need persistence follow a consistent JSON config pattern:

- Config lives at `<GameInstallDir>/config/<ModName>Config.json`
- A `ModConfig` static class handles directory creation, file read/write, and deserialization via `Newtonsoft.Json`
- A `ConfigData` singleton holds typed properties with baked-in defaults — if the file is missing, defaults are used and the file is written on first save
- Some mods support a `.local.json` variant for settings that should survive workshop updates (e.g. `BannedWordsConfig.local.json`)

### Shared Library — `PuckUtilities`

`PuckUtilities` is a shared helper library referenced by other mods. Put cross-mod utilities here rather than duplicating them.

### Key Dependencies

| Library | Purpose |
|---|---|
| `0Harmony.dll` | Runtime method patching (core of all mods) |
| `Newtonsoft.Json` | Config serialization |
| `Unity.Netcode.Runtime` | Game's multiplayer networking layer |
| `Steamworks.NET` | Steam platform/workshop integration |
| `SocketIOClient` | WebSocket communication with backend |
| `MonoMod.RuntimeDetour` | Low-level detour support (used by Harmony) |

All dependency DLLs live in the `libs/` folder and are referenced via wildcard in each `.csproj` (excluding `System.*` assemblies).

### Networking Patterns

The game uses **Netcode for GameObjects** (Unity Multiplayer). When patching networked behavior, be aware of:
- RPC naming convention: `Client_*Rpc()` for client-targeted, `Server_*Rpc()` for server-targeted
- `NetworkVariable<T>` for synced state — requires `NetworkVariableSerializationHelper` for custom types
- `PlayerInput` is the central hub for all player input state and is a frequent patch target
