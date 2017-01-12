using System;
using System.Collections.Generic;
using Engine.Utility;
using UnityEngine;

namespace Engine.Events {

    public class InputSystemEvents {
        public static string inputAxis = "event-input-axis";
        public static string inputSwipe = "event-input-swipe";


        public static string inputUp = "event-input-up";
        public static string inputDown = "event-input-down";
        public static string inputMove = "event-input-move";
        public static string inputDragMove = "event-input-drag-move";
    }

    public class InputTouchInfo {
        public Vector2 position2d;
        public Vector3 position3d;

        public InputTouchInfo() {
            position2d = Vector2.zero;
            position3d = Vector3.zero;
        }
    }

    public class InputSystem : GameObjectBehavior {
        private bool inputEnabled = true;
        public bool useAcceleration = false;

        public Vector2 lastNormalizedTouch = new Vector2(0f, 0f);
        public Vector2 lastNormalizedTouch2 = new Vector2(0f, 0f);
        public Vector3 lastTargetDirection = new Vector3(0f, 0f, 0f);
        public Vector3 lastTargetDirection2 = new Vector3(0f, 0f, 0f);
        public Vector3 lastAccelerometer = Vector3.zero;
        public bool mousePressed = false;
        public bool mouseSecondaryPressed = false;
        public bool touchPressed = false;
        public bool leftPressed = false;
        public bool rightPressed = false;
        public bool upPressed = false;
        public bool downPressed = false;
        public bool usePressed = false;
        public InputTouchInfo lastTouch;
        public List<InputTouchInfo> touchesList;
        private InputTouchInfo touchInfo;

        public bool deferTap = false;

        public GameObject currentDraggableGameObject = null;
        public GameObject currentDraggableUIGameObject = null;

        public bool allowedTouch = true;
        public bool inputButtonDown = false;
        public bool inputAxisDown = false;

        float updateTouchStartTime = 0f;
        float updateTouchMaxTime = 2f;
        bool inputGestureDown = false;
        bool inputGestureUp = false;

        public bool shouldTouch = false;

        Vector3 lastDownAllowedPosition = Vector3.zero;
        Vector3 lastUpAllowedPosition = Vector3.zero;

        public Vector3 positionStart;
        public Vector3 positionEnd;
        public Vector3 positionLastLaunch;
        public float powerDistance;
        public Vector3 positionLastLaunchedNormalized;
        public GameObject pointStartObject;
        public GameObject pointEndObject;
        public UnityEngine.Object prefabPointStart;
        public UnityEngine.Object prefabPointEnd;
        public bool isCreatingStart = false;
        public bool isCreatingEnd = false;
        public Camera camHud = null;
        public Camera camUI = null;
        //public Camera camDialog = null;
        //public Camera camOverlay = null;

        bool showPoints = false;

        private static InputSystem _instance = null;

        public static InputSystem Instance {
            get {
                if (!_instance) {

                    _instance = FindObjectOfType(typeof(InputSystem)) as InputSystem;

                    if (!_instance) {
                        var obj = new GameObject("_InputSystem");
                        _instance = obj.AddComponent<InputSystem>();
                    }
                }

                return _instance;
            }
        }

        public static bool isInst {
            get {
                if (Instance != null) {
                    return true;
                }
                return false;
            }
        }

        private void OnApplicationQuit() {
            _instance = null;
        }

        private void OnEnable() {

            Messenger<TapGesture>.AddListener(FingerGesturesMessages.OnTap,
             FingerGestures_OnTap);

            Messenger<DragGesture>.AddListener(FingerGesturesMessages.OnDrag,
             FingerGestures_OnDragMove);

            Messenger<SwipeGesture>.AddListener(FingerGesturesMessages.OnSwipe,
             FingerGestures_OnSwipe);

            Messenger<PinchGesture>.AddListener(FingerGesturesMessages.OnPinch,
             FingerGestures_OnPinchMove);

            Messenger<TwistGesture>.AddListener(FingerGesturesMessages.OnTwist,
             FingerGestures_OnRotationMove);

            Messenger<LongPressGesture>.AddListener(FingerGesturesMessages.OnLongPress,
             FingerGestures_OnLongPress);

            Messenger<TapGesture>.AddListener(FingerGesturesMessages.OnDoubleTap,
             FingerGestures_OnDoubleTap);
        }

        private void OnDisable() {

            Messenger<TapGesture>.RemoveListener(FingerGesturesMessages.OnTap,
             FingerGestures_OnTap);

            Messenger<DragGesture>.RemoveListener(FingerGesturesMessages.OnDrag,
             FingerGestures_OnDragMove);

            Messenger<SwipeGesture>.RemoveListener(FingerGesturesMessages.OnSwipe,
             FingerGestures_OnSwipe);

            Messenger<PinchGesture>.RemoveListener(FingerGesturesMessages.OnPinch,
             FingerGestures_OnPinchMove);

            Messenger<TwistGesture>.RemoveListener(FingerGesturesMessages.OnTwist,
             FingerGestures_OnRotationMove);

            Messenger<LongPressGesture>.RemoveListener(FingerGesturesMessages.OnLongPress,
             FingerGestures_OnLongPress);

            Messenger<TapGesture>.RemoveListener(FingerGesturesMessages.OnDoubleTap,
             FingerGestures_OnDoubleTap);
        }

        public void Start() {
            inputEnabled = true;
            touchInfo = new InputTouchInfo();
            touchesList = new List<InputTouchInfo>();
        }

        //


        // INPUT


        public static void UpdateTouchLaunch() {

            if (isInst) {
                Instance.updateTouchLaunch();
            }
        }

        public static bool CheckIfAllowedTouch(Vector3 pos) {
            if (isInst) {
                return Instance.checkIfAllowedTouch(pos);
            }
            return false;
        }

        public static void HandleTouchLaunch(Vector2 move) {

            if (isInst) {
                Instance.handleTouchLaunch(move);
            }
        }

        //

        public static bool isMousePressed {
            get {
                return Instance.mousePressed;
            }
        }

        public static bool isMouseSecondaryPressed {
            get {
                return Instance.mouseSecondaryPressed;
            }
        }

        public static bool isTouchPressed {
            get {
                return Instance.touchPressed;
            }
        }

        public static bool isLeftPressed {
            get {
                return Instance.leftPressed;
            }
        }

        public static bool isRightPressed {
            get {
                return Instance.rightPressed;
            }
        }

        public static bool isUpPressed {
            get {
                return Instance.upPressed;
            }
        }

        public static bool isDownPressed {
            get {
                return Instance.downPressed;
            }
        }

        public static bool isUsePressed {
            get {
                return Instance.usePressed;
            }
        }

        //

        public virtual void FingerGestures_OnDragMove(DragGesture gesture) { //Vector2 fingerPos, Vector2 delta) {
            Vector2 fingerPos = gesture.Position;
            Vector2 delta = gesture.TotalMove;

            if (!IsInputAllowed()) {
                return;
            }

            if (currentDraggableGameObject != null) {
                //DragObject(currentDraggableGameObject, fingerPos, delta);
            }

            if (currentDraggableUIGameObject != null) {
                DragObject(currentDraggableUIGameObject, fingerPos, delta);
            }

            ///HandleRotators(camUI, fingerPos, delta);

        }

        public virtual void HandleRotators(Camera cam, Vector2 fingerPos, Vector2 delta) {

            GameObject goRotator = GameObjectHelper.HitObject(
                cam,
                Vector3.zero
                .WithX(fingerPos.x)
                .WithY(fingerPos.y),
                "rotator");

            //Debug.Log("goRotator:" + goRotator);

            if (goRotator != null) {
                //Debug.Log("goRotator:FOUND:" + goRotator);

                DragObject(goRotator, fingerPos, delta);
            }
            else {
                //Debug.Log("goRotator:NOTFOUND:" + goRotator);
            }

            Messenger<Vector2, Vector2>.Broadcast(InputSystemEvents.inputDragMove, fingerPos, delta);
        }

        public virtual void DragObject(GameObject go, Vector2 fingerPos, Vector2 delta) {
            if (go != null) {

                deferTap = true;

                Rigidbody rb = go.GetComponent<Rigidbody>();

                if (rb == null) {
                    go.AddComponent<Rigidbody>();
                    rb = go.GetComponent<Rigidbody>();
                    rb.constraints =
                        RigidbodyConstraints.FreezePosition
                        | RigidbodyConstraints.FreezeRotationX
                        | RigidbodyConstraints.FreezeRotationZ;
                    rb.useGravity = false;
                    rb.angularDrag = 3f;
                }

                go.transform.localRotation =
                    Quaternion.Euler(go.transform.localRotation.eulerAngles.WithY(-delta.x));

                if (Math.Abs(delta.x) > .05f) {
                    rb.angularVelocity = (new Vector3(0, -delta.x / 50, 0));
                }
                else {
                    rb.angularVelocity = Vector3.zero;
                }

                //GamePlayerProgress.Instance.ProcessProgressSpins();
            }
        }

        public virtual void FingerGestures_OnPinchMove(PinchGesture gesture) {
            //Vector2 fingerPos1 = gesture.Fingers[0].Position;
            //Vector2 fingerPos2 = gesture.
            //float delta = gesture.Delta;

            if (!IsInputAllowed()) {
                return;
            }
            //ScaleCurrentObjects(delta);
        }

        public virtual void FingerGestures_OnRotationMove(TwistGesture gesture) {
            //Vector2 fingerPos1, Vector2 fingerPos2, float rotationAngleDelta) {
            //float rotationAngleDelta = gesture.DeltaRotation;
            if (!IsInputAllowed()) {
                return;
            }
            // RotateCurrentObjects(Vector3.zero.WithY(rotationAngleDelta));
        }

        public virtual void FingerGestures_OnLongPress(LongPressGesture gesture) {
            Vector2 pos = gesture.Position;
            if (!IsInputAllowed()) {
                return;
            }

            if (currentDraggableGameObject != null) {
                LongPressObject(currentDraggableGameObject, pos);
            }
        }

        public virtual void LongPressObject(GameObject go, Vector2 pos) {
            if (go != null) {

                Rigidbody rb = go.GetComponent<Rigidbody>();

                if (rb != null) {
                    rb.angularVelocity = Vector3.zero;
                }
                deferTap = true;

                //ResetObject(go);
            }
        }

        public virtual void FingerGestures_OnTap(TapGesture gesture) {//Vector2 fingerPos) {
                                                                      //Vector2 fingerPos = gesture.Position;
                                                                      //LogUtil.Log("FingerGestures_OnTap", fingerPos);
            if (!IsInputAllowed()) {
                return;
            }

            //bool allowTap = true;

            if (currentDraggableGameObject != null) {
                //TapObject(currentDraggableGameObject, fingerPos, allowTap);
            }
        }

        public virtual void TapObject(GameObject go, Vector2 fingerPos, bool allowTap) {
            if (go != null) {
                deferTap = !allowTap;

                //LogUtil.Log("Tap:" + fingerPos);
                //LogUtil.Log("Tap:Screen.Height:" + Screen.height);

                float heightToCheck = Screen.height - Screen.height * .85f;
                // LogUtil.Log("Tap:heightToCheck:" + heightToCheck);

                if (fingerPos.y < heightToCheck) {
                    deferTap = true;
                }

                // LogUtil.Log("Tap:deferTap:" + deferTap);

                if (!deferTap) {

                    //var fwd = transform.TransformDirection(Vector3.forward);
                    //Ray ray = Camera.main.ScreenPointToRay(Vector3.zero);
                    //RaycastHit hit;
                    //if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
                    //print("hit an object:" + hit.transform.name);

                    //if(hit.transform.name == "UILoaderContainer") {
                    //   GameObject loaderCube = hit.transform.gameObject;
                    //   if(loaderCube != null) {
                    //UILoaderContainer loaderContainer = loaderCube.GetComponent<UILoaderContainer>();
                    //if(loaderContainer != null) {
                    //   if(loaderContainer.placeholderObject != null) {
                    //       loaderContainer.placeholderObject.SetPlaceholderObject();
                    //   }
                    //}
                    //   }
                    //}
                    //}

                    //if (!AppViewerUIController.Instance.uiVisible) {
                    //AppViewerAppController.Instance.ChangeActionNext();
                    // GamePlayerProgress.Instance.ProcessProgressTaps();
                    //}
                }
                else {
                    deferTap = false;
                }
            }
        }

        public virtual void DoubleTapObject(GameObject go, Vector2 pos) {
            if (go != null) {

                Rigidbody rb = go.GetComponent<Rigidbody>();

                if (rb != null) {
                    rb.angularVelocity = Vector3.zero;
                    deferTap = true;
                }

                //ResetObject(go);
            }
        }

        public virtual void FingerGestures_OnDoubleTap(TapGesture gesture) {
            if (!IsInputAllowed()) {
                return;
            }

            if (gesture.Taps == 2) {

                if (currentDraggableGameObject != null) {
                    DoubleTapObject(currentDraggableGameObject, gesture.Position);
                }
            }

            //var fwd = transform.TransformDirection(Vector3.forward);
            ////Ray ray = Camera.main.ScreenPointToRay(Vector3.zero);
            ////RaycastHit hit;
            ////if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
            ////    print("double tap hit an object:" + hit.transform.name);
            ////}

            //AppViewerAppController.Instance.ChangeActionNext();
        }

        public virtual void FingerGestures_OnTwoFingerSwipe(Vector2 startPos,
         FingerGestures.SwipeDirection direction, float velocity) {
            if (!IsInputAllowed()) {
                return;
            }

            if (direction == FingerGestures.SwipeDirection.All) {

                // if swiped any direction
            }

            if (direction == FingerGestures.SwipeDirection.Right
                || direction == FingerGestures.SwipeDirection.Down) {

                //AppViewerAppController.Instance.ChangeActionPrevious();
            }
            else if (direction == FingerGestures.SwipeDirection.Left
                || direction == FingerGestures.SwipeDirection.Up) {

                //AppViewerAppController.Instance.ChangeActionNext();
            }
        }

        public virtual void FingerGestures_OnSwipe(SwipeGesture gesture) {

            //Vector2 startPos = gesture.StartPosition;
            FingerGestures.SwipeDirection direction = gesture.Direction;
            //float velocity = gesture.Velocity;

            if (!IsInputAllowed()) {
                return;
            }

            bool allowSwipe = true;//AppViewerAppController.Instance.AllowCurrentActionAdvanceSwipe();

            if (direction == FingerGestures.SwipeDirection.Right
                || direction == FingerGestures.SwipeDirection.Down) {
                //if (!AppViewerUIController.Instance.uiVisible) {
                if (allowSwipe) {
                    //AppViewerAppController.Instance.ChangeActionPrevious();
                }
                GamePlayerProgress.Instance.ProcessProgressSwipes();
                //}
            }
            else if (direction == FingerGestures.SwipeDirection.Left
                || direction == FingerGestures.SwipeDirection.Up) {
                //if (!AppViewerUIController.Instance.uiVisible) {
                if (allowSwipe) {
                    //AppViewerAppController.Instance.ChangeActionNext();
                }
                GamePlayerProgress.Instance.ProcessProgressSwipes();
                //}
            }

            /*
            Vector2 swipeDirectionValue = gesture.Move;

            Vector3 dir = GameController.CurrentGamePlayerController.thirdPersonController.movementDirection;//GameController.CurrentGamePlayerController.transform.position;//swipeDirectionValue;//GameController.CurrentGamePlayerController.thirdPersonController.aimingDirection;

            Vector3 dirMovement =
                Vector3.zero
                    .WithX(GameController.CurrentGamePlayerController.thirdPersonController.horizontalInput)
                    .WithY(GameController.CurrentGamePlayerController.thirdPersonController.verticalInput);

            float angle =  Vector3.Angle(
                swipeDirectionValue,
                dirMovement);
            float force = 100f;

            if(angle > 320 && angle < 45) { // forwardish
                LogUtil.Log("swipe controller: FORWARD :angle:" + angle);
                GameController.CurrentGamePlayerController.Boost(force);
            }
            else if(angle < 225 && angle > 135) { // backish
                LogUtil.Log("swipe controller: BACK :angle:" + angle);
                GameController.CurrentGamePlayerController.Spin(force);
            }
            else if(angle > 45 && angle < 135) { // leftish
                LogUtil.Log("swipe controller: LEFT :angle:" + angle);
                GameController.CurrentGamePlayerController.StrafeLeft(force);
            }
            else if(angle > 320 && angle < 225) { // rightish
                LogUtil.Log("swipe controller: RIGHT :angle:" + angle);
                GameController.CurrentGamePlayerController.StrafeRight(force);
            }
            */
        }

        /*
        public Vector3 swipeCurrentStartPoint;
        public Vector3 swipeCurrentEndPoint;
        public Vector3 swipePositionLastTouch;
        public Vector3 swipePositionStart;
        public Vector3 swipePositionRelease;

        public void UpdateProjectile() {

            if(gameState == GameGameState.GamePause
                        || GameDraggableEditor.isEditing) {
                return;
            }

            if(gameState == GameGameState.GameStarted
                        && runtimeData.timeRemaining > 0) {

                // SHOOT

                //CheckForGameOver();

                bool touchDown = false;
                bool touchUp = false;

                if(Application.isEditor) {
                    touchDown = Input.GetMouseButtonDown(0);
                    touchUp = Input.GetMouseButtonUp(0);
                }
                else {
                    touchDown = Input.touches.Length > 0 ? true : false;
                    touchUp = !touchDown;
                }

                if(swipePositionStart != Vector3.zero
                                && swipePositionRelease == Vector3.zero) {

                    swipeCurrentStartPoint = Input.mousePosition;
                    swipeCurrentEndPoint = swipeCurrentStartPoint;
                }
                else {                          
                    //launcherObject.transform.localScale = Vector3.one;

                    //if(launcherObject != null && launchAimerObject != null) {
                    //      swipeCurrentStartPoint = launcherObject.transform.position;
                    //      swipeCurrentEndPoint = launchAimerObject.transform.position;
                    swipeCurrentStartPoint.z = 0f;
                    swipeCurrentEndPoint.z = 0f;
                    //}
                }

                Vector3 angles = CardinalAngles(swipeCurrentStartPoint, swipeCurrentEndPoint);//Vector3.zero.WithZ (
                //Vector3.Angle(swipeCurrentEndPoint, swipeCurrentStartPoint));//CardinalAngles(swipeCurrentStartPoint, swipeCurrentEndPoint);
                //LogUtil.Log("cardinalAngles:" + cardinalAngles);                        

                var dist = Vector3.Distance(swipeCurrentStartPoint, swipeCurrentEndPoint);

                Quaternion rotationTo = Quaternion.Euler(0, 0, angles.z);

                if(touchDown) {

                    swipePositionLastTouch = Input.mousePosition;
                    if(swipePositionStart == Vector3.zero) {
                        swipePositionStart = swipePositionLastTouch;
                        LogUtil.Log("swipePositionStart:" + swipePositionStart);
                        swipePositionRelease = Vector3.zero;
                    }                               
                }
                else if(touchUp) {
                    if(swipePositionStart != Vector3.zero) {
                        if(swipePositionRelease == Vector3.zero) {
                            swipePositionRelease = Input.mousePosition;
                            LogUtil.Log("swipePositionRelease:" + swipePositionRelease);
                            // Shoot

                            Quaternion rotationProjectile = Quaternion.Euler(90, 0, 0);

                            GameObject projectileObject = Instantiate(
                                                        prefabProjectile, //Resources.Load("Prefabs/GameProjectile"), 
                                                        currentGamePlayerController.gamePlayerModelHolderModel.transform.position, 
                                                        Quaternion.Euler(90, 0, 0)
                                                ) as GameObject;


                            LogUtil.Log("rotationProjectile:" + rotationProjectile);

                            projectileObject.transform.rotation = rotationProjectile;

                            //LogUtil.Log("launcherObject.transform.position:" + launcherObject.transform.position);
                            //LogUtil.Log("launchAimerObject.transform.position:" + launchAimerObject.transform.position);

                            swipeCurrentStartPoint = swipePositionRelease;
                            swipeCurrentEndPoint = swipePositionStart;
                            swipeCurrentStartPoint.z = 0f;
                            swipeCurrentEndPoint.z = 0f;

                            Vector3 crossProduct = Vector3.Cross(swipeCurrentStartPoint, swipeCurrentEndPoint);         

                            var angle = Vector3.Angle(swipeCurrentStartPoint, swipeCurrentEndPoint);
                            LogUtil.Log("Angle to other: " + angle);  

                            //var forward = transform.forward;
                            if(crossProduct.y < 0) {
                                //Do left stuff
                                //launcherObject.transform.Rotate(0, 0, -angle);
                            }
                            else {
                                //Do right stuff
                                //launcherObject.transform.Rotate(0, 0, angle);
                            }

                            //gameProjectile.direction = crossProduct;
                            LogUtil.Log("crossProduct to other: " + crossProduct);    

                            var distLaunch = Vector3.Distance(swipeCurrentStartPoint, swipeCurrentEndPoint);
                            print("Distance to other: " + distLaunch);
                            //distLaunch = 1;

                            LogUtil.Log("Rotation:" + projectileObject.transform.rotation);
                            LogUtil.Log("angle:" + angle);

                            var shootVector = swipeCurrentStartPoint - swipeCurrentEndPoint;                  
                            var multiplier = .001f;//.05f;
                            float forceAdd = distLaunch * multiplier;
                            LogUtil.Log("forceAdd:" + forceAdd);
                            forceAdd = Mathf.Clamp(forceAdd, .01f, .9f);
                            projectileObject.rigidbody.AddForce(-shootVector * forceAdd);//, ForceMode.Impulse);

                            swipePositionStart = Vector3.zero;
                            swipePositionRelease = Vector3.zero;

                            ShootOne();
                        }
                    }
                }
            }
        }
        */


        public virtual void HandleFingerGesturesOnLongPress(Vector2 fingerPos) {
            //LogUtil.Log("HandleFingerGesturesOnLongPress: " 
            //   + " fingerPos:" + fingerPos);   

            if (!IsInputAllowed()) {
                return;
            }

            // Create asset at touch point (long press) if in game and editing       
            LongPress(fingerPos);
        }

        public virtual void HandleFingerGesturesOnTap(Vector2 fingerPos) {
            //LogUtil.Log("HandleFingerGesturesOnTap: " 
            //   + " fingerPos:" + fingerPos);

            if (!IsInputAllowed()) {
                return;
            }

            // ...   
            Tap(fingerPos);

        }

        public virtual void HandleFingerGesturesOnDoubleTap(Vector2 fingerPos) {
            //LogUtil.Log("HandleFingerGesturesOnDoubleTap: " 
            //   + " fingerPos:" + fingerPos);

            if (!IsInputAllowed()) {
                return;
            }

            // ...   
            DoubleTap(fingerPos);

        }

        public virtual void HandleFingerGesturesOnDragMove(Vector2 fingerPos, Vector2 delta) {
            //LogUtil.Log("HandleFingerGesturesOnDragMove: " 
            //   + " fingerPos:" + fingerPos 
            //   + " delta:" + delta);

            if (!IsInputAllowed()) {
                return;
            }

            // scale current selected object 
            DragMove(fingerPos, delta);

        }

        public virtual void HandleFingerGesturesOnPinchMove(Vector2 fingerPos1, Vector2 fingerPos2, float delta) {
            //LogUtil.Log("HandleFingerGesturesOnPinchMove: " 
            //   + " fingerPos1:" + fingerPos1 
            //   + " fingerPos2:" + fingerPos2
            //   + " delta:" + delta);

            if (!IsInputAllowed()) {
                return;
            }

            // scale current selected object 
            PinchMove(fingerPos1, fingerPos2, delta);

        }

        public virtual void HandleFingerGesturesOnRotationMove(Vector2 fingerPos1, Vector2 fingerPos2, float rotationAngleDelta) {
            //LogUtil.Log("HandleFingerGesturesOnRotationMove: " 
            //   + " fingerPos1:" + fingerPos1 
            //   + " fingerPos2:" + fingerPos2
            //   + " rotationAngleDelta:" + rotationAngleDelta); 

            if (!IsInputAllowed()) {
                return;
            }

            // rotate current object if editing
            RotationMove(fingerPos1, fingerPos2, rotationAngleDelta);

        }

        public virtual void HandleFingerGesturesOnSwipe(Vector2 startPos, FingerGestures.SwipeDirection direction, float velocity) {
            //LogUtil.Log("HandleFingerGesturesOnSwipe: " 
            //   + " startPos:" + startPos 
            ///  + " direction:" + direction
            //   + " velocity:" + velocity); 

            if (!IsInputAllowed()) {
                return;
            }

            // ...
            Swipe(startPos, direction, velocity);
        }

        public virtual void HandleFingerGesturesOnTwoFingerSwipe(Vector2 startPos, FingerGestures.SwipeDirection direction, float velocity) {
            //LogUtil.Log("HandleFingerGesturesOnTwoFingerSwipe: " 
            //   + " startPos:" + startPos 
            //   + " direction:" + direction
            //   + " velocity:" + velocity); 

            if (!IsInputAllowed()) {
                return;
            }

            // ...
            TwoFingerSwipe(startPos, direction, velocity);
        }

        public virtual void LongPress(Vector2 fingerPos) {
            if (GameDraggableEditor.appEditState == GameDraggableEditEnum.StateEditing) {

                ResetCurrentObject(fingerPos);
            }
        }

        public virtual void RotationMove(Vector2 fingerPos1, Vector2 fingerPos2, float rotationAngleDelta) {
            if (GameDraggableEditor.appEditState == GameDraggableEditEnum.StateEditing) {
                RotateCurrentObject(rotationAngleDelta);
            }
        }

        public virtual void DragMove(Vector2 fingerPos, Vector2 delta) {
            if (GameDraggableEditor.appEditState == GameDraggableEditEnum.StateEditing) {
                //SpinCurrentObject(fingerPos, delta);

                bool doScale = false;
                bool doRotation = false;

                if (Input.GetKey(KeyCode.S)) {
                    doScale = true;
                }

                if (Input.GetKey(KeyCode.R)) {
                    doRotation = true;
                }

                if (doRotation) {
                    RotateCurrentObject(delta.x);
                }

                if (doScale) {
                    ScaleCurrentObject(delta.y);
                }

            }
        }

        public virtual void PinchMove(Vector2 fingerPos1, Vector2 fingerPos2, float delta) {
            if (GameDraggableEditor.appEditState == GameDraggableEditEnum.StateEditing) {
                ScaleCurrentObject(delta);
            }
        }

        public virtual void Tap(Vector2 fingerPos) {
            if (GameDraggableEditor.appEditState == GameDraggableEditEnum.StateEditing) {

            }
        }

        public virtual void DoubleTap(Vector2 fingerPos) {
            if (GameDraggableEditor.appEditState == GameDraggableEditEnum.StateEditing) {

                GameDraggableEditor.EditModeCreateAsset(fingerPos);

                //var fwd = transform.TransformDirection(Vector3.forward);
                Ray ray = Camera.main.ScreenPointToRay(Vector3.zero);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
                    print("double tap hit an object:" + hit.transform.name);
                }
            }

            //AppController.Instance.ChangeActionNext();

            if (Application.isEditor) {
                GameController.CycleCharacterTypesNext();
            }

        }

        public virtual void TwoFingerSwipe(Vector2 startPos, FingerGestures.SwipeDirection direction, float velocity) {

            if (GameDraggableEditor.appEditState == GameDraggableEditEnum.StateEditing) {

                if (direction == FingerGestures.SwipeDirection.All) {
                    // if swiped any direction
                }

                if (direction == FingerGestures.SwipeDirection.Right
                    || direction == FingerGestures.SwipeDirection.Down) {
                    //AppController.Instance.ChangeActionPrevious();
                }
                else if (direction == FingerGestures.SwipeDirection.Left
                    || direction == FingerGestures.SwipeDirection.Up) {
                    //AppController.Instance.ChangeActionNext();
                }
            }

            if (direction == FingerGestures.SwipeDirection.Right
                || direction == FingerGestures.SwipeDirection.Down) {
                GameController.CycleCharacterTypesPrevious();
            }
            else if (direction == FingerGestures.SwipeDirection.Left
                || direction == FingerGestures.SwipeDirection.Up) {
                GameController.CycleCharacterTypesNext();
            }
        }

        public virtual void Swipe(Vector2 startPos, FingerGestures.SwipeDirection direction, float velocitys) {
            if (GameDraggableEditor.appEditState == GameDraggableEditEnum.StateEditing) {

            }

            //bool allowSwipe = true;

            if (direction == FingerGestures.SwipeDirection.Right
                || direction == FingerGestures.SwipeDirection.Down) {
                //if(!UIController.Instance.uiVisible && allowSwipe) {
                // AppController.Instance.ChangeActionPrevious();
                GamePlayerProgress.Instance.ProcessProgressSwipes();
                //}


            }
            else if (direction == FingerGestures.SwipeDirection.Left
                || direction == FingerGestures.SwipeDirection.Up) {
                //if(!UIController.Instance.uiVisible && allowSwipe) {
                //AppController.Instance.ChangeActionNext();
                GamePlayerProgress.Instance.ProcessProgressSwipes();
                //}
            }
        }

        public virtual bool IsInputAllowed() {
            return true;
        }

        public virtual void ScaleCurrentObject(float delta) {
            GameObject go = GameDraggableEditor.GetCurrentSpriteObject();

            if (go != null) {
                GameObjectHelper.ScaleObject(go, delta);
            }
        }

        public virtual void RotateCurrentObject(float delta) {
            GameObject go = GameDraggableEditor.GetCurrentSpriteObject();

            if (go != null) {
                GameObjectHelper.RotateObjectZ(go, delta);
            }
        }

        public virtual void SpinCurrentObject(Vector2 fingerPos, Vector2 delta) {
            GameObject go = GameDraggableEditor.GetCurrentSpriteObject();

            if (go != null) {
                GameObjectHelper.SpinObject(go, fingerPos, delta);
                GameObjectHelper.deferTap = true;
                GamePlayerProgress.Instance.ProcessProgressSpins();
            }
        }

        public virtual void ResetCurrentObject(Vector2 pos) {
            GameObject go = GameDraggableEditor.GetCurrentSpriteObject();

            if (go != null) {
                Rigidbody rb = go.GetComponent<Rigidbody>();

                if (rb != null) {
                    if (rb != null) {
                        rb.angularVelocity = Vector3.zero;
                    }
                }
                GameObjectHelper.deferTap = true;

                GameObjectHelper.ResetObject(go);
            }
        }

        public virtual void FingerGestures_OnTap(Vector2 fingerPos) {
            if (!IsInputAllowed()) {
                return;
            }

            TapObject(GameDraggableEditor.GetCurrentSpriteObject(), fingerPos, true);
        }

        public virtual void handleTouchLaunch(Vector2 move) {

            float force = 20f;
            //LogUtil.Log("SWIPE:move:" + move);
            float angleGesture = move.CrossAngle();
            float anglePlayer = GameController.CurrentGamePlayerController.transform.rotation.eulerAngles.y;
            float distance = Vector2.Distance(Vector2.zero, move);

            force = distance;

            force = Mathf.Clamp(force, 0f, 60f);

            //angleGesture = Mathf.Abs(angleGesture - anglePlayer);
            //anglePlayer = 0;

            float angleDiff = angleGesture - anglePlayer;

            //LogUtil.Log("SWIPE:angleGesture:" + angleGesture);

            //LogUtil.Log("SWIPE:anglePlayer:" + anglePlayer);
            //LogUtil.Log("SWIPE:angleDiff:" + angleDiff);

            if (angleDiff < 0) {
                angleDiff = angleDiff + 360;
                angleDiff = Mathf.Abs(angleDiff);
            }
            //if(angleDiff > 180) {
            //    angleDiff = 360 - angleDiff;
            //}

            //LogUtil.Log("SWIPE:angleDiff2:" + angleDiff);

            var forceVector = Quaternion.AngleAxis(angleDiff, transform.up) *
                GameController.CurrentGamePlayerController.transform.forward;

            //forceVector.y = 0f;

            if (angleDiff > 320 || angleDiff <= 45) { // forwardish
                LogUtil.Log("swipe controller: FORWARD :angleDiff:" + angleDiff);
                GameController.CurrentGamePlayerController.Boost(forceVector, force * 1.2f);
            }
            else if (angleDiff < 225 && angleDiff >= 135) { // backish
                LogUtil.Log("swipe controller: BACK :angleDiff:" + angleDiff);
                GameController.CurrentGamePlayerController.Spin(forceVector, force * 1.8f);
                GamePlayerProgress.Instance.ProcessProgressTotal(GameStatCodes.spins, 1f);
            }
            else if (angleDiff > 45 && angleDiff < 135) { // rightish
                LogUtil.Log("swipe controller: RIGHT :angleDiff:" + angleDiff);
                GameController.CurrentGamePlayerController.Strafe(forceVector, force * 2f);
            }
            else if (angleDiff <= 320 && angleDiff >= 225) { // leftish
                LogUtil.Log("swipe controller: LEFT :angleDiff:" + angleDiff);
                GameController.CurrentGamePlayerController.StrafeLeft(forceVector, force * 2f);
            }
        }

        public virtual void updateTouchLaunch() {

            if (!GameConfigs.isGameRunning) {
                return;
            }

            shouldTouch = true;
            inputButtonDown = false;
            inputAxisDown = false;
            inputGestureDown = false;
            inputGestureUp = false;

            //bool inHitArea = false;

            if ((Input.mousePosition.x > Screen.width / 3
                && Input.mousePosition.x < Screen.width - Screen.width / 3)
                && (Input.mousePosition.y > Screen.height / 4
                && Input.mousePosition.y < Screen.height - Screen.height / 3)) {
                //inHitArea = true;
            }

            if (camHud == null) {
                foreach (Camera camItem in Camera.allCameras) {
                    if (camItem.cullingMask == LayerMask.NameToLayer("UIHUDScaled")) {
                        camHud = camItem;
                    }
                }
                if (camHud == null) {
                    camHud = Camera.main;
                }
            }

            bool hasTouches = false;
            //bool hasTouchesDown = false;
            //bool hasTouchesUp = false;
            bool hasTouchesDownAllowed = false;
            bool hasTouchesUpAllowed = false;

            if (Input.touches.Length > 0) {
                hasTouches = true;
            }

            lastDownAllowedPosition = Vector3.zero;
            lastUpAllowedPosition = Vector3.zero;

            //hasTouchesDown = checkIfTouchesDown();
            //hasTouchesUp = checkIfTouchesUp();
            hasTouchesDownAllowed = checkIfTouchesDownAllowed();
            hasTouchesUpAllowed = checkIfTouchesUpAllowed();
            ////

            if (hasTouches) {
                //LogUtil.Log("hasTouches: " + hasTouches);
            }
            if (hasTouchesDownAllowed) {
                //LogUtil.Log("hasTouchesDownAllowed: " + hasTouchesDownAllowed);
            }
            if (hasTouchesUpAllowed) {
                //LogUtil.Log("hasTouchesUpAllowed: " + hasTouchesUpAllowed);
            }

            if (!hasTouches) {
                lastDownAllowedPosition = Input.mousePosition;
                checkIfAllowedTouch(lastDownAllowedPosition);
            }

            if (!shouldTouch) {
                //return;
            }

            if (((Input.GetMouseButtonDown(0) && !hasTouchesDownAllowed) || hasTouchesDownAllowed)
                && !inputAxisDown
                && !inputButtonDown) {
                inputGestureDown = true;
            }

            if (((Input.GetMouseButtonUp(0) && !hasTouchesUpAllowed) || hasTouchesUpAllowed)
                && !inputAxisDown
                && !inputButtonDown) {
                inputGestureUp = true;
            }

            if (inputGestureDown
                            //&& (Input.mousePosition.x > Screen.width / 4 
                            //|| Input.mousePosition.y > Screen.height / 4)
                            //&& (Input.mousePosition.x < Screen.width - (Screen.width / 5) 
                            //&& Input.mousePosition.y < Screen.height - (Screen.height / 5) )
                            ) {
                if (positionStart == Vector3.zero) {
                    positionEnd = Vector3.zero;
                    positionStart = lastDownAllowedPosition;
                    showPoints = true;
                    updateTouchStartTime = Time.time;
                    //LogUtil.Log("GetMouseButtonDown:positionStart:" + positionStart);
                    //LogUtil.Log("GetMouseButtonDown:positionEnd:" + positionEnd);
                    //LogUtil.Log("GetMouseButtonDown:positionLastLaunch:" + positionLastLaunch);
                }


                if (GameController.CurrentGamePlayerController != null) {
                    if (GameController.CurrentGamePlayerController.IsPlayerControlled) {
                        //Vector3 dir = positionStart - Input.mousePosition;
                        //Vector3 posNormalized = dir.normalized;
                        //gamePlayerController.UpdateAim(-posNormalized.x, -posNormalized.y);
                        showPoints = true;
                    }
                }
            }
            else if (inputGestureUp) {
                if (positionEnd == Vector3.zero
                    && positionStart != Vector3.zero) {

                    if (hasTouches) {
                        positionEnd = lastUpAllowedPosition;
                    }
                    else {
                        positionEnd = Input.mousePosition;
                    }

                    // launch
                    powerDistance = Vector3.Distance(positionStart, positionEnd);
                    positionLastLaunch = positionStart - positionEnd;
                    //LogUtil.Log("GetMouseButtonUp:positionEnd:" + positionEnd);
                    //LogUtil.Log("GetMouseButtonUp:positionStart:" + positionStart);
                    //LogUtil.Log("GetMouseButtonUp:positionLastLaunch:" + positionLastLaunch);

                    positionLastLaunchedNormalized = positionLastLaunch.normalized;

                    //LogUtil.Log("GetMouseButtonUp:posNormalized:" + posNormalized);

                    bool doAction = true;

                    if (Time.time > updateTouchStartTime + updateTouchMaxTime) {
                        updateTouchStartTime = Time.time;
                        doAction = false;
                        showPoints = false;
                    }

                    if (!doAction) {
                        positionStart = Vector3.zero;
                        return;
                    }

                    if (GameController.CurrentGamePlayerController != null) {
                        if (GameController.CurrentGamePlayerController.IsPlayerControlled) {
                            //Attack();
                            //gamePlayerController.gamePlayerModelHolderModel.
                            //gamePlayerController.UpdateAim(-positionLastLaunchNormalized.x, -positionLastLaunchNormalized.y);
                            //Attack();

                            //PhysicsUtil.PlotTrajectory(transform.position, positionLastLaunchNormalized, .1f, 4f);


                            Messenger<Vector3>.Broadcast(UIControllerMessages.uiUpdateTouchLaunch, positionLastLaunchedNormalized);
                            //LogUtil.Log("positionLastLaunchedNormalized:" + positionLastLaunchedNormalized);
                            //LogUtil.Log("positionLastLaunch:" + positionLastLaunch);
                            //LogUtil.Log("powerDistance:" + powerDistance);

                            Vector2 touchLaunch = Vector2.zero.WithX(-positionLastLaunchedNormalized.x).WithY(-positionLastLaunchedNormalized.y);

                            HandleTouchLaunch(touchLaunch);

                            //ResetAimDelayed(.8f);
                        }
                    }
                    showPoints = true;
                    positionStart = Vector3.zero;
                    positionEnd = Vector3.zero;
                }
            }
            else {

            }

            if (showPoints) {
                if (positionStart != Vector3.zero) {
                    //showStartPoint(positionStart);
                }

                if (positionEnd != Vector3.zero) {
                    //showEndPoint(positionEnd);
                }
            }
            else {
                //hidePoints();
            }
        }


        public virtual void hidePoints() {
            hideStartPoint();
            hideEndPoint();
        }

        public virtual void hideStartPoint() {
            if (pointStartObject != null) {
                pointStartObject.transform.position = Vector3.zero.WithY(3000);
            }
        }

        public virtual void hideEndPoint() {
            if (pointEndObject != null) {
                pointEndObject.transform.position = Vector3.zero.WithY(3000);
            }
        }

        public virtual void showStartPoint(Vector3 pos) {
            //

            if (pointStartObject == null) {

                if (!isCreatingStart) {
                    isCreatingStart = true;
                    if (prefabPointStart == null) {
                        prefabPointStart = Resources.Load(
                                                    ContentPaths.appCacheVersionSharedPrefabWeapons + "GamePlayerWeaponCharacterLaunchPoint") as UnityEngine.Object;
                    }
                    pointStartObject = Instantiate(prefabPointStart) as GameObject;
                }
            }

            if (pointStartObject != null) {
                pointStartObject.transform.position = Camera.main.ScreenToWorldPoint(pos);
            }
        }

        public virtual void showEndPoint(Vector3 pos) {

            if (pointEndObject == null) {
                if (!isCreatingEnd) {
                    isCreatingEnd = true;
                    if (prefabPointEnd == null) {
                        prefabPointEnd = Resources.Load(
                            ContentPaths.appCacheVersionSharedPrefabWeapons +
                            "GamePlayerWeaponCharacterLaunchPoint") as UnityEngine.Object;
                    }
                    pointEndObject = Instantiate(prefabPointEnd) as GameObject;
                }
            }

            if (pointEndObject != null) {
                pointEndObject.transform.position = Camera.main.ScreenToWorldPoint(pos);
            }
        }

        public virtual bool checkIfAllowedTouch(Vector3 pos) {

            if (camHud == null) {
                return false;
            }

            Ray screenRay = camHud.ScreenPointToRay(pos);

            RaycastHit hit;

            allowedTouch = true;
            inputButtonDown = false;
            inputAxisDown = false;
            shouldTouch = false;

            if (Physics.Raycast(screenRay, out hit, Mathf.Infinity) && hit.transform != null) {

                if (hit.transform.name.Contains("ButtonInput")
                    || hit.transform.name.Contains("Axis")
                    || hit.transform.name.Contains("Ignore")
                    || hit.transform.name.Contains("Pad")) {
                    inputButtonDown = true;
                    shouldTouch = false;
                    allowedTouch = false;
                }

                if (allowedTouch) {
                    if (hit.transform.gameObject.Has<GameTouchInputAxis>()) {
                        // not over axis controller
                        inputAxisDown = true;
                        shouldTouch = false;
                        allowedTouch = false;
                    }

                    if (hit.transform.gameObject.Has<GameTouchInputAxis>()) {
                        // not over axis controller
                        inputAxisDown = true;
                        shouldTouch = false;
                        allowedTouch = false;
                    }

                    if (hit.transform.gameObject.Has<UIButton>()) {
                        // not over button
                        inputButtonDown = true;
                        shouldTouch = false;
                        allowedTouch = false;
                    }

                    if (hit.transform.gameObject.Has<UIImageButton>()) {
                        // not over button
                        inputButtonDown = true;
                        shouldTouch = false;
                        allowedTouch = false;
                    }
                }

                //LogUtil.Log("hit:" + hit);
                //LogUtil.Log("hit.transform.name:" + hit.transform.name);
            }

            return allowedTouch;
        }

        public virtual bool checkIfTouchesDownAllowed() {
            foreach (Touch t in Input.touches) {
                if (t.phase == TouchPhase.Began) {
                    if (checkIfAllowedTouch(t.position)) {
                        lastDownAllowedPosition = t.position;
                        return true;
                    }
                }
            }
            if (Input.GetMouseButtonDown(0)) {
                if (checkIfAllowedTouch(Input.mousePosition)) {
                    lastDownAllowedPosition = Input.mousePosition;
                    return true;
                }
            }
            return false;
        }

        public virtual bool checkIfTouchesUpAllowed() {
            foreach (Touch t in Input.touches) {
                if (t.phase == TouchPhase.Ended) {
                    if (checkIfAllowedTouch(t.position)) {
                        lastUpAllowedPosition = t.position;
                        return true;
                    }
                }
            }
            if (Input.GetMouseButtonUp(0)) {
                if (checkIfAllowedTouch(Input.mousePosition)) {
                    lastUpAllowedPosition = Input.mousePosition;
                    return true;
                }
            }
            return false;
        }

        public virtual bool checkIfTouchesDown() {
            foreach (Touch t in Input.touches) {
                if (t.phase == TouchPhase.Began) {
                    return true;
                }
            }
            if (Input.GetMouseButtonDown(0)) {
                return true;
            }
            return false;
        }

        public virtual bool checkIfTouchesUp() {
            foreach (Touch t in Input.touches) {
                if (t.phase == TouchPhase.Ended) {
                    return true;
                }
            }
            if (Input.GetMouseButtonUp(0)) {
                return true;
            }
            return false;
        }

        /*
        public virtual void DetectSwipe() {
            if(Input.touchCount > 0 || Input.GetMouseButtonDown(0)) {

                var startPos = Vector2.zero;
                var startTime = 0f;
                var touch = Input.touches[0];
                bool couldBeSwipe = false;
                float comfortZone = 500f;
                float minSwipeTime = .2f;
                float minSwipeDist = .2f;
                float maxSwipeTime = 1.7f;

                switch(touch.phase) {
                case TouchPhase.Began:
                    couldBeSwipe = true;
                    startPos = touch.position;
                    startTime = Time.time;
                    break;

                case TouchPhase.Moved:
                    if(Mathf.Abs(touch.position.y - startPos.y) > comfortZone) {
                        couldBeSwipe = false;
                    }
                    break;
                case TouchPhase.Stationary:
                    couldBeSwipe = false;
                    break;
                case TouchPhase.Ended:
                    var swipeTime = Time.time - startTime;
                    var swipeDist = (touch.position - startPos).magnitude;
                    if(couldBeSwipe && (swipeTime < maxSwipeTime) && (swipeDist > minSwipeDist)) {
                        // It's a swiiiiiiiiiiiipe!
                        var swipeDirection = Mathf.Sign(touch.position.y - startPos.y);
                        // Do something here in reaction to the swipe.

                        LogUtil.Log("swipeDirection:" + swipeDirection);
                        LogUtil.Log("swipeTime:" + swipeTime);
                        LogUtil.Log("swipeDist:" + swipeDist);
                    }
                    break;
                }
            }
        }
        */

        //

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
                if (touchInfo != null) {
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
                        if (touchInfo != null) {
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

                lastAccelerometer = GetAccelerationAxis();

                IsInputTouchUp();
                IsInputTouchDown();

                mousePressed = Input.GetMouseButton(0);
                mouseSecondaryPressed = Input.GetMouseButton(1);
                touchPressed = Input.touchCount > 0 ? true : false;

                leftPressed = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
                rightPressed = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
                upPressed = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
                downPressed = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);

                usePressed = Input.GetKey(KeyCode.E);

                //LogUtil.Log("InputSystem::Update");
            }
        }
    }
}