# Backend Services

## BackendManager

Static class — makes HTTP calls to the game's backend API for authentication, matchmaking, leaderboards, and server management.

**Access:** `BackendManager` (static — no `Instance` property)

### Key Response Types

All responses are wrapped in `Response<TSuccess, TError>` for consistent error handling.

| Response Type | Description |
|---|---|
| `PlayerAuthenticateResponse` | Auth token + player data after login |
| `PlayerDeployServerResponse` | Result of requesting a cloud server deployment |
| `PlayerGetLocationsResponse` | Available Edgegap data center locations |
| `PlayerStartTransactionResponse` | Start a shop transaction |
| `ServerAuthenticateResponse` | Server auth token from backend |
| `ServerBrowserEndPointsResponse` | List of endpoints for server browser |
| `ServerConnectionApprovalResponse` | Backend validation for a connection attempt |
| *(additional types)* | Verify in decompiled source for full list |

### Generic Response Wrapper

```csharp
class Response<TSuccess, TError>
{
    // contains either a success value or an error value
}

class EmptyResponse { }  // for calls with no meaningful return data
```

## BackendUtils

Static utility class — shared HTTP helpers used by `BackendManager`.

## WebSocketManager

Static class — maintains a persistent WebSocket connection to the game backend for real-time events (matchmaking updates, server status, etc.).

**Access:** `WebSocketManager` (static — no `Instance` property)

## WebSocketManagerController

Static controller that initializes `WebSocketManager`.

## TCP Server/Client

Internal TCP communication used for server-browser data exchange.

| Class | Description |
|---|---|
| `TCPServer` | Lightweight TCP server (runs on game server instances) |
| `TCPClient` | TCP client (connects from game client to query server data) |
| `TCPServerMessage` | Base class for all TCP messages |
| `TCPServerPreviewRequest` | Request for server preview data |
| `TCPServerPreviewResponse` | Response with `ServerPreviewData` |

### TCPServerMessageType Enum

| Value | Description |
|---|---|
| *(values to verify)* | e.g. PreviewRequest, PreviewResponse |

## SteamManager / SteamIntegrationManager

Static — Steam platform integration. Handles Steam identity, authentication tokens, and P2P relay.

**Access:** `SteamManager`, `SteamIntegrationManager` (both static)

## SteamWorkshopManager

Static — Steam Workshop integration. Manages querying, downloading, and subscribing to Workshop items (mods, cosmetics).

**Access:** `SteamWorkshopManager` (static)

## TransactionPhase Enum

Tracks the state of a shop purchase transaction:

| Value | Description |
|---|---|
| *(values to verify)* | e.g. Idle, Pending, Complete, Failed |

## Modding Notes

- Mod code should generally **not** call `BackendManager` directly — it's the game's own API client
- `SteamWorkshopManager` is relevant if you need to query installed Workshop items beyond what `ModManager` provides
- `SteamManager` provides the local player's Steam ID, which can be useful for per-player data in mods

### Getting local Steam ID

```csharp
// Use SteamManager to get identity without calling BackendManager
// Verify the exact property/method name in decompiled source
var steamId = SteamManager.// SteamId property
```
