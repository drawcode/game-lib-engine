using System;
using System.Collections.Generic;
using Engine.Animation;
using Engine.Utility;
using UnityEngine;

public static class AnimationExtentions {

    public static void AnimationsCopy(this Animation ani, Animation aniTo) {

        AnimationHelper.AnimationsCopy(ani, aniTo);
    }
}