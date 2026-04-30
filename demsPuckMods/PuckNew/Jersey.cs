using System;
using UnityEngine;

// Token: 0x02000032 RID: 50
[Serializable]
public class Jersey
{
	// Token: 0x06000105 RID: 261 RVA: 0x000053F8 File Offset: 0x000035F8
	public bool IsForTeam(PlayerTeam team)
	{
		bool result;
		if (team != PlayerTeam.Blue)
		{
			if (team != PlayerTeam.Red)
			{
				result = ((this.Team & JerseyTeam.Any) > (JerseyTeam)0);
			}
			else
			{
				result = ((this.Team & JerseyTeam.Red) > (JerseyTeam)0);
			}
		}
		else
		{
			result = ((this.Team & JerseyTeam.Blue) > (JerseyTeam)0);
		}
		return result;
	}

	// Token: 0x040000BF RID: 191
	public int ID;

	// Token: 0x040000C0 RID: 192
	public JerseyTeam Team;

	// Token: 0x040000C1 RID: 193
	public Texture Texture;
}
