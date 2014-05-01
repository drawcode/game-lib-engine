using UnityEngine;
using System.Collections;

//using EnemyAI;

public class EventListenerExample : GameObjectBehavior {
   
    public GameObject respawnEffect;
    private string message;
    
    void OnEnable() {
        GameVehicleAIDriverController.onLastWaypoint += onLastWaypoint;        
        GameVehicleRespawnController.onRespawnWaypoint += onRespawnWaypoint;
    }

    void OnDisable() {       
        GameVehicleAIDriverController.onLastWaypoint -= onLastWaypoint;        
        GameVehicleRespawnController.onRespawnWaypoint -= onRespawnWaypoint;
    }

    void onLastWaypoint(GameVehicleEventArgs e) {
        //Example:
        //message = e.name + " reached last Waypoint '" + e.currentWaypointName + "' (" + e.currentWaypointIndex +").";     
        //StartCoroutine(ShowMessage(message,1));
    }

    void onRespawnWaypoint(GameVehicleEventArgs e) {
        Instantiate(respawnEffect, e.position, e.rotation);     
        //Example:
        //message = e.name + " was respawned at '" + e.position.x + ";" + e.position.x + ";" + e.position.x +"'.";
        //StartCoroutine(ShowMessage(message, 1));
    }

    IEnumerator ShowMessage(string text, float seconds) {
        message = text;       
        yield return new WaitForSeconds(seconds);
        message = "";
    }

    void OnGUI() {
        GUILayout.Space(20);
        
        GUILayout.Label(message);
    }
}
