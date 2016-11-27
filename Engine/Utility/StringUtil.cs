using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Text.RegularExpressions;

public class StringUtil {


    public static string GetKeyCombo(params string[] keys) {
        return Combine("-", keys);
    }

    public static string Dashed(params string[] keys) {
        return Combine("-", keys);
    }

    public static string Underscored(params string[] keys) {
        return Combine("_", keys);
    }

    public static string ParamsFiltered(params string[] keys) {
        return Combine("--", keys);
    }

    public static string Slashed(params string[] keys) {
        return Combine("/", keys);
    }

    public static string Combine(string delimiter = "-", params string[] keys) {

        StringBuilder sb = new StringBuilder();

        foreach (string key in keys) {

            if (key.IsNullOrEmpty()) {
                continue;
            }

            if (sb.Length > 0 && !string.IsNullOrEmpty(delimiter)) {
                sb.Append(delimiter);
            }
            sb.Append(key);
        }
        return sb.ToString();
    }

    public static List<object> ToLineList(string line, char quote, char delimiter) {

        if (!line.EndsWith(@"\n")) {
            line += '\n';
        }

        char[] chars = line.ToCharArray();

        bool inQuoted = false;

        List<object> results = new List<object>();

        StringBuilder sbItem = new StringBuilder();

        //char lastChar = '\0';

        bool isLookingForDelimiterInQuote = false;

        int charIndex = -1;

        foreach (char c in chars) {

            charIndex++;

            // If char is a quoted field and we aren't in quoted state
            // Set inQuoted and append char

            if (c == quote && !inQuoted) {
                inQuoted = true;

                // Start building this line
                sbItem.Append(c);
                //lastChar = c;

                continue;
            }

            if (inQuoted) {

                // If we are in a quoted state

                // If it is a quote then move into looking for delimiter state

                if (c == quote) {
                    if (charIndex < chars.Length - 1) {
                        isLookingForDelimiterInQuote = true;
                    }
                }

                    // If we are looking for delimiter
                    // Check until we find a space, delimiter or 
                    // or exit delimiter find state

                    else if (isLookingForDelimiterInQuote) {

                    bool lineEnd = false;

                    // If we find delimiter then end this line

                    if (c == delimiter
                            || charIndex == chars.Length - 1) {
                        inQuoted = false;
                        lineEnd = true;
                    }

                    if (lineEnd) {
                        // End item
                        results.Add(sbItem.ToString());
                        sbItem.Length = 0;
                        //lastChar = c;
                        isLookingForDelimiterInQuote = false;
                        continue;
                    }

                    // If not then end find state

                    //if (c == quote 
                    if ((c != ' ')
                        || c == quote){
                            //&& c != '\r'
                             //c != '\n') {
                        isLookingForDelimiterInQuote = false;
                        //inQuoted = false;
                    }
                }
            }
            else {
                if (c == delimiter
                        || (!inQuoted && c == '\n')) {
                    //sbItem.Append(c);
                    results.Add(sbItem.ToString());
                    sbItem.Length = 0;
                    //lastChar = c;
                    continue;
                }
            }

            // Append current character
            sbItem.Append(c);
            //lastChar = c;
        }

        if (inQuoted) {
            results.Clear();
        }

        return results;
    }

    public static string ToPascalCase(string val) {
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

    public static Dictionary<string, object> FilterStringToObject(string dataString) {

        Dictionary<string, object> attributeObject = null;

        if (string.IsNullOrEmpty(dataString)) {
            return attributeObject;
        }

        attributeObject = dataString.FromJson<Dictionary<string, object>>().ToDataObject<Dictionary<string, object>>();

        List<string> keys = attributeObject.Keys.ToList();

        foreach (string key in keys) {
            var val = attributeObject.Get(key);
            if (val.GetType() == typeof(string)) {
                val = attributeObject.Get<string>(key).ConvertJson();
            }
            attributeObject.Set(key, val);
        }

        return attributeObject;
    }

    public static string[] SplitOnStringDelimiter(string data, string delim) {
        return data.Split(new string[] { delim }, StringSplitOptions.None);
    }

    public static bool IsEqualIgnoreCase(string val1, string val2) {

        if (string.IsNullOrEmpty(val1)
                && string.IsNullOrEmpty(val2)) {
            return true;
        }

        if (string.IsNullOrEmpty(val1)
                || string.IsNullOrEmpty(val2)) {
            return false;
        }

        return val1.Equals(val2, StringComparison.CurrentCultureIgnoreCase);
    }

}