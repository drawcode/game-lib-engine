using UnityEngine;
using System.Collections;

//using EnemyAI;

[ExecuteInEditMode]
public class ViewPointBehaviour : GameObjectBehavior {
    GameVehicleAIDriver aiDriver;
    GameVehicleAIDriverController aiDriverController;

    public void OnDrawGizmos() {
        if (!Application.isPlaying) {
            GameVehicleAIDriver aiDriver = gameObject.transform.parent.GetComponent<GameVehicleAIDriver>() as GameVehicleAIDriver;
            if (aiDriver != null) {
                if (aiDriver.useObstacleAvoidance) {
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireSphere(gameObject.transform.position, 0.1f);
                }
            }
            else {
                GameVehicleAIDriverController aiDriverController = 
                    gameObject.transform.parent.GetComponent<GameVehicleAIDriverController>() as GameVehicleAIDriverController;
                if (aiDriverController != null) {                    
                    if (aiDriverController.useObstacleAvoidance) {
                        Gizmos.color = Color.cyan;
                        Gizmos.DrawWireSphere(gameObject.transform.position, 0.1f);
                    }
                }
            
            }
        }
    }
}
