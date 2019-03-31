using System;
using System.Collections;
using UnityEngine;

namespace Engine.UI {

    public class DraggableRigidbody : GameObjectBehavior {
        public float spring = 50.0f;
        public float damper = 5.0f;
        public float drag = 10.0f;
        public float angularDrag = 5.0f;
        public float distance = 0.2f;
        public bool attachToCenterOfMass = false;

        private SpringJoint springJoint;

        private void Update() {

            // Make sure the user pressed the mouse down
            if(!Input.GetMouseButtonDown(0)) {

                //LogUtil.Log("No input");
                return;
            }

            LogUtil.Log("Has input");

            Camera mainCamera = FindCamera();

            // We need to actually hit an object

            RaycastHit hit;

            if(!Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hit, 100))
                return;

            // We need to hit a rigidbody that is not kinematic
            if(!hit.rigidbody || hit.rigidbody.isKinematic)
                return;

            if(!springJoint) {
                LogUtil.Log("Adding rigidbody and joint for draggable");
                var go = new GameObject("Rigidbody dragger");
                var body = go.AddComponent<Rigidbody>();
                springJoint = go.AddComponent<SpringJoint>();
                body.isKinematic = true;
            }

            springJoint.transform.position = hit.point;

            if(attachToCenterOfMass) {
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
            while(Input.GetMouseButton(0)) {
                var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                springJoint.transform.position = ray.GetPoint(distance);
                yield return new WaitForSeconds(.1f);
            }

            if(springJoint.connectedBody) {
                springJoint.connectedBody.drag = oldDrag;
                springJoint.connectedBody.angularDrag = oldAngularDrag;
                springJoint.connectedBody = null;
            }
        }

        public Camera FindCamera() {
            if(camera)
                return camera;
            else
                return Camera.main;
        }
    }
}