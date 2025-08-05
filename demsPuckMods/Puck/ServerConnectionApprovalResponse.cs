using System;

// Token: 0x0200014F RID: 335
public class ServerConnectionApprovalResponse
{
	// Token: 0x17000111 RID: 273
	// (get) Token: 0x06000BEC RID: 3052 RVA: 0x0000EDF7 File Offset: 0x0000CFF7
	// (set) Token: 0x06000BED RID: 3053 RVA: 0x0000EDFF File Offset: 0x0000CFFF
	public bool success { get; set; }

	// Token: 0x17000112 RID: 274
	// (get) Token: 0x06000BEE RID: 3054 RVA: 0x0000EE08 File Offset: 0x0000D008
	// (set) Token: 0x06000BEF RID: 3055 RVA: 0x0000EE10 File Offset: 0x0000D010
	public string error { get; set; }

	// Token: 0x17000113 RID: 275
	// (get) Token: 0x06000BF0 RID: 3056 RVA: 0x0000EE19 File Offset: 0x0000D019
	// (set) Token: 0x06000BF1 RID: 3057 RVA: 0x0000EE21 File Offset: 0x0000D021
	public string steamId { get; set; }
}
