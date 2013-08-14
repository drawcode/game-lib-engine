using UnityEngine;
using System.Collections;

namespace Engine.Events.Inputs {

/// <summary>
/// Adaptation of the standard MouseOrbit script to use the finger drag gesture to rotate the current object using
/// the fingers/mouse around a target object
/// NOTE: TBInputManager NOT required
/// </summary>
[AddComponentMenu( "FingerGestures/Toolbox/Misc/DragOrbit" )]
public class TBDragOrbit : MonoBehaviour
{
    public Transform target;
    public float distance = 10.0f;

    public float xSpeed = 250.0f;
    public float ySpeed = 120.0f;

    public float yMinLimit = -20;
    public float yMaxLimit = 80;

    float x = 0;
    float y = 0;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        // Make the rigid body not change rotation
        if( rigidbody )
            rigidbody.freezeRotation = true;

        Apply();
    }

    void OnEnable()
    {
        FingerGestures.OnDragMove += FingerGestures_OnDragMove;
    }

    void OnDisable()
    {
        FingerGestures.OnDragMove -= FingerGestures_OnDragMove;
    }

    void FingerGestures_OnDragMove( int fingerIndex, Vector2 fingerPos, Vector2 delta )
    {
        if( target )
        {
            x += delta.x * xSpeed * 0.02f;
            y -= delta.y * ySpeed * 0.02f;

            y = ClampAngle( y, yMinLimit, yMaxLimit );

            Apply();
        }
    }

    void Apply()
    {
        var rotation = Quaternion.Euler( y, x, 0 );
        var position = rotation * new Vector3( 0, 0, -distance ) + target.position;

        transform.rotation = rotation;
        transform.position = position;
    }

    static float ClampAngle( float angle, float min, float max )
    {
        if( angle < -360 )
            angle += 360;
        
        if( angle > 360 )
            angle -= 360;

        return Mathf.Clamp( angle, min, max );
    }
}
}

