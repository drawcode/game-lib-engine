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

    public enum TweenCoordSpace {
        world,
        local 
    }

    public class Tweens {

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

        public static TweenLoopType ConvertTweensLoopType<T>(T loopTypeLib) {

            TweenLoopType loopType = TweenLoopType.loop;

            Type genericType = typeof(T);

            if (genericType == typeof(LeanTweenType)) {

                LeanTweenType libType = (LeanTweenType)(object)genericType;

                if (libType == LeanTweenType.once) {
                    loopType = TweenLoopType.once;
                }
                else if(libType == LeanTweenType.clamp) {
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
        // MOVE

        public static void MoveToObject(
            GameObject go, 
            Vector3 pos,
            float time = 1f, float delay = 0f, 
            TweenEaseType easeType = TweenEaseType.quadEaseInOut, 
            TweenLoopType loopType = TweenLoopType.once,
            Action onComplete = null, Action onStart = null, Action onUpdate = null) {

            MoveToObject(TweenLib.leanTween, 
                go, pos, time, delay, easeType, loopType, onComplete, onStart, onUpdate);
        }

        public static void MoveToObject(
            TweenLib lib, 
            GameObject go, 
            Vector3 pos,
            float time = 1f, float delay = 0f,
            TweenEaseType easeType = TweenEaseType.quadEaseInOut,
            TweenLoopType loopType = TweenLoopType.once,
            Action onComplete = null, Action onStart = null, Action onUpdate = null) {

            if (go == null) {
                return;
            }

            if (lib == TweenLib.iTween) {

                iTween.LoopType loopTypeLib = ConvertLibLoopType<iTween.LoopType>(loopType);
                iTween.EaseType easeTypeLib = ConvertLibEaseType<iTween.EaseType>(easeType);

                Hashtable hash = iTween.Hash(
                    "position", pos, 
                    "time", time, 
                    "delay", delay, 
                    "looptype", loopTypeLib,
                    "easetype", easeTypeLib);

                iTween.MoveTo(go, hash);
            }
            else if (lib == TweenLib.leanTween) {

                LTDescr info = LeanTween.move(go, pos, time).setDelay(delay);

                LeanTweenType loopTypeLib = ConvertLibLoopType<LeanTweenType>(loopType);
                
                info.setLoopType(ConvertLibLoopType<LeanTweenType>(loopType));
                
                if (onComplete != null) {
                    //info.setOnCompleteOnStart(true);
                    info.setOnComplete(onComplete);
                }

                if (onStart != null) {
                    //info.setOnCompleteOnStart(true);
                    info.setOnStart(onStart);
                }

                if (onUpdate != null) {
                    //info.setOnUpdate(onUpdate);
                }
            }
            else if (lib == TweenLib.nguiUITweener) {
                UITweenerUtil.MoveTo(go, UITweener.Method.EaseInOut, UITweener.Style.Loop, time, delay, pos);
            }
        }

        // --------------------------------------------------------------------
        // FADE

        public static void FadeToObjectLeanTween(
            GameObject go, 
            float alpha,
            float time = 1f, float delay = 0f,
            TweenEaseType easeType = TweenEaseType.quadEaseInOut,
            TweenLoopType loopType = TweenLoopType.once,
            Action onComplete = null, Action onStart = null, Action onUpdate = null) {

            FadeToObject(TweenLib.leanTween, 
                go, alpha, time, delay, easeType, loopType, onComplete, onStart, onUpdate);
        }

        public static void FadeToObjectiTween(
            GameObject go,
            float alpha,
            float time = 1f, float delay = 0f,
            TweenEaseType easeType = TweenEaseType.quadEaseInOut,
            TweenLoopType loopType = TweenLoopType.once,
            Action onComplete = null, Action onStart = null, Action onUpdate = null) {

            FadeToObject(TweenLib.iTween,
                go, alpha, time, delay, easeType, loopType, onComplete, onStart, onUpdate);
        }

        public static void FadeToObjectUITweener(
            GameObject go,
            float alpha,
            float time = 1f, float delay = 0f,
            TweenEaseType easeType = TweenEaseType.quadEaseInOut,
            TweenLoopType loopType = TweenLoopType.once,
            Action onComplete = null, Action onStart = null, Action onUpdate = null) {

            FadeToObject(TweenLib.nguiUITweener,
                go, alpha, time, delay, easeType, loopType, onComplete, onStart, onUpdate);
        }

        public static void FadeToObject(
            TweenLib lib,
            GameObject go,
            float alpha,
            float time = 1f, float delay = 0f,
            TweenEaseType easeType = TweenEaseType.quadEaseInOut,
            TweenLoopType loopType = TweenLoopType.once,
            Action onComplete = null, Action onStart = null, Action onUpdate = null) {

            if (go == null) {
                return;
            }

            if (lib == TweenLib.iTween) {
                
                iTween.LoopType loopTypeLib = ConvertLibLoopType<iTween.LoopType>(loopType);
                iTween.EaseType easeTypeLib = ConvertLibEaseType<iTween.EaseType>(easeType);

                Hashtable hash = iTween.Hash(
                    "alpha", alpha,
                    "time", time,
                    "delay", delay,
                    "looptype", loopTypeLib,
                    "easetype", easeTypeLib);

                iTween.FadeTo(go, hash);
            }
            else if (lib == TweenLib.leanTween) {

                LTDescr info = LeanTween.alpha(go, alpha, time).setDelay(delay);

                LeanTweenType loopTypeLib = ConvertLibLoopType<LeanTweenType>(loopType);
                LeanTweenType easeTypeLib = ConvertLibEaseType<LeanTweenType>(easeType);

                info.setLoopType(loopTypeLib);
                info.setLoopType(easeTypeLib);

                if (onComplete != null) {
                    //info.setOnCompleteOnStart(true);
                    info.setOnComplete(onComplete);
                }

                if (onStart != null) {
                    //info.setOnCompleteOnStart(true);
                    info.setOnStart(onStart);
                }

                if (onUpdate != null) {
                    //info.setOnUpdate(onUpdate);
                }
            }
            else if (lib == TweenLib.nguiUITweener) {

                UITweener.Style loopTypeLib = ConvertLibLoopType<UITweener.Style>(loopType);
                UITweener.Method easeTypeLib = ConvertLibEaseType<UITweener.Method>(easeType);

                UITweenerUtil.FadeTo(go, easeTypeLib, loopTypeLib, time, delay, alpha);
            }
        }
    }
}
