namespace PuckAIPractice.AI
{
    public enum GoalieDifficulty { Easy, Normal, Hard }

    public class GoalieSettings
    {
        private static GoalieSettings _instance;
        public static GoalieSettings Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new GoalieSettings();
                return _instance;
            }
        }

        public GoalieDifficulty Difficulty { get; private set; } = GoalieDifficulty.Hard;

        // Dash logic
        public float DashCooldown { get; private set; }
        public float DashCancelGrace { get; private set; }
        public float DashThreshold { get; private set; }
        public float CancelThreshold { get; private set; }
        public float ReactionTime { get; private set; }

        // Rotation
        public float MaxRotationAngle { get; private set; }
        public float RotationSpeed { get; private set; }
        public float DistanceFromNet { get; set; }
        private GoalieSettings()
        {
            ApplyDifficulty(Difficulty);
        }

        public void ApplyDifficulty(GoalieDifficulty difficulty)
        {
            Difficulty = difficulty;

            switch (difficulty)
            {
                case GoalieDifficulty.Easy:
                    DashCooldown = 1f;
                    DashCancelGrace = 0.25f;
                    DashThreshold = 0.6f;
                    CancelThreshold = 0.08f;
                    ReactionTime = 0.25f;
                    MaxRotationAngle = 45f;
                    RotationSpeed = 6f;
                    DistanceFromNet = 1f;
                    break;

                case GoalieDifficulty.Normal:
                    DashCooldown = 1f;
                    DashCancelGrace = 0.75f;
                    DashThreshold = 0.4f;
                    CancelThreshold = 0.05f;
                    ReactionTime = 0.15f;
                    MaxRotationAngle = 60f;
                    RotationSpeed = 10f;
                    DistanceFromNet = .75f;
                    break;

                case GoalieDifficulty.Hard:
                    DashCooldown = 1f;
                    DashCancelGrace = 0.08f;
                    DashThreshold = 0.1f;
                    CancelThreshold = 0.02f;
                    ReactionTime = 0.05f;
                    MaxRotationAngle = 30f;
                    RotationSpeed = 14f;
                    DistanceFromNet = .5f;
                    break;
            }
        }
    }
}
