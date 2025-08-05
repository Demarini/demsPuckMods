using System;

// Token: 0x02000151 RID: 337
public class ServerBrowserServer
{
	// Token: 0x17000115 RID: 277
	// (get) Token: 0x06000BF6 RID: 3062 RVA: 0x0000EE3B File Offset: 0x0000D03B
	// (set) Token: 0x06000BF7 RID: 3063 RVA: 0x0000EE43 File Offset: 0x0000D043
	public string ipAddress { get; set; }

	// Token: 0x17000116 RID: 278
	// (get) Token: 0x06000BF8 RID: 3064 RVA: 0x0000EE4C File Offset: 0x0000D04C
	// (set) Token: 0x06000BF9 RID: 3065 RVA: 0x0000EE54 File Offset: 0x0000D054
	public ushort port { get; set; }

	// Token: 0x17000117 RID: 279
	// (get) Token: 0x06000BFA RID: 3066 RVA: 0x0000EE5D File Offset: 0x0000D05D
	// (set) Token: 0x06000BFB RID: 3067 RVA: 0x0000EE65 File Offset: 0x0000D065
	public ushort pingPort { get; set; }

	// Token: 0x17000118 RID: 280
	// (get) Token: 0x06000BFC RID: 3068 RVA: 0x0000EE6E File Offset: 0x0000D06E
	// (set) Token: 0x06000BFD RID: 3069 RVA: 0x0000EE76 File Offset: 0x0000D076
	public string name { get; set; }

	// Token: 0x17000119 RID: 281
	// (get) Token: 0x06000BFE RID: 3070 RVA: 0x0000EE7F File Offset: 0x0000D07F
	// (set) Token: 0x06000BFF RID: 3071 RVA: 0x0000EE87 File Offset: 0x0000D087
	public int maxPlayers { get; set; }

	// Token: 0x1700011A RID: 282
	// (get) Token: 0x06000C00 RID: 3072 RVA: 0x0000EE90 File Offset: 0x0000D090
	// (set) Token: 0x06000C01 RID: 3073 RVA: 0x0000EE98 File Offset: 0x0000D098
	public bool isPasswordProtected { get; set; }

	// Token: 0x1700011B RID: 283
	// (get) Token: 0x06000C02 RID: 3074 RVA: 0x0000EEA1 File Offset: 0x0000D0A1
	// (set) Token: 0x06000C03 RID: 3075 RVA: 0x0000EEA9 File Offset: 0x0000D0A9
	public int players { get; set; }
}
