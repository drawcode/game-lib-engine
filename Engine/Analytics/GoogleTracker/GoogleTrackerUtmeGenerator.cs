#define UNITY3D
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class GoogleTrackerUtmeGenerator
{
    private readonly GoogleTracker _GoogleTracker;

    private enum ValueType
    {
        Event = 5,
        GoogleTrackerCustomVariableName = 8,
        GoogleTrackerCustomVariableValue = 9
    }

    public GoogleTrackerUtmeGenerator(GoogleTracker GoogleTracker)
    {
        _GoogleTracker = GoogleTracker;
    }

    public string Generate()
    {
        return GenerateCustomVariables();
    }

    private string GenerateCustomVariables()
    {
        Func<GoogleTrackerCustomVariable, Func<GoogleTrackerCustomVariable, string>, string> getProperty =
            (cv, f) => cv == null ? null : f(cv);
        Func<Func<GoogleTrackerCustomVariable, string>, ValueType, string> getValues =
            (f, type) => SerializeValues(_GoogleTracker.GoogleTrackerCustomVariables.Select(f).ToArray(), type);

        var names = getValues(cv => getProperty(cv, cv1 => cv1.Name), ValueType.GoogleTrackerCustomVariableName);

        if (string.IsNullOrEmpty(names)) return string.Empty;

        var values = getValues(cv => getProperty(cv, cv1 => cv1.Value), ValueType.GoogleTrackerCustomVariableValue);

        return names + values;
    }

    private static string SerializeValues(IList<string> values, ValueType type)
    {
        var builder = new StringBuilder();

        var hasNonNullValues = false;

        for (var i = 0; i < values.Count; i++)
        {
            if (values[i] == null) continue;

            if (!hasNonNullValues)
                hasNonNullValues = true;
            else
                builder.Append("*");

            if (i > 0 && values[i - 1] == null)
                builder.AppendFormat("{0}!", i + 1);

            builder.Append(values[i]);
        }

        return hasNonNullValues ? string.Format("{0}({1})", (int)type, builder) : string.Empty;
    }
}