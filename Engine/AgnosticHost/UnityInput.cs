using System;
using System.Collections.Generic;

using Agnostic.Core;
using Agnostic.Host;
using Agnostic.Input;

using UnityEngine;

using DisplayInfo = Agnostic.Host.DisplayInfo;

namespace Engine.AgnosticHost {

    // U-I raw side, over the legacy Input Manager (the project's only input backend today).
    //
    // The adapter polls a fixed set of WATCHED controls once a frame and pushes only changes into
    // the sink; the action map keeps raw state between pushes, so an unchanged key costs a poll
    // and nothing else. Watch() resolves each portable name once, up front — a name the adapter
    // cannot map is logged once and skipped, never re-parsed per frame.
    //
    //   key.<name>            KeyCode by name: key.space, key.w, key.left-shift, key.up, key.enter
    //                         (a composite2d binding watches its four parts; its own label is not polled)
    //   mouse.left|right|middle
    //   pad.south|east|west|north|left-shoulder|right-shoulder|select|start
    //                         legacy joystick buttons 0..7 (Xbox layout; the mapping is per-OS in
    //                         the legacy manager, which is why a real pad wants the Input System)
    //   pad.<axis>[.x|.y]     an Input Manager axis registered with SetAxisName; unregistered = none
    //   pointer.*             not polled per name: every touch (or the mouse, without touches) is
    //                         pushed through OnPointer in design space
    //   touch.*               virtual controls fed by the core's own on-screen stick; ignored here
    public class UnityInput : IHostInput {

        public const int keyboardDevice = 1;
        public const int mouseDevice = 2;
        public const int gamepadDevice = 3;
        public const int touchDevice = 4;

        private enum Source {
            key,
            mouseButton,
            axis
        }

        private class Watched {
            public string control;
            public Source source;
            public KeyCode key;
            public int mouseButton;
            public string axisName;
            public int device;
            public bool down;
            public float value;
        }

        private readonly List<Watched> watched = new List<Watched>();
        private readonly HashSet<string> watchedNames = new HashSet<string>(StringComparer.Ordinal);
        private readonly Dictionary<string, string> axisNames = new Dictionary<string, string>(StringComparer.Ordinal);
        private readonly HashSet<int> activeTouches = new HashSet<int>();
        private readonly List<int> endedBuffer = new List<int>();
        private readonly IHostDisplay display;
        private readonly IHostLog log;

        private IRawInputSink sink;
        private bool mouseDown;
        private Vector3 lastMouse;
        private int lastJoystickCount = -1;
        private float nextJoystickCheck;

        // Input.GetJoystickNames allocates an array, so device changes are checked on this cadence
        // rather than every frame. A pad plugged in mid-round is noticed within a second.
        public float joystickCheckSeconds = 1f;

        // Mouse position is only pushed as pointer 0 while no touch is active, so a touch screen
        // that also synthesises mouse events does not report every tap twice.
        public bool mouseAsPointer = true;

        public UnityInput(IHostDisplay display, IHostLog log) {
            this.display = display;
            this.log = log;
        }

        public void SetSink(IRawInputSink sink) {
            this.sink = sink;
        }

        public int GetDevices(List<InputDevice> results) {

            if (results == null) {
                return 0;
            }

            results.Clear();
            results.Add(Device(keyboardDevice, DeviceClass.keyboard));

            if (Input.mousePresent) {
                results.Add(Device(mouseDevice, DeviceClass.mouse));
            }

            if (Input.touchSupported) {
                results.Add(Device(touchDevice, DeviceClass.touch));
            }

            if (JoystickCount() > 0) {
                results.Add(Device(gamepadDevice, DeviceClass.gamepad));
            }

            return results.Count;
        }

        // Registers an Input Manager axis for a portable axis control, e.g.
        // SetAxisName("pad.left-stick.x", "Pad Left X"). Call before Watch.
        public void SetAxisName(string control, string inputManagerAxis) {

            if (!string.IsNullOrEmpty(control)) {
                axisNames[control] = inputManagerAxis;
            }
        }

        // Watches every control a map binds, including composite parts and the .x/.y halves of a
        // 2D control.
        public void WatchMap(ActionMapDef def) {

            if (def == null) {
                return;
            }

            for (int s = 0; s < def.actionSets.Count; s++) {
                List<ActionDef> actions = def.actionSets[s].actions;

                for (int a = 0; a < actions.Count; a++) {
                    List<BindingDef> bindings = actions[a].bindings;

                    for (int b = 0; b < bindings.Count; b++) {
                        BindingDef bd = bindings[b];

                        if (bd.composite2d != null) {
                            Watch(bd.composite2d.up);
                            Watch(bd.composite2d.down);
                            Watch(bd.composite2d.left);
                            Watch(bd.composite2d.right);
                            continue;
                        }

                        if (actions[a].type == ActionType.axis2d) {
                            Watch(bd.control + ".x");
                            Watch(bd.control + ".y");
                        }

                        Watch(bd.control);
                    }
                }
            }
        }

        public bool Watch(string control) {

            if (string.IsNullOrEmpty(control) || watchedNames.Contains(control)) {
                return false;
            }

            watchedNames.Add(control);

            if (control.StartsWith("pointer.", StringComparison.Ordinal)
                || control.StartsWith("touch.", StringComparison.Ordinal)) {
                return false;
            }

            string axis;

            if (axisNames.TryGetValue(control, out axis) && !string.IsNullOrEmpty(axis)) {

                if (!AxisExists(axis)) {
                    Log(LogLevel.warning, "axis '" + axis + "' for " + control + " is not in the Input Manager");
                    return false;
                }

                Add(control, Source.axis, KeyCode.None, 0, axis, gamepadDevice);
                return true;
            }

            if (control.StartsWith("mouse.", StringComparison.Ordinal)) {

                int button = control == "mouse.left" ? 0 : control == "mouse.right" ? 1 : control == "mouse.middle" ? 2 : -1;

                if (button >= 0) {
                    Add(control, Source.mouseButton, KeyCode.None, button, null, mouseDevice);
                    return true;
                }
            }

            KeyCode key;

            if (control.StartsWith("key.", StringComparison.Ordinal) && TryKey(control.Substring(4), out key)) {
                Add(control, Source.key, key, 0, null, keyboardDevice);
                return true;
            }

            if (control.StartsWith("pad.", StringComparison.Ordinal) && TryPadButton(control.Substring(4), out key)) {
                Add(control, Source.key, key, 0, null, gamepadDevice);
                return true;
            }

            // A 2D control's own name is only a prefix for its .x/.y halves; not an error.
            if (!axisNames.ContainsKey(control + ".x")
                && !control.EndsWith(".x", StringComparison.Ordinal)
                && !control.EndsWith(".y", StringComparison.Ordinal)
                && !control.EndsWith("-stick", StringComparison.Ordinal)) {
                Log(LogLevel.info, "no Unity mapping for control '" + control + "'");
            }

            return false;
        }

        public int watchedCount {
            get {
                return watched.Count;
            }
        }

        // Once a frame, before the core's OnFrame. Pushes changes only.
        public void Poll() {

            if (sink == null) {
                return;
            }

            for (int i = 0; i < watched.Count; i++) {
                Watched w = watched[i];

                switch (w.source) {
                    case Source.key: {
                            bool d = Input.GetKey(w.key);

                            if (d != w.down) {
                                w.down = d;
                                sink.OnButton(w.device, w.control, d);
                            }

                            break;
                        }
                    case Source.mouseButton: {
                            bool d = Input.GetMouseButton(w.mouseButton);

                            if (d != w.down) {
                                w.down = d;
                                sink.OnButton(w.device, w.control, d);
                            }

                            break;
                        }
                    case Source.axis: {
                            float v = Input.GetAxisRaw(w.axisName);

                            if (v != w.value) {
                                w.value = v;
                                sink.OnAxis(w.device, w.control, v);
                            }

                            break;
                        }
                }
            }

            PollPointers();

            float now = Time.unscaledTime;

            if (now < nextJoystickCheck) {
                return;
            }

            nextJoystickCheck = now + joystickCheckSeconds;
            int joysticks = JoystickCount();

            if (joysticks != lastJoystickCount) {

                if (lastJoystickCount >= 0) {
                    sink.OnDevicesChanged();
                }

                lastJoystickCount = joysticks;
            }
        }

        // Focus loss: report every held control as released, because the OS will not send the
        // key-up for a key that was down when the app went to the background.
        public void ReleaseAll() {

            for (int i = 0; i < watched.Count; i++) {
                Watched w = watched[i];

                if (w.down && sink != null) {
                    sink.OnButton(w.device, w.control, false);
                }

                if (w.value != 0f && sink != null) {
                    sink.OnAxis(w.device, w.control, 0f);
                }

                w.down = false;
                w.value = 0f;
            }

            if (sink != null) {
                foreach (int id in activeTouches) {
                    sink.OnPointer(id, PointerPhase.cancel, Vec2.zero);
                }

                if (mouseDown) {
                    sink.OnPointer(0, PointerPhase.cancel, ToDesign(lastMouse));
                }
            }

            activeTouches.Clear();
            mouseDown = false;
        }

        private void PollPointers() {

            int touches = Input.touchCount;

            if (touches > 0 || activeTouches.Count > 0) {

                endedBuffer.Clear();
                endedBuffer.AddRange(activeTouches);

                for (int i = 0; i < touches; i++) {
                    Touch t = Input.GetTouch(i);
                    Vec2 p = ToDesign(t.position);

                    // Touch ids are offset by one: pointer 0 is the mouse.
                    int id = t.fingerId + 1;
                    endedBuffer.Remove(id);

                    switch (t.phase) {
                        case TouchPhase.Began:
                            activeTouches.Add(id);
                            sink.OnPointer(id, PointerPhase.down, p);
                            break;
                        case TouchPhase.Moved:
                            sink.OnPointer(id, PointerPhase.move, p);
                            break;
                        case TouchPhase.Ended:
                            activeTouches.Remove(id);
                            sink.OnPointer(id, PointerPhase.up, p);
                            break;
                        case TouchPhase.Canceled:
                            activeTouches.Remove(id);
                            sink.OnPointer(id, PointerPhase.cancel, p);
                            break;
                    }
                }

                // A touch that vanished without an Ended phase (focus loss mid-drag) is cancelled.
                for (int i = 0; i < endedBuffer.Count; i++) {
                    activeTouches.Remove(endedBuffer[i]);
                    sink.OnPointer(endedBuffer[i], PointerPhase.cancel, Vec2.zero);
                }

                return;
            }

            if (!mouseAsPointer || !Input.mousePresent) {
                return;
            }

            Vector3 m = Input.mousePosition;
            bool down = Input.GetMouseButton(0);

            if (down && !mouseDown) {
                sink.OnPointer(0, PointerPhase.down, ToDesign(m));
            }
            else if (!down && mouseDown) {
                sink.OnPointer(0, PointerPhase.up, ToDesign(m));
            }
            else if (m != lastMouse) {
                sink.OnPointer(0, PointerPhase.move, ToDesign(m));
            }

            mouseDown = down;
            lastMouse = m;
        }

        // Pixels (bottom-left origin) -> design units (top-left origin, y down).
        private Vec2 ToDesign(Vector2 pixels) {

            DisplayInfo d = display != null ? display.GetDisplay() : default(DisplayInfo);
            float scale = d.designScale > 0f ? d.designScale : 1f;
            float height = d.pixelHeight > 0 ? d.pixelHeight : Screen.height;
            return new Vec2(pixels.x / scale, (height - pixels.y) / scale);
        }

        private void Add(string control, Source source, KeyCode key, int mouseButton, string axis, int device) {
            Watched w = new Watched();
            w.control = control;
            w.source = source;
            w.key = key;
            w.mouseButton = mouseButton;
            w.axisName = axis;
            w.device = device;
            watched.Add(w);
        }

        private static readonly Dictionary<string, KeyCode> keyAliases = new Dictionary<string, KeyCode>(StringComparer.Ordinal) {
            { "enter", KeyCode.Return },
            { "return", KeyCode.Return },
            { "esc", KeyCode.Escape },
            { "up", KeyCode.UpArrow },
            { "down", KeyCode.DownArrow },
            { "left", KeyCode.LeftArrow },
            { "right", KeyCode.RightArrow },
            { "left-ctrl", KeyCode.LeftControl },
            { "right-ctrl", KeyCode.RightControl },
            { "left-cmd", KeyCode.LeftCommand },
            { "right-cmd", KeyCode.RightCommand }
        };

        private static bool TryKey(string name, out KeyCode key) {

            key = KeyCode.None;

            if (string.IsNullOrEmpty(name)) {
                return false;
            }

            if (keyAliases.TryGetValue(name, out key)) {
                return true;
            }

            if (name.Length == 1 && name[0] >= '0' && name[0] <= '9') {
                key = KeyCode.Alpha0 + (name[0] - '0');
                return true;
            }

            // kebab-case -> PascalCase: "left-shift" -> "LeftShift", "f1" -> "F1".
            char[] chars = new char[name.Length];
            int n = 0;
            bool upper = true;

            for (int i = 0; i < name.Length; i++) {

                if (name[i] == '-') {
                    upper = true;
                    continue;
                }

                chars[n++] = upper ? char.ToUpperInvariant(name[i]) : name[i];
                upper = false;
            }

            return Enum.TryParse(new string(chars, 0, n), false, out key) && key != KeyCode.None;
        }

        private static bool TryPadButton(string name, out KeyCode key) {

            int index;

            switch (name) {
                case "south": index = 0; break;
                case "east": index = 1; break;
                case "west": index = 2; break;
                case "north": index = 3; break;
                case "left-shoulder": index = 4; break;
                case "right-shoulder": index = 5; break;
                case "select": index = 6; break;
                case "start": index = 7; break;
                default:
                    key = KeyCode.None;
                    return false;
            }

            key = KeyCode.JoystickButton0 + index;
            return true;
        }

        private static bool AxisExists(string axis) {

            try {
                Input.GetAxisRaw(axis);
                return true;
            }
            catch (ArgumentException) {
                return false;
            }
        }

        private static int JoystickCount() {

            string[] names = Input.GetJoystickNames();
            int n = 0;

            for (int i = 0; i < names.Length; i++) {
                if (!string.IsNullOrEmpty(names[i])) {
                    n++;
                }
            }

            return n;
        }

        private static InputDevice Device(int id, DeviceClass c) {
            InputDevice d;
            d.id = id;
            d.deviceClass = c;
            return d;
        }

        private void Log(LogLevel level, string message) {

            if (log != null && log.IsEnabled(level)) {
                log.Log(level, "host.input", message);
            }
        }
    }
}
