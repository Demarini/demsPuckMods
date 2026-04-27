using System;

// Token: 0x020000D0 RID: 208
public class ConnectionRejection
{
	// Token: 0x170000C8 RID: 200
	// (get) Token: 0x0600066C RID: 1644 RVA: 0x00020951 File Offset: 0x0001EB51
	// (set) Token: 0x0600066D RID: 1645 RVA: 0x00020959 File Offset: 0x0001EB59
	public ConnectionRejectionCode code { get; set; }

	// Token: 0x170000C9 RID: 201
	// (get) Token: 0x0600066E RID: 1646 RVA: 0x00020962 File Offset: 0x0001EB62
	// (set) Token: 0x0600066F RID: 1647 RVA: 0x0002096A File Offset: 0x0001EB6A
	public string message { get; set; }

	// Token: 0x170000CA RID: 202
	// (get) Token: 0x06000670 RID: 1648 RVA: 0x00020973 File Offset: 0x0001EB73
	// (set) Token: 0x06000671 RID: 1649 RVA: 0x0002097B File Offset: 0x0001EB7B
	public ulong[] clientRequiredModIds { get; set; }
}
