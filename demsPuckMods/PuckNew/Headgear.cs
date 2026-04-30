using System;
using UnityEngine;

// Token: 0x02000025 RID: 37
[Serializable]
public class Headgear
{
	// Token: 0x17000012 RID: 18
	// (get) Token: 0x060000D1 RID: 209 RVA: 0x00004AAD File Offset: 0x00002CAD
	public MeshRendererTexturer FlagMeshRendererTexturer
	{
		get
		{
			if (!this.FlagGameObject)
			{
				return null;
			}
			return this.FlagGameObject.GetComponent<MeshRendererTexturer>();
		}
	}

	// Token: 0x060000D2 RID: 210 RVA: 0x00004ACC File Offset: 0x00002CCC
	public bool IsForRole(PlayerRole role)
	{
		bool result;
		if (role != PlayerRole.Attacker)
		{
			if (role != PlayerRole.Goalie)
			{
				result = ((this.Role & HeadgearRole.Any) > (HeadgearRole)0);
			}
			else
			{
				result = ((this.Role & HeadgearRole.Goalie) > (HeadgearRole)0);
			}
		}
		else
		{
			result = ((this.Role & HeadgearRole.Attacker) > (HeadgearRole)0);
		}
		return result;
	}

	// Token: 0x04000089 RID: 137
	public int ID;

	// Token: 0x0400008A RID: 138
	public GameObject GameObject;

	// Token: 0x0400008B RID: 139
	public GameObject FlagGameObject;

	// Token: 0x0400008C RID: 140
	public HeadgearRole Role;
}
