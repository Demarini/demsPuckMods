using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace TestProject.Behaviors
{
    /// <summary>
    /// Install once, then SimpleModal.Show("Title", "<b>rich</b> text...");
    /// </summary>
    public static class SimpleModal
    {
        static GameObject _go;
        static Impl _impl;

        public static void Install()
        {
            if (_impl != null) return;

            _go = new GameObject("YourMod_SimpleModal");
            _go.SetActive(true);
            UnityEngine.Object.DontDestroyOnLoad(_go);

            _impl = _go.AddComponent<Impl>();
            Debug.Log("[SimpleModal] Install -> Impl added");
        }

        public static void Uninstall()
        {
            _impl?.TearDown();
            if (_go) UnityEngine.Object.Destroy(_go);
            _go = null; _impl = null;
        }

        public static void Show(string title, string richText = null, Action<VisualElement> build = null)
            => _impl?.RequestShow(title, richText, build);

        public static void Hide() => _impl?.Hide();

        public static bool IsOpen => _impl != null && _impl.IsOpen;
    }

    [UnityEngine.Scripting.Preserve]   // helps with IL2CPP stripping
    public sealed class Impl : MonoBehaviour
    {
        VisualElement _overlay, _panel, _body, _footer;
        Label _title;
        ScrollView _scroll;
        Button _closeBtn;
        bool _built;
        TextElement sampleText;
        // pending request in case Show() comes before Build()
        string _pendingTitle, _pendingRich;
        Action<VisualElement> _pendingBuild;
        bool _hasPending;
        FontDefinition? fontDef;
        public bool IsOpen => _overlay != null && _overlay.resolvedStyle.display != DisplayStyle.None;

        void Awake()
        {
            Debug.Log("[SimpleModal] Impl.Awake");
            StartCoroutine(Bootstrap());
        }

        IEnumerator Bootstrap()
        {
            // wait for UI root to exist
            while (UIManager.Instance == null || UIManager.Instance.RootVisualElement == null)
                yield return null;

            Debug.Log("[SimpleModal] Bootstrap -> Build()");
            Build(UIManager.Instance.RootVisualElement);
        }

        void Build(VisualElement root)
        {
            if (_built) return;

            Debug.Log("Entered Build");
            // Fullscreen translucent overlay
            _overlay = new VisualElement { name = "YourModModalOverlay" };
            _overlay.style.position = Position.Absolute;
            _overlay.style.left = 0; _overlay.style.top = 0;
            _overlay.style.right = 0; _overlay.style.bottom = 0;
            _overlay.style.backgroundColor = new Color(0, 0, 0, 0.55f);
            _overlay.pickingMode = PickingMode.Position; // eat clicks
            _overlay.style.display = DisplayStyle.None;

            // Centered panel
            _panel = new VisualElement { name = "YourModModalPanel" };
            _panel.style.position = Position.Absolute;
            _panel.style.width = 800;
            _panel.style.maxWidth = Length.Percent(90);
            _panel.style.height = 520;
            _panel.style.maxHeight = Length.Percent(85);
            _panel.style.left = Length.Percent(50);
            _panel.style.top = Length.Percent(50);
            _panel.style.translate = new Translate(Length.Percent(-50), Length.Percent(-50), 0);
            _panel.style.flexDirection = FlexDirection.Column;
            _panel.style.backgroundColor = new Color(0.1f, 0.1f, 0.12f, 1f);
            _panel.style.borderTopLeftRadius = 12;
            _panel.style.borderTopRightRadius = 12;
            _panel.style.borderBottomLeftRadius = 12;
            _panel.style.borderBottomRightRadius = 12;
            _panel.style.paddingLeft = 16; _panel.style.paddingRight = 16;
            _panel.style.paddingTop = 12; _panel.style.paddingBottom = 12;
            sampleText = root.Q<TextElement>();
            if (sampleText != null)
            {
                fontDef = sampleText.resolvedStyle.unityFontDefinition; // <- this is key

                // Apply to every text element you create
                // When you create the body label in DoShow():
                // lbl.style.unityFontDefinition = fontDef;  (see below)
            }
            // Try to copy classes from an existing container to blend style
            //var src = root.Q<VisualElement>("ServerBrowserContainer")
            //       ?? root.Q<VisualElement>("PauseMenuContainer");
            //if (src != null)
            //    foreach (var c in src.GetClasses()) _panel.AddToClassList(c);

            // Title
            _title = new Label("Title");
            _title.style.unityFontStyleAndWeight = FontStyle.Bold;
            _title.style.fontSize = 40;
            _title.style.alignSelf = Align.Center;
            _title.style.marginBottom = 8;
            _title.style.color = Color.white;
            _title.style.opacity = 1f;
            _title.style.backgroundColor = new Color(1, 0, 0, 0.25f);
            if (fontDef.HasValue) _title.style.unityFontDefinition = fontDef.Value;
            // Body (scrollable)
            _scroll = new ScrollView { name = "YourModModalScroll" };
            _scroll.style.flexGrow = 1;
            _scroll.style.marginBottom = 10;

            _body = new VisualElement { name = "YourModModalBody" };
            _body.style.flexDirection = FlexDirection.Column;
            ApplyVGap(_body, 6f);
            _scroll.Add(_body);

            // Footer with a single Close button (you can add more later)
            _footer = new VisualElement { name = "YourModModalFooter" };
            _footer.style.flexDirection = FlexDirection.Row;
            _footer.style.justifyContent = Justify.FlexEnd;
            ApplyHGap(_footer, 8f);

            _closeBtn = new Button(() => Hide()) { text = "CLOSE" };
            _closeBtn.style.height = 44;
            _closeBtn.style.minWidth = 120;
            _closeBtn.style.paddingLeft = 16;
            _closeBtn.style.paddingRight = 16;
            _closeBtn.style.borderTopLeftRadius = 8;
            _closeBtn.style.borderTopRightRadius = 8;
            _closeBtn.style.borderBottomLeftRadius = 8;
            _closeBtn.style.borderBottomRightRadius = 8;
            _closeBtn.style.backgroundColor = new Color(0.22f, 0.22f, 0.24f, 1f);
            _closeBtn.style.color = Color.white;
            _closeBtn.style.opacity = 1f;
            if (fontDef.HasValue) _closeBtn.style.unityFontDefinition = fontDef.Value;
            _overlay.RegisterCallback<MouseDownEvent>(e =>
            {
                // If you click the dimmed area (not the panel or its children), close
                if (!_panel.worldBound.Contains(e.mousePosition)) Hide();
            });
            // Copy button look from any known footer button
            //var footBtn = root.Q<Button>("RefreshButton") ?? root.Q<Button>("CloseButton");
            //if (footBtn != null)
            //    foreach (var c in footBtn.GetClasses()) _closeBtn.AddToClassList(c);
            _footer.Add(_closeBtn);

            _panel.Add(_title);
            _panel.Add(_scroll);
            _panel.Add(_footer);
            _overlay.Add(_panel);
            root.Add(_overlay);

            // Close on ESC
            root.RegisterCallback<KeyDownEvent>(OnKeyDown, TrickleDown.TrickleDown);

            // Keep on top
            _overlay.RegisterCallback<AttachToPanelEvent>(_ => _overlay.BringToFront());

            _built = true;
            if (_hasPending)
            {
                DoShow(_pendingTitle, _pendingRich, _pendingBuild);
                _hasPending = false;
                _pendingTitle = null; _pendingRich = null; _pendingBuild = null;
            }
        }

        public void RequestShow(string title, string richText, Action<VisualElement> build)
        {
            Debug.Log("Request Show");
            if (_built) DoShow(title, richText, build);
            else
            {
                _pendingTitle = title;
                _pendingRich = richText;
                _pendingBuild = build;
                _hasPending = true;
            }
        }
        void DoShow(string title, string richText, Action<VisualElement> build)
        {
            if (!_built) return;

            _title.text = title;

            _body.Clear();
            if (!string.IsNullOrEmpty(richText))
            {
                var lbl = new Label { enableRichText = true };
                lbl.text = richText;
                lbl.style.whiteSpace = WhiteSpace.Normal;
                lbl.style.fontSize = 16;
                lbl.style.color = Color.white;
                lbl.style.opacity = 1f;
                // ALSO set the borrowed font:
                if (sampleText != null)
                    lbl.style.unityFontDefinition = sampleText.resolvedStyle.unityFontDefinition;

                _body.Add(lbl);
            }
            build?.Invoke(_body);

            // Re-parent to the current root as LAST child (guaranteed on top)
            var root = UIManager.Instance?.RootVisualElement;
            if (root != null)
            {
                _overlay.RemoveFromHierarchy();
                root.Add(_overlay);
            }

            // Show
            _overlay.visible = true;                      // turn the visibility flag ON
            _overlay.style.display = DisplayStyle.Flex;   // and ensure display is ON
            _overlay.BringToFront();
            _overlay.Focus(); // optional: so ESC goes to us

            // (optional) one-time geometry log to confirm it’s on screen
            _overlay.schedule.Execute(() =>
                Debug.Log($"[SimpleModal] overlay={_overlay.worldBound} panel={_panel.worldBound} display={_overlay.resolvedStyle.display} visible={_overlay.visible}")
            ).StartingIn(0);
        }
        public void Show(string title, string richText = null, Action<VisualElement> build = null)
        {
            if (!_built) return;
            _title.text = title;

            _body.Clear();
            if (!string.IsNullOrEmpty(richText))
            {
                var lbl = new Label { enableRichText = true };
                lbl.text = richText;
                lbl.style.whiteSpace = WhiteSpace.Normal;
                lbl.style.fontSize = 16;
                _body.Add(lbl);
            }

            // optional custom content builder
            build?.Invoke(_body);

            _overlay.visible = true;
            _overlay.BringToFront();
        }

        public void Hide()
        {
            if (_overlay == null) return;
            _overlay.visible = false;
            _overlay.style.display = DisplayStyle.None;
        }

        public void TearDown()
        {
            var root = UIManager.Instance?.RootVisualElement;
            if (root != null) root.UnregisterCallback<KeyDownEvent>(OnKeyDown);
            _overlay?.RemoveFromHierarchy();
            _overlay = null;
        }

        void OnKeyDown(KeyDownEvent e)
        {
            if (_overlay == null || _overlay.resolvedStyle.display == DisplayStyle.None) return;
            if (e.keyCode == KeyCode.Escape)
            {
                e.StopImmediatePropagation();
                e.PreventDefault();
                Hide();
            }
        }
        static void ApplyHGap(VisualElement row, float px)
        {
            int i = 0;
            foreach (var child in row.Children())
            {
                child.style.marginLeft = (i == 0) ? 0 : px;
                i++;
            }
        }

        // Fake a vertical gap between children (like row-gap)
        static void ApplyVGap(VisualElement column, float px)
        {
            int i = 0;
            foreach (var child in column.Children())
            {
                child.style.marginTop = (i == 0) ? 0 : px;
                i++;
            }
        }
    }
}
