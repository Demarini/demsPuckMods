# UI System

The game uses **Unity UI Toolkit** (not uGUI/UGUI). All screens are `UIView` subclasses; each has a paired `UIViewController<T>` controller.

## UIManager

`MonoBehaviourSingleton<UIManager>` — owns and coordinates the entire UI hierarchy. Routes phase transitions, shows/hides screens, and delegates to specialized overlay/popup/toast managers.

**Access:** `UIManager.Instance`

## UIView

`MonoBehaviour` — base class for all UI screens. Subclasses are the individual screens listed below.

## UIViewController\<T\>

Generic controller base paired with a `UIView` subclass. Contains business logic, subscriptions, and data binding for its screen.

## UIPhase

Enum controlling which top-level UI state is active (main menu, in-game, etc.).

## UIState

Struct that holds the current UI phase and relevant context data.

## Specialized Managers

| Class | Access | Description |
|---|---|---|
| `UIOverlayManager` | Via `UIManager.Instance` | Manages overlay elements drawn on top of all screens |
| `UIPopupManager` | Via `UIManager.Instance` | Queues and displays popup dialogs |
| `UIToastManager` | Via `UIManager.Instance` | Queues and displays toast notifications |

## Screens

### Main Navigation

| View | Controller | Description |
|---|---|---|
| `UIMainMenu` | `UIMainMenuController` | Root menu — play, settings, appearance, mods |
| `UIFooter` | `UIFooterController` | Persistent footer with nav icons |
| `UIPlay` | `UIPlayController` | Play/lobby entry screen |

### Lobby & Team Selection

| View | Controller | Description |
|---|---|---|
| `UITeamSelect` | `UITeamSelectController` | Blue/Red team picker |
| `UIPositionSelect` | `UIPositionSelectController` | Attacker/Goalie picker |

### In-Game HUD

| View | Controller | Description |
|---|---|---|
| `UIHUD` | `UIHUDController` | Main in-game heads-up display |
| `UIGameState` | `UIGameStateController` | Score, period, clock display |
| `UIScoreboard` | `UIScoreboardController` | Tab scoreboard |
| `UIAnnouncements` | `UIAnnouncementsController` | Center-screen announcements (goal, period start, etc.) |
| `UIMinimap` | `UIMinimapController` | Minimap display |
| `UIPlayerUsernames` | `UIPlayerUsernamesController` | Floating player name tags |
| `UIPlayerMenu` | `UIPlayerMenuController` | Right-click context menu on a player |

### Social & Communication

| View | Controller | Description |
|---|---|---|
| `UIChat` | `UIChatController` | Text chat window |
| `UIFriends` | `UIFriendsController` | Steam friends list |

### Server & Matchmaking

| View | Controller | Description |
|---|---|---|
| `UIMatchmaking` | `UIMatchmakingController` | Ranked/casual matchmaking UI |
| `UIServerBrowser` | `UIServerBrowserController` | Browse public servers |
| `UINewServer` | `UINewServerController` | Create/host a new server |

### Settings & Customization

| View | Controller | Description |
|---|---|---|
| `UISettings` | `UISettingsController` | Graphics, audio, control settings |
| `UIAppearance` | `UIAppearanceController` | Cosmetic customization (jersey, stick, headgear) |
| `UIIdentity` | `UIIdentityController` | Player profile/flag |
| `UIMods` | `UIModsController` | Browse, enable, disable mods |

### Pause & Debug

| View | Controller | Description |
|---|---|---|
| `UIPauseMenu` | `UIPauseMenuController` | In-game pause menu |
| `UIHierarchy` | `UIHierarchyController` | Debug scene hierarchy viewer |
| `UIDebug` | `UIDebugController` | Debug overlay |

## Popup System

| Class | Description |
|---|---|
| `Popup` | Data class describing a popup (title, content, buttons) |
| `IPopupContent` | Interface for custom popup content views |
| `PopupTextContent` | Simple text message popup |
| `PopupPasswordContent` | Password entry popup |

### Showing a Popup

```csharp
// UIPopupManager queues popups — call via UIManager
UIManager.Instance.// ... check actual method via decompiled source
```

## Toast System

| Class | Description |
|---|---|
| `Toast` | Data class describing a toast (message, duration) |

## UI Components (VisualElements)

| Class | Base | Description |
|---|---|---|
| `Icon` | `VisualElement` | Reusable icon element |
| `IconButton` | `VisualElement` | Button with an icon |
| `PlayButton` | `VisualElement` | Styled play action button |
| `Spinner` | `VisualElement` | Animated loading indicator |
| `Mmr` | `VisualElement` | MMR/rating display badge |
| `Friend` | `VisualElement` | Single friend list entry |
| `User` | `VisualElement` | Player/user display card |
| `ChildClassifier` | `VisualElement` | Layout helper for categorized children |
| `KeyBindField` | `VisualElement` | Key binding row in settings |
| `Overlay` | — | Overlay layer element |

## MonoBehaviour UI Components

| Class | Description |
|---|---|
| `Scoreboard` | Renders the scoreboard table |
| `ScoreboardController` | Data binding for scoreboard |
| `Hover` | Hover highlight behavior on an element |

## Utilities

| Class | Description |
|---|---|
| `UIUtils` | Static helper methods (element query shortcuts, etc.) |

## Enums

### SpinnerDirection

| Value | Description |
|---|---|
| Clockwise | Spins right |
| CounterClockwise | Spins left |

## Harmony Notes

`VisualElementHarmonyPatch` is a class in `PuckNew` that applies a Harmony patch to Unity's UI Toolkit `VisualElement` — this suggests the game itself patches the UI layer. Be aware of potential interaction if your mod also patches `VisualElement`.

Patching individual screen `UIView` subclasses is the cleanest way to inject UI behavior:

```csharp
[HarmonyPatch(typeof(UIHUDController), "SomeMethod")]
class Patch_HUD
{
    static void Postfix(UIHUDController __instance) { }
}
```
