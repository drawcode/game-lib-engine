using System;
using UnityEngine;

public static class PhysicMaterialUtil {

    public static bool IsEqual(string sourceName, PhysicMaterial mat) {
        return mat.name == sourceName || mat.name == sourceName + " (Instance)";
    }

    public static string SourceNameOf(PhysicMaterial mat) {
        if (mat.name.EndsWith(" (Instance)"))
            return mat.name.Substring(0, mat.name.Length - 11);
        return mat.name;
    }
}