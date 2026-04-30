using System;

// Token: 0x020000C1 RID: 193
public class PendingMod
{
	// Token: 0x060005D4 RID: 1492 RVA: 0x0001F2BB File Offset: 0x0001D4BB
	public PendingMod(ulong id, Mod mod = null)
	{
		this.Id = id;
		this.Mod = mod;
	}

	// Token: 0x0400039F RID: 927
	public ulong Id;

	// Token: 0x040003A0 RID: 928
	public Mod Mod;
}
