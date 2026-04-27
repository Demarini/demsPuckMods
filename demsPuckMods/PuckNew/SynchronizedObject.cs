using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Token: 0x0200006E RID: 110
public class SynchronizedObject : NetworkBehaviour
{
	// Token: 0x060003A3 RID: 931 RVA: 0x00015309 File Offset: 0x00013509
	private void Awake()
	{
		this.Rigidbody = base.GetComponent<Rigidbody>();
	}

	// Token: 0x060003A4 RID: 932 RVA: 0x00015318 File Offset: 0x00013518
	protected override void OnNetworkPostSpawn()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			this.Rigidbody.isKinematic = true;
			this.Rigidbody.interpolation = RigidbodyInterpolation.None;
		}
		EventManager.TriggerEvent("Event_Everyone_OnSynchronizedObjectSpawned", new Dictionary<string, object>
		{
			{
				"synchronizedObject",
				this
			}
		});
		base.OnNetworkPostSpawn();
	}

	// Token: 0x060003A5 RID: 933 RVA: 0x0001536A File Offset: 0x0001356A
	public override void OnNetworkDespawn()
	{
		EventManager.TriggerEvent("Event_Everyone_OnSynchronizedObjectDespawned", new Dictionary<string, object>
		{
			{
				"synchronizedObject",
				this
			}
		});
		base.OnNetworkDespawn();
	}

	// Token: 0x060003A6 RID: 934 RVA: 0x00015390 File Offset: 0x00013590
	public void OnClientTick(Vector3 position, Quaternion rotation, float serverDeltaTime)
	{
		this.PredictedLinearVelocity = (position - base.transform.position) / serverDeltaTime;
		this.PredictedAngularVelocity = (rotation * Quaternion.Inverse(this.lastReceivedRotation)).eulerAngles / serverDeltaTime;
		this.lastReceivedPosition = position;
		this.lastReceivedRotation = rotation;
		base.transform.position = position;
		base.transform.rotation = rotation;
	}

	// Token: 0x060003A7 RID: 935 RVA: 0x00015408 File Offset: 0x00013608
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

	// Token: 0x060003A8 RID: 936 RVA: 0x00015480 File Offset: 0x00013680
	public ValueTuple<Vector3, Quaternion, ulong> OnServerTick(float serverDeltaTime)
	{
		this.PredictedLinearVelocity = (base.transform.position - this.lastSentPosition) / serverDeltaTime;
		this.PredictedAngularVelocity = Quaternion.Inverse(base.transform.rotation) * this.lastSentRotation.eulerAngles / serverDeltaTime;
		this.lastSentPosition = base.transform.position;
		this.lastSentRotation = base.transform.rotation;
		return new ValueTuple<Vector3, Quaternion, ulong>(base.transform.position, base.transform.rotation, base.NetworkObjectId);
	}

	// Token: 0x060003A9 RID: 937 RVA: 0x00015520 File Offset: 0x00013720
	public bool ShouldSendPosition(int tickRate)
	{
		float num = Vector3.Distance(this.lastSentPosition, base.transform.position);
		float num2 = this.positionThreshold * (float)(100 / tickRate);
		return num > num2;
	}

	// Token: 0x060003AA RID: 938 RVA: 0x00015554 File Offset: 0x00013754
	public bool ShouldSendRotation(int tickRate)
	{
		float num = Quaternion.Angle(this.lastSentRotation, base.transform.rotation);
		float num2 = this.rotationThreshold * (float)(100 / tickRate);
		return num > num2;
	}

	// Token: 0x060003AC RID: 940 RVA: 0x000155F4 File Offset: 0x000137F4
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x060003AD RID: 941 RVA: 0x000021BE File Offset: 0x000003BE
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x060003AE RID: 942 RVA: 0x0001560A File Offset: 0x0001380A
	protected internal override string __getTypeName()
	{
		return "SynchronizedObject";
	}

	// Token: 0x04000285 RID: 645
	[Header("Settings")]
	[SerializeField]
	private float positionThreshold = 0.001f;

	// Token: 0x04000286 RID: 646
	[SerializeField]
	private float rotationThreshold = 0.01f;

	// Token: 0x04000287 RID: 647
	[HideInInspector]
	public Rigidbody Rigidbody;

	// Token: 0x04000288 RID: 648
	[HideInInspector]
	public Vector3 PredictedLinearVelocity = Vector3.zero;

	// Token: 0x04000289 RID: 649
	[HideInInspector]
	public Vector3 PredictedAngularVelocity = Vector3.zero;

	// Token: 0x0400028A RID: 650
	private Vector3 lastSentPosition = Vector3.zero;

	// Token: 0x0400028B RID: 651
	private Quaternion lastSentRotation = Quaternion.identity;

	// Token: 0x0400028C RID: 652
	private Vector3 lastReceivedPosition = Vector3.zero;

	// Token: 0x0400028D RID: 653
	private Quaternion lastReceivedRotation = Quaternion.identity;
}
