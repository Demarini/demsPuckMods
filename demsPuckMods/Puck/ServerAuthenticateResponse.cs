using System;

// Token: 0x0200014E RID: 334
public class ServerAuthenticateResponse
{
	// Token: 0x1700010D RID: 269
	// (get) Token: 0x06000BE3 RID: 3043 RVA: 0x0000EDB3 File Offset: 0x0000CFB3
	// (set) Token: 0x06000BE4 RID: 3044 RVA: 0x0000EDBB File Offset: 0x0000CFBB
	public bool success { get; set; }

	// Token: 0x1700010E RID: 270
	// (get) Token: 0x06000BE5 RID: 3045 RVA: 0x0000EDC4 File Offset: 0x0000CFC4
	// (set) Token: 0x06000BE6 RID: 3046 RVA: 0x0000EDCC File Offset: 0x0000CFCC
	public string error { get; set; }

	// Token: 0x1700010F RID: 271
	// (get) Token: 0x06000BE7 RID: 3047 RVA: 0x0000EDD5 File Offset: 0x0000CFD5
	// (set) Token: 0x06000BE8 RID: 3048 RVA: 0x0000EDDD File Offset: 0x0000CFDD
	public string ipAddress { get; set; }

	// Token: 0x17000110 RID: 272
	// (get) Token: 0x06000BE9 RID: 3049 RVA: 0x0000EDE6 File Offset: 0x0000CFE6
	// (set) Token: 0x06000BEA RID: 3050 RVA: 0x0000EDEE File Offset: 0x0000CFEE
	public bool isAuthenticated { get; set; }
}
