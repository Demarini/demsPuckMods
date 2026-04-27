using System;

// Token: 0x0200021F RID: 543
public class PlayerGroupData
{
	// Token: 0x17000150 RID: 336
	// (get) Token: 0x06000F23 RID: 3875 RVA: 0x00044984 File Offset: 0x00042B84
	// (set) Token: 0x06000F24 RID: 3876 RVA: 0x0004498C File Offset: 0x00042B8C
	public string id { get; set; }

	// Token: 0x17000151 RID: 337
	// (get) Token: 0x06000F25 RID: 3877 RVA: 0x00044995 File Offset: 0x00042B95
	// (set) Token: 0x06000F26 RID: 3878 RVA: 0x0004499D File Offset: 0x00042B9D
	public string ownerSteamId { get; set; }

	// Token: 0x17000152 RID: 338
	// (get) Token: 0x06000F27 RID: 3879 RVA: 0x000449A6 File Offset: 0x00042BA6
	// (set) Token: 0x06000F28 RID: 3880 RVA: 0x000449AE File Offset: 0x00042BAE
	public string[] memberSteamIds { get; set; }
}
