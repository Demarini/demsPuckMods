using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// Token: 0x020000C0 RID: 192
public class UIManager : NetworkBehaviourSingleton<UIManager>
{
	// Token: 0x060005A9 RID: 1449 RVA: 0x000233A0 File Offset: 0x000215A0
	public override void Awake()
	{
		base.Awake();
		this.AudioSource = base.gameObject.GetComponent<AudioSource>();
		this.UiDocument = base.GetComponent<UIDocument>();
		this.PanelSettings = this.UiDocument.panelSettings;
		this.RootVisualElement = this.UiDocument.rootVisualElement;
		this.RootVisualElement.RegisterCallback<MouseEnterEvent>(delegate(MouseEnterEvent e)
		{
			VisualElement visualElement = e.target as VisualElement;
			bool flag = visualElement != null && visualElement is Button;
			bool flag2 = visualElement != null && visualElement.name == "unity-tab__header";
			if (flag || flag2)
			{
				this.PlayerSelectSound();
			}
		}, TrickleDown.TrickleDown);
		this.RootVisualElement.RegisterCallback<MouseDownEvent>(delegate(MouseDownEvent e)
		{
			VisualElement visualElement = e.target as VisualElement;
			bool flag = visualElement != null && visualElement is Button;
			bool flag2 = visualElement != null && visualElement.name.Contains("unity-tab__header");
			if (flag || flag2)
			{
				this.PlayerClickSound();
			}
		}, TrickleDown.TrickleDown);
		this.MainMenu = base.gameObject.GetComponent<UIMainMenu>();
		this.MainMenu.Initialize(this.RootVisualElement);
		this.components.Add(this.MainMenu);
		this.PauseMenu = base.gameObject.GetComponent<UIPauseMenu>();
		this.PauseMenu.Initialize(this.RootVisualElement);
		this.components.Add(this.PauseMenu);
		this.ServerBrowser = base.gameObject.GetComponent<UIServerBrowser>();
		this.ServerBrowser.Initialize(this.RootVisualElement);
		this.components.Add(this.ServerBrowser);
		this.GameState = base.gameObject.GetComponent<UIGameState>();
		this.GameState.Initialize(this.RootVisualElement);
		this.components.Add(this.GameState);
		this.Chat = base.gameObject.GetComponent<UIChat>();
		this.Chat.Initialize(this.RootVisualElement);
		this.components.Add(this.Chat);
		this.TeamSelect = base.gameObject.GetComponent<UITeamSelect>();
		this.TeamSelect.Initialize(this.RootVisualElement);
		this.components.Add(this.TeamSelect);
		this.PositionSelect = base.gameObject.GetComponent<UIPositionSelect>();
		this.PositionSelect.Initialize(this.RootVisualElement);
		this.components.Add(this.PositionSelect);
		this.Scoreboard = base.gameObject.GetComponent<UIScoreboard>();
		this.Scoreboard.Initialize(this.RootVisualElement);
		this.components.Add(this.Scoreboard);
		this.Settings = base.gameObject.GetComponent<UISettings>();
		this.Settings.Initialize(this.RootVisualElement);
		this.components.Add(this.Settings);
		this.Hud = base.gameObject.GetComponent<UIHUD>();
		this.Hud.Initialize(this.RootVisualElement);
		this.components.Add(this.Hud);
		this.Announcement = base.gameObject.GetComponent<UIAnnouncement>();
		this.Announcement.Initialize(this.RootVisualElement);
		this.components.Add(this.Announcement);
		this.Minimap = base.gameObject.GetComponent<UIMinimap>();
		this.Minimap.Initialize(this.RootVisualElement);
		this.components.Add(this.Minimap);
		this.ServerLauncher = base.gameObject.GetComponent<UIServerLauncher>();
		this.ServerLauncher.Initialize(this.RootVisualElement);
		this.components.Add(this.ServerLauncher);
		this.ToastManager = base.gameObject.GetComponent<UIToastManager>();
		this.ToastManager.Initialize(this.RootVisualElement);
		this.components.Add(this.ToastManager);
		this.OverlayManager = base.gameObject.GetComponent<UIOverlayManager>();
		this.OverlayManager.Initialize(this.RootVisualElement);
		this.components.Add(this.OverlayManager);
		this.PlayerMenu = base.gameObject.GetComponent<UIPlayerMenu>();
		this.PlayerMenu.Initialize(this.RootVisualElement);
		this.components.Add(this.PlayerMenu);
		this.Identity = base.gameObject.GetComponent<UIIdentity>();
		this.Identity.Initialize(this.RootVisualElement);
		this.components.Add(this.Identity);
		this.Appearance = base.gameObject.GetComponent<UIAppearance>();
		this.Appearance.Initialize(this.RootVisualElement);
		this.components.Add(this.Appearance);
		this.PopupManager = base.gameObject.GetComponent<UIPopupManager>();
		this.PopupManager.Initialize(this.RootVisualElement);
		this.components.Add(this.PopupManager);
		this.PlayerUsernames = base.gameObject.GetComponent<UIPlayerUsernames>();
		this.PlayerUsernames.Initialize(this.RootVisualElement);
		this.components.Add(this.PlayerUsernames);
		this.Debug = base.gameObject.GetComponent<UIDebug>();
		this.Debug.Initialize(this.RootVisualElement);
		this.components.Add(this.Debug);
		this.Mods = base.gameObject.GetComponent<UIMods>();
		this.Mods.Initialize(this.RootVisualElement);
		this.components.Add(this.Mods);
	}

	// Token: 0x060005AA RID: 1450 RVA: 0x0000A7DE File Offset: 0x000089DE
	private void Start()
	{
		this.AddComponentCallbacks();
	}

	// Token: 0x060005AB RID: 1451 RVA: 0x0000A7E6 File Offset: 0x000089E6
	public override void OnDestroy()
	{
		this.RemoveComponentCallbacks();
		base.OnDestroy();
	}

	// Token: 0x060005AC RID: 1452 RVA: 0x00023884 File Offset: 0x00021A84
	public void HideAllComponents()
	{
		foreach (UIComponent uicomponent in this.components)
		{
			uicomponent.Hide(false);
		}
	}

	// Token: 0x060005AD RID: 1453 RVA: 0x0000A7F4 File Offset: 0x000089F4
	public void ShowMainMenuComponents()
	{
		this.MainMenu.Show();
		this.Chat.Show();
	}

	// Token: 0x060005AE RID: 1454 RVA: 0x0000A80C File Offset: 0x00008A0C
	public void HideMainMenuComponents()
	{
		this.MainMenu.Hide(false);
		this.Chat.Hide(false);
	}

	// Token: 0x060005AF RID: 1455 RVA: 0x0000A826 File Offset: 0x00008A26
	public void ShowGameComponents()
	{
		this.GameState.Show();
		this.Chat.Show();
		this.Minimap.Show();
	}

	// Token: 0x060005B0 RID: 1456 RVA: 0x0000A849 File Offset: 0x00008A49
	public void HideGameComponents()
	{
		this.GameState.Hide(false);
		this.Chat.Hide(false);
		this.Minimap.Hide(false);
	}

	// Token: 0x060005B1 RID: 1457 RVA: 0x0000A86F File Offset: 0x00008A6F
	private void ShowMouse()
	{
		this.isMouseActive = true;
		UnityEngine.Cursor.lockState = CursorLockMode.None;
		UnityEngine.Cursor.visible = true;
	}

	// Token: 0x060005B2 RID: 1458 RVA: 0x0000A884 File Offset: 0x00008A84
	private void HideMouse()
	{
		this.isMouseActive = false;
		UnityEngine.Cursor.lockState = CursorLockMode.Locked;
		UnityEngine.Cursor.visible = false;
	}

	// Token: 0x060005B3 RID: 1459 RVA: 0x000238D8 File Offset: 0x00021AD8
	private void AddComponentCallbacks()
	{
		foreach (UIComponent uicomponent in this.components)
		{
			uicomponent.OnVisibilityChanged += this.OnMouseRequiredComponentChangedVisibility;
			uicomponent.OnFocusChanged += this.OnMouseRequiredComponentChangedFocus;
		}
	}

	// Token: 0x060005B4 RID: 1460 RVA: 0x000238D8 File Offset: 0x00021AD8
	private void RemoveComponentCallbacks()
	{
		foreach (UIComponent uicomponent in this.components)
		{
			uicomponent.OnVisibilityChanged += this.OnMouseRequiredComponentChangedVisibility;
			uicomponent.OnFocusChanged += this.OnMouseRequiredComponentChangedFocus;
		}
	}

	// Token: 0x060005B5 RID: 1461 RVA: 0x00023948 File Offset: 0x00021B48
	public void SetUiState(UIState state)
	{
		this.UIState = state;
		UIState uistate = this.UIState;
		if (uistate == UIState.MainMenu)
		{
			this.HideAllComponents();
			this.ShowMainMenuComponents();
			return;
		}
		if (uistate != UIState.Play)
		{
			return;
		}
		this.HideAllComponents();
		this.ShowGameComponents();
	}

	// Token: 0x060005B6 RID: 1462 RVA: 0x0000A899 File Offset: 0x00008A99
	public void SetUiScale(float value)
	{
		this.PanelSettings.scale = value;
	}

	// Token: 0x060005B7 RID: 1463 RVA: 0x0000A8A7 File Offset: 0x00008AA7
	private void OnMouseRequiredComponentChangedVisibility(object sender, EventArgs args)
	{
		this.UpdateMouseVisibility();
	}

	// Token: 0x060005B8 RID: 1464 RVA: 0x0000A8A7 File Offset: 0x00008AA7
	private void OnMouseRequiredComponentChangedFocus(object sender, EventArgs args)
	{
		this.UpdateMouseVisibility();
	}

	// Token: 0x060005B9 RID: 1465 RVA: 0x00023984 File Offset: 0x00021B84
	private void UpdateMouseVisibility()
	{
		foreach (UIComponent uicomponent in this.components)
		{
			if ((uicomponent.IsVisible && uicomponent.VisibilityRequiresMouse) || (uicomponent.IsFocused && uicomponent.FocusRequiresMouse))
			{
				this.ShowMouse();
				return;
			}
		}
		this.HideMouse();
	}

	// Token: 0x060005BA RID: 1466 RVA: 0x00023A00 File Offset: 0x00021C00
	public void PlayerSelectSound()
	{
		if (Time.time - this.lastSelectSoundTime < this.selectAudioClip.length)
		{
			return;
		}
		if (this.selectAudioClip != null)
		{
			this.lastSelectSoundTime = Time.time;
			this.AudioSource.PlayOneShot(this.selectAudioClip);
		}
	}

	// Token: 0x060005BB RID: 1467 RVA: 0x00023A54 File Offset: 0x00021C54
	public void PlayerClickSound()
	{
		if (Time.time - this.lastClickSoundTime < this.clickAudioClip.length)
		{
			return;
		}
		if (this.clickAudioClip != null)
		{
			this.lastClickSoundTime = Time.time;
			this.AudioSource.PlayOneShot(this.clickAudioClip);
		}
	}

	// Token: 0x060005BC RID: 1468 RVA: 0x00023AA8 File Offset: 0x00021CA8
	public void PlayerNotificationSound()
	{
		if (Time.time - this.lastNotificationSoundTime < this.notificationAudioClip.length)
		{
			return;
		}
		if (this.notificationAudioClip != null)
		{
			this.lastNotificationSoundTime = Time.time;
			this.AudioSource.PlayOneShot(this.notificationAudioClip);
		}
	}

	// Token: 0x060005C0 RID: 1472 RVA: 0x00023B94 File Offset: 0x00021D94
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x060005C1 RID: 1473 RVA: 0x0000A8C2 File Offset: 0x00008AC2
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x060005C2 RID: 1474 RVA: 0x0000A8CC File Offset: 0x00008ACC
	protected internal override string __getTypeName()
	{
		return "UIManager";
	}

	// Token: 0x04000315 RID: 789
	[Header("References")]
	[SerializeField]
	private AudioClip selectAudioClip;

	// Token: 0x04000316 RID: 790
	[SerializeField]
	private AudioClip clickAudioClip;

	// Token: 0x04000317 RID: 791
	[SerializeField]
	private AudioClip notificationAudioClip;

	// Token: 0x04000318 RID: 792
	public AudioSource AudioSource;

	// Token: 0x04000319 RID: 793
	[HideInInspector]
	public UIMainMenu MainMenu;

	// Token: 0x0400031A RID: 794
	[HideInInspector]
	public UIPauseMenu PauseMenu;

	// Token: 0x0400031B RID: 795
	[HideInInspector]
	public UIServerBrowser ServerBrowser;

	// Token: 0x0400031C RID: 796
	[HideInInspector]
	public UIGameState GameState;

	// Token: 0x0400031D RID: 797
	[HideInInspector]
	public UIChat Chat;

	// Token: 0x0400031E RID: 798
	[HideInInspector]
	public UITeamSelect TeamSelect;

	// Token: 0x0400031F RID: 799
	[HideInInspector]
	public UIPositionSelect PositionSelect;

	// Token: 0x04000320 RID: 800
	[HideInInspector]
	public UIScoreboard Scoreboard;

	// Token: 0x04000321 RID: 801
	[HideInInspector]
	public UISettings Settings;

	// Token: 0x04000322 RID: 802
	[HideInInspector]
	public UIHUD Hud;

	// Token: 0x04000323 RID: 803
	[HideInInspector]
	public UIAnnouncement Announcement;

	// Token: 0x04000324 RID: 804
	[HideInInspector]
	public UIMinimap Minimap;

	// Token: 0x04000325 RID: 805
	[HideInInspector]
	public UIServerLauncher ServerLauncher;

	// Token: 0x04000326 RID: 806
	[HideInInspector]
	public UIToastManager ToastManager;

	// Token: 0x04000327 RID: 807
	[HideInInspector]
	public UIOverlayManager OverlayManager;

	// Token: 0x04000328 RID: 808
	[HideInInspector]
	public UIPlayerMenu PlayerMenu;

	// Token: 0x04000329 RID: 809
	[HideInInspector]
	public UIIdentity Identity;

	// Token: 0x0400032A RID: 810
	[HideInInspector]
	public UIAppearance Appearance;

	// Token: 0x0400032B RID: 811
	[HideInInspector]
	public UIPopupManager PopupManager;

	// Token: 0x0400032C RID: 812
	[HideInInspector]
	public UIPlayerUsernames PlayerUsernames;

	// Token: 0x0400032D RID: 813
	[HideInInspector]
	public UIDebug Debug;

	// Token: 0x0400032E RID: 814
	[HideInInspector]
	public UIMods Mods;

	// Token: 0x0400032F RID: 815
	[HideInInspector]
	public bool isMouseActive;

	// Token: 0x04000330 RID: 816
	[HideInInspector]
	public UIState UIState;

	// Token: 0x04000331 RID: 817
	[HideInInspector]
	public UIDocument UiDocument;

	// Token: 0x04000332 RID: 818
	[HideInInspector]
	public PanelSettings PanelSettings;

	// Token: 0x04000333 RID: 819
	[HideInInspector]
	public VisualElement RootVisualElement;

	// Token: 0x04000334 RID: 820
	private List<UIComponent> components = new List<UIComponent>();

	// Token: 0x04000335 RID: 821
	private float lastSelectSoundTime;

	// Token: 0x04000336 RID: 822
	private float lastClickSoundTime;

	// Token: 0x04000337 RID: 823
	private float lastNotificationSoundTime;
}
