using System;
using Engine.Utility;
using UnityEngine;

public static class MaterialExtensions {

    public static Material LoadMaterialFromResources(
        this Material inst, string resourcesPath) {

        inst = MaterialUtil.LoadMaterialFromResources(resourcesPath);

        return inst;
    }

    public static PhysicsMaterial LoadPhysicMaterialFromResources(
        this PhysicsMaterial inst, string resourcesPath) {

        inst = MaterialUtil.LoadPhysicMaterialFromResources(resourcesPath);

        return inst;
    }
}