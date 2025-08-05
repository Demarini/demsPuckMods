using System;
using Unity.Netcode;
using UnityEngine;

// Token: 0x020000F6 RID: 246
public class StickPositioner : NetworkBehaviour
{
	// Token: 0x170000C4 RID: 196
	// (get) Token: 0x06000862 RID: 2146 RVA: 0x0000C1C4 File Offset: 0x0000A3C4
	[HideInInspector]
	public PlayerBodyV2 PlayerBody
	{
		get
		{
			return this.Player.PlayerBody;
		}
	}

	// Token: 0x170000C5 RID: 197
	// (get) Token: 0x06000863 RID: 2147 RVA: 0x0000C1D1 File Offset: 0x0000A3D1
	[HideInInspector]
	public Stick Stick
	{
		get
		{
			return this.Player.Stick;
		}
	}

	// Token: 0x170000C6 RID: 198
	// (get) Token: 0x06000864 RID: 2148 RVA: 0x0000C1DE File Offset: 0x0000A3DE
	[HideInInspector]
	public Vector3 BladeTargetPosition
	{
		get
		{
			return this.bladeTarget.transform.position;
		}
	}

	// Token: 0x170000C7 RID: 199
	// (get) Token: 0x06000865 RID: 2149 RVA: 0x0000C1F0 File Offset: 0x0000A3F0
	[HideInInspector]
	public Vector3 BladeTargetVelocity
	{
		get
		{
			return this.bladeTargetVelocity;
		}
	}

	// Token: 0x170000C8 RID: 200
	// (get) Token: 0x06000866 RID: 2150 RVA: 0x0000C1F8 File Offset: 0x0000A3F8
	[HideInInspector]
	public Vector3 ShaftTargetPosition
	{
		get
		{
			return this.shaftTarget.transform.position;
		}
	}

	// Token: 0x170000C9 RID: 201
	// (get) Token: 0x06000867 RID: 2151 RVA: 0x0000C20A File Offset: 0x0000A40A
	[HideInInspector]
	public Vector3 RaycastOriginPosition
	{
		get
		{
			return this.raycastOrigin.transform.position;
		}
	}

	// Token: 0x170000CA RID: 202
	// (get) Token: 0x06000868 RID: 2152 RVA: 0x0000C21C File Offset: 0x0000A41C
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

	// Token: 0x170000CB RID: 203
	// (get) Token: 0x06000869 RID: 2153 RVA: 0x0000C254 File Offset: 0x0000A454
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

	// Token: 0x0600086A RID: 2154 RVA: 0x0000C28C File Offset: 0x0000A48C
	private void Awake()
	{
		this.bladeTargetFocusPointInitialLocalPosition = this.bladeTargetFocusPoint.transform.localPosition;
		this.raycastOriginInitialLocalPosition = this.raycastOrigin.transform.localPosition;
	}

	// Token: 0x0600086B RID: 2155 RVA: 0x00034814 File Offset: 0x00032A14
	protected override void OnNetworkPostSpawn()
	{
		NetworkObject networkObject;
		if (this.PlayerReference.Value.TryGet(out networkObject, null))
		{
			this.Player = networkObject.GetComponent<Player>();
		}
		if (this.Player)
		{
			this.Player.StickPositioner = this;
			this.stickRaycastOriginAngleInput = this.Player.PlayerInput.StickRaycastOriginAngleInput.ServerValue;
			this.raycastOriginAngle = this.stickRaycastOriginAngleInput;
			this.Handedness = this.Player.Handedness.Value;
		}
		base.OnNetworkPostSpawn();
	}

	// Token: 0x0600086C RID: 2156 RVA: 0x000348A4 File Offset: 0x00032AA4
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
		this.stickRaycastOriginAngleInput = this.Player.PlayerInput.StickRaycastOriginAngleInput.ServerValue;
		this.ShootPaddingRay();
		this.RotateRaycastOrigin();
		this.ShootRaycast();
		this.UpdateAudio();
	}

	// Token: 0x0600086D RID: 2157 RVA: 0x0003497C File Offset: 0x00032B7C
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

	// Token: 0x0600086E RID: 2158 RVA: 0x00034A54 File Offset: 0x00032C54
	private void RotateRaycastOrigin()
	{
		this.raycastOriginAngleDelta = this.pidController.Update(Time.fixedDeltaTime, this.raycastOriginAngle, this.stickRaycastOriginAngleInput);
		this.raycastOriginAngle += this.raycastOriginAngleDelta * Time.fixedDeltaTime;
		this.raycastOrigin.transform.localRotation = Quaternion.Euler(this.raycastOriginAngle);
	}

	// Token: 0x0600086F RID: 2159 RVA: 0x00034AD4 File Offset: 0x00032CD4
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

	// Token: 0x06000870 RID: 2160 RVA: 0x00034DD8 File Offset: 0x00032FD8
	private void PositionBladeTarget(Vector3 hitPosition)
	{
		this.bladeTarget.transform.position = hitPosition;
		this.bladeTarget.transform.rotation = Quaternion.LookRotation(this.bladeTarget.transform.position - this.bladeTargetFocusPoint.transform.position);
		this.bladeTargetVelocity = (this.bladeTarget.transform.position - this.lastBladeTargetPosition) / Time.fixedDeltaTime;
		this.lastBladeTargetPosition = this.bladeTarget.transform.position;
	}

	// Token: 0x06000871 RID: 2161 RVA: 0x00034E74 File Offset: 0x00033074
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

	// Token: 0x06000872 RID: 2162 RVA: 0x00034F64 File Offset: 0x00033164
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

	// Token: 0x06000873 RID: 2163 RVA: 0x0000C2BA File Offset: 0x0000A4BA
	public void PrepareShaftTarget(Stick stick)
	{
		this.shaftTarget.transform.localPosition = stick.ShaftHandleLocalPosition - stick.BladeHandleLocalPosition;
	}

	// Token: 0x06000874 RID: 2164 RVA: 0x00035010 File Offset: 0x00033210
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

	// Token: 0x06000875 RID: 2165 RVA: 0x000350D8 File Offset: 0x000332D8
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

	// Token: 0x06000876 RID: 2166 RVA: 0x00035208 File Offset: 0x00033408
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

	// Token: 0x06000877 RID: 2167 RVA: 0x0000C2DD File Offset: 0x0000A4DD
	private void OnUngrounded()
	{
		if (!this.IsGrounded)
		{
			return;
		}
		this.IsGrounded = false;
	}

	// Token: 0x06000879 RID: 2169 RVA: 0x000353A4 File Offset: 0x000335A4
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

	// Token: 0x0600087A RID: 2170 RVA: 0x00006C33 File Offset: 0x00004E33
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x0600087B RID: 2171 RVA: 0x0000C2EF File Offset: 0x0000A4EF
	protected internal override string __getTypeName()
	{
		return "StickPositioner";
	}

	// Token: 0x040004F3 RID: 1267
	[Header("References")]
	[SerializeField]
	private GameObject raycastOrigin;

	// Token: 0x040004F4 RID: 1268
	[SerializeField]
	private GameObject bladeTargetFocusPoint;

	// Token: 0x040004F5 RID: 1269
	[SerializeField]
	private GameObject bladeTarget;

	// Token: 0x040004F6 RID: 1270
	[SerializeField]
	private GameObject shaftTarget;

	// Token: 0x040004F7 RID: 1271
	[Space(20f)]
	[SerializeField]
	private SynchronizedAudio windAudioSource;

	// Token: 0x040004F8 RID: 1272
	[SerializeField]
	private SynchronizedAudio iceHitAudioSource;

	// Token: 0x040004F9 RID: 1273
	[SerializeField]
	private SynchronizedAudio iceDragAudioSource;

	// Token: 0x040004FA RID: 1274
	[SerializeField]
	private float iceDragVolumeFallOffSpeed = 10f;

	// Token: 0x040004FB RID: 1275
	[Header("Settings")]
	[SerializeField]
	private float proportionalGain = 0.75f;

	// Token: 0x040004FC RID: 1276
	[SerializeField]
	private float integralGain = 5f;

	// Token: 0x040004FD RID: 1277
	[SerializeField]
	private float integralSaturation = 5f;

	// Token: 0x040004FE RID: 1278
	[SerializeField]
	private DerivativeMeasurement derivativeMeasurement;

	// Token: 0x040004FF RID: 1279
	[SerializeField]
	private float derivativeGain;

	// Token: 0x04000500 RID: 1280
	[SerializeField]
	private float derivativeSmoothing;

	// Token: 0x04000501 RID: 1281
	[SerializeField]
	private float outputMin = -15f;

	// Token: 0x04000502 RID: 1282
	[SerializeField]
	private float outputMax = 15f;

	// Token: 0x04000503 RID: 1283
	[Space(20f)]
	[SerializeField]
	private float maximumReach = 2.5f;

	// Token: 0x04000504 RID: 1284
	[Space(20f)]
	[SerializeField]
	private float bladeTargetRotationThreshold = 25f;

	// Token: 0x04000505 RID: 1285
	[SerializeField]
	private float bladeTargetMaxAngle = 45f;

	// Token: 0x04000506 RID: 1286
	[Space(20f)]
	[SerializeField]
	private LayerMask raycastLayerMask;

	// Token: 0x04000507 RID: 1287
	[Space(20f)]
	[SerializeField]
	private float raycastOriginPadding = 0.2f;

	// Token: 0x04000508 RID: 1288
	[Space(20f)]
	[SerializeField]
	private bool applySoftCollision = true;

	// Token: 0x04000509 RID: 1289
	[SerializeField]
	private float softCollisionForce = 1f;

	// Token: 0x0400050A RID: 1290
	[Space(20f)]
	[SerializeField]
	private AnimationCurve windVolumeCurve;

	// Token: 0x0400050B RID: 1291
	[SerializeField]
	private AnimationCurve windPitchCurve;

	// Token: 0x0400050C RID: 1292
	[SerializeField]
	private AnimationCurve iceHitVolumeCurve;

	// Token: 0x0400050D RID: 1293
	[SerializeField]
	private AnimationCurve iceHitPitchCurve;

	// Token: 0x0400050E RID: 1294
	[SerializeField]
	private AnimationCurve iceDragVolumeCurve;

	// Token: 0x0400050F RID: 1295
	[SerializeField]
	private AnimationCurve iceDragPitchCurve;

	// Token: 0x04000510 RID: 1296
	[HideInInspector]
	public NetworkVariable<NetworkObjectReference> PlayerReference = new NetworkVariable<NetworkObjectReference>(default(NetworkObjectReference), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x04000511 RID: 1297
	[HideInInspector]
	public Player Player;

	// Token: 0x04000512 RID: 1298
	[HideInInspector]
	public bool IsGrounded;

	// Token: 0x04000513 RID: 1299
	[HideInInspector]
	public PlayerHandedness Handedness;

	// Token: 0x04000514 RID: 1300
	private Vector3 lastBladeTargetPosition = Vector3.zero;

	// Token: 0x04000515 RID: 1301
	private Vector3 bladeTargetVelocity = Vector3.zero;

	// Token: 0x04000516 RID: 1302
	private Vector3 bladeTargetFocusPointInitialLocalPosition = Vector3.zero;

	// Token: 0x04000517 RID: 1303
	private Vector3 raycastOriginInitialLocalPosition = Vector3.zero;

	// Token: 0x04000518 RID: 1304
	private Vector2 stickRaycastOriginAngleInput;

	// Token: 0x04000519 RID: 1305
	private Vector2 raycastOriginAngle = Vector2.zero;

	// Token: 0x0400051A RID: 1306
	private Vector2 raycastOriginAngleDelta = Vector3.zero;

	// Token: 0x0400051B RID: 1307
	private float iceDragVolume;

	// Token: 0x0400051C RID: 1308
	private float iceDragPitch;

	// Token: 0x0400051D RID: 1309
	private Vector3PIDController pidController = new Vector3PIDController(0f, 0f, 0f);
}
