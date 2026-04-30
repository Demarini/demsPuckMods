using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Unity.Netcode;
using UnityEngine;

// Token: 0x02000035 RID: 53
public class PlayerBody : NetworkBehaviour
{
	// Token: 0x17000015 RID: 21
	// (get) Token: 0x0600010E RID: 270 RVA: 0x0000555C File Offset: 0x0000375C
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

	// Token: 0x17000016 RID: 22
	// (get) Token: 0x0600010F RID: 271 RVA: 0x00005589 File Offset: 0x00003789
	[HideInInspector]
	public PlayerCamera PlayerCamera
	{
		get
		{
			return this.Player.PlayerCamera;
		}
	}

	// Token: 0x17000017 RID: 23
	// (get) Token: 0x06000110 RID: 272 RVA: 0x00005596 File Offset: 0x00003796
	[HideInInspector]
	public Stick Stick
	{
		get
		{
			return this.Player.Stick;
		}
	}

	// Token: 0x17000018 RID: 24
	// (get) Token: 0x06000111 RID: 273 RVA: 0x000055A3 File Offset: 0x000037A3
	[HideInInspector]
	public PlayerMesh PlayerMesh
	{
		get
		{
			return this.playerMesh;
		}
	}

	// Token: 0x17000019 RID: 25
	// (get) Token: 0x06000112 RID: 274 RVA: 0x000055AB File Offset: 0x000037AB
	[HideInInspector]
	public AudioSource VoiceAudioSource
	{
		get
		{
			return this.voiceAudioSource;
		}
	}

	// Token: 0x1700001A RID: 26
	// (get) Token: 0x06000113 RID: 275 RVA: 0x000055B3 File Offset: 0x000037B3
	[HideInInspector]
	public float Upwardness
	{
		get
		{
			return Vector3.Dot(base.transform.up, Vector3.up);
		}
	}

	// Token: 0x1700001B RID: 27
	// (get) Token: 0x06000114 RID: 276 RVA: 0x000055CA File Offset: 0x000037CA
	[HideInInspector]
	public bool IsUpright
	{
		get
		{
			return this.Upwardness > this.upwardnessThreshold;
		}
	}

	// Token: 0x1700001C RID: 28
	// (get) Token: 0x06000115 RID: 277 RVA: 0x000055DA File Offset: 0x000037DA
	[HideInInspector]
	public bool IsSlipping
	{
		get
		{
			return this.Upwardness < this.upwardnessThreshold;
		}
	}

	// Token: 0x1700001D RID: 29
	// (get) Token: 0x06000116 RID: 278 RVA: 0x000055EA File Offset: 0x000037EA
	[HideInInspector]
	public bool IsSideways
	{
		get
		{
			return this.Upwardness < this.sidewaysThreshold;
		}
	}

	// Token: 0x1700001E RID: 30
	// (get) Token: 0x06000117 RID: 279 RVA: 0x000055FA File Offset: 0x000037FA
	[HideInInspector]
	public bool IsGrounded
	{
		get
		{
			return this.Hover.IsGrounded && this.IsUpright;
		}
	}

	// Token: 0x1700001F RID: 31
	// (get) Token: 0x06000118 RID: 280 RVA: 0x00005611 File Offset: 0x00003811
	[HideInInspector]
	public bool IsJumping
	{
		get
		{
			return !this.Hover.IsGrounded && this.IsUpright;
		}
	}

	// Token: 0x17000020 RID: 32
	// (get) Token: 0x06000119 RID: 281 RVA: 0x00005628 File Offset: 0x00003828
	[HideInInspector]
	public bool IsBalanced
	{
		get
		{
			return this.KeepUpright.Balance >= 1f;
		}
	}

	// Token: 0x17000021 RID: 33
	// (get) Token: 0x0600011A RID: 282 RVA: 0x0000563F File Offset: 0x0000383F
	[HideInInspector]
	public Transform MovementDirection
	{
		get
		{
			return this.movementDirection;
		}
	}

	// Token: 0x0600011B RID: 283 RVA: 0x00005648 File Offset: 0x00003848
	public virtual void Awake()
	{
		this.Rigidbody = base.GetComponent<Rigidbody>();
		this.MeshRendererHider = base.GetComponent<MeshRendererHider>();
		this.CollisionRecorder = base.GetComponent<CollisionRecorder>();
		CollisionRecorder collisionRecorder = this.CollisionRecorder;
		collisionRecorder.CollisionDeferred = (Action<GameObject, float>)Delegate.Combine(collisionRecorder.CollisionDeferred, new Action<GameObject, float>(this.Server_OnCollisionDeferred));
		this.Movement = base.GetComponent<Movement>();
		this.Movement.MovementDirection = this.MovementDirection;
		this.VelocityLean = base.GetComponent<VelocityLean>();
		this.VelocityLean.MovementDirection = this.MovementDirection;
		this.Hover = base.GetComponent<Hover>();
		this.Skate = base.GetComponent<Skate>();
		this.Skate.MovementDirection = this.MovementDirection;
		this.KeepUpright = base.GetComponent<KeepUpright>();
	}

	// Token: 0x0600011C RID: 284 RVA: 0x00005710 File Offset: 0x00003910
	protected override void OnNetworkPreSpawn(ref NetworkManager networkManager)
	{
		this.InitializeNetworkVariables(default(NetworkObjectReference), 0f, 0f, false, false, false, false, false);
		base.OnNetworkPreSpawn(ref networkManager);
	}

	// Token: 0x0600011D RID: 285 RVA: 0x00005744 File Offset: 0x00003944
	public override void OnNetworkSpawn()
	{
		NetworkVariable<NetworkObjectReference> playerReference = this.PlayerReference;
		playerReference.OnValueChanged = (NetworkVariable<NetworkObjectReference>.OnValueChangedDelegate)Delegate.Combine(playerReference.OnValueChanged, new NetworkVariable<NetworkObjectReference>.OnValueChangedDelegate(this.OnPlayerReferenceChanged));
		this.Stamina.OnRawValueChanged += this.OnStaminaChanged;
		this.Speed.OnRawValueChanged += this.OnSpeedChanged;
		NetworkVariable<bool> isSprinting = this.IsSprinting;
		isSprinting.OnValueChanged = (NetworkVariable<bool>.OnValueChangedDelegate)Delegate.Combine(isSprinting.OnValueChanged, new NetworkVariable<bool>.OnValueChangedDelegate(this.OnIsSprintingChanged));
		NetworkVariable<bool> isSliding = this.IsSliding;
		isSliding.OnValueChanged = (NetworkVariable<bool>.OnValueChangedDelegate)Delegate.Combine(isSliding.OnValueChanged, new NetworkVariable<bool>.OnValueChangedDelegate(this.OnIsSlidingChanged));
		NetworkVariable<bool> isStopping = this.IsStopping;
		isStopping.OnValueChanged = (NetworkVariable<bool>.OnValueChangedDelegate)Delegate.Combine(isStopping.OnValueChanged, new NetworkVariable<bool>.OnValueChangedDelegate(this.OnIsStoppingChanged));
		NetworkVariable<bool> isExtendedLeft = this.IsExtendedLeft;
		isExtendedLeft.OnValueChanged = (NetworkVariable<bool>.OnValueChangedDelegate)Delegate.Combine(isExtendedLeft.OnValueChanged, new NetworkVariable<bool>.OnValueChangedDelegate(this.OnIsExtendedLeftChanged));
		NetworkVariable<bool> isExtendedRight = this.IsExtendedRight;
		isExtendedRight.OnValueChanged = (NetworkVariable<bool>.OnValueChangedDelegate)Delegate.Combine(isExtendedRight.OnValueChanged, new NetworkVariable<bool>.OnValueChangedDelegate(this.OnIsExtendedRightChanged));
		base.OnNetworkSpawn();
	}

	// Token: 0x0600011E RID: 286 RVA: 0x00005870 File Offset: 0x00003A70
	protected override void OnNetworkPostSpawn()
	{
		EventManager.TriggerEvent("Event_Everyone_OnPlayerBodySpawned", new Dictionary<string, object>
		{
			{
				"playerBody",
				this
			}
		});
		if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsConnectedClient)
		{
			this.ProcessInitialNetworkVariableValues();
		}
		base.OnNetworkPostSpawn();
	}

	// Token: 0x0600011F RID: 287 RVA: 0x000058BC File Offset: 0x00003ABC
	protected override void OnNetworkSessionSynchronized()
	{
		this.ProcessInitialNetworkVariableValues();
		base.OnNetworkSessionSynchronized();
	}

	// Token: 0x06000120 RID: 288 RVA: 0x000058CC File Offset: 0x00003ACC
	public override void OnNetworkDespawn()
	{
		EventManager.TriggerEvent("Event_Everyone_OnPlayerBodyDespawned", new Dictionary<string, object>
		{
			{
				"playerBody",
				this
			}
		});
		NetworkVariable<NetworkObjectReference> playerReference = this.PlayerReference;
		playerReference.OnValueChanged = (NetworkVariable<NetworkObjectReference>.OnValueChangedDelegate)Delegate.Remove(playerReference.OnValueChanged, new NetworkVariable<NetworkObjectReference>.OnValueChangedDelegate(this.OnPlayerReferenceChanged));
		this.Stamina.OnRawValueChanged -= this.OnStaminaChanged;
		this.Speed.OnRawValueChanged -= this.OnSpeedChanged;
		NetworkVariable<bool> isSprinting = this.IsSprinting;
		isSprinting.OnValueChanged = (NetworkVariable<bool>.OnValueChangedDelegate)Delegate.Remove(isSprinting.OnValueChanged, new NetworkVariable<bool>.OnValueChangedDelegate(this.OnIsSprintingChanged));
		NetworkVariable<bool> isSliding = this.IsSliding;
		isSliding.OnValueChanged = (NetworkVariable<bool>.OnValueChangedDelegate)Delegate.Remove(isSliding.OnValueChanged, new NetworkVariable<bool>.OnValueChangedDelegate(this.OnIsSlidingChanged));
		NetworkVariable<bool> isStopping = this.IsStopping;
		isStopping.OnValueChanged = (NetworkVariable<bool>.OnValueChangedDelegate)Delegate.Remove(isStopping.OnValueChanged, new NetworkVariable<bool>.OnValueChangedDelegate(this.OnIsStoppingChanged));
		NetworkVariable<bool> isExtendedLeft = this.IsExtendedLeft;
		isExtendedLeft.OnValueChanged = (NetworkVariable<bool>.OnValueChangedDelegate)Delegate.Remove(isExtendedLeft.OnValueChanged, new NetworkVariable<bool>.OnValueChangedDelegate(this.OnIsExtendedLeftChanged));
		NetworkVariable<bool> isExtendedRight = this.IsExtendedRight;
		isExtendedRight.OnValueChanged = (NetworkVariable<bool>.OnValueChangedDelegate)Delegate.Remove(isExtendedRight.OnValueChanged, new NetworkVariable<bool>.OnValueChangedDelegate(this.OnIsExtendedRightChanged));
		base.OnNetworkDespawn();
	}

	// Token: 0x06000121 RID: 289 RVA: 0x00005A14 File Offset: 0x00003C14
	public override void OnDestroy()
	{
		Tween tween = this.balanceLossTween;
		if (tween != null)
		{
			tween.Kill(false);
		}
		Tween tween2 = this.balanceRecoveryTween;
		if (tween2 != null)
		{
			tween2.Kill(false);
		}
		Tween tween3 = this.dashDragTween;
		if (tween3 != null)
		{
			tween3.Kill(false);
		}
		Tween tween4 = this.dashLegPadTween;
		if (tween4 != null)
		{
			tween4.Kill(false);
		}
		CollisionRecorder collisionRecorder = this.CollisionRecorder;
		collisionRecorder.CollisionDeferred = (Action<GameObject, float>)Delegate.Remove(collisionRecorder.CollisionDeferred, new Action<GameObject, float>(this.Server_OnCollisionDeferred));
		base.OnDestroy();
	}

	// Token: 0x06000122 RID: 290 RVA: 0x00005A98 File Offset: 0x00003C98
	public void InitializeNetworkVariables(NetworkObjectReference playerReference = default(NetworkObjectReference), float stamina = 0f, float speed = 0f, bool isSprinting = false, bool isSliding = false, bool isStopping = false, bool isExtendedLeft = false, bool isExtendedRight = false)
	{
		if (this.isNetworkVariablesInitialized)
		{
			return;
		}
		this.isNetworkVariablesInitialized = true;
		this.PlayerReference = new NetworkVariable<NetworkObjectReference>(playerReference, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
		this.Stamina = CompressedNetworkVariable<float, byte>.CreateFloatToByte(0f, 1f, stamina, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
		this.Speed = CompressedNetworkVariable<float, byte>.CreateFloatToByte(0f, 16f, speed, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
		this.IsSprinting = new NetworkVariable<bool>(isSprinting, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
		this.IsSliding = new NetworkVariable<bool>(isSliding, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
		this.IsStopping = new NetworkVariable<bool>(isStopping, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
		this.IsExtendedLeft = new NetworkVariable<bool>(isExtendedLeft, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
		this.IsExtendedRight = new NetworkVariable<bool>(isExtendedRight, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
	}

	// Token: 0x06000123 RID: 291 RVA: 0x00005B40 File Offset: 0x00003D40
	private void ProcessInitialNetworkVariableValues()
	{
		this.OnPlayerReferenceChanged(default(NetworkObjectReference), this.PlayerReference.Value);
		this.OnStaminaChanged(0f, this.Stamina.Value);
		this.OnSpeedChanged(0f, this.Speed.Value);
		this.OnIsSprintingChanged(false, this.IsSprinting.Value);
		this.OnIsSlidingChanged(false, this.IsSliding.Value);
		this.OnIsStoppingChanged(false, this.IsStopping.Value);
		this.OnIsExtendedLeftChanged(false, this.IsExtendedLeft.Value);
		this.OnIsExtendedRightChanged(false, this.IsExtendedRight.Value);
	}

	// Token: 0x06000124 RID: 292 RVA: 0x00005BF0 File Offset: 0x00003DF0
	private void FixedUpdate()
	{
		if (!this.Player)
		{
			return;
		}
		PlayerInput playerInput = this.Player.PlayerInput;
		if (!playerInput)
		{
			return;
		}
		if (playerInput.LookInput.ServerValue || playerInput.TrackInput.ServerValue)
		{
			if (this.PlayerCamera)
			{
				this.playerMesh.LookAt(this.PlayerCamera.transform.position + this.PlayerCamera.transform.forward * 10f, Time.fixedDeltaTime, true, true);
			}
		}
		else if (this.Stick)
		{
			this.playerMesh.LookAt(this.Stick.BladeHandlePosition, Time.fixedDeltaTime, true, true);
		}
		float b = this.IsJumping ? 1.05f : (this.IsSliding.Value ? 0.95f : 1f);
		this.PlayerMesh.Stretch = Mathf.Lerp(this.PlayerMesh.Stretch, b, Time.fixedDeltaTime * this.stretchSpeed);
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (!this.Player.IsReplay.Value)
		{
			this.HandleInputs(playerInput);
		}
		this.Speed.Value = this.Movement.Speed;
		if (this.IsSprinting.Value)
		{
			this.Stamina.Value -= (this.IsSprinting.Value ? (Time.deltaTime / this.sprintStaminaDrainRate) : 0f);
		}
		else if (this.Stamina.Value < 1f)
		{
			this.Stamina.Value += Time.fixedDeltaTime / this.staminaRegenerationRate;
			this.Stamina.Value = Mathf.Clamp(this.Stamina.Value, 0f, 1f);
		}
		if (this.IsUpright)
		{
			this.Rigidbody.AddForce(Vector3.up * -Physics.gravity.y, ForceMode.Acceleration);
			this.Rigidbody.AddForce(Vector3.down * -Physics.gravity.y * this.gravityMultiplier, ForceMode.Acceleration);
		}
		this.MovementDirection.localRotation = Quaternion.FromToRotation(base.transform.forward, Utils.Vector3Slerp3(-base.transform.right, base.transform.forward, base.transform.right, this.Laterality));
		this.Movement.Sprint = this.IsSprinting.Value;
		this.Movement.TurnMultiplier = (this.IsSliding.Value ? this.slideTurnMultiplier : (this.IsJumping ? this.jumpTurnMultiplier : 1f));
		this.Movement.AmbientDrag = (this.HasFallen ? this.fallenDrag : (this.HasDashed ? this.Movement.AmbientDrag : (this.IsStopping.Value ? this.stopDrag : (this.IsSliding.Value ? this.slideDrag : 0f))));
		this.Hover.TargetDistance = (this.IsSliding.Value ? this.slideHoverDistance : (this.KeepUpright.Balance * this.hoverDistance));
		this.Skate.Intensity = ((this.IsSliding.Value || this.IsStopping.Value || !this.IsGrounded) ? 0f : this.KeepUpright.Balance);
		this.VelocityLean.AngularIntensity = Mathf.Max(0.1f, this.Movement.NormalizedMaximumSpeed) / (this.IsSliding.Value ? 2f : (this.IsJumping ? 2f : 1f));
		this.VelocityLean.Inverted = (!this.IsJumping && !this.IsSliding.Value && this.Movement.IsMovingBackwards);
		this.VelocityLean.UseWorldLinearVelocity = (this.IsJumping || this.IsSliding.Value);
		if (!this.HasSlipped && !this.HasFallen && this.IsSlipping)
		{
			this.OnSlip();
		}
		else if (this.HasSlipped && !this.HasFallen && this.IsSideways)
		{
			this.OnFall();
		}
		else if (this.HasFallen && !this.HasSlipped && this.IsUpright)
		{
			this.OnStandUp();
		}
		this.Server_UpdateAudio();
	}

	// Token: 0x06000125 RID: 293 RVA: 0x00006090 File Offset: 0x00004290
	public void ApplyCustomizations()
	{
		if (this.Player.Team == PlayerTeam.None || this.Player.Role == PlayerRole.None)
		{
			return;
		}
		this.PlayerMesh.SetUsername(this.Player.Username.Value.ToString());
		this.PlayerMesh.SetNumber(this.Player.Number.Value.ToString());
		this.PlayerMesh.SetLegsPadsActive(this.Player.Role == PlayerRole.Goalie);
		this.PlayerMesh.SetFlagID(this.Player.FlagID);
		this.PlayerMesh.SetHeadgearID(this.Player.GetPlayerHeadgearID(), this.Player.Role);
		this.PlayerMesh.SetMustacheID(this.Player.MustacheID);
		this.PlayerMesh.SetBeardID(this.Player.BeardID);
		this.PlayerMesh.SetJerseyID(this.Player.GetPlayerJerseyID(), this.Player.Team);
	}

	// Token: 0x06000126 RID: 294 RVA: 0x000061A4 File Offset: 0x000043A4
	private void HandleInputs(PlayerInput playerInput)
	{
		if (!this.IsSprinting.Value && playerInput.SprintInput.ServerValue && !this.IsSliding.Value && this.IsGrounded && this.Stamina.Value > 0.25f)
		{
			this.IsSprinting.Value = true;
		}
		else if (this.IsSprinting.Value && !playerInput.SprintInput.ServerValue)
		{
			this.IsSprinting.Value = false;
		}
		else if (this.IsSprinting.Value)
		{
			this.IsSprinting.Value = (!this.IsSliding.Value && this.IsGrounded && this.Stamina.Value > 0f);
		}
		this.IsSliding.Value = (playerInput.SlideInput.ServerValue && this.IsGrounded);
		this.IsStopping.Value = (playerInput.StopInput.ServerValue && this.IsGrounded);
		if (!this.HasDashExtended)
		{
			this.IsExtendedLeft.Value = (playerInput.ExtendLeftInput.ServerValue && this.IsGrounded && this.IsSliding.Value);
			this.IsExtendedRight.Value = (playerInput.ExtendRightInput.ServerValue && this.IsGrounded && this.IsSliding.Value);
		}
		this.Movement.MoveForwards = (!this.IsSliding.Value && playerInput.MoveInput.ServerValue.y > 0.05f);
		this.Movement.MoveBackwards = (!this.IsSliding.Value && playerInput.MoveInput.ServerValue.y < -0.05f);
		this.Movement.TurnRight = (playerInput.MoveInput.ServerValue.x > 0.05f);
		this.Movement.TurnLeft = (playerInput.MoveInput.ServerValue.x < -0.05f);
		float t = Mathf.Clamp01(1f - this.Movement.NormalizedMinimumSpeed);
		float num = Mathf.Lerp(this.minimumLateralitySpeed, this.maximumLateralitySpeed, t);
		float num2 = Mathf.Lerp(this.minimumLaterality, this.maximumLaterality, t);
		if (playerInput.LateralLeftInput.ServerValue)
		{
			this.Laterality = Mathf.Lerp(this.Laterality, -num2, Time.fixedDeltaTime * num);
			return;
		}
		if (playerInput.LateralRightInput.ServerValue)
		{
			this.Laterality = Mathf.Lerp(this.Laterality, num2, Time.fixedDeltaTime * num);
			return;
		}
		this.Laterality = Mathf.Lerp(this.Laterality, 0f, Time.fixedDeltaTime * num);
	}

	// Token: 0x06000127 RID: 295 RVA: 0x00006464 File Offset: 0x00004664
	public void OnSlip()
	{
		this.HasSlipped = true;
		this.HasFallen = false;
		Tween tween = this.balanceRecoveryTween;
		if (tween != null)
		{
			tween.Kill(false);
		}
		Tween tween2 = this.balanceLossTween;
		if (tween2 != null)
		{
			tween2.Kill(false);
		}
		this.balanceLossTween = DOTween.To(() => this.KeepUpright.Balance, delegate(float value)
		{
			this.KeepUpright.Balance = value;
		}, 0f, this.balanceLossTime).SetEase(Ease.Linear);
	}

	// Token: 0x06000128 RID: 296 RVA: 0x000064D8 File Offset: 0x000046D8
	public void OnFall()
	{
		this.HasSlipped = false;
		this.HasFallen = true;
		Tween tween = this.balanceRecoveryTween;
		if (tween != null)
		{
			tween.Kill(false);
		}
		this.balanceRecoveryTween = DOTween.To(() => this.KeepUpright.Balance, delegate(float value)
		{
			this.KeepUpright.Balance = value;
		}, 1f, this.balanceRecoveryTime).SetEase(Ease.Linear);
	}

	// Token: 0x06000129 RID: 297 RVA: 0x00006539 File Offset: 0x00004739
	public void OnStandUp()
	{
		this.HasSlipped = false;
		this.HasFallen = false;
	}

	// Token: 0x0600012A RID: 298 RVA: 0x0000654C File Offset: 0x0000474C
	public void Jump()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (!this.IsGrounded)
		{
			return;
		}
		if (this.Stamina.Value < this.jumpStaminaDrain)
		{
			return;
		}
		this.Rigidbody.AddForce(Vector3.up * this.jumpVelocity, ForceMode.VelocityChange);
		this.Stamina.Value = Mathf.Max(0f, this.Stamina.Value - this.jumpStaminaDrain);
	}

	// Token: 0x0600012B RID: 299 RVA: 0x000065C8 File Offset: 0x000047C8
	public void TwistLeft()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (!this.IsJumping)
		{
			return;
		}
		if (this.Stamina.Value < this.twistStaminaDrain)
		{
			return;
		}
		this.Rigidbody.AddTorque(-base.transform.up * this.twistVelocity, ForceMode.VelocityChange);
		this.Stamina.Value = Mathf.Max(0f, this.Stamina.Value - this.twistStaminaDrain);
	}

	// Token: 0x0600012C RID: 300 RVA: 0x00006650 File Offset: 0x00004850
	public void TwistRight()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (!this.IsJumping)
		{
			return;
		}
		if (this.Stamina.Value < this.twistStaminaDrain)
		{
			return;
		}
		this.Rigidbody.AddTorque(base.transform.up * this.twistVelocity, ForceMode.VelocityChange);
		this.Stamina.Value = Mathf.Max(0f, this.Stamina.Value - this.twistStaminaDrain);
	}

	// Token: 0x0600012D RID: 301 RVA: 0x000066D0 File Offset: 0x000048D0
	public void DashLeft()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (!this.canDash)
		{
			return;
		}
		if (!this.IsSliding.Value)
		{
			return;
		}
		if (this.Stamina.Value < this.dashStaminaDrain)
		{
			return;
		}
		this.Rigidbody.AddForce(-base.transform.right * this.dashVelocity, ForceMode.VelocityChange);
		this.Stamina.Value = Mathf.Max(0f, this.Stamina.Value - this.dashStaminaDrain);
		this.HasDashed = true;
		this.Movement.AmbientDrag = this.dashDrag;
		Tween tween = this.dashDragTween;
		if (tween != null)
		{
			tween.Kill(false);
		}
		this.dashDragTween = DOTween.To(() => this.Movement.AmbientDrag, delegate(float value)
		{
			this.Movement.AmbientDrag = value;
		}, 0f, this.dashDragTime).OnComplete(delegate
		{
			this.HasDashed = false;
		}).SetEase(Ease.Linear);
		this.HasDashExtended = true;
		this.IsExtendedRight.Value = false;
		this.IsExtendedRight.Value = true;
		Tween tween2 = this.dashLegPadTween;
		if (tween2 != null)
		{
			tween2.Kill(false);
		}
		this.dashLegPadTween = DOVirtual.DelayedCall(this.dashDragTime / 4f, delegate
		{
			this.HasDashExtended = false;
		}, true);
	}

	// Token: 0x0600012E RID: 302 RVA: 0x00006828 File Offset: 0x00004A28
	public void DashRight()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (!this.canDash)
		{
			return;
		}
		if (!this.IsSliding.Value)
		{
			return;
		}
		if (this.Stamina.Value < this.dashStaminaDrain)
		{
			return;
		}
		this.Rigidbody.AddForce(base.transform.right * this.dashVelocity, ForceMode.VelocityChange);
		this.Stamina.Value = Mathf.Max(0f, this.Stamina.Value - this.dashStaminaDrain);
		this.HasDashed = true;
		this.Movement.AmbientDrag = this.dashDrag;
		Tween tween = this.dashDragTween;
		if (tween != null)
		{
			tween.Kill(false);
		}
		this.dashDragTween = DOTween.To(() => this.Movement.AmbientDrag, delegate(float value)
		{
			this.Movement.AmbientDrag = value;
		}, 0f, this.dashDragTime).OnComplete(delegate
		{
			this.HasDashed = false;
		}).SetEase(Ease.Linear);
		this.HasDashExtended = true;
		this.IsExtendedLeft.Value = false;
		this.IsExtendedLeft.Value = true;
		Tween tween2 = this.dashLegPadTween;
		if (tween2 != null)
		{
			tween2.Kill(false);
		}
		this.dashLegPadTween = DOVirtual.DelayedCall(this.dashDragTime / 4f, delegate
		{
			this.HasDashExtended = false;
		}, true);
	}

	// Token: 0x0600012F RID: 303 RVA: 0x0000697C File Offset: 0x00004B7C
	public void CancelDash()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		Tween tween = this.dashDragTween;
		if (tween != null)
		{
			tween.Kill(false);
		}
		this.HasDashed = false;
		Tween tween2 = this.dashLegPadTween;
		if (tween2 != null)
		{
			tween2.Kill(false);
		}
		this.HasDashExtended = false;
		this.IsExtendedLeft.Value = false;
		this.IsExtendedRight.Value = false;
	}

	// Token: 0x06000130 RID: 304 RVA: 0x000069E0 File Offset: 0x00004BE0
	private void OnPlayerReferenceChanged(NetworkObjectReference oldReference, NetworkObjectReference newReference)
	{
		if (!this.Player)
		{
			return;
		}
		this.Player.PlayerBody = this;
		this.ApplyCustomizations();
		if (this.Player.IsReplay.Value)
		{
			this.Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
			this.Rigidbody.interpolation = RigidbodyInterpolation.None;
		}
	}

	// Token: 0x06000131 RID: 305 RVA: 0x00006A38 File Offset: 0x00004C38
	private void OnStaminaChanged(float oldStamina, float newStamina)
	{
		EventManager.TriggerEvent("Event_Everyone_OnPlayerBodyStaminaChanged", new Dictionary<string, object>
		{
			{
				"playerBody",
				this
			},
			{
				"oldStamina",
				oldStamina
			},
			{
				"newStamina",
				newStamina
			}
		});
	}

	// Token: 0x06000132 RID: 306 RVA: 0x00006A77 File Offset: 0x00004C77
	private void OnSpeedChanged(float oldSpeed, float newSpeed)
	{
		EventManager.TriggerEvent("Event_Everyone_OnPlayerBodySpeedChanged", new Dictionary<string, object>
		{
			{
				"playerBody",
				this
			},
			{
				"oldSpeed",
				oldSpeed
			},
			{
				"newSpeed",
				newSpeed
			}
		});
	}

	// Token: 0x06000133 RID: 307 RVA: 0x00006AB6 File Offset: 0x00004CB6
	private void OnIsSprintingChanged(bool oldIsSprinting, bool newIsSprinting)
	{
		EventManager.TriggerEvent("Event_Everyone_OnPlayerBodyIsSprintingChanged", new Dictionary<string, object>
		{
			{
				"playerBody",
				this
			},
			{
				"oldIsSprinting",
				oldIsSprinting
			},
			{
				"newIsSprinting",
				newIsSprinting
			}
		});
	}

	// Token: 0x06000134 RID: 308 RVA: 0x00006AF8 File Offset: 0x00004CF8
	private void OnIsSlidingChanged(bool oldIsSliding, bool newIsSliding)
	{
		this.PlayerMesh.PlayerLegPadLeft.State = (newIsSliding ? (this.IsExtendedLeft.Value ? PlayerLegPadState.ButterflyExtended : PlayerLegPadState.Butterfly) : PlayerLegPadState.Idle);
		this.PlayerMesh.PlayerLegPadRight.State = (newIsSliding ? (this.IsExtendedRight.Value ? PlayerLegPadState.ButterflyExtended : PlayerLegPadState.Butterfly) : PlayerLegPadState.Idle);
		this.CancelDash();
		EventManager.TriggerEvent("Event_Everyone_OnPlayerBodyIsSlidingChanged", new Dictionary<string, object>
		{
			{
				"playerBody",
				this
			},
			{
				"oldIsSliding",
				oldIsSliding
			},
			{
				"newIsSliding",
				newIsSliding
			}
		});
	}

	// Token: 0x06000135 RID: 309 RVA: 0x00006B96 File Offset: 0x00004D96
	private void OnIsStoppingChanged(bool oldIsStopping, bool newIsStopping)
	{
		EventManager.TriggerEvent("Event_Everyone_OnPlayerBodyIsStoppingChanged", new Dictionary<string, object>
		{
			{
				"playerBody",
				this
			},
			{
				"oldIsStopping",
				oldIsStopping
			},
			{
				"newIsStopping",
				newIsStopping
			}
		});
	}

	// Token: 0x06000136 RID: 310 RVA: 0x00006BD8 File Offset: 0x00004DD8
	private void OnIsExtendedLeftChanged(bool oldIsExtendedLeft, bool newIsExtendedLeft)
	{
		this.PlayerMesh.PlayerLegPadLeft.State = (this.IsSliding.Value ? (newIsExtendedLeft ? PlayerLegPadState.ButterflyExtended : PlayerLegPadState.Butterfly) : PlayerLegPadState.Idle);
		EventManager.TriggerEvent("Event_Everyone_OnPlayerBodyIsExtendedLeftChanged", new Dictionary<string, object>
		{
			{
				"playerBody",
				this
			},
			{
				"oldIsExtendedLeft",
				oldIsExtendedLeft
			},
			{
				"newIsExtendedLeft",
				newIsExtendedLeft
			}
		});
	}

	// Token: 0x06000137 RID: 311 RVA: 0x00006C4C File Offset: 0x00004E4C
	private void OnIsExtendedRightChanged(bool oldIsExtendedRight, bool newIsExtendedRight)
	{
		this.PlayerMesh.PlayerLegPadRight.State = (this.IsSliding.Value ? (newIsExtendedRight ? PlayerLegPadState.ButterflyExtended : PlayerLegPadState.Butterfly) : PlayerLegPadState.Idle);
		EventManager.TriggerEvent("Event_Everyone_OnPlayerBodyIsExtendedRightChanged", new Dictionary<string, object>
		{
			{
				"playerBody",
				this
			},
			{
				"oldIsExtendedRight",
				oldIsExtendedRight
			},
			{
				"newIsExtendedRight",
				newIsExtendedRight
			}
		});
	}

	// Token: 0x06000138 RID: 312 RVA: 0x00006CC0 File Offset: 0x00004EC0
	public void Server_Teleport(Vector3 position, Quaternion rotation)
	{
		base.transform.position = position;
		base.transform.rotation = rotation;
		this.Rigidbody.position = position;
		this.Rigidbody.rotation = rotation;
		this.Rigidbody.linearVelocity = Vector3.zero;
		this.Rigidbody.angularVelocity = Vector3.zero;
		this.Stick.Teleport(position, rotation);
	}

	// Token: 0x06000139 RID: 313 RVA: 0x00006D2A File Offset: 0x00004F2A
	public void Server_Freeze()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.Rigidbody.constraints = (RigidbodyConstraints)10;
	}

	// Token: 0x0600013A RID: 314 RVA: 0x00006D46 File Offset: 0x00004F46
	public void Server_Unfreeze()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.Rigidbody.constraints = RigidbodyConstraints.None;
	}

	// Token: 0x0600013B RID: 315 RVA: 0x00006D64 File Offset: 0x00004F64
	private void Server_UpdateAudio()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		float time = Mathf.Min(this.Movement.NormalizedMaximumSpeed, 1f);
		this.windAudioSource.Server_SetVolume(this.windVolumeCurve.Evaluate(time));
		float num = (!this.IsGrounded) ? 0f : (this.IsStopping.Value ? 3f : (this.IsSliding.Value ? 1.5f : (this.Skate.IsTractionLost ? 2f : 1f)));
		float num2 = this.IsStopping.Value ? 3f : (this.IsSliding.Value ? 1.5f : (this.Skate.IsTractionLost ? 2f : 1f));
		float time2 = Mathf.Min(this.Movement.NormalizedMaximumSpeed, 1f);
		this.iceAudioSource.Server_SetVolume(this.iceVolumeCurve.Evaluate(time2) * num);
		float time3 = Mathf.Min(this.Movement.NormalizedMaximumSpeed, 1f);
		this.iceAudioSource.Server_SetPitch(this.icePitchCurve.Evaluate(time3) * num2);
	}

	// Token: 0x0600013C RID: 316 RVA: 0x00006EA0 File Offset: 0x000050A0
	private void Server_OnCollisionDeferred(GameObject gameObject, float force)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.gruntAudioSource.Server_Play(this.gruntVolumeCurve.Evaluate(force), this.gruntPitchCurve.Evaluate(force), true, -1, 0f, true, false, false, 0f, false, 0f, -1f);
	}

	// Token: 0x0600013D RID: 317 RVA: 0x00006EF8 File Offset: 0x000050F8
	private void OnCollisionEnter(Collision collision)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (collision.gameObject.layer != LayerMask.NameToLayer("Player"))
		{
			return;
		}
		PlayerBody component = collision.gameObject.GetComponent<PlayerBody>();
		if (!component)
		{
			return;
		}
		float normalizedMaximumSpeed = this.Movement.NormalizedMaximumSpeed;
		float normalizedMaximumSpeed2 = component.Movement.NormalizedMaximumSpeed;
		float collisionForce = Utils.GetCollisionForce(collision);
		if (this.Speed.Value < this.tackleSpeedThreshold)
		{
			return;
		}
		if (normalizedMaximumSpeed < normalizedMaximumSpeed2)
		{
			return;
		}
		if (collisionForce < this.tackleForceThreshold)
		{
			return;
		}
		if (this.IsGrounded)
		{
			return;
		}
		if (!this.IsBalanced)
		{
			return;
		}
		if (this.HasSlipped || this.HasFallen)
		{
			return;
		}
		component.OnSlip();
		Vector3 a = Vector3.ClampMagnitude(collision.relativeVelocity, this.tackleBounceMaximumMagnitude);
		component.Rigidbody.AddForceAtPosition(-a * this.tackleForceMultiplier, this.Rigidbody.worldCenterOfMass + base.transform.up * 0.5f, ForceMode.VelocityChange);
	}

	// Token: 0x0600014B RID: 331 RVA: 0x000071B8 File Offset: 0x000053B8
	protected override void __initializeVariables()
	{
		bool flag = this.PlayerReference == null;
		if (flag)
		{
			throw new Exception("PlayerBody.PlayerReference cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.PlayerReference.Initialize(this);
		base.__nameNetworkVariable(this.PlayerReference, "PlayerReference");
		this.NetworkVariableFields.Add(this.PlayerReference);
		flag = (this.Stamina == null);
		if (flag)
		{
			throw new Exception("PlayerBody.Stamina cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.Stamina.Initialize(this);
		base.__nameNetworkVariable(this.Stamina, "Stamina");
		this.NetworkVariableFields.Add(this.Stamina);
		flag = (this.Speed == null);
		if (flag)
		{
			throw new Exception("PlayerBody.Speed cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.Speed.Initialize(this);
		base.__nameNetworkVariable(this.Speed, "Speed");
		this.NetworkVariableFields.Add(this.Speed);
		flag = (this.IsSprinting == null);
		if (flag)
		{
			throw new Exception("PlayerBody.IsSprinting cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.IsSprinting.Initialize(this);
		base.__nameNetworkVariable(this.IsSprinting, "IsSprinting");
		this.NetworkVariableFields.Add(this.IsSprinting);
		flag = (this.IsSliding == null);
		if (flag)
		{
			throw new Exception("PlayerBody.IsSliding cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.IsSliding.Initialize(this);
		base.__nameNetworkVariable(this.IsSliding, "IsSliding");
		this.NetworkVariableFields.Add(this.IsSliding);
		flag = (this.IsStopping == null);
		if (flag)
		{
			throw new Exception("PlayerBody.IsStopping cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.IsStopping.Initialize(this);
		base.__nameNetworkVariable(this.IsStopping, "IsStopping");
		this.NetworkVariableFields.Add(this.IsStopping);
		flag = (this.IsExtendedLeft == null);
		if (flag)
		{
			throw new Exception("PlayerBody.IsExtendedLeft cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.IsExtendedLeft.Initialize(this);
		base.__nameNetworkVariable(this.IsExtendedLeft, "IsExtendedLeft");
		this.NetworkVariableFields.Add(this.IsExtendedLeft);
		flag = (this.IsExtendedRight == null);
		if (flag)
		{
			throw new Exception("PlayerBody.IsExtendedRight cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.IsExtendedRight.Initialize(this);
		base.__nameNetworkVariable(this.IsExtendedRight, "IsExtendedRight");
		this.NetworkVariableFields.Add(this.IsExtendedRight);
		base.__initializeVariables();
	}

	// Token: 0x0600014C RID: 332 RVA: 0x000021BE File Offset: 0x000003BE
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x0600014D RID: 333 RVA: 0x00007436 File Offset: 0x00005636
	protected internal override string __getTypeName()
	{
		return "PlayerBody";
	}

	// Token: 0x040000C8 RID: 200
	[Header("Settings")]
	[SerializeField]
	private float gravityMultiplier = 2f;

	// Token: 0x040000C9 RID: 201
	[SerializeField]
	private float hoverDistance = 1.2f;

	// Token: 0x040000CA RID: 202
	[Space(20f)]
	[SerializeField]
	private float upwardnessThreshold = 0.8f;

	// Token: 0x040000CB RID: 203
	[SerializeField]
	private float sidewaysThreshold = 0.2f;

	// Token: 0x040000CC RID: 204
	[Space(20f)]
	[SerializeField]
	private float balanceLossTime = 0.25f;

	// Token: 0x040000CD RID: 205
	[SerializeField]
	private float balanceRecoveryTime = 5f;

	// Token: 0x040000CE RID: 206
	[Space(20f)]
	[SerializeField]
	private float staminaRegenerationRate = 10f;

	// Token: 0x040000CF RID: 207
	[Space(20f)]
	[SerializeField]
	private float sprintStaminaDrainRate = 1.4f;

	// Token: 0x040000D0 RID: 208
	[Space(20f)]
	[SerializeField]
	private float slideTurnMultiplier = 2f;

	// Token: 0x040000D1 RID: 209
	[SerializeField]
	private float slideHoverDistance = 0.8f;

	// Token: 0x040000D2 RID: 210
	[Space(20f)]
	[SerializeField]
	private float jumpVelocity = 6f;

	// Token: 0x040000D3 RID: 211
	[SerializeField]
	private float jumpStaminaDrain = 0.125f;

	// Token: 0x040000D4 RID: 212
	[SerializeField]
	private float jumpTurnMultiplier = 5f;

	// Token: 0x040000D5 RID: 213
	[Space(20f)]
	[SerializeField]
	private float twistVelocity = 5f;

	// Token: 0x040000D6 RID: 214
	[SerializeField]
	private float twistStaminaDrain = 0.125f;

	// Token: 0x040000D7 RID: 215
	[Space(20f)]
	[SerializeField]
	private bool canDash = true;

	// Token: 0x040000D8 RID: 216
	[SerializeField]
	private float dashVelocity = 6f;

	// Token: 0x040000D9 RID: 217
	[SerializeField]
	private float dashStaminaDrain = 0.125f;

	// Token: 0x040000DA RID: 218
	[SerializeField]
	private float dashDrag = 5f;

	// Token: 0x040000DB RID: 219
	[SerializeField]
	private float dashDragTime = 1f;

	// Token: 0x040000DC RID: 220
	[Space(20f)]
	[SerializeField]
	private float slideDrag = 0.2f;

	// Token: 0x040000DD RID: 221
	[SerializeField]
	private float stopDrag = 2.5f;

	// Token: 0x040000DE RID: 222
	[SerializeField]
	private float fallenDrag = 0.2f;

	// Token: 0x040000DF RID: 223
	[Space(20f)]
	[SerializeField]
	private float tackleSpeedThreshold = 7.6f;

	// Token: 0x040000E0 RID: 224
	[SerializeField]
	private float tackleForceThreshold = 7f;

	// Token: 0x040000E1 RID: 225
	[SerializeField]
	private float tackleForceMultiplier = 0.3f;

	// Token: 0x040000E2 RID: 226
	[SerializeField]
	private float tackleBounceMaximumMagnitude = 10f;

	// Token: 0x040000E3 RID: 227
	[Space(20f)]
	[SerializeField]
	private float stretchSpeed = 10f;

	// Token: 0x040000E4 RID: 228
	[Space(20f)]
	[SerializeField]
	private float maximumLaterality = 1f;

	// Token: 0x040000E5 RID: 229
	[SerializeField]
	private float minimumLaterality = 0.5f;

	// Token: 0x040000E6 RID: 230
	[SerializeField]
	private float minimumLateralitySpeed = 2f;

	// Token: 0x040000E7 RID: 231
	[SerializeField]
	private float maximumLateralitySpeed = 5f;

	// Token: 0x040000E8 RID: 232
	[Space(20f)]
	[SerializeField]
	private AnimationCurve windVolumeCurve;

	// Token: 0x040000E9 RID: 233
	[SerializeField]
	private AnimationCurve iceVolumeCurve;

	// Token: 0x040000EA RID: 234
	[SerializeField]
	private AnimationCurve icePitchCurve;

	// Token: 0x040000EB RID: 235
	[SerializeField]
	private AnimationCurve gruntVolumeCurve;

	// Token: 0x040000EC RID: 236
	[SerializeField]
	private AnimationCurve gruntPitchCurve;

	// Token: 0x040000ED RID: 237
	[Header("References")]
	[SerializeField]
	private Transform movementDirection;

	// Token: 0x040000EE RID: 238
	[SerializeField]
	private PlayerMesh playerMesh;

	// Token: 0x040000EF RID: 239
	[SerializeField]
	private SynchronizedAudio windAudioSource;

	// Token: 0x040000F0 RID: 240
	[SerializeField]
	private SynchronizedAudio iceAudioSource;

	// Token: 0x040000F1 RID: 241
	[SerializeField]
	private SynchronizedAudio gruntAudioSource;

	// Token: 0x040000F2 RID: 242
	[SerializeField]
	private AudioSource voiceAudioSource;

	// Token: 0x040000F3 RID: 243
	[HideInInspector]
	public NetworkVariable<NetworkObjectReference> PlayerReference;

	// Token: 0x040000F4 RID: 244
	[HideInInspector]
	public CompressedNetworkVariable<float, byte> Stamina;

	// Token: 0x040000F5 RID: 245
	[HideInInspector]
	public CompressedNetworkVariable<float, byte> Speed;

	// Token: 0x040000F6 RID: 246
	[HideInInspector]
	public NetworkVariable<bool> IsSprinting;

	// Token: 0x040000F7 RID: 247
	[HideInInspector]
	public NetworkVariable<bool> IsSliding;

	// Token: 0x040000F8 RID: 248
	[HideInInspector]
	public NetworkVariable<bool> IsStopping;

	// Token: 0x040000F9 RID: 249
	[HideInInspector]
	public NetworkVariable<bool> IsExtendedLeft;

	// Token: 0x040000FA RID: 250
	[HideInInspector]
	public NetworkVariable<bool> IsExtendedRight;

	// Token: 0x040000FB RID: 251
	[HideInInspector]
	public Rigidbody Rigidbody;

	// Token: 0x040000FC RID: 252
	[HideInInspector]
	public Movement Movement;

	// Token: 0x040000FD RID: 253
	[HideInInspector]
	public VelocityLean VelocityLean;

	// Token: 0x040000FE RID: 254
	[HideInInspector]
	public Hover Hover;

	// Token: 0x040000FF RID: 255
	[HideInInspector]
	public Skate Skate;

	// Token: 0x04000100 RID: 256
	[HideInInspector]
	public KeepUpright KeepUpright;

	// Token: 0x04000101 RID: 257
	[HideInInspector]
	public MeshRendererHider MeshRendererHider;

	// Token: 0x04000102 RID: 258
	[HideInInspector]
	public CollisionRecorder CollisionRecorder;

	// Token: 0x04000103 RID: 259
	[HideInInspector]
	public bool HasDashed;

	// Token: 0x04000104 RID: 260
	[HideInInspector]
	public bool HasDashExtended;

	// Token: 0x04000105 RID: 261
	[HideInInspector]
	public bool HasSlipped;

	// Token: 0x04000106 RID: 262
	[HideInInspector]
	public bool HasFallen;

	// Token: 0x04000107 RID: 263
	[HideInInspector]
	public float Laterality;

	// Token: 0x04000108 RID: 264
	private bool isNetworkVariablesInitialized;

	// Token: 0x04000109 RID: 265
	private Tween balanceLossTween;

	// Token: 0x0400010A RID: 266
	private Tween balanceRecoveryTween;

	// Token: 0x0400010B RID: 267
	private Tween dashDragTween;

	// Token: 0x0400010C RID: 268
	private Tween dashLegPadTween;
}
