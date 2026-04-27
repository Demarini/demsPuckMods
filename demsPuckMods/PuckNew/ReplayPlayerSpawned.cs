using System;
using Unity.Collections;

// Token: 0x020000F6 RID: 246
public struct ReplayPlayerSpawned
{
	// Token: 0x0400042F RID: 1071
	public ulong OwnerClientId;

	// Token: 0x04000430 RID: 1072
	public PlayerGameState GameState;

	// Token: 0x04000431 RID: 1073
	public PlayerCustomizationState CustomizationState;

	// Token: 0x04000432 RID: 1074
	public PlayerHandedness Handedness;

	// Token: 0x04000433 RID: 1075
	public FixedString32Bytes SteamId;

	// Token: 0x04000434 RID: 1076
	public FixedString32Bytes Username;

	// Token: 0x04000435 RID: 1077
	public int Number;

	// Token: 0x04000436 RID: 1078
	public int PatreonLevel;

	// Token: 0x04000437 RID: 1079
	public int AdminLevel;

	// Token: 0x04000438 RID: 1080
	public bool IsMuted;
}
