using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Token: 0x0200012D RID: 301
internal class UIScoreboardController : NetworkBehaviour
{
	// Token: 0x06000A98 RID: 2712 RVA: 0x0000DC2F File Offset: 0x0000BE2F
	private void Awake()
	{
		this.uiScoreboard = base.GetComponent<UIScoreboard>();
	}

	// Token: 0x06000A99 RID: 2713 RVA: 0x0003D490 File Offset: 0x0003B690
	private void Start()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerSpawned", new Action<Dictionary<string, object>>(this.Event_OnPlayerSpawned));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerDespawned", new Action<Dictionary<string, object>>(this.Event_OnPlayerDespawned));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerAdded", new Action<Dictionary<string, object>>(this.Event_OnPlayerAdded));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerRemoved", new Action<Dictionary<string, object>>(this.Event_OnPlayerRemoved));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerTeamChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerTeamChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerUsernameChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerUsernameChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerGoalsChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerGoalsChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerAssistsChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerAssistsChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerPingChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerPingChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerPositionChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerPositionChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerPatreonLevelChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerPatreonLevelChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerAdminLevelChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerAdminLevelChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerSteamIdChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerSteamIdChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnGamePhaseChanged", new Action<Dictionary<string, object>>(this.Event_OnGamePhaseChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnServerConfiguration", new Action<Dictionary<string, object>>(this.Event_Client_OnServerConfiguration));
	}

	// Token: 0x06000A9A RID: 2714 RVA: 0x0000DC3D File Offset: 0x0000BE3D
	public override void OnNetworkDespawn()
	{
		this.uiScoreboard.Clear();
		base.OnNetworkDespawn();
	}

	// Token: 0x06000A9B RID: 2715 RVA: 0x0003D634 File Offset: 0x0003B834
	public override void OnDestroy()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerSpawned", new Action<Dictionary<string, object>>(this.Event_OnPlayerSpawned));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerDespawned", new Action<Dictionary<string, object>>(this.Event_OnPlayerDespawned));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerAdded", new Action<Dictionary<string, object>>(this.Event_OnPlayerAdded));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerRemoved", new Action<Dictionary<string, object>>(this.Event_OnPlayerRemoved));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerTeamChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerTeamChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerUsernameChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerUsernameChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerGoalsChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerGoalsChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerAssistsChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerAssistsChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerPingChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerPingChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerPositionChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerPositionChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerPatreonLevelChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerPatreonLevelChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerAdminLevelChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerAdminLevelChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerSteamIdChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerSteamIdChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnGamePhaseChanged", new Action<Dictionary<string, object>>(this.Event_OnGamePhaseChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnServerConfiguration", new Action<Dictionary<string, object>>(this.Event_Client_OnServerConfiguration));
		base.OnDestroy();
	}

	// Token: 0x06000A9C RID: 2716 RVA: 0x0003D7DC File Offset: 0x0003B9DC
	private void Event_OnPlayerSpawned(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		if (player.IsReplay.Value)
		{
			return;
		}
		this.uiScoreboard.AddPlayer(player);
	}

	// Token: 0x06000A9D RID: 2717 RVA: 0x0003D814 File Offset: 0x0003BA14
	private void Event_OnPlayerDespawned(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		this.uiScoreboard.RemovePlayer(player);
	}

	// Token: 0x06000A9E RID: 2718 RVA: 0x0000DC50 File Offset: 0x0000BE50
	private void Event_OnPlayerAdded(Dictionary<string, object> message)
	{
		this.uiScoreboard.UpdateServer(NetworkBehaviourSingleton<ServerManager>.Instance.Server, NetworkBehaviourSingleton<PlayerManager>.Instance.GetPlayers(false).Count);
	}

	// Token: 0x06000A9F RID: 2719 RVA: 0x0000DC50 File Offset: 0x0000BE50
	private void Event_OnPlayerRemoved(Dictionary<string, object> message)
	{
		this.uiScoreboard.UpdateServer(NetworkBehaviourSingleton<ServerManager>.Instance.Server, NetworkBehaviourSingleton<PlayerManager>.Instance.GetPlayers(false).Count);
	}

	// Token: 0x06000AA0 RID: 2720 RVA: 0x0003D840 File Offset: 0x0003BA40
	private void Event_OnPlayerTeamChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		this.uiScoreboard.UpdatePlayer(player);
	}

	// Token: 0x06000AA1 RID: 2721 RVA: 0x0003D840 File Offset: 0x0003BA40
	private void Event_OnPlayerUsernameChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		this.uiScoreboard.UpdatePlayer(player);
	}

	// Token: 0x06000AA2 RID: 2722 RVA: 0x0003D840 File Offset: 0x0003BA40
	private void Event_OnPlayerGoalsChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		this.uiScoreboard.UpdatePlayer(player);
	}

	// Token: 0x06000AA3 RID: 2723 RVA: 0x0003D840 File Offset: 0x0003BA40
	private void Event_OnPlayerAssistsChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		this.uiScoreboard.UpdatePlayer(player);
	}

	// Token: 0x06000AA4 RID: 2724 RVA: 0x0003D840 File Offset: 0x0003BA40
	private void Event_OnPlayerPingChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		this.uiScoreboard.UpdatePlayer(player);
	}

	// Token: 0x06000AA5 RID: 2725 RVA: 0x0003D840 File Offset: 0x0003BA40
	private void Event_OnPlayerPositionChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		this.uiScoreboard.UpdatePlayer(player);
	}

	// Token: 0x06000AA6 RID: 2726 RVA: 0x0003D840 File Offset: 0x0003BA40
	private void Event_OnPlayerPatreonLevelChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		this.uiScoreboard.UpdatePlayer(player);
	}

	// Token: 0x06000AA7 RID: 2727 RVA: 0x0003D840 File Offset: 0x0003BA40
	private void Event_OnPlayerAdminLevelChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		this.uiScoreboard.UpdatePlayer(player);
	}

	// Token: 0x06000AA8 RID: 2728 RVA: 0x0003D840 File Offset: 0x0003BA40
	private void Event_OnPlayerSteamIdChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		this.uiScoreboard.UpdatePlayer(player);
	}

	// Token: 0x06000AA9 RID: 2729 RVA: 0x0003D86C File Offset: 0x0003BA6C
	private void Event_OnGamePhaseChanged(Dictionary<string, object> message)
	{
		GamePhase gamePhase = (GamePhase)message["newGamePhase"];
		int num = (int)message["period"];
		if (gamePhase == GamePhase.Playing || gamePhase == GamePhase.GameOver)
		{
			string data = "";
			NetworkBehaviourSingleton<PlayerManager>.Instance.GetPlayers(false).ForEach(delegate(Player player)
			{
				data += string.Format("\nPlayer {0} ({1}) [{2}] has {3} goals and {4} assists", new object[]
				{
					player.Username.Value,
					player.OwnerClientId,
					player.SteamId.Value,
					player.Goals.Value,
					player.Assists.Value
				});
			});
			string str = (gamePhase == GamePhase.Playing) ? string.Format("Period {0}", num) : "Game over";
			Debug.Log("[UIScoreboardController] " + str + ": " + data);
		}
	}

	// Token: 0x06000AAA RID: 2730 RVA: 0x0003D908 File Offset: 0x0003BB08
	private void Event_Client_OnServerConfiguration(Dictionary<string, object> message)
	{
		Server server = (Server)message["server"];
		this.uiScoreboard.UpdateServer(server, NetworkBehaviourSingleton<PlayerManager>.Instance.GetPlayers(false).Count);
	}

	// Token: 0x06000AAC RID: 2732 RVA: 0x0000F5B8 File Offset: 0x0000D7B8
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x06000AAD RID: 2733 RVA: 0x00006C33 File Offset: 0x00004E33
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x06000AAE RID: 2734 RVA: 0x0000DC77 File Offset: 0x0000BE77
	protected internal override string __getTypeName()
	{
		return "UIScoreboardController";
	}

	// Token: 0x0400062E RID: 1582
	private UIScoreboard uiScoreboard;
}
