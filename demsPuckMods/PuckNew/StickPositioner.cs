using System;
using Unity.Netcode;
using UnityEngine;

// Token: 0x02000052 RID: 82
public class StickPositioner : NetworkBehaviour
{
	// Token: 0x17000053 RID: 83
	// (get) Token: 0x060002BA RID: 698 RVA: 0x00011704 File Offset: 0x0000F904
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

	// Token: 0x17000054 RID: 84
	// (get) Token: 0x060002BB RID: 699 RVA: 0x00011731 File Offset: 0x0000F931
	[HideInInspector]
	public PlayerBody PlayerBody
	{
		get
		{
			return this.Player.PlayerBody;
		}
	}

	// Token: 0x17000055 RID: 85
	// (get) Token: 0x060002BC RID: 700 RVA: 0x0001173E File Offset: 0x0000F93E
	[HideInInspector]
	public Stick Stick
	{
		get
		{
			return this.Player.Stick;
		}
	}

	// Token: 0x17000056 RID: 86
	// (get) Token: 0x060002BD RID: 701 RVA: 0x0001174B File Offset: 0x0000F94B
	[HideInInspector]
	public Vector3 BladeTargetPosition
	{
		get
		{
			return this.bladeTarget.transform.position;
		}
	}

	// Token: 0x17000057 RID: 87
	// (get) Token: 0x060002BE RID: 702 RVA: 0x0001175D File Offset: 0x0000F95D
	[HideInInspector]
	public Vector3 BladeTargetVelocity
	{
		get
		{
			return this.bladeTargetVelocity;
		}
	}

	// Token: 0x17000058 RID: 88
	// (get) Token: 0x060002BF RID: 703 RVA: 0x00011765 File Offset: 0x0000F965
	[HideInInspector]
	public Vector3 ShaftTargetPosition
	{
		get
		{
			return this.shaftTarget.transform.position;
		}
	}

	// Token: 0x17000059 RID: 89
	// (get) Token: 0x060002C0 RID: 704 RVA: 0x00011777 File Offset: 0x0000F977
	[HideInInspector]
	public Vector3 RaycastOriginPosition
	{
		get
		{
			return this.raycastOrigin.transform.position;
		}
	}

	// Token: 0x1700005A RID: 90
	// (get) Token: 0x060002C1 RID: 705 RVA: 0x00011789 File Offset: 0x0000F989
	private Vector3 BladeTargetFocusPointInitialLocalPosition
	{
		get
		{
			if (this.Handedness != PlayerHandedness.Left)
			{
				return this.bladeTargetFocusPointInitialLocalPosition;
			}
			return new Vector3(-this.bladeTargetFocusPointInitialLocalPosition.x, this.bladeTargetFocusPointInitialLocalPosition.y, this.bladeTargetFocusPointInitialLocalPosition.z);
		}
	}

	// Token: 0x1700005B RID: 91
	// (get) Token: 0x060002C2 RID: 706 RVA: 0x000117C2 File Offset: 0x0000F9C2
	private Vector3 RaycastOriginInitialLocalPosition
	{
		get
		{
			if (this.Handedness != PlayerHandedness.Left)
			{
				return this.raycastOriginInitialLocalPosition;
			}
			return new Vector3(-this.raycastOriginInitialLocalPosition.x, this.raycastOriginInitialLocalPosition.y, this.raycastOriginInitialLocalPosition.z);
		}
	}

	// Token: 0x060002C3 RID: 707 RVA: 0x000117FB File Offset: 0x0000F9FB
	private void Awake()
	{
		this.bladeTargetFocusPointInitialLocalPosition = this.bladeTargetFocusPoint.transform.localPosition;
		this.raycastOriginInitialLocalPosition = this.raycastOrigin.transform.localPosition;
	}

	// Token: 0x060002C4 RID: 708 RVA: 0x0001182C File Offset: 0x0000FA2C
	protected override void OnNetworkPreSpawn(ref NetworkManager networkManager)
	{
		this.InitializeNetworkVariables(default(NetworkObjectReference));
		base.OnNetworkPreSpawn(ref networkManager);
	}

	// Token: 0x060002C5 RID: 709 RVA: 0x0001184F File Offset: 0x0000FA4F
	public override void OnNetworkSpawn()
	{
		NetworkVariable<NetworkObjectReference> playerReference = this.PlayerReference;
		playerReference.OnValueChanged = (NetworkVariable<NetworkObjectReference>.OnValueChangedDelegate)Delegate.Combine(playerReference.OnValueChanged, new NetworkVariable<NetworkObjectReference>.OnValueChangedDelegate(this.OnPlayerReferenceChanged));
		base.OnNetworkSpawn();
	}

	// Token: 0x060002C6 RID: 710 RVA: 0x0001187E File Offset: 0x0000FA7E
	protected override void OnNetworkPostSpawn()
	{
		if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsConnectedClient)
		{
			this.ProcessInitialNetworkVariableValues();
		}
		base.OnNetworkPostSpawn();
	}

	// Token: 0x060002C7 RID: 711 RVA: 0x000118A4 File Offset: 0x0000FAA4
	protected override void OnNetworkSessionSynchronized()
	{
		this.ProcessInitialNetworkVariableValues();
		base.OnNetworkSessionSynchronized();
	}

	// Token: 0x060002C8 RID: 712 RVA: 0x000118B2 File Offset: 0x0000FAB2
	public override void OnNetworkDespawn()
	{
		NetworkVariable<NetworkObjectReference> playerReference = this.PlayerReference;
		playerReference.OnValueChanged = (NetworkVariable<NetworkObjectReference>.OnValueChangedDelegate)Delegate.Remove(playerReference.OnValueChanged, new NetworkVariable<NetworkObjectReference>.OnValueChangedDelegate(this.OnPlayerReferenceChanged));
		base.OnNetworkDespawn();
	}

	// Token: 0x060002C9 RID: 713 RVA: 0x000118E1 File Offset: 0x0000FAE1
	public void InitializeNetworkVariables(NetworkObjectReference playerReference = default(NetworkObjectReference))
	{
		if (this.isNetworkVariablesInitialized)
		{
			return;
		}
		this.isNetworkVariablesInitialized = true;
		this.PlayerReference = new NetworkVariable<NetworkObjectReference>(playerReference, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
	}

	// Token: 0x060002CA RID: 714 RVA: 0x00011904 File Offset: 0x0000FB04
	private void ProcessInitialNetworkVariableValues()
	{
		this.OnPlayerReferenceChanged(default(NetworkObjectReference), this.PlayerReference.Value);
	}

	// Token: 0x060002CB RID: 715 RVA: 0x0001192C File Offset: 0x0000FB2C
	private void FixedUpdate()
	{
		if (!this.Player)
		{
			return;
		}
		this.pidController.proportionalGain = this.proportionalGain;
		this.pidController.integralGain = this.integralGain;
		this.pidController.integralSaturation = this.integralSaturation;
		this.pidController.derivativeMeasurement = this.derivativeMeasurement;
		this.pidController.derivativeGain = this.derivativeGain;
		this.pidController.derivativeSmoothing = this.derivativeSmoothing;
		this.pidController.outputMin = this.outputMin;
		this.pidController.outputMax = this.outputMax;
		this.raycastOriginAngleTarget = this.Player.PlayerInput.StickRaycastOriginAngleInput.ServerValue;
		this.ShootPaddingRay();
		this.RotateRaycastOrigin();
		this.ShootRaycast();
		this.UpdateAudio();
	}

	// Token: 0x060002CC RID: 716 RVA: 0x00011A04 File Offset: 0x0000FC04
	private void ShootPaddingRay()
	{
		Vector3 vector = new Vector3(0f, this.RaycastOriginInitialLocalPosition.y, 0f);
		Vector3 normalized = (this.RaycastOriginInitialLocalPosition - vector).normalized;
		float num = Vector3.Distance(vector, this.RaycastOriginInitialLocalPosition) + this.raycastOriginPadding;
		Vector3 vector2 = base.transform.TransformPoint(vector);
		Vector3 vector3 = base.transform.TransformDirection(normalized);
		Debug.DrawRay(vector2, vector3 * num, Color.yellow);
		RaycastHit raycastHit;
		if (Physics.Raycast(vector2, vector3, out raycastHit, num, this.raycastLayerMask))
		{
			this.raycastOrigin.transform.localPosition = vector + normalized * (raycastHit.distance - this.raycastOriginPadding);
			return;
		}
		this.raycastOrigin.transform.localPosition = this.RaycastOriginInitialLocalPosition;
	}

	// Token: 0x060002CD RID: 717 RVA: 0x00011ADC File Offset: 0x0000FCDC
	private void RotateRaycastOrigin()
	{
		this.raycastOriginAngleDelta = this.pidController.Update(Time.fixedDeltaTime, this.raycastOriginAngle, this.raycastOriginAngleTarget);
		this.raycastOriginAngle += this.raycastOriginAngleDelta * Time.fixedDeltaTime;
		this.raycastOrigin.transform.localRotation = Quaternion.Euler(this.raycastOriginAngle);
	}

	// Token: 0x060002CE RID: 718 RVA: 0x00011B5C File Offset: 0x0000FD5C
	private void ShootRaycast()
	{
		RaycastHit hit;
		Vector3 vector5;
		if (Physics.Raycast(this.raycastOrigin.transform.position, this.raycastOrigin.transform.forward, out hit, this.maximumReach, this.raycastLayerMask))
		{
			this.OnGrounded(hit.transform.gameObject);
			Vector3 vector = this.raycastOrigin.transform.position + this.raycastOrigin.transform.forward * this.maximumReach;
			Vector3 b = Vector3.Scale(Utils.Vector3Abs(hit.normal), hit.point);
			Vector3 vector2 = vector - Vector3.Scale(Utils.Vector3Abs(hit.normal), vector) + b;
			Debug.DrawRay(this.raycastOrigin.transform.position, this.raycastOrigin.transform.forward * hit.distance, Color.red);
			Vector3 normalized = (vector2 - this.raycastOrigin.transform.position).normalized;
			RaycastHit raycastHit;
			if (Physics.Raycast(this.raycastOrigin.transform.position, normalized, out raycastHit, this.maximumReach, this.raycastLayerMask))
			{
				Vector3 vector3 = this.raycastOrigin.transform.position + this.raycastOrigin.transform.forward * this.maximumReach;
				Vector3 b2 = Vector3.Scale(Utils.Vector3Abs(raycastHit.normal), raycastHit.point);
				Vector3 vector4 = vector3 - Vector3.Scale(Utils.Vector3Abs(raycastHit.normal), vector3) + b2;
				if (hit.normal == Vector3.up && raycastHit.normal == Vector3.up)
				{
					vector5 = vector2;
				}
				else if (hit.normal == Vector3.up && raycastHit.normal != Vector3.up)
				{
					vector5 = vector4;
					vector5.y = Mathf.Max(0f, vector5.y);
				}
				else
				{
					vector5 = vector2;
					vector5.y = Mathf.Max(0f, vector5.y);
				}
				Debug.DrawRay(this.raycastOrigin.transform.position, normalized * raycastHit.distance, Color.blue);
			}
			else
			{
				vector5 = hit.point;
			}
			this.ApplySoftCollision(hit, vector5);
		}
		else
		{
			this.OnUngrounded();
			vector5 = this.raycastOrigin.transform.position + this.raycastOrigin.transform.forward * this.maximumReach;
			Debug.DrawRay(this.raycastOrigin.transform.position, this.raycastOrigin.transform.forward * this.maximumReach, Color.red);
		}
		this.PositionBladeTarget(vector5);
		this.PositionBladeTargetFocusPoint(vector5);
		this.RotateBladeTargetFocusPoint();
	}

	// Token: 0x060002CF RID: 719 RVA: 0x00011E60 File Offset: 0x00010060
	private void PositionBladeTarget(Vector3 hitPosition)
	{
		this.bladeTarget.transform.position = hitPosition;
		this.bladeTarget.transform.rotation = Quaternion.LookRotation(this.bladeTarget.transform.position - this.bladeTargetFocusPoint.transform.position);
		this.bladeTargetVelocity = (this.bladeTarget.transform.position - this.lastBladeTargetPosition) / Time.fixedDeltaTime;
		this.lastBladeTargetPosition = this.bladeTarget.transform.position;
	}

	// Token: 0x060002D0 RID: 720 RVA: 0x00011EFC File Offset: 0x000100FC
	private void PositionBladeTargetFocusPoint(Vector3 hitPosition)
	{
		float num = Vector3.Distance(this.raycastOrigin.transform.position, new Vector3(hitPosition.x, this.raycastOrigin.transform.position.y, hitPosition.z));
		float d = this.maximumReach - num;
		Vector3 vector = this.raycastOrigin.transform.localPosition - base.transform.InverseTransformPoint(hitPosition);
		Vector3 normalized = new Vector3(vector.x, 0f, vector.z).normalized;
		Vector3 a = base.transform.TransformDirection(normalized);
		Debug.DrawRay(this.bladeTargetFocusPoint.transform.position, -a * d, Color.grey);
		Vector3 b = normalized * d;
		this.bladeTargetFocusPoint.transform.localPosition = this.BladeTargetFocusPointInitialLocalPosition + b;
	}

	// Token: 0x060002D1 RID: 721 RVA: 0x00011FEC File Offset: 0x000101EC
	private void RotateBladeTargetFocusPoint()
	{
		PlayerInput playerInput = this.Player.PlayerInput;
		if (!playerInput)
		{
			return;
		}
		float num = Mathf.Lerp(1f, 0f, (playerInput.MaximumStickRaycastOriginAngle.x - this.raycastOriginAngle.x) / this.bladeTargetRotationThreshold);
		num *= (float)((this.Handedness == PlayerHandedness.Left) ? -1 : 1);
		this.bladeTargetFocusPoint.transform.localPosition = Utils.RotatePointAroundPivot(this.bladeTargetFocusPoint.transform.localPosition, this.bladeTarget.transform.localPosition, new Vector3(0f, this.bladeTargetMaxAngle * num, 0f));
	}

	// Token: 0x060002D2 RID: 722 RVA: 0x00012099 File Offset: 0x00010299
	public void PrepareShaftTarget(Stick stick)
	{
		this.shaftTarget.transform.localPosition = stick.ShaftHandleLocalPosition - stick.BladeHandleLocalPosition;
	}

	// Token: 0x060002D3 RID: 723 RVA: 0x000120BC File Offset: 0x000102BC
	private void ApplySoftCollision(RaycastHit hit, Vector3 hitPosition)
	{
		if (!this.applySoftCollision)
		{
			return;
		}
		if (!this.PlayerBody)
		{
			return;
		}
		if (hit.collider.CompareTag("Soft Collider"))
		{
			float d = this.maximumReach - hit.distance;
			float magnitude = Vector3.Cross(hit.normal, this.raycastOrigin.transform.forward).magnitude;
			float num = 1f - magnitude;
			Debug.DrawRay(hitPosition, hit.normal * d * this.softCollisionForce, Color.green);
			this.PlayerBody.Rigidbody.AddForceAtPosition(hit.normal * d * (this.softCollisionForce * num), hitPosition, ForceMode.Acceleration);
		}
	}

	// Token: 0x060002D4 RID: 724 RVA: 0x00012184 File Offset: 0x00010384
	private void UpdateAudio()
	{
		this.windAudioSource.transform.position = this.BladeTargetPosition;
		this.iceHitAudioSource.transform.position = this.BladeTargetPosition;
		this.iceDragAudioSource.transform.position = this.BladeTargetPosition;
		float num = this.IsGrounded ? this.iceDragVolumeCurve.Evaluate(this.BladeTargetVelocity.magnitude) : 0f;
		if (num > this.iceDragVolume)
		{
			this.iceDragVolume = num;
		}
		else
		{
			this.iceDragVolume = Mathf.Lerp(this.iceDragVolume, num, Time.fixedDeltaTime * this.iceDragVolumeFallOffSpeed);
		}
		this.iceDragPitch = this.iceDragPitchCurve.Evaluate(this.BladeTargetVelocity.magnitude);
		this.iceDragAudioSource.Server_SetVolume(this.iceDragVolume);
		this.iceDragAudioSource.Server_SetPitch(this.iceDragPitch);
		float volume = this.windVolumeCurve.Evaluate(this.raycastOriginAngleDelta.magnitude);
		float pitch = this.windPitchCurve.Evaluate(this.raycastOriginAngleDelta.magnitude);
		this.windAudioSource.Server_SetVolume(volume);
		this.windAudioSource.Server_SetPitch(pitch);
	}

	// Token: 0x060002D5 RID: 725 RVA: 0x000122B4 File Offset: 0x000104B4
	private void OnGrounded(GameObject ground)
	{
		if (this.IsGrounded)
		{
			return;
		}
		if (ground.layer == LayerMask.NameToLayer("Ice"))
		{
			float volume = this.iceHitVolumeCurve.Evaluate(Mathf.Abs(this.raycastOriginAngleDelta.x));
			float pitch = this.iceHitPitchCurve.Evaluate(Mathf.Abs(this.raycastOriginAngleDelta.x));
			this.iceHitAudioSource.Server_Play(volume, pitch, true, -1, 0f, true, false, false, 0f, false, 0f, -1f);
		}
		this.IsGrounded = true;
	}

	// Token: 0x060002D6 RID: 726 RVA: 0x00012342 File Offset: 0x00010542
	private void OnUngrounded()
	{
		if (!this.IsGrounded)
		{
			return;
		}
		this.IsGrounded = false;
	}

	// Token: 0x060002D7 RID: 727 RVA: 0x00012354 File Offset: 0x00010554
	private void OnPlayerReferenceChanged(NetworkObjectReference previousValue, NetworkObjectReference newValue)
	{
		if (!this.Player)
		{
			return;
		}
		this.Player.StickPositioner = this;
		if (this.Player.Stick)
		{
			this.PrepareShaftTarget(this.Player.Stick);
		}
		this.Handedness = this.Player.Handedness.Value;
	}

	// Token: 0x060002D9 RID: 729 RVA: 0x000124B4 File Offset: 0x000106B4
	protected override void __initializeVariables()
	{
		bool flag = this.PlayerReference == null;
		if (flag)
		{
			throw new Exception("StickPositioner.PlayerReference cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.PlayerReference.Initialize(this);
		base.__nameNetworkVariable(this.PlayerReference, "PlayerReference");
		this.NetworkVariableFields.Add(this.PlayerReference);
		base.__initializeVariables();
	}

	// Token: 0x060002DA RID: 730 RVA: 0x000021BE File Offset: 0x000003BE
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x060002DB RID: 731 RVA: 0x00012517 File Offset: 0x00010717
	protected internal override string __getTypeName()
	{
		return "StickPositioner";
	}

	// Token: 0x040001D7 RID: 471
	[Header("Settings")]
	[SerializeField]
	private float proportionalGain = 0.75f;

	// Token: 0x040001D8 RID: 472
	[SerializeField]
	private float integralGain = 5f;

	// Token: 0x040001D9 RID: 473
	[SerializeField]
	private float integralSaturation = 5f;

	// Token: 0x040001DA RID: 474
	[SerializeField]
	private DerivativeMeasurement derivativeMeasurement;

	// Token: 0x040001DB RID: 475
	[SerializeField]
	private float derivativeGain;

	// Token: 0x040001DC RID: 476
	[SerializeField]
	private float derivativeSmoothing;

	// Token: 0x040001DD RID: 477
	[SerializeField]
	private float outputMin = -15f;

	// Token: 0x040001DE RID: 478
	[SerializeField]
	private float outputMax = 15f;

	// Token: 0x040001DF RID: 479
	[Space(20f)]
	[SerializeField]
	private float maximumReach = 2.5f;

	// Token: 0x040001E0 RID: 480
	[Space(20f)]
	[SerializeField]
	private float bladeTargetRotationThreshold = 25f;

	// Token: 0x040001E1 RID: 481
	[SerializeField]
	private float bladeTargetMaxAngle = 45f;

	// Token: 0x040001E2 RID: 482
	[Space(20f)]
	[SerializeField]
	private LayerMask raycastLayerMask;

	// Token: 0x040001E3 RID: 483
	[Space(20f)]
	[SerializeField]
	private float raycastOriginPadding = 0.2f;

	// Token: 0x040001E4 RID: 484
	[Space(20f)]
	[SerializeField]
	private bool applySoftCollision = true;

	// Token: 0x040001E5 RID: 485
	[SerializeField]
	private float softCollisionForce = 1f;

	// Token: 0x040001E6 RID: 486
	[Space(20f)]
	[SerializeField]
	private AnimationCurve windVolumeCurve;

	// Token: 0x040001E7 RID: 487
	[SerializeField]
	private AnimationCurve windPitchCurve;

	// Token: 0x040001E8 RID: 488
	[SerializeField]
	private AnimationCurve iceHitVolumeCurve;

	// Token: 0x040001E9 RID: 489
	[SerializeField]
	private AnimationCurve iceHitPitchCurve;

	// Token: 0x040001EA RID: 490
	[SerializeField]
	private AnimationCurve iceDragVolumeCurve;

	// Token: 0x040001EB RID: 491
	[SerializeField]
	private AnimationCurve iceDragPitchCurve;

	// Token: 0x040001EC RID: 492
	[Header("References")]
	[SerializeField]
	private GameObject raycastOrigin;

	// Token: 0x040001ED RID: 493
	[SerializeField]
	private GameObject bladeTargetFocusPoint;

	// Token: 0x040001EE RID: 494
	[SerializeField]
	private GameObject bladeTarget;

	// Token: 0x040001EF RID: 495
	[SerializeField]
	private GameObject shaftTarget;

	// Token: 0x040001F0 RID: 496
	[Space(20f)]
	[SerializeField]
	private SynchronizedAudio windAudioSource;

	// Token: 0x040001F1 RID: 497
	[SerializeField]
	private SynchronizedAudio iceHitAudioSource;

	// Token: 0x040001F2 RID: 498
	[SerializeField]
	private SynchronizedAudio iceDragAudioSource;

	// Token: 0x040001F3 RID: 499
	[SerializeField]
	private float iceDragVolumeFallOffSpeed = 10f;

	// Token: 0x040001F4 RID: 500
	[HideInInspector]
	public NetworkVariable<NetworkObjectReference> PlayerReference;

	// Token: 0x040001F5 RID: 501
	[HideInInspector]
	public bool IsGrounded;

	// Token: 0x040001F6 RID: 502
	[HideInInspector]
	public PlayerHandedness Handedness;

	// Token: 0x040001F7 RID: 503
	private bool isNetworkVariablesInitialized;

	// Token: 0x040001F8 RID: 504
	private Vector3 lastBladeTargetPosition = Vector3.zero;

	// Token: 0x040001F9 RID: 505
	private Vector3 bladeTargetVelocity = Vector3.zero;

	// Token: 0x040001FA RID: 506
	private Vector3 bladeTargetFocusPointInitialLocalPosition = Vector3.zero;

	// Token: 0x040001FB RID: 507
	private Vector3 raycastOriginInitialLocalPosition = Vector3.zero;

	// Token: 0x040001FC RID: 508
	private Vector2 raycastOriginAngleTarget = Vector2.zero;

	// Token: 0x040001FD RID: 509
	private Vector2 raycastOriginAngle = Vector2.zero;

	// Token: 0x040001FE RID: 510
	private Vector2 raycastOriginAngleDelta = Vector3.zero;

	// Token: 0x040001FF RID: 511
	private float iceDragVolume;

	// Token: 0x04000200 RID: 512
	private float iceDragPitch;

	// Token: 0x04000201 RID: 513
	private Vector3PIDController pidController = new Vector3PIDController(0f, 0f, 0f);
}
