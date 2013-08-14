//	OnTouchDown.cs
//	Allows "OnMouseDown()" events to work on the iPhone.
//	Attack to the main camera.

#if UNITY_IPHONE

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Engine.Utility;

namespace Engine.Events {
	public class OnTouchDown : MonoBehaviour
	{
		void Start() {
		}

		void Update () {

			// Code for OnMouseDown in the iPhone. Unquote to test.
			RaycastHit hit = new RaycastHit();
			for (int i = 0; i < Input.touchCount; ++i) {
				if (Input.GetTouch(i).phase.Equals(TouchPhase.Began)) {

				// Construct a ray from the current touch coordinates
				Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
				if (Physics.Raycast(ray, out hit)) {
					if(hit.transform != null) {
						if(hit.transform.gameObject != null) {
							hit.transform.gameObject.SendMessage("OnMouseDown", SendMessageOptions.DontRequireReceiver);
						}
					}
			      }
			   }
		   }
		}
	}
}
#endif