using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200004C RID: 76
public class StickMesh : MonoBehaviour
{
	// Token: 0x17000049 RID: 73
	// (get) Token: 0x06000285 RID: 645 RVA: 0x00010A80 File Offset: 0x0000EC80
	[HideInInspector]
	public Collider ShaftCollider
	{
		get
		{
			return this.shaftCollider;
		}
	}

	// Token: 0x1700004A RID: 74
	// (get) Token: 0x06000286 RID: 646 RVA: 0x00010A88 File Offset: 0x0000EC88
	[HideInInspector]
	public Collider BladeCollider
	{
		get
		{
			return this.bladeCollider;
		}
	}

	// Token: 0x06000287 RID: 647 RVA: 0x00010A90 File Offset: 0x0000EC90
	private void OnDestroy()
	{
		Object.Destroy(this.stickMeshRenderer.material);
		Object.Destroy(this.shaftTapeMeshRenderer.material);
		Object.Destroy(this.bladeTapeMeshRenderer.material);
	}

	// Token: 0x06000288 RID: 648 RVA: 0x00010AC4 File Offset: 0x0000ECC4
	public void SetSkinID(int skinID, PlayerTeam team)
	{
		StickSkin stickSkin = this.skins.Find((StickSkin s) => s.ID == skinID && s.IsForTeam(team));
		if (stickSkin == null)
		{
			Debug.LogWarning(string.Format("[StickMesh] Tried to set invalid skinID {0}", skinID));
			return;
		}
		Object.Destroy(this.stickMeshRenderer.material);
		this.stickMeshRenderer.material = new Material(stickSkin.Material);
	}

	// Token: 0x06000289 RID: 649 RVA: 0x00010B44 File Offset: 0x0000ED44
	public void SetShaftTapeID(int shaftTapeID)
	{
		if (shaftTapeID == -1)
		{
			this.shaftTapeGameObject.SetActive(false);
			return;
		}
		StickTape stickTape = this.shaftTapes.Find((StickTape t) => t.ID == shaftTapeID && t.Material != null);
		if (stickTape == null)
		{
			Debug.LogWarning(string.Format("[StickMesh] Tried to set invalid shaftTapeID {0}", shaftTapeID));
			return;
		}
		this.shaftTapeGameObject.SetActive(true);
		Object.Destroy(this.shaftTapeMeshRenderer.material);
		this.shaftTapeMeshRenderer.material = new Material(stickTape.Material);
	}

	// Token: 0x0600028A RID: 650 RVA: 0x00010BDC File Offset: 0x0000EDDC
	public void SetBladeTapeID(int bladeTapeID)
	{
		if (bladeTapeID == -1)
		{
			this.bladeTapeGameObject.SetActive(false);
			return;
		}
		StickTape stickTape = this.bladeTapes.Find((StickTape t) => t.ID == bladeTapeID && t.Material != null);
		if (stickTape == null)
		{
			Debug.LogWarning(string.Format("[StickMesh] Tried to set invalid bladeTapeID {0}", bladeTapeID));
			return;
		}
		this.bladeTapeGameObject.SetActive(true);
		Object.Destroy(this.bladeTapeMeshRenderer.material);
		this.bladeTapeMeshRenderer.material = new Material(stickTape.Material);
	}

	// Token: 0x040001AC RID: 428
	[Header("Settings")]
	[SerializeField]
	private List<StickSkin> skins = new List<StickSkin>();

	// Token: 0x040001AD RID: 429
	[SerializeField]
	private List<StickTape> shaftTapes = new List<StickTape>();

	// Token: 0x040001AE RID: 430
	[SerializeField]
	private List<StickTape> bladeTapes = new List<StickTape>();

	// Token: 0x040001AF RID: 431
	[Header("References")]
	[SerializeField]
	private MeshRenderer stickMeshRenderer;

	// Token: 0x040001B0 RID: 432
	[SerializeField]
	private GameObject shaftTapeGameObject;

	// Token: 0x040001B1 RID: 433
	[SerializeField]
	private MeshRenderer shaftTapeMeshRenderer;

	// Token: 0x040001B2 RID: 434
	[SerializeField]
	private GameObject bladeTapeGameObject;

	// Token: 0x040001B3 RID: 435
	[SerializeField]
	private MeshRenderer bladeTapeMeshRenderer;

	// Token: 0x040001B4 RID: 436
	[Space(20f)]
	[SerializeField]
	private Collider shaftCollider;

	// Token: 0x040001B5 RID: 437
	[SerializeField]
	private Collider bladeCollider;
}
