using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

// Token: 0x0200019B RID: 411
public class UIMainMenu : UIView
{
	// Token: 0x06000BAA RID: 2986 RVA: 0x000370D4 File Offset: 0x000352D4
	public void Initialize(VisualElement rootVisualElement)
	{
		base.View = rootVisualElement.Query("MainMenuView", null);
		this.mainMenu = base.View.Query("MainMenu", null);
		this.debug = base.View.Query("Debug", null);
		this.social = base.View.Query("Social", null);
		this.playButton = this.mainMenu.Query("PlayButton", null);
		this.playButton.clicked += this.OnClickPlay;
		this.playerButton = this.mainMenu.Query("PlayerButton", null);
		this.playerButton.clicked += this.OnClickPlayer;
		this.settingsButton = this.mainMenu.Query("SettingsButton", null);
		this.settingsButton.clicked += this.OnClickSettings;
		this.modsButton = this.mainMenu.Query("ModsButton", null);
		this.modsButton.clicked += this.OnClickMods;
		this.exitGameButton = this.mainMenu.Query("ExitGameButton", null);
		this.exitGameButton.clicked += this.OnClickExitGame;
		this.discordButton = this.social.Query("DiscordButton", null);
		this.discordButton.clicked += this.OnClickDiscord;
		this.patreonButton = this.social.Query("PatreonButton", null);
		this.patreonButton.clicked += this.OnClickPatreon;
		this.ipAddressTextField = this.debug.Query("IpAddressTextField", null).First().Query(null, null);
		this.ipAddressTextField.RegisterValueChangedCallback(new EventCallback<ChangeEvent<string>>(this.OnIpAddressChanged));
		this.ipAddressTextField.value = this.ipAddress;
		this.portIntegerField = this.debug.Query("PortIntegerField", null).First().Query(null, null);
		this.portIntegerField.RegisterValueChangedCallback(new EventCallback<ChangeEvent<int>>(this.OnPortChanged));
		this.portIntegerField.value = (int)this.port;
		this.passwordTextField = this.debug.Query("PasswordTextField", null).First().Query(null, null);
		this.passwordTextField.RegisterValueChangedCallback(new EventCallback<ChangeEvent<string>>(this.OnPasswordChanged));
		this.passwordTextField.value = this.password;
		this.joinServerButton = this.debug.Query("JoinServerButton", null);
		this.joinServerButton.clicked += this.OnClickJoinServer;
		this.hostServerButton = this.debug.Query("HostServerButton", null);
		this.hostServerButton.clicked += this.OnClickHostServer;
	}

	// Token: 0x06000BAB RID: 2987 RVA: 0x00037413 File Offset: 0x00035613
	public override bool Show()
	{
		bool flag = base.Show();
		if (flag)
		{
			EventManager.TriggerEvent("Event_OnMainMenuShow", null);
		}
		return flag;
	}

	// Token: 0x06000BAC RID: 2988 RVA: 0x00037429 File Offset: 0x00035629
	public override bool Hide()
	{
		bool flag = base.Hide();
		if (flag)
		{
			EventManager.TriggerEvent("Event_OnMainMenuHide", null);
		}
		return flag;
	}

	// Token: 0x06000BAD RID: 2989 RVA: 0x0003743F File Offset: 0x0003563F
	public void ShowDebug()
	{
		this.debug.style.display = DisplayStyle.Flex;
	}

	// Token: 0x06000BAE RID: 2990 RVA: 0x00037457 File Offset: 0x00035657
	public void HideDebug()
	{
		this.debug.style.display = DisplayStyle.None;
	}

	// Token: 0x06000BAF RID: 2991 RVA: 0x0003746F File Offset: 0x0003566F
	private void OnIpAddressChanged(ChangeEvent<string> changeEvent)
	{
		this.ipAddress = changeEvent.newValue;
	}

	// Token: 0x06000BB0 RID: 2992 RVA: 0x0003747D File Offset: 0x0003567D
	private void OnPortChanged(ChangeEvent<int> changeEvent)
	{
		this.port = (ushort)changeEvent.newValue;
	}

	// Token: 0x06000BB1 RID: 2993 RVA: 0x0003748C File Offset: 0x0003568C
	private void OnPasswordChanged(ChangeEvent<string> changeEvent)
	{
		this.password = changeEvent.newValue;
	}

	// Token: 0x06000BB2 RID: 2994 RVA: 0x0003749C File Offset: 0x0003569C
	private void OnClickJoinServer()
	{
		EventManager.TriggerEvent("Event_OnMainMenuClickJoinServer", new Dictionary<string, object>
		{
			{
				"ipAddress",
				this.ipAddress
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

	// Token: 0x06000BB3 RID: 2995 RVA: 0x000374F0 File Offset: 0x000356F0
	private void OnClickHostServer()
	{
		EventManager.TriggerEvent("Event_OnMainMenuClickHostServer", new Dictionary<string, object>
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

	// Token: 0x06000BB4 RID: 2996 RVA: 0x00037528 File Offset: 0x00035728
	private void OnClickPlay()
	{
		EventManager.TriggerEvent("Event_OnMainMenuClickPlay", null);
	}

	// Token: 0x06000BB5 RID: 2997 RVA: 0x00037535 File Offset: 0x00035735
	private void OnClickPlayer()
	{
		EventManager.TriggerEvent("Event_OnMainMenuClickPlayer", null);
	}

	// Token: 0x06000BB6 RID: 2998 RVA: 0x00037542 File Offset: 0x00035742
	private void OnClickSettings()
	{
		EventManager.TriggerEvent("Event_OnMainMenuClickSettings", null);
	}

	// Token: 0x06000BB7 RID: 2999 RVA: 0x0003754F File Offset: 0x0003574F
	private void OnClickMods()
	{
		EventManager.TriggerEvent("Event_OnMainMenuClickMods", null);
	}

	// Token: 0x06000BB8 RID: 3000 RVA: 0x0003755C File Offset: 0x0003575C
	private void OnClickExitGame()
	{
		EventManager.TriggerEvent("Event_OnMainMenuClickExitGame", null);
	}

	// Token: 0x06000BB9 RID: 3001 RVA: 0x00037569 File Offset: 0x00035769
	private void OnClickDiscord()
	{
		EventManager.TriggerEvent("Event_OnSocialClickDiscord", null);
	}

	// Token: 0x06000BBA RID: 3002 RVA: 0x00037576 File Offset: 0x00035776
	private void OnClickPatreon()
	{
		EventManager.TriggerEvent("Event_OnSocialClickPatreon", null);
	}

	// Token: 0x040006E4 RID: 1764
	private VisualElement mainMenu;

	// Token: 0x040006E5 RID: 1765
	private VisualElement debug;

	// Token: 0x040006E6 RID: 1766
	private TextField ipAddressTextField;

	// Token: 0x040006E7 RID: 1767
	private IntegerField portIntegerField;

	// Token: 0x040006E8 RID: 1768
	private TextField passwordTextField;

	// Token: 0x040006E9 RID: 1769
	private Button joinServerButton;

	// Token: 0x040006EA RID: 1770
	private Button hostServerButton;

	// Token: 0x040006EB RID: 1771
	private Button playButton;

	// Token: 0x040006EC RID: 1772
	private Button playerButton;

	// Token: 0x040006ED RID: 1773
	private Button settingsButton;

	// Token: 0x040006EE RID: 1774
	private Button modsButton;

	// Token: 0x040006EF RID: 1775
	private Button exitGameButton;

	// Token: 0x040006F0 RID: 1776
	private VisualElement social;

	// Token: 0x040006F1 RID: 1777
	private Button discordButton;

	// Token: 0x040006F2 RID: 1778
	private Button patreonButton;

	// Token: 0x040006F3 RID: 1779
	private string ipAddress = "127.0.0.1";

	// Token: 0x040006F4 RID: 1780
	private ushort port = 30609;

	// Token: 0x040006F5 RID: 1781
	private string password;
}
