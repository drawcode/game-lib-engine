using System;
using System.Collections.Generic;
using Engine.Utility;
using UnityEngine;

public static class AnimationExtentions {

    public static void AnimationPlay(this Animation ani, string name, PlayMode playMode) {
        if (ani == null) {
            return;
        }
        ani.Play(name, playMode);
    }

    public static void AnimationPlay(this Animation ani) {
        if (ani == null) {
            return;
        }
        ani.Play(PlayMode.StopSameLayer);
    }

    public static void AnimationCrossFade(this Animation ani, Animation aniTo, GameObject actor) {
        if (ani == null) {
            return;
        }
    
        if (aniTo == null) {
            return;
        }
    
        if (actor == null) {
            return;
        }

        Animation anim = actor.GetComponent<Animation>();
    
        if (anim == null) {
            return;
        }
    
        if (anim[aniTo.name] != null) {
            anim.CrossFade(aniTo.name);
        }
    }

    public static void AnimationBlend(this Animation ani, Animation aniTo, GameObject actor) {
        AnimationBlend(ani, aniTo, actor,  .5f, .5f);
    }

    public static void AnimationBlend(this Animation ani, Animation aniTo, GameObject actor, float targetWeight) {
        AnimationBlend(ani, aniTo, actor, targetWeight, .5f);
    }

    public static void AnimationBlend(this Animation ani, Animation aniTo, GameObject actor, float targetWeight, float fadeLength) {
        if (ani == null) {
            return;
        }
    
        if (aniTo == null) {
            return;
        }
    
        if (actor == null) {
            return;
        }
        
        Animation anim = actor.GetComponent<Animation>();
        
        if (anim == null) {
            return;
        }
    
        if (anim[aniTo.name] != null) {
            ani.Blend(aniTo.name, targetWeight, fadeLength);
        }
    }

    public static void AnimationStatePlay(this AnimationState ani, GameObject actor) {
        if (ani == null) {
            return;
        }
    
        if (actor == null) {
            return;
        }
        
        Animation anim = actor.GetComponent<Animation>();
        
        if (anim == null) {
            return;
        }
    
        anim.Play(ani.name, PlayMode.StopSameLayer);
    }

    public static void AnimationStateCrossFade(this AnimationState ani, GameObject actor, AnimationState aniTo) {
        if (ani == null) {
            return;
        }
    
        if (aniTo == null) {
            return;
        }
    
        if (actor == null) {
            return;
        }
        
        Animation anim = actor.GetComponent<Animation>();
        
        if (anim == null) {
            return;
        }

        if (anim[aniTo.name] != null) {
            anim.CrossFade(aniTo.name);
        }
    }

    public static void AnimationStateBlend(this AnimationState ani, AnimationState aniTo, GameObject actor) {
        AnimationStateBlend(ani, aniTo, actor, .8f, .5f);
    }

    public static void AnimationStateBlend(this AnimationState ani, AnimationState aniTo, GameObject actor, float targetWeight) {
        AnimationStateBlend(ani, aniTo, actor, targetWeight, .5f);
    }

    public static void AnimationStateBlend(this AnimationState ani, AnimationState aniTo, GameObject actor, float targetWeight, float fadeLength) {
        if (ani == null) {
            return;
        }
    
        if (aniTo == null) {
            return;
        }
    
        if (actor == null) {
            return;
        }
        
        Animation anim = actor.GetComponent<Animation>();
        
        if (anim == null) {
            return;
        }
    
        if (anim[aniTo.name] != null) {
            anim.Blend(aniTo.name, targetWeight, fadeLength);
        }
    }
}

