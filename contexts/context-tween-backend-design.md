---
name: context-tween-backend-design
description: Design (plan chunk 1.1) — ITweenBackend/ITweenTarget provider seam in TweenUtil with AnimationEasing as the default internal backend; contracts, adapters, cancellation model, and 1.2 implementation/test checklist
metadata:
  type: design
  repo: game-lib-engine
  plan: plan-ui-migration-uitoolkit
  chunk: "1.1"
  status: approved-design
  created: 2026-07-12
---

# Tween provider seam: ITweenBackend + AnimationEasing default backend

Design for plan chunk 1.1 (tween unification). Implemented by 1.2. Call-site surface of
`TweenUtil` (173 sites) is preserved verbatim; `UITweenerUtil` (100 sites) gets shimmed onto
TweenUtil in 1.3. Nothing above the provider layer may reference iTween/LeanTween/NGUI tweener
or `UnityEngine.UIElements` — those names may appear only inside backend/adapter files.

## Verified current state (drives the design)

- `Engine/Utility/TweenUtil.cs` (1873 lines): static facade, 5 op families
  (Move/Scale/Rotate/Fade/Color × [defaults | lib-explicit | meta] overloads) + directional
  Show/Hide helpers (`ShowObjectTop/Bottom/Left/Right`, `TweenObjectState`) using ±4500-unit
  closed positions. `TweenMeta` carries time/delay/ease/loop/coord/onStart/onComplete/onFinal/
  onUpdate/stopCurrent.
- **Forced-NGUI override**: `MoveToObject/ScaleToObject/RotateToObject/ColorToObject(meta,…)`
  hard-set `meta.lib = TweenLib.nguiUITweener` under
  `#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3 || USE_EASING_NGUI`; Fade routes NGUI sprites there
  conditionally. So **runtime today is 100% NGUI UITweener**; removal is chunk 1.5, not 1.2.
- **Pre-existing quirks (preserve knowledge, decide at 1.5):**
  - `ScaleToObjectLeanTween/iTween/UITweener` bodies call `MoveToObject`, not `ScaleToObject`
    (TweenUtil.cs:772-815) — lib-named scale entries misroute to position.
  - `ScaleToObject(meta,…)` NGUI branch is fully commented out (TweenUtil.cs:932-949) →
    **scale tweens are a no-op in production today**. When internalEasing makes them real,
    screens may move differently; this is plan risk 4 (feel drift), tuned per screen at 1.5.
  - NGUI branches never invoke onStart/onComplete/onFinal callbacks (commented out,
    e.g. TweenUtil.cs:728-730) — callers relying on completion callbacks only worked on the
    (dead) LeanTween/iTween paths. internalEasing must implement callbacks properly; audit
    call sites passing onComplete during 1.4 for double-fire assumptions.
  - `FadeToObject`/`ColorToObject` implement the `-a-NN` child-name alpha-cap convention in
    facade code (TweenUtil.cs:1436-1466, 1703-1729) — keep in facade, not backend.
  - Fade/Color auto Show() on alpha>0 begin and Hide() on alpha==0 complete — keep in facade
    by composing into onStart/onComplete before dispatch.
- `Engine/Animation/AnimationEasing.cs`: full Penner static equation set (keep as the single
  math source) + a minimal keyed float animator that is **broken/limited**: `easeUpdate`
  hardcodes `QuadEaseInOut` ignoring `item.equationType`; `timeDelay` never applied; no loops;
  no callbacks; removal queue pops only one item per frame. Zero call sites → free to upgrade
  in place.
- `Engine/Animation/{Ease,Linear,Quad,…}.cs`: duplicate Penner set used by `AnimationHelper`.
  Leave untouched; backend uses `AnimationEasing` statics only.
- `UITweenerUtil` (games repo, Tools/UITweenerUtil.cs): MoveTo/RotateTo/ColorTo/FadeTo/
  FadeIn/FadeOut/Begin<T>/ResetTween + CameraFade/CameraColor. Its op set is covered by
  ITweenBackend below; 1.3 shims it onto TweenUtil (CameraFade → fullscreen fade op via
  facade helper, not backend-specific).
- `UIPanelBase.cs:467` (games-ui) calls `LeanTween.cancelAll()` → becomes
  `TweenUtil.CancelAll()` in 1.4.

## Contracts (new files in `Engine/Animation/Tween/`)

Namespace `Engine.Animation`. TweenUtil (Engine.Utility) consumes them.

```csharp
public interface ITweenTarget {
    object native { get; }              // Transform | VisualElement | ...
    int targetId { get; }               // stable id for cancel keys (GO instanceID / RuntimeId)
    Vector3 GetPosition(TweenCoord coord); void SetPosition(Vector3 v, TweenCoord coord);
    Vector3 GetScale();                  void SetScale(Vector3 v);
    Vector3 GetRotation(TweenCoord coord); void SetRotation(Vector3 euler, TweenCoord coord);
    float GetAlpha();                    void SetAlpha(float a);
    Color GetColor();                    void SetColor(Color c);
}

public interface ITweenBackend {
    void Move(ITweenTarget t, Vector3 to, TweenMeta meta);
    void Scale(ITweenTarget t, Vector3 to, TweenMeta meta);
    void Rotate(ITweenTarget t, Vector3 to, TweenMeta meta);
    void Fade(ITweenTarget t, float to, TweenMeta meta);
    void ColorTo(ITweenTarget t, Color to, TweenMeta meta);
    void Value(string key, float from, float to, TweenMeta meta, Action<float> onValue);
    void Cancel(ITweenTarget t);        // all channels on target (stopCurrent semantics)
    void Cancel(ITweenTarget t, TweenChannel channel);
    void Cancel(string key);            // Value tweens
    void CancelAll();
    bool IsTweening(ITweenTarget t);
}

public enum TweenChannel { position, scale, rotation, alpha, color, value }
```

- `TweenLib` enum (TweenUtil.cs:18) gains `internalEasing`.
- Registration on `TweenUtil`: `public static ITweenBackend backend` with lazy default
  `EasingTweenBackend.Instance`; `SetBackend(ITweenBackend)` for future providers. Scripting
  defines keep gating which backend *files* compile, never call-site branches.
- Target resolution stays inside TweenUtil: `ITweenTarget ResolveTarget(GameObject go)` →
  `TransformTweenTarget`; a `ResolveTarget(object)` overload returns
  `VisualElementTweenTarget` for `VisualElement` (used by the UI Toolkit backend in Phase 2+).

## EasingTweenBackend (default; `TweenLib.internalEasing`)

Upgrade `AnimationEasing` in place (zero call sites — safe):
- `easeUpdate` dispatches on `item.equationType` via an explicit switch over the existing
  static methods (no reflection).
- Honor `timeDelay`: item inactive (val = valStart, no callbacks) until
  `Time.time >= timeStart + timeDelay`.
- Loop support per `TweenLoopType`: `once` (remove at end), `loop` (restart timeStart),
  `pingPong` (swap valStart/valEnd and restart), `bounce` → treat as pingPong (matches
  existing ConvertLibLoopType collapses).
- Callbacks on `AnimationItem`: `onStart` (once, first active tick), `onUpdate(double val)`
  (each tick), `onComplete` (end of once-loop only). Backend composes meta.onComplete +
  meta.onFinal exactly like the facade does today (CombineAction) — end value is always
  snapped exactly (`val = valEnd`) before onComplete.
- Fix removal queue: drain the whole queue per Update, not one entry.
- `AnimationEasing` itself stays a plain float animator. `EasingTweenBackend : ITweenBackend`
  is a separate class that composes it: each op registers 1-3 float items (e.g. Move = x,y,z
  channels driven off a single normalized item applying `Vector3.LerpUnclamped(from, to, t)`
  through `ITweenTarget.SetPosition` — prefer ONE item per op with an apply delegate over
  3 scalar items, so keys and callbacks stay 1:1 with the logical tween).
- **Cancel keys**: `"{targetId}:{channel}"`. `stopCurrent=true` cancels that op's channel on
  that target before starting (matches LeanTween.cancel(go)/iTween.Stop(go) closely enough:
  those cancel all channels — do all-channels cancel to match, i.e. `Cancel(t)`).
  `CancelAll()` clears every item — replaces `LeanTween.cancelAll()`.
- Ease mapping: `TweenEaseType.linear/quadEaseIn/quadEaseOut/quadEaseInOut` →
  `Equations.Linear/QuadEaseIn/QuadEaseOut/QuadEaseInOut`. (Only these 4 exist at call sites;
  the full Equations enum stays available for animation-preset tokens at 1.5.)

## Adapters

`TransformTweenTarget` (Engine/Animation/Tween/TransformTweenTarget.cs):
- position/rotation: world vs local per `TweenCoord`; scale: localScale.
- alpha/color resolution order (mirrors today's behavior, incl. coexistence):
  1. `#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3` — if GO (or children) carry NGUI `UIWidget`s:
     replicate NGUI 2.7 `TweenAlpha`/`TweenColor` semantics. **1.2 implementer: read
     `Assets/NGUI/Scripts/Tweening/TweenAlpha.cs` (2.7) first and mirror exactly which
     widgets it touches (self vs children)** — panel fades looking identical is the Phase 1
     gate. This is the one adapter block allowed to name NGUI types, behind the define.
  2. `CanvasGroup` → group alpha (color: tint Graphics individually).
  3. UGUI `Graphic` (Image/Text) → `.color` (matches `TweenUtil.SetImageAlpha`).
  4. `Renderer` → material color (instantiated material, as LeanTween.alpha did).
- Show/Hide side effects stay in the TweenUtil facade (unchanged), not the adapter.

`VisualElementTweenTarget` (Engine/Animation/Tween/VisualElementTweenTarget.cs):
- The only tween file referencing `UnityEngine.UIElements` (module ships with Unity 6 — no
  asmdef/define gate needed).
- position → `style.translate` (px); scale → `style.scale`; rotation → `style.rotate` (z);
  alpha → `style.opacity`; color → `style.unityBackgroundImageTintColor`, falling back to
  `style.color` for text elements. `targetId` → `element.GetHashCode()` (stable per element);
  `TweenCoord` ignored (translate is always self-relative).
- Driven by the same `EasingTweenBackend` Update pump — no UI Toolkit scheduler dependency,
  works in play mode and (later) editor panels.

## Facade wiring (1.2 scope in TweenUtil.cs)

- Add `TweenLib.internalEasing` branches to the 5 `(meta, …)` op bodies:
  `else if (meta.lib == TweenLib.internalEasing) { backend.Move(ResolveTarget(meta.go), pos, meta); }`
  — **no `#if` around these branches** (internal backend always compiles).
- Do NOT touch the forced-NGUI overrides or existing lib branches — that's 1.5.
- Add: `public static void ValueTo(string key, float from, float to, float time, float delay,
  TweenEaseType ease, Action<float> onValue, Action onComplete = null)`,
  `Cancel(GameObject go)`, `Cancel(string key)`, `CancelAll()`.
- Fullscreen fade for the `UITweenerUtil.CameraFade` shim (1.3): facade helper
  `FadeScreenOverlay(float amount, float time)` — implement against the existing overlay GO
  the current CameraFade drives (check `iTween.CameraFadeAdd` usage in games repo at 1.3).

## Editor tests (1.2, `game-lib-engine` Tests/ — EditMode)

1. Equation correctness: each of the 4 mapped eases sampled at t = 0/.25/.5/.75/1 equals the
   Penner closed form.
2. Delay: no value change and no onStart before delay elapses (simulate via item timeStart
   manipulation, not real waiting).
3. Loop/pingPong: value at 1.5× duration matches restart/reverse expectations.
4. Callback order and counts: onStart once, onUpdate ≥ 1, onComplete exactly once (once-loop);
   end value snapped exactly to target.
5. Cancel: per-target cancel stops all its channels, key cancel stops Value tween, CancelAll
   empties; canceled tweens fire no further callbacks.
6. TransformTweenTarget: pos/scale/rot local+world get/set round-trips on a temp GO;
   alpha on CanvasGroup and UGUI Image.
7. VisualElementTweenTarget: translate/opacity/scale on a detached VisualElement.
8. PlayMode smoke (1 test): MoveToObject with lib=internalEasing actually moves a GO across
   3 pumped frames.

## Out of scope for 1.1/1.2 (later chunks)

- Removing forced-NGUI overrides, `USE_EASING_*` gates, vendored lib deletion → 1.5/1.6.
- Named animation-preset tokens (`panel-in-left`, durations/eases as data) → 1.5, then
  formalized into the bitty token source at 2.1.
- UITweenerUtil shim → 1.3 (games repo).
