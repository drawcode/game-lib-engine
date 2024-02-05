#define UNITY3D

namespace Engine.Analytics
{
    public interface GoogleTrackerIAnalyticsSession
    {
        string GenerateSessionId();
        string GenerateCookieValue();
    }
    public class GoogleTrackerCustomVariable
    {
        public string Name { get; private set; }
        public string Value { get; private set; }

        public GoogleTrackerCustomVariable(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
