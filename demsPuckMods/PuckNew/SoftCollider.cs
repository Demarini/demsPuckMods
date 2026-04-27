using System;
using UnityEngine;

// Token: 0x02000065 RID: 101
public class SoftCollider : MonoBehaviour
{
	// Token: 0x0600035C RID: 860 RVA: 0x00013DD5 File Offset: 0x00011FD5
	private void Awake()
	{
		this.Rigidbody = base.GetComponent<Rigidbody>();
	}

	// Token: 0x0600035D RID: 861 RVA: 0x00013DE4 File Offset: 0x00011FE4
	private void FixedUpdate()
	{
		this.worldOrigin = base.transform.TransformPoint(this.localOrigin);
		foreach (Vector3 vector in new Vector3[]
		{
			(base.transform.forward + base.transform.right).normalized,
			(base.transform.forward - base.transform.right).normalized,
			(-base.transform.forward + base.transform.right).normalized,
			(-base.transform.forward - base.transform.right).normalized
		})
		{
			Debug.DrawRay(this.worldOrigin, vector * this.distance, Color.black);
			RaycastHit raycastHit;
			if (Physics.Raycast(this.worldOrigin, vector, out raycastHit, this.distance, this.layerMask))
			{
				Debug.DrawRay(this.worldOrigin, vector * raycastHit.distance, Color.white);
				float d = this.distance - raycastHit.distance;
				float magnitude = Vector3.Cross(raycastHit.normal, vector).magnitude;
				float num = 1f - magnitude;
				Debug.DrawRay(raycastHit.point, raycastHit.normal * d * this.force, Color.green);
				this.Rigidbody.AddForceAtPosition(raycastHit.normal * d * (this.force * num), raycastHit.point, ForceMode.Acceleration);
			}
		}
	}

	// Token: 0x04000253 RID: 595
	[Header("Settings")]
	[SerializeField]
	private Vector3 localOrigin = Vector3.zero;

	// Token: 0x04000254 RID: 596
	[SerializeField]
	private LayerMask layerMask;

	// Token: 0x04000255 RID: 597
	[SerializeField]
	private float distance = 0.5f;

	// Token: 0x04000256 RID: 598
	[SerializeField]
	private float force = 10f;

	// Token: 0x04000257 RID: 599
	[HideInInspector]
	public float Intensity = 1f;

	// Token: 0x04000258 RID: 600
	[HideInInspector]
	public Rigidbody Rigidbody;

	// Token: 0x04000259 RID: 601
	private Vector3 worldOrigin = Vector3.zero;
}
