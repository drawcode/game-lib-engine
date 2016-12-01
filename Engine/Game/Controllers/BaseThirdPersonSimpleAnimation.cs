using System.Collections;
using Engine;
using Engine.Data;
using Engine.Networking;
using Engine.Utility;
using UnityEngine;

namespace Engine.Game.Controllers {

    [AddComponentMenu("Third Person Player/Third Person Player Animation")]
    public class BaseThirdPersonSimpleAnimation : BaseEngineBehavior {
        public float runSpeedScale = 1.0f;
        public float walkSpeedScale = 1.0f;
        public Transform torso;
        public GameObject actor;

        public bool isRunning = false;

        private BaseThirdPersonController thirdPersonController;
        private UnityEngine.AI.NavMeshAgent navAgent;

        private void Awake() {
            Init();
        }

        public void Init() {
            thirdPersonController = GetComponent<BaseThirdPersonController>();
            navAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            Reset();
        }

        public void Reset() {
            if (actor != null) {
                
                
                if(actor == null) {
                    return;
                }
                
                Animation anim = actor.GetComponent<Animation>();

                if (anim != null) {

                    // By default loop all animations
                    anim.wrapMode = WrapMode.Loop;

                    // We are in full control here - don't let any other animations play when we start
                    anim.Stop();
                    anim.Play("idle");
                    isRunning = true;
                }
            }
        }

        Animation ani;

        private void Update() {
            if (isRunning) {
                
                if(actor == null) {
                    return;
                }
                
                ani = actor.GetComponent<Animation>();

                var currentSpeed = thirdPersonController.GetSpeed();

                //LogUtil.Log("currentSpeed:" + currentSpeed);
                LogUtil.Log("navAgent:" + navAgent);

                if (navAgent != null) {
                    if (navAgent.enabled) {
                        currentSpeed = navAgent.velocity.magnitude;
                    }
                }

                LogUtil.Log("currentSpeed:" + currentSpeed);
                LogUtil.Log("currentSpeed:" + thirdPersonController.walkSpeed);

                // Fade in run
                if (currentSpeed > thirdPersonController.walkSpeed) {
                    ani.CrossFade("run");

                    // We fade out jumpland quick otherwise we get sliding feet
                    ani.Blend("jump", 0);
                    SendMessage("SyncAnimation", "run", SendMessageOptions.DontRequireReceiver);
                }

                // Fade in walk
                else if (currentSpeed > 0.1) {
                    ani.CrossFade("walk");

                    // We fade out jumpland realy quick otherwise we get sliding feet
                    ani.Blend("jump", 0);
                    SendMessage("SyncAnimation", "walk", SendMessageOptions.DontRequireReceiver);
                }

                // Fade out walk and run
                else {
                    ani.CrossFade("idle");
                    SendMessage("SyncAnimation", "idle", SendMessageOptions.DontRequireReceiver);
                }

                ani["run"].normalizedSpeed = runSpeedScale;
                ani["walk"].normalizedSpeed = walkSpeedScale;

                if (thirdPersonController.IsJumping()) {
                    if (thirdPersonController.IsCapeFlying()) {

                        //actor.animation.CrossFade("jetpackjump", 0.2f);
                        //SendMessage("SyncAnimation", "jetpackjump", SendMessageOptions.DontRequireReceiver);
                        ani.CrossFade("jump", 0.2f);
                        SendMessage("SyncAnimation", "jump", SendMessageOptions.DontRequireReceiver);
                    }
                    else if (thirdPersonController.HasJumpReachedApex()) {

                        //actor.animation.CrossFade("jumpfall", 0.2f);
                        //SendMessage("SyncAnimation", "jumpfall", SendMessageOptions.DontRequireReceiver);
                        ani.CrossFade("jump", 0.2f);
                        SendMessage("SyncAnimation", "jump", SendMessageOptions.DontRequireReceiver);
                    }
                    else {
                        ani.CrossFade("jump", 0.2f);
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
            if(actor == null) {
                return;
            }
            
            Animation anim = actor.GetComponent<Animation>();

            //actor.animation.Play("jumpland");
            //SendMessage("SyncAnimation", "jumpland", SendMessageOptions.DontRequireReceiver);
            anim.Play("jump");
            SendMessage("SyncAnimation", "jumpland", SendMessageOptions.DontRequireReceiver);
        }

        public void DidAttack() {
            LogUtil.Log("DidAttack:");
            float currentSpeed = thirdPersonController.GetSpeed();
            
            if(actor == null) {
                return;
            }
            
            Animation anim = actor.GetComponent<Animation>();

            // Fade in run
            if (currentSpeed > thirdPersonController.walkSpeed) {
                anim.CrossFade("attack_far");
                SendMessage("SyncAnimation", "run", SendMessageOptions.DontRequireReceiver);
            }

            // Fade in walk
            else if (currentSpeed > 0.1) {
                anim.CrossFade("attack_far");
                SendMessage("SyncAnimation", "walk", SendMessageOptions.DontRequireReceiver);
            }

            // Fade out walk and run
            else {
                anim.Play("attack_far");
                SendMessage("SyncAnimation", "idle", SendMessageOptions.DontRequireReceiver);
            }
        }

        public void DidSkill() {
            LogUtil.Log("DidSkill:");
            float currentSpeed = thirdPersonController.GetSpeed();

            if(actor == null) {
                return;
            }

            Animation anim = actor.GetComponent<Animation>();

            // Fade in run
            if (currentSpeed > thirdPersonController.walkSpeed) {
                anim.CrossFade("skill");
                SendMessage("SyncAnimation", "run", SendMessageOptions.DontRequireReceiver);
            }

            // Fade in walk
            else if (currentSpeed > 0.1) {
                anim.CrossFade("skill");
                SendMessage("SyncAnimation", "walk", SendMessageOptions.DontRequireReceiver);
            }

            // Fade out walk and run
            else {
                anim.Play("skill");
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
            
            if(actor == null) {
                return;
            }
            
            Animation anim = actor.GetComponent<Animation>();

            anim.CrossFade("hit", 0.1f);
            SendMessage("SyncAnimation", "hit", SendMessageOptions.DontRequireReceiver);
        }

        public void DidWallJump() {
            
            if(actor == null) {
                return;
            }
            
            Animation anim = actor.GetComponent<Animation>();

            // Wall jump animation is played without fade.
            // We are turning the character controller 180 degrees around when doing a wall jump so the animation accounts for that.
            // But we really have to make sure that the animation is in full control so
            // that we don't do weird blends between 180 degree apart rotations
            anim.Play("walljump");
            SendMessage("SyncAnimation", "walljump");
        }
    }
}