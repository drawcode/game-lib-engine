using System;
using System.Collections.Generic;

namespace Engine.Animation {

    public class SequenceTween : SequenceBase {
        public System.Collections.Hashtable sequenceProperties;

        public SequenceTween() {
            Reset();
        }

        public void Reset() {
            type = "";
            attributes = new Dictionary<string, object>();
            sequenceProperties = DrawTween.Hash("time", 0.0, "delay", 0.0, "easeType", "linear");
        }
    }
}