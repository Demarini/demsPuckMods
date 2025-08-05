using System;
using Unity.Collections;
using UnityEngine;

// Token: 0x02000083 RID: 131
public struct ReplayStickSpawned
{
	// Token: 0x040001E7 RID: 487
	public ulong OwnerClientId;

	// Token: 0x040001E8 RID: 488
	public Vector3 Position;

	// Token: 0x040001E9 RID: 489
	public Quaternion Rotation;

	// Token: 0x040001EA RID: 490
	public FixedString32Bytes StickAttackerBlueSkin;

	// Token: 0x040001EB RID: 491
	public FixedString32Bytes StickAttackerRedSkin;

	// Token: 0x040001EC RID: 492
	public FixedString32Bytes StickGoalieBlueSkin;

	// Token: 0x040001ED RID: 493
	public FixedString32Bytes StickGoalieRedSkin;

	// Token: 0x040001EE RID: 494
	public FixedString32Bytes StickShaftAttackerBlueTapeSkin;

	// Token: 0x040001EF RID: 495
	public FixedString32Bytes StickShaftAttackerRedTapeSkin;

	// Token: 0x040001F0 RID: 496
	public FixedString32Bytes StickShaftGoalieBlueTapeSkin;

	// Token: 0x040001F1 RID: 497
	public FixedString32Bytes StickShaftGoalieRedTapeSkin;

	// Token: 0x040001F2 RID: 498
	public FixedString32Bytes StickBladeAttackerBlueTapeSkin;

	// Token: 0x040001F3 RID: 499
	public FixedString32Bytes StickBladeAttackerRedTapeSkin;

	// Token: 0x040001F4 RID: 500
	public FixedString32Bytes StickBladeGoalieBlueTapeSkin;

	// Token: 0x040001F5 RID: 501
	public FixedString32Bytes StickBladeGoalieRedTapeSkin;
}
