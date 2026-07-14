using System.Collections.Generic;

using Engine.Utility;

namespace Engine.Animation {

    public class TweenPreset {

        public string name = "";
        public float time = .45f;
        public float delay = 0f;
        public TweenEaseType easeType = TweenEaseType.quadEaseInOut;
        public TweenLoopType loopType = TweenLoopType.once;
        public bool fade = true;

        public TweenPreset(
            string name, float time, float delay,
            TweenEaseType easeType = TweenEaseType.quadEaseInOut,
            TweenLoopType loopType = TweenLoopType.once,
            bool fade = true) {

            this.name = name;
            this.time = time;
            this.delay = delay;
            this.easeType = easeType;
            this.loopType = loopType;
            this.fade = fade;
        }
    }

    // Named animation presets for panel/HUD transitions. Values seeded from the
    // legacy TweenUtil constants; the bitty token source becomes the authoritative
    // definition of these when the data-driven UI platform lands (plan chunk 2.1).
    public class TweenPresets {

        private static Dictionary<string, TweenPreset> presets = null;

        private static Dictionary<string, TweenPreset> GetPresets() {

            if (presets == null) {

                presets = new Dictionary<string, TweenPreset>();

                Set(new TweenPreset("panel-show", .45f, .5f));
                Set(new TweenPreset("panel-hide", .45f, 0f));
                Set(new TweenPreset("dialog-show", .3f, 0f));
                Set(new TweenPreset("dialog-hide", .3f, 0f));
                Set(new TweenPreset("fade-in", .5f, 0f));
                Set(new TweenPreset("fade-out", .5f, 0f));
                Set(new TweenPreset("hud-show", .45f, .5f));
                Set(new TweenPreset("hud-hide", .45f, 0f));
            }

            return presets;
        }

        public static TweenPreset Get(string name) {

            TweenPreset preset = null;

            if (!GetPresets().TryGetValue(name, out preset)) {
                return GetPresets()["panel-show"];
            }

            return preset;
        }

        public static void Set(TweenPreset preset) {

            if (preset == null || string.IsNullOrEmpty(preset.name)) {
                return;
            }

            GetPresets()[preset.name] = preset;
        }
    }
}
