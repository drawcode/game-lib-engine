using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Engine.Events;

public class FingerGesturesMessages {
    public static string OnTap = "FingerGesture_OnTap";
    public static string OnSwipe = "FingerGesture_OnSwipe";
    public static string OnPinch = "FingerGesture_OnPinch";
    public static string OnLongPress = "FingerGesture_OnLongPress";
    public static string OnTwist = "FingerGesture_OnTwist";
    public static string OnDrag = "FingerGesture_OnDrag";
    public static string OnDoubleTap = "FingerGesture_OnDoubleTap";
    // TODO others...
}

public class FingerGesturesBroadcaster : GameObjectBehavior {
    
    void Start() {
    
    }
    
    void OnTap(TapGesture gesture) {
        //if( gesture.Selection )
        //    LogUtil.Log( "Tapped object: " + gesture.Selection.name );
        //else
        //    LogUtil.Log( "No object was tapped at " + gesture.Position );
        
        Messenger<TapGesture>.Broadcast(FingerGesturesMessages.OnTap, gesture);
    
    }
    
    void OnDoubleTap(TapGesture gesture) {
        //if( gesture.Selection )
        //    LogUtil.Log( "Tapped object: " + gesture.Selection.name );
        //else
        //    LogUtil.Log( "No object was tapped at " + gesture.Position );
        
        Messenger<TapGesture>.Broadcast(FingerGesturesMessages.OnDoubleTap, gesture);
    
    }
    
    void OnSwipe(SwipeGesture gesture) {
        //if( gesture.Selection )
        //    LogUtil.Log( "Swipe object: " + gesture.Selection.name );
        //else
        //    LogUtil.Log( "No object was swiped at " + gesture.Position );
        
        Messenger<SwipeGesture>.Broadcast(FingerGesturesMessages.OnSwipe, gesture);
    }
    
    void OnPinch(PinchGesture gesture) {
        //if( gesture.Selection )
        //    LogUtil.Log( "Pinch object: " + gesture.Selection.name );
        //else
        //    LogUtil.Log( "No object was pinched at " + gesture.Position );
        
        Messenger<PinchGesture>.Broadcast(FingerGesturesMessages.OnPinch, gesture);
    }
    
    void OnLongPress(LongPressGesture gesture) {
        //if( gesture.Selection )
        //    LogUtil.Log( "LongPress object: " + gesture.Selection.name );
        //else
        //    LogUtil.Log( "No object was long pressed at " + gesture.Position );
        
        Messenger<LongPressGesture>.Broadcast(FingerGesturesMessages.OnLongPress, gesture);
    }
        
    void OnTwist(TwistGesture gesture) {
        //if( gesture.Selection )
        //    LogUtil.Log( "Twist object: " + gesture.Selection.name );
        //else
        //    LogUtil.Log( "No object was twisted at " + gesture.Position );
        
        Messenger<TwistGesture>.Broadcast(FingerGesturesMessages.OnTwist, gesture);
    }
    
    void OnDrag(DragGesture gesture) {
        //if( gesture.Selection )
        //    LogUtil.Log( "Drag object: " + gesture.Selection.name );
        //else
        //    LogUtil.Log( "No object was dragged at " + gesture.Position );
        
        Messenger<DragGesture>.Broadcast(FingerGesturesMessages.OnDrag, gesture);
    }
}

