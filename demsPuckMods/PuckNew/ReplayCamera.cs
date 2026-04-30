using System;
using UnityEngine;

// Token: 0x02000060 RID: 96
public class ReplayCamera : BaseCamera
{
	// Token: 0x06000342 RID: 834 RVA: 0x00013788 File Offset: 0x00011988
	public override void OnTick(float deltaTime)
	{
		base.OnTick(deltaTime);
		if (!this.Target)
		{
			return;
		}
		Vector3 normalized = (this.CenterPoint - this.Target.position).normalized;
		Vector3 b = this.Target.position + normalized * this.followDistance;
		b.y = this.followHeight;
		base.transform.position = Vector3.Lerp(base.transform.position, b, deltaTime * this.followSpeed);
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation(this.Target.position - base.transform.position), deltaTime * this.rotationSpeed);
	}

	// Token: 0x06000344 RID: 836 RVA: 0x0001389C File Offset: 0x00011A9C
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x06000345 RID: 837 RVA: 0x00002D76 File Offset: 0x00000F76
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x06000346 RID: 838 RVA: 0x000138B2 File Offset: 0x00011AB2
	protected internal override string __getTypeName()
	{
		return "ReplayCamera";
	}

	// Token: 0x0400023F RID: 575
	[Header("Settings")]
	[SerializeField]
	private float followSpeed = 10f;

	// Token: 0x04000240 RID: 576
	[SerializeField]
	private float followDistance = 5f;

	// Token: 0x04000241 RID: 577
	[SerializeField]
	private float followHeight = 5f;

	// Token: 0x04000242 RID: 578
	[SerializeField]
	private float rotationSpeed = 10f;

	// Token: 0x04000243 RID: 579
	[HideInInspector]
	public Transform Target;

	// Token: 0x04000244 RID: 580
	[HideInInspector]
	public Vector3 CenterPoint = Vector3.zero;
}
