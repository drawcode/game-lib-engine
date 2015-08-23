using System;
using System.Collections.Generic;
using Engine.Utility;
using UnityEngine;

public static class NavMeshExtentions {

    public static void On(this NavMeshAgent navMeshAgent) {
        if (navMeshAgent == null) {
            return;
        }
        
        //navMeshAgent.updatePosition = true;
        //navMeshAgent.updateRotation = true;        
    }

    public static void Off(this NavMeshAgent navMeshAgent) {
        if (navMeshAgent == null) {
            return;
        }

        //navMeshAgent.updatePosition = false;
        //navMeshAgent.updateRotation = false;
    }

    public static void StopAgent(this NavMeshAgent navMeshAgent) {
        if (navMeshAgent == null) {
            return;
        }
        if (navMeshAgent != null) {
            if (navMeshAgent.enabled) {
                //navMeshAgent.Off();
                navMeshAgent.Stop();
                navMeshAgent.enabled = false;
            }
        }
    }

    public static void StartAgent(this NavMeshAgent navMeshAgent) {
        if (navMeshAgent == null) {
            return;
        }
        if (navMeshAgent != null) {
            if (!navMeshAgent.enabled) {
                navMeshAgent.enabled = true;
                //navMeshAgent.Warp(navMeshAgent.gameObject.transform.position);
                //navMeshAgent.On();
                navMeshAgent.Resume();
            }
        }
    }
}

