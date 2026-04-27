# Items & Customization

## ItemManager

Static class — catalog of all game items (cosmetics, stick skins, headgear, jerseys, flags). Loaded from game data at startup.

**Access:** `ItemManager` (static — no `Instance` property)

## Item Classes

| Class | Description |
|---|---|
| `Item` | Base item — has an ID and display data |
| `ItemDetails` | Rich metadata: title, description, preview URL, Steam Workshop ID |
| `InstalledItem` | An item + its filesystem path (used for mods and workshop items) |
| `PlayerItem` | An item instantiated/assigned to a specific player |

## Appearance System

### AppearanceCategory Enum

Top-level categories in the appearance UI:

| Value | Description |
|---|---|
| *(values to verify)* | e.g. Headgear, Jersey, Stick, Flag, Tape |

### AppearanceSubcategory Enum

Sub-categories within each appearance category (e.g. Blue team jersey vs Red team jersey):

| Value | Description |
|---|---|
| *(values to verify)* | Team-specific or role-specific variants |

### AppearanceItem

Internal struct used within the appearance selection UI to represent a selectable item entry.

## Customization State

`PlayerCustomizationState` — `INetworkSerializable` struct that travels with the player across the network. Contains all appearance IDs needed to reconstruct the player's look on any client. See [player-system.md](player-system.md) for the full field list.

## Flags & Countries

| Class | Description |
|---|---|
| `Flag` | A flag item — links to a country and a texture |
| `Country` | Country data (name, code) |
| `CountryUtils` | Static helpers for country lookup/display |

### AvatarSize Enum

Used when requesting country or player avatar images at different resolutions:

| Value | Description |
|---|---|
| *(values to verify)* | Small, Medium, Large |

## Locker Room Cosmetics

The locker room (`LockerRoom`, `LockerRoomPlayer`, `LockerRoomStick`) renders each player's selected appearance before the game starts. Classes involved:

| Class | Description |
|---|---|
| `LockerRoom` | The locker room environment |
| `LockerRoomController` | Controller for locker room logic |
| `LockerRoomPlayer` | Player avatar in the locker room |
| `LockerRoomPlayerController` | Controller for locker room player |
| `LockerRoomStick` | Stick model shown in locker room |
| `LockerRoomStickController` | Stick controller |

## Rendering Components

| Class | Base | Description |
|---|---|---|
| `MeshRendererHider` | `MonoBehaviour` | Conditionally hides mesh renderers |
| `MeshRendererTexturer` | `MonoBehaviour` | Applies textures to mesh renderers at runtime |

These are used internally to swap cosmetic variants without instantiating separate GameObjects for each item.

## Modding Notes

- **Add custom cosmetic previews:** Patch `UIAppearanceController` to inject additional items into the selection UI
- **Force appearance:** Patch `PlayerCustomizationState` deserialization or the point where it's applied to the player model
- **Flag/country display:** Patch `CountryUtils` if you want to add custom flag entries
- **Block cosmetic changes in-game:** Patch `SettingsManager` property setters for headgear/jersey/stick fields
