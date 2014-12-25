using System;
using System.Collections;
using System.Collections.Generic;

using System.Linq;

public class FormatUtil {

    public static string GetFormattedTime(double seconds, string format) {
        TimeSpan t = TimeSpan.FromSeconds(seconds);

        string formatted = string.Format(format,
            t.Hours,
            t.Minutes,
            t.Seconds,
            t.Milliseconds);

        return formatted;
    }

    public static string GetFormattedTimeMinutesSecondsMsSmall(double seconds) {
        return GetFormattedTime(seconds, "{1:D2}:{2:D2}.{3:D1}");
    }

    public static string GetFormattedTimeMinutesSecondsMs(double seconds) {
        return GetFormattedTime(seconds, "{1:D2}:{2:D2}.{3:D3}");
    }

    public static string GetFormattedTimeHoursMinutesSecondsMsSmall(double seconds) {
        return GetFormattedTime(seconds, "{0:D2}:{1:D2}:{2:D2}.{3:D1}");
    }

    public static string GetFormattedTimeHoursMinutesSecondsMs(double seconds) {
        return GetFormattedTime(seconds, "{0:D2}:{1:D2}:{2:D2}.{3:D3}");
    }

    public static string GetStringTrimmedExact(string longString, int descriptionTextRowLength) {
        if (longString.Length > descriptionTextRowLength) {
            longString = longString.Substring(0, descriptionTextRowLength);
            longString += "...";
        }

        return longString;
    }

    public static string GetStringTrimmed(string longString, int descriptionTextRowLength) {
        string filtered = longString;

        if (!string.IsNullOrEmpty(filtered)) {
            List<string> lines = FormatUtil.SplitStringAcrossLines(longString, descriptionTextRowLength);
            if (lines.Count > 0) {
                filtered = lines[0];
                if (filtered.Length > descriptionTextRowLength) {
                    filtered += "...";
                }
            }
        }

        return filtered;
    }

    public static string GetStringTrimmedWithBreaks(string longString, int lineLength) {
        string trimmedString = "";

        if (!string.IsNullOrEmpty(longString)) {
            List<string> lines = FormatUtil.SplitStringAcrossLines(longString, lineLength);
            foreach (string s in lines) {
                trimmedString += s + "\n";
            }
        }

        return trimmedString;
    }

    public static List<string> SplitStringAcrossLines(string fullString, int maxStringLength) {
        List<string> lines = new List<string>();

        string[] words = fullString.Split(' ');

        int currentLength = 0;
        int currentMaxLength = maxStringLength;

        string currentLine = "";

        foreach (string word in words) {
            currentLength += word.Length + 1;

            if (currentLength >= currentMaxLength) {

                // start next line
                lines.Add(currentLine);
                currentLine = "";
                currentLength = word.Length + 1;
            }
            currentLine += word + " ";
        }

        if (!string.IsNullOrEmpty(currentLine)) {
            lines.Add(currentLine);
        }

        return lines;
    }

    // BASE 64
        
    public static bool IsStringBase64(string base64String) {
        if (base64String.Replace(" ", "").Length % 4 != 0) {
            return false;
        }
        
        try {
            Convert.FromBase64String(base64String);
            return true;
        }
        catch (Exception exception) {
            // Handle the exception
            UnityEngine.Debug.Log(exception.ToJson());
        }
        return false;
    }
    
    public static string StringToBase64(string plainText) {
        if (string.IsNullOrEmpty(plainText)) {
            return plainText;
        }
        
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        return System.Convert.ToBase64String(plainTextBytes);
    }

    public static string StringFromBase64(string base64String) {
        if (string.IsNullOrEmpty(base64String) || !IsStringBase64(base64String)) {
            return base64String;
        }

        var base64EncodedBytes = System.Convert.FromBase64String(base64String);
        return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
    }

    public static byte[] BytesFromBase64(string base64String) {
        if (string.IsNullOrEmpty(base64String) || !IsStringBase64(base64String)) {
            return System.Text.Encoding.UTF8.GetBytes(base64String);
        }
        
        var base64EncodedBytes = System.Convert.FromBase64String(base64String);
        return base64EncodedBytes;
    }

    //

        
    public static string GetCharactersLimited(string text, int length) {
        // If text in shorter or equal to length, just return it
        if (text.Length <= length) {
            return text;
        }
        
        // Text is longer, so try to find out where to cut
        char[] delimiters = new char[] { ' ', '.', ',', ':', ';' };
        int index = text.LastIndexOfAny(delimiters, length - 3);
        
        if (index > (length / 2)) {
            return text.Substring(0, index) + "...";
        }
        else {
            return text.Substring(0, length - 3) + "...";
        }
    }
    
    public static string ConvertToBase36(string val) {
        var result = val.Sum(x => x);
        return ConvertToBase(result, 36);
    }
    
    public static string ConvertToBase(int num, int nbase) {
        string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        
        // check if we can convert to another base
        if (nbase < 2 || nbase > chars.Length)
            return "";
        
        int r;
        string newNumber = "";
        
        // in r we have the offset of the char that was converted to the new base
        while (num >= nbase) {
            r = num % nbase;
            newNumber = chars[r] + newNumber;
            num = num / nbase;
        }
        // the last number to convert
        newNumber = chars[num] + newNumber;
        
        return newNumber;
    }
}