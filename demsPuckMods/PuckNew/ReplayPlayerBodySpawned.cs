using System;
using Unity.Collections;
using UnityEngine;

// Token: 0x020000F9 RID: 249
public struct ReplayPlayerBodySpawned
{
	// Token: 0x0400043F RID: 1087
	public ulong OwnerClientId;

	// Token: 0x04000440 RID: 1088
	public Vector3 Position;

	// Token: 0x04000441 RID: 1089
	public Quaternion Rotation;

	// Token: 0x04000442 RID: 1090
	public PlayerGameState GameState;

	// Token: 0x04000443 RID: 1091
	public PlayerCustomizationState CustomizationState;

	// Token: 0x04000444 RID: 1092
	public FixedString32Bytes Username;

	// Token: 0x04000445 RID: 1093
	public int Number;
}
