using System.Collections;
using Engine;
using Engine.Data;
using Engine.Networking;
using Engine.Utility;
using UnityEngine;

/*
namespace Engine.Game.Controllers {

    public class BaseThirdPersonSimpleAnimation2 : BaseEngineBehavior {
        public float runSpeedScale = 1.0f;
        public float walkSpeedScale = 1.0f;
        public Transform torso;
        public GameObject actor;

        public bool isRunning = false;

        private void Awake() {
            Init();
        }

        public void Init() {
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
                BaseThirdPersonController marioController = GetComponent<BaseThirdPersonController>();
                var currentSpeed = marioController.GetSpeed();

                // Fade in run
                if (currentSpeed > marioController.walkSpeed) {
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

                if (marioController.IsJumping()) {
                    if (marioController.IsCapeFlying()) {

                        //actor.animation.CrossFade("jetpackjump", 0.2f);
                        //SendMessage("SyncAnimation", "jetpackjump", SendMessageOptions.DontRequireReceiver);
                        actor.animation.CrossFade("jump", 0.2f);
                        SendMessage("SyncAnimation", "jump", SendMessageOptions.DontRequireReceiver);
                    }
                    else if (marioController.HasJumpReachedApex()) {

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
                else if (!marioController.IsGroundedWithTimeout()) {

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

        public void DidPunch() {

            //actor.animation.CrossFadeQueued("punch", 0.3f, QueueMode.PlayNow);
        }

        public void DidButtStomp() {

            //actor.animation.CrossFade("buttstomp", 0.1f);
            //SendMessage("SyncAnimation", "buttstomp", SendMessageOptions.DontRequireReceiver);
            //actor.animation.CrossFadeQueued("jumpland", 0.2f);
        }

        public void ApplyDamage() {

            //actor.animation.CrossFade("gothit", 0.1f);
            //SendMessage("SyncAnimation", "gothit", SendMessageOptions.DontRequireReceiver);
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
*/