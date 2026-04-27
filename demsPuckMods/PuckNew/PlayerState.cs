using System;

// Token: 0x02000087 RID: 135
public struct PlayerState
{
	// Token: 0x06000486 RID: 1158 RVA: 0x00018B68 File Offset: 0x00016D68
	public bool Equals(PlayerState other)
	{
		return this.AuthenticationPhase == other.AuthenticationPhase && this.PlayerData == other.PlayerData && this.PartyData == other.PartyData && this.GroupData == other.GroupData && this.MatchData == other.MatchData && this.PlayerStatistics == other.PlayerStatistics && this.Key == other.Key;
	}

	// Token: 0x06000487 RID: 1159 RVA: 0x00018BDC File Offset: 0x00016DDC
	public override bool Equals(object obj)
	{
		if (obj is PlayerState)
		{
			PlayerState other = (PlayerState)obj;
			return this.Equals(other);
		}
		return false;
	}

	// Token: 0x06000488 RID: 1160 RVA: 0x00018C01 File Offset: 0x00016E01
	public override int GetHashCode()
	{
		return HashCode.Combine<AuthenticationPhase, PlayerData, PlayerPartyData, PlayerGroupData, PlayerMatchData, PlayerStatistics, string>(this.AuthenticationPhase, this.PlayerData, this.PartyData, this.GroupData, this.MatchData, this.PlayerStatistics, this.Key);
	}

	// Token: 0x06000489 RID: 1161 RVA: 0x00018C34 File Offset: 0x00016E34
	public override string ToString()
	{
		return string.Format("AuthenticationPhase: {0}, PlayerData: {1}, PartyData: {2}, GroupData: {3}, MatchData: {4}, PlayerStatistics: {5}, Key: {6}", new object[]
		{
			this.AuthenticationPhase,
			this.PlayerData,
			this.PartyData,
			this.GroupData,
			this.MatchData,
			this.PlayerStatistics,
			this.Key
		});
	}

	// Token: 0x040002C7 RID: 711
	public AuthenticationPhase AuthenticationPhase;

	// Token: 0x040002C8 RID: 712
	public PlayerData PlayerData;

	// Token: 0x040002C9 RID: 713
	public PlayerPartyData PartyData;

	// Token: 0x040002CA RID: 714
	public PlayerGroupData GroupData;

	// Token: 0x040002CB RID: 715
	public PlayerMatchData MatchData;

	// Token: 0x040002CC RID: 716
	public PlayerStatistics PlayerStatistics;

	// Token: 0x040002CD RID: 717
	public string Key;
}
