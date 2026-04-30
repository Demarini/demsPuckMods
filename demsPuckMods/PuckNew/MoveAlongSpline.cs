using System;
using UnityEngine;
using UnityEngine.Splines;

// Token: 0x020001E3 RID: 483
public class MoveAlongSpline : MonoBehaviour
{
	// Token: 0x06000E17 RID: 3607 RVA: 0x00042568 File Offset: 0x00040768
	private void Update()
	{
		this.splinePosition += Time.deltaTime * this.speed;
		if (this.splinePosition >= 1f)
		{
			this.splinePosition = 0f;
		}
		Vector3 position = this.spline.EvaluatePosition(this.splinePosition);
		base.transform.position = position;
		base.transform.LookAt(Vector3.zero);
	}

	// Token: 0x0400088D RID: 2189
	[Header("Settings")]
	public float speed = 1f;

	// Token: 0x0400088E RID: 2190
	[Header("References")]
	public SplineContainer spline;

	// Token: 0x0400088F RID: 2191
	private float splinePosition;
}
