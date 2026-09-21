---
name: context-renderer-visibility-allocations
description: The per-object-per-frame helpers in GameObjectHelper and what they allocated — IsRenderersVisibleByCamera built two renderer arrays and a Plane[6] PER RENDERER on every call, and SetParticleSystemStartColor ran a GetComponent that always MISSED plus two GetComponentsInChildren arrays per call, the single biggest allocator in gameplay at ~2 KB/frame. The non-alloc rewrites, the EntityId-keyed cache idiom, and what a missing GetComponent really costs in a player build.
metadata:
  type: repo
  repo: game-lib-engine
  path: Assets/Code/Libs/game-lib-engine
  created: 2026-09-03
  updated: 2026-09-20
---

# The visibility test was the allocation, not the raycast

`GameObjectHelper.IsRenderersVisibleByCamera` is on the per-frame gameplay path twice over:
`BaseGamePlayerIndicator.LateUpdate` calls it once per off-screen indicator, `ActorShadow.Update`
once per actor. Every call allocated three ways.

## 1. `GeometryUtility.CalculateFrustumPlanes(camera)` — per renderer

The one-argument overload **returns a new `Plane[6]`**. It sat inside `Renderer.IsVisibleFrom`,
which the helper called once per renderer — so an actor with five renderers allocated five arrays,
per frame, per indicator watching it.

The `CalculateFrustumPlanes(camera, Plane[])` overload fills a caller-supplied buffer. And the
frustum is a property of the CAMERA, not the renderer, so it only needs computing once per object
however many renderers get tested.

## 2. `GetComponentsInChildren<Renderer>()` — twice per call

The array-returning form allocates. The `List<T>` overload fills a reused buffer.

## 3. The `IsRenderersVisible()` pre-pass could never change the answer

```csharp
if (!inst.IsRenderersVisible()) {   // walks every renderer asking `enabled`
    return false;
}
...
foreach (Renderer c in inst.GetComponentsInChildren<Renderer>()) {
    if (c.enabled) { ... }          // asks the same question again
}
```

A strict subset of the loop below it, at the cost of a second full sweep. Dropped.

## Shared buffers are safe here, and why

`renderersShared`, `frustumPlanesShared` and `RendererFrustum.planesShared` are static. Each is
filled and read inside the single call that fills it and never handed out, and these wrap Unity
APIs that are main-thread only anyway. A caller batching many renderers against one camera off the
main thread should pass its own buffer to `IsVisibleFrom(renderer, camera, planes)`.

## Kept, not removed

`RendererExtensions.IsVisibleFrom(camera)` keeps its signature — game-lib-* is shared, so the
extension stays and simply routes through the non-alloc path. The `(camera, planes)` overload is
additive.

## The same shape, in the same file: `SetParticleSystemStartColor` (2026-09-20)

`GameObjectHelper.SetParticleSystemStartColor` (`:1466`) is driven per frame off the player tint —
`BaseGamePlayerController.HandlePlayerEffectsObjectTick` lerps a colour every tick and pushes it
into the effect holder (`BaseGamePlayerController.cs:1193`), with four more callers on the actor
shadow, the zone score effects, the indicator items and `GameObjectChoice`. It did two lookups on
every call:

```csharp
ParticleSystem particleSystemCurrent = inst.GetComponent<ParticleSystem>();   // always missed
...
ParticleSystem[] particleSystems = inst.GetComponentsInChildren<ParticleSystem>(true);
```

The three live holders in this title (`Ground`, `Boost`, `GamePlayerShadow`) carry **no root
`ParticleSystem`**, only one in a child — so the `GetComponent` missed every frame, forever, and
the array form allocated a fresh `ParticleSystem[]` twice per call. Measured at **~2 KB/frame, the
biggest single allocator in a gameplay frame**.

Now resolved once behind `GetParticleSystems` (`:1429`), an `EntityId`-keyed cache
(`particleSystemCache`, `:1410`) built the same way as `GetPoolKey` further down the file (`:2347`).

**Two things the cache has to defend against, and both are general:**

- **Unity reuses an `EntityId` once an object is unloaded**, so the cache stores the holder
  reference beside the result and compares it — a reused id then degrades to an ordinary miss
  rather than handing back another object's components.
- A **destroyed member** still occupies the array while Unity's `==` reports it null, so every
  entry is checked before the entry is trusted; a failed check forces a re-resolve. Without that a
  cache outlives the thing it cached and hands back a dead `ParticleSystem`.

(`GetInstanceID` is deprecated as of Unity 6.5 — `GetEntityId()` is the key these caches use.)

### What a MISSING `GetComponent` actually costs

This is the third time this project has paid for it (see the `GamePlayerItem.GetCollectReach`
measurement in `game-lib-games/contexts/context-per-frame-actor-costs.md`), so it is worth stating
exactly:

- In the **Editor and development builds**, a miss builds a `GetComponentNullErrorMessage` string —
  measured **570–614 B** here, 537 B in the earlier item-path case. That part does **not** ship.
- What ships on **every** platform is the native component search itself, and, for the
  array-returning `GetComponentsInChildren<T>()`, the array — **40 B a call** in a player build,
  here twice per call per holder per frame.

So a profiler capture in the Editor **overstates** the win, and the honest claim is the lookup and
the array, not the byte count. Price such a fix on device before quoting it.
See [[gc-total-memory-lies-in-editor]] for the other half of that caution.

## Related

- `game-lib-games/contexts/context-per-frame-actor-costs.md` — the callers, and the rest of that pass
- `context-input-touch-launch-costs.md` — the other every-frame engine path, and the string/`Input.touches` allocation classes
- `context-timer-throttle-design.md` — the gate most of these callers sit behind, and what its cadence really is
