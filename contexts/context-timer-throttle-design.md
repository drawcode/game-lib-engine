---
name: context-timer-throttle-design
description: GameObjectTimer's IsTimerPerf gate — its modifier was 30/currentFPS, which SHRANK the interval as the framerate rose, so at 120fps every gate in the game passed every frame and the whole update cadence scaled with the hardware; what the interval means now that it is clamped at 1, why anything time-based behind a gate must charge REAL elapsed time, the FPSDisplay instance the modifier divides by (an inactive one froze it), and the last_time boxing that a plain backing field now overrides.
metadata:
  type: repo
  repo: game-lib-engine
  path: Assets/Code/Libs/game-lib-engine
  created: 2026-09-20
---

# The throttle gate: what it promises, and what it used to do instead

`GameObjectTimer` (`Engine/GameObject/GameObjectTimer.cs`) is the load-shedding throttle every
gameplay `Update` in `game-lib-games` sits behind. One instance per behaviour —
`GameObjectTimerBehavior.gameObjectTimer` is a `new GameObjectTimer()` field (line 9 of
`GameObjectTimerBehavior.cs`), not a singleton — so each actor keeps its own `last_time` per key
and the actors are not synchronised to one another.

The call chain is three layers deep and the modifier is applied at the bottom, which is why the
defect below survived so long:

```
IsTimerPerf(key, modifier)          :194  delta  = GetInterval(key) * modifier
  IsTimerPerfRelative(key, delta)   :186  passes currentModifier as the timer modifier
    IsTimer(key, delta, modifier)   :168  fires when last_time + (delta * modifier) < Time.time
```

`GetInterval` returns `defaultInterval` = `1f/30f` for every key registered in `InitIntervals`
(`:83`), and the per-call `modifier` argument is the caller's own coarseness dial — gameplay passes
`IsPlayerControlled ? 1f : 2f`, so a non-player actor is throttled to half the player's rate.

## The defect: the modifier ran the wrong way

`currentModifier` was `GetFPSOffset()` alone, i.e. `desiredFPS / currentFPS` (`:208`). That is a
number **below 1 whenever the game runs faster than 30fps**, and it MULTIPLIES the interval — so a
higher framerate produced a SHORTER gate interval. The formula's intent was load shedding (tick
less often when the machine is struggling); what it actually did was tick *more* often the faster
the machine was, until the interval fell under one frame period and every gate in the game passed
on every frame.

That is what was happening in this title. With `FPSDisplay.lastFPS` reading 165.87 (see below) the
modifier came out at `30/165.87 ≈ 0.18`, so the `game-update-all` interval was about 6 ms — shorter
than a frame at 120fps. Measured actor tick rate: **127/s, and it followed the framerate**, meaning
a phone and a desktop ran the same round at different simulation rates.

Clamped at 1 (`frameRateIndependent`, `:235`; `currentModifier`, `:237`) the interval is a **floor
in real seconds**:

- `game-update-all` with caller modifier `1f` → at most **30 ticks a second**, on any hardware.
- caller modifier `2f` → at most 15 a second.
- Below the 30fps target the offset is still > 1 and still stretches the interval, so the
  load-shedding the original formula wanted is intact — it just can no longer run the other way.

Measured after the clamp: **26 ticks/s at 105–124fps and 26 ticks/s at 28fps.** (26 rather than 30
because the gate can only fire on a frame boundary and a fractional remainder is dropped each time.)

`frameRateIndependent = false` restores the old scaling for another product that depended on it —
these libs are shared, so the behaviour change is a flag, not a rewrite.
See [[core-libs-shared-additive-only]].

## The rule this leaves behind: sampled is fine, integrated is not

A gate changes how *often* code runs, so:

- Code that **samples** a value behind a gate is unaffected in kind — it just resamples less often.
  The animation cadence is the example (`game-lib-games/contexts/context-animation-speed-cadence.md`).
- Code that **integrates** time behind a gate is wrong unless it charges the elapsed time since the
  previous TICK. `Time.deltaTime` is the delta of the one frame the tick happened to land on, and
  every frame the gate skipped is silently thrown away.

The round countdown was exactly this: `runtimeData.SubtractTime(Time.deltaTime)` inside
`checkForGameOver`, behind the `game-update-all` gate. It was **accidentally correct** only while
the throttle bug made the gate pass every frame. Clamp the modifier and the same line started
charging 0.248 of the real seconds that passed — a "90 second" round ran for minutes. The fix lives
in `game-lib-games/Game/Controller/BaseGameController.cs:3406`, `SubtractRoundTimeElapsed()`, which
stamps `Time.time` per tick and subtracts the difference; it is deliberately `public virtual` so an
app override reuses it instead of re-deriving the bug (the app's override had done exactly that).

**Re-measure anything time-based after changing a tick rate.** A throttle fix exposes every
integration that was riding on the broken cadence.

## The gate divides by a component that may not be running

`currentFPS` (`:211`) is `FPSDisplay.GetCurrentFPS()` under `USE_GAME_LIB_GAMES`. That is a
MonoBehaviour in `game-lib-games/Tools/FPSDisplay.cs`, and both instances in this title's game scene
are **inactive**. An inactive component never runs `Update`, so `lastFPS` was frozen at whatever it
last measured — found live at a stale **165.87** — and every `IsTimerPerf` gate in the game, plus
both spawn directors, were dividing by it.

`FPSDisplay.isInst` now requires `Instance.isActiveAndEnabled`, not just `Instance != null`
(`FPSDisplay.cs:33`), and the no-instance fallback is `targetFPS` = 30 (`:26`, `:73`) rather than a
made-up `21f` penalty — so with no live display the modifier comes out at exactly 1 and gates run at
their authored interval. `IsFPSLessThan` answers off `GetCurrentFPS()` too (`:82`); it used to
return `true` with no live display, i.e. "assume the worst", which quietly put every `isUnderNNFPS`
caller into its degraded path.

**A frozen reading is worse than no reading.** For anything whose value is produced by an `Update`,
test `isActiveAndEnabled`, never `!= null`.

`FPSDisplay` had a second fault underneath that: the accumulator reset (`timeleft`/`accum`/`frames`)
sat in the innermost `else` of the colour-picking code, so it needed a label AND `fps >= 27`. Below
27fps, or in a scene with no label at all, the window never closed and `lastFPS` became a **lifetime
cumulative average** a long session could not move. It is a rolling average over `updateInterval`
again (`FPSDisplay.cs:170`), which matters well beyond the readout — the throttle and the spawn
directors now follow the current framerate and relax again once a dip clears.

**NOT verified:** the rolling average through a genuinely active instance, and the `isUnderNNFPS`
callers. Everything above was measured with both instances inactive.

## `last_time` no longer lives in the attribute dictionary

`GameObjectTimerData` is a `GameDataObject`, whose `last_time` is
`Get<double>/Set<double>(BaseDataObjectKeys.last_time)` over a string-keyed
`Dictionary<string, object>` (`Engine/Game/Data/GameDataObjects.cs:2085`). Every timer fire writes
it, on every actor, so every fire **boxed a double**: measured 1 alloc / 24 B on roughly 60–65% of
calls, and this is the hottest write in the engine.

`GameObjectTimerData` now overrides the property with a plain `double lastTimeValue` backing field
(`GameObjectTimer.cs:36-46`).

**What that costs anyone reading it through the raw DataObject API:** the attribute is simply not
there any more. `obj.Get<double>("last_time")` returns the default, attribute enumeration will not
list it, and a serialiser will omit it — while the property still answers correctly. Worse,
`obj.Set<double>("last_time", x)` writes the dictionary and the property will not see it, so the
two diverge silently. This is safe **only** because `GameObjectTimerData` exists nowhere except
`GameObjectTimer.timers`: nothing serialises it and nothing enumerates it. Do not copy the pattern
onto a data object that is persisted or inspected generically.

Two more lookups went with it, both on the same per-gate path: `GetInterval` uses `TryGetValue`
instead of `ContainsKey` + indexer (`:114`), `GetTimer` the same instead of `Has` + `Get` (`:144`),
and `IsTimer` no longer re-`Set`s the object it was just handed (`:173`) — `GetTimer` has already
stored that exact reference under that key.

Related: `game-lib-games/contexts/context-per-frame-actor-costs.md` (the callers, and the frame
budget this gate now governs); the argument-allocation trap in front of these gates is
`workspace context-handoff-gameplay-tuning-iter21` §1.

## Also in this pass: `app-mode-game-missions`

`BaseAppModes.cs:29` adds the PLURAL key. The content data ships `app-mode-game-missions`, the
constant was the singular `app-mode-game-mission`, so `isAppModeGameMission` (`:103`) never matched
and `checkForGameOver`'s mode branch could never fire — a missions round ignored both the expired
timer and player death. Both spellings are accepted rather than renaming the constant, because
another product's data may still ship the singular. See
`game-lib-games/contexts/context-weapon-hitscan-and-gameover-latch.md` §4 for what that branch
gates.
