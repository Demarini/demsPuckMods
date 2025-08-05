using System;
using Unity.Netcode;
using UnityEngine;

// Token: 0x020000D6 RID: 214
[RequireComponent(typeof(Rigidbody))]
public class KeepUpright : MonoBehaviour
{
	// Token: 0x06000675 RID: 1653 RVA: 0x0000B160 File Offset: 0x00009360
	private void Awake()
	{
		this.Rigidbody = base.GetComponent<Rigidbody>();
	}

	// Token: 0x06000676 RID: 1654 RVA: 0x000264CC File Offset: 0x000246CC
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

	// Token: 0x0400038D RID: 909
	[Header("Settings")]
	[SerializeField]
	private float proportionalGain = 50f;

	// Token: 0x0400038E RID: 910
	[SerializeField]
	private float integralGain;

	// Token: 0x0400038F RID: 911
	[SerializeField]
	private float derivativeGain = 5f;

	// Token: 0x04000390 RID: 912
	[HideInInspector]
	public float Balance = 1f;

	// Token: 0x04000391 RID: 913
	[HideInInspector]
	public Rigidbody Rigidbody;

	// Token: 0x04000392 RID: 914
	private Vector3PIDController pidController = new Vector3PIDController(0f, 0f, 0f);
}
