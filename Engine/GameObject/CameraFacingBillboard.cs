using UnityEngine;
using System.Collections;

public class CameraFacingBillboard : MonoBehaviour {
    public Camera cameraMain;
	
	void Start() {
		if(cameraMain == null) {
			cameraMain = Camera.main;
		}
	}

    void Update() {
        transform.LookAt(transform.position + cameraMain.transform.rotation * Vector3.forward,
            cameraMain.transform.rotation * Vector3.up);
    }
}