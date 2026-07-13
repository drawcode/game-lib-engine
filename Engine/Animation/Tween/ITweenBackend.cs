using System;

using UnityEngine;

using Engine.Utility;

namespace Engine.Animation {

    public enum TweenChannel {
        position,
        scale,
        rotation,
        alpha,
        color,
        value
    }

    public interface ITweenBackend {

        void Move(ITweenTarget t, Vector3 to, TweenMeta meta);
        void Scale(ITweenTarget t, Vector3 to, TweenMeta meta);
        void Rotate(ITweenTarget t, Vector3 to, TweenMeta meta);
        void Fade(ITweenTarget t, float to, TweenMeta meta);
        void ColorTo(ITweenTarget t, Color to, TweenMeta meta);
        void Value(string key, float from, float to, TweenMeta meta, Action<float> onValue);

        void Cancel(ITweenTarget t);
        void Cancel(ITweenTarget t, TweenChannel channel);
        void Cancel(string key);
        void CancelAll();

        bool IsTweening(ITweenTarget t);
    }
}
