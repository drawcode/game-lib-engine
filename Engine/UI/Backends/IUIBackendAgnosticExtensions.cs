using System;

using Agnostic.Core;

using Engine.AgnosticHost;

using UnityEngine;

namespace Engine.UI {

    // ColorRgba / Vec2 overloads of the Unity-typed IUIBackend members (bitty gap-list step 1).
    // Extension methods rather than interface members, so no backend has to implement each op
    // twice; they convert through UnityFrame (both 1:1 — sRGB on both sides, no axis flip in UI
    // space) and forward to the Color/Vector2 members, which stay canonical.
    public static class IUIBackendAgnosticExtensions {

        // LABELS

        public static void SetLabelColor(this IUIBackend backend, UIRef r, ColorRgba c) {
            backend.SetLabelColor(r, UnityFrame.ToUnity(c));
        }

        // IMAGES

        public static void SetSpriteColor(this IUIBackend backend, UIRef r, ColorRgba c) {
            backend.SetSpriteColor(r, UnityFrame.ToUnity(c));
        }

        public static void SetElementColor(this IUIBackend backend, UIRef r, ColorRgba c) {
            backend.SetElementColor(r, UnityFrame.ToUnity(c));
        }

        // DRAG / STICK / TRANSLATE
        // The handler overloads wrap the callback rather than the argument: the caller's
        // Action<Vec2> (or <Vec2, bool>) never sees a Vector2, so agnostic call sites never
        // reference UnityEngine either.

        public static void SetElementDragHandler(this IUIBackend backend, UIRef r, Action<Vec2> onDrag) {

            if (onDrag == null) {
                backend.SetElementDragHandler(r, (Action<Vector2>)null);
                return;
            }

            backend.SetElementDragHandler(r, v => onDrag(UnityFrame.ToCore(v)));
        }

        public static void SetElementStickHandler(this IUIBackend backend, UIRef r, Action<Vec2, bool> onStick) {

            if (onStick == null) {
                backend.SetElementStickHandler(r, (Action<Vector2, bool>)null);
                return;
            }

            backend.SetElementStickHandler(r, (v, released) => onStick(UnityFrame.ToCore(v), released));
        }

        public static void SetElementTranslate(this IUIBackend backend, UIRef r, Vec2 offset) {
            backend.SetElementTranslate(r, UnityFrame.ToUnity(offset));
        }

        // BUTTONS

        public static void SetButtonColor(this IUIBackend backend, UIRef r, ColorRgba c) {
            backend.SetButtonColor(r, UnityFrame.ToUnity(c));
        }

        // POINTER / EVENT SOURCE

        public static bool IsPointerOver(this IUIBackend backend, Vec2 screenPos) {
            return backend.IsPointerOver(UnityFrame.ToUnity(screenPos));
        }
    }
}
