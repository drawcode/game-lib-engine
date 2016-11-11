using System;
using System.Collections;
using UnityEngine;

public class CameraFly : GameObjectBehavior {
    /*
    8-4-6-5 : basic movement
    shift : Makes camera accelerate
    space : Moves camera on X and Z axis only.  So camera doesn't gain any height
    p - take screenshot
    only on in editor and must be toggled by the keypad 0 then use keypad to control, space/shift*/

#if UNITY_EDITOR333

		public float mainSpeed = 100.0f; //regular speed
		public float shiftAdd = 250.0f; //multiplied by how long shift is held.  Basically running
		public float maxShift = 1000.0f; //Maximum speed when holdin gshift
		public float camSens = 0.25f; //How sensitive it with mouse
		private Vector3 lastMouse = new Vector3(255, 255, 255); //kind of in the middle of the screen, rather than at the top (play)
		private float totalRun  = 1.0f;

		public bool enableFlyCam = false;
		public bool lastEnableFlyCam = false;

		CameraBehavior cameraGame;
		GameObject inputLeft;
		GameObject inputRight;

		GameObject gameHud;

		void Start() {
			FindGameCamera();
			FindObjects();
		}

		void FindGameCamera() {
			if(cameraGame == null) {
				cameraGame = GameObject.FindObjectOfType(typeof(CameraBehavior)) as CameraBehavior;
			}
		}

		void FindObjects() {
			if(inputLeft == null) {
				inputLeft = GameObject.Find("Left Button");
			}

			if(inputRight == null) {
				inputRight = GameObject.Find("Right Button");
			}

			if(gameHud == null) {
				gameHud = GameObject.Find("GameHUD");
			}
		}

		void  Update(){
			FindGameCamera();
			FindObjects();

			if(enableFlyCam) {
			    lastMouse = Input.mousePosition - lastMouse ;
			    lastMouse = new Vector3(-lastMouse.y * camSens, lastMouse.x * camSens, 0 );
			    lastMouse = new Vector3(transform.eulerAngles.x + lastMouse.x , transform.eulerAngles.y + lastMouse.y, 0);

			    transform.eulerAngles = lastMouse;

			    lastMouse =  Input.mousePosition;

			    //Mouse & camera angle done.
			    //Keyboard commands

			    float f = 0.0f;
			    Vector3 p = GetBaseInput();

			    if (Input.GetKey (KeyCode.LeftShift)) {
			        totalRun += Time.deltaTime;
			        p  = p * totalRun * shiftAdd;
			        p.x = Mathf.Clamp(p.x, -maxShift, maxShift);
			        p.y = Mathf.Clamp(p.y, -maxShift, maxShift);
			        p.z = Mathf.Clamp(p.z, -maxShift, maxShift);
			    }
			    else {
			        totalRun = Mathf.Clamp(totalRun * 0.5f, 1, 1000);
			        p = p * mainSpeed;
			    }

			    p = p * Time.deltaTime;

			    if (Input.GetKey(KeyCode.Space)) { //If player wants to move on X and Z axis only
					Vector3 temp = transform.position;
			        f = temp.y;
			        transform.Translate(p);
					temp.y = f;

			        transform.position = temp;
			    }
			    else {
			        transform.Translate(p);
			    }
			}

			if(Input.GetKeyDown(KeyCode.P)) {

				// take a screenshot since it is hard to move to get mac screenshot while in fly mode
				CameraUtil.SaveScreenshotEditor();
			}

			// enable or disable
			ToggleFlyCam();
		}

		void ToggleFlyCam() {
			if(Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.O)) {
				enableFlyCam = !enableFlyCam;
				if(enableFlyCam != lastEnableFlyCam) {
					lastEnableFlyCam = enableFlyCam;
					FindGameCamera();

					if(enableFlyCam) {
						if(cameraGame != null) {
							cameraGame.isUpdating = false;
							inputLeft.Hide();
							inputRight.Hide();
							gameHud.Hide();
						}
					}
					else {
						if(cameraGame != null) {
							cameraGame.isUpdating = true;
							inputLeft.Show();
							inputRight.Show();
							gameHud.Show();
						}
					}
				}
			}
		}

		Vector3 GetBaseInput() {
		    Vector3 velocity = Vector3.zero;

		    if (Input.GetKey (KeyCode.Keypad8) || Input.GetKey (KeyCode.U)) {
		        velocity += new Vector3(0f, 0f, 1f);
		    }

		    if (Input.GetKey (KeyCode.Keypad5) || Input.GetKey (KeyCode.J)) {
		        velocity += new Vector3(0f, 0f, -1f);
		    }

		    if (Input.GetKey (KeyCode.Keypad4) || Input.GetKey (KeyCode.H)) {
		        velocity += new Vector3(-1f, 0f, 0f);
		    }

		    if (Input.GetKey (KeyCode.Keypad6) || Input.GetKey (KeyCode.K)) {
		        velocity += new Vector3(1f, 0f, 0f);
		    }

		    return velocity;
		}

#endif
}