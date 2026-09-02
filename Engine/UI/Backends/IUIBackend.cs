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
        void SetSliderHandlerChange(UIRef r, Action<float> onChange);

        // TOGGLES

        void SetToggleValue(UIRef r, bool val);
        bool GetToggleValue(UIRef r);

        // The change half of a toggle, and the missing piece under UI Toolkit.
        //
        // The legacy path never needed it: CheckboxEvents/SliderEvents are MonoBehaviours that
        // ride the NGUI widget's own GameObject and rebroadcast its callback onto the Messenger
        // bus, so a panel only had to listen. A toolkit toggle is a VisualElement -- no
        // GameObject, so no component, so nothing broadcasts and every migrated toggle is inert
        // however correctly it is bound. Panels register here instead.
        void SetToggleHandlerChange(UIRef r, Action<bool> onChange);

        // IMAGES

        void SetImageFillValue(UIRef r, float val);
        float GetImageFillValue(UIRef r);
        void SetSpriteColor(UIRef r, Color c);

        // Show a runtime texture (typically a UIRenderStage's RenderTexture — 3D-in-UI widgets)
        // as the element's image. Replaces any styled background and neutralizes its tint: the
        // caller is saying "display exactly this texture".
        void SetImageTexture(UIRef r, Texture texture);

        // Paint an element's own fill, independent of any background IMAGE it carries.
        // SetSpriteColor above tints a sprite; this is the flat fill a swatch/chip needs, where
        // there is no sprite to tint and the colour IS the content.
        void SetElementColor(UIRef r, Color c);

        // DRAG SURFACES (colour pickers, and anything else that reads a position inside a rect
        // rather than a discrete click).
        //
        // NGUI had no equivalent: the legacy picker raycast a MeshCollider from Update and did
        // its own camera maths. A toolkit element already knows its own rect, so the backend
        // hands the panel a NORMALIZED point instead — (0,0) at the element's BOTTOM-LEFT,
        // (1,1) at its top-right, clamped, fired on pointer down and on every move while held.
        // Panels never see pointer ids, capture, or local-vs-world coordinates.
        void SetElementDragHandler(UIRef r, Action<Vector2> onDrag);

        // Place a child inside its parent by fraction of the parent's box — (0,0) bottom-left,
        // (1,1) top-right. The element is expected to be absolutely positioned and centred on
        // its own point (translate -50% -50%), so this moves a THUMB without the caller knowing
        // the parent's pixel size. Percent, not pixels, so it survives a resize.
        void SetElementOffsetPercent(UIRef r, float xPercent, float yPercent);

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

        // LISTS (wave 3D — bitty list pattern)
        //
        // Dynamic rows for a bitty "list": the view declares one row named "<X>Template"
        // (class list-item-template, hidden by common.uss) as the clone source. AddListItem
        // rebuilds a fresh row from the retained bitty tree, names it, and appends it to the
        // list's content; ClearListItems removes every non-template row. The NGUI backend
        // no-ops both (legacy panels keep their own NGUITools.AddChild grid path).

        UIRef AddListItem(UIRef view, string listName, string templateName, string itemName);
        void ClearListItems(UIRef view, string listName);

        // Rename an element. List rows use it to put the $-encoded click payload in the
        // element NAME (the legacy naming idiom) — elements have no GameObjectData, so the
        // name is the only payload channel the click bus carries.
        void SetElementName(UIRef r, string name);

        // VIEW LIFECYCLE
        //
        // LoadView is ASYNCHRONOUS: onReady fires with the view's root UIRef once it is built.
        // This is not gratuitous — the UI Toolkit backend is built on PanelRenderer (UIDocument
        // is deprecated in Unity 6.5), and PanelRenderer loads its VisualTreeAsset deferred: the
        // root only exists in its reload callback, never synchronously after assignment (verified
        // in-editor). The NGUIBackend calls onReady synchronously, so callers write one flow.
        // onReady receives UIRef.none if the view can't be loaded.
        void LoadView(string viewKey, Action<UIRef> onReady);

        // Same, with an explicit draw-order band (see UILayers). Draw order used to be load order,
        // which breaks always-on chrome: the header loads early but must render ABOVE screens
        // loaded later. Pass UILayers.auto to keep the original auto-assigned behavior.
        void LoadView(string viewKey, int sortingOrder, Action<UIRef> onReady);

        void DestroyView(UIRef view);

        // POINTER / EVENT SOURCE
        // Replaces the 4 UICamera.currentTouchID reads (InputEvents, SliderEvents,
        // CheckboxEvents, ListEvents).

        int currentPointerId { get; }
        bool IsPointerOver(Vector2 screenPos);
    }
}
