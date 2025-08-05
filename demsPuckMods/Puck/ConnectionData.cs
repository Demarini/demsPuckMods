using System;

// Token: 0x02000059 RID: 89
public class ConnectionData
{
	// Token: 0x1700003E RID: 62
	// (get) Token: 0x06000282 RID: 642 RVA: 0x0000889A File Offset: 0x00006A9A
	// (set) Token: 0x06000283 RID: 643 RVA: 0x000088A2 File Offset: 0x00006AA2
	public string Password { get; set; }

	// Token: 0x1700003F RID: 63
	// (get) Token: 0x06000284 RID: 644 RVA: 0x000088AB File Offset: 0x00006AAB
	// (set) Token: 0x06000285 RID: 645 RVA: 0x000088B3 File Offset: 0x00006AB3
	public string SteamId { get; set; }

	// Token: 0x17000040 RID: 64
	// (get) Token: 0x06000286 RID: 646 RVA: 0x000088BC File Offset: 0x00006ABC
	// (set) Token: 0x06000287 RID: 647 RVA: 0x000088C4 File Offset: 0x00006AC4
	public string SocketId { get; set; }

	// Token: 0x17000041 RID: 65
	// (get) Token: 0x06000288 RID: 648 RVA: 0x000088CD File Offset: 0x00006ACD
	// (set) Token: 0x06000289 RID: 649 RVA: 0x000088D5 File Offset: 0x00006AD5
	public ulong[] EnabledModIds { get; set; }
}
