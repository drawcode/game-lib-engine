using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class AnimationHelper {

    public static void AnimationsCopy(Animation ani, Animation aniTo) {

        if(ani == null || aniTo == null) {
            return;
        }

        foreach(AnimationState state in ani) {
            aniTo.AddClip(ani[state.name].clip, state.name);
        }
    }
}