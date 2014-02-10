using UnityEngine;
using System.Collections;

//using EnemyAI;

[ExecuteInEditMode]
public class ViewPointBehaviour : MonoBehaviour {
    AIDriver aiDriver;
    AIDriverController aiDriverController;

    public void OnDrawGizmos() {
        if (!Application.isPlaying) {
            AIDriver aiDriver = gameObject.transform.parent.GetComponent<AIDriver>() as AIDriver;
            if (aiDriver != null) {
                if (aiDriver.useObstacleAvoidance) {
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireSphere(gameObject.transform.position, 0.1f);
                }
            }
            else {
                AIDriverController aiDriverController = 
                    gameObject.transform.parent.GetComponent<AIDriverController>() as AIDriverController;
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
