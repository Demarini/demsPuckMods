using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000149 RID: 329
public class VoteManagerController : MonoBehaviour
{
	// Token: 0x060009D9 RID: 2521 RVA: 0x0002FA58 File Offset: 0x0002DC58
	private void Awake()
	{
		this.voteManager = base.GetComponent<VoteManager>();
		EventManager.AddEventListener("Event_Server_OnChatCommand", new Action<Dictionary<string, object>>(this.Event_Server_OnChatCommand));
	}

	// Token: 0x060009DA RID: 2522 RVA: 0x000020D3 File Offset: 0x000002D3
	private void Start()
	{
	}

	// Token: 0x060009DB RID: 2523 RVA: 0x0002FA7C File Offset: 0x0002DC7C
	private void OnDestroy()
	{
		EventManager.RemoveEventListener("Event_Server_OnChatCommand", new Action<Dictionary<string, object>>(this.Event_Server_OnChatCommand));
	}

	// Token: 0x060009DC RID: 2524 RVA: 0x0002FA94 File Offset: 0x0002DC94
	private void Event_Server_OnChatCommand(Dictionary<string, object> message)
	{
		ulong clientId = (ulong)message["clientId"];
		string a = (string)message["command"];
		string[] array = (string[])message["args"];
		Player playerByClientId = MonoBehaviourSingleton<PlayerManager>.Instance.GetPlayerByClientId(clientId);
		if (!playerByClientId)
		{
			return;
		}
		NetworkBehaviourSingleton<ServerManager>.Instance.AdminManager.IsSteamIdAdmin(playerByClientId.SteamId.Value.ToString());
		if (!(a == "/vs") && !(a == "/votestart"))
		{
			if (!(a == "/vw") && !(a == "/votewarmup"))
			{
				if (!(a == "/vk") && !(a == "/votekick"))
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
				int votesNeeded = Mathf.RoundToInt((float)MonoBehaviourSingleton<PlayerManager>.Instance.GetPlayers(false).Count / 2f + 0.5f);
				Player player = MonoBehaviourSingleton<PlayerManager>.Instance.GetPlayerByUsername(array[0], false);
				if (!player)
				{
					int number;
					if (int.TryParse(array[0], out number))
					{
						player = MonoBehaviourSingleton<PlayerManager>.Instance.GetPlayerByNumber(number);
					}
					if (!player)
					{
						return;
					}
				}
				if (NetworkBehaviourSingleton<ServerManager>.Instance.AdminManager.IsSteamIdAdmin(player.SteamId.Value.ToString()))
				{
					return;
				}
				this.voteManager.Server_CreateVote(VoteType.Kick, votesNeeded, playerByClientId, player.SteamId.Value);
				this.voteManager.Server_SubmitVote(VoteType.Kick, playerByClientId);
				return;
			}
			else
			{
				if (this.voteManager.Server_IsVoteStarted(VoteType.Warmup))
				{
					this.voteManager.Server_SubmitVote(VoteType.Warmup, playerByClientId);
					return;
				}
				int votesNeeded2 = Mathf.RoundToInt((float)MonoBehaviourSingleton<PlayerManager>.Instance.GetPlayers(false).Count / 2f + 0.5f);
				this.voteManager.Server_CreateVote(VoteType.Warmup, votesNeeded2, playerByClientId, null);
				this.voteManager.Server_SubmitVote(VoteType.Warmup, playerByClientId);
				return;
			}
		}
		else
		{
			if (this.voteManager.Server_IsVoteStarted(VoteType.Start))
			{
				this.voteManager.Server_SubmitVote(VoteType.Start, playerByClientId);
				return;
			}
			int votesNeeded3 = Mathf.RoundToInt((float)MonoBehaviourSingleton<PlayerManager>.Instance.GetPlayers(false).Count / 2f + 0.5f);
			this.voteManager.Server_CreateVote(VoteType.Start, votesNeeded3, playerByClientId, null);
			this.voteManager.Server_SubmitVote(VoteType.Start, playerByClientId);
			return;
		}
	}

	// Token: 0x040005B0 RID: 1456
	private VoteManager voteManager;
}
