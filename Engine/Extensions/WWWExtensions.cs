using System;
using UnityEngine;

#if !UNITY_FLASH

public static class WWWExtensions {

    //public static bool HasError(this WWW inst) {
    //    string length;
    //    bool hasLength = inst.responseHeaders.TryGetValue("CONTENT-LENGTH", out length);
    //    int responseHeaderCount = inst.responseHeaders.Count;

    //    return (hasLength && length == "0") ||
    //        (responseHeaderCount > 0 && !hasLength) ||
    //        inst.error != null;
    //}
}

#endif