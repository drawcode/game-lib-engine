using System;
using System.Collections.Generic;
using UnityEngine;

namespace Engine.Animation {

    public class DrawSequencer : GameObjectBehavior {
        public static string SEQUENCE_SCENE_LOAD = "SequenceSceneLoad";
        public static string SEQUENCE_SCENE_UNLOAD = "SequenceSceneUnload";

        //public static string SEQUENCE_SCENE_LOAD = "SequenceObjectLoad";
        //public static string SEQUENCE_SCENE_UNLOAD = "SequenceObjectUnload";

        private static DrawSequencer instance = null;
        public static string className = "DrawSequencer";

        public static DrawSequencer Instance {
            get {
                if (instance == null) {
                    instance = FindObjectOfType(typeof(DrawSequencer)) as DrawSequencer;
                    if (instance == null)
                        LogUtil.LogError(String.Format("Could not locate an {0} object. You have to have exactly one {0} in the scene.", className));
                }

                return instance;
            }
            set {
                instance = value;
            }
        }

        private void Awake() {
            if (Instance != null && this != Instance) {

                //There is already a copy of this script running
                Destroy(this);
                return;
            }
            Instance = this;
        }

        public void ApplySequence(GameObject go, SequenceSets sequenceSets) {
            foreach (KeyValuePair<string, Dictionary<string, SequenceBase>> sequenceSet in sequenceSets.sequences) {
                foreach (KeyValuePair<string, SequenceBase> sequenceItem in sequenceSet.Value) {
                    if (sequenceItem.Value.type == "tween") {
                        SequenceTween tween = sequenceItem.Value as SequenceTween;

                        Debug.Log(tween.type);
                    }
                    else if (sequenceItem.Value.type == "action") {

                        // Perform action such as load new scene, play sound
                    }
                }
            }
        }
    }
}