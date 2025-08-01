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

        public static Queue<Vector3> PositionHistory = new Queue<Vector3>();
        public static Queue<float> TimestampHistory = new Queue<float>();

        public static Vector3 LastPosition = Vector3.zero;
        public static float LastLocalZVelocity = 0f;
        private static bool initialized = false;

        // Controls how responsive the smoothing is (higher = more weight on recent values)
        private const float smoothingFactor = 0.3f; // 0.1f is very smooth, 0.5f is very reactive

        public static void Update(PlayerBodyV2 player)
        {
            Vector3 currentPosition = player.transform.position;
            float currentTime = Time.time;

            PositionHistory.Enqueue(currentPosition);
            TimestampHistory.Enqueue(currentTime);

            while (PositionHistory.Count > WarmupCount)
            {
                PositionHistory.Dequeue();
                TimestampHistory.Dequeue();
            }

            if (PositionHistory.Count < 2)
                return;

            // Use short-term delta during warmup
            if (PositionHistory.Count < WarmupCount)
            {
                Vector3 delta = currentPosition - PositionHistory.ElementAt(PositionHistory.Count - 2);
                float dt = currentTime - TimestampHistory.ElementAt(TimestampHistory.Count - 2);
                Vector3 localDelta = player.transform.InverseTransformDirection(delta);
                LastLocalZVelocity = localDelta.z / Mathf.Max(dt, 0.001f);
            }
            else
            {
                // Compare against macro sample N frames back
                Vector3 pastPosition = PositionHistory.Peek();
                float pastTime = TimestampHistory.Peek();

                Vector3 delta = currentPosition - pastPosition;
                float dt = currentTime - pastTime;
                Vector3 localDelta = player.transform.InverseTransformDirection(delta);
                LastLocalZVelocity = localDelta.z / Mathf.Max(dt, 0.001f);
            }

            Debug.Log($"[VelocityTracker] Local Z Velocity (Hybrid Smoothed): {LastLocalZVelocity:F3}");
        }
    }
}
