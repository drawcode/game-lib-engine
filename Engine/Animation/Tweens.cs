using System;
using System.Collections;
using System.Collections.Generic;
using Engine.Utility;
using UnityEngine;

namespace Engine.Animation {

    /*
    public class Tweens {
        private static volatile Tweens instance;
        private static System.Object syncRoot = new System.Object();

        public static Tweens Instance {
            get {
                if (instance == null) {
                    lock (syncRoot) {
                        if (instance == null)
                            instance = new Tweens();
                    }
                }

                return instance;
            }
        }

        public Tweens() {
        }

        public void FadeToObject(GameObject go, float alpha, float time, float delay) {
            if (go != null) {
                go.Show();

                //iTween.FadeTo(go, iTween.Hash("alpha", alpha,
                //                                  "time", time,
                //                                  "delay", delay));
                if (alpha < 0.001f) {
                    CoroutineUtil.Start(HideObjectCoroutine(go, time + delay + .1f));
                }
            }
        }

        public void FadeToObjectPingPong(GameObject go, float alpha, float time, float delay) {
            if (go != null) {
                go.Show();

                //iTween.FadeTo(go, iTween.Hash("alpha", alpha,
                //                                  "time", time,
                //                                  "delay", delay,
                //                                  "looptype", iTween.LoopType.pingPong));
            }
        }

        public IEnumerator HideObjectCoroutine(GameObject go, float waitTime) {
            yield return new WaitForSeconds(waitTime);
            go.Hide();
        }

        public void ScaleToObject(GameObject go, Vector3 scale, float time, float delay) {
            if (go != null) {
                //go.ScaleTo(scale, time, delay);
            }
        }

        public void MoveToObject(GameObject go, Vector3 position, float time, float delay) {
            if (go != null) {
                //go.MoveTo(position, time, delay);
            }
        }

        public void FadeFromObject(GameObject go, float alpha, float time, float delay) {
            if (go != null) {
                //go.FadeFrom(alpha, time, delay);
            }
        }

        public void ScaleFromObject(GameObject go, Vector3 scale, float time, float delay) {
            if (go != null) {
                //go.ScaleFrom(scale, time, delay);
            }
        }

        public void MoveFromObject(GameObject go, Vector3 position, float time, float delay) {
            if (go != null) {
               // go.MoveFrom(position, time, delay);
            }
        }
    }
    */
}