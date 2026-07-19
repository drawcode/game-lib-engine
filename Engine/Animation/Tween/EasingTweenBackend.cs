using System;

using UnityEngine;

using Engine.Utility;

namespace Engine.Animation {

    // Default ITweenBackend (TweenLib.internalEasing). Composes AnimationEasing:
    // every op is a single normalized [0,1] AnimationItem whose onUpdate applies
    // LerpUnclamped(from, to, val) through the ITweenTarget setter, so the eased
    // end value is always exactly `to` when the item completes at val == 1.
    public class EasingTweenBackend : ITweenBackend {

        private static EasingTweenBackend _instance = null;

        public static EasingTweenBackend Instance {
            get {
                if (_instance == null) {
                    _instance = new EasingTweenBackend();
                }

                return _instance;
            }
        }

        private static readonly TweenChannel[] targetChannels = new TweenChannel[] {
            TweenChannel.position,
            TweenChannel.scale,
            TweenChannel.rotation,
            TweenChannel.alpha,
            TweenChannel.color
        };

        public void Move(ITweenTarget t, Vector3 to, TweenMeta meta) {

            if (t == null || meta == null) {
                return;
            }

            Vector3 from = t.GetPosition(meta.coord);
            TweenCoord coord = meta.coord;

            StartVectorTween(t, TweenChannel.position, from, to, meta, (v) => {
                t.SetPosition(v, coord);
            });
        }

        public void Scale(ITweenTarget t, Vector3 to, TweenMeta meta) {

            if (t == null || meta == null) {
                return;
            }

            Vector3 from = t.GetScale();

            StartVectorTween(t, TweenChannel.scale, from, to, meta, (v) => {
                t.SetScale(v);
            });
        }

        public void Rotate(ITweenTarget t, Vector3 to, TweenMeta meta) {

            if (t == null || meta == null) {
                return;
            }

            Vector3 from = t.GetRotation(meta.coord);
            TweenCoord coord = meta.coord;

            StartVectorTween(t, TweenChannel.rotation, from, to, meta, (v) => {
                t.SetRotation(v, coord);
            });
        }

        public void Fade(ITweenTarget t, float to, TweenMeta meta) {

            if (t == null || meta == null) {
                return;
            }

            if (meta.stopCurrent) {
                Cancel(t, TweenChannel.alpha);
            }

            float from = t.GetAlpha();

            AnimationEasing.AnimationItem item = new AnimationEasing.AnimationItem();
            item.key = BuildKey(t, TweenChannel.alpha);
            item.equationType = ToEquation(meta.easeType);
            item.loopType = meta.loopType;
            item.valStart = 0.0;
            item.valEnd = 1.0;
            item.timeDuration = meta.time;
            item.timeDelay = meta.delay;

            Action metaOnUpdate = meta.onUpdate;

            item.onStart = ComposeOnStart(meta);
            item.onComplete = ComposeOnComplete(meta);
            item.onUpdate = (val) => {

                if (!t.alive) {
                    AnimationEasing.EaseRemove(item.key);
                    return;
                }

                t.SetAlpha(Mathf.LerpUnclamped(from, to, (float)val));

                if (metaOnUpdate != null) {
                    metaOnUpdate();
                }
            };

            AnimationEasing.EaseAdd(item);
        }

        public void ColorTo(ITweenTarget t, Color to, TweenMeta meta) {

            if (t == null || meta == null) {
                return;
            }

            if (meta.stopCurrent) {
                Cancel(t, TweenChannel.color);
            }

            Color from = t.GetColor();

            AnimationEasing.AnimationItem item = new AnimationEasing.AnimationItem();
            item.key = BuildKey(t, TweenChannel.color);
            item.equationType = ToEquation(meta.easeType);
            item.loopType = meta.loopType;
            item.valStart = 0.0;
            item.valEnd = 1.0;
            item.timeDuration = meta.time;
            item.timeDelay = meta.delay;

            Action metaOnUpdate = meta.onUpdate;

            item.onStart = ComposeOnStart(meta);
            item.onComplete = ComposeOnComplete(meta);
            item.onUpdate = (val) => {

                if (!t.alive) {
                    AnimationEasing.EaseRemove(item.key);
                    return;
                }

                t.SetColor(Color.LerpUnclamped(from, to, (float)val));

                if (metaOnUpdate != null) {
                    metaOnUpdate();
                }
            };

            AnimationEasing.EaseAdd(item);
        }

        public void Value(string key, float from, float to, TweenMeta meta, Action<float> onValue) {

            if (string.IsNullOrEmpty(key) || meta == null) {
                return;
            }

            if (meta.stopCurrent) {
                Cancel(key);
            }

            AnimationEasing.AnimationItem item = new AnimationEasing.AnimationItem();
            item.key = key;
            item.equationType = ToEquation(meta.easeType);
            item.loopType = meta.loopType;
            item.valStart = 0.0;
            item.valEnd = 1.0;
            item.timeDuration = meta.time;
            item.timeDelay = meta.delay;

            Action metaOnUpdate = meta.onUpdate;

            item.onStart = ComposeOnStart(meta);
            item.onComplete = ComposeOnComplete(meta);
            item.onUpdate = (val) => {

                if (onValue != null) {
                    onValue(Mathf.LerpUnclamped(from, to, (float)val));
                }

                if (metaOnUpdate != null) {
                    metaOnUpdate();
                }
            };

            AnimationEasing.EaseAdd(item);
        }

        public void Cancel(ITweenTarget t) {

            if (t == null) {
                return;
            }

            for (int i = 0; i < targetChannels.Length; i++) {
                AnimationEasing.EaseRemove(BuildKey(t, targetChannels[i]));
            }
        }

        public void Cancel(ITweenTarget t, TweenChannel channel) {

            if (t == null) {
                return;
            }

            AnimationEasing.EaseRemove(BuildKey(t, channel));
        }

        public void Cancel(string key) {

            if (string.IsNullOrEmpty(key)) {
                return;
            }

            AnimationEasing.EaseRemove(key);
        }

        public void CancelAll() {

            AnimationEasing inst = AnimationEasing.Instance;

            if (inst == null) {
                return;
            }

            inst.getAnimationItems().Clear();
            inst.queueRemove.Clear();
        }

        public bool IsTweening(ITweenTarget t) {

            if (t == null) {
                return false;
            }

            for (int i = 0; i < targetChannels.Length; i++) {
                if (AnimationEasing.EaseExists(BuildKey(t, targetChannels[i]))) {
                    return true;
                }
            }

            return false;
        }

        // --------------------------------------------------------------------

        private void StartVectorTween(
            ITweenTarget t, TweenChannel channel, Vector3 from, Vector3 to, TweenMeta meta, Action<Vector3> apply) {

            // Same-channel only: NGUI tweeners were isolated per component, so a
            // fade never killed an in-flight move. Cross-channel cancel scrambles
            // the boot-time Show/Hide races the legacy choreography relies on.
            if (meta.stopCurrent) {
                Cancel(t, channel);
            }

            AnimationEasing.AnimationItem item = new AnimationEasing.AnimationItem();
            item.key = BuildKey(t, channel);
            item.equationType = ToEquation(meta.easeType);
            item.loopType = meta.loopType;
            item.valStart = 0.0;
            item.valEnd = 1.0;
            item.timeDuration = meta.time;
            item.timeDelay = meta.delay;

            Action metaOnUpdate = meta.onUpdate;

            item.onStart = ComposeOnStart(meta);
            item.onComplete = ComposeOnComplete(meta);
            item.onUpdate = (val) => {

                if (!t.alive) {
                    AnimationEasing.EaseRemove(item.key);
                    return;
                }

                apply(Vector3.LerpUnclamped(from, to, (float)val));

                if (metaOnUpdate != null) {
                    metaOnUpdate();
                }
            };

            AnimationEasing.EaseAdd(item);
        }

        private static string BuildKey(ITweenTarget t, TweenChannel channel) {
            return t.targetId + ":" + channel;
        }

        private static AnimationEasing.Equations ToEquation(TweenEaseType easeType) {

            if (easeType == TweenEaseType.linear) {
                return AnimationEasing.Equations.Linear;
            }
            else if (easeType == TweenEaseType.quadEaseIn) {
                return AnimationEasing.Equations.QuadEaseIn;
            }
            else if (easeType == TweenEaseType.quadEaseOut) {
                return AnimationEasing.Equations.QuadEaseOut;
            }

            return AnimationEasing.Equations.QuadEaseInOut;
        }

        private static Action ComposeOnStart(TweenMeta meta) {

            Action onBegin = () => {

            };

            if (meta.onStart != null) {
                onBegin = onBegin.CombineAction(meta.onStart);
            }

            return onBegin;
        }

        private static Action ComposeOnComplete(TweenMeta meta) {

            Action onFinish = () => {

            };

            if (meta.onComplete != null) {
                onFinish = onFinish.CombineAction(meta.onComplete);
            }

            if (meta.onFinal != null) {
                onFinish = onFinish.CombineAction(meta.onFinal);
            }

            return onFinish;
        }
    }
}
