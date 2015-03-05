using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameVehicleRespawnController : GameObjectBehavior {
    
    public float heightOffset = 0;
    private Transform currentRespawnPoint;
    [HideInInspector]
    public int
        currentRespawnPointInt;
    //public List<WheelCollider> myWcs;
    private bool isStartingRespawn = false;
    private GameVehicleAIDriverController aiDriverControllerScript;
    private List<Transform> waypoints;
    //private Vector3 lastPosition;
    public float timeTillRespawn = 5;
    [HideInInspector]
    public float
        lastTimeToReachNextWP;

    //Event 2
    public delegate void RespawnHandler(GameVehicleEventArgs e);

    public static RespawnHandler onRespawnWaypoint;

    void Awake() {        
        
        
    }
    
    void Start() {
        //wir machen dies in der Start-Routine, weil wir im GameVehicleAIDriver-Skript erst in der Awake-Fkt die Waypoints zuweisen
        //Um sicherzugehen, dass die Waypoints auch gefunden und der List zugewiesen wurde, weisen wir dies deshalb erst hier zu!

        aiDriverControllerScript = gameObject.GetComponent("GameVehicleAIDriverController") as GameVehicleAIDriverController;
        waypoints = aiDriverControllerScript.waypoints;
    }

    public void StartRespawn() {
        if (!isStartingRespawn) {
            isStartingRespawn = true;           
            Respawn();
        }
    }

    void Update() {       
        if (!IsCorrectMoving()) {
            StartRespawn();
        }
    }
                
    void Respawn() { 
        Vector3 startPos;
        
        StartCoroutine(Freeze(1));
        
        int currentWaypoint = aiDriverControllerScript.currentWaypoint;
        if (currentWaypoint == 0) {
            currentWaypoint = waypoints.Count - 1;
        }
        else {
            currentWaypoint -= 1;
        }
        currentRespawnPoint = waypoints[currentWaypoint];
        startPos = currentRespawnPoint.position;
        startPos.y += heightOffset; 
        transform.position = startPos;
        transform.rotation = currentRespawnPoint.rotation;
        
        aiDriverControllerScript.aiSteerAngle = 0;
        aiDriverControllerScript.currentAngle = 0;        
        
        isStartingRespawn = false;
        lastTimeToReachNextWP = 0;
        
        //fire event BEGIN
        if (onRespawnWaypoint != null) {            
            GameVehicleEventArgs e = new GameVehicleEventArgs();
            e.name = gameObject.name;
            e.currentWaypointIndex = currentWaypoint;
            e.currentWaypointName = waypoints[currentWaypoint].name;
            e.position = gameObject.transform.position;         
            e.rotation = gameObject.transform.rotation;
            e.tag = gameObject.tag;
            onRespawnWaypoint(e);
        }
        //fire event END
    }

    bool IsCorrectMoving() {
        bool moving = true;
        lastTimeToReachNextWP += Time.deltaTime;
       
        if (lastTimeToReachNextWP >= timeTillRespawn) {
           
            moving = false;          

        }
        //lastPosition = transform.position;
        return moving;
    }
    
    IEnumerator Freeze(float seconds) {
        
        rigidbody.freezeRotation = true;
        rigidbody.velocity = Vector3.zero;
        yield return new WaitForSeconds(seconds);
        rigidbody.freezeRotation = false;
        
    }
    
   
}
