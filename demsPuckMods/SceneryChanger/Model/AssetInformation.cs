using System;

namespace SceneryChanger.Model
{
    [Serializable]
    public class AssetInformation
    {
        public bool useGlass = true;

        public bool useCustomAmbient = false;
        public float ambientMultiplier = 1f;

        public bool musicEnabled = false;
        public string musicPath = "";
        public float musicVolume = 0.5f;

        public bool ambientAudioEnabled = false;
        public string ambientAudioPath = "";
        public float ambientAudioVolume = 0.3f;

        public float goalCrowdNoiseVolume = 0.37f;
    }
}