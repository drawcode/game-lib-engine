using System;
using System.Collections.Generic;
using Engine.Utility;


public static class NavMeshExtentions {

    public static void On(
        this UnityEngine.AI.NavMeshAgent navMeshAgent) {

        if(navMeshAgent == null) {
            return;
        }

        //navMeshAgent.updatePosition = true;
        //navMeshAgent.updateRotation = true;        
    }

    public static void Off(
        this UnityEngine.AI.NavMeshAgent navMeshAgent) {

        if(navMeshAgent == null) {
            return;
        }

        //navMeshAgent.updatePosition = false;
        //navMeshAgent.updateRotation = false;
    }

    public static void StopAgent(
        this UnityEngine.AI.NavMeshAgent navMeshAgent) {

        if(navMeshAgent == null) {
            return;
        }

        if(navMeshAgent != null) {

            if(navMeshAgent.enabled) {

                //navMeshAgent.Off();
                navMeshAgent.isStopped = true;
                navMeshAgent.enabled = false;
            }
        }
    }

    public static void StartAgent(
        this UnityEngine.AI.NavMeshAgent navMeshAgent) {

        if(navMeshAgent == null) {
            return;
        }

        if(navMeshAgent != null) {

            if(!navMeshAgent.enabled) {

                navMeshAgent.enabled = true;
                //navMeshAgent.Warp(navMeshAgent.gameObject.transform.position);
                //navMeshAgent.On();
                navMeshAgent.isStopped = false;
            }
        }
    }
}