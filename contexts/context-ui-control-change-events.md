---
name: context-ui-control-change-events
description: IUIBackend could set and read a toggle/slider's value but never hear it change, so every migrated control was inert however correctly it was bound — the legacy path got its change events from MonoBehaviours riding the NGUI widget's GameObject, and a VisualElement has none. Also why GameIndicatorConfigs is non-generic.
metadata:
  type: repo
  repo: game-lib-engine
  path: Assets/Code/Libs/game-lib-engine
  created: 2026-08-31
---

# The change half of a control, and why it was missing

`IUIBackend` shipped `SetToggleValue` / `GetToggleValue` and `SetSliderValue` /
`GetSliderValue` — the **value** half — and `SetButtonHandlerClick`, the click half of a
button. There was no change half for a toggle or a slider at all. Added in `880b18c`.

## Why nobody noticed until a settings page was migrated

The legacy path never needed the method, and that is the whole trap.

`CheckboxEvents` and `SliderEvents` (game-lib-games, `Game/Events/`) are **MonoBehaviours
authored onto the NGUI widget's own GameObject**. NGUI calls their `OnActivate(bool)` /
slider callback, and they rebroadcast it onto the Messenger bus:

```csharp
Messenger<string, bool>.Broadcast(CheckboxEvents.EVENT_ITEM_CHANGE, transform.name, selected);
```

So a panel only ever had to **listen**. Nothing in a panel wires a control to itself.

A UI Toolkit control is a `VisualElement`. **No GameObject, so no component, so no
rebroadcaster, so no event.** A migrated toggle sets and reads correctly, renders its
checked state correctly, and is completely inert. Nothing errors and nothing logs.

## The surface now

```csharp
void SetToggleHandlerChange(UIRef r, Action<bool> onChange);
void SetSliderHandlerChange(UIRef r, Action<float> onChange);
```

with `UIUtil.SetToggleHandlerChange` / `SetSliderHandlerChange` facades beside the existing
value ops, dispatching through `UIPlatform.For(r)` like everything else.

- **`UIToolkitBackend`** registers `RegisterValueChangedCallback` on the `Toggle` /
  `Slider` (and `valueChanged` on a `Scroller`). Deliberately the callback and not a wrapper
  around the setter: Toolkit suppresses the callback when the new value equals the old, so a
  panel that answers a change by re-syncing every control — which is exactly what
  `BaseGameUIPanelSettingsControls.SyncCheckedState` does — settles instead of looping. A
  handler that *flips* the value it was told about will still recurse; that is on the caller.
- **`NGUIBackend`** wires the **uGUI** components only (`Toggle.onValueChanged`,
  `Slider`/`Scrollbar.onValueChanged`) and leaves the NGUI widgets to their existing
  rebroadcasters. A panel listening both ways during a migration would otherwise get each
  change **twice**. Same shape as `SetButtonHandlerClick`, whose NGUI branch is also a no-op.

## The rule for a migrated panel

Binding is only half the job. See `game-lib-games-ui/contexts/context-settings-controls-binding.md`
for the panel side — Settings: Controls had **both** faults at once, so fixing only the
binding still left a dead page.

Register in an override of `BindElements`, which is the continuation the async `LoadView`
runs **after** the elements exist AND after `base.BindElements` has filled the fields. Not
`Init`, not `OnEnable` — the view does not exist yet at either.

## Unrelated, same commit: GameIndicatorConfigs is non-generic on purpose

`BaseGameConfigs.cs` now also carries

```csharp
public static class GameIndicatorConfigs {
    public static float scale = .9f;            // 10% shrink, the shipped default
    public static float scaleMin = .5f;
    public static float scaleMax = 1.5f;
    public static float edgeBorderScale = .5f;  // multiplies the prefab's clampBorderSize
}
```

**Not** on `BaseGameConfigs<T>`. That class is generic, so its statics are only reachable
through a closed type — the app's `GameConfigs` — which this lib cannot name.
`BaseGameProfile` lives here and needs the same numbers for its slider bounds, so they sit
in a non-generic class both sides can see. If you add a dial that the engine lib itself has
to read, put it here, not on the generic.

`BaseGameProfile` gained `Get/SetControlIndicatorScale` (`ATT_CONTROL_INDICATOR_SCALE`),
defaulting to `GameIndicatorConfigs.scale` rather than a literal 1 so that "never touched the
slider" and "dragged it back to the shipping value" agree.

**`Mathf` is fully qualified there, and must stay that way.** Adding `using UnityEngine` to
`BaseGameProfile.cs` makes the `new Object()` in `BaseGameProfiles` ambiguous with
`System.Object` and the file stops compiling.

## Related

- `context-ui-backend-design.md` — the interface's design rules; this is an addition to it
- `game-lib-games/contexts/context-offscreen-indicator-dials.md` — who reads the dials
