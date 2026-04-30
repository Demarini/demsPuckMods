using System;

// Token: 0x02000228 RID: 552
public class PlayerCooldown
{
	// Token: 0x17000165 RID: 357
	// (get) Token: 0x06000F55 RID: 3925 RVA: 0x00044B60 File Offset: 0x00042D60
	// (set) Token: 0x06000F56 RID: 3926 RVA: 0x00044B68 File Offset: 0x00042D68
	public int id { get; set; }

	// Token: 0x17000166 RID: 358
	// (get) Token: 0x06000F57 RID: 3927 RVA: 0x00044B71 File Offset: 0x00042D71
	// (set) Token: 0x06000F58 RID: 3928 RVA: 0x00044B79 File Offset: 0x00042D79
	public string matchId { get; set; }

	// Token: 0x17000167 RID: 359
	// (get) Token: 0x06000F59 RID: 3929 RVA: 0x00044B82 File Offset: 0x00042D82
	// (set) Token: 0x06000F5A RID: 3930 RVA: 0x00044B8A File Offset: 0x00042D8A
	public double issuedAt { get; set; }

	// Token: 0x17000168 RID: 360
	// (get) Token: 0x06000F5B RID: 3931 RVA: 0x00044B93 File Offset: 0x00042D93
	// (set) Token: 0x06000F5C RID: 3932 RVA: 0x00044B9B File Offset: 0x00042D9B
	public double expiresAt { get; set; }
}
