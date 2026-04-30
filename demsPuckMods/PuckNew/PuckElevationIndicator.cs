using System;
using UnityEngine;

// Token: 0x02000057 RID: 87
public class PuckElevationIndicator : MonoBehaviour
{
	// Token: 0x1700005C RID: 92
	// (get) Token: 0x060002F6 RID: 758 RVA: 0x000128FA File Offset: 0x00010AFA
	// (set) Token: 0x060002F7 RID: 759 RVA: 0x00012902 File Offset: 0x00010B02
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

	// Token: 0x060002F8 RID: 760 RVA: 0x0001292D File Offset: 0x00010B2D
	private void Awake()
	{
		this.lineRenderer.positionCount = 2;
		this.material = this.planeMeshRenderer.material;
		this.planeMeshRenderer.enabled = false;
		this.lineRenderer.enabled = false;
	}

	// Token: 0x060002F9 RID: 761 RVA: 0x00012964 File Offset: 0x00010B64
	private void OnDestroy()
	{
		Object.Destroy(this.material);
	}

	// Token: 0x060002FA RID: 762 RVA: 0x00012974 File Offset: 0x00010B74
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

	// Token: 0x060002FB RID: 763 RVA: 0x00012AA0 File Offset: 0x00010CA0
	private void UpdateLineRendererPositions(Vector3 hitPosition)
	{
		this.lineRenderer.SetPosition(0, hitPosition);
		this.lineRenderer.SetPosition(1, base.transform.position);
	}

	// Token: 0x0400020A RID: 522
	[Header("Settings")]
	[SerializeField]
	private float maximumDistance = 15f;

	// Token: 0x0400020B RID: 523
	[SerializeField]
	private float raycastVerticalOffset = 0.01f;

	// Token: 0x0400020C RID: 524
	[SerializeField]
	private LayerMask raycastLayerMask;

	// Token: 0x0400020D RID: 525
	[Header("References")]
	[SerializeField]
	private MeshRenderer planeMeshRenderer;

	// Token: 0x0400020E RID: 526
	[SerializeField]
	private LineRenderer lineRenderer;

	// Token: 0x0400020F RID: 527
	private bool isVisible;

	// Token: 0x04000210 RID: 528
	private Material material;
}
