using System;

using Agnostic.Core;
using Agnostic.Host;

using Engine.Data.Json;

using UnityEngine;

namespace Engine.AgnosticHost {

    // The boundary for perf-tiers.v1: the core parses no JSON, so the Unity side maps the document
    // onto PerfDef with the house parser (LitJson). Same field rules as the vector runner's
    // PerfVectors.ParseDef, so a document that passes the perf.* vectors loads identically here.
    //
    // Nothing throws across the boundary: a malformed document logs once and returns null, and the
    // caller leaves every quality setting as authored.
    public static class PerfTiersJson {

        public static PerfDef Parse(string json, IHostLog log = null) {

            if (string.IsNullOrEmpty(json)) {
                return null;
            }

            try {
                return Map(JsonMapper.ToObject(json));
            }
            catch (Exception e) {

                if (log != null) {
                    log.Log(LogLevel.error, "perf", "perf-tiers rejected: " + e.Message);
                }

                return null;
            }
        }

        // Resources path without extension, e.g. "agnostic/perf-tiers". Missing is not an error:
        // the asset's presence is how a game opts in.
        public static PerfDef LoadResource(string path, IHostLog log = null) {

            TextAsset asset = Resources.Load<TextAsset>(path);

            if (asset == null) {
                return null;
            }

            PerfDef def = Parse(asset.text, log);
            Resources.UnloadAsset(asset);
            return def;
        }

        private static PerfDef Map(JsonData doc) {

            PerfDef def = new PerfDef();

            if (Has(doc, "version")) {
                def.version = (int)Number(doc["version"], 1);
            }

            JsonData tiers = doc["tiers"];

            for (int i = 0; i < tiers.Count; i++) {
                JsonData td = tiers[i];
                PerfTierDef t = new PerfTierDef();
                t.id = (string)td["id"];

                if (Has(td, "features")) {
                    JsonData f = td["features"];

                    foreach (string key in f.Keys) {
                        t.features[key] = Number(f[key], 0.0);
                    }
                }

                def.tiers.Add(t);
            }

            if (Has(doc, "device")) {
                JsonData dev = doc["device"];
                def.defaultTier = Str(dev, "default");

                if (Has(dev, "rules")) {
                    JsonData rules = dev["rules"];

                    for (int i = 0; i < rules.Count; i++) {
                        JsonData rd = rules[i];
                        PerfDeviceRule r = new PerfDeviceRule();
                        r.tier = Str(rd, "tier");
                        r.platform = Str(rd, "platform");
                        r.minMemoryMB = Num(rd, "minMemoryMB", 0.0);
                        r.minGpuMemoryMB = Num(rd, "minGpuMemoryMB", 0.0);
                        r.minCores = Num(rd, "minCores", 0.0);
                        r.minShaderLevel = Num(rd, "minShaderLevel", 0.0);
                        def.rules.Add(r);
                    }
                }

                if (Has(dev, "caps")) {
                    JsonData caps = dev["caps"];

                    for (int i = 0; i < caps.Count; i++) {
                        JsonData cd = caps[i];
                        PerfDeviceCap c = new PerfDeviceCap();
                        c.field = Str(cd, "field");
                        c.match = Str(cd, "match");
                        c.tier = Str(cd, "tier");
                        def.caps.Add(c);
                    }
                }
            }

            if (Has(doc, "governor")) {
                JsonData g = doc["governor"];
                PerfGovernorSettings s = def.governor;
                s.targetFrameMs = Num(g, "targetFrameMs", s.targetFrameMs);
                s.smoothingSec = Num(g, "smoothingSec", s.smoothingSec);
                s.downRatio = Num(g, "downRatio", s.downRatio);
                s.upRatio = Num(g, "upRatio", s.upRatio);
                s.downAfterSec = Num(g, "downAfterSec", s.downAfterSec);
                s.upAfterSec = Num(g, "upAfterSec", s.upAfterSec);
                s.cooldownSec = Num(g, "cooldownSec", s.cooldownSec);
                s.probeWindowSec = Num(g, "probeWindowSec", s.probeWindowSec);
                s.backoffMax = Num(g, "backoffMax", s.backoffMax);
                s.thermalDown = Num(g, "thermalDown", s.thermalDown);
                s.thermalUpMax = Num(g, "thermalUpMax", s.thermalUpMax);
                s.workDownRatio = Num(g, "workDownRatio", s.workDownRatio);
                s.workUpRatio = Num(g, "workUpRatio", s.workUpRatio);
            }

            return def;
        }

        private static bool Has(JsonData d, string key) {
            return d != null && d.IsObject && d.Keys.Contains(key) && d[key] != null;
        }

        private static string Str(JsonData d, string key) {
            return Has(d, key) ? (string)d[key] : "";
        }

        private static double Num(JsonData d, string key, double fallback) {
            return Has(d, key) ? Number(d[key], fallback) : fallback;
        }

        // LitJson keeps 1 as an int and 0.15 as a double, and each cast only accepts its own kind.
        private static double Number(JsonData d, double fallback) {

            if (d.IsDouble) {
                return (double)d;
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
