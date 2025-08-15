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
    //[HarmonyPatch(typeof(UIPauseMenu), "Initialize")]
    //public static class UIPauseMenu_Initialize_Postfix
    //{
    //    // Grab private fields on UIPauseMenu
    //    private static readonly AccessTools.FieldRef<UIPauseMenu, VisualElement> _containerRef =
    //        AccessTools.FieldRefAccess<UIPauseMenu, VisualElement>("container");

    //    private static readonly AccessTools.FieldRef<UIPauseMenu, Button> _settingsButtonRef =
    //        AccessTools.FieldRefAccess<UIPauseMenu, Button>("settingsButton");

    //    public static void Postfix(UIPauseMenu __instance, VisualElement rootVisualElement)
    //    {
    //        try
    //        {
    //            Debug.Log("Initialize Pause Menu");
    //            var container = _containerRef(__instance);
    //            if (container == null)
    //            {
    //                Debug.LogWarning("[YourMod] PauseMenu container not found.");
    //                return;
    //            }

    //            // Avoid duplicates if Initialize ever runs more than once
    //            if (container.Q<Button>("ServerBrowserButton") != null)
    //                return;

    //            // Create the new button
    //            var btn = new Button
    //            {
    //                name = "ServerBrowserButton",
    //                text = "SERVER BROWSER"
    //            };

    //            // Copy style classes from an existing pause menu button for matching visuals
    //            var templateBtn = _settingsButtonRef(__instance);
    //            if (templateBtn != null)
    //            {
    //                foreach (var cls in templateBtn.GetClasses())
    //                    btn.AddToClassList(cls);

    //                // If they used inline styles (unlikely), you can copy a few basics:
    //                btn.style.height = templateBtn.style.height;
    //                btn.style.marginLeft = templateBtn.style.marginLeft;
    //                btn.style.marginRight = templateBtn.style.marginRight;
    //                btn.style.marginTop = templateBtn.style.marginTop;
    //                btn.style.marginBottom = templateBtn.style.marginBottom;
    //            }

    //            // Insert near the top (after "Switch Team"); adjust index to taste
    //            var insertIndex = Math.Min(1, container.childCount);
    //            container.hierarchy.Insert(insertIndex, btn);

    //            // Wire to the SAME handler the main menu button uses
    //            var menu = UIManager.Instance?.MainMenu;
    //            if (menu != null)
    //            {
    //                var mi = AccessTools.Method(typeof(UIMainMenu), "OnClickServerBrowser");
    //                if (mi != null)
    //                {
    //                    // Create a delegate bound to the UIMainMenu instance even if it's private
    //                    var del = AccessTools.MethodDelegate<Action>(mi, menu);
    //                    btn.clicked += del;
    //                }
    //                else
    //                {
    //                    // Fallback: open the browser via UIManager directly if you have an API for it
    //                    btn.clicked += () =>
    //                    {
    //                        Debug.Log("[YourMod] Server Browser (fallback) clicked.");
    //                        // UIManager.Instance.ServerBrowser.Show(); // if such a method exists
    //                    };
    //                }
    //            }
    //            else
    //            {
    //                Debug.LogWarning("[YourMod] UIManager/MainMenu not ready; ServerBrowserButton added without handler.");
    //            }
    //        }
    //        catch (Exception e)
    //        {
    //            Debug.LogError($"[YourMod] Injecting ServerBrowserButton into PauseMenu failed: {e}");
    //        }
    //    }
    //}
    //[HarmonyPatch(typeof(UIManager), "Awake")]
    //public static class UIManager_Awake_Postfix
    //{
    //    // Access private fields on UIMainMenu
    //    private static readonly AccessTools.FieldRef<UIMainMenu, Button> _serverBrowserBtnRef =
    //        AccessTools.FieldRefAccess<UIMainMenu, Button>("serverBrowserButton");

    //    static void Postfix(UIManager __instance)
    //    {
    //        try
    //        {
    //            Debug.Log("Entered Awake UI Manager");
    //            var root = __instance.RootVisualElement;
    //            var pauseContainer = root.Q<VisualElement>("PauseMenuContainer");
    //            if (pauseContainer == null) { Debug.LogWarning("[YourMod] PauseMenuContainer not found."); return; }

    //            // Don't add twice
    //            if (pauseContainer.Q<Button>("PauseServerBrowserButton") != null) return;

    //            // Get the existing main-menu button (for style/text)
    //            var menu = __instance.MainMenu;
    //            var srcBtn = menu != null ? _serverBrowserBtnRef(menu) : null;

    //            // Create the new pause-menu button
    //            var btn = new Button { name = "PauseServerBrowserButton", text = srcBtn != null ? srcBtn.text : "SERVER BROWSER" };

    //            // Copy USS classes from an existing pause button for consistent styling
    //            var pauseSettingsBtn = pauseContainer.Q<Button>("SettingsButton");
    //            if (pauseSettingsBtn != null)
    //            {
    //                foreach (var cls in pauseSettingsBtn.GetClasses())
    //                    btn.AddToClassList(cls);

    //                // (optional) copy a few inline styles if used
    //                btn.style.height = pauseSettingsBtn.style.height;
    //                btn.style.marginTop = pauseSettingsBtn.style.marginTop;
    //                btn.style.marginBottom = pauseSettingsBtn.style.marginBottom;
    //                btn.style.marginLeft = pauseSettingsBtn.style.marginLeft;
    //                btn.style.marginRight = pauseSettingsBtn.style.marginRight;
    //            }

    //            // Insert where you want it (0 = top, childCount = bottom). Here: just under "Switch Team".
    //            var insertIndex = Math.Min(1, pauseContainer.childCount);
    //            pauseContainer.hierarchy.Insert(insertIndex, btn);

    //            // Wire to the SAME click logic as the main menu button
    //            var mi = AccessTools.Method(typeof(UIMainMenu), "OnClickServerBrowser");
    //            if (menu != null && mi != null)
    //            {
    //                var del = AccessTools.MethodDelegate<Action>(mi, menu);
    //                btn.clicked += del;
    //            }
    //            else
    //            {
    //                btn.clicked += () => Debug.Log("[YourMod] Server Browser clicked (fallback).");
    //            }
    //        }
    //        catch (Exception e)
    //        {
    //            Debug.LogError($"[YourMod] Injecting Pause Server Browser button failed: {e}");
    //        }
    //    }
    //}
    //[HarmonyPatch(typeof(UIPauseMenu))]
    //public static class UIPauseMenu_Initialize_Postfix
    //{
    //    [HarmonyPostfix]
    //    [HarmonyPatch(nameof(UIPauseMenu.Initialize), new Type[] { typeof(VisualElement) })]
    //    static void Postfix(UIPauseMenu __instance, VisualElement rootVisualElement)
    //    {
    //        // your button injection here (same as above, but get container via rootVisualElement.Q<VisualElement>("PauseMenuContainer"))
    //        try
    //        {
    //            Debug.Log("Initialize Pause Menu");
    //            var root = rootVisualElement.Q<VisualElement>("PauseMenuContainer");
    //            if (root == null) return;

    //            // Pause menu container is already resolved by Initialize calls in Awake
    //            var container = root.Q<VisualElement>("PauseMenuContainer");
    //            if (container == null) { Debug.LogWarning("[YourMod] PauseMenuContainer not found."); return; }

    //            // Don’t add twice
    //            if (container.Q<Button>("ServerBrowserButton") != null) return;

    //            var btn = new Button { name = "ServerBrowserButton", text = "SERVER BROWSER" };

    //            // Copy style from an existing pause button (e.g., Settings)
    //            var templateBtn = container.Q<Button>("SettingsButton");
    //            if (templateBtn != null)
    //            {
    //                foreach (var cls in templateBtn.GetClasses())
    //                    btn.AddToClassList(cls);

    //                // (optional) copy a few inline styles if they use them
    //                btn.style.height = templateBtn.style.height;
    //                btn.style.marginTop = templateBtn.style.marginTop;
    //                btn.style.marginBottom = templateBtn.style.marginBottom;
    //                btn.style.marginLeft = templateBtn.style.marginLeft;
    //                btn.style.marginRight = templateBtn.style.marginRight;
    //            }

    //            // Place after "Switch Team" (index 1). Use 0 for top, or Add(...) for bottom.
    //            container.hierarchy.Insert(Math.Min(1, container.childCount), btn);

    //            // Wire to the same handler as the main menu
    //            var menu = UIManager.Instance.MainMenu;
    //            var mi = AccessTools.Method(typeof(UIMainMenu), "OnClickServerBrowser");
    //            if (menu != null && mi != null)
    //            {
    //                var del = AccessTools.MethodDelegate<Action>(mi, menu);
    //                btn.clicked += del;
    //            }
    //            else
    //            {
    //                // Fallback: call your own API if you have one
    //                btn.clicked += () => Debug.Log("[YourMod] Server Browser clicked (fallback).");
    //            }
    //        }
    //        catch (Exception e)
    //        {
    //            Debug.LogError($"[YourMod] Inject ServerBrowser into PauseMenu failed: {e}");
    //        }
    //    }
    //}
    //[HarmonyPatch(typeof(UIManager), "Awake")]
    //public static class UIManager_Awake_Postfix
    //{
    //    static void Postfix(UIManager __instance)
    //    {

    //    }
    //}
}