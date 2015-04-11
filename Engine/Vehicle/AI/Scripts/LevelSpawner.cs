using UnityEngine;
using System.Collections;

public class LevelSpawner : GameObjectBehavior {
    
    public Transform spawnPointRoad;
    public Transform spawnPointHill;
    public Transform playerCar;
    public Transform hillyAICar;
    //private GameVehicleSmoothFollow carSmoothFollow;

    void Awake() {
        //carSmoothFollow = GetComponent<GameVehicleSmoothFollow>();
    }
    // Use this for initialization
    void Start() {
    
    }
    
    // Update is called once per frame
    void Update() {
    
    }
    
    //void OnGUI() {
    //    if (GUILayout.Button("Respawn Road")) {/
    //
    //       Rigidbody rb = playerCar.GetComponent<Rigidbody>();
    //
    //        rb.velocity = Vector3.zero;
    //        rb.freezeRotation = true;
    //        playerCar.position = spawnPointRoad.position;
    //        playerCar.rotation = spawnPointRoad.rotation;
    //        StartCoroutine("ResetRotation");
    //        carSmoothFollow.target = playerCar;
    //    }
        /*
        if (GUILayout.Button("Respawn Hill"))
        {
            playerCar.rigidbody.velocity = Vector3.zero;
            playerCar.rigidbody.freezeRotation = true;
            playerCar.rigidbody.rotation = Quaternion.identity;
            playerCar.position = spawnPointHill.position;
            playerCar.rotation = spawnPointHill.rotation;
            StartCoroutine("ResetRotation");
        }
        */
    //    if (GUILayout.Button("Watch Hilly AI")) {
            
     //       playerCar.rigidbody.velocity = Vector3.zero;
     //       playerCar.rigidbody.freezeRotation = true;
            /*
            playerCar.rigidbody.rotation = Quaternion.identity;
            playerCar.position = spawnPointHill.position;
            playerCar.rotation = spawnPointHill.rotation;
            */
     //       carSmoothFollow.target = hillyAICar;
     //       //StartCoroutine("ResetRotation");
     //   }
        
    //}
    
    IEnumerator ResetRotation() {
        yield return new WaitForSeconds(0.5f);
        //playerCar.rigidbody.freezeRotation = false;
    }
}
