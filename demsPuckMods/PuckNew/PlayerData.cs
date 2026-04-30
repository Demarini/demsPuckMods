using System;

// Token: 0x0200021D RID: 541
public class PlayerData
{
	// Token: 0x17000141 RID: 321
	// (get) Token: 0x06000F03 RID: 3843 RVA: 0x00044885 File Offset: 0x00042A85
	// (set) Token: 0x06000F04 RID: 3844 RVA: 0x0004488D File Offset: 0x00042A8D
	public string steamId { get; set; }

	// Token: 0x17000142 RID: 322
	// (get) Token: 0x06000F05 RID: 3845 RVA: 0x00044896 File Offset: 0x00042A96
	// (set) Token: 0x06000F06 RID: 3846 RVA: 0x0004489E File Offset: 0x00042A9E
	public string username { get; set; }

	// Token: 0x17000143 RID: 323
	// (get) Token: 0x06000F07 RID: 3847 RVA: 0x000448A7 File Offset: 0x00042AA7
	// (set) Token: 0x06000F08 RID: 3848 RVA: 0x000448AF File Offset: 0x00042AAF
	public int number { get; set; }

	// Token: 0x17000144 RID: 324
	// (get) Token: 0x06000F09 RID: 3849 RVA: 0x000448B8 File Offset: 0x00042AB8
	// (set) Token: 0x06000F0A RID: 3850 RVA: 0x000448C0 File Offset: 0x00042AC0
	public double? usernameChangedAt { get; set; }

	// Token: 0x17000145 RID: 325
	// (get) Token: 0x06000F0B RID: 3851 RVA: 0x000448C9 File Offset: 0x00042AC9
	// (set) Token: 0x06000F0C RID: 3852 RVA: 0x000448D1 File Offset: 0x00042AD1
	public int patreonLevel { get; set; }

	// Token: 0x17000146 RID: 326
	// (get) Token: 0x06000F0D RID: 3853 RVA: 0x000448DA File Offset: 0x00042ADA
	// (set) Token: 0x06000F0E RID: 3854 RVA: 0x000448E2 File Offset: 0x00042AE2
	public int mmr { get; set; }

	// Token: 0x17000147 RID: 327
	// (get) Token: 0x06000F0F RID: 3855 RVA: 0x000448EB File Offset: 0x00042AEB
	// (set) Token: 0x06000F10 RID: 3856 RVA: 0x000448F3 File Offset: 0x00042AF3
	public int adminLevel { get; set; }

	// Token: 0x17000148 RID: 328
	// (get) Token: 0x06000F11 RID: 3857 RVA: 0x000448FC File Offset: 0x00042AFC
	// (set) Token: 0x06000F12 RID: 3858 RVA: 0x00044904 File Offset: 0x00042B04
	public PlayerItem[] items { get; set; }

	// Token: 0x17000149 RID: 329
	// (get) Token: 0x06000F13 RID: 3859 RVA: 0x0004490D File Offset: 0x00042B0D
	// (set) Token: 0x06000F14 RID: 3860 RVA: 0x00044915 File Offset: 0x00042B15
	public PlayerMute[] mutes { get; set; }

	// Token: 0x1700014A RID: 330
	// (get) Token: 0x06000F15 RID: 3861 RVA: 0x0004491E File Offset: 0x00042B1E
	// (set) Token: 0x06000F16 RID: 3862 RVA: 0x00044926 File Offset: 0x00042B26
	public PlayerBan[] bans { get; set; }

	// Token: 0x1700014B RID: 331
	// (get) Token: 0x06000F17 RID: 3863 RVA: 0x0004492F File Offset: 0x00042B2F
	// (set) Token: 0x06000F18 RID: 3864 RVA: 0x00044937 File Offset: 0x00042B37
	public PlayerCooldown[] cooldowns { get; set; }
}
