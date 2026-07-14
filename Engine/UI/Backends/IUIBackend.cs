using System;
using System.Collections.Generic;

using UnityEngine;

namespace Engine.UI {

    // The UI provider contract. Mirrors ITweenBackend: one interface, plain C# singleton
    // implementations, no MonoBehaviour, no #if inside call sites (defines gate which
    // backend FILES compile and register, never which branch a caller takes).
    //
    // Scope note: this is deliberately much smaller than UIUtil's ~120 public members.
    // UIUtil is an overload-resolution shim — each family is (NGUI overload) /
    // (uGUI overload) / (GameObject resolver), and only the GameObject resolver is
    // genuinely polymorphic. That resolver is the seam, so the interface is the ~8 live
    // op groups below. UIUtil's dead members (IsEventReady, FindLabel, UIButtonEnable,
    // GridReposition(Grid), UIButtonMeta) are excluded but NOT deleted — core libs are
    // shared with other projects, so UIUtil's public surface is additive-only.
    //
    // Every op must no-op (or return the type's zero) on a ref that is not alive.
    public interface IUIBackend {

        // Per-object coexistence dispatch. NGUIBackend claims GameObject; UIToolkitBackend
        // claims VisualElement. Both are registered and live simultaneously through all of
        // Phases 2-3 — this is what lets a migrated panel and an NGUI panel coexist with
        // zero churn at the 525 UIUtil call sites.
        bool Handles(object native);

        // RESOLUTION

        UIRef Resolve(UIRef root, string name);
        UIRef ResolveDeep(UIRef root, string name);
        List<UIRef> ResolveLike(UIRef root, string code);

        // LABELS

        void SetLabelValue(UIRef r, string val);
        string GetLabelValue(UIRef r);
        void SetLabelColor(UIRef r, Color c);

        // INPUTS

        void SetInputValue(UIRef r, string val);
        string GetInputValue(UIRef r);

        // SLIDERS

        void SetSliderValue(UIRef r, float val);
        float GetSliderValue(UIRef r);

        // TOGGLES

        void SetToggleValue(UIRef r, bool val);
        bool GetToggleValue(UIRef r);

        // IMAGES

        void SetImageFillValue(UIRef r, float val);
        float GetImageFillValue(UIRef r);
        void SetSpriteColor(UIRef r, Color c);

        // BUTTONS
        // The object half only. The name-compare half (IsButtonClicked(string, string),
        // 199 call sites) is NOT here — it is the event bus, and it lives on UIEvents.

        bool IsButton(UIRef r);
        void SetButtonColor(UIRef r, Color c);
        void SetButtonHandlerClick(UIRef r, Action onClick);

        // VISIBILITY
        // Display-state only. Tweens must never call these: Phase 1 gate learning #1 is that
        // tweens do not own active-state, with the single documented exception of
        // FadeToObject's Show()/Hide() side effects on non-sprite containers (learning #8),
        // which stays where it already is inside TweenUtil. Re-implementing that policy here
        // would silently re-break the header title/backer bug learning #8 documents.

        void Show(UIRef r);
        void Hide(UIRef r);
        bool IsVisible(UIRef r);

        // LAYOUT

        void GridReposition(UIRef r);

        // VIEW LIFECYCLE

        UIRef LoadView(string viewKey);
        void Attach(UIRef view, UIRef parent);
        void Detach(UIRef view);
        void DestroyView(UIRef view);

        // POINTER / EVENT SOURCE
        // Replaces the 4 UICamera.currentTouchID reads (InputEvents, SliderEvents,
        // CheckboxEvents, ListEvents).

        int currentPointerId { get; }
        bool IsPointerOver(Vector2 screenPos);
    }
}
