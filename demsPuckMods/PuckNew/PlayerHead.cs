using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000028 RID: 40
[ExecuteInEditMode]
public class PlayerHead : MonoBehaviour
{
	// Token: 0x060000D6 RID: 214 RVA: 0x00004B0C File Offset: 0x00002D0C
	public void SetFlagID(int flagID)
	{
		this.headgear.ForEach(delegate(Headgear h)
		{
			if (h.FlagGameObject != null)
			{
				h.FlagGameObject.SetActive(false);
			}
		});
		if (flagID == -1)
		{
			return;
		}
		Flag flag = this.flags.FirstOrDefault((Flag f) => f.ID == flagID);
		if (flag == null)
		{
			Debug.LogWarning(string.Format("[PlayerHead] Tried to set invalid flagID {0}", flagID));
			return;
		}
		this.headgear.ForEach(delegate(Headgear h)
		{
			if (h.FlagGameObject != null)
			{
				h.FlagGameObject.SetActive(true);
				if (h.FlagMeshRendererTexturer != null)
				{
					h.FlagMeshRendererTexturer.SetTexture(flag.Texture);
				}
			}
		});
	}

	// Token: 0x060000D7 RID: 215 RVA: 0x00004BB4 File Offset: 0x00002DB4
	public void SetHeadgearID(int headgearID, PlayerRole role)
	{
		this.headgear.ForEach(delegate(Headgear h)
		{
			h.GameObject.SetActive(false);
		});
		if (headgearID == -1)
		{
			return;
		}
		Headgear headgear = this.headgear.FirstOrDefault((Headgear h) => h.ID == headgearID && h.IsForRole(role));
		if (headgear == null)
		{
			Debug.LogWarning(string.Format("[PlayerHead] Tried to set invalid headgearID {0} for role {1}", headgearID, role));
			return;
		}
		headgear.GameObject.SetActive(true);
	}

	// Token: 0x060000D8 RID: 216 RVA: 0x00004C58 File Offset: 0x00002E58
	public void SetMustacheID(int mustacheID)
	{
		this.mustaches.ForEach(delegate(Mustache m)
		{
			m.GameObject.SetActive(false);
		});
		if (mustacheID == -1)
		{
			return;
		}
		Mustache mustache = this.mustaches.FirstOrDefault((Mustache m) => m.ID == mustacheID);
		if (mustache == null)
		{
			Debug.LogWarning(string.Format("[PlayerHead] Tried to set invalid mustacheID {0}", mustacheID));
			return;
		}
		mustache.GameObject.SetActive(true);
	}

	// Token: 0x060000D9 RID: 217 RVA: 0x00004CE8 File Offset: 0x00002EE8
	public void SetBeardID(int beardID)
	{
		this.beards.ForEach(delegate(Beard b)
		{
			b.GameObject.SetActive(false);
		});
		if (beardID == -1)
		{
			return;
		}
		Beard beard = this.beards.FirstOrDefault((Beard b) => b.ID == beardID);
		if (beard == null)
		{
			Debug.LogWarning(string.Format("[PlayerHead] Tried to set invalid beardID {0}", beardID));
			return;
		}
		beard.GameObject.SetActive(true);
	}

	// Token: 0x04000091 RID: 145
	[Header("References")]
	[SerializeField]
	private List<Flag> flags = new List<Flag>();

	// Token: 0x04000092 RID: 146
	[SerializeField]
	private List<Headgear> headgear = new List<Headgear>();

	// Token: 0x04000093 RID: 147
	[SerializeField]
	private List<Mustache> mustaches = new List<Mustache>();

	// Token: 0x04000094 RID: 148
	[SerializeField]
	private List<Beard> beards = new List<Beard>();
}
