using System;
using UnityEngine;

// Token: 0x020000FC RID: 252
[Serializable]
internal struct AppearanceItem
{
	// Token: 0x04000532 RID: 1330
	public int Id;

	// Token: 0x04000533 RID: 1331
	public string Name;

	// Token: 0x04000534 RID: 1332
	public Texture Image;

	// Token: 0x04000535 RID: 1333
	public bool IsTwoTone;

	// Token: 0x04000536 RID: 1334
	public Texture BlueImage;

	// Token: 0x04000537 RID: 1335
	public Texture RedImage;

	// Token: 0x04000538 RID: 1336
	public bool Purchaseable;

	// Token: 0x04000539 RID: 1337
	public string Price;

	// Token: 0x0400053A RID: 1338
	public bool Hidden;
}
