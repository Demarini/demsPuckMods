using System;

namespace SceneryChanger.Model
{
    public class AssetInformation
    {
        public bool useGlass = true;

        public bool useCustomAmbient = false;
        public float ambientMultiplier = 1f;

        public bool musicEnabled = false;
        public string musicPath = "";
        public float musicVolume = 0.5f;
    }
}