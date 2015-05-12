using System;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
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
        if (string.IsNullOrEmpty(val)) {
            return false;
        }

        return Compress.IsStringCompressed(val);
    }

    public static bool IsBase64(this string val) {
        return FormatUtil.IsStringBase64(val);
    }

    public static string ToBase64(this string val) {
        if (string.IsNullOrEmpty(val)) {
            return val;
        }
        return FormatUtil.StringToBase64(val);
    }
        
    public static string FromBase64(this string val) {
        if (string.IsNullOrEmpty(val)) {
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

        if (!val.IsCompressed()) {
            return Compress.CompressString(val);
        }
        
        return val;
    }
    
    public static string ToDecompressed(this string val) {
        
        if (string.IsNullOrEmpty(val)) {
            return val;
        }
        
        if (val.IsCompressed()) {
            return Compress.DecompressString(val);
        }
        
        return val;
    }

    // ENCRYPT

    public static string ToEncrypted(this string val) {
        
        if (string.IsNullOrEmpty(val)) {
            return val;
        }

        return CryptoUtil.EncryptStringAES(val).ToBase64();
    }
    
    public static string ToDecrypted(this string val) {
        
        if (string.IsNullOrEmpty(val)) {
            return val;
        }
        
        return CryptoUtil.DecryptStringAES(val.FromBase64());
    }

    // REGEX

    public static bool RegexIsMatch(this string val, string regex) {
        
        return RegexUtil.RegexIsMatch(val, regex);
        
    }

    public static MatchCollection RegexMatches(this string val, string regex) {

        return RegexUtil.RegexMatches(val, regex);

    }

    public static string RegexMatchesReplace(this string val, string regex, string replacement) {
        return RegexUtil.RegexReplace(val, regex, replacement);
    }

    // 
    public static string LineBreaksToHtml(this string val) {
        if (!string.IsNullOrEmpty(val)) {
            val = val.Replace("\\r\\n", "<br>");
            val = val.Replace("\\r", "<br>");
            val = val.Replace("\\n", "<br>");
        }
        return val;
    }
    
    public static string ToBase36(this string val) {
        return FormatUtil.ConvertToBase36(val);
    }
    
    public static string ToPascalCase(this string val) {
        string output = "";
        char[] chars = val.ToCharArray();
        bool firstUpper = true;
        bool upperNext = false;
        foreach (char ch in chars) {
            if (char.IsUpper(ch)) {
                if (firstUpper) {
                    firstUpper = false;
                    output += ch.ToString().ToLower();
                    
                }
                else if (ch == '-' || ch == '_') {
                    // skip but cap next
                    upperNext = true;
                }
                else {
                    if (upperNext) {
                        upperNext = false;
                        output += ch.ToString().ToUpper();
                    }
                }
            }
            else {
                output += ch.ToString();
            }
        }
        return output;
    }
    
    //public static string ToTitleCase(this string val) {
    //    return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(val);
    //}
    
    public static string ToDelimitedUnderscore(this string val) {
        return val.ToDelimited("_");
    }
    
    public static string ToDelimitedDashed(this string val) {
        return val.ToDelimited("-");
    }
        
    public static string ToDelimited(this string val, string delimiter = "-") {
        string output = "";
        char[] chars = val.ToCharArray();
        foreach (char ch in chars) {
            if (char.IsUpper(ch)) {
                if (output.Length > 0) {
                    output += delimiter;
                }
                output += ch.ToString().ToLower();
            }
            else {
                output += ch.ToString();
            }
        }
        return output;
    }
    
    public static string ToNonDelimited(this string val, string replaceDelimeter = " ", string delimiter = "-") {
        if (val == null) {
            return null;
        }
        val = val.Replace(delimiter, replaceDelimeter);
        return val;
    }

    public static string StripToAlphanumerics(this string input) {
        string output = "";
        output = Regex.Replace(input, @"[^\w\s _-]|", "");
        //.replace(/\s+/g, " ");
        return output;
    }
    
    public static bool IsRegexMatch(this string input, string pattern) {
        if (string.IsNullOrEmpty(input)) {
            return false;
        }
        
        //^[A-Z][a-z]*( [A-Z][a-z]*)*$
        if (Regex.IsMatch(input, pattern)) {
            return true;
        }
        return false;
    }
    
    public static bool EndsWithSlash(this string input) {
        return input.IsRegexMatch("/$");
    }
    
    public static Match RegexMatch(this string input, string pattern) {
        return Regex.Match(input, pattern);
    }
    
    public static string Substring(this string str, string StartString, string EndString) {
        if (str.Contains(StartString)) {
            int iStart = str.IndexOf(StartString) + StartString.Length;
            int iEnd = str.IndexOf(EndString, iStart);
            return str.Substring(iStart, (iEnd - iStart));
        }
        return null;
    }
    
    public static string ToHashMD5(this string val) {
        return CryptoUtil.HashMD5(val);
    }
    
    public static bool IsNullOrEmpty(this string val) {
        return string.IsNullOrEmpty(val);
    }

}