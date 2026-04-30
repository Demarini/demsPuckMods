# Server & Admin Systems

## ServerManager

`NetworkBehaviourSingleton<ServerManager>` — server-side authority for session configuration, client roster, and integration with sub-managers.

**Access:** `ServerManager.Instance`

### Key Properties

| Property | Type | Description |
|---|---|---|
| `ClientRequiredModIds` | collection | Mod IDs clients must have to join this server |
| `EnabledModIds` | collection | Mods currently enabled on this server |

### Sub-managers Coordinated by ServerManager

`ServerManager` owns and delegates to:

| Manager | Description |
|---|---|
| `EdgegapManager` | Cloud server hosting (Edgegap platform) |
| `ConnectionApprovalManager` | Approves/rejects incoming client connections |
| `TimeoutManager` | Disconnects idle/timed-out clients |
| `BanManager` | Enforces bans |
| `AdminManager` | Handles admin commands |

## AdminManager

`MonoBehaviourSingleton<AdminManager>` — processes admin commands (kick, ban, change settings) issued by privileged players.

**Access:** `AdminManager.Instance`

### Key Data

| Class | Description |
|---|---|
| `ServerKickPlayer` | Data struct for a kick action (target client ID, reason) |

### Harmony Target

Patch `AdminManager` methods to intercept or extend admin commands (e.g. add custom admin-only mod commands).

## BanManager

`MonoBehaviourSingleton<BanManager>` — maintains the ban list and enforces bans on connect.

**Access:** `BanManager.Instance`

### Key Data

| Class | Description |
|---|---|
| `PlayerBan` | A ban record (Steam ID, reason, expiry) |

### Existing Mod — BanIdiots

The `BanIdiots` project in this solution patches `BanManager`. Review it before writing ban-related mods.

## WhitelistManager

`MonoBehaviourSingleton<WhitelistManager>` — restricts server access to a list of approved Steam IDs.

**Access:** `WhitelistManager.Instance`

## VoteManager

`MonoBehaviourSingleton<VoteManager>` — manages player-initiated votes (kick vote, map change, etc.).

**Access:** `VoteManager.Instance`

### Key Data

| Class | Description |
|---|---|
| `Vote` | An in-progress vote (type, initiator, votes for/against, expiry) |

### VoteType Enum

| Value | Description |
|---|---|
| *(values to verify)* | Kick, ChangeMap, etc. — confirm in decompiled source |

### Harmony Patch Example

```csharp
// Intercept vote creation
[HarmonyPatch(typeof(VoteManager), "StartVote")]  // verify method name
class Patch_VoteManager_Start
{
    static bool Prefix(VoteManager __instance, Vote vote)
    {
        // return false to block the vote
        return true;
    }
}
```

## ConnectionApprovalManager

`MonoBehaviourSingleton<ConnectionApprovalManager>` — runs during connection handshake and decides whether to approve or reject a client.

**Access:** `ConnectionApprovalManager.Instance`

### Key Data

| Class | Description |
|---|---|
| `ConnectionApproval` | Approval response sent back to client |
| `ConnectionRejection` | Rejection response with a `ConnectionRejectionCode` |

### ConnectionRejectionCode Enum

Reason codes sent to clients when their connection is rejected:

| Value | Description |
|---|---|
| *(values to verify)* | Password wrong, server full, banned, missing required mods, etc. |

### Harmony Patch Example — Custom Connection Filtering

```csharp
[HarmonyPatch(typeof(ConnectionApprovalManager), "ApproveConnection")]  // verify
class Patch_ConnectionApproval
{
    static void Postfix(ConnectionApprovalManager __instance, ref ConnectionApproval approval)
    {
        // add custom approval logic
    }
}
```

## TimeoutManager

`MonoBehaviourSingleton<TimeoutManager>` — disconnects clients who stop responding within a configurable timeout period.

**Access:** `TimeoutManager.Instance`

## Server Configuration

| Class | Description |
|---|---|
| `ServerConfig` | Server settings (name, password, max players, mode, required mods) |
| `ServerData` | Runtime server info |
| `ServerPreviewData` | Data sent to the server browser |
| `Server` | `INetworkSerializable` struct with essential server details |
| `ServerState` | Struct for current server status |

## EdgegapManager

`MonoBehaviour` — integration with the Edgegap platform for cloud-hosted server deployment.

| Enum | Description |
|---|---|
| `EdgegapDependency` | Dependency tags for Edgegap server configuration |
