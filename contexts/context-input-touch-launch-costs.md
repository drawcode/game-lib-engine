---
name: context-input-touch-launch-costs
description: InputSystem.updateTouchLaunch runs every frame ahead of the gameplay throttle and cost 748 B/frame doing work nothing could read — an unmasked infinite Physics.Raycast plus a pick across every UI Toolkit panel, re-derived on frames with no touch and no click. Also the two allocation classes it leaked through — Input.touches builds a fresh Touch[] on every get, and Transform.name marshals a new managed string on every read.
metadata:
  type: repo
  repo: game-lib-engine
  path: Assets/Code/Libs/game-lib-engine
  created: 2026-09-20
---

# The touch-launch path pays every frame, and it is not throttled

`InputSystem.updateTouchLaunch` (`Engine/Events/InputSystem.cs:1378`) is driven from
`GameController.Update` **above** its `IsTimerPerf` gate (`:352`), so unlike almost everything else
in gameplay it really does run once per frame at the full framerate. Measured in a live round it
was **748 B/frame** — for a frame with no touch, no click and no player input at all.

## The shape of the waste: deriving a verdict nobody consumes yet

The method publishes a set of flags (`allowedTouch`, `inputButtonDown`, `inputAxisDown`,
`shouldTouch`) and then decides gestures from them. The flags come from `checkIfAllowedTouch`
(`:1677`), which is expensive by construction:

- `Physics.Raycast(screenRay, out hit, Mathf.Infinity)` with **no layer mask** — an unbounded query
  against every collider in the level, which also forces a full `Physics.SyncTransforms`;
- `UIPlatform.IsPointerOverUI(pos)` — a pick across **every** UI Toolkit panel;
- then four `hit.transform.name.Contains(...)` tests.

That ran on every frame, from the mouse branch of `updateTouchLaunch` (`:1436`), purely to
re-derive flags that **nothing reads until there is a click**: every gesture test below it is gated
on `GetMouseButtonDown`/`GetMouseButtonUp`, and on a click frame
`checkIfTouchesDownAllowed`/`checkIfTouchesUpAllowed` (`:1754`, `:1786`) have *already* run the
identical test at the identical position moments earlier in the same method.

It now runs only on a frame that actually has a click; an idle frame publishes the state
`checkIfAllowedTouch` leaves when the pointer hits nothing (`allowedTouch = true`, the three others
false).

**The rule.** An expensive derivation whose result is only consumed on an event belongs *on* that
event. Before caching or micro-optimising a per-frame query, check whether any consumer can observe
the answer on a frame where nothing happened — here the answer was recomputed ~60 times for every
one time it mattered, and the duplicate on the click frame made it twice for that one.

**Behavioural risk, and what was verified.** The idle-frame flag values are an assumption: they
mirror what `checkIfAllowedTouch` sets before its raycast, i.e. "pointer over nothing". What was
verified live is the click path — `IsPointerOverUI` true on 8/8 real HUD elements and false on 6/6
empty points. A real NGUI tap after the change was **not** re-tested on a device, and neither was
multi-touch, where the mouse branch is skipped entirely (`hasTouches`, `:1410`).

## Two allocation classes this path leaked through

Both are generic Unity facts, not local bugs, and both recur across `game-lib-games`
(see `game-lib-games/contexts/context-per-frame-actor-costs.md`).

**`Input.touches` builds a fresh `Touch[]` on every get.** It is a property, not a field. Three
gets per frame here — one to test emptiness, two `foreach` loops. `Input.touchCount` is a plain
read and `Input.GetTouch(i)` is indexed and allocation-free; the loops are indexed now
(`:1410`, `:1758`, `:1790`). On a device the array is per-frame garbage proportional to nothing
useful, since the common case is zero or one touch.

**`Transform.name` / `GameObject.name` marshal a NEW managed string out of native on every read.**
`checkIfAllowedTouch` read `hit.transform.name` four times for its four `Contains` tests — four
throwaway strings per hit. Read it once into a local (`:1705`). The same class is the reason
`GameObject.tag` is replaced by `CompareTag` wherever it appears in a per-frame or per-collision
path, and it recurs in `BaseGameController.getGamePlayerControllerObject` and in
`BaseGamePlayerController.OnCollisionEnter`, which read the same name five times per contact point.

There is no "just comparing" exemption: `name.Contains(...)`, `name == "x"` and `tag == "x"` all
materialise the string first.

## Related

- `context-timer-throttle-design.md` — why most gameplay code is NOT on this every-frame path
- `context-renderer-visibility-allocations.md` — the other per-frame engine helper that allocated
  per object
- `game-lib-games/contexts/context-input-axis-pads.md` — the virtual pads that consume these flags
