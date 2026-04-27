using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

// Token: 0x0200009E RID: 158
public class ChatManagerController : MonoBehaviour
{
	// Token: 0x06000508 RID: 1288 RVA: 0x0001BB34 File Offset: 0x00019D34
	private void Awake()
	{
		this.chatManager = base.GetComponent<ChatManager>();
		InputManager.QuickChat1Action.performed += this.OnQuickChatAction1Performed;
		InputManager.QuickChat2Action.performed += this.OnQuickChatAction2Performed;
		InputManager.QuickChat3Action.performed += this.OnQuickChatAction3Performed;
		InputManager.QuickChat4Action.performed += this.OnQuickChatAction4Performed;
		InputManager.QuickChat5Action.performed += this.OnQuickChatAction5Performed;
		EventManager.AddEventListener("Event_OnChatSubmitMessage", new Action<Dictionary<string, object>>(this.Event_OnChatSubmitMessage));
		EventManager.AddEventListener("Event_OnClientStopped", new Action<Dictionary<string, object>>(this.Event_OnClientStopped));
		EventManager.AddEventListener("Event_Server_OnChatMessageReceived", new Action<Dictionary<string, object>>(this.Event_Server_OnChatMessageReceived));
		EventManager.AddEventListener("Event_Server_OnVoteStarted", new Action<Dictionary<string, object>>(this.Event_Server_OnVoteStarted));
		EventManager.AddEventListener("Event_Server_OnVoteProgress", new Action<Dictionary<string, object>>(this.Event_Server_OnVoteProgress));
		EventManager.AddEventListener("Event_Server_OnVoteSuccess", new Action<Dictionary<string, object>>(this.Event_Server_OnVoteSuccess));
		EventManager.AddEventListener("Event_Server_OnVoteFailed", new Action<Dictionary<string, object>>(this.Event_Server_OnVoteFailed));
		EventManager.AddEventListener("Event_Server_OnChatCommand", new Action<Dictionary<string, object>>(this.Event_Server_OnChatCommand));
		WebSocketManager.AddMessageListener("playerAnnouncement", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnPlayerAnnouncement));
	}

	// Token: 0x06000509 RID: 1289 RVA: 0x0001BC84 File Offset: 0x00019E84
	private void OnDestroy()
	{
		InputManager.QuickChat1Action.performed -= this.OnQuickChatAction1Performed;
		InputManager.QuickChat2Action.performed -= this.OnQuickChatAction2Performed;
		InputManager.QuickChat3Action.performed -= this.OnQuickChatAction3Performed;
		InputManager.QuickChat4Action.performed -= this.OnQuickChatAction4Performed;
		InputManager.QuickChat5Action.performed -= this.OnQuickChatAction5Performed;
		EventManager.RemoveEventListener("Event_OnChatSubmitMessage", new Action<Dictionary<string, object>>(this.Event_OnChatSubmitMessage));
		EventManager.RemoveEventListener("Event_OnClientStopped", new Action<Dictionary<string, object>>(this.Event_OnClientStopped));
		EventManager.RemoveEventListener("Event_Server_OnChatMessageReceived", new Action<Dictionary<string, object>>(this.Event_Server_OnChatMessageReceived));
		EventManager.RemoveEventListener("Event_Server_OnVoteStarted", new Action<Dictionary<string, object>>(this.Event_Server_OnVoteStarted));
		EventManager.RemoveEventListener("Event_Server_OnVoteProgress", new Action<Dictionary<string, object>>(this.Event_Server_OnVoteProgress));
		EventManager.RemoveEventListener("Event_Server_OnVoteSuccess", new Action<Dictionary<string, object>>(this.Event_Server_OnVoteSuccess));
		EventManager.RemoveEventListener("Event_Server_OnVoteFailed", new Action<Dictionary<string, object>>(this.Event_Server_OnVoteFailed));
		EventManager.RemoveEventListener("Event_Server_OnChatCommand", new Action<Dictionary<string, object>>(this.Event_Server_OnChatCommand));
		WebSocketManager.RemoveMessageListener("playerAnnouncement", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnPlayerAnnouncement));
	}

	// Token: 0x0600050A RID: 1290 RVA: 0x0001BDC8 File Offset: 0x00019FC8
	private void OnQuickChatAction1Performed(InputAction.CallbackContext context)
	{
		if (GlobalStateManager.UIState.Phase != UIPhase.Playing)
		{
			return;
		}
		if (GlobalStateManager.UIState.IsInteracting)
		{
			return;
		}
		this.chatManager.Client_QuickChatAction(0);
	}

	// Token: 0x0600050B RID: 1291 RVA: 0x0001BE00 File Offset: 0x0001A000
	private void OnQuickChatAction2Performed(InputAction.CallbackContext context)
	{
		if (GlobalStateManager.UIState.Phase != UIPhase.Playing)
		{
			return;
		}
		if (GlobalStateManager.UIState.IsInteracting)
		{
			return;
		}
		this.chatManager.Client_QuickChatAction(1);
	}

	// Token: 0x0600050C RID: 1292 RVA: 0x0001BE38 File Offset: 0x0001A038
	private void OnQuickChatAction3Performed(InputAction.CallbackContext context)
	{
		if (GlobalStateManager.UIState.Phase != UIPhase.Playing)
		{
			return;
		}
		if (GlobalStateManager.UIState.IsInteracting)
		{
			return;
		}
		this.chatManager.Client_QuickChatAction(2);
	}

	// Token: 0x0600050D RID: 1293 RVA: 0x0001BE70 File Offset: 0x0001A070
	private void OnQuickChatAction4Performed(InputAction.CallbackContext context)
	{
		if (GlobalStateManager.UIState.Phase != UIPhase.Playing)
		{
			return;
		}
		if (GlobalStateManager.UIState.IsInteracting)
		{
			return;
		}
		this.chatManager.Client_QuickChatAction(3);
	}

	// Token: 0x0600050E RID: 1294 RVA: 0x0001BEA8 File Offset: 0x0001A0A8
	private void OnQuickChatAction5Performed(InputAction.CallbackContext context)
	{
		if (GlobalStateManager.UIState.Phase != UIPhase.Playing)
		{
			return;
		}
		if (GlobalStateManager.UIState.IsInteracting)
		{
			return;
		}
		this.chatManager.Client_QuickChatAction(4);
	}

	// Token: 0x0600050F RID: 1295 RVA: 0x0001BEE0 File Offset: 0x0001A0E0
	private void Event_OnChatSubmitMessage(Dictionary<string, object> message)
	{
		string content = (string)message["content"];
		bool isTeamChat = (bool)message["isTeamChat"];
		this.chatManager.Client_SendChatMessage(content, false, isTeamChat);
	}

	// Token: 0x06000510 RID: 1296 RVA: 0x0001BF1D File Offset: 0x0001A11D
	private void Event_OnClientStopped(Dictionary<string, object> message)
	{
		this.chatManager.ClearChatMessages();
	}

	// Token: 0x06000511 RID: 1297 RVA: 0x0001BF2C File Offset: 0x0001A12C
	private void Event_Server_OnChatMessageReceived(Dictionary<string, object> message)
	{
		ChatMessage chatMessage = (ChatMessage)message["chatMessage"];
		if (!chatMessage.IsTeamChat)
		{
			this.chatManager.Server_BroadcastChatMessage(chatMessage);
			return;
		}
		if (chatMessage.Team == null)
		{
			return;
		}
		List<ulong> list = new List<ulong>();
		if (chatMessage.Team.Value == PlayerTeam.Spectator || chatMessage.Team.Value == PlayerTeam.None)
		{
			list.AddRange(MonoBehaviourSingleton<PlayerManager>.Instance.GetPlayersByTeams(new PlayerTeam[]
			{
				PlayerTeam.None,
				PlayerTeam.Spectator
			}, false).ConvertAll<ulong>((Player player) => player.OwnerClientId));
		}
		else
		{
			list.AddRange(MonoBehaviourSingleton<PlayerManager>.Instance.GetPlayersByTeam(chatMessage.Team.Value, false).ConvertAll<ulong>((Player player) => player.OwnerClientId));
		}
		this.chatManager.Server_SendChatMessageToClients(chatMessage, list.ToArray());
	}

	// Token: 0x06000512 RID: 1298 RVA: 0x0001C024 File Offset: 0x0001A224
	private void Event_Server_OnVoteStarted(Dictionary<string, object> message)
	{
		Vote vote = (Vote)message["vote"];
		Player startedBy = vote.StartedBy;
		if (!startedBy)
		{
			return;
		}
		string arg = StringUtils.WrapInTeamColor(startedBy.Username.Value.ToString(), startedBy.Team);
		switch (vote.Type)
		{
		case VoteType.Start:
			this.chatManager.Server_BroadcastChatMessage(string.Format("{0} started a vote to start a new game [1/{1}]", arg, vote.VotesNeeded));
			return;
		case VoteType.Warmup:
			this.chatManager.Server_BroadcastChatMessage(string.Format("{0} started a vote to enter warmup [1/{1}]", arg, vote.VotesNeeded));
			return;
		case VoteType.Kick:
		{
			FixedString32Bytes steamId = (FixedString32Bytes)vote.Data;
			Player playerBySteamId = MonoBehaviourSingleton<PlayerManager>.Instance.GetPlayerBySteamId(steamId);
			if (!playerBySteamId)
			{
				return;
			}
			string arg2 = StringUtils.WrapInTeamColor(playerBySteamId.Username.Value.ToString(), playerBySteamId.Team);
			this.chatManager.Server_BroadcastChatMessage(string.Format("{0} started a vote to kick {1} [1/{2}]", arg, arg2, vote.VotesNeeded));
			return;
		}
		default:
			return;
		}
	}

	// Token: 0x06000513 RID: 1299 RVA: 0x0001C148 File Offset: 0x0001A348
	private void Event_Server_OnVoteProgress(Dictionary<string, object> message)
	{
		Vote vote = (Vote)message["vote"];
		Player player = (Player)message["voter"];
		if (!player)
		{
			return;
		}
		string text = StringUtils.WrapInTeamColor(player.Username.Value.ToString(), player.Team);
		switch (vote.Type)
		{
		case VoteType.Start:
			this.chatManager.Server_BroadcastChatMessage(string.Format("{0} voted to start a new game [{1}/{2}]", text, vote.Votes, vote.VotesNeeded));
			return;
		case VoteType.Warmup:
			this.chatManager.Server_BroadcastChatMessage(string.Format("{0} voted to enter warmup [{1}/{2}]", text, vote.Votes, vote.VotesNeeded));
			return;
		case VoteType.Kick:
		{
			FixedString32Bytes steamId = (FixedString32Bytes)vote.Data;
			Player playerBySteamId = MonoBehaviourSingleton<PlayerManager>.Instance.GetPlayerBySteamId(steamId);
			if (!playerBySteamId)
			{
				return;
			}
			string text2 = StringUtils.WrapInTeamColor(playerBySteamId.Username.Value.ToString(), playerBySteamId.Team);
			this.chatManager.Server_BroadcastChatMessage(string.Format("{0} voted to kick {1} [{2}/{3}]", new object[]
			{
				text,
				text2,
				vote.Votes,
				vote.VotesNeeded
			}));
			return;
		}
		default:
			return;
		}
	}

	// Token: 0x06000514 RID: 1300 RVA: 0x0001C2A8 File Offset: 0x0001A4A8
	private void Event_Server_OnVoteSuccess(Dictionary<string, object> message)
	{
		Vote vote = (Vote)message["vote"];
		switch (vote.Type)
		{
		case VoteType.Start:
			this.chatManager.Server_BroadcastChatMessage("Vote passed! Starting a new game.");
			return;
		case VoteType.Warmup:
			this.chatManager.Server_BroadcastChatMessage("Vote passed! Entering warmup.");
			return;
		case VoteType.Kick:
		{
			FixedString32Bytes steamId = (FixedString32Bytes)vote.Data;
			Player playerBySteamId = MonoBehaviourSingleton<PlayerManager>.Instance.GetPlayerBySteamId(steamId);
			if (!playerBySteamId)
			{
				return;
			}
			string str = StringUtils.WrapInTeamColor(playerBySteamId.Username.Value.ToString(), playerBySteamId.Team);
			this.chatManager.Server_BroadcastChatMessage("Vote passed! Kicking " + str + ".");
			return;
		}
		default:
			return;
		}
	}

	// Token: 0x06000515 RID: 1301 RVA: 0x0001C366 File Offset: 0x0001A566
	private void Event_Server_OnVoteFailed(Dictionary<string, object> message)
	{
		VoteType type = ((Vote)message["vote"]).Type;
		this.chatManager.Server_BroadcastChatMessage("Vote failed. Timed out.");
	}

	// Token: 0x06000516 RID: 1302 RVA: 0x0001C390 File Offset: 0x0001A590
	private void Event_Server_OnChatCommand(Dictionary<string, object> message)
	{
		ulong num = (ulong)message["clientId"];
		string a = (string)message["command"];
		string[] array = (string[])message["args"];
		Player playerByClientId = MonoBehaviourSingleton<PlayerManager>.Instance.GetPlayerByClientId(num);
		if (!playerByClientId)
		{
			return;
		}
		bool flag = NetworkBehaviourSingleton<ServerManager>.Instance.AdminManager.IsSteamIdAdmin(playerByClientId.SteamId.Value.ToString());
		if (a == "/help")
		{
			string text = "Server commands:\n* <b>/votestart</b>(/vs) - Cast a vote to start a new game\n* <b>/votewarmup</b>(/vw) - Cast a vote to enter warmup\n* <b>/votekick [name/number]</b>(/vk) - Cast a vote to kick a player\n* <b>/help</b> - Displays this message";
			if (flag)
			{
				text += "\n\nAdmin commands:\n* <b>/kick [name/number]</b> - Kick a player\n* <b>/ban [name/number]</b> - Ban a player\n* <b>/bansteamid [Steam ID]</b> - Ban a Steam ID\n* <b>/unbansteamid [Steam ID]</b> - Unban a Steam ID\n* <b>/pause</b> - Pause the game\n* <b>/resume</b> - Resume the game";
			}
			this.chatManager.Server_SendChatMessageToClients(text, new ulong[]
			{
				num
			});
			return;
		}
		if (!(a == "/clear"))
		{
			return;
		}
		this.chatManager.ClearChatMessages();
	}

	// Token: 0x06000517 RID: 1303 RVA: 0x0001C468 File Offset: 0x0001A668
	private void WebSocket_Event_OnPlayerAnnouncement(Dictionary<string, object> message)
	{
		InMessage inMessage = (InMessage)message["inMessage"];
		ChatMessage chatMessage = new ChatMessage
		{
			SteamID = null,
			Username = null,
			Team = null,
			Content = inMessage.GetData<string>(),
			Timestamp = Utils.GetTimestamp(),
			IsQuickChat = false,
			IsTeamChat = false,
			IsSystem = true
		};
		this.chatManager.AddChatMessage(chatMessage);
	}

	// Token: 0x0400031C RID: 796
	private ChatManager chatManager;
}
