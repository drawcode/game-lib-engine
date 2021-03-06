using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Engine.Game.Actor {
    public class NavMeshAgentFollowController : GameObjectBehavior {
        public UnityEngine.AI.NavMeshAgent agent;
        public Transform targetFollow;

        // Use this for initialization
        private void Start() {
            agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            NavigateToDestination();
        }

        public void NavigateToDestination() {
            if (agent != null) {
                if (targetFollow != null) {
                    float distance = Vector3.Distance(agent.destination, targetFollow.position);

                    if (distance < 5) {
                        agent.isStopped = true;
                    }
                    else {
                        agent.destination = targetFollow.position;
                    }
                }
            }
        }

        // Update is called once per frame
        private void Update() {

            if (!GameConfigs.isGameRunning) {

                if (agent != null) {
                    agent.isStopped = true;
                }
                return;
            }

            if (agent != null) {
                if (agent.remainingDistance == 0 || agent.isPathStale) {
                    NavigateToDestination();
                }
            }
        }
    }
}