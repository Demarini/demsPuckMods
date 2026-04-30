using System;
using UnityEngine;

// Token: 0x02000019 RID: 25
public class MeshRendererTexturer : MonoBehaviour
{
	// Token: 0x17000004 RID: 4
	// (get) Token: 0x0600008D RID: 141 RVA: 0x00003A2D File Offset: 0x00001C2D
	public MeshRenderer MeshRenderer
	{
		get
		{
			if (!this.meshRenderer)
			{
				this.meshRenderer = base.GetComponent<MeshRenderer>();
			}
			return this.meshRenderer;
		}
	}

	// Token: 0x17000005 RID: 5
	// (get) Token: 0x0600008E RID: 142 RVA: 0x00003A50 File Offset: 0x00001C50
	public Material Material
	{
		get
		{
			if (!this.material)
			{
				if (Application.isPlaying)
				{
					this.material = this.MeshRenderer.material;
					this.MeshRenderer.material = this.material;
					this.isMaterialInstantiated = true;
				}
				else
				{
					this.material = this.MeshRenderer.sharedMaterial;
				}
			}
			return this.material;
		}
	}

	// Token: 0x0600008F RID: 143 RVA: 0x00003AB3 File Offset: 0x00001CB3
	private void OnDestroy()
	{
		if (this.isMaterialInstantiated)
		{
			Object.Destroy(this.Material);
		}
	}

	// Token: 0x06000090 RID: 144 RVA: 0x00003AC8 File Offset: 0x00001CC8
	public void SetTexture(Texture texture)
	{
		this.Material.mainTexture = texture;
	}

	// Token: 0x0400003E RID: 62
	[Header("References")]
	[SerializeField]
	private MeshRenderer meshRenderer;

	// Token: 0x0400003F RID: 63
	[SerializeField]
	private Material material;

	// Token: 0x04000040 RID: 64
	private bool isMaterialInstantiated;
}
