using System;
using UnityEngine;
using UnityEngine.Splines;

// Token: 0x02000158 RID: 344
public class MoveAlongSpline : MonoBehaviour
{
	// Token: 0x06000C29 RID: 3113 RVA: 0x000416D0 File Offset: 0x0003F8D0
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

	// Token: 0x04000701 RID: 1793
	[Header("References")]
	public SplineContainer spline;

	// Token: 0x04000702 RID: 1794
	[Header("Settings")]
	public float speed = 1f;

	// Token: 0x04000703 RID: 1795
	private float splinePosition;
}
