using System;

// Token: 0x0200011D RID: 285
public class ModConfig
{
	// Token: 0x170000DB RID: 219
	// (get) Token: 0x060007D3 RID: 2003 RVA: 0x000260F3 File Offset: 0x000242F3
	// (set) Token: 0x060007D4 RID: 2004 RVA: 0x000260FB File Offset: 0x000242FB
	public ulong id { get; set; }

	// Token: 0x170000DC RID: 220
	// (get) Token: 0x060007D5 RID: 2005 RVA: 0x00026104 File Offset: 0x00024304
	// (set) Token: 0x060007D6 RID: 2006 RVA: 0x0002610C File Offset: 0x0002430C
	public bool enabled { get; set; }

	// Token: 0x170000DD RID: 221
	// (get) Token: 0x060007D7 RID: 2007 RVA: 0x00026115 File Offset: 0x00024315
	// (set) Token: 0x060007D8 RID: 2008 RVA: 0x0002611D File Offset: 0x0002431D
	public bool clientRequired { get; set; }
}
