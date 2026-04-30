using System;

// Token: 0x02000225 RID: 549
public class PlayerStatistics
{
	// Token: 0x1700015A RID: 346
	// (get) Token: 0x06000F3C RID: 3900 RVA: 0x00044AA5 File Offset: 0x00042CA5
	// (set) Token: 0x06000F3D RID: 3901 RVA: 0x00044AAD File Offset: 0x00042CAD
	public PlayerManagerStatistics playerManager { get; set; }

	// Token: 0x1700015B RID: 347
	// (get) Token: 0x06000F3E RID: 3902 RVA: 0x00044AB6 File Offset: 0x00042CB6
	// (set) Token: 0x06000F3F RID: 3903 RVA: 0x00044ABE File Offset: 0x00042CBE
	public ServerManagerStatistics serverManager { get; set; }

	// Token: 0x1700015C RID: 348
	// (get) Token: 0x06000F40 RID: 3904 RVA: 0x00044AC7 File Offset: 0x00042CC7
	// (set) Token: 0x06000F41 RID: 3905 RVA: 0x00044ACF File Offset: 0x00042CCF
	public MatchmakingManagerStatistics matchmakingManager { get; set; }
}
