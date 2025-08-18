using HarmonyLib;
using PuckAIPractice.Utilities;
using ServerBrowserInGame.Models;
using ServerBrowserInGame.Utilities;
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

                // initial state from the store
                fav.text = FavoriteStore.IsFavorite(server) ? "★" : "☆";

                // sizing like you had
                var pingLabelForSize = serverBtn.Q<TextElement>("PingLabel");
                float basePx = 14f;
                try { if (pingLabelForSize != null) basePx = pingLabelForSize.resolvedStyle.fontSize; } catch { }
                var starSize = basePx * 1.5f;

                fav.style.position = Position.Absolute;
                fav.style.top = 0;
                fav.style.bottom = 0;
                fav.style.right = 6;
                fav.style.width = starSize;
                fav.style.fontSize = starSize;
                fav.style.unityTextAlign = TextAnchor.MiddleCenter;
                fav.style.flexShrink = 0;
                fav.pickingMode = PickingMode.Position;

                serverBtn.Add(fav);
                fav.BringToFront();

                // block row join
                fav.RegisterCallback<PointerDownEvent>(e => { e.StopImmediatePropagation(); e.PreventDefault(); });
                fav.RegisterCallback<PointerUpEvent>(e => { e.StopImmediatePropagation(); e.PreventDefault(); });

                fav.RegisterCallback<ClickEvent>(e =>
                {
                    e.StopImmediatePropagation();
                    e.PreventDefault();

                    bool nowFav = FavoriteStore.Toggle(server);  // this already adds/removes + saves
                    fav.text = nowFav ? "★" : "☆";

                    if (FavoriteFilter.Enabled)                  // optional: only when active
                        FavoriteFilter.ApplyNow();
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
    [HarmonyPatch(typeof(UIServerBrowser), "SortServers")]
    public static class FavOnly_Postfix
    {
        private static readonly AccessTools.FieldRef<UIServerBrowser, VisualElement> _serverContainerRef =
            AccessTools.FieldRefAccess<UIServerBrowser, VisualElement>("serverContainer");

        [HarmonyPostfix]
        private static void AfterSort(UIServerBrowser __instance)
        {
            if (!FavoriteFilter.Enabled) return;

            var container = _serverContainerRef(__instance);
            if (container == null) return;

            // Only hide more; never force-show something the game already hid.
            foreach (var row in container.Children().OfType<TemplateContainer>())
            {
                // skip rows already hidden by the game
                if (row.resolvedStyle.display == DisplayStyle.None) continue;

                var ud = row.userData as Dictionary<string, object>;
                if (ud == null) continue;

                var server = ud.TryGetValue("server", out var sObj) ? sObj as ServerBrowserServer : null;
                if (!FavoriteStore.IsFavorite(server))
                {
                    row.style.display = DisplayStyle.None;
                }
            }
        }
    }
    //[HarmonyPatch]
    //public static class FavOnlyFooterSimple
    //{
    //    private const string FavOnlyFieldName = "FavOnlyToggleField";

    //    private static readonly AccessTools.FieldRef<UIServerBrowser, VisualElement> _containerRef =
    //        AccessTools.FieldRefAccess<UIServerBrowser, VisualElement>("container");

    //    [HarmonyPostfix]
    //    [HarmonyPatch(typeof(UIServerBrowser), nameof(UIServerBrowser.Initialize), new[] { typeof(VisualElement) })]
    //    private static void Initialize_Postfix(UIServerBrowser __instance)
    //    {
    //        Debug.Log("[FavOnly] Entered FavOnly");
    //        var container = _containerRef(__instance);
    //        if (container == null)
    //        {
    //            Debug.Log("[FavOnly] container == null");
    //            return;
    //        }

    //        // Use the same lookup pattern the game uses
    //        var emptyWrap = container.Query("ShowEmptyToggle").First();
    //        var fullWrap = container.Query("ShowFullToggle").First();

    //        // Prefer SHOW EMPTY row; fall back to SHOW FULL
    //        var baseWrap = emptyWrap ?? fullWrap;
    //        if (baseWrap == null)
    //        {
    //            Debug.Log("[FavOnly] Could not find ShowEmptyToggle nor ShowFullToggle wrappers");
    //            return;
    //        }

    //        var row = baseWrap.parent;
    //        if (row == null)
    //        {
    //            Debug.Log("[FavOnly] baseWrap.parent == null");
    //            return;
    //        }

    //        // Already injected?
    //        if (row.Q<VisualElement>(FavOnlyFieldName) != null)
    //        {
    //            Debug.Log("[FavOnly] Already injected");
    //            return;
    //        }

    //        // ---- Build a sibling field that looks like the existing one ----
    //        var favField = new VisualElement { name = FavOnlyFieldName };
    //        foreach (var cls in baseWrap.GetClasses())
    //            favField.AddToClassList(cls);

    //        favField.style.flexDirection = FlexDirection.Row;
    //        favField.style.alignItems = Align.Center;
    //        favField.style.justifyContent = Justify.SpaceBetween;
    //        favField.style.flexShrink = 0;

    //        // Clone label styling
    //        var baseLabel = baseWrap.Q<Label>();
    //        var lbl = new Label("SHOW ONLY FAVORITES");
    //        if (baseLabel != null)
    //            foreach (var c in baseLabel.GetClasses())
    //                lbl.AddToClassList(c);

    //        var tgl = new Toggle { text = string.Empty, value = false };

    //        favField.Add(lbl);
    //        favField.Add(tgl);

    //        // Insert immediately after the base field
    //        var idx = row.IndexOf(baseWrap);
    //        if (idx < 0) idx = row.childCount - 1;
    //        row.hierarchy.Insert(idx + 1, favField);
    //        Debug.Log("[FavOnly] Injected next to " + (baseWrap == emptyWrap ? "ShowEmpty" : "ShowFull"));

    //        // ---- Split widths AFTER layout resolves ----
    //        void Split()
    //        {
    //            // We want both fields to have the same fixed width
    //            var w = baseWrap.resolvedStyle.width;
    //            if (w <= 0) return; // not ready yet

    //            var half = Mathf.Max(120f, Mathf.Floor(w * 0.5f) - 2f);

    //            baseWrap.style.width = half; baseWrap.style.flexGrow = 0; baseWrap.style.flexShrink = 0;
    //            favField.style.width = half; favField.style.flexGrow = 0; favField.style.flexShrink = 0;

    //            // Optional: keep label/toggle nicely spaced
    //            lbl.style.marginRight = 6;
    //            tgl.style.marginLeft = 6;
    //        }

    //        // Run once after geometry + re-run if the row changes size
    //        row.RegisterCallback<GeometryChangedEvent>(_ => Split());
    //        row.schedule.Execute(Split).StartingIn(0);
    //    }
    //}
}