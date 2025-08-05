using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

// Token: 0x02000110 RID: 272
public class UIMainMenu : UIComponent<UIMainMenu>
{
	// Token: 0x06000988 RID: 2440 RVA: 0x0000D003 File Offset: 0x0000B203
	private void Start()
	{
		base.VisibilityRequiresMouse = true;
	}

	// Token: 0x06000989 RID: 2441 RVA: 0x00039AA8 File Offset: 0x00037CA8
	public void Initialize(VisualElement rootVisualElement)
	{
		this.container = rootVisualElement.Query("MainMenuContainer", null);
		this.debugToolsContainer = this.container.Query("DebugToolsContainer", null);
		this.ipTextField = this.container.Query("IpTextField", null).First().Query("TextField", null);
		this.ipTextField.RegisterValueChangedCallback(new EventCallback<ChangeEvent<string>>(this.OnIpChanged));
		this.ipTextField.value = this.ip;
		this.portTextField = this.container.Query("PortTextField", null).First().Query("TextField", null);
		this.portTextField.RegisterValueChangedCallback(new EventCallback<ChangeEvent<string>>(this.OnPortChanged));
		this.portTextField.value = this.port.ToString();
		this.passwordTextField = this.container.Query("PasswordTextField", null).First().Query("TextField", null);
		this.passwordTextField.RegisterValueChangedCallback(new EventCallback<ChangeEvent<string>>(this.OnPasswordChanged));
		this.passwordTextField.value = this.password;
		this.hostServerButton = this.container.Query("HostServerButton", null);
		this.hostServerButton.clicked += this.OnClickHostServer;
		this.joinServerButton = this.container.Query("JoinServerButton", null);
		this.joinServerButton.clicked += this.OnClickJoinServer;
		this.practiceButton = this.container.Query("PracticeButton", null);
		this.practiceButton.clicked += this.OnClickPractice;
		this.serverBrowserButton = this.container.Query("ServerBrowserButton", null);
		this.serverBrowserButton.clicked += this.OnClickServerBrowser;
		this.playerButton = this.container.Query("PlayerButton", null);
		this.playerButton.clicked += this.OnClickPlayer;
		this.settingsButton = this.container.Query("SettingsButton", null);
		this.settingsButton.clicked += this.OnClickSettings;
		this.modsButton = this.container.Query("ModsButton", null);
		this.modsButton.clicked += this.OnClickMods;
		this.exitGameButton = this.container.Query("ExitGameButton", null);
		this.exitGameButton.clicked += this.OnClickExitGame;
		this.socialContainer = rootVisualElement.Query("SocialContainer", null);
		this.discordButton = this.socialContainer.Query("DiscordButton", null);
		this.discordButton.clicked += this.OnClickDiscord;
		this.patreonButton = this.socialContainer.Query("PatreonButton", null);
		this.patreonButton.clicked += this.OnClickPatreon;
	}

	// Token: 0x0600098A RID: 2442 RVA: 0x0000D00C File Offset: 0x0000B20C
	public override void Show()
	{
		base.Show();
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnMainMenuShow", null);
		if (this.socialContainer == null)
		{
			return;
		}
		this.socialContainer.style.display = DisplayStyle.Flex;
	}

	// Token: 0x0600098B RID: 2443 RVA: 0x0000D043 File Offset: 0x0000B243
	public override void Hide(bool ignoreAlwaysVisible = false)
	{
		base.Hide(ignoreAlwaysVisible);
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnMainMenuHide", null);
		if (this.socialContainer == null)
		{
			return;
		}
		this.socialContainer.style.display = DisplayStyle.None;
	}

	// Token: 0x0600098C RID: 2444 RVA: 0x0000D07B File Offset: 0x0000B27B
	public void ShowDebugTools()
	{
		this.debugToolsContainer.style.display = DisplayStyle.Flex;
	}

	// Token: 0x0600098D RID: 2445 RVA: 0x0000D093 File Offset: 0x0000B293
	public void HideDebugTools()
	{
		this.debugToolsContainer.style.display = DisplayStyle.None;
	}

	// Token: 0x0600098E RID: 2446 RVA: 0x0000D0AB File Offset: 0x0000B2AB
	private void OnIpChanged(ChangeEvent<string> changeEvent)
	{
		this.ip = changeEvent.newValue;
	}

	// Token: 0x0600098F RID: 2447 RVA: 0x00039E0C File Offset: 0x0003800C
	private void OnPortChanged(ChangeEvent<string> changeEvent)
	{
		ushort num;
		if (ushort.TryParse(changeEvent.newValue, out num))
		{
			this.port = num;
		}
	}

	// Token: 0x06000990 RID: 2448 RVA: 0x0000D0B9 File Offset: 0x0000B2B9
	private void OnPasswordChanged(ChangeEvent<string> changeEvent)
	{
		this.password = changeEvent.newValue;
	}

	// Token: 0x06000991 RID: 2449 RVA: 0x0000D0C7 File Offset: 0x0000B2C7
	private void OnClickHostServer()
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnMainMenuClickHostServer", new Dictionary<string, object>
		{
			{
				"port",
				this.port
			},
			{
				"password",
				this.password
			}
		});
	}

	// Token: 0x06000992 RID: 2450 RVA: 0x00039E30 File Offset: 0x00038030
	private void OnClickJoinServer()
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnMainMenuClickJoinServer", new Dictionary<string, object>
		{
			{
				"ip",
				this.ip
			},
			{
				"port",
				this.port
			},
			{
				"password",
				this.password
			}
		});
	}

	// Token: 0x06000993 RID: 2451 RVA: 0x0000D104 File Offset: 0x0000B304
	private void OnClickPractice()
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnMainMenuClickPractice", null);
	}

	// Token: 0x06000994 RID: 2452 RVA: 0x0000D116 File Offset: 0x0000B316
	private void OnClickServerBrowser()
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnMainMenuClickServerBrowser", null);
	}

	// Token: 0x06000995 RID: 2453 RVA: 0x0000D128 File Offset: 0x0000B328
	private void OnClickPlayer()
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnMainMenuClickPlayer", null);
	}

	// Token: 0x06000996 RID: 2454 RVA: 0x0000D13A File Offset: 0x0000B33A
	private void OnClickSettings()
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnMainMenuClickSettings", null);
	}

	// Token: 0x06000997 RID: 2455 RVA: 0x0000D14C File Offset: 0x0000B34C
	private void OnClickMods()
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnMainMenuClickMods", null);
	}

	// Token: 0x06000998 RID: 2456 RVA: 0x0000D15E File Offset: 0x0000B35E
	private void OnClickExitGame()
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnMainMenuClickExitGame", null);
	}

	// Token: 0x06000999 RID: 2457 RVA: 0x0000D170 File Offset: 0x0000B370
	private void OnClickDiscord()
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnSocialClickDiscord", null);
	}

	// Token: 0x0600099A RID: 2458 RVA: 0x0000D182 File Offset: 0x0000B382
	private void OnClickPatreon()
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnSocialClickPatreon", null);
	}

	// Token: 0x0600099C RID: 2460 RVA: 0x00039E8C File Offset: 0x0003808C
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x0600099D RID: 2461 RVA: 0x0000D1BD File Offset: 0x0000B3BD
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x0600099E RID: 2462 RVA: 0x0000D1C7 File Offset: 0x0000B3C7
	protected internal override string __getTypeName()
	{
		return "UIMainMenu";
	}

	// Token: 0x040005B4 RID: 1460
	private VisualElement debugToolsContainer;

	// Token: 0x040005B5 RID: 1461
	private TextField ipTextField;

	// Token: 0x040005B6 RID: 1462
	private TextField portTextField;

	// Token: 0x040005B7 RID: 1463
	private TextField passwordTextField;

	// Token: 0x040005B8 RID: 1464
	private Button hostServerButton;

	// Token: 0x040005B9 RID: 1465
	private Button joinServerButton;

	// Token: 0x040005BA RID: 1466
	private Button practiceButton;

	// Token: 0x040005BB RID: 1467
	private Button serverBrowserButton;

	// Token: 0x040005BC RID: 1468
	private Button playerButton;

	// Token: 0x040005BD RID: 1469
	private Button settingsButton;

	// Token: 0x040005BE RID: 1470
	private Button modsButton;

	// Token: 0x040005BF RID: 1471
	private Button exitGameButton;

	// Token: 0x040005C0 RID: 1472
	private VisualElement socialContainer;

	// Token: 0x040005C1 RID: 1473
	private Button discordButton;

	// Token: 0x040005C2 RID: 1474
	private Button patreonButton;

	// Token: 0x040005C3 RID: 1475
	private string ip = "127.0.0.1";

	// Token: 0x040005C4 RID: 1476
	private ushort port = 7777;

	// Token: 0x040005C5 RID: 1477
	private string password = "";
}
