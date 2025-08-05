using System;
using UnityEngine;

// Token: 0x0200002B RID: 43
public class SoftCollider : MonoBehaviour
{
	// Token: 0x06000134 RID: 308 RVA: 0x00007A49 File Offset: 0x00005C49
	private void Awake()
	{
		this.Rigidbody = base.GetComponent<Rigidbody>();
	}

	// Token: 0x06000135 RID: 309 RVA: 0x00011EE8 File Offset: 0x000100E8
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

	// Token: 0x0400009C RID: 156
	[Header("Settings")]
	[SerializeField]
	private Vector3 localOrigin = Vector3.zero;

	// Token: 0x0400009D RID: 157
	[SerializeField]
	private LayerMask layerMask;

	// Token: 0x0400009E RID: 158
	[SerializeField]
	private float distance = 0.5f;

	// Token: 0x0400009F RID: 159
	[SerializeField]
	private float force = 10f;

	// Token: 0x040000A0 RID: 160
	[HideInInspector]
	public float Intensity = 1f;

	// Token: 0x040000A1 RID: 161
	[HideInInspector]
	public Rigidbody Rigidbody;

	// Token: 0x040000A2 RID: 162
	private Vector3 worldOrigin = Vector3.zero;
}
