using System;

using UnityEngine;

namespace Engine.UI {

    // OPTIONAL backend capability: a FLOATING virtual stick. Kept out of IUIBackend so adding it
    // breaks no existing implementer (core libs are shared, additive-only); UIUtil asks for it
    // with `is` and falls back to the anchored SetElementStickHandler when a backend lacks it.
    public interface IUIFloatingStickBackend {

        // A press anywhere in `zone` moves `stick` so its centre sits under the thumb, and the
        // offsets that follow are measured from THAT point -- the stick comes to the thumb rather
        // than the thumb having to find the stick. A press on the stick itself works as the
        // anchored stick does. On release, cancel or lost capture the stick goes back to where its
        // layout put it. Same handler contract as IUIBackend.SetElementStickHandler: offset in
        // layout units, x right / y UP, unclamped, then once with released = true. One pointer per
        // stick, shared between the zone and the stick.
        void SetElementFloatingStickHandler(UIRef zone, UIRef stick, Action<Vector2, bool> onStick);
    }
}
