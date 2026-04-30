using System;
using System.Collections.Generic;

// Token: 0x02000072 RID: 114
public class CompetitiveGameModeConfig : StandardGameModeConfig
{
	// Token: 0x1700006B RID: 107
	// (get) Token: 0x060003F1 RID: 1009 RVA: 0x0001631D File Offset: 0x0001451D
	// (set) Token: 0x060003F2 RID: 1010 RVA: 0x00016325 File Offset: 0x00014525
	public Dictionary<PlayerTeam, string[]> teamAssignments { get; set; } = new Dictionary<PlayerTeam, string[]>();
}
