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
