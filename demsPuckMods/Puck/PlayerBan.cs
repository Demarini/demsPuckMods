using System;

// Token: 0x0200014C RID: 332
public class PlayerBan
{
	// Token: 0x17000107 RID: 263
	// (get) Token: 0x06000BD5 RID: 3029 RVA: 0x0000ED4D File Offset: 0x0000CF4D
	// (set) Token: 0x06000BD6 RID: 3030 RVA: 0x0000ED55 File Offset: 0x0000CF55
	public int id { get; set; }

	// Token: 0x17000108 RID: 264
	// (get) Token: 0x06000BD7 RID: 3031 RVA: 0x0000ED5E File Offset: 0x0000CF5E
	// (set) Token: 0x06000BD8 RID: 3032 RVA: 0x0000ED66 File Offset: 0x0000CF66
	public double at { get; set; }

	// Token: 0x17000109 RID: 265
	// (get) Token: 0x06000BD9 RID: 3033 RVA: 0x0000ED6F File Offset: 0x0000CF6F
	// (set) Token: 0x06000BDA RID: 3034 RVA: 0x0000ED77 File Offset: 0x0000CF77
	public double until { get; set; }

	// Token: 0x1700010A RID: 266
	// (get) Token: 0x06000BDB RID: 3035 RVA: 0x0000ED80 File Offset: 0x0000CF80
	// (set) Token: 0x06000BDC RID: 3036 RVA: 0x0000ED88 File Offset: 0x0000CF88
	public string reason { get; set; }
}
