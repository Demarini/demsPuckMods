using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000011 RID: 17
public class MeshRendererHider : MonoBehaviour
{
	// Token: 0x06000085 RID: 133 RVA: 0x0000713B File Offset: 0x0000533B
	private void Awake()
	{
		if (this.useChildrenMeshRenderers)
		{
			this.meshRenderers = new List<MeshRenderer>(base.GetComponentsInChildren<MeshRenderer>(true));
			this.meshRenderers.RemoveAll((MeshRenderer meshRenderer) => this.meshRendererBlacklist.Contains(meshRenderer));
		}
	}

	// Token: 0x06000086 RID: 134 RVA: 0x00010B7C File Offset: 0x0000ED7C
	public void HideMeshRenderers()
	{
		foreach (MeshRenderer meshRenderer in this.meshRenderers)
		{
			meshRenderer.enabled = false;
		}
	}

	// Token: 0x06000087 RID: 135 RVA: 0x00010BD0 File Offset: 0x0000EDD0
	public void ShowMeshRenderers()
	{
		foreach (MeshRenderer meshRenderer in this.meshRenderers)
		{
			meshRenderer.enabled = true;
		}
	}

	// Token: 0x0400003B RID: 59
	[Header("Settings")]
	[SerializeField]
	public List<MeshRenderer> meshRenderers;

	// Token: 0x0400003C RID: 60
	[SerializeField]
	public List<MeshRenderer> meshRendererBlacklist;

	// Token: 0x0400003D RID: 61
	[SerializeField]
	public bool useChildrenMeshRenderers = true;
}
