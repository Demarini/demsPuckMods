using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;

// Token: 0x0200005B RID: 91
public class Puck : NetworkBehaviour
{
	// Token: 0x1700005D RID: 93
	// (get) Token: 0x0600030A RID: 778 RVA: 0x00012BCF File Offset: 0x00010DCF
	[HideInInspector]
	public float PredictedSpeed
	{
		get
		{
			return this.SynchronizedObject.PredictedLinearVelocity.magnitude;
		}
	}

	// Token: 0x1700005E RID: 94
	// (get) Token: 0x0600030B RID: 779 RVA: 0x00012BE1 File Offset: 0x00010DE1
	[HideInInspector]
	public float PredictedAngularSpeed
	{
		get
		{
			return this.SynchronizedObject.PredictedAngularVelocity.magnitude;
		}
	}

	// Token: 0x1700005F RID: 95
	// (get) Token: 0x0600030C RID: 780 RVA: 0x00012BF3 File Offset: 0x00010DF3
	// (set) Token: 0x0600030D RID: 781 RVA: 0x00012BFB File Offset: 0x00010DFB
	[HideInInspector]
	public float ShotSpeed { get; private set; }

	// Token: 0x17000060 RID: 96
	// (get) Token: 0x0600030E RID: 782 RVA: 0x00012C04 File Offset: 0x00010E04
	// (set) Token: 0x0600030F RID: 783 RVA: 0x00012C0C File Offset: 0x00010E0C
	[HideInInspector]
	public bool IsGrounded { get; private set; }

	// Token: 0x17000061 RID: 97
	// (get) Token: 0x06000310 RID: 784 RVA: 0x00012C15 File Offset: 0x00010E15
	[HideInInspector]
	public SphereCollider NetSphereCollider
	{
		get
		{
			return this.netSphereCollider;
		}
	}

	// Token: 0x17000062 RID: 98
	// (get) Token: 0x06000311 RID: 785 RVA: 0x00012C1D File Offset: 0x00010E1D
	[HideInInspector]
	public Collider StickCollider
	{
		get
		{
			return this.stickCollider;
		}
	}

	// Token: 0x17000063 RID: 99
	// (get) Token: 0x06000312 RID: 786 RVA: 0x00012C25 File Offset: 0x00010E25
	[HideInInspector]
	public Collider IceCollider
	{
		get
		{
			return this.iceCollider;
		}
	}

	// Token: 0x17000064 RID: 100
	// (get) Token: 0x06000313 RID: 787 RVA: 0x00012C2D File Offset: 0x00010E2D
	// (set) Token: 0x06000314 RID: 788 RVA: 0x00012C35 File Offset: 0x00010E35
	[HideInInspector]
	public Stick TouchingStick { get; private set; }

	// Token: 0x17000065 RID: 101
	// (get) Token: 0x06000315 RID: 789 RVA: 0x00012C3E File Offset: 0x00010E3E
	[HideInInspector]
	public bool IsTouchingStick
	{
		get
		{
			return this.TouchingStick != null;
		}
	}

	// Token: 0x17000066 RID: 102
	// (get) Token: 0x06000316 RID: 790 RVA: 0x00012C4C File Offset: 0x00010E4C
	[HideInInspector]
	public float MaxSpeed
	{
		get
		{
			return this.maxSpeed;
		}
	}

	// Token: 0x17000067 RID: 103
	// (get) Token: 0x06000317 RID: 791 RVA: 0x00012C54 File Offset: 0x00010E54
	[HideInInspector]
	public float MaxAngularSpeed
	{
		get
		{
			return this.maxAngularSpeed;
		}
	}

	// Token: 0x06000318 RID: 792 RVA: 0x00012C5C File Offset: 0x00010E5C
	private void Awake()
	{
		this.Rigidbody = base.GetComponent<Rigidbody>();
		this.SynchronizedObject = base.GetComponent<SynchronizedObject>();
		this.NetworkObjectCollisionRecorder = base.GetComponent<NetworkObjectCollisionRecorder>();
		this.CollisionRecorder = base.GetComponent<CollisionRecorder>();
		CollisionRecorder collisionRecorder = this.CollisionRecorder;
		collisionRecorder.CollisionDeferred = (Action<GameObject, float>)Delegate.Combine(collisionRecorder.CollisionDeferred, new Action<GameObject, float>(this.OnCollisionDeferred));
		this.NetSphereCollider.enabled = false;
	}

	// Token: 0x06000319 RID: 793 RVA: 0x00012CCC File Offset: 0x00010ECC
	private void FixedUpdate()
	{
		this.Speed = this.Rigidbody.linearVelocity.magnitude;
		this.AngularSpeed = this.Rigidbody.angularVelocity.magnitude;
		this.IsGrounded = Physics.CheckSphere(base.transform.position, this.groundedCheckSphereRadius, this.groundedCheckSphereLayerMask);
		if (this.IsGrounded)
		{
			this.Rigidbody.centerOfMass = base.transform.TransformVector(this.groundedCenterOfMass);
		}
		else
		{
			this.Rigidbody.centerOfMass = Vector3.zero;
		}
		float num = this.IsGrounded ? 0f : Mathf.Clamp(this.PredictedSpeed * 0.025f, 0.15f, 0.75f);
		if (this.NetSphereCollider.radius < num)
		{
			this.NetSphereCollider.radius = num;
		}
		else if (this.NetSphereCollider.radius > num)
		{
			this.NetSphereCollider.radius = Mathf.Lerp(this.NetSphereCollider.radius, num, Time.fixedDeltaTime * 5f);
		}
		if (this.IsTouchingStick)
		{
			this.Server_UpdateStickTensor(this.stickTensor, Quaternion.identity);
			this.TouchingStick = null;
		}
		else
		{
			this.Server_UpdateStickTensor(this.defaultTensor, Quaternion.identity);
		}
		this.Server_UpdateAudio();
	}

	// Token: 0x0600031A RID: 794 RVA: 0x00012E1D File Offset: 0x0001101D
	protected override void OnNetworkPreSpawn(ref NetworkManager networkManager)
	{
		this.InitializeNetworkVariables(false);
		base.OnNetworkPreSpawn(ref networkManager);
	}

	// Token: 0x0600031B RID: 795 RVA: 0x00012E2D File Offset: 0x0001102D
	public override void OnNetworkSpawn()
	{
		NetworkVariable<bool> isReplay = this.IsReplay;
		isReplay.OnValueChanged = (NetworkVariable<bool>.OnValueChangedDelegate)Delegate.Combine(isReplay.OnValueChanged, new NetworkVariable<bool>.OnValueChangedDelegate(this.OnIsReplayChanged));
		base.OnNetworkSpawn();
	}

	// Token: 0x0600031C RID: 796 RVA: 0x00012E5C File Offset: 0x0001105C
	protected override void OnNetworkPostSpawn()
	{
		EventManager.TriggerEvent("Event_Everyone_OnPuckSpawned", new Dictionary<string, object>
		{
			{
				"puck",
				this
			}
		});
		if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsConnectedClient)
		{
			this.ProcessInitialNetworkVariableValues();
		}
		base.OnNetworkPostSpawn();
	}

	// Token: 0x0600031D RID: 797 RVA: 0x00012EA8 File Offset: 0x000110A8
	protected override void OnNetworkSessionSynchronized()
	{
		this.ProcessInitialNetworkVariableValues();
		base.OnNetworkSessionSynchronized();
	}

	// Token: 0x0600031E RID: 798 RVA: 0x00012EB8 File Offset: 0x000110B8
	public override void OnNetworkDespawn()
	{
		EventManager.TriggerEvent("Event_Everyone_OnPuckDespawned", new Dictionary<string, object>
		{
			{
				"puck",
				this
			}
		});
		NetworkVariable<bool> isReplay = this.IsReplay;
		isReplay.OnValueChanged = (NetworkVariable<bool>.OnValueChangedDelegate)Delegate.Remove(isReplay.OnValueChanged, new NetworkVariable<bool>.OnValueChangedDelegate(this.OnIsReplayChanged));
		base.OnNetworkDespawn();
	}

	// Token: 0x0600031F RID: 799 RVA: 0x00012F0D File Offset: 0x0001110D
	public override void OnDestroy()
	{
		CollisionRecorder collisionRecorder = this.CollisionRecorder;
		collisionRecorder.CollisionDeferred = (Action<GameObject, float>)Delegate.Remove(collisionRecorder.CollisionDeferred, new Action<GameObject, float>(this.OnCollisionDeferred));
		base.transform.DOKill(false);
	}

	// Token: 0x06000320 RID: 800 RVA: 0x00012F43 File Offset: 0x00011143
	public void InitializeNetworkVariables(bool isReplay = false)
	{
		if (this.isNetworkVariablesInitialized)
		{
			return;
		}
		this.isNetworkVariablesInitialized = true;
		this.IsReplay = new NetworkVariable<bool>(isReplay, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
	}

	// Token: 0x06000321 RID: 801 RVA: 0x00012F63 File Offset: 0x00011163
	private void ProcessInitialNetworkVariableValues()
	{
		this.OnIsReplayChanged(false, this.IsReplay.Value);
	}

	// Token: 0x06000322 RID: 802 RVA: 0x00012F77 File Offset: 0x00011177
	public void Server_Freeze()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
	}

	// Token: 0x06000323 RID: 803 RVA: 0x00012F93 File Offset: 0x00011193
	public void Server_Unfreeze()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.Rigidbody.constraints = RigidbodyConstraints.None;
	}

	// Token: 0x06000324 RID: 804 RVA: 0x00012FB0 File Offset: 0x000111B0
	public List<KeyValuePair<Player, float>> GetPlayerCollisions()
	{
		List<KeyValuePair<Player, float>> list = this.NetworkObjectCollisionRecorder.NetworkObjectCollisions.Select(delegate(NetworkObjectCollision collision)
		{
			NetworkObject networkObject;
			if (collision.NetworkObjectReference.TryGet(out networkObject, null))
			{
				PlayerBody playerBody;
				networkObject.TryGetComponent<PlayerBody>(out playerBody);
				Stick stick;
				networkObject.TryGetComponent<Stick>(out stick);
				if (playerBody)
				{
					return new KeyValuePair<Player, float>(playerBody.Player, collision.Time);
				}
				if (stick)
				{
					return new KeyValuePair<Player, float>(stick.Player, collision.Time);
				}
			}
			return new KeyValuePair<Player, float>(null, collision.Time);
		}).ToList<KeyValuePair<Player, float>>();
		list.RemoveAll((KeyValuePair<Player, float> collision) => collision.Key == null);
		return list;
	}

	// Token: 0x06000325 RID: 805 RVA: 0x00013018 File Offset: 0x00011218
	public List<KeyValuePair<Player, float>> GetPlayerCollisionsByTeam(PlayerTeam team)
	{
		return this.GetPlayerCollisions().Where(delegate(KeyValuePair<Player, float> collision)
		{
			Player key = collision.Key;
			return key != null && key.Team == team;
		}).ToList<KeyValuePair<Player, float>>();
	}

	// Token: 0x06000326 RID: 806 RVA: 0x0001304E File Offset: 0x0001124E
	private void Server_UpdateStickTensor(Vector3 inertiaTensor, Quaternion inertiaTensorRotation)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.Rigidbody.inertiaTensor = inertiaTensor;
		this.Rigidbody.inertiaTensorRotation = inertiaTensorRotation;
	}

	// Token: 0x06000327 RID: 807 RVA: 0x00013078 File Offset: 0x00011278
	private void Server_UpdateAudio()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		float time = Mathf.Min(this.Speed / this.MaxSpeed, 1f);
		this.windAudioSource.Server_SetVolume(this.windVolumeCurve.Evaluate(time));
		float time2 = Mathf.Min(this.Speed / this.MaxSpeed, 1f);
		this.windAudioSource.Server_SetPitch(this.windPitchCurve.Evaluate(time2));
	}

	// Token: 0x06000328 RID: 808 RVA: 0x000130F0 File Offset: 0x000112F0
	private void OnIsReplayChanged(bool oldIsReplay, bool newIsReplay)
	{
		if (newIsReplay)
		{
			this.Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
			this.Rigidbody.interpolation = RigidbodyInterpolation.None;
		}
		EventManager.TriggerEvent("Event_Everyone_OnPuckIsReplayChanged", new Dictionary<string, object>
		{
			{
				"puck",
				this
			},
			{
				"oldIsReplay",
				oldIsReplay
			},
			{
				"newIsReplay",
				newIsReplay
			}
		});
	}

	// Token: 0x06000329 RID: 809 RVA: 0x00013158 File Offset: 0x00011358
	private void OnCollisionDeferred(GameObject gameObject, float force)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (!gameObject)
		{
			return;
		}
		string a = LayerMask.LayerToName(gameObject.layer);
		if (a == "Goal Post")
		{
			this.hitGoalPostAudioSource.Server_Play(this.hitGoalPostVolumeCurve.Evaluate(force), this.hitGoalPostPitchCurve.Evaluate(force), true, -1, 0f, true, false, false, 0f, false, 0f, -1f);
			return;
		}
		if (!(a == "Boards"))
		{
			this.hitIceAudioSource.Server_Play(this.hitIceVolumeCurve.Evaluate(force), this.hitIcePitchCurve.Evaluate(force), true, -1, 0f, true, false, false, 0f, false, 0f, -1f);
			return;
		}
		this.hitBoardsAudioSource.Server_Play(this.hitBoardsVolumeCurve.Evaluate(force), this.hitBoardsPitchCurve.Evaluate(force), true, -1, 0f, true, false, false, 0f, false, 0f, -1f);
	}

	// Token: 0x0600032A RID: 810 RVA: 0x0001325C File Offset: 0x0001145C
	private void OnCollisionEnter(Collision collision)
	{
		Stick component = collision.gameObject.GetComponent<Stick>();
		if (component)
		{
			this.TouchingStick = component;
			this.ShotSpeed = 0f;
		}
		if (this.IsGrounded)
		{
			return;
		}
		string a = LayerMask.LayerToName(collision.gameObject.layer);
		Vector3 a2 = Vector3.zero;
		int num = 0;
		foreach (ContactPoint contactPoint in collision.contacts)
		{
			a2 += contactPoint.normal;
			num++;
		}
		a2 /= (float)num;
		float t = Mathf.Abs(Vector3.Dot(collision.relativeVelocity.normalized, a2.normalized));
		if (!(a == "Goal Net"))
		{
			a == "Goal Post";
			return;
		}
		if (this.Rigidbody.linearVelocity.magnitude > this.goalNetLinearVelocityMaximumMagnitude)
		{
			Vector3 b = this.Rigidbody.linearVelocity.normalized * this.goalNetLinearVelocityMaximumMagnitude;
			b.y = 0f;
			this.Rigidbody.linearVelocity = Vector3.Lerp(this.Rigidbody.linearVelocity, b, t);
		}
		if (this.Rigidbody.angularVelocity.magnitude > this.goalNetAngularVelocityMaximumMagnitude)
		{
			Vector3 b2 = this.Rigidbody.angularVelocity.normalized * this.goalNetAngularVelocityMaximumMagnitude;
			this.Rigidbody.angularVelocity = Vector3.Lerp(this.Rigidbody.angularVelocity, b2, t);
		}
	}

	// Token: 0x0600032B RID: 811 RVA: 0x000133F4 File Offset: 0x000115F4
	private void OnCollisionStay(Collision collision)
	{
		Stick component = collision.gameObject.GetComponent<Stick>();
		if (!component)
		{
			return;
		}
		this.TouchingStick = component;
	}

	// Token: 0x0600032C RID: 812 RVA: 0x00013420 File Offset: 0x00011620
	private void OnCollisionExit(Collision collision)
	{
		if (!collision.gameObject.GetComponent<Stick>())
		{
			return;
		}
		this.ShotSpeed = this.Speed;
		this.Rigidbody.linearVelocity = Vector3.ClampMagnitude(this.Rigidbody.linearVelocity, this.MaxSpeed);
		this.Rigidbody.angularVelocity = Vector3.ClampMagnitude(this.Rigidbody.angularVelocity, this.MaxAngularSpeed);
		Vector3 force = new Vector3(0f, Mathf.Min(0f, -this.Rigidbody.linearVelocity.y), 0f) * 5f;
		this.Rigidbody.AddForce(force, ForceMode.Acceleration);
	}

	// Token: 0x0600032D RID: 813 RVA: 0x000134D0 File Offset: 0x000116D0
	public void OnDrawGizmos()
	{
		if (!Application.isEditor)
		{
			return;
		}
		Gizmos.color = Color.black;
		Gizmos.DrawWireSphere(base.transform.position, this.groundedCheckSphereRadius);
	}

	// Token: 0x0600032F RID: 815 RVA: 0x00013594 File Offset: 0x00011794
	protected override void __initializeVariables()
	{
		bool flag = this.IsReplay == null;
		if (flag)
		{
			throw new Exception("Puck.IsReplay cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.IsReplay.Initialize(this);
		base.__nameNetworkVariable(this.IsReplay, "IsReplay");
		this.NetworkVariableFields.Add(this.IsReplay);
		base.__initializeVariables();
	}

	// Token: 0x06000330 RID: 816 RVA: 0x000021BE File Offset: 0x000003BE
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x06000331 RID: 817 RVA: 0x000135F7 File Offset: 0x000117F7
	protected internal override string __getTypeName()
	{
		return "Puck";
	}

	// Token: 0x04000214 RID: 532
	[Header("Settings")]
	[SerializeField]
	private float maxSpeed = 30f;

	// Token: 0x04000215 RID: 533
	[SerializeField]
	private float maxAngularSpeed = 30f;

	// Token: 0x04000216 RID: 534
	[Space(20f)]
	[SerializeField]
	private Vector3 stickTensor = new Vector3(0.006f, 0.002f, 0.006f);

	// Token: 0x04000217 RID: 535
	[SerializeField]
	private Vector3 defaultTensor = new Vector3(0.002f, 0.002f, 0.002f);

	// Token: 0x04000218 RID: 536
	[Space(20f)]
	[SerializeField]
	private float groundedCheckSphereRadius = 0.075f;

	// Token: 0x04000219 RID: 537
	[SerializeField]
	private LayerMask groundedCheckSphereLayerMask;

	// Token: 0x0400021A RID: 538
	[Space(20f)]
	[SerializeField]
	private Vector3 groundedCenterOfMass = new Vector3(0f, -0.01f, 0f);

	// Token: 0x0400021B RID: 539
	[Space(20f)]
	[SerializeField]
	private float goalNetLinearVelocityMaximumMagnitude = 2f;

	// Token: 0x0400021C RID: 540
	[SerializeField]
	private float goalNetAngularVelocityMaximumMagnitude = 2f;

	// Token: 0x0400021D RID: 541
	[Space(20f)]
	[SerializeField]
	private AnimationCurve hitIceVolumeCurve;

	// Token: 0x0400021E RID: 542
	[SerializeField]
	private AnimationCurve hitIcePitchCurve;

	// Token: 0x0400021F RID: 543
	[SerializeField]
	private AnimationCurve hitBoardsVolumeCurve;

	// Token: 0x04000220 RID: 544
	[SerializeField]
	private AnimationCurve hitBoardsPitchCurve;

	// Token: 0x04000221 RID: 545
	[SerializeField]
	private AnimationCurve hitGoalPostVolumeCurve;

	// Token: 0x04000222 RID: 546
	[SerializeField]
	private AnimationCurve hitGoalPostPitchCurve;

	// Token: 0x04000223 RID: 547
	[SerializeField]
	private AnimationCurve windVolumeCurve;

	// Token: 0x04000224 RID: 548
	[SerializeField]
	private AnimationCurve windPitchCurve;

	// Token: 0x04000225 RID: 549
	[Header("References")]
	[SerializeField]
	private PuckElevationIndicator verticalityIndicator;

	// Token: 0x04000226 RID: 550
	[SerializeField]
	private SphereCollider netSphereCollider;

	// Token: 0x04000227 RID: 551
	[SerializeField]
	private Collider stickCollider;

	// Token: 0x04000228 RID: 552
	[SerializeField]
	private Collider iceCollider;

	// Token: 0x04000229 RID: 553
	[Space(20f)]
	[SerializeField]
	private SynchronizedAudio hitIceAudioSource;

	// Token: 0x0400022A RID: 554
	[SerializeField]
	private SynchronizedAudio hitBoardsAudioSource;

	// Token: 0x0400022B RID: 555
	[SerializeField]
	private SynchronizedAudio hitGoalPostAudioSource;

	// Token: 0x0400022C RID: 556
	[SerializeField]
	private SynchronizedAudio windAudioSource;

	// Token: 0x0400022D RID: 557
	[HideInInspector]
	public NetworkVariable<bool> IsReplay;

	// Token: 0x0400022E RID: 558
	[HideInInspector]
	public Rigidbody Rigidbody;

	// Token: 0x0400022F RID: 559
	[HideInInspector]
	public SynchronizedObject SynchronizedObject;

	// Token: 0x04000230 RID: 560
	[HideInInspector]
	public NetworkObjectCollisionRecorder NetworkObjectCollisionRecorder;

	// Token: 0x04000231 RID: 561
	[HideInInspector]
	public CollisionRecorder CollisionRecorder;

	// Token: 0x04000232 RID: 562
	[HideInInspector]
	public float Speed;

	// Token: 0x04000233 RID: 563
	[HideInInspector]
	public float AngularSpeed;

	// Token: 0x04000237 RID: 567
	private bool isNetworkVariablesInitialized;
}
