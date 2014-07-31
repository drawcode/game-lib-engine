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
        if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) {
            value = value.Substring(2);
        }
        return Int32.Parse(value, NumberStyles.HexNumber);
    }
    
    public static string UnescapeXML(this string s) {
        if (string.IsNullOrEmpty(s))
            return s;
        
        string returnString = s;
        returnString = returnString.Replace("&apos;", "'");
        returnString = returnString.Replace("&quot;", "\"");
        returnString = returnString.Replace("&gt;", ">");
        returnString = returnString.Replace("&lt;", "<");
        returnString = returnString.Replace("&amp;", "&");
        
        return returnString;
    }
    
    public static bool IsCompressed(this string val) {
        if(string.IsNullOrEmpty(val)) {
            return false;
        }

        return Compress.IsStringCompressed(val);
    }

    public static bool IsBase64(this string val) {
        return FormatUtil.IsStringBase64(val);
    }

    public static string ToBase64(this string val) {
        if(string.IsNullOrEmpty(val)) {
            return val;
        }
        return FormatUtil.StringToBase64(val);
    }
        
    public static string FromBase64(this string val) {
        if(string.IsNullOrEmpty(val)) {
            return val;
        }
        return FormatUtil.StringFromBase64(val);
    }

    public static byte[] FromBase64Bytes(this string val) {
        return FormatUtil.BytesFromBase64(val);
    }

    public static string ToCompressed(this string val) {
        
        if (string.IsNullOrEmpty(val)) {
            return val;
        }

        if (val.IsCompressed()) {
            return Compress.CompressString(val);
        }
        
        return val;
    }
    
    public static string ToDecompressed(this string val) {
        
        if (string.IsNullOrEmpty(val)) {
            return val;
        }
        
        if (val.IsCompressed() || val.IsBase64()) {
            return Compress.DecompressString(val);
        }
        
        return val;
    }

    // ENCRYPT

    public static string ToEncrypted(this string val) {
        
        if (string.IsNullOrEmpty(val)) {
            return val;
        }

        return CryptoUtil.EncryptStringAES(val);
    }
    
    public static string ToDecrypted(this string val) {
        
        if (string.IsNullOrEmpty(val)) {
            return val;
        }
        
        return CryptoUtil.DecryptStringAES(val);
    }
}