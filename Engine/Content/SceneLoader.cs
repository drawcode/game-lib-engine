using System;
using System.Collections.Generic;
using Engine;
using UnityEngine;

namespace Engine.Content {

    public class SceneLoader : BaseEngineBehavior {

        public void Start() {
        }

        public void LoadSceneByName(string name) {
            Context.Current.ApplicationLoadLevelByName(name);
        }
    }
}