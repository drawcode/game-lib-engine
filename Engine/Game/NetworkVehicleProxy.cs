using System;
using Engine.Utility;
using UnityEngine;

namespace Engine.Game.Racing {

    public class NetworkVehicleProxy : GameObjectBehavior {
        /*
        public int competitorIndex;
        public GameObject source;
        public GameObject target;

        // For now wheels...
        public Transform modelTarget;

        public Transform wheelModelFrontLeftTarget;
        public Transform wheelModelFrontRightTarget;
        public Transform wheelModelRearLeftTarget;
        public Transform wheelModelRearRightTarget;

        public Transform wheelControlFrontLeftTarget;
        public Transform wheelControlFrontRightTarget;
        public Transform wheelControlRearLeftTarget;
        public Transform wheelControlRearRightTarget;

        public Transform modelSource;

        public Transform wheelModelFrontLeftSource;
        public Transform wheelModelFrontRightSource;
        public Transform wheelModelRearLeftSource;
        public Transform wheelModelRearRightSource;

        public Transform wheelControlFrontLeftSource;
        public Transform wheelControlFrontRightSource;
        public Transform wheelControlRearLeftSource;
        public Transform wheelControlRearRightSource;

        public double interpolationBackTime = 0.1;
        public double extrapolationLimit = 0.5;

        internal struct State
        {
            internal double timestamp;
            internal Vector3 pos;
            internal Vector3 velocity;
            internal Quaternion rot;
            internal Vector3 angularVelocity;

            internal Vector3 modelPos;
            internal Quaternion modelRot;

            internal Vector3 wheelModelFrontLeftPos;
            internal Vector3 wheelModelFrontRightPos;
            internal Vector3 wheelModelRearLeftPos;
            internal Vector3 wheelModelRearRightPos;

            internal Quaternion wheelModelFrontLeftRot;
            internal Quaternion wheelModelFrontRightRot;
            internal Quaternion wheelModelRearLeftRot;
            internal Quaternion wheelModelRearRightRot;

            internal Vector3 wheelControlFrontLeftPos;
            internal Vector3 wheelControlFrontRightPos;
            internal Vector3 wheelControlRearLeftPos;
            internal Vector3 wheelControlRearRightPos;

            internal Quaternion wheelControlFrontLeftRot;
            internal Quaternion wheelControlFrontRightRot;
            internal Quaternion wheelControlRearLeftRot;
            internal Quaternion wheelControlRearRightRot;
        }

        // We store twenty states with "playback" information
        State[] bufferedState = new State[20];

        // Keep track of what slots are used
        int timestampCount;

        //Race race;

        public void FindRace()
        {
            if(race == null) {
                race = Objects.FindRequiredObject<Race>();
            }
        }

        public void FindTarget()
        {
            if(target == null) {
                target = Objects.FindObject<NetworkController>();
                LogUtil.Log("NetworkVehicleProxy:: TARGET target FOUND");
            }
            else {
                LogUtil.Log("NetworkVehicleProxy:: TARGET target NOT FOUND");
            }
        }

        public void FindTargetObjects()
        {
            FindTarget();

            if(target != null) {
                if(modelTarget == null) {
                    var modelTargetObject = target.gameObject.transform.Find("Offset/Model");
                    if(modelTargetObject) {
                        modelTarget = modelTargetObject.transform;
                        LogUtil.Log("NetworkVehicleProxy:: TARGET found modelTarget Model");
                    }
                }
                else {
                    LogUtil.Log("NetworkVehicleProxy:: TARGET modelTarget NOT FOUND");
                }

                if(wheelControlFrontLeftTarget == null) {
                    var wheelControlFrontLeftTargetObject = target.gameObject.transform.Find("Offset/Wheelset/Control Isolator");
                    if(wheelControlFrontLeftTargetObject) {
                        wheelControlFrontLeftTarget = wheelControlFrontLeftTargetObject.transform;

                        LogUtil.Log("NetworkVehicleProxy:: TARGET found wheelControlFrontLeftTarget Control Isolator");

                        if(wheelModelFrontLeftTarget == null) {
                            var wheelModelFrontLeftTargetObject = wheelControlFrontLeftTarget.Find("FD/Spin Isolator");
                            if(wheelModelFrontLeftTargetObject != null) {
                                wheelModelFrontLeftTarget = wheelModelFrontLeftTargetObject.transform;
                                LogUtil.Log("NetworkVehicleProxy:: TARGET found wheelModelFrontLeftTarget Spin Isolator");
                            }
                        }
                        else {
                            LogUtil.Log("NetworkVehicleProxy:: TARGET wheelModelFrontLeftTarget NOT FOUND");
                        }
                    }
                }
                else {
                    LogUtil.Log("NetworkVehicleProxy:: TARGET wheelControlFrontLeftTarget NOT FOUND");
                }

                if(wheelControlFrontRightTarget == null) {
                    var wheelControlFrontRightTargetObject = target.gameObject.transform.Find("Offset/Wheelset/Control Isolator");
                    if(wheelControlFrontRightTargetObject) {
                        wheelControlFrontRightTarget = wheelControlFrontRightTargetObject.transform;
                        LogUtil.Log("NetworkVehicleProxy:: TARGET found wheelControlFrontRightTarget Control Isolator");

                        if(wheelModelFrontRightTarget == null) {
                            var wheelModelFrontRightTargetObject = wheelControlFrontRightTarget.Find("FP/Spin Isolator");
                            if(wheelModelFrontRightTargetObject != null) {
                                wheelModelFrontRightTarget = wheelModelFrontRightTargetObject.transform;
                                LogUtil.Log("NetworkVehicleProxy:: TARGET found wheelModelFrontRightTarget Spin Isolator");
                            }
                        }
                        else {
                            LogUtil.Log("NetworkVehicleProxy:: TARGET wheelModelFrontRightTarget NOT FOUND");
                        }
                    }
                }
                else {
                    LogUtil.Log("NetworkVehicleProxy:: TARGET wheelControlFrontRightTarget NOT FOUND");
                }

                if(wheelControlRearLeftTarget == null) {
                    var wheelControlRearLeftTargetObject = target.gameObject.transform.Find("Offset/Wheelset/Control Isolator");
                    if(wheelControlRearLeftTargetObject) {
                        wheelControlRearLeftTarget = wheelControlRearLeftTargetObject.transform;
                        LogUtil.Log("NetworkVehicleProxy:: TARGET found wheelControlRearLeftTarget Control Isolator");

                        if(wheelModelRearLeftTarget == null) {
                            var wheelModelRearLeftTargetObject = wheelControlRearLeftTarget.Find("RD/Spin Isolator");
                            if(wheelModelRearLeftTargetObject != null) {
                                wheelModelRearLeftTarget = wheelModelRearLeftTargetObject.transform;
                                LogUtil.Log("NetworkVehicleProxy:: TARGET found wheelModelRearLeftTarget Spin Isolator");
                            }
                        }
                        else {
                            LogUtil.Log("NetworkVehicleProxy:: TARGET wheelModelRearLeftTarget NOT FOUND");
                        }
                    }
                }
                else {
                    LogUtil.Log("NetworkVehicleProxy:: TARGET wheelControlRearLeftTarget NOT FOUND");
                }

                if(wheelControlRearRightTarget == null) {
                    var wheelControlRearRightTargetObject = target.gameObject.transform.Find("Offset/Wheelset/Control Isolator");
                    if(wheelControlRearRightTargetObject) {
                        wheelControlRearRightTarget = wheelControlRearRightTargetObject.transform;
                        LogUtil.Log("NetworkVehicleProxy:: TARGET found wheelControlRearRightTarget Control Isolator");

                        if(wheelModelRearRightTarget == null) {
                            var wheelModelRearRightTargetObject = wheelControlRearRightTarget.Find("RP/Spin Isolator");
                            if(wheelModelRearRightTargetObject != null) {
                                wheelModelRearRightTarget = wheelModelRearRightTargetObject.transform;
                                LogUtil.Log("NetworkVehicleProxy:: TARGET found wheelModelRearRightTarget Spin Isolator");
                            }
                        }
                        else {
                            LogUtil.Log("NetworkVehicleProxy:: TARGET wheelModelRearRightTarget NOT FOUND");
                        }
                    }
                }
                else {
                    LogUtil.Log("NetworkVehicleProxy:: TARGET wheelControlRearRightTarget NOT FOUND");
                }
            }
        }

        public void FindSourceObjects()
        {
            if(source != null) {
                if(modelSource == null) {
                    var modelSourceObject = source.gameObject.transform.Find("Offset/Model");
                    if(modelSourceObject) {
                        modelSource = modelSourceObject.transform;
                        LogUtil.Log("NetworkVehicleProxy:: SOURCE found modelSource Model");
                    }
                }
                else {
                    LogUtil.Log("NetworkVehicleProxy:: TARGET modelSource NOT FOUND");
                }

                if(wheelControlFrontLeftSource == null) {
                    var wheelControlFrontLeftSourceObject = source.gameObject.transform.Find("Offset/Wheelset/Control Isolator");
                    if(wheelControlFrontLeftSourceObject)  {
                        wheelControlFrontLeftSource = wheelControlFrontLeftSourceObject.transform;
                        LogUtil.Log("NetworkVehicleProxy:: SOURCE found wheelControlFrontLeftSource Control Isolator");

                        if(wheelModelFrontLeftSource == null) {
                            var wheelModelFrontLeftSourceObject = wheelControlFrontLeftSource.Find("FD/Spin Isolator");
                            if(wheelModelFrontLeftSourceObject != null) {
                                wheelModelFrontLeftSource = wheelModelFrontLeftSourceObject.transform;
                                LogUtil.Log("NetworkVehicleProxy:: SOURCE found wheelModelFrontLeftSource Spin Isolator");
                            }
                        }
                        else {
                            LogUtil.Log("NetworkVehicleProxy:: TARGET wheelModelFrontLeftSource NOT FOUND");
                        }
                    }
                }
                else {
                    LogUtil.Log("NetworkVehicleProxy:: TARGET wheelControlFrontLeftSource NOT FOUND");
                }

                if(wheelControlFrontRightSource == null) {
                    var wheelControlFrontRightSourceObject = source.gameObject.transform.Find("Offset/Wheelset/Control Isolator");
                    if(wheelControlFrontRightSourceObject) {
                        wheelControlFrontRightSource = wheelControlFrontRightSourceObject.transform;
                        LogUtil.Log("NetworkVehicleProxy:: SOURCE found wheelControlFrontRightSource Control Isolator");

                        if(wheelModelFrontRightSource == null) {
                            var wheelModelFrontRightSourceObject = wheelControlFrontRightSource.Find("FP/Spin Isolator");
                            if(wheelModelFrontRightSourceObject != null) {
                                wheelModelFrontRightSource = wheelModelFrontRightSourceObject.transform;
                                LogUtil.Log("NetworkVehicleProxy:: SOURCE found wheelModelFrontRightSource Spin Isolator");
                            }
                        }
                        else {
                            LogUtil.Log("NetworkVehicleProxy:: TARGET wheelModelFrontRightSource NOT FOUND");
                        }
                    }
                }
                else {
                    LogUtil.Log("NetworkVehicleProxy:: TARGET wheelControlFrontRightSource NOT FOUND");
                }

                if(wheelControlRearLeftSource == null) {
                    var wheelControlRearLeftSourceObject = source.gameObject.transform.Find("Offset/Wheelset/Control Isolator");
                    if(wheelControlRearLeftSourceObject) {
                        wheelControlRearLeftSource = wheelControlRearLeftSourceObject.transform;
                        LogUtil.Log("NetworkVehicleProxy:: SOURCE found wheelControlRearLeftSource Control Isolator");

                        if(wheelModelRearLeftSource == null) {
                            var wheelModelRearLeftSourceObject = wheelControlRearLeftSource.Find("RD/Spin Isolator");
                            if(wheelModelRearLeftSourceObject != null) {
                                wheelModelRearLeftSource = wheelModelRearLeftSourceObject.transform;
                                LogUtil.Log("NetworkVehicleProxy:: SOURCE found wheelModelRearLeftSource Spin Isolator");
                            }
                        }
                        else {
                            LogUtil.Log("NetworkVehicleProxy:: TARGET wheelModelRearLeftSource NOT FOUND");
                        }
                    }
                }
                else {
                    LogUtil.Log("NetworkVehicleProxy:: TARGET wheelControlRearLeftSource NOT FOUND");
                }

                if(wheelControlRearRightSource == null) {
                    var wheelControlRearRightSourceObject = source.gameObject.transform.Find("Offset/Wheelset/Control Isolator");
                    if(wheelControlRearRightSourceObject) {
                        wheelControlRearRightSource = wheelControlRearRightSourceObject.transform;
                        LogUtil.Log("NetworkVehicleProxy:: SOURCE found wheelControlRearRightSource Control Isolator");

                        if(wheelModelRearRightSource == null) {
                            var wheelModelRearRightSourceObject = wheelControlRearRightSource.Find("RP/Spin Isolator");
                            if(wheelModelRearRightSourceObject != null) {
                                wheelModelRearRightSource = wheelModelRearRightSourceObject.transform;
                                LogUtil.Log("NetworkVehicleProxy:: SOURCE found wheelModelRearRightSource Spin Isolator");
                            }
                        }
                        else {
                            LogUtil.Log("NetworkVehicleProxy:: TARGET wheelModelRearRightSource NOT FOUND");
                        }
                    }
                }
                else {
                    LogUtil.Log("NetworkVehicleProxy:: TARGET wheelControlRearRightSource NOT FOUND");
                }
            }
        }

        public bool IsCompetitorAvailable
        {
            get
            {
                if(race == null)
                    FindRace();

                if(wheelControlFrontLeftSource == null)
                    FindTargetObjects();

                if(race != null)
                {
                    if(race.Competitors.Count > competitorIndex)
                    {
                        if(race.Competitors[competitorIndex] != null)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        public void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
        {
            // Send data to server
            if (stream.isWriting)
            {
                if(IsCompetitorAvailable)
                {
                    FindSourceObjects();

                    var sourceRigidBody = source.rigidbody;

                    var pos = sourceRigidBody.transform.position;
                    var rot = sourceRigidBody.transform.rotation;

                    stream.Serialize(ref competitorIndex);
                    stream.Serialize(ref pos);

                    //stream.Serialize(ref velocity);
                    stream.Serialize(ref rot);

                    //stream.Serialize(ref angularVelocity);

                    var modelSourcePos = Vector3.zero;
                    if(modelSource)
                        modelSourcePos = modelSource.position;
                    stream.Serialize(ref modelSourcePos);

                    var modelSourceRot = Quaternion.identity;
                    if(modelSource)
                        modelSourceRot = modelSource.rotation;
                    stream.Serialize(ref modelSourceRot);

                    var wheelControlFrontLeftPos = Vector3.zero;
                    if(wheelControlFrontLeftSource)
                        wheelControlFrontLeftPos = wheelControlFrontLeftSource.position;
                    stream.Serialize(ref wheelControlFrontLeftPos);

                    var wheelControlFrontRightPos = Vector3.zero;
                    if(wheelControlFrontRightSource)
                        wheelControlFrontRightPos = wheelControlFrontRightSource.position;
                    stream.Serialize(ref wheelControlFrontRightPos);

                    var wheelControlRearLeftPos = Vector3.zero;
                    if(wheelControlRearLeftSource)
                        wheelControlRearLeftPos = wheelControlRearLeftSource.position;
                    stream.Serialize(ref wheelControlRearLeftPos);

                    var wheelControlRearRightPos = Vector3.zero;
                    if(wheelControlRearRightSource)
                        wheelControlRearRightPos = wheelControlRearRightSource.position;
                    stream.Serialize(ref wheelControlRearRightPos);

                    var wheelControlFrontLeftRot = Quaternion.identity;
                    if(wheelControlFrontLeftSource)
                        wheelControlFrontLeftRot = wheelControlFrontLeftSource.rotation;
                    stream.Serialize(ref wheelControlFrontLeftRot);

                    var wheelControlFrontRightRot = Quaternion.identity;
                    if(wheelControlFrontRightSource)
                        wheelControlFrontRightRot = wheelControlFrontRightSource.rotation;
                    stream.Serialize(ref wheelControlFrontRightRot);

                    var wheelControlRearLeftRot = Quaternion.identity;
                    if(wheelControlRearLeftSource)
                        wheelControlRearLeftRot = wheelControlRearLeftSource.rotation;
                    stream.Serialize(ref wheelControlRearLeftRot);

                    var wheelControlRearRightRot = Quaternion.identity;
                    if(wheelControlRearRightSource)
                        wheelControlRearRightRot = wheelControlRearRightSource.rotation;
                    stream.Serialize(ref wheelControlRearRightRot);
                }
            }

            // Read data from remote client
            else
            {
                if(IsCompetitorAvailable)
                {
                    stream.Serialize(ref competitorIndex);

                    if(race != null)
                    {
                        FindTarget();

                        if(target != null)
                        {
                            target = race.Competitors[competitorIndex].controller as NetworkController;
                            var wcVehicle = target.GetComponent<WCVehicle>();

                            if(wcVehicle != null)
                                wcVehicle.enabled = false;

                            Vector3 pos = Vector3.zero;
                            Vector3 velocity = Vector3.zero;
                            Quaternion rot = Quaternion.identity;
                            Vector3 angularVelocity = Vector3.zero;

                            Vector3 modelPos = Vector3.zero;

                            Quaternion modelRot = Quaternion.identity;

                            Vector3 wheelControlFrontLeftPos = Vector3.zero;
                            Vector3 wheelControlFrontRightPos = Vector3.zero;
                            Vector3 wheelControlRearLeftPos = Vector3.zero;
                            Vector3 wheelControlRearRightPos = Vector3.zero;

                            Quaternion wheelControlFrontLeftRot = Quaternion.identity;
                            Quaternion wheelControlFrontRightRot = Quaternion.identity;
                            Quaternion wheelControlRearLeftRot = Quaternion.identity;
                            Quaternion wheelControlRearRightRot = Quaternion.identity;

                            Vector3 wheelModelFrontLeftPos = Vector3.zero;
                            Vector3 wheelModelFrontRightPos = Vector3.zero;
                            Vector3 wheelModelRearLeftPos = Vector3.zero;
                            Vector3 wheelModelRearRightPos = Vector3.zero;

                            Quaternion wheelModelFrontLeftRot = Quaternion.identity;
                            Quaternion wheelModelFrontRightRot = Quaternion.identity;
                            Quaternion wheelModelRearLeftRot = Quaternion.identity;
                            Quaternion wheelModelRearRightRot = Quaternion.identity;

                            stream.Serialize(ref pos);

                            //stream.Serialize(ref velocity);
                            stream.Serialize(ref rot);

                            //stream.Serialize(ref angularVelocity);

                            // Shift the buffer sideways, deleting state 20
                            for (int i = bufferedState.Length - 1; i >= 1; i--) {
                                bufferedState[i] = bufferedState[i - 1];
                            }

                            // Record current state in slot 0
                            State state;
                            state.timestamp = info.timestamp;
                            state.pos = pos;
                            state.velocity = velocity;
                            state.rot = rot;
                            state.angularVelocity = angularVelocity;

                            state.modelPos = modelPos;
                            state.modelRot = modelRot;

                            state.wheelControlFrontLeftPos = wheelControlFrontLeftPos;
                            state.wheelControlFrontRightPos = wheelControlFrontRightPos;
                            state.wheelControlRearLeftPos = wheelControlRearLeftPos;
                            state.wheelControlRearRightPos = wheelControlRearRightPos;

                            state.wheelControlFrontLeftRot = wheelControlFrontLeftRot;
                            state.wheelControlFrontRightRot = wheelControlFrontRightRot;
                            state.wheelControlRearLeftRot = wheelControlRearLeftRot;
                            state.wheelControlRearRightRot = wheelControlRearRightRot;

                            state.wheelModelFrontLeftPos = wheelModelFrontLeftPos;
                            state.wheelModelFrontRightPos = wheelModelFrontRightPos;
                            state.wheelModelRearLeftPos = wheelModelRearLeftPos;
                            state.wheelModelRearRightPos = wheelModelRearRightPos;

                            state.wheelModelFrontLeftRot = wheelModelFrontLeftRot;
                            state.wheelModelFrontRightRot = wheelModelFrontRightRot;
                            state.wheelModelRearLeftRot = wheelModelRearLeftRot;
                            state.wheelModelRearRightRot = wheelModelRearRightRot;

                            bufferedState[0] = state;

                            // Update used slot count, however never exceed the buffer size
                            // Slots aren't actually freed so this just makes sure the buffer is
                            // filled up and that uninitalized slots aren't used.
                            timestampCount = Mathf.Min(timestampCount + 1, bufferedState.Length);

                            // Check if states are in order, if it is inconsistent you could reshuffel or
                            // drop the out-of-order state. Nothing is done here
                            for (int i = 0; i < timestampCount - 1; i++) {
                                if (bufferedState[i].timestamp < bufferedState[i + 1].timestamp)
                                    Debug.LogUtil.Log("State inconsistent");
                            }
                        }
                    }
                }
            }
        }

        public void Update()
        {
            if(IsCompetitorAvailable)
            {
                FindTarget();

                if(target == null) {
                    LogUtil.Log("NetworkVehicleProxy:: target is NULL!");
                    return;
                }

                // This is the target playback time of the rigid body
                double interpolationTime = Network.time - interpolationBackTime;

                // Use interpolation if the target playback time is present in the buffer
                if (bufferedState[0].timestamp > interpolationTime) {

                    // Go through buffer and find correct state to play back
                    for (int i = 0; i < timestampCount; i++) {
                        if (bufferedState[i].timestamp <= interpolationTime || i == timestampCount - 1) {

                            // The state one slot newer (<100ms) than the best playback state
                            State rhs = bufferedState[Mathf.Max(i - 1, 0)];

                            // The best playback state (closest to 100 ms old (default time))
                            State lhs = bufferedState[i];

                            // Use the time between the two slots to determine if interpolation is necessary
                            double length = rhs.timestamp - lhs.timestamp;
                            float t = 0.0f;

                            // As the time difference gets closer to 100 ms t gets closer to 1 in
                            // which case rhs is only used
                            // Example:
                            // Time is 10.000, so sampleTime is 9.900
                            // lhs.time is 9.910 rhs.time is 9.980 length is 0.070
                            // t is 9.900 - 9.910 / 0.070 = 0.14. So it uses 14% of rhs, 86% of lhs
                            if (length > 0.0001) {
                                t = (float)((interpolationTime - lhs.timestamp) / length);
                            }

                            //      Debug.Log(t);
                            // if t=0 => lhs is used directly
                            target.transform.localPosition = Vector3.Lerp(lhs.pos, rhs.pos, t);
                            target.transform.localRotation = Quaternion.Slerp(lhs.rot, rhs.rot, t);

                            FindTargetObjects();

                            if(modelTarget) {
                                modelTarget.localPosition = Vector3.Lerp(lhs.modelPos, rhs.modelPos, t);
                                modelTarget.localRotation = Quaternion.Slerp(lhs.modelRot, rhs.modelRot, t);
                            }

                            if(wheelControlFrontLeftTarget) {
                                wheelControlFrontLeftTarget.localPosition = Vector3.Lerp(lhs.wheelControlFrontLeftPos, rhs.wheelControlFrontLeftPos, t);
                                wheelControlFrontLeftTarget.localRotation = Quaternion.Slerp(lhs.wheelControlFrontLeftRot, rhs.wheelControlFrontLeftRot, t);
                            }
                            if(wheelControlFrontRightTarget) {
                                wheelControlFrontRightTarget.localPosition = Vector3.Lerp(lhs.wheelControlFrontRightPos, rhs.wheelControlFrontRightPos, t);
                                wheelControlFrontRightTarget.localRotation = Quaternion.Slerp(lhs.wheelControlFrontRightRot, rhs.wheelControlFrontRightRot, t);
                            }
                            if(wheelControlRearLeftTarget) {
                                wheelControlRearLeftTarget.localPosition = Vector3.Lerp(lhs.wheelControlRearLeftPos, rhs.wheelControlRearLeftPos, t);
                                wheelControlRearLeftTarget.localRotation = Quaternion.Slerp(lhs.wheelControlRearLeftRot, rhs.wheelControlRearLeftRot, t);
                            }
                            if(wheelControlRearRightTarget) {
                                wheelControlRearRightTarget.localPosition = Vector3.Lerp(lhs.wheelControlRearRightPos, rhs.wheelControlRearRightPos, t);
                                wheelControlRearRightTarget.localRotation = Quaternion.Slerp(lhs.wheelControlRearRightRot, rhs.wheelControlRearRightRot, t);
                            }

                            if(wheelModelFrontLeftTarget) {
                                wheelModelFrontLeftTarget.localPosition = Vector3.Lerp(lhs.wheelModelFrontLeftPos, rhs.wheelModelFrontLeftPos, t);
                                wheelModelFrontLeftTarget.localRotation = Quaternion.Slerp(lhs.wheelModelFrontLeftRot, rhs.wheelModelFrontLeftRot, t);
                            }
                            if(wheelModelFrontRightTarget) {
                                wheelModelFrontRightTarget.localPosition = Vector3.Lerp(lhs.wheelModelFrontRightPos, rhs.wheelModelFrontRightPos, t);
                                wheelModelFrontRightTarget.localRotation = Quaternion.Slerp(lhs.wheelModelFrontRightRot, rhs.wheelModelFrontRightRot, t);
                            }
                            if(wheelModelRearLeftTarget) {
                                wheelModelRearLeftTarget.localPosition = Vector3.Lerp(lhs.wheelModelRearLeftPos, rhs.wheelModelRearLeftPos, t);
                                wheelModelRearLeftTarget.localRotation = Quaternion.Slerp(lhs.wheelModelRearLeftRot, rhs.wheelModelRearLeftRot, t);
                            }
                            if(wheelModelRearRightTarget) {
                                wheelModelRearRightTarget.localPosition = Vector3.Lerp(lhs.wheelModelRearRightPos, rhs.wheelModelRearRightPos, t);
                                wheelModelRearRightTarget.localRotation = Quaternion.Slerp(lhs.wheelModelRearRightRot, rhs.wheelModelRearRightRot, t);
                            }

                            return;
                        }
                    }
                }

                // Use extrapolation
                else {
                    State latest = bufferedState[0];

                    float extrapolationLength = (float)(interpolationTime - latest.timestamp);

                    // Don't extrapolation for more than 500 ms, you would need to do that carefully
                    if (extrapolationLength < extrapolationLimit) {
                        float axisLength = extrapolationLength * latest.angularVelocity.magnitude * Mathf.Rad2Deg;
                        Quaternion angularRotation = Quaternion.AngleAxis(axisLength, latest.angularVelocity);

                        if(target != null) {
                            target.transform.position = latest.pos + latest.velocity * extrapolationLength;
                            target.transform.rotation = angularRotation * latest.rot;

                            //rigidbody.velocity = latest.velocity;
                            //rigidbody.angularVelocity = latest.angularVelocity;
                        }
                    }
                }
            }
        }
        */
    }
}