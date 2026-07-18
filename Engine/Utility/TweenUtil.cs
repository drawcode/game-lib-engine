// #USE_

// USE_EASING_ITWEEN
// USE_EASING_LEANTWEEN
// USE_EASING_NGUI

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

using Engine.Animation;
using Engine.UI;

namespace Engine.Utility {

    public enum TweenLib {
        none,
        iTween,
        leanTween,
        nguiUITweener,
        internalEasing
    }

    public enum TweenEaseType {
        linear,
        quadEaseOut,
        quadEaseIn,
        quadEaseInOut
    }

    public enum TweenLoopType {
        once,
        loop,
        pingPong,
        bounce
    }

    public enum TweenCoord {
        world,
        local
    }

    public class TweenMeta {

        public TweenLib _lib = TweenLib.none;

        public GameObject _go;
        public float _time = 1f;
        public float _delay = 0f;
        public TweenEaseType _easeType = TweenEaseType.quadEaseInOut;
        public TweenLoopType _loopType = TweenLoopType.once;
        public TweenCoord _coord = TweenCoord.local;
        public Action _onComplete = null;
        public Action _onFinal = null;
        public Action _onStart = null;
        public Action _onUpdate = null;
        public bool _stopCurrent = false;

        public float durationShow = .45f;
        public float durationDelayShow = .5f;

        public float durationHide = .45f;
        public float durationDelayHide = 0f;

        public float leftOpenX = 0f;
        public float leftClosedX = -4500f;

        public float rightOpenX = 0f;
        public float rightClosedX = 4500f;

        public float bottomOpenY = 0f;
        public float bottomClosedY = -4500f;

        public float topOpenY = 0f;
        public float topClosedY = 4500f;

        public TweenLib lib {
            get {
                return _lib;
            }
            set {
                _lib = value;
            }
        }

        public GameObject go {
            get {
                return _go;
            }
            set {
                _go = value;
            }
        }

        public float time {
            get {
                return _time;
            }
            set {
                _time = value;
            }
        }

        public float delay {
            get {
                return _delay;
            }
            set {
                _delay = value;
            }
        }

        public TweenEaseType easeType {
            get {
                return _easeType;
            }
            set {
                _easeType = value;
            }
        }

        public TweenLoopType loopType {
            get {
                return _loopType;
            }
            set {
                _loopType = value;
            }
        }

        public TweenCoord coord {
            get {
                return _coord;
            }
            set {
                _coord = value;
            }
        }

        public Action onFinal {
            get {
                return _onFinal;
            }
            set {
                _onFinal = value;
            }
        }

        public Action onComplete {
            get {
                return _onComplete;
            }
            set {
                _onComplete = value;
            }
        }

        public Action onStart {
            get {
                return _onStart;
            }
            set {
                _onStart = value;
            }
        }

        public Action onUpdate {
            get {
                return _onUpdate;
            }
            set {
                _onUpdate = value;
            }
        }

        public bool stopCurrent {
            get {
                return _stopCurrent;
            }
            set {
                _stopCurrent = value;
            }
        }
    }

    public class TweenUtil {

        public static float durationShow = .45f;
        public static float durationDelayShow = .5f;

        public static float durationHide = .45f;
        public static float durationDelayHide = 0f;

        public static float leftOpenX = 0f;
        public static float leftClosedX = -4500f;

        public static float rightOpenX = 0f;
        public static float rightClosedX = 4500f;

        public static float bottomOpenY = 0f;
        public static float bottomClosedY = -4500f;

        public static float topOpenY = 0f;
        public static float topClosedY = 4500f;

        public static int increment = 0;

        // --------------------------------------------------------------------
        // BACKEND

        private static ITweenBackend _backend = null;

        public static ITweenBackend backend {
            get {
                if (_backend == null) {
                    _backend = EasingTweenBackend.Instance;
                }

                return _backend;
            }
        }

        public static void SetBackend(ITweenBackend value) {
            _backend = value;
        }

        public static ITweenTarget ResolveTarget(GameObject go) {

            if (go == null) {
                return null;
            }

            return new TransformTweenTarget(go);
        }

        public static ITweenTarget ResolveTarget(object native) {

            if (native == null) {
                return null;
            }

            ITweenTarget target = VisualElementTweenTarget.TryCreate(native);

            if (target != null) {
                return target;
            }

            GameObject go = native as GameObject;

            if (go != null) {
                return ResolveTarget(go);
            }

            return null;
        }

        // LOOP TYPES

        public static T ConvertLibLoopType<T>(TweenLoopType loopType) {

            T libType = default(T);

            Type genericType = typeof(T);

            if (genericType == null) {

            }




            return libType;
        }

        public static TweenLoopType ConvertTweensLoopType<T>(T loopTypeLib) {

            TweenLoopType loopType = TweenLoopType.loop;

            Type genericType = typeof(T);

            if (genericType == null) {

            }




            return loopType;
        }

        // EASE TYPES

        public static T ConvertLibEaseType<T>(TweenEaseType easeType) {

            T libType = default(T);

            Type genericType = typeof(T);

            if (genericType == null) {

            }




            return libType;
        }

        public static TweenEaseType ConvertTweensEaseType<T>(T loopTypeLib) {

            TweenEaseType easeType = TweenEaseType.linear;

            Type genericType = typeof(T);

            if (genericType == null) {

            }




            return easeType;
        }

        // --------------------------------------------------------------------
        // META

        public static TweenMeta GetMetaDefault(
           TweenLib lib,
           GameObject go,
           float time = .5f, float delay = .5f,
           bool stopCurrent = true,
           TweenCoord coord = TweenCoord.world,
           TweenEaseType easeType = TweenEaseType.quadEaseInOut,
           TweenLoopType loopType = TweenLoopType.once) {

            TweenMeta meta = new TweenMeta();
            meta.lib = lib;
            meta.go = go;
            meta.time = time;
            meta.delay = delay;
            meta.stopCurrent = stopCurrent;
            meta.coord = coord;
            meta.easeType = easeType;
            meta.loopType = loopType;

            return meta;
        }

        public static void OnTweenBegin(Action action) {
            action();
        }

        public static void OnTweenFinish(Action action) {
            action();
        }

        public static void OnTweenTick(Action action) {
            action();
        }

        public static void SetImageAlpha(float val, Image img) {
            Color color = img.color;
            color.a = val;
            img.color = color;
        }

        // --------------------------------------------------------------------
        // MOVE

        public static void MoveToObject(
           GameObject go,
           Vector3 pos,
           float time = .5f, float delay = .5f,
           bool stopCurrent = true,
           TweenCoord coord = TweenCoord.world,
           TweenEaseType easeType = TweenEaseType.quadEaseInOut,
           TweenLoopType loopType = TweenLoopType.once) {

            MoveToObjectLeanTween(
                go, pos, time, delay, stopCurrent, coord, easeType, loopType);
        }

        public static void MoveToObject(
           TweenLib lib,
           GameObject go,
           Vector3 pos,
           float time = .5f, float delay = .5f,
           bool stopCurrent = true,
           TweenCoord coord = TweenCoord.world,
           TweenEaseType easeType = TweenEaseType.quadEaseInOut,
           TweenLoopType loopType = TweenLoopType.once) {

            if (go == null) {
                return;
            }

            TweenMeta meta =
                GetMetaDefault(
                    lib, go, time, delay, stopCurrent, coord, easeType, loopType);

            MoveToObject(meta, pos);
        }

        public static void MoveToObjectLeanTween(
           GameObject go,
           Vector3 pos,
           float time = .5f, float delay = .5f,
           bool stopCurrent = true,
           TweenCoord coord = TweenCoord.world,
           TweenEaseType easeType = TweenEaseType.quadEaseInOut,
           TweenLoopType loopType = TweenLoopType.once) {

            MoveToObject(
                TweenLib.leanTween,
                go,
                pos, time, delay, stopCurrent, coord, easeType, loopType);
        }

        public static void MoveToObjectiTween(
           GameObject go,
           Vector3 pos,
           float time = .5f, float delay = .5f,
           bool stopCurrent = true,
           TweenCoord coord = TweenCoord.world,
           TweenEaseType easeType = TweenEaseType.quadEaseInOut,
           TweenLoopType loopType = TweenLoopType.once) {

            MoveToObject(
                TweenLib.iTween,
                go,
                pos, time, delay, stopCurrent, coord, easeType, loopType);
        }

        public static void MoveToObjectUITweener(
           GameObject go,
           Vector3 pos,
           float time = .5f, float delay = .5f,
           bool stopCurrent = true,
           TweenCoord coord = TweenCoord.world,
           TweenEaseType easeType = TweenEaseType.quadEaseInOut,
           TweenLoopType loopType = TweenLoopType.once) {

            MoveToObject(
                TweenLib.nguiUITweener,
                go,
                pos, time, delay, stopCurrent, coord, easeType, loopType);
        }

        public static void MoveToObject(
            TweenMeta meta,
            Vector3 pos) {

            if (meta.go == null) {
                return;
            }

            // All tweens run on the internal backend; legacy lib branches below
            // are unreachable and get deleted with the vendored libs.
            meta.lib = TweenLib.internalEasing;

            // NGUI TweenPosition always animated localPosition regardless of the
            // requested coord, and a decade of call sites (which default to world)
            // rely on that — honoring world moves panels to wrong absolute spots.
            meta.coord = TweenCoord.local;

            Action onBegin = () => {

            };

            Action onFinish = () => {

            };

            Action onTick = () => {

            };

            if (meta.onStart != null) {
                onBegin = onBegin.CombineAction(meta.onStart);
            }

            if (meta.onComplete != null) {
                onFinish = onFinish.CombineAction(meta.onComplete);
            }

            if (meta.onFinal != null) {
                onFinish = onFinish.CombineAction(meta.onFinal);
            }

            if (meta.onUpdate != null) {
                onTick = onTick.CombineAction(meta.onUpdate);
            }

            if (meta.lib == TweenLib.none) {

            }



            else if (meta.lib == TweenLib.internalEasing) {
                backend.Move(ResolveTarget(meta.go), pos, meta);
            }
        }

        // --------------------------------------------------------------------
        // SCALE

        public static void ScaleToObject(
           GameObject go,
           Vector3 pos,
           float time = .5f, float delay = .5f,
           bool stopCurrent = true,
           TweenCoord coord = TweenCoord.world,
           TweenEaseType easeType = TweenEaseType.quadEaseInOut,
           TweenLoopType loopType = TweenLoopType.once) {

            ScaleToObjectLeanTween(
                go, pos, time, delay, stopCurrent, coord, easeType, loopType);
        }

        public static void ScaleToObject(
           TweenLib lib,
           GameObject go,
           Vector3 pos,
           float time = .5f, float delay = .5f,
           bool stopCurrent = true,
           TweenCoord coord = TweenCoord.world,
           TweenEaseType easeType = TweenEaseType.quadEaseInOut,
           TweenLoopType loopType = TweenLoopType.once) {

            if (go == null) {
                return;
            }

            TweenMeta meta =
                GetMetaDefault(
                    lib, go, time, delay, stopCurrent, coord, easeType, loopType);

            ScaleToObject(meta, pos);
        }

        public static void ScaleToObjectLeanTween(
           GameObject go,
           Vector3 pos,
           float time = .5f, float delay = .5f,
           bool stopCurrent = true,
           TweenCoord coord = TweenCoord.world,
           TweenEaseType easeType = TweenEaseType.quadEaseInOut,
           TweenLoopType loopType = TweenLoopType.once) {

            MoveToObject(
                TweenLib.leanTween,
                go,
                pos, time, delay, stopCurrent, coord, easeType, loopType);
        }

        public static void ScaleToObjectiTween(
           GameObject go,
           Vector3 pos,
           float time = .5f, float delay = .5f,
           bool stopCurrent = true,
           TweenCoord coord = TweenCoord.world,
           TweenEaseType easeType = TweenEaseType.quadEaseInOut,
           TweenLoopType loopType = TweenLoopType.once) {

            MoveToObject(
                TweenLib.iTween,
                go,
                pos, time, delay, stopCurrent, coord, easeType, loopType);
        }

        public static void ScaleToObjectUITweener(
           GameObject go,
           Vector3 pos,
           float time = .5f, float delay = .5f,
           bool stopCurrent = true,
           TweenCoord coord = TweenCoord.world,
           TweenEaseType easeType = TweenEaseType.quadEaseInOut,
           TweenLoopType loopType = TweenLoopType.once) {

            MoveToObject(
                TweenLib.nguiUITweener,
                go,
                pos, time, delay, stopCurrent, coord, easeType, loopType);
        }

        public static void ScaleToObject(
            TweenMeta meta,
            Vector3 pos) {

            if (meta.go == null) {
                return;
            }


            // All tweens run on the internal backend; legacy lib branches below
            // are unreachable and get deleted with the vendored libs.
            meta.lib = TweenLib.internalEasing;

            Action onBegin = () => {

            };

            Action onFinish = () => {

            };

            Action onTick = () => {

            };

            if (meta.onStart != null) {
                onBegin = onBegin.CombineAction(meta.onStart);
            }

            if (meta.onComplete != null) {
                onFinish = onFinish.CombineAction(meta.onComplete);
            }

            if (meta.onFinal != null) {
                onFinish = onFinish.CombineAction(meta.onFinal);
            }

            if (meta.onUpdate != null) {
                onTick = onTick.CombineAction(meta.onUpdate);
            }

            if (meta.lib == TweenLib.none) {

            }



            else if (meta.lib == TweenLib.internalEasing) {
                backend.Scale(ResolveTarget(meta.go), pos, meta);
            }
        }

        // --------------------------------------------------------------------
        // ROTATE

        public static void RotateToObject(
           GameObject go,
           Vector3 pos,
           float time = .5f, float delay = .5f,
           bool stopCurrent = true,
           TweenCoord coord = TweenCoord.world,
           TweenEaseType easeType = TweenEaseType.quadEaseInOut,
           TweenLoopType loopType = TweenLoopType.once) {

            RotateToObjectLeanTween(
                go, pos, time, delay, stopCurrent, coord, easeType, loopType);
        }

        public static void RotateToObject(
           TweenLib lib,
           GameObject go,
           Vector3 pos,
           float time = .5f, float delay = .5f,
           bool stopCurrent = true,
           TweenCoord coord = TweenCoord.world,
           TweenEaseType easeType = TweenEaseType.quadEaseInOut,
           TweenLoopType loopType = TweenLoopType.once) {

            TweenMeta meta =
                GetMetaDefault(
                    lib, go, time, delay, stopCurrent, coord, easeType, loopType);

            RotateToObject(meta, pos);
        }

        public static void RotateToObjectLeanTween(
           GameObject go,
           Vector3 pos,
           float time = .5f, float delay = .5f,
           bool stopCurrent = true,
           TweenCoord coord = TweenCoord.world,
           TweenEaseType easeType = TweenEaseType.quadEaseInOut,
           TweenLoopType loopType = TweenLoopType.once) {

            RotateToObject(
                TweenLib.leanTween,
                go,
                pos, time, delay, stopCurrent, coord, easeType, loopType);
        }

        public static void RotateToObjectiTween(
           GameObject go,
           Vector3 pos,
           float time = .5f, float delay = .5f,
           bool stopCurrent = true,
           TweenCoord coord = TweenCoord.world,
           TweenEaseType easeType = TweenEaseType.quadEaseInOut,
           TweenLoopType loopType = TweenLoopType.once) {

            RotateToObject(
                TweenLib.iTween,
                go,
                pos, time, delay, stopCurrent, coord, easeType, loopType);
        }

        public static void RotateToObjectUITweener(
           GameObject go,
           Vector3 pos,
           float time = .5f, float delay = .5f,
           bool stopCurrent = true,
           TweenCoord coord = TweenCoord.world,
           TweenEaseType easeType = TweenEaseType.quadEaseInOut,
           TweenLoopType loopType = TweenLoopType.once) {

            RotateToObject(
                TweenLib.nguiUITweener,
                go,
                pos, time, delay, stopCurrent, coord, easeType, loopType);
        }

        public static void RotateToObject(
            TweenMeta meta,
            Vector3 pos) {

            if (meta.go == null) {
                return;
            }

            // All tweens run on the internal backend; legacy lib branches below
            // are unreachable and get deleted with the vendored libs.
            meta.lib = TweenLib.internalEasing;

            // NGUI TweenRotation always animated localRotation — see MoveToObject.
            meta.coord = TweenCoord.local;

            Action onBegin = () => {

            };

            Action onFinish = () => {

            };

            Action onTick = () => {

            };

            if (meta.onStart != null) {
                onBegin = onBegin.CombineAction(meta.onStart);
            }

            if (meta.onComplete != null) {
                onFinish = onFinish.CombineAction(meta.onComplete);
            }

            if (meta.onFinal != null) {
                onFinish = onFinish.CombineAction(meta.onFinal);
            }

            if (meta.onUpdate != null) {
                onTick = onTick.CombineAction(meta.onUpdate);
            }

            if (meta.lib == TweenLib.none) {

            }



            else if (meta.lib == TweenLib.internalEasing) {
                backend.Rotate(ResolveTarget(meta.go), pos, meta);
            }
        }

        // --------------------------------------------------------------------
        // FADE

        public static void FadeToObject(
           GameObject go,
           float alpha,
           float time = .5f, float delay = .5f,
           bool stopCurrent = true,
           TweenCoord coord = TweenCoord.world,
           TweenEaseType easeType = TweenEaseType.quadEaseInOut,
           TweenLoopType loopType = TweenLoopType.once) {

            if (go == null) {
                return;
            }

            TweenLib lib = TweenLib.leanTween;

            FadeToObject(lib,
                go,
                alpha, time, delay, stopCurrent, coord, easeType, loopType);
        }

        public static void FadeToObject(
           TweenLib lib,
           GameObject go,
           float alpha,
           float time = .5f, float delay = .5f,
           bool stopCurrent = true,
           TweenCoord coord = TweenCoord.world,
           TweenEaseType easeType = TweenEaseType.quadEaseInOut,
           TweenLoopType loopType = TweenLoopType.once) {

            if (go == null) {
                return;
            }

            TweenMeta meta =
                GetMetaDefault(
                    lib, go, time, delay, stopCurrent, coord, easeType, loopType);

            FadeToObject(meta, alpha);
        }

        public static void FadeToObjectLeanTween(
           GameObject go,
           float alpha,
           float time = .5f, float delay = .5f,
           bool stopCurrent = true,
           TweenCoord coord = TweenCoord.world,
           TweenEaseType easeType = TweenEaseType.quadEaseInOut,
           TweenLoopType loopType = TweenLoopType.once) {

            FadeToObject(
                TweenLib.leanTween,
                go,
                alpha, time, delay, stopCurrent, coord, easeType, loopType);
        }

        public static void FadeToObjectiTween(
           GameObject go,
           float alpha,
           float time = .5f, float delay = .5f,
           bool stopCurrent = true,
           TweenCoord coord = TweenCoord.world,
           TweenEaseType easeType = TweenEaseType.quadEaseInOut,
           TweenLoopType loopType = TweenLoopType.once) {

            FadeToObject(
                TweenLib.iTween,
                go,
                alpha, time, delay, stopCurrent, coord, easeType, loopType);
        }

        public static void FadeToObjectUITweener(
           GameObject go,
           float alpha,
           float time = .5f, float delay = .5f,
           bool stopCurrent = true,
           TweenCoord coord = TweenCoord.world,
           TweenEaseType easeType = TweenEaseType.quadEaseInOut,
           TweenLoopType loopType = TweenLoopType.once) {

            FadeToObject(
                TweenLib.nguiUITweener,
                go,
                alpha, time, delay, stopCurrent, coord, easeType, loopType);
        }

        // Convenience shims for UITweenerUtil.FadeIn/FadeOut/FadeOutNow (chunk 1.3 port map).
        // Route through FadeToObject so the forced-NGUI override / lib resolution is untouched.

        public static void FadeInObject(
           GameObject go,
           float time = 1f, float delay = 1f,
           bool stopCurrent = true,
           TweenCoord coord = TweenCoord.world,
           TweenEaseType easeType = TweenEaseType.quadEaseIn,
           TweenLoopType loopType = TweenLoopType.once) {

            FadeToObject(go, 1f, time, delay, stopCurrent, coord, easeType, loopType);
        }

        public static void FadeOutObject(
           GameObject go,
           float time = 1f, float delay = 0f,
           bool stopCurrent = true,
           TweenCoord coord = TweenCoord.world,
           TweenEaseType easeType = TweenEaseType.quadEaseIn,
           TweenLoopType loopType = TweenLoopType.once) {

            FadeToObject(go, 0f, time, delay, stopCurrent, coord, easeType, loopType);
        }

        public static void FadeOutObjectNow(
           GameObject go,
           bool stopCurrent = true,
           TweenCoord coord = TweenCoord.world) {

            FadeToObject(go, 0f, 0f, 0f, stopCurrent, coord, TweenEaseType.quadEaseIn, TweenLoopType.once);
        }

        public static void FadeToObject(
            TweenMeta meta,
            float alpha) {

            if (meta.go == null) {
                return;
            }

            Action onBegin = () => {

            };

            Action onFinish = () => {

            };

            Action onTick = () => {

            };

            if (meta.onStart != null) {
                onBegin = onBegin.CombineAction(meta.onStart);
            }

            if (meta.onComplete != null) {
                onFinish = onFinish.CombineAction(meta.onComplete);
            }

            if (meta.onFinal != null) {
                onFinish = onFinish.CombineAction(meta.onFinal);
            }

            if (meta.onUpdate != null) {
                onTick = onTick.CombineAction(meta.onUpdate);
            }

            if (alpha > 0f) {
                onBegin = onBegin.CombineAction(() => {
                    meta.go.Show();
                });
            }

            if (alpha == 0f) {
                onFinish = onFinish.CombineAction(() => {
                    meta.go.Hide();
                });
            }

            // Pre-flip, only sprite-on-self GOs got forced onto the NGUI tweener
            // (whose callbacks were dead - see below); every other GO fell through
            // to the default lib (LeanTween), which DID fire onStart/onComplete for
            // real, so plain containers (header title/backer wrappers, etc.) were
            // actually Show()/Hide()'d by GameObject active-state. Capture that
            // distinction before collapsing everything onto the internal backend,
            // so non-sprite fades keep getting their Show()/Hide() side effects.
            bool hadSpriteOnSelf = false;
#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3 || USE_EASING_NGUI
            hadSpriteOnSelf =
                meta.go.Has<UISlicedSprite>()
                || meta.go.Has<UISprite>()
                || meta.go.Has<UITiledSprite>();
#endif

            // All tweens run on the internal backend; legacy lib branches below
            // are unreachable and get deleted with the vendored libs.
            meta.lib = TweenLib.internalEasing;

            //             if(meta.lib == TweenLib.none) {
            // #if USE_UI_NGUI_2_7 || USE_UI_NGUI_3 || USE_EASING_NGUI
            //                 if(meta.lib == TweenLib.none) {
            //                     meta.lib = TweenLib.nguiUITweener;
            //                 }
            // #elif USE_EASING_LEANTWEEN
            //                 if(meta.lib == TweenLib.none) {
            //                     meta.lib = TweenLib.leanTween;
            //                 }
            // #elif USE_EASING_ITWEEN
            //                 if(meta.lib == TweenLib.none) {
            //                     meta.lib = TweenLib.iTween;
            //                 }
            // #endif
            //             }



            if (meta.lib == TweenLib.internalEasing) {

                if (hadSpriteOnSelf) {

                    // Raw meta on purpose: the NGUI tweener never fired the composed
                    // Show()/Hide() side effects on sprite widgets, and panel code
                    // owns active-state there. A backend-driven Hide() on fade-out
                    // would deactivate objects that legacy AnimateIn/ShowDefault
                    // flows expect to stay active (gate learning #5).
                    backend.Fade(ResolveTarget(meta.go), alpha, meta);
                }
                else {

                    // Dispatch with the composed local callbacks so the Show()/Hide()
                    // side effects above reach the backend (meta itself lacks them).
                    // This restores the historically-live default-lib (LeanTween)
                    // behavior for non-sprite GOs (plain containers), whose
                    // onStart/onComplete really did fire pre-flip.
                    TweenMeta metaDispatch = GetMetaDefault(
                        meta.lib, meta.go, meta.time, meta.delay,
                        meta.stopCurrent, meta.coord, meta.easeType, meta.loopType);
                    metaDispatch.onStart = onBegin;
                    metaDispatch.onComplete = onFinish;
                    metaDispatch.onUpdate = meta.onUpdate;

                    backend.Fade(ResolveTarget(meta.go), alpha, metaDispatch);
                }
            }

            // The legacy "-a-NN" child recursion was removed here: it only ever ran on
            // the LeanTween path, where child fades were silent no-ops on NGUI widgets.
            // Once tweens became real it forced children like BackgroundDark-a-70 to
            // NN% alpha on EVERY fade — including fade-outs — leaving translucent dark
            // backdrops stuck over the menu (Phase 1 gate finding, 2026-07-12).
            // ColorToObject keeps its recursion: that one was live via the NGUI path.
        }

        // --------------------------------------------------------------------
        // COLOR

        public static void ColorToObject(
           GameObject go,
           Color color,
           float time = .5f, float delay = .5f,
           bool stopCurrent = true,
           TweenCoord coord = TweenCoord.world,
           TweenEaseType easeType = TweenEaseType.quadEaseInOut,
           TweenLoopType loopType = TweenLoopType.once) {

            ColorToObjectLeanTween(
                go,
                color, time, delay, stopCurrent, coord, easeType, loopType);
        }

        public static void ColorToObject(
           TweenLib lib,
           GameObject go,
           Color color,
           float time = .5f, float delay = .5f,
           bool stopCurrent = true,
           TweenCoord coord = TweenCoord.world,
           TweenEaseType easeType = TweenEaseType.quadEaseInOut,
           TweenLoopType loopType = TweenLoopType.once) {

            if (go == null) {
                return;
            }

            TweenMeta meta =
                GetMetaDefault(
                    lib, go, time, delay, stopCurrent, coord, easeType, loopType);

            ColorToObject(meta, color);
        }

        public static void ColorToObjectLeanTween(
           GameObject go,
           Color color,
           float time = .5f, float delay = .5f,
           bool stopCurrent = true,
           TweenCoord coord = TweenCoord.world,
           TweenEaseType easeType = TweenEaseType.quadEaseInOut,
           TweenLoopType loopType = TweenLoopType.once) {

            ColorToObject(
                TweenLib.leanTween,
                go,
                color, time, delay, stopCurrent, coord, easeType, loopType);
        }

        public static void ColorToObjectiTween(
           GameObject go,
           Color color,
           float time = .5f, float delay = .5f,
           bool stopCurrent = true,
           TweenCoord coord = TweenCoord.world,
           TweenEaseType easeType = TweenEaseType.quadEaseInOut,
           TweenLoopType loopType = TweenLoopType.once) {

            ColorToObject(
                TweenLib.iTween,
                go,
                color, time, delay, stopCurrent, coord, easeType, loopType);
        }

        public static void ColorToObjectUITweener(
           GameObject go,
           Color color,
           float time = .5f, float delay = .5f,
           bool stopCurrent = true,
           TweenCoord coord = TweenCoord.world,
           TweenEaseType easeType = TweenEaseType.quadEaseInOut,
           TweenLoopType loopType = TweenLoopType.once) {

            ColorToObject(
                TweenLib.nguiUITweener,
                go,
                color, time, delay, stopCurrent, coord, easeType, loopType);
        }

        public static void ColorToObject(
            TweenMeta meta,
            Color color) {

            if (meta.go == null) {
                return;
            }

            // All tweens run on the internal backend; legacy lib branches below
            // are unreachable and get deleted with the vendored libs.
            meta.lib = TweenLib.internalEasing;

            Action onBegin = () => {

            };

            Action onFinish = () => {

            };

            Action onTick = () => {

            };

            if (meta.onStart != null) {
                onBegin = onBegin.CombineAction(meta.onStart);
            }

            if (meta.onComplete != null) {
                onFinish = onFinish.CombineAction(meta.onComplete);
            }

            if (meta.onFinal != null) {
                onFinish = onFinish.CombineAction(meta.onFinal);
            }

            if (meta.onUpdate != null) {
                onTick = onTick.CombineAction(meta.onUpdate);
            }

            if (color.a > 0f) {
                onBegin = onBegin.CombineAction(() => {
                    meta.go.Show();
                });
            }

            if (color.a == 0f) {
                onFinish = onFinish.CombineAction(() => {
                    meta.go.Hide();
                });
            }

            if (meta.lib == TweenLib.none) {

            }



            else if (meta.lib == TweenLib.internalEasing) {

                // Raw meta on purpose — see FadeToObject: tweens must not flip
                // active state; the NGUI path never ran these side effects.
                backend.ColorTo(ResolveTarget(meta.go), color, meta);
            }

            /*
             * TODO nested -a- marked objects to keep alpha on on nested when needed
             * ex: objectname-a-50 = alpha 50% on nested no matter parent
             *
             */
            foreach (Transform t in meta.go.transform) {
                string toLook = "-a-";
                int alphaMarker = t.name.IndexOf(toLook);
                //string alphaObject = t.name;
                if (alphaMarker > -1) {
                    // Fade it immediately
                    //FadeToObject(t.gameObject, alpha, meta.time, meta.delay);
                    // Fade to the correct value after initial fade in
                    string val = t.name.Substring(alphaMarker + toLook.Length);
                    if (!string.IsNullOrEmpty(val)) {
                        float valNumeric = 0f;
                        float.TryParse(val, out valNumeric);

                        if (valNumeric > 0f) {
                            valNumeric = valNumeric / 100f;

                            color.a = valNumeric;

                            //FadeTo(t.gameObject, UITweener.Method.Linear, UITweener.Style.Once,
                            //    duration + .05f, duration + delay, valNumeric);

                            ColorToObject(t.gameObject, color, meta.time, meta.delay + .05f);
                        }
                    }
                }
                //FadeToObject(t.gameObject, alpha, meta.time, meta.delay);
            }
        }

        // --------------------------------------------------------------------
        // VALUE / CANCEL (internalEasing backend)

        public static void ValueTo(
            string key,
            float from, float to,
            float time, float delay,
            TweenEaseType ease,
            Action<float> onValue,
            Action onComplete = null) {

            if (string.IsNullOrEmpty(key) || onValue == null) {
                return;
            }

            TweenMeta meta = new TweenMeta();
            meta.time = time;
            meta.delay = delay;
            meta.easeType = ease;
            meta.onComplete = onComplete;

            backend.Value(key, from, to, meta, onValue);
        }

        public static void Cancel(GameObject go) {

            if (go == null) {
                return;
            }

            backend.Cancel(ResolveTarget(go));
        }

        public static void Cancel(string key) {

            if (string.IsNullOrEmpty(key)) {
                return;
            }

            backend.Cancel(key);
        }

        public static void CancelAll() {
            backend.CancelAll();
        }

        // --------------------------------------------------------------------
        // EASING HELPERS FOR UI and OBJECTS

        // top

        public static void ShowObjectTop(
            GameObject go,
            TweenCoord coord = TweenCoord.local, bool fade = true,
            float time = .5f, float delay = .55f) {
            ShowObject(go, Vector3.zero.WithY(topOpenY), coord, fade, time, delay);
        }

        public static void HideObjectTop(
            GameObject go,
            TweenCoord coord = TweenCoord.local, bool fade = true,
            float time = .5f, float delay = 0f) {
            HideObject(go, Vector3.zero.WithY(topClosedY), coord, fade, time, delay);
        }

        // bottom

        public static void ShowObjectBottom(
            GameObject go,
            TweenCoord coord = TweenCoord.local, bool fade = true,
            float time = .5f, float delay = .55f) {
            ShowObject(go, Vector3.zero.WithY(bottomOpenY), coord, fade, time, delay);
        }

        public static void HideObjectBottom(
            GameObject go,
            TweenCoord coord = TweenCoord.local, bool fade = true,
            float time = .5f, float delay = 0f) {
            HideObject(go, Vector3.zero.WithY(bottomClosedY), coord, fade, time, delay);
        }

        // left

        public static void ShowObjectLeft(
            GameObject go,
            TweenCoord coord = TweenCoord.local, bool fade = true,
            float time = .5f, float delay = .55f) {
            ShowObject(go, Vector3.zero.WithX(leftOpenX), coord, fade, time, delay);
        }

        public static void HideObjectLeft(
            GameObject go,
            TweenCoord coord = TweenCoord.local, bool fade = true,
            float time = .5f, float delay = 0f) {
            HideObject(go, Vector3.zero.WithX(leftClosedX), coord, fade);
        }

        // right

        public static void ShowObjectRight(
            GameObject go,
            TweenCoord coord = TweenCoord.local, bool fade = true,
            float time = .5f, float delay = .55f) {
            ShowObject(go, Vector3.zero.WithX(rightOpenX), coord, fade, time, delay);
        }

        public static void HideObjectRight(
            GameObject go,
            TweenCoord coord = TweenCoord.local, bool fade = true,
            float time = .5f, float delay = 0f) {
            HideObject(go, Vector3.zero.WithX(rightClosedX), coord, fade, time, delay);
        }

        //

        public static void ShowObject(
            GameObject go, Vector3 pos,
            TweenCoord coord = TweenCoord.local, bool fade = true,
            float time = .5f, float delay = .55f) {

            if (go == null) {
                return;
            }

            TweenObjectState(go, pos, 1f, coord, fade, time, delay);
        }

        public static void HideObject(
            GameObject go, Vector3 pos,
            TweenCoord coord = TweenCoord.local, bool fade = true,
            float time = .5f, float delay = 0f) {

            if (go == null) {
                return;
            }

            TweenObjectState(go, pos, 0f, coord, fade, time, delay);
        }

        //

        public static void TweenObjectState(
            GameObject go, Vector3 pos, float alpha = 1f,
            TweenCoord coord = TweenCoord.local, bool fade = true,
            float time = .5f, float delay = 0f) {

            if (go == null) {
                return;
            }

            //float time = durationShow;
            //float delay = durationDelayShow;

            //if (alpha > 0f) {
            //    go.Show();
            //}
            //else {
            //    //time = durationHide;
            //    //delay = durationDelayHide;
            //
            //   //go.HideObjectDelayed(time + delay);
            //}

            if (fade) {

                TweenUtil.FadeToObject(go, alpha, time, delay, true, coord);
            }

            TweenUtil.MoveToObject(
                go, pos, time, delay, false, coord);
        }

        // --------------------------------------------------------------------
        // UIREF PATH (UI Toolkit panels)
        //
        // Preset-driven, so panel timing lives in tokens.json (UITokens seeds TweenPresets)
        // rather than in the ±4500-unit literals scattered through UIPanelBase. Backend-blind:
        // ResolveTarget(object) already picks VisualElementTweenTarget for a VisualElement and
        // TransformTweenTarget for a GameObject, so the same call works either way.
        //
        // These do NOT touch display/active state. Gate learning #1: tweens never own
        // visibility. The panel system calls IUIBackend.Show/Hide around them.

        public static void FadeToObject(UIRef r, float alpha, string presetName) {

            if (r == null || !r.alive) {
                return;
            }

            ITweenTarget target = ResolveTarget(r.native);

            if (target == null) {
                return;
            }

            TweenPreset preset = TweenPresets.Get(presetName);

            TweenMeta meta = new TweenMeta();
            meta.time = preset.time;
            meta.delay = preset.delay;
            meta.easeType = preset.easeType;
            meta.loopType = preset.loopType;
            meta.stopCurrent = true;

            backend.Fade(target, alpha, meta);
        }

        public static void ShowObject(UIRef r, string presetName = "panel-show") {
            FadeToObject(r, 1f, presetName);
        }

        public static void HideObject(UIRef r, string presetName = "panel-hide") {
            FadeToObject(r, 0f, presetName);
        }

        // A toolkit VIEW slides down from off-screen top (translate) AND fades. ShowObject above
        // only fades, which is why a migrated panel's content sat still while the backer slid.
        //
        // A VisualElement's translate is y-DOWN, the opposite of the NGUI backer's y-up world
        // constants, so "off-screen top" is a NEGATIVE offset here. Timing comes from the
        // panel-show/panel-hide presets (tokens.json) — the same feel as every other panel
        // transition. (An earlier attempt synced to the backer's ~1.1s first-entry delay; that
        // lagged every navigation, because the shared backer only slides on FIRST entry and stays
        // resident between panels.)
        private const float viewTopOffset = -720f;   // px above the panel; clears the 640 view

        public static void ShowObjectTop(UIRef r, string presetName = "panel-show") {

            ITweenTarget target = SlideTarget(r);

            if (target == null) {
                return;
            }

            // Start off-screen top + invisible, then slide to 0 and fade in.
            target.SetPosition(new Vector3(0f, viewTopOffset, 0f), TweenCoord.local);
            target.SetAlpha(0f);

            SlideMove(target, Vector3.zero, presetName);
            SlideFade(target, 1f, presetName);
        }

        public static void HideObjectTop(UIRef r, string presetName = "panel-hide") {

            ITweenTarget target = SlideTarget(r);

            if (target == null) {
                return;
            }

            SlideMove(target, new Vector3(0f, viewTopOffset, 0f), presetName);
            SlideFade(target, 0f, presetName);
        }

        private static ITweenTarget SlideTarget(UIRef r) {

            if (r == null || !r.alive) {
                return null;
            }

            return ResolveTarget(r.native);
        }

        private static TweenMeta SlideMeta(string presetName) {

            TweenPreset preset = TweenPresets.Get(presetName);

            TweenMeta meta = new TweenMeta();
            meta.time = preset.time;
            meta.delay = preset.delay;
            meta.easeType = preset.easeType;
            meta.loopType = preset.loopType;
            meta.stopCurrent = true;   // per-channel: Move and Fade don't cancel each other

            return meta;
        }

        private static void SlideMove(ITweenTarget target, Vector3 pos, string presetName) {
            backend.Move(target, pos, SlideMeta(presetName));
        }

        private static void SlideFade(ITweenTarget target, float alpha, string presetName) {
            backend.Fade(target, alpha, SlideMeta(presetName));
        }

        public static void Cancel(UIRef r) {

            if (r == null || !r.alive) {
                return;
            }

            ITweenTarget target = ResolveTarget(r.native);

            if (target != null) {
                backend.Cancel(target);
            }
        }
    }
}