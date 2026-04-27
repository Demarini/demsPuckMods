using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000004 RID: 4
public class CollisionRecorder : MonoBehaviour
{
	// Token: 0x06000013 RID: 19 RVA: 0x000022E2 File Offset: 0x000004E2
	private void Awake()
	{
		this.Rigidbody = base.GetComponent<Rigidbody>();
	}

	// Token: 0x06000014 RID: 20 RVA: 0x000022F0 File Offset: 0x000004F0
	private void OnDestroy()
	{
		this.StopDeferringCollision();
	}

	// Token: 0x06000015 RID: 21 RVA: 0x000022F8 File Offset: 0x000004F8
	private void StartDeferringCollision()
	{
		this.StopDeferringCollision();
		this.deferCollisionCoroutine = this.IDeferCollision(this.deferTime);
		base.StartCoroutine(this.deferCollisionCoroutine);
	}

	// Token: 0x06000016 RID: 22 RVA: 0x0000231F File Offset: 0x0000051F
	private void StopDeferringCollision()
	{
		if (this.deferCollisionCoroutine == null)
		{
			return;
		}
		base.StopCoroutine(this.deferCollisionCoroutine);
	}

	// Token: 0x06000017 RID: 23 RVA: 0x00002336 File Offset: 0x00000536
	private IEnumerator IDeferCollision(float duration)
	{
		yield return new WaitForSeconds(duration);
		KeyValuePair<GameObject, float> keyValuePair = (from x in this.collisionGameObjectForceMap
		orderby x.Value descending
		select x).FirstOrDefault<KeyValuePair<GameObject, float>>();
		if (keyValuePair.Key)
		{
			Action<GameObject, float> collisionDeferred = this.CollisionDeferred;
			if (collisionDeferred != null)
			{
				collisionDeferred(keyValuePair.Key, keyValuePair.Value);
			}
		}
		this.recording = false;
		this.collisionGameObjectForceMap.Clear();
		yield break;
	}

	// Token: 0x06000018 RID: 24 RVA: 0x0000234C File Offset: 0x0000054C
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

	// Token: 0x04000006 RID: 6
	[Header("Settings")]
	[SerializeField]
	private float deferTime = 0.1f;

	// Token: 0x04000007 RID: 7
	[HideInInspector]
	public Action<GameObject, float> CollisionDeferred;

	// Token: 0x04000008 RID: 8
	[HideInInspector]
	public Rigidbody Rigidbody;

	// Token: 0x04000009 RID: 9
	private bool recording;

	// Token: 0x0400000A RID: 10
	private Dictionary<GameObject, float> collisionGameObjectForceMap = new Dictionary<GameObject, float>();

	// Token: 0x0400000B RID: 11
	private IEnumerator deferCollisionCoroutine;
}
