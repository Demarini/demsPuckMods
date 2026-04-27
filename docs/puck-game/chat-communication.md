# Chat & Communication

## ChatManager

`NetworkBehaviourSingleton<ChatManager>` — handles sending, receiving, and broadcasting chat messages over the network.

**Access:** `ChatManager.Instance`

### Key Behavior

- Messages are sent as `ChatMessage` structs via RPCs
- Clients have a `ChatTickets` budget (property on `Player`) — running out restricts further chat
- `Player.IsChatAvailable` is a derived property based on tickets and current phase

### Harmony Patch Targets

```csharp
// Intercept outgoing messages (client-side)
[HarmonyPatch(typeof(ChatManager), "Server_SendMessage")]  // verify RPC name
class Patch_ChatManager_Send
{
    static bool Prefix(ChatManager __instance, ref ChatMessage message)
    {
        // inspect/modify/block message before it goes to the server
        return true;
    }
}

// Intercept incoming messages (all clients)
[HarmonyPatch(typeof(ChatManager), "Client_ReceiveMessage")]  // verify RPC name
class Patch_ChatManager_Receive
{
    static void Postfix(ChatManager __instance, ChatMessage message)
    {
        // log, filter, or respond to received messages
    }
}
```

## ChatMessage

`INetworkSerializable`, `IEquatable<ChatMessage>` struct — the serialized form of a chat message sent over the network.

```csharp
struct ChatMessage : INetworkSerializable, IEquatable<ChatMessage>
{
    // likely contains: sender ID, text content, timestamp/tick
}
```

Exact fields must be verified in `PuckNew/ChatMessage.cs`.

## QuickChat

Struct representing a pre-defined quick-chat message (bound to a key shortcut rather than typed).

| Member | Type | Description |
|---|---|---|
| Category | `QuickChatCategory` | Which category this message belongs to |
| *(message content)* | — | Verify exact fields in decompiled source |

## QuickChatCategory Enum

| Value | Description |
|---|---|
| *(values to verify)* | Categories like Callouts, Reactions, Team, etc. |

## Voice Communication

### PlayerVoiceRecorder / PlayerVoiceRecorderController

`NetworkBehaviour` — captures microphone input on the local client and streams it to other players. One per player.

Patching `PlayerVoiceRecorder` is the way to intercept, mute, or modify voice data before it's transmitted.

## Player Chat Throttling

| Property | Location | Description |
|---|---|---|
| `ChatTickets` | `Player` | Remaining chat messages this player can send |
| `IsChatAvailable` | `Player` | True if `ChatTickets > 0` and phase allows chat |
| `PlayerCooldown` | — | Tracks time until the next ticket is restored |
| `PlayerMute` | — | Represents a muted player (no voice or chat) |

## Existing Mod — FixChat

The `FixChat` project in this solution already patches chat behavior. Review it before writing new chat mods to avoid conflict and to see working examples of `ChatManager` patching.
