using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

// Token: 0x020000DF RID: 223
public class PlayerBodyV2 : NetworkBehaviour
{
	// Token: 0x1700009E RID: 158
	// (get) Token: 0x060006BE RID: 1726 RVA: 0x0000B675 File Offset: 0x00009875
	// (set) Token: 0x060006BF RID: 1727 RVA: 0x0000B689 File Offset: 0x00009889
	[HideInInspector]
	public float Stamina
	{
		get
		{
			return (float)this.StaminaCompressed.Value / 16383f;
		}
		set
		{
			this.StaminaCompressed.Value = (short)(value * 16383f);
		}
	}

	// Token: 0x1700009F RID: 159
	// (get) Token: 0x060006C0 RID: 1728 RVA: 0x0000B69E File Offset: 0x0000989E
	// (set) Token: 0x060006C1 RID: 1729 RVA: 0x0000B6B2 File Offset: 0x000098B2
	[HideInInspector]
	public float Speed
	{
		get
		{
			return (float)this.SpeedCompressed.Value / 327f;
		}
		set
		{
			this.SpeedCompressed.Value = (short)(value * 327f);
		}
	}

	// Token: 0x170000A0 RID: 160
	// (get) Token: 0x060006C2 RID: 1730 RVA: 0x0000B6C7 File Offset: 0x000098C7
	[HideInInspector]
	public PlayerCamera PlayerCamera
	{
		get
		{
			return this.Player.PlayerCamera;
		}
	}

	// Token: 0x170000A1 RID: 161
	// (get) Token: 0x060006C3 RID: 1731 RVA: 0x0000B6D4 File Offset: 0x000098D4
	[HideInInspector]
	public Stick Stick
	{
		get
		{
			return this.Player.Stick;
		}
	}

	// Token: 0x170000A2 RID: 162
	// (get) Token: 0x060006C4 RID: 1732 RVA: 0x0000B6E1 File Offset: 0x000098E1
	[HideInInspector]
	public PlayerMesh PlayerMesh
	{
		get
		{
			return this.playerMesh;
		}
	}

	// Token: 0x170000A3 RID: 163
	// (get) Token: 0x060006C5 RID: 1733 RVA: 0x0000B6E9 File Offset: 0x000098E9
	[HideInInspector]
	public AudioSource VoiceAudioSource
	{
		get
		{
			return this.voiceAudioSource;
		}
	}

	// Token: 0x170000A4 RID: 164
	// (get) Token: 0x060006C6 RID: 1734 RVA: 0x0000B6F1 File Offset: 0x000098F1
	[HideInInspector]
	public float Upwardness
	{
		get
		{
			return Vector3.Dot(base.transform.up, Vector3.up);
		}
	}

	// Token: 0x170000A5 RID: 165
	// (get) Token: 0x060006C7 RID: 1735 RVA: 0x0000B708 File Offset: 0x00009908
	[HideInInspector]
	public bool IsUpright
	{
		get
		{
			return this.Upwardness > this.upwardnessThreshold;
		}
	}

	// Token: 0x170000A6 RID: 166
	// (get) Token: 0x060006C8 RID: 1736 RVA: 0x0000B718 File Offset: 0x00009918
	[HideInInspector]
	public bool IsSlipping
	{
		get
		{
			return this.Upwardness < this.upwardnessThreshold;
		}
	}

	// Token: 0x170000A7 RID: 167
	// (get) Token: 0x060006C9 RID: 1737 RVA: 0x0000B728 File Offset: 0x00009928
	[HideInInspector]
	public bool IsSideways
	{
		get
		{
			return this.Upwardness < this.sidewaysThreshold;
		}
	}

	// Token: 0x170000A8 RID: 168
	// (get) Token: 0x060006CA RID: 1738 RVA: 0x0000B738 File Offset: 0x00009938
	[HideInInspector]
	public bool IsGrounded
	{
		get
		{
			return this.Hover.IsGrounded && this.IsUpright;
		}
	}

	// Token: 0x170000A9 RID: 169
	// (get) Token: 0x060006CB RID: 1739 RVA: 0x0000B74F File Offset: 0x0000994F
	[HideInInspector]
	public bool IsJumping
	{
		get
		{
			return !this.Hover.IsGrounded && this.IsUpright;
		}
	}

	// Token: 0x170000AA RID: 170
	// (get) Token: 0x060006CC RID: 1740 RVA: 0x0000B766 File Offset: 0x00009966
	[HideInInspector]
	public bool IsBalanced
	{
		get
		{
			return this.KeepUpright.Balance >= 1f;
		}
	}

	// Token: 0x170000AB RID: 171
	// (get) Token: 0x060006CD RID: 1741 RVA: 0x0000B77D File Offset: 0x0000997D
	[HideInInspector]
	public Transform MovementDirection
	{
		get
		{
			return this.movementDirection;
		}
	}

	// Token: 0x060006CE RID: 1742 RVA: 0x00027334 File Offset: 0x00025534
	public virtual void Awake()
	{
		this.Rigidbody = base.GetComponent<Rigidbody>();
		this.MeshRendererHider = base.GetComponent<MeshRendererHider>();
		this.CollisionRecorder = base.GetComponent<CollisionRecorder>();
		CollisionRecorder collisionRecorder = this.CollisionRecorder;
		collisionRecorder.OnDeferredCollision = (Action<GameObject, float>)Delegate.Combine(collisionRecorder.OnDeferredCollision, new Action<GameObject, float>(this.Server_OnDeferredCollision));
		this.Movement = base.GetComponent<Movement>();
		this.Movement.MovementDirection = this.MovementDirection;
		this.VelocityLean = base.GetComponent<VelocityLean>();
		this.VelocityLean.MovementDirection = this.MovementDirection;
		this.Hover = base.GetComponent<Hover>();
		this.Skate = base.GetComponent<Skate>();
		this.Skate.MovementDirection = this.MovementDirection;
		this.KeepUpright = base.GetComponent<KeepUpright>();
	}

	// Token: 0x060006CF RID: 1743 RVA: 0x000273FC File Offset: 0x000255FC
	public override void OnNetworkSpawn()
	{
		MonoBehaviourSingleton<InputManager>.Instance.DebugTackleAction.performed += this.OnDebugTackleActionPerformed;
		this.StaminaCompressed.Initialize(this);
		NetworkVariable<short> staminaCompressed = this.StaminaCompressed;
		staminaCompressed.OnValueChanged = (NetworkVariable<short>.OnValueChangedDelegate)Delegate.Combine(staminaCompressed.OnValueChanged, new NetworkVariable<short>.OnValueChangedDelegate(this.OnStaminaChanged));
		this.SpeedCompressed.Initialize(this);
		NetworkVariable<short> speedCompressed = this.SpeedCompressed;
		speedCompressed.OnValueChanged = (NetworkVariable<short>.OnValueChangedDelegate)Delegate.Combine(speedCompressed.OnValueChanged, new NetworkVariable<short>.OnValueChangedDelegate(this.OnSpeedChanged));
		this.IsSprinting.Initialize(this);
		NetworkVariable<bool> isSprinting = this.IsSprinting;
		isSprinting.OnValueChanged = (NetworkVariable<bool>.OnValueChangedDelegate)Delegate.Combine(isSprinting.OnValueChanged, new NetworkVariable<bool>.OnValueChangedDelegate(this.OnIsSprintingChanged));
		this.IsSliding.Initialize(this);
		NetworkVariable<bool> isSliding = this.IsSliding;
		isSliding.OnValueChanged = (NetworkVariable<bool>.OnValueChangedDelegate)Delegate.Combine(isSliding.OnValueChanged, new NetworkVariable<bool>.OnValueChangedDelegate(this.OnIsSlidingChanged));
		this.IsStopping.Initialize(this);
		NetworkVariable<bool> isStopping = this.IsStopping;
		isStopping.OnValueChanged = (NetworkVariable<bool>.OnValueChangedDelegate)Delegate.Combine(isStopping.OnValueChanged, new NetworkVariable<bool>.OnValueChangedDelegate(this.OnIsStoppingChanged));
		this.IsExtendedLeft.Initialize(this);
		NetworkVariable<bool> isExtendedLeft = this.IsExtendedLeft;
		isExtendedLeft.OnValueChanged = (NetworkVariable<bool>.OnValueChangedDelegate)Delegate.Combine(isExtendedLeft.OnValueChanged, new NetworkVariable<bool>.OnValueChangedDelegate(this.OnIsExtendedLeftChanged));
		this.IsExtendedRight.Initialize(this);
		NetworkVariable<bool> isExtendedRight = this.IsExtendedRight;
		isExtendedRight.OnValueChanged = (NetworkVariable<bool>.OnValueChangedDelegate)Delegate.Combine(isExtendedRight.OnValueChanged, new NetworkVariable<bool>.OnValueChangedDelegate(this.OnIsExtendedRightChanged));
		this.Client_InitializeNetworkVariables();
		if (NetworkManager.Singleton.IsServer)
		{
			this.Stamina = 1f;
		}
		base.OnNetworkSpawn();
	}

	// Token: 0x060006D0 RID: 1744 RVA: 0x000275AC File Offset: 0x000257AC
	protected override void OnNetworkPostSpawn()
	{
		NetworkObject networkObject;
		if (this.PlayerReference.Value.TryGet(out networkObject, null))
		{
			this.Player = networkObject.GetComponent<Player>();
		}
		if (this.Player)
		{
			this.Player.PlayerBody = this;
		}
		if (this.Player && this.Player.IsReplay.Value)
		{
			this.Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
			this.Rigidbody.interpolation = RigidbodyInterpolation.None;
		}
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnPlayerBodySpawned", new Dictionary<string, object>
		{
			{
				"playerBody",
				this
			}
		});
		base.OnNetworkPostSpawn();
	}

	// Token: 0x060006D1 RID: 1745 RVA: 0x0000B785 File Offset: 0x00009985
	protected override void OnNetworkSessionSynchronized()
	{
		this.Client_InitializeNetworkVariables();
		base.OnNetworkSessionSynchronized();
	}

	// Token: 0x060006D2 RID: 1746 RVA: 0x00027654 File Offset: 0x00025854
	public override void OnNetworkDespawn()
	{
		MonoBehaviourSingleton<InputManager>.Instance.DebugTackleAction.performed -= this.OnDebugTackleActionPerformed;
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnPlayerBodyDespawned", new Dictionary<string, object>
		{
			{
				"playerBody",
				this
			}
		});
		NetworkVariable<short> staminaCompressed = this.StaminaCompressed;
		staminaCompressed.OnValueChanged = (NetworkVariable<short>.OnValueChangedDelegate)Delegate.Remove(staminaCompressed.OnValueChanged, new NetworkVariable<short>.OnValueChangedDelegate(this.OnStaminaChanged));
		this.StaminaCompressed.Dispose();
		NetworkVariable<short> speedCompressed = this.SpeedCompressed;
		speedCompressed.OnValueChanged = (NetworkVariable<short>.OnValueChangedDelegate)Delegate.Remove(speedCompressed.OnValueChanged, new NetworkVariable<short>.OnValueChangedDelegate(this.OnSpeedChanged));
		this.SpeedCompressed.Dispose();
		NetworkVariable<bool> isSprinting = this.IsSprinting;
		isSprinting.OnValueChanged = (NetworkVariable<bool>.OnValueChangedDelegate)Delegate.Remove(isSprinting.OnValueChanged, new NetworkVariable<bool>.OnValueChangedDelegate(this.OnIsSprintingChanged));
		this.IsSprinting.Dispose();
		NetworkVariable<bool> isSliding = this.IsSliding;
		isSliding.OnValueChanged = (NetworkVariable<bool>.OnValueChangedDelegate)Delegate.Remove(isSliding.OnValueChanged, new NetworkVariable<bool>.OnValueChangedDelegate(this.OnIsSlidingChanged));
		this.IsSliding.Dispose();
		NetworkVariable<bool> isStopping = this.IsStopping;
		isStopping.OnValueChanged = (NetworkVariable<bool>.OnValueChangedDelegate)Delegate.Remove(isStopping.OnValueChanged, new NetworkVariable<bool>.OnValueChangedDelegate(this.OnIsStoppingChanged));
		this.IsStopping.Dispose();
		NetworkVariable<bool> isExtendedLeft = this.IsExtendedLeft;
		isExtendedLeft.OnValueChanged = (NetworkVariable<bool>.OnValueChangedDelegate)Delegate.Remove(isExtendedLeft.OnValueChanged, new NetworkVariable<bool>.OnValueChangedDelegate(this.OnIsExtendedLeftChanged));
		this.IsExtendedLeft.Dispose();
		NetworkVariable<bool> isExtendedRight = this.IsExtendedRight;
		isExtendedRight.OnValueChanged = (NetworkVariable<bool>.OnValueChangedDelegate)Delegate.Remove(isExtendedRight.OnValueChanged, new NetworkVariable<bool>.OnValueChangedDelegate(this.OnIsExtendedRightChanged));
		this.IsExtendedRight.Dispose();
		base.OnNetworkDespawn();
	}

	// Token: 0x060006D3 RID: 1747 RVA: 0x00027800 File Offset: 0x00025A00
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
		collisionRecorder.OnDeferredCollision = (Action<GameObject, float>)Delegate.Remove(collisionRecorder.OnDeferredCollision, new Action<GameObject, float>(this.Server_OnDeferredCollision));
		base.OnDestroy();
	}

	// Token: 0x060006D4 RID: 1748 RVA: 0x00027884 File Offset: 0x00025A84
	public virtual void FixedUpdate()
	{
		if (!this.Player)
		{
			return;
		}
		global::PlayerInput playerInput = this.Player.PlayerInput;
		if (!playerInput)
		{
			return;
		}
		if (playerInput.LookInput.ServerValue || playerInput.TrackInput.ServerValue)
		{
			if (this.PlayerCamera)
			{
				this.playerMesh.LookAt(this.PlayerCamera.transform.position + this.PlayerCamera.transform.forward * 10f, Time.fixedDeltaTime);
			}
		}
		else if (this.Stick)
		{
			this.playerMesh.LookAt(this.Stick.BladeHandlePosition, Time.fixedDeltaTime);
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
		this.Speed = this.Movement.Speed;
		if (this.IsSprinting.Value)
		{
			this.Stamina -= (this.IsSprinting.Value ? (Time.deltaTime / this.sprintStaminaDrainRate) : 0f);
		}
		else if (this.Stamina < 1f)
		{
			this.Stamina += Time.fixedDeltaTime / this.staminaRegenerationRate;
			this.Stamina = Mathf.Clamp(this.Stamina, 0f, 1f);
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

	// Token: 0x060006D5 RID: 1749 RVA: 0x00027D04 File Offset: 0x00025F04
	private void HandleInputs(global::PlayerInput playerInput)
	{
		if (!this.IsSprinting.Value && playerInput.SprintInput.ServerValue && !this.IsSliding.Value && this.IsGrounded && this.Stamina > 0.25f)
		{
			this.IsSprinting.Value = true;
		}
		else if (this.IsSprinting.Value && !playerInput.SprintInput.ServerValue)
		{
			this.IsSprinting.Value = false;
		}
		else if (this.IsSprinting.Value)
		{
			this.IsSprinting.Value = (!this.IsSliding.Value && this.IsGrounded && this.Stamina > 0f);
		}
		this.IsSliding.Value = (playerInput.SlideInput.ServerValue && this.IsGrounded);
		this.IsStopping.Value = (playerInput.StopInput.ServerValue && this.IsGrounded);
		if (!this.HasDashExtended)
		{
			this.IsExtendedLeft.Value = (playerInput.ExtendLeftInput.ServerValue && this.IsGrounded && this.IsSliding.Value);
			this.IsExtendedRight.Value = (playerInput.ExtendRightInput.ServerValue && this.IsGrounded && this.IsSliding.Value);
		}
		this.Movement.MoveForwards = (!this.IsSliding.Value && playerInput.MoveInput.ServerValue.y > 0f);
		this.Movement.MoveBackwards = (!this.IsSliding.Value && playerInput.MoveInput.ServerValue.y < 0f);
		this.Movement.TurnRight = (playerInput.MoveInput.ServerValue.x > 0f);
		this.Movement.TurnLeft = (playerInput.MoveInput.ServerValue.x < 0f);
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

	// Token: 0x060006D6 RID: 1750 RVA: 0x00027FB8 File Offset: 0x000261B8
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

	// Token: 0x060006D7 RID: 1751 RVA: 0x0002802C File Offset: 0x0002622C
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

	// Token: 0x060006D8 RID: 1752 RVA: 0x0000B793 File Offset: 0x00009993
	public void OnStandUp()
	{
		this.HasSlipped = false;
		this.HasFallen = false;
	}

	// Token: 0x060006D9 RID: 1753 RVA: 0x00028090 File Offset: 0x00026290
	public void UpdateMesh()
	{
		if (!this.Player)
		{
			return;
		}
		this.playerMesh.SetUsername(this.Player.Username.Value.ToString());
		this.playerMesh.SetNumber(this.Player.Number.Value.ToString());
		this.PlayerMesh.SetJersey(this.Player.Team.Value, this.Player.GetPlayerJerseySkin().ToString());
		this.PlayerMesh.SetRole(this.Player.Role.Value);
		this.PlayerMesh.PlayerHead.SetHelmetFlag(this.Player.Country.Value.ToString());
		this.PlayerMesh.PlayerHead.SetHelmetVisor(this.Player.GetPlayerVisorSkin().ToString());
		this.PlayerMesh.PlayerHead.SetMustache(this.Player.Mustache.Value.ToString());
		this.PlayerMesh.PlayerHead.SetBeard(this.Player.Beard.Value.ToString());
	}

	// Token: 0x060006DA RID: 1754 RVA: 0x000281FC File Offset: 0x000263FC
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
		if (this.Stamina < this.jumpStaminaDrain)
		{
			return;
		}
		this.Rigidbody.AddForce(Vector3.up * this.jumpVelocity, ForceMode.VelocityChange);
		this.Stamina = Mathf.Max(0f, this.Stamina - this.jumpStaminaDrain);
	}

	// Token: 0x060006DB RID: 1755 RVA: 0x00028268 File Offset: 0x00026468
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
		if (this.Stamina < this.twistStaminaDrain)
		{
			return;
		}
		this.Rigidbody.AddTorque(-base.transform.up * this.twistVelocity, ForceMode.VelocityChange);
		this.Stamina = Mathf.Max(0f, this.Stamina - this.twistStaminaDrain);
	}

	// Token: 0x060006DC RID: 1756 RVA: 0x000282E0 File Offset: 0x000264E0
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
		if (this.Stamina < this.twistStaminaDrain)
		{
			return;
		}
		this.Rigidbody.AddTorque(base.transform.up * this.twistVelocity, ForceMode.VelocityChange);
		this.Stamina = Mathf.Max(0f, this.Stamina - this.twistStaminaDrain);
	}

	// Token: 0x060006DD RID: 1757 RVA: 0x00028354 File Offset: 0x00026554
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
		if (this.Stamina < this.dashStaminaDrain)
		{
			return;
		}
		this.Rigidbody.AddForce(-base.transform.right * this.dashVelocity, ForceMode.VelocityChange);
		this.Stamina = Mathf.Max(0f, this.Stamina - this.dashStaminaDrain);
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

	// Token: 0x060006DE RID: 1758 RVA: 0x0002849C File Offset: 0x0002669C
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
		if (this.Stamina < this.dashStaminaDrain)
		{
			return;
		}
		this.Rigidbody.AddForce(base.transform.right * this.dashVelocity, ForceMode.VelocityChange);
		this.Stamina = Mathf.Max(0f, this.Stamina - this.dashStaminaDrain);
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

	// Token: 0x060006DF RID: 1759 RVA: 0x000285E0 File Offset: 0x000267E0
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

	// Token: 0x060006E0 RID: 1760 RVA: 0x00028644 File Offset: 0x00026844
	public void OnDebugTackleActionPerformed(InputAction.CallbackContext context)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (!this.Rigidbody)
		{
			return;
		}
		this.OnSlip();
		Vector3 a = Vector3.ClampMagnitude(base.transform.forward * 100f, this.tackleBounceMaximumMagnitude);
		this.Rigidbody.AddForceAtPosition(-a * this.tackleForceMultiplier, this.Rigidbody.worldCenterOfMass + base.transform.up * 0.5f, ForceMode.VelocityChange);
	}

	// Token: 0x060006E1 RID: 1761 RVA: 0x000286D8 File Offset: 0x000268D8
	private void OnStaminaChanged(short oldStamina, short newStamina)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnPlayerBodyStaminaChanged", new Dictionary<string, object>
		{
			{
				"playerBody",
				this
			},
			{
				"oldStamina",
				(float)oldStamina / 16383f
			},
			{
				"newStamina",
				(float)newStamina / 16383f
			}
		});
	}

	// Token: 0x060006E2 RID: 1762 RVA: 0x00028738 File Offset: 0x00026938
	private void OnSpeedChanged(short oldSpeed, short newSpeed)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnPlayerBodySpeedChanged", new Dictionary<string, object>
		{
			{
				"playerBody",
				this
			},
			{
				"oldSpeed",
				(float)oldSpeed / 327f
			},
			{
				"newSpeed",
				(float)newSpeed / 327f
			}
		});
	}

	// Token: 0x060006E3 RID: 1763 RVA: 0x00028798 File Offset: 0x00026998
	private void OnIsSprintingChanged(bool oldIsSprinting, bool newIsSprinting)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnPlayerBodyIsSprintingChanged", new Dictionary<string, object>
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

	// Token: 0x060006E4 RID: 1764 RVA: 0x000287E8 File Offset: 0x000269E8
	private void OnIsSlidingChanged(bool oldIsSliding, bool newIsSliding)
	{
		this.PlayerMesh.PlayerLegPadLeft.State = (newIsSliding ? (this.IsExtendedLeft.Value ? PlayerLegPadState.ButterflyExtended : PlayerLegPadState.Butterfly) : PlayerLegPadState.Idle);
		this.PlayerMesh.PlayerLegPadRight.State = (newIsSliding ? (this.IsExtendedRight.Value ? PlayerLegPadState.ButterflyExtended : PlayerLegPadState.Butterfly) : PlayerLegPadState.Idle);
		this.CancelDash();
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnPlayerBodyIsSlidingChanged", new Dictionary<string, object>
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

	// Token: 0x060006E5 RID: 1765 RVA: 0x0002888C File Offset: 0x00026A8C
	private void OnIsStoppingChanged(bool oldIsStopping, bool newIsStopping)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnPlayerBodyIsStoppingChanged", new Dictionary<string, object>
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

	// Token: 0x060006E6 RID: 1766 RVA: 0x000288DC File Offset: 0x00026ADC
	private void OnIsExtendedLeftChanged(bool oldIsExtendedLeft, bool newIsExtendedLeft)
	{
		this.PlayerMesh.PlayerLegPadLeft.State = (this.IsSliding.Value ? (newIsExtendedLeft ? PlayerLegPadState.ButterflyExtended : PlayerLegPadState.Butterfly) : PlayerLegPadState.Idle);
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnPlayerBodyIsExtendedLeftChanged", new Dictionary<string, object>
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

	// Token: 0x060006E7 RID: 1767 RVA: 0x00028954 File Offset: 0x00026B54
	private void OnIsExtendedRightChanged(bool oldIsExtendedRight, bool newIsExtendedRight)
	{
		this.PlayerMesh.PlayerLegPadRight.State = (this.IsSliding.Value ? (newIsExtendedRight ? PlayerLegPadState.ButterflyExtended : PlayerLegPadState.Butterfly) : PlayerLegPadState.Idle);
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnPlayerBodyIsExtendedRightChanged", new Dictionary<string, object>
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

	// Token: 0x060006E8 RID: 1768 RVA: 0x000289CC File Offset: 0x00026BCC
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

	// Token: 0x060006E9 RID: 1769 RVA: 0x0000B7A3 File Offset: 0x000099A3
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

	// Token: 0x060006EA RID: 1770 RVA: 0x0000B7D2 File Offset: 0x000099D2
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

	// Token: 0x060006EB RID: 1771 RVA: 0x00028A38 File Offset: 0x00026C38
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

	// Token: 0x060006EC RID: 1772 RVA: 0x00028B74 File Offset: 0x00026D74
	private void Server_OnDeferredCollision(GameObject gameObject, float force)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.gruntAudioSource.Server_Play(this.gruntVolumeCurve.Evaluate(force), this.gruntPitchCurve.Evaluate(force), true, -1, 0f, true, false, false, 0f, false, 0f, -1f);
	}

	// Token: 0x060006ED RID: 1773 RVA: 0x00028BCC File Offset: 0x00026DCC
	public void Client_InitializeNetworkVariables()
	{
		this.OnStaminaChanged(this.StaminaCompressed.Value, this.StaminaCompressed.Value);
		this.OnSpeedChanged(this.SpeedCompressed.Value, this.SpeedCompressed.Value);
		this.OnIsSprintingChanged(this.IsSprinting.Value, this.IsSprinting.Value);
		this.OnIsSlidingChanged(this.IsSliding.Value, this.IsSliding.Value);
		this.OnIsStoppingChanged(this.IsStopping.Value, this.IsStopping.Value);
	}

	// Token: 0x060006EE RID: 1774 RVA: 0x00028C68 File Offset: 0x00026E68
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
		PlayerBodyV2 component = collision.gameObject.GetComponent<PlayerBodyV2>();
		if (!component)
		{
			return;
		}
		float normalizedMaximumSpeed = this.Movement.NormalizedMaximumSpeed;
		float normalizedMaximumSpeed2 = component.Movement.NormalizedMaximumSpeed;
		float collisionForce = Utils.GetCollisionForce(collision);
		if (this.Speed < this.tackleSpeedThreshold)
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

	// Token: 0x060006FC RID: 1788 RVA: 0x00028F54 File Offset: 0x00027154
	protected override void __initializeVariables()
	{
		bool flag = this.PlayerReference == null;
		if (flag)
		{
			throw new Exception("PlayerBodyV2.PlayerReference cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.PlayerReference.Initialize(this);
		base.__nameNetworkVariable(this.PlayerReference, "PlayerReference");
		this.NetworkVariableFields.Add(this.PlayerReference);
		flag = (this.StaminaCompressed == null);
		if (flag)
		{
			throw new Exception("PlayerBodyV2.StaminaCompressed cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.StaminaCompressed.Initialize(this);
		base.__nameNetworkVariable(this.StaminaCompressed, "StaminaCompressed");
		this.NetworkVariableFields.Add(this.StaminaCompressed);
		flag = (this.SpeedCompressed == null);
		if (flag)
		{
			throw new Exception("PlayerBodyV2.SpeedCompressed cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.SpeedCompressed.Initialize(this);
		base.__nameNetworkVariable(this.SpeedCompressed, "SpeedCompressed");
		this.NetworkVariableFields.Add(this.SpeedCompressed);
		flag = (this.IsSprinting == null);
		if (flag)
		{
			throw new Exception("PlayerBodyV2.IsSprinting cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.IsSprinting.Initialize(this);
		base.__nameNetworkVariable(this.IsSprinting, "IsSprinting");
		this.NetworkVariableFields.Add(this.IsSprinting);
		flag = (this.IsSliding == null);
		if (flag)
		{
			throw new Exception("PlayerBodyV2.IsSliding cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.IsSliding.Initialize(this);
		base.__nameNetworkVariable(this.IsSliding, "IsSliding");
		this.NetworkVariableFields.Add(this.IsSliding);
		flag = (this.IsStopping == null);
		if (flag)
		{
			throw new Exception("PlayerBodyV2.IsStopping cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.IsStopping.Initialize(this);
		base.__nameNetworkVariable(this.IsStopping, "IsStopping");
		this.NetworkVariableFields.Add(this.IsStopping);
		flag = (this.IsExtendedLeft == null);
		if (flag)
		{
			throw new Exception("PlayerBodyV2.IsExtendedLeft cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.IsExtendedLeft.Initialize(this);
		base.__nameNetworkVariable(this.IsExtendedLeft, "IsExtendedLeft");
		this.NetworkVariableFields.Add(this.IsExtendedLeft);
		flag = (this.IsExtendedRight == null);
		if (flag)
		{
			throw new Exception("PlayerBodyV2.IsExtendedRight cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.IsExtendedRight.Initialize(this);
		base.__nameNetworkVariable(this.IsExtendedRight, "IsExtendedRight");
		this.NetworkVariableFields.Add(this.IsExtendedRight);
		base.__initializeVariables();
	}

	// Token: 0x060006FD RID: 1789 RVA: 0x00006C33 File Offset: 0x00004E33
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x060006FE RID: 1790 RVA: 0x0000B848 File Offset: 0x00009A48
	protected internal override string __getTypeName()
	{
		return "PlayerBodyV2";
	}

	// Token: 0x040003E5 RID: 997
	[Header("References")]
	[SerializeField]
	private Transform movementDirection;

	// Token: 0x040003E6 RID: 998
	[SerializeField]
	private PlayerMesh playerMesh;

	// Token: 0x040003E7 RID: 999
	[SerializeField]
	private SynchronizedAudio windAudioSource;

	// Token: 0x040003E8 RID: 1000
	[SerializeField]
	private SynchronizedAudio iceAudioSource;

	// Token: 0x040003E9 RID: 1001
	[SerializeField]
	private SynchronizedAudio gruntAudioSource;

	// Token: 0x040003EA RID: 1002
	[SerializeField]
	private AudioSource voiceAudioSource;

	// Token: 0x040003EB RID: 1003
	[Header("Settings")]
	[SerializeField]
	private float gravityMultiplier = 2f;

	// Token: 0x040003EC RID: 1004
	[SerializeField]
	private float hoverDistance = 1.2f;

	// Token: 0x040003ED RID: 1005
	[Space(20f)]
	[SerializeField]
	private float upwardnessThreshold = 0.8f;

	// Token: 0x040003EE RID: 1006
	[SerializeField]
	private float sidewaysThreshold = 0.2f;

	// Token: 0x040003EF RID: 1007
	[Space(20f)]
	[SerializeField]
	private float balanceLossTime = 0.25f;

	// Token: 0x040003F0 RID: 1008
	[SerializeField]
	private float balanceRecoveryTime = 5f;

	// Token: 0x040003F1 RID: 1009
	[Space(20f)]
	[SerializeField]
	private float staminaRegenerationRate = 10f;

	// Token: 0x040003F2 RID: 1010
	[Space(20f)]
	[SerializeField]
	private float sprintStaminaDrainRate = 1.4f;

	// Token: 0x040003F3 RID: 1011
	[Space(20f)]
	[SerializeField]
	private float slideTurnMultiplier = 2f;

	// Token: 0x040003F4 RID: 1012
	[SerializeField]
	private float slideHoverDistance = 0.8f;

	// Token: 0x040003F5 RID: 1013
	[Space(20f)]
	[SerializeField]
	private float jumpVelocity = 6f;

	// Token: 0x040003F6 RID: 1014
	[SerializeField]
	private float jumpStaminaDrain = 0.125f;

	// Token: 0x040003F7 RID: 1015
	[SerializeField]
	private float jumpTurnMultiplier = 5f;

	// Token: 0x040003F8 RID: 1016
	[Space(20f)]
	[SerializeField]
	private float twistVelocity = 5f;

	// Token: 0x040003F9 RID: 1017
	[SerializeField]
	private float twistStaminaDrain = 0.125f;

	// Token: 0x040003FA RID: 1018
	[Space(20f)]
	[SerializeField]
	private bool canDash = true;

	// Token: 0x040003FB RID: 1019
	[SerializeField]
	private float dashVelocity = 6f;

	// Token: 0x040003FC RID: 1020
	[SerializeField]
	private float dashStaminaDrain = 0.125f;

	// Token: 0x040003FD RID: 1021
	[SerializeField]
	private float dashDrag = 5f;

	// Token: 0x040003FE RID: 1022
	[SerializeField]
	private float dashDragTime = 1f;

	// Token: 0x040003FF RID: 1023
	[Space(20f)]
	[SerializeField]
	private float slideDrag = 0.2f;

	// Token: 0x04000400 RID: 1024
	[SerializeField]
	private float stopDrag = 2.5f;

	// Token: 0x04000401 RID: 1025
	[SerializeField]
	private float fallenDrag = 0.2f;

	// Token: 0x04000402 RID: 1026
	[Space(20f)]
	[SerializeField]
	private float tackleSpeedThreshold = 7.6f;

	// Token: 0x04000403 RID: 1027
	[SerializeField]
	private float tackleForceThreshold = 7f;

	// Token: 0x04000404 RID: 1028
	[SerializeField]
	private float tackleForceMultiplier = 0.3f;

	// Token: 0x04000405 RID: 1029
	[SerializeField]
	private float tackleBounceMaximumMagnitude = 10f;

	// Token: 0x04000406 RID: 1030
	[Space(20f)]
	[SerializeField]
	private float stretchSpeed = 10f;

	// Token: 0x04000407 RID: 1031
	[Space(20f)]
	[SerializeField]
	private float maximumLaterality = 1f;

	// Token: 0x04000408 RID: 1032
	[SerializeField]
	private float minimumLaterality = 0.5f;

	// Token: 0x04000409 RID: 1033
	[SerializeField]
	private float minimumLateralitySpeed = 2f;

	// Token: 0x0400040A RID: 1034
	[SerializeField]
	private float maximumLateralitySpeed = 5f;

	// Token: 0x0400040B RID: 1035
	[Space(20f)]
	[SerializeField]
	private AnimationCurve windVolumeCurve;

	// Token: 0x0400040C RID: 1036
	[SerializeField]
	private AnimationCurve iceVolumeCurve;

	// Token: 0x0400040D RID: 1037
	[SerializeField]
	private AnimationCurve icePitchCurve;

	// Token: 0x0400040E RID: 1038
	[SerializeField]
	private AnimationCurve gruntVolumeCurve;

	// Token: 0x0400040F RID: 1039
	[SerializeField]
	private AnimationCurve gruntPitchCurve;

	// Token: 0x04000410 RID: 1040
	[HideInInspector]
	public NetworkVariable<NetworkObjectReference> PlayerReference = new NetworkVariable<NetworkObjectReference>(default(NetworkObjectReference), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x04000411 RID: 1041
	[HideInInspector]
	public NetworkVariable<short> StaminaCompressed = new NetworkVariable<short>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x04000412 RID: 1042
	[HideInInspector]
	public NetworkVariable<short> SpeedCompressed = new NetworkVariable<short>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x04000413 RID: 1043
	[HideInInspector]
	public NetworkVariable<bool> IsSprinting = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x04000414 RID: 1044
	[HideInInspector]
	public NetworkVariable<bool> IsSliding = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x04000415 RID: 1045
	[HideInInspector]
	public NetworkVariable<bool> IsStopping = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x04000416 RID: 1046
	[HideInInspector]
	public NetworkVariable<bool> IsExtendedLeft = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x04000417 RID: 1047
	[HideInInspector]
	public NetworkVariable<bool> IsExtendedRight = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x04000418 RID: 1048
	[HideInInspector]
	public Player Player;

	// Token: 0x04000419 RID: 1049
	[HideInInspector]
	public Rigidbody Rigidbody;

	// Token: 0x0400041A RID: 1050
	[HideInInspector]
	public Movement Movement;

	// Token: 0x0400041B RID: 1051
	[HideInInspector]
	public VelocityLean VelocityLean;

	// Token: 0x0400041C RID: 1052
	[HideInInspector]
	public Hover Hover;

	// Token: 0x0400041D RID: 1053
	[HideInInspector]
	public Skate Skate;

	// Token: 0x0400041E RID: 1054
	[HideInInspector]
	public KeepUpright KeepUpright;

	// Token: 0x0400041F RID: 1055
	[HideInInspector]
	public MeshRendererHider MeshRendererHider;

	// Token: 0x04000420 RID: 1056
	[HideInInspector]
	public CollisionRecorder CollisionRecorder;

	// Token: 0x04000421 RID: 1057
	[HideInInspector]
	public bool HasDashed;

	// Token: 0x04000422 RID: 1058
	[HideInInspector]
	public bool HasDashExtended;

	// Token: 0x04000423 RID: 1059
	[HideInInspector]
	public bool HasSlipped;

	// Token: 0x04000424 RID: 1060
	[HideInInspector]
	public bool HasFallen;

	// Token: 0x04000425 RID: 1061
	[HideInInspector]
	public float Laterality;

	// Token: 0x04000426 RID: 1062
	private Tween balanceLossTween;

	// Token: 0x04000427 RID: 1063
	private Tween balanceRecoveryTween;

	// Token: 0x04000428 RID: 1064
	private Tween dashDragTween;

	// Token: 0x04000429 RID: 1065
	private Tween dashLegPadTween;
}
