using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CanvasExtensions {

    public static Rect RectTransformToScreenSpace(RectTransform transform) {

        Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);

        return new Rect((Vector2)transform.position - (size * 0.5f), size);
    }

    /*
    public static Vector3 CalculatePositionFromTransformToRectTransform(this Canvas canvas, Vector3 pos, Camera cam) {

        Vector3 val = Vector3.zero;

        if (canvas == null) {
            return val;
        }

        if (cam == null) {
            return val;
        }
        
        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay) {
            val = cam.WorldToScreenPoint(pos);
        }
        else if (canvas.renderMode == RenderMode.ScreenSpaceCamera) {
            Vector2 tempVector = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, cam.WorldToScreenPoint(pos), cam, out tempVector);
            val = canvas.transform.TransformPoint(tempVector);
        }

        return val;
    }
    */

    #region Canvas
    /// <summary>
    /// Calulates Position for RectTransform.position from a transform.position. Does not Work with WorldSpace Canvas!
    /// </summary>
    /// <param name="canvas"> The Canvas parent of the RectTransform.</param>
    /// <param name="pos">Position of in world space of the "Transform" you want the "RectTransform" to be.</param>
    /// <param name="cam">The Camera which is used. Note this is useful for split screen and both RenderModes of the Canvas.</param>
    /// <returns></returns>
    public static Vector3 CalculatePositionFromTransformToRectTransform(
        this Canvas canvas, Vector3 pos, Camera cam) {

        Vector3 val = Vector3.zero;

        if(canvas.renderMode == RenderMode.ScreenSpaceOverlay) {
            val = cam.WorldToScreenPoint(pos);
        }
        else if(canvas.renderMode == RenderMode.ScreenSpaceCamera) {

            Vector2 tempVector = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform, cam.WorldToScreenPoint(pos), cam, out tempVector);

            val = canvas.transform.TransformPoint(tempVector);
        }

        return val;
    }

    /// <summary>
    /// Calulates Position for RectTransform.position Mouse Position. Does not Work with WorldSpace Canvas!
    /// </summary>
    /// <param name="canvas">The Canvas parent of the RectTransform.</param>
    /// <param name="cam">The Camera which is used. Note this is useful for split screen and both RenderModes of the Canvas.</param>
    /// <returns></returns>
    public static Vector3 CalculatePositionFromMouseToRectTransform(this Canvas canvas, Camera cam) {

        Vector3 val = Vector3.zero;

        if(canvas.renderMode == RenderMode.ScreenSpaceOverlay) {
            val = Input.mousePosition;
        }
        else if(canvas.renderMode == RenderMode.ScreenSpaceCamera) {

            Vector2 tempVector = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform, Input.mousePosition, cam, out tempVector);

            val = canvas.transform.TransformPoint(tempVector);
        }

        return val;
    }

    /// <summary>
    /// Calculates Position for "Transform".position from a "RectTransform".position. Does not Work with WorldSpace Canvas!
    /// </summary>
    /// <param name="canvas">The Canvas parent of the RectTransform.</param>
    /// <param name="pos">Position of the "RectTransform" UI element you want the "Transform" object to be placed to.</param>
    /// <param name="cam">The Camera which is used. Note this is useful for split screen and both RenderModes of the Canvas.</param>
    /// <returns></returns>
    public static Vector3 CalculatePositionFromRectTransformToTransform(
        this Canvas canvas, Vector3 pos, Camera cam) {

        Vector3 val = Vector3.zero;

        if(canvas.renderMode == RenderMode.ScreenSpaceOverlay) {

            val = cam.ScreenToWorldPoint(pos);
        }
        else if(canvas.renderMode == RenderMode.ScreenSpaceCamera) {

            RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.transform as RectTransform, cam.WorldToScreenPoint(pos), cam, out val);
        }

        return val;
    }
    #endregion
}