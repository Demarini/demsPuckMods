using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;

// Token: 0x02000022 RID: 34
public class Puck : NetworkBehaviour
{
	// Token: 0x17000010 RID: 16
	// (get) Token: 0x060000E7 RID: 231 RVA: 0x0000769B File Offset: 0x0000589B
	[HideInInspector]
	public float PredictedSpeed
	{
		get
		{
			return this.SynchronizedObject.PredictedLinearVelocity.magnitude;
		}
	}

	// Token: 0x17000011 RID: 17
	// (get) Token: 0x060000E8 RID: 232 RVA: 0x000076AD File Offset: 0x000058AD
	[HideInInspector]
	public float PredictedAngularSpeed
	{
		get
		{
			return this.SynchronizedObject.PredictedAngularVelocity.magnitude;
		}
	}

	// Token: 0x17000012 RID: 18
	// (get) Token: 0x060000E9 RID: 233 RVA: 0x000076BF File Offset: 0x000058BF
	// (set) Token: 0x060000EA RID: 234 RVA: 0x000076C7 File Offset: 0x000058C7
	[HideInInspector]
	public float ShotSpeed { get; private set; }

	// Token: 0x17000013 RID: 19
	// (get) Token: 0x060000EB RID: 235 RVA: 0x000076D0 File Offset: 0x000058D0
	// (set) Token: 0x060000EC RID: 236 RVA: 0x000076D8 File Offset: 0x000058D8
	[HideInInspector]
	public bool IsGrounded { get; private set; }

	// Token: 0x17000014 RID: 20
	// (get) Token: 0x060000ED RID: 237 RVA: 0x000076E1 File Offset: 0x000058E1
	[HideInInspector]
	public SphereCollider NetSphereCollider
	{
		get
		{
			return this.netSphereCollider;
		}
	}

	// Token: 0x17000015 RID: 21
	// (get) Token: 0x060000EE RID: 238 RVA: 0x000076E9 File Offset: 0x000058E9
	[HideInInspector]
	public Collider StickCollider
	{
		get
		{
			return this.stickCollider;
		}
	}

	// Token: 0x17000016 RID: 22
	// (get) Token: 0x060000EF RID: 239 RVA: 0x000076F1 File Offset: 0x000058F1
	[HideInInspector]
	public Collider IceCollider
	{
		get
		{
			return this.iceCollider;
		}
	}

	// Token: 0x17000017 RID: 23
	// (get) Token: 0x060000F0 RID: 240 RVA: 0x000076F9 File Offset: 0x000058F9
	// (set) Token: 0x060000F1 RID: 241 RVA: 0x00007701 File Offset: 0x00005901
	[HideInInspector]
	public Stick TouchingStick { get; private set; }

	// Token: 0x17000018 RID: 24
	// (get) Token: 0x060000F2 RID: 242 RVA: 0x0000770A File Offset: 0x0000590A
	[HideInInspector]
	public bool IsTouchingStick
	{
		get
		{
			return this.TouchingStick != null;
		}
	}

	// Token: 0x17000019 RID: 25
	// (get) Token: 0x060000F3 RID: 243 RVA: 0x00007718 File Offset: 0x00005918
	[HideInInspector]
	public float MaxSpeed
	{
		get
		{
			return this.maxSpeed;
		}
	}

	// Token: 0x1700001A RID: 26
	// (get) Token: 0x060000F4 RID: 244 RVA: 0x00007720 File Offset: 0x00005920
	[HideInInspector]
	public float MaxAngularSpeed
	{
		get
		{
			return this.maxAngularSpeed;
		}
	}

	// Token: 0x060000F5 RID: 245 RVA: 0x000113CC File Offset: 0x0000F5CC
	private void Awake()
	{
		this.Rigidbody = base.GetComponent<Rigidbody>();
		this.SynchronizedObject = base.GetComponent<SynchronizedObject>();
		this.NetworkObjectCollisionBuffer = base.GetComponent<NetworkObjectCollisionBuffer>();
		this.CollisionRecorder = base.GetComponent<CollisionRecorder>();
		CollisionRecorder collisionRecorder = this.CollisionRecorder;
		collisionRecorder.OnDeferredCollision = (Action<GameObject, float>)Delegate.Combine(collisionRecorder.OnDeferredCollision, new Action<GameObject, float>(this.OnDeferredCollision));
		this.NetSphereCollider.enabled = false;
	}

	// Token: 0x060000F6 RID: 246 RVA: 0x0001143C File Offset: 0x0000F63C
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

	// Token: 0x060000F7 RID: 247 RVA: 0x00011590 File Offset: 0x0000F790
	protected override void OnNetworkPostSpawn()
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnPuckSpawned", new Dictionary<string, object>
		{
			{
				"puck",
				this
			}
		});
		if (this.IsReplay.Value)
		{
			this.Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
			this.Rigidbody.interpolation = RigidbodyInterpolation.None;
		}
		base.OnNetworkPostSpawn();
	}

	// Token: 0x060000F8 RID: 248 RVA: 0x00007728 File Offset: 0x00005928
	public override void OnNetworkDespawn()
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnPuckDespawned", new Dictionary<string, object>
		{
			{
				"puck",
				this
			}
		});
		base.OnNetworkDespawn();
	}

	// Token: 0x060000F9 RID: 249 RVA: 0x00007750 File Offset: 0x00005950
	public override void OnDestroy()
	{
		CollisionRecorder collisionRecorder = this.CollisionRecorder;
		collisionRecorder.OnDeferredCollision = (Action<GameObject, float>)Delegate.Remove(collisionRecorder.OnDeferredCollision, new Action<GameObject, float>(this.OnDeferredCollision));
		base.transform.DOKill(false);
		base.OnDestroy();
	}

	// Token: 0x060000FA RID: 250 RVA: 0x0000778C File Offset: 0x0000598C
	public void Server_Freeze()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (this.IsReplay.Value)
		{
			return;
		}
		this.Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
	}

	// Token: 0x060000FB RID: 251 RVA: 0x000077B6 File Offset: 0x000059B6
	public void Server_Unfreeze()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (this.IsReplay.Value)
		{
			return;
		}
		this.Rigidbody.constraints = RigidbodyConstraints.None;
	}

	// Token: 0x060000FC RID: 252 RVA: 0x000115EC File Offset: 0x0000F7EC
	public List<KeyValuePair<Player, float>> GetPlayerCollisions()
	{
		List<KeyValuePair<Player, float>> list = this.NetworkObjectCollisionBuffer.Buffer.Select(delegate(NetworkObjectCollision collision)
		{
			NetworkObject networkObject;
			if (collision.NetworkObjectReference.TryGet(out networkObject, null))
			{
				PlayerBodyV2 playerBodyV;
				networkObject.TryGetComponent<PlayerBodyV2>(out playerBodyV);
				Stick stick;
				networkObject.TryGetComponent<Stick>(out stick);
				if (playerBodyV)
				{
					return new KeyValuePair<Player, float>(playerBodyV.Player, collision.Time);
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

	// Token: 0x060000FD RID: 253 RVA: 0x00011654 File Offset: 0x0000F854
	public List<KeyValuePair<Player, float>> GetPlayerCollisionsByTeam(PlayerTeam team)
	{
		return this.GetPlayerCollisions().Where(delegate(KeyValuePair<Player, float> collision)
		{
			Player key = collision.Key;
			return key != null && key.Team.Value == team;
		}).ToList<KeyValuePair<Player, float>>();
	}

	// Token: 0x060000FE RID: 254 RVA: 0x000077DF File Offset: 0x000059DF
	private void Server_UpdateStickTensor(Vector3 inertiaTensor, Quaternion inertiaTensorRotation)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.Rigidbody.inertiaTensor = inertiaTensor;
		this.Rigidbody.inertiaTensorRotation = inertiaTensorRotation;
	}

	// Token: 0x060000FF RID: 255 RVA: 0x0001168C File Offset: 0x0000F88C
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

	// Token: 0x06000100 RID: 256 RVA: 0x00011704 File Offset: 0x0000F904
	private void OnDeferredCollision(GameObject gameObject, float force)
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

	// Token: 0x06000101 RID: 257 RVA: 0x00011808 File Offset: 0x0000FA08
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

	// Token: 0x06000102 RID: 258 RVA: 0x000119A0 File Offset: 0x0000FBA0
	private void OnCollisionStay(Collision collision)
	{
		Stick component = collision.gameObject.GetComponent<Stick>();
		if (!component)
		{
			return;
		}
		this.TouchingStick = component;
	}

	// Token: 0x06000103 RID: 259 RVA: 0x000119CC File Offset: 0x0000FBCC
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

	// Token: 0x06000104 RID: 260 RVA: 0x00007806 File Offset: 0x00005A06
	public void OnDrawGizmos()
	{
		if (!Application.isEditor)
		{
			return;
		}
		Gizmos.color = Color.black;
		Gizmos.DrawWireSphere(base.transform.position, this.groundedCheckSphereRadius);
	}

	// Token: 0x06000106 RID: 262 RVA: 0x00011B24 File Offset: 0x0000FD24
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

	// Token: 0x06000107 RID: 263 RVA: 0x00006C33 File Offset: 0x00004E33
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x06000108 RID: 264 RVA: 0x00007830 File Offset: 0x00005A30
	protected internal override string __getTypeName()
	{
		return "Puck";
	}

	// Token: 0x04000069 RID: 105
	[Header("References")]
	[SerializeField]
	private PuckElevationIndicator verticalityIndicator;

	// Token: 0x0400006A RID: 106
	[SerializeField]
	private SphereCollider netSphereCollider;

	// Token: 0x0400006B RID: 107
	[SerializeField]
	private Collider stickCollider;

	// Token: 0x0400006C RID: 108
	[SerializeField]
	private Collider iceCollider;

	// Token: 0x0400006D RID: 109
	[Space(20f)]
	[SerializeField]
	private SynchronizedAudio hitIceAudioSource;

	// Token: 0x0400006E RID: 110
	[SerializeField]
	private SynchronizedAudio hitBoardsAudioSource;

	// Token: 0x0400006F RID: 111
	[SerializeField]
	private SynchronizedAudio hitGoalPostAudioSource;

	// Token: 0x04000070 RID: 112
	[SerializeField]
	private SynchronizedAudio windAudioSource;

	// Token: 0x04000071 RID: 113
	[Header("Settings")]
	[SerializeField]
	private float maxSpeed = 30f;

	// Token: 0x04000072 RID: 114
	[SerializeField]
	private float maxAngularSpeed = 30f;

	// Token: 0x04000073 RID: 115
	[Space(20f)]
	[SerializeField]
	private Vector3 stickTensor = new Vector3(0.006f, 0.002f, 0.006f);

	// Token: 0x04000074 RID: 116
	[SerializeField]
	private Vector3 defaultTensor = new Vector3(0.002f, 0.002f, 0.002f);

	// Token: 0x04000075 RID: 117
	[Space(20f)]
	[SerializeField]
	private float groundedCheckSphereRadius = 0.075f;

	// Token: 0x04000076 RID: 118
	[SerializeField]
	private LayerMask groundedCheckSphereLayerMask;

	// Token: 0x04000077 RID: 119
	[Space(20f)]
	[SerializeField]
	private Vector3 groundedCenterOfMass = new Vector3(0f, -0.01f, 0f);

	// Token: 0x04000078 RID: 120
	[Space(20f)]
	[SerializeField]
	private float goalNetLinearVelocityMaximumMagnitude = 2f;

	// Token: 0x04000079 RID: 121
	[SerializeField]
	private float goalNetAngularVelocityMaximumMagnitude = 2f;

	// Token: 0x0400007A RID: 122
	[Space(20f)]
	[SerializeField]
	private AnimationCurve hitIceVolumeCurve;

	// Token: 0x0400007B RID: 123
	[SerializeField]
	private AnimationCurve hitIcePitchCurve;

	// Token: 0x0400007C RID: 124
	[SerializeField]
	private AnimationCurve hitBoardsVolumeCurve;

	// Token: 0x0400007D RID: 125
	[SerializeField]
	private AnimationCurve hitBoardsPitchCurve;

	// Token: 0x0400007E RID: 126
	[SerializeField]
	private AnimationCurve hitGoalPostVolumeCurve;

	// Token: 0x0400007F RID: 127
	[SerializeField]
	private AnimationCurve hitGoalPostPitchCurve;

	// Token: 0x04000080 RID: 128
	[SerializeField]
	private AnimationCurve windVolumeCurve;

	// Token: 0x04000081 RID: 129
	[SerializeField]
	private AnimationCurve windPitchCurve;

	// Token: 0x04000082 RID: 130
	[HideInInspector]
	public NetworkVariable<bool> IsReplay = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x04000083 RID: 131
	[HideInInspector]
	public Rigidbody Rigidbody;

	// Token: 0x04000084 RID: 132
	[HideInInspector]
	public SynchronizedObject SynchronizedObject;

	// Token: 0x04000085 RID: 133
	[HideInInspector]
	public NetworkObjectCollisionBuffer NetworkObjectCollisionBuffer;

	// Token: 0x04000086 RID: 134
	[HideInInspector]
	public CollisionRecorder CollisionRecorder;

	// Token: 0x04000087 RID: 135
	[HideInInspector]
	public float Speed;

	// Token: 0x04000088 RID: 136
	[HideInInspector]
	public float AngularSpeed;
}
