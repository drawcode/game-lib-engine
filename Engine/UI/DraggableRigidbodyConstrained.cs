using System;
using System.Collections;
using UnityEngine;

namespace Engine.UI {

    public class DraggableRigidbodyConstrained : GameObjectBehavior {
        public float spring = 50.0f;
        public float damper = 5.0f;
        public float drag = 5.0f;
        public float angularDrag = 5.0f;
        public float distance = 0.5f;
        public bool attachToCenterOfMass = true;
        private Vector3 currentPosition;

        //Vector3 constrainedPosition;

        public float boundaryXLeft = 100f;
        public float boundaryXRight = -300f;
        public float boundaryYTop = -50f;
        public float boundaryYBottom = 7.5f;

        public Camera cameraScrollable;

        private SpringJoint springJoint;

        private void Update() {
            CheckBoundaries();

            // Make sure the user pressed the mouse down
            if (!Input.GetMouseButtonDown(0)) {	 // will work on device and desktop
                return;
            }

            Camera mainCamera = FindCamera();

            // We need to actually hit an object

            RaycastHit hit;

            if (!Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hit, 100))
                return;

            // We need to hit a rigidbody that is not kinematic
            if (!hit.rigidbody || hit.rigidbody.isKinematic)
                return;

            if (!springJoint) {
                LogUtil.Log("Adding rigidbody and joint for draggable");
                var go = new GameObject("Rigidbody dragger");
                var body = go.AddComponent("Rigidbody") as Rigidbody;
                springJoint = go.AddComponent("SpringJoint") as SpringJoint;
                body.isKinematic = true;
            }

            springJoint.transform.position = hit.point;

            if (attachToCenterOfMass) {
                var anchor = transform.TransformDirection(hit.rigidbody.centerOfMass) + hit.rigidbody.transform.position;
                anchor = springJoint.transform.InverseTransformPoint(anchor);
                springJoint.anchor = anchor;
            }
            else {
                springJoint.anchor = Vector3.zero;
            }

            springJoint.spring = spring;
            springJoint.damper = damper;
            springJoint.maxDistance = distance;
            springJoint.connectedBody = hit.rigidbody;

            StartCoroutine("DragObject", hit.distance);
        }

        public IEnumerator DragObject(float distance) {
            var oldDrag = springJoint.connectedBody.drag;
            var oldAngularDrag = springJoint.connectedBody.angularDrag;
            springJoint.connectedBody.drag = drag;
            springJoint.connectedBody.angularDrag = angularDrag;
            var mainCamera = FindCamera();
            while (Input.GetMouseButton(0)) {
                var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                springJoint.transform.position = ray.GetPoint(distance);

                CheckBoundaries();

                yield return new WaitForSeconds(.1f);
            }

            if (springJoint.connectedBody) {
                springJoint.connectedBody.drag = oldDrag;
                springJoint.connectedBody.angularDrag = oldAngularDrag;
                springJoint.connectedBody = null;
            }

            CheckBoundaries();
        }

        public void CheckBoundaries() {

            // Constrain to boundaries, if past stop velocity.

            currentPosition = gameObject.transform.position;
            Vector3 angularVelocityBody = gameObject.rigidbody.angularVelocity;

            //LogUtil.Log("currentPosition:" + currentPosition);

            if (currentPosition.x > boundaryXLeft) {
                currentPosition.x = boundaryXLeft;
                angularVelocityBody.x = 0;
            }
            if (currentPosition.x < boundaryXRight) {
                currentPosition.x = boundaryXRight;
                angularVelocityBody.x = 0;
            }
            if (currentPosition.y < boundaryYTop) {
                currentPosition.y = boundaryYTop;
                angularVelocityBody.y = 0;
            }
            if (currentPosition.y > boundaryYBottom) {
                currentPosition.y = boundaryYBottom;
                angularVelocityBody.y = 0;
            }

            gameObject.rigidbody.angularVelocity = angularVelocityBody;
            gameObject.transform.position = currentPosition;
        }

        public Camera FindCamera() {
            if (cameraScrollable != null)
                return cameraScrollable;
            else
                return Camera.main;
        }
    }
}