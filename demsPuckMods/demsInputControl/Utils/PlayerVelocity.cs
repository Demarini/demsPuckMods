using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace demsInputControl.Utils
{
    public static class LocalVelocityTracker
    {
        //    private const int WarmupCount = 15;
        //    private const float MinValidDt = 0.005f;
        //    private const float MaxValidVelocity = 10f;
        //    private const float AccelerationDamping = 0.5f;
        //    private const float PredictionHorizon = 0.1f;
        //    private const float FaceoffSpeedThreshold = 0.05f;
        //    private const float FaceoffTime = 2.0f;

        //    public static List<Vector3> PositionHistory = new List<Vector3>();
        //    public static List<float> TimestampHistory = new List<float>();
        //    public static List<float> VelocityHistory = new List<float>();

        //    public static float LastLocalZVelocity { get; private set; }
        //    public static float PredictedLocalZVelocity { get; private set; }
        //    public static bool IsFaceoff { get; private set; }

        //    private static float faceoffTimer = 0f;
        //    private static float previousVelocity = 0f;

        //    public static void Update(PlayerBodyV2 player)
        //    {
        //        Vector3 currentPosition = player.transform.position;
        //        float currentTime = Time.time;

        //        PositionHistory.Add(currentPosition);
        //        TimestampHistory.Add(currentTime);

        //        if (PositionHistory.Count > WarmupCount)
        //        {
        //            PositionHistory.RemoveAt(0);
        //            TimestampHistory.RemoveAt(0);
        //        }

        //        int lastIndex = PositionHistory.Count - 1;
        //        if (PositionHistory.Count < 2) return;

        //        Vector3 delta = PositionHistory[lastIndex] - PositionHistory[lastIndex - 1];
        //        float dt = TimestampHistory[lastIndex] - TimestampHistory[lastIndex - 1];
        //        if (dt < MinValidDt) return;

        //        Vector3 localDelta = player.transform.InverseTransformDirection(delta);
        //        float rawLocalZ = localDelta.z / Mathf.Max(dt, 0.001f);
        //        if (Mathf.Abs(rawLocalZ) > MaxValidVelocity) return;

        //        VelocityHistory.Add(rawLocalZ);
        //        if (VelocityHistory.Count > WarmupCount)
        //            VelocityHistory.RemoveAt(0);

        //        // Weighted smoothing
        //        float weightedSum = 0f;
        //        float totalWeight = 0f;
        //        for (int i = 0; i < VelocityHistory.Count; i++)
        //        {
        //            float weight = (i + 1); // Linear weighting: more recent = higher weight
        //            weightedSum += VelocityHistory[i] * weight;
        //            totalWeight += weight;
        //        }
        //        float smoothedZ = weightedSum / totalWeight;
        //        LastLocalZVelocity = smoothedZ;

        //        // Prediction based on acceleration
        //        float acceleration = (smoothedZ - previousVelocity) / dt;
        //        acceleration = Mathf.Lerp(acceleration, 0f, AccelerationDamping);
        //        PredictedLocalZVelocity = smoothedZ + acceleration * PredictionHorizon;
        //        previousVelocity = smoothedZ;

        //        // Faceoff check
        //        if (Mathf.Abs(smoothedZ) < FaceoffSpeedThreshold)
        //        {
        //            faceoffTimer += dt;
        //            IsFaceoff = faceoffTimer >= FaceoffTime;
        //        }
        //        else
        //        {
        //            faceoffTimer = 0f;
        //            IsFaceoff = false;
        //        }
        //        Vector3 directionVector = player.MovementDirection.forward;
        //        player.
        //        float dot = Vector3.Dot(directionVector, player.transform.forward);
        //        if (dot > 0.1f)
        //            Debug.Log("Moving FORWARD");
        //        else if (dot < -0.1f)
        //            Debug.Log("Moving BACKWARD");
        //        else
        //            Debug.Log("Moving sideways or standing still");
        //        Debug.Log($"[VelocityTracker] LocalZ={LastLocalZVelocity:F3}, Predicted={PredictedLocalZVelocity:F3}, Faceoff={IsFaceoff}");
        //        Debug.Log($"[VelocityTracker] Speed={player.Speed:F3}");
        //        Debug.Log($"[VelocityTracker] Moving Forwards={player.Movement.IsMovingForwards}");
        //        Debug.Log($"[VelocityTracker] Moving Backwards={player.Movement.IsMovingBackwards}");
        //        Debug.Log($"[VelocityTracker] MovementDirection={player.MovementDirection.forward:F3}");
        //        LastLocalZVelocity = player.Speed;
        //    }
        //}
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
            Debug.Log($"[VelocityTracker] LocalZ={LastLocalZVelocity:F3}, Predicted={PredictedLocalZVelocity:F3}, Faceoff={IsFaceoff}, Speed={velocity.magnitude:F3}");
            Debug.Log($"[VelocityTracker] MovementDirection={SmoothedMovementDirection}, IsBackwards={IsMovingBackwards}");
        }
    }
}