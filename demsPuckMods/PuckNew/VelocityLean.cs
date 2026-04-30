using System;
using Unity.Netcode;
using UnityEngine;

// Token: 0x02000038 RID: 56
[RequireComponent(typeof(Rigidbody))]
public class VelocityLean : MonoBehaviour
{
	// Token: 0x06000165 RID: 357 RVA: 0x00007B28 File Offset: 0x00005D28
	private void Awake()
	{
		this.Rigidbody = base.GetComponent<Rigidbody>();
	}

	// Token: 0x06000166 RID: 358 RVA: 0x00007B38 File Offset: 0x00005D38
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

	// Token: 0x04000113 RID: 275
	[Header("Settings")]
	[SerializeField]
	private float linearForceMultiplier = 1f;

	// Token: 0x04000114 RID: 276
	[SerializeField]
	private float angularForceMultiplier = 6f;

	// Token: 0x04000115 RID: 277
	[HideInInspector]
	public Rigidbody Rigidbody;

	// Token: 0x04000116 RID: 278
	[HideInInspector]
	public float LinearIntensity = 1f;

	// Token: 0x04000117 RID: 279
	[HideInInspector]
	public float AngularIntensity = 1f;

	// Token: 0x04000118 RID: 280
	[HideInInspector]
	public bool Inverted;

	// Token: 0x04000119 RID: 281
	[HideInInspector]
	public bool UseWorldLinearVelocity;

	// Token: 0x0400011A RID: 282
	[HideInInspector]
	public Transform MovementDirection;
}
