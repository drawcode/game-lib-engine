using UnityEngine;
using System.Collections;

// This class simulates an anti-roll bar.
// Anti roll bars transfer suspension compressions forces from one wheel to another.
// This is used to minimize body roll in corners, and improve grip by balancing the wheel loads.
// Typical modern cars have one anti-roll bar per axle.
public class GameVehicleAntiRollBar : GameObjectBehavior {

    // The two wheels connected by the anti-roll bar. These should be on the same axle.
    public WheelCollider wheelL;
    public WheelCollider wheelR;
    
    // Coeefficient determining how much force is transfered by the bar.
    public float antiRoll = 5000.0F;
    WheelHit hit;
    float travelL = 1.0F;
    float travelR = 1.0F;
    
    void FixedUpdate() {
        
        if(GameConfigs.isUIRunning) {
            return;
        }
        
        if(!GameConfigs.isGameRunning) {
            return;
        }
        /*
        float force = (wheelL.compression - wheelR.compression) * coefficient;
        wheelL.suspensionForceInput =+ force;
        wheelR.suspensionForceInput =- force;
        */
        
        bool groundedL = wheelL.GetGroundHit(out hit);  
        
        if (groundedL) 
            travelL = (-wheelL.transform.InverseTransformPoint(hit.point).y - wheelL.radius) 
                / wheelL.suspensionDistance;

        bool groundedR = wheelR.GetGroundHit(out hit); 
        if (groundedR) 
            travelR = (-wheelR.transform.InverseTransformPoint(hit.point).y - wheelR.radius) 
                / wheelR.suspensionDistance;

        float antiRollForce = (travelL - travelR) * antiRoll;

        if (groundedL) 
            rigidbody.AddForceAtPosition(wheelL.transform.up * -antiRollForce, wheelL.transform.position); 
        if (groundedR) 
            rigidbody.AddForceAtPosition(wheelR.transform.up * antiRollForce, wheelR.transform.position); 
                 
    
    }
}
