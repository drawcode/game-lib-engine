---
name: context-profile-progression-data
description: game-lib-engine data layer for Profile / Progression / RPG — DataObject+DataAttribute persistence, the definition catalogs (BaseGameLeaderboards/Statistics/Achievements/RPGTypes loaded from data/*.json), the BaseGameProfile* container family, and BaseGameState per-container JSON save/encrypt/sync. Referenced by the shared overview context-profile-progression-rpg-shared.
metadata:
  type: reference
  repo: game-lib-engine
  created: 2026-07-21
---

# game-lib-engine — Profile / Progression / RPG data layer

Engine half of the shared systems: the data model, the designer-authored
definition catalogs, the saved-profile containers, and persistence. Runtime
increment/report lives in game-lib-games ([[context-progression-runtime-networks]]);
cross-cutting overview + platform-generator + easy-setup in
[[context-profile-progression-rpg-shared]].

## Persistence primitives (`Engine.Game.Data`)
- **`BaseDataObject : Dictionary<string,object>`** — typed props stored by string
  key via `Get<T>(code[,default])` / `Set<T>(code,val)`; object *is* the dict, so
  it JSON-round-trips. Also holds a nested `Dictionary<string,DataAttribute>` under
  key `"attributes"`.
- **`DataAttribute`** = `{ uid, code, type, otype, name, val }`. `code`=key,
  `type`=value primitive, `otype`=category bucket for `GetAttributesList(otype)`.
- **`DataObject : BaseDataObject`** (pass-through) is the base of every profile
  container. **`GameDataObject`** adds `uuid/code/display_name/description/points/
  complete/...`. **`GameDataObjectLocalized`** runs name/desc through
  `Locos.GetReplaceLocalized(...)`.
- Design rule (stated in every class): **add state as attributes, never as new C#
  properties** — attributes never force a save-format migration; properties can.
- Gotchas: `Get<T>(code,default)` **inserts default on miss** (getter mutates);
  `GetAttribute(code)` returns an empty `DataAttribute`, never null.

## Definition catalogs (loaded from `data/*.json` as `DataObjects<T>`)
All expose `BaseCurrent`/`BaseInstance` singletons; `code` is the universal key.

| Class | Data file | Item fields |
|---|---|---|
| `BaseGameStatistics<T>` / `BaseGameStatistic:GameDataObjectLocalized` | `game-statistic-data.json` | `code`, localized name/desc, `order`(accumulate/ascending/descending), `store_count`, `key`, `type`(int/time) |
| `BaseGameAchievements<T>` / `BaseGameAchievement:GameDataObjectLocalized` | `game-achievement-data.json` | `code`, name/desc, `data.points`, `data.filters[]`, `data.networks[]`, `data.type`, flags `leaderboard/game_stat/global` |
| `BaseGameLeaderboards<T>` / `BaseGameLeaderboard:GameDataObject` | `game-leaderboard-data.json` | `code`, `display_name`, `data.datatype`, `data.direction`, `data.type`(statistic), `data.networks[]` |
| `BaseGameRPGTypes<T>` / `BaseGameRPGType` | `game-rpg-types-data.json` | extensible content records |

- `{{^game_..._display_name}}` tokens in the JSON are localization keys.
- **`GameNetworkData : GameDataObject`** (`{type, code}`) is the platform-mapping
  unit inside every `data.networks[]` — `type` ∈ GameNetworkType strings, `code` =
  the platform's leaderboard/achievement ID. This is the auto-generation hook.
- Achievement filter model: `GameFilter`→`GameFilterBase{codes[], codeLike,
  compareType→StatEqualityTypeEnum, compareValue, includeKeys(none/all/current for
  defaultKey/app_content_state/action)}`. Types: statisticSingle/Set/All/Like/
  Compare, achievementSet.
- Well-known stat codes: `times-played`, `time-played`, `total-wins`,
  `total-losses` (`BaseGameStatistics`).

## Profile containers (`Engine.Game.App.BaseApp`)
`Profile:DataObject` → `BaseGameProfile:Profile` (title `GameProfile`). Singleton
`GameProfiles.Current`/`.Instance`. `BaseGameProfile` = facade of
`Get/SetAttribute*` accessors: controls, audio volumes, current mode/world/level/
app-state/app-content-state, help/UI flags, custom audio JSON, third-party network
flag, `access-permissions` JSON list, game settings, `network_items` (per-social id/
token/name). Save signal = `Messenger.Broadcast(BaseGameProfileMessages.ProfileShouldBeSaved)`.

Sub-containers — each = `BaseGameProfileXxxAttributes` keys + `BaseGameProfileXxxs`
singleton + payload with `SetValue(code,val)`→`DataAttribute{otype}` and
`GetList()`=`GetAttributesList(otype)`:

| Container | otype | Holds |
|---|---|---|
| Currency | rpg | soft currency + `progress-currency` |
| Characters | character | multi-character roster; items nest RPG/progress/custom; statics `currentCharacter/currentProgress/currentRPG/currentCustom` |
| Customizations | customization | audio/colors/textures |
| Modes | mode | mission "content-collect" progress keyed app_state:world:level:action |
| Products | rpg | IAP unlocks, promo, product-local access-permissions (casts raw `List<string>` — divergent from BaseGameProfile's JSON form) |
| RPGs | rpg | global player RPG stats — see below |
| Statistic | statistic | `SetStatisticValue/GetStatisticValue` (double, negatives→0) |
| Achievement | achievement | `SetAchievementValue/GetAchievementValue` (bool) |
| Teams / Trackers / Vehicles | team/tracker/vehicle | simple keyed collections |

Copy-paste artifacts to ignore: Trackers/Vehicles `*Attributes` both declare
`ATT_TEAMS="teams"` unused.

## RPG data (`BaseGameProfileRPGs`, title `GameProfileRPGs`)
- **No XP→level curve exists** — `xp` and `level` are independent doubles; level
  only via explicit `Add/Subtract…Level`. Titles must add their own curve.
- Canonical schema `GameDataItemRPG` (double): duration, scale, speed, attack,
  defense, health, energy, jump, fly, boost, attack_speed, recharge_speed,
  upgrades_applied, upgrades, xp, level, power, currency, data. Progress keys:
  `progress-currency/xp/health/energy/level/points/stars/coins`.
- Live per-character progress (`GameProfilePlayerProgressItem`): XP default 10
  unbounded, Level default 0, Health/Energy Add **clamps [0,1]**.
- Global RPG (`BaseGameProfileRPG`): upgrades default 3, two currencies
  (`ATT_CURRENCY` default 10 + `ATT_PROGRESS_CURRENCY`), collectible points system
  syncing total into `GameProfileStatistics ATT_TOTAL_POINTS`.
- **Latent bug:** `GameProfileRPGItem.Get/SetCurrency` read/write the *xp* key.

## Persistence orchestration (`BaseGameState`)
- `GameState.SaveProfile()` serializes each container to its own keyed JSON file
  under `ContentPaths.appCachePathAllSharedUserData`, optional encrypt/compress
  (`ProfileConfigs.useStorageEncryption/useStorageCompression`), optional cloud
  `GameSync` — see [[context-cloud-save-sync]].
- **`KEY_PROFILE`** (e.g. `profile-v1111`) namespaces the schema version; bump it
  instead of writing a migration.

## Reuse
Subclass `GameProfile*`/`GameProfile*s` per container; extend `*Attributes` keys;
store new state via `Set/GetAttribute*`; author the three `data/*.json` catalogs;
set a fresh `KEY_PROFILE`; define `USE_GAME_LIB_GAMES` + provide
`ProfileConfigs`/`GameConfigs`. Small titles: skip Characters roster / Modes /
Teams / Vehicles / Trackers.
