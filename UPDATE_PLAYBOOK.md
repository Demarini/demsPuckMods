# Puck Update Playbook

Research dump for the upcoming Puck game update. Load this at the start of an update-day session so a fresh Claude has full context without re-discovering it.

**Last researched:** 2026-05-11

---

## TL;DR — Update Day Procedure

1. Decompile the new game build into `demsPuckMods/PuckNew/` (overwrites the current "upcoming build" reference) — or update the existing `PuckNew/` if it already matches.
2. Run `dotnet build demsPuckMods.sln` and let it fail. The build errors are the breakage list.
3. Walk the failures by mod (use the **Mod Inventory** table below to prioritize highest-patch-count mods first).
4. Test each fixed mod by launching the game with that mod in `Plugins/`. The post-build target already copies the DLL to `E:\SteamLibrary\steamapps\common\Puck\Plugins\<ModName>\`.
5. Publish: `.\publish.ps1 all "Compatibility update for vX.Y"` from repo root.
6. Restart the three game servers — see `.claude/server-infrastructure.md` for `systemctl` commands.

---

## What Actually Changed (confirmed against pre-release 2026-05-11)

The `PuckNew/` decompile in this repo **predates the pre-release** and is now stale. Confirmed game-side changes from the pre-release announcement and the build:

| Old (current Puck.dll backup) | New (pre-release Puck.dll) | Affects |
|---|---|---|
| `IPuckMod` | `IPuckPlugin` | **Every mod** — class declarations |
| `PlayerBodyV2` | `PlayerBody` | PuckUtilities, PlayerStickRadius, ModifyMinimapIcons, PuckAIPractice |
| `UIScoreboard.UpdatePlayer` | (removed/renamed) | FixScoreboard |
| `SpectatorManager.SpawnSpectators` | (removed/renamed) | BanIdiots |
| `PlayerInput.SleepInput` / `Client_SleepInputRpc` | (removed) | DelayInputs |
| `UIServerBrowser.Hide(arg)` | `Hide()` (no args) | ServerBrowserInGame |
| `ServerBrowserServer`, `UIManagerInputs` | (removed/renamed) | ServerFavorites |

**`ModManagerV2` → `ModManager`** and `EventManager` → static were also expected from the older `PuckNew/` decompile, but no mod references those — so they're irrelevant for our builds.

### libs/ gotcha

The wildcard `..\libs\*.dll` in every csproj will pull in **any** `.dll` in libs/ besides `System.*`. If `Puck_Backup.dll` and `Puck_Modded.dll` are present (without `.bak`), MSBuild picks one arbitrarily and 13 of 14 mods fail to find `IPuckPlugin`. After overwriting libs/ on update day, verify those backups still end in `.dll.bak`. Only `demsInputControl.csproj` excludes them explicitly.

---

## Project Layout

```
demsPuckMods/
├── demsPuckMods.sln                  # Builds everything
├── Directory.Build.props             # Defines $(PuckInstallDir) — single source of truth for deploy path
├── libs/                             # 0Harmony, Newtonsoft.Json, Steamworks.NET, Unity.Netcode, etc.
├── Puck/                             # Decompiled CURRENT game (reference only — not a buildable mod)
├── PuckNew/                          # Decompiled UPCOMING game (reference only — not a buildable mod)
├── PuckUtilities/                    # Shared helper library
└── <each mod>/                       # 14 mod projects
publish.ps1                            # Steam Workshop uploader
workshop.json                          # Mod → publishedfileid registry
docs/puck-game/                        # Pre-written docs for the NEW build (uses ModManager naming)
.claude/server-infrastructure.md       # Linode VPS + 3 game-server systemd setup
CLAUDE.md                              # Project context (note: deploy path is OUT OF DATE — says A:\, actual is E:\)
```

### Deploy path

Centralized in `demsPuckMods/Directory.Build.props`:
```xml
<PuckInstallDir>E:\SteamLibrary\steamapps\common\Puck</PuckInstallDir>
```
Every mod's `.csproj` has a `PostBuildMoveDll` target that copies `<AssemblyName>.dll` (plus any content files) to `$(PuckInstallDir)\Plugins\<ModName>\`. Editing this one file relocates all deploys.

When `publish.ps1` builds, it overrides this via `/p:PuckInstallDir=E:\SteamCMD\staging` so local game install isn't polluted with workshop staging.

---

## Mod Inventory & Patch Surface

Sorted by patch count (most fragile first). Each entry lists Harmony patch targets — these are the breakage hotspots.

### PuckAIPractice — workshop ID `3543744568`, active
AI practice mode with goalie difficulty, replay fixes, chat commands.
- `GameManager.OnGameStateChanged` (Postfix)
- `WebSocketManager` (class-level, custom `TargetMethod`)
- `ConnectionManager` (class-level, custom `TargetMethod`)
- `ReplayRecorder.Server_Tick`, `Server_StartRecording` (Postfix)
- `ReplayRecorderController.Event_Everyone_OnPlayerSpawned`, `OnPlayerBodySpawned`, `OnStickSpawned` (Prefix)
- `ServerManagerController.Event_Everyone_OnClientDisconnected` (Postfix)
- `ConnectionManagerController.Event_OnPauseMenuClickDisconnect` (Postfix)
- `PlayerManager.AddPlayer`, `GetPlayers` (Postfix)
- `VoteManagerController.Event_Server_OnChatCommand` (Postfix)
- `Movement.FixedUpdate` (Postfix)
- `PlayerBody.DashRight`, `DashLeft`, `CancelDash`, `FixedUpdate` (Postfix)
- See `PuckAIPractice/HANDOFF.md` for known goalie-duplication bug during replay.

### demsInputControl — workshop ID `3539688632`, active (folder name: `InputControl`)
Custom input mapping, stop/sprint tuning, chat command hooks.
- `PlayerInput` (class-level Prefix)
- `PlayerInput.ClientTick`, `Update`, `Client_MoveInputRpc`, `Client_StopInputRpc` (Prefix)
- `PlayerBody.FixedUpdate`, `HandleInputs` (Postfix)
- `Movement.FixedUpdate` (Postfix)
- `VoteManagerController.Event_Server_OnChatCommand` (Postfix)

### ServerFavorites — workshop ID `3551287814`, **inactive**
Favorites system for server browser. Inactive — confirm with user whether to revive or skip.
- `UIServerBrowser.OnClickClose`, `Initialize`, `AddServerButton`, `SortServers` (Postfix)
- `UIManagerInputs.OnPauseActionPerformed` (Postfix)

### BanIdiots — no workshop ID, **inactive**
Chat word filter + arena scenery swap (rink-only mode). Heavy reflection. No published ID — was never released.
- `UIChat.Server_ProcessPlayerChatMessage` (Prefix)
- `SpectatorManager.SpawnSpectators` (Prefix)

### ModifyMinimapIcons — workshop ID `3537113445`, active
Customizes minimap icon size/appearance.
- `UIMinimap.SetScale` (Postfix)
- `UIMinimap.Update` (Prefix/Postfix)

### RotateMinimap — workshop ID `3536070072`, active
Rotates minimap with camera angle.
- `UIMinimap.RemovePlayerBody` (Prefix)
- `UIMinimap.Update` (Prefix)
- Heavy `Traverse` use on private fields (`minimap`, `foreground`, `background`, `content`, `Bounds`).

### SceneryChanger — workshop ID `3566470321`, active (folder name: `SceneryLoader`)
Custom arena scenery + audio volume chat commands. Has `WORKSHOP_DESCRIPTION.txt` and `HANDOFF.md`.
- `VoteManagerController.Event_Server_OnChatCommand` (Prefix)
- `WebSocketManager.Emit` (custom `TargetMethod`, Postfix)

### TestProject (publishes as `MOTD`) — workshop ID `3554730098`, active
Message-of-the-day modal on game launch.
- `SceneManager.Server_OnClientSceneSynchronizeComplete` (Postfix)
- `WebSocketManager.Emit` (custom `TargetMethod`, Postfix)

### PlayerStickRadius — no workshop ID, **inactive**
Stick collision radius tuning.
- `PlayerBodyV2.OnNetworkSpawn`, `OnNetworkDespawn`, `FixedUpdate` (Prefix)
- ⚠️ Patches `PlayerBodyV2` — in `PuckNew/` it's `PlayerBody` (no `V2`). Will need rename if/when re-activated.

### DelayInputs — no workshop ID, **inactive**
Latency simulator for testing.
- `ConnectionManager`, `WebSocketManager`, `PlayerInput` (class-level)
- Reflectively enumerates `NetworkedInput<>` types.

### FixChat — no workshop ID, **inactive**
Chat display fix.
- `UIChat.Update` (Prefix/Postfix)

### FixScoreboard — workshop ID `3538336529`, **inactive**
Scoreboard stat display fix.
- `UIScoreboard.UpdatePlayer` (Prefix/Postfix)

### ServerBrowserInGame — workshop ID `3549138879`, **inactive**
Opens server browser from in-game pause menu.
- `UIServerBrowser.OnServerBrowserClickClose` (Prefix)

### AutoJoin — **not in workshop.json yet**
Untracked new mod (in `git status`). Uses Harmony but `PatchAll()` finds no patch attributes in the inventory — likely relies on `AutoJoinManager` doing the work outside Harmony. Needs a registry entry before it can publish.

---

## Publish Workflow

Fully automated via `publish.ps1` + `workshop.json` + SteamCMD at `E:\SteamCMD`.

### Commands

```powershell
.\publish.ps1 list                                  # Show all mods and active/inactive status
.\publish.ps1 all "Compatibility update for vX.Y"   # Build + publish every active mod
.\publish.ps1 <ModName> "Hotfix"                    # Build + publish one mod
.\publish.ps1 toggle <ModName>                      # Flip active flag (persists to workshop.json)
.\publish.ps1 -NoBuild <ModName> "..."              # Skip rebuild, use existing staging
.\publish.ps1 clean                                 # Remove locally-deployed mods from game Plugins
.\publish.ps1 clean <ModName>                       # Remove one
```

### Mechanics

1. Builds with `/p:PuckInstallDir=E:\SteamCMD\staging` so post-build copies land in staging, not your live game install.
2. Generates a temporary VDF (`workshopitem` block, app_id `2994020`) per mod.
3. Invokes `steamcmd.exe +login Demarini +workshop_build_item <vdf> +quit`.
4. Skips any mod with empty `publishedfileid`.

### Currently active mods (6)
ModifyMinimapIcons, PuckAIPractice, SceneryLoader, InputControl, RotateMinimap, MOTD

### Open registry items
- **AutoJoin** — not in `workshop.json` at all. Add an entry (with empty `publishedfileid` and `active: false`) if you want it in the registry but not yet published.
- **No-ID, inactive mods** — `BanIdiots`, `FixChat`, `DelayInputs`, `PlayerStickRadius`. Either retire from registry or fill in IDs before re-activating.
- **Has-ID, inactive mods** — `FixScoreboard`, `ServerBrowserInGame`, `ServerFavorites`. Deprecated or just paused? Confirm intent before update day so you know whether to test/publish them.

---

## Per-Mod Server Deployment Notes

- **SceneryLoader (SceneryChanger)** — server only needs the DLL + `Config/SceneryLoader/SceneryLoaderConfig.local.json`. **Do not upload asset bundles to the server** — bundles ride along to clients via a separate `clientRequired` workshop asset mod, and the dedicated server doesn't render so it doesn't need them. Server-side bundle dir wastes disk and slows backups.
- **MOTD** — needs `Plugins/MOTD/<dll>` + `Plugins/MOTD/MOTDConfig.local.json` (the `.local.json` overrides the workshop default and survives workshop updates). Set `jsonFileLocation` to a Linux path like `/srv/puck/MOTD.json` (which all three servers can share). The actual MOTD content JSON has the rules/banner/Discord button structure.
- **PuckAIPractice** — `Plugins/PuckAIPractice/<dll>` + portable `PuckAIPracticeConfig.json`. No path adjustments needed.

## Known Mod Bugs to Clean Up Post-Update (cosmetic, not blockers)

- **PuckAIPractice path resolution uses Windows separators on Linux.** Logs show `Resolved gameDir: /srv/puck/serverN/Plugins/PuckAIPractice/..\..\..\../common/Puck` — literal `\` characters in the path. On Linux this creates a real directory named `..\..\..\..` inside the plugin folder, which works but is ugly and confuses anyone navigating the filesystem. Fix: search PuckAIPractice source for hardcoded `\..\..\..\..` and replace with `Path.Combine` or `/`.

## Likely Breakage Hotspots on Update

Order to investigate failures, from most-likely to least:

1. **`PlayerInput` signature changes** — touched by demsInputControl (5 patches) and DelayInputs.
2. **`PlayerBody` / `PlayerBodyV2` rename** — `Puck/` has `PlayerBodyV2`, `PuckNew/` has `PlayerBody`. PlayerStickRadius will *definitely* break. PuckAIPractice and demsInputControl both patch `PlayerBody` already (good).
3. **`ReplayRecorder*` event signatures** — PuckAIPractice has 5+ patches here. Replay system changes are common between builds.
4. **`UIMinimap` internals** — RotateMinimap uses private-field reflection and is brittle to refactors.
5. **`UIServerBrowser` UI surface** — ServerFavorites + ServerBrowserInGame both patch it.
6. **`WebSocketManager.Emit`** — used via custom `TargetMethod` by SceneryChanger, MOTD, PuckAIPractice. Signature stability matters.

---

## Server-Side Config Migration (verified against pre-release 2026-05-11)

Server3 has been successfully migrated. **Action on update day for servers 1 & 2**: follow the same procedure documented here.

### Required filenames (new build looks for these by hardcoded name in WorkingDirectory)

| File | Purpose | Notes |
|---|---|---|
| `server_config.json` | Connection + global settings | **Renamed from `config.json`**. New build ignores the `--serverConfigurationPath` CLI flag and looks up this exact name in cwd. |
| `public_game_mode_config.json` | Phase tunings, spawn delay, periods | Filename is `<gameMode>_game_mode_config.json` (so if `gameMode: "public"`, this name). |
| `admin_steam_ids.json` | JSON array of admin Steam IDs | **Moved out of `server_config.json`**. Just `["id1","id2"]`. |
| `whitelisted_steam_ids.json` | JSON array | New. Paired with `useWhitelist` flag. |
| `banned_steam_ids.json` | JSON array | Same as before; preserved during migration. |
| `banned_ip_addresses.json` | JSON array | New file, IPs separated from Steam ID bans. |

### `mods` array — new schema (verified 2026-05-12)

```json
"mods": [
  { "id": "3554730098", "isEnabled": true, "isClientRequired": true },
  { "id": "3543744568", "isEnabled": true, "isClientRequired": true },
  { "id": "3566470321", "isEnabled": true, "isClientRequired": true }
]
```

| Old field | New field | Notes |
|---|---|---|
| `id` (number, e.g. `3554730098`) | `id` (string, e.g. `"3554730098"`) | **Type changed to string** — quote it |
| `enabled` | `isEnabled` | "is" prefix |
| `clientRequired` | `isClientRequired` | "is" prefix |

When mods are listed here, the server auto-downloads them from Steam Workshop on startup into `/srv/puck/serverN/steamapps/workshop/content/2994020/<id>/`. **Don't also place a manual copy in `Plugins/<ModName>/`** — the server may end up loading the same DLL twice (workshop install + manual install) and double-applying Harmony patches.

### MOTDConfig.local.json placement gotcha

For workshop-installed MOTD, the `MOTDConfig.local.json` override goes into the **workshop content dir** (`/srv/puck/serverN/steamapps/workshop/content/2994020/3554730098/`), not into `Plugins/MOTD/` (which doesn't exist in the workshop-required setup). Steam doesn't delete files it didn't put there, so the override survives mod updates. Without this file, MOTD falls back to the workshop default config (`MOTDConfig.json`) which points at the original author's Windows path and silently does nothing.

### Field renames in `server_config.json`

| Old (single config.json) | New (`server_config.json`) |
|---|---|
| `voip` | `useVoip` |
| `serverTickRate`, `clientTickRate`, `targetFrameRate` | Unified `tickRate` |
| `pingPort` | Removed — single `port` handles everything (uPnP forwards one port) |
| `adminSteamIds: [...]` | Moved to separate `admin_steam_ids.json` |
| `phaseDurationMap` | Moved to `public_game_mode_config.json` |
| `reloadBannedSteamIds`, `usePuckBannedSteamIds`, `printMetrics`, `kickTimeout`, `sleepTimeout`, `joinMidMatchDelay`, `startPaused`, `allowVoting` | Not present in new example — verify behavior; many may be defaults now |
| (new) | `useWhitelist: bool`, `gameMode: "public"`, `level: "default"` |

### `phaseDurationMap` key renames

| Old | New |
|---|---|
| `Playing` | `Play` |
| `PeriodOver` | Split into `Intermission` and `PostGame` |
| (new) | `None`, `PreGame`, `Intermission`, `PostGame` |

### Linux dedicated server install via SteamCMD

- **Anonymous login does NOT work** for pre-release branch — get "Password check returned error Failure" even with the correct `-betapassword`.
- **`-betapassword` flag is broken/unnecessary** if the user's authenticated Steam account already has access via the Steam client UI. Just `-beta pre-release` works.
- Working command (Windows SteamCMD pulling Linux build):
  ```
  steamcmd.exe +@sSteamCmdForcePlatformType linux +force_install_dir <path> \
    +login <user> +app_update 3481440 -beta pre-release validate +quit
  ```
  (Note: `+force_install_dir` is sometimes ignored — files may land at SteamCMD's default `steamapps/common/Puck Dedicated Server/`.)
- App ID is **3481440** ("Puck Dedicated Server"), separate from the game app ID 2994020.
- Beta branch name is **`pre-release`** (with hyphen).

### Binary name change

- Old: `Puck.x86_64`
- New: `Puck` (no extension)
- New `start_server.sh` autodetects both. If migrating, **delete the old `Puck.x86_64`** or the script will keep using the stale binary.

### Migration procedure (verified working on server3)

1. SSH to server, stop the instance: `sudo systemctl stop puck@serverN`
2. Tar-backup current dir: `sudo tar czf /srv/puck/serverN-backup-YYYY-MM-DD.tar.gz -C /srv/puck serverN`
3. Stage new Linux files in `/srv/puck/serverN-new/` (scp from a Windows SteamCMD install, or run SteamCMD on the box if you can auth interactively)
4. Preserve `banned_steam_ids.json` and `motd.txt` from old dir → `/tmp/`
5. `sudo mv /srv/puck/serverN /srv/puck/serverN-pre-update && sudo mv /srv/puck/serverN-new /srv/puck/serverN`
6. Restore preserved files into new dir, recreate `Plugins/` and `Logs/` dirs
7. Write `server_config.json`, `public_game_mode_config.json`, populate `admin_steam_ids.json`
8. `sudo chown -R puckd:puckd /srv/puck/serverN && sudo chmod +x .../Puck .../start_server.sh`
9. `sudo systemctl start puck@serverN`
10. Verify in logs: look for `Deserializing server config from file (./server_config.json)`, `WebSocket connected`, `TCPServer] Server started on port N`, `serverAuthenticateRequest -> success:true`

---

## Cross-References

- **Game class/method docs** — `docs/puck-game/*.md`. Already written against PuckNew. Use `mod-system.md`, `player-system.md`, `input-system.md`, `networking.md`, `ui-system.md` when patches break.
- **Server ops** — `.claude/server-infrastructure.md`. 3 systemd `puck@serverN` instances on Linode `172.233.221.208`. SSH via `mcp__ssh-mcp__exec`.
- **Out-of-date in CLAUDE.md** — claims deploy path is `A:\SteamLibrary\...`; actual is `E:\SteamLibrary\...` (per `Directory.Build.props`). Fix on update day or sooner.

---

## How to Resume in a Future Session

Drop this prompt into a fresh Claude:

> Read `UPDATE_PLAYBOOK.md` at the repo root. Then we're ready to react to a Puck update. New game build is now on Steam — pull the latest decompile into `demsPuckMods/PuckNew/` and walk the breakage from `dotnet build demsPuckMods.sln`.
