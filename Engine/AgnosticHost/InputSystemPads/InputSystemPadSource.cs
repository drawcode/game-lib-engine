using Agnostic.Host;

using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Scripting;

// Nothing references this assembly statically (UnityInput binds by name), so without this the
// player's managed stripping would drop it whole.
[assembly: AlwaysLinkAssembly]

namespace Engine.AgnosticHost.Pads {

    // U-I gamepads through the Input System, pushed into the same sink UnityInput feeds, under the
    // same portable pad.* names. Its own assembly so it only compiles where com.unity.inputsystem
    // is installed (versionDefines + defineConstraints): manifest.json is gitignored, so a machine
    // without the package must still build, and ENABLE_INPUT_SYSTEM tracks the player setting,
    // not the package. UnityInput finds Poll/ReleaseAll by name and binds them once as delegates.
    //
    // Values are UNPROCESSED: the Input System's stick dead zone would stack on the core's radial
    // one, and the core owns shaping so a stick feels the same on every engine.
    [Preserve]
    public static class InputSystemPadSource {

        public const int gamepadDevice = 3;    // UnityInput.gamepadDevice

        private static readonly string[] buttonNames = {
            "pad.south", "pad.east", "pad.west", "pad.north",
            "pad.left-shoulder", "pad.right-shoulder", "pad.select", "pad.start",
            "pad.left-stick-press", "pad.right-stick-press",
            "pad.dpad-up", "pad.dpad-down", "pad.dpad-left", "pad.dpad-right"
        };

        private static readonly string[] axisNames = {
            "pad.left-stick.x", "pad.left-stick.y",
            "pad.right-stick.x", "pad.right-stick.y",
            "pad.left-trigger", "pad.right-trigger"
        };

        private static readonly bool[] down = new bool[buttonNames.Length];
        private static readonly float[] values = new float[axisNames.Length];

        [Preserve]
        public static void Poll(IRawInputSink sink) {

            Gamepad pad = Gamepad.current;

            if (sink == null) {
                return;
            }

            // Unplugged mid-press: the pad sends no release, so report one.
            if (pad == null) {
                ReleaseAll(sink);
                return;
            }

            Button(sink, 0, pad.buttonSouth);
            Button(sink, 1, pad.buttonEast);
            Button(sink, 2, pad.buttonWest);
            Button(sink, 3, pad.buttonNorth);
            Button(sink, 4, pad.leftShoulder);
            Button(sink, 5, pad.rightShoulder);
            Button(sink, 6, pad.selectButton);
            Button(sink, 7, pad.startButton);
            Button(sink, 8, pad.leftStickButton);
            Button(sink, 9, pad.rightStickButton);
            Button(sink, 10, pad.dpad.up);
            Button(sink, 11, pad.dpad.down);
            Button(sink, 12, pad.dpad.left);
            Button(sink, 13, pad.dpad.right);

            Axis(sink, 0, pad.leftStick.x.ReadUnprocessedValue());
            Axis(sink, 1, pad.leftStick.y.ReadUnprocessedValue());
            Axis(sink, 2, pad.rightStick.x.ReadUnprocessedValue());
            Axis(sink, 3, pad.rightStick.y.ReadUnprocessedValue());
            Axis(sink, 4, pad.leftTrigger.ReadUnprocessedValue());
            Axis(sink, 5, pad.rightTrigger.ReadUnprocessedValue());
        }

        [Preserve]
        public static void ReleaseAll(IRawInputSink sink) {

            for (int i = 0; i < down.Length; i++) {

                if (down[i]) {
                    down[i] = false;

                    if (sink != null) {
                        sink.OnButton(gamepadDevice, buttonNames[i], false);
                    }
                }
            }

            for (int i = 0; i < values.Length; i++) {

                if (values[i] != 0f) {
                    values[i] = 0f;

                    if (sink != null) {
                        sink.OnAxis(gamepadDevice, axisNames[i], 0f);
                    }
                }
            }
        }

        private static void Button(IRawInputSink sink, int i, ButtonControl control) {

            bool d = control.isPressed;

            if (d != down[i]) {
                down[i] = d;
                sink.OnButton(gamepadDevice, buttonNames[i], d);
            }
        }

        private static void Axis(IRawInputSink sink, int i, float v) {

            if (v != values[i]) {
                values[i] = v;
                sink.OnAxis(gamepadDevice, axisNames[i], v);
            }
        }
    }
}
