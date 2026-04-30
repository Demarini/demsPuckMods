using System;
using System.Collections.Generic;

// Token: 0x02000079 RID: 121
public class StandardGameModeConfig : BaseGameModeConfig
{
	// Token: 0x1700006D RID: 109
	// (get) Token: 0x0600041A RID: 1050 RVA: 0x00016BE2 File Offset: 0x00014DE2
	// (set) Token: 0x0600041B RID: 1051 RVA: 0x00016BEA File Offset: 0x00014DEA
	public Dictionary<GamePhase, int> phaseDurationMap { get; set; } = new Dictionary<GamePhase, int>
	{
		{
			GamePhase.None,
			0
		},
		{
			GamePhase.Warmup,
			60
		},
		{
			GamePhase.PreGame,
			10
		},
		{
			GamePhase.FaceOff,
			5
		},
		{
			GamePhase.Play,
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
			GamePhase.Intermission,
			10
		},
		{
			GamePhase.GameOver,
			30
		},
		{
			GamePhase.PostGame,
			10
		}
	};

	// Token: 0x1700006E RID: 110
	// (get) Token: 0x0600041C RID: 1052 RVA: 0x00016BF3 File Offset: 0x00014DF3
	// (set) Token: 0x0600041D RID: 1053 RVA: 0x00016BFB File Offset: 0x00014DFB
	public float spawnDelay { get; set; } = 5f;

	// Token: 0x1700006F RID: 111
	// (get) Token: 0x0600041E RID: 1054 RVA: 0x00016C04 File Offset: 0x00014E04
	// (set) Token: 0x0600041F RID: 1055 RVA: 0x00016C0C File Offset: 0x00014E0C
	public int maxPeriods { get; set; } = 3;
}
