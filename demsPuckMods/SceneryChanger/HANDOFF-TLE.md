# SceneryChanger — TypeLoadException Log Flood Handoff

## Status
Root cause not yet identified. Multiple capture mechanisms attempted; the
exception bypasses all of them. Burst-pattern correlations point at our
Harmony patches but no stack trace has been obtained.

---

## The Problem

After the Puck game update, `Puck.log` is flooded with thousands of
unframed `TypeLoadException: Invalid generic instantiation` lines (no
timestamp, no stack trace). User confirmed the flood disappears when
`SceneryChanger.dll` is removed from the Plugins folder, so the source
is somewhere in this mod.

Counts: ~3,000–4,500 entries per ~5-minute session (varies by gameplay
events). Logs are visually unreadable.

---

## What We Know

### Burst pattern (correlations)
TLE counts logged immediately *before* known game events (`awk` over the log):

| Event | TLE burst size |
|---|---|
| WS Emit `playerAuthenticateRequest` | 141 |
| WS receive `playerPartyData` | 86 |
| `[ServerManager] Starting Puck listener` (~7s window) | 1054 |
| `[CameraManager] Setting active camera to type Cinematic` | 170 |
| `[Patch] GAME STATE CHANGED` (per fire) | 68, 214, 275 |
| Idle (between `Trying to Find Lights...` 1Hz logs) | ~13 |

Idle baseline ≈ 13/sec. Bursts during events scale with event frequency
(e.g. 144/sec ≈ per-frame during scene transitions). The **WS Emit** and
**GAME STATE CHANGED** correlations match the methods our Harmony patches
target (`PracticeModeDetector.Emit_Postfix` patches `WebSocketManager.Emit`;
`Patch_LevelController.Postfix` patches `LevelController.Event_Everyone_OnGameStateChanged`).

### First TLE timing
Always immediately after `[Mod] Enabled mod 56` (SceneryLoader OnEnable
returns) and the first `[CameraManager] Enabling camera of type LockerRoom`
log. So something installed by `OnEnable` is responsible.

### Game runtime
- Mono (not IL2CPP). Confirmed by `file Puck.dll` → "Intel i386 Mono/.Net assembly"
- Unity `6000.2.15f1`
- Bundle was rebuilt to Unity 6 already; that fixed rendering but not this

---

## What's Ruled Out

| Hypothesis | How ruled out |
|---|---|
| Bundle/asset shader incompatibility | Fixed earlier via `RebindShadersToGameRuntime`; arena renders fine |
| Deprecated `Object.FindObjectsOfType<T>(bool)` | Replaced 3 hot-path call sites with `FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None)` in `ArenaLightUtil.cs`, `GetAudio.cs`, `LevelManagerPatch.cs`. Flood persisted |
| Per-frame `MonoBehaviour.Update` body | All our Update methods early-return immediately (`DetectGameState`, `UpdateAudioSources`, `RinkOnlyPruner` is empty, `CoroutineRunner` has no Update). None do generic work per frame |
| `SpectatorManager.Instance` access (this *did* silently kill the spectator coroutine — fixed via `AccessTools.TypeByName + FindFirstObjectByType` reflection path) | Removing it didn't change TLE rate |
| Patch target methods missing in new game | `WebSocketManager.Emit`, `LevelController`, `LevelController.Event_Everyone_OnGameStateChanged`, `ChatMessage` all confirmed present in `Puck.dll` (verified via `tr -c '[:print:]\n' '\n' < Puck.dll \| grep ...`). Harmony reports no patch failures from our mod |

---

## What We Tried for Diagnostics (none captured the TLE)

1. **`Application.logMessageReceived += OnLogMessage`** — fired on every
   Unity log. **Zero captures** of the TLE → confirms the message is written
   to native stderr, bypassing Unity's `ILogger` pipeline.

2. **`Application.logMessageReceivedThreaded`** — not separately wired but
   same channel; would also miss it.

3. **`AppDomain.CurrentDomain.FirstChanceException += OnFirstChanceException`**
   — added in `Initialization.cs`. Captures *any* managed exception even
   if swallowed. **Also got zero captures**, which is the surprising part.
   This implies the TLE is being raised in unmanaged code (Mono runtime
   itself) when JIT fails on a method body, *before* it becomes a managed
   `TypeLoadException` instance the AppDomain hook can see.

Both handlers are confirmed present in the deployed DLL via
`tr -c '[:print:]\n' '\n' < SceneryChanger.dll | grep FirstChanceException`.

---

## Top Suspects

### 1. `PracticeModeDetector.Emit_Postfix` (most likely)
File: `Patches/PracticeModeDetector.cs:14-31`

Patches `WebSocketManager.Emit(string, Dictionary<string, object>, string)`.

Burst correlation is direct: every WS message produces a TLE burst. The
new game's `WebSocketManager` exposes `EmitAsync` (verified in
`Puck.dll` symbol table) — but `Emit` strings *also* appear, suggesting
both methods exist. If the actual `Emit` signature changed (e.g. an
`async Task` return, or a generic param like `Emit<T>(string, T, string)`),
Harmony's wrapper IL might reference a generic instantiation that fails
to JIT, producing one TLE per call.

### 2. `Patch_LevelController.Postfix`
File: `Patches/LevelManagerPatch.cs:18-76`

Patches `LevelController.Event_Everyone_OnGameStateChanged` via dynamic
`TargetMethod()` resolution. Postfix takes
`(object __instance, Dictionary<string, object> eventParams)`.

Game's actual signature (from `PuckNew/SpectatorManagerController.cs:57`):
`Event_Everyone_OnGameStateChanged(Dictionary<string, object> message)`.
Note the parameter name mismatch (`message` vs `eventParams`) — Harmony
matches by name, not position. This *could* be the issue, though typically
that produces a Harmony patch failure, not a per-call TLE.

### 3. `UIChat_AddChatMessage_LoadMapPatch.Prefix`
File: `Patches/UIChatControllerPatch.cs:62-120`

Patches `UIChat.AddChatMessage` via `TargetMethod()` resolution. Takes
`(UIChat __instance, ChatMessage chatMessage)`. `ChatMessage` exists in
`Puck.dll`; could be a struct now where it was a class before, which
would change calling convention and break Harmony's wrapper IL.

---

## Why Capture Mechanisms Failed (theory)

Mono's "Invalid generic instantiation" error is raised in the JIT compiler
in unmanaged C code. Path:
1. JIT tries to compile a method body
2. Encounters `MyMethod<T>` where `T` doesn't satisfy resolution
3. C-level `g_warning("Invalid generic instantiation")` writes directly
   to stderr
4. The method call returns an internal failure code; the runtime then
   *may or may not* surface a `TypeLoadException` to managed code

If Harmony's generated dynamic IL stub fails to JIT, the failure could
be silently swallowed at the runtime level, with the warning hitting
stderr but no managed exception ever being thrown — explaining why
`FirstChanceException` doesn't see it.

---

## Suggested Next Steps

### Option A — Patch-by-patch elimination (most direct)
Disable each Harmony patch one at a time (comment out the `[HarmonyPatch]`
attributes) and run a short session. The patch whose removal stops or
significantly reduces the TLE flood is the culprit. Order:

1. `PracticeModeDetector.Emit_Postfix` — strongest suspect
2. `Patch_LevelController.Postfix`
3. `UIChat_AddChatMessage_LoadMapPatch.Prefix`
4. `PracticeModeDetector.ConnectionManagerPracticePatches.{AfterStartClient,AfterDisconnect}`
5. `UIChatControllerPatch.SyncComplete_Postfix`

If removing patch (1) alone clears the flood, focus on its signature.
The fix is likely converting it to a `TargetMethod()`-style patch with
explicit signature inspection via reflection, or rewriting the parameter
list to match the actual game method signature exactly.

### Option B — Mono diagnostic env vars
Launch Puck with these env vars set (via `setx` or a launcher batch):
```
MONO_LOG_LEVEL=debug
MONO_LOG_MASK=type
```
This enables verbose Mono logging including type-loading details. The
output goes to stderr (same place TLE goes), but with much more context
about the type that failed to instantiate. The next line after each TLE
should now include the type name.

### Option C — `[HarmonyDebug]` attribute
Add `[HarmonyDebug]` to suspect patch classes. Harmony will write its
generated IL to disk (path shown in log) on patch installation. Inspect
the generated wrappers for generic types referencing game-side symbols.

### Option D — Rewrite suspect patches with full reflection
Convert all signature-sensitive patches to use:
```csharp
static MethodBase TargetMethod() {
    var t = AccessTools.TypeByName("WebSocketManager");
    return t == null ? null : AccessTools.FirstMethod(t, m => m.Name == "Emit");
}
static void Postfix(object[] __args) {
    // Manually pull params from __args, no signature dependency
}
```
The `__args` accessor sidesteps name/type matching entirely and accepts
any signature. If the TLE is from signature mismatch, this kills it.

---

## Files Involved

| File | Role |
|---|---|
| `Initialization.cs` | Has the `OnLogMessage` + `OnFirstChanceException` capture handlers (currently silent) |
| `Patches/PracticeModeDetector.cs` | Suspect #1 — patches WebSocketManager.Emit |
| `Patches/LevelManagerPatch.cs` | Suspect #2 — patches LevelController state changes |
| `Patches/UIChatControllerPatch.cs` | Suspect #3 — patches UIChat.AddChatMessage |
| `Patches/SpectatorManagerPatch.cs` | Dead code (no `[HarmonyPatch]` attribute, never installed). Safe to delete |

---

## What's Working (do not touch)

| Feature | Implementation | Notes |
|---|---|---|
| Bundle resolution + load | `Services/BundleResolver.cs`, `Services/BundleLoader.cs` | Now logs `[BundleLoader] Resolved` and `[BundleLoader] RAW ->` on success |
| Shader rebind for cross-Unity-version compat | `RinkSceneLoader.RebindShadersToGameRuntime` | Critical — `Shader.Find` replaces compiled-against shaders with current-runtime shaders. Arena meshes invisible without this |
| Spectator spawning (PuckNew event-driven) | `RinkSceneLoader.SpawnSpectatorsFromStagedRoot` | Walks staged prefab for `Spectator*` transforms, adds `SpectatorPosition` component via `AccessTools.TypeByName + AddComponent(Type)`. Triggers game's event chain. Does NOT touch `SpectatorManager.Instance` (avoids silent generic-singleton failure) |
| Diagnostic dump after instantiation | `RinkSceneLoader.DumpStagedRoot` | Logs renderer/shader/layer/camera/shadow-mode histograms. One-shot per scene load — not flood-class. Worth keeping |
| `level_default` scene name (was `level_1`) | `RinkOnlyPruner.OnSceneLoaded`, `SceneLoadCoordinator.OnSceneLoaded`, `RinkOnlyPruner.targetScene` | Scene was renamed in the update |
| Config path | `E:\SteamLibrary\steamapps\common\Puck\Config\SceneryLoader\SceneryLoaderConfig.local.json` | NOT the one in Plugins/ — that's only the seed |

---

## Other Notes

- `RinkOnlyPruner.scene` (the static `Scene` property) is **never assigned
  anywhere** in the codebase. `RemoveHangar()` calls `HideEverythingExceptRink(scene)`
  and `TryPruneScene(scene, ...)` with a default/invalid `Scene` struct →
  silent no-op (confirmed via `[RinkExcluder] Excluded non-rink content. Objects affected (approx): 0`
  in early logs). Doesn't break anything but worth fixing or deleting.

- Two `.csproj` files in the same folder both produce `SceneryChanger.dll`
  to `bin\Debug\` (overwriting each other): `SceneryChanger.csproj` is
  near-empty (only compiles `Class1.cs`), `SceneryLoader.csproj` is the
  real one. Always build the **solution** or specifically `SceneryLoader.csproj`.

- TypeLoadException flood does NOT prevent any feature from working —
  arena, spectators, skybox all functional. It's purely a log-spam issue
  that makes debugging anything else painful.
