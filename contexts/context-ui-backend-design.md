---
name: context-ui-backend-design
description: IUIBackend/UIRef provider seam, UIPlatform dispatch, BindElements manifest, and bitty schema v1 — the agnostic UI platform contract (plan chunk 2.1)
metadata:
  type: design
  repo: game-lib-engine
  plan: plan-ui-migration-uitoolkit
  chunk: "2.1"
  status: designed
  created: 2026-07-13
---

# UI backend design — IUIBackend, UIRef, UIPlatform, bitty v1

Phase 2 chunk 2.1. This is the load-bearing design chunk: it fixes the contract that
2.2–2.11 implement and that every Phase 3 panel wave consumes.

The headline rule, mirroring [[context-tween-backend-design]]:

> Nothing above the provider layer may reference `UnityEngine.UIElements` or NGUI.
> Those names may appear only inside backend/adapter files.

The tween seam is the precedent. Where a decision could go either way, it goes the way
`ITweenBackend`/`ITweenTarget` already went — same namespaces, same lowercase interface
properties, same lazy-static registration, same "rules live in `//` comments sited at the
constraint" convention. A developer who has read the tween seam should find no surprises here.

## Ground truth that drives the design (verified 2026-07-13, not assumed)

The plan's Phase 2 estimates were built on pre-exploration guesses. Four of them were wrong,
and the corrections change the contract — record them before the design, because the design
only makes sense in their light.

- **UIUtil is not a 120-method surface to be mirrored.** It is an *overload-resolution shim*:
  every family is the same triple — (NGUI component overload) / (uGUI component overload) /
  (GameObject resolver overload). Call sites select a backend today by **the static type of the
  field they pass**, not by any runtime switch. The `GameObject` overload is the only genuinely
  polymorphic entry point, and it is the natural seam. `IUIBackend` therefore has ~8 op groups,
  not 120 methods.
- **~199 of the ~200 `IsButtonClicked` calls are the `(string, string)` overload** — a pure
  `==`/`.Contains()` compare of a broadcast button name against a constant. It touches no UI
  object at all. It is the **event-bus dispatch side** and must NOT go on `IUIBackend`. This is
  the single biggest mis-grouping in the plan's D0.
- **Live NGUI-typed app fields = 5, not ~37.** GameHUD's 21 and GameStore's 10 are inside
  `/* ... */` block comments and do not compile (the live `GameHUD` class has zero fields; there
  is no live `GameStore` class). The only live ones are `UIPanelResultsBase` (5 × `UILabel`).
  The "37" was miscounted from the 37 unassigned `{fileID: 0}` widget slots in the prefabs.
- **The `#if` field mirror covers only 15 of 59 `Base*` classes** (78 fields, mirrored 1:1
  NGUI↔uGUI). Chunk 2.11 is ~4× smaller than budgeted.

And one correction that makes Phase 4 *bigger*:

- **The define-drop blocker is unguarded NGUI, not the `#if` fields.** `UIAppPanelBaseListViews`
  carries bare `UIGrid`/`UIPanel` fields outside any `#if`; so do `AlertDialog`, `GameUI`,
  `GameUISceneRoot`, `GameRPG`, `UIPanelResultsBase`. Plus inline `NGUITools`/`UILabel`/`UISprite`
  in `GameUIPanelAchievements` and a `GetComponent<UIButton>()` raycast in `GameUIController`.
  Flipping `USE_UI_NGUI_2_7` off will not compile until these are converted, and **gameverses'
  Community panels inherit straight through `UIAppPanelBaseListViews`** — so that one class is a
  shared chokepoint for two repos. Needs its own chunk (4.0), ahead of the define drop.

### Hard constraint: core libs are shared — additive only

`game-lib-*` are consumed by other projects. Zero call sites *in this repo* is not evidence of
deadness. UIUtil's public surface is therefore **additive-only**: 2.2 adds the backend seam and
makes existing bodies *delegate inward* to it. No public member is deleted, renamed, or re-signed
— including the ones that are visibly dead here (`IsEventReady`, `FindLabel`, `UIButtonEnable`,
`GridReposition(Grid)`, `UIButtonMeta`). They stay, excluded from `IUIBackend`.

This is the opposite of Phase 1, where deleting vendored libs and `#if`-dead branches was correct.
The difference: those were *retiring a third-party dependency behind the facade*; this is *the
facade itself*, which is the product other projects consume.

## Layers

```
call sites (525 UIUtil + 199 IsButtonClicked + 58 panel classes)
──────────────────────────────────────────────────────────────────── agnostic API
  UIUtil (unchanged surface + additive UIRef overloads)   Engine/UI/UIUtil.cs
  UIEvents (name-keyed button bus — the (string,string) half)  Engine/UI/UIEvents.cs
  UIRef (opaque handle)                                   Engine/UI/UIRef.cs
  TweenUtil / TweenPresets                                Engine/Utility, Engine/Animation
──────────────────────────────────────────────────────────────────── provider (pluggable)
  IUIBackend                                              Engine/UI/Backends/IUIBackend.cs
  UIPlatform (registry + per-object dispatch)             Engine/UI/Backends/UIPlatform.cs
  NGUIBackend       ← may name NGUI + UnityEngine.UI      Engine/UI/Backends/NGUIBackend.cs
  UIToolkitBackend  ← may name UnityEngine.UIElements     Engine/UI/Backends/UIToolkitBackend.cs
──────────────────────────────────────────────────────────────────── data-driven definition
  bitty schema v1 + token source                          Engine/UI/Bitty/  (runtime → Phase 3)
```

## D1 — UIRef: the opaque handle

```csharp
namespace Engine.UI {

    // Opaque handle to a backend element. Deliberately NOT [Serializable] and NOT a
    // UnityEngine.Object: swapping a panel's #else-branch field to UIRef drops nothing
    // from the prefab (Unity simply ignores the field) and forces the value to come from
    // BindElements at runtime. That is the point — it makes name-binding the only path
    // and prevents a half-inspector/half-bound hybrid.
    public class UIRef {

        public object native { get; }      // GameObject | VisualElement — never leaked upward
        public string name { get; }
        public bool alive { get; }         // false once the underlying object is destroyed
        public GameObject gameObject { get; }  // convenience for the GameObject backends; null for a VisualElement

        public static readonly UIRef none;

        public static UIRef Of(GameObject go);
        public static UIRef Of(object native, string name);
    }
}
```

**As-built note (2.2):** the earlier sketch also carried an `isValid` alongside `alive`. It shipped
with `alive` only — two names for one concept is exactly the kind of thing that rots. Mirror the
code, not this sketch, if they ever disagree.

`alive` mirrors `ITweenTarget.alive` and exists for the same reason: a handle can outlive its
element, and every backend op must no-op rather than throw on a dead one. `UIRef.none` is never
null — same "Get never returns null" policy as `TweenPresets.Get`.

`native` is `object`, not a typed member, so the facade can hold and pass a `VisualElement`
without the engine assembly naming `UnityEngine.UIElements` above the provider layer. Backends
downcast it internally. This is exactly the `ITweenTarget.native` trick.

## D2 — IUIBackend: the provider contract

Grouped to match UIUtil's *live* families. Dead members are excluded (but not deleted — see the
additive-only constraint). Every op takes a `UIRef` and must no-op on `!isValid`.

```csharp
namespace Engine.UI {

    public interface IUIBackend {

        // Per-object coexistence dispatch. NGUIBackend returns true for GameObject;
        // UIToolkitBackend returns true for VisualElement. UIPlatform probes in
        // registration order. This is what lets both backends run simultaneously
        // with zero call-site churn.
        bool Handles(object native);

        // resolution
        UIRef Resolve(UIRef root, string name);                 // direct child by name
        UIRef ResolveDeep(UIRef root, string name);             // recursive — UpdateLabelObject
        List<UIRef> ResolveLike(UIRef root, string code);       // name-substring — SetTextValue/SetTextColor

        // label
        void SetLabelValue(UIRef r, string val);
        string GetLabelValue(UIRef r);
        void SetLabelColor(UIRef r, Color c);

        // input
        void SetInputValue(UIRef r, string val);
        string GetInputValue(UIRef r);

        // slider
        void SetSliderValue(UIRef r, float val);
        float GetSliderValue(UIRef r);

        // toggle / checkbox
        void SetToggleValue(UIRef r, bool val);
        bool GetToggleValue(UIRef r);

        // image / sprite
        void SetImageFillValue(UIRef r, float val);
        float GetImageFillValue(UIRef r);
        void SetSpriteColor(UIRef r, Color c);

        // button (the object half only — the name-compare half lives on UIEvents)
        bool IsButton(UIRef r);
        void SetButtonColor(UIRef r, Color c);
        void SetButtonHandlerClick(UIRef r, Action onClick);

        // visibility
        void Show(UIRef r);
        void Hide(UIRef r);
        bool IsVisible(UIRef r);

        // layout
        void GridReposition(UIRef r);

        // panel-view lifecycle
        UIRef LoadView(string viewKey);                         // "panel-settings" → instantiated view
        void Attach(UIRef view, UIRef parent);
        void Detach(UIRef view);
        void DestroyView(UIRef view);

        // pointer/event source — replaces UICamera.currentTouchID
        int currentPointerId { get; }
        bool IsPointerOver(Vector2 screenPos);
    }
}
```

Notes that are load-bearing:

- **`Show`/`Hide` are display-state ops, and tweens must never call them.** Gate learning #1 from
  Phase 1 says tweens don't own active-state — with the documented exception of `FadeToObject`'s
  Show/Hide side effects on non-sprite containers (learning #8). `UIToolkitBackend.Show/Hide` map
  to `display: flex | none`, and the fade side-effect policy stays where it already is (in
  `TweenUtil.FadeToObject`), NOT re-implemented in the backend. Moving it would silently re-break
  the header title/backer bug that learning #8 documents.
- **`ResolveLike` exists only because `SetTextValue`/`SetTextColor` do substring matching**
  (`name.Contains(code)` over children). 3 call sites total. It is ugly and it is preserved
  verbatim — this is a compatibility seam, not a redesign.
- **No `Awake`/`Update` on the interface.** Backends are plain lazy C# singletons like
  `EasingTweenBackend`, not MonoBehaviours. Only `UIToolkitClickBridge` (which needs
  `UIDocument`) is a MonoBehaviour, and it lives inside the UIToolkitBackend file group.

## D3 — UIPlatform: registration and per-object dispatch

The tween seam has exactly one backend, so it got a single lazy static property plus `SetBackend`.
UI needs **two backends live at once** for the whole of Phases 2–3 (NGUI screens and Toolkit
screens coexisting), so this is the one place the UI seam must diverge from the precedent: a list
plus an ordered `Handles()` probe.

```csharp
namespace Engine.UI {

    public class UIPlatform {

        private static List<IUIBackend> _backends = null;

        public static void Register(IUIBackend backend);        // idempotent; first-registered wins ties
        public static IUIBackend For(object native);            // null if unclaimed
        public static IUIBackend For(UIRef r);
        public static IUIBackend viewBackend { get; set; }      // who owns LoadView for new panels
    }
}
```

`viewBackend` is separate from `For()` because `LoadView("panel-settings")` has no native object
to dispatch on yet — it is the "which backend builds a *new* screen" switch, and it is what a
per-panel migration actually flips. During Phase 3 a panel migrates by having its view key resolve
through `UIToolkitBackend` instead of `NGUIBackend`; nothing else about the panel changes.

Registration happens once at startup (engine bootstrap), gated only by which backend *files*
compile — `#if USE_UI_NGUI_2_7` around the `Register(NGUIBackend...)` line, `#if USE_UI_TOOLKIT`
around the Toolkit one. Per the tween precedent: **defines gate which files/registrations compile,
never call-site branches.**

### How UIUtil delegates (additive, behavior-preserving)

The existing `GameObject` resolver overloads become dispatch; the typed overloads
(`SetLabelValue(UILabel, …)`) are left exactly as they are, since other projects call them.

```csharp
public static void SetLabelValue(GameObject obj, string val) {

    if (obj == null) {
        return;
    }

    IUIBackend backend = UIPlatform.For(obj);

    if (backend != null) {
        backend.SetLabelValue(UIRef.Of(obj), val);
        return;
    }

    // ...existing NGUI/uGUI probe body stays as the fallback...
}
```

Plus one additive `UIRef` overload per op (`SetLabelValue(UIRef, string)`), which is what migrated
panel code calls. `NGUIBackend` is the extraction of the existing probe body, so the dispatch path
and the fallback path are the same code — that is what makes 2.2 provably behavior-preserving, and
it is testable: the same GameObject through both paths must produce identical results.

## D4 — The event bus is not the backend

`IsButtonClicked(string, string)` / `IsButtonClickedLike(string, string)` — 199 call sites — move
*conceptually* to `Engine/UI/UIEvents.cs`. `UIUtil`'s existing members stay and delegate (additive
rule), so no call site changes.

```csharp
namespace Engine.UI {

    public class UIEvents {

        public const string EVENT_BUTTON_CLICK = "ui-button-click";
        public const string EVENT_BUTTON_CLICK_DATA = "ui-button-click-data";

        public static bool IsButtonClicked(string button, string buttonClickedName);
        public static bool IsButtonClickedLike(string button, string buttonClickedName);
        public static void BroadcastClick(string elementName);
    }
}
```

The constant lives in engine because engine cannot depend on `game-lib-games`, where
`ButtonEvents` defines it today; `ButtonEvents` aliases the engine constant.

**Why this matters more than it looks:** the click path is *already* backend-agnostic today —
`ButtonEvents` broadcasts `gameObject.name`, `BaseUIController` re-broadcasts by name, and panels
switch on the string in `OnButtonClickEventHandler(string)`. UI Toolkit needs no new event model
at all. `UIToolkitClickBridge` registers one bubbling `ClickEvent` per document root and calls
`UIEvents.BroadcastClick(element.name)`. Every existing handler then works untouched. This is the
cheapest, highest-leverage fact in the whole migration, and it is why the pilot is achievable.

## D5 — BindElements: manifest primary, convention fallback

The plan had this inverted (convention primary, manifest as override). Ground truth flips it:
NGUI prefab GameObject names are PascalCase/arbitrary (`ButtonFacebook`, `Container/LabelName`),
field names are camelCase (`buttonProfileFacebook`), and ~27% of widget slots are already
`{fileID: 0}` — unwired and tolerated, because every consumer null-checks. So convention alone
would silently fail to bind a large minority of fields.

**Manifest is authoritative; convention is a convenience that removes manifest lines.**

```csharp
// UIAppPanel
protected virtual void BindElements(UIRef root);   // called from Awake(), after GetClassName
```

Resolution order per public `UIRef` field:
1. bind-manifest alias for `(className, fieldName)` → element name
2. exact field-name match
3. kebab-case of the field name (`buttonProfileFacebook` → `button-profile-facebook`)
4. unresolved → `UIRef.none` + one warning naming class, field, and view key

Manifest format (per panel, emitted by the 2.4/2.5 converter from the prefab's serialized
MonoBehaviour wirings — the `fileID` → GameObject-name mapping is exactly the data we need, and it
is *already* the ground truth of what binds to what):

```json
{
  "view": "panel-settings-profiles",
  "class": "GameUIPanelSettingsProfile",
  "binds": {
    "buttonProfileFacebook": "button-profile-facebook",
    "inputProfileName": "input-profile-name"
  },
  "structure": {
    "panelContainer": "panel-container",
    "panelCenterObject": "panel-center"
  }
}
```

### The structural fields are the risky half

`panelContainer` + the nine `panel{Center,Left,LeftTop,LeftBottom,Right,RightTop,RightBottom,Top,Bottom}Object`
fields are **77 live serialized references** and are *not* `#if`-guarded — they are plain
`GameObject` in both branches, and `AnimateIn`/`AnimateOut`/`ShowPanel`/`HidePanel` are built
entirely on them. Under UI Toolkit there is no GameObject to put there.

Therefore `UIPanelBase` gains a parallel `UIRef` set (`panelContainerRef`, `panelCenterObjectRef`,
…), bound from the manifest's `structure` block, and its show/hide calls `TweenUtil` overloads
taking `UIRef`. The `GameObject` fields stay for the NGUI path and are untouched. Chunk 2.9 owns
this; it is the single most delicate edit in Phase 2 because it is the class every panel in two
repos inherits from.

`TweenUtil` already accepts this shape for free: `ResolveTarget(object native)` tries
`VisualElementTweenTarget.TryCreate(native)` first, then `GameObject`. So `TweenUtil.MoveToObject(UIRef)`
is a thin overload passing `ref.native` — no tween-side redesign.

### Two pre-existing bugs found in the panel system (fix in 2.9, not silently)

- `UIPanelBase.OnDisable()` calls `Messenger.AddListener` (not `RemoveListener`) for the two
  `ButtonEvents` listeners — listeners accumulate across enable cycles.
- `OnEnable()` does `panelTypes.Add(typeDefault)` every enable — the list grows unbounded.

Neither is migration-caused. Both are in the class 2.9 rewrites, so fix them there, deliberately,
and note them in the commit — do not let them ride in as invisible drive-bys.

## D6 — bitty schema v1 (schema + tokens now; runtime in Phase 3)

**User decision 2026-07-13:** Phase 2 defines the bitty schema and token source; the bitty
*runtime* (parser + pattern catalog) lands in Phase 3, where waves 3A (option-row) and 3D (list)
are the first real consumers. The pilot ships on UXML/USS through `IUIBackend`.

Rationale: the "prototype" is not a promotion candidate. It is ~190 lines of dict→VisualElement
builder plus a demo host — no data files, no schema doc, no events, no theming, no patterns, no
`class`/USS support at all (inline styles only, 12 hardcoded keys, pixels only, no `%`/`auto`),
and zero consumers. Its element factory and style applier are 100% backend code that belongs
*below* `IUIBackend`. The salvageable IP is the key vocabulary and the recursion shape. Designing
the pattern catalog before a single real panel has exercised it is how you get a catalog that
fits nothing.

Schema v1 — the vocabulary, extending the prototype's with what it structurally lacked
(`class`, `template`, `pattern`, `bind`, `events`):

```json
{
  "view": "panel-settings",
  "pattern": "dialog",
  "children": [
    { "type": "container", "name": "panel-container", "class": ["panel", "panel-center"],
      "children": [
        { "type": "label", "name": "label-section", "text": "@loc:settings.title", "class": ["title"] },
        { "type": "row", "pattern": "option-row", "name": "row-music",
          "bind": { "label": "@loc:settings.music", "value": "settings.musicEnabled" },
          "events": { "change": "settings-music-toggle" } },
        { "type": "button", "name": "button-back", "class": ["btn"],
          "events": { "click": "button-back" } }
      ]}
  ]
}
```

- `name` is the contract with everything else: it is the element name, the `BindElements` key,
  AND the string the click bridge broadcasts. One identifier, three jobs — that is the whole
  reason the platform holds together.
- `class` (absent from the prototype) is mandatory in v1. Styling is classes + tokens; inline
  `style` is permitted only as an escape hatch, never for anything shared or themed.
- `template` lets a bitty view defer to native presentation: `{"template": "panel-settings.uxml"}`
  and `UIToolkitBackend` resolves it to UXML. This is what keeps app-IP screens (HUD, results,
  storefront) pixel-native while their *data* stays portable.
- `pattern` names a catalog entry (dialog / list / option-row / chrome / hud-widget / overlay).
  Phase 2 defines the six names and their required keys; Phase 3 implements them.
- `events` maps a native event to an **element name string**, which goes straight onto the
  existing name-keyed bus. No new event plumbing, ever.
- `@loc:` prefix routes through `Locos.GetString` — this is how `UILocalizedLabel` survives.

### Token source

One file, `tokens.json`, is the authority for palette, fonts, sizes, and **durations/eases**.
Two materializers consume it:

- `UIToolkitBackend` → generated `vars.uss` (never hand-edited)
- `TweenPresets` → seeded from the same `motion` block

`TweenPresets` already exists, has zero call sites, and carries the comment *"the bitty token
source becomes the authoritative definition of these when the data-driven UI platform lands (plan
chunk 2.1)"*. This is that landing. Wiring it is part of 2.1's output, not a later cleanup.

```json
{
  "color": { "panel-bg": "#2D2D2D", "text-primary": "#E0E0E0" },
  "size":  { "gap-m": 16, "radius-m": 8 },
  "font":  { "title": "FontKomika", "body": "FontMain" },
  "motion": {
    "panel-show":  { "time": 0.45, "delay": 0.5, "ease": "quadEaseInOut" },
    "panel-hide":  { "time": 0.45, "delay": 0.0, "ease": "quadEaseInOut" },
    "dialog-show": { "time": 0.30, "delay": 0.0, "ease": "quadEaseInOut" }
  }
}
```

The `motion` values are seeded from the legacy `TweenUtil` constants so Phase 3 starts at
today's timings, then tunes in data instead of in code.

## D7 — Coexistence and click-through

Per plan D4, unchanged by this design: UI Toolkit overlay panels render above the URP camera
stack; a migrated fullscreen panel simply covers NGUI behind it. The guard is one grep-marked
(`// UITK-MIGRATION`) early-out in NGUI's `UICamera.Raycast` calling
`UIPlatform.For(toolkit).IsPointerOver(screenPos)`; the click bridge ignores picks on the
transparent root so gameplay taps fall through. `UICamera.currentTouchID` reads (4 sites:
`InputEvents:38`, `SliderEvents:69`, `CheckboxEvents:39`, `ListEvents:32`) re-source to
`IUIBackend.currentPointerId`.

## Phase 2 gate (unchanged in substance, sharpened)

1. Pilot (`panel-settings-credits`) renders from UXML through `UIToolkitBackend`.
2. Its clicks reach the **existing, unmodified** `OnButtonClickEventHandler(string)` via the
   name-keyed bus.
3. Localization and preset transitions work through `TweenPresets` fed by `tokens.json`.
4. All NGUI screens still work; no click-through in either direction.
5. **Platform review [F]:** `grep -r "UnityEngine.UIElements"` outside `Engine/UI/Backends/` and
   `Engine/Animation/Tween/VisualElementTweenTarget.cs` returns nothing; same for NGUI types
   outside `NGUIBackend`. This is the check that the seam is real.
6. `NGUIBackend` dispatch is provably behavior-preserving: same GameObject through the legacy body
   and through the backend produces identical results (EditMode test, mirroring
   `TweenBackendTests`).

## As-built findings from 2.2 (implementation)

- **The Slider-before-Toggle probe order in `SetToggleValue` is unreachable.** It looks like a bug
  worth fixing. It is not reachable: `Slider` and `Toggle` both derive from `Selectable`, and Unity
  refuses two Selectables on one GameObject — `AddComponent<Toggle>()` returns **null** when a
  Slider is already present (verified in-editor). No object can hit the ambiguous branch, so
  preserving the ordering is free. Pinned by
  `SliderAndToggle_CannotCoexist_SoProbeOrderIsUnreachable` so it is not re-litigated.
- **`UIPlatform.autoRegisterDefaults`** is a test-only seam. Once the backend auto-registers, the
  legacy body in UIUtil becomes unreachable and "behavior-preserving" becomes an untestable claim.
  Turning defaults off makes `For()` return null, UIUtil falls back to its original body, and the
  A/B comparison becomes possible. Production never touches it.
- **`GridReposition` is a deliberate no-op on the Toolkit backend.** `UIGrid.Reposition()` exists
  only because NGUI had no layout engine; UI Toolkit reflows itself via flex-wrap. The 2 call sites
  become free once their panels are UXML.
- **Fill has no UI Toolkit equivalent.** `SetImageFillValue` maps to horizontal `width: %` for v1,
  which covers every current consumer (it has zero direct call sites; fills arrive via
  `SetSliderValue`'s fallback). Radial/vertical gauges are a real gap and land with the HUD in 3H.

## Open risks

- **`UIAppPanelBaseListViews` is a two-repo chokepoint** (unguarded `UIGrid`/`UIPanel`; gameverses
  inherits it). It is not in any current chunk. Added as 4.0, but if Phase 3D (lists) needs it
  earlier, it moves up.
- **`Awake()` overrides that skip `base.Awake()`** (e.g. `GameHUD`) would skip `BindElements`.
  Audit all `Awake` overrides in 2.9; consider binding from `Init()` instead, which is more
  consistently chained.
- **`UIPanelBase.Start()` → `AnimateOut()` on every panel** and `HideAllPanels()` calling
  `AnimateOut` twice per navigation. Harmless under tweens; verify it stays harmless when
  `Hide()` becomes a real `display: none` rather than an off-screen slide.

---

## As-built through the pilot (2.6–2.10, 2026-07-14)

The pilot (`panel-settings-credits`) shipped end-to-end. Several 2.1 design assumptions changed
under contact with the running editor; **trust this section over the original design above where
they differ.**

### PanelRenderer, not UIDocument (the biggest change)
`UIDocument` is **deprecated** in Unity 6000.5.2f1 (can't be added to new GameObjects). The backend
runs on `UnityEngine.UIElements.PanelRenderer` instead (confirmed present). This also killed the
"one persistent host with layer containers" model:
- **Per-panel PanelRenderer.** Each migrated panel's view is its own GameObject (`uitk-<viewKey>`)
  carrying a PanelRenderer + VisualTreeAsset. Destroying it frees the UXML — that is the unload the
  memory story needs. Layer containers / the layer resolver are GONE.
- **One shared `PanelSettings`** (registered by the slim `UIToolkitHost` MonoBehaviour) → one runtime
  panel, many renderer subtrees, no render target per panel. Ref resolution must be **1138×640,
  match=height** to reproduce NGUI's 640-tall UIRoot; the default 1200×800/match-width rescaled every
  offset.
- **Loading is DEFERRED.** Assigning `visualTreeAsset` does NOT build synchronously; the root arrives
  in `RegisterUIReloadCallback`. So `IUIBackend.LoadView` is now `LoadView(viewKey, Action<UIRef> onReady)`
  — NGUIBackend calls back synchronously, Toolkit from the reload callback. Bind/hide/NGUI-suppress all
  moved into the continuation. On first show a panel briefly runs NGUI until the view arrives a frame
  later. Show/hide/bind operate on the view's OWN named subtree (`root.Q(viewKey)`), never the shared
  panel root.

### Per-panel migration cost = one property
A panel migrates by overriding `toolkitViewKey` (the same string key the NGUI path uses). The view is
lazy-loaded on first `AnimateIn` — NOT Init/Start, because panels are instantiated **inactive**
(pooled), so Start never runs until first show.

### Destroy-on-hide — panels are POOLED, not destroyed
This project pools panels (`_ObjectPoolKeyedManager`): navigate-away DEACTIVATES the panel (fires
`OnDisable`), it does NOT destroy it (OnDestroy never fires on navigation). So the view is freed in
`UIPanelBase.OnDisable` (verified: open→1 renderer, back→0, reopen→1). **Systemic gotcha:** many
`Base*` panels override `OnEnable`/`OnDisable` WITHOUT calling `base` (e.g.
`BaseGameUIPanelSettingsCredits`), which silently skips the free — every migrated panel must chain to
base. A companion-hook that doesn't depend on override discipline is a Phase 3 item.
Also fixed here: the pre-existing `UIPanelBase.OnDisable` bug that called `AddListener` (not Remove)
for the two ButtonEvents — a real per-hide listener leak, unrelated to UI Toolkit.

### Kill switch
`UIPlatform.toolkitViewsEnabled = false` reverts every panel to NGUI on next show. NGUI is the
shipping path for all of Phase 3 by design; this makes that a one-flag revert.

### Reusable ScrollView pattern (credits, worlds, levels, statistics, achievements)
A clipped NGUI `UIPanel` → `ui:ScrollView`. All of this lives in the converter + backend so no panel
repeats it:
- **Drop the NGUI `UIScrollBar` node** (ScrollView draws its own — two bars otherwise).
- **Top-anchor** scroll children (a scroll region is top-anchored, not centre-anchored like panel
  widgets) and **wrap them in one content element of the real height** (a min-height USS rule on the
  ScrollView's internal content-container is fragile — the descendant selector resolved to 0).
- **`ScrollDrag` manipulator** on `.ngui-scrollview`: pointer drag scrolls for BOTH mouse and touch
  (UI Toolkit does neither with a mouse), with **momentum** (velocity over real elapsed time,
  exponential decay), **auto-hide** (bar fades ~0.8s after scrolling), and the bar **inset 8px** so
  it stays inside the backer. One `.schedule.Every(16)` ticker drives decay + idle-hide.

### Coexistence click-through guard (Phase 2 gate)
- `UIPlatform.IsPointerOverUI(screenPos)` — backend-agnostic OR over each backend's `IsPointerOver`
  (NGUI false; Toolkit does `panel.Pick`).
- `ConfigurePicking` sets `.ngui-container`/`.ngui-root` to `PickingMode.Ignore` on load, so only
  real widgets count as over-UI — otherwise the full-bleed wrappers would block the always-on NGUI
  header. **Only fires when a view is loaded**; NGUI input is untouched normally.
- One grep-marked `// UITK-MIGRATION`, `USE_UI_TOOLKIT`-gated early-out in `UICamera.Raycast`.
  Verified selective: True over panel content, False over header/back-button/gameplay.

### Perf / memory (still open — measure on device)
Editor A/B **exonerated the theme/atlases**: NGUI already keeps the 5 UI atlases resident, so the
theme's `sprites.uss` adds no net texture memory. The felt memory + scene-transition regression is
NOT the atlases. Editor managed-heap numbers are noise-dominated; the trustworthy verdict needs an
**Android build** (toolkit on vs off via the kill switch). destroy-on-hide is wired, so the
architecture can free views — magnitude TBD on device. See memory [[ui-toolkit-perf-regression]].

### Font
`FontMain` = `DimboRegular` (93 refs, the dominant live face), TTF present → real SDF FontAsset at
`Assets/UI Toolkit/Fonts/Dimbo-SDF.asset`, applied at `:root`. The default UI Toolkit face is wider
than Dimbo, which is what made ported absolute label rows overlap.

### Converter as-built (see also context-ngui-uxml-converter)
- **USS has NO `calc()`** — Unity logs "Unknown function 'calc'", drops the declaration, everything
  collapses to the top-left (looks exactly like a failed stylesheet; it isn't). Position via
  `left/top: 50%` + `translate` / `margin` instead.
- Pivot → self-relative `translate %` (NOT folded into the pixel offset: a label's localScale is its
  FONT SIZE, not text width, so folding it misplaced every label).
- Colour from `mColor`, alignment from pivot, sprite via `.spr-<name>` class; `-unity-slice-*` takes a
  bare integer, not px.
- `RectTransform` roots + `Canvas`/`CanvasRenderer` are UI, not "non-UI" (dropping them ate
  panel-header/footer). Duplicate BUTTON names are fine (name-keyed clicks); duplicate BIND TARGETS
  are fatal (`panel-worlds` has one — fix in 3D).
- Designed list content (credits) is reflowed to a **flex column by hand** in the cleanup pass; the
  converter's absolute output is scaffolding.

## As-built: 2.11 — panel field mirror `#else` → UIRef (2026-07-15)

The panel field mirror is `#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3` (NGUI types) `#else` (was UGUI
`Button`/`Text`/`Slider`/`InputField`/`GameObject`) `#endif`. 2.11 flips the **`#else` branch to
`Engine.UI.UIRef`** so the same call sites work backend-blind and the value must come from
`BindElements` at runtime (UIRef is not `[Serializable]` — Unity ignores the field, which is the point).

- **Scope was 16 classes / 77 fields**, all in `game-lib-games-ui/Game/UI/Panels/` (NOT the ~59-class
  whole-universe of the mirror; the gameverses Community mirror classes + `UIPanel*`/notification/dialog
  families are later chunks). Three `Panels/` files use the NGUI `#if` only around **method bodies**, not
  fields (`BaseGameUIPanelAchievements/Products/Statistics`) — not in 2.11.
- **Fully-qualified `Engine.UI.UIRef`** in the field decls rather than adding `using Engine.UI;` to 15
  files — one edit per file, no NGUI-branch ambiguity, self-documenting. (UIUtil itself is **global
  namespace** but imports Engine.UI; panels call `UIUtil.X` with no using, so only the UIRef type name
  needed qualifying.)
- **Core libs are additive-only** → the seam methods the fields flow into got **added UIRef overloads**,
  never changed signatures. Only 5 UIUtil methods actually receive a 2.11 field; 2 already had UIRef
  overloads (`SetLabelValue`, `SetInputValue`), so **3 were added**: `IsButtonClicked(UIRef,string)`
  (name-compare `== r.name`, mirroring the Button/GameObject overloads — clicks dispatch by name),
  `ShowLabel(UIRef)` / `HideLabel(UIRef)` (delegate to `backend.Show/Hide`, mirroring
  `ShowObject/HideObject(UIRef)`).
- **One hard direct-access break**: `BaseGameHUD.cs:2081` `labelTime.text = time;` sat in **unguarded**
  common code → replaced with `UIUtil.SetLabelValue(labelTime, time)` (backend-blind; has UILabel/Text/
  UIRef overloads so it compiles in BOTH define branches). Two `field.name` reads (Header:183,
  SettingsProfile:119) compile unchanged via `UIRef.name`.
- **Declaration-only fields** (no consumer anywhere): HUD `sliderHealth`/`sliderEnergy`, Loader
  `labelLoading`/`sliderProgress` — converted for consistency, zero behavioural effect.
- **Verified by whole-project Unity recompile: no CS errors** (concrete `GameUIPanel*` subclasses
  included — the compiler confirmed no broken member access the static sweep might have missed). The
  scope map (which methods, which direct-access sites) is the reusable recipe for the later mirror chunks;
  those additionally exercise `SetSliderValue`/`SetToggleValue`/`IsToggleOn`/`IsCheckboxChecked` — check
  for missing UIRef overloads (`IsToggleOn(UIRef)`/`IsCheckboxChecked(UIRef)` were absent as of 2.11).
- Systemic sibling finding: many `Base*` panels skip `base.OnDisable()` → latent `FreeToolkitView` leak;
  a per-panel Phase-3 prerequisite (see plan + memory `ondisable-chain-breaks-at-base-layer`).

## As-built: Wave 3A — bitty runtime + Settings option-row/menu (2026-07-15)

Phase 3 wave 3A. This is where the **bitty runtime landed** (deferred from 2.3) and the **option-row
pattern** was designed against real panels, per the plan's abstraction-overreach guard. Trust this
section for how bitty actually runs.

### The bitty runtime (4 files)
- `Engine/UI/Bitty/BittyNode.cs` — agnostic parsed element; fields mirror `BittySchema.Keys` 1:1.
- `Engine/UI/Bitty/BittyParser.cs` — JSON→tree on **MiniJSON** (`Engine/Utility`), NOT `FromJson<T>`:
  the schema is polymorphic (`class` string|string[], `value` bool|string|float), so MiniJSON's
  Dictionary/List/primitive output is normalized once here. Root object names itself with `view`;
  inner nodes with `name`. Bad JSON → null + log (view fails to load → panel stays NGUI), never half-build.
- `Engine/UI/Bitty/BittyPatterns.cs` — pattern expansion **at parse time** (builder only sees concrete
  elements). Only `option-row` is structural in v1; the other five names attach their common.uss class
  and recurse (structure lands with their own waves).
- `Engine/UI/Backends/BittyToolkitBuilder.cs` — BittyNode→VisualElement. **Backend layer** (names
  UIElements; lives in `Backends/` for the leak grep). Styling is classes only — inline `style` strings
  are NOT applied at runtime (parsing a USS block to `VisualElement.style` is the prototype's grave).
  `@loc:` resolves through `Locos` at build time.

### How a bitty view loads (`UIToolkitBackend.LoadView`)
A view key resolves as **UXML `VisualTreeAsset` OR bitty JSON `TextAsset`** at `ui/views/<key>` —
`Resources.Load` is type-disambiguated so a `.uxml` and a `.json` at the same path never collide; UXML
wins if both exist. Bitty path: parse → `BittyToolkitBuilder.Build` → **attach the built tree straight to
the callback `root`** (its root node is uniquely named `viewKey`, so it stays isolated even when the
callback hands back the *shared* panel root). A shared empty stub `Resources/ui/bitty-host.uxml` exists
only to spin the `PanelRenderer` up and fire the reload callback — nothing is read back from it.

### The VALUE bridge (the 3A analogue of the click bridge)
Migrated sliders/toggles/inputs must reach the existing name-keyed handlers with no per-widget
MonoBehaviour (NGUI needed `SliderEvents`/`CheckboxEvents` components on each). So the backend registers
**bubbling `ChangeEvent<float>/<bool>/<string>` on each view root** and broadcasts `(element.name, value)`,
exactly mirroring the click bridge. Guard: skip `evt.target.name` starting with `"unity-"` (a ScrollView's
internal scroller is a `Slider` too and would spam the bus). Broadcast constants added to `UIEvents`
(`EVENT_SLIDER_CHANGE`/`EVENT_CHECKBOX_CHANGE`/`EVENT_INPUT_CHANGE` + `BroadcastSliderChange` etc.), with
values **byte-identical to game-lib-games' `SliderEvents`/`CheckboxEvents`/`InputEvents.EVENT_ITEM_CHANGE`**
— duplicated in engine exactly as the button constants already are (games not touched; engine can't
depend on it). A migrated toggle/slider therefore hits `OnSliderChange`/`OnCheckboxChange` untouched.

### The option-row pattern (two variants, driven by the two real panels)
- **slider control → `.option-row--stacked`**: external `.option-row__label` ABOVE a full-width control
  (audio: "MUSIC VOLUME" over the bar).
- **toggle control → `.option-row--inline`**: the toggle carries `label` as its OWN native text (controls:
  box + "Vibrate on HITS"), because UI Toolkit's `Toggle` renders its label natively — a separate label
  element would double it. The control child keeps its own `name` (broadcast/bind key); only the wrapper
  takes the row's name.

### Scope correction — "Settings family (5)" is heterogeneous (not 5 option-rows)
- **option-row ×2**: `panel-settings-audio` (2 sliders), `panel-settings-controls` (3 toggles) → **bitty JSON**.
- **button-menu ×2**: `panel-settings` root (5 tiles), `panel-settings-profiles` → hand **UXML**.
- **list ×1**: `panel-settings-help` (scrolling help rows) → belongs to **3D** (list pattern).
- **DONE this wave:** audio, controls, root. **DEFERRED:** profiles — its declared UIRef fields
  (`buttonProfileFacebook/Twitter/GameNetwork` + `inputProfileName`) do **not** match its baseline
  (Apply Code / Sync Profile buttons); a real data discrepancy needing in-editor reconciliation before
  authoring. Help → 3D.

### Interactive-panel migration recipe (extends the credits "just name the view key")
Credits was static. An interactive panel also needs: (1) `base.OnDisable()` at the **Base\*** layer
(FreeToolkitView chain); (2) `toolkitViewKey` on the concrete class; (3) finish the **2.11 UIRef swap for
the panel's widget fields** (audio had none/commented → added `sliderAudio*Volume`; controls was raw
`UICheckbox` → `#if`/`#else` UIRef, and `ChangeCheckedState` branch-guarded to `UIUtil.SetToggleValue(UIRef)`);
(4) a **bind manifest** mapping `field → element`, where the element name == the **prefab GameObject name**
(the handlers compare the broadcast name to `field.name`). Audio volume persistence was **restored** (it was
dead/commented on both branches; null-guarded so the unwired NGUI branch stays a no-op).

### USS / visual as-built (from the first in-editor screenshots)
- **The drawlabs-common cartoon 9-slice = `game-drawlabs-assets-common-1/textures/ui/image/ui_button_up.png`**
  (256×64 tintable rounded button, soft top gradient) + `ui_bar_button.png` (48² round knob). Import
  border is zero → author `-unity-slice-*` in USS. **Tint** via `-unity-background-image-tint-color`
  (matches the NGUI buttons, which tinted `ui-backer-fade` per color). Used for menu tiles, slider track,
  slider knob, toggle box. (`.spr-ui-backer-fade` in `sprites.uss` has NO slice; `.spr-ui-backer-sm` does.)
- **Toggle text label is `.unity-toggle__text`** (the `.text` label), NOT `.unity-toggle__label` (the empty
  `BaseField` label) — targeting the latter is why labels rendered tiny + default-dark.
- **Composite clickable** (a named `VisualElement`/`Button` with children): decorative children need
  `picking-mode="Ignore"`, else the deepest hit child (empty `name`) is `ClickEvent.target` and nothing
  broadcasts. Named tile must be the pick target.
- Content must clear the always-on header band (**~88px** in the 640 ref) — `padding-top` on the centered
  container. The header is NGUI and persistent; the toolkit view sits under it.
- **USS units (Unity 6.5):** NO `em`/`rem`; `font-size` is **px only** (no `%`); no `vw/vh/vmin/vmax`; no
  `calc()`. `%` DOES work for width/height/min-max/flex-basis, margin/padding, left-top-right-bottom,
  `translate` (self-relative — the pivot trick), border-radius, background-size, transform-origin. Also
  `s/ms` (transitions) and `deg/grad/rad/turn` (rotate). **Responsive scaling is the PanelSettings, not
  units:** `ScaleWithScreenSize` @ 1138×640 `match:height` → every px (incl. font-size) scales *uniformly*
  by `screenHeight/640` (no distortion). Horizontal fit is **aspect-dependent**, so the cross axis is done
  with flex + centering (`.settings-center`/`.settings-content`), never absolute px — absolutely-positioned
  wide content (raw converter output) is what overflows at extreme aspect ratios.

### Open / carry into tuning + later waves
- **Backer entrance:** the dark backer is the shared **`PanelBacker`** background (`backgroundDisplayState
  = PanelBacker`), animated with the panel's `panelCenterObject` sliding from the BOTTOM; the toolkit view
  itself only `display`-toggles (no slide). To make it "ease down from top", either drive the view via a
  from-top `TweenPreset` on show, or retarget the shared backer — needs hands-on to pick.
- **Header should clip scrolled content** (top disappears under the header) — a credits/help scroll-clip
  concern; the toolkit view needs clipping to below the header when it scrolls. 3A panels don't scroll, so
  `padding-top` suffices there.
- **Toggle checked-star** (yellow `icons-star-filled-64` overlay when on) needs a custom toggle build —
  the native checkmark mechanism only swaps one background-image, which the cartoon box already occupies.
  Currently: grey box (off) → green box (on), no star.
- **Slider fill:** UI Toolkit `Slider` has no progress fill; the whole bar is green. A filled-to-the-knob
  look needs a custom fill element.
- **Initial-value sync:** sliders/toggles don't yet reflect the saved profile on open (user-change
  persistence works). Programmatic set must use `SetValueWithoutNotify` or the value bridge broadcasts the
  set and clobbers state.
- **Logo artifacts** (Action Bots / Drawlabs Game Studio) are on the still-**NGUI Help panel** (deferred to
  3D) — an atlas/NGUI-render issue, not the toolkit path.
- **Pre-existing bug** to fix with the profiles migration: `BaseGameUIPanelSettingsProfile.cs:95` calls
  `AddListener` (not `RemoveListener`) in OnDisable — the input listener accumulates per hide.
- Whole-project Unity recompile CLEAN (RunCommand probe `isCompilationSuccessful`); console = 15
  pre-existing errors (14 URP shader + 1 stale iOS pod), 0 new.

### 3A refinements — from the in-editor iteration (2026-07-15)
Several first-cut choices changed under real screenshots; trust these over the initial 3A notes above.

- **Cartoon backer = `ui-backer-fade` (atlas), NOT `ui_button_up`.** The good NGUI tiles tint
  `ui-backer-fade` (the solid white→grey gradient tile with the *slanted bottom corners* = the cartoon
  slant). `ui_button_up` has a low-alpha center + opaque rim, so tinting it rendered as a translucent fill
  + colored outline. Applied as `background-size: 100% 100%` (stretch — the sprite's aspect ≈ a tile and
  the slant lives in the corners) with a per-element `-unity-background-image-tint-color`. Interaction
  feedback = scale-up + a DARKER per-color tint on `:hover`/`:active` (not a native focus rectangle).
- **Slider and toggle are CODE-BUILT COMPOSITES** (`BittyToolkitBuilder.BuildSlider`/`BuildToggle`), not
  USS-skinned natives — because UI Toolkit's `Slider` has no fill element and its `Toggle` swaps the
  checkmark background per state (which blanked the cartoon box). Slider = grey `.slider-track` + green
  `.slider-fill` (width = value%, updated on `ChangeEvent`) behind the native drag-container (tracker made
  transparent) + a cartoon slant knob. Toggle = native checkmark hidden, own `.toggle-box` (cartoon) +
  `.toggle-star` (`icons-star-filled-64`) overlay, with tint/star/hover-lighten all driven in code so no
  reliance on the exact `:checked` selector form. Both keep the node's `name` for bind + the value bridge.
- **Content slides from the top** (was fade-only → looked stationary). `TweenUtil.ShowObjectTop(UIRef)` /
  `HideObjectTop(UIRef)` translate the view from off-screen top (VisualElement translate is y-DOWN, so a
  NEGATIVE `-720px` offset) + fade, on the `panel-show`/`panel-hide` presets. Wired in BOTH the normal
  AnimateIn toolkit branch AND the async load continuation (first shows load late, so AnimateIn's call
  no-ops — the continuation must slide too, parking off-screen in the same frame as `Show` to avoid a
  flash). A first attempt synced to the backer's ~1.1s delay LAGGED every navigation: the shared backer
  only slides on FIRST entry and stays resident between panels — do not delay content to match it.
- **Crash fixed (NRE `ApplyStyleTranslate`):** a pooled-away panel destroys its view mid-slide, and the
  still-ticking translate tween then wrote style on a panel-less element. Fix: `FreeToolkitView` now calls
  `TweenUtil.Cancel(viewRoot)` before `DestroyView`; and `VisualElementTweenTarget`'s transform setters
  skip when `element.panel == null` (the `detached` guard — `alive`'s `element != null` is not enough).
- **Backer entrance fixed:** `BaseGameUIPanelBackgrounds.ShowUI/HideUI` were hard-coded to
  `ShowObjectBottom`/`HideObjectBottom`; switched to the **Top** variants so the shared backer eases down
  from the top (its panel-top anchor) on every PanelBacker panel.
- **Profiles DONE** (no longer deferred): migrated to the **baseline** — 2 gameverses tiles
  (`ButtonProfileGameversesApplyCode` gift / `ButtonProfileGameversesSync` reload), UXML button menu,
  cartoon backer tinted red. Sync reaches `GameState.SyncProfile` by name (`BaseUIController:2638`). The
  2.11 UIRef fields (Facebook/Twitter/GameNetwork + input) are STALE — for social buttons the build gates
  off — so `BindElements` logs ~4 benign unresolved-field warnings; reconcile the class fields with the
  shipped panel in a later cleanup. Fixed the `AddListener`→`RemoveListener` leak at
  `BaseGameUIPanelSettingsProfile.cs`.
- **Logo artifacts** turned out to be already-uncompressed textures → confirmed an NGUI-under-URP render
  issue on the still-NGUI Help panel (no import fix; clears when Help migrates in 3D).

Wave 3A is COMPLETE — all five Settings panels on UI Toolkit (credits pilot + audio + controls + root +
profiles); help → 3D. Committed engine → games-ui → content → app.
