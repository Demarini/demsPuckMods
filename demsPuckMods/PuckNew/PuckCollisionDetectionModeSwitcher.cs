using System;
using Unity.Netcode;
using UnityEngine;

// Token: 0x0200005F RID: 95
public class PuckCollisionDetectionModeSwitcher : MonoBehaviour
{
	// Token: 0x0600033C RID: 828 RVA: 0x000136B5 File Offset: 0x000118B5
	private void Awake()
	{
		this.Rigidbody = base.GetComponent<Rigidbody>();
		Utils.SetRigidbodyCollisionDetectionMode(this.Rigidbody, CollisionDetectionMode.ContinuousDynamic);
	}

	// Token: 0x0600033D RID: 829 RVA: 0x000136CF File Offset: 0x000118CF
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

	// Token: 0x0600033E RID: 830 RVA: 0x00013707 File Offset: 0x00011907
	private void OnCollisionEnter(Collision collision)
	{
		if (!collision.gameObject.GetComponent<Stick>())
		{
			return;
		}
		this.IsContactingStick = true;
		Utils.SetRigidbodyCollisionDetectionMode(this.Rigidbody, CollisionDetectionMode.ContinuousSpeculative);
	}

	// Token: 0x0600033F RID: 831 RVA: 0x00013707 File Offset: 0x00011907
	private void OnCollisionStay(Collision collision)
	{
		if (!collision.gameObject.GetComponent<Stick>())
		{
			return;
		}
		this.IsContactingStick = true;
		Utils.SetRigidbodyCollisionDetectionMode(this.Rigidbody, CollisionDetectionMode.ContinuousSpeculative);
	}

	// Token: 0x06000340 RID: 832 RVA: 0x00013730 File Offset: 0x00011930
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

	// Token: 0x0400023D RID: 573
	[HideInInspector]
	public Rigidbody Rigidbody;

	// Token: 0x0400023E RID: 574
	[HideInInspector]
	public bool IsContactingStick;
}
