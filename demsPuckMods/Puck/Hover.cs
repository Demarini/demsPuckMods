using System;
using Unity.Netcode;
using UnityEngine;

// Token: 0x020000D5 RID: 213
[RequireComponent(typeof(Rigidbody))]
public class Hover : MonoBehaviour
{
	// Token: 0x06000672 RID: 1650 RVA: 0x0000B152 File Offset: 0x00009352
	private void Awake()
	{
		this.Rigidbody = base.GetComponent<Rigidbody>();
	}

	// Token: 0x06000673 RID: 1651 RVA: 0x0002633C File Offset: 0x0002453C
	private void FixedUpdate()
	{
		this.pidController.proportionalGain = this.proportionalGain;
		this.pidController.integralGain = this.integralGain;
		this.pidController.derivativeGain = this.derivativeGain;
		Vector3 vector = base.transform.position + this.raycastOffset;
		Vector3 down = Vector3.down;
		Debug.DrawRay(vector, down * this.raycastDistance, Color.black);
		RaycastHit raycastHit;
		if (Physics.Raycast(vector, down, out raycastHit, this.raycastDistance, this.raycastLayerMask))
		{
			this.currentDistance = raycastHit.distance;
		}
		else
		{
			this.currentDistance = this.raycastDistance;
		}
		this.IsGrounded = (this.currentDistance < this.raycastDistance);
		float num = this.pidController.Update(Time.fixedDeltaTime, this.currentDistance, this.TargetDistance);
		num = Mathf.Clamp(num, 0f, this.maxForce);
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.Rigidbody.AddForce(Vector3.up * num, ForceMode.Acceleration);
	}

	// Token: 0x04000381 RID: 897
	[Header("Settings")]
	[SerializeField]
	private float maxForce = 40f;

	// Token: 0x04000382 RID: 898
	[SerializeField]
	private Vector3 raycastOffset = new Vector3(0f, 1f, 0f);

	// Token: 0x04000383 RID: 899
	[SerializeField]
	private float raycastDistance = 1.45f;

	// Token: 0x04000384 RID: 900
	[SerializeField]
	private LayerMask raycastLayerMask;

	// Token: 0x04000385 RID: 901
	[Space(20f)]
	[SerializeField]
	private float proportionalGain = 100f;

	// Token: 0x04000386 RID: 902
	[SerializeField]
	private float integralGain;

	// Token: 0x04000387 RID: 903
	[SerializeField]
	private float derivativeGain = 15f;

	// Token: 0x04000388 RID: 904
	[Space(20f)]
	public float TargetDistance = 1f;

	// Token: 0x04000389 RID: 905
	[HideInInspector]
	public bool IsGrounded;

	// Token: 0x0400038A RID: 906
	[HideInInspector]
	public Rigidbody Rigidbody;

	// Token: 0x0400038B RID: 907
	private PIDController pidController = new PIDController(0f, 0f, 0f);

	// Token: 0x0400038C RID: 908
	private float currentDistance;
}
