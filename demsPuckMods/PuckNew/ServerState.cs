using System;

// Token: 0x02000088 RID: 136
public struct ServerState
{
	// Token: 0x0600048A RID: 1162 RVA: 0x00018C95 File Offset: 0x00016E95
	public bool Equals(ServerState other)
	{
		return this.AuthenticationPhase == other.AuthenticationPhase && this.ServerData == other.ServerData && this.MatchData == other.MatchData;
	}

	// Token: 0x0600048B RID: 1163 RVA: 0x00018CC4 File Offset: 0x00016EC4
	public override bool Equals(object obj)
	{
		if (obj is ServerState)
		{
			ServerState other = (ServerState)obj;
			return this.Equals(other);
		}
		return false;
	}

	// Token: 0x0600048C RID: 1164 RVA: 0x00018CE9 File Offset: 0x00016EE9
	public override int GetHashCode()
	{
		return HashCode.Combine<AuthenticationPhase, ServerData, ServerMatchData>(this.AuthenticationPhase, this.ServerData, this.MatchData);
	}

	// Token: 0x0600048D RID: 1165 RVA: 0x00018D02 File Offset: 0x00016F02
	public override string ToString()
	{
		return string.Format("AuthenticationPhase: {0}, ServerData: {1}, MatchData: {2}", this.AuthenticationPhase, this.ServerData, this.MatchData);
	}

	// Token: 0x040002CE RID: 718
	public AuthenticationPhase AuthenticationPhase;

	// Token: 0x040002CF RID: 719
	public ServerData ServerData;

	// Token: 0x040002D0 RID: 720
	public ServerMatchData MatchData;
}
