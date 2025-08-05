using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x020000C9 RID: 201
public class VoteManagerController : MonoBehaviour
{
	// Token: 0x06000615 RID: 1557 RVA: 0x0000AD29 File Offset: 0x00008F29
	private void Awake()
	{
		this.voteManager = base.GetComponent<VoteManager>();
	}

	// Token: 0x06000616 RID: 1558 RVA: 0x0000AD37 File Offset: 0x00008F37
	private void Start()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Server_OnChatCommand", new Action<Dictionary<string, object>>(this.Event_Server_OnChatCommand));
	}

	// Token: 0x06000617 RID: 1559 RVA: 0x0000AD54 File Offset: 0x00008F54
	private void OnDestroy()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Server_OnChatCommand", new Action<Dictionary<string, object>>(this.Event_Server_OnChatCommand));
	}

	// Token: 0x06000618 RID: 1560 RVA: 0x00024D90 File Offset: 0x00022F90
	private void Event_Server_OnChatCommand(Dictionary<string, object> message)
	{
		ulong clientId = (ulong)message["clientId"];
		string a = (string)message["command"];
		string[] array = (string[])message["args"];
		Player playerByClientId = NetworkBehaviourSingleton<PlayerManager>.Instance.GetPlayerByClientId(clientId);
		bool allowVoting = NetworkBehaviourSingleton<ServerManager>.Instance.ServerConfigurationManager.ServerConfiguration.allowVoting;
		if (!(a == "/vs") && !(a == "/votestart"))
		{
			if (!(a == "/vw") && !(a == "/votewarmup"))
			{
				if (!(a == "/vk") && !(a == "/votekick"))
				{
					return;
				}
				if (!allowVoting)
				{
					return;
				}
				if (this.voteManager.Server_IsVoteStarted(VoteType.Kick))
				{
					this.voteManager.Server_SubmitVote(VoteType.Kick, playerByClientId);
					return;
				}
				if (array.Length < 1)
				{
					return;
				}
				int votesNeeded = Mathf.RoundToInt((float)NetworkBehaviourSingleton<PlayerManager>.Instance.GetPlayers(false).Count / 2f + 0.5f);
				Player player = NetworkBehaviourSingleton<PlayerManager>.Instance.GetPlayerByUsername(array[0], false);
				if (!player)
				{
					int number;
					if (int.TryParse(array[0], out number))
					{
						player = NetworkBehaviourSingleton<PlayerManager>.Instance.GetPlayerByNumber(number);
					}
					if (!player)
					{
						return;
					}
				}
				if (NetworkBehaviourSingleton<ServerManager>.Instance.AdminSteamIds.Contains(player.SteamId.Value.ToString()))
				{
					return;
				}
				this.voteManager.Server_CreateVote(VoteType.Kick, votesNeeded, playerByClientId, player.SteamId.Value);
				this.voteManager.Server_SubmitVote(VoteType.Kick, playerByClientId);
				return;
			}
			else
			{
				if (!allowVoting)
				{
					return;
				}
				if (this.voteManager.Server_IsVoteStarted(VoteType.Warmup))
				{
					this.voteManager.Server_SubmitVote(VoteType.Warmup, playerByClientId);
					return;
				}
				int votesNeeded2 = Mathf.RoundToInt((float)NetworkBehaviourSingleton<PlayerManager>.Instance.GetPlayers(false).Count / 2f + 0.5f);
				this.voteManager.Server_CreateVote(VoteType.Warmup, votesNeeded2, playerByClientId, null);
				this.voteManager.Server_SubmitVote(VoteType.Warmup, playerByClientId);
				return;
			}
		}
		else
		{
			if (!allowVoting)
			{
				return;
			}
			if (this.voteManager.Server_IsVoteStarted(VoteType.Start))
			{
				this.voteManager.Server_SubmitVote(VoteType.Start, playerByClientId);
				return;
			}
			int votesNeeded3 = Mathf.RoundToInt((float)NetworkBehaviourSingleton<PlayerManager>.Instance.GetPlayers(false).Count / 2f + 0.5f);
			this.voteManager.Server_CreateVote(VoteType.Start, votesNeeded3, playerByClientId, null);
			this.voteManager.Server_SubmitVote(VoteType.Start, playerByClientId);
			return;
		}
	}

	// Token: 0x0400034C RID: 844
	private VoteManager voteManager;
}
