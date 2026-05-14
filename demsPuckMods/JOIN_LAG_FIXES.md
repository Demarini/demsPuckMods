# Join-Lag Fixes — SceneryLoader & MOTD

Reference doc for the join-time performance work done in commit `24a36b4`
(*"Fixed puck AI and scenery/MOTD lag"*).

## Symptom

When clients connected to a server running both **SceneryLoader** and **MOTD**,
the server tick visibly hitched. With several joiners arriving close together
(start-of-match, post-restart, mass reconnects after a server hiccup), the
hitches stacked into multi-second freezes. Existing players experienced
rubber-banding and missed input frames; the joiner saw a delayed scene load
and a delayed MOTD.

The work that ran on every join was:

- A synchronous disk read of `MOTD.json`, JSON parse, and chunked encoding
  for the MOTD broadcast.
- A `ConfigData.Load()` call inside SceneryLoader that re-read
  `SceneryLoaderConfig.local.json`, re-deserialized it, and re-built a
  `SceneInformation` object.
- JSON-serializing that `SceneInformation` and obfuscating it into the
  `!LoadMap` payload string.
- HarmonyLib `AccessTools` reflection lookups of `ChatManager`, its
  singleton `Instance`, and `Server_SendChatMessageToClients` — done from
  scratch on every join in both mods.

Disk I/O on the main thread is the most expensive part; reflection lookups
are second; JSON serialization is third. All three multiplied per joiner.

## Files affected

- `SceneryChanger/Patches/UIChatControllerPatch.cs` — SceneryLoader's
  `SyncComplete_Postfix` and `SendChatToClient`.
- `TestProject/Patches/UIChatControllerPatch.cs` — MOTD's
  `SyncComplete_Postfix` and former `SendMotdToClient` (now refactored
  into `GetCachedMessages` + `EnsureChatReflection`).

Both files patch
`global::SceneManager.Server_OnClientSceneSynchronizeComplete` — the
server-side hook that fires once per client when the client finishes
syncing the active scene. That's "the join handler" in both mods.

## Fix #1 — SceneryLoader `!LoadMap` payload caching

**Before:** every join called `ConfigData.Load()` (disk + parse), then
`JsonConvert.SerializeObject(sceneInformation)`, then
`MessageObfuscation.Encode(...)`. The result is the same payload for every
joiner until the server admin changes the active scene.

**After:** `ConfigData.Load()` is removed from the join path.
`ConfigData.Instance` already lazy-loads on first access; the active scene
only changes on scene reload, and that path calls `Load()` itself (in
`RinkOnlyPruner.OnSceneLoaded`). So the join handler can trust the
already-loaded instance.

The encoded payload is cached behind a reference-equality check on the
`SceneInformation` object:

```csharp
static SceneInformation _cachedSi;
static string _cachedPayload;
...
if (!ReferenceEquals(si, _cachedSi))
{
    _cachedSi = si;
    _cachedPayload = "!LoadMap " + MessageObfuscation.Encode(JsonConvert.SerializeObject(si));
}
SendChatToClient(_cachedPayload, clientId);
```

`ConfigData.Load()` rebuilds the `ConfigData` singleton (and therefore the
`sceneInformation` reference) when a new scene is loaded, so reference
inequality is a reliable invalidation signal — no manual cache-bust needed.

## Fix #2 — MOTD payload caching keyed on file mtime

**Before:** every join called `ConfigData.Load()`, then
`File.ReadAllText(MOTDFilePath)`, then `ModalDocIO.TryLoad` (JSON parse +
validation), then chunked the resulting string for the chat protocol.
Every joiner triggered the same disk read + parse for the same file
contents.

**After:** the chunked outgoing messages are cached. Cache invalidation
keys on the file's `LastWriteTimeUtc` so admins can still hot-edit
`MOTD.json` without restarting the server:

```csharp
static string _cachedJsonPath;
static DateTime _cachedJsonMtime;
static string[] _cachedMessages;

static string[] GetCachedMessages(string path)
{
    if (!File.Exists(path)) return null;
    var mtime = File.GetLastWriteTimeUtc(path);

    if (_cachedMessages != null && _cachedJsonPath == path && _cachedJsonMtime == mtime)
        return _cachedMessages;

    // ... read, parse, chunk, cache ...
}
```

A cache miss costs the same as the original (one disk read + one parse +
one chunking pass), but only when the file actually changed — typically
once at server start, then once per admin edit. All other joins hit the
cache.

Failures (`File.Exists == false`, `ModalDocIO.TryLoad` returns null) are
also memoized — `_cachedMessages = null` plus the path/mtime — so a
broken/missing MOTD doesn't trigger repeat parses-of-failure on every
joiner either.

## Fix #3 — Reflection lookup caching (both mods)

`AccessTools.TypeByName("ChatManager")`,
`AccessTools.PropertyGetter(...)`, and `AccessTools.Method(...)` are not
free — each walks loaded assemblies / type metadata. They were called
from `SendChatToClient` (or its MOTD equivalent) per join.

**After:** type, property getter, send method, and the `ChatManager`
singleton itself are cached in static fields populated lazily on first
use:

```csharp
static Type _chatManagerType;
static MethodInfo _chatManagerInstanceGetter;
static MethodInfo _sendChatMethod;
static object _chatManagerInstance;

if (_chatManagerType == null) { /* one-time AccessTools lookups */ }
if (_chatManagerInstance == null)
    _chatManagerInstance = _chatManagerInstanceGetter?.Invoke(null, null);
_sendChatMethod?.Invoke(_chatManagerInstance, new object[] { message, new ulong[] { clientId } });
```

The `_chatManagerInstance` is cached separately because the singleton
itself can be torn down between scene loads in some game states; we
re-resolve it via the cached getter if it's null on a later join. The
type/method handles never change for the lifetime of the process and so
are cached unconditionally.

## Caveats

- **Singleton lifetime.** `ChatManager.Instance` may become null between
  game-state transitions. The fix re-resolves it via the cached
  `PropertyGetter` if `_chatManagerInstance` is null at call time, so
  the cache survives transitions but doesn't return a stale reference.
- **MOTD admin edits.** `_cachedJsonMtime` is set to the file's
  `LastWriteTimeUtc` at parse time. If an admin saves the file with
  a clock skew that goes backward, the cache will think nothing
  changed and serve the old MOTD until the next forward edit. Real
  admin workflows don't hit this, but it's there.
- **Scenery hot-reload.** Scenery reloads invalidate the SceneryLoader
  cache via the reference-equality check on `_cachedSi`, but only
  because `ConfigData.Load()` allocates a new `ConfigData` instance
  (and therefore a new `sceneInformation` reference). If anyone
  refactors `ConfigData.Load()` to mutate the existing instance in
  place, this invalidation breaks silently. Worth a comment on
  `ConfigData.Load()` itself if the refactor is ever considered.
- **First-join cost still exists.** The very first join after a server
  start (or a config change) still pays the original cost of one disk
  read + parse + reflection resolution. The fix only flattens the
  per-join cost from O(N) to O(1) for N joiners on the same content.

## Verifying the fix

The visible test is the same as the original repro: have several clients
join the server in rapid succession (e.g. a full lobby reconnecting after
a restart). Pre-fix you'd see the server tick stall noticeably; post-fix
the joins should land without other players noticing.

Quantitatively, you can add `Stopwatch` instrumentation around the
`SyncComplete_Postfix` body in either patch and log the elapsed
milliseconds. Pre-fix observed values for a populated MOTD on disk were
in the tens of ms per join; post-fix the cached path is sub-millisecond
(reflection invoke + one chat send call).
