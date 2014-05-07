//  OnTouchDown.cs
//  Allows "OnMouseDown()" events to work on the iPhone.
//  Attach to the main camera.
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Engine.Utility;

namespace Engine.Events {
    public class OnTouchUp : GameObjectBehavior {
        void Start() {
        }

        void Update() {
            
#if UNITY_IPHONE || UNITY_ANDROID

            // Code for OnMouseDown in the iPhone. Unquote to test.
            RaycastHit hit = new RaycastHit();
            for (int i = 0; i < Input.touchCount; ++i) {
                if (Input.GetTouch(i).phase.Equals(TouchPhase.Began)) {

                    if(Camera.main == null) 
                        break;
                    if(Camera.main.transform == null)
                        break;

                    // Construct a ray from the current touch coordinates
                    Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
                    if (!Physics.Raycast(ray, out hit)) {
                        if(hit.transform != null) {
                            if(hit.transform.gameObject != null) {
                                hit.transform.gameObject.SendMessage(
                                    "OnMouseUp", SendMessageOptions.DontRequireReceiver);
                            }
                        }
                    }
               }
           }
#endif
        }
    }
}