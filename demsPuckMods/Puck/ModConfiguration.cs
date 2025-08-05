using System;

// Token: 0x02000098 RID: 152
public class ModConfiguration
{
	// Token: 0x17000058 RID: 88
	// (get) Token: 0x060003D1 RID: 977 RVA: 0x00009632 File Offset: 0x00007832
	// (set) Token: 0x060003D2 RID: 978 RVA: 0x0000963A File Offset: 0x0000783A
	public ulong id { get; set; }

	// Token: 0x17000059 RID: 89
	// (get) Token: 0x060003D3 RID: 979 RVA: 0x00009643 File Offset: 0x00007843
	// (set) Token: 0x060003D4 RID: 980 RVA: 0x0000964B File Offset: 0x0000784B
	public bool enabled { get; set; } = true;

	// Token: 0x1700005A RID: 90
	// (get) Token: 0x060003D5 RID: 981 RVA: 0x00009654 File Offset: 0x00007854
	// (set) Token: 0x060003D6 RID: 982 RVA: 0x0000965C File Offset: 0x0000785C
	public bool clientRequired { get; set; }
}
