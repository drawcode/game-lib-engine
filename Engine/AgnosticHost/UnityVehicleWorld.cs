#if USE_AGNOSTIC_VEHICLE
using System;
using System.Collections.Generic;

using Agnostic.Core;
using Agnostic.Vehicle;

using UnityEngine;

namespace Engine.AgnosticHost {

    // IVehicleWorld over PhysX: the arcade vehicle core's suspension probes and hull sweep (arcade design
    // section 4). Everything crosses UnityFrame, so the core sees the canonical frame.
    //
    // - Queries fill preallocated hit buffers and keep the nearest hit that is not under an ignore root (the
    //   vehicle itself, its rider), so a step allocates nothing.
    // - Triggers are skipped (pickups and mounts never hold a vehicle up).
    // - A sweep that starts inside a collider is skipped rather than reported at distance 0: the core keeps a
    //   skin off walls, so an overlap means something moved into the hull, and blocking on it would pin the
    //   vehicle in place for good.
    // - Surface: the collider's PhysicsMaterial name looked up in `surfaceIds` (case-insensitive; unknown and
    //   no material = 0, "default"). Cached per material by reference, because Object.name allocates.
    //
    // Compiled only with USE_AGNOSTIC_VEHICLE (see VehicleTuningJson).
    public class UnityVehicleWorld : IVehicleWorld {

        public const int bufferSize = 16;

        public int mask = Physics.DefaultRaycastLayers;
        public QueryTriggerInteraction triggers = QueryTriggerInteraction.Ignore;
        public Transform ignore0;
        public Transform ignore1;

        private readonly RaycastHit[] hits = new RaycastHit[bufferSize];
        private readonly Dictionary<string, int> surfaceIds = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<UnityEngine.Object, int> materialIds = new Dictionary<UnityEngine.Object, int>(ReferenceComparer.instance);

        // id -> surface name, as VehicleTuningJson.ParseSurfaces lists them.
        public void SetSurfaces(List<string> names) {

            surfaceIds.Clear();
            materialIds.Clear();

            if (names == null) {
                return;
            }

            for (int i = 0; i < names.Count; i++) {
                surfaceIds[names[i]] = i;
            }
        }

        public bool Probe(Vec3 origin, Vec3 dir, float maxDist, out GroundHit hit) {
            int n = Physics.RaycastNonAlloc(UnityFrame.ToUnity(origin), UnityFrame.ToUnity(dir), hits, maxDist, mask, triggers);
            return Nearest(n, false, out hit);
        }

        // The capsule's segment runs along rot * (0, 0, -1) in the core, which is q * forward in Unity.
        public bool Sweep(float radius, float halfHeight, Vec3 from, Vec3 to, Quat rot, out GroundHit hit) {

            Vector3 a = UnityFrame.ToUnity(from);
            Vector3 delta = UnityFrame.ToUnity(to) - a;
            float distance = delta.magnitude;

            if (distance <= 0f) {
                hit = default(GroundHit);
                return false;
            }

            Vector3 axis = UnityFrame.ToUnity(rot) * Vector3.forward * halfHeight;
            int n = Physics.CapsuleCastNonAlloc(a - axis, a + axis, radius, delta / distance, hits, distance, mask, triggers);
            return Nearest(n, true, out hit);
        }

        private bool Nearest(int n, bool skipOverlaps, out GroundHit hit) {

            hit = default(GroundHit);
            int best = -1;
            float bestDistance = float.MaxValue;

            for (int i = 0; i < n; i++) {
                RaycastHit h = hits[i];

                if (h.distance >= bestDistance || Ignored(h.collider)) {
                    continue;
                }

                // CapsuleCast reports a starting overlap as distance 0 with a zero point.
                if (skipOverlaps && h.distance <= 0f && h.point == Vector3.zero) {
                    continue;
                }

                best = i;
                bestDistance = h.distance;
            }

            if (best < 0) {
                return false;
            }

            RaycastHit rh = hits[best];
            hit.point = UnityFrame.ToCore(rh.point);
            hit.normal = UnityFrame.ToCore(rh.normal);
            hit.distance = rh.distance;
            hit.surface = SurfaceOf(rh.collider);
            return true;
        }

        private bool Ignored(Collider c) {

            if (c == null) {
                return true;
            }

            Transform t = c.transform;
            return (ignore0 != null && t.IsChildOf(ignore0)) || (ignore1 != null && t.IsChildOf(ignore1));
        }

        private int SurfaceOf(Collider c) {

            PhysicsMaterial m = c.sharedMaterial;

            if (m == null || surfaceIds.Count <= 1) {
                return 0;
            }

            int id;

            if (!materialIds.TryGetValue(m, out id)) {
                id = surfaceIds.TryGetValue(m.name, out id) ? id : 0;
                materialIds[m] = id;
            }

            return id;
        }
    }
}
#endif
