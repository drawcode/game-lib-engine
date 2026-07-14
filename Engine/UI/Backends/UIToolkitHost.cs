using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

namespace Engine.UI {

    // Scene host for the UI Toolkit backend: owns the UIDocument, publishes the ordered layer
    // containers, and registers the root with UIToolkitBackend (which wires the click bridge and
    // the pointer source).
    //
    // This lives in Engine/UI/Backends/ ON PURPOSE. It needs UIDocument, and the platform rule is
    // that UnityEngine.UIElements may be named only inside backend/adapter files. Putting the host
    // in games-ui — where it "belongs" conceptually — would punch a UIElements dependency straight
    // through the provider layer and break the seam the whole migration rests on. The panel system
    // talks to it through UIRef only.
    //
    // Layer order is the coexistence contract (plan D4/D7): screens render under the HUD, overlays
    // above both, and a migrated fullscreen panel simply covers the NGUI behind it.
    [RequireComponent(typeof(UIDocument))]
    public class UIToolkitHost : MonoBehaviour {

        private static UIToolkitHost _instance = null;

        public static UIToolkitHost Instance {
            get {
                return _instance;
            }
        }

        private UIDocument document;
        private readonly Dictionary<string, VisualElement> layers = new Dictionary<string, VisualElement>();

        void OnEnable() {

            _instance = this;

            document = GetComponent<UIDocument>();

            if (document == null) {
                return;
            }

            VisualElement root = document.rootVisualElement;

            if (root == null) {
                return;
            }

            // The root must not swallow gameplay taps. Picks that land on it (rather than on a
            // real widget) fall through to NGUI/gameplay — see UIToolkitBackend.IsPointerOver,
            // which ignores picks on the root for exactly this reason.
            root.pickingMode = PickingMode.Ignore;
            root.style.flexGrow = 1f;

            EnsureLayer(root, UIPlatform.layerScreens);
            EnsureLayer(root, UIPlatform.layerHud);
            EnsureLayer(root, UIPlatform.layerOverlay);

            UIToolkitBackend.Instance.RegisterRoot(root);

            // The panel system asks UIPlatform for a layer, never this class — so games-ui never
            // names a backend host type.
            UIPlatform.SetLayerResolver(GetLayer);
        }

        void OnDisable() {

            if (document != null && document.rootVisualElement != null) {
                UIToolkitBackend.Instance.UnregisterRoot(document.rootVisualElement);
            }

            layers.Clear();

            UIPlatform.SetLayerResolver(null);

            if (_instance == this) {
                _instance = null;
            }
        }

        private void EnsureLayer(VisualElement root, string name) {

            VisualElement layer = root.Q(name);

            if (layer == null) {

                layer = new VisualElement();
                layer.name = name;
                layer.pickingMode = PickingMode.Ignore;

                // Layers stack; children position themselves within them.
                layer.style.position = Position.Absolute;
                layer.style.left = 0f;
                layer.style.top = 0f;
                layer.style.right = 0f;
                layer.style.bottom = 0f;

                root.Add(layer);
            }

            layers[name] = layer;
        }

        // The only way the panel system reaches a layer: as an opaque UIRef. No UIElements type
        // crosses this boundary.
        public UIRef GetLayer(string name) {

            VisualElement layer = null;

            if (!layers.TryGetValue(name, out layer) || layer == null) {
                return UIRef.none;
            }

            return UIRef.Of(layer, name);
        }

    }
}
