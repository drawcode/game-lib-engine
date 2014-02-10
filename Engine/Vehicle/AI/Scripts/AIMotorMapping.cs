using UnityEngine;
using System.Collections;

public class AIMotorMapping : MonoBehaviour {
    [HideInInspector]
    public float
        steerInput;
    [HideInInspector]
    public float
        motorInput;
    [HideInInspector]
    public float
        brakeInput;
    [HideInInspector]
    public float
        handbrakeInput;
    [HideInInspector]
    public float
        steerMax;
    [HideInInspector]
    public float
        speedMax;
    public Transform flWheelMesh;
    public Transform frWheelMesh;
    public bool usingAIDriverMotor = true;
    private AIDriverMotor aIDriverMotor;
    
    ////Edy's -B
    //private CarControl carControl;
    ////Edy's -E
    
    ////Unity Car Tutorial -B
    //private Car car;
    ////Unity Car Tutorial -E
    
    void Awake() {
        if (usingAIDriverMotor) {
            aIDriverMotor = this.GetComponent<AIDriverMotor>();
            steerMax = aIDriverMotor.maxSteerAngle;
            speedMax = aIDriverMotor.maxSpeed;
        }
        else {
            ////Edy's -B
            //carControl = this.GetComponent<CarControl>();
            //steerMax = carControl.steerMax;
            //speedMax = carControl.maxSpeed;
            ////Edy's -E

            ////Unity Car Tutorial -B
            //car = this.GetComponent<Car>();
            //steerMax = car.maximumTurn;
            //speedMax = car.topSpeed;                  
            ////Unity Car Tutorial -B
        }
    }
        
    // Update is called once per frame
    void Update() {

        if (usingAIDriverMotor) {            
            aIDriverMotor.aiSteerAngle = steerInput;
            aIDriverMotor.aiSpeedPedal = motorInput;
            aIDriverMotor.aiBrakePedal = brakeInput;
        }
        else {
            ////Edy's -B
            //carControl.steerInput = steerInput;
            //carControl.brakeInput = brakeInput;            
            //if (motorInput > 0)
            //{
            //    carControl.motorInput = motorInput;
            //    carControl.gearInput = 1;
            //}
            //else
            //{
            //    carControl.motorInput = (-1) * motorInput;
            //    carControl.gearInput = -1;
            //}
            ////Edy's -E
            
            ////Unity Car Tutorial -B           
            //car.steer = steerInput;     
            //if (brakeInput > 0)
            //{
            //  car.throttle = (-1) * brakeInput; 
            //}
            //else
            //{
            //  car.throttle = motorInput;  
            //}
            ////Unity Car Tutorial -E
        }
    }
}
