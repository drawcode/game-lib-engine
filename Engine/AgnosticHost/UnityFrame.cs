using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Agnostic.Core;

using UnityEngine;

namespace Engine.AgnosticHost {

    // The canonical frame (glTF: right-handed, Y-up, metres, forward = -Z) to Unity's
    // (left-handed, Y-up, metres, forward = +Z), and back.
    //
    // The conversion is a mirror across the XY plane: z flips. Unity's forward (+Z) becomes the
    // canonical forward (-Z), so a character facing "forward" in a Unity prefab faces forward in
    // the core too, and Godot (already RH Y-up, forward -Z) needs no conversion at all.
    //
    // A mirror applied to a rotation keeps the rotation's component along the mirror normal and
    // negates the two in the plane: (x, y, z, w) -> (-x, -y, z, w). It is its own inverse, so the
    // same two lines go both ways. Never cast a Quat to a Quaternion: both are xyzw, and a cast
    // compiles, runs and turns every character the wrong way round.
    public static class UnityFrame {

        public static Vector3 ToUnity(Vec3 v) {
            return new Vector3(v.x, v.y, -v.z);
        }

        public static Vec3 ToCore(Vector3 v) {
            return new Vec3(v.x, v.y, -v.z);
        }

        // Scale is a magnitude per axis, not a direction, so it does not mirror.
        public static Vector3 ScaleToUnity(Vec3 v) {
            return new Vector3(v.x, v.y, v.z);
        }

        public static Vec3 ScaleToCore(Vector3 v) {
            return new Vec3(v.x, v.y, v.z);
        }

        public static Quaternion ToUnity(Quat q) {
            return new Quaternion(-q.x, -q.y, q.z, q.w);
        }

        public static Quat ToCore(Quaternion q) {
            return new Quat(-q.x, -q.y, q.z, q.w);
        }

        // Both sides are sRGB-authored: a Unity Color set on a material or a UI element is the
        // value an artist picked, and Unity linearises it itself in linear colour space.
        public static Color ToUnity(ColorRgba c) {
            return new Color(c.r, c.g, c.b, c.a);
        }

        public static ColorRgba ToCore(Color c) {
            return new ColorRgba(c.r, c.g, c.b, c.a);
        }

        public static Vector2 ToUnity(Vec2 v) {
            return new Vector2(v.x, v.y);
        }

        public static Vec2 ToCore(Vector2 v) {
            return new Vec2(v.x, v.y);
        }
    }

    // Reference identity for engine objects. UnityEngine.Object's own Equals treats every destroyed
    // object as equal to null (and so to each other), which is wrong for a table key; and
    // GetInstanceID is a compile error from Unity 6.5.
    public sealed class ReferenceComparer : IEqualityComparer<Object> {

        public static readonly ReferenceComparer instance = new ReferenceComparer();

        public bool Equals(Object a, Object b) {
            return ReferenceEquals(a, b);
        }

        public int GetHashCode(Object o) {
            return RuntimeHelpers.GetHashCode(o);
        }
    }

    // Handle <-> UnityEngine.Object. Ids come from a counter, not the engine's object id, so they are
    // positive, never 0 (Handle.none) and never reused within a session.
    //
    // Alive means "registered and not destroyed". Removal is explicit on Destroy, because
    // Object.Destroy only lands at the end of the frame and the contract says a destroyed handle
    // answers deadHandle immediately. Anything the engine destroys behind the core's back is
    // caught by the Unity null check and pruned on the next lookup.
    public class UnityHandleTable {

        private readonly Dictionary<int, Object> byId = new Dictionary<int, Object>();
        private readonly Dictionary<Object, int> byObject = new Dictionary<Object, int>(ReferenceComparer.instance);
        private int nextId = 1;

        public int count {
            get {
                return byId.Count;
            }
        }

        // Returns the existing handle for an object already tracked, so a raycast hitting the
        // same collider twice reports the same handle both times.
        public Handle Track(Object obj) {

            if (obj == null) {
                return Handle.none;
            }

            int id;

            if (byObject.TryGetValue(obj, out id)) {
                Object existing;

                if (byId.TryGetValue(id, out existing) && existing != null) {
                    return new Handle(id);
                }

                Remove(id);
            }

            id = nextId++;
            byId[id] = obj;
            byObject[obj] = id;
            return new Handle(id);
        }

        public bool TryGet<T>(Handle h, out T obj) where T : Object {

            obj = null;
            Object o;

            if (!h.isSome || !byId.TryGetValue(h.id, out o)) {
                return false;
            }

            if (o == null) {
                Remove(h.id);
                return false;
            }

            obj = o as T;
            return obj != null;
        }

        public bool TryGetGameObject(Handle h, out GameObject go) {

            go = null;
            Object o;

            if (!h.isSome || !byId.TryGetValue(h.id, out o)) {
                return false;
            }

            if (o == null) {
                Remove(h.id);
                return false;
            }

            go = o as GameObject;

            if (go == null) {
                Component c = o as Component;

                if (c != null) {
                    go = c.gameObject;
                }
            }

            return go != null;
        }

        public bool Alive(Handle h) {
            Object o;
            return TryGet(h, out o);
        }

        public bool Release(Handle h) {
            return Remove(h.id);
        }

        private readonly List<Transform> hierarchyBuffer = new List<Transform>();

        // Releases every tracked object at or below root. Object.Destroy takes the children with
        // it at the end of the frame, but the contract (and the null host) says a destroyed
        // parent's children answer deadHandle from the moment Destroy returns.
        public int ReleaseHierarchy(GameObject root) {

            if (root == null) {
                return 0;
            }

            int released = 0;
            root.GetComponentsInChildren(true, hierarchyBuffer);

            for (int i = 0; i < hierarchyBuffer.Count; i++) {
                int id;
                GameObject go = hierarchyBuffer[i].gameObject;

                if (byObject.TryGetValue(go, out id) && Remove(id)) {
                    released++;
                }
            }

            hierarchyBuffer.Clear();
            return released;
        }

        private bool Remove(int id) {

            Object o;

            if (!byId.TryGetValue(id, out o)) {
                return false;
            }

            byId.Remove(id);

            // Keyed by reference, so the reverse entry is found even after the native object is
            // gone and `o == null` already reads true.
            if ((object)o != null) {
                byObject.Remove(o);
            }

            return true;
        }
    }
}
