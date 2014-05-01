using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class ShowControllerRaycasts : GameObjectBehavior {

    public bool show = false;
    private GameVehicleAIDriverController aiDriverController;
    private GameVehicleMotorMapping aIMotorMapping;

    public void OnDrawGizmos() {
        if (!Application.isPlaying || show) {
            aiDriverController = gameObject.GetComponent("GameVehicleAIDriverController") as GameVehicleAIDriverController;
            aIMotorMapping = gameObject.GetComponent("GameVehicleMotorMapping") as GameVehicleMotorMapping;
            if (aiDriverController.useObstacleAvoidance && aIMotorMapping.flWheelMesh != null) {
                Vector3 viewPointLeft;
                Vector3 viewPointRight;
                Vector3 forwardDirection = aiDriverController.viewPoint.TransformDirection(Vector3.forward * aiDriverController.oADistance);

                //Vector3 centerPointL = transform.position + transform.TransformDirection(Vector3.left * aiDriverController.oASideOffset);
                //centerPointL.y = aiDriverController.viewPoint.position.y;

                //Vector3 centerPointR = transform.position + transform.TransformDirection(Vector3.right * aiDriverController.oASideOffset);
                //centerPointR.y = aiDriverController.viewPoint.position.y;

                viewPointLeft = aiDriverController.viewPoint.transform.position;
                viewPointRight = aiDriverController.viewPoint.transform.position;
                viewPointLeft += aiDriverController.viewPoint.TransformDirection((Vector3.right * aIMotorMapping.flWheelMesh.localPosition.x));
                viewPointRight += aiDriverController.viewPoint.TransformDirection((Vector3.right * aIMotorMapping.frWheelMesh.localPosition.x));
                float obstacleAvoidanceWidth = aiDriverController.oAWidth;               

                Vector3 leftDirection = aiDriverController.viewPoint.TransformDirection((Vector3.left * obstacleAvoidanceWidth) + (Vector3.forward * aiDriverController.oADistance));
                Vector3 rightDirection = aiDriverController.viewPoint.TransformDirection((Vector3.right * obstacleAvoidanceWidth) + (Vector3.forward * aiDriverController.oADistance));

                Vector3 leftSide = aiDriverController.viewPoint.TransformDirection(Vector3.left * aiDriverController.oASideDistance);
                Vector3 rightSide = aiDriverController.viewPoint.TransformDirection(Vector3.right * aiDriverController.oASideDistance);

                Debug.DrawRay(viewPointLeft, leftDirection, Color.cyan);
                Debug.DrawRay(viewPointRight, rightDirection, Color.cyan);
                Debug.DrawRay(aiDriverController.viewPoint.position, forwardDirection, Color.green);
                Debug.DrawRay(viewPointLeft, forwardDirection, Color.green);
                Debug.DrawRay(viewPointRight, forwardDirection, Color.green);
                //Debug.DrawRay(centerPointL, leftSide, Color.magenta);
                //Debug.DrawRay(centerPointR, rightSide, Color.magenta);


                //----------------------------------------
                //leftFront
                Vector3 leftFrontSidePos = transform.position + transform.TransformDirection(Vector3.left * aiDriverController.oASideOffset);
                leftFrontSidePos.y = aiDriverController.viewPoint.position.y;
                leftFrontSidePos += transform.TransformDirection(Vector3.forward * aiDriverController.oASideFromMid);                
                Debug.DrawRay(leftFrontSidePos, leftSide, Color.magenta);

                //leftRear
                Vector3 leftRearSidePos = transform.position + transform.TransformDirection(Vector3.left * aiDriverController.oASideOffset);
                leftRearSidePos.y = aiDriverController.viewPoint.position.y;
                leftRearSidePos -= transform.TransformDirection(Vector3.forward * aiDriverController.oASideFromMid);
                Debug.DrawRay(leftRearSidePos, leftSide, Color.magenta);

                //rightFront
                Vector3 rightFrontSidePos = transform.position + transform.TransformDirection(Vector3.right * aiDriverController.oASideOffset);
                rightFrontSidePos.y = aiDriverController.viewPoint.position.y;
                rightFrontSidePos += transform.TransformDirection(Vector3.forward * aiDriverController.oASideFromMid);
                Debug.DrawRay(rightFrontSidePos, rightSide, Color.magenta);

                //rightRear
                Vector3 rightRearSidePos = transform.position + transform.TransformDirection(Vector3.right * aiDriverController.oASideOffset);
                rightRearSidePos.y = aiDriverController.viewPoint.position.y;
                rightRearSidePos -= transform.TransformDirection(Vector3.forward * aiDriverController.oASideFromMid);
                Debug.DrawRay(rightRearSidePos, rightSide, Color.magenta);
            }            
        }
    }

    void Update() {
        if (!Application.isPlaying) {
            aiDriverController = gameObject.GetComponent("GameVehicleAIDriverController") as GameVehicleAIDriverController;
            Transform colliderBottom = transform.FindChild("Colliders/ColliderBottom");
            if (colliderBottom != null) {
                aiDriverController.oASideOffset = Mathf.Abs(colliderBottom.localPosition.x) + colliderBottom.localScale.x / 2 + 0.1f;
                Vector3 vpPos = aiDriverController.viewPoint.localPosition;
                vpPos.z = colliderBottom.localPosition.z + colliderBottom.localScale.z / 2 + 0.1f;
                vpPos.x = 0;
                aiDriverController.viewPoint.localPosition = vpPos;
            }

        }
    }
}
