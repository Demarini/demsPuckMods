using System;

// Token: 0x020001F2 RID: 498
public interface Snapshot
{
	// Token: 0x17000129 RID: 297
	// (get) Token: 0x06000E5C RID: 3676
	// (set) Token: 0x06000E5D RID: 3677
	double remoteTime { get; set; }

	// Token: 0x1700012A RID: 298
	// (get) Token: 0x06000E5E RID: 3678
	// (set) Token: 0x06000E5F RID: 3679
	double localTime { get; set; }
}
