using System;
using System.Collections;
using System.Collections.Generic;
using Engine.Utility;
using UnityEngine;

public static class CameraExtensions {
    
    public static void LayerCullingShow(this Camera cam, int layerMask) {

        cam.cullingMask |= layerMask;
    }
    
    public static void LayerCullingShow(this Camera cam, string layer) {

        LayerCullingShow(cam, 1 << LayerMask.NameToLayer(layer));
    }
    
    public static void LayerCullingHide(this Camera cam, int layerMask) {

        cam.cullingMask &= ~layerMask;
    }
    
    public static void LayerCullingHide(this Camera cam, string layer) {

        LayerCullingHide(cam, 1 << LayerMask.NameToLayer(layer));
    }
    
    public static void LayerCullingToggle(this Camera cam, int layerMask) {

        cam.cullingMask ^= layerMask;
    }
    
    public static void LayerCullingToggle(this Camera cam, string layer) {

        LayerCullingToggle(cam, 1 << LayerMask.NameToLayer(layer));
    }
    
    public static bool LayerCullingIncludes(this Camera cam, int layerMask) {

        return (cam.cullingMask & layerMask) > 0;
    }
    
    public static bool LayerCullingIncludes(this Camera cam, string layer) {

        return LayerCullingIncludes(cam, 1 << LayerMask.NameToLayer(layer));
    }
    
    public static void LayerCullingToggle(this Camera cam, int layerMask, bool isOn) {

        bool included = LayerCullingIncludes(cam, layerMask);

        if (isOn && !included) {

            LayerCullingShow(cam, layerMask);
        }
        else if (!isOn && included) {

            LayerCullingHide(cam, layerMask);
        }
    }
    
    public static void LayerCullingToggle(this Camera cam, string layer, bool isOn) {

        LayerCullingToggle(cam, 1 << LayerMask.NameToLayer(layer), isOn);
    }
}

public static class CamerAnimationExtensions {

    // SHOW

    public static void ShowCameraFadeIn(this Camera cam) {

        if(cam == null) {
            return;
        }

        CoroutineUtil.Start(showCameraCo(cam));
    }

    public static IEnumerator showCameraCo(Camera cam) {

        yield return new WaitForSeconds(.4f);

        if(cam != null) {

            if(cam.gameObject != null) {

                cam.gameObject.Show();
                cam.enabled = true;

                TweenUtil.FadeToObject(cam.gameObject, 1f, .4f, .55f);

                //UITweenerUtil.FadeTo(cam.gameObject, UITweener.Method.EaseIn, UITweener.Style.Once, .5f, .5f, 1f);
            }
        }
    }

    // HIDE 

    public static void HideCameraFadeOut(this Camera cam) {

        if(cam == null) {
            return;
        }

        CoroutineUtil.Start(hideCameraCo(cam));
    }

    public static IEnumerator hideCameraCo(Camera cam) {

        if(cam != null) {

            if(cam.gameObject != null) {

                TweenUtil.FadeToObject(cam.gameObject, 0f, .1f, 0f);

                //UITweenerUtil.FadeTo(cam.gameObject, UITweener.Method.EaseIn, UITweener.Style.Once, .5f, .5f, 0f);
            }
        }

        yield return new WaitForSeconds(.3f);

        if(cam != null) {

            cam.enabled = false;

            if(cam.gameObject != null) {

                //cam.gameObject.Hide();
            }
        }
    }
}