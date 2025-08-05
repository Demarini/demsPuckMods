using System;

// Token: 0x0200005C RID: 92
public class ConnectionRejection
{
	// Token: 0x17000045 RID: 69
	// (get) Token: 0x06000292 RID: 658 RVA: 0x00008911 File Offset: 0x00006B11
	// (set) Token: 0x06000293 RID: 659 RVA: 0x00008919 File Offset: 0x00006B19
	public ConnectionRejectionCode code { get; set; }

	// Token: 0x17000046 RID: 70
	// (get) Token: 0x06000294 RID: 660 RVA: 0x00008922 File Offset: 0x00006B22
	// (set) Token: 0x06000295 RID: 661 RVA: 0x0000892A File Offset: 0x00006B2A
	public ulong[] clientRequiredModIds { get; set; }
}
