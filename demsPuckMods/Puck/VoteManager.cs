using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

// Token: 0x020000C6 RID: 198
public class VoteManager : NetworkBehaviourSingleton<VoteManager>
{
	// Token: 0x06000602 RID: 1538 RVA: 0x00024C18 File Offset: 0x00022E18
	private void Update()
	{
		if (!base.IsSpawned)
		{
			return;
		}
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		foreach (Vote vote in this.votes.ToList<Vote>())
		{
			vote.Tick(Time.deltaTime);
		}
	}

	// Token: 0x06000603 RID: 1539 RVA: 0x00024C88 File Offset: 0x00022E88
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

	// Token: 0x06000604 RID: 1540 RVA: 0x0000ABF0 File Offset: 0x00008DF0
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

	// Token: 0x06000605 RID: 1541 RVA: 0x00024CE8 File Offset: 0x00022EE8
	public bool Server_IsVoteStarted(VoteType type)
	{
		return NetworkManager.Singleton.IsServer && this.votes.Any((Vote v) => v.Type == type && v.IsInProgress);
	}

	// Token: 0x06000606 RID: 1542 RVA: 0x00024D28 File Offset: 0x00022F28
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

	// Token: 0x06000607 RID: 1543 RVA: 0x0000AC17 File Offset: 0x00008E17
	private void AddVote(Vote vote)
	{
		this.votes.Add(vote);
	}

	// Token: 0x06000608 RID: 1544 RVA: 0x0000AC25 File Offset: 0x00008E25
	private void RemoveVote(Vote vote)
	{
		this.votes.Remove(vote);
	}

	// Token: 0x06000609 RID: 1545 RVA: 0x0000AC34 File Offset: 0x00008E34
	private void OnVoteStarted(Vote vote)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Server_OnVoteStarted", new Dictionary<string, object>
		{
			{
				"vote",
				vote
			}
		});
		this.AddVote(vote);
	}

	// Token: 0x0600060A RID: 1546 RVA: 0x0000AC5D File Offset: 0x00008E5D
	private void OnVoteProgress(Vote vote, Player voter)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Server_OnVoteProgress", new Dictionary<string, object>
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

	// Token: 0x0600060B RID: 1547 RVA: 0x0000AC8B File Offset: 0x00008E8B
	private void OnVoteSuccess(Vote vote)
	{
		this.RemoveVote(vote);
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Server_OnVoteSuccess", new Dictionary<string, object>
		{
			{
				"vote",
				vote
			}
		});
	}

	// Token: 0x0600060C RID: 1548 RVA: 0x0000ACB4 File Offset: 0x00008EB4
	private void OnVoteFailed(Vote vote)
	{
		this.RemoveVote(vote);
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Server_OnVoteFailed", new Dictionary<string, object>
		{
			{
				"vote",
				vote
			}
		});
	}

	// Token: 0x0600060E RID: 1550 RVA: 0x00024D78 File Offset: 0x00022F78
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x0600060F RID: 1551 RVA: 0x0000ACF0 File Offset: 0x00008EF0
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x06000610 RID: 1552 RVA: 0x0000ACFA File Offset: 0x00008EFA
	protected internal override string __getTypeName()
	{
		return "VoteManager";
	}

	// Token: 0x04000349 RID: 841
	private List<Vote> votes = new List<Vote>();
}
