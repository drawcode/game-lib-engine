using System.Collections;
using System.Collections.Generic;
using Engine;
using Engine.Data;
using Engine.Game.Controllers;
using Engine.Networking;
using Engine.Utility;
using UnityEngine;

namespace Engine.Networking {
    public class NetworkController : BaseEngineBehavior {
        
        #if NETWORK_USE_UNITY

        public BaseThirdPersonController targetController;
        private bool jumpButton;
        private float verticalInput;
        private float horizontalInput;
        private bool lastJumpButton;
        private float lastVerticalInput;
        private float lastHorizontalInput;

        private void Update() {

            // Sample user input
            verticalInput = Input.GetAxisRaw("Vertical");
            horizontalInput = Input.GetAxisRaw("Horizontal");
            jumpButton = Input.GetButton("Jump");

            int tmpVal = 0;
            if (jumpButton)
                tmpVal = 1;

            if (verticalInput != lastVerticalInput || horizontalInput != lastHorizontalInput || lastJumpButton != jumpButton) {
                if (networkView.viewID != NetworkViewID.unassigned)
                    networkView.RPC("SendUserInput", RPCMode.Server, horizontalInput, verticalInput, tmpVal);
            }

            lastJumpButton = jumpButton;
            lastVerticalInput = verticalInput;
            lastHorizontalInput = horizontalInput;
        }

        [RPC]
        private void SendUserInput(float h, float v, float j) {
            targetController.horizontalInput = h;
            targetController.verticalInput = v;
            if (j == 1)
                targetController.jumpButton = true;
            else
                targetController.jumpButton = false;
        }

#endif
    }
}