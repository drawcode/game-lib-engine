using System;
using System.Collections.Generic;
using System.Reflection;
using Engine.Utility;
using UnityEngine;

public static class AnimatorExtentions {

    public static void PlayOneShotBool(this Animator animator, string paramName) {
        AnimatorHelper.PlayOneShotBool(animator, paramName);
    }

    public static void PlayOneShotFloat(this Animator animator, string paramName) {
        AnimatorHelper.PlayOneShotFloat(animator, paramName);
    }

    public static void PlayOneShotFloat(this Animator animator, string paramName, float startValue, float endValue) {
        AnimatorHelper.PlayOneShotFloat(animator, paramName, startValue, endValue);
    }
}