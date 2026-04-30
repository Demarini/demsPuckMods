using System;
using System.Collections.Generic;

// Token: 0x020001C0 RID: 448
internal class UIScoreboardController : UIViewController<UIScoreboard>
{
	// Token: 0x06000CE1 RID: 3297 RVA: 0x0003CB90 File Offset: 0x0003AD90
	public override void Awake()
	{
		base.Awake();
		this.uiScoreboard = base.GetComponent<UIScoreboard>();
		EventManager.AddEventListener("Event_Everyone_OnPlayerAdded", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerAdded));
		EventManager.AddEventListener("Event_Everyone_OnPlayerRemoved", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerRemoved));
		EventManager.AddEventListener("Event_Everyone_OnPlayerGameStateChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerGameStateChanged));
		EventManager.AddEventListener("Event_Everyone_OnPlayerUsernameChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerUsernameChanged));
		EventManager.AddEventListener("Event_Everyone_OnPlayerGoalsChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerGoalsChanged));
		EventManager.AddEventListener("Event_Everyone_OnPlayerAssistsChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerAssistsChanged));
		EventManager.AddEventListener("Event_Everyone_OnPlayerPingChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerPingChanged));
		EventManager.AddEventListener("Event_Everyone_OnPlayerPositionChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerPositionChanged));
		EventManager.AddEventListener("Event_Everyone_OnPlayerPatreonLevelChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerPatreonLevelChanged));
		EventManager.AddEventListener("Event_Everyone_OnPlayerAdminLevelChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerAdminLevelChanged));
		EventManager.AddEventListener("Event_Everyone_OnPlayerSteamIdChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerSteamIdChanged));
		EventManager.AddEventListener("Event_Everyone_OnServerChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnServerChanged));
	}

	// Token: 0x06000CE2 RID: 3298 RVA: 0x0003CCB8 File Offset: 0x0003AEB8
	public override void OnDestroy()
	{
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerAdded", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerAdded));
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerRemoved", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerRemoved));
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerGameStateChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerGameStateChanged));
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerUsernameChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerUsernameChanged));
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerGoalsChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerGoalsChanged));
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerAssistsChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerAssistsChanged));
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerPingChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerPingChanged));
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerPositionChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerPositionChanged));
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerPatreonLevelChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerPatreonLevelChanged));
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerAdminLevelChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerAdminLevelChanged));
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerSteamIdChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerSteamIdChanged));
		EventManager.RemoveEventListener("Event_Everyone_OnServerChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnServerChanged));
		base.OnDestroy();
	}

	// Token: 0x06000CE3 RID: 3299 RVA: 0x0003CDD4 File Offset: 0x0003AFD4
	private void Event_Everyone_OnPlayerAdded(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		if (player.IsReplay.Value)
		{
			return;
		}
		this.uiScoreboard.AddPlayer(player);
		this.uiScoreboard.StyleServer(NetworkBehaviourSingleton<ServerManager>.Instance.Server.Value, MonoBehaviourSingleton<PlayerManager>.Instance.GetPlayers(false).Count);
	}

	// Token: 0x06000CE4 RID: 3300 RVA: 0x0003CE38 File Offset: 0x0003B038
	private void Event_Everyone_OnPlayerRemoved(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		if (player.IsReplay.Value)
		{
			return;
		}
		this.uiScoreboard.RemovePlayer(player);
		this.uiScoreboard.StyleServer(NetworkBehaviourSingleton<ServerManager>.Instance.Server.Value, MonoBehaviourSingleton<PlayerManager>.Instance.GetPlayers(false).Count);
	}

	// Token: 0x06000CE5 RID: 3301 RVA: 0x0003CE9C File Offset: 0x0003B09C
	private void Event_Everyone_OnPlayerGameStateChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		this.uiScoreboard.StylePlayer(player);
	}

	// Token: 0x06000CE6 RID: 3302 RVA: 0x0003CEC8 File Offset: 0x0003B0C8
	private void Event_Everyone_OnPlayerUsernameChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		this.uiScoreboard.StylePlayer(player);
	}

	// Token: 0x06000CE7 RID: 3303 RVA: 0x0003CEF4 File Offset: 0x0003B0F4
	private void Event_Everyone_OnPlayerGoalsChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		this.uiScoreboard.StylePlayer(player);
	}

	// Token: 0x06000CE8 RID: 3304 RVA: 0x0003CF20 File Offset: 0x0003B120
	private void Event_Everyone_OnPlayerAssistsChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		this.uiScoreboard.StylePlayer(player);
	}

	// Token: 0x06000CE9 RID: 3305 RVA: 0x0003CF4C File Offset: 0x0003B14C
	private void Event_Everyone_OnPlayerPingChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		this.uiScoreboard.StylePlayer(player);
	}

	// Token: 0x06000CEA RID: 3306 RVA: 0x0003CF78 File Offset: 0x0003B178
	private void Event_Everyone_OnPlayerPositionChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		this.uiScoreboard.StylePlayer(player);
	}

	// Token: 0x06000CEB RID: 3307 RVA: 0x0003CFA4 File Offset: 0x0003B1A4
	private void Event_Everyone_OnPlayerPatreonLevelChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		this.uiScoreboard.StylePlayer(player);
	}

	// Token: 0x06000CEC RID: 3308 RVA: 0x0003CFD0 File Offset: 0x0003B1D0
	private void Event_Everyone_OnPlayerAdminLevelChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		this.uiScoreboard.StylePlayer(player);
	}

	// Token: 0x06000CED RID: 3309 RVA: 0x0003CFFC File Offset: 0x0003B1FC
	private void Event_Everyone_OnPlayerSteamIdChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		this.uiScoreboard.StylePlayer(player);
	}

	// Token: 0x06000CEE RID: 3310 RVA: 0x0003D026 File Offset: 0x0003B226
	private void Event_Everyone_OnServerChanged(Dictionary<string, object> message)
	{
		this.uiScoreboard.StyleServer(NetworkBehaviourSingleton<ServerManager>.Instance.Server.Value, MonoBehaviourSingleton<PlayerManager>.Instance.GetPlayers(false).Count);
	}

	// Token: 0x0400079B RID: 1947
	private UIScoreboard uiScoreboard;
}
