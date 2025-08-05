using System;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;

// Token: 0x020000F4 RID: 244
public class Stick : NetworkBehaviour
{
	// Token: 0x170000BD RID: 189
	// (get) Token: 0x06000841 RID: 2113 RVA: 0x0000C073 File Offset: 0x0000A273
	[HideInInspector]
	public PlayerBodyV2 PlayerBody
	{
		get
		{
			return this.Player.PlayerBody;
		}
	}

	// Token: 0x170000BE RID: 190
	// (get) Token: 0x06000842 RID: 2114 RVA: 0x0000C080 File Offset: 0x0000A280
	[HideInInspector]
	public StickPositioner StickPositioner
	{
		get
		{
			return this.Player.StickPositioner;
		}
	}

	// Token: 0x170000BF RID: 191
	// (get) Token: 0x06000843 RID: 2115 RVA: 0x0000C08D File Offset: 0x0000A28D
	[HideInInspector]
	public StickMesh StickMesh
	{
		get
		{
			return this.stickMesh;
		}
	}

	// Token: 0x170000C0 RID: 192
	// (get) Token: 0x06000844 RID: 2116 RVA: 0x0000C095 File Offset: 0x0000A295
	[HideInInspector]
	public Vector3 ShaftHandleLocalPosition
	{
		get
		{
			return this.shaftHandle.transform.localPosition;
		}
	}

	// Token: 0x170000C1 RID: 193
	// (get) Token: 0x06000845 RID: 2117 RVA: 0x0000C0A7 File Offset: 0x0000A2A7
	[HideInInspector]
	public Vector3 BladeHandleLocalPosition
	{
		get
		{
			return this.bladeHandle.transform.localPosition;
		}
	}

	// Token: 0x170000C2 RID: 194
	// (get) Token: 0x06000846 RID: 2118 RVA: 0x0000C0B9 File Offset: 0x0000A2B9
	[HideInInspector]
	public Vector3 ShaftHandlePosition
	{
		get
		{
			return this.shaftHandle.transform.position;
		}
	}

	// Token: 0x170000C3 RID: 195
	// (get) Token: 0x06000847 RID: 2119 RVA: 0x0000C0CB File Offset: 0x0000A2CB
	[HideInInspector]
	public Vector3 BladeHandlePosition
	{
		get
		{
			return this.bladeHandle.transform.position;
		}
	}

	// Token: 0x06000848 RID: 2120 RVA: 0x0000C0DD File Offset: 0x0000A2DD
	private void Awake()
	{
		this.Rigidbody = base.GetComponent<Rigidbody>();
		this.NetworkObjectCollisionBuffer = base.GetComponent<NetworkObjectCollisionBuffer>();
		this.Length = Vector3.Distance(this.ShaftHandlePosition, this.BladeHandlePosition);
	}

	// Token: 0x06000849 RID: 2121 RVA: 0x00033FDC File Offset: 0x000321DC
	protected override void OnNetworkPostSpawn()
	{
		NetworkObject networkObject;
		if (this.PlayerReference.Value.TryGet(out networkObject, null))
		{
			this.Player = networkObject.GetComponent<Player>();
		}
		if (this.Player)
		{
			this.Player.Stick = this;
			this.UpdateStick();
			if (this.Player.IsReplay.Value)
			{
				this.Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
				this.Rigidbody.interpolation = RigidbodyInterpolation.None;
			}
		}
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnStickSpawned", new Dictionary<string, object>
		{
			{
				"stick",
				this
			}
		});
		base.OnNetworkPostSpawn();
	}

	// Token: 0x0600084A RID: 2122 RVA: 0x0000C10E File Offset: 0x0000A30E
	public override void OnNetworkDespawn()
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnStickDespawned", new Dictionary<string, object>
		{
			{
				"stick",
				this
			}
		});
		base.OnNetworkDespawn();
	}

	// Token: 0x0600084B RID: 2123 RVA: 0x0000C136 File Offset: 0x0000A336
	public override void OnDestroy()
	{
		base.transform.DOKill(false);
		base.OnDestroy();
	}

	// Token: 0x0600084C RID: 2124 RVA: 0x00034080 File Offset: 0x00032280
	private void FixedUpdate()
	{
		if (!this.Player || !this.StickPositioner)
		{
			return;
		}
		PlayerInput playerInput = this.Player.PlayerInput;
		if (!playerInput)
		{
			return;
		}
		float angle = (float)playerInput.BladeAngleInput.ServerValue * this.bladeAngleStep;
		this.rotationContainer.transform.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.shaftHandlePIDController.proportionalGain = this.shaftHandleProportionalGain * this.shaftHandleProportionalGainMultiplier;
		this.shaftHandlePIDController.integralGain = this.shaftHandleIntegralGain;
		this.shaftHandlePIDController.integralSaturation = this.shaftHandleIntegralSaturation;
		this.shaftHandlePIDController.derivativeGain = this.shaftHandleDerivativeGain;
		this.shaftHandlePIDController.derivativeSmoothing = this.shaftHandleDerivativeSmoothing;
		this.bladeHandlePIDController.proportionalGain = this.bladeHandleProportionalGain * this.bladeHandleProportionalGainMultiplier;
		this.bladeHandlePIDController.integralGain = this.bladeHandleIntegralGain;
		this.bladeHandlePIDController.integralSaturation = this.bladeHandleIntegralSaturation;
		this.bladeHandlePIDController.derivativeGain = this.bladeHandleDerivativeGain;
		this.bladeHandlePIDController.derivativeSmoothing = this.bladeHandleDerivativeSmoothing;
		Vector3 a = this.shaftHandlePIDController.Update(Time.fixedDeltaTime, this.ShaftHandlePosition, this.StickPositioner.ShaftTargetPosition);
		Vector3 a2 = this.bladeHandlePIDController.Update(Time.fixedDeltaTime, this.BladeHandlePosition, this.StickPositioner.BladeTargetPosition);
		this.Rigidbody.AddForceAtPosition(this.PlayerBody.Rigidbody.GetPointVelocity(this.shaftHandle.transform.position) * this.linearVelocityTransferMultiplier * Time.fixedDeltaTime, this.shaftHandle.transform.position, ForceMode.VelocityChange);
		this.Rigidbody.AddForceAtPosition(this.PlayerBody.Rigidbody.GetPointVelocity(this.bladeHandle.transform.position) * this.linearVelocityTransferMultiplier * Time.fixedDeltaTime, this.bladeHandle.transform.position, ForceMode.VelocityChange);
		this.Rigidbody.AddForceAtPosition(a * Time.fixedDeltaTime, this.ShaftHandlePosition, ForceMode.VelocityChange);
		this.Rigidbody.AddForceAtPosition(a2 * Time.fixedDeltaTime, this.BladeHandlePosition, ForceMode.VelocityChange);
		Vector3 direction = base.transform.InverseTransformVector(this.Rigidbody.angularVelocity);
		direction.z = 0f;
		this.Rigidbody.angularVelocity = base.transform.TransformDirection(direction);
		Vector3 vector = Utils.WrapEulerAngles(base.transform.eulerAngles);
		Quaternion rot = Quaternion.Euler(new Vector3(vector.x, vector.y, 0f));
		this.Rigidbody.MoveRotation(rot);
		Vector3 a3 = Vector3.Scale(this.Rigidbody.angularVelocity, new Vector3(0.5f, 1f, 0f)) * this.angularVelocityTransferMultiplier;
		if (this.transferAngularVelocity)
		{
			this.PlayerBody.Rigidbody.AddTorque(-a3, ForceMode.Acceleration);
		}
		this.bladeHandleProportionalGainMultiplier = 1f;
	}

	// Token: 0x0600084D RID: 2125 RVA: 0x000343A8 File Offset: 0x000325A8
	public void Teleport(Vector3 position, Quaternion rotation)
	{
		base.transform.position = position;
		base.transform.rotation = rotation;
		this.Rigidbody.position = position;
		this.Rigidbody.rotation = rotation;
		this.Rigidbody.linearVelocity = Vector3.zero;
		this.Rigidbody.angularVelocity = Vector3.zero;
		this.shaftHandlePIDController.Reset();
		this.bladeHandlePIDController.Reset();
	}

	// Token: 0x0600084E RID: 2126 RVA: 0x0000C14B File Offset: 0x0000A34B
	public void Server_Freeze()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (this.Player.IsReplay.Value)
		{
			return;
		}
		this.Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
	}

	// Token: 0x0600084F RID: 2127 RVA: 0x0000C17A File Offset: 0x0000A37A
	public void Server_Unfreeze()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (this.Player.IsReplay.Value)
		{
			return;
		}
		this.Rigidbody.constraints = RigidbodyConstraints.None;
	}

	// Token: 0x06000850 RID: 2128 RVA: 0x0003441C File Offset: 0x0003261C
	public void UpdateStick()
	{
		if (!this.Player)
		{
			return;
		}
		this.stickMesh.SetSkin(this.Player.Team.Value, this.Player.GetPlayerStickSkin().ToString());
		this.stickMesh.SetShaftTape(this.Player.GetPlayerStickShaftTapeSkin().ToString());
		this.stickMesh.SetBladeTape(this.Player.GetPlayerStickBladeTapeSkin().ToString());
	}

	// Token: 0x06000851 RID: 2129 RVA: 0x000344B4 File Offset: 0x000326B4
	private void OnCollisionStay(Collision collision)
	{
		Stick component = collision.gameObject.GetComponent<Stick>();
		if (!component)
		{
			return;
		}
		if (collision.contacts.Length == 0)
		{
			return;
		}
		ContactPoint contactPoint = collision.contacts[0];
		Component thisCollider = contactPoint.thisCollider;
		Collider otherCollider = contactPoint.otherCollider;
		if (thisCollider.tag != "Stick Blade" || otherCollider.tag != "Stick Shaft")
		{
			return;
		}
		Vector3 point = contactPoint.point;
		float num = Mathf.Clamp(Vector3.Distance(component.ShaftHandlePosition, point) / this.Length, this.minShaftHandleProportionalGainMultiplier, 1f);
		this.bladeHandleProportionalGainMultiplier = num;
	}

	// Token: 0x06000853 RID: 2131 RVA: 0x00034640 File Offset: 0x00032840
	protected override void __initializeVariables()
	{
		bool flag = this.PlayerReference == null;
		if (flag)
		{
			throw new Exception("Stick.PlayerReference cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.PlayerReference.Initialize(this);
		base.__nameNetworkVariable(this.PlayerReference, "PlayerReference");
		this.NetworkVariableFields.Add(this.PlayerReference);
		base.__initializeVariables();
	}

	// Token: 0x06000854 RID: 2132 RVA: 0x00006C33 File Offset: 0x00004E33
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x06000855 RID: 2133 RVA: 0x0000C1A8 File Offset: 0x0000A3A8
	protected internal override string __getTypeName()
	{
		return "Stick";
	}

	// Token: 0x040004D6 RID: 1238
	[Header("References")]
	[SerializeField]
	private GameObject shaftHandle;

	// Token: 0x040004D7 RID: 1239
	[SerializeField]
	private GameObject bladeHandle;

	// Token: 0x040004D8 RID: 1240
	[SerializeField]
	private GameObject rotationContainer;

	// Token: 0x040004D9 RID: 1241
	[SerializeField]
	private StickMesh stickMesh;

	// Token: 0x040004DA RID: 1242
	[Header("Settings")]
	[SerializeField]
	private float bladeAngleStep = 12.5f;

	// Token: 0x040004DB RID: 1243
	[Space(20f)]
	[SerializeField]
	private bool transferAngularVelocity = true;

	// Token: 0x040004DC RID: 1244
	[SerializeField]
	private float angularVelocityTransferMultiplier = 0.25f;

	// Token: 0x040004DD RID: 1245
	[Space(20f)]
	[SerializeField]
	private float shaftHandleProportionalGain = 500f;

	// Token: 0x040004DE RID: 1246
	[SerializeField]
	private float shaftHandleIntegralGain;

	// Token: 0x040004DF RID: 1247
	[SerializeField]
	private float shaftHandleIntegralSaturation;

	// Token: 0x040004E0 RID: 1248
	[SerializeField]
	private float shaftHandleDerivativeGain = 20f;

	// Token: 0x040004E1 RID: 1249
	[SerializeField]
	private float shaftHandleDerivativeSmoothing = 0.1f;

	// Token: 0x040004E2 RID: 1250
	[SerializeField]
	private float minShaftHandleProportionalGainMultiplier = 0.25f;

	// Token: 0x040004E3 RID: 1251
	[Space(20f)]
	[SerializeField]
	private float bladeHandleProportionalGain = 500f;

	// Token: 0x040004E4 RID: 1252
	[SerializeField]
	private float bladeHandleIntegralGain;

	// Token: 0x040004E5 RID: 1253
	[SerializeField]
	private float bladeHandleIntegralSaturation;

	// Token: 0x040004E6 RID: 1254
	[SerializeField]
	private float bladeHandleDerivativeGain = 20f;

	// Token: 0x040004E7 RID: 1255
	[SerializeField]
	private float bladeHandleDerivativeSmoothing = 0.1f;

	// Token: 0x040004E8 RID: 1256
	[Space(20f)]
	[SerializeField]
	private float linearVelocityTransferMultiplier = 0.25f;

	// Token: 0x040004E9 RID: 1257
	[HideInInspector]
	public NetworkVariable<NetworkObjectReference> PlayerReference = new NetworkVariable<NetworkObjectReference>(default(NetworkObjectReference), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x040004EA RID: 1258
	[HideInInspector]
	public Player Player;

	// Token: 0x040004EB RID: 1259
	[HideInInspector]
	public Rigidbody Rigidbody;

	// Token: 0x040004EC RID: 1260
	[HideInInspector]
	public NetworkObjectCollisionBuffer NetworkObjectCollisionBuffer;

	// Token: 0x040004ED RID: 1261
	[HideInInspector]
	public float Length;

	// Token: 0x040004EE RID: 1262
	private Vector3PIDController shaftHandlePIDController = new Vector3PIDController(0f, 0f, 0f);

	// Token: 0x040004EF RID: 1263
	private Vector3PIDController bladeHandlePIDController = new Vector3PIDController(0f, 0f, 0f);

	// Token: 0x040004F0 RID: 1264
	private float shaftHandleProportionalGainMultiplier = 1f;

	// Token: 0x040004F1 RID: 1265
	private float bladeHandleProportionalGainMultiplier = 1f;
}
