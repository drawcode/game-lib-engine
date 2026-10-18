using UnityEngine;
using UnityEngine.UIElements;

using Object = UnityEngine.Object;

namespace Engine.UI.Tests {

    // A real UI Toolkit panel for EditMode tests (UIDocument + a throwaway PanelSettings; both work
    // outside Play mode).
    //
    // Why: the toolkit backend and VisualElementTweenTarget treat an element with no panel as DEAD
    // and no-op every write -- that is the guard against writing into a torn-down view (Unity
    // recycles a freed view's elements a frame later, and writing into them throws inside
    // UIElements). A detached `new Label()` is therefore indistinguishable from a recycled one, and
    // a test that writes to it measures nothing. Attach the element here instead, as LoadView does
    // before it ever hands out a UIRef.
    //
    //     using (ToolkitTestPanel panel = new ToolkitTestPanel()) {
    //         Label label = panel.Attach(new Label());
    //         ...
    //     }
    public sealed class ToolkitTestPanel : System.IDisposable {

        private GameObject host;
        private PanelSettings settings;

        public VisualElement root { get; private set; }

        public ToolkitTestPanel() {

            settings = ScriptableObject.CreateInstance<PanelSettings>();

            host = new GameObject("ToolkitTestPanel");

            UIDocument document = host.AddComponent<UIDocument>();
            document.panelSettings = settings;

            root = document.rootVisualElement;
        }

        public T Attach<T>(T element) where T : VisualElement {
            root.Add(element);
            return element;
        }

        public void Dispose() {

            if (host) {
                Object.DestroyImmediate(host);
            }

            if (settings) {
                Object.DestroyImmediate(settings);
            }

            host = null;
            settings = null;
            root = null;
        }
    }
}
