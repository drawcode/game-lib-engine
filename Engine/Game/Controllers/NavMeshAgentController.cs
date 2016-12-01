using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Engine.Game.Controllers {

    public class NavMeshAgentController : GameObjectBehavior {
        public UnityEngine.AI.NavMeshAgent agent;
        public Vector3 nextDestination;

        // Use this for initialization
        private void Start() {
            agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            nextDestination = transform.position;
            NavigateToDestination();
        }

        public void NavigateToDestination() {
            if (agent != null) {
                agent.destination = nextDestination;
            }
        }

        // Update is called once per frame
        private void Update() {
            if (agent != null) {
                if (agent.remainingDistance <= 5 || agent.isPathStale) {

                    // get next destination
                    nextDestination = new Vector3(UnityEngine.Random.Range(0, 50), 0, UnityEngine.Random.Range(0, 50));
                    NavigateToDestination();
                }
            }
        }
    }
}