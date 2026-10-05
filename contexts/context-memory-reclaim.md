---
name: context-memory-reclaim
description: MemoryUtil — the one place the game asks for memory back. What it guarantees, where every caller is wired, the coverage map for screen changes and game<->UI flows, the three gaps closed on 2026-09-07 and how each was verified live, and what is still NOT covered. Read before adding any GC.Collect or Resources.UnloadUnusedAssets call anywhere.
metadata:
  type: reference
  repo: game-lib-engine
  path: Engine/Utility/MemoryUtil.cs
  created: 2026-09-07
  updated: 2026-09-07
  verified-against: Unity 6000.5.2f1, Editor play session, gcIncremental=1
---

# Memory reclaim — MemoryUtil

`Engine/Utility/MemoryUtil.cs`. Added 2026-08-28 (`e05516e`), hardened 2026-09-07
(`423384b` engine, `930ffef` game-lib-games).

## The rule

**Never call `GC.Collect()` or `Resources.UnloadUnusedAssets()` directly.** Every one of
those is a full, blocking, stop-the-world pause. Before MemoryUtil they were called from a
HUD coroutine, two audio recorders and the audio downloader — and at least one of them
could fire mid-round, which is exactly what a frame spike looks like.

Route through MemoryUtil instead:

| call | when | blocks? |
| --- | --- | --- |
| `RequestCollect(reason)` | "some memory just became garbage" — end of a wave, a pool trim, a panel teardown | never; nudges the incremental collector while busy |
| `RequestUnloadUnusedAssets(reason)` | assets may now be unreferenced | never inline; always deferred to a safe point |
| `CollectAtSafePoint(reason)` | a loading screen, results, a pause — cooldown-guarded | at the safe point |
| `CollectAtSafePoint(reason, force)` | a level load/teardown, worth a collect every time and cannot be spammed | at the safe point |
| `CollectAtSafePoint(reason, force, delaySeconds)` | a **screen change that animates in** — see §3 | after the settle delay |
| `CollectBlocking(reason)` | escape hatch only, the frame is already lost | yes, and warns if `busy` |

`SetBusy(bool)` is the gate. game-lib-engine cannot see game state, so the game layer tells
it. While busy nothing blocks; the `false` edge is itself a safe point and services
everything the round queued.

## Coverage map — every wired caller

| site | call | note |
| --- | --- | --- |
| `BaseGameController.Update` | `SetBusy(GameConfigs.isGameRunning)` | per frame, above the `isGameRunning` early-return so the END of a round is seen at all |
| `BaseGameController.changeGameState` | `SetBusy` + `CollectAtSafePoint` on `GameResults` / `GameQuit` | `GamePrepare` deliberately absent — `prepareGame` already collects, and does it BEFORE loading |
| `BaseGameController.prepareGame` | `CollectAtSafePoint(..., force: true)` | frees the level being left before the next one's assets land |
| `BaseUIController.showUI` | `CollectAtSafePoint(..., false, uiTransitionSettleSeconds)` | the game→UI transition; `showResults` reaches it through here |
| `MemoryUtil.OnSceneLoaded` | `CollectAtSafePoint` | **single-mode loads only** |
| `MemoryUtil.OnLowMemory` | forced, ignores `busy` and the cooldown | being killed by the OS costs more than a dropped frame |
| `MemoryUtil.OnApplicationPause` | forced | backgrounding is a free safe point, and is exactly when the OS decides whether to keep the process |
| `GameHUD` / `BaseGameHUD` / audio recorders / `AudioSystem` | `RequestCollect` + `RequestUnloadUnusedAssets` | all formerly direct `GC.Collect()` |

## Safe-point order — do not reorder

`ReclaimReferences` (pool trim + `onSafePointReclaim`) → `GC.Collect` → `UnloadUnusedAssets`
→ `GC.Collect`.

**Drop references FIRST.** `Resources.UnloadUnusedAssets` cannot free a mesh, material or
texture that a parked pool object still references, so a collect that runs before the pools
are trimmed frees nothing and the unload that follows still sees every asset as referenced.
That is what `poolKeepPerBucket` and `ObjectPool*Manager.trimPooled` exist for — without
them the level you just left stays resident behind a few hundred recycled bullets.

## The three gaps closed 2026-09-07, and how each was proved

1. **The driver did not exist until something asked.** In practice the first `SetBusy` or
   `showUI` — so boot, splash and the first menu load, the heaviest asset phase the app
   has, ran with **no `Application.lowMemory` handler and no backgrounding safe point at
   all**. Now a `[RuntimeInitializeOnLoadMethod(BeforeSceneLoad)]` creates it and resets
   every static. *Verified: `driver=True` at frame 2.*

2. **No scene-load hook.** The root ↔ game scene change (`GameUISceneRoot` ↔
   `GameSceneDynamic`) went through **no reclaim at all** — the largest single opportunity
   in the app, because a single-mode load has already destroyed the outgoing scene's
   objects. *Verified: `lastSafePointReason=scene-loaded-GameSceneDynamic` at frame 415,
   2 collects + 1 unload on a boot that previously did none.*

3. **A request made while a safe point ran was silently dropped.** It set the pending
   flags, was refused by the `safePointRunning` guard, and nothing re-checked them
   afterwards. Not theoretical: `UnloadUnusedAssets` spans several frames and a level
   transition fires a burst across exactly those frames. *Verified deterministically by
   injecting a request from an `onSafePointReclaim` handler — which runs after the flags
   are latched, i.e. the exact dropped window — and observing it serviced as
   `outer-probe+queued`.*

The re-run is guarded on `!busy`. A request that `RequestCollect` /
`RequestUnloadUnusedAssets` **deliberately deferred** because gameplay was live must keep
waiting for the `SetBusy(false)` edge; draining it here would put the spike this class
exists to prevent straight back into the round.

## 3. Screen changes animate — the safe point must wait

`showUI()` collected one frame before `AnimateIn()` started its tween, so the blocking
collect landed **on** the transition animation. A screen change is a safe point in the
sense that nothing is being aimed at, but it is not a silent one.

`uiTransitionSettleSeconds` (default 0.6) holds the work until the motion settles.
**Real time, not scaled** — menus and pause screens routinely sit at `timeScale` 0, where a
scaled wait never finishes. A round starting during the wait abandons the safe point and
hands the flags back to the `SetBusy(false)` edge.

*Measured, forced requests, same session:* undelayed serviced **2 frames** after the call
(f3966→f3968); delayed serviced **52 frames** after it (f2183→f2235).

## Verifying it from a probe

`lastSafePointReason` / `lastSafePointFrame` exist **because** turning `logEnabled` on
before the app boots is impossible for anything that happens during boot itself. With
`logEnabled = true` the log lines read
`MemoryUtil:safe point serviced collect unload pooledTrimmed:N reason:... heapMB:...`.

```csharp
return "driver=" + (GameObject.Find("_MemoryUtil") != null)
     + " tier=" + MemoryUtil.deviceTier
     + " lastSP=" + MemoryUtil.lastSafePointReason + "@f" + MemoryUtil.lastSafePointFrame
     + " collects=" + MemoryUtil.fullCollects + " unloads=" + MemoryUtil.unloads
     + " busy=" + MemoryUtil.busy;
```

## Device tiers

Derived once from `SystemInfo.systemMemorySize` and `processorCount`; unknown memory is
treated as Medium rather than punished. Low (<3GB or ≤4 cores) collects sooner, in smaller
bites, and keeps 4 per pool bucket; High keeps 16 and waits 30s between full collects. Set
`autoTuneForDevice = false` BEFORE the driver is created to pin your own values — which now
means before `BeforeSceneLoad`, so from an earlier `RuntimeInitializeOnLoadMethod`.

## NOT covered / open

- **Nothing measured on a device.** Every number here is Editor, desktop, tier High. The
  tier Low path has never run on real hardware.
- **No profiler attribution for the safe points themselves.** How long a full collect +
  unload actually costs at a transition on a phone is unknown; the settle delay moves the
  spike, it does not price it.
- **`onSafePointReclaim` has no subscribers.** Products drop no caches of their own. The
  UI Toolkit views (UXML/USS/atlas textures) are the obvious candidate.
- **`GameHUD.LoadLevelHandlerCo`** does a real `Application.LoadLevelAsync` scene load and
  is now covered by the scene hook — but whether that path is live in this game (vs the
  asset-based `prepareGame` path) was not established.
- `heapGrowthTriggerBytes` auto-trigger only runs when `isIncrementalAvailable`. This
  project has `gcIncremental: 1`, so it is live here; a project with it off degrades to
  safe points only.
- **Do not trust `GC.GetTotalMemory` deltas** as a per-frame allocation rate — see
  `context-spawn-path-costs.md`. Heap MB in the MemoryUtil log is for eyeballing trends
  only.
