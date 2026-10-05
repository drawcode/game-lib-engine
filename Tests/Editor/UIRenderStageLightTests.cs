using System.Collections.Generic;

using NUnit.Framework;

using UnityEngine;

using Object = UnityEngine.Object;

using Engine.UI;

namespace Engine.UI.Tests {

    // UIRenderStage LIGHTS (iter 36): one shared light per layer, MAX of the visible stages'
    // requests (never the sum), the oldest owner's when only borrowers are visible, off when no
    // stage is visible, gone with the last stage. Headless: stages are attached to a plain cube.
    public class UIRenderStageLightTests {

        private const int stageLayer = 31;

        private readonly List<GameObject> spawned = new List<GameObject>();
        private readonly List<UIRenderStage> stages = new List<UIRenderStage>();

        [TearDown]
        public void TearDown() {

            for (int i = 0; i < stages.Count; i++) {
                if (stages[i]) {
                    stages[i].Detach();
                }
            }

            stages.Clear();

            for (int i = 0; i < spawned.Count; i++) {
                if (spawned[i]) {
                    Object.DestroyImmediate(spawned[i]);
                }
            }

            spawned.Clear();
        }

        private UIRenderStage Attach(float lightIntensity) {

            GameObject mesh = GameObject.CreatePrimitive(PrimitiveType.Cube);
            spawned.Add(mesh);

            UIRenderStage stage = UIRenderStage.Attach(mesh, stageLayer, 64, 1.15f, false, false, lightIntensity);
            stages.Add(stage);

            return stage;
        }

        private static float Intensity() {

            Light light = UIRenderStage.LayerLight(stageLayer);

            return light != null && light.enabled ? light.intensity : 0f;
        }

        [Test]
        public void VisibleOwners_TakeTheMax_NotTheSum() {

            Attach(0.97f);
            Attach(1.1f);
            Attach(0.7f);

            Assert.AreEqual(1.1f, Intensity(), 1e-5f);
            Assert.AreEqual(stageLayer, LayerOf(UIRenderStage.LayerLight(stageLayer)));
        }

        [Test]
        public void HiddenOwner_StopsLighting() {

            UIRenderStage coin = Attach(0.97f);
            UIRenderStage bot = Attach(1.1f);

            bot.SetVisible(false);
            Assert.AreEqual(0.97f, Intensity(), 1e-5f, "hidden bot no longer stacks on the coin");

            coin.SetVisible(false);
            Assert.AreEqual(0f, Intensity(), "nothing visible -> light off");

            bot.SetVisible(true);
            Assert.AreEqual(1.1f, Intensity(), 1e-5f);
        }

        [Test]
        public void OnlyBorrowersVisible_InheritTheOldestOwner() {

            UIRenderStage header = Attach(0.97f);
            UIRenderStage bot = Attach(1.1f);
            Attach(0f);

            header.SetVisible(false);
            bot.SetVisible(false);

            Assert.AreEqual(0.97f, Intensity(), 1e-5f);
        }

        [Test]
        public void OnlyBorrowers_NoLight_AndLastDetachRemovesIt() {

            Attach(0f);
            Assert.IsNull(UIRenderStage.LayerLight(stageLayer), "a borrower never creates a light");

            UIRenderStage owner = Attach(0.97f);
            Assert.IsNotNull(UIRenderStage.LayerLight(stageLayer));

            for (int i = 0; i < stages.Count; i++) {
                stages[i].Detach();
            }

            stages.Clear();

            Assert.IsNull(UIRenderStage.LayerLight(stageLayer), "light goes with the last stage");
            Assert.IsTrue(owner == null);
        }

        private static int LayerOf(Light light) {

            for (int i = 0; i < 32; i++) {
                if (light.cullingMask == 1 << i) {
                    return i;
                }
            }

            return -1;
        }
    }
}
