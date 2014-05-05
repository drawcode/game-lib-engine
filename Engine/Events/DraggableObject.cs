using System;
using Engine;
using UnityEngine;

namespace Engine.Events {

    [RequireComponent(typeof(Rigidbody))]
    public class DraggableObject : GameObjectBehavior {
        public int normalCollisionCount = 1;
        public float moveLimit = .5f;
        public float collisionMoveFactor = .01f;
        public float addHeightWhenClicked = 0.0f;
        public bool freezeRotationOnDrag = true;
        public Camera cam;

        private bool canMove = false;
        private float yPos;
        private bool gravitySetting;
        private bool freezeRotationSetting;

        //private float sqrMoveLimit;
        private int collisionCount = 0;

        //private Transform camTransform;

        private void Start() {
            if (!cam) {
                cam = Camera.main;
            }
            if (!cam) {
                LogUtil.LogError("Can't find camera tagged MainCamera");
                return;
            }

            //camTransform = cam.transform;
            //sqrMoveLimit = moveLimit * moveLimit;   // Since we're using sqrMagnitude, which is faster than magnitude
        }

        private void OnMouseDown() {
            canMove = true;
            gameObject.transform.Translate(Vector3.up * addHeightWhenClicked);
            gravitySetting = gameObject.rigidbody.useGravity;
            freezeRotationSetting = gameObject.rigidbody.freezeRotation;
            gameObject.rigidbody.useGravity = false;
            gameObject.rigidbody.freezeRotation = freezeRotationOnDrag;
            yPos = gameObject.transform.position.y;
        }

        private void OnMouseUp() {
            canMove = false;
            gameObject.rigidbody.useGravity = gravitySetting;
            gameObject.rigidbody.freezeRotation = freezeRotationSetting;
            if (!gameObject.rigidbody.useGravity) {
                float _y = yPos - addHeightWhenClicked;

                LogUtil.Log(_y);

                //gameObject.transform.position.y = _y;
            }
        }

        private void OnCollisionEnter() {
            collisionCount++;
        }

        private void OnCollisionExit() {
            collisionCount--;
        }

        private void FixedUpdate() {
            if (!canMove) return;
            /*
            gameObject.rigidbody.velocity = Vector3.zero;
            gameObject.rigidbody.angularVelocity = Vector3.zero;

            //gameObject.transform.position.y = yPos;
            var mousePos = Input.mousePosition;
            var move = cam.ScreenToWorldPoint(Vector3(mousePos.x, mousePos.y, camTransform.position.y - gameObject.transform.position.y)) - gameObject.transform.position;
            move.y = 0.0;
            if (collisionCount > normalCollisionCount) {
                move = move.normalized*collisionMoveFactor;
            }
            else if (move.sqrMagnitude > sqrMoveLimit) {
                move = move.normalized*moveLimit;
            }
            */

            //gameObject.transform.MovePosition(gameObject.transform.position + move);
        }
    }
}