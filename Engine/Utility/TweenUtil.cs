using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Engine.Utility {

    public enum TweenLib {
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

        public TweenLib _lib = TweenLib.leanTween;

        public GameObject _go;
        public float _time = 1f;
        public float _delay = 0f;
        public TweenEaseType _easeType = TweenEaseType.quadEaseInOut;
        public TweenLoopType _loopType = TweenLoopType.once;
        public TweenCoord _coord = TweenCoord.world;
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

        public int increment = 0;

        // LOOP TYPES

        public static T ConvertLibLoopType<T>(TweenLoopType loopType) {

            T libType = default(T);

            Type genericType = typeof(T);

            if (genericType == typeof(LeanTweenType)) {

                if (loopType == TweenLoopType.once) {
                    libType = (T)(object)LeanTweenType.once;
                }
                else if (loopType == TweenLoopType.loop) {
                    libType = (T)(object)LeanTweenType.clamp;
                }
                else if (loopType == TweenLoopType.pingPong) {
                    libType = (T)(object)LeanTweenType.clamp;
                }
            }

            else if (genericType == typeof(iTween.LoopType)) {

                if (loopType == TweenLoopType.once) {
                    libType = (T)(object)iTween.LoopType.none;
                }
                else if (loopType == TweenLoopType.loop) {
                    libType = (T)(object)iTween.LoopType.loop;
                }
                else if (loopType == TweenLoopType.pingPong) {
                    libType = (T)(object)iTween.LoopType.pingPong;
                }
            }

            else if (genericType == typeof(UITweener.Style)) {

                if (loopType == TweenLoopType.once) {
                    libType = (T)(object)UITweener.Style.Once;
                }
                else if (loopType == TweenLoopType.loop) {
                    libType = (T)(object)UITweener.Style.Loop;
                }
                else if (loopType == TweenLoopType.pingPong) {
                    libType = (T)(object)UITweener.Style.PingPong;
                }
            }

            return libType;
        }

        public static TweenLoopType ConvertTweensLoopType<T>(T loopTypeLib) {

            TweenLoopType loopType = TweenLoopType.loop;

            Type genericType = typeof(T);

            if (genericType == typeof(LeanTweenType)) {

                LeanTweenType libType = (LeanTweenType)(object)genericType;

                if (libType == LeanTweenType.once) {
                    loopType = TweenLoopType.once;
                }
                else if (libType == LeanTweenType.clamp) {
                    loopType = TweenLoopType.loop;
                }
                else if (libType == LeanTweenType.pingPong) {
                    loopType = TweenLoopType.pingPong;
                }
            }
            else if (genericType == typeof(iTween.LoopType)) {

                iTween.LoopType libType = (iTween.LoopType)(object)genericType;

                if (libType == iTween.LoopType.none) {
                    loopType = TweenLoopType.once;
                }
                else if (libType == iTween.LoopType.loop) {
                    loopType = TweenLoopType.loop;
                }
                else if (libType == iTween.LoopType.pingPong) {
                    loopType = TweenLoopType.pingPong;
                }
            }
            else if (genericType == typeof(UITweener.Style)) {

                UITweener.Style libType = (UITweener.Style)(object)genericType;

                if (libType == UITweener.Style.Once) {
                    loopType = TweenLoopType.once;
                }
                else if (libType == UITweener.Style.Loop) {
                    loopType = TweenLoopType.loop;
                }
                else if (libType == UITweener.Style.PingPong) {
                    loopType = TweenLoopType.pingPong;
                }
            }

            return loopType;
        }

        // EASE TYPES

        public static T ConvertLibEaseType<T>(TweenEaseType easeType) {

            T libType = default(T);

            Type genericType = typeof(T);

            if (genericType == typeof(LeanTweenType)) {

                if (easeType == TweenEaseType.linear) {
                    libType = (T)(object)LeanTweenType.linear;
                }
                else if (easeType == TweenEaseType.quadEaseInOut) {
                    libType = (T)(object)LeanTweenType.easeInOutQuad;
                }
                else if (easeType == TweenEaseType.quadEaseIn) {
                    libType = (T)(object)LeanTweenType.easeInQuad;
                }
                else if (easeType == TweenEaseType.quadEaseOut) {
                    libType = (T)(object)LeanTweenType.easeOutQuad;
                }
            }

            else if (genericType == typeof(iTween.EaseType)) {

                if (easeType == TweenEaseType.linear) {
                    libType = (T)(object)iTween.EaseType.linear;
                }
                else if (easeType == TweenEaseType.quadEaseInOut) {
                    libType = (T)(object)iTween.EaseType.easeInOutQuad;
                }
                else if (easeType == TweenEaseType.quadEaseIn) {
                    libType = (T)(object)iTween.EaseType.easeInQuad;
                }
                else if (easeType == TweenEaseType.quadEaseOut) {
                    libType = (T)(object)iTween.EaseType.easeOutQuad;
                }
            }

            else if (genericType == typeof(UITweener.Method)) {

                if (easeType == TweenEaseType.linear) {
                    libType = (T)(object)UITweener.Method.Linear;
                }
                else if (easeType == TweenEaseType.quadEaseInOut) {
                    libType = (T)(object)UITweener.Method.EaseInOut;
                }
                else if (easeType == TweenEaseType.quadEaseIn) {
                    libType = (T)(object)UITweener.Method.EaseIn;
                }
                else if (easeType == TweenEaseType.quadEaseOut) {
                    libType = (T)(object)UITweener.Method.EaseOut;
                }
            }

            return libType;
        }

        public static TweenEaseType ConvertTweensEaseType<T>(T loopTypeLib) {

            TweenEaseType easeType = TweenEaseType.linear;

            Type genericType = typeof(T);

            if (genericType == typeof(LeanTweenType)) {

                LeanTweenType libType = (LeanTweenType)(object)genericType;

                if (libType == LeanTweenType.linear) {
                    easeType = TweenEaseType.linear;
                }
                else if (libType == LeanTweenType.easeInOutQuad) {
                    easeType = TweenEaseType.quadEaseInOut;
                }
                else if (libType == LeanTweenType.easeOutQuad) {
                    easeType = TweenEaseType.quadEaseOut;
                }
                else if (libType == LeanTweenType.easeInQuad) {
                    easeType = TweenEaseType.quadEaseIn;
                }
            }
            else if (genericType == typeof(iTween.LoopType)) {

                iTween.EaseType libType = (iTween.EaseType)(object)genericType;

                if (libType == iTween.EaseType.linear) {
                    easeType = TweenEaseType.linear;
                }
                else if (libType == iTween.EaseType.easeInOutQuad) {
                    easeType = TweenEaseType.quadEaseInOut;
                }
                else if (libType == iTween.EaseType.easeOutQuad) {
                    easeType = TweenEaseType.quadEaseOut;
                }
                else if (libType == iTween.EaseType.easeInQuart) {
                    easeType = TweenEaseType.quadEaseIn;
                }
            }
            else if (genericType == typeof(UITweener.Style)) {

                UITweener.Method libType = (UITweener.Method)(object)genericType;

                if (libType == UITweener.Method.Linear) {
                    easeType = TweenEaseType.linear;
                }
                else if (libType == UITweener.Method.EaseInOut) {
                    easeType = TweenEaseType.quadEaseInOut;
                }
                else if (libType == UITweener.Method.EaseOut) {
                    easeType = TweenEaseType.quadEaseOut;
                }
                else if (libType == UITweener.Method.EaseIn) {
                    easeType = TweenEaseType.quadEaseIn;
                }
            }

            return easeType;
        }


        // --------------------------------------------------------------------
        // META

        public static TweenMeta GetMetaDefault(
           TweenLib lib,
           GameObject go,
           float time = 1f, float delay = 0f,
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

        // --------------------------------------------------------------------
        // MOVE

        public static void MoveToObject(
           GameObject go,
           Vector3 pos,
           float time = 1f, float delay = 0f,
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
           float time = 1f, float delay = 0f,
           bool stopCurrent = true, 
           TweenCoord coord = TweenCoord.world,
           TweenEaseType easeType = TweenEaseType.quadEaseInOut,
           TweenLoopType loopType = TweenLoopType.once) {

            TweenMeta meta = 
                GetMetaDefault(
                    lib, go, time, delay, stopCurrent, coord, easeType, loopType);

            MoveToObject(meta, pos);
        }

        public static void MoveToObjectLeanTween(
           GameObject go,
           Vector3 pos,
           float time = 1f, float delay = 0f,
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
           float time = 1f, float delay = 0f,
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
           float time = 1f, float delay = 0f,
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

            if (meta.lib == TweenLib.iTween) {

                if (meta.stopCurrent) {
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
                    "easetype", easeTypeLib);

                iTween.MoveTo(meta.go, hash);
            }
            else if (meta.lib == TweenLib.leanTween) {

                if (meta.stopCurrent) {
                    LeanTween.cancel(meta.go);
                }

                LTDescr info =
                    LeanTween.move(meta.go, pos, meta.time).setDelay(meta.delay);

                LeanTweenType loopTypeLib =
                    ConvertLibLoopType<LeanTweenType>(meta.loopType);

                info.setLoopType(ConvertLibLoopType<LeanTweenType>(meta.loopType));

                if (meta.onComplete != null) {
                    //info.setOnCompleteOnStart(true);
                    info.setOnComplete(meta.onComplete);
                }

                if (meta.onStart != null) {
                    //info.setOnCompleteOnStart(true);
                    info.setOnStart(meta.onStart);
                }

                if (meta.onUpdate != null) {
                    //info.setOnUpdate(onUpdate);
                }
            }
            else if (meta.lib == TweenLib.nguiUITweener) {

                UITweenerUtil.MoveTo(
                    meta.go,
                    UITweener.Method.EaseInOut, UITweener.Style.Loop,
                    meta.time, meta.delay, pos);
            }
        }

        // --------------------------------------------------------------------
        // FADE

        public static void FadeToObject(
           GameObject go,
           float alpha,
           float time = 1f, float delay = 0f,
           bool stopCurrent = true,
           TweenCoord coord = TweenCoord.world,
           TweenEaseType easeType = TweenEaseType.quadEaseInOut,
           TweenLoopType loopType = TweenLoopType.once) {

            FadeToObjectLeanTween(
                go,
                alpha, time, delay, stopCurrent, coord, easeType, loopType);
        }

        public static void FadeToObject(
           TweenLib lib,
           GameObject go,
           float alpha,
           float time = 1f, float delay = 0f,
           bool stopCurrent = true,
           TweenCoord coord = TweenCoord.world,
           TweenEaseType easeType = TweenEaseType.quadEaseInOut,
           TweenLoopType loopType = TweenLoopType.once) {

            TweenMeta meta =
                GetMetaDefault(
                    lib, go, time, delay, stopCurrent, coord, easeType, loopType);

            FadeToObject(meta, alpha);
        }

        public static void FadeToObjectLeanTween(
           GameObject go,
           float alpha,
           float time = 1f, float delay = 0f,
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
           float time = 1f, float delay = 0f,
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
           float time = 1f, float delay = 0f,
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

            if (meta.go == null) {
                return;
            }

            if (meta.lib == TweenLib.iTween) {

                if (meta.stopCurrent) {
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
                    "easetype", easeTypeLib);

                iTween.FadeTo(meta.go, hash);
            }
            else if (meta.lib == TweenLib.leanTween) {

                if (meta.stopCurrent) {
                    LeanTween.cancel(meta.go);
                }

                LTDescr info = LeanTween.alpha(
                    meta.go, alpha, meta.time).setDelay(meta.delay);

                LeanTweenType loopTypeLib =
                    ConvertLibLoopType<LeanTweenType>(meta.loopType);

                LeanTweenType easeTypeLib =
                    ConvertLibEaseType<LeanTweenType>(meta.easeType);

                info.setLoopType(loopTypeLib);
                info.setLoopType(easeTypeLib);

                if (meta.onComplete != null) {
                    //info.setOnCompleteOnStart(true);
                    info.setOnComplete(meta.onComplete);
                }

                if (meta.onStart != null) {
                    //info.setOnCompleteOnStart(true);
                    info.setOnStart(meta.onStart);
                }

                if (meta.onUpdate != null) {
                    //info.setOnUpdate(onUpdate);
                }
            }
            else if (meta.lib == TweenLib.nguiUITweener) {

                UITweener.Style loopTypeLib =
                    ConvertLibLoopType<UITweener.Style>(meta.loopType);

                UITweener.Method easeTypeLib =
                    ConvertLibEaseType<UITweener.Method>(meta.easeType);

                UITweenerUtil.FadeTo(
                    meta.go, easeTypeLib, loopTypeLib, meta.time, meta.delay, alpha);
            }

            /*
             * TODO nested -a- marked objects to keep alpha on on nested when needed
             * ex: objectname-a-50 = alpha 50% on nested no matter parent
             * 
            foreach (Transform t in go.transform) {
                string toLook = "-a-";
                int alphaMarker = t.name.IndexOf(toLook);
                //string alphaObject = t.name;
                if (alphaMarker > -1) {
                    // Fade it immediately
                    FadeTo(t.gameObject, UITweener.Method.Linear, UITweener.Style.Once, 0f, 0f, 0f);
                    // Fade to the correct value after initial fade in
                    string val = t.name.Substring(alphaMarker + toLook.Length);
                    if (!string.IsNullOrEmpty(val)) {
                        float valNumeric = 0f;
                        float.TryParse(val, out valNumeric);

                        if (valNumeric > 0f) {
                            valNumeric = valNumeric / 100f;

                            FadeTo(t.gameObject, UITweener.Method.Linear, UITweener.Style.Once,
                                duration + .05f, duration + delay, valNumeric);
                        }
                    }
                }
                FadeInHandler(t.gameObject, duration, delay);
            }
            */
        }
    }
}