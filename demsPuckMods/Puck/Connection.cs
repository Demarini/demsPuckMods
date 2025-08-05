using System;

// Token: 0x0200005A RID: 90
public class Connection
{
	// Token: 0x17000042 RID: 66
	// (get) Token: 0x0600028B RID: 651 RVA: 0x000088DE File Offset: 0x00006ADE
	// (set) Token: 0x0600028C RID: 652 RVA: 0x000088E6 File Offset: 0x00006AE6
	public string IpAddress { get; set; }

	// Token: 0x17000043 RID: 67
	// (get) Token: 0x0600028D RID: 653 RVA: 0x000088EF File Offset: 0x00006AEF
	// (set) Token: 0x0600028E RID: 654 RVA: 0x000088F7 File Offset: 0x00006AF7
	public ushort Port { get; set; }

	// Token: 0x17000044 RID: 68
	// (get) Token: 0x0600028F RID: 655 RVA: 0x00008900 File Offset: 0x00006B00
	// (set) Token: 0x06000290 RID: 656 RVA: 0x00008908 File Offset: 0x00006B08
	public string Password { get; set; }
}
