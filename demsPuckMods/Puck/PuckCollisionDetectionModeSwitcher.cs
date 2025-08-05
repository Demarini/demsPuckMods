using System;
using Unity.Netcode;
using UnityEngine;

// Token: 0x02000026 RID: 38
public class PuckCollisionDetectionModeSwitcher : MonoBehaviour
{
	// Token: 0x06000113 RID: 275 RVA: 0x00007881 File Offset: 0x00005A81
	private void Awake()
	{
		this.Rigidbody = base.GetComponent<Rigidbody>();
		Utils.SetRigidbodyCollisionDetectionMode(this.Rigidbody, CollisionDetectionMode.ContinuousDynamic);
	}

	// Token: 0x06000114 RID: 276 RVA: 0x0000789B File Offset: 0x00005A9B
	private void FixedUpdate()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (this.IsContactingStick)
		{
			Utils.SetRigidbodyCollisionDetectionMode(this.Rigidbody, CollisionDetectionMode.ContinuousSpeculative);
		}
		else
		{
			Utils.SetRigidbodyCollisionDetectionMode(this.Rigidbody, CollisionDetectionMode.ContinuousDynamic);
		}
		this.IsContactingStick = false;
	}

	// Token: 0x06000115 RID: 277 RVA: 0x000078D3 File Offset: 0x00005AD3
	private void OnCollisionEnter(Collision collision)
	{
		if (!collision.gameObject.GetComponent<Stick>())
		{
			return;
		}
		this.IsContactingStick = true;
		Utils.SetRigidbodyCollisionDetectionMode(this.Rigidbody, CollisionDetectionMode.ContinuousSpeculative);
	}

	// Token: 0x06000116 RID: 278 RVA: 0x000078D3 File Offset: 0x00005AD3
	private void OnCollisionStay(Collision collision)
	{
		if (!collision.gameObject.GetComponent<Stick>())
		{
			return;
		}
		this.IsContactingStick = true;
		Utils.SetRigidbodyCollisionDetectionMode(this.Rigidbody, CollisionDetectionMode.ContinuousSpeculative);
	}

	// Token: 0x06000117 RID: 279 RVA: 0x00011BF8 File Offset: 0x0000FDF8
	public void OnDrawGizmos()
	{
		if (!Application.isEditor)
		{
			return;
		}
		if (this.Rigidbody)
		{
			Gizmos.color = ((this.Rigidbody.collisionDetectionMode == CollisionDetectionMode.ContinuousSpeculative) ? Color.red : Color.green);
		}
		Gizmos.DrawWireSphere(base.transform.position, 0.5f);
	}

	// Token: 0x04000091 RID: 145
	[HideInInspector]
	public Rigidbody Rigidbody;

	// Token: 0x04000092 RID: 146
	[HideInInspector]
	public bool IsContactingStick;
}
