using System;
using System.Collections.Generic;
using System.Globalization;
using Engine.Game.Data;

namespace Engine.Game.App.BaseApp {

    // GAMEPLAY TRACKING: the player's small first-time and running markers -- a tutorial played, a
    // hint seen, an intro shown, a counter -- in their OWN profile blob ("tracking", next to mode,
    // rpg, ...), so the core profile and the mode blob don't grow with every one-off flag.
    //
    // Flat on purpose: one "<group>:<code>" key per marker, each value a bool, number or string. A
    // JSON load gives primitives back (bool, long or double, string), so every reader converts
    // instead of casting, and the file stays a plain, portable, hand-readable JSON object:
    //   { "tutorial:tutorial-attack": true, "hint:joystick": 3 }
    // Saved and loaded with the other blobs by BaseGameState (per profile slot); writers change it
    // in memory and the caller's usual GameState.SaveProfile writes it.
    public class GameProfileTracking : DataObject {

        // the blob type BaseGameState.getProfileTrackingKey passes to getKey
        public static string blobType = "tracking";

        private static volatile GameProfileTracking current;
        private static object syncRoot = new Object();

        public static GameProfileTracking Current {
            get {
                if (current == null) {
                    lock (syncRoot) {
                        if (current == null) {
                            current = new GameProfileTracking();
                        }
                    }
                }
                return current;
            }
            set {
                current = value;
            }
        }

        public static string Key(string group, string code) {
            return (group ?? "") + ":" + (code ?? "");
        }

        object Raw(string group, string code) {
            object raw;
            return TryGetValue(Key(group, code), out raw) ? raw : null;
        }

        // FLAGS: set means true, "true", or a non-zero number

        public bool IsSet(string group, string code) {

            object raw = Raw(group, code);

            if (raw == null) {
                return false;
            }

            if (raw is bool) {
                return (bool)raw;
            }

            string text = Convert.ToString(raw, CultureInfo.InvariantCulture).Trim().ToLowerInvariant();

            if (text == "true") {
                return true;
            }

            double number;
            return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out number) && number != 0;
        }

        public void Mark(string group, string code) {
            Set<object>(Key(group, code), true);
        }

        public void Clear(string group, string code) {
            Remove(Key(group, code));
        }

        // NUMBERS: counters, stages, timestamps (unix seconds)

        public double GetNumber(string group, string code, double defaultValue = 0) {

            object raw = Raw(group, code);

            if (raw == null || raw is bool) {
                return defaultValue;
            }

            double number;
            return double.TryParse(Convert.ToString(raw, CultureInfo.InvariantCulture), NumberStyles.Float,
                CultureInfo.InvariantCulture, out number) ? number : defaultValue;
        }

        public void SetNumber(string group, string code, double value) {
            Set<object>(Key(group, code), value);
        }

        // adds to the stored number (0 when unset) and returns the new value
        public double Add(string group, string code, double by = 1) {
            double value = GetNumber(group, code) + by;
            SetNumber(group, code, value);
            return value;
        }

        // STRINGS

        public string GetString(string group, string code, string defaultValue = null) {
            object raw = Raw(group, code);
            return raw == null ? defaultValue : Convert.ToString(raw, CultureInfo.InvariantCulture);
        }

        public void SetString(string group, string code, string value) {
            Set<object>(Key(group, code), value);
        }

        // the codes stored under a group, in no particular order
        public List<string> Codes(string group) {

            List<string> codes = new List<string>();
            string prefix = Key(group, "");

            foreach (string key in Keys) {
                if (key.StartsWith(prefix, StringComparison.Ordinal)) {
                    codes.Add(key.Substring(prefix.Length));
                }
            }

            return codes;
        }
    }
}
