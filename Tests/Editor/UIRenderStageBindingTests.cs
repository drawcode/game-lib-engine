using System.Collections.Generic;

using NUnit.Framework;

using UnityEngine;
using UnityEngine.UIElements;

using Object = UnityEngine.Object;

using Engine.UI;

namespace Engine.UI.Tests {

    // UIRenderStageBinding lifetime, headless (B9 S1). The view here is a plain GameObject tree,
    // which NGUIBackend claims: IsVisible = activeSelf, SetImageTexture a no-op, no geometry
    // capability — so this exercises the lifetime + visibility + fallback-size paths without a
    // panel. The toolkit geometry path (TryGetElementPixelSize on a laid-out element) needs a live
    // PanelRenderer and is covered in play mode, not here; only its "detached = not alive" branch
    // is headless.
    public class UIRenderStageBindingTests {

        private readonly List<GameObject> spawned = new List<GameObject>();

        // A layer the content is guaranteed not to start on, so a flip is observable even in a
        // project without UIWidget3D/UI3D defined.
        private const int stageLayer = 31;

        [SetUp]
        public void SetUp() {
            UIPlatform.autoRegisterDefaults = true;
            UIPlatform.Reset();
        }

        [TearDown]
        public void TearDown() {

            for (int i = 0; i < spawned.Count; i++) {
                if (spawned[i]) {
                    Object.DestroyImmediate(spawned[i]);
                }
            }

            spawned.Clear();

            UIPlatform.autoRegisterDefaults = true;
            UIPlatform.Reset();
        }

        private GameObject Spawn(string name, Transform parent = null) {

            GameObject go = new GameObject(name);

            if (parent != null) {
                go.transform.SetParent(parent, false);
            }
            else {
                spawned.Add(go);
            }

            return go;
        }

        private GameObject SpawnContent() {

            GameObject content = Spawn("b9-test-content");
            content.layer = 0;

            GameObject mesh = GameObject.CreatePrimitive(PrimitiveType.Cube);
            mesh.name = "mesh";
            mesh.transform.SetParent(content.transform, false);
            mesh.layer = 5;

            return content;
        }

        private static int CountStageObjects() {

            int n = 0;
            GameObject[] all = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include);

            for (int i = 0; i < all.Length; i++) {
                if (all[i].name.StartsWith("ui-render-stage-")) {
                    n++;
                }
            }

            return n;
        }

        private static UIRenderStageBinding.Options TestOptions() {

            UIRenderStageBinding.Options o = new UIRenderStageBinding.Options();
            o.layer = stageLayer;
            o.size = 192;
            o.lightIntensity = 0f;   // borrow: no light object, nothing else to clean

            return o;
        }

        [Test]
        public void BindThenUnbind_LeavesNoStageObjects_AndRestoresLayers() {

            int before = CountStageObjects();

            GameObject view = Spawn("b9-test-view");
            GameObject element = Spawn("CharacterStage", view.transform);
            GameObject content = SpawnContent();
            Transform mesh = content.transform.Find("mesh");

            UIRenderStageBinding binding = UIRenderStageBinding.Bind(
                content, UIRef.Of(view), "CharacterStage", TestOptions());

            Assert.IsNotNull(binding, "bind");
            Assert.AreEqual(before + 1, CountStageObjects(), "one stage object while bound");
            Assert.AreEqual(stageLayer, content.layer, "root flipped");
            Assert.AreEqual(stageLayer, mesh.gameObject.layer, "child flipped");
            Assert.AreEqual(192, binding.size, "no geometry backend -> caller size");
            Assert.AreEqual(192, binding.stage.texture.width);
            Assert.AreSame(element, binding.element.gameObject);

            binding.Unbind();

            Assert.AreEqual(before, CountStageObjects(), "no leftover ui-render-stage-* objects");
            Assert.AreEqual(0, content.layer, "root layer restored");
            Assert.AreEqual(5, mesh.gameObject.layer, "child layer restored");

            // Idempotent.
            binding.Unbind();
        }

        [Test]
        public void Visibility_FollowsContentElementAndView() {

            GameObject view = Spawn("b9-test-view");
            GameObject element = Spawn("CharacterStage", view.transform);
            GameObject content = SpawnContent();

            UIRenderStageBinding binding = UIRenderStageBinding.Bind(
                content, UIRef.Of(view), "CharacterStage", TestOptions());

            Camera cam = binding.stage.stageCamera;

            Assert.IsTrue(cam.enabled, "all visible");

            content.SetActive(false);
            binding.Refresh();
            Assert.IsFalse(cam.enabled, "content inactive");

            content.SetActive(true);
            binding.Refresh();
            Assert.IsTrue(cam.enabled, "content back");

            element.SetActive(false);
            binding.Refresh();
            Assert.IsFalse(cam.enabled, "element hidden");

            element.SetActive(true);
            view.SetActive(false);
            binding.Refresh();
            Assert.IsFalse(cam.enabled, "view hidden");

            view.SetActive(true);
            binding.Refresh();
            Assert.IsTrue(cam.enabled, "view back");

            // Element destroyed (the GameObject analogue of a recycled toolkit element).
            Object.DestroyImmediate(element);
            binding.Refresh();
            Assert.IsFalse(cam.enabled, "element dead");

            binding.Unbind();
        }

        [Test]
        public void Bind_MissingElement_StagesNothing() {

            int before = CountStageObjects();

            GameObject view = Spawn("b9-test-view");
            GameObject content = SpawnContent();

            UIRenderStageBinding binding = UIRenderStageBinding.Bind(
                content, UIRef.Of(view), "NoSuchElement", TestOptions());

            Assert.IsNull(binding);
            Assert.AreEqual(before, CountStageObjects());
            Assert.AreEqual(0, content.layer, "layers untouched");
        }

        [Test]
        public void ResolveSize_RoundsUpTo64_AndClamps() {

            Assert.AreEqual(256, UIRenderStageBinding.ResolveSize(Vector2.zero, 256, 128, 512));
            Assert.AreEqual(128, UIRenderStageBinding.ResolveSize(new Vector2(40, 20), 256, 128, 512));
            Assert.AreEqual(192, UIRenderStageBinding.ResolveSize(new Vector2(129, 60), 256, 128, 512));
            Assert.AreEqual(320, UIRenderStageBinding.ResolveSize(new Vector2(100, 300), 256, 128, 512));
            Assert.AreEqual(512, UIRenderStageBinding.ResolveSize(new Vector2(816, 514), 256, 128, 512));
            Assert.AreEqual(256,
                UIRenderStageBinding.ResolveSize(new Vector2(float.NaN, float.NaN), 256, 128, 512));
        }

        [Test]
        public void ToolkitGeometry_DetachedElement_IsNotAlive() {

            Vector2 px;
            VisualElement detached = new VisualElement();

            Assert.IsFalse(UIToolkitBackend.Instance.TryGetElementPixelSize(
                UIRef.Of(detached, "detached"), out px));
            Assert.AreEqual(Vector2.zero, px);
        }
    }
}
