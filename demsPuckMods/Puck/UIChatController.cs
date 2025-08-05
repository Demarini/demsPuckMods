using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;

// Token: 0x02000103 RID: 259
public class UIChatController : NetworkBehaviour
{
	// Token: 0x06000902 RID: 2306 RVA: 0x0000C94A File Offset: 0x0000AB4A
	private void Awake()
	{
		this.uiChat = base.GetComponent<UIChat>();
	}

	// Token: 0x06000903 RID: 2307 RVA: 0x000380B4 File Offset: 0x000362B4
	private void Start()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerDespawned", new Action<Dictionary<string, object>>(this.Event_OnPlayerDespawned));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerTeamChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerTeamChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnGoalScored", new Action<Dictionary<string, object>>(this.Event_OnGoalScored));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnGameOver", new Action<Dictionary<string, object>>(this.Event_OnGameOver));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnShowGameUserInterfaceChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnShowGameUserInterfaceChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnChatOpacityChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnChatOpacityChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnChatScaleChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnChatScaleChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Server_OnSynchronizeComplete", new Action<Dictionary<string, object>>(this.Event_Server_OnSynchronizeComplete));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Server_OnPlayerSubscription", new Action<Dictionary<string, object>>(this.Event_Server_OnPlayerSubscription));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Server_OnVoteStarted", new Action<Dictionary<string, object>>(this.Event_Server_OnVoteStarted));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Server_OnVoteProgress", new Action<Dictionary<string, object>>(this.Event_Server_OnVoteProgress));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Server_OnVoteSuccess", new Action<Dictionary<string, object>>(this.Event_Server_OnVoteSuccess));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Server_OnVoteFailed", new Action<Dictionary<string, object>>(this.Event_Server_OnVoteFailed));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Server_OnChatCommand", new Action<Dictionary<string, object>>(this.Event_Server_OnChatCommand));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Server_OnPlayerStateDelayed", new Action<Dictionary<string, object>>(this.Event_Server_OnPlayerStateDelayed));
		this.uiChat.Blur();
		this.uiChat.ClearChatMessages();
	}

	// Token: 0x06000904 RID: 2308 RVA: 0x0000C958 File Offset: 0x0000AB58
	public override void OnNetworkDespawn()
	{
		this.uiChat.ClearChatMessages();
		base.OnNetworkDespawn();
	}

	// Token: 0x06000905 RID: 2309 RVA: 0x0003826C File Offset: 0x0003646C
	public override void OnDestroy()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerDespawned", new Action<Dictionary<string, object>>(this.Event_OnPlayerDespawned));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerTeamChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerTeamChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnGoalScored", new Action<Dictionary<string, object>>(this.Event_OnGoalScored));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnGameOver", new Action<Dictionary<string, object>>(this.Event_OnGameOver));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnShowGameUserInterfaceChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnShowGameUserInterfaceChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnChatOpacityChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnChatOpacityChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnChatScaleChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnChatScaleChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Server_OnSynchronizeComplete", new Action<Dictionary<string, object>>(this.Event_Server_OnSynchronizeComplete));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Server_OnPlayerSubscription", new Action<Dictionary<string, object>>(this.Event_Server_OnPlayerSubscription));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Server_OnVoteStarted", new Action<Dictionary<string, object>>(this.Event_Server_OnVoteStarted));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Server_OnVoteProgress", new Action<Dictionary<string, object>>(this.Event_Server_OnVoteProgress));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Server_OnVoteSuccess", new Action<Dictionary<string, object>>(this.Event_Server_OnVoteSuccess));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Server_OnVoteFailed", new Action<Dictionary<string, object>>(this.Event_Server_OnVoteFailed));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Server_OnChatCommand", new Action<Dictionary<string, object>>(this.Event_Server_OnChatCommand));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Server_OnPlayerStateDelayed", new Action<Dictionary<string, object>>(this.Event_Server_OnPlayerStateDelayed));
		base.OnDestroy();
	}

	// Token: 0x06000906 RID: 2310 RVA: 0x0000C96B File Offset: 0x0000AB6B
	private void Event_Client_OnShowGameUserInterfaceChanged(Dictionary<string, object> message)
	{
		if (NetworkBehaviourSingleton<UIManager>.Instance.UIState == UIState.MainMenu)
		{
			return;
		}
		if ((bool)message["value"])
		{
			this.uiChat.Show();
			return;
		}
		this.uiChat.Hide(false);
	}

	// Token: 0x06000907 RID: 2311 RVA: 0x00038414 File Offset: 0x00036614
	private void Event_Client_OnChatOpacityChanged(Dictionary<string, object> message)
	{
		float opacity = (float)message["value"];
		this.uiChat.SetOpacity(opacity);
	}

	// Token: 0x06000908 RID: 2312 RVA: 0x00038440 File Offset: 0x00036640
	private void Event_Client_OnChatScaleChanged(Dictionary<string, object> message)
	{
		float scale = (float)message["value"];
		this.uiChat.SetScale(scale);
	}

	// Token: 0x06000909 RID: 2313 RVA: 0x0003846C File Offset: 0x0003666C
	private void Event_OnPlayerDespawned(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		if (base.NetworkManager.ShutdownInProgress)
		{
			return;
		}
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (player.IsReplay.Value)
		{
			return;
		}
		this.uiChat.Server_SendSystemChatMessage(string.Format("<b>{0}</b> has left the server.", player.Username.Value));
	}

	// Token: 0x0600090A RID: 2314 RVA: 0x000384D8 File Offset: 0x000366D8
	private void Event_OnPlayerTeamChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		PlayerTeam team = (PlayerTeam)message["newTeam"];
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (player.IsReplay.Value)
		{
			return;
		}
		string text;
		switch (team)
		{
		case PlayerTeam.Spectator:
			text = this.uiChat.WrapInTeamColor(team, "SPECTATOR");
			break;
		case PlayerTeam.Blue:
			text = this.uiChat.WrapInTeamColor(team, "BLUE");
			break;
		case PlayerTeam.Red:
			text = this.uiChat.WrapInTeamColor(team, "RED");
			break;
		default:
			text = null;
			break;
		}
		if (text == null)
		{
			return;
		}
		this.uiChat.Server_SendSystemChatMessage(this.uiChat.WrapPlayerUsername(player) + " joined team " + text + ".");
	}

	// Token: 0x0600090B RID: 2315 RVA: 0x000385A4 File Offset: 0x000367A4
	private void Event_OnGoalScored(Dictionary<string, object> message)
	{
		bool flag = (bool)message["hasGoalPlayer"];
		ulong clientId = (ulong)message["goalPlayerClientId"];
		bool flag2 = (bool)message["hasAssistPlayer"];
		ulong num = (ulong)message["assistPlayerClientId"];
		float num2 = (float)message["speedAcrossLine"];
		float num3 = (float)message["highestSpeedSinceStick"];
		string text = num2.ToString().Replace(",", ".");
		string text2 = num3.ToString().Replace(",", ".");
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (!NetworkBehaviourSingleton<PuckManager>.Instance.GetPuck(false))
		{
			return;
		}
		if (flag)
		{
			Player playerByClientId = NetworkBehaviourSingleton<PlayerManager>.Instance.GetPlayerByClientId(clientId);
			if (playerByClientId)
			{
				this.uiChat.Server_SendSystemChatMessage(string.Concat(new string[]
				{
					this.uiChat.WrapPlayerUsername(playerByClientId),
					" scored a goal, <b><united>",
					text,
					"</united> &units</b> across line, <b><united>",
					text2,
					"</united> &units</b> from stick."
				}));
				return;
			}
		}
		else
		{
			this.uiChat.Server_SendSystemChatMessage(string.Concat(new string[]
			{
				"Goal scored, <b><united>",
				text,
				"</united> &units</b> across line, <b><united>",
				text2,
				"</united> &units</b> from stick."
			}));
		}
	}

	// Token: 0x0600090C RID: 2316 RVA: 0x00038704 File Offset: 0x00036904
	private void Event_OnGameOver(Dictionary<string, object> message)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		PlayerTeam playerTeam = (PlayerTeam)message["winningTeam"];
		int num = (int)message["blueScore"];
		int num2 = (int)message["redScore"];
		string text;
		if (playerTeam != PlayerTeam.Blue)
		{
			if (playerTeam != PlayerTeam.Red)
			{
				text = null;
			}
			else
			{
				text = this.uiChat.WrapInTeamColor(playerTeam, "RED");
			}
		}
		else
		{
			text = this.uiChat.WrapInTeamColor(playerTeam, "BLUE");
		}
		if (text == null)
		{
			return;
		}
		this.uiChat.Server_SendSystemChatMessage(string.Concat(new string[]
		{
			text,
			" wins the game with a score ",
			this.uiChat.WrapInTeamColor(PlayerTeam.Blue, num.ToString()),
			"-",
			this.uiChat.WrapInTeamColor(PlayerTeam.Red, num2.ToString()),
			"."
		}));
	}

	// Token: 0x0600090D RID: 2317 RVA: 0x000387E8 File Offset: 0x000369E8
	private void Event_Server_OnSynchronizeComplete(Dictionary<string, object> message)
	{
		ulong clientId = (ulong)message["clientId"];
		this.uiChat.Server_SendSystemChatMessage("Welcome to Puck! Use the <b>/help</b> command to display available server chat commands.", clientId);
	}

	// Token: 0x0600090E RID: 2318 RVA: 0x00038818 File Offset: 0x00036A18
	private void Event_Server_OnPlayerSubscription(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		if (player.IsReplay.Value)
		{
			return;
		}
		this.uiChat.Server_SendSystemChatMessage(this.uiChat.WrapPlayerUsername(player) + " has joined the server!");
	}

	// Token: 0x0600090F RID: 2319 RVA: 0x00038868 File Offset: 0x00036A68
	private void Event_Server_OnVoteStarted(Dictionary<string, object> message)
	{
		Vote vote = (Vote)message["vote"];
		Player startedBy = vote.StartedBy;
		if (!startedBy)
		{
			return;
		}
		switch (vote.Type)
		{
		case VoteType.Start:
			this.uiChat.Server_SendSystemChatMessage(string.Format("{0} has started a vote to start a new game. (1/{1})", this.uiChat.WrapPlayerUsername(startedBy), vote.VotesNeeded));
			return;
		case VoteType.Warmup:
			this.uiChat.Server_SendSystemChatMessage(string.Format("{0} has started a vote to enter warmup. (1/{1})", this.uiChat.WrapPlayerUsername(startedBy), vote.VotesNeeded));
			return;
		case VoteType.Kick:
		{
			FixedString32Bytes steamId = (FixedString32Bytes)vote.Data;
			Player playerBySteamId = NetworkBehaviourSingleton<PlayerManager>.Instance.GetPlayerBySteamId(steamId);
			if (!playerBySteamId)
			{
				return;
			}
			this.uiChat.Server_SendSystemChatMessage(string.Format("{0} has started a vote to kick #{1} {2}. (1/{3})", new object[]
			{
				this.uiChat.WrapPlayerUsername(startedBy),
				playerBySteamId.Number.Value,
				playerBySteamId.Username.Value,
				vote.VotesNeeded
			}));
			return;
		}
		default:
			return;
		}
	}

	// Token: 0x06000910 RID: 2320 RVA: 0x0003898C File Offset: 0x00036B8C
	private void Event_Server_OnVoteProgress(Dictionary<string, object> message)
	{
		Vote vote = (Vote)message["vote"];
		Player player = (Player)message["voter"];
		if (!player)
		{
			return;
		}
		switch (vote.Type)
		{
		case VoteType.Start:
			this.uiChat.Server_SendSystemChatMessage(string.Format("{0} voted to start a new game. ({1}/{2})", this.uiChat.WrapPlayerUsername(player), vote.Votes, vote.VotesNeeded));
			return;
		case VoteType.Warmup:
			this.uiChat.Server_SendSystemChatMessage(string.Format("{0} voted to enter warmup. ({1}/{2})", this.uiChat.WrapPlayerUsername(player), vote.Votes, vote.VotesNeeded));
			return;
		case VoteType.Kick:
		{
			FixedString32Bytes steamId = (FixedString32Bytes)vote.Data;
			Player playerBySteamId = NetworkBehaviourSingleton<PlayerManager>.Instance.GetPlayerBySteamId(steamId);
			if (!playerBySteamId)
			{
				return;
			}
			this.uiChat.Server_SendSystemChatMessage(string.Format("{0} voted to kick #{1} {2}. ({3}/{4})", new object[]
			{
				this.uiChat.WrapPlayerUsername(player),
				playerBySteamId.Number.Value,
				playerBySteamId.Username.Value,
				vote.Votes,
				vote.VotesNeeded
			}));
			return;
		}
		default:
			return;
		}
	}

	// Token: 0x06000911 RID: 2321 RVA: 0x00038AE0 File Offset: 0x00036CE0
	private void Event_Server_OnVoteSuccess(Dictionary<string, object> message)
	{
		Vote vote = (Vote)message["vote"];
		switch (vote.Type)
		{
		case VoteType.Start:
			this.uiChat.Server_SendSystemChatMessage("Vote passed - starting a new game!");
			return;
		case VoteType.Warmup:
			this.uiChat.Server_SendSystemChatMessage("Vote passed - entering warmup!");
			return;
		case VoteType.Kick:
		{
			FixedString32Bytes steamId = (FixedString32Bytes)vote.Data;
			Player playerBySteamId = NetworkBehaviourSingleton<PlayerManager>.Instance.GetPlayerBySteamId(steamId);
			if (!playerBySteamId)
			{
				return;
			}
			this.uiChat.Server_SendSystemChatMessage(string.Format("Vote passed - kicking #{0} {1}!", playerBySteamId.Number.Value, playerBySteamId.Username.Value));
			return;
		}
		default:
			return;
		}
	}

	// Token: 0x06000912 RID: 2322 RVA: 0x0000C9A4 File Offset: 0x0000ABA4
	private void Event_Server_OnVoteFailed(Dictionary<string, object> message)
	{
		VoteType type = ((Vote)message["vote"]).Type;
		this.uiChat.Server_SendSystemChatMessage("Vote failed - timed out.");
	}

	// Token: 0x06000913 RID: 2323 RVA: 0x00038B90 File Offset: 0x00036D90
	private void Event_Server_OnChatCommand(Dictionary<string, object> message)
	{
		ulong clientId = (ulong)message["clientId"];
		string a = (string)message["command"];
		string[] array = (string[])message["args"];
		Player playerByClientId = NetworkBehaviourSingleton<PlayerManager>.Instance.GetPlayerByClientId(clientId);
		if (!playerByClientId)
		{
			return;
		}
		bool flag = NetworkBehaviourSingleton<ServerManager>.Instance.AdminSteamIds.Contains(playerByClientId.SteamId.Value.ToString());
		if (a == "/help")
		{
			string str = "";
			if (flag)
			{
				str = "\nAdmin commands:\n* <b>/kick [name/number]</b> - Kick a player\n* <b>/ban [name/number]</b> - Ban a player\n* <b>/bansteamid [Steam ID]</b> - Ban a Steam ID\n* <b>/unbansteamid [Steam ID]</b> - Unban a Steam ID\n* <b>/pause</b> - Pause the game\n* <b>/resume</b> - Resume the game";
			}
			this.uiChat.Server_SendSystemChatMessage("Server commands:\n* <b>/help</b> - Displays this message\n* <b>/votestart</b>(/vs) - Cast a vote to start a new game\n* <b>/votewarmup</b>(/vw) - Cast a vote to enter warmup\n* <b>/votekick [name/number]</b>(/vk) - Cast a vote to kick a player" + str, clientId);
		}
	}

	// Token: 0x06000914 RID: 2324 RVA: 0x00038C48 File Offset: 0x00036E48
	private void Event_Server_OnPlayerStateDelayed(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		PlayerState playerState = (PlayerState)message["newState"];
		PlayerState playerState2 = (PlayerState)message["oldState"];
		float num = (float)message["delay"];
		if (!player)
		{
			return;
		}
		if ((playerState2 == PlayerState.PositionSelectBlue || playerState2 == PlayerState.PositionSelectRed) && playerState == PlayerState.Play)
		{
			this.uiChat.Server_SendSystemChatMessage(string.Format("Joining the match in {0} seconds...", num), player.OwnerClientId);
		}
	}

	// Token: 0x06000916 RID: 2326 RVA: 0x0000F5B8 File Offset: 0x0000D7B8
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x06000917 RID: 2327 RVA: 0x00006C33 File Offset: 0x00004E33
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x06000918 RID: 2328 RVA: 0x0000C9CC File Offset: 0x0000ABCC
	protected internal override string __getTypeName()
	{
		return "UIChatController";
	}

	// Token: 0x04000585 RID: 1413
	private UIChat uiChat;
}
