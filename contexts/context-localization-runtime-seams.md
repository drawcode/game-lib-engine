---
name: context-localization-runtime-seams
description: Three things about the localization runtime that a consumer gets wrong — Tr(key, args) puts the raw key on screen in a game that doesn't ship it (use the formatted TrOrDefault overload), applying a locale writes a profile attribute even when SetLanguage is skipped, and the per-locale NumberFormat getter costs nothing per frame (measured).
metadata:
  type: repo
  repo: game-lib-engine
  path: Assets/Code/Libs/game-lib-engine
  created: 2026-09-21
---

# Localization runtime — the three seams that bite

`Engine/Game/App/BaseApp/GameLocalizationService.cs` and its `L10n` facade. This lib ships to
several games, and every item below comes from that: **a key this game has is a key another game
does not.**

## 1. `Tr(key, args)` is wrong in shared-lib code

`Tr` returns **the key itself** when nothing in the chain defines it. In a shared lib that means a
consumer game renders `game_ui_overview_mode_tip_status` where it wanted `Tip 1 of 3`. The
non-formatting case had `TrOrDefault(key, defaultValue)` for exactly this; the formatting case had
nothing, so call sites reached for `Tr(key, args)` and the trap was invisible in the game that owns
the keys.

Added `TrOrDefault(key, defaultFormat, params object[] args)` (`1cc57c5`, additive — the
two-argument overload still binds first for two arguments):

```csharp
string format = Has(key) ? Tr(key) : defaultFormat;
return string.Format(FormatCulture(), format, args);
```

**Rule for this lib:** `Tr` only where the lib itself guarantees the key. Everything a consumer
might not ship goes through `TrOrDefault`, carrying the English text as the default. The validator
in `contexts/base/localization/tools/validate_loc.py` now scans `TrOrDefault(` call sites too —
before that, every one of them looked like an unreferenced key.

## 2. Applying a locale WRITES the profile, even without `SetLanguage`

A probe that wants another locale without touching the player's save reaches for the private
`GameLocalizationService.Apply(code, true)` instead of `SetLanguage`. That does skip the
`app-language` write — and still persists:

```
Apply -> BuildChain -> Locos.Instance.ChangeCurrent(targetFileCode)
      -> DataObjects.SetStateCode -> GameProfiles.Current.SetGameDataState(pathKey, code)
```

so the legacy `game-localization-data` attribute lands on disk. `GetAppLanguage()` returning the
old value proves nothing — read the file.

**The two attributes are not peers.** `app-language` is authoritative and is written only by
`SetLanguage`. `game-localization-data` is a derived mirror, rewritten on every apply and never
read back as the user's choice; seeded deliberately out of sync it heals itself on the next save.
Nothing may restore either from a profile backup — that silently reverts the player's language.

## 3. Boot applies the locale twice, on purpose

`Init()` runs before the profile exists, so it applies the pref-mirror/system value; the host then
calls `ResyncFromProfile()` once the profile has loaded (in this product,
`GameGlobal.InitContentSystemPost`). Measured live: `L10n=en` at the loader scene with
`app-language=[]`, then `L10n=de` at the menu. **A reading taken during the first window is not a
bug** — check the scene before believing it. `ResyncFromProfile` compares the profile against the
**pref mirror**, not against what was applied, so a host that never calls it leaves the early value
in place.

## 4. `NumberFormat` is free — measured, not assumed

`L10n.NumberFormat` caches a `NumberFormatInfo` per locale, so locale-aware counters can sit in an
`Update`. Live round, 121 profiled frames per locale: `GameHUD.Update()` allocated **76,816 B in de
against 76,414 B in en** at the same sample count. 50k-iteration micro-benchmark: the getter
measures **0.000 B/call**, and `ToString("N0", NumberFormat)` runs 0.302 us against 0.301 us for a
bare `ToString("N0")`.
