using System;

// Token: 0x02000226 RID: 550
public class PlayerMute
{
	// Token: 0x1700015D RID: 349
	// (get) Token: 0x06000F43 RID: 3907 RVA: 0x00044AD8 File Offset: 0x00042CD8
	// (set) Token: 0x06000F44 RID: 3908 RVA: 0x00044AE0 File Offset: 0x00042CE0
	public int id { get; set; }

	// Token: 0x1700015E RID: 350
	// (get) Token: 0x06000F45 RID: 3909 RVA: 0x00044AE9 File Offset: 0x00042CE9
	// (set) Token: 0x06000F46 RID: 3910 RVA: 0x00044AF1 File Offset: 0x00042CF1
	public double issuedAt { get; set; }

	// Token: 0x1700015F RID: 351
	// (get) Token: 0x06000F47 RID: 3911 RVA: 0x00044AFA File Offset: 0x00042CFA
	// (set) Token: 0x06000F48 RID: 3912 RVA: 0x00044B02 File Offset: 0x00042D02
	public double expiresAt { get; set; }

	// Token: 0x17000160 RID: 352
	// (get) Token: 0x06000F49 RID: 3913 RVA: 0x00044B0B File Offset: 0x00042D0B
	// (set) Token: 0x06000F4A RID: 3914 RVA: 0x00044B13 File Offset: 0x00042D13
	public string reason { get; set; }
}
