using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

// Token: 0x02000146 RID: 326
public class VoteManager : MonoBehaviourSingleton<VoteManager>
{
	// Token: 0x060009C9 RID: 2505 RVA: 0x0002F7E0 File Offset: 0x0002D9E0
	private void Update()
	{
		if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
		{
			return;
		}
		foreach (Vote vote in this.votes.ToList<Vote>())
		{
			vote.Tick(Time.deltaTime);
		}
	}

	// Token: 0x060009CA RID: 2506 RVA: 0x0002F854 File Offset: 0x0002DA54
	public void Server_CreateVote(VoteType voteType, int votesNeeded, Player startedBy, object data = null)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (this.Server_IsVoteStarted(voteType))
		{
			return;
		}
		new Vote(voteType, votesNeeded, startedBy, new Action<Vote>(this.OnVoteStarted), new Action<Vote, Player>(this.OnVoteProgress), new Action<Vote>(this.OnVoteSuccess), new Action<Vote>(this.OnVoteFailed), data);
	}

	// Token: 0x060009CB RID: 2507 RVA: 0x0002F8B3 File Offset: 0x0002DAB3
	public void Server_SubmitVote(VoteType voteType, Player voter)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (!this.Server_IsVoteStarted(voteType))
		{
			return;
		}
		this.Server_GetVote(voteType).SubmitVote(voter, true);
	}

	// Token: 0x060009CC RID: 2508 RVA: 0x0002F8DC File Offset: 0x0002DADC
	public bool Server_IsVoteStarted(VoteType type)
	{
		return NetworkManager.Singleton.IsServer && this.votes.Any((Vote v) => v.Type == type && v.IsInProgress);
	}

	// Token: 0x060009CD RID: 2509 RVA: 0x0002F91C File Offset: 0x0002DB1C
	public Vote Server_GetVote(VoteType type)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return null;
		}
		if (!this.Server_IsVoteStarted(type))
		{
			return null;
		}
		return this.votes.FirstOrDefault((Vote v) => v.Type == type);
	}

	// Token: 0x060009CE RID: 2510 RVA: 0x0002F96B File Offset: 0x0002DB6B
	private void AddVote(Vote vote)
	{
		this.votes.Add(vote);
	}

	// Token: 0x060009CF RID: 2511 RVA: 0x0002F979 File Offset: 0x0002DB79
	private void RemoveVote(Vote vote)
	{
		this.votes.Remove(vote);
	}

	// Token: 0x060009D0 RID: 2512 RVA: 0x0002F988 File Offset: 0x0002DB88
	private void OnVoteStarted(Vote vote)
	{
		EventManager.TriggerEvent("Event_Server_OnVoteStarted", new Dictionary<string, object>
		{
			{
				"vote",
				vote
			}
		});
		this.AddVote(vote);
	}

	// Token: 0x060009D1 RID: 2513 RVA: 0x0002F9AC File Offset: 0x0002DBAC
	private void OnVoteProgress(Vote vote, Player voter)
	{
		EventManager.TriggerEvent("Event_Server_OnVoteProgress", new Dictionary<string, object>
		{
			{
				"vote",
				vote
			},
			{
				"voter",
				voter
			}
		});
	}

	// Token: 0x060009D2 RID: 2514 RVA: 0x0002F9D5 File Offset: 0x0002DBD5
	private void OnVoteSuccess(Vote vote)
	{
		this.RemoveVote(vote);
		EventManager.TriggerEvent("Event_Server_OnVoteSuccess", new Dictionary<string, object>
		{
			{
				"vote",
				vote
			}
		});
	}

	// Token: 0x060009D3 RID: 2515 RVA: 0x0002F9F9 File Offset: 0x0002DBF9
	private void OnVoteFailed(Vote vote)
	{
		this.RemoveVote(vote);
		EventManager.TriggerEvent("Event_Server_OnVoteFailed", new Dictionary<string, object>
		{
			{
				"vote",
				vote
			}
		});
	}

	// Token: 0x040005AD RID: 1453
	private List<Vote> votes = new List<Vote>();
}
