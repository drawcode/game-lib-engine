using System;
using UnityEngine;

namespace Engine.State {
    public abstract class BaseState : GameObjectBehavior {

        public abstract void OnActivate();

        public abstract void OnDeactivate();

        public abstract void OnUpdate();
    }
}