using System;
using Unity.Collections;
using UnityEngine;

// Token: 0x02000080 RID: 128
public struct ReplayPlayerBodySpawned
{
	// Token: 0x040001CA RID: 458
	public ulong OwnerClientId;

	// Token: 0x040001CB RID: 459
	public Vector3 Position;

	// Token: 0x040001CC RID: 460
	public Quaternion Rotation;

	// Token: 0x040001CD RID: 461
	public FixedString32Bytes Username;

	// Token: 0x040001CE RID: 462
	public int Number;

	// Token: 0x040001CF RID: 463
	public PlayerTeam Team;

	// Token: 0x040001D0 RID: 464
	public PlayerRole Role;

	// Token: 0x040001D1 RID: 465
	public FixedString32Bytes Country;

	// Token: 0x040001D2 RID: 466
	public FixedString32Bytes VisorAttackerBlueSkin;

	// Token: 0x040001D3 RID: 467
	public FixedString32Bytes VisorAttackerRedSkin;

	// Token: 0x040001D4 RID: 468
	public FixedString32Bytes VisorGoalieBlueSkin;

	// Token: 0x040001D5 RID: 469
	public FixedString32Bytes VisorGoalieRedSkin;

	// Token: 0x040001D6 RID: 470
	public FixedString32Bytes Mustache;

	// Token: 0x040001D7 RID: 471
	public FixedString32Bytes Beard;

	// Token: 0x040001D8 RID: 472
	public FixedString32Bytes JerseyAttackerBlueSkin;

	// Token: 0x040001D9 RID: 473
	public FixedString32Bytes JerseyAttackerRedSkin;

	// Token: 0x040001DA RID: 474
	public FixedString32Bytes JerseyGoalieBlueSkin;

	// Token: 0x040001DB RID: 475
	public FixedString32Bytes JerseyGoalieRedSkin;
}
