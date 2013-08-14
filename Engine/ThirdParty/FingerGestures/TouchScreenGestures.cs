using UnityEngine;
using System.Collections;

namespace Engine.Events.Inputs {
/// <summary>
/// This tracks input gestures for a touch-screen device (mobiles)
/// </summary>
public class TouchScreenGestures : FingerGestures
{
	/// <summary>
    /// Maximum number of simultaneous fingers to track
	/// </summary>
	public int maxFingers = 5;

    /// <summary>
    /// Pinch DOT product treshold - this controls how tolerant the pinch gesture detector is to the two fingers
    /// moving in opposite directions.
    /// Setting this to -1 means the fingers have to move in exactly opposite directions to each other.
    /// this value should be kept between -1 and 0 excluded.
    /// </summary>
    public float pinchDOT = -0.7f;

    /// <summary>
    /// Minimum pinch distance required to trigger the pinch gesture
    /// </summary>
    public float minPinchDistance = 0.5f;

    /// <summary>
    /// Rotation DOT product treshold - this controls how tolerant the twist gesture detector is to the two fingers
    /// moving in opposite directions.
    /// Setting this to -1 means the fingers have to move in exactly opposite directions to each other.
    /// this value should be kept between -1 and 0 excluded.
    /// </summary>
    public float rotationDOT = -0.7f;

    /// <summary>
    /// Minimum amount of rotation required to trigger the rotation gesture (in degrees)
    /// </summary>
    public float minRotationAmount = 0.5f;

	public override int MaxFingers
	{
		get { return maxFingers; }
	}

	protected override void Start()
	{
        finger2touchMap = new int[MaxFingers];

        base.Start();
	}

    #region Touch > Finger mapping

    Touch nullTouch = new Touch();

    int[] finger2touchMap;  // finger.index -> touch index map

    void UpdateFingerTouchMap()
    {
        for( int i = 0; i < finger2touchMap.Length; ++i )
            finger2touchMap[i] = -1;

        for( int i = 0; i < Input.touchCount; ++i )
        {
            int fingerIndex = Input.touches[i].fingerId;

            if( fingerIndex < finger2touchMap.Length )
                finger2touchMap[fingerIndex] = i;
        }
    }

    bool HasValidTouch( Finger finger )
    {
        return finger2touchMap[finger.index] != -1;
    }

    Touch GetTouch( Finger finger )
    {
        int touchIndex = finger2touchMap[finger.index];

        if( touchIndex == -1 )
            return nullTouch;

        return Input.touches[touchIndex];
    }

    #endregion

    protected override void Update()
    {
        UpdateFingerTouchMap();

        base.Update();

        if( trackPinch || trackRotation )
            TrackTwoFingerGestures();
    }


    bool pinching = false;
    bool rotating = false;
    float totalRotation = 0;

    void TrackTwoFingerGestures()
    {
        Finger finger1 = GetFinger( 0 );
        Finger finger2 = GetFinger( 1 );

        if( Input.touchCount == 2 )
        {
            // both fingers must have moved past the tolerance threshold
            if( !finger1.moved || !finger2.moved )
                return;

            FingerPhase finger1phase = GetPhase( finger1 );
            FingerPhase finger2phase = GetPhase( finger2 );

            // both fingers must move at same time
            if( finger1phase != FingerPhase.Moved || finger2phase != FingerPhase.Moved )
                return;
            
            Touch touch1 = GetTouch( finger1 );
            Touch touch2 = GetTouch( finger2 );

            Vector2 prevPos1 = touch1.position - touch1.deltaPosition;
            Vector2 prevPos2 = touch2.position - touch2.deltaPosition;
            
            Vector2 prevDelta = prevPos1 - prevPos2;
            Vector2 curDelta = touch1.position - touch2.position;

            float lenDelta = curDelta.magnitude - prevDelta.magnitude;

            if( Mathf.Abs( lenDelta ) < 0.001f )
                return;

            // fingers must move in opposite directions to qualify as a valid pinch move
            float dot = Vector2.Dot( touch1.deltaPosition.normalized, touch2.deltaPosition.normalized );

            // Pinch gesture detection
            if( trackPinch && dot < pinchDOT )
            {
                if( !pinching )
                {
                    // check if we've pinched past the treshold distance by comparing the distance 
                    // between the initial finger positions and the distance between the current ones
                    Vector2 startDelta = finger1.startPos - finger2.startPos;
                    float startLenDelta = curDelta.magnitude - startDelta.magnitude;

                    if( Mathf.Abs( startLenDelta ) >= minPinchDistance )
                    {
                        pinching = true;
                        RaiseOnPinchBegin( finger1.startPos, finger2.startPos );
                        RaiseOnPinchMove( touch1.position, touch2.position, startLenDelta - Mathf.Sign( startLenDelta ) * minPinchDistance );
                    }
                }
                else
                {
                    RaiseOnPinchMove( touch1.position, touch2.position, lenDelta );
                }
            }

            // Rotation gesture detection
            if( trackRotation && dot < rotationDOT )
            {
                Vector2 curDir = curDelta.normalized;

                if( !rotating )
                {
                    Vector2 startDelta = finger1.startPos - finger2.startPos;
                    Vector2 startDir = startDelta.normalized;

                    // check if we went past the minimum rotation amount treshold
                    float angle = Mathf.Rad2Deg * SignedAngle( startDir, curDir );

                    if( Mathf.Abs( angle ) >= minRotationAmount )
                    {
                        rotating = true;
                        RaiseOnRotationBegin( finger1.startPos, finger2.startPos );
                        RaiseOnRotationMove( touch1.position, touch2.position, angle - Mathf.Sign( angle ) * minRotationAmount );
                        totalRotation = minRotationAmount;
                    }
                }
                else
                {
                    Vector2 prevDir = prevDelta.normalized;
                    
                    // signed angle in degrees between previous line and new line
                    float angle = Mathf.Rad2Deg * SignedAngle( prevDir, curDir );
                    
                    RaiseOnRotationMove( touch1.position, touch2.position, angle );
                    totalRotation += angle;
                }
            }
        }
        else
        {
            if( pinching )
            {
                RaiseOnPinchEnd( finger1.lastPos, finger2.lastPos );
                pinching = false;
            }

            if( rotating )
            {
                RaiseOnRotationEnd( finger1.lastPos, finger2.lastPos, totalRotation );
                rotating = false;
            }
        }
    }

	protected override FingerGestures.FingerPhase GetPhase( FingerGestures.Finger finger )
	{
        if( HasValidTouch( finger ) )
		{
            Touch touch = GetTouch( finger );

			switch( touch.phase )
			{
				case TouchPhase.Began:
					return FingerPhase.Began;

				case TouchPhase.Moved:
					return FingerPhase.Moved;

				case TouchPhase.Stationary:
					return FingerPhase.Stationary;

				default:
					return FingerPhase.Ended;
			}
		}

		return FingerPhase.None;
	}

	protected override Vector2 GetPosition( FingerGestures.Finger finger )
	{
        Touch touch = GetTouch( finger );
        return touch.position;
	}

    // returns signed angle in radians
    public static float SignedAngle( Vector2 from, Vector2 to )
    {
        // perpendicular dot product
        float perpDot = ( from.x * to.y ) - ( from.y * to.x );
        return Mathf.Atan2( perpDot, Vector2.Dot( from, to ) );
    }
}
}
