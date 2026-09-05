---
name: context-spawn-path-costs
description: The item/actor spawn path measured with the profiler in a live round — PrefabsPool re-hashed the prefab path with SHA-1 on every lookup (505 bytes, 3.19 us per call), and the spawn frame itself costs 290 KB of GC and 32.6 ms of PlayerLoop. Also records three plausible spike causes that MEASUREMENT KILLED.
metadata:
  type: repo
  repo: game-lib-engine
  path: Assets/Code/Libs/game-lib-engine
  created: 2026-09-05
---

# What a spawn frame costs

Measured with the Unity profiler in a live round (48–84 items, ~57 fps in the Editor), 2026-09-05.

## The fix that landed

`PrefabsPool.PoolPrefab(path)` computed `CryptoUtil.CalculateSHA1ASCII(path)` on **every call**,
purely to key a dictionary whose prefab was already cached. `CalculateSHA1ASCII` allocates a fresh
`ASCIIEncoding` and an **undisposed** `SHA1CryptoServiceProvider` per call.

| | bytes/call | µs/call |
| --- | --- | --- |
| before | 505 | 3.19 |
| after (`bfc1177`) | **0** | **0.151** |

Correctness checked, not assumed: the memoized path returns the *same prefab instance* as a warm
call (`ReferenceEquals` true).

**The key scheme was deliberately NOT changed.** `prefabs` is a public field and these libs are
shared with other projects, so the SHA is memoized per path rather than replaced by the path
itself. See [[core-libs-shared-additive-only]].

`PoolGameObjects.PoolPrefab` carries the identical pattern and was **left alone** — no call sites
in this project, and it is shared. It also loads the asset twice (`AssetUtil.LoadAsset` to test,
then `Resources.Load` to store); not touched for the same reason.

## The spawn frame itself — still open

The frame where items spawn, from the profiler:

- **290,767 bytes** of GC allocation (median frame in the same round: 16,194)
- **32.6 ms of PlayerLoop** — a genuine ~2-frame hitch, and this is the Editor, so a phone is worse
- `GamePlayerControllerAnimation.Update()` on the player: **10.6 ms self time, 103 KB**
- `GC.Collect` 3.6 ms + `GarbageCollector.CollectIncremental` 3.0 ms in the same frame
- `GameItemController.Update()` 36 KB self + `GameController.loadItemCo()` 37 KB

The animation cost is **spike-only** — that script does not appear at all in the median frame,
where `BehaviourUpdate` totals 2.9 ms. Root cause NOT isolated; the MCP profiler connection was
revoked mid-investigation (see below).

## THREE HYPOTHESES THAT MEASUREMENT KILLED

Recorded because each was plausible enough to have been "fixed" on reasoning alone, and all three
were wrong. This is the same failure mode as iteration 10 §7.

| hypothesis | why it was plausible | what the measurement said |
| --- | --- | --- |
| `Animation[string]` indexer allocates | called **23×** per actor per frame in that Update, many literally duplicated back-to-back | **0 bytes**, 0.143 µs — Unity 6 caches the `AnimationState` wrappers |
| `SyncAnimationMessage`'s reflection scan is the spike | `GetComponents` + `GetMethod` per component, and a first timing said **426 ms** | **0.041 ms cold, 0.010 ms warm.** The 426 ms was one-time reflection-subsystem init inside the `eval` context, not game cost |
| the results screen allocates 1.4 MB/frame | `GC.GetTotalMemory` delta said so | profiler median for the same period: **11.9 KB**. See [[gc-total-memory-lies-in-editor]] |

The 23 duplicated indexer calls are still ugly (`if (x[run] != null) { if (x[run] != null) {`), and
`CrossFade`/`Blend` are called every frame while moving, which is an anti-pattern on legacy
`Animation`. Neither is a measured cost. **Do not "fix" either without a profiler capture first.**

## Tooling note

Only the **MCP** profiler tools can do this attribution — the `unity` CLI has no profiler command.
MCP approval is revoked by a domain reload, so a `.cs` edit mid-investigation ends the capture and
the user has to re-approve in *Project Settings > AI > Unity MCP*. Batch the code edits, or do all
the profiling before the first edit. See [[unity-mcp-dies-on-domain-reload]].

Pause play mode (`editor_pause`) before analysing: the profiler buffer is 2000 frames and scrolls
out from under a multi-call drill-down.

Frame indices are **off by one** between the tools: the range summary names frame N, and the
per-frame tools want N-1 to inspect it.

## Related

- `game-lib-games/contexts/context-per-frame-actor-costs.md` — the actor half, and the item fix
- `context-renderer-visibility-allocations.md` — the earlier allocation pass
