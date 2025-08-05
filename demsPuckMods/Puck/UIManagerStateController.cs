using System;
using System.Collections.Generic;
using Unity.Netcode;

// Token: 0x020000C3 RID: 195
public class UIManagerStateController : NetworkBehaviour
{
	// Token: 0x060005E1 RID: 1505 RVA: 0x0000A960 File Offset: 0x00008B60
	private void Awake()
	{
		this.uiManager = base.GetComponent<UIManager>();
	}

	// Token: 0x060005E2 RID: 1506 RVA: 0x00024534 File Offset: 0x00022734
	private void Start()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerStateChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerStateChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnClientStopped", new Action<Dictionary<string, object>>(this.Event_Client_OnClientStopped));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnMainMenuClickServerBrowser", new Action<Dictionary<string, object>>(this.Event_Client_OnMainMenuClickServerBrowser));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnMainMenuClickPlayer", new Action<Dictionary<string, object>>(this.Event_Client_OnMainMenuClickPlayer));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnMainMenuClickSettings", new Action<Dictionary<string, object>>(this.Event_Client_OnMainMenuClickSettings));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnMainMenuClickMods", new Action<Dictionary<string, object>>(this.Event_Client_OnMainMenuClickMods));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnPlayerMenuClickBack", new Action<Dictionary<string, object>>(this.Event_Client_OnPlayerMenuClickBack));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnPlayerMenuClickIdentity", new Action<Dictionary<string, object>>(this.Event_Client_OnPlayerMenuClickIdentity));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnPlayerMenuClickAppearance", new Action<Dictionary<string, object>>(this.Event_Client_OnPlayerMenuClickAppearance));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnIdentityClickClose", new Action<Dictionary<string, object>>(this.Event_Client_OnIdentityClickClose));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnAppearanceClickClose", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceClickClose));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnPauseMenuClickSettings", new Action<Dictionary<string, object>>(this.Event_Client_OnPauseMenuClickSettings));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnPauseMenuClickSwitchTeam", new Action<Dictionary<string, object>>(this.Event_Client_OnPauseMenuClickSwitchTeam));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnServerBrowserClickClose", new Action<Dictionary<string, object>>(this.Event_Client_OnServerBrowserClickClose));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnServerBrowserClickServer", new Action<Dictionary<string, object>>(this.Event_Client_OnServerBrowserClickServer));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnServerBrowserClickServerLauncher", new Action<Dictionary<string, object>>(this.Event_Client_OnServerBrowserClickServerLauncher));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnTeamSelectClickTeamBlue", new Action<Dictionary<string, object>>(this.Event_Client_OnTeamSelectClickTeamBlue));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnTeamSelectClickTeamRed", new Action<Dictionary<string, object>>(this.Event_Client_OnTeamSelectClickTeamRed));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnTeamSelectClickTeamSpectator", new Action<Dictionary<string, object>>(this.Event_Client_OnTeamSelectClickTeamSpectator));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnSettingsClickClose", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsClickClose));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnServerLauncherClickClose", new Action<Dictionary<string, object>>(this.Event_Client_OnServerLauncherClickClose));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnModsClickClose", new Action<Dictionary<string, object>>(this.Event_Client_OnModsClickClose));
		this.uiManager.SetUiState(UIState.MainMenu);
	}

	// Token: 0x060005E3 RID: 1507 RVA: 0x000247A0 File Offset: 0x000229A0
	public override void OnDestroy()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerStateChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerStateChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnClientStopped", new Action<Dictionary<string, object>>(this.Event_Client_OnClientStopped));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnMainMenuClickServerBrowser", new Action<Dictionary<string, object>>(this.Event_Client_OnMainMenuClickServerBrowser));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnMainMenuClickPlayer", new Action<Dictionary<string, object>>(this.Event_Client_OnMainMenuClickPlayer));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnMainMenuClickSettings", new Action<Dictionary<string, object>>(this.Event_Client_OnMainMenuClickSettings));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnMainMenuClickMods", new Action<Dictionary<string, object>>(this.Event_Client_OnMainMenuClickMods));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnPlayerMenuClickBack", new Action<Dictionary<string, object>>(this.Event_Client_OnPlayerMenuClickBack));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnPlayerMenuClickIdentity", new Action<Dictionary<string, object>>(this.Event_Client_OnPlayerMenuClickIdentity));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnPlayerMenuClickAppearance", new Action<Dictionary<string, object>>(this.Event_Client_OnPlayerMenuClickAppearance));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnIdentityClickClose", new Action<Dictionary<string, object>>(this.Event_Client_OnIdentityClickClose));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnAppearanceClickClose", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceClickClose));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnPauseMenuClickSettings", new Action<Dictionary<string, object>>(this.Event_Client_OnPauseMenuClickSettings));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnPauseMenuClickSwitchTeam", new Action<Dictionary<string, object>>(this.Event_Client_OnPauseMenuClickSwitchTeam));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnServerBrowserClickClose", new Action<Dictionary<string, object>>(this.Event_Client_OnServerBrowserClickClose));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnServerBrowserClickServer", new Action<Dictionary<string, object>>(this.Event_Client_OnServerBrowserClickServer));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnServerBrowserClickServerLauncher", new Action<Dictionary<string, object>>(this.Event_Client_OnServerBrowserClickServerLauncher));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnTeamSelectClickTeamBlue", new Action<Dictionary<string, object>>(this.Event_Client_OnTeamSelectClickTeamBlue));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnTeamSelectClickTeamRed", new Action<Dictionary<string, object>>(this.Event_Client_OnTeamSelectClickTeamRed));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnTeamSelectClickTeamSpectator", new Action<Dictionary<string, object>>(this.Event_Client_OnTeamSelectClickTeamSpectator));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnSettingsClickClose", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsClickClose));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnServerLauncherClickClose", new Action<Dictionary<string, object>>(this.Event_Client_OnServerLauncherClickClose));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnModsClickClose", new Action<Dictionary<string, object>>(this.Event_Client_OnModsClickClose));
		base.OnDestroy();
	}

	// Token: 0x060005E4 RID: 1508 RVA: 0x00024A08 File Offset: 0x00022C08
	private void Event_OnPlayerStateChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		PlayerState playerState = (PlayerState)message["oldState"];
		if (!player.IsLocalPlayer)
		{
			return;
		}
		if (playerState == PlayerState.None)
		{
			this.uiManager.SetUiState(UIState.Play);
		}
		PlayerState value = player.State.Value;
		if (value == PlayerState.TeamSelect)
		{
			this.uiManager.PositionSelect.Hide(false);
			this.uiManager.TeamSelect.Show();
			return;
		}
		if (value - PlayerState.PositionSelectBlue > 1)
		{
			this.uiManager.TeamSelect.Hide(false);
			this.uiManager.PositionSelect.Hide(false);
			return;
		}
		this.uiManager.TeamSelect.Hide(false);
		this.uiManager.PositionSelect.Show();
	}

	// Token: 0x060005E5 RID: 1509 RVA: 0x0000A96E File Offset: 0x00008B6E
	private void Event_Client_OnClientStopped(Dictionary<string, object> message)
	{
		this.uiManager.SetUiState(UIState.MainMenu);
	}

	// Token: 0x060005E6 RID: 1510 RVA: 0x0000A97C File Offset: 0x00008B7C
	private void Event_Client_OnMainMenuClickServerBrowser(Dictionary<string, object> message)
	{
		this.uiManager.MainMenu.Hide(false);
		this.uiManager.ServerBrowser.Show();
	}

	// Token: 0x060005E7 RID: 1511 RVA: 0x0000A99F File Offset: 0x00008B9F
	private void Event_Client_OnMainMenuClickPlayer(Dictionary<string, object> message)
	{
		this.uiManager.MainMenu.Hide(false);
		this.uiManager.PlayerMenu.Show();
	}

	// Token: 0x060005E8 RID: 1512 RVA: 0x0000A9C2 File Offset: 0x00008BC2
	private void Event_Client_OnMainMenuClickSettings(Dictionary<string, object> message)
	{
		this.uiManager.MainMenu.Hide(false);
		this.uiManager.Settings.Show();
	}

	// Token: 0x060005E9 RID: 1513 RVA: 0x0000A9E5 File Offset: 0x00008BE5
	private void Event_Client_OnMainMenuClickMods(Dictionary<string, object> message)
	{
		this.uiManager.MainMenu.Hide(false);
		this.uiManager.Mods.Show();
	}

	// Token: 0x060005EA RID: 1514 RVA: 0x0000AA08 File Offset: 0x00008C08
	private void Event_Client_OnPlayerMenuClickBack(Dictionary<string, object> message)
	{
		this.uiManager.MainMenu.Show();
		this.uiManager.PlayerMenu.Hide(false);
	}

	// Token: 0x060005EB RID: 1515 RVA: 0x0000AA2B File Offset: 0x00008C2B
	private void Event_Client_OnPlayerMenuClickIdentity(Dictionary<string, object> message)
	{
		this.uiManager.PlayerMenu.Hide(false);
		this.uiManager.Identity.Show();
	}

	// Token: 0x060005EC RID: 1516 RVA: 0x0000AA4E File Offset: 0x00008C4E
	private void Event_Client_OnPlayerMenuClickAppearance(Dictionary<string, object> message)
	{
		this.uiManager.PlayerMenu.Hide(false);
		this.uiManager.Appearance.Show();
	}

	// Token: 0x060005ED RID: 1517 RVA: 0x0000AA71 File Offset: 0x00008C71
	private void Event_Client_OnIdentityClickClose(Dictionary<string, object> message)
	{
		this.uiManager.PlayerMenu.Show();
		this.uiManager.Identity.Hide(false);
	}

	// Token: 0x060005EE RID: 1518 RVA: 0x0000AA94 File Offset: 0x00008C94
	private void Event_Client_OnAppearanceClickClose(Dictionary<string, object> message)
	{
		this.uiManager.PlayerMenu.Show();
		this.uiManager.Appearance.Hide(false);
	}

	// Token: 0x060005EF RID: 1519 RVA: 0x0000AAB7 File Offset: 0x00008CB7
	private void Event_Client_OnPauseMenuClickSettings(Dictionary<string, object> message)
	{
		this.uiManager.Settings.Show();
		this.uiManager.PauseMenu.Hide(false);
	}

	// Token: 0x060005F0 RID: 1520 RVA: 0x0000AADA File Offset: 0x00008CDA
	private void Event_Client_OnPauseMenuClickSwitchTeam(Dictionary<string, object> message)
	{
		this.uiManager.PauseMenu.Hide(false);
	}

	// Token: 0x060005F1 RID: 1521 RVA: 0x0000AAED File Offset: 0x00008CED
	private void Event_Client_OnServerBrowserClickClose(Dictionary<string, object> message)
	{
		this.uiManager.ServerBrowser.Hide(false);
		this.uiManager.MainMenu.Show();
	}

	// Token: 0x060005F2 RID: 1522 RVA: 0x0000AB10 File Offset: 0x00008D10
	private void Event_Client_OnServerBrowserClickServer(Dictionary<string, object> message)
	{
		this.uiManager.ServerBrowser.Hide(false);
	}

	// Token: 0x060005F3 RID: 1523 RVA: 0x0000AB23 File Offset: 0x00008D23
	private void Event_Client_OnServerBrowserClickServerLauncher(Dictionary<string, object> message)
	{
		this.uiManager.ServerBrowser.Hide(false);
		this.uiManager.ServerLauncher.Show();
	}

	// Token: 0x060005F4 RID: 1524 RVA: 0x0000AB46 File Offset: 0x00008D46
	private void Event_Client_OnTeamSelectClickTeamBlue(Dictionary<string, object> message)
	{
		this.uiManager.TeamSelect.Hide(false);
	}

	// Token: 0x060005F5 RID: 1525 RVA: 0x0000AB46 File Offset: 0x00008D46
	private void Event_Client_OnTeamSelectClickTeamRed(Dictionary<string, object> message)
	{
		this.uiManager.TeamSelect.Hide(false);
	}

	// Token: 0x060005F6 RID: 1526 RVA: 0x0000AB46 File Offset: 0x00008D46
	private void Event_Client_OnTeamSelectClickTeamSpectator(Dictionary<string, object> message)
	{
		this.uiManager.TeamSelect.Hide(false);
	}

	// Token: 0x060005F7 RID: 1527 RVA: 0x00024AD0 File Offset: 0x00022CD0
	private void Event_Client_OnSettingsClickClose(Dictionary<string, object> message)
	{
		this.uiManager.Settings.Hide(false);
		if (this.uiManager.UIState == UIState.MainMenu)
		{
			this.uiManager.MainMenu.Show();
			return;
		}
		this.uiManager.PauseMenu.Show();
	}

	// Token: 0x060005F8 RID: 1528 RVA: 0x0000AB59 File Offset: 0x00008D59
	private void Event_Client_OnServerLauncherClickClose(Dictionary<string, object> message)
	{
		this.uiManager.ServerLauncher.Hide(false);
		this.uiManager.ServerBrowser.Show();
	}

	// Token: 0x060005F9 RID: 1529 RVA: 0x0000AB7C File Offset: 0x00008D7C
	private void Event_Client_OnModsClickClose(Dictionary<string, object> message)
	{
		this.uiManager.Mods.Hide(false);
		this.uiManager.MainMenu.Show();
	}

	// Token: 0x060005FB RID: 1531 RVA: 0x0000F5B8 File Offset: 0x0000D7B8
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x060005FC RID: 1532 RVA: 0x00006C33 File Offset: 0x00004E33
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x060005FD RID: 1533 RVA: 0x0000AB9F File Offset: 0x00008D9F
	protected internal override string __getTypeName()
	{
		return "UIManagerStateController";
	}

	// Token: 0x0400033A RID: 826
	private UIManager uiManager;
}
