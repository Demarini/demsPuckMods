# Puck Game — Decompiled Source Reference

Documentation generated from the `PuckNew` decompiled game project. Use these docs to identify what classes and systems to target when writing mods.

## Document Index

| File | Coverage |
|---|---|
| [mod-system.md](mod-system.md) | `IPuckMod`, `ModManager`, `Mod`, mod lifecycle |
| [game-state.md](game-state.md) | `GameManager`, `GamePhase`, `GameState`, phases & scoring |
| [player-system.md](player-system.md) | `Player`, `PlayerInput`, `PlayerBody`, roles, teams, stats |
| [puck-physics.md](puck-physics.md) | `Puck`, `SynchronizedObject`, physics managers |
| [input-system.md](input-system.md) | `InputManager`, all input actions, key bindings |
| [ui-system.md](ui-system.md) | `UIManager`, all 23+ screens, popups, toasts |
| [audio-system.md](audio-system.md) | `AudioManager`, mixer channels, `SynchronizedAudio` |
| [gameplay-mechanics.md](gameplay-mechanics.md) | `Stick`, `Goal`, `Level`, `Movement`, `Skate` |
| [camera-system.md](camera-system.md) | Camera types, `CameraManager`, `CameraType` enum |
| [replay-system.md](replay-system.md) | `ReplayManager`, `ReplayRecorder`, all replay data structs |
| [chat-communication.md](chat-communication.md) | `ChatManager`, `ChatMessage`, `QuickChat` |
| [server-admin.md](server-admin.md) | `ServerManager`, admin, ban, vote, whitelist systems |
| [networking.md](networking.md) | `ConnectionManager`, approval, serialization, Netcode patterns |
| [backend-services.md](backend-services.md) | `BackendManager`, WebSocket, TCP, response types |
| [settings-persistence.md](settings-persistence.md) | `SettingsManager`, `SaveManager`, `ApplicationManager` |
| [game-modes.md](game-modes.md) | `IGameMode`, Standard/Competitive/Public modes |
| [items-customization.md](items-customization.md) | `ItemManager`, appearance categories, customization state |
| [events-utilities.md](events-utilities.md) | `EventManager`, utility classes, singleton patterns |
| [enums-reference.md](enums-reference.md) | All 29 enums with known values |

## Quick Reference — Singleton Access Patterns

Most game systems use one of two singleton patterns:

```csharp
// MonoBehaviourSingleton<T> — access via Instance
GameManager.Instance
AudioManager.Instance
PlayerManager.Instance
PuckManager.Instance
ChatManager.Instance
ModManager.Instance
VoteManager.Instance
AdminManager.Instance
BanManager.Instance
WhitelistManager.Instance
SpectatorManager.Instance
ReplayManager.Instance
PhysicsManager.Instance
ConnectionManager.Instance
ConnectionApprovalManager.Instance
ThreadManager.Instance

// NetworkBehaviourSingleton<T> — access via Instance (server-side)
ServerManager.Instance
SynchronizedObjectManager.Instance
```

## Netcode for GameObjects Patterns

```csharp
// RPC naming convention
void Client_SomeRpc() { }   // runs on all clients
void Server_SomeRpc() { }   // runs on server

// Networked state
NetworkVariable<T> someState;
```

## Common Harmony Patch Targets for Mods

| Goal | Target Class | Key Method/Property |
|---|---|---|
| Intercept game phase changes | `GameManager` | `OnGameStateChanged()` |
| Modify player input | `PlayerInput` | Various input methods |
| Hook chat messages | `ChatManager` | RPC methods |
| React to goals/scoring | `GameManager` | `BlueScore`, `RedScore`, `Phase` |
| Change audio volumes | `AudioManager` | `SetGlobalVolume()` etc. |
| Intercept connections | `ConnectionApprovalManager` | Approval methods |
| Modify UI | Any `UIView` subclass | `OnEnable()`, show/hide methods |
