using System;
using System.Collections;
using UnityEngine;

public static class StringExtensions {

    public static string pathForBundleResource(string file) {
        var path = Application.dataPath.Replace("Data", "");
        return PathUtil.Combine(path, file);
    }

    public static string pathForDocumentsResource(string file) {
        return PathUtil.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), file);
    }
}