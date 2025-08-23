using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MOTD.Models;
using TestProject.Singletons;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using static MOTD.Behaviors.SimpleModal;

namespace MOTD.Behaviors
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
            //Debug.Log("[SimpleModal] Install -> Impl added");
        }

        public static void Uninstall()
        {
            _impl?.TearDown();
            if (_go) UnityEngine.Object.Destroy(_go);
            _go = null; _impl = null;
        }

        public static void Show(string title, string richText, string dontShowKey,
                        Action<VisualElement> build = null,
                        string bannerUrl = null, string panelBgUrl = null, float height = 50, float width = 50, ThemeDto theme = null, ModalDoc doc = null)
        {
            if (!string.IsNullOrEmpty(dontShowKey) && ModalPrefs.GetHide(dontShowKey)) return;
            _impl?.RequestShow(title, richText, build, dontShowKey, bannerUrl, panelBgUrl, height, width, theme, doc);
        }
        public static void ShowFromFile(string title, string filePath, string dontShowKey = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(dontShowKey) && ModalPrefs.GetHide(dontShowKey)) return;
                var txt = System.IO.File.ReadAllText(filePath, Encoding.UTF8);
                _impl?.RequestShow(title, txt, null, dontShowKey);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SimpleModal] ShowFromFile failed: {ex}");
            }
        }

        public static void Hide() => _impl?.Hide();

        public static bool IsOpen => _impl != null && _impl.IsOpen;
        public struct Theme
        {
            public Color Overlay;        // dim behind the panel
            public Color Panel;          // card background
            public Color Text;           // default text color
            public Color MutedText;      // optional secondary text
            public Color Button;         // normal button bg
            public Color ButtonHover;    // hover button bg
            public Color ButtonActive;   // pressed button bg
            public Color ToggleBox;      // checkbox box bg
            public Color ToggleCheck;    // checkbox check/tint
            public Color ScrollbarTrack; // scroll track
            public Color ScrollbarThumb; // scroll thumb
        }

        public static void ApplyTheme(Theme theme) => _impl?.ApplyTheme(theme);

        // Quick dark default (tweak as you like)
        public static Theme DarkDefault => new Theme
        {
            Overlay = new Color(0, 0, 0, 0.55f),
            Panel = new Color(0.10f, 0.10f, 0.12f, 1f),
            Text = Color.white,
            MutedText = new Color(0.78f, 0.78f, 0.8f, 1f),
            Button = new Color(0.22f, 0.22f, 0.24f, 1f),
            ButtonHover = new Color(0.28f, 0.28f, 0.32f, 1f),
            ButtonActive = new Color(0.18f, 0.18f, 0.20f, 1f),
            ToggleBox = new Color(0.38f, 0.38f, 0.40f, 1f),
            ToggleCheck = new Color(1f, 1f, 1f, 1f),
            ScrollbarTrack = new Color(0.12f, 0.12f, 0.14f, 1f),
            ScrollbarThumb = new Color(0.30f, 0.30f, 0.34f, 1f),
        };
        public static Theme TestTheme => new Theme
        {
            Overlay = new Color(0, 0, 0, 0.55f),
            Panel = Color.red,
            Text = Color.green,
            MutedText = Color.yellow,
            Button = Color.magenta,
            ButtonHover = Color.grey,
            ButtonActive = Color.white,
            ToggleBox = Color.gray,
            ToggleCheck = Color.black,
            ScrollbarTrack = Color.cyan,
            ScrollbarThumb = Color.red,
        };
    }

    [UnityEngine.Scripting.Preserve]   // helps with IL2CPP stripping
    public sealed class Impl : MonoBehaviour
    {
        // footer UI
        VisualElement _footLeft, _footCenter, _footRight;
        Toggle _optOutToggle;
        Label _optOutLabel;
        string _optOutKey;                   // current modal’s key (null/empty if none)
        string _pendingOptOutKey;
        VisualElement _overlay, _panel, _body, _footer;
        Label _title;
        ScrollView _scroll;
        Button _closeBtn;
        bool _built;
        TextElement sampleText;
        // pending request in case Show() comes before Build()
        string _pendingTitle, _pendingRich, _pendingBannerUrl;
        Action<VisualElement> _pendingBuild;
        bool _hasPending;
        FontDefinition? fontDef;
        public bool IsOpen => _overlay != null && _overlay.resolvedStyle.display != DisplayStyle.None;
        Image _banner;
        string _requestedBannerUrl;
        float _bannerAspect = 0f;
        readonly Dictionary<string, Texture2D> _bannerCache = new Dictionary<string, Texture2D>();
        Image _panelBgImg;
        float _panelBgAspect;
        readonly Dictionary<string, Texture2D> _texCache = new Dictionary<string, Texture2D>();
        string _pendingPanelBgUrl;
        string _currentPanelBgUrl;
        ThemeDto _pendingTheme;
        Action _onDiscordClicked, _onActionClicked;
        Action _discordHandler;
        Action _actionHandler;
        void Awake()
        {
            //Debug.Log("[SimpleModal] Impl.Awake");
            StartCoroutine(Bootstrap());
        }

        IEnumerator Bootstrap()
        {
            // wait for UI root to exist
            while (UIManager.Instance == null || UIManager.Instance.RootVisualElement == null)
                yield return null;

            //Debug.Log("[SimpleModal] Bootstrap -> Build()");
            Build(UIManager.Instance.RootVisualElement);
        }
        static string NormalizePathOrUrl(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;

            // already a URL?
            if (s.StartsWith("http", StringComparison.OrdinalIgnoreCase) ||
                s.StartsWith("file://", StringComparison.OrdinalIgnoreCase))
                return s;

            // local file?
            if (File.Exists(s))
                return "file://" + s.Replace("\\", "/");

            return null; // not a valid url or file path
        }
        void Build(VisualElement root)
        {
            if (_built) return;

            //Debug.Log("Entered Build");
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
            _panel.style.width = Length.Percent(50);
            _panel.style.maxWidth = Length.Percent(90);
            _panel.style.height = Length.Percent(90); ;
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
            _panel.style.overflow = Overflow.Hidden;
            _panelBgImg = new Image { name = "YourModModalPanelBg" };
            _panelBgImg.pickingMode = PickingMode.Ignore;
            _panelBgImg.scaleMode = ScaleMode.ScaleAndCrop;  // cover the panel
            _panelBgImg.style.position = Position.Absolute;
            _panelBgImg.style.left = 0; _panelBgImg.style.right = 0;
            _panelBgImg.style.top = 0; _panelBgImg.style.bottom = 0;
            _panel.Add(_panelBgImg);
            var panelTint = new VisualElement();
            panelTint.style.position = Position.Absolute;
            panelTint.style.left = 0; panelTint.style.right = 0;
            panelTint.style.top = 0; panelTint.style.bottom = 0;
            panelTint.style.backgroundColor = new Color(0, 0, 0, 0.25f);
            _panel.Add(panelTint);
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
            _banner = new Image { name = "YourModModalBanner" };
            _banner.style.display = DisplayStyle.None;
            _banner.style.width = Length.Percent(100);
            _banner.style.height = 140; // provisional height; we’ll sync to aspect later
            _banner.scaleMode = ScaleMode.ScaleToFit;   // keep aspect inside the box
            _banner.pickingMode = PickingMode.Ignore;   // clicks pass through to panel
            _panel.Add(_banner);
            _title = new Label { name = "YourModModalTitle", enableRichText = true };
            _title.style.whiteSpace = WhiteSpace.Normal;      // allow wrapping
            _title.style.unityFontStyleAndWeight = FontStyle.Bold;
            _title.style.fontSize = 40;
            _title.style.alignSelf = Align.Center;
            _title.style.color = Color.white;
            _title.style.opacity = 1f;
            _title.style.maxHeight = 96;     // ~2–3 lines depending on fontSize
            _title.style.overflow = Overflow.Hidden;
            if (fontDef.HasValue) _title.style.unityFontDefinition = fontDef.Value;
            // Body (scrollable)
            _scroll = new ScrollView { name = "YourModModalScroll" };
            _scroll.style.flexGrow = 1;
            _scroll.style.flexShrink = 1;   // <- critical
            _scroll.style.flexBasis = 0;    // <- avoid min-content sizing
            _scroll.style.minHeight = 0;    // <- critical in UI Toolkit
            _scroll.contentContainer.style.minHeight = 0;

            _scroll.style.marginBottom = 10;

            _body = new VisualElement { name = "YourModModalBody" };
            _body.style.flexDirection = FlexDirection.Column;
            ApplyVGap(_body, 6f);
            _scroll.Add(_body);

            //_footer = new VisualElement { name = "YourModModalFooter" };
            //_footer.style.flexDirection = FlexDirection.Row;
            //_footer.style.justifyContent = Justify.SpaceBetween;

            //// left side: opt-out area (hidden by default)
            //_footLeft = new VisualElement { name = "YourModModalFooterLeft" };
            //_footLeft.style.flexDirection = FlexDirection.Row;
            //ApplyHGap(_footLeft, 8f);

            _footer = new VisualElement { name = "YourModModalFooter" };
            _footer.style.flexDirection = FlexDirection.Row;
            _footer.style.alignItems = Align.Center;

            // Lanes
            _footLeft = new VisualElement { name = "YourModModalFooterLeft" };
            _footCenter = new VisualElement { name = "YourModModalFooterCenter" };
            _footRight = new VisualElement { name = "YourModModalFooterRight" };

            _footLeft.style.flexDirection = FlexDirection.Row;
            _footCenter.style.flexDirection = FlexDirection.Row;
            _footRight.style.flexDirection = FlexDirection.Row;

            // Keep left/right at content-size, let center expand and center its content
            _footLeft.style.flexShrink = 0;
            _footRight.style.flexShrink = 0;
            _footCenter.style.flexGrow = 1;
            _footCenter.style.justifyContent = Justify.Center;
            _footCenter.style.alignItems = Align.Center;

            _footer.Add(_footLeft);
            _footer.Add(_footCenter);
            _footer.Add(_footRight);

            // Buttons (images)
            //_discordBtn = MakeImageButton(out _discordImg, 44f, 8f, 0f, 5);
            //_discordBtn.tooltip = "Join our Discord";
            //_discordBtn.style.display = DisplayStyle.None;
            //_discordBtn.style.marginRight = 12;
            
            //_actionBtn = MakeImageButton(out _actionImg, 44f, 8f, 0f, 5);
            //_actionBtn.tooltip = "Open link";
            //_actionBtn.style.display = DisplayStyle.None;
            //SetButtonSize(_discordBtn, 44f); // 44 x 110
            //SetButtonSize(_actionBtn, 44f);  // 44 x 110
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
            _closeBtn.style.unityTextAlign = TextAnchor.MiddleCenter; // vertical + horizontal
            _closeBtn.style.alignItems = Align.Center;            // center any children
            _closeBtn.style.justifyContent = Justify.Center;          // …
            _closeBtn.style.paddingTop = 0;
            _closeBtn.style.paddingBottom = 0;
            _closeBtn.style.opacity = 1f;
            if (fontDef.HasValue) _closeBtn.style.unityFontDefinition = fontDef.Value;
            _footRight.Add(_closeBtn);
            //_footLeft.Add(_discordBtn);
            //_footLeft.Add(_actionBtn);
            
            _footer.Add(_footLeft);
            _footer.Add(_footCenter);
            _footer.Add(_footRight);

            
            //_overlay.RegisterCallback<MouseDownEvent>(e =>
            //{
            //    // If you click the dimmed area (not the panel or its children), close
            //    if (!_panel.worldBound.Contains(e.mousePosition)) Hide();
            //});
            // Copy button look from any known footer button
            //var footBtn = root.Q<Button>("RefreshButton") ?? root.Q<Button>("CloseButton");
            //if (footBtn != null)
            //    foreach (var c in footBtn.GetClasses()) _closeBtn.AddToClassList(c);
            //_footer.Add(_closeBtn);

            _panel.Add(_title);
            _panel.Add(_scroll);
            _panel.Add(_footer);
            _overlay.Add(_panel);
            root.Add(_overlay);
            // Close on ESC
            root.RegisterCallback<KeyDownEvent>(OnKeyDown, TrickleDown.TrickleDown);

            // Keep on top
            _overlay.RegisterCallback<AttachToPanelEvent>(_ => _overlay.BringToFront());
            _panel.RegisterCallback<GeometryChangedEvent>(_ => SyncBannerSize());
            _built = true;
            if (_hasPending)
            {
                DoShow(_pendingTitle, _pendingRich, _pendingBuild, _pendingOptOutKey, _pendingBannerUrl, _pendingPanelBgUrl);
                _hasPending = false;
                _pendingTitle = _pendingRich = _pendingOptOutKey = _pendingPanelBgUrl = null;
                _pendingBuild = null;
            }
        }
        static string NormalizeRich(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            return s.Replace("<br/>", "\n")
                    .Replace("<br>", "\n")
                    .Replace("<BR/>", "\n")
                    .Replace("<BR>", "\n");
        }
        FooterBtn CreateAndBindFooterButton(ButtonSpec spec)
        {
            if (spec == null || string.IsNullOrWhiteSpace(spec.url)) return null;

            var fb = new FooterBtn();
            fb.button = MakeImageButton(out fb.image, (spec.height > 0 ? spec.height : 60f),
                                        (spec.corner > 0 ? spec.corner : 8f), spec.pad, spec.choke, 0f, ThemeMapper.Hex(spec.backgroundColor, Color.clear), ThemeMapper.Hex(spec.backgroundHover, Color.clear), ThemeMapper.Hex(spec.backgroundActive, Color.clear));
            SetButtonSize(fb.button, (spec.height > 0 ? spec.height : 60f),
                                        (spec.aspect > 0 ? spec.aspect : 2.5f));
            fb.button.tooltip = string.IsNullOrWhiteSpace(spec.name) ? "Open link" : spec.name;
            fb.button.style.marginRight = spec.marginRight;

            // theme background like other buttons (if you have a StyleButton(theme) helper)
            StyleButton(fb.button, new Theme()
            {
                Button = ThemeMapper.Hex(spec.backgroundColor, Color.clear),
                ButtonActive = ThemeMapper.Hex(spec.backgroundActive, Color.clear),
                ButtonHover = ThemeMapper.Hex(spec.backgroundHover, Color.clear)
            });

            // Click handler
            fb.handler = () => Application.OpenURL(spec.url);
            fb.button.clicked += fb.handler;

            // Load icon (optional)
            fb.button.style.display = DisplayStyle.None;
            fb.image.image = null;

            if (string.IsNullOrWhiteSpace(spec.iconUrl))
            {
                // no icon — just show the empty button (you could add a text label here if you want)
                fb.button.style.display = DisplayStyle.Flex;
            }
            else
            {
                StartCoroutine(LoadIntoImage(spec.iconUrl, fb.image, ok =>
                {
                    fb.button.style.display = ok ? DisplayStyle.Flex : DisplayStyle.None;
                }));
            }

            // Add to the footer (left lane)
            _footLeft.style.display = DisplayStyle.Flex;
            _footLeft.Add(fb.button);

            _dynButtons.Add(fb);
            return fb;
        }
        Button MakeImageButton(out Image img, float height, float aspect, float pad, float cornerRadius, float chokePx, Color background, Color backgroundHover, Color backgroundActive)
        {
            var b = new Button { pickingMode = PickingMode.Position };

            // Size + rounded clip
            SetButtonSize(b, height, aspect);
            b.style.borderTopLeftRadius = cornerRadius;
            b.style.borderTopRightRadius = cornerRadius;
            b.style.borderBottomLeftRadius = cornerRadius;
            b.style.borderBottomRightRadius = cornerRadius;
            b.style.overflow = Overflow.Hidden;     // <-- makes rounding clip the child
            b.style.backgroundColor = background;
            b.style.justifyContent = Justify.Center;
            b.style.alignItems = Align.Center;

            // Image fills the button, but keeps its own aspect
            img = new Image { pickingMode = PickingMode.Ignore };
            img.scaleMode = ScaleMode.ScaleToFit;                  // <-- no stretching
            img.style.width = Length.Percent(100);
            img.style.height = Length.Percent(100);

            // “Choke” the image slightly so corners look nicely rounded
            if (chokePx > 0f)
            {
                img.style.marginLeft = chokePx;
                img.style.marginRight = chokePx;
                img.style.marginTop = chokePx;
                img.style.marginBottom = chokePx;
            }

            b.Add(img);

            // simple hover/press
            b.RegisterCallback<PointerEnterEvent>(_ => b.style.backgroundColor = backgroundHover);
            b.RegisterCallback<PointerLeaveEvent>(_ => b.style.backgroundColor = background);
            b.RegisterCallback<PointerDownEvent>(_ => b.style.backgroundColor = backgroundActive);
            b.RegisterCallback<PointerUpEvent>(_ => b.style.backgroundColor = backgroundHover);

            return b;
        }
        static void SetButtonSize(VisualElement b, float height, float aspect = 2.5f)
        {
            float width = height * aspect;
            b.style.width = width;
            b.style.height = height;
            b.style.minWidth = width; b.style.minHeight = height;
            b.style.maxWidth = width; b.style.maxHeight = height;
            b.style.flexGrow = 0; b.style.flexShrink = 0;
        }
        public void RequestShow(string title, string richText, Action<VisualElement> build,
                        string optOutKey = null, string bannerUrl = null, string panelBgUrl = null, float height = 50, float width = 50, ThemeDto theme = null, ModalDoc doc = null)
        {
            if (_built)
                DoShow(title, richText, build, optOutKey, bannerUrl, panelBgUrl, height, width, theme, doc); // <-- pass it
            else
            {
                _pendingTitle = title;
                _pendingRich = richText;
                _pendingBuild = build;
                _pendingOptOutKey = optOutKey;
                _pendingPanelBgUrl = panelBgUrl;   // keep for later
                _pendingTheme = theme;
                _hasPending = true;
            }
        }
        void DoShow(
    string title,
    string richText,
    Action<VisualElement> build,
    string optOutKey,
    string bannerUrl,
    string panelBgUrl = null,
    float height = 50f,
    float width = 50f,
    ThemeDto theme = null,
    ModalDoc doc = null)
        {
            _optOutKey = optOutKey;

            // Theme (if provided)
            if (theme != null)
                ApplyTheme(ThemeMapper.ToTheme(theme, SimpleModal.DarkDefault));

            // Title prepared even if banner hides it (normalize for <br/> support)
            _title.text = NormalizeRich(title ?? string.Empty);
            _title.style.display = DisplayStyle.Flex;

            // Reset banner state
            _banner.image = null;
            _banner.style.display = DisplayStyle.None;
            _requestedBannerUrl = null;

            // Reset BG image state (hidden until loaded)
            _panelBgImg.image = null;
            _panelBgImg.style.display = DisplayStyle.None;
            _currentPanelBgUrl = null;

            // Panel size as percentages (validated)
            width = Mathf.Clamp(width, 10f, 100f);
            height = Mathf.Clamp(height, 10f, 100f);
            _panel.style.width = Length.Percent(width);
            _panel.style.height = Length.Percent(height);

            // Body
            _body.Clear();
            if (!string.IsNullOrEmpty(richText))
            {
                var lbl = new Label { enableRichText = true, text = NormalizeRich(richText) };
                lbl.style.whiteSpace = WhiteSpace.Normal;
                lbl.style.fontSize = 16;
                lbl.style.color = _theme.Text; // keep in sync with theme
                if (fontDef.HasValue) lbl.style.unityFontDefinition = fontDef.Value;
                _body.Add(lbl);
            }
            build?.Invoke(_body);

            // Banner
            if (!string.IsNullOrEmpty(bannerUrl))
            {
                _requestedBannerUrl = bannerUrl;
                if (_bannerCache.TryGetValue(bannerUrl, out var cached) && cached != null)
                    ApplyBannerTexture(cached);
                else
                    StartCoroutine(LoadBannerCoroutine(bannerUrl));
            }
            else
            {
                _banner.style.display = DisplayStyle.None;
                _title.style.display = DisplayStyle.Flex;
            }

            // Panel background (show only after load succeeds)
            _currentPanelBgUrl = panelBgUrl ?? string.Empty;
            if (!string.IsNullOrEmpty(_currentPanelBgUrl))
            {
                StartCoroutine(LoadIntoImage(_currentPanelBgUrl, _panelBgImg, ok =>
                {
                    _panelBgImg.style.display = ok ? DisplayStyle.Flex : DisplayStyle.None;
                }));
            }
            else
            {
                _panelBgImg.image = null;
                _panelBgImg.style.display = DisplayStyle.None;
            }

            // Footer image buttons — pre-hide, then bind if doc provided
            //_discordBtn.style.display = DisplayStyle.None;
            //_actionBtn.style.display = DisplayStyle.None;
            ClearDynamicButtons();
            _footLeft.style.display = DisplayStyle.None; // will flip to Flex if we add any

            //EnsureButtonsFromLegacy(doc);
            if (doc?.buttons != null && doc.buttons.Count > 0)
            {
                foreach (var spec in doc.buttons)
                    CreateAndBindFooterButton(spec);
            }

            // Show
            var root = UIManager.Instance?.RootVisualElement;
            if (root != null) { _overlay.RemoveFromHierarchy(); root.Add(_overlay); }
            _overlay.style.display = DisplayStyle.Flex;
            _overlay.visible = true;
            _overlay.BringToFront();
            _overlay.Focus();
        }
        readonly List<FooterBtn> _dynButtons = new List<FooterBtn>();

        void ClearDynamicButtons()
        {
            foreach (var fb in _dynButtons)
            {
                if (fb != null && fb.button != null)
                {
                    if (fb.handler != null) fb.button.clicked -= fb.handler;
                    fb.button.RemoveFromHierarchy();
                }
            }
            _dynButtons.Clear();
        }
        //void BindDiscordButton(string iconUrl, string clickUrl, float size)
        //{
        //    // Hide if there’s no target link
        //    if (string.IsNullOrWhiteSpace(clickUrl))
        //    {
        //        _discordBtn.style.display = DisplayStyle.None;
        //        return;
        //    }

        //    // size (optional)
        //    if (size > 0f) { _discordBtn.style.width = size; _discordBtn.style.height = size; }

        //    // reset visual state while loading
        //    _discordBtn.style.display = DisplayStyle.None;
        //    _discordImg.image = null;
        //    _discordImg.tintColor = Color.white;

        //    // (re)bind click
        //    if (_discordHandler != null) _discordBtn.clicked -= _discordHandler;
        //    _discordHandler = () => Application.OpenURL(clickUrl);
        //    _discordBtn.clicked += _discordHandler;

        //    if (string.IsNullOrWhiteSpace(iconUrl))
        //    {
        //        // allow showing without an icon if you want
        //        _discordBtn.style.display = DisplayStyle.Flex;
        //        return;
        //    }

        //    StartCoroutine(LoadIntoImage(iconUrl, _discordImg, ok =>
        //    {
        //        _discordBtn.style.display = ok ? DisplayStyle.Flex : DisplayStyle.None;
        //    }));
        //}

        //void BindActionButton(string iconUrl, string clickUrl, float size)
        //{
        //    if (string.IsNullOrWhiteSpace(clickUrl))
        //    {
        //        _actionBtn.style.display = DisplayStyle.None;
        //        return;
        //    }

        //    if (size > 0f) { _actionBtn.style.width = size; _actionBtn.style.height = size; }

        //    _actionBtn.style.display = DisplayStyle.None;
        //    _actionImg.image = null;
        //    _actionImg.tintColor = Color.white;

        //    if (_actionHandler != null) _actionBtn.clicked -= _actionHandler;
        //    _actionHandler = () => Application.OpenURL(clickUrl);
        //    _actionBtn.clicked += _actionHandler;

        //    if (string.IsNullOrWhiteSpace(iconUrl))
        //    {
        //        _actionBtn.style.display = DisplayStyle.Flex;
        //        return;
        //    }

        //    StartCoroutine(LoadIntoImage(iconUrl, _actionImg, ok =>
        //    {
        //        _actionBtn.style.display = ok ? DisplayStyle.Flex : DisplayStyle.None;
        //    }));
        //}
        IEnumerator LoadIntoImage(string url, Image target, Action<bool> done = null)
        {
            if (target == null || string.IsNullOrWhiteSpace(url))
            {
                done?.Invoke(false);
                yield break;
            }

            // local file normalization
            if (!url.StartsWith("http", StringComparison.OrdinalIgnoreCase) &&
                !url.StartsWith("file://", StringComparison.OrdinalIgnoreCase) &&
                System.IO.File.Exists(url))
            {
                url = "file://" + url.Replace("\\", "/");
            }

            if (_texCache.TryGetValue(url, out var cached) && cached != null)
            {
                target.image = cached;
                done?.Invoke(true);
                yield break;
            }

            using (var req = UnityEngine.Networking.UnityWebRequestTexture.GetTexture(url))
            {
                yield return req.SendWebRequest();
#if UNITY_2020_2_OR_NEWER
        bool ok = req.result == UnityEngine.Networking.UnityWebRequest.Result.Success;
#else
                bool ok = !req.isNetworkError && !req.isHttpError;
#endif
                if (!ok)
                {
                    Debug.LogWarning($"[SimpleModal] Failed to load image: {url} ({req.error})");
                    done?.Invoke(false);
                    yield break;
                }

                var tex = UnityEngine.Networking.DownloadHandlerTexture.GetContent(req);
                if (tex != null)
                {
                    _texCache[url] = tex;
                    target.image = tex;
                    done?.Invoke(true);
                }
                else
                {
                    done?.Invoke(false);
                }
            }
        }
        IEnumerator LoadBannerCoroutine(string url)
        {
            using (var req = UnityWebRequestTexture.GetTexture(url))
            {
                yield return req.SendWebRequest();

#if UNITY_2020_2_OR_NEWER
        bool ok = req.result == UnityWebRequest.Result.Success;
#else
                bool ok = !req.isNetworkError && !req.isHttpError;
#endif
                if (!ok)
                {
                    Debug.LogWarning($"[SimpleModal] Banner load failed: {url} ({req.error})");
                    if (_requestedBannerUrl == url)
                    {
                        // ensure fallback: hide banner, show title
                        _banner.image = null;
                        _banner.style.display = DisplayStyle.None;
                        _title.style.display = DisplayStyle.Flex;
                    }
                    yield break;
                }

                var tex = DownloadHandlerTexture.GetContent(req);
                if (tex == null)
                {
                    if (_requestedBannerUrl == url)
                    {
                        _banner.image = null;
                        _banner.style.display = DisplayStyle.None;
                        _title.style.display = DisplayStyle.Flex;
                    }
                    yield break;
                }

                _bannerCache[url] = tex;
                if (_requestedBannerUrl == url)
                    ApplyBannerTexture(tex); // shows banner + hides title
            }
        }

        void ApplyBannerTexture(Texture2D tex)
        {
            _banner.image = tex;                    // UI Toolkit Image
            _bannerAspect = (tex != null && tex.width > 0) ? (float)tex.height / tex.width : 0f;

            _banner.style.display = DisplayStyle.Flex;  // show banner
            _title.style.display = DisplayStyle.None;  // hide title

            SyncBannerSize();                           // size it to panel
        }

        // Maintain aspect ratio based on panel width
        void SyncBannerSize()
        {
            if (_banner.style.display == DisplayStyle.None) return;
            float panelW = _panel.resolvedStyle.width;
            if (panelW <= 0 || _bannerAspect <= 0) return;

            // Full width, height from aspect; clamp a little
            float h = Mathf.Clamp(panelW * _bannerAspect, 80f, 260f);
            _banner.style.width = Length.Percent(100);
            _banner.style.height = h;
        }
        public void Show(string title, string richText = null, Action<VisualElement> build = null)
        {
            if (!_built) return;
            _title.text = title;

            _body.Clear();
            if (!string.IsNullOrEmpty(richText))
            {
                var lbl = new Label { enableRichText = true };
                lbl.text = NormalizeRich(richText);
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

            // commit opt-out if this modal had a key
            if (!string.IsNullOrEmpty(_optOutKey))
                ModalPrefs.SetHide(_optOutKey, _optOutToggle.value);

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
        SimpleModal.Theme _theme = SimpleModal.DarkDefault;

        public void ApplyTheme(SimpleModal.Theme t)
        {
            _theme = t;

            // Container backgrounds
            if (_overlay != null) _overlay.style.backgroundColor = t.Overlay;
            if (_panel != null) _panel.style.backgroundColor = t.Panel;

            // Default text color for everything inside the panel
            if (_panel != null) _panel.style.color = t.Text; // inherited by TextElements

            // Title
            if (_title != null) _title.style.color = t.Text;

            // Close button (base color)
            if (_closeBtn != null)
            {
                StyleButton(_closeBtn, t);
            }

            // Toggle (checkbox)
            if (_optOutToggle != null)
            {
                var box = _optOutToggle.Q(className: "unity-toggle__input")
                       ?? _optOutToggle.Q(className: "unity-base-field__input");
                if (box != null) box.style.backgroundColor = t.ToggleBox;

                var check = _optOutToggle.Q(className: "unity-toggle__checkmark");
                if (check != null)
                {
                    // Many themes color the checkmark via image tint:
                    check.style.unityBackgroundImageTintColor = t.ToggleCheck;
                    // if no effect, also try:
                    check.style.backgroundColor = t.ToggleCheck;
                }

                if (_optOutLabel != null) _optOutLabel.style.color = t.Text;

                StyleOptOutToggle();
            }

            // Scrollbar colors
            if (_scroll != null)
            {
                var v = _scroll.verticalScroller;
                var h = _scroll.horizontalScroller;

                void StyleScroller(Scroller sc)
                {
                    if (sc == null) return;
                    var track = sc.Q(className: "unity-base-slider__tracker");
                    var thumb = sc.Q(className: "unity-base-slider__dragger");
                    if (track != null) track.style.backgroundColor = t.ScrollbarTrack;
                    if (thumb != null) thumb.style.backgroundColor = t.ScrollbarThumb;
                }
                StyleScroller(v);
                StyleScroller(h);
            }

            // Re-style any existing rich text blocks in the body
            if (_body != null)
            {
                foreach (var te in _body.Children().OfType<TextElement>())
                    te.style.color = t.Text;
            }
        }
        void StyleButton(Button b, SimpleModal.Theme t)
        {
            b.style.backgroundColor = t.Button;
            b.style.color = t.Text;

            // Remove old handlers then rewire (so we don’t stack them)
            b.UnregisterCallback<PointerEnterEvent>(OnBtnEnter);
            b.UnregisterCallback<PointerLeaveEvent>(OnBtnLeave);
            b.UnregisterCallback<PointerDownEvent>(OnBtnDown);
            b.UnregisterCallback<PointerUpEvent>(OnBtnUp);

            b.RegisterCallback<PointerEnterEvent>(OnBtnEnter);
            b.RegisterCallback<PointerLeaveEvent>(OnBtnLeave);
            b.RegisterCallback<PointerDownEvent>(OnBtnDown);
            b.RegisterCallback<PointerUpEvent>(OnBtnUp);

            void OnBtnEnter(PointerEnterEvent _) => b.style.backgroundColor = t.ButtonHover;
            void OnBtnLeave(PointerLeaveEvent _) => b.style.backgroundColor = t.Button;
            void OnBtnDown(PointerDownEvent _) => b.style.backgroundColor = t.ButtonActive;
            void OnBtnUp(PointerUpEvent _) => b.style.backgroundColor = t.ButtonHover;
        }
        void StyleOptOutToggle()
        {
            if (_optOutToggle == null) return;

            var box = _optOutToggle.Q(className: "unity-toggle__input")
                     ?? _optOutToggle.Q(className: "unity-base-field__input");
            if (box != null)
                box.style.backgroundColor = _theme.ToggleBox;

            var check = _optOutToggle.Q(className: "unity-toggle__checkmark");
            if (check != null)
            {
                // Tint only; DO NOT force it visible with backgroundColor/display/opacity
                check.style.unityBackgroundImageTintColor = _theme.ToggleCheck;
                check.style.backgroundColor = StyleKeyword.Null; // critical
                check.style.display = StyleKeyword.Null; // let USS show/hide
                check.style.opacity = StyleKeyword.Null;
            }
        }
    }
    sealed class FooterBtn
    {
        public Button button;
        public Image image;
        public Action handler;
    }
}
