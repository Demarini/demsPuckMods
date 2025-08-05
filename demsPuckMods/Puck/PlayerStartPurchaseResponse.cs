using System;

// Token: 0x02000148 RID: 328
public class PlayerStartPurchaseResponse
{
	// Token: 0x170000F4 RID: 244
	// (get) Token: 0x06000BAB RID: 2987 RVA: 0x0000EC0A File Offset: 0x0000CE0A
	// (set) Token: 0x06000BAC RID: 2988 RVA: 0x0000EC12 File Offset: 0x0000CE12
	public bool success { get; set; }

	// Token: 0x170000F5 RID: 245
	// (get) Token: 0x06000BAD RID: 2989 RVA: 0x0000EC1B File Offset: 0x0000CE1B
	// (set) Token: 0x06000BAE RID: 2990 RVA: 0x0000EC23 File Offset: 0x0000CE23
	public string error { get; set; }

	// Token: 0x170000F6 RID: 246
	// (get) Token: 0x06000BAF RID: 2991 RVA: 0x0000EC2C File Offset: 0x0000CE2C
	// (set) Token: 0x06000BB0 RID: 2992 RVA: 0x0000EC34 File Offset: 0x0000CE34
	public string orderId { get; set; }
}
