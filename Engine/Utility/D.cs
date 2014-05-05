#define DEBUG_LEVEL_LOG
#define DEBUG_LEVEL_WARN
#define DEBUG_LEVEL_ERROR

using System.Collections;
using UnityEngine;

// setting the conditional to the platform of choice will only compile the method for that platform
// alternatively, use the #defines at the top of this file
public class D {

    [System.Diagnostics.Conditional("DEBUG_LEVEL_LOG")]
    [System.Diagnostics.Conditional("DEBUG_LEVEL_WARN")]
    [System.Diagnostics.Conditional("DEBUG_LEVEL_ERROR")]
    public static void log(object format, params object[] paramList) {
        if (format is string)
            LogUtil.Log(string.Format(format as string, paramList));
        else
            LogUtil.Log(format);
    }

    [System.Diagnostics.Conditional("DEBUG_LEVEL_WARN")]
    [System.Diagnostics.Conditional("DEBUG_LEVEL_ERROR")]
    public static void warn(object format, params object[] paramList) {
        if (format is string)
            LogUtil.LogWarning(string.Format(format as string, paramList));
        else
            LogUtil.LogWarning(format);
    }

    [System.Diagnostics.Conditional("DEBUG_LEVEL_ERROR")]
    public static void error(object format, params object[] paramList) {
        if (format is string)
            LogUtil.LogError(string.Format(format as string, paramList));
        else
            LogUtil.LogError(format);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    [System.Diagnostics.Conditional("DEBUG_LEVEL_LOG")]
    public static void assert(bool condition) {
        assert(condition, string.Empty, true);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    [System.Diagnostics.Conditional("DEBUG_LEVEL_LOG")]
    public static void assert(bool condition, string assertString) {
        assert(condition, assertString, false);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    [System.Diagnostics.Conditional("DEBUG_LEVEL_LOG")]
    public static void assert(bool condition, string assertString, bool pauseOnFail) {
        if (!condition) {
            LogUtil.LogError("assert failed! " + assertString);

            if (pauseOnFail)
                Debug.Break();
        }
    }
}