using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SetForces : MonoBehaviour {
           
    public bool relativeForce = true;
    public float x = 0f;
    public float xDeviation = 0f; // deviation means how much randomity it has, for example a value of 3, deviation 1 can be anything from 2 to 4
    
    public float y = 0f;
    public float yDeviation = 0f;
    public float z = 0f;
    public float zDeviation = 0f;
    public bool relativeTorque = true;
    public float torqueScale = 100f; // scale up torque power

    public float xRot = 0f;
    public float xRotDeviation = 0f;
    public float yRot = 0f;
    public float yRotDeviation = 0f;
    public float zRot = 0f;
    public float zRotDeviation = 0f;
    
    public void Start() {

        Init();
    }

    public void Init() {
        if (relativeForce == true)
            rigidbody.AddRelativeForce(
                Random.Range(x - xDeviation, x + xDeviation), 
                Random.Range(y - yDeviation, y + yDeviation), 
                Random.Range(z - zDeviation, z + zDeviation));
        if (relativeForce == false)
            rigidbody.AddForce(
                Random.Range(x - xDeviation, x + xDeviation), 
                Random.Range(y - yDeviation, y + yDeviation), 
                Random.Range(z - zDeviation, z + zDeviation));
        
        if (relativeTorque == true)
            rigidbody.AddRelativeTorque(
                Random.Range(xRot - xRotDeviation, xRot + xRotDeviation) * torqueScale, 
                Random.Range(yRot - yRotDeviation, yRot + yRotDeviation) * torqueScale, 
                Random.Range(zRot - zRotDeviation, zRot + zRotDeviation) * torqueScale);
        if (relativeTorque == false)
            rigidbody.AddTorque(
                Random.Range(xRot - xRotDeviation, xRot + xRotDeviation) * torqueScale, 
                Random.Range(yRot - yRotDeviation, yRot + yRotDeviation) * torqueScale, 
                Random.Range(zRot - zRotDeviation, zRot + zRotDeviation) * torqueScale);       

    }
    
    public void FixedUpdate() {
        
    }
}


