using System;
using Engine.Utility;
using UnityEngine;

public class PhysicsUtil {

    public static Vector3 PlotTrajectoryAtTime (Vector3 start, Vector3 startVelocity, float time) {
	    return start + startVelocity*time + Physics.gravity*time*time*0.5f;
	}

	public static void PlotTrajectory (Vector3 start, Vector3 startVelocity, float timestep, float maxTime) {
	    Vector3 prev = start;
	    for (int i=1;;i++) {
	        float t = timestep*i;
	        if (t > maxTime) break;
	        Vector3 pos = PlotTrajectoryAtTime (start, startVelocity, t);
	        if (Physics.Linecast (prev,pos)) break;
	        Debug.DrawLine (prev,pos,Color.red);
	        prev = pos;
	    }
	}
	
}
