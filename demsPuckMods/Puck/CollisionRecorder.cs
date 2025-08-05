using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000008 RID: 8
public class CollisionRecorder : MonoBehaviour
{
	// Token: 0x0600005A RID: 90 RVA: 0x00006F86 File Offset: 0x00005186
	private void Awake()
	{
		this.Rigidbody = base.GetComponent<Rigidbody>();
	}

	// Token: 0x0600005B RID: 91 RVA: 0x00006F94 File Offset: 0x00005194
	private void OnDestroy()
	{
		this.StopDeferringCollision();
	}

	// Token: 0x0600005C RID: 92 RVA: 0x00006F9C File Offset: 0x0000519C
	private void StartDeferringCollision()
	{
		this.StopDeferringCollision();
		this.deferCollisionCoroutine = this.IDeferCollision(this.deferTime);
		base.StartCoroutine(this.deferCollisionCoroutine);
	}

	// Token: 0x0600005D RID: 93 RVA: 0x00006FC3 File Offset: 0x000051C3
	private void StopDeferringCollision()
	{
		if (this.deferCollisionCoroutine == null)
		{
			return;
		}
		base.StopCoroutine(this.deferCollisionCoroutine);
	}

	// Token: 0x0600005E RID: 94 RVA: 0x00006FDA File Offset: 0x000051DA
	private IEnumerator IDeferCollision(float duration)
	{
		yield return new WaitForSeconds(duration);
		KeyValuePair<GameObject, float> keyValuePair = (from x in this.collisionGameObjectForceMap
		orderby x.Value descending
		select x).FirstOrDefault<KeyValuePair<GameObject, float>>();
		if (keyValuePair.Key)
		{
			Action<GameObject, float> onDeferredCollision = this.OnDeferredCollision;
			if (onDeferredCollision != null)
			{
				onDeferredCollision(keyValuePair.Key, keyValuePair.Value);
			}
		}
		this.recording = false;
		this.collisionGameObjectForceMap.Clear();
		yield break;
	}

	// Token: 0x0600005F RID: 95 RVA: 0x000104C0 File Offset: 0x0000E6C0
	private void OnCollisionEnter(Collision collision)
	{
		if (!this.recording)
		{
			this.recording = true;
			this.StartDeferringCollision();
		}
		float collisionForce = Utils.GetCollisionForce(collision);
		GameObject gameObject = collision.collider.gameObject;
		if (!gameObject)
		{
			return;
		}
		if (this.collisionGameObjectForceMap.ContainsKey(gameObject))
		{
			this.collisionGameObjectForceMap[gameObject] = Mathf.Max(this.collisionGameObjectForceMap[gameObject], collisionForce);
			return;
		}
		this.collisionGameObjectForceMap.Add(gameObject, collisionForce);
	}

	// Token: 0x04000021 RID: 33
	[Header("Settings")]
	[SerializeField]
	private float deferTime = 0.1f;

	// Token: 0x04000022 RID: 34
	[HideInInspector]
	public Action<GameObject, float> OnDeferredCollision;

	// Token: 0x04000023 RID: 35
	[HideInInspector]
	public Rigidbody Rigidbody;

	// Token: 0x04000024 RID: 36
	private bool recording;

	// Token: 0x04000025 RID: 37
	private Dictionary<GameObject, float> collisionGameObjectForceMap = new Dictionary<GameObject, float>();

	// Token: 0x04000026 RID: 38
	private IEnumerator deferCollisionCoroutine;
}
