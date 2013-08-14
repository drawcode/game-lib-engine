using System;
using System.Collections.Generic;

namespace Engine.Animation {

    public abstract class SequenceBase {
        public string type;
        public Dictionary<string, object> attributes;

        public SequenceBase() {
        }
    }
}