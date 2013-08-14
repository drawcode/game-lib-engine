using System;
using System.Collections;
using System.Collections.Generic;

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
}