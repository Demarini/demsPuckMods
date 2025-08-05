using System;
using UnityEngine;

// Token: 0x02000081 RID: 129
public struct ReplayPlayerBodyMove
{
	// Token: 0x040001DC RID: 476
	public ulong OwnerClientId;

	// Token: 0x040001DD RID: 477
	public Vector3 Position;

	// Token: 0x040001DE RID: 478
	public Quaternion Rotation;

	// Token: 0x040001DF RID: 479
	public short Stamina;

	// Token: 0x040001E0 RID: 480
	public short Speed;

	// Token: 0x040001E1 RID: 481
	public bool IsSprinting;

	// Token: 0x040001E2 RID: 482
	public bool IsSliding;

	// Token: 0x040001E3 RID: 483
	public bool IsStopping;

	// Token: 0x040001E4 RID: 484
	public bool IsExtendedLeft;

	// Token: 0x040001E5 RID: 485
	public bool IsExtendedRight;
}
