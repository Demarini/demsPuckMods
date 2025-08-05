using System;
using Unity.Netcode;
using UnityEngine;

// Token: 0x020000E1 RID: 225
[RequireComponent(typeof(Rigidbody))]
public class Skate : MonoBehaviour
{
	// Token: 0x0600071A RID: 1818 RVA: 0x0000B864 File Offset: 0x00009A64
	private void Awake()
	{
		this.Rigidbody = base.GetComponent<Rigidbody>();
	}

	// Token: 0x0600071B RID: 1819 RVA: 0x00029998 File Offset: 0x00027B98
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

	// Token: 0x0400042B RID: 1067
	[Header("Settings")]
	[SerializeField]
	private float traction = 0.15f;

	// Token: 0x0400042C RID: 1068
	[HideInInspector]
	public Rigidbody Rigidbody;

	// Token: 0x0400042D RID: 1069
	[HideInInspector]
	public float Intensity = 1f;

	// Token: 0x0400042E RID: 1070
	[HideInInspector]
	public bool IsTractionLost;

	// Token: 0x0400042F RID: 1071
	[HideInInspector]
	public Transform MovementDirection;
}
