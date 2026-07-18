using UnityEngine;
using UnityEngine.UIElements;

using Engine.Game.App.BaseApp;
using Engine.UI.Bitty;

namespace Engine.UI {

    // The UI Toolkit element factory: BittyNode tree -> VisualElement tree.
    //
    // This is BACKEND code — it names UnityEngine.UIElements, so it lives in Engine/UI/Backends/
    // alongside UIToolkitBackend (the Phase 2 gate greps for UIElements leaks OUTSIDE this folder).
    // Splitting it from BittyParser/BittyPatterns (which are agnostic) is the whole D6 point: the
    // parse and the pattern expansion are portable; only this last mile is framework-specific. A
    // DOM backend would ship its own builder against the same BittyNode tree.
    //
    // Deliberately small: v1 supports the element types the settings panels actually use. Styling
    // is classes only (BittySchema's rule) — inline `style` strings are NOT applied here, because
    // parsing a USS declaration block to VisualElement.style at runtime is exactly the
    // hand-rolled-CSS the prototype died of. Anything an option-row needs is a class in common.uss.
    public static class BittyToolkitBuilder {

        // Volume/percent controls are 0..1; that covers every current slider consumer. A slider
        // that needs another range is a later extension (a min/max on the schema), not a v1 gap.
        private const float sliderLow = 0f;
        private const float sliderHigh = 1f;

        public static VisualElement Build(BittyNode node) {

            if (node == null) {
                return null;
            }

            VisualElement el = Create(node);

            if (el == null) {
                return null;
            }

            if (!string.IsNullOrEmpty(node.name)) {
                el.name = node.name;
            }

            for (int i = 0; i < node.classes.Count; i++) {

                if (!string.IsNullOrEmpty(node.classes[i])) {
                    el.AddToClassList(node.classes[i]);
                }
            }

            for (int i = 0; i < node.children.Count; i++) {

                VisualElement child = Build(node.children[i]);

                if (child != null) {
                    el.Add(child);
                }
            }

            return el;
        }

        private static VisualElement Create(BittyNode n) {

            string type = n.type ?? BittySchema.Types.container;

            if (type == BittySchema.Types.label) {
                return new Label(Loc(n.text));
            }

            if (type == BittySchema.Types.button) {

                Button button = new Button();
                button.text = Loc(n.text);

                // No click handler wired here: clicks reach handlers through the root's bubbling
                // ClickEvent -> UIEvents.BroadcastClick(name), exactly like UXML buttons. The
                // button's `name` (set by Build) is the broadcast key.
                return button;
            }

            if (type == BittySchema.Types.image) {
                // Sprite comes from a `.spr-<name>` class (sprites.uss), same as the converter's
                // output — the node carries it in `class`.
                return new VisualElement();
            }

            if (type == BittySchema.Types.textfield) {

                TextField field = new TextField();
                field.value = Str(n.value);

                return field;
            }

            if (type == BittySchema.Types.toggle) {
                return BuildToggle(n);
            }

            if (type == BittySchema.Types.slider) {
                return BuildSlider(n);
            }

            if (type == BittySchema.Types.dropdown) {

                DropdownField dropdown = new DropdownField();

                if (n.choices != null) {
                    dropdown.choices = n.choices;
                }

                string v = Str(n.value);

                if (!string.IsNullOrEmpty(v)) {
                    dropdown.value = v;
                }

                return dropdown;
            }

            if (type == BittySchema.Types.scrollview) {

                // The reusable scroll pattern: tagging it .ngui-scrollview makes UIToolkitBackend's
                // ConfigureScrollViews wire the momentum/auto-hide ScrollDrag on load — no per-panel
                // scroll code, same as the converter-emitted credits scroller.
                ScrollView scroll = new ScrollView();
                scroll.AddToClassList("ngui-scrollview");

                return scroll;
            }

            // container / row / anything unknown -> a plain box; `class` gives it its layout.
            return new VisualElement();
        }

        // SLIDER — UI Toolkit's Slider has no fill, so we compose the NGUI look: a grey track and a
        // green fill (tracking the value) BEHIND the native drag-container's cartoon knob. Document
        // order sets z: add fill then SendToBack, then track then SendToBack, so the final child
        // order is track(0) < fill(1) < drag-container(2). The native tracker is made transparent in
        // USS. The Slider keeps node.name (set by Build), so bind + the value bridge still key on it.
        private static VisualElement BuildSlider(BittyNode n) {

            Slider slider = new Slider(sliderLow, sliderHigh);
            slider.value = Float(n.value, sliderHigh);

            VisualElement fill = Deco("slider-fill");
            slider.Add(fill);
            fill.SendToBack();

            VisualElement track = Deco("slider-track");
            slider.Add(track);
            track.SendToBack();

            ApplyFill(fill, slider.value);
            slider.RegisterCallback<ChangeEvent<float>>(evt => ApplyFill(fill, evt.newValue));

            return slider;
        }

        private static void ApplyFill(VisualElement fill, float v01) {
            fill.style.width = Length.Percent(Mathf.Clamp01(v01) * 100f);
        }

        // TOGGLE — the native checkmark is HIDDEN and replaced by our own cartoon box + star overlay,
        // driven from the value in code. This sidesteps the default theme, which swaps the
        // checkmark's background-image per state — that swap is what made the cartoon box vanish when
        // checked/hovered. The box is a picking-ignore child of the input, so taps still reach the
        // Toggle and flip its value, and the Toggle's own ChangeEvent<bool> still reaches the value
        // bridge unchanged. State (tint + star + hover-lighten) is all set here, not via USS
        // :checked selectors (whose exact form varies), so the behavior is version-robust.
        private static readonly Color toggleOff = new Color32(0x6E, 0x6E, 0x6E, 0xFF);
        private static readonly Color toggleOffHover = new Color32(0x9A, 0x9A, 0x9A, 0xFF);
        private static readonly Color toggleOn = new Color32(0x60, 0xD6, 0x00, 0xFF);
        private static readonly Color toggleOnHover = new Color32(0x7B, 0xE8, 0x22, 0xFF);

        private static VisualElement BuildToggle(BittyNode n) {

            Toggle toggle = new Toggle();
            toggle.text = Loc(n.text);
            toggle.value = Bool(n.value);

            VisualElement native = toggle.Q(className: "unity-toggle__checkmark");
            VisualElement input = toggle.Q(className: "unity-toggle__input");

            if (native != null) {
                native.style.display = DisplayStyle.None;
            }

            if (input != null) {

                VisualElement box = Deco("toggle-box");
                input.Insert(0, box);

                VisualElement star = Deco("toggle-star");
                box.Add(star);

                bool[] hovering = { false };

                System.Action apply = () => {
                    bool on = toggle.value;
                    box.style.unityBackgroundImageTintColor =
                        on ? (hovering[0] ? toggleOnHover : toggleOn)
                           : (hovering[0] ? toggleOffHover : toggleOff);
                    star.style.display = on ? DisplayStyle.Flex : DisplayStyle.None;
                };

                apply();
                toggle.RegisterValueChangedCallback(evt => apply());
                box.RegisterCallback<PointerEnterEvent>(evt => { hovering[0] = true; apply(); });
                box.RegisterCallback<PointerLeaveEvent>(evt => { hovering[0] = false; apply(); });
            }

            return toggle;
        }

        private static VisualElement Deco(string cls) {

            VisualElement e = new VisualElement();
            e.AddToClassList(cls);
            e.pickingMode = PickingMode.Ignore;

            return e;
        }

        // @loc:key -> Locos.GetString(key); literal text passes through. Resolved at BUILD time so
        // BittyNode stays a pure data record and the same tree localizes per-locale on each build.
        private static string Loc(string text) {

            if (string.IsNullOrEmpty(text)) {
                return text;
            }

            if (BittySchema.IsLocalized(text)) {
                return Locos.GetString(BittySchema.LocKey(text));
            }

            return text;
        }

        // MiniJSON hands numbers back as double/long and bools as bool; coerce defensively so an
        // author writing 1 vs 1.0 vs "1" all work.

        private static string Str(object value) {

            if (value == null) {
                return "";
            }

            return value as string ?? value.ToString();
        }

        private static bool Bool(object value) {

            if (value is bool) {
                return (bool)value;
            }

            if (value is string) {
                return (string)value == "true" || (string)value == "1";
            }

            if (value is double) {
                return (double)value != 0d;
            }

            if (value is long) {
                return (long)value != 0L;
            }

            return false;
        }

        private static float Float(object value, float fallback) {

            if (value is double) {
                return (float)(double)value;
            }

            if (value is long) {
                return (float)(long)value;
            }

            if (value is float) {
                return (float)value;
            }

            if (value is string) {

                float parsed = 0f;

                if (float.TryParse((string)value, out parsed)) {
                    return parsed;
                }
            }

            return fallback;
        }
    }
}
