using System;
using System.Collections.Generic;

namespace Engine.Animation {

    public class SequenceSets {
        public Dictionary<string, Dictionary<string, SequenceBase>> sequences;

        public SequenceSets() {
            Reset();
        }

        public void Reset() {
            sequences = new Dictionary<string, Dictionary<string, SequenceBase>>();
        }
    }
}