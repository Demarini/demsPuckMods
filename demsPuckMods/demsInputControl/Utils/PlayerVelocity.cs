using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace demsInputControl.Utils
{
    public static class LocalVelocityTracker
    {
        const int WarmupCount = 15; // Number of frames before macro kicks in
        const int MacroLookbackFrames = 10; // How far back to compare once warmed up
        const float FixedDelta = 0.02f; // Default fixed timestep

        public static List<Vector3> PositionHistory = new List<Vector3>();
        public static List<float> TimestampHistory = new List<float>();

        public static Vector3 LastPosition = Vector3.zero;
        public static float LastLocalZVelocity = 0f;
        public static bool IsFaceoff = false;
        private static bool initialized = false;

        // Controls how responsive the smoothing is (higher = more weight on recent values)
        private const float smoothingFactor = 0.3f; // 0.1f is very smooth, 0.5f is very reactive

        public static void Update(PlayerBodyV2 player)
        {
            Vector3 currentPosition = player.transform.position;
            float currentTime = Time.time;

            PositionHistory.Add(currentPosition);
            TimestampHistory.Add(currentTime);

            if (PositionHistory.Count > WarmupCount)
            {
                PositionHistory.RemoveAt(0);
                TimestampHistory.RemoveAt(0);
            }

            if (PositionHistory.Count < 2)
                return;

            // Use short-term delta during warmup
            int lastIndex = PositionHistory.Count - 1;

            if (PositionHistory.Count < WarmupCount)
            {
                if (PositionHistory.Count < 2) return; // Just in case

                Vector3 delta = PositionHistory[lastIndex] - PositionHistory[lastIndex - 1];
                float dt = TimestampHistory[lastIndex] - TimestampHistory[lastIndex - 1];
                Vector3 localDelta = player.transform.InverseTransformDirection(delta);
                LastLocalZVelocity = localDelta.z / Mathf.Max(dt, 0.001f);
            }
            else
            {
                int macroIndex = 0; // Oldest
                Vector3 delta = PositionHistory[lastIndex] - PositionHistory[macroIndex];
                float dt = TimestampHistory[lastIndex] - TimestampHistory[macroIndex];
                Vector3 localDelta = player.transform.InverseTransformDirection(delta);
                LastLocalZVelocity = localDelta.z / Mathf.Max(dt, 0.001f);
            }
            Debug.Log($"[VelocityTracker] Local Z Velocity (Hybrid Smoothed): {LastLocalZVelocity:F3}");
        }
    }
}
