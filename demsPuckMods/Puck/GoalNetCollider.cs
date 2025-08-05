using System;
using UnityEngine;

// Token: 0x0200000F RID: 15
public class GoalNetCollider : MonoBehaviour
{
	// Token: 0x06000081 RID: 129 RVA: 0x00010A58 File Offset: 0x0000EC58
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

	// Token: 0x04000037 RID: 55
	[Header("Settings")]
	[SerializeField]
	private float damping = 0.25f;

	// Token: 0x04000038 RID: 56
	[SerializeField]
	private float linearVelocityMaximumMagnitude = 2f;

	// Token: 0x04000039 RID: 57
	[SerializeField]
	private float angularVelocityMaximumMagnitude = 2f;
}
