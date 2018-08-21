using System;
using System.Collections;
using System.Collections.Generic;
using Engine.Utility;
using UnityEngine;

public static class AnimatorExtentions {

    public static void ResetFloat(
        this Animator animator, string paramName) {

        if(animator == null) {
            return;
        }

        if(animator != null
            && animator.enabled) {

            animator.SetFloat(paramName, 0.0f);
        }
    }

    public static void SetBool(
        this Animator animator, string key, bool val) {

        if(animator == null) {
            return;
        }

        AnimatorHelper.SetBool(animator, key, val);
    }

    public static void SetFloat(
        this Animator animator, string key, float val) {

        if(animator == null) {
            return;
        }

        AnimatorHelper.SetFloat(animator, key, val);
    }

    public static void PlayOneShotBool(
        this Animator animator, string paramName) {

        if(animator == null) {
            return;
        }

        AnimatorHelper.PlayOneShotBool(animator, paramName);
    }

    public static void PlayOneShotFloat(
        this Animator animator, string paramName) {

        if(animator == null) {
            return;
        }

        AnimatorHelper.PlayOneShotFloat(animator, paramName);
    }

    public static void PlayOneShotFloat(
        this Animator animator, string paramName, float startValue, float endValue) {

        if(animator == null) {
            return;
        }

        AnimatorHelper.PlayOneShotFloat(animator, paramName, startValue, endValue);
    }

    public static void PlayOneShotFloat(
        this Animator animator, string paramName, float startValue) {

        if(animator == null) {
            return;
        }

        AnimatorHelper.PlayOneShotFloat(animator, paramName, startValue, 0f);
    }
}