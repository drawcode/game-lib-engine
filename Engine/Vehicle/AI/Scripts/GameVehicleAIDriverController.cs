using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(SplinePathWaypoints))]

public class GameVehicleAIDriverController : GameObjectBehavior {
      
    
    private float m_calcMaxSpeed = 200.0f;
    //public float torque = 150.0f;
    //public float brakeTorque = 500.0f;
    [HideInInspector]
    public float
        steerAngle = 20.0f;
    public float hsSteerAngle = 5.0f;
    public float steeringSpeed = 100;
    public float offsetWaypointSpeed = 0; //2012-07-09 RacingTest-Modification
    private float m_maxSpeed = 0;
    private float m_currentSpeed = 0.0f;
    private bool m_isBraking = false;
    private float m_leftRightDistanceLength = 0;
    public float m_frontDistanceLength = 0;
    private float m_leftRightSideDistanceLength = 0;
    private bool optimizedWpTargeting = true; //Use the next waypoint when it's better //2013-06-17
    
    ////if wheels turn in wrong direction, please activate this parameter
    //private bool inverseWheelTurning = false;
    //private int  wheelTurningParameter = 1;
    //Gaenge
    //public int gears=5;
    //private List<int> gearSpeed = new List <int>();

    //private int currentGear =0;

    //Autosound
    //public bool playSound = true;
    //public AudioClip motorSound;
    //public float soundVolume = 1;
    //private AudioSource motorAudioSource; 

    //IA
    [HideInInspector]
    public GameVehicleMotorMapping
        aiPreMotor;
    public DriveMode driveMode = DriveMode.Laps;
    public bool steerAbsolute = false;
    private string m_waypointPreName = "MyWaypoint";
    private string m_waypointFolder = "MyWaypoints";
    [HideInInspector]
    public List<Transform>
        waypoints = new List<Transform>();
    [HideInInspector]
    public float
        currentAngle;
    private float m_targetAngle;
    //private float wheelRadius;
    private GameVehicleRespawnController aiRespawnControllerScript;
    [HideInInspector]
    public int
        currentWaypoint = 0;
    [HideInInspector]
    public float
        aiSteerAngle;
    [HideInInspector]
    public float
        aiSpeedPedal = 1;
    //public float centerOfMassY = 0;

    ////Reifenvisualisierung
    [HideInInspector]
    public Transform
        flWheel;
    [HideInInspector]
    public Transform
        frWheel;
    //public Transform rlWheel;
    //public Transform rrWheel;
    ////Fahrzeugsteuerung
    //public WheelCollider flWheelCollider;
    //public WheelCollider frWheelCollider;
    //public WheelCollider rlWheelCollider;
    //public WheelCollider rrWheelCollider;

    //ObstacleAvoidance
    [HideInInspector]
    public Transform
        viewPoint;
    public bool useObstacleAvoidance = true;
    public bool ignoreWaypointsForObstacleAvoidanceControl = false;
    public bool onlyStoppingWhileOa = false; 
    //public bool ignoreWaypointsForObstacleAvoidanceControl = false;
    public float oADistance = 10;
    public float oAWidth = 4;
    public float oASideDistance = 3;
    public float oASideOffset = 1.5f;
    public float oASideFromMid = 1.5f;
    public SteeringMode steeringMode = SteeringMode.Cautious;
    public float roadMaxWidth = 20; //Parameter for switching to next waypoint;  
    public float wpContactAreaRadius = 2;
    public LayerMask visibleLayers = -1;
    [HideInInspector]
    public GameObject
        viewPointLeftGO;
    [HideInInspector]
    public GameObject
        viewPointRightGO;
    [HideInInspector]
    public GameObject
        viewPointEndGO;
    [HideInInspector]
    public GameObject
        viewPointLeftEndGO;
    [HideInInspector]
    public GameObject
        viewPointRightEndGO;
    private float m_sqrDistanceToWaypoint = 4;
    private float m_sqrDistanceToWpOa = 0;
    private float m_sqrDistanceToWpNoOa = 4;
    private Vector3 m_leftDirection;
    private Vector3 m_rightDirection;
    private Vector3 m_centerPointL;
    private Vector3 m_centerPointR;
    private float m_obstacleAvoidanceWidth;
    private bool m_backwardDriving = false;
    private bool m_isBackwardDriving = false;
    private float m_currentMaxSteerAngle = 0;
    private float m_lastSqrDistanceNextWp;
    private float m_lastSqrDistanceAfterNextWp;
    
    [HideInInspector]
    public GameObject
        leftDirectionGO;
    [HideInInspector]
    public GameObject
        rightDirectionGO;
    //[HideInInspector]
    //public GameObject leftSideGO;
    //[HideInInspector]
    //public GameObject rightSideGO;
    [HideInInspector]
    public GameObject
        centerPointLGO;
    [HideInInspector]
    public GameObject
        centerPointRGO;
    [HideInInspector]
    public GameObject
        centerPointEndLGO;
    [HideInInspector]
    public GameObject
        centerPointEndRGO;
    [HideInInspector]
    public Transform
        frontCollider;
    [HideInInspector]
    public GameObject
        leftFrontSideGO;
    [HideInInspector]
    public GameObject
        rightFrontSideGO;
    [HideInInspector]
    public GameObject
        leftRearSideGO;
    [HideInInspector]
    public GameObject
        rightRearSideGO;
    [HideInInspector]
    public GameObject
        leftFrontSideEndGO;
    [HideInInspector]
    public GameObject
        rightFrontSideEndGO;
    [HideInInspector]
    public GameObject
        leftRearSideEndGO;
    [HideInInspector]
    public GameObject
        rightRearSideEndGO;
    Vector3 m_leftFrontSidePos;
    Vector3 m_rightFrontSidePos;
    Vector3 m_leftRearSidePos;
    Vector3 m_rightRearSidePos;
    //Event 1
    public delegate void LastWaypointHandler(GameVehicleEventArgs e);

    public static LastWaypointHandler onLastWaypoint;
    private float currentWaitTimeToSwitchBackToWpMode = 0;
    private float defaultWaitTimeToSwitchBackToWpMode = 5;
    private bool oAisActive = false;
    
    public enum DriveMode { 
        OneWay,
        Laps
    }

    public enum SteeringMode {
        Cautious,
        Tough
    }

    void Awake() {
                
        //2011-12-27
        //GetWaypointNames();
        //FillWaypointList();
        //2011-12-27      
        
        aiRespawnControllerScript = gameObject.GetComponent<GameVehicleRespawnController>();
        aiPreMotor = GetComponent<GameVehicleMotorMapping>();
        flWheel = aiPreMotor.flWheelMesh;
        frWheel = aiPreMotor.frWheelMesh;
    }

    void Start() {      
        m_sqrDistanceToWpNoOa = wpContactAreaRadius * wpContactAreaRadius;
        //2011-12-27
        GetWaypointNames();
        FillWaypointList();
        //2011-12-27
        steerAngle = aiPreMotor.steerMax;
        m_calcMaxSpeed = aiPreMotor.speedMax;
        m_maxSpeed = m_calcMaxSpeed; 
        m_sqrDistanceToWpOa = roadMaxWidth * roadMaxWidth;
        
        if (steerAngle < hsSteerAngle) {
            LogUtil.LogError("hsSteerAngle is bigger then aiPreMotor.steerMax. It has to be lower or equal.");
        }
        //ObstacleAvoidance
        if (useObstacleAvoidance) {
            //2012-07-10 
            if (ignoreWaypointsForObstacleAvoidanceControl) {                                                   
                m_sqrDistanceToWaypoint = m_sqrDistanceToWpOa; 
            }
            else {
                m_sqrDistanceToWaypoint = m_sqrDistanceToWpNoOa; //2012-06-24
            }
            //2012-07-10 
            
            ////unnoetig
            //if (viewPoint == null)
            //{
            //    viewPoint = transform.FindChild("ViewPoint");  
            //}

            viewPointLeftGO = new GameObject("viewPointLeftGO");
            viewPointLeftGO.transform.parent = transform;
            viewPointLeftGO.transform.position = viewPoint.transform.position;
            viewPointLeftGO.transform.position += viewPoint.TransformDirection((Vector3.right * flWheel.localPosition.x));
            viewPointLeftGO.transform.rotation = transform.rotation;

            viewPointRightGO = new GameObject("viewPointRightGO");
            viewPointRightGO.transform.parent = transform;
            viewPointRightGO.transform.position = viewPoint.transform.position;                      
            viewPointRightGO.transform.position += viewPoint.TransformDirection((Vector3.right * frWheel.localPosition.x));
            viewPointRightGO.transform.rotation = transform.rotation;

            //obstacleAvoidanceWidth = viewPointRightGO.transform.localPosition.x + oAWidth;           
            //leftDirection = viewPoint.position + viewPoint.TransformDirection((Vector3.left * obstacleAvoidanceWidth) + (Vector3.forward * oADistance));           
            //rightDirection = viewPoint.position + viewPoint.TransformDirection((Vector3.right * obstacleAvoidanceWidth) + (Vector3.forward * oADistance));
                
            m_centerPointL = transform.position + transform.TransformDirection(Vector3.left * oASideOffset);
            m_centerPointL.y = viewPoint.position.y;
            m_centerPointR = transform.position + transform.TransformDirection(Vector3.right * oASideOffset);
            m_centerPointR.y = viewPoint.position.y;

            //leftDirectionGO = new GameObject("leftDirectionGO");
            //leftDirectionGO.transform.parent = transform;
            //leftDirectionGO.transform.position = leftDirection;
            //leftDirectionGO.transform.rotation = transform.rotation;

            //rightDirectionGO = new GameObject("rightDirectionGO");
            //rightDirectionGO.transform.parent = transform;
            //rightDirectionGO.transform.position = rightDirection;
            //rightDirectionGO.transform.rotation = transform.rotation;

            //centerPointLGO = new GameObject("centerPointLGO");
            //centerPointLGO.transform.parent = transform;
            //centerPointLGO.transform.position = m_centerPointL;
            //centerPointLGO.transform.rotation = transform.rotation;           

            centerPointRGO = new GameObject("centerPointRGO");
            centerPointRGO.transform.parent = transform;
            centerPointRGO.transform.position = m_centerPointR;
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

            //centerPointEndLGO = new GameObject("centerPointEndLGO");
            //centerPointEndLGO.transform.parent = transform;
            //centerPointEndLGO.transform.position = m_centerPointL + transform.TransformDirection(Vector3.left * oASideDistance);
            //centerPointEndLGO.transform.rotation = transform.rotation;

            centerPointEndRGO = new GameObject("centerPointEndRGO");
            centerPointEndRGO.transform.parent = transform;
            centerPointEndRGO.transform.position = m_centerPointR + transform.TransformDirection(Vector3.right * oASideDistance);
            centerPointEndRGO.transform.rotation = transform.rotation;

            //-------------
            // Seiten-Raycasts--------------------------------------------------------------------------------------------

            //leftFront
            m_leftFrontSidePos = transform.position + transform.TransformDirection(Vector3.left * oASideOffset);
            m_leftFrontSidePos.y = viewPoint.position.y;
            m_leftFrontSidePos += transform.TransformDirection(Vector3.forward * oASideFromMid);

            leftFrontSideGO = new GameObject("leftFrontSideGO");
            leftFrontSideGO.transform.parent = transform;
            leftFrontSideGO.transform.position = m_leftFrontSidePos;
            leftFrontSideGO.transform.rotation = transform.rotation;

            leftFrontSideEndGO = new GameObject("leftFrontSideEndGO");
            leftFrontSideEndGO.transform.parent = transform;
            leftFrontSideEndGO.transform.position = m_leftFrontSidePos + transform.TransformDirection(Vector3.left * oASideDistance);
            leftFrontSideEndGO.transform.rotation = transform.rotation;

            //leftRear
            m_leftRearSidePos = transform.position + transform.TransformDirection(Vector3.left * oASideOffset);
            m_leftRearSidePos.y = viewPoint.position.y;
            m_leftRearSidePos -= transform.TransformDirection(Vector3.forward * oASideFromMid);
            
            leftRearSideGO = new GameObject("leftRearSideGO");
            leftRearSideGO.transform.parent = transform;
            leftRearSideGO.transform.position = m_leftRearSidePos;
            leftRearSideGO.transform.rotation = transform.rotation;

            leftRearSideEndGO = new GameObject("leftRearSideEndGO");
            leftRearSideEndGO.transform.parent = transform;
            leftRearSideEndGO.transform.position = m_leftRearSidePos + transform.TransformDirection(Vector3.left * oASideDistance);
            leftRearSideEndGO.transform.rotation = transform.rotation;

            //rightFront
            m_rightFrontSidePos = transform.position + transform.TransformDirection(Vector3.right * oASideOffset);
            m_rightFrontSidePos.y = viewPoint.position.y;
            m_rightFrontSidePos += transform.TransformDirection(Vector3.forward * oASideFromMid);

            rightFrontSideGO = new GameObject("rightFrontSideGO");
            rightFrontSideGO.transform.parent = transform;
            rightFrontSideGO.transform.position = m_rightFrontSidePos;
            rightFrontSideGO.transform.rotation = transform.rotation;

            rightFrontSideEndGO = new GameObject("rightFrontSideEndGO");
            rightFrontSideEndGO.transform.parent = transform;
            rightFrontSideEndGO.transform.position = m_rightFrontSidePos + transform.TransformDirection(Vector3.right * oASideDistance);
            rightFrontSideEndGO.transform.rotation = transform.rotation;

            //rightRear
            m_rightRearSidePos = transform.position + transform.TransformDirection(Vector3.right * oASideOffset);
            m_rightRearSidePos.y = viewPoint.position.y;
            m_rightRearSidePos -= transform.TransformDirection(Vector3.forward * oASideFromMid);

            rightRearSideGO = new GameObject("rightRearSideGO");
            rightRearSideGO.transform.parent = transform;
            rightRearSideGO.transform.position = m_rightRearSidePos;
            rightRearSideGO.transform.rotation = transform.rotation;

            rightRearSideEndGO = new GameObject("rightRearSideEndGO");
            rightRearSideEndGO.transform.parent = transform;
            rightRearSideEndGO.transform.position = m_rightRearSidePos + transform.TransformDirection(Vector3.right * oASideDistance);
            rightRearSideEndGO.transform.rotation = transform.rotation;

            //Ende Seiten-Raycasts--------------------------------------------------------------------------------------------
            
            //------------
            leftDirectionGO = new GameObject("leftDirectionGO");
            leftDirectionGO.transform.parent = transform;
            leftDirectionGO.transform.position = viewPointLeftEndGO.transform.position + viewPointLeftEndGO.transform.TransformDirection(Vector3.left * oAWidth);
            leftDirectionGO.transform.rotation = transform.rotation;

            rightDirectionGO = new GameObject("rightDirectionGO");
            rightDirectionGO.transform.parent = transform;
            rightDirectionGO.transform.position = viewPointRightEndGO.transform.position + viewPointRightEndGO.transform.TransformDirection(Vector3.right * oAWidth);
            //rightDirectionGO.transform.position = viewPointRightEndGO.transform.position;
            
            rightDirectionGO.transform.rotation = transform.rotation;
            //------------

            m_leftRightDistanceLength = Vector3.Distance(viewPointRightGO.transform.position, rightDirectionGO.transform.position);
            m_leftRightSideDistanceLength = Vector3.Distance(centerPointRGO.transform.position, centerPointEndRGO.transform.position);            
            m_frontDistanceLength = Vector3.Distance(viewPoint.transform.position, viewPointEndGO.transform.position);
            //LogUtil.Log("leftRightDistanceLength: " + leftRightDistanceLength.ToString());
            //LogUtil.Log("leftRightSideDistanceLength: " + leftRightSideDistanceLength.ToString());
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
        else {
            m_sqrDistanceToWaypoint = m_sqrDistanceToWpNoOa;
        }
    }

    void FixedUpdate() {
        
        m_currentSpeed = Mathf.Round(rigidbody.velocity.magnitude * 3.6f);
        
        if (m_currentSpeed > (m_maxSpeed + 10)) {
            m_isBraking = true;
        }
        else {
            m_isBraking = false;            
            aiPreMotor.brakeInput = 0;
        }

        if (m_isBraking == false) {
           
            if (m_currentSpeed < m_maxSpeed) {                                 
                aiPreMotor.motorInput = aiSpeedPedal;
            }
            else {
                aiPreMotor.motorInput = 0;
            }
        }
        else {
            aiPreMotor.motorInput = 0;
            aiPreMotor.brakeInput = 1;
        }
        
        if (ignoreWaypointsForObstacleAvoidanceControl) {
            //Old AI Behaviour
            AIIgnoreWaypointsForObstacleAvoidanceControl();
        }
        else {
            AI();
        }

    }
    
    void AIIgnoreWaypointsForObstacleAvoidanceControl() {
        if (currentWaypoint < waypoints.Count) {
            Vector3 target = waypoints[currentWaypoint].position;
            Vector3 moveDirection = target - transform.position;                
            Vector3 localTarget = transform.InverseTransformPoint(waypoints[currentWaypoint].position);
            
            
            
            //je hoeher die Geschwindigkeit,  desto geringer der maximale Einschlagwinkel.

            float speedProcent = m_currentSpeed / m_calcMaxSpeed;
            speedProcent = Mathf.Clamp(speedProcent, 0, 1);

            m_currentMaxSteerAngle = steerAngle - ((steerAngle - hsSteerAngle) * speedProcent);
            if (!useObstacleAvoidance) {
                m_targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;              
            }
            else {
                //ObstacleAvoidance
                
                //m_targetAngle = ObstacleAvoidanceSteering();  
                bool dummy = false;
                m_targetAngle = ObstacleAvoidanceSteering(ref dummy);
            }

            if (currentAngle < m_targetAngle) {
                currentAngle = currentAngle + (Time.deltaTime * steeringSpeed);
                if (currentAngle > m_targetAngle) {
                    currentAngle = m_targetAngle;
                }
            }
            else if (currentAngle > m_targetAngle) {
                currentAngle = currentAngle - (Time.deltaTime * steeringSpeed);
                if (currentAngle < m_targetAngle) {
                    currentAngle = m_targetAngle;
                }
            }
                        
            aiSteerAngle = Mathf.Clamp(currentAngle, (-1) * m_currentMaxSteerAngle, m_currentMaxSteerAngle);

            aiPreMotor.steerInput = (aiSteerAngle / m_currentMaxSteerAngle);
            
              
            //Noch Performance pruefen und ggf. verbessern!!!
            //Vector3 afterNextPos  = waypoints[AfterNextWaypointIndex()].position;
            //Vector3 moveDirectionAfter  = afterNextPos - transform.position;
            //float afterNextSqrDistance =  moveDirectionAfter.sqrMagnitude;
            float sqrMagnitude = moveDirection.sqrMagnitude;
            
            
            //if (moveDirection.sqrMagnitude < m_sqrDistanceToWaypoint)
            if (sqrMagnitude < m_sqrDistanceToWaypoint) {                 
                  
                NextWaypoint();
                    
                //GameVehicleAIWaypoint aiWaypoint;
                //aiWaypoint = waypoints[currentWaypoint].GetComponent("GameVehicleAIWaypoint") as GameVehicleAIWaypoint;
                
                //if (aiWaypoint != null)
                //{
                //    m_maxSpeed = aiWaypoint.speed;
                //}
                //else 
                //{
                //    m_maxSpeed = m_calcMaxSpeed;
                //}
                
                //currentWaypoint++;

                //aiRespawnControllerScript.lastTimeToReachNextWP = 0;

                //if (currentWaypoint >= waypoints.Count)
                //{
                //    //fire event BEGIN
                //    if (onLastWaypoint != null)
                //    {
                //        GameVehicleEventArgs e = new GameVehicleEventArgs();
                //        e.name = gameObject.name;
                //        e.currentWaypointIndex = currentWaypoint;
                //        e.currentWaypointName = waypoints[currentWaypoint -1].name;
                //        e.position = gameObject.transform.position;
                //        e.rotation = gameObject.transform.rotation;
                //        e.tag = gameObject.tag;
                //        onLastWaypoint(e);
                //    }
                //    //fire event END
                //}
                
                
                //afterNextPos  = waypoints[AfterNextWaypointIndex()].position;
                //moveDirectionAfter  = afterNextPos - transform.position;
                //afterNextSqrDistance =  moveDirectionAfter.sqrMagnitude;
                //target  = waypoints[currentWaypoint].position;
                //moveDirection  = target - transform.position; 
                //sqrMagnitude = moveDirection.sqrMagnitude;
                
            }
            //else if(sqrMagnitude > m_lastSqrDistanceNextWp) 
            //{
            //  LogUtil.Log("afterNextSqrDistance: " + afterNextSqrDistance.ToString());
            //  if (m_lastSqrDistanceAfterNextWp > afterNextSqrDistance)
            //  {
            //if i drive aside my waypoint, but I'm on the right way, switch to the next waypoint! --> TESTEN!!!
            //      NextWaypoint(); 
                    
            //      afterNextPos  = waypoints[AfterNextWaypointIndex()].position;
            //      moveDirectionAfter  = afterNextPos - transform.position;
            //      afterNextSqrDistance =  moveDirectionAfter.sqrMagnitude;
            //      target  = waypoints[currentWaypoint].position;
            //      moveDirection  = target - transform.position; 
            //      sqrMagnitude = moveDirection.sqrMagnitude;
            //  }
            //} 
            
            //m_lastSqrDistanceNextWp = sqrMagnitude; 
            //m_lastSqrDistanceAfterNextWp = afterNextSqrDistance;
            
        }
        else {
            
            //if (driveCircleModus)
            if (driveMode == DriveMode.Laps) {       
                currentWaypoint = 0; 
            }
            else {               
                aiSpeedPedal = 0;                
                aiRespawnControllerScript.enabled = false;                            
            }
            
        }        
        
    }
    
    void AI() {
        bool linecastsHitsObject = false;
        
        if (currentWaypoint < waypoints.Count) {
            
            Vector3 target = waypoints[currentWaypoint].position;
            Vector3 moveDirection = target - transform.position;                
            Vector3 localTarget = transform.InverseTransformPoint(waypoints[currentWaypoint].position);

            Vector3 localTargetNext; 
            float targetAngleNext;
            
            if (currentWaypoint + 2 < waypoints.Count) {
                localTargetNext = transform.InverseTransformPoint(waypoints[currentWaypoint + 2].position);
            }
            else {
                localTargetNext = transform.InverseTransformPoint(waypoints[currentWaypoint + 2 - waypoints.Count].position);
            }   

            //je hoeher die Geschwindigkeit,  desto geringer der maximale Einschlagwinkel.

            float speedProcent = m_currentSpeed / m_calcMaxSpeed;
            speedProcent = Mathf.Clamp(speedProcent, 0, 1);

            m_currentMaxSteerAngle = steerAngle - ((steerAngle - hsSteerAngle) * speedProcent);
            
            if (!useObstacleAvoidance) {
                m_targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;
                
                //targetAngleNext = Mathf.Atan2(localTargetNext.x, localTargetNext.z) * Mathf.Rad2Deg;
                //if (Mathf.Abs(m_targetAngle) > Mathf.Abs (targetAngleNext))
                //  m_targetAngle = targetAngleNext;

            }
            else {
                 
                if (onlyStoppingWhileOa) {
                    bool hitFront = ObstacleAvoidanceFrontDetection();                  
                    //float targetAngleWp   = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg; //2012-08-06
                    m_targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;      //2012-08-06

                    targetAngleNext = Mathf.Atan2(localTargetNext.x, localTargetNext.z) * Mathf.Rad2Deg;
                    if (Mathf.Abs(m_targetAngle) > Mathf.Abs(targetAngleNext))
                        m_targetAngle = targetAngleNext;

                    if (hitFront) {
                        m_isBraking = true;
                        aiPreMotor.motorInput = 0;
                        aiPreMotor.brakeInput = 1;
                    }
                    else {
                        m_isBraking = false;
                    }
                    
                }
                else {
                     
                    //ObstacleAvoidance
                    //m_targetAngle = ObstacleAvoidanceSteering();  
                    float targetAngleOa = ObstacleAvoidanceSteering(ref linecastsHitsObject);                   
                    float targetAngleWp = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;

                    if (optimizedWpTargeting) {
                        
                        //LogUtil.Log("doublewptest");                    
                        targetAngleNext = Mathf.Atan2(localTargetNext.x, localTargetNext.z) * Mathf.Rad2Deg;
                        if (Mathf.Abs(targetAngleWp) > Mathf.Abs(targetAngleNext))
                            targetAngleWp = targetAngleNext;
                        
                    }
                    
                    targetAngleWp = Mathf.Clamp(targetAngleWp, (-1) * m_currentMaxSteerAngle, m_currentMaxSteerAngle);//2012-06-30
                    //LogUtil.Log("targetAngleOa: " + targetAngleOa + "; targetAngleWp: " + targetAngleWp);
                    
                    if (!linecastsHitsObject) {
                        
                        //m_targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;
                        if (!oAisActive) {
                            m_targetAngle = targetAngleWp;
                            m_sqrDistanceToWaypoint = m_sqrDistanceToWpNoOa;
                        }
                        else {
                            
                            //m_targetAngle = targetAngleOa;

                            //m_targetAngle = (targetAngleWp + targetAngleOa) / 2;                      
                            m_targetAngle = GetTimeDependendSteeringAngle(targetAngleOa, targetAngleWp);

                            m_sqrDistanceToWaypoint = m_sqrDistanceToWpOa;
                            
                        }
                        
                    }
                    else {
                        
                        //wenn rechts ein Fremdobjekt ist und mein naechster WP links ist, dann nehme ich den Mittel-Weg, der durch die WPs vorgeben wird.
                        if ((targetAngleOa > 0.5 && targetAngleWp > 0.5) || (targetAngleOa < -0.5 && targetAngleWp < -0.5)) { //0.5 nehme ich, um nicht bei kleinen Differenzen auf die Nase zu fallen.
                            
                            //m_targetAngle = targetAngleWp;
                            //m_targetAngle = (targetAngleWp + targetAngleOa) / 2;                      
                            m_targetAngle = GetTimeDependendSteeringAngle(targetAngleOa, targetAngleWp);

                        }
                        else {
                            m_targetAngle = targetAngleOa;
                        }
                        
                        m_sqrDistanceToWaypoint = m_sqrDistanceToWpOa;
                        
                        if (oAisActive) {
                            currentWaitTimeToSwitchBackToWpMode = defaultWaitTimeToSwitchBackToWpMode;
                        }
                        else {
                            StartCoroutine(WaitForSwitchingToWpMode());
                        }
                        
                    }
                }
                
            }

            if (currentAngle < m_targetAngle) {
                currentAngle = currentAngle + (Time.deltaTime * steeringSpeed);
                if (currentAngle > m_targetAngle) {
                    currentAngle = m_targetAngle;
                }
            }
            else if (currentAngle > m_targetAngle) {
                currentAngle = currentAngle - (Time.deltaTime * steeringSpeed);
                if (currentAngle < m_targetAngle) {
                    currentAngle = m_targetAngle;
                }
            }
                        
            aiSteerAngle = Mathf.Clamp(currentAngle, (-1) * m_currentMaxSteerAngle, m_currentMaxSteerAngle);

            aiPreMotor.steerInput = (aiSteerAngle / m_currentMaxSteerAngle);
            
              
            //Noch Performance pruefen und ggf. verbessern!!!
            //Vector3 afterNextPos  = waypoints[AfterNextWaypointIndex()].position;
            //Vector3 moveDirectionAfter  = afterNextPos - transform.position;
            //float afterNextSqrDistance =  moveDirectionAfter.sqrMagnitude;
            float sqrMagnitude = moveDirection.sqrMagnitude;
            
            
            //if (moveDirection.sqrMagnitude < m_sqrDistanceToWaypoint)
            if (sqrMagnitude < m_sqrDistanceToWaypoint) {                 
                  
                NextWaypoint();
                                    
            }
            
        }
        else {
            
            //if (driveCircleModus)
            if (driveMode == DriveMode.Laps) {       
                currentWaypoint = 0; 
            }
            else {               
                aiSpeedPedal = 0;                
                aiRespawnControllerScript.enabled = false;                            
            }
            
        }        
        
    }
        
    private float GetTimeDependendSteeringAngle(float angleOa, float angleWp) {
        float angle = 0;
        currentWaitTimeToSwitchBackToWpMode = 0;
        defaultWaitTimeToSwitchBackToWpMode = 5;
        float percent = currentWaitTimeToSwitchBackToWpMode / defaultWaitTimeToSwitchBackToWpMode;
        
        angle = angleOa * (1 - percent) + angleWp * percent;
        return angle;
    }
    
    private IEnumerator WaitForSwitchingToWpMode() {
        
        
        while (currentWaitTimeToSwitchBackToWpMode > 0) {
            yield return new WaitForSeconds(1);
            currentWaitTimeToSwitchBackToWpMode -= 1;
        }
        
        oAisActive = false;
    }
    
    public void NextWaypoint() {
        GameVehicleAIWaypoint aiWaypoint;
        aiWaypoint = waypoints[currentWaypoint].GetComponent("GameVehicleAIWaypoint") as GameVehicleAIWaypoint;
        
        if (aiWaypoint != null) {
            //m_maxSpeed = aiWaypoint.speed;//2012-07-09
            m_maxSpeed = aiWaypoint.speed + offsetWaypointSpeed;//2012-07-09
        }
        else {
            m_maxSpeed = m_calcMaxSpeed;
        }
        
        currentWaypoint++;

        aiRespawnControllerScript.lastTimeToReachNextWP = 0;

        if (currentWaypoint >= waypoints.Count) {
            //currentWaypoint = waypoints.Count -1; //2011-12-27
            //fire event BEGIN
            if (onLastWaypoint != null) {
                GameVehicleEventArgs e = new GameVehicleEventArgs();
                e.name = gameObject.name;
                e.currentWaypointIndex = currentWaypoint;
                e.currentWaypointName = waypoints[currentWaypoint - 1].name; //2011-12-27
                //e.currentWaypointName = waypoints[currentWaypoint].name;  //2011-12-27
                e.position = gameObject.transform.position;
                e.rotation = gameObject.transform.rotation;
                e.tag = gameObject.tag;
                onLastWaypoint(e);
            }
            //fire event END
        }
    }
    
    
    //private int AfterNextWaypointIndex()
    //{                     
    //  int nextWaypoint = currentWaypoint +1;       

    //    if (nextWaypoint >= waypoints.Count)
    //    {
    //        nextWaypoint = 0;
    //    }
    //  return nextWaypoint;
    //}
    
    
    void FillWaypointList() {               
        waypoints.Clear(); //2012-06-23
        bool found = true;
        int counter = 1;
        while (found) {
            GameObject go;                      
            string currentName;
            currentName = "/" + m_waypointFolder + "/" + m_waypointPreName + counter.ToString();            
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
                forerunnerName = "/" + m_waypointFolder + "/" + m_waypointPreName + (counter - 2).ToString();                               
                GameObject forerunnerGo;
                forerunnerGo = GameObject.Find(forerunnerName);
                forerunnerGo.transform.LookAt(go.transform);
            }

            //rotate last waypoint
            if (counter > 2 && !found) {
                string lastName;
                lastName = "/" + m_waypointFolder + "/" + m_waypointPreName + (counter - 1).ToString();
                GameObject lastGo;
                lastGo = GameObject.Find(lastName);                

                string firstName;
                firstName = "/" + m_waypointFolder + "/" + m_waypointPreName + "1";               
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
            m_waypointPreName = aiWaypointEditor.preName + "_";
            m_waypointFolder = aiWaypointEditor.folderName;
        }
    }
     
    /// <summary>
    /// Calculate the steering angle when using Obstacles avoidance.
    /// </summary>
    /// <returns>
    /// The steering angle.
    /// </returns>
    /// <param name='hitsObject'>
    /// returns if linecasts hits an object.
    /// </param>
    //ObstacleAvoidance
    float ObstacleAvoidanceSteering(ref bool hitsObject) {
        float newSteerAngle;
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
        
        hitsObject = false; 
        //Vector3 forwardDirection = viewPoint.TransformDirection(Vector3.forward * oADistance);      

        //front raycasts BEGIN ------------------------------------     

        if (Physics.Linecast(viewPoint.position, viewPointEndGO.transform.position, out hitFrontMid, visibleLayers)) {
            
            frontContact = true;
            hitsObject = true; 
            frontMinDistance = hitFrontMid.distance;
            frontMaxDistance = hitFrontMid.distance; 
            
            
        }        
       
        if (Physics.Linecast(viewPointLeftGO.transform.position, viewPointLeftEndGO.transform.position, out hitFrontLeft, visibleLayers)) {
            frontContact = true;
            hitsObject = true; 
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
            hitsObject = true; 
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

        //cast forward sideways BEGIN ------------------------------------        
        if (Physics.Linecast(viewPointLeftGO.transform.position, leftDirectionGO.transform.position, out hitL, visibleLayers)) {
            hitsObject = true; 
            leftDistance = hitL.distance;  
            
        }
                
        if (Physics.Linecast(viewPointRightGO.transform.position, rightDirectionGO.transform.position, out hitR, visibleLayers)) {
            hitsObject = true; 
            rightDistance = hitR.distance;
        }
        //cast forward sideways END ------------------------------------ 


        //Space for avoiding/steering? BEGIN ------------------------------------         
        //if (Physics.Linecast(centerPointLGO.transform.position, centerPointEndLGO.transform.position, out hitLSide, visibleLayers))
        //{
        //    leftSideDistance = hitLSide.distance;
        //    //LogUtil.Log("center left: " + hitLSide.collider.gameObject.name);
        //}        
        if (Physics.Linecast(leftFrontSideGO.transform.position, leftFrontSideEndGO.transform.position, out hitLSide, visibleLayers)) {
            hitsObject = true; 
            leftSideDistance = hitLSide.distance;            
            //weil wir gleich und auch spaeter auf 0 pruefen und annehmen, dass bei 0 keine Detektion stattfindet.
            if (leftSideDistance == 0) {
                leftSideDistance = 0.01f;
            }
            
        }

        if (Physics.Linecast(leftRearSideGO.transform.position, leftRearSideEndGO.transform.position, out hitLSide, visibleLayers)) {
            
            hitsObject = true; 
            if (leftSideDistance == 0 || leftSideDistance > hitLSide.distance) {
                leftSideDistance = hitLSide.distance;
            }
            //weil wir spaeter auf 0 pruefen und annehmen, dass bei 0 keine Detektion stattfindet.
            if (leftSideDistance == 0) {
                leftSideDistance = 0.01f;
            }
            
        }

        //if (Physics.Linecast(centerPointRGO.transform.position, centerPointEndRGO.transform.position, out hitRSide, visibleLayers))
        //{
        //    rightSideDistance = hitRSide.distance;
        //}
        if (Physics.Linecast(rightFrontSideGO.transform.position, rightFrontSideEndGO.transform.position, out hitRSide, visibleLayers)) {
            hitsObject = true; 
            rightSideDistance = hitRSide.distance;
            //weil wir gleich und auch spaeter auf 0 pruefen und annehmen, dass bei 0 keine Detektion stattfindet.
            if (rightSideDistance == 0) {
                rightSideDistance = 0.01f;
            }
        }

        if (Physics.Linecast(rightRearSideGO.transform.position, rightRearSideEndGO.transform.position, out hitRSide, visibleLayers)) {
            hitsObject = true; 
            if (rightSideDistance == 0 || rightSideDistance > hitRSide.distance) {
                rightSideDistance = hitRSide.distance;
            }
            //weil wir spaeter auf 0 pruefen und annehmen, dass bei 0 keine Detektion stattfindet.
            if (rightSideDistance == 0) {
                rightSideDistance = 0.01f;
            }
        }

        //Space for avoiding/steering? END ------------------------------------    


        newSteerAngle = SteeringDecision(leftSideDistance, rightSideDistance, leftDistance, rightDistance, frontMinDistance, frontContact, steeringMode);

        if (m_backwardDriving) {

            if (m_currentSpeed > 2 && m_isBackwardDriving == false) {
               
                aiPreMotor.motorInput = 0;
                aiPreMotor.brakeInput = 1;           

            }
            else {
                
                aiPreMotor.motorInput = -0.5f;
                aiPreMotor.brakeInput = 0;
                newSteerAngle = (-1) * newSteerAngle;                
                m_isBackwardDriving = true;
            }

            if (frontMinDistance > 8 || frontMinDistance == 0) {
                m_backwardDriving = false;
            }

        }
        else {
            m_isBackwardDriving = false;
        }
        
        return newSteerAngle;
    }
    
     
    public bool ObstacleAvoidanceFrontDetection() {
        //float newSteerAngle;
        //bool frontContact = false;
        float frontMinDistance = 0;
        float frontMaxDistance = -1;
        //float leftDistance = 0;
        //float rightDistance = 0;
       // float leftSideDistance = 0;
       // float rightSideDistance = 0;
        //float localSteeringAngle = steerAngle;
        //bool doSteering = false;
        RaycastHit hitFrontMid;
        RaycastHit hitFrontLeft;
        RaycastHit hitFrontRight;
        //RaycastHit hitR;
        //RaycastHit hitL;
        //RaycastHit hitRSide;
        //RaycastHit hitLSide;
        bool hitsObject = false;
        
        hitsObject = false; 
        //Vector3 forwardDirection = viewPoint.TransformDirection(Vector3.forward * oADistance);      

        //front raycasts BEGIN ------------------------------------     

        if (Physics.Linecast(viewPoint.position, viewPointEndGO.transform.position, out hitFrontMid, visibleLayers)) {
            
            //frontContact = true;
            hitsObject = true; 
            frontMinDistance = hitFrontMid.distance;
            frontMaxDistance = hitFrontMid.distance; 
            
            
        }        
       
        if (Physics.Linecast(viewPointLeftGO.transform.position, viewPointLeftEndGO.transform.position, out hitFrontLeft, visibleLayers)) {
            //frontContact = true;
            hitsObject = true; 
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
            //frontContact = true;
            hitsObject = true; 
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

        
        return hitsObject;
    }

    private float SteeringDecision(float leftSideDistance, float rightSideDistance, float leftDistance, float rightDistance, float frontMinDistance, bool frontContact, SteeringMode style) {
        //float localSteeringAngle = steerAngle;
        float localSteeringAngle = m_currentMaxSteerAngle;
        
        float result = 0;
        //keine Prozent sonder von 0 bis 1;
        float rightPercent = 1;
        float rightSidePercent = 1;
        float leftPercent = 1;
        float leftSidePercent = 1;

        if (frontContact && (frontMinDistance < 2)) {
            m_backwardDriving = true;
        }

        switch (style) { 
        case SteeringMode.Cautious:
                //steer left
            if (leftSideDistance == 0 && ((leftDistance == 0 && rightDistance > 0) || (rightDistance != 0 && leftDistance != 0 && leftDistance > rightDistance)                 
                || (leftDistance == 0 && frontMinDistance > 0) || (rightDistance < leftDistance && frontMinDistance > 0 && rightDistance != 0) || (frontContact == false && rightSideDistance > 0))) {
                //|| (leftDistance == 0 && frontMinDistance > 0) || (rightDistance > leftDistance && frontMinDistance > 0) || (frontContact==false && rightSideDistance > 0)))
                if (!steerAbsolute) {
                    //result = (-1) * localSteeringAngle; old
                    if (frontMinDistance > 0) {
                        result = (-1) * localSteeringAngle;                 
                        //rightPercent = frontMinDistance / m_frontDistanceLength;                  
                        //result = (-1) * localSteeringAngle * (1 - rightPercent);
                    }
                    else {
                                                   
                        if (rightSideDistance > 0) {
                            rightSidePercent = rightSideDistance / m_leftRightSideDistanceLength;
                        }
    
                        if (rightDistance > 0) {
                            rightPercent = rightDistance / m_leftRightDistanceLength;
                        }
    
                        if (rightSidePercent < rightPercent) {
                            result = (-1) * localSteeringAngle * (1 - rightSidePercent);
                        }
                        else {
                            result = (-1) * localSteeringAngle * (1 - rightPercent);
                        }
    
                    }
                }
                else {
                    result = (-1) * localSteeringAngle; 
                }
                                        
            }

                //steer right
                //No "(leftSideDistance > 0) ||" because pushing away should not be possible
            if (rightSideDistance == 0 && ((rightDistance == 0 && leftDistance > 0) || (rightDistance != 0 && leftDistance != 0 && rightDistance > leftDistance)    
                || (rightDistance == 0 && frontMinDistance > 0) || (leftDistance < rightDistance && frontMinDistance > 0 && leftDistance != 0) || (frontContact == false && leftSideDistance > 0))) {
                //|| (rightDistance == 0 && frontMinDistance > 0) || (leftDistance > rightDistance && frontMinDistance > 0) || (frontContact == false && leftSideDistance > 0)))
                if (!steerAbsolute) {
                    if (frontMinDistance > 0) {
                        result = localSteeringAngle;
                        //leftPercent = frontMinDistance / m_frontDistanceLength;                   
                        //result = localSteeringAngle * (1 - leftPercent);
                    }
                    else {
                            
                        if (leftSideDistance > 0) {
                            leftSidePercent = leftSideDistance / m_leftRightSideDistanceLength;                           
                        }
    
                        if (leftDistance > 0) {
                            leftPercent = leftDistance / m_leftRightDistanceLength;                          
                        }
    
                        if (leftSidePercent < leftPercent) {
                            result = localSteeringAngle * (1 - leftSidePercent);
                        }
                        else {
                            result = localSteeringAngle * (1 - leftPercent);
                        }                   
                            
                    }                   
                }
                else {
                    result = localSteeringAngle;
                }
            }

            if (rightSideDistance != 0 && leftSideDistance != 0) {
                if (rightDistance > 0) {
                    rightPercent = rightDistance / m_leftRightDistanceLength;
                }

                if (leftDistance > 0) {
                    leftPercent = leftDistance / m_leftRightDistanceLength;
                }

                if (rightPercent < leftPercent || leftPercent == 0) {
                    result = (-1) * localSteeringAngle * (1 - rightPercent);
                    
                }
                else if (rightPercent > leftPercent || rightPercent == 0) {
                    result = localSteeringAngle * (1 - leftPercent);
                }
            }

            break;

        case SteeringMode.Tough:
                //steering
                //steer left
                //No "(rightSideDistance > 0) ||" because pushing away should not be possible
            if (leftSideDistance == 0 && ((leftDistance == 0 && rightDistance > 0) || (rightDistance != 0 && leftDistance != 0 && leftDistance > rightDistance)
                || (leftDistance == 0 && frontMinDistance > 0) || (rightDistance > leftDistance && frontMinDistance > 0))) {
                if (!steerAbsolute) {
                    
                    if (frontMinDistance > 0) {
                        result = (-1) * localSteeringAngle;
                        //rightPercent = frontMinDistance / m_frontDistanceLength;                  
                        //result = (-1) * localSteeringAngle * (1 - rightPercent);
                    }
                    else {
                                                   
                        if (rightDistance > 0) {
                            rightPercent = rightDistance / m_leftRightDistanceLength;
                        }
                           
                        result = (-1) * localSteeringAngle * (1 - rightPercent);
                            
    
                    }
                }
                else {
                    result = (-1) * localSteeringAngle;
                }
            }

                //steer right
                //No "(leftSideDistance > 0) ||" because pushing away should not be possible
            if (rightSideDistance == 0 && ((rightDistance == 0 && leftDistance > 0) || (rightDistance != 0 && leftDistance != 0 && rightDistance > leftDistance)
                || (rightDistance == 0 && frontMinDistance > 0) || (leftDistance > rightDistance && frontMinDistance > 0))) {
                if (!steerAbsolute) {
                    //result = localSteeringAngle;
                    if (frontMinDistance > 0) {
                        result = localSteeringAngle;
                        //leftPercent = frontMinDistance / m_frontDistanceLength;                   
                        //result = localSteeringAngle * (1 - leftPercent);
                    }
                    else {
    
                        if (leftDistance > 0) {
                            leftPercent = leftDistance / m_leftRightDistanceLength;
                        }
    
                        result = localSteeringAngle * (1 - leftPercent);
    
                    }
                }
                else {
                    result = localSteeringAngle;
                }
            }
            break;        
        }    
            
        return result;
    }

    public void SwitchOaMode(bool useOa) {
        useObstacleAvoidance = useOa;
        if (useOa) {
            m_sqrDistanceToWaypoint = m_sqrDistanceToWpOa;  
        }
        else {
            m_sqrDistanceToWaypoint = m_sqrDistanceToWpNoOa;    
        }
    }
    
    public void SetNewWaypointSet(string folderName, string preName, float maxSpeed, int nextWaypointNo) {
        
        GameVehicleAIWaypointEditor aiWaypointEditor;

        aiWaypointEditor = GetComponent("GameVehicleAIWaypointEditor") as GameVehicleAIWaypointEditor;
        if (aiWaypointEditor != null) {
            aiWaypointEditor.folderName = folderName;
            aiWaypointEditor.preName = preName;           
            
        }
        
        GetWaypointNames();
        FillWaypointList();
        
        m_maxSpeed = maxSpeed;      
        currentWaypoint = nextWaypointNo - 1;
        
    }
    
}
