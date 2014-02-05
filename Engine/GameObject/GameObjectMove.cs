using UnityEngine;
using System.Collections;

public class GameObjectMove : MonoBehaviour {

    public float translationSpeedX = 0f;
    public float translationSpeedY = 1f;
    public float translationSpeedZ = 0f;
    public bool local = true;
    
    public void Start() {
    
    }

    public void Update() {
    

        if (local == true) {
            transform.Translate(
            new Vector3(translationSpeedX, translationSpeedY, translationSpeedZ) 
            * Time.deltaTime);
        }

        if (local == false) {
            transform.Translate(
            new Vector3(translationSpeedX, translationSpeedY, translationSpeedZ) 
            * Time.deltaTime, Space.World);
        }
 
    }
}


