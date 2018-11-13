using System;
using Engine.Utility;
using UnityEngine;

public class SpriteUtil {

    public SpriteUtil() {

    }

    public static void SetColorAlpha(GameObject sprite, float alpha) {

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
        
        UISprite iconSprite = sprite.Get<UISprite>();
	    iconSprite.alpha = alpha;
#else
        SpriteRenderer spriteRenderer = sprite.Get<SpriteRenderer>();

        if(spriteRenderer == null) {
            return;
        }

        Color tmp = spriteRenderer.color;
        tmp.a = alpha;
        spriteRenderer.color = tmp;
#endif
    }
}