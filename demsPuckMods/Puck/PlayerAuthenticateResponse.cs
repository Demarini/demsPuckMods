using System;

// Token: 0x02000147 RID: 327
public class PlayerAuthenticateResponse
{
	// Token: 0x170000F1 RID: 241
	// (get) Token: 0x06000BA4 RID: 2980 RVA: 0x0000EBD7 File Offset: 0x0000CDD7
	// (set) Token: 0x06000BA5 RID: 2981 RVA: 0x0000EBDF File Offset: 0x0000CDDF
	public bool success { get; set; }

	// Token: 0x170000F2 RID: 242
	// (get) Token: 0x06000BA6 RID: 2982 RVA: 0x0000EBE8 File Offset: 0x0000CDE8
	// (set) Token: 0x06000BA7 RID: 2983 RVA: 0x0000EBF0 File Offset: 0x0000CDF0
	public string error { get; set; }

	// Token: 0x170000F3 RID: 243
	// (get) Token: 0x06000BA8 RID: 2984 RVA: 0x0000EBF9 File Offset: 0x0000CDF9
	// (set) Token: 0x06000BA9 RID: 2985 RVA: 0x0000EC01 File Offset: 0x0000CE01
	public string steamId { get; set; }
}
