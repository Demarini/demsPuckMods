using demsInputControl.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace demsInputControl.Utils
{
    public static class LocalVelocityTracker
    {
        private const int WarmupCount = 15;
        private const int RegressionWindowSize = 10;
        private const int DirectionHistorySize = 5;

        private const float MinValidDt = 0.005f;
        private const float MaxValidVelocity = 10f;
        private const float FaceoffSpeedThreshold = 0.05f;
        private const float FaceoffTime = 2.0f;

        public static List<Vector3> PositionHistory = new List<Vector3>();
        public static List<float> TimestampHistory = new List<float>();
        private static Queue<Vector3> DirectionHistory = new Queue<Vector3>();

        public static float LastLocalZVelocity { get; private set; }
        public static float PredictedLocalZVelocity { get; private set; }
        public static bool IsFaceoff { get; private set; }
        public static Vector3 SmoothedMovementDirection { get; private set; }
        public static bool IsMovingBackwards { get; private set; }

        private static float faceoffTimer = 0f;

        public static void Update(PlayerBodyV2 player)
        {
            Vector3 currentPosition = player.transform.position;
            float currentTime = Time.time;

            PositionHistory.Add(currentPosition);
            TimestampHistory.Add(currentTime);
            if (PositionHistory.Count > RegressionWindowSize)
            {
                PositionHistory.RemoveAt(0);
                TimestampHistory.RemoveAt(0);
            }

            if (PositionHistory.Count < 2)
                return;

            // Regression-based velocity estimation
            int n = PositionHistory.Count;
            float meanTime = TimestampHistory.Average();
            float denom = TimestampHistory.Sum(t => (t - meanTime) * (t - meanTime));

            if (denom <= 0f)
                return;

            Vector3 meanPos = Vector3.zero;
            foreach (var pos in PositionHistory)
                meanPos += pos;
            meanPos /= n;

            Vector3 numer = Vector3.zero;
            for (int i = 0; i < n; i++)
            {
                float dt = TimestampHistory[i] - meanTime;
                numer += (PositionHistory[i] - meanPos) * dt;
            }

            Vector3 velocity = numer / denom;
            Vector3 localVelocity = player.transform.InverseTransformDirection(velocity);
            float rawZ = localVelocity.z;

            if (Mathf.Abs(rawZ) > MaxValidVelocity)
                return;

            LastLocalZVelocity = rawZ;

            // Predict forward (linear extrapolation)
            float predictionHorizon = 0.1f;
            PredictedLocalZVelocity = rawZ; // Could refine with acceleration later

            // Faceoff detection
            if (Mathf.Abs(rawZ) < FaceoffSpeedThreshold)
            {
                faceoffTimer += Time.deltaTime;
                IsFaceoff = faceoffTimer >= FaceoffTime;
            }
            else
            {
                faceoffTimer = 0f;
                IsFaceoff = false;
            }

            // Track movement direction (smoothed)
            Vector3 currentDelta = currentPosition - PositionHistory[Math.Max(0, PositionHistory.Count - 2)];
            Vector3 currentDir = currentDelta.normalized;
            if (currentDir != Vector3.zero)
                DirectionHistory.Enqueue(currentDir);
            if (DirectionHistory.Count > DirectionHistorySize)
                DirectionHistory.Dequeue();

            SmoothedMovementDirection = Vector3.zero;
            foreach (var dir in DirectionHistory)
                SmoothedMovementDirection += dir;
            SmoothedMovementDirection.Normalize();

            // Forward/backward check
            float forwardDot = Vector3.Dot(player.transform.forward, SmoothedMovementDirection);
            IsMovingBackwards = forwardDot < -0.2f;
            LastLocalZVelocity = IsMovingBackwards ? player.Speed * -1 : player.Speed;
            InputControlLogger.Log(LogCategory.Velocity, $"[VelocityTracker] LocalZ={LastLocalZVelocity:F3}, Predicted={PredictedLocalZVelocity:F3}, Faceoff={IsFaceoff}, Speed={velocity.magnitude:F3}");
            InputControlLogger.Log(LogCategory.Velocity, $"[VelocityTracker] MovementDirection={SmoothedMovementDirection}, IsBackwards={IsMovingBackwards}");
            InputControlLogger.Log(LogCategory.Velocity, $"[VelocityTracker] Velocitymovementthing={player.VelocityLean.Inverted}");
        }
    }
}