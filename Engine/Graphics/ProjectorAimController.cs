using System;
using Engine;
using UnityEngine;

namespace Engine.Graphics {

    public class ProjectorAimController : MonoBehaviour {
        public string sourceName;
        public Transform target;
        public float targetOffset;

        private Vector3 offset;
        private Vector3 angles;

        private void Start() {

            //Disconnect from any parent
            transform.parent = null;

            if (!string.IsNullOrEmpty(sourceName)) {

                //Find the source object
                var sourceGO = GameObject.Find(sourceName);
                if (!sourceGO) {
                    Debug.Log("Unable to find projector aim source");
                    enabled = false;
                    return;
                }

                var source = sourceGO.transform;

                //Calculate initial rotation angles from source and target
                var sourceDir = (source.position - target.position).normalized;
                angles.x = Vector3.Angle(sourceDir, target.up);

                var dirProj = sourceDir - (Vector3.Dot(sourceDir, target.up) * target.up);
                angles.z = -Vector3.Angle(dirProj, target.forward);

                offset = sourceDir * targetOffset;
            }
            else {
                offset = transform.position - target.position;
                angles = transform.eulerAngles;
            }
        }

        private void LateUpdate() {
            if (!target)
                return;

            var transform = this.transform;

            transform.position = target.position + offset;
            transform.eulerAngles = angles;
            transform.RotateAround(transform.forward, -target.localEulerAngles.y * Mathf.Deg2Rad);
        }
    }
}