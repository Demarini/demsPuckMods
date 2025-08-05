using System;
using System.Collections.Generic;

// Token: 0x02000099 RID: 153
public class ServerConfiguration
{
	// Token: 0x1700005B RID: 91
	// (get) Token: 0x060003D8 RID: 984 RVA: 0x00009674 File Offset: 0x00007874
	// (set) Token: 0x060003D9 RID: 985 RVA: 0x0000967C File Offset: 0x0000787C
	public ushort port { get; set; } = 7777;

	// Token: 0x1700005C RID: 92
	// (get) Token: 0x060003DA RID: 986 RVA: 0x00009685 File Offset: 0x00007885
	// (set) Token: 0x060003DB RID: 987 RVA: 0x0000968D File Offset: 0x0000788D
	public ushort pingPort { get; set; } = 7778;

	// Token: 0x1700005D RID: 93
	// (get) Token: 0x060003DC RID: 988 RVA: 0x00009696 File Offset: 0x00007896
	// (set) Token: 0x060003DD RID: 989 RVA: 0x0000969E File Offset: 0x0000789E
	public string name { get; set; } = "MY PUCK SERVER";

	// Token: 0x1700005E RID: 94
	// (get) Token: 0x060003DE RID: 990 RVA: 0x000096A7 File Offset: 0x000078A7
	// (set) Token: 0x060003DF RID: 991 RVA: 0x000096AF File Offset: 0x000078AF
	public int maxPlayers { get; set; } = 10;

	// Token: 0x1700005F RID: 95
	// (get) Token: 0x060003E0 RID: 992 RVA: 0x000096B8 File Offset: 0x000078B8
	// (set) Token: 0x060003E1 RID: 993 RVA: 0x000096C0 File Offset: 0x000078C0
	public string password { get; set; } = "";

	// Token: 0x17000060 RID: 96
	// (get) Token: 0x060003E2 RID: 994 RVA: 0x000096C9 File Offset: 0x000078C9
	// (set) Token: 0x060003E3 RID: 995 RVA: 0x000096D1 File Offset: 0x000078D1
	public bool voip { get; set; }

	// Token: 0x17000061 RID: 97
	// (get) Token: 0x060003E4 RID: 996 RVA: 0x000096DA File Offset: 0x000078DA
	// (set) Token: 0x060003E5 RID: 997 RVA: 0x000096E2 File Offset: 0x000078E2
	public bool isPublic { get; set; } = true;

	// Token: 0x17000062 RID: 98
	// (get) Token: 0x060003E6 RID: 998 RVA: 0x000096EB File Offset: 0x000078EB
	// (set) Token: 0x060003E7 RID: 999 RVA: 0x000096F3 File Offset: 0x000078F3
	public string[] adminSteamIds { get; set; } = new string[0];

	// Token: 0x17000063 RID: 99
	// (get) Token: 0x060003E8 RID: 1000 RVA: 0x000096FC File Offset: 0x000078FC
	// (set) Token: 0x060003E9 RID: 1001 RVA: 0x00009704 File Offset: 0x00007904
	public bool reloadBannedSteamIds { get; set; }

	// Token: 0x17000064 RID: 100
	// (get) Token: 0x060003EA RID: 1002 RVA: 0x0000970D File Offset: 0x0000790D
	// (set) Token: 0x060003EB RID: 1003 RVA: 0x00009715 File Offset: 0x00007915
	public bool usePuckBannedSteamIds { get; set; } = true;

	// Token: 0x17000065 RID: 101
	// (get) Token: 0x060003EC RID: 1004 RVA: 0x0000971E File Offset: 0x0000791E
	// (set) Token: 0x060003ED RID: 1005 RVA: 0x00009726 File Offset: 0x00007926
	public bool printMetrics { get; set; } = true;

	// Token: 0x17000066 RID: 102
	// (get) Token: 0x060003EE RID: 1006 RVA: 0x0000972F File Offset: 0x0000792F
	// (set) Token: 0x060003EF RID: 1007 RVA: 0x00009737 File Offset: 0x00007937
	public float kickTimeout { get; set; } = 300f;

	// Token: 0x17000067 RID: 103
	// (get) Token: 0x060003F0 RID: 1008 RVA: 0x00009740 File Offset: 0x00007940
	// (set) Token: 0x060003F1 RID: 1009 RVA: 0x00009748 File Offset: 0x00007948
	public float sleepTimeout { get; set; } = 60f;

	// Token: 0x17000068 RID: 104
	// (get) Token: 0x060003F2 RID: 1010 RVA: 0x00009751 File Offset: 0x00007951
	// (set) Token: 0x060003F3 RID: 1011 RVA: 0x00009759 File Offset: 0x00007959
	public float joinMidMatchDelay { get; set; } = 10f;

	// Token: 0x17000069 RID: 105
	// (get) Token: 0x060003F4 RID: 1012 RVA: 0x00009762 File Offset: 0x00007962
	// (set) Token: 0x060003F5 RID: 1013 RVA: 0x0000976A File Offset: 0x0000796A
	public int targetFrameRate { get; set; } = 120;

	// Token: 0x1700006A RID: 106
	// (get) Token: 0x060003F6 RID: 1014 RVA: 0x00009773 File Offset: 0x00007973
	// (set) Token: 0x060003F7 RID: 1015 RVA: 0x0000977B File Offset: 0x0000797B
	public int serverTickRate { get; set; } = 100;

	// Token: 0x1700006B RID: 107
	// (get) Token: 0x060003F8 RID: 1016 RVA: 0x00009784 File Offset: 0x00007984
	// (set) Token: 0x060003F9 RID: 1017 RVA: 0x0000978C File Offset: 0x0000798C
	public int clientTickRate { get; set; } = 200;

	// Token: 0x1700006C RID: 108
	// (get) Token: 0x060003FA RID: 1018 RVA: 0x00009795 File Offset: 0x00007995
	// (set) Token: 0x060003FB RID: 1019 RVA: 0x0000979D File Offset: 0x0000799D
	public bool startPaused { get; set; }

	// Token: 0x1700006D RID: 109
	// (get) Token: 0x060003FC RID: 1020 RVA: 0x000097A6 File Offset: 0x000079A6
	// (set) Token: 0x060003FD RID: 1021 RVA: 0x000097AE File Offset: 0x000079AE
	public bool allowVoting { get; set; } = true;

	// Token: 0x1700006E RID: 110
	// (get) Token: 0x060003FE RID: 1022 RVA: 0x000097B7 File Offset: 0x000079B7
	// (set) Token: 0x060003FF RID: 1023 RVA: 0x000097BF File Offset: 0x000079BF
	public Dictionary<GamePhase, int> phaseDurationMap { get; set; } = new Dictionary<GamePhase, int>
	{
		{
			GamePhase.Warmup,
			600
		},
		{
			GamePhase.FaceOff,
			3
		},
		{
			GamePhase.Playing,
			300
		},
		{
			GamePhase.BlueScore,
			5
		},
		{
			GamePhase.RedScore,
			5
		},
		{
			GamePhase.Replay,
			10
		},
		{
			GamePhase.PeriodOver,
			15
		},
		{
			GamePhase.GameOver,
			15
		}
	};

	// Token: 0x1700006F RID: 111
	// (get) Token: 0x06000400 RID: 1024 RVA: 0x000097C8 File Offset: 0x000079C8
	// (set) Token: 0x06000401 RID: 1025 RVA: 0x000097D0 File Offset: 0x000079D0
	public ModConfiguration[] mods { get; set; } = new ModConfiguration[0];
}
