using System;
using System.Collections.Generic;
using Unity.Collections;

// Token: 0x02000145 RID: 325
public class Vote
{
	// Token: 0x170000F7 RID: 247
	// (get) Token: 0x060009C5 RID: 2501 RVA: 0x0002F699 File Offset: 0x0002D899
	public int Votes
	{
		get
		{
			return this.VoterSteamIds.Count;
		}
	}

	// Token: 0x060009C6 RID: 2502 RVA: 0x0002F6A8 File Offset: 0x0002D8A8
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

	// Token: 0x060009C7 RID: 2503 RVA: 0x0002F724 File Offset: 0x0002D924
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

	// Token: 0x060009C8 RID: 2504 RVA: 0x0002F7A1 File Offset: 0x0002D9A1
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

	// Token: 0x040005A3 RID: 1443
	public VoteType Type;

	// Token: 0x040005A4 RID: 1444
	public List<FixedString32Bytes> VoterSteamIds = new List<FixedString32Bytes>();

	// Token: 0x040005A5 RID: 1445
	public int VotesNeeded;

	// Token: 0x040005A6 RID: 1446
	public float Timeout = 60f;

	// Token: 0x040005A7 RID: 1447
	public Player StartedBy;

	// Token: 0x040005A8 RID: 1448
	public bool IsInProgress;

	// Token: 0x040005A9 RID: 1449
	public object Data;

	// Token: 0x040005AA RID: 1450
	private Action<Vote, Player> onVoteProgress;

	// Token: 0x040005AB RID: 1451
	private Action<Vote> onVoteSuccess;

	// Token: 0x040005AC RID: 1452
	private Action<Vote> onVoteFailed;
}
