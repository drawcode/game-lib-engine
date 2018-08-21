using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class AnimatorHelper {

    public static void SetBool(Animator animator, string key, bool val) {

        if(animator == null) {
            return;
        }

        if(animator != null
            && animator.enabled
            && animator.gameObject.activeInHierarchy
            && animator.gameObject.activeSelf) {

            animator.SetBool(key, val);
        }
    }

    public static void SetFloat(Animator animator, string key, float val) {

        if(animator == null) {
            return;
        }

        if(animator != null
            && animator.enabled
            && animator.gameObject.activeInHierarchy
            && animator.gameObject.activeSelf) {

            animator.SetFloat(key, val);
        }
    }

    public static void PlayOneShotBool(Animator animator, string paramName) {
        CoroutineUtil.Start(PlayOneShotBoolCo(animator, paramName));
    }

    public static IEnumerator PlayOneShotBoolCo(Animator animator, string paramName) {
        if(animator != null
            && animator.enabled
            && animator.gameObject.activeInHierarchy
            && animator.gameObject.activeSelf) {
            animator.SetBool(paramName, true);
            yield return null;
            animator.SetBool(paramName, false);
        }
    }

    public static void PlayOneShotFloat(Animator animator, string paramName) {
        CoroutineUtil.Start(PlayOneShotFloatCo(animator, paramName, 1.0f, 0.0f));
    }

    public static void PlayOneShotFloat(Animator animator, string paramName, float startValue, float endValue) {
        CoroutineUtil.Start(PlayOneShotFloatCo(animator, paramName, startValue, endValue));
    }

    public static IEnumerator PlayOneShotFloatCo(Animator animator, string paramName, float startValue, float endValue) {
        if(animator != null
            && animator.enabled
            && animator.gameObject.activeInHierarchy
            && animator.gameObject.activeSelf) {
            animator.SetFloat(paramName, 1.0f);
            yield return null;
            animator.SetFloat(paramName, 0.0f);
        }
    }
}