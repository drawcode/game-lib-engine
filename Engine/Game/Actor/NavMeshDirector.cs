using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Engine.Game.Actor {

    public class NavMeshDirector : MonoBehaviour {
        public NavMeshAgent agentTester;
        public Vector3 nextDestination;

        // Use this for initialization
        private void Start() {
            nextDestination = transform.position;
            NavigateToDestination();
        }

        public void NavigateToDestination() {
            if (agentTester != null) {
                agentTester.destination = nextDestination;
            }
        }

        // Update is called once per frame
        private void Update() {
            if (agentTester != null) {
                if (agentTester.remainingDistance == 0) {

                    // get next destination
                    nextDestination = new Vector3(UnityEngine.Random.Range(0, 20), 0, UnityEngine.Random.Range(0, 20));
                    NavigateToDestination();
                }
            }
        }
    }
}