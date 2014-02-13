using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameVehicleEventArgs {
    public string name;
    public Vector3 position;
    public Quaternion rotation;
    public string currentWaypointName;
    public int currentWaypointIndex;
    //public string nextWaypointName;
    //public int nextWaypointIndex;
    public string tag;
}
