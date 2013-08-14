using System.Collections;
using Engine;
using Engine.Data;
using Engine.Networking;
using Engine.Utility;
using UnityEngine;

namespace Engine.Game.Controllers {

    [AddComponentMenu("Third Person Player/Third Person Player Animation")]
    public class ThirdPersonSimpleAnimation : BaseEngineBehavior {
        public float runSpeedScale = 1.0f;
        public float walkSpeedScale = 1.0f;
        public Transform torso;
        public GameObject actor;

        public bool isRunning = false;

        private ThirdPersonController thirdPersonController;
        private NavMeshAgent navAgent;

        private void Awake() {
            Init();
        }

        public void Init() {
            thirdPersonController = GetComponent<ThirdPersonController>();
            navAgent = GetComponent<NavMeshAgent>();
            Reset();
        }

        public void Reset() {
            if (actor != null) {
                if (actor.animation != null) {

                    // By default loop all animations
                    actor.animation.wrapMode = WrapMode.Loop;

                    // We are in full control here - don't let any other animations play when we start
                    actor.animation.Stop();
                    actor.animation.Play("idle");
                    isRunning = true;
                }
            }
        }

        private void Update() {
            if (isRunning) {
                var currentSpeed = thirdPersonController.GetSpeed();

                //Debug.Log("currentSpeed:" + currentSpeed);
                Debug.Log("navAgent:" + navAgent);

                if (navAgent != null) {
                    if (navAgent.enabled) {
                        currentSpeed = navAgent.velocity.magnitude;
                    }
                }

                Debug.Log("currentSpeed:" + currentSpeed);
                Debug.Log("currentSpeed:" + thirdPersonController.walkSpeed);

                // Fade in run
                if (currentSpeed > thirdPersonController.walkSpeed) {
                    actor.animation.CrossFade("run");

                    // We fade out jumpland quick otherwise we get sliding feet
                    actor.animation.Blend("jump", 0);
                    SendMessage("SyncAnimation", "run", SendMessageOptions.DontRequireReceiver);
                }

                // Fade in walk
                else if (currentSpeed > 0.1) {
                    actor.animation.CrossFade("walk");

                    // We fade out jumpland realy quick otherwise we get sliding feet
                    actor.animation.Blend("jump", 0);
                    SendMessage("SyncAnimation", "walk", SendMessageOptions.DontRequireReceiver);
                }

                // Fade out walk and run
                else {
                    actor.animation.CrossFade("idle");
                    SendMessage("SyncAnimation", "idle", SendMessageOptions.DontRequireReceiver);
                }

                actor.animation["run"].normalizedSpeed = runSpeedScale;
                actor.animation["walk"].normalizedSpeed = walkSpeedScale;

                if (thirdPersonController.IsJumping()) {
                    if (thirdPersonController.IsCapeFlying()) {

                        //actor.animation.CrossFade("jetpackjump", 0.2f);
                        //SendMessage("SyncAnimation", "jetpackjump", SendMessageOptions.DontRequireReceiver);
                        actor.animation.CrossFade("jump", 0.2f);
                        SendMessage("SyncAnimation", "jump", SendMessageOptions.DontRequireReceiver);
                    }
                    else if (thirdPersonController.HasJumpReachedApex()) {

                        //actor.animation.CrossFade("jumpfall", 0.2f);
                        //SendMessage("SyncAnimation", "jumpfall", SendMessageOptions.DontRequireReceiver);
                        actor.animation.CrossFade("jump", 0.2f);
                        SendMessage("SyncAnimation", "jump", SendMessageOptions.DontRequireReceiver);
                    }
                    else {
                        actor.animation.CrossFade("jump", 0.2f);
                        SendMessage("SyncAnimation", "jump", SendMessageOptions.DontRequireReceiver);
                    }
                }

                // We fell down somewhere
                else if (!thirdPersonController.IsGroundedWithTimeout()) {

                    //actor.animation.CrossFade("ledgefall", 0.2f);
                    //SendMessage("SyncAnimation", "ledgefall", SendMessageOptions.DontRequireReceiver);
                }

                // We are not falling down anymore
                else {

                    //actor.animation.Blend("ledgefall", 0.0f, 0.2f);
                }
            }
        }

        public void DidLand() {

            //actor.animation.Play("jumpland");
            //SendMessage("SyncAnimation", "jumpland", SendMessageOptions.DontRequireReceiver);
            actor.animation.Play("jump");
            SendMessage("SyncAnimation", "jumpland", SendMessageOptions.DontRequireReceiver);
        }

        public void DidAttack() {
            Debug.Log("DidAttack:");
            float currentSpeed = thirdPersonController.GetSpeed();

            // Fade in run
            if (currentSpeed > thirdPersonController.walkSpeed) {
                actor.animation.CrossFade("attack_far");
                SendMessage("SyncAnimation", "run", SendMessageOptions.DontRequireReceiver);
            }

            // Fade in walk
            else if (currentSpeed > 0.1) {
                actor.animation.CrossFade("attack_far");
                SendMessage("SyncAnimation", "walk", SendMessageOptions.DontRequireReceiver);
            }

            // Fade out walk and run
            else {
                actor.animation.Play("attack_far");
                SendMessage("SyncAnimation", "idle", SendMessageOptions.DontRequireReceiver);
            }
        }

        public void DidSkill() {
            Debug.Log("DidSkill:");
            float currentSpeed = thirdPersonController.GetSpeed();

            // Fade in run
            if (currentSpeed > thirdPersonController.walkSpeed) {
                actor.animation.CrossFade("skill");
                SendMessage("SyncAnimation", "run", SendMessageOptions.DontRequireReceiver);
            }

            // Fade in walk
            else if (currentSpeed > 0.1) {
                actor.animation.CrossFade("skill");
                SendMessage("SyncAnimation", "walk", SendMessageOptions.DontRequireReceiver);
            }

            // Fade out walk and run
            else {
                actor.animation.Play("skill");
                SendMessage("SyncAnimation", "idle", SendMessageOptions.DontRequireReceiver);
            }

            //SendMessage("SyncAnimation", "run", SendMessageOptions.DontRequireReceiver);
        }

        public void DidButtStomp() {

            //actor.animation.CrossFade("buttstomp", 0.1f);
            //SendMessage("SyncAnimation", "buttstomp", SendMessageOptions.DontRequireReceiver);
            //actor.animation.CrossFadeQueued("jumpland", 0.2f);
        }

        public void ApplyDamage() {
            actor.animation.CrossFade("hit", 0.1f);
            SendMessage("SyncAnimation", "hit", SendMessageOptions.DontRequireReceiver);
        }

        public void DidWallJump() {

            // Wall jump animation is played without fade.
            // We are turning the character controller 180 degrees around when doing a wall jump so the animation accounts for that.
            // But we really have to make sure that the animation is in full control so
            // that we don't do weird blends between 180 degree apart rotations
            actor.animation.Play("walljump");
            SendMessage("SyncAnimation", "walljump");
        }
    }
}