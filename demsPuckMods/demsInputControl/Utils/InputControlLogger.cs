using UnityEngine;
using demsInputControl.Singletons;

namespace demsInputControl.Logging
{
    public static class InputControlLogger
    {
        public static void Log(LogCategory category, string message)
        {
            if (!ShouldLog(category)) return;

            Debug.Log($"[InputControl][{category}] {message}");
        }

        public static void LogWarning(LogCategory category, string message)
        {
            if (!ShouldLog(category)) return;

            Debug.LogWarning($"[InputControl][{category}] {message}");
        }

        public static void LogError(LogCategory category, string message)
        {
            if (!ShouldLog(category)) return;

            Debug.LogError($"[InputControl][{category}] {message}");
        }

        private static bool ShouldLog(LogCategory category)
        {
            var logConfig = ConfigData.Instance.Logging;

            switch (category)
            {
                case LogCategory.Velocity:
                    return logConfig.VelocityLogging;
                case LogCategory.RPC:
                    return logConfig.RPCLogging;
                case LogCategory.SprintControl:
                    return logConfig.SprintControlLogging;
                case LogCategory.PracticeModeDetection:
                    return logConfig.PracticeModeDetectionLogging;
                case LogCategory.StopControl:
                    return logConfig.StopControlLogging;
                default:
                    return false;
            }
        }
    }
    public enum LogCategory
    {
        Velocity,
        RPC,
        SprintControl,
        PracticeModeDetection,
        StopControl
    }
}