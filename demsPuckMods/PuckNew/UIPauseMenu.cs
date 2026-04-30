using System;
using UnityEngine.UIElements;

// Token: 0x020001AC RID: 428
public class UIPauseMenu : UIView
{
	// Token: 0x06000C4E RID: 3150 RVA: 0x0003A3B4 File Offset: 0x000385B4
	public void Initialize(VisualElement rootVisualElement)
	{
		base.View = rootVisualElement.Query("PauseMenuView", null);
		this.pauseMenu = base.View.Query("PauseMenu", null);
		this.selectTeamButton = this.pauseMenu.Query("SelectTeamButton", null);
		this.selectTeamButton.clicked += this.OnClickSelectTeam;
		this.selectPositionButton = this.pauseMenu.Query("SelectPositionButton", null);
		this.selectPositionButton.clicked += this.OnClickSelectPosition;
		this.serverBrowserButton = this.pauseMenu.Query("ServerBrowserButton", null);
		this.serverBrowserButton.clicked += this.OnClickServerBrowser;
		this.settingsButton = this.pauseMenu.Query("SettingsButton", null);
		this.settingsButton.clicked += this.OnClickSettings;
		this.disconnectButton = this.pauseMenu.Query("DisconnectButton", null);
		this.disconnectButton.clicked += this.OnClickDisconnect;
		this.exitGameButton = this.pauseMenu.Query("ExitGameButton", null);
		this.exitGameButton.clicked += this.OnClickExitGame;
	}

	// Token: 0x06000C4F RID: 3151 RVA: 0x0003A526 File Offset: 0x00038726
	private void OnClickSelectTeam()
	{
		EventManager.TriggerEvent("Event_OnPauseMenuClickSelectTeam", null);
	}

	// Token: 0x06000C50 RID: 3152 RVA: 0x0003A533 File Offset: 0x00038733
	private void OnClickSelectPosition()
	{
		EventManager.TriggerEvent("Event_OnPauseMenuClickSelectPosition", null);
	}

	// Token: 0x06000C51 RID: 3153 RVA: 0x0003A540 File Offset: 0x00038740
	private void OnClickServerBrowser()
	{
		EventManager.TriggerEvent("Event_OnPauseMenuClickServerBrowser", null);
	}

	// Token: 0x06000C52 RID: 3154 RVA: 0x0003A54D File Offset: 0x0003874D
	private void OnClickSettings()
	{
		EventManager.TriggerEvent("Event_OnPauseMenuClickSettings", null);
	}

	// Token: 0x06000C53 RID: 3155 RVA: 0x0003A55A File Offset: 0x0003875A
	private void OnClickDisconnect()
	{
		EventManager.TriggerEvent("Event_OnPauseMenuClickDisconnect", null);
	}

	// Token: 0x06000C54 RID: 3156 RVA: 0x0003A567 File Offset: 0x00038767
	private void OnClickExitGame()
	{
		EventManager.TriggerEvent("Event_OnPauseMenuClickExitGame", null);
	}

	// Token: 0x0400074D RID: 1869
	private VisualElement pauseMenu;

	// Token: 0x0400074E RID: 1870
	private Button selectTeamButton;

	// Token: 0x0400074F RID: 1871
	private Button selectPositionButton;

	// Token: 0x04000750 RID: 1872
	private Button serverBrowserButton;

	// Token: 0x04000751 RID: 1873
	private Button disconnectButton;

	// Token: 0x04000752 RID: 1874
	private Button settingsButton;

	// Token: 0x04000753 RID: 1875
	private Button exitGameButton;
}
