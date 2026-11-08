using System;
using System.Collections.Generic;

using UnityEngine;

#if INPUTCOMPAT_INPUT_SYSTEM && ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UIS = UnityEngine.InputSystem;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;
#endif

namespace Engine {

    // Drop-in stand-in for the legacy UnityEngine.Input class, so game code reads input the same
    // way under every Active Input Handling setting.
    //
    //   Legacy or Both   every member forwards to UnityEngine.Input: behaviour is byte-for-byte
    //                    what it was, which keeps projects that still run the old manager unchanged.
    //   Input System     (ENABLE_INPUT_SYSTEM without ENABLE_LEGACY_INPUT_MANAGER, and the package
    //                    installed: INPUTCOMPAT_INPUT_SYSTEM from this asmdef's versionDefines) the
    //                    members are rebuilt on the Input System. In that mode UnityEngine.Input
    //                    throws InvalidOperationException on every read, so nothing may call it.
    //
    // Its own auto-referenced assembly, so Assembly-CSharp-firstpass (Plugins/) can use it as well
    // as Assembly-CSharp. The Unity.InputSystem reference is optional: where the package is
    // missing, the reference is skipped and the forwarding branch compiles.
    //
    // A file opts in with one line and keeps its call sites as they are:
    //
    //   using Input = Engine.InputCompat;
    //
    // The Input System path emulates the Input Manager: axes come from a table that mirrors the
    // default ProjectSettings/InputManager.asset (Horizontal, Vertical, Fire1-3, Jump, Mouse X/Y,
    // Mouse ScrollWheel, Submit, Cancel), keyboard axes keep their sensitivity/gravity/snap ramp,
    // and the legacy button names ("left ctrl", "mouse 0", "joystick button 3") resolve to
    // controls. RegisterAxis adds or overrides an axis for a project with its own entries. Joystick
    // buttons follow the legacy Xbox numbering (0 south, 1 east, 2 west, 3 north, 4/5 shoulders,
    // 6 select, 7 start, 8/9 stick presses).
    public static class InputCompat {

        // ---- shared surface ----------------------------------------------------------------

        public static LocationService location {
            get {
                // LocationService is its own native service, not the input backend; it keeps
                // working with the legacy manager off.
                return global::UnityEngine.Input.location;
            }
        }

        public enum AxisType {
            keyOrButton = 0,
            mouseMovement = 1,
            joystickAxis = 2,
            windowMovement = 3
        }

        // One Input Manager entry. Several entries may share a name; the largest magnitude wins,
        // as in the legacy manager.
        public class Axis {
            public string name;
            public AxisType type;
            public string negativeButton;
            public string positiveButton;
            public string altNegativeButton;
            public string altPositiveButton;
            public float gravity;
            public float dead;
            public float sensitivity;
            public bool snap;
            public bool invert;
            public int axis;

            public Axis(string name, AxisType type, string positive, string negative, string altPositive, string altNegative,
                float sensitivity, float gravity, bool snap, float dead, bool invert, int axis) {
                this.name = name;
                this.type = type;
                positiveButton = positive;
                negativeButton = negative;
                altPositiveButton = altPositive;
                altNegativeButton = altNegative;
                this.sensitivity = sensitivity;
                this.gravity = gravity;
                this.snap = snap;
                this.dead = dead;
                this.invert = invert;
                this.axis = axis;
            }
        }

#if !(INPUTCOMPAT_INPUT_SYSTEM && ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER)

        // ---- legacy / both: forward ----------------------------------------------------------

        public static bool backendIsInputSystem { get { return false; } }

        public static Vector3 mousePosition { get { return global::UnityEngine.Input.mousePosition; } }
        public static bool mousePresent { get { return global::UnityEngine.Input.mousePresent; } }
        public static Vector2 mouseScrollDelta { get { return global::UnityEngine.Input.mouseScrollDelta; } }
        public static bool anyKey { get { return global::UnityEngine.Input.anyKey; } }
        public static bool anyKeyDown { get { return global::UnityEngine.Input.anyKeyDown; } }

        public static int touchCount { get { return global::UnityEngine.Input.touchCount; } }
        public static Touch[] touches { get { return global::UnityEngine.Input.touches; } }
        public static bool touchSupported { get { return global::UnityEngine.Input.touchSupported; } }
        public static bool multiTouchEnabled {
            get { return global::UnityEngine.Input.multiTouchEnabled; }
            set { global::UnityEngine.Input.multiTouchEnabled = value; }
        }
        public static Touch GetTouch(int index) { return global::UnityEngine.Input.GetTouch(index); }

        public static Vector3 acceleration { get { return global::UnityEngine.Input.acceleration; } }
        public static int accelerationEventCount { get { return global::UnityEngine.Input.accelerationEventCount; } }
        public static AccelerationEvent GetAccelerationEvent(int index) { return global::UnityEngine.Input.GetAccelerationEvent(index); }
        public static DeviceOrientation deviceOrientation { get { return global::UnityEngine.Input.deviceOrientation; } }

        public static bool GetKey(KeyCode key) { return global::UnityEngine.Input.GetKey(key); }
        public static bool GetKeyDown(KeyCode key) { return global::UnityEngine.Input.GetKeyDown(key); }
        public static bool GetKeyUp(KeyCode key) { return global::UnityEngine.Input.GetKeyUp(key); }
        public static bool GetKey(string name) { return global::UnityEngine.Input.GetKey(name); }
        public static bool GetKeyDown(string name) { return global::UnityEngine.Input.GetKeyDown(name); }
        public static bool GetKeyUp(string name) { return global::UnityEngine.Input.GetKeyUp(name); }

        public static bool GetMouseButton(int button) { return global::UnityEngine.Input.GetMouseButton(button); }
        public static bool GetMouseButtonDown(int button) { return global::UnityEngine.Input.GetMouseButtonDown(button); }
        public static bool GetMouseButtonUp(int button) { return global::UnityEngine.Input.GetMouseButtonUp(button); }

        public static float GetAxis(string axisName) { return global::UnityEngine.Input.GetAxis(axisName); }
        public static float GetAxisRaw(string axisName) { return global::UnityEngine.Input.GetAxisRaw(axisName); }
        public static bool GetButton(string buttonName) { return global::UnityEngine.Input.GetButton(buttonName); }
        public static bool GetButtonDown(string buttonName) { return global::UnityEngine.Input.GetButtonDown(buttonName); }
        public static bool GetButtonUp(string buttonName) { return global::UnityEngine.Input.GetButtonUp(buttonName); }

        public static string[] GetJoystickNames() { return global::UnityEngine.Input.GetJoystickNames(); }

        public static void ResetInputAxes() { global::UnityEngine.Input.ResetInputAxes(); }

        // True when the Input Manager has an axis or button of this name.
        public static bool HasAxis(string axisName) {
            try {
                global::UnityEngine.Input.GetAxisRaw(axisName);
                return true;
            }
            catch (ArgumentException) {
                return false;
            }
        }

        public static void RegisterAxis(Axis axis) {
            // The Input Manager asset is the source of truth here; nothing to register.
        }

#else

        // ---- Input System ------------------------------------------------------------------

        public static bool backendIsInputSystem { get { return true; } }

        public static Vector3 mousePosition {
            get {
                // Legacy reports the mouse, or the last touch on a device without one.
                UIS.Pointer pointer = UIS.Mouse.current != null ? (UIS.Pointer)UIS.Mouse.current : UIS.Pointer.current;
                if (pointer == null) {
                    return lastPointer;
                }
                Vector2 p = pointer.position.ReadValue();
                lastPointer = new Vector3(p.x, p.y, 0f);
                return lastPointer;
            }
        }

        public static bool mousePresent { get { return UIS.Mouse.current != null; } }

        public static Vector2 mouseScrollDelta {
            get {
                UIS.Mouse mouse = UIS.Mouse.current;
                return mouse != null ? mouse.scroll.ReadValue() : Vector2.zero;
            }
        }

        public static bool anyKey {
            get {
                UIS.Keyboard kb = UIS.Keyboard.current;
                UIS.Mouse mouse = UIS.Mouse.current;
                return (kb != null && kb.anyKey.isPressed)
                    || (mouse != null && (mouse.leftButton.isPressed || mouse.rightButton.isPressed || mouse.middleButton.isPressed));
            }
        }

        public static bool anyKeyDown {
            get {
                UIS.Keyboard kb = UIS.Keyboard.current;
                UIS.Mouse mouse = UIS.Mouse.current;
                return (kb != null && kb.anyKey.wasPressedThisFrame)
                    || (mouse != null && (mouse.leftButton.wasPressedThisFrame || mouse.rightButton.wasPressedThisFrame || mouse.middleButton.wasPressedThisFrame));
            }
        }

        // ---- touch ----

        public static int touchCount {
            get {
                RefreshTouches();
                return touchBuffer.Count;
            }
        }

        public static Touch[] touches {
            get {
                RefreshTouches();
                // Legacy hands out a fresh array per call; callers may keep it, so the same.
                return touchBuffer.ToArray();
            }
        }

        public static Touch GetTouch(int index) {
            RefreshTouches();
            if (index < 0 || index >= touchBuffer.Count) {
                throw new ArgumentOutOfRangeException("index", "Index out of bounds.");
            }
            return touchBuffer[index];
        }

        public static bool touchSupported { get { return UIS.Touchscreen.current != null; } }

        public static bool multiTouchEnabled { get; set; } = true;

        // ---- motion ----

        public static Vector3 acceleration {
            get {
                UIS.Accelerometer acc = UIS.Accelerometer.current;
                if (acc == null) {
                    return Vector3.zero;
                }
                if (!acc.enabled) {
                    UIS.InputSystem.EnableDevice(acc);
                }
                return acc.acceleration.ReadValue();
            }
        }

        // AccelerationEvent has no public constructor; callers on this backend read acceleration.
        public static int accelerationEventCount { get { return 0; } }

        public static AccelerationEvent GetAccelerationEvent(int index) {
            throw new ArgumentOutOfRangeException("index", "No acceleration events on the Input System backend; read InputCompat.acceleration.");
        }

        public static DeviceOrientation deviceOrientation {
            get {
                switch (Screen.orientation) {
                    case ScreenOrientation.Portrait: return DeviceOrientation.Portrait;
                    case ScreenOrientation.PortraitUpsideDown: return DeviceOrientation.PortraitUpsideDown;
                    case ScreenOrientation.LandscapeLeft: return DeviceOrientation.LandscapeLeft;
                    case ScreenOrientation.LandscapeRight: return DeviceOrientation.LandscapeRight;
                    default: return DeviceOrientation.Unknown;
                }
            }
        }

        // ---- keys ----

        public static bool GetKey(KeyCode key) { return ReadKey(key, Phase.held); }
        public static bool GetKeyDown(KeyCode key) { return ReadKey(key, Phase.down); }
        public static bool GetKeyUp(KeyCode key) { return ReadKey(key, Phase.up); }

        public static bool GetKey(string name) { return Read(ResolveButton(name), Phase.held); }
        public static bool GetKeyDown(string name) { return Read(ResolveButton(name), Phase.down); }
        public static bool GetKeyUp(string name) { return Read(ResolveButton(name), Phase.up); }

        public static bool GetMouseButton(int button) { return Read(MouseButton(button), Phase.held); }
        public static bool GetMouseButtonDown(int button) { return Read(MouseButton(button), Phase.down); }
        public static bool GetMouseButtonUp(int button) { return Read(MouseButton(button), Phase.up); }

        // ---- virtual axes / buttons ----

        public static float GetAxis(string axisName) { return ReadAxis(axisName, false); }
        public static float GetAxisRaw(string axisName) { return ReadAxis(axisName, true); }

        public static bool GetButton(string buttonName) { return ReadButton(buttonName, Phase.held); }
        public static bool GetButtonDown(string buttonName) { return ReadButton(buttonName, Phase.down); }
        public static bool GetButtonUp(string buttonName) { return ReadButton(buttonName, Phase.up); }

        public static string[] GetJoystickNames() {
            var pads = UIS.Gamepad.all;
            string[] names = new string[pads.Count];
            for (int i = 0; i < pads.Count; i++) {
                names[i] = pads[i].displayName ?? pads[i].name;
            }
            return names;
        }

        public static void ResetInputAxes() {
            foreach (AxisState state in axisStates.Values) {
                state.value = 0f;
            }
        }

        // True when the emulated Input Manager has an axis or button of this name.
        public static bool HasAxis(string axisName) {
            EnsureAxes();
            return axisName != null && axes.ContainsKey(axisName);
        }

        public static void RegisterAxis(Axis axis) {
            if (axis == null || string.IsNullOrEmpty(axis.name)) {
                return;
            }
            EnsureAxes();
            List<Axis> list;
            if (!axes.TryGetValue(axis.name, out list)) {
                list = new List<Axis>();
                axes[axis.name] = list;
            }
            list.Add(axis);
            axisStates.Remove(axis.name);
        }

        // ---- implementation ----

        private enum Phase {
            held,
            down,
            up
        }

        private static Vector3 lastPointer;

        private static UIS.Controls.ButtonControl Read(Func<UIS.Controls.ButtonControl> control) {
            return control != null ? control() : null;
        }

        private static bool Read(Func<UIS.Controls.ButtonControl> control, Phase phase) {
            UIS.Controls.ButtonControl c = Read(control);
            if (c == null) {
                return false;
            }
            switch (phase) {
                case Phase.down: return c.wasPressedThisFrame;
                case Phase.up: return c.wasReleasedThisFrame;
                default: return c.isPressed;
            }
        }

        // KeyCode -> control getter, built once. Null entries are keys with no Input System
        // equivalent; they read as never pressed.
        private static Func<UIS.Controls.ButtonControl>[] keyControls;

        private static bool ReadKey(KeyCode key, Phase phase) {
            if (keyControls == null) {
                BuildKeyControls();
            }
            int i = (int)key;
            if (i < 0 || i >= keyControls.Length) {
                return false;
            }
            return Read(keyControls[i], phase);
        }

        private static readonly Dictionary<string, string> keyRenames = new Dictionary<string, string>(StringComparer.Ordinal) {
            { "Return", "Enter" },
            { "KeypadEnter", "NumpadEnter" },
            { "KeypadPlus", "NumpadPlus" },
            { "KeypadMinus", "NumpadMinus" },
            { "KeypadMultiply", "NumpadMultiply" },
            { "KeypadDivide", "NumpadDivide" },
            { "KeypadPeriod", "NumpadPeriod" },
            { "KeypadEquals", "NumpadEquals" },
            { "LeftControl", "LeftCtrl" },
            { "RightControl", "RightCtrl" },
            { "LeftCommand", "LeftMeta" },
            { "LeftApple", "LeftMeta" },
            { "LeftWindows", "LeftMeta" },
            { "RightCommand", "RightMeta" },
            { "RightApple", "RightMeta" },
            { "RightWindows", "RightMeta" },
            { "BackQuote", "Backquote" },
            { "Numlock", "NumLock" },
            { "Print", "PrintScreen" },
            { "Menu", "ContextMenu" },
            { "AltGr", "AltGr" }
        };

        private static void BuildKeyControls() {
            int max = 0;
            foreach (KeyCode code in Enum.GetValues(typeof(KeyCode))) {
                max = Math.Max(max, (int)code);
            }
            keyControls = new Func<UIS.Controls.ButtonControl>[max + 1];

            foreach (KeyCode code in Enum.GetValues(typeof(KeyCode))) {
                string name = code.ToString();
                Func<UIS.Controls.ButtonControl> getter = null;

                if (name.StartsWith("Alpha", StringComparison.Ordinal) && name.Length == 6) {
                    name = "Digit" + name.Substring(5);
                }
                else if (name.StartsWith("Keypad", StringComparison.Ordinal) && name.Length == 7 && char.IsDigit(name[6])) {
                    name = "Numpad" + name.Substring(6);
                }
                else if (name.StartsWith("Mouse", StringComparison.Ordinal) && name.Length == 6 && char.IsDigit(name[5])) {
                    getter = MouseButton(name[5] - '0');
                }
                else if (name.StartsWith("Joystick", StringComparison.Ordinal)) {
                    int at = name.LastIndexOf("Button", StringComparison.Ordinal);
                    int n;
                    if (at >= 0 && int.TryParse(name.Substring(at + 6), out n)) {
                        getter = JoystickButton(n);
                    }
                }

                string renamed;
                if (getter == null && keyRenames.TryGetValue(name, out renamed)) {
                    name = renamed;
                }

                UIS.Key key;
                if (getter == null && Enum.TryParse(name, false, out key) && key != UIS.Key.None) {
                    UIS.Key k = key;
                    getter = () => UIS.Keyboard.current != null ? UIS.Keyboard.current[k] : null;
                }

                if (getter != null && keyControls[(int)code] == null) {
                    keyControls[(int)code] = getter;
                }
            }
        }

        private static Func<UIS.Controls.ButtonControl> MouseButton(int button) {
            switch (button) {
                case 0: return () => UIS.Mouse.current != null ? UIS.Mouse.current.leftButton : TouchAsMouse();
                case 1: return () => UIS.Mouse.current != null ? UIS.Mouse.current.rightButton : null;
                case 2: return () => UIS.Mouse.current != null ? UIS.Mouse.current.middleButton : null;
                case 3: return () => UIS.Mouse.current != null ? UIS.Mouse.current.backButton : null;
                case 4: return () => UIS.Mouse.current != null ? UIS.Mouse.current.forwardButton : null;
                default: return null;
            }
        }

        // Legacy maps the first touch onto mouse button 0 on a touch device.
        private static UIS.Controls.ButtonControl TouchAsMouse() {
            UIS.Touchscreen ts = UIS.Touchscreen.current;
            return ts != null ? ts.primaryTouch.press : null;
        }

        private static Func<UIS.Controls.ButtonControl> JoystickButton(int n) {
            switch (n) {
                case 0: return () => UIS.Gamepad.current != null ? UIS.Gamepad.current.buttonSouth : null;
                case 1: return () => UIS.Gamepad.current != null ? UIS.Gamepad.current.buttonEast : null;
                case 2: return () => UIS.Gamepad.current != null ? UIS.Gamepad.current.buttonWest : null;
                case 3: return () => UIS.Gamepad.current != null ? UIS.Gamepad.current.buttonNorth : null;
                case 4: return () => UIS.Gamepad.current != null ? UIS.Gamepad.current.leftShoulder : null;
                case 5: return () => UIS.Gamepad.current != null ? UIS.Gamepad.current.rightShoulder : null;
                case 6: return () => UIS.Gamepad.current != null ? UIS.Gamepad.current.selectButton : null;
                case 7: return () => UIS.Gamepad.current != null ? UIS.Gamepad.current.startButton : null;
                case 8: return () => UIS.Gamepad.current != null ? UIS.Gamepad.current.leftStickButton : null;
                case 9: return () => UIS.Gamepad.current != null ? UIS.Gamepad.current.rightStickButton : null;
                default: return null;
            }
        }

        // Legacy button names, as the Input Manager and Input.GetKey(string) spell them.
        private static readonly Dictionary<string, Func<UIS.Controls.ButtonControl>> namedButtons =
            new Dictionary<string, Func<UIS.Controls.ButtonControl>>(StringComparer.Ordinal);

        private static readonly Dictionary<string, KeyCode> legacyNames = new Dictionary<string, KeyCode>(StringComparer.Ordinal) {
            { "left ctrl", KeyCode.LeftControl }, { "right ctrl", KeyCode.RightControl },
            { "left alt", KeyCode.LeftAlt }, { "right alt", KeyCode.RightAlt },
            { "left shift", KeyCode.LeftShift }, { "right shift", KeyCode.RightShift },
            { "left cmd", KeyCode.LeftCommand }, { "right cmd", KeyCode.RightCommand },
            { "left windows", KeyCode.LeftWindows }, { "right windows", KeyCode.RightWindows },
            { "up", KeyCode.UpArrow }, { "down", KeyCode.DownArrow },
            { "left", KeyCode.LeftArrow }, { "right", KeyCode.RightArrow },
            { "space", KeyCode.Space }, { "return", KeyCode.Return }, { "enter", KeyCode.KeypadEnter },
            { "escape", KeyCode.Escape }, { "backspace", KeyCode.Backspace }, { "tab", KeyCode.Tab },
            { "delete", KeyCode.Delete }, { "insert", KeyCode.Insert }, { "home", KeyCode.Home },
            { "end", KeyCode.End }, { "page up", KeyCode.PageUp }, { "page down", KeyCode.PageDown },
            { "caps lock", KeyCode.CapsLock }, { "numlock", KeyCode.Numlock }, { "scroll lock", KeyCode.ScrollLock },
            { "-", KeyCode.Minus }, { "=", KeyCode.Equals }, { "[", KeyCode.LeftBracket }, { "]", KeyCode.RightBracket },
            { "\\", KeyCode.Backslash }, { ";", KeyCode.Semicolon }, { "'", KeyCode.Quote },
            { ",", KeyCode.Comma }, { ".", KeyCode.Period }, { "/", KeyCode.Slash }, { "`", KeyCode.BackQuote }
        };

        private static Func<UIS.Controls.ButtonControl> ResolveButton(string name) {
            if (string.IsNullOrEmpty(name)) {
                return null;
            }

            Func<UIS.Controls.ButtonControl> getter;
            if (namedButtons.TryGetValue(name, out getter)) {
                return getter;
            }

            string n = name.Trim().ToLowerInvariant();
            int number;
            KeyCode code;

            if (n.StartsWith("mouse ", StringComparison.Ordinal) && int.TryParse(n.Substring(6), out number)) {
                getter = MouseButton(number);
            }
            else if (n.StartsWith("joystick", StringComparison.Ordinal)) {
                int at = n.LastIndexOf("button ", StringComparison.Ordinal);
                if (at >= 0 && int.TryParse(n.Substring(at + 7), out number)) {
                    getter = JoystickButton(number);
                }
            }
            else if (n.StartsWith("[", StringComparison.Ordinal) && n.EndsWith("]", StringComparison.Ordinal) && n.Length > 2) {
                // keypad: "[0]", "[+]", "[enter]"
                string inner = n.Substring(1, n.Length - 2);
                if (inner.Length == 1 && char.IsDigit(inner[0])) {
                    getter = KeyGetter((KeyCode)((int)KeyCode.Keypad0 + (inner[0] - '0')));
                }
                else if (inner == "enter") { getter = KeyGetter(KeyCode.KeypadEnter); }
                else if (inner == "+") { getter = KeyGetter(KeyCode.KeypadPlus); }
                else if (inner == "-") { getter = KeyGetter(KeyCode.KeypadMinus); }
                else if (inner == "*") { getter = KeyGetter(KeyCode.KeypadMultiply); }
                else if (inner == "/") { getter = KeyGetter(KeyCode.KeypadDivide); }
                else if (inner == ".") { getter = KeyGetter(KeyCode.KeypadPeriod); }
            }
            else if (legacyNames.TryGetValue(n, out code)) {
                getter = KeyGetter(code);
            }
            else if (n.Length == 1 && n[0] >= 'a' && n[0] <= 'z') {
                getter = KeyGetter((KeyCode)((int)KeyCode.A + (n[0] - 'a')));
            }
            else if (n.Length == 1 && char.IsDigit(n[0])) {
                getter = KeyGetter((KeyCode)((int)KeyCode.Alpha0 + (n[0] - '0')));
            }
            else if (n.Length >= 2 && n[0] == 'f' && int.TryParse(n.Substring(1), out number) && number >= 1 && number <= 15) {
                getter = KeyGetter((KeyCode)((int)KeyCode.F1 + number - 1));
            }

            if (getter == null) {
                Debug.LogWarning("InputCompat: no Input System control for legacy button name '" + name + "'; it reads as never pressed.");
            }

            namedButtons[name] = getter;
            return getter;
        }

        private static Func<UIS.Controls.ButtonControl> KeyGetter(KeyCode code) {
            if (keyControls == null) {
                BuildKeyControls();
            }
            int i = (int)code;
            return i >= 0 && i < keyControls.Length ? keyControls[i] : null;
        }

        // ---- axes ----

        private class AxisState {
            public float value;
            public int frame = -1;
            public float time;
            public float result;
            public float resultRaw;
        }

        private static Dictionary<string, List<Axis>> axes;
        private static readonly Dictionary<string, AxisState> axisStates = new Dictionary<string, AxisState>(StringComparer.Ordinal);
        private static readonly HashSet<string> missingAxes = new HashSet<string>(StringComparer.Ordinal);

        // The default Input Manager entries (ProjectSettings/InputManager.asset).
        private static void EnsureAxes() {
            if (axes != null) {
                return;
            }
            axes = new Dictionary<string, List<Axis>>(StringComparer.Ordinal);
            Axis[] defaults = {
                new Axis("Horizontal", AxisType.keyOrButton, "right", "left", "d", "a", 3f, 3f, true, 0.001f, false, 0),
                new Axis("Vertical", AxisType.keyOrButton, "up", "down", "w", "s", 3f, 3f, true, 0.001f, false, 0),
                new Axis("Fire1", AxisType.keyOrButton, "left ctrl", null, "mouse 0", null, 1000f, 1000f, false, 0.001f, false, 0),
                new Axis("Fire2", AxisType.keyOrButton, "left alt", null, "mouse 1", null, 1000f, 1000f, false, 0.001f, false, 0),
                new Axis("Fire3", AxisType.keyOrButton, "left cmd", null, "mouse 2", null, 1000f, 1000f, false, 0.001f, false, 0),
                new Axis("Jump", AxisType.keyOrButton, "space", null, null, null, 1000f, 1000f, false, 0.001f, false, 0),
                new Axis("Mouse X", AxisType.mouseMovement, null, null, null, null, 0.1f, 0f, false, 0f, false, 0),
                new Axis("Mouse Y", AxisType.mouseMovement, null, null, null, null, 0.1f, 0f, false, 0f, false, 1),
                new Axis("Mouse ScrollWheel", AxisType.mouseMovement, null, null, null, null, 0.1f, 0f, false, 0f, false, 2),
                new Axis("Horizontal", AxisType.joystickAxis, null, null, null, null, 1f, 0f, false, 0.19f, false, 0),
                new Axis("Vertical", AxisType.joystickAxis, null, null, null, null, 1f, 0f, false, 0.19f, true, 1),
                new Axis("Fire1", AxisType.keyOrButton, "joystick button 0", null, null, null, 1000f, 1000f, false, 0.001f, false, 0),
                new Axis("Fire2", AxisType.keyOrButton, "joystick button 1", null, null, null, 1000f, 1000f, false, 0.001f, false, 0),
                new Axis("Fire3", AxisType.keyOrButton, "joystick button 2", null, null, null, 1000f, 1000f, false, 0.001f, false, 0),
                new Axis("Jump", AxisType.keyOrButton, "joystick button 3", null, null, null, 1000f, 1000f, false, 0.001f, false, 0),
                new Axis("Submit", AxisType.keyOrButton, "return", null, "joystick button 0", null, 1000f, 1000f, false, 0.001f, false, 0),
                new Axis("Submit", AxisType.keyOrButton, "enter", null, "space", null, 1000f, 1000f, false, 0.001f, false, 0),
                new Axis("Cancel", AxisType.keyOrButton, "escape", null, "joystick button 1", null, 1000f, 1000f, false, 0.001f, false, 0)
            };
            foreach (Axis a in defaults) {
                List<Axis> list;
                if (!axes.TryGetValue(a.name, out list)) {
                    list = new List<Axis>();
                    axes[a.name] = list;
                }
                list.Add(a);
            }
        }

        private static List<Axis> AxesNamed(string axisName) {
            EnsureAxes();
            List<Axis> list;
            if (axisName != null && axes.TryGetValue(axisName, out list)) {
                return list;
            }
            if (axisName != null && missingAxes.Add(axisName)) {
                // Legacy throws ArgumentException here; a missing axis reads 0 instead of
                // stopping the frame, and is reported once.
                Debug.LogWarning("InputCompat: input axis '" + axisName + "' is not set up; it reads 0. Add it with InputCompat.RegisterAxis.");
            }
            return null;
        }

        private static float ReadAxis(string axisName, bool raw) {
            List<Axis> list = AxesNamed(axisName);
            if (list == null) {
                return 0f;
            }

            AxisState state;
            if (!axisStates.TryGetValue(axisName, out state)) {
                state = new AxisState();
                axisStates[axisName] = state;
            }

            // One evaluation per frame, so a ramp advances by the frame time however many
            // callers read it.
            int frame = Time.frameCount;
            if (state.frame != frame) {
                float now = Time.unscaledTime;
                float dt = state.frame < 0 ? 0f : Mathf.Min(now - state.time, 0.25f);
                state.frame = frame;
                state.time = now;
                Evaluate(list, state, dt);
            }

            return raw ? state.resultRaw : state.result;
        }

        private static void Evaluate(List<Axis> list, AxisState state, float dt) {
            float best = 0f;
            float bestRaw = 0f;
            float keyTarget = 0f;
            Axis keyAxis = null;

            for (int i = 0; i < list.Count; i++) {
                Axis a = list[i];
                float v = 0f;

                switch (a.type) {
                    case AxisType.keyOrButton: {
                        bool pos = Read(ResolveButton(a.positiveButton), Phase.held) || Read(ResolveButton(a.altPositiveButton), Phase.held);
                        bool neg = Read(ResolveButton(a.negativeButton), Phase.held) || Read(ResolveButton(a.altNegativeButton), Phase.held);
                        v = (pos ? 1f : 0f) - (neg ? 1f : 0f);
                        if (a.invert) {
                            v = -v;
                        }
                        if (keyAxis == null || Mathf.Abs(v) > Mathf.Abs(keyTarget)) {
                            keyTarget = v;
                            keyAxis = a;
                        }
                        break;
                    }
                    case AxisType.mouseMovement: {
                        UIS.Mouse mouse = UIS.Mouse.current;
                        if (mouse != null) {
                            if (a.axis == 0) {
                                v = mouse.delta.ReadValue().x;
                            }
                            else if (a.axis == 1) {
                                v = mouse.delta.ReadValue().y;
                            }
                            else if (a.axis == 2) {
                                // One wheel notch reads 1 under the uniform scroll behaviour, as
                                // legacy's delta did, before sensitivity.
                                v = mouse.scroll.ReadValue().y;
                            }
                        }
                        v *= a.sensitivity;
                        if (a.invert) {
                            v = -v;
                        }
                        break;
                    }
                    case AxisType.joystickAxis: {
                        UIS.Gamepad pad = UIS.Gamepad.current;
                        if (pad != null) {
                            switch (a.axis) {
                                case 0: v = pad.leftStick.ReadValue().x; break;
                                case 1: v = -pad.leftStick.ReadValue().y; break;    // legacy Y is down-positive
                                case 3: v = pad.rightStick.ReadValue().x; break;
                                case 4: v = -pad.rightStick.ReadValue().y; break;
                                case 5: v = pad.dpad.ReadValue().x; break;
                                case 6: v = pad.dpad.ReadValue().y; break;
                                case 8: v = pad.leftTrigger.ReadValue(); break;
                                case 9: v = pad.rightTrigger.ReadValue(); break;
                            }
                        }
                        if (Mathf.Abs(v) < a.dead) {
                            v = 0f;
                        }
                        v *= a.sensitivity;
                        if (a.invert) {
                            v = -v;
                        }
                        break;
                    }
                    default:
                        break;
                }

                if (a.type != AxisType.keyOrButton && Mathf.Abs(v) > Mathf.Abs(best)) {
                    best = v;
                    bestRaw = v;
                }
            }

            // Keyboard ramp: towards the target at sensitivity, back to rest at gravity, snapping
            // through zero on a reversal.
            if (keyAxis != null) {
                float value = state.value;
                if (keyTarget != 0f) {
                    if (keyAxis.snap && value != 0f && Mathf.Sign(value) != Mathf.Sign(keyTarget)) {
                        value = 0f;
                    }
                    value = Mathf.MoveTowards(value, keyTarget, keyAxis.sensitivity * dt);
                }
                else {
                    value = Mathf.MoveTowards(value, 0f, keyAxis.gravity * dt);
                }
                if (Mathf.Abs(value) < keyAxis.dead) {
                    value = 0f;
                }
                state.value = value;

                if (Mathf.Abs(value) > Mathf.Abs(best)) {
                    best = value;
                }
                if (Mathf.Abs(keyTarget) > Mathf.Abs(bestRaw)) {
                    bestRaw = keyTarget;
                }
            }

            state.result = best;
            state.resultRaw = bestRaw;
        }

        private static bool ReadButton(string buttonName, Phase phase) {
            List<Axis> list = AxesNamed(buttonName);
            if (list == null) {
                return false;
            }
            for (int i = 0; i < list.Count; i++) {
                Axis a = list[i];
                if (a.type != AxisType.keyOrButton) {
                    continue;
                }
                if (Read(ResolveButton(a.positiveButton), phase) || Read(ResolveButton(a.altPositiveButton), phase)) {
                    return true;
                }
            }
            return false;
        }

        // ---- touch buffer ----

        private static readonly List<Touch> touchBuffer = new List<Touch>();
        private static int touchFrame = -1;

        private static void RefreshTouches() {
            int frame = Time.frameCount;
            if (touchFrame == frame) {
                return;
            }
            touchFrame = frame;
            touchBuffer.Clear();

            if (!ETouch.EnhancedTouchSupport.enabled) {
                ETouch.EnhancedTouchSupport.Enable();
            }

            var active = ETouch.Touch.activeTouches;
            for (int i = 0; i < active.Count; i++) {
                ETouch.Touch t = active[i];
                Touch legacy = new Touch();
                legacy.fingerId = t.touchId;
                legacy.position = t.screenPosition;
                legacy.rawPosition = t.screenPosition;
                legacy.deltaPosition = t.delta;
                legacy.deltaTime = Time.unscaledDeltaTime;
                legacy.tapCount = t.tapCount;
                legacy.pressure = t.pressure;
                legacy.maximumPossiblePressure = 1f;
                legacy.radius = (t.radius.x + t.radius.y) * 0.5f;
                legacy.type = TouchType.Direct;
                legacy.phase = ToLegacyPhase(t.phase);
                touchBuffer.Add(legacy);

                if (!multiTouchEnabled) {
                    break;
                }
            }
        }

        private static TouchPhase ToLegacyPhase(UIS.TouchPhase phase) {
            switch (phase) {
                case UIS.TouchPhase.Began: return TouchPhase.Began;
                case UIS.TouchPhase.Moved: return TouchPhase.Moved;
                case UIS.TouchPhase.Ended: return TouchPhase.Ended;
                case UIS.TouchPhase.Canceled: return TouchPhase.Canceled;
                default: return TouchPhase.Stationary;
            }
        }

#endif
    }
}
