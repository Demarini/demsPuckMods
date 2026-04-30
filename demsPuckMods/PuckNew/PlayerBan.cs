using System;

// Token: 0x02000227 RID: 551
public class PlayerBan
{
	// Token: 0x17000161 RID: 353
	// (get) Token: 0x06000F4C RID: 3916 RVA: 0x00044B1C File Offset: 0x00042D1C
	// (set) Token: 0x06000F4D RID: 3917 RVA: 0x00044B24 File Offset: 0x00042D24
	public int id { get; set; }

	// Token: 0x17000162 RID: 354
	// (get) Token: 0x06000F4E RID: 3918 RVA: 0x00044B2D File Offset: 0x00042D2D
	// (set) Token: 0x06000F4F RID: 3919 RVA: 0x00044B35 File Offset: 0x00042D35
	public double issuedAt { get; set; }

	// Token: 0x17000163 RID: 355
	// (get) Token: 0x06000F50 RID: 3920 RVA: 0x00044B3E File Offset: 0x00042D3E
	// (set) Token: 0x06000F51 RID: 3921 RVA: 0x00044B46 File Offset: 0x00042D46
	public double expiresAt { get; set; }

	// Token: 0x17000164 RID: 356
	// (get) Token: 0x06000F52 RID: 3922 RVA: 0x00044B4F File Offset: 0x00042D4F
	// (set) Token: 0x06000F53 RID: 3923 RVA: 0x00044B57 File Offset: 0x00042D57
	public string reason { get; set; }
}
