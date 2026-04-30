using UnityEngine;

namespace SceneryChanger.Services
{
    public static class SceneryAudioState
    {
        public static AudioSource MusicSource;
        public static AudioSource AmbientAudioSource;

        public static float MusicVolume = 0.5f;
        public static float AmbientAudioVolume = 0.3f;
        public static float GoalCrowdNoiseVolume = 0.37f;

        public static void Reset()
        {
            MusicSource = null;
            AmbientAudioSource = null;
            MusicVolume = 0.5f;
            AmbientAudioVolume = 0.3f;
            GoalCrowdNoiseVolume = 0.37f;
        }
    }
}
