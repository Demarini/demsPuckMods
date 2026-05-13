using System;

namespace SceneryChanger.Model
{
    [Serializable]
    public class AssetInformation
    {
        public bool useGlass = true;

        // Overall Puck lighting — multiplies the game's ambient light to dim/brighten the whole scene.
        public bool useCustomAmbient = false;
        public float ambientMultiplier = 1f;

        // Use the game's default directional light. It already has the right culling/bias to put
        // a shadow under the player stick. When false, the game directional is disabled and the
        // bundle must provide its own "MainSun" directional to get any shadow caster.
        public bool useGameDirectional = true;

        // -1 = leave the game's intensity alone (typically 0.5). Set low (e.g. 0.05–0.15) for moody
        // bundles like the dance club so the directional barely lights but still casts a shadow.
        public float gameDirectionalIntensity = -1f;

        // -1 = leave the game's shadow strength alone. 0 = no shadow, 1 = fully dark.
        public float gameDirectionalShadowStrength = -1f;

        // When false (default), every Renderer inside the bundle gets shadowCastingMode=Off.
        // The bundle still RECEIVES shadows (the ice/floor still gets the stick shadow), but
        // its props don't render into the shadow map. Big perf win for dense bundles. Set true
        // only if a specific prop needs to throw a real cast shadow.
        public bool propsCastShadows = false;

        public bool musicEnabled = false;
        public string musicPath = "";
        public float musicVolume = 0.5f;

        public bool ambientAudioEnabled = false;
        public string ambientAudioPath = "";
        public float ambientAudioVolume = 0.3f;

        public float goalCrowdNoiseVolume = 0.37f;
    }
}