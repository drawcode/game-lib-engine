using UnityEngine;
using System.Collections;

public class CameraFacingObject : MonoBehaviour {
    public Camera cameraMain;
	
	void Start() {
		if(cameraMain == null) {
			cameraMain = Camera.main;
		}
	}

    void Update() {
        transform.LookAt(transform.position + cameraMain.transform.rotation * Vector3.up,
            cameraMain.transform.rotation * Vector3.up);
    }
}