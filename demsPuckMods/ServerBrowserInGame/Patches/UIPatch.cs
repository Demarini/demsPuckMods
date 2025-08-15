using HarmonyLib;
using PuckAIPractice.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace PuckAIPractice.Patches
{
    [HarmonyPatch(typeof(UIServerBrowser), "OnClickClose")]
    static class UIServerBrowser_OnClickClose_Prefix
    {
        static bool Prefix(UIServerBrowser __instance)
        {
            var ui = UIManager.Instance;
            if (ui == null) return true;

            // Only hijack if we launched it from the Pause screen
            if (!PauseMenuServerBrowserInjector.OpenedFromPause)
                return true; // let the game handle it (main menu behavior)

            // We handled it: just hide the browser and bring back the Pause menu
            try
            {
                __instance.Hide(false); // UIComponent.Hide(bool animate)
                ui.PauseMenu?.Show();
            }
            finally
            {
                PauseMenuServerBrowserInjector.OpenedFromPause = false; // reset for next time
            }

            // Skip original, which would toggle to Main Menu
            return false;
        }
    }
    [HarmonyPatch(typeof(UIManagerInputs), "OnPauseActionPerformed", new Type[] { typeof(InputAction.CallbackContext) })]
    static class UIManagerInput_OnPauseActionPerformed_Prefix
    {
        static bool Prefix(InputAction.CallbackContext context)
        {
            var ui = UIManager.Instance;
            if (ui == null) return true; // no UI, let original run (harmless)

            // If the Server Browser was opened from Pause, pressing ESC should close BOTH
            if (PauseMenuServerBrowserInjector.OpenedFromPause &&
                ui.ServerBrowser != null && ui.ServerBrowser.IsVisible)
            {
                // Close the browser first so its own Close logic doesn't flip UI state
                ui.ServerBrowser.Hide(false);

                // Also close Pause
                ui.PauseMenu?.Hide(false);

                // Clear the flag so next ESC behaves normally
                PauseMenuServerBrowserInjector.OpenedFromPause = false;

                // SKIP the game's default pause toggle for this press
                return false;
            }

            // Otherwise, let the game's normal pause toggle run
            return true;
        }
    }
    //[HarmonyPatch]
    //public static class BrowserWidth
    //{
    //    private static readonly AccessTools.FieldRef<UIServerBrowser, VisualElement> _containerRef =
    //AccessTools.FieldRefAccess<UIServerBrowser, VisualElement>("container");

    //    [HarmonyPostfix]
    //    [HarmonyPatch(typeof(UIServerBrowser), nameof(UIServerBrowser.Initialize), new[] { typeof(VisualElement) })]
    //    static void Initialize_Postfix(UIServerBrowser __instance)
    //    {
    //        var container = _containerRef(__instance);
    //        if (container == null) return;

    //        const float extra = 200f; // roughly star width + gutter
    //        container.schedule.Execute(() =>
    //        {
    //            // read resolved width after first layout and bump it
    //            var w = container.resolvedStyle.width;
    //            if (w > 0)
    //                container.style.width = w + extra;
    //        }).StartingIn(0);
    //    }
    //}
    [HarmonyPatch]
    public static class FavColumn
    {
        private static readonly AccessTools.FieldRef<UIServerBrowser, Button> _nameHeaderRef =
            AccessTools.FieldRefAccess<UIServerBrowser, Button>("nameHeaderButton");
        private static readonly AccessTools.FieldRef<UIServerBrowser, Button> _playersHeaderRef =
            AccessTools.FieldRefAccess<UIServerBrowser, Button>("playersHeaderButton");
        private static readonly AccessTools.FieldRef<UIServerBrowser, Button> _pingHeaderRef =
            AccessTools.FieldRefAccess<UIServerBrowser, Button>("pingHeaderButton");
        private static readonly AccessTools.FieldRef<UIServerBrowser, List<TemplateContainer>> _serverButtonsRef =
            AccessTools.FieldRefAccess<UIServerBrowser, List<TemplateContainer>>("serverButtons");

        private static readonly HashSet<string> _favorites = new HashSet<string>();

        private const string FavHeaderName = "FavHeaderButton";
        private const string FavCellName = "FavCell";
        private const string FavClass = "yourmod-favcol";

        private static string Key(ServerBrowserServer s) => $"{s.ipAddress}:{s.pingPort}";

        private static float sPlayersColWidth = -1f;
        private static float sPingColWidth = -1f;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIServerBrowser), nameof(UIServerBrowser.Initialize), new[] { typeof(VisualElement) })]
        private static void Initialize_Postfix(UIServerBrowser __instance)
        {
            var playersHeader = _playersHeaderRef(__instance);
            var pingHeader = _pingHeaderRef(__instance);
            var headerRow = pingHeader?.parent;
            if (headerRow == null) return;

            headerRow.style.flexWrap = Wrap.NoWrap;

            // read widths after first layout tick
            headerRow.schedule.Execute(() =>
            {
                try
                {
                    if (playersHeader != null) sPlayersColWidth = playersHeader.resolvedStyle.width;
                    if (pingHeader != null) sPingColWidth = pingHeader.resolvedStyle.width;
                }
                catch { /* ok */ }
            }).StartingIn(0);
        }

        // ---------- ROW CELL: add to the RIGHT of PING ----------
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIServerBrowser), "AddServerButton", new Type[] { typeof(ServerBrowserServer), typeof(long) })]
        private static void AddServerButton_Postfix(UIServerBrowser __instance, ServerBrowserServer server, long ping)
        {
            try
            {
                var rows = _serverButtonsRef(__instance);
                if (rows == null) return;

                // find the just-added row for this server
                TemplateContainer row = null;
                for (int i = rows.Count - 1; i >= 0; --i)
                {
                    var ud = rows[i].userData as Dictionary<string, object>;
                    if (ud != null && ReferenceEquals(ud["server"], server))
                    {
                        row = rows[i];
                        break;
                    }
                }
                if (row == null) return;

                InjectFavCellToRight(row, server);
            }
            catch (Exception e)
            {
                Debug.LogError($"[YourMod] Fav cell inject failed: {e}");
            }
        }

        private static void InjectFavCellToRight(TemplateContainer row, ServerBrowserServer server)
        {
            var serverBtn = row.Q<Button>("ServerButton");
            if (serverBtn == null) return;

            // --- STAR (absolute overlay inside the button) ---
            var fav = serverBtn.Q<Label>(FavCellName);
            if (fav == null)
            {
                fav = new Label { name = FavCellName };
                var key = Key(server);
                fav.text = _favorites.Contains(key) ? "★" : "☆";

                // Base size off ping label font (fallback 14)
                var pingLabelForSize = serverBtn.Q<TextElement>("PingLabel");
                float basePx = 14f;
                try { if (pingLabelForSize != null) basePx = pingLabelForSize.resolvedStyle.fontSize; } catch { /* ok */ }
                var starSize = basePx * 1.5f;

                fav.style.position = Position.Absolute;
                fav.style.top = 0;
                fav.style.bottom = 0;
                fav.style.right = 6;                 // keep inside row; no overflow
                fav.style.width = starSize;
                fav.style.fontSize = starSize;
                fav.style.unityTextAlign = TextAnchor.MiddleCenter;
                fav.style.flexShrink = 0;
                fav.pickingMode = PickingMode.Position;

                serverBtn.Add(fav);
                fav.BringToFront(); // ensure it renders above the row button

                // Stop join when clicking the star
                fav.RegisterCallback<PointerDownEvent>(e => { e.StopImmediatePropagation(); e.PreventDefault(); });
                fav.RegisterCallback<PointerUpEvent>(e => { e.StopImmediatePropagation(); e.PreventDefault(); });
                fav.RegisterCallback<ClickEvent>(e =>
                {
                    e.StopImmediatePropagation();
                    e.PreventDefault();

                    if (_favorites.Contains(key)) { _favorites.Remove(key); fav.text = "☆"; }
                    else { _favorites.Add(key); fav.text = "★"; }
                }, TrickleDown.NoTrickleDown);
            }

            // --- ALIGN PLAYERS to header width (right-aligned) ---
            var playersLabel = serverBtn.Q<Label>("PlayersLabel");
            if (playersLabel != null && sPlayersColWidth > 0f)
            {
                playersLabel.style.width = sPlayersColWidth;
                playersLabel.style.flexShrink = 0;
                playersLabel.style.unityTextAlign = TextAnchor.MiddleRight;
            }

            // NOTE: leave PING alone to avoid hiding/overlapping the star.
            // If you really want to normalize ping too, subtract a star pad:
            // var pingLabel = serverBtn.Q<TextElement>("PingLabel");
            // if (pingLabel != null && sPingColWidth > 0f) {
            //     var starPad = fav.resolvedStyle.width + 8f;
            //     pingLabel.style.width = Mathf.Max(0, sPingColWidth - starPad);
            //     pingLabel.style.flexShrink = 0;
            //     pingLabel.style.unityTextAlign = TextAnchor.MiddleRight;
            // }
        }
    }
}