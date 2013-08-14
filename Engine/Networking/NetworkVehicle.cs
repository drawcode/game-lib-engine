//  Copyright © 2011 Impossible Interactive LLC

using System;
using UnityEngine;

namespace Engine.Networking {

    // TODO: NOTE this shouldn't be here, we need to move the items in Start, Awake etc to base methods that can
    // be called from Vehicle and NetworkVehicle on their respective Start, Awake functions, for now I am
    // using this to attach the network player.
    public class NetworkVehicle {
#if !UNITY_FLASH

        // Needed for current network setup, we can change.  Or pull in or make a networkvehicle...
        // For all network movement wrap with isMe (or change) so you only affect your vehicle.
        public bool isMe = false;

        public void SetOwner(bool owner) {
            isMe = owner;
        }

        /*
        new public float WheelBase
        {
            get { return Vector3.Distance(frontWheelSet.CenterPosition, rearWheelSet.CenterPosition); }
        }

        new public float SlipVelocity
        {
            get {
                float val = 0.0f;

                foreach(Wheel w in frontWheelSet)
                    val += w.SlipVelocity / rearWheelSet.Count;

                foreach(Wheel w in rearWheelSet)
                    val += w.SlipVelocity / rearWheelSet.Count;

                return val;
            }
        }

        new public float Steering
        {
            set {
                frontWheelSet.Steering = value;
            }
        }

        new public float Brake
        {
            set {
                frontWheelSet.Brake = value;
                rearWheelSet.Brake = value;
            }
        }

        new public float HandBrake
        {
            get {
                return handBrake;
            }
            set {
                handBrake = value;
                rearWheelSet.HandBrake = value;
            }
        }

        new public void ShiftUp()
        {
            if(isMe)
            {
                drivetrain.ShiftUp();
            }
        }

        new public void ShiftDown()
        {
            if(isMe)
            {
                drivetrain.ShiftDown();
            }
        }

        new public void Update ()
        {
            if(isMe)
            {
                frontWheelSet.Update();
                rearWheelSet.Update();
            }
        }

        new public void FixedUpdate()
        {
            if(isMe)
            {
                frontWheelSet.Simulate(0f, 0f, 0f);
                drivetrain.Simulate(engine, rearWheelSet, rigidbody);
            }
        }
        */
#endif
    }
}