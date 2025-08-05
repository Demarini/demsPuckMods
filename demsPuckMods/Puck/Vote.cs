using System;
using System.Collections.Generic;
using Unity.Collections;

// Token: 0x020000C5 RID: 197
public class Vote
{
	// Token: 0x17000088 RID: 136
	// (get) Token: 0x060005FE RID: 1534 RVA: 0x0000ABA6 File Offset: 0x00008DA6
	public int Votes
	{
		get
		{
			return this.VoterSteamIds.Count;
		}
	}

	// Token: 0x060005FF RID: 1535 RVA: 0x00024B1C File Offset: 0x00022D1C
	public Vote(VoteType type, int votesNeeded, Player startedBy = null, Action<Vote> onVoteStarted = null, Action<Vote, Player> onVoteProgress = null, Action<Vote> onVoteSuccess = null, Action<Vote> onVoteFailed = null, object data = null)
	{
		this.Type = type;
		this.VotesNeeded = votesNeeded;
		this.StartedBy = startedBy;
		this.IsInProgress = true;
		this.onVoteProgress = onVoteProgress;
		this.onVoteSuccess = onVoteSuccess;
		this.onVoteFailed = onVoteFailed;
		this.Data = data;
		if (onVoteStarted != null)
		{
			onVoteStarted(this);
		}
		this.SubmitVote(startedBy, false);
	}

	// Token: 0x06000600 RID: 1536 RVA: 0x00024B98 File Offset: 0x00022D98
	public void SubmitVote(Player voter, bool notifyListeners = true)
	{
		if (this.VoterSteamIds.Contains(voter.SteamId.Value))
		{
			return;
		}
		this.VoterSteamIds.Add(voter.SteamId.Value);
		if (notifyListeners)
		{
			Action<Vote, Player> action = this.onVoteProgress;
			if (action != null)
			{
				action(this, voter);
			}
		}
		if (this.VoterSteamIds.Count >= this.VotesNeeded)
		{
			this.IsInProgress = false;
			Action<Vote> action2 = this.onVoteSuccess;
			if (action2 == null)
			{
				return;
			}
			action2(this);
		}
	}

	// Token: 0x06000601 RID: 1537 RVA: 0x0000ABB3 File Offset: 0x00008DB3
	public void Tick(float deltaTime)
	{
		this.Timeout -= deltaTime;
		if (this.IsInProgress && this.Timeout <= 0f)
		{
			this.IsInProgress = false;
			Action<Vote> action = this.onVoteFailed;
			if (action == null)
			{
				return;
			}
			action(this);
		}
	}

	// Token: 0x0400033F RID: 831
	public VoteType Type;

	// Token: 0x04000340 RID: 832
	public List<FixedString32Bytes> VoterSteamIds = new List<FixedString32Bytes>();

	// Token: 0x04000341 RID: 833
	public int VotesNeeded;

	// Token: 0x04000342 RID: 834
	public float Timeout = 60f;

	// Token: 0x04000343 RID: 835
	public Player StartedBy;

	// Token: 0x04000344 RID: 836
	public bool IsInProgress;

	// Token: 0x04000345 RID: 837
	public object Data;

	// Token: 0x04000346 RID: 838
	private Action<Vote, Player> onVoteProgress;

	// Token: 0x04000347 RID: 839
	private Action<Vote> onVoteSuccess;

	// Token: 0x04000348 RID: 840
	private Action<Vote> onVoteFailed;
}
