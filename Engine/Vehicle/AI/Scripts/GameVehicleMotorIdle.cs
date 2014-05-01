using UnityEngine;
using System.Collections;
 
public class GameVehicleMotorIdle : GameObjectBehavior {

    public void FixedUpdate () {
    	IdleSound();
    }

    public void IdleSound(){
    	float currentPitch = 0.00f;
    		
    	currentPitch = Input.GetAxis("Vertical") + 0.8f;
    	audio.pitch = currentPitch;
    }
}