using System;
using System.Collections.Generic;

using Agnostic.Core;
using Agnostic.Host;
using Agnostic.Input;

using Engine.Data.Json;

using UnityEngine;

namespace Engine.AgnosticHost {

    // The boundary for input-action-map.v1: the core parses no JSON, so the Unity side maps the
    // document onto ActionMapDef with the house parser (LitJson). Same field rules as the vector
    // runner's ParseMap, so a map that passes the vectors loads identically here.
    //
    // Nothing throws across the boundary: a malformed document logs once and returns null, and the
    // caller keeps its legacy input path.
    public static class ActionMapJson {

        public static ActionMapDef Parse(string json, IHostLog log = null) {

            if (string.IsNullOrEmpty(json)) {
                return null;
            }

            try {
                return Map(JsonMapper.ToObject(json));
            }
            catch (Exception e) {

                if (log != null) {
                    log.Log(LogLevel.error, "input", "input-action-map rejected: " + e.Message);
                }

                return null;
            }
        }

        // Resources path without extension, e.g. "agnostic/input-action-map". Missing is not an
        // error: the asset's presence is how a game opts in.
        public static ActionMapDef LoadResource(string path, IHostLog log = null) {

            TextAsset asset = Resources.Load<TextAsset>(path);

            if (asset == null) {
                return null;
            }

            ActionMapDef def = Parse(asset.text, log);
            Resources.UnloadAsset(asset);
            return def;
        }

        private static ActionMapDef Map(JsonData doc) {

            ActionMapDef def = new ActionMapDef();

            if (Has(doc, "version")) {
                def.version = (int)Number(doc["version"]);
            }

            JsonData sets = doc["actionSets"];

            for (int s = 0; s < sets.Count; s++) {
                JsonData sd = sets[s];
                ActionSetDef set = new ActionSetDef();
                set.id = (string)sd["id"];

                JsonData actions = sd["actions"];

                for (int a = 0; a < actions.Count; a++) {
                    set.actions.Add(MapAction(actions[a]));
                }

                def.actionSets.Add(set);
            }

            return def;
        }

        private static ActionDef MapAction(JsonData a) {

            ActionDef action = new ActionDef();
            action.id = (string)a["id"];
            action.type = (ActionType)Enum.Parse(typeof(ActionType), (string)a["type"]);

            if (Has(a, "rebindable")) {
                action.rebindable = (bool)a["rebindable"];
            }

            if (Has(a, "deadZone")) {
                action.deadZone = Number(a["deadZone"]);
            }

            if (Has(a, "sensitivity")) {
                action.sensitivity = Number(a["sensitivity"]);
            }

            if (Has(a, "invert")) {
                JsonData inv = a["invert"];
                action.invertX = Has(inv, "x") && (bool)inv["x"];
                action.invertY = Has(inv, "y") && (bool)inv["y"];
            }

            if (Has(a, "responseCurve")) {
                JsonData c = a["responseCurve"];
                ResponseCurve curve = new ResponseCurve();
                curve.kind = (CurveKind)Enum.Parse(typeof(CurveKind), (string)c["kind"]);

                if (Has(c, "points")) {
                    JsonData points = c["points"];

                    for (int p = 0; p < points.Count; p++) {
                        curve.points.Add(new Vec2(Number(points[p][0]), Number(points[p][1])));
                    }
                }

                action.responseCurve = curve;
            }

            JsonData bindings = a["bindings"];

            for (int b = 0; b < bindings.Count; b++) {
                JsonData bd = bindings[b];
                BindingDef binding = new BindingDef();
                binding.device = ParseDevice((string)bd["device"]);
                binding.control = (string)bd["control"];

                if (Has(bd, "composite2d")) {
                    JsonData c = bd["composite2d"];
                    Composite2d comp = new Composite2d();
                    comp.up = (string)c["up"];
                    comp.down = (string)c["down"];
                    comp.left = (string)c["left"];
                    comp.right = (string)c["right"];
                    binding.composite2d = comp;
                }

                action.bindings.Add(binding);
            }

            return action;
        }

        // The schema spells it "virtual-stick"; the enum cannot carry a hyphen.
        private static BindingDevice ParseDevice(string name) {

            if (name == "virtual-stick") {
                return BindingDevice.virtualStick;
            }

            return (BindingDevice)Enum.Parse(typeof(BindingDevice), name);
        }

        // LitJson's indexer throws on a missing key, and a JSON null arrives as a null entry.
        private static bool Has(JsonData d, string key) {
            return d != null && d.IsObject && d.Keys.Contains(key) && d[key] != null;
        }

        // LitJson keeps 1 as an int and 0.15 as a double, and each cast only accepts its own kind.
        private static float Number(JsonData d) {

            if (d.IsDouble) {
                return (float)(double)d;
            }

            if (d.IsLong) {
                return (long)d;
            }

            return (int)d;
        }
    }
}
