using UnityEngine;
using System.Collections;

public class CameraFacingBillboard : MonoBehaviour {
    public Camera cameraMain;

    void Start() {
        if (cameraMain == null) {
            cameraMain = Camera.main;
        }
    }

    // Camera.main is null at the menu (no enabled MainCamera), so a billboard that starts there
    // threw every frame. Resolve late and skip until a camera exists.
    void Update() {

        if (cameraMain == null) {
            cameraMain = Camera.main;

            if (cameraMain == null) {
                return;
            }
        }

        transform.LookAt(transform.position + cameraMain.transform.rotation * Vector3.forward,
            cameraMain.transform.rotation * Vector3.up);
    }
}