using System;
using AYellowpaper.SerializedCollections;
using UnityEngine;

// Token: 0x020000F3 RID: 243
public class StickMesh : MonoBehaviour
{
	// Token: 0x170000BB RID: 187
	// (get) Token: 0x0600083A RID: 2106 RVA: 0x0000C002 File Offset: 0x0000A202
	[HideInInspector]
	public Collider ShaftCollider
	{
		get
		{
			return this.shaftCollider;
		}
	}

	// Token: 0x170000BC RID: 188
	// (get) Token: 0x0600083B RID: 2107 RVA: 0x0000C00A File Offset: 0x0000A20A
	[HideInInspector]
	public Collider BladeCollider
	{
		get
		{
			return this.bladeCollider;
		}
	}

	// Token: 0x0600083C RID: 2108 RVA: 0x00033E44 File Offset: 0x00032044
	public void SetSkin(PlayerTeam team, string skinName)
	{
		string key = ((team == PlayerTeam.Blue) ? "blue_" : "red_") + skinName;
		if (this.stickMaterialMap == null || !this.stickMaterialMap.ContainsKey(key))
		{
			return;
		}
		if (!this.stickMeshRenderer)
		{
			return;
		}
		UnityEngine.Object.Destroy(this.stickMeshRenderer.material);
		this.stickMeshRenderer.material = this.stickMaterialMap[key];
	}

	// Token: 0x0600083D RID: 2109 RVA: 0x00033EB4 File Offset: 0x000320B4
	public void SetShaftTape(string tapeSkinName)
	{
		if (this.shaftTapeMaterialMap == null || !this.shaftTapeMaterialMap.ContainsKey(tapeSkinName))
		{
			return;
		}
		if (!this.shaftTapeMeshRenderer)
		{
			return;
		}
		if (!this.shaftTapeGameObject)
		{
			return;
		}
		if (this.shaftTapeMaterialMap[tapeSkinName] == null)
		{
			this.shaftTapeGameObject.SetActive(false);
			return;
		}
		this.shaftTapeGameObject.SetActive(true);
		UnityEngine.Object.Destroy(this.shaftTapeMeshRenderer.material);
		this.shaftTapeMeshRenderer.material = this.shaftTapeMaterialMap[tapeSkinName];
	}

	// Token: 0x0600083E RID: 2110 RVA: 0x00033F48 File Offset: 0x00032148
	public void SetBladeTape(string tapeSkinName)
	{
		if (this.bladeTapeMaterialMap == null || !this.bladeTapeMaterialMap.ContainsKey(tapeSkinName))
		{
			return;
		}
		if (!this.bladeTapeMeshRenderer)
		{
			return;
		}
		if (!this.bladeTapeGameObject)
		{
			return;
		}
		if (this.bladeTapeMaterialMap[tapeSkinName] == null)
		{
			this.bladeTapeGameObject.SetActive(false);
			return;
		}
		this.bladeTapeGameObject.SetActive(true);
		UnityEngine.Object.Destroy(this.bladeTapeMeshRenderer.material);
		this.bladeTapeMeshRenderer.material = this.bladeTapeMaterialMap[tapeSkinName];
	}

	// Token: 0x0600083F RID: 2111 RVA: 0x0000C012 File Offset: 0x0000A212
	private void OnDestroy()
	{
		this.stickMaterialMap.Clear();
		this.bladeTapeMaterialMap.Clear();
		this.shaftTapeMaterialMap.Clear();
		this.stickMaterialMap = null;
		this.bladeTapeMaterialMap = null;
		this.shaftTapeMaterialMap = null;
	}

	// Token: 0x040004CC RID: 1228
	[Header("References")]
	[SerializeField]
	private MeshRenderer stickMeshRenderer;

	// Token: 0x040004CD RID: 1229
	[SerializeField]
	private GameObject shaftTapeGameObject;

	// Token: 0x040004CE RID: 1230
	[SerializeField]
	private MeshRenderer shaftTapeMeshRenderer;

	// Token: 0x040004CF RID: 1231
	[SerializeField]
	private GameObject bladeTapeGameObject;

	// Token: 0x040004D0 RID: 1232
	[SerializeField]
	private MeshRenderer bladeTapeMeshRenderer;

	// Token: 0x040004D1 RID: 1233
	[Space(20f)]
	[SerializeField]
	private Collider shaftCollider;

	// Token: 0x040004D2 RID: 1234
	[SerializeField]
	private Collider bladeCollider;

	// Token: 0x040004D3 RID: 1235
	[Header("Settings")]
	[SerializeField]
	private SerializedDictionary<string, Material> stickMaterialMap = new SerializedDictionary<string, Material>();

	// Token: 0x040004D4 RID: 1236
	[SerializeField]
	private SerializedDictionary<string, Material> bladeTapeMaterialMap = new SerializedDictionary<string, Material>();

	// Token: 0x040004D5 RID: 1237
	[SerializeField]
	private SerializedDictionary<string, Material> shaftTapeMaterialMap = new SerializedDictionary<string, Material>();
}
