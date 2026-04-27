using System;
using UnityEngine;

// Token: 0x0200004A RID: 74
[Serializable]
public class StickSkin
{
	// Token: 0x06000282 RID: 642 RVA: 0x00010A40 File Offset: 0x0000EC40
	public bool IsForTeam(PlayerTeam team)
	{
		bool result;
		if (team != PlayerTeam.Blue)
		{
			if (team != PlayerTeam.Red)
			{
				result = ((this.Team & StickSkinTeam.Any) > (StickSkinTeam)0);
			}
			else
			{
				result = ((this.Team & StickSkinTeam.Red) > (StickSkinTeam)0);
			}
		}
		else
		{
			result = ((this.Team & StickSkinTeam.Blue) > (StickSkinTeam)0);
		}
		return result;
	}

	// Token: 0x040001A7 RID: 423
	public int ID;

	// Token: 0x040001A8 RID: 424
	public StickSkinTeam Team;

	// Token: 0x040001A9 RID: 425
	public Material Material;
}
