using System;

// Token: 0x0200021E RID: 542
public class PlayerPartyData
{
	// Token: 0x1700014C RID: 332
	// (get) Token: 0x06000F1A RID: 3866 RVA: 0x00044940 File Offset: 0x00042B40
	// (set) Token: 0x06000F1B RID: 3867 RVA: 0x00044948 File Offset: 0x00042B48
	public string id { get; set; }

	// Token: 0x1700014D RID: 333
	// (get) Token: 0x06000F1C RID: 3868 RVA: 0x00044951 File Offset: 0x00042B51
	// (set) Token: 0x06000F1D RID: 3869 RVA: 0x00044959 File Offset: 0x00042B59
	public string steamLobbyId { get; set; }

	// Token: 0x1700014E RID: 334
	// (get) Token: 0x06000F1E RID: 3870 RVA: 0x00044962 File Offset: 0x00042B62
	// (set) Token: 0x06000F1F RID: 3871 RVA: 0x0004496A File Offset: 0x00042B6A
	public string ownerSteamId { get; set; }

	// Token: 0x1700014F RID: 335
	// (get) Token: 0x06000F20 RID: 3872 RVA: 0x00044973 File Offset: 0x00042B73
	// (set) Token: 0x06000F21 RID: 3873 RVA: 0x0004497B File Offset: 0x00042B7B
	public string[] memberSteamIds { get; set; }
}
