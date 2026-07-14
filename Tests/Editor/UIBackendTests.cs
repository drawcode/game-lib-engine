using System.Collections.Generic;

using NUnit.Framework;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

// UnityEngine.UI and UnityEngine.UIElements both define Button/Slider/Toggle/Image.
// These aliases pin the uGUI ones; the UI Toolkit tests below fully qualify theirs.
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;
using Object = UnityEngine.Object;
using Slider = UnityEngine.UI.Slider;
using Toggle = UnityEngine.UI.Toggle;

using Engine.UI;

namespace Engine.UI.Tests {

    // Two jobs:
    //
    //  1. BEHAVIOR PRESERVATION (the Phase 2 gate). UIUtil's GameObject overloads now dispatch
    //     to NGUIBackend. NGUIBackend is supposed to be a verbatim extraction of the bodies that
    //     used to live in UIUtil. These tests prove it by running the same GameObject down BOTH
    //     paths — legacy (no backend registered) and backend — and asserting identical results.
    //     UIPlatform.autoRegisterDefaults = false is what makes the legacy path reachable.
    //
    //  2. CONTRACT. The quirks NGUIBackend's header comment promises to preserve are asserted
    //     here, so a later "cleanup" that breaks one fails a test instead of a screen.
    public class UIBackendTests {

        private List<GameObject> spawned = new List<GameObject>();

        [SetUp]
        public void SetUp() {
            UIPlatform.autoRegisterDefaults = true;
            UIPlatform.Reset();
        }

        [TearDown]
        public void TearDown() {

            UIPlatform.autoRegisterDefaults = true;
            UIPlatform.Reset();

            for (int i = 0; i < spawned.Count; i++) {
                if (spawned[i]) {
                    Object.DestroyImmediate(spawned[i]);
                }
            }

            spawned.Clear();
        }

        private GameObject Spawn(string name) {

            GameObject go = new GameObject(name);
            spawned.Add(go);

            return go;
        }

        private static void UseLegacyPath() {
            UIPlatform.autoRegisterDefaults = false;
            UIPlatform.Reset();
        }

        private static void UseBackendPath() {
            UIPlatform.autoRegisterDefaults = true;
            UIPlatform.Reset();
        }

        // --------------------------------------------------------------------
        // A/B: legacy body vs backend, same GameObject

        [Test]
        public void LabelValue_BackendMatchesLegacy() {

            GameObject a = Spawn("label-a");
            a.AddComponent<Text>();

            GameObject b = Spawn("label-b");
            b.AddComponent<Text>();

            UseLegacyPath();
            UIUtil.SetLabelValue(a, "hello");
            string legacy = UIUtil.GetLabelValue(a);

            UseBackendPath();
            UIUtil.SetLabelValue(b, "hello");
            string backend = UIUtil.GetLabelValue(b);

            Assert.AreEqual("hello", legacy);
            Assert.AreEqual(legacy, backend);
        }

        [Test]
        public void InputValue_BackendMatchesLegacy() {

            GameObject a = Spawn("input-a");
            a.AddComponent<InputField>();

            GameObject b = Spawn("input-b");
            b.AddComponent<InputField>();

            UseLegacyPath();
            UIUtil.SetInputValue(a, "typed");
            string legacy = UIUtil.GetInputValue(a);

            UseBackendPath();
            UIUtil.SetInputValue(b, "typed");
            string backend = UIUtil.GetInputValue(b);

            Assert.AreEqual("typed", legacy);
            Assert.AreEqual(legacy, backend);
        }

        [Test]
        public void SliderValue_BackendMatchesLegacy() {

            GameObject a = Spawn("slider-a");
            a.AddComponent<Slider>();

            GameObject b = Spawn("slider-b");
            b.AddComponent<Slider>();

            UseLegacyPath();
            UIUtil.SetSliderValue(a, .25f);
            float legacy = UIUtil.GetSliderValue(a);

            UseBackendPath();
            UIUtil.SetSliderValue(b, .25f);
            float backend = UIUtil.GetSliderValue(b);

            Assert.AreEqual(.25f, legacy, .0001f);
            Assert.AreEqual(legacy, backend, .0001f);
        }

        [Test]
        public void ToggleValue_BackendMatchesLegacy() {

            GameObject a = Spawn("toggle-a");
            a.AddComponent<Toggle>();

            GameObject b = Spawn("toggle-b");
            b.AddComponent<Toggle>();

            UseLegacyPath();
            UIUtil.SetToggleValue(a, true);
            bool legacy = UIUtil.GetToggleValue(a);

            UseBackendPath();
            UIUtil.SetToggleValue(b, true);
            bool backend = UIUtil.GetToggleValue(b);

            Assert.IsTrue(legacy);
            Assert.AreEqual(legacy, backend);
        }

        [Test]
        public void IsButton_BackendMatchesLegacy() {

            GameObject button = Spawn("button");
            button.AddComponent<Button>();

            GameObject plain = Spawn("plain");

            UseLegacyPath();
            bool legacyButton = UIUtil.IsButton(button);
            bool legacyPlain = UIUtil.IsButton(plain);

            UseBackendPath();
            bool backendButton = UIUtil.IsButton(button);
            bool backendPlain = UIUtil.IsButton(plain);

            Assert.IsTrue(legacyButton);
            Assert.IsFalse(legacyPlain);
            Assert.AreEqual(legacyButton, backendButton);
            Assert.AreEqual(legacyPlain, backendPlain);
        }

        // --------------------------------------------------------------------
        // Contract: the preserved quirks

        [Test]
        public void GetLabelValue_ReturnsNull_WhenNoLabel() {

            GameObject go = Spawn("empty");

            // Null, not "". Callers distinguish the two, so a "helpful" empty string here
            // would be a silent behavior change.
            Assert.IsNull(UIUtil.GetLabelValue(go));
            Assert.IsNull(UIUtil.GetInputValue(go));
        }

        // NGUIBackend probes Slider BEFORE Toggle in SetToggleValue, which looks like a bug you
        // would want to "fix". It is unreachable: Slider and Toggle both derive from Selectable,
        // and Unity refuses to put two Selectables on one GameObject — AddComponent<Toggle>()
        // returns null when a Slider is already there (verified in-editor). So no GameObject can
        // ever hit the ambiguous branch, the ordering can never misfire, and preserving it is
        // free. This test pins that fact, so nobody re-litigates the ordering later.
        [Test]
        public void SliderAndToggle_CannotCoexist_SoProbeOrderIsUnreachable() {

            GameObject go = Spawn("slider-and-toggle");

            Slider slider = go.AddComponent<Slider>();
            Toggle toggle = go.AddComponent<Toggle>();

            Assert.IsNotNull(slider);
            Assert.IsTrue(toggle == null, "Unity allowed two Selectables on one GameObject — "
                + "the SetToggleValue probe order in NGUIBackend is now reachable and matters.");
        }

        [Test]
        public void SetToggleValue_DrivesSlider_WhenTargetIsASlider() {

            GameObject go = Spawn("slider-as-toggle");
            Slider slider = go.AddComponent<Slider>();

            UIUtil.SetToggleValue(go, true);
            Assert.AreEqual(1f, slider.value, .0001f);

            UIUtil.SetToggleValue(go, false);
            Assert.AreEqual(0f, slider.value, .0001f);
        }

        [Test]
        public void SetToggleValue_DrivesToggle_WhenTargetIsAToggle() {

            GameObject go = Spawn("real-toggle");
            Toggle toggle = go.AddComponent<Toggle>();

            Assert.IsFalse(toggle.isOn);

            UIUtil.SetToggleValue(go, true);
            Assert.IsTrue(UIUtil.GetToggleValue(go));
        }

        [Test]
        public void SetSliderValue_FallsBackToImageFill_WhenNoSlider() {

            GameObject go = Spawn("fill");
            Image image = go.AddComponent<Image>();
            image.type = Image.Type.Filled;

            UIUtil.SetSliderValue(go, .5f);

            Assert.AreEqual(.5f, image.fillAmount, .0001f);
            Assert.AreEqual(.5f, UIUtil.GetSliderValue(go), .0001f);
        }

        [Test]
        public void ShowHide_RoundTripsWithIsVisible() {

            GameObject go = Spawn("panel");
            UIRef r = UIRef.Of(go);

            UIUtil.HideObject(r);
            Assert.IsFalse(UIPlatform.For(r).IsVisible(r));

            UIUtil.ShowObject(r);
            Assert.IsTrue(UIPlatform.For(r).IsVisible(r));
        }

        // --------------------------------------------------------------------
        // UIRef + dispatch

        [Test]
        public void UIRef_None_IsNotAlive_AndOpsNoOp() {

            Assert.IsFalse(UIRef.none.alive);

            // Must not throw. A missed bind degrades to "nothing happens", never to an NRE.
            UIUtil.SetLabelValue(UIRef.none, "x");
            Assert.IsNull(UIUtil.GetLabelValue(UIRef.none));
        }

        [Test]
        public void UIRef_IsNotAlive_AfterDestroy() {

            GameObject go = new GameObject("doomed");
            UIRef r = UIRef.Of(go);

            Assert.IsTrue(r.alive);

            Object.DestroyImmediate(go);

            // Unity overloads ==, so the native reference is non-null in C# but "null" to Unity.
            Assert.IsFalse(r.alive);
        }

        [Test]
        public void Dispatch_GameObjectGoesToNGUIBackend_VisualElementGoesToToolkit() {

            GameObject go = Spawn("go");
            VisualElement el = new VisualElement();

            Assert.AreSame(NGUIBackend.Instance, UIPlatform.For(go));

#if USE_UI_TOOLKIT
            Assert.AreSame(UIToolkitBackend.Instance, UIPlatform.For(el));
#endif
            Assert.IsTrue(NGUIBackend.Instance.Handles(go));
            Assert.IsFalse(NGUIBackend.Instance.Handles(el));
        }

        [Test]
        public void Resolve_FindsChildByName() {

            GameObject root = Spawn("root");
            GameObject child = Spawn("child");
            child.transform.parent = root.transform;

            UIRef found = NGUIBackend.Instance.Resolve(UIRef.Of(root), "child");

            Assert.IsTrue(found.alive);
            Assert.AreSame(child, found.native);

            Assert.IsFalse(NGUIBackend.Instance.Resolve(UIRef.Of(root), "nope").alive);
        }

        [Test]
        public void ResolveDeep_FindsGrandchild() {

            GameObject root = Spawn("root");
            GameObject mid = Spawn("mid");
            GameObject deep = Spawn("deep");

            mid.transform.parent = root.transform;
            deep.transform.parent = mid.transform;

            UIRef found = NGUIBackend.Instance.ResolveDeep(UIRef.Of(root), "deep");

            Assert.IsTrue(found.alive);
            Assert.AreSame(deep, found.native);
        }

        // --------------------------------------------------------------------
        // UIEvents — the name-keyed bus that is deliberately NOT on IUIBackend

        [Test]
        public void UIEvents_IsButtonClicked_MatchesExactly() {

            Assert.IsTrue(UIEvents.IsButtonClicked("button-back", "button-back"));
            Assert.IsFalse(UIEvents.IsButtonClicked("button-back", "button-next"));
            Assert.IsFalse(UIEvents.IsButtonClicked(null, "button-back"));
        }

        [Test]
        public void UIEvents_IsButtonClickedLike_MatchesSubstring() {

            Assert.IsTrue(UIEvents.IsButtonClickedLike("button", "button-back"));
            Assert.IsFalse(UIEvents.IsButtonClickedLike("slider", "button-back"));
        }

        // The wire format between the broadcaster and ~200 live listeners. If these strings
        // ever drift from ButtonEvents' (game-lib-games), every click silently stops arriving.
        [Test]
        public void UIEvents_EventNames_MatchTheLegacyWireFormat() {

            Assert.AreEqual("event-button-click", UIEvents.EVENT_BUTTON_CLICK);
            Assert.AreEqual("event-button-click-object", UIEvents.EVENT_BUTTON_CLICK_OBJECT);
            Assert.AreEqual("event-button-click-data", UIEvents.EVENT_BUTTON_CLICK_DATA);
        }

        // --------------------------------------------------------------------
        // UIToolkitBackend

#if USE_UI_TOOLKIT

        [Test]
        public void Toolkit_LabelValue_RoundTrips() {

            Label label = new Label();
            label.name = "label-section";

            UIRef r = UIRef.Of(label, label.name);

            UIUtil.SetLabelValue(r, "SETTINGS");

            Assert.AreEqual("SETTINGS", label.text);
            Assert.AreEqual("SETTINGS", UIUtil.GetLabelValue(r));
        }

        [Test]
        public void Toolkit_ToggleAndSlider_RoundTrip() {

            UnityEngine.UIElements.Toggle toggle = new UnityEngine.UIElements.Toggle();
            UIRef toggleRef = UIRef.Of(toggle, "toggle-music");

            UIUtil.SetToggleValue(toggleRef, true);
            Assert.IsTrue(UIUtil.GetToggleValue(toggleRef));

            UnityEngine.UIElements.Slider slider = new UnityEngine.UIElements.Slider();
            slider.lowValue = 0f;
            slider.highValue = 1f;

            UIRef sliderRef = UIRef.Of(slider, "slider-volume");

            UIUtil.SetSliderValue(sliderRef, .75f);
            Assert.AreEqual(.75f, UIUtil.GetSliderValue(sliderRef), .0001f);
        }

        [Test]
        public void Toolkit_ShowHide_UsesDisplay() {

            VisualElement el = new VisualElement();
            UIRef r = UIRef.Of(el, "panel");

            UIUtil.HideObject(r);
            Assert.AreEqual(DisplayStyle.None, el.style.display.value);

            UIUtil.ShowObject(r);
            Assert.AreEqual(DisplayStyle.Flex, el.style.display.value);
        }

        [Test]
        public void Toolkit_Resolve_FindsByName() {

            VisualElement root = new VisualElement();
            root.name = "panel-settings";

            Label child = new Label();
            child.name = "label-title";
            root.Add(child);

            UIRef found = UIToolkitBackend.Instance.Resolve(UIRef.Of(root, root.name), "label-title");

            Assert.IsTrue(found.alive);
            Assert.AreSame(child, found.native);
        }

        [Test]
        public void Toolkit_LoadView_ReturnsNone_WhenMissing() {

            // Must degrade, not throw — a missing UXML is a content bug, not a crash.
            UIRef view = UIToolkitBackend.Instance.LoadView("panel-does-not-exist");

            Assert.IsFalse(view.alive);
        }

#endif
    }
}
