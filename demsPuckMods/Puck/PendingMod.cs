using System;

// Token: 0x0200004F RID: 79
public class PendingMod
{
	// Token: 0x0600023E RID: 574 RVA: 0x00008548 File Offset: 0x00006748
	public PendingMod(ulong id, Mod mod = null)
	{
		this.Id = id;
		this.Mod = mod;
	}

	// Token: 0x0400014D RID: 333
	public ulong Id;

	// Token: 0x0400014E RID: 334
	public Mod Mod;
}
