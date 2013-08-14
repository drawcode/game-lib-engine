using System;
using UnityEngine;

public static class MaterialUtil {

    public static bool IsEqual(string sourceName, Material mat) {
        return mat.name == sourceName || mat.name == sourceName + " (Instance)";
    }

    public static string SourceNameOf(Material mat) {
        if (mat.name.EndsWith(" (Instance)"))
            return mat.name.Substring(0, mat.name.Length - 11);
        return mat.name;
    }

    public static Material LoadMaterialFromResources(string resourcesPath) {
        return (Material)Resources.Load(resourcesPath);
    }

    public static PhysicMaterial LoadPhysicMaterialFromResources(string resourcesPath) {
        return (PhysicMaterial)Resources.Load(resourcesPath);
    }
}