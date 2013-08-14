// FingerGestures v1.4 (august 26 2011)
// The FingerGestures library is copyright (c) of William Ravaine
// Please send feedback or bug reports to spk@fatalfrog.com
// More FingerGestures information at http://www.fatalfrog.com/?page_id=140
// Visit my website @ http://www.fatalfrog.com

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Engine.Events.Inputs {
	/// <summary>
	/// Main interface to the FingerGestures API
	/// </summary>
	/// <remarks>
	/// FingerGesture is a script package that lets you easily detect and react to common input gestures performed with either a mouse or a touch screen device.
	/// 
	/// <para>
	/// The library currently detects the following common input events and gestures:
	/// - Finger Down (finger has just been pressed down)
	/// - Finger Stationary (finger is held down without moving)
    /// - Finger Move (finger is moving)
	/// - Finger Up (finger has just been released)
    /// - Long-Press (fires when finger stays held down without moving for a specific amount of time)
	/// - Tap (single or multiple consecutive press and release gestures at the same location)
	/// - Drag and Drop (press > move > release sequence)
	/// - Swipe (quick drag/drop motion in a specific direction)
    /// - Pinch in/out (two-fingers moving closer or further away from each other)
    /// - Rotate (two-fingers moving in circular motion)
	/// </para>
	/// 
	/// <para>
	/// Using the system is very straightforward. All you need to do is subscribe to the gesture events you are interested in and implement your own
	/// logic using the parameters provided by the event. The CSharpSample.cs file demonstrates in detail how to do this. You will also find a javascript skeleton code 
	/// in the JavascriptSkeletonSample.js that you can copy/paste directly in your own project and fill in the blanks.
	///</para>
	///</remarks>
	public abstract class FingerGestures : MonoBehaviour
	{
		// In the following code, the term finger is used as an abstraction for 
		// a user input: it can be a mouse button press or a real finger touching the screen. 

		/// <summary>
		/// Supported swipe gesture directions
		/// </summary>
		public enum SwipeDirection
		{
			/// <summary>
			/// Moved to the right
			/// </summary>
			Right,

			/// <summary>
			/// Moved to the left
			/// </summary>
			Left,

			/// <summary>
			/// Moved up
			/// </summary>
			Up,

			/// <summary>
			/// Moved down
			/// </summary>
			Down,
		}

		/// <summary>
		/// Access to the FingerGestures singleton instance
		/// </summary>
		public static FingerGestures Instance
		{
			get { return FingerGestures.instance; }
		}

		/// <summary>
		/// Maximum number of simultaneous fingers supported
		/// </summary>
		public abstract int MaxFingers { get; }

		#region Events

		// If you are not familiar with events and delegates, or need a refresher, please refer to this youtube video made by the guys 
        // over at Prime31 Studios - http://www.youtube.com/watch?v=N2zdwKIsXJs

        #region Finger events
        
        /// <summary>
		/// Event fired on the first frame the finger is down
		/// </summary>
		public static event FingerDownEventHandler OnFingerDown;

        /// <summary>
        /// The finger is starting its stationary state
        /// </summary>
		public static event FingerStationaryBeginEventHandler OnFingerStationaryBegin;

		/// <summary>
		/// The finger is held down and hasn't moved further away than moveTreshold units from its initial rest position. 
        /// This is fired once each frame until the finger is moved or released. 
		/// </summary>
		public static event FingerStationaryEventHandler OnFingerStationary;

        /// <summary>
        /// The finger just stopped being stationary
        /// </summary>
        public static event FingerStationaryEndEventHandler OnFingerStationaryEnd;

        /// <summary>
        /// Event fired when the finger starts moving beyond the moveTreshold distance
        /// </summary>
        public static event FingerMoveEventHandler OnFingerMoveBegin;

        /// <summary>
        /// Event fired when the finger is moving around
        /// </summary>
        public static event FingerMoveEventHandler OnFingerMove;

        /// <summary>
        /// Event fired to signal that the finger is no longer moving (either went stationary or was lifted up)
        /// </summary>
        public static event FingerMoveEventHandler OnFingerMoveEnd;

		/// <summary>
		/// Event fired when a finger, previously down, has just been released. e.g. this would be the equivalent of a "mouse up" button event.
		/// </summary>
		public static event FingerUpEventHandler OnFingerUp;

        #endregion

        #region Gestures

        /// <summary>
        /// Event fired when a finger is held down for longPressDuration seconds without moving
        /// </summary>
        public static event LongPressEventHandler OnLongPress;

		/// <summary>
		/// Event fired when the drag gesture starts, due to the user's finger having moved at least moveThreshold units away from its initial position
		/// </summary>
		/// <seealso cref="moveThreshold"/>
		public static event DragBeginEventHandler OnDragBegin;

		/// <summary>
		/// Event fired whenever the dragged finger position has changed during a drag gesture. This is fired once per frame maximum.
		/// </summary>
		public static event DragMoveEventHandler OnDragMove;

		/// <summary>
		/// Event fired to signal the end of a drag gesture (drop). This happens when the dragged finger is released.
		/// </summary>
		public static event DragEndEventHandler OnDragEnd;

		/// <summary>
		/// Event fired when a pinch gesture has been initiated by the user. A pinch gesture requires two fingers moving closer or further away from each other.
		/// </summary>
		/// <seealso cref="trackPinch"/>
		/// <seealso cref="pinchDeltaScale"/>
		public static event PinchEventHandler OnPinchBegin;

		/// <summary>
		/// Event fired each frame whenever the two fingers move further or closer to each other during a pinch gesture
		/// </summary>
		public static event PinchMoveEventHandler OnPinchMove;

		/// <summary>
		/// Event fired to signal the end of the pinch gesture. This happens when at least one of the two fingers is released.
		/// </summary>
		public static event PinchEventHandler OnPinchEnd;

		/// <summary>
		/// Event fired when a finger is consecutively pressed and released at the same location within a certain timeframe (maxTapTime). e.g.g With a mouse, this can be used 
		/// to detect double/triple clicks, etc.
		/// </summary>
		/// <seealso cref="maxTapTime"/>
		/// <seealso cref="tapMoveTolerance"/>
		public static event TapEventHandler OnTap;

		/// <summary>
		/// Event fired when a finger performs a short and quick drag motion in a specific direction
		/// </summary>
		/// <seealso cref="minSwipeDistance"/>
		/// <seealso cref="swipeDirectionTolerance"/>
		public static event SwipeEventHandler OnSwipe;

        /// <summary>
        /// Event fired when a rotation gesture has been initiated by the user. A rotation gesture requires two fingers, with at least one of them moving around the other.
        /// </summary>
        /// <seealso cref="trackRotation"/>
        public static event RotationBeginEventHandler OnRotationBegin;

        /// <summary>
        /// Event fired each frame whenever the angle between the previous and current line defined by the two fingers has changed
        /// </summary>
        public static event RotationMoveEventHandler OnRotationMove;

        /// <summary>
        /// Event fired to signal the end of the rotation gesture. This happens when at least one of the two fingers is released.
        /// </summary>
        public static event RotationEndEventHandler OnRotationEnd;

        #endregion

        #region Delegates

        /// <summary>
		/// Delegate for the OnFingerDown event
		/// </summary>
		/// <param name="fingerIndex">0-based index uniquely indentifying a specific finger. For touch screen gestures, this corresponds to Touch.index, and the button index for mouse gestures</param>
		/// <param name="fingerPos">Current position of the finger on the screen</param>
		public delegate void FingerDownEventHandler( int fingerIndex, Vector2 fingerPos );

        /// <summary>
        /// Delegate for the OnFingerStationaryBegin event
        /// </summary>
        /// <param name="fingerIndex">0-based index uniquely indentifying a specific finger. For touch screen gestures, this corresponds to Touch.index, and the button index for mouse gestures</param>
        /// <param name="fingerPos">Current position of the finger on the screen</param>
        public delegate void FingerStationaryBeginEventHandler( int fingerIndex, Vector2 fingerPos );

		/// <summary>
		/// Delegate for the OnFingerStationary event
		/// </summary>
		/// <param name="fingerIndex">0-based index uniquely indentifying a specific finger. For touch screen gestures, this corresponds to Touch.index, and the button index for mouse gestures</param>
		/// <param name="fingerPos">Current position of the finger on the screen</param>
        /// <param name="elapsedTime">How much time has elapsed, in seconds, since the last OnFingerStationaryBegin fired on this finger</param>
		public delegate void FingerStationaryEventHandler( int fingerIndex, Vector2 fingerPos, float elapsedTime );

        /// <summary>
        /// Delegate for the OnFingerStationaryEnd event
        /// </summary>
        /// <param name="fingerIndex">0-based index uniquely indentifying a specific finger. For touch screen gestures, this corresponds to Touch.index, and the button index for mouse gestures</param>
        /// <param name="fingerPos">Current position of the finger on the screen</param>
        /// <param name="elapsedTime">How much time has elapsed, in seconds, since the last OnFingerStationaryBegin fired on this finger</param>
        public delegate void FingerStationaryEndEventHandler( int fingerIndex, Vector2 fingerPos, float elapsedTime );

		/// <summary>
		/// Delegate for the OnFingerUp event
		/// </summary>
		/// <param name="fingerIndex">0-based index uniquely indentifying a specific finger. For touch screen gestures, this corresponds to Touch.index, and the button index for mouse gestures</param>
		/// <param name="fingerPos">Current position of the finger on the screen</param>
		/// <param name="timeHeldDown">How long the finger has been held down before getting released, in seconds</param>
		public delegate void FingerUpEventHandler( int fingerIndex, Vector2 fingerPos, float timeHeldDown );

        /// <summary>
        /// Delegate for the OnFingerMoveBegin, OnFingerMove, OnFingerMoveEnd events
        /// </summary>
        /// <param name="fingerIndex">0-based index uniquely indentifying a specific finger. For touch screen gestures, this corresponds to Touch.index, and the button index for mouse gestures</param>
        /// <param name="fingerPos">Current position of the finger on the screen</param>
        public delegate void FingerMoveEventHandler( int fingerIndex, Vector2 fingerPos );

        /// <summary>
        /// Delegate for the OnLongPress event
        /// </summary>
        /// <param name="fingerIndex">0-based index uniquely indentifying a specific finger. For touch screen gestures, this corresponds to Touch.index, and the button index for mouse gestures</param>
        /// <param name="fingerPos">Current position of the finger on the screen</param>
        public delegate void LongPressEventHandler( int fingerIndex, Vector2 fingerPos );

		/// <summary>
		/// Delegate for the OnDragBegin event
		/// </summary>
		/// <param name="fingerIndex">0-based index uniquely indentifying a specific finger. For touch screen gestures, this corresponds to Touch.index, and the button index for mouse gestures</param>
		/// <param name="fingerPos">Current position of the finger on the screen</param>
		/// <param name="startPos">The initial finger position on the screen.</param>
		/// <remark>Since the finger has to move beyond a certain treshold distance (specified by the moveThreshold property) 
		/// before the gesture registers as a drag motion, fingerPos and startPos are likely to be different if you specified a non-zero moveThreshold.</remark>
		public delegate void DragBeginEventHandler( int fingerIndex, Vector2 fingerPos, Vector2 startPos );

		/// <summary>
		/// Delegate for the OnDragMove event
		/// </summary>
		/// <param name="fingerIndex">0-based index uniquely indentifying a specific finger. For touch screen gestures, this corresponds to Touch.index, and the button index for mouse gestures</param>
		/// <param name="fingerPos">Current position of the finger on the screen</param>
		/// <param name="delta">How much the finger has moved since the last update. This is the difference between the previous finger position and the new one.</param>
		public delegate void DragMoveEventHandler( int fingerIndex, Vector2 fingerPos, Vector2 delta );

		/// <summary>
		/// Delegate for the OnDragEnd event
		/// </summary>
		/// <param name="fingerIndex">0-based index uniquely indentifying a specific finger. For touch screen gestures, this corresponds to Touch.index, and the button index for mouse gestures</param>
		/// <param name="fingerPos">Current position of the finger on the screen</param>
		public delegate void DragEndEventHandler( int fingerIndex, Vector2 fingerPos );

		/// <summary>
		/// Delegate for the OnPinchBegin and OnPinchEnd events
		/// </summary>
		/// <param name="fingerPos1">Current position of the first finger on the screen</param>
		/// <param name="fingerPos2">Current position of the second finger on the screen</param>
		public delegate void PinchEventHandler( Vector2 fingerPos1, Vector2 fingerPos2 );

		/// <summary>
		/// Delegate for the OnPinchMove event
		/// </summary>
		/// <param name="fingerPos1">Current position of the first finger on the screen</param>
		/// <param name="fingerPos2">Current position of the second finger on the screen</param>
		/// <param name="delta">How much the distance between the two fingers has changed since the last update. A negative value means the two fingers got closer, while a positive value means they moved further apart</param>
		public delegate void PinchMoveEventHandler( Vector2 fingerPos1, Vector2 fingerPos2, float delta );

		/// <summary>
		/// Delegate for the OnTap event
		/// </summary>
		/// <param name="fingerIndex">0-based index uniquely indentifying a specific finger. For touch screen gestures, this corresponds to Touch.index, and the button index for mouse gestures</param>
		/// <param name="fingerPos">Current position of the finger on the screen</param>
		/// <param name="tapCount">How many times the user has consecutively tapped his finger at this location</param>
		public delegate void TapEventHandler( int fingerIndex, Vector2 fingerPos, int tapCount );

		/// <summary>
		/// Delegate for the OnSwipe event
		/// </summary>
		/// <param name="fingerIndex">0-based index uniquely indentifying a specific finger. For touch screen gestures, this corresponds to Touch.index, and the button index for mouse gestures</param>
		/// <param name="startPos">Initial position of the finger</param>
		/// <param name="direction">Direction of the swipe gesture</param>
		/// <param name="velocity">How quickly the finger has moved (in screen pixels per second)</param>
		public delegate void SwipeEventHandler( int fingerIndex, Vector2 startPos, SwipeDirection direction, float velocity );

        /// <summary>
        /// Delegate for the OnRotationBegin event
        /// </summary>
        /// <param name="fingerPos1">Current position of the first finger on the screen</param>
        /// <param name="fingerPos2">Current position of the second finger on the screen</param>
        public delegate void RotationBeginEventHandler( Vector2 fingerPos1, Vector2 fingerPos2 );

        /// <summary>
        /// Delegate for the OnRotationMove event
        /// </summary>
        /// <param name="fingerPos1">Current position of the first finger on the screen</param>
        /// <param name="fingerPos2">Current position of the second finger on the screen</param>
        /// <param name="rotationAngleDelta">Angle difference, in degrees, since the last update.</param>
        public delegate void RotationMoveEventHandler( Vector2 fingerPos1, Vector2 fingerPos2, float rotationAngleDelta );

        /// <summary>
        /// Delegate for the OnRotationEnd event
        /// </summary>
        /// <param name="fingerPos1">Current position of the first finger on the screen</param>
        /// <param name="fingerPos2">Current position of the second finger on the screen</param>
        /// <param name="totalRotationAngle">Total rotation performed during the gesture, in degrees</param>
        public delegate void RotationEndEventHandler( Vector2 fingerPos1, Vector2 fingerPos2, float totalRotationAngle );

		#endregion

		#endregion

		#region Properties exposed to the editor

        /// <summary>
        /// Minimum distance a finger must move from its initial position to be considered an actual move
        /// </summary>
        /// <remarks>
        /// The distance is expressed in screen units
        /// </remarks>
        public float moveThreshold = 5.0f;

		/// <summary>
		/// Whether or not to track the pinch gesture
		/// </summary>
		/// <remarks>
		/// Changing this at runtime won't have any effect
		/// </remarks>
		public bool trackPinch = true;

		/// <summary>
		/// How much to scale the internal pinch delta by before raising the OnPinchMove event
		/// </summary>
		public float pinchDeltaScale = 1.0f;

		/// <summary>
		/// Maximum window of time during which a single or multiple tap events can occur. Beyond this time, the tap counter is reset.
		/// </summary>
		public float maxTapTime = 0.75f;

		/// <summary>
		/// How far apart additional taps can be from the first tap position and still be treated as a valid new tap in the current sequence.
		/// Distance in screen units
		/// </summary>
		public float tapMoveTolerance = 2.0f;

		/// <summary>
		/// Minimum distance, in screen units, that the finger must travel in order to register as a possible swipe gesture
		/// </summary>
		public float minSwipeDistance = 2.0f;

		/// <summary>
		/// Amount of tolerance when determining if the finger motion was performed along one of the supported swipe directions.
		/// This amount should be kept between 0 and 0.5f, where 0 means no tolerance and 0.5f means you can move within 45 degrees away from the allowed direction
		/// </summary>
		public float swipeDirectionTolerance = 0.2f;

        /// <summary>
        /// Track 2-fingers rotation gesture?
        /// </summary>
        public bool trackRotation = true;

        /// <summary>
        /// How long the user must hold down his finger without moving in order to fire the OnLongPress event
        /// <seealso cref="OnLongPress"/>
        /// </summary>
        public float longPressDuration = 1.5f;

		#endregion

		#region Internal

		// access to the singleton
		static FingerGestures instance;

		protected enum FingerPhase
		{
			None,
			Began,
			Moved,
			Stationary,
			Ended,
		}

        protected enum FingerMotionState
        {
            None,
            Stationary,
            Moving,
        }

		protected class Finger
		{
			public int index = 0;
			public FingerPhase lastPhase = FingerPhase.None;
			public bool active = false;
			public bool moved = false;	    // has the finger moved at least once during the fingerDown -> fingerUp lifetime
            public FingerMotionState motionState = FingerMotionState.None;     // is the finger currently flagged as moving?
			public Vector2 startPos = Vector2.zero;
			public float startTime = 0; // when the finger was first pressed down
            public bool longPressFired = false;
            public Vector2 stationaryStartPos = Vector2.zero;
            public float stationaryStartTime = 0; // when the finger started becoming stationary again
            public Vector2 lastPos = Vector2.zero;
			public bool dragging = false;
			public float dragStartTime = 0;
			public int tapCount = 0;
			public float tapStartTime = 0;
			public Vector2 tapStartPos = Vector2.zero;

			public Finger( int index )
			{
				this.index = index;
			}

            public virtual void OnActivate( Vector2 position )
            {
                active = true;
                moved = false;
                motionState = FingerMotionState.None;
                startTime = Time.time;
                stationaryStartTime = Time.time;
                stationaryStartPos = position;
                startPos = position;
                lastPos = position;
                dragging = false;
                longPressFired = false;
            }

            public virtual void OnRelease()
            {
                active = false;
            }
		}

		Finger[] fingers;
		protected Finger GetFinger( int index )
		{
			return fingers[index];
		}

		void InitFingers( int count )
		{
			// pre-allocate a touch data entry for each finger
			fingers = new Finger[count];
			for( int i = 0; i < fingers.Length; ++i )
				fingers[i] = CreateFinger( i );
		}

		// Use this for initialization
		protected virtual void Awake()
		{
			instance = this;

			InitFingers( MaxFingers );
		}

		protected virtual void Start()
		{
			// reserved for future use	
		}

		protected virtual void Update()
		{
			if( fingers != null )
			{
				foreach( Finger finger in fingers )
				{
					FingerPhase phase = GetPhase( finger );

					if( phase == FingerPhase.None )
					{
						// In low framerate situations, it is possible to miss some input updates and thus skip the Ended phase.
						// We therefore have to make sure we release the fingers that are still flagged as active but are no longer available through the Input.touches collection
						if( finger.active && finger.lastPhase != FingerPhase.Ended )
						{
							Debug.LogWarning( "Unexpected phase transition: " + finger.lastPhase + " to " + phase + ". Forcing finger release." );
							Release( finger );
						}
					}
					else
					{
						Vector2 fingerPos = GetPosition( finger );
						UpdateFinger( finger, phase, fingerPos );
					}

					finger.lastPhase = phase;
				}
			}
			else
			{
				// this can happen if the code is recompiled at runtime
				Debug.LogWarning( "Fingers list was destroyed. Forcing reinitialization." );
				InitFingers( MaxFingers );
			}
		}

		protected virtual Finger CreateFinger( int index )
		{
			return new Finger( index );
		}

		// return the current phase of the finger
		protected abstract FingerPhase GetPhase( Finger finger );

		// return the current position of the finger on the screen
		protected abstract Vector2 GetPosition( Finger finger );

		// phase is guaranteed to not be FingerPhase.None
		protected virtual void UpdateFinger( Finger finger, FingerPhase phase, Vector2 fingerPos )
		{
			if( phase == FingerPhase.Ended )
			{
				// make sure we have the most up-to-date position of the touch saved in lastPos
				finger.lastPos = fingerPos;

				Release( finger );
			}
			else
			{
				// detect if it's the first frame the touch is down. We can't rely 100% on FingerPhase.Began to be received so we also check data.active flag.
				if( phase == FingerPhase.Began || !finger.active )
				{
                    finger.OnActivate( fingerPos );
					RaiseOnFingerDown( finger.index, finger.startPos );
				}
				else if( phase == FingerPhase.Moved ) // the finger has moved since last update
				{
                    if( finger.motionState != FingerMotionState.Moving )
                    {
                        bool validMove = false;
                        Vector3 moveStartPos = fingerPos;

                        // already started moving before
                        if( finger.moved )
                        {
                            validMove = true;
                        }
                        else
                        {
                            // have we gone past the moveTreshold?
                            moveStartPos = finger.startPos;
                            Vector2 moveDelta = fingerPos - finger.startPos;
                            validMove = ( moveDelta.sqrMagnitude > moveThreshold * moveThreshold );
                        }
                        
                        if( validMove )
                        {
                            if( finger.motionState == FingerMotionState.Stationary )
                                StopStationary( finger, finger.lastPos );

                            finger.motionState = FingerMotionState.Moving;
                            RaiseOnFingerMoveBegin( finger.index, moveStartPos );

                            // if the finger hasn't moved before, kick-off the drag event
                            if( !finger.moved )
                            {
                                finger.dragging = true;
                                finger.dragStartTime = Time.time;
                                RaiseOnDragBegin( finger.index, fingerPos, finger.startPos );

                                // we've now moved at least once
                                finger.moved = true;
                                finger.lastPos = fingerPos;
                            }
                        }
                        else
                        {
                            // still consider ourselves as stationary
                            HandleStationaryFinger( finger, fingerPos );
                        }
                    }

                    if( finger.moved )
                    {
                        Vector2 delta = fingerPos - finger.lastPos;

                        if( delta.sqrMagnitude > 0 )
                        {
                            if( finger.motionState == FingerMotionState.Moving  )
                                RaiseOnFingerMove( finger.index, fingerPos );

                            if( finger.dragging )
                                RaiseOnDragMove( finger.index, fingerPos, delta );
                        }
                    }
				}
				else if( phase == FingerPhase.Stationary ) // the finger hasn't moved since last frame
				{
                    HandleStationaryFinger( finger, fingerPos );
				}

				finger.lastPos = fingerPos;
			}
		}

        void StopMoving( Finger finger, Vector2 fingerPos )
        {
            // stop moving
            finger.motionState = FingerMotionState.None;
            RaiseOnFingerMoveEnd( finger.index, fingerPos );
        }

        void StopStationary( Finger finger, Vector2 fingerPos )
        {
            finger.motionState = FingerMotionState.None;
            RaiseOnFingerStationaryEnd( finger.index, fingerPos, Time.time - finger.stationaryStartTime );
        }

        protected void HandleStationaryFinger( Finger finger, Vector2 fingerPos )
        {
            // check if we need to reset stationary properties
            if( finger.motionState != FingerMotionState.Stationary )
            {
                // were we moving previously?
                if( finger.motionState == FingerMotionState.Moving )
                    StopMoving( finger, fingerPos );

                finger.motionState = FingerMotionState.Stationary;
                finger.stationaryStartTime = Time.time;
                finger.stationaryStartPos = fingerPos;

                RaiseOnFingerStationaryBegin( finger.index, fingerPos );
            }

            float elapsedTime = Time.time - finger.stationaryStartTime;

            // if the finger has never moved before, check for long-press gesture
            if( !finger.longPressFired && !finger.moved && elapsedTime >= longPressDuration )
            {
                RaiseOnLongPress( finger.index, finger.startPos );

                // dont fire the event again
                finger.longPressFired = true;
            }

            RaiseOnFingerStationary( finger.index, finger.stationaryStartPos, elapsedTime );
        }

		protected virtual void Release( Finger finger )
		{
			if( finger.moved || finger.dragging )
			{
				// also reset tap count since the finger moved anyway
				finger.tapCount = 0;

				// send notification that the drag gesture is over
				RaiseOnDragEnd( finger.index, finger.lastPos );

				// check if we have a valid swipe gesture & raise the event
				SwipeDirection swipeDir;
				float swipeVelocity;

				if( GetSwipeGesture( finger, out swipeDir, out swipeVelocity ) )
					RaiseOnSwipe( finger.index, finger.startPos, swipeDir, swipeVelocity );
			}
			else
			{
				if( ShouldResetTap( finger ) )
					finger.tapCount = 0;
				
				if( finger.tapCount == 0 )
				{
					finger.tapStartTime = Time.time;
					finger.tapStartPos = finger.lastPos;
				}

				RaiseOnTap( finger.index, finger.lastPos, ++finger.tapCount );
			}

            if( finger.motionState == FingerMotionState.Stationary )
                StopStationary( finger, finger.lastPos );
            else if( finger.motionState == FingerMotionState.Moving )
                StopMoving( finger, finger.lastPos );
            
            float elapsedTime = Time.time - finger.startTime;
            RaiseOnFingerUp( finger.index, finger.lastPos, elapsedTime );
            
            finger.OnRelease();
		}

		bool GetSwipeGesture( Finger finger, out SwipeDirection direction, out float velocity )
		{
			direction = SwipeDirection.Right;
			velocity = 0;

			Vector3 delta = finger.lastPos - finger.startPos;
			float distance = delta.magnitude;

			// didnt move far enough
			if( distance < minSwipeDistance )
				return false;

			float minSwipeDot = Mathf.Clamp01( 1.0f - swipeDirectionTolerance ); 

			// check if the finger along one of the supported swipe directions, within the allowed tolerance threshold
			Vector2 dir = delta.normalized;
			if( Vector2.Dot( dir, Vector2.right ) >= minSwipeDot )
				direction = SwipeDirection.Right;
			else if( Vector2.Dot( dir, -Vector2.right ) >= minSwipeDot )
				direction = SwipeDirection.Left;
			else if( Vector2.Dot( dir, Vector2.up ) >= minSwipeDot )
				direction = SwipeDirection.Up;
			else if( Vector2.Dot( dir, -Vector2.up ) >= minSwipeDot )
				direction = SwipeDirection.Down;
			else
			{
				// not a valid direction
				return false;
			}

			float elapsedTime = Time.time - finger.dragStartTime;
			velocity = distance / elapsedTime;

			return true;
		}

		bool ShouldResetTap( Finger finger )
		{
			if( finger.tapCount > 0 )
			{
				// check if too much time has elapsed
				float elapsedTapTime = Time.time - finger.tapStartTime;
				if( elapsedTapTime > maxTapTime )
					return true;

				// check if this new tap moved too far away from the initial tap position
				Vector2 tapDelta = finger.lastPos - finger.tapStartPos;
				if( tapDelta.SqrMagnitude() > ( tapMoveTolerance * tapMoveTolerance ) )
					return true;
			}

			return false;
		}
		
		#region Event raising methods used by derived classes

		protected void RaiseOnFingerDown( int fingerIndex, Vector2 fingerPos )
		{
			if( OnFingerDown != null )
				OnFingerDown( fingerIndex, fingerPos );
		}

        protected void RaiseOnFingerStationaryBegin( int fingerIndex, Vector2 fingerPos )
        {
            if( OnFingerStationaryBegin != null )
                OnFingerStationaryBegin( fingerIndex, fingerPos );
        }

		protected void RaiseOnFingerStationary( int fingerIndex, Vector2 fingerPos, float elapsedTime )
		{
			if( OnFingerStationary != null )
				OnFingerStationary( fingerIndex, fingerPos, elapsedTime );
		}

        protected void RaiseOnFingerStationaryEnd( int fingerIndex, Vector2 fingerPos, float elapsedTime )
        {
            if( OnFingerStationaryEnd != null )
                OnFingerStationaryEnd( fingerIndex, fingerPos, elapsedTime );
        }

        protected void RaiseOnFingerMoveBegin( int fingerIndex, Vector2 fingerPos )
        {
            if( OnFingerMoveBegin != null )
                OnFingerMoveBegin( fingerIndex, fingerPos );
        }

        protected void RaiseOnFingerMove( int fingerIndex, Vector2 fingerPos )
        {
            if( OnFingerMove != null )
                OnFingerMove( fingerIndex, fingerPos );
        }

        protected void RaiseOnFingerMoveEnd( int fingerIndex, Vector2 fingerPos )
        {
            if( OnFingerMoveEnd != null )
                OnFingerMoveEnd( fingerIndex, fingerPos );
        }

		protected void RaiseOnFingerUp( int fingerIndex, Vector2 fingerPos, float timeHeldDown )
		{
			if( OnFingerUp != null )
				OnFingerUp( fingerIndex, fingerPos, timeHeldDown );
		}

        protected void RaiseOnLongPress( int fingerIndex, Vector2 fingerPos )
        {
            if( OnLongPress != null )
                OnLongPress( fingerIndex, fingerPos );
        }

		protected void RaiseOnDragBegin( int fingerIndex, Vector2 fingerPos, Vector2 startPos )
		{
			if( OnDragBegin != null )
				OnDragBegin( fingerIndex, fingerPos, startPos );
		}

		protected void RaiseOnDragMove( int fingerIndex, Vector2 fingerPos, Vector2 delta )
		{
			if( OnDragMove != null )
				OnDragMove( fingerIndex, fingerPos, delta );
		}

		protected void RaiseOnDragEnd( int fingerIndex, Vector2 fingerPos )
		{
			if( OnDragEnd != null )
				OnDragEnd( fingerIndex, fingerPos );
		}

		protected void RaiseOnPinchBegin( Vector2 fingerPos1, Vector2 fingerPos2 )
		{
			if( OnPinchBegin != null )
				OnPinchBegin( fingerPos1, fingerPos2 );
		}

		protected void RaiseOnPinchMove( Vector2 fingerPos1, Vector2 fingerPos2, float delta )
		{
			if( OnPinchMove != null )
				OnPinchMove( fingerPos1, fingerPos2, pinchDeltaScale * delta );
		}

		protected void RaiseOnPinchEnd( Vector2 fingerPos1, Vector2 fingerPos2 )
		{
			if( OnPinchEnd != null )
				OnPinchEnd( fingerPos1, fingerPos2 );
		}

		protected void RaiseOnTap( int fingerIndex, Vector2 fingerPos, int tapCount )
		{
			if( OnTap != null )
				OnTap( fingerIndex, fingerPos, tapCount );
		}

		protected void RaiseOnSwipe( int fingerIndex, Vector2 startPos, SwipeDirection direction, float velocity )
		{
			if( OnSwipe != null )
				OnSwipe( fingerIndex, startPos, direction, velocity );
		}

        protected void RaiseOnRotationBegin( Vector2 fingerPos1, Vector2 fingerPos2 )
        {
            if( OnRotationBegin != null )
                OnRotationBegin( fingerPos1, fingerPos2 );
        }

        protected void RaiseOnRotationMove( Vector2 fingerPos1, Vector2 fingerPos2, float rotationAngleDelta )
        {
            if( OnRotationMove != null )
                OnRotationMove( fingerPos1, fingerPos2, rotationAngleDelta );
        }

        protected void RaiseOnRotationEnd( Vector2 fingerPos1, Vector2 fingerPos2, float totalRotationAngle )
        {
            if( OnRotationEnd != null )
                OnRotationEnd( fingerPos1, fingerPos2, totalRotationAngle );
        }

		#endregion

		#endregion
	}
}