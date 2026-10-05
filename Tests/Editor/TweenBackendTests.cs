using System.Collections.Generic;

using NUnit.Framework;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

using Image = UnityEngine.UI.Image;
using Object = UnityEngine.Object;

using Engine.Animation;
using Engine.Utility;

namespace Engine.Animation.Tests {

    public class TweenBackendTests {

        private List<GameObject> spawned = new List<GameObject>();

        [TearDown]
        public void TearDown() {

            AnimationEasing.Instance.getAnimationItems().Clear();
            AnimationEasing.Instance.queueRemove.Clear();

            for (int i = 0; i < spawned.Count; i++) {
                if (spawned[i]) {
                    Object.DestroyImmediate(spawned[i]);
                }
            }

            spawned.Clear();
        }

        private GameObject NewGO(string name) {
            GameObject go = new GameObject(name);
            spawned.Add(go);
            return go;
        }

        // 1. Equation correctness: mapped eases match the Penner closed form.

        [Test]
        public void EquationValue_MatchesPennerClosedForm_ForMappedEases() {

            double[] samples = new double[] { 0.0, .25, .5, .75, 1.0 };

            for (int i = 0; i < samples.Length; i++) {
                double t = samples[i];

                Assert.AreEqual(
                    AnimationEasing.Linear(t, 0, 10, 1),
                    AnimationEasing.EquationValue(AnimationEasing.Equations.Linear, t, 0, 10, 1),
                    0.0000001);

                Assert.AreEqual(
                    AnimationEasing.QuadEaseIn(t, 0, 10, 1),
                    AnimationEasing.EquationValue(AnimationEasing.Equations.QuadEaseIn, t, 0, 10, 1),
                    0.0000001);

                Assert.AreEqual(
                    AnimationEasing.QuadEaseOut(t, 0, 10, 1),
                    AnimationEasing.EquationValue(AnimationEasing.Equations.QuadEaseOut, t, 0, 10, 1),
                    0.0000001);

                Assert.AreEqual(
                    AnimationEasing.QuadEaseInOut(t, 0, 10, 1),
                    AnimationEasing.EquationValue(AnimationEasing.Equations.QuadEaseInOut, t, 0, 10, 1),
                    0.0000001);
            }
        }

        // 2. Delay: no value change and no onStart before delay elapses.

        [Test]
        public void EaseUpdate_HonorsTimeDelay() {

            int startCount = 0;

            AnimationEasing.AnimationItem item = new AnimationEasing.AnimationItem();
            item.equationType = AnimationEasing.Equations.Linear;
            item.valStart = 0;
            item.valEnd = 10;
            item.timeDuration = 1;
            item.timeDelay = 1;
            item.onStart = () => {
                startCount++;
            };

            AnimationEasing.Instance.easeUpdate(item, 10.0);
            Assert.AreEqual(0.0, item.val, 0.0001);
            Assert.AreEqual(0, startCount);

            AnimationEasing.Instance.easeUpdate(item, 10.5);
            Assert.AreEqual(0.0, item.val, 0.0001);
            Assert.AreEqual(0, startCount);

            AnimationEasing.Instance.easeUpdate(item, 11.0);
            Assert.AreEqual(1, startCount);
        }

        // 3. Loop/pingPong: value at 1.5x duration matches restart/reverse expectations.

        [Test]
        public void EaseUpdate_Loop_RestartsFromValStart() {

            AnimationEasing.AnimationItem item = new AnimationEasing.AnimationItem();
            item.equationType = AnimationEasing.Equations.Linear;
            item.valStart = 0;
            item.valEnd = 10;
            item.timeDuration = 1;
            item.timeDelay = 0;
            item.loopType = TweenLoopType.loop;

            AnimationEasing.Instance.easeUpdate(item, 10.0);
            AnimationEasing.Instance.easeUpdate(item, 11.25);

            Assert.AreEqual(2.5, item.val, 0.001);
        }

        [Test]
        public void EaseUpdate_PingPong_ReversesOnRestart() {

            AnimationEasing.AnimationItem item = new AnimationEasing.AnimationItem();
            item.equationType = AnimationEasing.Equations.Linear;
            item.valStart = 0;
            item.valEnd = 10;
            item.timeDuration = 1;
            item.timeDelay = 0;
            item.loopType = TweenLoopType.pingPong;

            AnimationEasing.Instance.easeUpdate(item, 10.0);
            AnimationEasing.Instance.easeUpdate(item, 11.25);

            Assert.AreEqual(7.5, item.val, 0.001);
        }

        // 4. Callback order and counts; end value snapped exactly to target (once-loop).

        [Test]
        public void EaseUpdate_Once_FiresCallbacksInOrder_AndSnapsEndValue() {

            int startCount = 0;
            int updateCount = 0;
            int completeCount = 0;

            AnimationEasing.AnimationItem item = new AnimationEasing.AnimationItem();
            item.equationType = AnimationEasing.Equations.Linear;
            item.valStart = 0;
            item.valEnd = 5;
            item.timeDuration = 1;
            item.timeDelay = 0;
            item.loopType = TweenLoopType.once;
            item.onStart = () => {
                startCount++;
            };
            item.onUpdate = (val) => {
                updateCount++;
            };
            item.onComplete = () => {
                completeCount++;
            };

            AnimationEasing.Instance.easeUpdate(item, 10.0);
            AnimationEasing.Instance.easeUpdate(item, 10.3);
            AnimationEasing.Instance.easeUpdate(item, 10.6);
            AnimationEasing.Instance.easeUpdate(item, 11.2);

            Assert.AreEqual(1, startCount);
            Assert.GreaterOrEqual(updateCount, 3);
            Assert.AreEqual(1, completeCount);
            Assert.AreEqual(5.0, item.val, 0.0000001);
        }

        // 5. Cancel: per-target cancel stops all channels, key cancel stops Value tween,
        // CancelAll empties; canceled tweens fire no further callbacks.

        [Test]
        public void Cancel_Target_RemovesAllChannels_AndStopsFurtherCallbacks() {

            GameObject go = NewGO("cancelTarget");
            ITweenTarget target = new TransformTweenTarget(go);

            TweenMeta moveMeta = new TweenMeta();
            moveMeta.time = 1f;
            moveMeta.delay = 0f;

            int updateCount = 0;
            moveMeta.onUpdate = () => {
                updateCount++;
            };

            EasingTweenBackend.Instance.Move(target, new Vector3(10, 0, 0), moveMeta);
            EasingTweenBackend.Instance.Fade(target, 0.5f, moveMeta);

            Assert.IsTrue(EasingTweenBackend.Instance.IsTweening(target));

            string positionKey = go.GetEntityId().ToString() + ":" + TweenChannel.position;
            AnimationEasing.AnimationItem item = AnimationEasing.EaseGet(positionKey);
            AnimationEasing.Instance.easeUpdate(item, item.timeStart + 0.1);

            Assert.Greater(updateCount, 0);

            EasingTweenBackend.Instance.Cancel(target);

            Assert.IsFalse(EasingTweenBackend.Instance.IsTweening(target));

            int countBefore = updateCount;
            AnimationEasing.Instance.Update();
            Assert.AreEqual(countBefore, updateCount);
        }

        [Test]
        public void Cancel_Key_StopsValueTween() {

            string key = "test-value-key";
            int updateCount = 0;

            TweenMeta meta = new TweenMeta();
            meta.time = 1f;
            meta.delay = 0f;

            EasingTweenBackend.Instance.Value(key, 0f, 1f, meta, (v) => {
                updateCount++;
            });

            Assert.IsTrue(AnimationEasing.EaseExists(key));

            AnimationEasing.AnimationItem item = AnimationEasing.EaseGet(key);
            AnimationEasing.Instance.easeUpdate(item, item.timeStart + 0.1);

            Assert.Greater(updateCount, 0);

            EasingTweenBackend.Instance.Cancel(key);

            Assert.IsFalse(AnimationEasing.EaseExists(key));

            int countBefore = updateCount;
            AnimationEasing.Instance.Update();
            Assert.AreEqual(countBefore, updateCount);
        }

        [Test]
        public void CancelAll_EmptiesAllItems() {

            GameObject go = NewGO("cancelAllTarget");
            ITweenTarget target = new TransformTweenTarget(go);

            TweenMeta meta = new TweenMeta();
            meta.time = 1f;
            meta.delay = 0f;

            EasingTweenBackend.Instance.Move(target, new Vector3(1, 1, 1), meta);
            EasingTweenBackend.Instance.Value("some-key", 0f, 1f, meta, (v) => { });

            Assert.Greater(AnimationEasing.Instance.getAnimationItems().Count, 0);

            EasingTweenBackend.Instance.CancelAll();

            Assert.AreEqual(0, AnimationEasing.Instance.getAnimationItems().Count);
        }

        // 6. TransformTweenTarget: pos/scale/rot local+world round-trips; alpha on
        // CanvasGroup and UGUI Image.

        [Test]
        public void TransformTweenTarget_PositionScaleRotation_RoundTrip() {

            GameObject parent = NewGO("parent");
            GameObject child = NewGO("child");
            child.transform.SetParent(parent.transform, false);
            parent.transform.position = new Vector3(5, 0, 0);

            ITweenTarget target = new TransformTweenTarget(child);

            target.SetPosition(new Vector3(1, 2, 3), TweenCoord.local);
            Assert.AreEqual(new Vector3(1, 2, 3), target.GetPosition(TweenCoord.local));
            Assert.AreEqual(new Vector3(6, 2, 3), target.GetPosition(TweenCoord.world));

            target.SetPosition(new Vector3(10, 0, 0), TweenCoord.world);
            Assert.AreEqual(new Vector3(10, 0, 0), target.GetPosition(TweenCoord.world));

            target.SetScale(new Vector3(2, 2, 2));
            Assert.AreEqual(new Vector3(2, 2, 2), target.GetScale());

            target.SetRotation(new Vector3(0, 90, 0), TweenCoord.local);
            Vector3 rot = target.GetRotation(TweenCoord.local);
            Assert.AreEqual(90f, rot.y, 0.01f);
        }

        [Test]
        public void TransformTweenTarget_Alpha_CanvasGroup() {

            GameObject go = NewGO("canvasGroupTarget");
            CanvasGroup group = go.AddComponent<CanvasGroup>();
            group.alpha = 1f;

            ITweenTarget target = new TransformTweenTarget(go);

            target.SetAlpha(0.35f);
            Assert.AreEqual(0.35f, target.GetAlpha(), 0.0001f);
            Assert.AreEqual(0.35f, group.alpha, 0.0001f);
        }

        [Test]
        public void TransformTweenTarget_Alpha_UguiImage() {

            GameObject go = new GameObject("imageTarget", typeof(RectTransform), typeof(Image));
            spawned.Add(go);

            Image image = go.GetComponent<Image>();
            image.color = Color.white;

            ITweenTarget target = new TransformTweenTarget(go);

            target.SetAlpha(0.6f);
            Assert.AreEqual(0.6f, target.GetAlpha(), 0.0001f);
            Assert.AreEqual(0.6f, image.color.a, 0.0001f);
        }

        // 7. VisualElementTweenTarget: translate/opacity/scale on a detached VisualElement.

        [Test]
        public void VisualElementTweenTarget_TranslateOpacityScale_RoundTrip() {

            VisualElement element = new VisualElement();
            ITweenTarget target = new VisualElementTweenTarget(element);

            target.SetPosition(new Vector3(12, 34, 0), TweenCoord.world);
            Vector3 pos = target.GetPosition(TweenCoord.world);
            Assert.AreEqual(12f, pos.x, 0.01f);
            Assert.AreEqual(34f, pos.y, 0.01f);

            target.SetAlpha(0.42f);
            Assert.AreEqual(0.42f, target.GetAlpha(), 0.001f);

            target.SetScale(new Vector3(2f, 3f, 1f));
            Vector3 scale = target.GetScale();
            Assert.AreEqual(2f, scale.x, 0.01f);
            Assert.AreEqual(3f, scale.y, 0.01f);
        }

        // 8. "PlayMode smoke" reimplemented as EditMode: pump the backend manually via
        // the explicit-time API instead of waiting on real frames. Calling
        // TweenUtil.MoveToObject(TweenLib.internalEasing, ...) directly is not usable
        // here since the forced-NGUI override in MoveToObject(meta,...) is untouched
        // (out of scope for 1.2) and always wins under this project's active
        // USE_UI_NGUI_2_7 define, so this drives TweenUtil.backend/ResolveTarget
        // directly to validate the internalEasing wiring instead.
        [Test]
        public void Backend_MoveToObject_MovesGameObject_AcrossPumpedFrames() {

            GameObject go = NewGO("smokeTarget");
            go.transform.position = Vector3.zero;

            Vector3 to = new Vector3(10, 0, 0);

            TweenMeta meta = new TweenMeta();
            meta.time = 1f;
            meta.delay = 0f;
            meta.coord = TweenCoord.world;

            TweenUtil.backend.Move(TweenUtil.ResolveTarget(go), to, meta);

            string key = go.GetEntityId().ToString() + ":" + TweenChannel.position;
            AnimationEasing.AnimationItem item = AnimationEasing.EaseGet(key);
            Assert.IsNotNull(item);

            AnimationEasing.Instance.easeUpdate(item, item.timeStart + 0.3);
            AnimationEasing.Instance.easeUpdate(item, item.timeStart + 0.6);
            AnimationEasing.Instance.easeUpdate(item, item.timeStart + 1.0);

            Assert.AreEqual(10f, go.transform.position.x, 0.001f);
        }
    }
}
