using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class ShowRaycasts : GameObjectBehavior {

    public bool show = false;
    private GameVehicleAIDriver aiDriver;

    public void OnDrawGizmos() {
        if (!Application.isPlaying || show) {
            aiDriver = gameObject.GetComponent("GameVehicleAIDriver") as GameVehicleAIDriver;

            if (aiDriver.useObstacleAvoidance) {
                Vector3 viewPointLeft;
                Vector3 viewPointRight;
                Vector3 forwardDirection = aiDriver.viewPoint.TransformDirection(Vector3.forward * aiDriver.oADistance);

                Vector3 centerPointL = transform.position + transform.TransformDirection(Vector3.left * aiDriver.oASideOffset);
                centerPointL.y = aiDriver.viewPoint.position.y;

                Vector3 centerPointR = transform.position + transform.TransformDirection(Vector3.right * aiDriver.oASideOffset);
                centerPointR.y = aiDriver.viewPoint.position.y;

                viewPointLeft = aiDriver.viewPoint.transform.position;
                viewPointRight = aiDriver.viewPoint.transform.position;
                viewPointLeft += aiDriver.viewPoint.TransformDirection((Vector3.right * aiDriver.flWheel.localPosition.x));
                viewPointRight += aiDriver.viewPoint.TransformDirection((Vector3.right * aiDriver.frWheel.localPosition.x));
                float obstacleAvoidanceWidth = aiDriver.oAWidth;               

                Vector3 leftDirection = aiDriver.viewPoint.TransformDirection((Vector3.left * obstacleAvoidanceWidth) + (Vector3.forward * aiDriver.oADistance));
                Vector3 rightDirection = aiDriver.viewPoint.TransformDirection((Vector3.right * obstacleAvoidanceWidth) + (Vector3.forward * aiDriver.oADistance));

                Vector3 leftSide = aiDriver.viewPoint.TransformDirection(Vector3.left * aiDriver.oASideDistance);
                Vector3 rightSide = aiDriver.viewPoint.TransformDirection(Vector3.right * aiDriver.oASideDistance);

                Debug.DrawRay(viewPointLeft, leftDirection, Color.cyan);
                Debug.DrawRay(viewPointRight, rightDirection, Color.cyan);
                Debug.DrawRay(aiDriver.viewPoint.position, forwardDirection, Color.green);
                Debug.DrawRay(viewPointLeft, forwardDirection, Color.green);
                Debug.DrawRay(viewPointRight, forwardDirection, Color.green);
                Debug.DrawRay(centerPointL, leftSide, Color.magenta);
                Debug.DrawRay(centerPointR, rightSide, Color.magenta);
            }            
        }
    }

    void Update() {
        if (!Application.isPlaying) {
            aiDriver = gameObject.GetComponent("GameVehicleAIDriver") as GameVehicleAIDriver;
            Transform colliderBottom = transform.FindChild("Colliders/ColliderBottom");
            aiDriver.oASideOffset = Mathf.Abs(colliderBottom.localPosition.x) + colliderBottom.localScale.x / 2 + 0.1f;
            Vector3 vpPos = aiDriver.viewPoint.localPosition;
            vpPos.z = colliderBottom.localPosition.z + colliderBottom.localScale.z / 2 + 0.1f;
            vpPos.x = 0;
            aiDriver.viewPoint.localPosition = vpPos;

        }
    }
}
