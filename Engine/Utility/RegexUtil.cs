using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using UnityEngine;

public class RegexUtil {

    public static bool RegexIsMatch(
        string val, string regex) {        

        return Regex.IsMatch(val, regex);        
    }
    
    public static MatchCollection RegexMatches(
        string val, string regex) {        

        return Regex.Matches(val, regex);        
    }

    public static string RegexReplace(
        string val, string regex, string valReplacement) {
        
        if(string.IsNullOrEmpty(val)) {
            return val;
        }

        return Regex.Replace(val, regex, valReplacement);
    }
    
    public static string RegexReplaceMatches(
        string val, string regex, string valReplacement) {

        if (RegexIsMatch(val, regex)) {
            val = RegexReplace(val, regex, valReplacement);
        }

        return val;
    }

}