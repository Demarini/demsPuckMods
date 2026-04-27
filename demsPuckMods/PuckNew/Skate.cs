using System;
using Unity.Netcode;
using UnityEngine;

// Token: 0x02000037 RID: 55
[RequireComponent(typeof(Rigidbody))]
public class Skate : MonoBehaviour
{
	// Token: 0x06000162 RID: 354 RVA: 0x00007A4D File Offset: 0x00005C4D
	private void Awake()
	{
		this.Rigidbody = base.GetComponent<Rigidbody>();
	}

	// Token: 0x06000163 RID: 355 RVA: 0x00007A5C File Offset: 0x00005C5C
	private void FixedUpdate()
	{
		Vector3 vector = this.MovementDirection.InverseTransformVector(this.Rigidbody.linearVelocity);
		vector.y = 0f;
		vector.z = 0f;
		float num = -vector.x;
		this.IsTractionLost = (num > this.traction * Time.fixedDeltaTime);
		num = Mathf.Clamp(num, -this.traction * Time.fixedDeltaTime, this.traction * Time.fixedDeltaTime);
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.Rigidbody.AddForce(this.MovementDirection.right * num * this.Intensity, ForceMode.VelocityChange);
	}

	// Token: 0x0400010E RID: 270
	[Header("Settings")]
	[SerializeField]
	private float traction = 0.15f;

	// Token: 0x0400010F RID: 271
	[HideInInspector]
	public Rigidbody Rigidbody;

	// Token: 0x04000110 RID: 272
	[HideInInspector]
	public float Intensity = 1f;

	// Token: 0x04000111 RID: 273
	[HideInInspector]
	public bool IsTractionLost;

	// Token: 0x04000112 RID: 274
	[HideInInspector]
	public Transform MovementDirection;
}
