#if USE_AGNOSTIC_VEHICLE
using System.Collections.Generic;

using Agnostic.Vehicle;

using Engine.Data.Json;

namespace Engine.AgnosticHost {

    // The boundary for vehicle-tuning.v1 and the game-vehicle-data `control` and `surfaces` blocks: the core
    // parses no JSON and the Unity profile has no System.Text.Json, so the host maps them with the house parser
    // (LitJson). Same field rules and defaults as the vector runner's VehicleVectors.ParseTuning / ParseControl,
    // so a tuning that passes the vehicle.* vectors loads identically here.
    //
    // Compiled only with USE_AGNOSTIC_VEHICLE: Agnostic.Vehicle is in game-lib-bitty-base from
    // feature/agnostic-vehicle on, and a title pinned to an older bitty-base still compiles this lib.
    public static class VehicleTuningJson {

        // vehicle-tuning.v1 -> VehicleTuning. Missing optional fields keep VehicleTuning.Defaults().
        public static VehicleTuning ParseTuning(JsonData n) {

            VehicleTuning t = VehicleTuning.Defaults();

            if (n == null || !n.IsObject) {
                return t;
            }

            t.mass = Num(n, "mass", t.mass);
            t.maxSpeed = Num(n, "maxSpeed", t.maxSpeed);
            t.maxReverse = Num(n, "maxReverse", t.maxReverse);
            t.accel = Has(n, "accel") ? Curve(n["accel"]) : t.accel;
            t.brakeDecel = Num(n, "brakeDecel", t.brakeDecel);
            t.dragLinear = Num(n, "dragLinear", t.dragLinear);
            t.dragQuad = Num(n, "dragQuad", t.dragQuad);
            t.rolling = Num(n, "rolling", t.rolling);
            t.steerRate = Has(n, "steerRate") ? Curve(n["steerRate"]) : t.steerRate;
            t.steerResponse = Num(n, "steerResponse", t.steerResponse);
            t.steerMinSpeed = Num(n, "steerMinSpeed", t.steerMinSpeed);
            t.grip = Num(n, "grip", t.grip);
            t.driftGrip = Num(n, "driftGrip", t.driftGrip);
            t.driftMinSpeed = Num(n, "driftMinSpeed", t.driftMinSpeed);
            t.driftExitSlip = Num(n, "driftExitSlip", t.driftExitSlip);
            t.driftSteerMul = Num(n, "driftSteerMul", t.driftSteerMul);

            if (Has(n, "probes") && n["probes"].IsArray) {
                JsonData probes = n["probes"];

                for (int i = 0; i < probes.Count; i++) {
                    t.AddProbe(Num(probes[i], "x", 0f), Num(probes[i], "z", 0f));
                }
            }

            t.restLength = Num(n, "restLength", t.restLength);
            t.travel = Num(n, "travel", t.travel);
            t.spring = Num(n, "spring", t.spring);
            t.damper = Num(n, "damper", t.damper);
            t.minGroundedProbes = (int)Num(n, "minGroundedProbes", t.minGroundedProbes);
            t.alignRate = Num(n, "alignRate", t.alignRate);
            t.airAlignRate = Num(n, "airAlignRate", t.airAlignRate);
            t.gravityMul = Num(n, "gravityMul", t.gravityMul);
            t.jumpImpulse = Num(n, "jumpImpulse", t.jumpImpulse);
            t.airPitchRate = Num(n, "airPitchRate", t.airPitchRate);
            t.airRollRate = Num(n, "airRollRate", t.airRollRate);
            t.airYawRate = Num(n, "airYawRate", t.airYawRate);
            t.landPerfect = Num(n, "landPerfect", t.landPerfect);
            t.landBail = Num(n, "landBail", t.landBail);
            t.bailImpact = Num(n, "bailImpact", t.bailImpact);
            t.wallFriction = Num(n, "wallFriction", t.wallFriction);
            t.respawnDelay = Num(n, "respawnDelay", t.respawnDelay);
            t.boostAccel = Num(n, "boostAccel", t.boostAccel);
            t.boostMaxMul = Num(n, "boostMaxMul", t.boostMaxMul);
            t.boostDrain = Num(n, "boostDrain", t.boostDrain);
            t.bumpRadius = Num(n, "bumpRadius", t.bumpRadius);
            t.bumpRestitution = Num(n, "bumpRestitution", t.bumpRestitution);

            if (Has(n, "hull")) {
                JsonData hull = n["hull"];
                t.hullRadius = Num(hull, "radius", t.hullRadius);
                t.hullHalfHeight = Num(hull, "halfHeight", t.hullHalfHeight);
            }

            if (Has(n, "rig")) {
                JsonData rig = n["rig"];
                t.leanMax = Num(rig, "leanMax", t.leanMax);
                t.rollGain = Num(rig, "rollGain", t.rollGain);
                t.wheelie = Bool(rig, "wheelie") ? 1 : 0;
                t.wheelieMax = Num(rig, "wheelieMax", t.wheelieMax);
                t.wheelRadius = Num(rig, "wheelRadius", t.wheelRadius);
            }

            return t;
        }

        // game-vehicle-data `control` block (angles in degrees) -> ControlTuning. `wheelie` is the tuning's
        // rig.wheelie (a data row carries it there, not in the control block). The mode is the class default;
        // the caller overrides it with the player's choice (D8) through ResolveMode.
        public static ControlTuning ParseControl(JsonData n, int wheelie) {

            ControlTuning c = ControlTuning.Defaults();
            c.wheelie = wheelie;

            if (n == null || !n.IsObject) {
                return c;
            }

            c.mode = ResolveMode(Str(n, "mode"), (int)ControlMode.heading);
            c.steerGain = Num(n, "steerGain", c.steerGain);
            c.reverseAngle = Has(n, "reverseAngle") ? Num(n, "reverseAngle", 0f) * VehicleMath.degToRad : c.reverseAngle;
            c.wheelieAngle = Has(n, "wheelieAngle") ? Num(n, "wheelieAngle", 0f) * VehicleMath.degToRad : c.wheelieAngle;
            c.wheelieThrottle = Num(n, "wheelieThrottle", c.wheelieThrottle);
            c.wheelieMinSpeed = Num(n, "wheelieMinSpeed", c.wheelieMinSpeed);
            c.aimFireThreshold = Num(n, "aimFireThreshold", c.aimFireThreshold);
            return c;
        }

        // "heading" / "direct" -> ControlMode; anything else (the "" a player who has not picked stores) -> fallback.
        public static int ResolveMode(string mode, int fallback) {

            if (mode == "heading") {
                return (int)ControlMode.heading;
            }

            if (mode == "direct") {
                return (int)ControlMode.direct;
            }

            return fallback;
        }

        // A data row's `surfaces` object, { name: { grip, drag } }. "default" is always id 0 and the other
        // names take ids in document order. `names` gets the id -> name list, for the world's material map.
        public static SurfaceTable ParseSurfaces(JsonData n, List<string> names) {

            SurfaceTable table = SurfaceTable.Defaults();

            if (names != null) {
                names.Clear();
                names.Add("default");
            }

            if (n == null || !n.IsObject) {
                return table;
            }

            if (Has(n, "default")) {
                table.Set(0, Num(n["default"], "grip", 1f), Num(n["default"], "drag", 1f));
            }

            int id = 1;

            foreach (string key in n.Keys) {

                if (key == "default") {
                    continue;
                }

                if (!table.Set(id, Num(n[key], "grip", 1f), Num(n[key], "drag", 1f))) {
                    break;
                }

                if (names != null) {
                    names.Add(key);
                }

                id++;
            }

            return table;
        }

        public static VehicleRuleset ParseRuleset(JsonData n) {

            VehicleRuleset r = new VehicleRuleset();

            if (n != null && n.IsObject) {
                r.driftToBoost = Num(n, "driftToBoost", 0f);
                r.trickToBoost = Num(n, "trickToBoost", 0f);
                r.airToBoost = Num(n, "airToBoost", 0f);
            }

            return r;
        }

        private static Curve8 Curve(JsonData n) {

            if (!n.IsArray) {
                return Curve8.Constant(0f);
            }

            float[] xs = new float[n.Count];
            float[] ys = new float[n.Count];

            for (int i = 0; i < n.Count; i++) {
                xs[i] = Number(n[i][0], 0f);
                ys[i] = Number(n[i][1], 0f);
            }

            return Curve8.From(xs, ys);
        }

        public static bool Has(JsonData d, string key) {
            return d != null && d.IsObject && d.Keys.Contains(key) && d[key] != null;
        }

        public static string Str(JsonData d, string key) {
            return Has(d, key) && d[key].IsString ? (string)d[key] : "";
        }

        public static bool Bool(JsonData d, string key) {
            return Has(d, key) && d[key].IsBoolean && (bool)d[key];
        }

        public static float Num(JsonData d, string key, float fallback) {
            return Has(d, key) ? Number(d[key], fallback) : fallback;
        }

        // LitJson keeps 1 as an int and 0.15 as a double, and each cast only accepts its own kind.
        public static float Number(JsonData d, float fallback) {

            if (d == null) {
                return fallback;
            }

            if (d.IsDouble) {
                return (float)(double)d;
            }

            if (d.IsLong) {
                return (long)d;
            }

            if (d.IsInt) {
                return (int)d;
            }

            return fallback;
        }
    }
}
#endif
