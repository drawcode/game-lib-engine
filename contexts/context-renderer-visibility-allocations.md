---
name: context-renderer-visibility-allocations
description: IsRenderersVisibleByCamera allocated two renderer arrays and one Plane[6] PER RENDERER on every call, and it is called per object per frame by the off-screen indicators and the actor shadows. The non-alloc rewrite, and the redundant pre-pass that doubled the cost for no answer.
metadata:
  type: repo
  repo: game-lib-engine
  path: Assets/Code/Libs/game-lib-engine
  created: 2026-09-03
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

## Related

- `game-lib-games/contexts/context-per-frame-actor-costs.md` — the callers, and the rest of that pass
