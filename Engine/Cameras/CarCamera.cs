using System.Collections;
using UnityEngine;

namespace Engine.Cameras
{
    public class CarCamera : GameObjectBehavior
    {

        public Transform target = null;
        public float height = 1f;
        public float positionDamping = 3f;
        public float velocityDamping = 3f;
        public float distance = 4f;
        public LayerMask ignoreLayers = -1;

        private RaycastHit hit = new RaycastHit();

        private Vector3 prevVelocity = Vector3.zero;
        private LayerMask raycastLayers = -1;

        private Vector3 currentVelocity = Vector3.zero;

        Rigidbody rb;

        private void Start()
        {

            raycastLayers = ~ignoreLayers;
            rb = target.root.GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {

            currentVelocity = Vector3.Lerp(
                prevVelocity, rb.velocity, velocityDamping * Time.deltaTime);

            currentVelocity.y = 0;
            prevVelocity = currentVelocity;
        }

        private void LateUpdate()
        {

            float speedFactor = Mathf.Clamp01(rb.velocity.magnitude / 70.0f);

            camera.fieldOfView = Mathf.Lerp(55, 72, speedFactor);

            float currentDistance = Mathf.Lerp(7.5f, 6.5f, speedFactor);

            currentVelocity = currentVelocity.normalized;

            Vector3 newTargetPosition = target.position + Vector3.up * height;
            Vector3 newPosition = newTargetPosition - (currentVelocity * currentDistance);
            newPosition.y = newTargetPosition.y;

            Vector3 targetDirection = newPosition - newTargetPosition;

            if (Physics.Raycast(
                newTargetPosition, targetDirection, out hit, currentDistance, raycastLayers))
            {

                newPosition = hit.point;
            }

            transform.position = newPosition;
            transform.LookAt(newTargetPosition);
        }
    }
}