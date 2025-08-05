using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Token: 0x020000D3 RID: 211
public class SynchronizedObject : NetworkBehaviour
{
	// Token: 0x0600065F RID: 1631 RVA: 0x0000B0F8 File Offset: 0x000092F8
	private void Awake()
	{
		this.Rigidbody = base.GetComponent<Rigidbody>();
	}

	// Token: 0x06000660 RID: 1632 RVA: 0x00026080 File Offset: 0x00024280
	protected override void OnNetworkPostSpawn()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			this.Rigidbody.isKinematic = true;
			this.Rigidbody.interpolation = RigidbodyInterpolation.None;
		}
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnSynchronizedObjectSpawned", new Dictionary<string, object>
		{
			{
				"synchronizedObject",
				this
			}
		});
		base.OnNetworkPostSpawn();
	}

	// Token: 0x06000661 RID: 1633 RVA: 0x0000B106 File Offset: 0x00009306
	public override void OnNetworkDespawn()
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnSynchronizedObjectDespawned", new Dictionary<string, object>
		{
			{
				"synchronizedObject",
				this
			}
		});
		base.OnNetworkDespawn();
	}

	// Token: 0x06000662 RID: 1634 RVA: 0x000260D8 File Offset: 0x000242D8
	public void OnClientTick(Vector3 position, Quaternion rotation, float serverDeltaTime)
	{
		this.PredictedLinearVelocity = (position - base.transform.position) / serverDeltaTime;
		this.PredictedAngularVelocity = (rotation * Quaternion.Inverse(this.lastReceivedRotation)).eulerAngles / serverDeltaTime;
		this.lastReceivedPosition = position;
		this.lastReceivedRotation = rotation;
		base.transform.position = position;
		base.transform.rotation = rotation;
	}

	// Token: 0x06000663 RID: 1635 RVA: 0x00026150 File Offset: 0x00024350
	public SynchronizedObjectSnapshot OnClientSmoothTick(Vector3 position, Quaternion rotation, SynchronizedObject synchronizedObject, float serverDeltaTime)
	{
		Vector3 linearVelocity = (position - this.lastReceivedPosition) / serverDeltaTime;
		Vector3 angularVelocity = (rotation * Quaternion.Inverse(this.lastReceivedRotation)).eulerAngles / serverDeltaTime;
		this.lastReceivedPosition = position;
		this.lastReceivedRotation = rotation;
		return new SynchronizedObjectSnapshot
		{
			SynchronizedObject = synchronizedObject,
			Position = position,
			Rotation = rotation,
			LinearVelocity = linearVelocity,
			AngularVelocity = angularVelocity
		};
	}

	// Token: 0x06000664 RID: 1636 RVA: 0x000261C8 File Offset: 0x000243C8
	public ValueTuple<Vector3, Quaternion, ulong> OnServerTick(float serverDeltaTime)
	{
		this.PredictedLinearVelocity = (base.transform.position - this.lastSentPosition) / serverDeltaTime;
		this.PredictedAngularVelocity = Quaternion.Inverse(base.transform.rotation) * this.lastSentRotation.eulerAngles / serverDeltaTime;
		this.lastSentPosition = base.transform.position;
		this.lastSentRotation = base.transform.rotation;
		return new ValueTuple<Vector3, Quaternion, ulong>(base.transform.position, base.transform.rotation, base.NetworkObjectId);
	}

	// Token: 0x06000665 RID: 1637 RVA: 0x00026268 File Offset: 0x00024468
	public bool ShouldSendPosition(int tickRate)
	{
		float num = Vector3.Distance(this.lastSentPosition, base.transform.position);
		float num2 = this.positionThreshold * (float)(100 / tickRate);
		return num > num2;
	}

	// Token: 0x06000666 RID: 1638 RVA: 0x0002629C File Offset: 0x0002449C
	public bool ShouldSendRotation(int tickRate)
	{
		float num = Quaternion.Angle(this.lastSentRotation, base.transform.rotation);
		float num2 = this.rotationThreshold * (float)(100 / tickRate);
		return num > num2;
	}

	// Token: 0x06000668 RID: 1640 RVA: 0x0000F5B8 File Offset: 0x0000D7B8
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x06000669 RID: 1641 RVA: 0x00006C33 File Offset: 0x00004E33
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x0600066A RID: 1642 RVA: 0x0000B12E File Offset: 0x0000932E
	protected internal override string __getTypeName()
	{
		return "SynchronizedObject";
	}

	// Token: 0x04000377 RID: 887
	[Header("Settings")]
	[SerializeField]
	private float positionThreshold = 0.001f;

	// Token: 0x04000378 RID: 888
	[SerializeField]
	private float rotationThreshold = 0.01f;

	// Token: 0x04000379 RID: 889
	[HideInInspector]
	public Rigidbody Rigidbody;

	// Token: 0x0400037A RID: 890
	[HideInInspector]
	public Vector3 PredictedLinearVelocity = Vector3.zero;

	// Token: 0x0400037B RID: 891
	[HideInInspector]
	public Vector3 PredictedAngularVelocity = Vector3.zero;

	// Token: 0x0400037C RID: 892
	private Vector3 lastSentPosition = Vector3.zero;

	// Token: 0x0400037D RID: 893
	private Quaternion lastSentRotation = Quaternion.identity;

	// Token: 0x0400037E RID: 894
	private Vector3 lastReceivedPosition = Vector3.zero;

	// Token: 0x0400037F RID: 895
	private Quaternion lastReceivedRotation = Quaternion.identity;
}
