using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text;

using UnityEngine;

using Engine.Utility;

namespace Engine.Animation {
    public class Ani : AnimationEasing {

        /*
        public static void To(AnimationItem aniItem) {

        }

        public static void To(string key, string to, Equations type, double time, double delay) {
            EaseAdd(key, type, 
        }
        */
    }

    public class AnimationEasing : MonoBehaviour {

        // Only one BroadcastNetworks can exist. We use a singleton pattern to enforce this.
        private static AnimationEasing _instance = null;

        public static AnimationEasing Instance {
            get {
                if (!_instance) {

                    // check if an ObjectPoolManager is already available in the scene graph
                    _instance = FindAnyObjectByType(typeof(AnimationEasing)) as AnimationEasing;

                    // nope, create a new one
                    if (!_instance) {
                        var obj = new GameObject("_AnimationEasing");
                        _instance = obj.AddComponent<AnimationEasing>();
                    }
                }

                return _instance;
            }
        }

        public enum Equations {
            Linear,
            QuadEaseOut,
            QuadEaseIn,
            QuadEaseInOut,
            QuadEaseOutIn,
            ExpoEaseOut,
            ExpoEaseIn,
            ExpoEaseInOut,
            ExpoEaseOutIn,
            CubicEaseOut,
            CubicEaseIn,
            CubicEaseInOut,
            CubicEaseOutIn,
            QuartEaseOut,
            QuartEaseIn,
            QuartEaseInOut,
            QuartEaseOutIn,
            QuintEaseOut,
            QuintEaseIn,
            QuintEaseInOut,
            QuintEaseOutIn,
            CircEaseOut,
            CircEaseIn,
            CircEaseInOut,
            CircEaseOutIn,
            SineEaseOut,
            SineEaseIn,
            SineEaseInOut,
            SineEaseOutIn,
            ElasticEaseOut,
            ElasticEaseIn,
            ElasticEaseInOut,
            ElasticEaseOutIn,
            BounceEaseOut,
            BounceEaseIn,
            BounceEaseInOut,
            BounceEaseOutIn,
            BackEaseOut,
            BackEaseIn,
            BackEaseInOut,
            BackEaseOutIn
        }

        public Dictionary<string, AnimationItem> animationItems = new Dictionary<string, AnimationItem>();
        public Queue<string> queueRemove = new Queue<string>();

        public class AnimationItem {
            public Equations equationType = Equations.QuadEaseInOut;
            public double val = 0;
            public double valStart = 0;
            public double valEnd = 0;
            public double timeStart = 0;
            public double timeDuration = 1.0;
            public double timeDelay = 1.0;
            public string key = "";

            public TweenLoopType loopType = TweenLoopType.once;

            public Action onStart = null;
            public Action<double> onUpdate = null;
            public Action onComplete = null;

            // First active (post-delay) tick fires onStart; tracked so it fires once only.
            private bool started = false;

            public AnimationItem() {
                Reset();
            }

            public void Reset() {
                equationType = Equations.QuadEaseInOut;
                val = 0f;
                valStart = 0f;
                valEnd = 1f;
                timeStart = 0f;
                timeDuration = 1.0f;
                timeDelay = 1.0f;
                key = System.Guid.NewGuid().ToString();
                loopType = TweenLoopType.once;
                onStart = null;
                onUpdate = null;
                onComplete = null;
                started = false;
            }

            public bool hasStarted {
                get {
                    return started;
                }
                set {
                    started = value;
                }
            }
        }

        private List<AnimationItem> updateBuffer = new List<AnimationItem>();

        public void Update() {

            while (queueRemove.Count > 0) {
                string key = queueRemove.Dequeue();
                easeRemove(key);
            }

            // Snapshot: onStart/onUpdate/onComplete fired inside easeUpdate may
            // add or cancel items, which would invalidate direct dict iteration.
            updateBuffer.Clear();

            foreach (KeyValuePair<string, AnimationItem> item in getAnimationItems()) {
                updateBuffer.Add(item.Value);
            }

            for (int i = 0; i < updateBuffer.Count; i++) {

                AnimationItem item = updateBuffer[i];

                // Skip items canceled or replaced by an earlier callback this frame:
                // easeUpdate would silently re-add them otherwise.
                AnimationItem current = easeGet(item.key);

                if (current == null || !object.ReferenceEquals(current, item)) {
                    continue;
                }

                easeUpdate(item);
            }
        }

        // GET

        public static bool EaseRemove(string key) {
            return Instance.easeRemove(key);
        }

        public bool easeRemove(string key) {

            if (getAnimationItems().ContainsKey(key)) {
                return animationItems.Remove(key);
            }

            return false;
        }

        //

        public static bool EaseExists(string key) {
            return Instance.easeExists(key);
        }

        public bool easeExists(string key) {

            if (getAnimationItems().ContainsKey(key)) {
                return true;
            }

            return false;
        }

        //

        public static Dictionary<string, AnimationItem> GetAnimationItems() {
            return Instance.getAnimationItems();
        }

        public Dictionary<string, AnimationItem> getAnimationItems() {

            if (animationItems == null) {
                animationItems = new Dictionary<string, AnimationItem>();
            }

            return animationItems;
        }

        public static double EaseGetValue(string key, double defaultValue) {
            return Instance.easeGetValue(key, defaultValue);
        }

        public double easeGetValue(string key, double defaultValue) {

            AnimationItem item = easeGet(key);

            if (item == null) {
                return defaultValue;
            }

            return easeGet(key).val;
        }

        public static AnimationItem EaseGet(string key) {
            return Instance.easeGet(key);
        }

        public AnimationItem easeGet(string key) {

            if (getAnimationItems().ContainsKey(key)) {
                return animationItems[key];
            }

            return null;
        }

        // UPDATE

        public static AnimationItem EaseUpdate(AnimationItem animationItem) {
            return Instance.easeUpdate(animationItem);
        }

        public AnimationItem easeUpdate(AnimationItem animationItem) {
            return easeUpdate(animationItem, Time.time);
        }

        // Explicit-time overload: lets callers (tests, backends) drive the
        // animation without waiting on real frames / Time.time.
        public static AnimationItem EaseUpdate(AnimationItem animationItem, double now) {
            return Instance.easeUpdate(animationItem, now);
        }

        public AnimationItem easeUpdate(AnimationItem animationItem, double now) {

            if (animationItem == null) {
                return null;
            }

            if (!animationItems.ContainsKey(animationItem.key)) {
                easeAdd(animationItem);
            }

            if (animationItem.timeStart == 0) {
                animationItem.timeStart = now;
            }

            double elapsed = now - animationItem.timeStart;

            if (elapsed < animationItem.timeDelay) {
                animationItem.val = animationItem.valStart;
                return animationItem;
            }

            double t = elapsed - animationItem.timeDelay;

            // Loop/pingPong: walk past every fully-elapsed cycle, restarting timeStart
            // (and swapping valStart/valEnd for pingPong/bounce) so the leftover time
            // carries into the next cycle instead of being lost.
            while (animationItem.timeDuration > 0
                && t >= animationItem.timeDuration
                && animationItem.loopType != TweenLoopType.once) {

                t -= animationItem.timeDuration;

                if (animationItem.loopType == TweenLoopType.pingPong
                    || animationItem.loopType == TweenLoopType.bounce) {

                    double swap = animationItem.valStart;
                    animationItem.valStart = animationItem.valEnd;
                    animationItem.valEnd = swap;
                }

                animationItem.timeStart = now - animationItem.timeDelay - t;
            }

            bool completed = false;

            if (animationItem.loopType == TweenLoopType.once && t >= animationItem.timeDuration) {
                t = animationItem.timeDuration;
                completed = true;
            }

            double valTo = EquationValue(
                animationItem.equationType,
                t,
                animationItem.valStart,
                animationItem.valEnd - animationItem.valStart,
                animationItem.timeDuration);

            if (completed) {
                valTo = animationItem.valEnd;
            }

            animationItem.val = valTo;

            if (!animationItem.hasStarted) {
                animationItem.hasStarted = true;

                if (animationItem.onStart != null) {
                    animationItem.onStart();
                }
            }

            if (animationItem.onUpdate != null) {
                animationItem.onUpdate(valTo);
            }

            if (completed) {

                // Remove before onComplete so a callback that re-adds the same key
                // (chained/restarted tweens) isn't deleted by a deferred removal.
                easeRemove(animationItem.key);

                if (animationItem.onComplete != null) {
                    animationItem.onComplete();
                }
            }

            return animationItem;
        }

        // Equation dispatch by equationType, calling the existing static Penner
        // methods directly (no reflection).
        public static double EquationValue(Equations equationType, double t, double b, double c, double d) {

            switch (equationType) {
                case Equations.Linear: return Linear(t, b, c, d);
                case Equations.QuadEaseOut: return QuadEaseOut(t, b, c, d);
                case Equations.QuadEaseIn: return QuadEaseIn(t, b, c, d);
                case Equations.QuadEaseInOut: return QuadEaseInOut(t, b, c, d);
                case Equations.QuadEaseOutIn: return QuadEaseOutIn(t, b, c, d);
                case Equations.ExpoEaseOut: return ExpoEaseOut(t, b, c, d);
                case Equations.ExpoEaseIn: return ExpoEaseIn(t, b, c, d);
                case Equations.ExpoEaseInOut: return ExpoEaseInOut(t, b, c, d);
                case Equations.ExpoEaseOutIn: return ExpoEaseOutIn(t, b, c, d);
                case Equations.CubicEaseOut: return CubicEaseOut(t, b, c, d);
                case Equations.CubicEaseIn: return CubicEaseIn(t, b, c, d);
                case Equations.CubicEaseInOut: return CubicEaseInOut(t, b, c, d);
                case Equations.CubicEaseOutIn: return CubicEaseOutIn(t, b, c, d);
                case Equations.QuartEaseOut: return QuartEaseOut(t, b, c, d);
                case Equations.QuartEaseIn: return QuartEaseIn(t, b, c, d);
                case Equations.QuartEaseInOut: return QuartEaseInOut(t, b, c, d);
                case Equations.QuartEaseOutIn: return QuartEaseOutIn(t, b, c, d);
                case Equations.QuintEaseOut: return QuintEaseOut(t, b, c, d);
                case Equations.QuintEaseIn: return QuintEaseIn(t, b, c, d);
                case Equations.QuintEaseInOut: return QuintEaseInOut(t, b, c, d);
                case Equations.QuintEaseOutIn: return QuintEaseOutIn(t, b, c, d);
                case Equations.CircEaseOut: return CircEaseOut(t, b, c, d);
                case Equations.CircEaseIn: return CircEaseIn(t, b, c, d);
                case Equations.CircEaseInOut: return CircEaseInOut(t, b, c, d);
                case Equations.CircEaseOutIn: return CircEaseOutIn(t, b, c, d);
                case Equations.SineEaseOut: return SineEaseOut(t, b, c, d);
                case Equations.SineEaseIn: return SineEaseIn(t, b, c, d);
                case Equations.SineEaseInOut: return SineEaseInOut(t, b, c, d);
                case Equations.SineEaseOutIn: return SineEaseOutIn(t, b, c, d);
                case Equations.ElasticEaseOut: return ElasticEaseOut(t, b, c, d);
                case Equations.ElasticEaseIn: return ElasticEaseIn(t, b, c, d);
                case Equations.ElasticEaseInOut: return ElasticEaseInOut(t, b, c, d);
                case Equations.ElasticEaseOutIn: return ElasticEaseOutIn(t, b, c, d);
                case Equations.BounceEaseOut: return BounceEaseOut(t, b, c, d);
                case Equations.BounceEaseIn: return BounceEaseIn(t, b, c, d);
                case Equations.BounceEaseInOut: return BounceEaseInOut(t, b, c, d);
                case Equations.BounceEaseOutIn: return BounceEaseOutIn(t, b, c, d);
                case Equations.BackEaseOut: return BackEaseOut(t, b, c, d);
                case Equations.BackEaseIn: return BackEaseIn(t, b, c, d);
                case Equations.BackEaseInOut: return BackEaseInOut(t, b, c, d);
                case Equations.BackEaseOutIn: return BackEaseOutIn(t, b, c, d);
                default: return QuadEaseInOut(t, b, c, d);
            }
        }

        // ADD

        //

        public static void EaseAdd(AnimationItem animationItem) {
            Instance.easeAdd(animationItem);
        }

        public void easeAdd(AnimationItem animationItem) {

            if (animationItem == null) {
                return;
            }

            string key = animationItem.key;

            if (animationItems.ContainsKey(key)) {
                animationItems[key] = animationItem;
            }
            else {
                animationItems.Add(key, animationItem);
            }
        }

        //

        public static void EaseAdd(
            string key,
            AnimationEasing.Equations equationType,
            double val,
            double valStart,
            double valEnd,
            double timeDuration,
            double timeDelay) {

            Instance.easeAdd(key, equationType, val, valStart, valEnd, timeDuration, timeDelay);
        }

        public void easeAdd(
            string key,
            AnimationEasing.Equations equationType,
            double val,
            double valStart,
            double valEnd,
            double timeDuration,
            double timeDelay) {

            AnimationItem animationItem = null;

            if (animationItems.ContainsKey(key)) {
                animationItem = animationItems[key];
            }
            else {
                animationItem = new AnimationItem();
            }

            animationItem.key = key;
            animationItem.val = val;
            animationItem.valStart = valStart;
            animationItem.valEnd = valEnd;
            animationItem.timeDuration = timeDuration;
            animationItem.equationType = equationType;
            animationItem.timeStart = Time.time;
            animationItem.timeDelay = timeDelay;

            //Debug.Log("easeAdd:" + " animationItem:" + animationItem.ToJson());

            easeAdd(animationItem);
        }

        #region Equations

        // These methods are all public to enable reflection in GetCurrentValueCore.

        #region Linear

        /// <summary>
        /// Easing equation function for a simple linear tweening, with no easing.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double Linear(double t, double b, double c, double d) {
            return c * t / d + b;
        }

        #endregion

        #region Expo

        /// <summary>
        /// Easing equation function for an exponential (2^t) easing out: 
        /// decelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double ExpoEaseOut(double t, double b, double c, double d) {
            return (t == d) ? b + c : c * (-Math.Pow(2, -10 * t / d) + 1) + b;
        }

        /// <summary>
        /// Easing equation function for an exponential (2^t) easing in: 
        /// accelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double ExpoEaseIn(double t, double b, double c, double d) {
            return (t == 0) ? b : c * Math.Pow(2, 10 * (t / d - 1)) + b;
        }

        /// <summary>
        /// Easing equation function for an exponential (2^t) easing in/out: 
        /// acceleration until halfway, then deceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double ExpoEaseInOut(double t, double b, double c, double d) {
            if (t == 0)
                return b;

            if (t == d)
                return b + c;

            if ((t /= d / 2) < 1)
                return c / 2 * Math.Pow(2, 10 * (t - 1)) + b;

            return c / 2 * (-Math.Pow(2, -10 * --t) + 2) + b;
        }

        /// <summary>
        /// Easing equation function for an exponential (2^t) easing out/in: 
        /// deceleration until halfway, then acceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double ExpoEaseOutIn(double t, double b, double c, double d) {
            if (t < d / 2)
                return ExpoEaseOut(t * 2, b, c / 2, d);

            return ExpoEaseIn((t * 2) - d, b + c / 2, c / 2, d);
        }

        #endregion

        #region Circular

        /// <summary>
        /// Easing equation function for a circular (sqrt(1-t^2)) easing out: 
        /// decelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double CircEaseOut(double t, double b, double c, double d) {
            return c * Math.Sqrt(1 - (t = t / d - 1) * t) + b;
        }

        /// <summary>
        /// Easing equation function for a circular (sqrt(1-t^2)) easing in: 
        /// accelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double CircEaseIn(double t, double b, double c, double d) {
            return -c * (Math.Sqrt(1 - (t /= d) * t) - 1) + b;
        }

        /// <summary>
        /// Easing equation function for a circular (sqrt(1-t^2)) easing in/out: 
        /// acceleration until halfway, then deceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double CircEaseInOut(double t, double b, double c, double d) {
            if ((t /= d / 2) < 1)
                return -c / 2 * (Math.Sqrt(1 - t * t) - 1) + b;

            return c / 2 * (Math.Sqrt(1 - (t -= 2) * t) + 1) + b;
        }

        /// <summary>
        /// Easing equation function for a circular (sqrt(1-t^2)) easing in/out: 
        /// acceleration until halfway, then deceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double CircEaseOutIn(double t, double b, double c, double d) {
            if (t < d / 2)
                return CircEaseOut(t * 2, b, c / 2, d);

            return CircEaseIn((t * 2) - d, b + c / 2, c / 2, d);
        }

        #endregion

        #region Quad

        /// <summary>
        /// Easing equation function for a quadratic (t^2) easing out: 
        /// decelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double QuadEaseOut(double t, double b, double c, double d) {
            return -c * (t /= d) * (t - 2) + b;
        }

        /// <summary>
        /// Easing equation function for a quadratic (t^2) easing in: 
        /// accelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double QuadEaseIn(double t, double b, double c, double d) {
            return c * (t /= d) * t + b;
        }

        /// <summary>
        /// Easing equation function for a quadratic (t^2) easing in/out: 
        /// acceleration until halfway, then deceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double QuadEaseInOut(double t, double b, double c, double d) {
            if ((t /= d / 2) < 1)
                return c / 2 * t * t + b;

            return -c / 2 * ((--t) * (t - 2) - 1) + b;
        }

        /// <summary>
        /// Easing equation function for a quadratic (t^2) easing out/in: 
        /// deceleration until halfway, then acceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double QuadEaseOutIn(double t, double b, double c, double d) {
            if (t < d / 2)
                return QuadEaseOut(t * 2, b, c / 2, d);

            return QuadEaseIn((t * 2) - d, b + c / 2, c / 2, d);
        }

        #endregion

        #region Sine

        /// <summary>
        /// Easing equation function for a sinusoidal (sin(t)) easing out: 
        /// decelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double SineEaseOut(double t, double b, double c, double d) {
            return c * Math.Sin(t / d * (Math.PI / 2)) + b;
        }

        /// <summary>
        /// Easing equation function for a sinusoidal (sin(t)) easing in: 
        /// accelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double SineEaseIn(double t, double b, double c, double d) {
            return -c * Math.Cos(t / d * (Math.PI / 2)) + c + b;
        }

        /// <summary>
        /// Easing equation function for a sinusoidal (sin(t)) easing in/out: 
        /// acceleration until halfway, then deceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double SineEaseInOut(double t, double b, double c, double d) {
            if ((t /= d / 2) < 1)
                return c / 2 * (Math.Sin(Math.PI * t / 2)) + b;

            return -c / 2 * (Math.Cos(Math.PI * --t / 2) - 2) + b;
        }

        /// <summary>
        /// Easing equation function for a sinusoidal (sin(t)) easing in/out: 
        /// deceleration until halfway, then acceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double SineEaseOutIn(double t, double b, double c, double d) {
            if (t < d / 2)
                return SineEaseOut(t * 2, b, c / 2, d);

            return SineEaseIn((t * 2) - d, b + c / 2, c / 2, d);
        }

        #endregion

        #region Cubic

        /// <summary>
        /// Easing equation function for a cubic (t^3) easing out: 
        /// decelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double CubicEaseOut(double t, double b, double c, double d) {
            return c * ((t = t / d - 1) * t * t + 1) + b;
        }

        /// <summary>
        /// Easing equation function for a cubic (t^3) easing in: 
        /// accelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double CubicEaseIn(double t, double b, double c, double d) {
            return c * (t /= d) * t * t + b;
        }

        /// <summary>
        /// Easing equation function for a cubic (t^3) easing in/out: 
        /// acceleration until halfway, then deceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double CubicEaseInOut(double t, double b, double c, double d) {
            if ((t /= d / 2) < 1)
                return c / 2 * t * t * t + b;

            return c / 2 * ((t -= 2) * t * t + 2) + b;
        }

        /// <summary>
        /// Easing equation function for a cubic (t^3) easing out/in: 
        /// deceleration until halfway, then acceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double CubicEaseOutIn(double t, double b, double c, double d) {
            if (t < d / 2)
                return CubicEaseOut(t * 2, b, c / 2, d);

            return CubicEaseIn((t * 2) - d, b + c / 2, c / 2, d);
        }

        #endregion

        #region Quartic

        /// <summary>
        /// Easing equation function for a quartic (t^4) easing out: 
        /// decelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double QuartEaseOut(double t, double b, double c, double d) {
            return -c * ((t = t / d - 1) * t * t * t - 1) + b;
        }

        /// <summary>
        /// Easing equation function for a quartic (t^4) easing in: 
        /// accelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double QuartEaseIn(double t, double b, double c, double d) {
            return c * (t /= d) * t * t * t + b;
        }

        /// <summary>
        /// Easing equation function for a quartic (t^4) easing in/out: 
        /// acceleration until halfway, then deceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double QuartEaseInOut(double t, double b, double c, double d) {
            if ((t /= d / 2) < 1)
                return c / 2 * t * t * t * t + b;

            return -c / 2 * ((t -= 2) * t * t * t - 2) + b;
        }

        /// <summary>
        /// Easing equation function for a quartic (t^4) easing out/in: 
        /// deceleration until halfway, then acceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double QuartEaseOutIn(double t, double b, double c, double d) {
            if (t < d / 2)
                return QuartEaseOut(t * 2, b, c / 2, d);

            return QuartEaseIn((t * 2) - d, b + c / 2, c / 2, d);
        }

        #endregion

        #region Quintic

        /// <summary>
        /// Easing equation function for a quintic (t^5) easing out: 
        /// decelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double QuintEaseOut(double t, double b, double c, double d) {
            return c * ((t = t / d - 1) * t * t * t * t + 1) + b;
        }

        /// <summary>
        /// Easing equation function for a quintic (t^5) easing in: 
        /// accelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double QuintEaseIn(double t, double b, double c, double d) {
            return c * (t /= d) * t * t * t * t + b;
        }

        /// <summary>
        /// Easing equation function for a quintic (t^5) easing in/out: 
        /// acceleration until halfway, then deceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double QuintEaseInOut(double t, double b, double c, double d) {
            if ((t /= d / 2) < 1)
                return c / 2 * t * t * t * t * t + b;
            return c / 2 * ((t -= 2) * t * t * t * t + 2) + b;
        }

        /// <summary>
        /// Easing equation function for a quintic (t^5) easing in/out: 
        /// acceleration until halfway, then deceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double QuintEaseOutIn(double t, double b, double c, double d) {
            if (t < d / 2)
                return QuintEaseOut(t * 2, b, c / 2, d);
            return QuintEaseIn((t * 2) - d, b + c / 2, c / 2, d);
        }

        #endregion

        #region Elastic

        /// <summary>
        /// Easing equation function for an elastic (exponentially decaying sine wave) easing out: 
        /// decelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double ElasticEaseOut(double t, double b, double c, double d) {
            if ((t /= d) == 1)
                return b + c;

            double p = d * .3;
            double s = p / 4;

            return (c * Math.Pow(2, -10 * t) * Math.Sin((t * d - s) * (2 * Math.PI) / p) + c + b);
        }

        /// <summary>
        /// Easing equation function for an elastic (exponentially decaying sine wave) easing in: 
        /// accelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double ElasticEaseIn(double t, double b, double c, double d) {
            if ((t /= d) == 1)
                return b + c;

            double p = d * .3;
            double s = p / 4;

            return -(c * Math.Pow(2, 10 * (t -= 1)) * Math.Sin((t * d - s) * (2 * Math.PI) / p)) + b;
        }

        /// <summary>
        /// Easing equation function for an elastic (exponentially decaying sine wave) easing in/out: 
        /// acceleration until halfway, then deceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double ElasticEaseInOut(double t, double b, double c, double d) {
            if ((t /= d / 2) == 2)
                return b + c;

            double p = d * (.3 * 1.5);
            double s = p / 4;

            if (t < 1)
                return -.5 * (c * Math.Pow(2, 10 * (t -= 1)) * Math.Sin((t * d - s) * (2 * Math.PI) / p)) + b;
            return c * Math.Pow(2, -10 * (t -= 1)) * Math.Sin((t * d - s) * (2 * Math.PI) / p) * .5 + c + b;
        }

        /// <summary>
        /// Easing equation function for an elastic (exponentially decaying sine wave) easing out/in: 
        /// deceleration until halfway, then acceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double ElasticEaseOutIn(double t, double b, double c, double d) {
            if (t < d / 2)
                return ElasticEaseOut(t * 2, b, c / 2, d);
            return ElasticEaseIn((t * 2) - d, b + c / 2, c / 2, d);
        }

        #endregion

        #region Bounce

        /// <summary>
        /// Easing equation function for a bounce (exponentially decaying parabolic bounce) easing out: 
        /// decelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double BounceEaseOut(double t, double b, double c, double d) {
            if ((t /= d) < (1 / 2.75))
                return c * (7.5625 * t * t) + b;
            else if (t < (2 / 2.75))
                return c * (7.5625 * (t -= (1.5 / 2.75)) * t + .75) + b;
            else if (t < (2.5 / 2.75))
                return c * (7.5625 * (t -= (2.25 / 2.75)) * t + .9375) + b;
            else
                return c * (7.5625 * (t -= (2.625 / 2.75)) * t + .984375) + b;
        }

        /// <summary>
        /// Easing equation function for a bounce (exponentially decaying parabolic bounce) easing in: 
        /// accelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double BounceEaseIn(double t, double b, double c, double d) {
            return c - BounceEaseOut(d - t, 0, c, d) + b;
        }

        /// <summary>
        /// Easing equation function for a bounce (exponentially decaying parabolic bounce) easing in/out: 
        /// acceleration until halfway, then deceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double BounceEaseInOut(double t, double b, double c, double d) {
            if (t < d / 2)
                return BounceEaseIn(t * 2, 0, c, d) * .5 + b;
            else
                return BounceEaseOut(t * 2 - d, 0, c, d) * .5 + c * .5 + b;
        }

        /// <summary>
        /// Easing equation function for a bounce (exponentially decaying parabolic bounce) easing out/in: 
        /// deceleration until halfway, then acceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double BounceEaseOutIn(double t, double b, double c, double d) {
            if (t < d / 2)
                return BounceEaseOut(t * 2, b, c / 2, d);
            return BounceEaseIn((t * 2) - d, b + c / 2, c / 2, d);
        }

        #endregion

        #region Back

        /// <summary>
        /// Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing out: 
        /// decelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double BackEaseOut(double t, double b, double c, double d) {
            return c * ((t = t / d - 1) * t * ((1.70158 + 1) * t + 1.70158) + 1) + b;
        }

        /// <summary>
        /// Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing in: 
        /// accelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double BackEaseIn(double t, double b, double c, double d) {
            return c * (t /= d) * t * ((1.70158 + 1) * t - 1.70158) + b;
        }

        /// <summary>
        /// Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing in/out: 
        /// acceleration until halfway, then deceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double BackEaseInOut(double t, double b, double c, double d) {
            double s = 1.70158;
            if ((t /= d / 2) < 1)
                return c / 2 * (t * t * (((s *= (1.525)) + 1) * t - s)) + b;
            return c / 2 * ((t -= 2) * t * (((s *= (1.525)) + 1) * t + s) + 2) + b;
        }

        /// <summary>
        /// Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing out/in: 
        /// deceleration until halfway, then acceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double BackEaseOutIn(double t, double b, double c, double d) {
            if (t < d / 2)
                return BackEaseOut(t * 2, b, c / 2, d);
            return BackEaseIn((t * 2) - d, b + c / 2, c / 2, d);
        }

        #endregion

        #endregion
    }
}