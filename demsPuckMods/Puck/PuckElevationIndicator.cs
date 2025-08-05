using System;
using UnityEngine;

// Token: 0x02000019 RID: 25
public class PuckElevationIndicator : MonoBehaviour
{
	// Token: 0x1700000B RID: 11
	// (get) Token: 0x060000BA RID: 186 RVA: 0x000073F0 File Offset: 0x000055F0
	// (set) Token: 0x060000BB RID: 187 RVA: 0x000073F8 File Offset: 0x000055F8
	[HideInInspector]
	public bool IsVisible
	{
		get
		{
			return this.isVisible;
		}
		set
		{
			this.isVisible = value;
			this.planeMeshRenderer.enabled = this.isVisible;
			this.lineRenderer.enabled = this.isVisible;
		}
	}

	// Token: 0x060000BC RID: 188 RVA: 0x00007423 File Offset: 0x00005623
	private void Awake()
	{
		this.lineRenderer.positionCount = 2;
		this.material = this.planeMeshRenderer.material;
		this.planeMeshRenderer.enabled = false;
		this.lineRenderer.enabled = false;
	}

	// Token: 0x060000BD RID: 189 RVA: 0x0000745A File Offset: 0x0000565A
	private void OnDestroy()
	{
		UnityEngine.Object.Destroy(this.material);
	}

	// Token: 0x060000BE RID: 190 RVA: 0x000110F0 File Offset: 0x0000F2F0
	private void Update()
	{
		if (!this.IsVisible)
		{
			return;
		}
		if (!this.material)
		{
			return;
		}
		Debug.DrawRay(base.transform.position, Vector3.down * float.PositiveInfinity, Color.black);
		RaycastHit raycastHit;
		if (Physics.Raycast(base.transform.position, Vector3.down, out raycastHit, float.PositiveInfinity, this.raycastLayerMask))
		{
			this.planeMeshRenderer.transform.position = raycastHit.point - Vector3.up * this.raycastVerticalOffset;
			this.planeMeshRenderer.transform.rotation = Quaternion.FromToRotation(Vector3.up, raycastHit.normal);
			this.material.SetFloat("_Size", Mathf.Clamp(raycastHit.distance / this.maximumDistance, 0f, 1f));
			this.UpdateLineRendererPositions(raycastHit.point);
			this.planeMeshRenderer.enabled = true;
			this.lineRenderer.enabled = true;
			return;
		}
		this.planeMeshRenderer.enabled = false;
		this.lineRenderer.enabled = false;
	}

	// Token: 0x060000BF RID: 191 RVA: 0x00007467 File Offset: 0x00005667
	private void UpdateLineRendererPositions(Vector3 hitPosition)
	{
		this.lineRenderer.SetPosition(0, hitPosition);
		this.lineRenderer.SetPosition(1, base.transform.position);
	}

	// Token: 0x0400004E RID: 78
	[Header("References")]
	[SerializeField]
	private MeshRenderer planeMeshRenderer;

	// Token: 0x0400004F RID: 79
	[SerializeField]
	private LineRenderer lineRenderer;

	// Token: 0x04000050 RID: 80
	[Header("Settings")]
	[SerializeField]
	private float maximumDistance = 15f;

	// Token: 0x04000051 RID: 81
	[SerializeField]
	private float raycastVerticalOffset = 0.01f;

	// Token: 0x04000052 RID: 82
	[SerializeField]
	private LayerMask raycastLayerMask;

	// Token: 0x04000053 RID: 83
	private bool isVisible;

	// Token: 0x04000054 RID: 84
	private Material material;
}
