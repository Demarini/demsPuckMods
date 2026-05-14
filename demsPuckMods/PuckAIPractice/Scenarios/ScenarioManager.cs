using System;
using UnityEngine;

namespace PuckAIPractice.Scenarios
{
    public static class ScenarioManager
    {
        public static IScenario Active { get; private set; }
        public static string LastScenarioName { get; private set; }
        public static ulong? LastCallerClientId { get; private set; }

        public static bool StartByName(string name, ulong callerClientId)
        {
            var scenario = Create(name);
            if (scenario == null)
            {
                Debug.LogWarning($"[Scenario] Unknown scenario '{name}'");
                return false;
            }
            Stop();
            Active = scenario;
            LastScenarioName = scenario.Name;
            LastCallerClientId = callerClientId;
            try
            {
                scenario.Start(callerClientId);
                Debug.Log($"[Scenario] Started '{scenario.Name}' for clientId={callerClientId}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Scenario] Start failed: {ex}");
                Active = null;
                return false;
            }
        }

        public static bool Restart(ulong callerClientId)
        {
            if (string.IsNullOrEmpty(LastScenarioName))
            {
                Debug.Log("[Scenario] No previous scenario to restart");
                return false;
            }
            return StartByName(LastScenarioName, callerClientId);
        }

        public static void Stop()
        {
            if (Active == null) return;
            try
            {
                Active.Stop();
                Debug.Log($"[Scenario] Stopped '{Active.Name}'");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Scenario] Stop failed: {ex}");
            }
            Active = null;
        }

        private static IScenario Create(string name)
        {
            switch ((name ?? string.Empty).ToUpperInvariant())
            {
                case "RUSH": return new RushScenario();
                default: return null;
            }
        }
    }
}
