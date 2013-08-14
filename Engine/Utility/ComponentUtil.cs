using System;
using System.Reflection;
using UnityEngine;

public static class ComponentUtil {

    public static void Copy(Component dst, Component src) {

        //Copy all fields from source to destination
        foreach (FieldInfo f in src.GetType().GetFields()) {
            var val = f.GetValue(src);
            if (val != null)
                f.SetValue(dst, val);
        }
    }
}