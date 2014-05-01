using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SetWheelColliderSettings : GameObjectBehavior {
    public WheelCollider flWheelCollider;
    public WheelCollider frWheelCollider;
    public WheelCollider rlWheelCollider;
    public WheelCollider rrWheelCollider;
    public Transform flWheel;
    public Transform frWheel;
    public Transform rlWheel;
    public Transform rrWheel;
    public float radiusFront = 0.3f;
    public float radiusBack = 0.3f;
    public bool mirrorWheels = true;
    private Vector3 frontLeftPosition;
    private Vector3 rearLeftPosition;
  
    void Awake() {
        flWheelCollider.transform.position = flWheel.position;
        rlWheelCollider.transform.position = rlWheel.position;
        frWheelCollider.transform.position = frWheel.position;
        rrWheelCollider.transform.position = rrWheel.position;

        flWheelCollider.radius = radiusFront;
        frWheelCollider.radius = radiusFront;
        rlWheelCollider.radius = radiusBack;
        rrWheelCollider.radius = radiusBack;
    }
   

    // Update is called once per frame
    void Update() {                
        if (!Application.isPlaying) {
            if (mirrorWheels) {

                Vector3 fllp = frWheel.localPosition;
                fllp.x = fllp.x * (-1);
                flWheel.localPosition = fllp;

                Vector3 rllp = rrWheel.localPosition;
                rllp.x = rllp.x * (-1);
                rlWheel.localPosition = rllp;
            }
            flWheelCollider.transform.position = flWheel.position;
            rlWheelCollider.transform.position = rlWheel.position;
            frWheelCollider.transform.position = frWheel.position;
            rrWheelCollider.transform.position = rrWheel.position;

            flWheelCollider.radius = radiusFront;
            frWheelCollider.radius = radiusFront;
            rlWheelCollider.radius = radiusBack;
            rrWheelCollider.radius = radiusBack;
        }
    }

    //public void OnDrawGizmosSelected()
    public void OnDrawGizmos() {
        if (!Application.isPlaying) {
            float frontLength = radiusFront * 2;
            float rearLength = radiusBack * 2;
            Vector3 cubeFront = new Vector3(frontLength, frontLength, frontLength);
            Vector3 cubeRear = new Vector3(rearLength, rearLength, rearLength);
            Gizmos.color = Color.white;

            Gizmos.DrawWireCube(frWheel.position, cubeFront);
            Gizmos.DrawWireCube(rrWheel.position, cubeRear);
            if (mirrorWheels) {
                Gizmos.color = Color.red;
            }
            Gizmos.DrawWireCube(flWheel.position, cubeFront);
            Gizmos.DrawWireCube(rlWheel.position, cubeRear);
        }
    }
}
