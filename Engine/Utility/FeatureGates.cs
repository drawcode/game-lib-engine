using System;
using System.Collections.Generic;

using UnityEngine;

// Named on/off switches for features that ship dark or get turned on piece by piece (first users:
// Action Bots' "toy" touches, 2026-10-28, owner: "do those in a separate push so we can review
// them ... so we can turn them on and off like feature gates which we probably need to start
// integrating").
//
// Game-agnostic and additive: until a game calls Load, nothing is managed and IsOn returns the
// caller's default, so every existing behaviour stays as it was. A game ships its gates as a JSON
// TextAsset under Resources:
//
//   { "gates": [ { "key": "toy-planet-tap-wobble", "on": true }, ... ] }
//
// (a list, not an object, so JsonUtility reads it). A key the file does not name keeps the
// caller's default; a missing or unreadable file manages nothing. Set() overrides at runtime
// (tests, probes, a future settings or remote-config layer) and wins over the file.
//
// Reads are a dictionary lookup on a string key: no allocation, fine per frame. Read a gate where
// the feature starts (Init, OnEnable, the input handler); a gate flipped later takes effect the
// next time that code asks.
//
// Relation to the older switches: CommunityFeatures (game-lib-games-ui) is the same idea for one
// feature's pieces, and the static-bool kill switches (UIPlatform.toolkitViewsEnabled, ...) guard
// migrations in code. New optional features should use a gate here.
public static class FeatureGates {

    [Serializable]
    public class Gate {
        public string key;
        public bool on;
    }

    [Serializable]
    public class Data {
        public List<Gate> gates = new List<Gate>();
    }

    static readonly Dictionary<string, bool> fromData = new Dictionary<string, bool>();
    static readonly Dictionary<string, bool> overrides = new Dictionary<string, bool>();

    // True once a game has loaded its gate file (even an empty or missing one).
    public static bool managed { get; private set; }

    public static string loadedFrom { get; private set; }

    public static bool IsOn(string key, bool defaultValue = false) {

        if (string.IsNullOrEmpty(key)) {
            return defaultValue;
        }

        bool on;

        if (overrides.TryGetValue(key, out on)) {
            return on;
        }

        if (fromData.TryGetValue(key, out on)) {
            return on;
        }

        return defaultValue;
    }

    public static void Set(string key, bool on) {
        if (!string.IsNullOrEmpty(key)) {
            overrides[key] = on;
        }
    }

    public static void ClearOverride(string key) {
        if (!string.IsNullOrEmpty(key)) {
            overrides.Remove(key);
        }
    }

    public static void ClearOverrides() {
        overrides.Clear();
    }

    // Every key the file or an override names, for probes and a future settings list.
    public static List<string> Keys() {

        List<string> keys = new List<string>(fromData.Keys);

        foreach (string key in overrides.Keys) {
            if (!keys.Contains(key)) {
                keys.Add(key);
            }
        }

        keys.Sort(StringComparer.Ordinal);
        return keys;
    }

    public static void Load(string resourcePath) {

        managed = true;
        loadedFrom = resourcePath;
        fromData.Clear();

        TextAsset asset = Resources.Load<TextAsset>(resourcePath);

        if (asset == null) {
            Debug.LogWarning("FeatureGates: no data at Resources/" + resourcePath + " - every gate keeps its default");
            return;
        }

        LoadJson(asset.text, resourcePath);
    }

    public static void LoadJson(string json, string source = "json") {

        managed = true;
        fromData.Clear();

        if (string.IsNullOrEmpty(json)) {
            return;
        }

        Data data = null;

        try {
            data = JsonUtility.FromJson<Data>(json);
        }
        catch (Exception e) {
            Debug.LogError("FeatureGates: unreadable " + source + ": " + e.Message);
        }

        if (data == null || data.gates == null) {
            return;
        }

        foreach (Gate gate in data.gates) {
            if (gate != null && !string.IsNullOrEmpty(gate.key)) {
                fromData[gate.key] = gate.on;
            }
        }
    }

    // Tests: back to unmanaged.
    public static void Reset() {
        managed = false;
        loadedFrom = null;
        fromData.Clear();
        overrides.Clear();
    }
}
