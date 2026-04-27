using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

// Token: 0x02000142 RID: 322
public class UIManager : MonoBehaviourSingleton<UIManager>
{
	// Token: 0x170000F5 RID: 245
	// (get) Token: 0x06000986 RID: 2438 RVA: 0x0002DEA7 File Offset: 0x0002C0A7
	[HideInInspector]
	public PanelSettings PanelSettings
	{
		get
		{
			return this.UIDocument.panelSettings;
		}
	}

	// Token: 0x170000F6 RID: 246
	// (get) Token: 0x06000987 RID: 2439 RVA: 0x0002DEB4 File Offset: 0x0002C0B4
	[HideInInspector]
	public VisualElement RootVisualElement
	{
		get
		{
			return this.UIDocument.rootVisualElement;
		}
	}

	// Token: 0x06000988 RID: 2440 RVA: 0x0002DEC4 File Offset: 0x0002C0C4
	public override void Awake()
	{
		base.Awake();
		this.UIDocument = base.GetComponent<UIDocument>();
		this.AudioSource = base.GetComponent<AudioSource>();
		this.RootVisualElement.style.display = (ApplicationManager.IsDedicatedGameServer ? DisplayStyle.None : DisplayStyle.Flex);
		this.RootVisualElement.RegisterCallback<PointerEnterEvent>(delegate(PointerEnterEvent e)
		{
			VisualElement visualElement = e.target as VisualElement;
			bool flag = visualElement != null && visualElement is Button && visualElement.enabledInHierarchy;
			bool flag2 = visualElement != null && visualElement.name == "unity-tab__header";
			if (flag || flag2)
			{
				this.PlaySelectSound();
			}
		}, TrickleDown.TrickleDown);
		this.RootVisualElement.RegisterCallback<PointerCaptureOutEvent>(delegate(PointerCaptureOutEvent e)
		{
			VisualElement visualElement = e.target as VisualElement;
			if (visualElement != null && visualElement is Button && visualElement.enabledInHierarchy)
			{
				this.PlayClickSound();
			}
		}, TrickleDown.TrickleDown);
		this.RootVisualElement.RegisterCallback<PointerDownEvent>(delegate(PointerDownEvent e)
		{
			VisualElement visualElement = e.target as VisualElement;
			if (visualElement != null && visualElement.name.Contains("unity-tab__header"))
			{
				this.PlayClickSound();
			}
		}, TrickleDown.TrickleDown);
		this.MainMenu = base.gameObject.GetComponent<UIMainMenu>();
		this.MainMenu.Initialize(this.RootVisualElement);
		this.views.Add(this.MainMenu);
		this.PauseMenu = base.gameObject.GetComponent<UIPauseMenu>();
		this.PauseMenu.Initialize(this.RootVisualElement);
		this.views.Add(this.PauseMenu);
		this.ServerBrowser = base.gameObject.GetComponent<UIServerBrowser>();
		this.ServerBrowser.Initialize(this.RootVisualElement);
		this.views.Add(this.ServerBrowser);
		this.GameState = base.gameObject.GetComponent<UIGameState>();
		this.GameState.Initialize(this.RootVisualElement);
		this.views.Add(this.GameState);
		this.Chat = base.gameObject.GetComponent<UIChat>();
		this.Chat.Initialize(this.RootVisualElement);
		this.views.Add(this.Chat);
		this.TeamSelect = base.gameObject.GetComponent<UITeamSelect>();
		this.TeamSelect.Initialize(this.RootVisualElement);
		this.views.Add(this.TeamSelect);
		this.PositionSelect = base.gameObject.GetComponent<UIPositionSelect>();
		this.PositionSelect.Initialize(this.RootVisualElement);
		this.views.Add(this.PositionSelect);
		this.Scoreboard = base.gameObject.GetComponent<UIScoreboard>();
		this.Scoreboard.Initialize(this.RootVisualElement);
		this.views.Add(this.Scoreboard);
		this.Settings = base.gameObject.GetComponent<UISettings>();
		this.Settings.Initialize(this.RootVisualElement);
		this.views.Add(this.Settings);
		this.Hud = base.gameObject.GetComponent<UIHUD>();
		this.Hud.Initialize(this.RootVisualElement);
		this.views.Add(this.Hud);
		this.Announcements = base.gameObject.GetComponent<UIAnnouncements>();
		this.Announcements.Initialize(this.RootVisualElement);
		this.views.Add(this.Announcements);
		this.Minimap = base.gameObject.GetComponent<UIMinimap>();
		this.Minimap.Initialize(this.RootVisualElement);
		this.views.Add(this.Minimap);
		this.NewServer = base.gameObject.GetComponent<UINewServer>();
		this.NewServer.Initialize(this.RootVisualElement);
		this.views.Add(this.NewServer);
		this.ToastManager = base.gameObject.GetComponent<UIToastManager>();
		this.ToastManager.Initialize(this.RootVisualElement);
		this.views.Add(this.ToastManager);
		this.OverlayManager = base.gameObject.GetComponent<UIOverlayManager>();
		this.OverlayManager.Initialize(this.RootVisualElement);
		this.views.Add(this.OverlayManager);
		this.PlayerMenu = base.gameObject.GetComponent<UIPlayerMenu>();
		this.PlayerMenu.Initialize(this.RootVisualElement);
		this.views.Add(this.PlayerMenu);
		this.Identity = base.gameObject.GetComponent<UIIdentity>();
		this.Identity.Initialize(this.RootVisualElement);
		this.views.Add(this.Identity);
		this.Appearance = base.gameObject.GetComponent<UIAppearance>();
		this.Appearance.Initialize(this.RootVisualElement);
		this.views.Add(this.Appearance);
		this.PopupManager = base.gameObject.GetComponent<UIPopupManager>();
		this.PopupManager.Initialize(this.RootVisualElement);
		this.views.Add(this.PopupManager);
		this.PlayerUsernames = base.gameObject.GetComponent<UIPlayerUsernames>();
		this.PlayerUsernames.Initialize(this.RootVisualElement);
		this.views.Add(this.PlayerUsernames);
		this.Debug = base.gameObject.GetComponent<UIDebug>();
		this.Debug.Initialize(this.RootVisualElement);
		this.views.Add(this.Debug);
		this.Mods = base.gameObject.GetComponent<UIMods>();
		this.Mods.Initialize(this.RootVisualElement);
		this.views.Add(this.Mods);
		this.Footer = base.gameObject.GetComponent<UIFooter>();
		this.Footer.Initialize(this.RootVisualElement);
		this.views.Add(this.Footer);
		this.Friends = base.gameObject.GetComponent<UIFriends>();
		this.Friends.Initialize(this.RootVisualElement);
		this.views.Add(this.Friends);
		this.Play = base.gameObject.GetComponent<UIPlay>();
		this.Play.Initialize(this.RootVisualElement);
		this.views.Add(this.Play);
		this.Matchmaking = base.gameObject.GetComponent<UIMatchmaking>();
		this.Matchmaking.Initialize(this.RootVisualElement);
		this.views.Add(this.Matchmaking);
		foreach (UIView uiview in this.views)
		{
			uiview.OnVisibility = (Action<UIView>)Delegate.Combine(uiview.OnVisibility, new Action<UIView>(this.OnViewVisibilityChanged));
			uiview.OnFocus = (Action<UIView>)Delegate.Combine(uiview.OnFocus, new Action<UIView>(this.OnViewFocusChanged));
		}
		InputManager.PauseAction.performed += this.OnPauseActionPerformed;
		InputManager.AllChatAction.canceled += this.OnAllChatActionPerformed;
		InputManager.TeamChatAction.canceled += this.OnTeamChatActionPerformed;
		InputManager.ScoreboardAction.started += this.OnScoreboardActionStarted;
		InputManager.ScoreboardAction.canceled += this.OnScoreboardActionCanceled;
	}

	// Token: 0x06000989 RID: 2441 RVA: 0x0002E574 File Offset: 0x0002C774
	private void OnDestroy()
	{
		foreach (UIView uiview in this.views)
		{
			uiview.OnVisibility = (Action<UIView>)Delegate.Remove(uiview.OnVisibility, new Action<UIView>(this.OnViewVisibilityChanged));
			uiview.OnFocus = (Action<UIView>)Delegate.Remove(uiview.OnFocus, new Action<UIView>(this.OnViewFocusChanged));
		}
		InputManager.PauseAction.performed -= this.OnPauseActionPerformed;
		InputManager.AllChatAction.canceled -= this.OnAllChatActionPerformed;
		InputManager.TeamChatAction.canceled -= this.OnTeamChatActionPerformed;
		InputManager.ScoreboardAction.started -= this.OnScoreboardActionStarted;
		InputManager.ScoreboardAction.canceled -= this.OnScoreboardActionCanceled;
	}

	// Token: 0x0600098A RID: 2442 RVA: 0x0002E670 File Offset: 0x0002C870
	private void Update()
	{
		if (!ApplicationManager.IsDedicatedGameServer && GlobalStateManager.UIState.IsMouseRequired)
		{
			this.CheckMouseOverUI();
		}
	}

	// Token: 0x0600098B RID: 2443 RVA: 0x0002E68C File Offset: 0x0002C88C
	private void HideAllViews()
	{
		foreach (UIView uiview in this.views)
		{
			uiview.Hide();
		}
	}

	// Token: 0x0600098C RID: 2444 RVA: 0x0002E6E0 File Offset: 0x0002C8E0
	public void ShowPhaseViews(UIPhase phase)
	{
		this.HideAllViews();
		switch (phase)
		{
		case UIPhase.None:
			break;
		case UIPhase.LockerRoom:
			this.Chat.Show();
			this.MainMenu.Show();
			this.Footer.Show();
			return;
		case UIPhase.Playing:
			this.Chat.Show();
			this.GameState.Show();
			this.Announcements.Show();
			break;
		default:
			return;
		}
	}

	// Token: 0x0600098D RID: 2445 RVA: 0x0002E74F File Offset: 0x0002C94F
	public void SetUIScale(float value)
	{
		this.PanelSettings.scale = value;
	}

	// Token: 0x0600098E RID: 2446 RVA: 0x0002E75D File Offset: 0x0002C95D
	public void PlaySelectSound()
	{
		if (this.selectAudioClip != null)
		{
			this.AudioSource.PlayOneShot(this.selectAudioClip);
		}
	}

	// Token: 0x0600098F RID: 2447 RVA: 0x0002E77E File Offset: 0x0002C97E
	public void PlayClickSound()
	{
		if (this.clickAudioClip != null)
		{
			this.AudioSource.PlayOneShot(this.clickAudioClip);
		}
	}

	// Token: 0x06000990 RID: 2448 RVA: 0x0002E79F File Offset: 0x0002C99F
	public void PlayNotificationSound()
	{
		if (this.notificationAudioClip != null)
		{
			this.AudioSource.PlayOneShot(this.notificationAudioClip);
		}
	}

	// Token: 0x06000991 RID: 2449 RVA: 0x0002E7C0 File Offset: 0x0002C9C0
	public void PlayWhooshSound()
	{
		if (this.whooshAudioClip != null)
		{
			this.AudioSource.PlayOneShot(this.whooshAudioClip);
		}
	}

	// Token: 0x06000992 RID: 2450 RVA: 0x0002E7E1 File Offset: 0x0002C9E1
	public void PlayTickSound()
	{
		if (this.tickAudioClip != null)
		{
			this.AudioSource.PlayOneShot(this.tickAudioClip);
		}
	}

	// Token: 0x06000993 RID: 2451 RVA: 0x0002E804 File Offset: 0x0002CA04
	private void CheckMouseRequirement()
	{
		foreach (UIView uiview in this.views)
		{
			if ((uiview.VisibilityRequiresMouse && uiview.IsVisible) || (uiview.FocusRequiresMouse && uiview.IsFocused))
			{
				GlobalStateManager.SetUIState(new Dictionary<string, object>
				{
					{
						"isMouseRequired",
						true
					}
				});
				return;
			}
		}
		GlobalStateManager.SetUIState(new Dictionary<string, object>
		{
			{
				"isMouseRequired",
				false
			}
		});
	}

	// Token: 0x06000994 RID: 2452 RVA: 0x0002E8A8 File Offset: 0x0002CAA8
	private void CheckInteraction()
	{
		List<UIView> list = new List<UIView>();
		foreach (UIView uiview in this.views)
		{
			if ((uiview.VisibilityIsInteractive && uiview.IsVisible) || (uiview.FocusIsInteractive && uiview.IsFocused))
			{
				list.Add(uiview);
			}
		}
		GlobalStateManager.SetUIState(new Dictionary<string, object>
		{
			{
				"interactingViews",
				list
			}
		});
	}

	// Token: 0x06000995 RID: 2453 RVA: 0x0002E938 File Offset: 0x0002CB38
	private void CheckMouseOverUI()
	{
		if (this.RootVisualElement == null)
		{
			return;
		}
		IPanel panel = this.RootVisualElement.panel;
		if (panel == null)
		{
			return;
		}
		Vector2 vector = InputManager.PointAction.ReadValue<Vector2>();
		vector.y = (float)Screen.height - vector.y;
		if (vector == this.lastPointerPosition)
		{
			return;
		}
		this.lastPointerPosition = vector;
		Vector2 point = RuntimePanelUtils.ScreenToPanel(panel, vector);
		bool flag = panel.Pick(point) != null;
		if (flag != GlobalStateManager.UIState.IsMouseOverUI)
		{
			GlobalStateManager.SetUIState(new Dictionary<string, object>
			{
				{
					"isMouseOverUI",
					flag
				}
			});
		}
	}

	// Token: 0x06000996 RID: 2454 RVA: 0x0002E9CE File Offset: 0x0002CBCE
	private void OnViewVisibilityChanged(UIView uiView)
	{
		if (uiView.VisibilityRequiresMouse)
		{
			this.CheckMouseRequirement();
		}
		if (uiView.VisibilityIsInteractive)
		{
			this.CheckInteraction();
		}
	}

	// Token: 0x06000997 RID: 2455 RVA: 0x0002E9EC File Offset: 0x0002CBEC
	private void OnViewFocusChanged(UIView uiView)
	{
		if (uiView.FocusRequiresMouse)
		{
			this.CheckMouseRequirement();
		}
		if (uiView.FocusIsInteractive)
		{
			this.CheckInteraction();
		}
	}

	// Token: 0x06000998 RID: 2456 RVA: 0x0002EA0C File Offset: 0x0002CC0C
	private void OnPauseActionPerformed(InputAction.CallbackContext context)
	{
		if (GlobalStateManager.UIState.Phase != UIPhase.Playing)
		{
			return;
		}
		if (this.PauseMenu.IsVisible && GlobalStateManager.UIState.IsViewTopmostInteracting<UIPauseMenu>())
		{
			this.PauseMenu.Hide();
			return;
		}
		if (!this.PauseMenu.IsVisible)
		{
			this.PauseMenu.Show();
		}
	}

	// Token: 0x06000999 RID: 2457 RVA: 0x0002EA6C File Offset: 0x0002CC6C
	private void OnAllChatActionPerformed(InputAction.CallbackContext context)
	{
		if (GlobalStateManager.UIState.Phase != UIPhase.Playing)
		{
			return;
		}
		if (GlobalStateManager.UIState.IsInteracting)
		{
			return;
		}
		this.Chat.StartInput(false);
	}

	// Token: 0x0600099A RID: 2458 RVA: 0x0002EAA4 File Offset: 0x0002CCA4
	private void OnTeamChatActionPerformed(InputAction.CallbackContext context)
	{
		if (GlobalStateManager.UIState.Phase != UIPhase.Playing)
		{
			return;
		}
		if (GlobalStateManager.UIState.IsInteracting)
		{
			return;
		}
		this.Chat.StartInput(true);
	}

	// Token: 0x0600099B RID: 2459 RVA: 0x0002EADC File Offset: 0x0002CCDC
	private void OnScoreboardActionStarted(InputAction.CallbackContext context)
	{
		if (GlobalStateManager.UIState.Phase != UIPhase.Playing)
		{
			return;
		}
		if (GlobalStateManager.UIState.IsInteracting)
		{
			return;
		}
		this.Scoreboard.Show();
	}

	// Token: 0x0600099C RID: 2460 RVA: 0x0002EB13 File Offset: 0x0002CD13
	private void OnScoreboardActionCanceled(InputAction.CallbackContext context)
	{
		this.Scoreboard.Hide();
	}

	// Token: 0x0400057B RID: 1403
	[Header("References")]
	[SerializeField]
	private AudioClip selectAudioClip;

	// Token: 0x0400057C RID: 1404
	[SerializeField]
	private AudioClip clickAudioClip;

	// Token: 0x0400057D RID: 1405
	[SerializeField]
	private AudioClip notificationAudioClip;

	// Token: 0x0400057E RID: 1406
	[SerializeField]
	private AudioClip whooshAudioClip;

	// Token: 0x0400057F RID: 1407
	[SerializeField]
	private AudioClip tickAudioClip;

	// Token: 0x04000580 RID: 1408
	[HideInInspector]
	public UIDocument UIDocument;

	// Token: 0x04000581 RID: 1409
	[HideInInspector]
	public AudioSource AudioSource;

	// Token: 0x04000582 RID: 1410
	[HideInInspector]
	public UIMainMenu MainMenu;

	// Token: 0x04000583 RID: 1411
	[HideInInspector]
	public UIPauseMenu PauseMenu;

	// Token: 0x04000584 RID: 1412
	[HideInInspector]
	public UIServerBrowser ServerBrowser;

	// Token: 0x04000585 RID: 1413
	[HideInInspector]
	public UIGameState GameState;

	// Token: 0x04000586 RID: 1414
	[HideInInspector]
	public UIChat Chat;

	// Token: 0x04000587 RID: 1415
	[HideInInspector]
	public UITeamSelect TeamSelect;

	// Token: 0x04000588 RID: 1416
	[HideInInspector]
	public UIPositionSelect PositionSelect;

	// Token: 0x04000589 RID: 1417
	[HideInInspector]
	public UIScoreboard Scoreboard;

	// Token: 0x0400058A RID: 1418
	[HideInInspector]
	public UISettings Settings;

	// Token: 0x0400058B RID: 1419
	[HideInInspector]
	public UIHUD Hud;

	// Token: 0x0400058C RID: 1420
	[HideInInspector]
	public UIAnnouncements Announcements;

	// Token: 0x0400058D RID: 1421
	[HideInInspector]
	public UIMinimap Minimap;

	// Token: 0x0400058E RID: 1422
	[HideInInspector]
	public UINewServer NewServer;

	// Token: 0x0400058F RID: 1423
	[HideInInspector]
	public UIToastManager ToastManager;

	// Token: 0x04000590 RID: 1424
	[HideInInspector]
	public UIOverlayManager OverlayManager;

	// Token: 0x04000591 RID: 1425
	[HideInInspector]
	public UIPlayerMenu PlayerMenu;

	// Token: 0x04000592 RID: 1426
	[HideInInspector]
	public UIIdentity Identity;

	// Token: 0x04000593 RID: 1427
	[HideInInspector]
	public UIAppearance Appearance;

	// Token: 0x04000594 RID: 1428
	[HideInInspector]
	public UIPopupManager PopupManager;

	// Token: 0x04000595 RID: 1429
	[HideInInspector]
	public UIPlayerUsernames PlayerUsernames;

	// Token: 0x04000596 RID: 1430
	[HideInInspector]
	public UIDebug Debug;

	// Token: 0x04000597 RID: 1431
	[HideInInspector]
	public UIMods Mods;

	// Token: 0x04000598 RID: 1432
	[HideInInspector]
	public UIFooter Footer;

	// Token: 0x04000599 RID: 1433
	[HideInInspector]
	public UIFriends Friends;

	// Token: 0x0400059A RID: 1434
	[HideInInspector]
	public UIPlay Play;

	// Token: 0x0400059B RID: 1435
	[HideInInspector]
	public UIMatchmaking Matchmaking;

	// Token: 0x0400059C RID: 1436
	private List<UIView> views = new List<UIView>();

	// Token: 0x0400059D RID: 1437
	private Vector2 lastPointerPosition = Vector2.zero;
}
