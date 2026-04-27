using System;
using System.Linq;
using System.Text.Json.Serialization;

// Token: 0x0200022F RID: 559
public class MatchData
{
	// Token: 0x1700017E RID: 382
	// (get) Token: 0x06000F8E RID: 3982 RVA: 0x00044D09 File Offset: 0x00042F09
	// (set) Token: 0x06000F8F RID: 3983 RVA: 0x00044D11 File Offset: 0x00042F11
	public MatchPlayer[] homePlayers { get; set; }

	// Token: 0x1700017F RID: 383
	// (get) Token: 0x06000F90 RID: 3984 RVA: 0x00044D1A File Offset: 0x00042F1A
	// (set) Token: 0x06000F91 RID: 3985 RVA: 0x00044D22 File Offset: 0x00042F22
	public MatchPlayer[] awayPlayers { get; set; }

	// Token: 0x17000180 RID: 384
	// (get) Token: 0x06000F92 RID: 3986 RVA: 0x00044D2B File Offset: 0x00042F2B
	// (set) Token: 0x06000F93 RID: 3987 RVA: 0x00044D33 File Offset: 0x00042F33
	public double? startedAt { get; set; }

	// Token: 0x17000181 RID: 385
	// (get) Token: 0x06000F94 RID: 3988 RVA: 0x00044D3C File Offset: 0x00042F3C
	// (set) Token: 0x06000F95 RID: 3989 RVA: 0x00044D44 File Offset: 0x00042F44
	public EndPoint endPoint { get; set; }

	// Token: 0x17000182 RID: 386
	// (get) Token: 0x06000F96 RID: 3990 RVA: 0x00044D4D File Offset: 0x00042F4D
	[JsonIgnore]
	public MatchPlayer[] Players
	{
		get
		{
			return this.homePlayers.Concat(this.awayPlayers).ToArray<MatchPlayer>();
		}
	}

	// Token: 0x17000183 RID: 387
	// (get) Token: 0x06000F97 RID: 3991 RVA: 0x00044D65 File Offset: 0x00042F65
	[JsonIgnore]
	public string[] SteamIds
	{
		get
		{
			return (from p in this.Players
			select p.steamId).ToArray<string>();
		}
	}

	// Token: 0x06000F98 RID: 3992 RVA: 0x00044D98 File Offset: 0x00042F98
	public MatchPlayer GetMatchPlayerBySteamId(string steamId)
	{
		return this.Players.FirstOrDefault((MatchPlayer p) => p.steamId == steamId);
	}
}
