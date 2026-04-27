# Networking

The game uses **Netcode for GameObjects** (Unity Multiplayer / NGO). All networked behavior follows NGO conventions.

## ConnectionManager

`MonoBehaviourSingleton<ConnectionManager>` — client-side connection lifecycle. Starts/stops client connections and serializes player customization into the connection payload.

**Access:** `ConnectionManager.Instance`

### Key Methods

| Method | Description |
|---|---|
| `Client_StartClient()` | Initiates a connection to a server |
| `Client_Disconnect()` | Gracefully disconnects the client |

### Connection Payload

`ConnectionData` is serialized and sent to the server during handshake. It contains:

| Field | Description |
|---|---|
| SteamID | Steam identity |
| Key | Auth key |
| Customization settings | Handedness, flags, headgear IDs, jersey IDs, stick skin IDs, tape IDs |

## Connection Data Structures

| Class | Description |
|---|---|
| `Connection` | Simple `EndPoint` + password pair used to initiate a connection |
| `ConnectionData` | Full serialized client payload sent to server on connect |
| `ConnectionState` | Struct tracking current connection state |
| `EndPoint` | IP:Port pair (`IEquatable<EndPoint>`) |

## Enums

### AuthenticationPhase

Stages of the client authentication handshake:

| Value | Description |
|---|---|
| *(values to verify)* | e.g. Connecting, Authenticating, Approved, Rejected |

### ConnectionRejectionCode

Reason a connection was rejected — see [server-admin.md](server-admin.md).

### DisconnectionCode

Reason a client was disconnected:

| Value | Description |
|---|---|
| *(values to verify)* | e.g. Timeout, Kicked, Banned, ServerClosed |

## NGO Patterns Used in This Game

### RPC Naming Convention

```csharp
// Runs on all clients (ClientRpc)
[ClientRpc]
private void Client_SomethingRpc() { }

// Runs on server (ServerRpc)
[ServerRpc]
private void Server_SomethingRpc() { }
```

Prefix `Client_` = routed to clients; `Server_` = routed to server. This is consistent across all networked classes.

### NetworkVariable\<T\>

Used for synchronized state that all clients read:

```csharp
NetworkVariable<GameState>  // in GameManager
NetworkVariable<T>          // pattern used widely
```

### CompressedNetworkVariable\<TRaw, TNetwork\>

Custom generic that compresses a value before syncing to reduce bandwidth:

```csharp
CompressedNetworkVariable<TRaw, TNetwork>
```

Used where precision can be traded for bandwidth (e.g. angles, velocities).

### INetworkSerializable

Interface implemented by all structs that cross the network:

```csharp
struct GameState : INetworkSerializable, IEquatable<GameState>
struct ChatMessage : INetworkSerializable, IEquatable<ChatMessage>
struct PlayerGameState : INetworkSerializable
struct PlayerCustomizationState : INetworkSerializable
struct SynchronizedObjectData : INetworkSerializable
struct Server : INetworkSerializable
```

### InMessage / OutMessage

Internal wrappers around raw network message buffers. Used when manually reading/writing custom NGO messages rather than using the `INetworkSerializable` pattern.

## Singleton Patterns

Most systems follow one of two singleton base classes:

```csharp
// Unity MonoBehaviour singleton (client + server)
class AudioManager : MonoBehaviourSingleton<AudioManager>

// NGO NetworkBehaviour singleton (server-authoritative)
class ServerManager : NetworkBehaviourSingleton<ServerManager>
```

Both expose `Instance` for static access.

## Modding Notes

### Patching RPCs

RPCs are standard methods decorated with `[ClientRpc]` / `[ServerRpc]`. Harmony patches them by method name as normal:

```csharp
[HarmonyPatch(typeof(ChatManager), "Client_ReceiveChatMessageRpc")]  // use actual name
class Patch_ChatRpc
{
    static void Postfix(ChatMessage message) { }
}
```

### NetworkVariable subscriptions

You can subscribe to value changes without patching:

```csharp
GameManager.Instance.// NetworkVariable — need actual field name from decompiled source
// .OnValueChanged += (old, next) => { };
```

### Server-only logic

Only run server-side patches when `NetworkManager.Singleton.IsServer` is true to avoid running server logic on clients.

```csharp
if (NetworkManager.Singleton.IsServer)
{
    // safe to call Server_ methods here
}
```
