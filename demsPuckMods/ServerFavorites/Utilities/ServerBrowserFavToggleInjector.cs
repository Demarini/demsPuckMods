using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using UnityEngine;
using ServerBrowserInGame.Models;

namespace ServerBrowserInGame.Utilities
{
    public static class ServerBrowserFavToggleInjector
    {
        public static bool StarActive;                    // UI-only flag (toggled by the button)
        public static event Action<bool> OnStarToggled;   // optional: hook if you need it later

        private static bool _installed;
        private static GameObject _go;
        private static Button _starBtn;

        private const string StarBtnName = "FavFooterStarButton"; // our element id
        private const string ContainerName = "ServerBrowserContainer";
        private const string NewServerName = "ServerLauncherButton";
        private const string RefreshName = "RefreshButton";
        private const string CloseName = "CloseButton";

        public static void Install()
        {
            if (_installed) return;
            _installed = true;

            _go = new GameObject("ServerBrowserFooterStarInjector");
            UnityEngine.Object.DontDestroyOnLoad(_go);
            _go.AddComponent<Injector>();
        }

        public static void Uninstall()
        {
            StarActive = false;

            // Clean any injected star
            var root = UIManager.Instance?.RootVisualElement;
            root?.Q<Button>(StarBtnName)?.RemoveFromHierarchy();
            _starBtn = null;

            if (_go != null) UnityEngine.Object.Destroy(_go);
            _go = null;
            _installed = false;
        }

        private sealed class Injector : MonoBehaviour
        {
            private const string FieldName = "FavOnlyToggleField";
            public static bool FavOnlyEnabled;                   // UI state only (no filtering yet)
            public static event System.Action<bool> OnChanged;

            private VisualElement _injected;
            private Toggle _toggle;

            private IEnumerator Start()
            {
                // Keep trying until injected; if panel rebuilds later our Detach handler re-arms this loop
                while (_starBtn == null)
                {
                    TryInject();
                    if (_starBtn == null) yield return new WaitForSeconds(0.25f);
                }
            }

            private void TryInject()
            {
                var root = UIManager.Instance?.RootVisualElement;
                if (root == null) return;

                var browser = root.Q<VisualElement>(ContainerName);
                if (browser == null) return;

                // Get footer buttons to locate the row
                var newServer = browser.Q<Button>(NewServerName);
                var refresh = browser.Q<Button>(RefreshName);
                var close = browser.Q<Button>(CloseName);
                var row = (newServer ?? refresh ?? close)?.parent.parent;
                if (row == null) return;

                // Already injected?
                var existing = row.Q<Button>(StarBtnName);
                if (existing != null) { _starBtn = existing; return; }

                // --- Create our star button ---
                var star = new Button { name = StarBtnName, text = StarActive ? "★" : "☆", tooltip = "Favorites" };

                // Copy look from NEW SERVER so it matches the footer buttons
                if (newServer != null)
                    foreach (var cls in newServer.GetClasses()) star.AddToClassList(cls);

                // Keep it compact & square; don’t let it stretch (and remove horizontal padding)
                star.style.flexGrow = 0;
                star.style.flexShrink = 0;
                star.style.paddingLeft = 0;
                star.style.paddingRight = 0;
                star.style.unityTextAlign = TextAnchor.MiddleCenter;

                // ---------- INSERT POSITION: flush on far left ----------
                // Many UIs place a non-button "gutter" element as the first child.
                // If present, put the star immediately AFTER that gutter; otherwise index 0.
                VisualElement leftGutter = null;
                foreach (var child in row.Children())
                {
                    if (child is Button) break;   // first interactive is NEW SERVER
                    leftGutter = child;           // keep the last non-button (gutter) we passed
                }
                int insertIndex = (leftGutter != null) ? row.IndexOf(leftGutter) + 1 : 0;
                // --- place star hard-left ---
                // Put it as the VERY FIRST child so any growing left spacer stays to its right.
                int idxNew = row.IndexOf(newServer);
                row.hierarchy.Insert(Mathf.Max(0, idxNew), star); // insert right before NEW SERVER

                // keep it compact and square; don’t let it stretch
                star.style.flexGrow = 0;
                star.style.flexShrink = 0;

                // small gap between star and NEW SERVER cluster
                star.style.marginLeft = 0;
                star.style.marginRight = 8;

                // make it a square matching the footer height (after layout)
                void SyncSize()
                {
                    if (newServer == null) return;
                    float h = newServer.resolvedStyle.height;
                    if (h > 0f)
                    {
                        star.style.height = h;
                        star.style.width = h;
                    }

                    // slightly larger glyph than footer text
                    try
                    {
                        var lbl = newServer.Q<Label>();
                        float baseFs = (lbl != null && lbl.resolvedStyle.fontSize > 0f)
                            ? lbl.resolvedStyle.fontSize
                            : 16f;
                        star.style.fontSize = baseFs * 1.25f;
                    }
                    catch { /* best effort */ }
                }

                row.RegisterCallback<GeometryChangedEvent>(_ => SyncSize());
                // after creating `star` in TryInject():
                FavoriteFilter.Enabled = FavoriteStore.FavOnly;   // use saved value
                star.text = FavoriteStore.FavOnly ? "★" : "☆";

                // make the list reflect the saved filter (after layout is ready)
                row.schedule.Execute(() => FavoriteFilter.ApplyNow()).StartingIn(0);

                // click handler (already mostly correct)
                star.clicked += () =>
                {
                    var newVal = !FavoriteFilter.Enabled;
                    FavoriteFilter.Enabled = newVal;
                    FavoriteStore.SetFavOnly(newVal);     // persist flag
                    star.text = newVal ? "★" : "☆";
                    FavoriteFilter.ApplyNow();
                };

                // Re-arm injector if the panel is rebuilt
                star.RegisterCallback<DetachFromPanelEvent>(_ =>
                {
                    if (_starBtn == star) _starBtn = null;
                });
                _starBtn = star;
            }
        }
    }
}