using System;
using System.Collections;
using System.Collections.Generic;
using Engine;
using Engine.Utility;
using UnityEngine;

namespace Engine.Events {

    public class TouchInputAxisObject : MonoBehaviour {
        public Camera collisionCamera;
        public Transform pad;// = gameObject.transform.FindChild("Pad");
        public string axisName = "main";

        public Vector3 axisInput;
        public Vector3 padPos;

        private void FindPad() {
            if (pad == null) {
                pad = gameObject.transform.FindChild("Pad");
            }
        }

        private void Update() {
            if (Input.GetMouseButton(0)) {
                if (collisionCamera) {
                    Ray screenRay = collisionCamera.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(screenRay, out hit, Mathf.Infinity) && hit.transform != null) {
                        if (hit.transform.gameObject == gameObject) {
                            axisInput.x = (hit.textureCoord.x - .5f) * 2;
                            axisInput.y = (hit.textureCoord.y - .5f) * 2;

                            Messenger<string, Vector3>.Broadcast("input-axis", "input-axis-" + axisName, axisInput);

                            if (pad != null) {
                                padPos = pad.localPosition;
                                padPos.x = -Mathf.Clamp(axisInput.x * 1.5f, -1.2f, 1.2f);
                                padPos.z = -Mathf.Clamp(axisInput.y * 1.5f, -1.2f, 1.2f);
                                padPos.y = 0f;
                                pad.localPosition = padPos;
                            }
                        }
                    }
                }
            }
            else {
                axisInput = Vector3.zero;
                
				Messenger<string, Vector3>.Broadcast("input-axis", "input-axis-" + axisName, axisInput);

                if (pad != null) {
                    Vector3 padPos = pad.localPosition;
                    padPos.x = 0;
                    padPos.y = 0;
                    padPos.z = 0;
                    pad.localPosition = padPos;
                }
            }
        }
    }
}