namespace PuckAIPractice.AI
{
    public enum GoalieDifficulty { Easy, Normal, Hard }

    public class GoalieSettings
    {
        private static GoalieSettings _instanceBlue;
        public static GoalieSettings InstanceBlue
        {
            get
            {
                if (_instanceBlue == null)
                    _instanceBlue = new GoalieSettings();
                return _instanceBlue;
            }
        }
        private static GoalieSettings _instanceRed;
        public static GoalieSettings InstanceRed
        {
            get
            {
                if (_instanceRed == null)
                    _instanceRed = new GoalieSettings();
                return _instanceRed;
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
                    DashThreshold = 1f;
                    CancelThreshold = 0.08f;
                    ReactionTime = 0.25f;
                    MaxRotationAngle = 30f;
                    RotationSpeed = 6f;
                    DistanceFromNet = 1f;
                    break;

                case GoalieDifficulty.Normal:
                    DashCooldown = .6f;
                    DashCancelGrace = 0.15f;
                    DashThreshold = 0.4f;
                    CancelThreshold = 0.05f;
                    ReactionTime = 0.15f;
                    MaxRotationAngle = 60f;
                    RotationSpeed = 12f;
                    DistanceFromNet = .7f;
                    break;

                case GoalieDifficulty.Hard:
                    DashCooldown = .2f;
                    DashCancelGrace = 0.15f;
                    DashThreshold = 0.2f;
                    CancelThreshold = 0.05f;
                    ReactionTime = 0.15f;
                    MaxRotationAngle = 80f;
                    RotationSpeed = 18f;
                    DistanceFromNet = .5f;
                    break;
            }
        }
    }
}