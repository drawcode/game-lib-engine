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

namespace Engine.Utility {

    public enum TweenLib {
        none,
        iTween,
        leanTween,
        nguiUITweener
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

        // LOOP TYPES

        public static T ConvertLibLoopType<T>(TweenLoopType loopType) {

            T libType = default(T);

            Type genericType = typeof(T);

            if(genericType == null) {

            }

#if USE_EASING_LEANTWEEN
            else if(genericType == typeof(LeanTweenType)) {

                if(loopType == TweenLoopType.once) {
                    libType = (T)(object)LeanTweenType.once;
                }
                else if(loopType == TweenLoopType.loop) {
                    libType = (T)(object)LeanTweenType.clamp;
                }
                else if(loopType == TweenLoopType.pingPong) {
                    libType = (T)(object)LeanTweenType.clamp;
                }
            }
#endif

#if USE_EASING_ITWEEN
            else if(genericType == typeof(iTween.LoopType)) {

                if(loopType == TweenLoopType.once) {
                    libType = (T)(object)iTween.LoopType.none;
                }
                else if(loopType == TweenLoopType.loop) {
                    libType = (T)(object)iTween.LoopType.loop;
                }
                else if(loopType == TweenLoopType.pingPong) {
                    libType = (T)(object)iTween.LoopType.pingPong;
                }
            }
#endif

#if USE_EASING_NGUI
            else if(genericType == typeof(UITweener.Style)) {

                if(loopType == TweenLoopType.once) {
                    libType = (T)(object)UITweener.Style.Once;
                }
                else if(loopType == TweenLoopType.loop) {
                    libType = (T)(object)UITweener.Style.Loop;
                }
                else if(loopType == TweenLoopType.pingPong) {
                    libType = (T)(object)UITweener.Style.PingPong;
                }
            }
#endif

            return libType;
        }

        public static TweenLoopType ConvertTweensLoopType<T>(T loopTypeLib) {

            TweenLoopType loopType = TweenLoopType.loop;

            Type genericType = typeof(T);

            if(genericType == null) {

            }

#if USE_EASING_LEANTWEEN
            else if(genericType == typeof(LeanTweenType)) {

                LeanTweenType libType = (LeanTweenType)(object)genericType;

                if(libType == LeanTweenType.once) {
                    loopType = TweenLoopType.once;
                }
                else if(libType == LeanTweenType.clamp) {
                    loopType = TweenLoopType.loop;
                }
                else if(libType == LeanTweenType.pingPong) {
                    loopType = TweenLoopType.pingPong;
                }
            }
#endif

#if USE_EASING_ITWEEN
            else if(genericType == typeof(iTween.LoopType)) {

                iTween.LoopType libType = (iTween.LoopType)(object)genericType;

                if(libType == iTween.LoopType.none) {
                    loopType = TweenLoopType.once;
                }
                else if(libType == iTween.LoopType.loop) {
                    loopType = TweenLoopType.loop;
                }
                else if(libType == iTween.LoopType.pingPong) {
                    loopType = TweenLoopType.pingPong;
                }
            }
#endif

#if USE_EASING_NGUI
            else if(genericType == typeof(UITweener.Style)) {

                UITweener.Style libType = (UITweener.Style)(object)genericType;

                if(libType == UITweener.Style.Once) {
                    loopType = TweenLoopType.once;
                }
                else if(libType == UITweener.Style.Loop) {
                    loopType = TweenLoopType.loop;
                }
                else if(libType == UITweener.Style.PingPong) {
                    loopType = TweenLoopType.pingPong;
                }
            }
#endif

            return loopType;
        }

        // EASE TYPES

        public static T ConvertLibEaseType<T>(TweenEaseType easeType) {

            T libType = default(T);

            Type genericType = typeof(T);

            if(genericType == null) {

            }

#if USE_EASING_LEANTWEEN
            else if(genericType == typeof(LeanTweenType)) {

                if(easeType == TweenEaseType.linear) {
                    libType = (T)(object)LeanTweenType.linear;
                }
                else if(easeType == TweenEaseType.quadEaseInOut) {
                    libType = (T)(object)LeanTweenType.easeInOutQuad;
                }
                else if(easeType == TweenEaseType.quadEaseIn) {
                    libType = (T)(object)LeanTweenType.easeInQuad;
                }
                else if(easeType == TweenEaseType.quadEaseOut) {
                    libType = (T)(object)LeanTweenType.easeOutQuad;
                }
            }
#endif

#if USE_EASING_ITWEEN
            else if(genericType == typeof(iTween.EaseType)) {

                if(easeType == TweenEaseType.linear) {
                    libType = (T)(object)iTween.EaseType.linear;
                }
                else if(easeType == TweenEaseType.quadEaseInOut) {
                    libType = (T)(object)iTween.EaseType.easeInOutQuad;
                }
                else if(easeType == TweenEaseType.quadEaseIn) {
                    libType = (T)(object)iTween.EaseType.easeInQuad;
                }
                else if(easeType == TweenEaseType.quadEaseOut) {
                    libType = (T)(object)iTween.EaseType.easeOutQuad;
                }
            }
#endif

#if USE_EASING_NGUI
            else if(genericType == typeof(UITweener.Method)) {

                if(easeType == TweenEaseType.linear) {
                    libType = (T)(object)UITweener.Method.Linear;
                }
                else if(easeType == TweenEaseType.quadEaseInOut) {
                    libType = (T)(object)UITweener.Method.EaseInOut;
                }
                else if(easeType == TweenEaseType.quadEaseIn) {
                    libType = (T)(object)UITweener.Method.EaseIn;
                }
                else if(easeType == TweenEaseType.quadEaseOut) {
                    libType = (T)(object)UITweener.Method.EaseOut;
                }
            }
#endif

            return libType;
        }

        public static TweenEaseType ConvertTweensEaseType<T>(T loopTypeLib) {

            TweenEaseType easeType = TweenEaseType.linear;

            Type genericType = typeof(T);

            if(genericType == null) {

            }

#if USE_EASING_LEANTWEEN
            else if(genericType == typeof(LeanTweenType)) {

                LeanTweenType libType = (LeanTweenType)(object)genericType;

                if(libType == LeanTweenType.linear) {
                    easeType = TweenEaseType.linear;
                }
                else if(libType == LeanTweenType.easeInOutQuad) {
                    easeType = TweenEaseType.quadEaseInOut;
                }
                else if(libType == LeanTweenType.easeOutQuad) {
                    easeType = TweenEaseType.quadEaseOut;
                }
                else if(libType == LeanTweenType.easeInQuad) {
                    easeType = TweenEaseType.quadEaseIn;
                }
            }
#endif

#if USE_EASING_ITWEEN
            else if(genericType == typeof(iTween.LoopType)) {

                iTween.EaseType libType = (iTween.EaseType)(object)genericType;

                if(libType == iTween.EaseType.linear) {
                    easeType = TweenEaseType.linear;
                }
                else if(libType == iTween.EaseType.easeInOutQuad) {
                    easeType = TweenEaseType.quadEaseInOut;
                }
                else if(libType == iTween.EaseType.easeOutQuad) {
                    easeType = TweenEaseType.quadEaseOut;
                }
                else if(libType == iTween.EaseType.easeInQuart) {
                    easeType = TweenEaseType.quadEaseIn;
                }
            }
#endif

#if USE_EASING_NGUI
            else if(genericType == typeof(UITweener.Style)) {

                UITweener.Method libType = (UITweener.Method)(object)genericType;

                if(libType == UITweener.Method.Linear) {
                    easeType = TweenEaseType.linear;
                }
                else if(libType == UITweener.Method.EaseInOut) {
                    easeType = TweenEaseType.quadEaseInOut;
                }
                else if(libType == UITweener.Method.EaseOut) {
                    easeType = TweenEaseType.quadEaseOut;
                }
                else if(libType == UITweener.Method.EaseIn) {
                    easeType = TweenEaseType.quadEaseIn;
                }
            }
#endif

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

            if(go == null) {
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

            if(meta.go == null) {
                return;
            }

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3 || USE_EASING_NGUI
            meta.lib = TweenLib.nguiUITweener;
#endif

            Action onBegin = () => {

            };

            Action onFinish = () => {

            };

            Action onTick = () => {

            };

            if(meta.onStart != null) {
                onBegin = onBegin.CombineAction(meta.onStart);
            }

            if(meta.onComplete != null) {
                onFinish = onFinish.CombineAction(meta.onComplete);
            }

            if(meta.onFinal != null) {
                onFinish = onFinish.CombineAction(meta.onFinal);
            }

            if(meta.onUpdate != null) {
                onTick = onTick.CombineAction(meta.onUpdate);
            }

            if(meta.lib == TweenLib.none) {

            }

#if USE_EASING_ITWEEN
            else if(meta.lib == TweenLib.iTween) {

                if(meta.stopCurrent) {
                    iTween.Stop(meta.go);
                }

                iTween.LoopType loopTypeLib =
                    ConvertLibLoopType<iTween.LoopType>(meta.loopType);

                iTween.EaseType easeTypeLib =
                    ConvertLibEaseType<iTween.EaseType>(meta.easeType);

                Hashtable hash = iTween.Hash(
                    "position", pos,
                    "time", meta.time,
                    "delay", meta.delay,
                    "looptype", loopTypeLib,
                    "easetype", easeTypeLib,
                    "islocal", meta.coord == TweenCoord.local,
                    "onstart", "OnTweenBegin",
                    "onstartparams", onBegin,
                    "oncomplete", "OnTweenFinish",
                    "oncompleteparams", onFinish);

                iTween.MoveTo(meta.go, hash);
            }
#endif

#if USE_EASING_LEANTWEEN
            else if(meta.lib == TweenLib.leanTween) {

                if(meta.stopCurrent) {
                    LeanTween.cancel(meta.go);
                }

                LTDescr info = null;

                if(meta.coord == TweenCoord.local) {
                    info =
                        LeanTween.moveLocal(meta.go, pos, meta.time)
                        .setDelay(meta.delay).pause();
                }
                else {
                    info =
                        LeanTween.move(meta.go, pos, meta.time)
                        .setDelay(meta.delay).pause();
                }

                LeanTweenType loopTypeLib =
                    ConvertLibLoopType<LeanTweenType>(meta.loopType);

                LeanTweenType easeTypeLib =
                    ConvertLibEaseType<LeanTweenType>(meta.easeType);

                info.setLoopType(loopTypeLib);
                info.setEase(easeTypeLib);

                info.setOnStart(onBegin);
                info.setOnComplete(onFinish);
                //info.setOnUpdate(onTick);

                if(meta.onUpdate != null) {
                    //info.setOnUpdate(onUpdate);
                }

                info.resume();
            }
#endif

#if USE_EASING_NGUI
            else if(meta.lib == TweenLib.nguiUITweener) {

                UITweener.Style loopTypeLib =
                    ConvertLibLoopType<UITweener.Style>(meta.loopType);

                UITweener.Method easeTypeLib =
                    ConvertLibEaseType<UITweener.Method>(meta.easeType);

                UITweenerUtil.MoveTo(
                    meta.go,
                    easeTypeLib, loopTypeLib,
                    meta.time, meta.delay, pos);

                //OnTweenBegin(onBegin);
                //OnTweenFinish(onFinish);
                //OnTweenTick(onTick);
            }
#endif
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

            if(go == null) {
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

            if(meta.go == null) {
                return;
            }


#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3 || USE_EASING_NGUI
            meta.lib = TweenLib.nguiUITweener;
#endif

            Action onBegin = () => {

            };

            Action onFinish = () => {

            };

            Action onTick = () => {

            };

            if(meta.onStart != null) {
                onBegin = onBegin.CombineAction(meta.onStart);
            }

            if(meta.onComplete != null) {
                onFinish = onFinish.CombineAction(meta.onComplete);
            }

            if(meta.onFinal != null) {
                onFinish = onFinish.CombineAction(meta.onFinal);
            }

            if(meta.onUpdate != null) {
                onTick = onTick.CombineAction(meta.onUpdate);
            }

            if(meta.lib == TweenLib.none) {

            }

#if USE_EASING_ITWEEN
            else if(meta.lib == TweenLib.iTween) {

                if(meta.stopCurrent) {
                    iTween.Stop(meta.go);
                }

                iTween.LoopType loopTypeLib =
                    ConvertLibLoopType<iTween.LoopType>(meta.loopType);

                iTween.EaseType easeTypeLib =
                    ConvertLibEaseType<iTween.EaseType>(meta.easeType);

                Hashtable hash = iTween.Hash(
                    "position", pos,
                    "time", meta.time,
                    "delay", meta.delay,
                    "looptype", loopTypeLib,
                    "easetype", easeTypeLib,
                    "islocal", meta.coord == TweenCoord.local,
                    "onstart", "OnTweenBegin",
                    "onstartparams", onBegin,
                    "oncomplete", "OnTweenFinish",
                    "oncompleteparams", onFinish);

                iTween.ScaleTo(meta.go, hash);
            }
#endif

#if USE_EASING_LEANTWEEN
            else if(meta.lib == TweenLib.leanTween) {

                if(meta.stopCurrent) {
                    LeanTween.cancel(meta.go);
                }

                LTDescr info = null;

                if(meta.coord == TweenCoord.local) {
                    info =
                        LeanTween.scale(meta.go, pos, meta.time)
                        .setDelay(meta.delay).pause();
                }
                else {
                    info =
                        LeanTween.scale(meta.go, pos, meta.time)
                        .setDelay(meta.delay).pause();
                }

                LeanTweenType loopTypeLib =
                    ConvertLibLoopType<LeanTweenType>(meta.loopType);

                LeanTweenType easeTypeLib =
                    ConvertLibEaseType<LeanTweenType>(meta.easeType);

                info.setLoopType(loopTypeLib);
                info.setEase(easeTypeLib);

                info.setOnStart(onBegin);
                info.setOnComplete(onFinish);
                //info.setOnUpdate(onTick);

                if(meta.onUpdate != null) {
                    //info.setOnUpdate(onUpdate);
                }

                info.resume();
            }
#endif

#if USE_EASING_NGUI
            else if(meta.lib == TweenLib.nguiUITweener) {

                //UITweener.Style loopTypeLib =
                //    ConvertLibLoopType<UITweener.Style>(meta.loopType);

                //UITweener.Method easeTypeLib =
                //    ConvertLibEaseType<UITweener.Method>(meta.easeType);

                //UITweenerUtil.Scal(
                //    meta.go,
                //    easeTypeLib, loopTypeLib,
                //    meta.time, meta.delay, pos);

                //OnTweenBegin(onBegin);
                //OnTweenFinish(onFinish);
                //OnTweenTick(onTick);
            }
#endif
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

            if(meta.go == null) {
                return;
            }

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3 || USE_EASING_NGUI
            meta.lib = TweenLib.nguiUITweener;
#endif

            Action onBegin = () => {

            };

            Action onFinish = () => {

            };

            Action onTick = () => {

            };

            if(meta.onStart != null) {
                onBegin = onBegin.CombineAction(meta.onStart);
            }

            if(meta.onComplete != null) {
                onFinish = onFinish.CombineAction(meta.onComplete);
            }

            if(meta.onFinal != null) {
                onFinish = onFinish.CombineAction(meta.onFinal);
            }

            if(meta.onUpdate != null) {
                onTick = onTick.CombineAction(meta.onUpdate);
            }

            if(meta.lib == TweenLib.none) {

            }

#if USE_EASING_ITWEEN
            else if(meta.lib == TweenLib.iTween) {

                if(meta.stopCurrent) {
                    iTween.Stop(meta.go);
                }

                iTween.LoopType loopTypeLib =
                    ConvertLibLoopType<iTween.LoopType>(meta.loopType);

                iTween.EaseType easeTypeLib =
                    ConvertLibEaseType<iTween.EaseType>(meta.easeType);

                Hashtable hash = iTween.Hash(
                    "rotation", pos,
                    "time", meta.time,
                    "delay", meta.delay,
                    "looptype", loopTypeLib,
                    "easetype", easeTypeLib,
                    "islocal", meta.coord == TweenCoord.local,
                    "onstart", "OnTweenBegin",
                    "onstartparams", onBegin,
                    "oncomplete", "OnTweenFinish",
                    "oncompleteparams", onFinish);

                iTween.RotateTo(meta.go, hash);
            }
#endif

#if USE_EASING_LEANTWEEN
            else if(meta.lib == TweenLib.leanTween) {

                if(meta.stopCurrent) {
                    LeanTween.cancel(meta.go);
                }

                LTDescr info = null;

                if(meta.coord == TweenCoord.local) {
                    info =
                        LeanTween.rotateLocal(meta.go, pos, meta.time)
                        .setDelay(meta.delay).pause();
                }
                else {
                    info =
                        LeanTween.rotate(meta.go, pos, meta.time)
                        .setDelay(meta.delay).pause();
                }

                LeanTweenType loopTypeLib =
                    ConvertLibLoopType<LeanTweenType>(meta.loopType);

                LeanTweenType easeTypeLib =
                    ConvertLibEaseType<LeanTweenType>(meta.easeType);

                info.setLoopType(loopTypeLib);
                info.setEase(easeTypeLib);

                info.setOnStart(onBegin);
                info.setOnComplete(onFinish);
                //info.setOnUpdate(onTick);

                if(meta.onUpdate != null) {
                    //info.setOnUpdate(onUpdate);
                }

                info.resume();
            }
#endif

#if USE_EASING_NGUI
            else if(meta.lib == TweenLib.nguiUITweener) {

                UITweener.Style loopTypeLib =
                    ConvertLibLoopType<UITweener.Style>(meta.loopType);

                UITweener.Method easeTypeLib =
                    ConvertLibEaseType<UITweener.Method>(meta.easeType);

                UITweenerUtil.RotateTo(
                    meta.go,
                    easeTypeLib, loopTypeLib,
                    meta.time, meta.delay, pos);

                //OnTweenBegin(onBegin);
                //OnTweenFinish(onFinish);
                //OnTweenTick(onTick);
            }
#endif
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

            if(go == null) {
                return;
            }

            TweenLib lib = TweenLib.leanTween;

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3 || USE_EASING_NGUI

            if(go.Has<UISlicedSprite>()
                || go.Has<UISprite>()
                || go.Has<UITiledSprite>()) {

                lib = TweenLib.nguiUITweener;
            }
#endif

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

            if(go == null) {
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

        public static void FadeToObject(
            TweenMeta meta,
            float alpha) {

            if(meta.go == null) {
                return;
            }

            Action onBegin = () => {

            };

            Action onFinish = () => {

            };

            Action onTick = () => {

            };

            if(meta.onStart != null) {
                onBegin = onBegin.CombineAction(meta.onStart);
            }

            if(meta.onComplete != null) {
                onFinish = onFinish.CombineAction(meta.onComplete);
            }

            if(meta.onFinal != null) {
                onFinish = onFinish.CombineAction(meta.onFinal);
            }

            if(meta.onUpdate != null) {
                onTick = onTick.CombineAction(meta.onUpdate);
            }

            if(alpha > 0f) {
                onBegin = onBegin.CombineAction(() => {
                    meta.go.Show();
                });
            }

            if(alpha == 0f) {
                onFinish = onFinish.CombineAction(() => {
                    meta.go.Hide();
                });
            }

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3 || USE_EASING_NGUI

            if(meta.go.Has<UISlicedSprite>()
                || meta.go.Has<UISprite>()
                || meta.go.Has<UITiledSprite>()) {

                meta.lib = TweenLib.nguiUITweener;
            }
#endif

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

#if USE_EASING_ITWEEN
            if(meta.lib == TweenLib.iTween) {

                if(meta.stopCurrent) {
                    iTween.Stop(meta.go);
                }

                iTween.LoopType loopTypeLib =
                    ConvertLibLoopType<iTween.LoopType>(meta.loopType);

                iTween.EaseType easeTypeLib =
                    ConvertLibEaseType<iTween.EaseType>(meta.easeType);

                Hashtable hash = iTween.Hash(
                    "alpha", alpha,
                    "time", meta.time,
                    "delay", meta.delay,
                    "looptype", loopTypeLib,
                    "easetype", easeTypeLib,
                    "islocal", meta.coord == TweenCoord.local,
                    "onstart", "OnTweenBegin",
                    "onstartparams", onBegin,
                    "oncomplete", "OnTweenFinish",
                    "oncompleteparams", onFinish);

                iTween.FadeTo(meta.go, hash);
            }
#endif

#if USE_EASING_LEANTWEEN
            else if(meta.lib == TweenLib.leanTween) {

                if(meta.stopCurrent) {
                    LeanTween.cancel(meta.go);
                }

                LTDescr info = null;

                if(meta.go.Has<Image>()) {
                    info = LeanTween.alpha(
                        meta.go.Get<Image>().rectTransform, alpha, meta.time).setDelay(meta.delay).pause();
                }
                else if(meta.go.Has<CanvasGroup>()) {
                    info = LeanTween.alphaCanvas(
                        meta.go.Get<CanvasGroup>(), alpha, meta.time).setDelay(meta.delay).pause();
                }
                else {
                    info = LeanTween.alpha(
                        meta.go, alpha, meta.time).setDelay(meta.delay).pause();
                }

                LeanTweenType loopTypeLib =
                    ConvertLibLoopType<LeanTweenType>(meta.loopType);

                LeanTweenType easeTypeLib =
                    ConvertLibEaseType<LeanTweenType>(meta.easeType);

                info.setLoopType(loopTypeLib);
                info.setEase(easeTypeLib);

                info.setOnStart(onBegin);
                info.setOnComplete(onFinish);
                //info.setOnUpdate(onTick);

                if(meta.onUpdate != null) {
                    //info.setOnUpdate(onUpdate);
                }

                info.resume();
            }
#endif

#if USE_EASING_NGUI
            else if(meta.lib == TweenLib.nguiUITweener) {

                UITweener.Style loopTypeLib =
                    ConvertLibLoopType<UITweener.Style>(meta.loopType);

                UITweener.Method easeTypeLib =
                    ConvertLibEaseType<UITweener.Method>(meta.easeType);

                UITweenerUtil.FadeTo(
                    meta.go, easeTypeLib, loopTypeLib, meta.time, meta.delay, alpha);

                //OnTweenBegin(onBegin);
                //OnTweenFinish(onFinish);
                //OnTweenTick(onTick);
            }
            
#endif

            /*
             * TODO nested -a- marked objects to keep alpha on on nested when needed
             * ex: objectname-a-50 = alpha 50% on nested no matter parent
             * 
             */

            if(meta.lib != TweenLib.nguiUITweener) {

                foreach(Transform t in meta.go.transform) {
                    string toLook = "-a-";
                    int alphaMarker = t.name.IndexOf(toLook);
                    //string alphaObject = t.name;
                    if(alphaMarker > -1) {
                        // Fade it immediately
                        //FadeToObject(t.gameObject, alpha, meta.time, meta.delay);
                        // Fade to the correct value after initial fade in
                        string val = t.name.Substring(alphaMarker + toLook.Length);
                        if(!string.IsNullOrEmpty(val)) {
                            float valNumeric = 0f;
                            float.TryParse(val, out valNumeric);

                            if(valNumeric > 0f) {
                                valNumeric = valNumeric / 100f;

                                //FadeTo(t.gameObject, UITweener.Method.Linear, UITweener.Style.Once,
                                //    duration + .05f, duration + delay, valNumeric);

                                if(t.gameObject != null) {

                                    FadeToObject(meta.lib, t.gameObject, valNumeric, meta.time, meta.delay + .05f);
                                }
                            }
                        }
                    }
                    //FadeToObject(t.gameObject, alpha, meta.time, meta.delay);
                }
            }
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

            if(go == null) {
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

            if(meta.go == null) {
                return;
            }

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3 || USE_EASING_NGUI
            meta.lib = TweenLib.nguiUITweener;
#endif

            Action onBegin = () => {

            };

            Action onFinish = () => {

            };

            Action onTick = () => {

            };

            if(meta.onStart != null) {
                onBegin = onBegin.CombineAction(meta.onStart);
            }

            if(meta.onComplete != null) {
                onFinish = onFinish.CombineAction(meta.onComplete);
            }

            if(meta.onFinal != null) {
                onFinish = onFinish.CombineAction(meta.onFinal);
            }

            if(meta.onUpdate != null) {
                onTick = onTick.CombineAction(meta.onUpdate);
            }

            if(color.a > 0f) {
                onBegin = onBegin.CombineAction(() => {
                    meta.go.Show();
                });
            }

            if(color.a == 0f) {
                onFinish = onFinish.CombineAction(() => {
                    meta.go.Hide();
                });
            }

            if(meta.lib == TweenLib.none) {

            }

#if USE_EASING_ITWEEN
            else if(meta.lib == TweenLib.iTween) {

                if(meta.stopCurrent) {
                    iTween.Stop(meta.go);
                }

                iTween.LoopType loopTypeLib =
                    ConvertLibLoopType<iTween.LoopType>(meta.loopType);

                iTween.EaseType easeTypeLib =
                    ConvertLibEaseType<iTween.EaseType>(meta.easeType);

                Hashtable hash = iTween.Hash(
                    "color", color,
                    "time", meta.time,
                    "delay", meta.delay,
                    "looptype", loopTypeLib,
                    "easetype", easeTypeLib,
                    "islocal", meta.coord == TweenCoord.local,
                    "onstart", "OnTweenBegin",
                    "onstartparams", onBegin,
                    "oncomplete", "OnTweenFinish",
                    "oncompleteparams", onFinish);

                iTween.FadeTo(meta.go, hash);
            }
#endif

#if USE_EASING_LEANTWEEN
            else if(meta.lib == TweenLib.leanTween) {

                if(meta.stopCurrent) {
                    LeanTween.cancel(meta.go);
                }

                LTDescr info = null;

                if(meta.go.Has<Image>()) {
                    info = LeanTween.color(
                        meta.go.Get<Image>().rectTransform, color, meta.time).setDelay(meta.delay).pause();
                }
                else if(meta.go.Has<CanvasGroup>()) {
                    info = LeanTween.alphaCanvas(
                        meta.go.Get<CanvasGroup>(), color.a, meta.time).setDelay(meta.delay).pause();
                }
                else {
                    info = LeanTween.color(
                        meta.go, color, meta.time).setDelay(meta.delay).pause();
                }

                LeanTweenType loopTypeLib =
                    ConvertLibLoopType<LeanTweenType>(meta.loopType);

                LeanTweenType easeTypeLib =
                    ConvertLibEaseType<LeanTweenType>(meta.easeType);

                info.setLoopType(loopTypeLib);
                info.setEase(easeTypeLib);

                info.setOnStart(onBegin);
                info.setOnComplete(onFinish);
                //info.setOnUpdate(onTick);

                if(meta.onUpdate != null) {
                    //info.setOnUpdate(onUpdate);
                }

                info.resume();
            }
#endif

#if USE_EASING_NGUI
            else if(meta.lib == TweenLib.nguiUITweener) {

                UITweener.Style loopTypeLib =
                    ConvertLibLoopType<UITweener.Style>(meta.loopType);

                UITweener.Method easeTypeLib =
                    ConvertLibEaseType<UITweener.Method>(meta.easeType);

                UITweenerUtil.ColorTo(
                    meta.go, easeTypeLib, loopTypeLib, meta.time, meta.delay, color);

                //OnTweenBegin(onBegin);
                //OnTweenFinish(onFinish);
                //OnTweenTick(onTick);
            }
#endif

            /*
             * TODO nested -a- marked objects to keep alpha on on nested when needed
             * ex: objectname-a-50 = alpha 50% on nested no matter parent
             * 
             */
            foreach(Transform t in meta.go.transform) {
                string toLook = "-a-";
                int alphaMarker = t.name.IndexOf(toLook);
                //string alphaObject = t.name;
                if(alphaMarker > -1) {
                    // Fade it immediately
                    //FadeToObject(t.gameObject, alpha, meta.time, meta.delay);
                    // Fade to the correct value after initial fade in
                    string val = t.name.Substring(alphaMarker + toLook.Length);
                    if(!string.IsNullOrEmpty(val)) {
                        float valNumeric = 0f;
                        float.TryParse(val, out valNumeric);

                        if(valNumeric > 0f) {
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

            if(go == null) {
                return;
            }

            TweenObjectState(go, pos, 1f, coord, fade, time, delay);
        }

        public static void HideObject(
            GameObject go, Vector3 pos,
            TweenCoord coord = TweenCoord.local, bool fade = true,
            float time = .5f, float delay = 0f) {

            if(go == null) {
                return;
            }

            TweenObjectState(go, pos, 0f, coord, fade, time, delay);
        }

        //

        public static void TweenObjectState(
            GameObject go, Vector3 pos, float alpha = 1f,
            TweenCoord coord = TweenCoord.local, bool fade = true,
            float time = .5f, float delay = 0f) {

            if(go == null) {
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

            if(fade) {

                bool found = false;

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3 || USE_EASING_NGUI

                if(go.Has<UISlicedSprite>()
                    || go.Has<UISprite>()
                    || go.Has<UITiledSprite>()) {

                    found = true;

                    TweenUtil.FadeToObject(TweenLib.nguiUITweener, go, alpha, time, delay, true, coord);
                }
#endif
                if(!found) {

                    TweenUtil.FadeToObject(go, alpha, time, delay, true, coord);
                }
            }

            TweenUtil.MoveToObject(
                go, pos, time, delay, false, coord);
        }
    }
}