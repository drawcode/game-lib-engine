using System.Collections;
using UnityEngine;

public enum AnimationType {
    Linear,
    PingPong,
    Sin
}

[RequireComponent(typeof(Renderer))]
public class SharedTextureAnimator : GameObjectBehavior {
    public float scrollSpeed = 0.7f;
    public bool animateX = false;
    public bool animateY = true;
    public bool useSharedMaterial = true; // using the sharedMaterial allows batching
    public AnimationType animationType = AnimationType.Linear;

    private Material mat; // material cache

    private void Start() {
        if (useSharedMaterial)
            mat = renderer.sharedMaterial;
        else
            mat = renderer.material;
    }

    private void Update() {
        float offset = 0.0f;

        // calculate the offset based on the chosen type
        switch (animationType) {
            case AnimationType.Linear:
                offset = Time.time * scrollSpeed % 1;
                break;

            case AnimationType.PingPong:
                offset = Mathf.PingPong(Time.time * scrollSpeed, 1);
                break;

            case AnimationType.Sin:
                offset = Mathf.Sin(Time.time * scrollSpeed);
                break;
        }

        if (animateX && animateY)
            mat.mainTextureOffset = new Vector2(offset, offset);
        else if (animateX)
            mat.mainTextureOffset = new Vector2(offset, 0);
        else if (animateY)
            mat.mainTextureOffset = new Vector2(0, offset);
    }
}