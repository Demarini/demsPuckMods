using System;
using UnityEngine.UIElements;

// Token: 0x02000144 RID: 324
public class UIPauseMenu : UIComponent<UIPauseMenu>
{
	// Token: 0x06000B91 RID: 2961 RVA: 0x0000EB15 File Offset: 0x0000CD15
	private void Start()
	{
		base.VisibilityRequiresMouse = true;
	}

	// Token: 0x06000B92 RID: 2962 RVA: 0x000413C4 File Offset: 0x0003F5C4
	public void Initialize(VisualElement rootVisualElement)
	{
		this.container = rootVisualElement.Query("PauseMenuContainer", null);
		this.switchTeamButton = this.container.Query("SwitchTeamButton", null);
		this.switchTeamButton.clicked += this.OnClickSwitchTeam;
		this.settingsButton = this.container.Query("SettingsButton", null);
		this.settingsButton.clicked += this.OnClickSettings;
		this.disconnectButton = this.container.Query("DisconnectButton", null);
		this.disconnectButton.clicked += this.OnClickDisconnect;
		this.exitGameButton = this.container.Query("ExitGameButton", null);
		this.exitGameButton.clicked += this.OnClickExitGame;
	}

	// Token: 0x06000B93 RID: 2963 RVA: 0x0000EB1E File Offset: 0x0000CD1E
	private void OnClickSwitchTeam()
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnPauseMenuClickSwitchTeam", null);
	}

	// Token: 0x06000B94 RID: 2964 RVA: 0x0000EB30 File Offset: 0x0000CD30
	private void OnClickSettings()
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnPauseMenuClickSettings", null);
	}

	// Token: 0x06000B95 RID: 2965 RVA: 0x0000EB42 File Offset: 0x0000CD42
	private void OnClickDisconnect()
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnPauseMenuClickDisconnect", null);
	}

	// Token: 0x06000B96 RID: 2966 RVA: 0x0000EB54 File Offset: 0x0000CD54
	private void OnClickExitGame()
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnPauseMenuClickExitGame", null);
	}

	// Token: 0x06000B98 RID: 2968 RVA: 0x000414B4 File Offset: 0x0003F6B4
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x06000B99 RID: 2969 RVA: 0x0000EB6E File Offset: 0x0000CD6E
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x06000B9A RID: 2970 RVA: 0x0000EB78 File Offset: 0x0000CD78
	protected internal override string __getTypeName()
	{
		return "UIPauseMenu";
	}

	// Token: 0x040006BB RID: 1723
	private Button switchTeamButton;

	// Token: 0x040006BC RID: 1724
	private Button disconnectButton;

	// Token: 0x040006BD RID: 1725
	private Button settingsButton;

	// Token: 0x040006BE RID: 1726
	private Button exitGameButton;
}
