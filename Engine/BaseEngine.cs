using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Engine {

    public class BaseEngine : BaseEngineObject {

        public BaseEngine() {
        }

        public virtual void Tick(float deltaTime) {
        }
    }
}