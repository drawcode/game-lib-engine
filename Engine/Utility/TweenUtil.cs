using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

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
                    "easetype", easeTypeLib,
                    "islocal", meta.coord == TweenCoord.local,
                    "onstart", "OnTweenBegin",
                    "onstartparams", onBegin,
                    "oncomplete", "OnTweenFinish",
                    "oncompleteparams", onFinish);

                iTween.MoveTo(meta.go, hash);
            }
            else if (meta.lib == TweenLib.leanTween) {

                if (meta.stopCurrent) {
                    LeanTween.cancel(meta.go);
                }

                LTDescr info = null;

                if (meta.coord == TweenCoord.local) {
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

                if (meta.onUpdate != null) {
                    //info.setOnUpdate(onUpdate);
                }

                info.resume();
            }
            else if (meta.lib == TweenLib.nguiUITweener) {

                UITweenerUtil.MoveTo(
                    meta.go,
                    UITweener.Method.EaseInOut, UITweener.Style.Loop,
                    meta.time, meta.delay, pos);

                //OnTweenBegin(onBegin);
                //OnTweenFinish(onFinish);
                //OnTweenTick(onTick);
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

            FadeToObjectLeanTween(
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
                    "easetype", easeTypeLib,
                    "islocal", meta.coord == TweenCoord.local,
                    "onstart", "OnTweenBegin",
                    "onstartparams", onBegin,
                    "oncomplete", "OnTweenFinish",
                    "oncompleteparams", onFinish);

                iTween.FadeTo(meta.go, hash);
            }
            else if (meta.lib == TweenLib.leanTween) {

                if (meta.stopCurrent) {
                    LeanTween.cancel(meta.go);
                }
                                
                LTDescr info = null;

                if (meta.go.Has<Image>()) {
                    info = LeanTween.alpha(
                        meta.go.Get<Image>().rectTransform, alpha, meta.time).setDelay(meta.delay).pause();
                }
                else if (meta.go.Has<CanvasGroup>()) {
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
                
                if (meta.onUpdate != null) {
                    //info.setOnUpdate(onUpdate);
                }

                info.resume();
            }
            else if (meta.lib == TweenLib.nguiUITweener) {

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

        //
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

            TweenObjectState(go, pos, 1f, coord, fade, time, delay);
        }

        public static void HideObject(
            GameObject go, Vector3 pos, 
            TweenCoord coord = TweenCoord.local, bool fade = true,
            float time = .5f, float delay = 0f) {

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
    }
}