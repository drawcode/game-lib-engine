using System;
using System.Collections;
using System.Globalization;
using System.Text;
using UnityEngine;

public static class StringExtensions {

    public static string pathForBundleResource(string file) {
        var path = Application.dataPath.Replace("Data", "");
        return PathUtil.Combine(path, file);
    }

    public static string pathForDocumentsResource(string file) {
        return PathUtil.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), file);
    }

    public static string ToHex(this int value) {
        return String.Format("{0:X}", value);
    }

    public static string ToHexPadded(this int value) {
        return String.Format("0x{0:X}", value);
    }

    public static int FromHex(string value) {
        // strip the leading 0x
        if ( value.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) {
            value = value.Substring(2);
        }
        return Int32.Parse(value, NumberStyles.HexNumber);
    }
}