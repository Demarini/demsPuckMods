using System;

// Token: 0x02000149 RID: 329
public class PlayerCompletePurchaseResponse
{
	// Token: 0x170000F7 RID: 247
	// (get) Token: 0x06000BB2 RID: 2994 RVA: 0x0000EC3D File Offset: 0x0000CE3D
	// (set) Token: 0x06000BB3 RID: 2995 RVA: 0x0000EC45 File Offset: 0x0000CE45
	public bool success { get; set; }

	// Token: 0x170000F8 RID: 248
	// (get) Token: 0x06000BB4 RID: 2996 RVA: 0x0000EC4E File Offset: 0x0000CE4E
	// (set) Token: 0x06000BB5 RID: 2997 RVA: 0x0000EC56 File Offset: 0x0000CE56
	public string error { get; set; }

	// Token: 0x170000F9 RID: 249
	// (get) Token: 0x06000BB6 RID: 2998 RVA: 0x0000EC5F File Offset: 0x0000CE5F
	// (set) Token: 0x06000BB7 RID: 2999 RVA: 0x0000EC67 File Offset: 0x0000CE67
	public string orderId { get; set; }
}
