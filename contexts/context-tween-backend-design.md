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
  - Fade/Color auto Show() on alpha>0 begin and Hide() on alpha==0 complete — **DO NOT wire
    these into the internal backend** (learned at the Phase 1 gate, 2026-07-12): the NGUI
    path never fired them, and a decade of panel code owns active-state itself. Wiring them
    deactivated objects on fade-out and broke HUD/pause + the bot-preview card on 15 panels
    ("Coroutine couldn't be started because the game object is inactive"). Tweens are purely
    visual; the internalEasing branches pass raw meta (user callbacks only).
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
- **Cancel keys**: `"{targetId}:{channel}"`. `stopCurrent=true` cancels ONLY the same channel
  on that target (gate learning #6: NGUI tweeners were isolated per component — a fade never
  killed an in-flight move; all-channels stopCurrent scrambled the boot-time Show/Hide races,
  leaving the PanelBackgroundUI backer stranded at open position). Explicit `Cancel(t)` /
  `TweenUtil.Cancel(go)` remain all-channels. `CancelAll()` clears every item — replaces
  `LeanTween.cancelAll()`.
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

## Gate learnings (2026-07-12, Phase 1 flip)

1. Tweens never flip GameObject active-state (see current-state bullet above).
2. `AnimationEasing` pump is `DontDestroyOnLoad` — NGUI tweeners lived on their targets and
   survived scene loads; a scene-local pump silently dropped transition-time tweens (stuck
   dialog backdrop, tip overlay re-appearing after gameplay). Dead targets self-remove via
   `ITweenTarget.alive` (looping tweens would otherwise leak across level loads).
3. **Never fade a camera GameObject through the facade** — the NGUI-adapter child-widget
   fallback resolves it to the first `UIWidget` of the whole UI layer under that camera
   (pre-flip these were silent LeanTween no-ops). The `CameraExtensions` fades were removed;
   camera visibility is `enabled` + `Show()`. Residual risk of the same class: any
   `FadeToObject` on a bare container GO now alpha-drives its first child widget where it
   no-op'd pre-flip — watch for one-widget fade artifacts in Phase 3 panel work.
4. **FadeToObject's `-a-NN` child recursion is deleted** — it only ever executed on the
   LeanTween path where child fades no-op'd on NGUI widgets. Made real, it forced scene
   objects like `BackgroundDark-a-70` / `Background-a-40` to NN% alpha on every ancestor
   fade including fade-outs. `ColorToObject`'s recursion stays (it was live via NGUI).
5. **Adapter NGUI alpha applies ONLY to sprites on self** (UISlicedSprite/UISprite/
   UITiledSprite via GetComponent on the target GO; then CanvasGroup → Graphic → Renderer;
   NEVER UIPanel, never generic UIWidget, never children). Trace-verified production rule:
   pre-flip, facade fades were real only when the old sprite-routing sent them to NGUI —
   which required a sprite on the GO itself. Every other fade (containers, UIPanel hosts,
   labels) was a LeanTween no-op, and whole subsystems depend on that: the
   BaseGameUIPanelBackgrounds Show/Hide* fade choreography is effectively dead code whose
   execution dims/blanks the vivid backgrounds; the PanelBackgroundUI dark backer shows/
   hides only by position, and panel-main's AnimateIn isVisible early-return means no
   reliable off-switch exists if its alpha ever gets driven. Color keeps its child-widget
   search — that path (TweenColor via the always-forced NGUI ColorToObject) really was live.

6. **stopCurrent cancels the same channel only** (see Cancel-keys bullet above): NGUI
   tweeners were per-component isolated; all-channel cancel scrambled boot Show/Hide races.
7. **Facade Move/Rotate coerce coord to LOCAL**: NGUI TweenPosition/TweenRotation always
   animated local transforms and ignored the requested coord; call sites default to world
   and depend on the local behavior (UIGameNotification's off-screen parking, panel slides).
   Explicit-backend calls (backend.Move directly) still honor world.
8. **`FadeToObject(TweenMeta, float)`'s Show()/Hide() side effects were only ever dead for
   the sprite-forced-NGUI path — they were LIVE for every other GO** (learning #5 talks
   about the *value* landing; this is about the composed onStart/onComplete callbacks that
   drive `meta.go.Show()`/`Hide()`). Pre-flip: `FadeToObject(GameObject,...)` only forced
   `meta.lib = nguiUITweener` when the GO carried `UISlicedSprite`/`UISprite`/`UITiledSprite`
   on itself; every other GO fell through to the *default* `TweenLib.leanTween`, whose
   `LTDescr.setOnStart/setOnComplete` really did fire, so plain container GOs (e.g.
   `panel-header`'s `TitleContainer`/`BackerObject`, which hold no sprite themselves — the
   sprite is a grandchild) got real `Hide()` on fade-to-0 / `Show()` on fade-to->0. The 1.2
   flip collapsed the sprite check into an unconditional `meta.lib = TweenLib.internalEasing`
   and, per learning #1/#5, dispatched "raw meta" (no onStart/onComplete) for ALL fades —
   correct for the sprite-bearing case (NGUI TweenAlpha really never fired those callbacks)
   but wrong for the non-sprite default-lib case, which silently stopped calling `Hide()`.
   Symptom: `GameUIPanelHeader.showMain()`'s `HideTitleObject()`/`HideBackerObject()` (both
   fade non-sprite containers) never deactivated them again after boot's `AnimateIn()`
   opened them, leaving the "PLAY GAMEMODE" title label and its dark backer sprite
   permanently visible over the main menu. Fix (`Engine/Utility/TweenUtil.cs`,
   `FadeToObject(TweenMeta, float)`): compute `hadSpriteOnSelf` (the old sprite check) before
   forcing `meta.lib = internalEasing`, then only skip the composed onStart/onComplete
   dispatch (`backend.Fade(..., meta)`, raw) when `hadSpriteOnSelf`; otherwise dispatch with
   `metaDispatch.onStart/onComplete` wired to the Show()/Hide() closures, restoring the
   historically-live default-lib behavior. `ColorToObject` does NOT need the equivalent fix:
   its lib-forcing was already unconditional pre-flip (no sprite check — see learning #5's
   "always-forced NGUI ColorToObject"), so its Show()/Hide() callbacks were dead in every
   case both before and after; only the color *value* application differs there, and that
   already keeps its NGUI-only child-widget recursion. Verified via the stash A/B recipe:
   pre-flip, `TitleContainer`/`BackerObject` end boot with `activeSelf=false`; post-flip
   (pre-fix) they stayed `true`; post-fix they match pre-flip exactly.

## Out of scope for 1.1/1.2 (later chunks)

- Removing forced-NGUI overrides, `USE_EASING_*` gates, vendored lib deletion → 1.5/1.6.
- Named animation-preset tokens (`panel-in-left`, durations/eases as data) → 1.5, then
  formalized into the bitty token source at 2.1.
- UITweenerUtil shim → 1.3 (games repo).

## OPEN at session end (2026-07-12): settings-family content regression — 1.5 flip UNCOMMITTED

Gate run 7 (full walk): main family clean; settings family (6 panels) BROKEN — header +
coins render, all content containers (button grids, sliders, credits text) absent. Settings
PASSED in gate run 6. Deltas 6→7: (a) per-channel stopCurrent, (b) facade Move/Rotate
coord→local coercion, (c) learning #8 Show/Hide callback split for non-sprite fades.

Prime suspect: (b). Panel containers' ±4500 closed / 0 open design values were applied as
WORLD coords in run 6 (default coord=world landed containers at world origin ≈ screen
center, accidentally "working"); local coercion now sends them to LOCAL design positions —
verify what NGUI TweenPosition actually produced for these containers (it used localPosition;
but the *from* it captured on Begin may differ from our from-capture). Second suspect: (c)
replacement-vs-stacking timing killing the fade-in's Show() when an out-fade lands in the
same window (LeanTween.cancel(go) analysis says equivalent, but only fades were LeanTween —
re-verify with the stash A/B probe on containerCenter of panel-settings during ShowSettings).

Evidence: run captures /private/tmp/claude-501/.../scratchpad/postflip7/ (vs postflip6/ which
passed settings) — compare panel-settings.png across runs 6/7 first. Verified clean and green:
cold boot main menu, backer (-4500), no banner, HUD, 13/13 EditMode tests.

All 8 learnings above are trace-verified and must be preserved by whoever finishes 1.5.
