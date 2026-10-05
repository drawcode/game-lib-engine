using UnityEngine;

using Engine.Utility;

namespace Engine.Animation {

    public interface ITweenTarget {

        object native { get; }

        // Key segment for cancel/replace lookups. String because Unity 6.5's
        // EntityId no longer converts to int without an obsolete-flagged cast.
        string targetId { get; }

        // False once the underlying object is destroyed; the backend removes
        // the tween (looping tweens would otherwise pump dead targets forever).
        bool alive { get; }

        Vector3 GetPosition(TweenCoord coord);
        void SetPosition(Vector3 v, TweenCoord coord);

        Vector3 GetScale();
        void SetScale(Vector3 v);

        Vector3 GetRotation(TweenCoord coord);
        void SetRotation(Vector3 euler, TweenCoord coord);

        float GetAlpha();
        void SetAlpha(float a);

        Color GetColor();
        void SetColor(Color c);
    }
}
