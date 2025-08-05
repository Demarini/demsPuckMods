using System;

// Token: 0x0200014A RID: 330
public class PlayerData
{
	// Token: 0x170000FA RID: 250
	// (get) Token: 0x06000BB9 RID: 3001 RVA: 0x0000EC70 File Offset: 0x0000CE70
	// (set) Token: 0x06000BBA RID: 3002 RVA: 0x0000EC78 File Offset: 0x0000CE78
	public string steamId { get; set; }

	// Token: 0x170000FB RID: 251
	// (get) Token: 0x06000BBB RID: 3003 RVA: 0x0000EC81 File Offset: 0x0000CE81
	// (set) Token: 0x06000BBC RID: 3004 RVA: 0x0000EC89 File Offset: 0x0000CE89
	public string username { get; set; } = "PLAYER";

	// Token: 0x170000FC RID: 252
	// (get) Token: 0x06000BBD RID: 3005 RVA: 0x0000EC92 File Offset: 0x0000CE92
	// (set) Token: 0x06000BBE RID: 3006 RVA: 0x0000EC9A File Offset: 0x0000CE9A
	public int number { get; set; } = new Random().Next(0, 100);

	// Token: 0x170000FD RID: 253
	// (get) Token: 0x06000BBF RID: 3007 RVA: 0x0000ECA3 File Offset: 0x0000CEA3
	// (set) Token: 0x06000BC0 RID: 3008 RVA: 0x0000ECAB File Offset: 0x0000CEAB
	public double lastUsernameChange { get; set; }

	// Token: 0x170000FE RID: 254
	// (get) Token: 0x06000BC1 RID: 3009 RVA: 0x0000ECB4 File Offset: 0x0000CEB4
	// (set) Token: 0x06000BC2 RID: 3010 RVA: 0x0000ECBC File Offset: 0x0000CEBC
	public int patreonLevel { get; set; }

	// Token: 0x170000FF RID: 255
	// (get) Token: 0x06000BC3 RID: 3011 RVA: 0x0000ECC5 File Offset: 0x0000CEC5
	// (set) Token: 0x06000BC4 RID: 3012 RVA: 0x0000ECCD File Offset: 0x0000CECD
	public int adminLevel { get; set; }

	// Token: 0x17000100 RID: 256
	// (get) Token: 0x06000BC5 RID: 3013 RVA: 0x0000ECD6 File Offset: 0x0000CED6
	// (set) Token: 0x06000BC6 RID: 3014 RVA: 0x0000ECDE File Offset: 0x0000CEDE
	public PlayerItem[] items { get; set; } = new PlayerItem[0];

	// Token: 0x17000101 RID: 257
	// (get) Token: 0x06000BC7 RID: 3015 RVA: 0x0000ECE7 File Offset: 0x0000CEE7
	// (set) Token: 0x06000BC8 RID: 3016 RVA: 0x0000ECEF File Offset: 0x0000CEEF
	public PlayerMute[] mutes { get; set; } = new PlayerMute[0];

	// Token: 0x17000102 RID: 258
	// (get) Token: 0x06000BC9 RID: 3017 RVA: 0x0000ECF8 File Offset: 0x0000CEF8
	// (set) Token: 0x06000BCA RID: 3018 RVA: 0x0000ED00 File Offset: 0x0000CF00
	public PlayerBan[] bans { get; set; } = new PlayerBan[0];
}
