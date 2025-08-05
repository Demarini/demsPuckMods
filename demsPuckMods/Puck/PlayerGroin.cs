using System;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

// Token: 0x020000D8 RID: 216
[RequireComponent(typeof(MeshRendererTexturer))]
public class PlayerGroin : MonoBehaviour
{
	// Token: 0x17000094 RID: 148
	// (get) Token: 0x06000688 RID: 1672 RVA: 0x0000B2FB File Offset: 0x000094FB
	public string[] TextureNames
	{
		get
		{
			return this.textureMap.Keys.ToArray<string>();
		}
	}

	// Token: 0x06000689 RID: 1673 RVA: 0x0000B30D File Offset: 0x0000950D
	private void Awake()
	{
		this.meshRendererTexturer = base.GetComponent<MeshRendererTexturer>();
	}

	// Token: 0x0600068A RID: 1674 RVA: 0x0000B31B File Offset: 0x0000951B
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

	// Token: 0x0600068B RID: 1675 RVA: 0x0000B351 File Offset: 0x00009551
	private void OnDestroy()
	{
		this.textureMap.Clear();
		this.textureMap = null;
	}

	// Token: 0x040003B2 RID: 946
	[Header("References")]
	[SerializeField]
	private SerializedDictionary<string, Texture> textureMap = new SerializedDictionary<string, Texture>();

	// Token: 0x040003B3 RID: 947
	private MeshRendererTexturer meshRendererTexturer;
}
