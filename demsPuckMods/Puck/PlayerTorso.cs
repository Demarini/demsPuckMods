using System;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

// Token: 0x020000DE RID: 222
[RequireComponent(typeof(MeshRendererTexturer))]
public class PlayerTorso : MonoBehaviour
{
	// Token: 0x1700009D RID: 157
	// (get) Token: 0x060006B9 RID: 1721 RVA: 0x0000B5F8 File Offset: 0x000097F8
	public string[] TextureNames
	{
		get
		{
			return this.textureMap.Keys.ToArray<string>();
		}
	}

	// Token: 0x060006BA RID: 1722 RVA: 0x0000B60A File Offset: 0x0000980A
	private void Awake()
	{
		this.meshRendererTexturer = base.GetComponent<MeshRendererTexturer>();
	}

	// Token: 0x060006BB RID: 1723 RVA: 0x0000B618 File Offset: 0x00009818
	public void SetTexture(string name)
	{
		if (!this.textureMap.ContainsKey(name))
		{
			return;
		}
		if (!this.meshRendererTexturer)
		{
			return;
		}
		this.meshRendererTexturer.SetTexture(this.textureMap[name]);
	}

	// Token: 0x060006BC RID: 1724 RVA: 0x0000B64E File Offset: 0x0000984E
	private void OnDestroy()
	{
		this.textureMap.Clear();
		this.textureMap = null;
	}

	// Token: 0x040003E3 RID: 995
	[Header("References")]
	[SerializeField]
	private SerializedDictionary<string, Texture> textureMap = new SerializedDictionary<string, Texture>();

	// Token: 0x040003E4 RID: 996
	private MeshRendererTexturer meshRendererTexturer;
}
