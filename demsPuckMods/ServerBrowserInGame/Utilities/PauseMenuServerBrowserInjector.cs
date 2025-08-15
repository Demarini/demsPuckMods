using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using UnityEngine;

namespace PuckAIPractice.Utilities
{
    public static class PauseMenuServerBrowserInjector
    {
        internal static bool OpenedFromPause; // set on click
        private static bool _installed;
        private static GameObject _injectorGO;
        internal static UnityEngine.UIElements.Button InjectedButton;   // handle to the injected button
        private const string InjectedClass = "yourmod-injected-serverbrowser"; // tag to find/remove as fallback
        public static void Uninstall(bool unpatchHarmony = false)
        {
            // 1) reset runtime state
            OpenedFromPause = false;

            // 2) remove the injected button (fast path via handle)
            if (InjectedButton != null)
            {
                InjectedButton.RemoveFromHierarchy();
                InjectedButton = null;
            }

            // 3) also remove any stragglers by name/class (covers UI rebuilds)
            var root = UIManager.Instance?.RootVisualElement;
            if (root != null)
            {
                // by name the injector used
                root.Q<Button>("PauseServerBrowserButton")?.RemoveFromHierarchy();

                // by our tag class (safe if name changes or multiples exist)
                root.Query<Button>(null, InjectedClass).ForEach(b => b.RemoveFromHierarchy());
            }

            // 4) destroy injector GO
            if (_injectorGO != null)
            {
                UnityEngine.Object.Destroy(_injectorGO);
                _injectorGO = null;
            }

            _installed = false;
        }
        public static void Install(string buttonText = "SERVER BROWSER", int insertIndex = 1)
        {
            if (_installed) return;
            _installed = true;

            _injectorGO = new GameObject("PauseMenuServerBrowserInjector");
            UnityEngine.Object.DontDestroyOnLoad(_injectorGO);

            var comp = _injectorGO.AddComponent<Injector>();
            comp.ButtonText = buttonText;
            comp.InsertIndex = insertIndex;
        }

        private sealed class Injector : MonoBehaviour
        {
            public string ButtonText = "SERVER BROWSER";
            public int InsertIndex = 1;

            private bool _injected;
            private bool _consumeEscapeThisFrame;  // set when we handle ESC ourselves
            private bool _pauseWasVisibleOnConsume; // to restore if someone toggled it

            private IEnumerator Start()
            {
                while (!_injected)
                {
                    TryInject();
                    if (!_injected) yield return new WaitForSeconds(0.25f);
                }
            }
            private bool _suppressNextPauseToggle;
            private void Update()
            {
                var ui = UIManager.Instance;
                if (ui == null) return;

                bool esc =
                    Input.GetKeyDown(KeyCode.Escape)
#if ENABLE_INPUT_SYSTEM
        || (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
#endif
                    ;

                if (!esc) return;

                // If we opened the browser from Pause, ESC should close BOTH
                if (PauseMenuServerBrowserInjector.OpenedFromPause)
                {
                    var browser = ui.ServerBrowser;
                    var pause = ui.PauseMenu;

                    Debug.Log("[YourMod] ESC: close ServerBrowser and Pause");
                    // Hide browser first (so Close button prefix won’t flip UI state)
                    browser?.Hide(false);

                    // Hide Pause too
                    pause?.Hide(false);

                    // Reset the context flag so later closes behave normally
                    PauseMenuServerBrowserInjector.OpenedFromPause = false;

                    // Guard: if the game’s own ESC toggler runs later this frame, squash it
                    _suppressNextPauseToggle = true;
                }
            }

            //private void LateUpdate()
            //{
            //    // Safety: if some other system also toggled Pause off in the same frame,
            //    // put it back. This guarantees ESC just closes the browser.
            //    if (_consumeEscapeThisFrame)
            //    {
            //        var ui = UIManager.Instance;
            //        var pause = ui?.PauseMenu;

            //        if (_pauseWasVisibleOnConsume && pause != null && !pause.IsVisible)
            //        {
            //            Debug.Log("Pause was visible and escape clicked");
            //            pause.Show();
            //        }
            //        _consumeEscapeThisFrame = false;
            //        _pauseWasVisibleOnConsume = false;
            //    }
            //}

            internal static Action BindMainMenuServerBrowser(UIMainMenu menu)
            {
                var mi = AccessTools.Method(typeof(UIMainMenu), "OnClickServerBrowser");
                return (menu != null && mi != null)
                    ? AccessTools.MethodDelegate<Action>(mi, menu)
                    : null;
            }

            private void TryInject()
            {
                var ui = UIManager.Instance;
                if (ui == null || ui.RootVisualElement == null) return;

                var pauseContainer = ui.RootVisualElement.Q<VisualElement>("PauseMenuContainer");
                if (pauseContainer == null) return;

                if (pauseContainer.Q<Button>("PauseServerBrowserButton") != null)
                {
                    _injected = true;
                    return;
                }

                var btn = new Button { name = "PauseServerBrowserButton", text = ButtonText };
                btn.AddToClassList(InjectedClass);
                InjectedButton = btn;
                btn.RegisterCallback<DetachFromPanelEvent>(_ =>
                {
                    // if something else removes it, clear the handle
                    if (InjectedButton == btn) InjectedButton = null;
                });
                // Copy look from Settings button
                var template = pauseContainer.Q<Button>("SettingsButton");
                if (template != null)
                {
                    foreach (var cls in template.GetClasses())
                        btn.AddToClassList(cls);

                    btn.style.height = template.style.height;
                    btn.style.marginTop = template.style.marginTop;
                    btn.style.marginBottom = template.style.marginBottom;
                    btn.style.marginLeft = template.style.marginLeft;
                    btn.style.marginRight = template.style.marginRight;
                }

                var index = Mathf.Clamp(InsertIndex, 0, pauseContainer.childCount);
                pauseContainer.hierarchy.Insert(index, btn);

                var menuDel = BindMainMenuServerBrowser(ui.MainMenu);
                if (menuDel != null)
                {
                    btn.clicked += () =>
                    {
                        OpenedFromPause = true; // flag ON CLICK
                        menuDel();
                    };
                }
                else
                {
                    btn.clicked += () =>
                    {
                        OpenedFromPause = true;
                        Debug.Log("[YourMod] Server Browser clicked (fallback).");
                        ui.ServerBrowser?.Show();
                    };
                    Debug.LogWarning("[YourMod] Could not bind UIMainMenu.OnClickServerBrowser; using fallback.");
                }

                _injected = true;
                Debug.Log("[YourMod] Injected Pause menu Server Browser button.");
            }
        }
    }
}
