using System;
using Engine.Events;
using Engine.Game.Actor;
using Engine.Utility;
using UnityEngine;

namespace Engine.Game.Actor {
    public class ActorObject : GameObjectBehavior {
        public string sequenceType;
        public string sequenceCode;

        public virtual void Awake() {
        }

        public virtual void Start() {
        }

        public virtual void Init() {
            LoadSequence();
        }

        public virtual void OnEnable() {
            //MessengerObject<InputTouchInfo>.AddListener(MessengerObbjectMessageType.OnEventInputDown, OnInputDown);
            //MessengerObject<InputTouchInfo>.AddListener(MessengerObjectMessageType.OnEventInputUp, OnInputUp);
        }

        public virtual void OnDisable() {
            //MessengerObject<InputTouchInfo>.RemoveListener(MessengerObjectMessageType.OnEventInputDown, OnInputDown);
            //MessengerObject<InputTouchInfo>.RemoveListener(MessengerObjectMessageType.OnEventInputUp, OnInputUp);
        }

        public virtual void OnInputDown(InputTouchInfo touchInfo) {
            LogUtil.Log("OnInputDown ActorObject");
            HitObject(gameObject, touchInfo);
        }

        public virtual void OnInputUp(InputTouchInfo touchInfo) {
            LogUtil.Log("OnInputDown ActorObject");
            HitObject(gameObject, touchInfo);
        }

        public virtual void LoadSequence() {

            // TODO: ...
        }

        public virtual bool HitObject(GameObject go, InputTouchInfo inputTouchInfo) {
            Ray screenRay = Camera.main.ScreenPointToRay(inputTouchInfo.position3d);
            RaycastHit hit;

            if (Physics.Raycast(screenRay, out hit, Mathf.Infinity) && hit.transform != null) {
                if (hit.transform.gameObject == go) {
                    return true;
                }
            }
            return false;
        }

        private void Update() {
            if (InputSystem.Instance.IsAxisPressed(InputSystem.InputAxis.Up)) {

                // do run
            }
            else {

                // do idle
            }
        }

        public virtual void PlayAnimation(string animationName, PlayMode mode) {
            gameObject.GetComponent<UnityEngine.Animation>().CrossFade(animationName, 1.0f, mode);
        }

        public virtual void PlayRun() {
            PlayAnimation("run", PlayMode.StopSameLayer);
        }

        public virtual void PlayWalk() {
            PlayAnimation("walk", PlayMode.StopSameLayer);
        }

        public virtual void PlayJump() {
            PlayAnimation("jump", PlayMode.StopSameLayer);
        }

        public virtual void PlayIdle() {
            PlayAnimation("idle", PlayMode.StopSameLayer);
        }
    }
}