using System;
using UnityEngine;

// Token: 0x0200000B RID: 11
public class GoalNetCollider : MonoBehaviour
{
	// Token: 0x06000036 RID: 54 RVA: 0x000028EC File Offset: 0x00000AEC
	private void OnCollisionEnter(Collision collision)
	{
		Puck componentInParent = collision.gameObject.GetComponentInParent<Puck>();
		if (!componentInParent)
		{
			return;
		}
		if (componentInParent.IsGrounded)
		{
			return;
		}
		componentInParent.Rigidbody.linearVelocity *= 1f - this.damping;
		componentInParent.Rigidbody.angularVelocity *= 1f - this.damping;
		if (componentInParent.Rigidbody.linearVelocity.magnitude > this.linearVelocityMaximumMagnitude)
		{
			componentInParent.Rigidbody.linearVelocity = componentInParent.Rigidbody.linearVelocity.normalized * this.linearVelocityMaximumMagnitude;
		}
		if (componentInParent.Rigidbody.angularVelocity.magnitude > this.angularVelocityMaximumMagnitude)
		{
			componentInParent.Rigidbody.angularVelocity = componentInParent.Rigidbody.angularVelocity.normalized * this.angularVelocityMaximumMagnitude;
		}
	}

	// Token: 0x0400001B RID: 27
	[Header("Settings")]
	[SerializeField]
	private float damping = 0.25f;

	// Token: 0x0400001C RID: 28
	[SerializeField]
	private float linearVelocityMaximumMagnitude = 2f;

	// Token: 0x0400001D RID: 29
	[SerializeField]
	private float angularVelocityMaximumMagnitude = 2f;
}
