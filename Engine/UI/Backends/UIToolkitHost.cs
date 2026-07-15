using UnityEngine;
using UnityEngine.UIElements;

namespace Engine.UI {

    // Scene presence for the UI Toolkit backend. In the PanelRenderer model it does almost
    // nothing: it holds the PanelSettings reference and hands it to the backend, which stamps it
    // onto every per-panel PanelRenderer it creates.
    //
    // It used to be a UIDocument host that built persistent layer containers and had panels
    // instantiate their UXML into them. That model was dropped for two reasons: UIDocument is
    // deprecated in Unity 6.5 (PanelRenderer is its replacement), and — more importantly — a
    // single persistent document never unloaded anything, so every panel ever opened stayed
    // resident. Per-panel PanelRenderers load and unload with the panel.
    //
    // Still in Engine/UI/Backends/ on purpose: it names UnityEngine.UIElements (PanelSettings),
    // and that may only happen inside backend files.
    public class UIToolkitHost : MonoBehaviour {

        // Assigned in the scene (the Assets/UI Toolkit/PanelSettings asset). Shared by every
        // view — one runtime panel, many PanelRenderer subtrees — so we don't pay a render
        // target per open panel.
        public PanelSettings panelSettings;

        private static UIToolkitHost _instance = null;

        public static UIToolkitHost Instance {
            get {
                return _instance;
            }
        }

        void OnEnable() {

            _instance = this;

            if (panelSettings != null) {
                UIToolkitBackend.panelSettings = panelSettings;
            }
            else {
                LogUtil.LogWarning("UIToolkitHost: no PanelSettings assigned; UI Toolkit views "
                    + "cannot render.");
            }
        }

        void OnDisable() {

            if (_instance == this) {
                _instance = null;
            }
        }
    }
}
