using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class BillboardFacingSprite : GameObjectBehavior {
    public Transform cameraTransform;

    // Use this for initialization
    private void Start() {
        if (cameraTransform == null) {
            if (Camera.main != null) {
                cameraTransform = Camera.main.transform;
            }
        }
    }

    // Update is called once per frame
    private void FixedUpdate() {
        transform.LookAt(cameraTransform);
        transform.position = Vector3.zero.WithX(0f).WithY(transform.position.y).WithZ(0f);
    }
}