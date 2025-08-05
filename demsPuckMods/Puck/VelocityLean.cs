using System;
using Unity.Netcode;
using UnityEngine;

// Token: 0x020000E2 RID: 226
[RequireComponent(typeof(Rigidbody))]
public class VelocityLean : MonoBehaviour
{
	// Token: 0x0600071D RID: 1821 RVA: 0x0000B890 File Offset: 0x00009A90
	private void Awake()
	{
		this.Rigidbody = base.GetComponent<Rigidbody>();
	}

	// Token: 0x0600071E RID: 1822 RVA: 0x00029A48 File Offset: 0x00027C48
	private void FixedUpdate()
	{
		float d = this.UseWorldLinearVelocity ? this.Rigidbody.linearVelocity.magnitude : this.MovementDirection.InverseTransformVector(this.Rigidbody.linearVelocity).z;
		float y = this.MovementDirection.InverseTransformVector(this.Rigidbody.angularVelocity).y;
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.Rigidbody.AddTorque(d * (this.Inverted ? (-base.transform.right) : base.transform.right) * this.linearForceMultiplier * this.LinearIntensity, ForceMode.Acceleration);
		this.Rigidbody.AddTorque(-y * (this.Inverted ? (-base.transform.forward) : base.transform.forward) * this.angularForceMultiplier * this.AngularIntensity, ForceMode.Acceleration);
	}

	// Token: 0x04000430 RID: 1072
	[Header("Settings")]
	[SerializeField]
	private float linearForceMultiplier = 1f;

	// Token: 0x04000431 RID: 1073
	[SerializeField]
	private float angularForceMultiplier = 6f;

	// Token: 0x04000432 RID: 1074
	[HideInInspector]
	public Rigidbody Rigidbody;

	// Token: 0x04000433 RID: 1075
	[HideInInspector]
	public float LinearIntensity = 1f;

	// Token: 0x04000434 RID: 1076
	[HideInInspector]
	public float AngularIntensity = 1f;

	// Token: 0x04000435 RID: 1077
	[HideInInspector]
	public bool Inverted;

	// Token: 0x04000436 RID: 1078
	[HideInInspector]
	public bool UseWorldLinearVelocity;

	// Token: 0x04000437 RID: 1079
	[HideInInspector]
	public Transform MovementDirection;
}
