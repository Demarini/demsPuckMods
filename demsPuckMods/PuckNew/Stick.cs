using System;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;

// Token: 0x02000050 RID: 80
public class Stick : NetworkBehaviour
{
	// Token: 0x1700004B RID: 75
	// (get) Token: 0x06000292 RID: 658 RVA: 0x00010CF8 File Offset: 0x0000EEF8
	[HideInInspector]
	public Player Player
	{
		get
		{
			NetworkObject networkObject;
			if (!this.PlayerReference.Value.TryGet(out networkObject, null))
			{
				return null;
			}
			return networkObject.GetComponent<Player>();
		}
	}

	// Token: 0x1700004C RID: 76
	// (get) Token: 0x06000293 RID: 659 RVA: 0x00010D25 File Offset: 0x0000EF25
	[HideInInspector]
	public PlayerBody PlayerBody
	{
		get
		{
			return this.Player.PlayerBody;
		}
	}

	// Token: 0x1700004D RID: 77
	// (get) Token: 0x06000294 RID: 660 RVA: 0x00010D32 File Offset: 0x0000EF32
	[HideInInspector]
	public StickPositioner StickPositioner
	{
		get
		{
			return this.Player.StickPositioner;
		}
	}

	// Token: 0x1700004E RID: 78
	// (get) Token: 0x06000295 RID: 661 RVA: 0x00010D3F File Offset: 0x0000EF3F
	[HideInInspector]
	public StickMesh StickMesh
	{
		get
		{
			return this.stickMesh;
		}
	}

	// Token: 0x1700004F RID: 79
	// (get) Token: 0x06000296 RID: 662 RVA: 0x00010D47 File Offset: 0x0000EF47
	[HideInInspector]
	public Vector3 ShaftHandleLocalPosition
	{
		get
		{
			return this.shaftHandle.transform.localPosition;
		}
	}

	// Token: 0x17000050 RID: 80
	// (get) Token: 0x06000297 RID: 663 RVA: 0x00010D59 File Offset: 0x0000EF59
	[HideInInspector]
	public Vector3 BladeHandleLocalPosition
	{
		get
		{
			return this.bladeHandle.transform.localPosition;
		}
	}

	// Token: 0x17000051 RID: 81
	// (get) Token: 0x06000298 RID: 664 RVA: 0x00010D6B File Offset: 0x0000EF6B
	[HideInInspector]
	public Vector3 ShaftHandlePosition
	{
		get
		{
			return this.shaftHandle.transform.position;
		}
	}

	// Token: 0x17000052 RID: 82
	// (get) Token: 0x06000299 RID: 665 RVA: 0x00010D7D File Offset: 0x0000EF7D
	[HideInInspector]
	public Vector3 BladeHandlePosition
	{
		get
		{
			return this.bladeHandle.transform.position;
		}
	}

	// Token: 0x0600029A RID: 666 RVA: 0x00010D8F File Offset: 0x0000EF8F
	private void Awake()
	{
		this.Rigidbody = base.GetComponent<Rigidbody>();
		this.NetworkObjectCollisionRecorder = base.GetComponent<NetworkObjectCollisionRecorder>();
		this.Length = Vector3.Distance(this.ShaftHandlePosition, this.BladeHandlePosition);
	}

	// Token: 0x0600029B RID: 667 RVA: 0x00010DC0 File Offset: 0x0000EFC0
	protected override void OnNetworkPreSpawn(ref NetworkManager networkManager)
	{
		this.InitializeNetworkVariables(default(NetworkObjectReference));
		base.OnNetworkPreSpawn(ref networkManager);
	}

	// Token: 0x0600029C RID: 668 RVA: 0x00010DE3 File Offset: 0x0000EFE3
	public override void OnNetworkSpawn()
	{
		NetworkVariable<NetworkObjectReference> playerReference = this.PlayerReference;
		playerReference.OnValueChanged = (NetworkVariable<NetworkObjectReference>.OnValueChangedDelegate)Delegate.Combine(playerReference.OnValueChanged, new NetworkVariable<NetworkObjectReference>.OnValueChangedDelegate(this.OnPlayerReferenceChanged));
		base.OnNetworkSpawn();
	}

	// Token: 0x0600029D RID: 669 RVA: 0x00010E14 File Offset: 0x0000F014
	protected override void OnNetworkPostSpawn()
	{
		EventManager.TriggerEvent("Event_Everyone_OnStickSpawned", new Dictionary<string, object>
		{
			{
				"stick",
				this
			}
		});
		if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsConnectedClient)
		{
			this.ProcessInitialNetworkVariableValues();
		}
		base.OnNetworkPostSpawn();
	}

	// Token: 0x0600029E RID: 670 RVA: 0x00010E60 File Offset: 0x0000F060
	protected override void OnNetworkSessionSynchronized()
	{
		this.ProcessInitialNetworkVariableValues();
		base.OnNetworkSessionSynchronized();
	}

	// Token: 0x0600029F RID: 671 RVA: 0x00010E70 File Offset: 0x0000F070
	public override void OnNetworkDespawn()
	{
		EventManager.TriggerEvent("Event_Everyone_OnStickDespawned", new Dictionary<string, object>
		{
			{
				"stick",
				this
			}
		});
		NetworkVariable<NetworkObjectReference> playerReference = this.PlayerReference;
		playerReference.OnValueChanged = (NetworkVariable<NetworkObjectReference>.OnValueChangedDelegate)Delegate.Remove(playerReference.OnValueChanged, new NetworkVariable<NetworkObjectReference>.OnValueChangedDelegate(this.OnPlayerReferenceChanged));
		base.OnNetworkDespawn();
	}

	// Token: 0x060002A0 RID: 672 RVA: 0x00010EC5 File Offset: 0x0000F0C5
	public override void OnDestroy()
	{
		base.transform.DOKill(false);
	}

	// Token: 0x060002A1 RID: 673 RVA: 0x00010ED4 File Offset: 0x0000F0D4
	public void InitializeNetworkVariables(NetworkObjectReference playerReference = default(NetworkObjectReference))
	{
		if (this.isNetworkVariablesInitialized)
		{
			return;
		}
		this.isNetworkVariablesInitialized = true;
		this.PlayerReference = new NetworkVariable<NetworkObjectReference>(playerReference, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
	}

	// Token: 0x060002A2 RID: 674 RVA: 0x00010EF4 File Offset: 0x0000F0F4
	private void ProcessInitialNetworkVariableValues()
	{
		this.OnPlayerReferenceChanged(default(NetworkObjectReference), this.PlayerReference.Value);
	}

	// Token: 0x060002A3 RID: 675 RVA: 0x00010F1C File Offset: 0x0000F11C
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

	// Token: 0x060002A4 RID: 676 RVA: 0x00011244 File Offset: 0x0000F444
	public void ApplyCustomizations()
	{
		if (this.Player.Team == PlayerTeam.None || this.Player.Role == PlayerRole.None)
		{
			return;
		}
		this.SetSkinID(this.Player.GetPlayerStickSkinID(), this.Player.Team);
		this.SetShaftTapeID(this.Player.GetPlayerStickShaftTapeID());
		this.SetBladeTapeID(this.Player.GetPlayerStickBladeTapeID());
	}

	// Token: 0x060002A5 RID: 677 RVA: 0x000112AC File Offset: 0x0000F4AC
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

	// Token: 0x060002A6 RID: 678 RVA: 0x0001131F File Offset: 0x0000F51F
	public void SetSkinID(int skinID, PlayerTeam team)
	{
		this.stickMesh.SetSkinID(skinID, team);
	}

	// Token: 0x060002A7 RID: 679 RVA: 0x0001132E File Offset: 0x0000F52E
	public void SetShaftTapeID(int shaftTapeID)
	{
		this.stickMesh.SetShaftTapeID(shaftTapeID);
	}

	// Token: 0x060002A8 RID: 680 RVA: 0x0001133C File Offset: 0x0000F53C
	public void SetBladeTapeID(int bladeTapeID)
	{
		this.stickMesh.SetBladeTapeID(bladeTapeID);
	}

	// Token: 0x060002A9 RID: 681 RVA: 0x0001134A File Offset: 0x0000F54A
	public void Server_Freeze()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
	}

	// Token: 0x060002AA RID: 682 RVA: 0x00011366 File Offset: 0x0000F566
	public void Server_Unfreeze()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.Rigidbody.constraints = RigidbodyConstraints.None;
	}

	// Token: 0x060002AB RID: 683 RVA: 0x00011384 File Offset: 0x0000F584
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

	// Token: 0x060002AC RID: 684 RVA: 0x00011428 File Offset: 0x0000F628
	private void OnPlayerReferenceChanged(NetworkObjectReference previousValue, NetworkObjectReference newValue)
	{
		if (!this.Player)
		{
			return;
		}
		this.Player.Stick = this;
		this.ApplyCustomizations();
		if (this.Player.IsReplay.Value)
		{
			this.Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
			this.Rigidbody.interpolation = RigidbodyInterpolation.None;
		}
	}

	// Token: 0x060002AE RID: 686 RVA: 0x00011554 File Offset: 0x0000F754
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

	// Token: 0x060002AF RID: 687 RVA: 0x000021BE File Offset: 0x000003BE
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x060002B0 RID: 688 RVA: 0x000115B7 File Offset: 0x0000F7B7
	protected internal override string __getTypeName()
	{
		return "Stick";
	}

	// Token: 0x040001BA RID: 442
	[Header("Settings")]
	[SerializeField]
	private float bladeAngleStep = 12.5f;

	// Token: 0x040001BB RID: 443
	[Space(20f)]
	[SerializeField]
	private bool transferAngularVelocity = true;

	// Token: 0x040001BC RID: 444
	[SerializeField]
	private float angularVelocityTransferMultiplier = 0.25f;

	// Token: 0x040001BD RID: 445
	[Space(20f)]
	[SerializeField]
	private float shaftHandleProportionalGain = 500f;

	// Token: 0x040001BE RID: 446
	[SerializeField]
	private float shaftHandleIntegralGain;

	// Token: 0x040001BF RID: 447
	[SerializeField]
	private float shaftHandleIntegralSaturation;

	// Token: 0x040001C0 RID: 448
	[SerializeField]
	private float shaftHandleDerivativeGain = 20f;

	// Token: 0x040001C1 RID: 449
	[SerializeField]
	private float shaftHandleDerivativeSmoothing = 0.1f;

	// Token: 0x040001C2 RID: 450
	[SerializeField]
	private float minShaftHandleProportionalGainMultiplier = 0.25f;

	// Token: 0x040001C3 RID: 451
	[Space(20f)]
	[SerializeField]
	private float bladeHandleProportionalGain = 500f;

	// Token: 0x040001C4 RID: 452
	[SerializeField]
	private float bladeHandleIntegralGain;

	// Token: 0x040001C5 RID: 453
	[SerializeField]
	private float bladeHandleIntegralSaturation;

	// Token: 0x040001C6 RID: 454
	[SerializeField]
	private float bladeHandleDerivativeGain = 20f;

	// Token: 0x040001C7 RID: 455
	[SerializeField]
	private float bladeHandleDerivativeSmoothing = 0.1f;

	// Token: 0x040001C8 RID: 456
	[Space(20f)]
	[SerializeField]
	private float linearVelocityTransferMultiplier = 0.25f;

	// Token: 0x040001C9 RID: 457
	[Header("References")]
	[SerializeField]
	private GameObject shaftHandle;

	// Token: 0x040001CA RID: 458
	[SerializeField]
	private GameObject bladeHandle;

	// Token: 0x040001CB RID: 459
	[SerializeField]
	private GameObject rotationContainer;

	// Token: 0x040001CC RID: 460
	[SerializeField]
	private StickMesh stickMesh;

	// Token: 0x040001CD RID: 461
	[HideInInspector]
	public NetworkVariable<NetworkObjectReference> PlayerReference;

	// Token: 0x040001CE RID: 462
	[HideInInspector]
	public Rigidbody Rigidbody;

	// Token: 0x040001CF RID: 463
	[HideInInspector]
	public NetworkObjectCollisionRecorder NetworkObjectCollisionRecorder;

	// Token: 0x040001D0 RID: 464
	[HideInInspector]
	public float Length;

	// Token: 0x040001D1 RID: 465
	private bool isNetworkVariablesInitialized;

	// Token: 0x040001D2 RID: 466
	private Vector3PIDController shaftHandlePIDController = new Vector3PIDController(0f, 0f, 0f);

	// Token: 0x040001D3 RID: 467
	private Vector3PIDController bladeHandlePIDController = new Vector3PIDController(0f, 0f, 0f);

	// Token: 0x040001D4 RID: 468
	private float shaftHandleProportionalGainMultiplier = 1f;

	// Token: 0x040001D5 RID: 469
	private float bladeHandleProportionalGainMultiplier = 1f;
}
