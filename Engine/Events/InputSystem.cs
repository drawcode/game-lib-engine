using System;
using System.Collections.Generic;
using Engine.Utility;
using UnityEngine;

namespace Engine.Events {

    public class InputTouchInfo {
        public Vector2 position2d;
        public Vector3 position3d;

        public InputTouchInfo() {
            position2d = Vector2.zero;
            position3d = Vector3.zero;
        }
    }

    public class InputSystem : MonoBehaviour {
        private bool inputEnabled = true;
        public bool useAcceleration = false;

        public static string EVENT_INPUT_UP = "EventInputUp";
        public static string EVENT_INPUT_DOWN = "EventInputDown";
        public static string EVENT_INPUT_MOVE = "EventInputMove";

        private static volatile InputSystem instance;
        private static System.Object syncRoot = new System.Object();

        public Vector2 lastNormalizedTouch = new Vector2(0f, 0f);
        public Vector2 lastNormalizedTouch2 = new Vector2(0f, 0f);
        public Vector3 lastTargetDirection = new Vector3(0f, 0f, 0f);
        public Vector3 lastTargetDirection2 = new Vector3(0f, 0f, 0f);
		
		public bool mousePressed = false;			
		public bool leftPressed = false;
		public bool rightPressed = false;
		public bool upPressed = false;
		public bool downPressed = false;

        public InputTouchInfo lastTouch;
        public List<InputTouchInfo> touchesList;

        private InputTouchInfo touchInfo;

        public static InputSystem Instance {
            get {
                if (instance == null) {
                    lock (syncRoot) {
                        if (instance == null)
                            instance = new InputSystem();
                    }
                }

                return instance;
            }
        }

        public void Awake() {
            instance = this;
        }

        public void Start() {
            inputEnabled = true;
            touchInfo = new InputTouchInfo();
            touchesList = new List<InputTouchInfo>();
        }

        public bool IsInputTouching() {

            //gameObject.SendMessage("OnInputPress");
            return IsInputTouchDown();
        }

        public void SendInputDownMessage(InputTouchInfo inputTouchInfo) {
            //MessengerObject<InputTouchInfo>.Broadcast(InputSystem.EVENT_INPUT_UP, inputTouchInfo, MessengerMode.DONT_REQUIRE_LISTENER);

            //LogUtil.Log("InputSystem::SendInputDownMessage");
        }

        public void SendInputUpMessage(InputTouchInfo inputTouchInfo) {
            //MessengerObject<InputTouchInfo>.Broadcast(InputSystem.EVENT_INPUT_UP, inputTouchInfo, MessengerMode.DONT_REQUIRE_LISTENER);

            //LogUtil.Log("InputSystem::SendInputUpMessage");
        }

        public void SendInputMoveMessage(InputTouchInfo inputTouchInfo) {
            //MessengerObject<InputTouchInfo>.Broadcast(InputSystem.EVENT_INPUT_MOVE, inputTouchInfo, MessengerMode.DONT_REQUIRE_LISTENER);

            //LogUtil.Log("InputSystem::SendInputMoveMessage");
        }

        public Vector3 GetAccelerationAxis() {
            Vector3 dir = Vector3.zero;
            dir.x = -Input.acceleration.y;
            dir.z = Input.acceleration.x;
            if (dir.sqrMagnitude > 1) {
                dir.Normalize();
            }

            dir *= Time.deltaTime;
            return dir;
        }

        public InputTouchInfo GetTouchInfoFromInput() {
            if (!Context.Current.isMobile) {
                if(touchInfo != null) {
                    if (Input.GetMouseButtonDown(0)) {
                        touchInfo.position2d.x = Input.mousePosition.x;
                        touchInfo.position2d.y = Input.mousePosition.y;
                        touchInfo.position3d = Input.mousePosition;
                        lastTouch = touchInfo;
    
                        //LogUtil.Log("lastTouch:" + lastTouch);
                        //LogUtil.Log("lastTouch.position2d.x:" + lastTouch.position2d.x);
                        //LogUtil.Log("lastTouch.position2d.y:" + lastTouch.position2d.y);
                    }
                }
            }
            else {
                foreach (Touch touch in Input.touches) {
                    if (touch.phase == TouchPhase.Moved
                       || touch.tapCount > 0) {
                        if(touchInfo != null) {
                            touchInfo.position2d = touch.position;
                            touchInfo.position3d.x = touch.position.x;
                            touchInfo.position3d.y = touch.position.y;
                            lastTouch = touchInfo;
    
                            //LogUtil.Log("lastTouch:" + lastTouch);
                            //LogUtil.Log("lastTouch.position2d.x:" + lastTouch.position2d.x);
                            //LogUtil.Log("lastTouch.position2d.y:" + lastTouch.position2d.y);
                        }
                    }
                }
            }

            //lastNormalizedTouch =

            return touchInfo;
        }

        public bool IsInputTouchMove() {
            if (!Context.Current.isMobile) {
                if (Input.GetMouseButtonDown(0)) {
                    SendInputDownMessage(GetTouchInfoFromInput());
                    return true;
                }
            }
            else {
                foreach (Touch touch in Input.touches) {
                    if (touch.phase == TouchPhase.Moved
                       || touch.tapCount > 0) {
                        SendInputDownMessage(GetTouchInfoFromInput());
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsInputTouchDown() {
            if (!Context.Current.isMobile) {
                if (Input.GetMouseButtonDown(0)) {
                    SendInputDownMessage(GetTouchInfoFromInput());
                    return true;
                }
            }
            else {
                foreach (Touch touch in Input.touches) {
                    if (touch.phase == TouchPhase.Began
                       || touch.phase == TouchPhase.Moved
                       || touch.tapCount > 0
                       || Input.GetMouseButtonDown(0)) {
                        SendInputDownMessage(GetTouchInfoFromInput());
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsInputTouchUp() {
            if (!Context.Current.isMobile) {
                if (Input.GetMouseButtonUp(0)) {
                    SendInputUpMessage(GetTouchInfoFromInput());
                    return true;
                }
            }
            else {
                foreach (Touch touch in Input.touches) {
                    if (touch.phase == TouchPhase.Ended) {
                        SendInputUpMessage(GetTouchInfoFromInput());
                        return true;
                    }
                }
            }
            return false;
        }

        public enum InputAxis {
            Up,
            Down,
            Left,
            Right,
            UpperRight,
            UpperLeft,
            LowerRight,
            LowerLeft
        }

        public bool IsAxisPressed(InputAxis inputAxis) {
            if (IsKeyDown(inputAxis)) {
                return true;
            }

            return false;
        }

        public bool IsAnyAxisPressed() {
            if (IsKeyDown(InputAxis.Up)
                || IsKeyDown(InputAxis.UpperLeft)
                || IsKeyDown(InputAxis.UpperRight)
                || IsKeyDown(InputAxis.Right)
                || IsKeyDown(InputAxis.LowerLeft)
                || IsKeyDown(InputAxis.LowerRight)
                || IsKeyDown(InputAxis.Left)
                || IsKeyDown(InputAxis.Down)) {
                LogUtil.Log("IsAnyKeyPressed:" + true);
                return true;
            }

            //LogUtil.Log("IsAnyKeyPressed:" + false);
            return false;
        }

        public bool IsKeyDown(InputAxis inputAxis) {
            if (inputAxis == InputAxis.Up
                || inputAxis == InputAxis.UpperLeft
                || inputAxis == InputAxis.UpperRight) {
                if (Input.GetKeyDown(KeyCode.UpArrow)
                    || Input.GetKeyDown(KeyCode.W)
                    || Input.GetAxis("Vertical") > 0
                    || useAcceleration && Input.acceleration.y > 0) {
                    return true;
                }
            }
            else if (inputAxis == InputAxis.Down
                || inputAxis == InputAxis.LowerLeft
                || inputAxis == InputAxis.LowerRight) {
                if (Input.GetKeyDown(KeyCode.DownArrow)
                    || Input.GetKeyDown(KeyCode.S)
                    || Input.GetAxis("Vertical") < 0
                    || useAcceleration && Input.acceleration.y < 0) {
                    return true;
                }
            }
            else if (inputAxis == InputAxis.Left
                || inputAxis == InputAxis.UpperLeft
                || inputAxis == InputAxis.LowerLeft) {
                if (Input.GetKeyDown(KeyCode.LeftArrow)
                    || Input.GetKeyDown(KeyCode.A)
                    || Input.GetAxis("Horizontal") < 0
                    || useAcceleration && Input.acceleration.x < 0) {
                    return true;
                }
            }
            else if (inputAxis == InputAxis.Right
                || inputAxis == InputAxis.UpperRight
                || inputAxis == InputAxis.LowerRight) {
                if (Input.GetKeyDown(KeyCode.RightArrow)
                    || Input.GetKeyDown(KeyCode.D)
                    || Input.GetAxis("Horizontal") > 0
                    || useAcceleration && Input.acceleration.x > 0) {
                    return true;
                }
            }

            return false;
        }

        private void Update() {
            if (inputEnabled) {
                IsInputTouchUp();
                IsInputTouchDown();
				
				mousePressed = Input.GetMouseButton(0);
				
				leftPressed = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
				rightPressed = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
				upPressed = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
				downPressed = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);

                //LogUtil.Log("InputSystem::Update");
            }
        }
    }
}