#pragma warning disable 0414
using UnityEngine;
using System.IO;
using System.Collections;

public class GameVehicleDriveWheel {
    public static string DriveFWD = "FWD";
    public static string DriveRWD = "RWD";
    public static string Drive4WD = "4WD";
}

public class BaseGameVehicleController : GameObjectBehavior {

    public WheelCollider colliderFL;
    public WheelCollider colliderFR;
    public WheelCollider colliderRL;
    public WheelCollider colliderRR;
    private Transform wheelFL;
    private Transform wheelFR;
    private Transform wheelRL;
    private Transform wheelRR;
    public float MaxSteeringAngle = 20;
    public float MaxEngineSpeed = 150;
    public string Drive = "FWD";
    private Transform CoG;
    private float steer = 0;
    private float forward = 0;
    private float back = 0;
    private bool brakeRelease = false;
    private float motor = 0;
    private float brake = 0;
    private bool reverse = false;
    private float speed = 0;
    private Vector3 ColliderCenterPointFL;
    private Vector3 ColliderCenterPointFR;
    private Vector3 ColliderCenterPointRL;
    private Vector3 ColliderCenterPointRR;
    private RaycastHit hit;
    private Rect windowRect;
    private int linefeed = 20;
    private bool  bShowDialog = true;
    private float RotationValueFL = 0.0f;
    private float RotationValueFR = 0.0f;
    private string RigType = "AlabCar";

    
    private float sliderWidth = 80;
    private float margin = 5;
    private float txtwidth = 200;
    
    public void Start() {

        wheelFL = transform.Find("/" + name + "/Wheels/FLWheel");
        wheelFR = transform.Find("/" + name + "/Wheels/FRWheel");
        wheelRL = transform.Find("/" + name + "/Wheels/RLWheel");
        wheelRR = transform.Find("/" + name + "/Wheels/RRWheel");
        
        CoG = transform.Find("/" + name + "/CenterOfGravity");

        rigidbody.centerOfMass = new Vector3(0, CoG.localPosition.y, 0);
    }
    
    void  Update() {
        
        if (Input.GetKeyDown(KeyCode.H)) {
            if (bShowDialog == true) {
                bShowDialog = false;
            }
            else {
                bShowDialog = true;
            }
        }
        
        colliderFL.steerAngle = MaxSteeringAngle * Input.GetAxis("Horizontal");
        colliderFR.steerAngle = MaxSteeringAngle * Input.GetAxis("Horizontal");
        
        if (Drive == GameVehicleDriveWheel.DriveFWD || Drive == GameVehicleDriveWheel.Drive4WD) {
            colliderFL.motorTorque = MaxEngineSpeed * Input.GetAxis("Vertical");
            colliderFR.motorTorque = MaxEngineSpeed * Input.GetAxis("Vertical");
        }
        if (Drive == "RWD" || Drive == "4WD") {
            colliderRL.motorTorque = MaxEngineSpeed * Input.GetAxis("Vertical");
            colliderRR.motorTorque = MaxEngineSpeed * Input.GetAxis("Vertical");
        }
        
        colliderFR.steerAngle = colliderFL.steerAngle;
        
        ColliderCenterPointFL = colliderFL.transform.TransformPoint(colliderFL.center);
        ColliderCenterPointFR = colliderFR.transform.TransformPoint(colliderFR.center);
        ColliderCenterPointRL = colliderRL.transform.TransformPoint(colliderRL.center);
        ColliderCenterPointRR = colliderRR.transform.TransformPoint(colliderRR.center);
        
        if (Physics.Raycast(ColliderCenterPointFL, -colliderFL.transform.up, out hit, colliderFL.suspensionDistance + colliderFL.radius)) {
            wheelFL.transform.position = hit.point + (colliderFL.transform.up * colliderFL.radius);
        }
        else {
            wheelFL.transform.position = ColliderCenterPointFL - (colliderFL.transform.up * colliderFL.suspensionDistance);
        }
        wheelFL.Rotate(colliderFL.rpm * 6 * Time.deltaTime, 0, 0);
        
        if (Physics.Raycast(ColliderCenterPointFR, -colliderFR.transform.up, out hit, colliderFR.suspensionDistance + colliderFR.radius)) {
            wheelFR.transform.position = hit.point + (colliderFR.transform.up * colliderFR.radius);
        }
        else {
            wheelFR.transform.position = ColliderCenterPointFR - (colliderFR.transform.up * colliderFR.suspensionDistance);
        }
        wheelFR.Rotate(colliderFR.rpm * -6 * Time.deltaTime, 0, 0);
        
        if (Physics.Raycast(ColliderCenterPointRL, -colliderRL.transform.up, out hit, colliderRL.suspensionDistance + colliderRL.radius)) {
            wheelRL.transform.position = hit.point + (colliderRL.transform.up * colliderRL.radius);
        }
        else {
            wheelRL.transform.position = ColliderCenterPointRL - (colliderRL.transform.up * colliderRL.suspensionDistance);
        }
        wheelRL.Rotate(colliderRL.rpm * 6 * Time.deltaTime, 0, 0);
        
        if (Physics.Raycast(ColliderCenterPointRR, -colliderRR.transform.up, out hit, colliderRR.suspensionDistance + colliderRR.radius)) {
            wheelRR.transform.position = hit.point + (colliderRR.transform.up * colliderRR.radius);
        }
        else {
            wheelRR.transform.position = ColliderCenterPointRR - (colliderRR.transform.up * colliderRR.suspensionDistance);
        }
        wheelRR.Rotate(colliderRR.rpm * -6 * Time.deltaTime, 0, 0);
        
        wheelFL.transform.rotation = colliderFL.transform.rotation * Quaternion.Euler(RotationValueFL, colliderFL.steerAngle, 0);
        wheelFR.transform.rotation = colliderFR.transform.rotation * Quaternion.Euler(RotationValueFR * -1, colliderFR.steerAngle + 180, 0);
        
        RotationValueFL += colliderFL.rpm * (360 / 60) * Time.deltaTime;
        RotationValueFR += colliderFR.rpm * (360 / 60) * Time.deltaTime;
        
        audio.pitch = Mathf.Abs(((colliderFL.rpm + colliderFR.rpm) / 2) / (MaxEngineSpeed * 4)) + 1.0f;
        
        if (audio.pitch > 2.0f) {
            audio.pitch = 2.0f;
        }
        
    }
    
    void  OnGUI() {
        if (bShowDialog == true) {
            windowRect = GUI.Window(0, new Rect(5, Screen.height - 145, Screen.width - 10, 140), WindowFunction, "Vehicle Editor");
        }
    }
    
    void  WindowFunction(int windowID) {
        sliderWidth = 80;
        margin = 5;
        linefeed = 20;
        txtwidth = 200;

        /*
        GUI.Label(new Rect(margin, linefeed - 5, txtwidth, 30), "Suspension Height: [" + Mathf.Floor(colliderFL.suspensionDistance * 100) / 100 + "]");
        colliderFL.suspensionDistance = GUI.HorizontalSlider(new Rect(margin + txtwidth + 5, linefeed, sliderWidth, 10), colliderFL.suspensionDistance, 0.0f, 1.0f);
        colliderFR.suspensionDistance = colliderFL.suspensionDistance;
        colliderRL.suspensionDistance = colliderFL.suspensionDistance;
        colliderRR.suspensionDistance = colliderFL.suspensionDistance;
        linefeed = linefeed + 20;
        
        GUI.Label(new Rect(margin, linefeed - 5, txtwidth, 30), "Spring Stiffness: [" + Mathf.Floor(colliderFL.suspensionSpring.spring * 100) / 100 + "]");
        colliderFL.suspensionSpring.spring = GUI.HorizontalSlider(new Rect(margin + txtwidth + 5, linefeed, sliderWidth, 10), colliderFL.suspensionSpring.spring, 2000, 8000);
        colliderFR.suspensionSpring.spring = colliderFL.suspensionSpring.spring;
        colliderRL.suspensionSpring.spring = colliderFL.suspensionSpring.spring;
        colliderRR.suspensionSpring.spring = colliderFL.suspensionSpring.spring;
        linefeed = linefeed + 20;
        
        GUI.Label(new Rect(margin, linefeed - 5, txtwidth, 30), "Spring Damper: [" + Mathf.Floor(colliderFL.suspensionSpring.damper * 100) / 100 + "]");
        colliderFL.suspensionSpring.damper = GUI.HorizontalSlider(new Rect(margin + txtwidth + 5, linefeed, sliderWidth, 10), colliderFL.suspensionSpring.damper, 20.0f, 100.0f);
        colliderFR.suspensionSpring.damper = colliderFL.suspensionSpring.damper;
        colliderRL.suspensionSpring.damper = colliderFL.suspensionSpring.damper;
        colliderRR.suspensionSpring.damper = colliderFL.suspensionSpring.damper;
        linefeed = linefeed + 20;
        
        GUI.Label(new Rect(margin, linefeed - 5, txtwidth, 30), "Tyre Grip: [" + Mathf.Floor(colliderFL.forwardFriction.stiffness * 100) / 100 + "]");
        colliderFL.forwardFriction.stiffness = GUI.HorizontalSlider(new Rect(margin + txtwidth + 5, linefeed, sliderWidth, 10), colliderFL.forwardFriction.stiffness, 0.01f, 1.0f);
        colliderFR.forwardFriction.stiffness = colliderFL.forwardFriction.stiffness;
        colliderRL.forwardFriction.stiffness = colliderFL.forwardFriction.stiffness;
        colliderRR.forwardFriction.stiffness = colliderFL.forwardFriction.stiffness;
        linefeed = linefeed + 20;
        
        GUI.Label(new Rect(margin, linefeed - 5, txtwidth, 30), "Tyre Slide/Drift: [" + Mathf.Floor(colliderFL.sidewaysFriction.stiffness * 100) / 100 + "]");
        colliderFL.sidewaysFriction.stiffness = GUI.HorizontalSlider(new Rect(margin + txtwidth + 5, linefeed, sliderWidth, 10), colliderFL.sidewaysFriction.stiffness, 0.1f, 0.01f);
        colliderFR.sidewaysFriction.stiffness = colliderFL.sidewaysFriction.stiffness;
        colliderRL.sidewaysFriction.stiffness = colliderFL.sidewaysFriction.stiffness;
        colliderRR.sidewaysFriction.stiffness = colliderFL.sidewaysFriction.stiffness;
        linefeed = linefeed + 20;
        
        GUI.Label(new Rect(margin, linefeed - 5, txtwidth, 30), "Centre Of Gravity (Stablity): [" + Mathf.Floor(rigidbody.centerOfMass.y * 100) / 100 + "]");
        rigidbody.centerOfMass.y = GUI.HorizontalSlider(new Rect(margin + txtwidth + 5, linefeed, sliderWidth, 10), rigidbody.centerOfMass.y, -1, -0.1f);
        linefeed = linefeed + 20;
        
*/
        margin = 300;
        linefeed = 20;
        txtwidth = 190;
        
        GUI.Label(new Rect(margin, linefeed - 5, txtwidth, 30), "Maximum Steering Angle: [" + Mathf.Floor(MaxSteeringAngle * 100) / 100 + "]");
        MaxSteeringAngle = GUI.HorizontalSlider(new Rect(margin + txtwidth + 5, linefeed, sliderWidth, 10), MaxSteeringAngle, 5, 60);
        linefeed = linefeed + 20;
        
        GUI.Label(new Rect(margin, linefeed - 5, txtwidth, 30), "Maximum Engine Speed: [" + Mathf.Floor(MaxEngineSpeed * 100) / 100 + "]");
        MaxEngineSpeed = GUI.HorizontalSlider(new Rect(margin + txtwidth + 5, linefeed, sliderWidth, 10), MaxEngineSpeed, 30, 250);
        linefeed = linefeed + 20;
        
        GUI.Label(new Rect(margin, linefeed - 5, txtwidth, 30), "Vehicle Mass: [" + Mathf.Floor(rigidbody.mass * 100) / 100 + "]");
        rigidbody.mass = GUI.HorizontalSlider(new Rect(margin + txtwidth + 5, linefeed, sliderWidth, 10), rigidbody.mass, 1000, 2000);
        linefeed = linefeed + 20;
        
        GUI.Label(new Rect(margin, linefeed - 5, txtwidth, 30), "Vehicle Drag: [" + Mathf.Floor(rigidbody.drag * 100) / 100 + "]");
        rigidbody.drag = GUI.HorizontalSlider(new Rect(margin + txtwidth + 5, linefeed, sliderWidth, 10), rigidbody.drag, 0, 2);
        linefeed = linefeed + 20;
        
        if (GUI.Button(new Rect(margin, linefeed, 188, 25), "Save Settings...")) {
            StreamWriter sw = new StreamWriter(Application.dataPath + "\\A-Lab Software RapidUnity Vehicle Resource Pack\\Vehicle Editor Settings\\AlabCar_" + string.Format("{0:dd-MM-yyyy HH.mm.ss}", System.DateTime.Now) + ".ves"); 
            sw.WriteLine("Rig Type=AlabCar"); 
            sw.WriteLine("Suspension Height=" + colliderFL.suspensionDistance); 
            sw.WriteLine("Spring Stiffness=" + colliderFL.suspensionSpring.spring); 
            sw.WriteLine("Spring Damper=" + colliderFL.suspensionSpring.damper); 
            sw.WriteLine("Tyre Grip=" + colliderFL.forwardFriction.stiffness); 
            sw.WriteLine("Tyre Slide/Drift=" + colliderFL.sidewaysFriction.stiffness); 
            sw.WriteLine("Centre Of Gravity (Stablity)=" + rigidbody.centerOfMass.y); 
            sw.WriteLine("Vehicle Mass=" + rigidbody.mass); 
            sw.WriteLine("Vehicle Drag=" + rigidbody.drag); 
            sw.WriteLine("Wheels Collider Radius=" + colliderFL.radius); 
            sw.Close();
        }
    }
    
}