using System;

// Token: 0x02000165 RID: 357
public interface Snapshot
{
	// Token: 0x1700012C RID: 300
	// (get) Token: 0x06000C5E RID: 3166
	// (set) Token: 0x06000C5F RID: 3167
	double remoteTime { get; set; }

	// Token: 0x1700012D RID: 301
	// (get) Token: 0x06000C60 RID: 3168
	// (set) Token: 0x06000C61 RID: 3169
	double localTime { get; set; }
}
