using System;
using System.Collections.Generic;

// Token: 0x0200007A RID: 122
public class GameResult
{
	// Token: 0x17000070 RID: 112
	// (get) Token: 0x06000421 RID: 1057 RVA: 0x00016CAC File Offset: 0x00014EAC
	// (set) Token: 0x06000422 RID: 1058 RVA: 0x00016CB4 File Offset: 0x00014EB4
	public PlayerTeam winningTeam { get; set; }

	// Token: 0x17000071 RID: 113
	// (get) Token: 0x06000423 RID: 1059 RVA: 0x00016CBD File Offset: 0x00014EBD
	// (set) Token: 0x06000424 RID: 1060 RVA: 0x00016CC5 File Offset: 0x00014EC5
	public int blueScore { get; set; }

	// Token: 0x17000072 RID: 114
	// (get) Token: 0x06000425 RID: 1061 RVA: 0x00016CCE File Offset: 0x00014ECE
	// (set) Token: 0x06000426 RID: 1062 RVA: 0x00016CD6 File Offset: 0x00014ED6
	public int redScore { get; set; }

	// Token: 0x17000073 RID: 115
	// (get) Token: 0x06000427 RID: 1063 RVA: 0x00016CDF File Offset: 0x00014EDF
	// (set) Token: 0x06000428 RID: 1064 RVA: 0x00016CE7 File Offset: 0x00014EE7
	public Dictionary<string, PlayerResult> playerResults { get; set; } = new Dictionary<string, PlayerResult>();
}
