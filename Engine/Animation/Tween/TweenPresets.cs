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

                // RETUNED 2026-09-12, on a direct brief: "use penner equations for Ease In on
                // show, and on hide a faster Ease Out so it sort of zips out, nothing that feels
                // dragged but snappy and not annoying as you continually play and see the same
                // screens."
                //
                // Read as the FEEL, not the equation names, because the two disagree: an
                // animation that "eases in" to a resting position is decelerating, which Penner
                // calls EaseOUT, and one that "zips out" is accelerating, which Penner calls
                // EaseIN. So shows decelerate into place and hides accelerate away.
                //
                //   SHOW  cubicEaseOut  — settles, with more character than the quad it replaces
                //   HIDE  quartEaseIn   — a harder curve than cubic; the screen is gone before
                //                         you finish reading it, which is the point on the
                //                         fiftieth time you see it
                //
                // Timing: hides are a little over HALF the show, and the show's delay drops from
                // .5 to .22. That delay exists so the outgoing screen clears before the incoming
                // one starts; with the hide now .2 it no longer needs half a second. A screen
                // change was .45 out + .5 wait + .45 in = 1.4s of animation and is now .2 + .22 +
                // .34 = 0.76s.
                Set(new TweenPreset("panel-show",  .34f, .22f, TweenEaseType.cubicEaseOut));
                Set(new TweenPreset("panel-hide",  .20f, 0f,   TweenEaseType.quartEaseIn));

                // Dialogs sit ON a screen rather than replacing one, so there is nothing to wait
                // for and they can be quicker still.
                Set(new TweenPreset("dialog-show", .24f, 0f,   TweenEaseType.cubicEaseOut));
                Set(new TweenPreset("dialog-hide", .14f, 0f,   TweenEaseType.quartEaseIn));

                // A crossfade has no travel, so it reads slower at the same duration. Sine keeps
                // it from looking like a hard cut at these lengths.
                Set(new TweenPreset("fade-in",     .28f, 0f,   TweenEaseType.sineEaseOut));
                Set(new TweenPreset("fade-out",    .16f, 0f,   TweenEaseType.sineEaseIn));

                // The HUD tracks the panel timings: it enters and leaves alongside them.
                Set(new TweenPreset("hud-show",    .34f, .22f, TweenEaseType.cubicEaseOut));
                Set(new TweenPreset("hud-hide",    .20f, 0f,   TweenEaseType.quartEaseIn));
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
