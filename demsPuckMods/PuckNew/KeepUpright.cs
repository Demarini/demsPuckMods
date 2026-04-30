using System;
using Unity.Netcode;
using UnityEngine;

// Token: 0x0200001F RID: 31
[RequireComponent(typeof(Rigidbody))]
public class KeepUpright : MonoBehaviour
{
	// Token: 0x060000B8 RID: 184 RVA: 0x00004246 File Offset: 0x00002446
	private void Awake()
	{
		this.Rigidbody = base.GetComponent<Rigidbody>();
	}

	// Token: 0x060000B9 RID: 185 RVA: 0x00004254 File Offset: 0x00002454
	private void FixedUpdate()
	{
		this.pidController.proportionalGain = this.proportionalGain * this.Balance;
		this.pidController.integralGain = this.integralGain * this.Balance;
		this.pidController.derivativeGain = this.derivativeGain * this.Balance;
		Vector3 a = Vector3.Cross(this.pidController.Update(Time.fixedDeltaTime, base.transform.up, Vector3.up), Vector3.up);
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.Rigidbody.AddTorque(-a, ForceMode.Acceleration);
	}

	// Token: 0x0400005A RID: 90
	[Header("Settings")]
	[SerializeField]
	private float proportionalGain = 50f;

	// Token: 0x0400005B RID: 91
	[SerializeField]
	private float integralGain;

	// Token: 0x0400005C RID: 92
	[SerializeField]
	private float derivativeGain = 5f;

	// Token: 0x0400005D RID: 93
	[HideInInspector]
	public float Balance = 1f;

	// Token: 0x0400005E RID: 94
	[HideInInspector]
	public Rigidbody Rigidbody;

	// Token: 0x0400005F RID: 95
	private Vector3PIDController pidController = new Vector3PIDController(0f, 0f, 0f);
}
