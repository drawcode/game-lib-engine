using UnityEngine;
using System.Collections;

namespace Engine.Events.Inputs {
/// <summary>
/// This tracks input gestures for a mouse device
/// </summary>
public class MouseGestures : FingerGestures
{
	// Number of mouse buttons to track
	public int maxMouseButtons = 3;

	// Axis to use in order to simulate pinch gesture
	public string pinchGestureAxis = "Mouse ScrollWheel";

	protected override void Start()
	{
		base.Start();

		if( trackPinch )
			StartCoroutine( TrackAxisForPinchGesture() );
	}

	public override int MaxFingers
	{
		get { return maxMouseButtons; }
	}
	
	protected override FingerGestures.FingerPhase GetPhase( FingerGestures.Finger finger )
	{
		int button = finger.index;

		// mouse button down?
		if( Input.GetMouseButton( button ) )
		{
			// did we just press it?
			if( Input.GetMouseButtonDown( button ) )
				return FingerPhase.Began;

			// find out if the mouse has moved since last update
			Vector3 delta = GetPosition( finger ) - finger.lastPos;

			if( delta.sqrMagnitude < 1.0f )
				return FingerPhase.Stationary;

			return FingerPhase.Moved;
		}
		
		// did we just release the button?
		if( Input.GetMouseButtonUp( button ) )
			return FingerPhase.Ended;

		return FingerPhase.None;
	}

	protected override Vector2 GetPosition( FingerGestures.Finger finger )
	{
		return Input.mousePosition;	
	}

	const float PinchDeltaEpsilon = 0.0001f;

	IEnumerator TrackAxisForPinchGesture()
	{
		float lastDelta = 0;

		while( true )
		{
			float delta = Input.GetAxis( pinchGestureAxis );

			if( Mathf.Abs( delta ) >= PinchDeltaEpsilon )
			{
				if( Mathf.Abs( lastDelta ) < PinchDeltaEpsilon )
					RaiseOnPinchBegin( Input.mousePosition, Input.mousePosition );

				RaiseOnPinchMove( Input.mousePosition, Input.mousePosition, delta );
			}
			else
			{
				if( Mathf.Abs( lastDelta ) >= PinchDeltaEpsilon )
					RaiseOnPinchEnd( Input.mousePosition, Input.mousePosition );
			}

			lastDelta = delta;
			yield return null;
		}
	}

	
}
}
