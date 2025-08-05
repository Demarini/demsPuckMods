using System;
using UnityEngine;

// Token: 0x02000027 RID: 39
public class ReplayCamera : BaseCamera
{
	// Token: 0x06000119 RID: 281 RVA: 0x000078FB File Offset: 0x00005AFB
	public void SetTarget(Transform target)
	{
		this.target = target;
	}

	// Token: 0x0600011A RID: 282 RVA: 0x00011C50 File Offset: 0x0000FE50
	public override void OnTick(float deltaTime)
	{
		base.OnTick(deltaTime);
		if (!this.target)
		{
			return;
		}
		base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.LookRotation(this.target.position - base.transform.position), deltaTime * this.rotationSpeed);
	}

	// Token: 0x0600011C RID: 284 RVA: 0x00011CB8 File Offset: 0x0000FEB8
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x0600011D RID: 285 RVA: 0x00007917 File Offset: 0x00005B17
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x0600011E RID: 286 RVA: 0x00007921 File Offset: 0x00005B21
	protected internal override string __getTypeName()
	{
		return "ReplayCamera";
	}

	// Token: 0x04000093 RID: 147
	[Header("Settings")]
	[SerializeField]
	private float rotationSpeed = 10f;

	// Token: 0x04000094 RID: 148
	private Transform target;
}
