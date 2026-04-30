using System;
using UnityEngine;

// Token: 0x020000FA RID: 250
public struct ReplayPlayerBodyMove
{
	// Token: 0x04000446 RID: 1094
	public ulong OwnerClientId;

	// Token: 0x04000447 RID: 1095
	public Vector3 Position;

	// Token: 0x04000448 RID: 1096
	public Quaternion Rotation;

	// Token: 0x04000449 RID: 1097
	public float Stamina;

	// Token: 0x0400044A RID: 1098
	public float Speed;

	// Token: 0x0400044B RID: 1099
	public bool IsSprinting;

	// Token: 0x0400044C RID: 1100
	public bool IsSliding;

	// Token: 0x0400044D RID: 1101
	public bool IsStopping;

	// Token: 0x0400044E RID: 1102
	public bool IsExtendedLeft;

	// Token: 0x0400044F RID: 1103
	public bool IsExtendedRight;
}
