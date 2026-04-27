using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000018 RID: 24
public class MeshRendererHider : MonoBehaviour
{
	// Token: 0x06000088 RID: 136 RVA: 0x00003932 File Offset: 0x00001B32
	private void Awake()
	{
		if (this.useChildrenMeshRenderers)
		{
			this.meshRenderers = new List<MeshRenderer>(base.GetComponentsInChildren<MeshRenderer>(true));
			this.meshRenderers.RemoveAll((MeshRenderer meshRenderer) => this.meshRendererBlacklist.Contains(meshRenderer));
		}
	}

	// Token: 0x06000089 RID: 137 RVA: 0x00003968 File Offset: 0x00001B68
	public void HideMeshRenderers()
	{
		foreach (MeshRenderer meshRenderer in this.meshRenderers)
		{
			meshRenderer.enabled = false;
		}
	}

	// Token: 0x0600008A RID: 138 RVA: 0x000039BC File Offset: 0x00001BBC
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
