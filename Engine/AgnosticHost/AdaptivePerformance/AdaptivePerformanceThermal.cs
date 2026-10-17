using UnityEngine.AdaptivePerformance;
using UnityEngine.Scripting;

// Nothing references this assembly statically (UnityPerfTiers binds by name), so without this the
// player's managed stripping would drop it whole.
[assembly: AlwaysLinkAssembly]

namespace Engine.AgnosticHost.Perf {

    // Thermal input for the perf governor, from Adaptive Performance. Its own assembly so it only
    // compiles where the Adaptive Performance module is installed (versionDefines +
    // defineConstraints); UnityPerfTiers finds TemperatureLevel by name and binds it once.
    //
    // Reads 0 when no provider is active (none is configured in the Editor or without a device
    // provider), which the governor treats as "cool".
    [Preserve]
    public static class AdaptivePerformanceThermal {

        // 0 = nominal, 1 = throttling imminent (ThermalMetrics.TemperatureLevel).
        public static float TemperatureLevel() {

            IAdaptivePerformance ap = Holder.Instance;

            if (ap == null || !ap.Active || ap.ThermalStatus == null) {
                return 0f;
            }

            float level = ap.ThermalStatus.ThermalMetrics.TemperatureLevel;
            return level > 0f ? level : 0f;
        }
    }
}
