using System;

// Token: 0x0200011E RID: 286
public class ServerConfig
{
	// Token: 0x170000DE RID: 222
	// (get) Token: 0x060007DA RID: 2010 RVA: 0x00026126 File Offset: 0x00024326
	// (set) Token: 0x060007DB RID: 2011 RVA: 0x0002612E File Offset: 0x0002432E
	public ushort port { get; set; } = 30609;

	// Token: 0x170000DF RID: 223
	// (get) Token: 0x060007DC RID: 2012 RVA: 0x00026137 File Offset: 0x00024337
	// (set) Token: 0x060007DD RID: 2013 RVA: 0x0002613F File Offset: 0x0002433F
	public string name { get; set; } = "MY PUCK SERVER";

	// Token: 0x170000E0 RID: 224
	// (get) Token: 0x060007DE RID: 2014 RVA: 0x00026148 File Offset: 0x00024348
	// (set) Token: 0x060007DF RID: 2015 RVA: 0x00026150 File Offset: 0x00024350
	public int maxPlayers { get; set; } = 12;

	// Token: 0x170000E1 RID: 225
	// (get) Token: 0x060007E0 RID: 2016 RVA: 0x00026159 File Offset: 0x00024359
	// (set) Token: 0x060007E1 RID: 2017 RVA: 0x00026161 File Offset: 0x00024361
	public string password { get; set; }

	// Token: 0x170000E2 RID: 226
	// (get) Token: 0x060007E2 RID: 2018 RVA: 0x0002616A File Offset: 0x0002436A
	// (set) Token: 0x060007E3 RID: 2019 RVA: 0x00026172 File Offset: 0x00024372
	public int tickRate { get; set; } = 200;

	// Token: 0x170000E3 RID: 227
	// (get) Token: 0x060007E4 RID: 2020 RVA: 0x0002617B File Offset: 0x0002437B
	// (set) Token: 0x060007E5 RID: 2021 RVA: 0x00026183 File Offset: 0x00024383
	public bool isPublic { get; set; } = true;

	// Token: 0x170000E4 RID: 228
	// (get) Token: 0x060007E6 RID: 2022 RVA: 0x0002618C File Offset: 0x0002438C
	// (set) Token: 0x060007E7 RID: 2023 RVA: 0x00026194 File Offset: 0x00024394
	public bool useVoip { get; set; }

	// Token: 0x170000E5 RID: 229
	// (get) Token: 0x060007E8 RID: 2024 RVA: 0x0002619D File Offset: 0x0002439D
	// (set) Token: 0x060007E9 RID: 2025 RVA: 0x000261A5 File Offset: 0x000243A5
	public bool useWhitelist { get; set; }

	// Token: 0x170000E6 RID: 230
	// (get) Token: 0x060007EA RID: 2026 RVA: 0x000261AE File Offset: 0x000243AE
	// (set) Token: 0x060007EB RID: 2027 RVA: 0x000261B6 File Offset: 0x000243B6
	public ModConfig[] mods { get; set; } = Constants.DEFAULT_SERVER_MODS;

	// Token: 0x170000E7 RID: 231
	// (get) Token: 0x060007EC RID: 2028 RVA: 0x000261BF File Offset: 0x000243BF
	// (set) Token: 0x060007ED RID: 2029 RVA: 0x000261C7 File Offset: 0x000243C7
	public string gameMode { get; set; } = "public";

	// Token: 0x170000E8 RID: 232
	// (get) Token: 0x060007EE RID: 2030 RVA: 0x000261D0 File Offset: 0x000243D0
	// (set) Token: 0x060007EF RID: 2031 RVA: 0x000261D8 File Offset: 0x000243D8
	public string level { get; set; } = "default";
}
