using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameVehicleAIDriver : GameObjectBehavior {
    
    public float calcMaxSpeed = 200.0f;
    public float torque = 150.0f;
    public float brakeTorque = 500.0f;
    public float steerAngle = 20.0f;
    public float hsSteerAngle = 5.0f;
    public float steeringSpeed = 100;
    private float maxSpeed = 0;
    private float currentSpeed = 0.0f;
    private bool isBraking = false; 
       
    //wenn die Raeder sich falsch herum drehen, dann diesen Parameter aktivieren.
    private bool inverseWheelTurning = false;
    private int  wheelTurningParameter = 1;
    //Gaenge
    public int gears = 5;
    private List<int> gearSpeed = new List <int>();
    private int currentGear = 0;

    //Autosound
    public bool playSound = true;
    public AudioClip motorSound;
    public float soundVolume = 1;
    private AudioSource motorAudioSource; 

    //IA
    public GameVehicleDriveMode driveMode = GameVehicleDriveMode.OneWay;
    private string waypointPreName = "MyWaypoint";
    private string waypointFolder = "MyWaypoints";
    public List<Transform> waypoints = new List<Transform>();
    public float currentAngle;
    private float targetAngle;
    private float wheelRadius;
    private GameVehicleRespawn aiRespawnScript;
    public int currentWaypoint = 0;
    public float aiSteerAngle;
    private float aiSpeedPedal = 1;
    public float centerOfMassY = 0;

    //Reifenvisualisierung
    public Transform flWheel;
    public Transform frWheel;
    public Transform rlWheel;
    public Transform rrWheel;
    //Fahrzeugsteuerung
    public WheelCollider flWheelCollider;
    public WheelCollider frWheelCollider;
    public WheelCollider rlWheelCollider;
    public WheelCollider rrWheelCollider;

    //ObstacleAvoidance
    public Transform viewPoint;
    public bool useObstacleAvoidance = true;
    public float oADistance = 10;
    public float oAWidth = 2;
    public float oASideDistance = 1.0f;
    public float oASideOffset = 1.5f;
    public GameVehicleSteeringMode steeringMode = GameVehicleSteeringMode.Cautious;
    public float roadMaxWidth = 20; //Parameter for switching to next waypoint;  
    public LayerMask visibleLayers = -1;
    public GameObject viewPointLeftGO;
    public GameObject viewPointRightGO;
    public GameObject viewPointEndGO;
    public GameObject viewPointLeftEndGO;
    public GameObject viewPointRightEndGO;
    private float sqrDistanceToWaypoint = 4;
    private Vector3 leftDirection;
    private Vector3 rightDirection;
    private Vector3 centerPointL;
    private Vector3 centerPointR;
    private float obstacleAvoidanceWidth;
    private bool backwardDriving = false;
    public GameObject leftDirectionGO;
    public GameObject rightDirectionGO;
    public GameObject leftSideGO;
    public GameObject rightSideGO;
    public GameObject centerPointLGO;
    public GameObject centerPointRGO;
    public GameObject centerPointEndLGO;
    public GameObject centerPointEndRGO;
    public Transform frontCollider;


    //Event 1
    public delegate void LastWaypointHandler(GameVehicleEventArgs e);

    public static LastWaypointHandler onLastWaypoint;   

    public enum GameVehicleDriveMode { 
        OneWay,
        Laps
    }

    public enum GameVehicleSteeringMode {
        Cautious,
        Tough
    }

    void Awake() {
        maxSpeed = calcMaxSpeed;
        wheelRadius = flWheelCollider.radius;
        GetWaypointNames();
        FillWaypointList();
        InitGearSpeeds();

        if (inverseWheelTurning) {
            wheelTurningParameter = -1;
        }
        else {
            wheelTurningParameter = 1;
        }
        
        rigidbody.centerOfMass = new Vector3(0, centerOfMassY, 0);
        if (playSound && motorSound != null) {
            InitSound();
        }
        aiRespawnScript = gameObject.GetComponent("GameVehicleRespawn") as GameVehicleRespawn;        
       
    }

    void Start() {
        //ObstacleAvoidance
        if (useObstacleAvoidance) {
            sqrDistanceToWaypoint += roadMaxWidth * roadMaxWidth;

            viewPointLeftGO = new GameObject("viewPointLeftGO");
            viewPointLeftGO.transform.parent = transform;
            viewPointLeftGO.transform.position = viewPoint.transform.position;
            viewPointLeftGO.transform.position += viewPoint.TransformDirection(
                (Vector3.right * flWheel.localPosition.x));
            viewPointLeftGO.transform.rotation = transform.rotation;

            viewPointRightGO = new GameObject("viewPointRightGO");
            viewPointRightGO.transform.parent = transform;
            viewPointRightGO.transform.position = viewPoint.transform.position;                      
            viewPointRightGO.transform.position += viewPoint.TransformDirection(
                (Vector3.right * frWheel.localPosition.x));
            viewPointRightGO.transform.rotation = transform.rotation;

            obstacleAvoidanceWidth = viewPointRightGO.transform.localPosition.x + oAWidth;           
            leftDirection = viewPoint.position + viewPoint.TransformDirection(
                (Vector3.left * obstacleAvoidanceWidth) + (Vector3.forward * oADistance));                    
            rightDirection = viewPoint.position + viewPoint.TransformDirection(
                (Vector3.right * obstacleAvoidanceWidth) + (Vector3.forward * oADistance));

            centerPointL = transform.position + transform.TransformDirection(Vector3.left * oASideOffset);
            centerPointL.y = viewPoint.position.y;
            centerPointR = transform.position + transform.TransformDirection(Vector3.right * oASideOffset);
            centerPointR.y = viewPoint.position.y;

            leftDirectionGO = new GameObject("leftDirectionGO");
            leftDirectionGO.transform.parent = transform;
            leftDirectionGO.transform.position = leftDirection;
            leftDirectionGO.transform.rotation = transform.rotation;

            rightDirectionGO = new GameObject("rightDirectionGO");
            rightDirectionGO.transform.parent = transform;
            rightDirectionGO.transform.position = rightDirection;
            rightDirectionGO.transform.rotation = transform.rotation;

            centerPointLGO = new GameObject("centerPointLGO");
            centerPointLGO.transform.parent = transform;
            centerPointLGO.transform.position = centerPointL;
            centerPointLGO.transform.rotation = transform.rotation;

            centerPointRGO = new GameObject("centerPointRGO");
            centerPointRGO.transform.parent = transform;
            centerPointRGO.transform.position = centerPointR;
            centerPointRGO.transform.rotation = transform.rotation;

            viewPointEndGO = new GameObject("viewPointEndGO");
            viewPointEndGO.transform.parent = transform;
            viewPointEndGO.transform.position = viewPoint.position + viewPoint.TransformDirection(Vector3.forward * oADistance);
            viewPointEndGO.transform.rotation = transform.rotation;

            viewPointLeftEndGO = new GameObject("viewPointLeftEndGO");
            viewPointLeftEndGO.transform.parent = transform;
            viewPointLeftEndGO.transform.position = viewPointLeftGO.transform.position + viewPointLeftGO.transform.TransformDirection(Vector3.forward * oADistance);
            viewPointLeftEndGO.transform.rotation = transform.rotation;

            viewPointRightEndGO = new GameObject("viewPointRightEndGO");
            viewPointRightEndGO.transform.parent = transform;
            viewPointRightEndGO.transform.position = viewPointRightGO.transform.position + viewPointRightGO.transform.TransformDirection(Vector3.forward * oADistance);
            viewPointRightEndGO.transform.rotation = transform.rotation;

            centerPointEndLGO = new GameObject("centerPointEndLGO");
            centerPointEndLGO.transform.parent = transform;
            centerPointEndLGO.transform.position = centerPointL + transform.TransformDirection(Vector3.left * oASideDistance);
            centerPointEndLGO.transform.rotation = transform.rotation;

            centerPointEndRGO = new GameObject("centerPointEndRGO");
            centerPointEndRGO.transform.parent = transform;
            centerPointEndRGO.transform.position = centerPointR + transform.TransformDirection(Vector3.right * oASideDistance);
            centerPointEndRGO.transform.rotation = transform.rotation;

            //frontCollider = GameObject.CreatePrimitive(PrimitiveType.Cube);
            frontCollider = transform.FindChild("ViewPointCollider");                  
            //frontCollider.transform.parent = transform;
            Vector3 fcPos = viewPoint.transform.localPosition;
            fcPos.y += 0.1f;            
            frontCollider.transform.localPosition = fcPos;
            frontCollider.transform.rotation = transform.rotation;
            frontCollider.transform.localScale = new Vector3(frWheel.localPosition.x * 2 + 0.1f, 0.05f, 0.05f);
            //frontCollider.renderer.enabled = false;
            
        }
    }

    void InitSound() {    

        motorAudioSource = gameObject.AddComponent<AudioSource>();
        motorAudioSource.clip = motorSound;
        motorAudioSource.loop = true;
        motorAudioSource.volume = soundVolume;
        motorAudioSource.playOnAwake = false;
        motorAudioSource.pitch = 0.1f;
        //motorAudioSource.rolloffMode = AudioRolloffMode.Linear;
        motorAudioSource.Play();    

    }

    void FixedUpdate() {      
            
        currentSpeed = (Mathf.PI * 2 * wheelRadius) * flWheelCollider.rpm * 60 / 1000;
        //currentSpeed = (int)currentSpeed;
        currentSpeed = Mathf.Round(currentSpeed);


        if (currentSpeed > (maxSpeed + 10)) {
            isBraking = true;
        }
        else {
            isBraking = false;
            flWheelCollider.brakeTorque = 0;
            frWheelCollider.brakeTorque = 0;
        }

        if (isBraking == false) {
           
            if (currentSpeed < maxSpeed) {  
                                
                flWheelCollider.motorTorque = torque * aiSpeedPedal;
                frWheelCollider.motorTorque = torque * aiSpeedPedal;                    
            }
            else {
                flWheelCollider.motorTorque = 0;
                frWheelCollider.motorTorque = 0;
            }
        }
        else {
            flWheelCollider.brakeTorque = brakeTorque;
            frWheelCollider.brakeTorque = brakeTorque;
            flWheelCollider.motorTorque = 0;
            frWheelCollider.motorTorque = 0;
        }

        AiSteering();
       

        flWheelCollider.steerAngle = aiSteerAngle;
        frWheelCollider.steerAngle = aiSteerAngle;

        if (playSound && motorSound != null) {
            SetCurrentGear();
            GearSound();
        }
    }

    void Update() {

        RotateWheels();
        SteelWheels();
        
    }

    void RotateWheels() {
        flWheel.Rotate(flWheelCollider.rpm / 60 * 360 * Time.deltaTime * wheelTurningParameter, 0, 0);  
        frWheel.Rotate(frWheelCollider.rpm / 60 * 360 * Time.deltaTime * wheelTurningParameter, 0, 0);  
        rlWheel.Rotate(rlWheelCollider.rpm / 60 * 360 * Time.deltaTime * wheelTurningParameter, 0, 0);  
        rrWheel.Rotate(rrWheelCollider.rpm / 60 * 360 * Time.deltaTime * wheelTurningParameter, 0, 0);  
    }

    void SteelWheels() {        
        flWheel.localEulerAngles = new Vector3(flWheel.localEulerAngles.x, flWheelCollider.steerAngle - flWheel.localEulerAngles.z, flWheel.localEulerAngles.z);
        frWheel.localEulerAngles = new Vector3(frWheel.localEulerAngles.x, frWheelCollider.steerAngle - frWheel.localEulerAngles.z, frWheel.localEulerAngles.z);
    }

    void SetCurrentGear() {
        int gearNumber;
        //gearNumber = gearSpeed.length;
        gearNumber = gearSpeed.Count;
        currentGear = gearNumber - 1;
        for (var i = 0; i < gearNumber; i++) {
            if (gearSpeed[i] >= currentSpeed) {
                currentGear = i;
                break;
            }
        }
    }

    void GearSound() {
        float tempMinSpeed = 0.00f;
        float tempMaxSpeed = 0.00f;
        float currentPitch = 0.00f;        

        switch (currentGear) {
        case 0:
            tempMinSpeed = 0.00f;
            tempMaxSpeed = gearSpeed[currentGear];                            
            break;
                
        default:
            tempMinSpeed = gearSpeed[currentGear - 1];
            tempMaxSpeed = gearSpeed[currentGear];                
            break;
        }

        //currentPitch = (float)(((Mathf.Abs(currentSpeed) - tempMinSpeed) / (tempMaxSpeed - tempMinSpeed)) + 0.8);
        float delta = tempMaxSpeed - tempMinSpeed;
        currentPitch = (float)((((currentSpeed - tempMinSpeed) / delta)) / 2 + 0.8);
        
        //LogUtil.Log(currentPitch + ";" + currentSpeed + ";" + tempMinSpeed + ";" + delta +";" + currentGear);
        if (currentPitch > 2) {
            currentPitch = 2;
        }
        motorAudioSource.pitch = currentPitch;

    }

    void AiSteering() {
        if (currentWaypoint < waypoints.Count) {
            Vector3 target = waypoints[currentWaypoint].position;
            Vector3 moveDirection = target - transform.position;                
            Vector3 localTarget = transform.InverseTransformPoint(waypoints[currentWaypoint].position);

            if (!useObstacleAvoidance) {
                targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;
            }
            else {
                //ObstacleAvoidance
                currentAngle = ObstacleAvoidanceSteering();
            }
            //iaSteerAngle = Mathf.Clamp(targetAngle, -1, 1);

            
            if (currentAngle < targetAngle) {
                currentAngle = currentAngle + (Time.deltaTime * steeringSpeed);
                if (currentAngle > targetAngle) {
                    currentAngle = targetAngle;
                }
            }
            else if (currentAngle > targetAngle) {
                currentAngle = currentAngle - (Time.deltaTime * steeringSpeed);
                if (currentAngle < targetAngle) {
                    currentAngle = targetAngle;
                }
            }
            

            //je hoeher die Geschwindigkeit,  desto geringer der maximale Einschlagwinkel.
            float aiCalculationMaxSpeed = calcMaxSpeed;
            //float speedProcent = currentSpeed / maxSpeed;
            float speedProcent = currentSpeed / aiCalculationMaxSpeed;
            speedProcent = Mathf.Clamp(speedProcent, 0, 1);
            float speedControlledMaxSteerAngle;
            speedControlledMaxSteerAngle = steerAngle - ((steerAngle - hsSteerAngle) * speedProcent);
           

            aiSteerAngle = Mathf.Clamp(currentAngle, (-1) * speedControlledMaxSteerAngle, speedControlledMaxSteerAngle);
           
            //if (moveDirection.magnitude < 2) {
            if (moveDirection.sqrMagnitude < sqrDistanceToWaypoint) {
                

                //LogUtil.Log("currentWaypoint: " + currentWaypoint.ToString());                     
                GameVehicleAIWaypoint aiWaypoint;
                aiWaypoint = waypoints[currentWaypoint].GetComponent("GameVehicleAIWaypoint") as GameVehicleAIWaypoint;
                if (aiWaypoint != null) {
                    maxSpeed = aiWaypoint.speed;
                }
                else {
                    maxSpeed = calcMaxSpeed;
                }
                currentWaypoint++;

                aiRespawnScript.lastTimeToReachNextWP = 0;


                if (currentWaypoint >= waypoints.Count) {
                    //fire event BEGIN
                    if (onLastWaypoint != null) {
                        GameVehicleEventArgs e = new GameVehicleEventArgs();
                        e.name = gameObject.name;
                        e.currentWaypointIndex = currentWaypoint;
                        e.currentWaypointName = waypoints[currentWaypoint - 1].name;
                        e.position = gameObject.transform.position;
                        e.rotation = gameObject.transform.rotation;
                        e.tag = gameObject.tag;
                        onLastWaypoint(e);
                    }
                    //fire event END
                }
            }
            
        }
        else {
            
            //if (driveCircleModus)
            if (driveMode == GameVehicleDriveMode.Laps) {       
                currentWaypoint = 0; 
            }
            else {               
                aiSpeedPedal = 0;                
                aiRespawnScript.enabled = false;                            
            }
            
        }
        
    }
    
    void FillWaypointList() {               
        bool found = true;
        int counter = 1;
        while (found) {
            GameObject go;                      
            string currentName;
            currentName = "/" + waypointFolder + "/" + waypointPreName + counter.ToString();            
            go = GameObject.Find(currentName);
            
            if (go != null) {                               
                waypoints.Add(go.transform);
                counter++;
            }
            else {
                found = false;               
            }

            //rotate forerunner waypoint            
            if (counter > 2 && found) {
                string forerunnerName;
                forerunnerName = "/" + waypointFolder + "/" + waypointPreName + (counter - 2).ToString();                               
                GameObject forerunnerGo;
                forerunnerGo = GameObject.Find(forerunnerName);
                forerunnerGo.transform.LookAt(go.transform);
            }

            //rotate last waypoint
            if (counter > 2 && !found) {
                string lastName;
                lastName = "/" + waypointFolder + "/" + waypointPreName + (counter - 1).ToString();
                GameObject lastGo;
                lastGo = GameObject.Find(lastName);                

                string firstName;
                firstName = "/" + waypointFolder + "/" + waypointPreName + "1";               
                GameObject firstGo;
                firstGo = GameObject.Find(firstName);

                lastGo.transform.LookAt(firstGo.transform);                
            }
        }        
    }

    void GetWaypointNames() {
        GameVehicleAIWaypointEditor aiWaypointEditor;

        aiWaypointEditor = GetComponent("GameVehicleAIWaypointEditor") as GameVehicleAIWaypointEditor;
        if (aiWaypointEditor != null) {
            waypointPreName = aiWaypointEditor.preName + "_";
            waypointFolder = aiWaypointEditor.folderName;
        }
    }

    void InitGearSpeeds() {
        int gearSpeedStep;

        if (gears < 1) {
            gears = 1;
        }
        
        gearSpeedStep = (int)Mathf.Round(calcMaxSpeed / gears);
        gearSpeed.Clear();  
     
        for (int i = 0; i < gears; i++) {
            gearSpeed.Add(gearSpeedStep * (i + 1));
        }

    }

    //ObstacleAvoidance
    float ObstacleAvoidanceSteering() {        
        bool frontContact = false;
        float frontMinDistance = 0;
        float frontMaxDistance = -1;
        float leftDistance = 0;
        float rightDistance = 0;
        float leftSideDistance = 0;
        float rightSideDistance = 0;
        //float localSteeringAngle = steerAngle;
        //bool doSteering = false;
        RaycastHit hitFrontMid;
        RaycastHit hitFrontLeft;
        RaycastHit hitFrontRight;
        RaycastHit hitR;
        RaycastHit hitL;
        RaycastHit hitRSide;
        RaycastHit hitLSide;

        //Vector3 forwardDirection = viewPoint.TransformDirection(Vector3.forward * oADistance);      

        //front raycasts BEGIN ------------------------------------     

        if (Physics.Linecast(viewPoint.position, viewPointEndGO.transform.position, out hitFrontMid, visibleLayers)) {
            frontContact = true;
            frontMinDistance = hitFrontMid.distance;
            frontMaxDistance = hitFrontMid.distance;    
        }        
       
        if (Physics.Linecast(viewPointLeftGO.transform.position, viewPointLeftEndGO.transform.position, out hitFrontLeft, visibleLayers)) {
            frontContact = true;
            if (frontMinDistance == 0 || frontMinDistance > hitFrontLeft.distance) {
                frontMinDistance = hitFrontLeft.distance;
            }

            if (frontMaxDistance != -1 && frontMaxDistance < hitFrontLeft.distance) {
                frontMaxDistance = hitFrontLeft.distance;
            }            
        }
        else {
            frontMaxDistance = -1;
        }
       
        if (Physics.Linecast(viewPointRightGO.transform.position, viewPointRightEndGO.transform.position, out hitFrontRight, visibleLayers)) {
            frontContact = true;
            if (frontMinDistance == 0 || frontMinDistance > hitFrontRight.distance) {
                frontMinDistance = hitFrontRight.distance;
            }

            if (frontMaxDistance != -1 && frontMaxDistance < hitFrontRight.distance) {
                frontMaxDistance = hitFrontRight.distance;
            }            
        }
        else {
            frontMaxDistance = -1;
        }
        //front raycasts END ------------------------------------ 

        //nach vorne zu den Seiten schauen BEGIN ------------------------------------        
        if (Physics.Linecast(
            viewPointLeftGO.transform.position, 
            leftDirectionGO.transform.position, 
            out hitL, visibleLayers)) {
            leftDistance = hitL.distance;            
        }
                
        if (Physics.Linecast(
            viewPointRightGO.transform.position, 
            rightDirectionGO.transform.position, 
            out hitR, visibleLayers)) {
            rightDistance = hitR.distance;
        }
        //nach vorne zu den Seiten schauen END ------------------------------------ 

        //Hab ich Platz zum ausweichen? BEGIN ------------------------------------         
        if (Physics.Linecast(
            centerPointLGO.transform.position, 
            centerPointEndLGO.transform.position, 
            out hitLSide, visibleLayers)) {
            leftSideDistance = hitLSide.distance;
            //LogUtil.Log("center left: " + hitLSide.collider.gameObject.name);
        }
        
        if (Physics.Linecast(
            centerPointRGO.transform.position, 
            centerPointEndRGO.transform.position, 
            out hitRSide, visibleLayers)) {
            rightSideDistance = hitRSide.distance;
        }
        //Hab ich Platz zum ausweichen? END ------------------------------------  

        
        ////steering
        ////nach links lenken
        ////Kein "(rightSideDistance > 0) ||" weil kein Abdrengeln m�glich sein soll
        //if (leftSideDistance == 0 && ((leftDistance == 0 && rightDistance > 0) || (rightDistance != 0 && leftDistance != 0 && leftDistance > rightDistance)
        //    || (leftDistance == 0 && frontMinDistance > 0) || (rightDistance > leftDistance && frontMinDistance > 0)))
        //{
        //    currentAngle = (-1) * localSteeringAngle;
        //    //LogUtil.Log(" 1 leftDistance: " + leftDistance + ";rightDistance: " + rightDistance);
        //}

        ////nach rechts lenken
        ////Kein "(leftSideDistance > 0) ||" weil kein Abdrengeln m�glich sein soll
        //if (rightSideDistance == 0 && ((rightDistance == 0 && leftDistance > 0) || (rightDistance != 0 && leftDistance != 0 && rightDistance > leftDistance)
        //    || (rightDistance == 0 && frontMinDistance > 0) || (leftDistance > rightDistance && frontMinDistance > 0)))
        //{
        //    currentAngle = localSteeringAngle;           
        //}     

        //if (frontContact && (frontMaxDistance < 2 && frontMaxDistance > -1))
        //{
        //    backwardDriving = true;
        //}

        currentAngle = SteeringDecision(
            leftSideDistance, 
            rightSideDistance, 
            leftDistance, 
            rightDistance, 
            frontMinDistance, 
            frontContact, 
            steeringMode);

        if (backwardDriving) {
            
            if (currentSpeed > 2) {
                flWheelCollider.motorTorque = 0;
                frWheelCollider.motorTorque = 0;
                flWheelCollider.brakeTorque = brakeTorque;
                frWheelCollider.brakeTorque = brakeTorque;
                           
            }
            else {               
                flWheelCollider.motorTorque = (-1) * 100;
                frWheelCollider.motorTorque = (-1) * 100;
                flWheelCollider.brakeTorque = 0;
                frWheelCollider.brakeTorque = 0;
                currentAngle = (-1) * currentAngle;
                ;
            }

            if (frontMinDistance > 8 || frontMinDistance == 0) {
                backwardDriving = false;
            }          
            
        }
        
        return currentAngle;
    }

    private float SteeringDecision(
        float leftSideDistance, 
        float rightSideDistance, 
        float leftDistance, 
        float rightDistance, 
        float frontMinDistance, 
        bool frontContact, 
        GameVehicleSteeringMode style) {

        float localSteeringAngle = steerAngle;
        float result = 0;

        switch (style) { 
        case GameVehicleSteeringMode.Cautious:
            if (leftSideDistance == 0 
                && 
                ((leftDistance == 0 && rightDistance > 0) 
                || (rightDistance != 0 && leftDistance != 0 && leftDistance > rightDistance)
                || (leftDistance == 0 && frontMinDistance > 0) 
                || (rightDistance > leftDistance && frontMinDistance > 0) 
                || (frontContact == false && rightSideDistance > 0))) {
                result = (-1) * localSteeringAngle;
                //LogUtil.Log(" 1 leftDistance: " + leftDistance + ";rightDistance: " + rightDistance);
            }

                //nach rechts lenken
                //Kein "(leftSideDistance > 0) ||" weil kein Abdrengeln m�glich sein soll
            if (rightSideDistance == 0 
                && ((rightDistance == 0 && leftDistance > 0) 
                || (rightDistance != 0 && leftDistance != 0 && rightDistance > leftDistance)
                || (rightDistance == 0 && frontMinDistance > 0) 
                || (leftDistance > rightDistance && frontMinDistance > 0) 
                || (frontContact == false && leftSideDistance > 0))) {
                result = localSteeringAngle;
            }
            break;

        case GameVehicleSteeringMode.Tough:
                //steering
                //nach links lenken
                //Kein "(rightSideDistance > 0) ||" weil kein Abdrengeln m�glich sein soll
            if (leftSideDistance == 0 
                && ((leftDistance == 0 && rightDistance > 0) 
                || (rightDistance != 0 && leftDistance != 0 && leftDistance > rightDistance)
                || (leftDistance == 0 && frontMinDistance > 0) 
                || (rightDistance > leftDistance && frontMinDistance > 0))) {
                result = (-1) * localSteeringAngle;
                //LogUtil.Log(" 1 leftDistance: " + leftDistance + ";rightDistance: " + rightDistance);
            }

                //nach rechts lenken
                //Kein "(leftSideDistance > 0) ||" weil kein Abdrengeln m�glich sein soll
            if (rightSideDistance == 0 
                && ((rightDistance == 0 && leftDistance > 0) 
                || (rightDistance != 0 && leftDistance != 0 && rightDistance > leftDistance)
                || (rightDistance == 0 && frontMinDistance > 0) 
                || (leftDistance > rightDistance && frontMinDistance > 0))) {
                result = localSteeringAngle;
            }
            break;        
        }     

        return result;
    }

}



//public class GameVehicleEventArgs
//{
//    public string name;
//    public Vector3 position;
//    public Quaternion rotation;
//    public string currentWaypointName;
//    public int currentWaypointIndex;
//    //public string nextWaypointName;
//    //public int nextWaypointIndex;
//    public string tag;
//}