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
			
			bool mousePressed = Input.GetMouseButton(0);
			
			bool leftPressed = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
			bool rightPressed = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
			bool upPressed = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
			bool downPressed = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
			
            if (mousePressed) {
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
				LogUtil.Log("W:", upPressed);
				LogUtil.Log("A:", rightPressed);
				LogUtil.Log("S:", downPressed);
				LogUtil.Log("D:", rightPressed);
				
			
			if(leftPressed
				|| rightPressed
				|| upPressed
				|| downPressed) {
				
				LogUtil.Log("W:", upPressed);
				LogUtil.Log("A:", rightPressed);
				LogUtil.Log("S:", downPressed);
				LogUtil.Log("D:", rightPressed);
				
				Vector3 axisInput = Vector3.zero;
				
				if(upPressed) {
					axisInput.y = 1;
				}
				
				if(leftPressed) {
					axisInput.x = -1;
				}
				
				if(downPressed) {
					axisInput.y = -1;
				}
				
				if(rightPressed) {
					axisInput.x = 1;
				}				
				
                if (pad != null) {
                    Vector3 padPos = pad.localPosition;
                    padPos.x = axisInput.x;
                    padPos.y = axisInput.y;
                    padPos.z = 0;
                    pad.localPosition = padPos;
                }
				
				
				LogUtil.Log("axisInput:", axisInput);
				
				Messenger<string, Vector3>.Broadcast("input-axis", "input-axis-" + axisName, axisInput);
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