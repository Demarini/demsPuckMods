using UnityEngine;

namespace PuckAIPractice.Scenarios
{
    // Simple OnGUI HUD for the gauntlet scenario. Reads static state set by
    // GauntletScenario (server-side) — works for the host. Remote clients
    // see ShowHud=false (the static stays at its default) and render nothing.
    public class GauntletHud : MonoBehaviour
    {
        private GUIStyle _statStyle;
        private GUIStyle _bannerStyle;
        private GUIStyle _bannerSubStyle;

        void OnGUI()
        {
            if (!GauntletScenario.ShowHud && GauntletScenario.LossBannerSecondsRemaining <= 0f) return;

            EnsureStyles();

            if (GauntletScenario.ShowHud)
            {
                GUI.Label(new Rect(24f, 24f, 400f, 50f), $"Passed: {GauntletScenario.PassedBots}", _statStyle);
                GUI.Label(new Rect(24f, 64f, 400f, 40f), $"Wall: {Mathf.Max(0f, GauntletScenario.WallDistance):F0}m back", _statStyle);
            }

            if (GauntletScenario.LossBannerSecondsRemaining > 0f)
            {
                float w = 400f, h = 100f;
                var rect = new Rect((Screen.width - w) * 0.5f, (Screen.height - h) * 0.5f, w, h);
                GUI.Label(rect, "CAUGHT!", _bannerStyle);

                var subRect = new Rect(rect.x, rect.y + h, w, 50f);
                GUI.Label(subRect, $"Final: {GauntletScenario.LastFinalScore}", _bannerSubStyle);
            }
        }

        private void EnsureStyles()
        {
            if (_statStyle == null)
            {
                _statStyle = new GUIStyle(GUI.skin.label);
                _statStyle.fontSize = 28;
                _statStyle.fontStyle = FontStyle.Bold;
                _statStyle.normal.textColor = Color.white;
            }
            if (_bannerStyle == null)
            {
                _bannerStyle = new GUIStyle(GUI.skin.label);
                _bannerStyle.fontSize = 64;
                _bannerStyle.fontStyle = FontStyle.Bold;
                _bannerStyle.alignment = TextAnchor.MiddleCenter;
                _bannerStyle.normal.textColor = new Color(1f, 0.25f, 0.25f);
            }
            if (_bannerSubStyle == null)
            {
                _bannerSubStyle = new GUIStyle(GUI.skin.label);
                _bannerSubStyle.fontSize = 36;
                _bannerSubStyle.fontStyle = FontStyle.Bold;
                _bannerSubStyle.alignment = TextAnchor.MiddleCenter;
                _bannerSubStyle.normal.textColor = Color.white;
            }
        }

        public static GauntletHud Create()
        {
            var go = new GameObject("GauntletHud");
            DontDestroyOnLoad(go);
            return go.AddComponent<GauntletHud>();
        }
    }
}
