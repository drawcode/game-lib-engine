using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using Engine.Utility;

namespace Engine.UI {

    // The GameObject backend. Despite the name it carries BOTH the NGUI and the uGUI probes,
    // because UIUtil's GameObject-resolver bodies always probed both inside the same method —
    // this class is the extraction of those bodies, and the extraction must be exact.
    //
    // Deleted in Phase 4 along with the NGUI define. Until then it is the backend that claims
    // every GameObject, which means the 525 existing UIUtil call sites keep behaving exactly
    // as they do today while the UIToolkitBackend serves migrated screens in the same frame.
    //
    // ---------------------------------------------------------------------------------------
    // EXTRACTION RULES — every quirk below is load-bearing and was verified against the
    // original bodies in Engine/UI/UIUtil.cs. Do not "clean these up":
    //
    //  * SetLabelValue / SetInputValue / SetLabelColor apply the NGUI branch AND the uGUI
    //    branch, with no else. A GameObject carrying both gets both set.
    //  * The GETTERS return on the first hit, and GetLabelValue/GetInputValue return NULL
    //    (not "") when nothing matches. Callers distinguish the two.
    //  * SetSliderValue probes Slider, ELSE Scrollbar, ELSE Image-as-fill. GetSliderValue
    //    mirrors it. The NGUI UISlider branch is independent of that chain.
    //  * SetToggleValue probes Slider FIRST, else Toggle. The ordering looks wrong but is
    //    unreachable: Slider and Toggle both derive from Selectable, and Unity refuses to put
    //    two Selectables on one GameObject (AddComponent<Toggle>() returns null when a Slider
    //    is already there). No object can hit the ambiguous branch, so preserving it is free.
    //    Pinned by SliderAndToggle_CannotCoexist_SoProbeOrderIsUnreachable.
    //  * SetSpriteColor is NOT component-gated: it colors any GameObject through
    //    TweenUtil.ColorToObject, whose NGUI child-widget recursion is live (gate learning #5).
    //  * SetButtonHandlerClick's NGUI branch is a NO-OP (the original body is commented out).
    //    Only the uGUI Button branch actually wires anything.
    // ---------------------------------------------------------------------------------------
    public class NGUIBackend : IUIBackend {

        private static NGUIBackend _instance = null;

        public static NGUIBackend Instance {
            get {

                if (_instance == null) {
                    _instance = new NGUIBackend();
                }

                return _instance;
            }
        }

        public bool Handles(object native) {
            return native is GameObject;
        }

        private static GameObject Go(UIRef r) {

            if (r == null || !r.alive) {
                return null;
            }

            return r.gameObject;
        }

        // RESOLUTION

        public UIRef Resolve(UIRef root, string name) {

            GameObject go = Go(root);

            if (go == null || string.IsNullOrEmpty(name)) {
                return UIRef.none;
            }

            Transform t = go.transform.Find(name);

            if (t == null) {
                return UIRef.none;
            }

            return UIRef.Of(t.gameObject);
        }

        // Mirrors UpdateLabelObject's recursion: direct Find first, then depth-first through
        // every child. Inactive children ARE reachable this way — Transform iteration does not
        // skip them, unlike GetComponentsInChildren.
        public UIRef ResolveDeep(UIRef root, string name) {

            GameObject go = Go(root);

            if (go == null || string.IsNullOrEmpty(name)) {
                return UIRef.none;
            }

            return ResolveDeep(go.transform, name);
        }

        private static UIRef ResolveDeep(Transform parent, string name) {

            Transform found = parent.Find(name);

            if (found != null) {
                return UIRef.Of(found.gameObject);
            }

            foreach (Transform child in parent) {

                UIRef r = ResolveDeep(child, name);

                if (r.alive) {
                    return r;
                }
            }

            return UIRef.none;
        }

        // Name-substring match over the widget-bearing children, matching SetTextValue's
        // original component sets (UILabel/UIInput + Text/InputField). Deduped by GameObject:
        // the original visited a GameObject once per matching component, but every consumer
        // is idempotent, so the observable result is identical.
        public List<UIRef> ResolveLike(UIRef root, string code) {

            List<UIRef> results = new List<UIRef>();

            GameObject go = Go(root);

            if (go == null || string.IsNullOrEmpty(code)) {
                return results;
            }

            HashSet<GameObject> seen = new HashSet<GameObject>();

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
            UILabel[] labels = go.GetComponentsInChildren<UILabel>();

            foreach (UILabel label in labels) {
                if (label.gameObject.name.Contains(code) && seen.Add(label.gameObject)) {
                    results.Add(UIRef.Of(label.gameObject));
                }
            }

            UIInput[] inputs = go.GetComponentsInChildren<UIInput>();

            foreach (UIInput input in inputs) {
                if (input.gameObject.name.Contains(code) && seen.Add(input.gameObject)) {
                    results.Add(UIRef.Of(input.gameObject));
                }
            }
#endif

            Text[] labelsNative = go.GetComponentsInChildren<Text>();

            foreach (Text label in labelsNative) {
                if (label.gameObject.name.Contains(code) && seen.Add(label.gameObject)) {
                    results.Add(UIRef.Of(label.gameObject));
                }
            }

            InputField[] inputsNative = go.GetComponentsInChildren<InputField>();

            foreach (InputField input in inputsNative) {
                if (input.gameObject.name.Contains(code) && seen.Add(input.gameObject)) {
                    results.Add(UIRef.Of(input.gameObject));
                }
            }

            return results;
        }

        // LABELS

        public void SetLabelValue(UIRef r, string val) {

            GameObject obj = Go(r);

            if (obj == null) {
                return;
            }

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
            if (obj.Has<UILabel>()) {
                UIUtil.SetLabelValue(obj.Get<UILabel>(), val);
            }
#endif
            if (obj.Has<Text>()) {
                UIUtil.SetLabelValue(obj.Get<Text>(), val);
            }
        }

        public string GetLabelValue(UIRef r) {

            GameObject obj = Go(r);

            if (obj != null) {

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
                if (obj.Has<UILabel>()) {
                    return UIUtil.GetLabelValue(obj.Get<UILabel>());
                }
#endif
                if (obj.Has<Text>()) {
                    return UIUtil.GetLabelValue(obj.Get<Text>());
                }
            }

            // null, not "" — the original does this and callers rely on it.
            return null;
        }

        public void SetLabelColor(UIRef r, Color c) {

            GameObject obj = Go(r);

            if (obj == null) {
                return;
            }

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
            if (obj.Has<UILabel>()) {
                UIUtil.SetLabelColor(obj.Get<UILabel>(), c);
            }
#endif
            if (obj.Has<Text>()) {
                UIUtil.SetLabelColor(obj.Get<Text>(), c);
            }
        }

        // INPUTS

        public void SetInputValue(UIRef r, string val) {

            GameObject obj = Go(r);

            if (obj == null) {
                return;
            }

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
            if (obj.Has<UIInput>()) {
                UIUtil.SetInputValue(obj.Get<UIInput>(), val);
            }
#endif
            if (obj.Has<InputField>()) {
                UIUtil.SetInputValue(obj.Get<InputField>(), val);
            }
        }

        public string GetInputValue(UIRef r) {

            GameObject obj = Go(r);

            if (obj != null) {

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
                if (obj.Has<UIInput>()) {
                    return UIUtil.GetInputValue(obj.Get<UIInput>());
                }
#endif
                if (obj.Has<InputField>()) {
                    return UIUtil.GetInputValue(obj.Get<InputField>());
                }
            }

            return null;
        }

        // SLIDERS

        public void SetSliderValue(UIRef r, float val) {

            GameObject obj = Go(r);

            if (obj == null) {
                return;
            }

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
            if (obj.Has<UISlider>()) {
                UIUtil.SetSliderValue(obj.Get<UISlider>(), val);
            }
#endif
            if (obj.Has<Slider>()) {
                UIUtil.SetSliderValue(obj.Get<Slider>(), val);
            }
            else if (obj.Has<Scrollbar>()) {
                UIUtil.SetSliderValue(obj.Get<Scrollbar>(), val);
            }
            else if (obj.Has<Image>()) {
                UIUtil.SetSliderValue(obj.Get<Image>(), (double)val);
            }
        }

        public float GetSliderValue(UIRef r) {

            GameObject obj = Go(r);

            if (obj != null) {

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
                if (obj.Has<UISlider>()) {
                    return UIUtil.GetSliderValue(obj.Get<UISlider>());
                }
#endif
                if (obj.Has<Slider>()) {
                    return UIUtil.GetSliderValue(obj.Get<Slider>());
                }
                else if (obj.Has<Scrollbar>()) {
                    return UIUtil.GetSliderValue(obj.Get<Scrollbar>());
                }
                else if (obj.Has<Image>()) {
                    return UIUtil.GetSliderValue(obj.Get<Image>());
                }
            }

            return 0f;
        }

        // TOGGLES

        public void SetToggleValue(UIRef r, bool val) {

            GameObject obj = Go(r);

            if (obj == null) {
                return;
            }

#if USE_UI_NGUI_2_7
            if (obj.Has<UICheckbox>()) {
                UIUtil.SetToggleValue(obj.Get<UICheckbox>(), val);
            }
#endif
#if USE_UI_NGUI_3
            if (obj.Has<UIToggle>()) {
                UIUtil.SetToggleValue(obj.Get<UIToggle>(), val);
            }
#endif
            // Slider before Toggle — preserved from the original. Unreachable ambiguity: Unity
            // won't allow both Selectables on one GameObject (see the header note).
            if (obj.Has<Slider>()) {
                UIUtil.SetToggleValue(obj.Get<Slider>(), val);
            }
            else if (obj.Has<Toggle>()) {
                UIUtil.SetToggleValue(obj.Get<Toggle>(), val);
            }
        }

        public bool GetToggleValue(UIRef r) {

            GameObject obj = Go(r);

            if (obj != null) {

#if USE_UI_NGUI_2_7
                if (obj.Has<UICheckbox>()) {
                    return UIUtil.GetToggleValue(obj.Get<UICheckbox>());
                }
#endif
#if USE_UI_NGUI_3
                if (obj.Has<UIToggle>()) {
                    return UIUtil.GetToggleValue(obj.Get<UIToggle>());
                }
#endif
                if (obj.Has<Toggle>()) {
                    return UIUtil.GetToggleValue(obj.Get<Toggle>());
                }
            }

            return false;
        }

        // IMAGES

        public void SetImageFillValue(UIRef r, float val) {

            GameObject obj = Go(r);

            if (obj == null) {
                return;
            }

            if (obj.Has<Image>()) {
                UIUtil.SetImageFillValue(obj.Get<Image>(), (double)val);
            }
        }

        public float GetImageFillValue(UIRef r) {

            GameObject obj = Go(r);

            if (obj == null) {
                return 0f;
            }

            if (obj.Has<Image>()) {
                return UIUtil.GetImageFillValue(obj.Get<Image>());
            }

            return 0f;
        }

        // Not component-gated: the original colors ANY GameObject through the tween facade,
        // whose NGUI child-widget recursion is live (gate learning #5). Do not add a probe.
        public void SetSpriteColor(UIRef r, Color c) {

            GameObject obj = Go(r);

            if (obj == null) {
                return;
            }

            TweenUtil.ColorToObject(obj, c, .5f, 0f);
        }

        // Deliberate no-op: NGUI/uGUI widgets take textures through their own atlas/sprite
        // pipeline, never a runtime-texture swap; the RT widget path is a toolkit-view feature.
        public void SetImageTexture(UIRef r, Texture texture) {
        }

        // BUTTONS

        public bool IsButton(UIRef r) {

            GameObject go = Go(r);

            if (go == null) {
                return false;
            }

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
            if (go.Has<UIButton>() || go.Has<UIImageButton>()) {
                return true;
            }
#endif
            if (go.Has<Button>()) {
                return true;
            }

            return false;
        }

        public void SetButtonColor(UIRef r, Color c) {

            GameObject obj = Go(r);

            if (obj == null) {
                return;
            }

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
            if (obj.Has<UIButton>()) {
                UIUtil.SetButtonColor(obj.Get<UIButton>(), c);
            }
#endif
            if (obj.Has<Button>()) {
                UIUtil.SetButtonColor(obj.Get<Button>(), c);
            }
        }

        public void SetButtonHandlerClick(UIRef r, Action onClick) {

            GameObject obj = Go(r);

            if (obj == null || onClick == null) {
                return;
            }

            // The NGUI branch of the original is commented out — wiring an NGUI button's
            // handler here has never done anything. Clicks reach panels through ButtonEvents'
            // name broadcast instead, which is why the whole input path is already agnostic.
            if (obj.Has<Button>()) {
                UIUtil.SetButtonHandlerClick(obj.Get<Button>(), new UnityAction(onClick));
            }
        }

        // VISIBILITY

        public void Show(UIRef r) {

            GameObject obj = Go(r);

            if (obj == null) {
                return;
            }

            obj.Show();
        }

        public void Hide(UIRef r) {

            GameObject obj = Go(r);

            if (obj == null) {
                return;
            }

            obj.Hide();
        }

        // activeSelf, not activeInHierarchy — GameObjectHelper.Show/Hide gate on activeSelf,
        // so this is the predicate that actually round-trips with them.
        public bool IsVisible(UIRef r) {

            GameObject obj = Go(r);

            if (obj == null) {
                return false;
            }

            return obj.activeSelf;
        }

        // LAYOUT

        public void GridReposition(UIRef r) {

            GameObject grid = Go(r);

            if (grid == null) {
                return;
            }

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
            if (grid.Has<UIGrid>()) {
                UIUtil.GridReposition(grid.Get<UIGrid>());
            }
#endif
            // The original also probed UnityEngine.Grid (the tilemap component) and called a
            // method whose body is commented out. That branch is a no-op in every sense, so it
            // is not reproduced — but UIUtil.GridReposition(Grid) itself stays, per the
            // additive-only rule for shared libs.
        }

        // VIEW LIFECYCLE

        // NGUI views are Resources prefabs loaded by the app's content pipeline
        // (AppContentAssets.LoadAssetUI), which lives above the engine. The panel system keeps
        // doing that itself in the NGUI path; nothing routes NGUI views through here. Fire the
        // continuation synchronously with none so callers share one flow with the Toolkit backend.
        public void LoadView(string viewKey, Action<UIRef> onReady) {

            if (onReady != null) {
                onReady(UIRef.none);
            }
        }

        // Draw order is meaningless here: NGUI screens are Resources prefabs instantiated by the
        // app's content pipeline (AppContentAssets.LoadAssetUI), never through this seam, so
        // LoadView is a deliberate no-op on both overloads.
        public void LoadView(string viewKey, int sortingOrder, Action<UIRef> onReady) {
            LoadView(viewKey, onReady);
        }

        // LISTS (wave 3D): no-op on the legacy backend — NGUI panels keep their own
        // NGUITools.AddChild grid path; only toolkit views build rows through the platform.
        public UIRef AddListItem(UIRef view, string listName, string templateName, string itemName) {
            return UIRef.none;
        }

        public void ClearListItems(UIRef view, string listName) {
        }

        public void DestroyView(UIRef view) {

            GameObject go = Go(view);

            if (go == null) {
                return;
            }

            UnityEngine.Object.Destroy(go);
        }

        // POINTER / EVENT SOURCE

        // 0, not -1, when NGUI is compiled out: the four legacy call sites (InputEvents,
        // SliderEvents, CheckboxEvents, ListEvents) all initialized `int camIndex = 0` and only
        // overwrote it under the NGUI define. Preserve that exactly.
        public int currentPointerId {
            get {

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
                return UICamera.currentTouchID;
#else
                return 0;
#endif
            }
        }

        // NGUI does its own raycasting through UICamera; nothing asks this backend whether a
        // pointer is over it. The Toolkit backend implements the real thing (panel.Pick), and
        // the coexistence guard in UICamera.Raycast consults THAT one.
        public bool IsPointerOver(Vector2 screenPos) {
            return false;
        }
    }
}
