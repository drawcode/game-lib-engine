using System;
using System.Collections;
using System.Collections.Generic;
using Engine;
using Engine.Data;
using Engine.Networking;
using Engine.Utility;
using UnityEngine;

namespace Engine.Networking {
    public class NetworkSyncAnimation : BaseEngineBehavior {
        
        #if NETWORK_UNITY

        public enum AniStates {
            walk = 0,
            run,
            kick,
            punch,
            jump,
            jumpfall,
            idle,
            gotbit,
            gothit,
            walljump,
            deathfall,
            jetpackjump,
            ledgefall,
            buttstomp,
            jumpland
        }

        public AniStates currentAnimation = AniStates.idle;
        public AniStates lastAnimation = AniStates.idle;

        public void SyncAnimation(string animationValue) {
            currentAnimation = (AniStates)Enum.Parse(typeof(AniStates), animationValue);
        }

        // Update is called once per frame
        private void Update() {
            if (lastAnimation != currentAnimation) {
                lastAnimation = currentAnimation;
                animation.CrossFade(System.Enum.GetName(typeof(AniStates), currentAnimation));
                animation["run"].normalizedSpeed = 1.0F;
                animation["walk"].normalizedSpeed = 1.0F;
            }
        }

        private void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
            if (stream.isWriting) {
                char ani = (char)currentAnimation;
                stream.Serialize(ref ani);
            }
            else {
                char ani = (char)0;
                stream.Serialize(ref ani);

                currentAnimation = (AniStates)ani;
            }
        }

#endif
    }
}