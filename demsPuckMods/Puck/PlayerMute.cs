using System;

// Token: 0x0200014B RID: 331
public class PlayerMute
{
	// Token: 0x17000103 RID: 259
	// (get) Token: 0x06000BCC RID: 3020 RVA: 0x0000ED09 File Offset: 0x0000CF09
	// (set) Token: 0x06000BCD RID: 3021 RVA: 0x0000ED11 File Offset: 0x0000CF11
	public int id { get; set; }

	// Token: 0x17000104 RID: 260
	// (get) Token: 0x06000BCE RID: 3022 RVA: 0x0000ED1A File Offset: 0x0000CF1A
	// (set) Token: 0x06000BCF RID: 3023 RVA: 0x0000ED22 File Offset: 0x0000CF22
	public double at { get; set; }

	// Token: 0x17000105 RID: 261
	// (get) Token: 0x06000BD0 RID: 3024 RVA: 0x0000ED2B File Offset: 0x0000CF2B
	// (set) Token: 0x06000BD1 RID: 3025 RVA: 0x0000ED33 File Offset: 0x0000CF33
	public double until { get; set; }

	// Token: 0x17000106 RID: 262
	// (get) Token: 0x06000BD2 RID: 3026 RVA: 0x0000ED3C File Offset: 0x0000CF3C
	// (set) Token: 0x06000BD3 RID: 3027 RVA: 0x0000ED44 File Offset: 0x0000CF44
	public string reason { get; set; }
}
