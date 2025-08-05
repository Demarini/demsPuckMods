using System;
using UnityEngine;

// Token: 0x02000012 RID: 18
public class MeshRendererTexturer : MonoBehaviour
{
	// Token: 0x0600008A RID: 138 RVA: 0x0000718C File Offset: 0x0000538C
	private void Awake()
	{
		if (!this.meshRenderer)
		{
			this.meshRenderer = base.GetComponent<MeshRenderer>();
		}
		this.material = this.meshRenderer.material;
	}

	// Token: 0x0600008B RID: 139 RVA: 0x000071B8 File Offset: 0x000053B8
	private void OnDestroy()
	{
		UnityEngine.Object.Destroy(this.material);
	}

	// Token: 0x0600008C RID: 140 RVA: 0x000071C5 File Offset: 0x000053C5
	public void SetTexture(Texture texture)
	{
		this.material.SetTexture("_BaseMap", texture);
	}

	// Token: 0x0400003E RID: 62
	[Header("References")]
	[SerializeField]
	private MeshRenderer meshRenderer;

	// Token: 0x0400003F RID: 63
	private Material material;
}
