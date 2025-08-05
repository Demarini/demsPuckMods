using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

// Token: 0x020000E5 RID: 229
public class PlayerInput : NetworkBehaviour
{
	// Token: 0x170000AD RID: 173
	// (get) Token: 0x06000732 RID: 1842 RVA: 0x0000B99D File Offset: 0x00009B9D
	[HideInInspector]
	public Vector2 MinimumStickRaycastOriginAngle
	{
		get
		{
			return this.minimumStickRaycastOriginAngle;
		}
	}

	// Token: 0x170000AE RID: 174
	// (get) Token: 0x06000733 RID: 1843 RVA: 0x0000B9A5 File Offset: 0x00009BA5
	[HideInInspector]
	public Vector2 MaximumStickRaycastOriginAngle
	{
		get
		{
			return this.maximumStickRaycastOriginAngle;
		}
	}

	// Token: 0x170000AF RID: 175
	// (get) Token: 0x06000734 RID: 1844 RVA: 0x0000B9AD File Offset: 0x00009BAD
	[HideInInspector]
	public Vector2 MinimumLookAngle
	{
		get
		{
			return this.minimumLookAngle;
		}
	}

	// Token: 0x170000B0 RID: 176
	// (get) Token: 0x06000735 RID: 1845 RVA: 0x0000B9B5 File Offset: 0x00009BB5
	[HideInInspector]
	public Vector2 MaximumLookAngle
	{
		get
		{
			return this.maximumLookAngle;
		}
	}

	// Token: 0x170000B1 RID: 177
	// (get) Token: 0x06000736 RID: 1846 RVA: 0x0000B9BD File Offset: 0x00009BBD
	[HideInInspector]
	public int MinimumBladeAngle
	{
		get
		{
			return this.minimumBladeAngle;
		}
	}

	// Token: 0x170000B2 RID: 178
	// (get) Token: 0x06000737 RID: 1847 RVA: 0x0000B9C5 File Offset: 0x00009BC5
	[HideInInspector]
	public int MaximumBladeAngle
	{
		get
		{
			return this.maximumBladeAngle;
		}
	}

	// Token: 0x170000B3 RID: 179
	// (get) Token: 0x06000738 RID: 1848 RVA: 0x0000B9CD File Offset: 0x00009BCD
	// (set) Token: 0x06000739 RID: 1849 RVA: 0x0000B9DA File Offset: 0x00009BDA
	[HideInInspector]
	public float InitialLookAngle
	{
		get
		{
			return this.initialLookAngle.x;
		}
		set
		{
			this.initialLookAngle = new Vector3(value, 0f, 0f);
		}
	}

	// Token: 0x0600073A RID: 1850 RVA: 0x0000B9F2 File Offset: 0x00009BF2
	private void Awake()
	{
		this.Player = base.GetComponent<Player>();
	}

	// Token: 0x0600073B RID: 1851 RVA: 0x0000BA00 File Offset: 0x00009C00
	private void Start()
	{
		InputSystem.onActionChange += this.OnInputActionChangeCallback;
	}

	// Token: 0x0600073C RID: 1852 RVA: 0x00029CC0 File Offset: 0x00027EC0
	public override void OnNetworkSpawn()
	{
		if (this.Player.IsReplay.Value)
		{
			this.shouldTickInputs = true;
			return;
		}
		if (!base.IsOwner)
		{
			return;
		}
		MonoBehaviourSingleton<InputManager>.Instance.DebugInputsAction.performed += this.OnDebugInputsActionPerformed;
		MonoBehaviourSingleton<InputManager>.Instance.DebugShootAction.performed += this.OnDebugShootActionPerformed;
		MonoBehaviourSingleton<InputManager>.Instance.BladeAngleUpAction.performed += this.OnBladeAngleUpActionPerformed;
		MonoBehaviourSingleton<InputManager>.Instance.BladeAngleDownAction.performed += this.OnBladeAngleDownActionPerformed;
		MonoBehaviourSingleton<InputManager>.Instance.JumpAction.performed += this.OnJumpActionPerformed;
		MonoBehaviourSingleton<InputManager>.Instance.TwistLeftAction.performed += this.OnTwistLeftActionPerformed;
		MonoBehaviourSingleton<InputManager>.Instance.TwistRightAction.performed += this.OnTwistRightActionPerformed;
		MonoBehaviourSingleton<InputManager>.Instance.DashLeftAction.performed += this.OnDashLeftActionPerformed;
		MonoBehaviourSingleton<InputManager>.Instance.DashRightAction.performed += this.OnDashRightActionPerformed;
		this.shouldUpdateInputs = true;
		this.shouldTickInputs = true;
		base.OnNetworkSpawn();
	}

	// Token: 0x0600073D RID: 1853 RVA: 0x00029DF8 File Offset: 0x00027FF8
	public override void OnNetworkDespawn()
	{
		if (!base.IsOwner)
		{
			return;
		}
		this.shouldUpdateInputs = false;
		this.shouldTickInputs = false;
		MonoBehaviourSingleton<InputManager>.Instance.DebugInputsAction.performed -= this.OnDebugInputsActionPerformed;
		MonoBehaviourSingleton<InputManager>.Instance.DebugShootAction.performed -= this.OnDebugShootActionPerformed;
		MonoBehaviourSingleton<InputManager>.Instance.BladeAngleUpAction.performed -= this.OnBladeAngleUpActionPerformed;
		MonoBehaviourSingleton<InputManager>.Instance.BladeAngleDownAction.performed -= this.OnBladeAngleDownActionPerformed;
		MonoBehaviourSingleton<InputManager>.Instance.JumpAction.performed -= this.OnJumpActionPerformed;
		MonoBehaviourSingleton<InputManager>.Instance.TwistLeftAction.performed -= this.OnTwistLeftActionPerformed;
		MonoBehaviourSingleton<InputManager>.Instance.TwistRightAction.performed -= this.OnTwistRightActionPerformed;
		MonoBehaviourSingleton<InputManager>.Instance.DashLeftAction.performed -= this.OnDashLeftActionPerformed;
		MonoBehaviourSingleton<InputManager>.Instance.DashRightAction.performed -= this.OnDashRightActionPerformed;
		base.OnNetworkDespawn();
	}

	// Token: 0x0600073E RID: 1854 RVA: 0x0000BA13 File Offset: 0x00009C13
	public override void OnDestroy()
	{
		InputSystem.onActionChange -= this.OnInputActionChangeCallback;
		Tween tween = this.sleepingTween;
		if (tween != null)
		{
			tween.Kill(false);
		}
		base.OnDestroy();
	}

	// Token: 0x0600073F RID: 1855 RVA: 0x00029F18 File Offset: 0x00028118
	private void Update()
	{
		if (this.shouldUpdateInputs)
		{
			this.UpdateInputs();
		}
		if (this.shouldTickInputs)
		{
			this.tickAccumulator += Time.deltaTime * (float)this.TickRate;
			if (this.tickAccumulator >= 1f)
			{
				while (this.tickAccumulator >= 1f)
				{
					this.tickAccumulator -= 1f;
				}
				this.ClientTick();
			}
		}
	}

	// Token: 0x06000740 RID: 1856 RVA: 0x00029F8C File Offset: 0x0002818C
	private void UpdateInputs()
	{
		if (NetworkBehaviourSingleton<UIManager>.Instance.isMouseActive)
		{
			return;
		}
		if (this.debugInputs)
		{
			if (Time.time - this.lastDebugInputUpdateTime > 1f)
			{
				this.MoveInput.ClientValue = new Vector2((float)((UnityEngine.Random.Range(0f, 1f) > 0.5f) ? 1 : -1), (float)((UnityEngine.Random.Range(0f, 1f) > 0.5f) ? 1 : -1));
				this.SlideInput.ClientValue = (UnityEngine.Random.Range(0f, 1f) > 0.5f);
				this.SprintInput.ClientValue = (UnityEngine.Random.Range(0f, 1f) > 0.5f);
				this.TrackInput.ClientValue = (UnityEngine.Random.Range(0f, 1f) > 0.5f);
				if (!this.TrackInput.ClientValue)
				{
					this.LookInput.ClientValue = (UnityEngine.Random.Range(0f, 1f) > 0.5f);
				}
				this.ExtendLeftInput.ClientValue = (UnityEngine.Random.Range(0f, 1f) > 0.5f);
				this.ExtendRightInput.ClientValue = (UnityEngine.Random.Range(0f, 1f) > 0.5f);
				this.LateralLeftInput.ClientValue = (UnityEngine.Random.Range(0f, 1f) > 0.5f);
				this.LateralRightInput.ClientValue = (UnityEngine.Random.Range(0f, 1f) > 0.5f);
				this.StopInput.ClientValue = (UnityEngine.Random.Range(0f, 1f) > 0.5f);
				if (this.LookInput.ClientValue)
				{
					this.LookAngleInput.ClientValue = new Vector2(UnityEngine.Random.Range(this.minimumLookAngle.x, this.maximumLookAngle.x), UnityEngine.Random.Range(this.minimumLookAngle.y, this.maximumLookAngle.y));
				}
				else
				{
					this.StickRaycastOriginAngleInput.ClientValue = new Vector2(UnityEngine.Random.Range(this.minimumStickRaycastOriginAngle.x, this.maximumStickRaycastOriginAngle.x), UnityEngine.Random.Range(this.minimumStickRaycastOriginAngle.y, this.maximumStickRaycastOriginAngle.y));
				}
				this.lastDebugInputUpdateTime = Time.time;
			}
			return;
		}
		this.MoveInput.ClientValue = new Vector2((float)((MonoBehaviourSingleton<InputManager>.Instance.TurnRightAction.IsInProgress() ? 1 : 0) + (MonoBehaviourSingleton<InputManager>.Instance.TurnLeftAction.IsInProgress() ? -1 : 0)), (float)((MonoBehaviourSingleton<InputManager>.Instance.MoveForwardAction.IsInProgress() ? 1 : 0) + (MonoBehaviourSingleton<InputManager>.Instance.MoveBackwardAction.IsInProgress() ? -1 : 0)));
		if (!this.LookInput.ClientValue)
		{
			Vector2 vector = MonoBehaviourSingleton<InputManager>.Instance.StickAction.ReadValue<Vector2>();
			Vector2 b = new Vector2(-vector.y * (MonoBehaviourSingleton<SettingsManager>.Instance.GlobalStickSensitivity / 2f) * MonoBehaviourSingleton<SettingsManager>.Instance.VerticalStickSensitivity, vector.x * (MonoBehaviourSingleton<SettingsManager>.Instance.GlobalStickSensitivity / 2f) * MonoBehaviourSingleton<SettingsManager>.Instance.HorizontalStickSensitivity);
			if (this.debugShootTween == null || !this.debugShootTween.active)
			{
				this.StickRaycastOriginAngleInput.ClientValue = Utils.Vector2Clamp(this.StickRaycastOriginAngleInput.ClientValue + b, this.minimumStickRaycastOriginAngle, this.maximumStickRaycastOriginAngle);
			}
		}
		this.SlideInput.ClientValue = MonoBehaviourSingleton<InputManager>.Instance.SlideAction.IsInProgress();
		this.SprintInput.ClientValue = MonoBehaviourSingleton<InputManager>.Instance.SprintAction.IsInProgress();
		this.TrackInput.ClientValue = MonoBehaviourSingleton<InputManager>.Instance.TrackAction.IsInProgress();
		this.LookInput.ClientValue = MonoBehaviourSingleton<InputManager>.Instance.LookAction.IsInProgress();
		this.ExtendLeftInput.ClientValue = MonoBehaviourSingleton<InputManager>.Instance.ExtendLeftAction.IsInProgress();
		this.ExtendRightInput.ClientValue = MonoBehaviourSingleton<InputManager>.Instance.ExtendRightAction.IsInProgress();
		this.TalkInput.ClientValue = MonoBehaviourSingleton<InputManager>.Instance.TalkAction.IsInProgress();
		this.StopInput.ClientValue = MonoBehaviourSingleton<InputManager>.Instance.StopAction.IsInProgress();
	}

	// Token: 0x06000741 RID: 1857 RVA: 0x0002A3D8 File Offset: 0x000285D8
	public void UpdateLookAngle(float deltaTime)
	{
		if (this.TrackInput.ClientValue && !this.LookInput.ClientValue)
		{
			Puck puck = NetworkBehaviourSingleton<PuckManager>.Instance.GetPlayerPuck(base.OwnerClientId);
			if (!puck)
			{
				puck = NetworkBehaviourSingleton<PuckManager>.Instance.GetPuck(false);
			}
			PlayerCamera playerCamera = this.Player.PlayerCamera;
			PlayerBodyV2 playerBody = this.Player.PlayerBody;
			if (puck && playerCamera && playerBody)
			{
				Quaternion rhs = Quaternion.LookRotation(puck.transform.position - playerCamera.transform.position);
				Vector3 vector = Utils.WrapEulerAngles((Quaternion.Inverse(playerBody.transform.rotation) * rhs).eulerAngles);
				vector = Utils.Vector2Clamp(vector, this.minimumLookAngle, this.maximumLookAngle);
				this.LookAngleInput.ClientValue = Vector3.LerpUnclamped(this.LookAngleInput.ClientValue, vector, deltaTime * 10f);
			}
		}
		if (this.LookInput.ClientValue)
		{
			Vector2 vector2 = MonoBehaviourSingleton<InputManager>.Instance.StickAction.ReadValue<Vector2>();
			Vector2 b = new Vector2(-vector2.y * (MonoBehaviourSingleton<SettingsManager>.Instance.LookSensitivity / 2f), vector2.x * (MonoBehaviourSingleton<SettingsManager>.Instance.LookSensitivity / 2f));
			this.LookAngleInput.ClientValue = Utils.Vector2Clamp(this.LookAngleInput.ClientValue + b, this.minimumLookAngle, this.maximumLookAngle);
			return;
		}
		if (!this.TrackInput.ClientValue)
		{
			this.LookAngleInput.ClientValue = Vector3.Lerp(this.LookAngleInput.ClientValue, this.initialLookAngle, deltaTime * 10f);
		}
	}

	// Token: 0x06000742 RID: 1858 RVA: 0x0002A5BC File Offset: 0x000287BC
	public void ResetInputs(bool invertStickRaycastOriginAngle = false)
	{
		this.MoveInput.ClientValue = Vector2.zero;
		this.LookAngleInput.ClientValue = this.initialLookAngle;
		this.StickRaycastOriginAngleInput.ClientValue = this.initialStickRaycastOriginAngle;
		if (invertStickRaycastOriginAngle)
		{
			this.StickRaycastOriginAngleInput.ClientValue.y = -this.StickRaycastOriginAngleInput.ClientValue.y;
		}
		this.BladeAngleInput.ClientValue = 0;
		this.SlideInput.ClientValue = false;
		this.SprintInput.ClientValue = false;
		this.TrackInput.ClientValue = false;
		this.LookInput.ClientValue = false;
		this.ExtendLeftInput.ClientValue = false;
		this.ExtendRightInput.ClientValue = false;
		this.LateralLeftInput.ClientValue = false;
		this.LateralRightInput.ClientValue = false;
		this.StopInput.ClientValue = false;
		this.bladeAngleBuffer = (float)this.BladeAngleInput.ClientValue;
		MonoBehaviourSingleton<InputManager>.Instance.Reset();
	}

	// Token: 0x06000743 RID: 1859 RVA: 0x0002A6B8 File Offset: 0x000288B8
	private void OnDebugShootActionPerformed(InputAction.CallbackContext context)
	{
		if (NetworkBehaviourSingleton<UIManager>.Instance.isMouseActive)
		{
			return;
		}
		if (!this.Player)
		{
			return;
		}
		this.StickRaycastOriginAngleInput.ClientValue = this.debugShootStartAngle;
		Tween tween = this.debugShootTween;
		if (tween != null)
		{
			tween.Kill(false);
		}
		this.debugShootTween = DOTween.Sequence().AppendInterval(0.2f).Append(DOTween.To(() => this.StickRaycastOriginAngleInput.ClientValue, delegate(Vector2 x)
		{
			this.StickRaycastOriginAngleInput.ClientValue = x;
		}, this.debugShootEndAngle, 0.2f).SetEase(Ease.Linear)).Append(DOTween.To(() => this.StickRaycastOriginAngleInput.ClientValue, delegate(Vector2 x)
		{
			this.StickRaycastOriginAngleInput.ClientValue = x;
		}, this.debugShootStartAngle, 0.2f).SetEase(Ease.Linear));
	}

	// Token: 0x06000744 RID: 1860 RVA: 0x0000BA3E File Offset: 0x00009C3E
	private void OnDebugInputsActionPerformed(InputAction.CallbackContext context)
	{
		if (NetworkBehaviourSingleton<UIManager>.Instance.isMouseActive)
		{
			return;
		}
		if (!this.Player)
		{
			return;
		}
		this.debugInputs = !this.debugInputs;
	}

	// Token: 0x06000745 RID: 1861 RVA: 0x0002A780 File Offset: 0x00028980
	private void OnBladeAngleUpActionPerformed(InputAction.CallbackContext context)
	{
		if (NetworkBehaviourSingleton<UIManager>.Instance.isMouseActive)
		{
			return;
		}
		if (!this.Player.Stick)
		{
			return;
		}
		this.bladeAngleBuffer += context.ReadValue<float>();
		this.bladeAngleBuffer = Mathf.Clamp(this.bladeAngleBuffer, (float)this.minimumBladeAngle, (float)this.maximumBladeAngle);
		this.BladeAngleInput.ClientValue = (sbyte)this.bladeAngleBuffer;
	}

	// Token: 0x06000746 RID: 1862 RVA: 0x0002A7F4 File Offset: 0x000289F4
	private void OnBladeAngleDownActionPerformed(InputAction.CallbackContext context)
	{
		if (NetworkBehaviourSingleton<UIManager>.Instance.isMouseActive)
		{
			return;
		}
		if (!this.Player.Stick)
		{
			return;
		}
		this.bladeAngleBuffer -= context.ReadValue<float>();
		this.bladeAngleBuffer = Mathf.Clamp(this.bladeAngleBuffer, (float)this.minimumBladeAngle, (float)this.maximumBladeAngle);
		this.BladeAngleInput.ClientValue = (sbyte)this.bladeAngleBuffer;
	}

	// Token: 0x06000747 RID: 1863 RVA: 0x0000BA6A File Offset: 0x00009C6A
	private void OnJumpActionPerformed(InputAction.CallbackContext context)
	{
		if (NetworkBehaviourSingleton<UIManager>.Instance.isMouseActive)
		{
			return;
		}
		NetworkedInput<byte> jumpInput = this.JumpInput;
		jumpInput.ClientValue += 1;
	}

	// Token: 0x06000748 RID: 1864 RVA: 0x0000BA8D File Offset: 0x00009C8D
	private void OnTwistLeftActionPerformed(InputAction.CallbackContext context)
	{
		if (NetworkBehaviourSingleton<UIManager>.Instance.isMouseActive)
		{
			return;
		}
		NetworkedInput<byte> twistLeftInput = this.TwistLeftInput;
		twistLeftInput.ClientValue += 1;
	}

	// Token: 0x06000749 RID: 1865 RVA: 0x0000BAB0 File Offset: 0x00009CB0
	private void OnTwistRightActionPerformed(InputAction.CallbackContext context)
	{
		if (NetworkBehaviourSingleton<UIManager>.Instance.isMouseActive)
		{
			return;
		}
		NetworkedInput<byte> twistRightInput = this.TwistRightInput;
		twistRightInput.ClientValue += 1;
	}

	// Token: 0x0600074A RID: 1866 RVA: 0x0000BAD3 File Offset: 0x00009CD3
	private void OnDashLeftActionPerformed(InputAction.CallbackContext context)
	{
		if (NetworkBehaviourSingleton<UIManager>.Instance.isMouseActive)
		{
			return;
		}
		NetworkedInput<byte> dashLeftInput = this.DashLeftInput;
		dashLeftInput.ClientValue += 1;
	}

	// Token: 0x0600074B RID: 1867 RVA: 0x0000BAF6 File Offset: 0x00009CF6
	private void OnDashRightActionPerformed(InputAction.CallbackContext context)
	{
		if (NetworkBehaviourSingleton<UIManager>.Instance.isMouseActive)
		{
			return;
		}
		NetworkedInput<byte> dashRightInput = this.DashRightInput;
		dashRightInput.ClientValue += 1;
	}

	// Token: 0x0600074C RID: 1868 RVA: 0x0002A868 File Offset: 0x00028A68
	private void ClientTick()
	{
		if (this.MoveInput.HasChanged)
		{
			short x = (short)(this.MoveInput.ClientValue.x * 32767f);
			short y = (short)(this.MoveInput.ClientValue.y * 32767f);
			this.Client_MoveInputRpc(x, y);
			this.MoveInput.ClientTick();
		}
		if (this.StickRaycastOriginAngleInput.HasChanged)
		{
			short x2 = (short)(this.StickRaycastOriginAngleInput.ClientValue.x / 360f * 32767f);
			short y2 = (short)(this.StickRaycastOriginAngleInput.ClientValue.y / 360f * 32767f);
			this.Client_RaycastOriginAngleInputRpc(x2, y2);
			this.StickRaycastOriginAngleInput.ClientTick();
		}
		if (this.LookAngleInput.HasChanged)
		{
			short x3 = (short)(this.LookAngleInput.ClientValue.x / 360f * 32767f);
			short y3 = (short)(this.LookAngleInput.ClientValue.y / 360f * 32767f);
			this.Client_LookAngleInputRpc(x3, y3);
			this.LookAngleInput.ClientTick();
		}
		if (this.BladeAngleInput.HasChanged)
		{
			this.Client_BladeAngleInputRpc(this.BladeAngleInput.ClientValue);
			this.BladeAngleInput.ClientTick();
		}
		if (this.SlideInput.HasChanged)
		{
			this.Client_SlideInputRpc(this.SlideInput.ClientValue);
			this.SlideInput.ClientTick();
		}
		if (this.SprintInput.HasChanged)
		{
			this.Client_SprintInputRpc(this.SprintInput.ClientValue);
			this.SprintInput.ClientTick();
		}
		if (this.TrackInput.HasChanged)
		{
			this.Client_TrackInputRpc(this.TrackInput.ClientValue);
			this.TrackInput.ClientTick();
		}
		if (this.LookInput.HasChanged)
		{
			this.Client_LookInputRpc(this.LookInput.ClientValue);
			this.LookInput.ClientTick();
		}
		if (this.JumpInput.HasChanged)
		{
			this.Client_JumpInputRpc();
			this.JumpInput.ClientTick();
		}
		if (this.StopInput.HasChanged)
		{
			this.Client_StopInputRpc(this.StopInput.ClientValue);
			this.StopInput.ClientTick();
		}
		if (this.TwistLeftInput.HasChanged)
		{
			this.Client_TwistLeftInputRpc();
			this.TwistLeftInput.ClientTick();
		}
		if (this.TwistRightInput.HasChanged)
		{
			this.Client_TwistRightInputRpc();
			this.TwistRightInput.ClientTick();
		}
		if (this.DashLeftInput.HasChanged)
		{
			this.Client_DashLeftInputRpc();
			this.DashLeftInput.ClientTick();
		}
		if (this.DashRightInput.HasChanged)
		{
			this.Client_DashRightInputRpc();
			this.DashRightInput.ClientTick();
		}
		if (this.ExtendLeftInput.HasChanged)
		{
			this.Client_ExtendLeftInputRpc(this.ExtendLeftInput.ClientValue);
			this.ExtendLeftInput.ClientTick();
		}
		if (this.ExtendRightInput.HasChanged)
		{
			this.Client_ExtendRightInputRpc(this.ExtendRightInput.ClientValue);
			this.ExtendRightInput.ClientTick();
		}
		if (this.LateralLeftInput.HasChanged)
		{
			this.Client_LateralLeftInputRpc(this.LateralLeftInput.ClientValue);
			this.LateralLeftInput.ClientTick();
		}
		if (this.LateralRightInput.HasChanged)
		{
			this.Client_LateralRightInputRpc(this.LateralRightInput.ClientValue);
			this.LateralRightInput.ClientTick();
		}
		if (this.TalkInput.HasChanged)
		{
			this.Client_TalkInputRpc(this.TalkInput.ClientValue);
			this.TalkInput.ClientTick();
		}
		if (this.SleepInput.HasChanged)
		{
			this.Client_SleepInputRpc(this.SleepInput.ClientValue);
			this.SleepInput.ClientTick();
		}
	}

	// Token: 0x0600074D RID: 1869 RVA: 0x0002AC04 File Offset: 0x00028E04
	[Rpc(SendTo.Server)]
	public void Client_MoveInputRpc(short x, short y)
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 354985997U;
			RpcParams rpcParams2;
			RpcParams rpcParams = rpcParams2;
			RpcAttribute.RpcAttributeParams attributeParams = default(RpcAttribute.RpcAttributeParams);
			FastBufferWriter writer = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
			BytePacker.WriteValueBitPacked(writer, x);
			BytePacker.WriteValueBitPacked(writer, y);
			base.__endSendRpc(ref writer, 354985997U, rpcParams2, attributeParams, SendTo.Server, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		Vector2 value = new Vector2((float)x / 32767f, (float)y / 32767f);
		this.MoveInput.ServerValue = Utils.Vector2Clamp(value, -Vector2.one, Vector2.one);
	}

	// Token: 0x0600074E RID: 1870 RVA: 0x0002AD14 File Offset: 0x00028F14
	[Rpc(SendTo.SpecifiedInParams, Delivery = RpcDelivery.Unreliable)]
	public void Server_MoveInputRpc(Vector2 value, RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 2333051307U;
			RpcAttribute.RpcAttributeParams attributeParams = new RpcAttribute.RpcAttributeParams
			{
				Delivery = RpcDelivery.Unreliable
			};
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.SpecifiedInParams, RpcDelivery.Unreliable);
			fastBufferWriter.WriteValueSafe(value);
			base.__endSendRpc(ref fastBufferWriter, 2333051307U, rpcParams, attributeParams, SendTo.SpecifiedInParams, RpcDelivery.Unreliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		this.MoveInput.ServerValue = value;
	}

	// Token: 0x0600074F RID: 1871 RVA: 0x0002ADF4 File Offset: 0x00028FF4
	[Rpc(SendTo.Server, Delivery = RpcDelivery.Unreliable)]
	public void Client_RaycastOriginAngleInputRpc(short x, short y)
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 3072819325U;
			RpcParams rpcParams2;
			RpcParams rpcParams = rpcParams2;
			RpcAttribute.RpcAttributeParams attributeParams = new RpcAttribute.RpcAttributeParams
			{
				Delivery = RpcDelivery.Unreliable
			};
			FastBufferWriter writer = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Unreliable);
			BytePacker.WriteValueBitPacked(writer, x);
			BytePacker.WriteValueBitPacked(writer, y);
			base.__endSendRpc(ref writer, 3072819325U, rpcParams2, attributeParams, SendTo.Server, RpcDelivery.Unreliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		Vector2 value = new Vector2((float)(x * 360) / 32767f, (float)(y * 360) / 32767f);
		if (!this.Player)
		{
			return;
		}
		this.StickRaycastOriginAngleInput.ServerValue = Utils.Vector2Clamp(value, this.minimumStickRaycastOriginAngle, this.maximumStickRaycastOriginAngle);
	}

	// Token: 0x06000750 RID: 1872 RVA: 0x0002AF28 File Offset: 0x00029128
	[Rpc(SendTo.SpecifiedInParams, Delivery = RpcDelivery.Unreliable)]
	public void Server_RaycastOriginAngleInputRpc(Vector2 value, RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 3003669798U;
			RpcAttribute.RpcAttributeParams attributeParams = new RpcAttribute.RpcAttributeParams
			{
				Delivery = RpcDelivery.Unreliable
			};
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.SpecifiedInParams, RpcDelivery.Unreliable);
			fastBufferWriter.WriteValueSafe(value);
			base.__endSendRpc(ref fastBufferWriter, 3003669798U, rpcParams, attributeParams, SendTo.SpecifiedInParams, RpcDelivery.Unreliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		this.StickRaycastOriginAngleInput.ServerValue = value;
	}

	// Token: 0x06000751 RID: 1873 RVA: 0x0002B008 File Offset: 0x00029208
	[Rpc(SendTo.Server, Delivery = RpcDelivery.Unreliable)]
	public void Client_LookAngleInputRpc(short x, short y)
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 3839358977U;
			RpcParams rpcParams2;
			RpcParams rpcParams = rpcParams2;
			RpcAttribute.RpcAttributeParams attributeParams = new RpcAttribute.RpcAttributeParams
			{
				Delivery = RpcDelivery.Unreliable
			};
			FastBufferWriter writer = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Unreliable);
			BytePacker.WriteValueBitPacked(writer, x);
			BytePacker.WriteValueBitPacked(writer, y);
			base.__endSendRpc(ref writer, 3839358977U, rpcParams2, attributeParams, SendTo.Server, RpcDelivery.Unreliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		Vector2 value = new Vector2((float)(x * 360) / 32767f, (float)(y * 360) / 32767f);
		if (!this.Player)
		{
			return;
		}
		this.LookAngleInput.ServerValue = Utils.Vector2Clamp(value, this.minimumLookAngle, this.maximumLookAngle);
		this.Server_LookAngleInputRpc(x, y, base.RpcTarget.NotServer);
	}

	// Token: 0x06000752 RID: 1874 RVA: 0x0002B154 File Offset: 0x00029354
	[Rpc(SendTo.SpecifiedInParams, Delivery = RpcDelivery.Unreliable)]
	public void Server_LookAngleInputRpc(short x, short y, RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 1047632353U;
			RpcAttribute.RpcAttributeParams attributeParams = new RpcAttribute.RpcAttributeParams
			{
				Delivery = RpcDelivery.Unreliable
			};
			FastBufferWriter writer = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.SpecifiedInParams, RpcDelivery.Unreliable);
			BytePacker.WriteValueBitPacked(writer, x);
			BytePacker.WriteValueBitPacked(writer, y);
			base.__endSendRpc(ref writer, 1047632353U, rpcParams, attributeParams, SendTo.SpecifiedInParams, RpcDelivery.Unreliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		Vector2 serverValue = new Vector2((float)(x * 360) / 32767f, (float)(y * 360) / 32767f);
		this.LookAngleInput.ServerValue = serverValue;
	}

	// Token: 0x06000753 RID: 1875 RVA: 0x0002B264 File Offset: 0x00029464
	[Rpc(SendTo.Server)]
	public void Client_BladeAngleInputRpc(sbyte value)
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 2671629003U;
			RpcParams rpcParams2;
			RpcParams rpcParams = rpcParams2;
			RpcAttribute.RpcAttributeParams attributeParams = default(RpcAttribute.RpcAttributeParams);
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
			fastBufferWriter.WriteValueSafe<sbyte>(value, default(FastBufferWriter.ForPrimitives));
			base.__endSendRpc(ref fastBufferWriter, 2671629003U, rpcParams2, attributeParams, SendTo.Server, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		if (!this.Player)
		{
			return;
		}
		if (!this.Player.Stick)
		{
			return;
		}
		this.BladeAngleInput.ServerValue = (sbyte)Mathf.Clamp((int)value, this.minimumBladeAngle, this.maximumBladeAngle);
		this.Server_BladeAngleInputRpc(this.BladeAngleInput.ServerValue, base.RpcTarget.NotServer);
	}

	// Token: 0x06000754 RID: 1876 RVA: 0x0002B398 File Offset: 0x00029598
	[Rpc(SendTo.SpecifiedInParams)]
	public void Server_BladeAngleInputRpc(sbyte value, RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 817646686U;
			RpcAttribute.RpcAttributeParams attributeParams = default(RpcAttribute.RpcAttributeParams);
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.SpecifiedInParams, RpcDelivery.Reliable);
			fastBufferWriter.WriteValueSafe<sbyte>(value, default(FastBufferWriter.ForPrimitives));
			base.__endSendRpc(ref fastBufferWriter, 817646686U, rpcParams, attributeParams, SendTo.SpecifiedInParams, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		this.BladeAngleInput.ServerValue = value;
	}

	// Token: 0x06000755 RID: 1877 RVA: 0x0002B478 File Offset: 0x00029678
	[Rpc(SendTo.Server)]
	public void Client_SlideInputRpc(bool value)
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 804686296U;
			RpcParams rpcParams2;
			RpcParams rpcParams = rpcParams2;
			RpcAttribute.RpcAttributeParams attributeParams = default(RpcAttribute.RpcAttributeParams);
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
			fastBufferWriter.WriteValueSafe<bool>(value, default(FastBufferWriter.ForPrimitives));
			base.__endSendRpc(ref fastBufferWriter, 804686296U, rpcParams2, attributeParams, SendTo.Server, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		this.SlideInput.ServerValue = value;
		this.Server_SlideInputRpc(this.SlideInput.ServerValue, base.RpcTarget.NotServer);
	}

	// Token: 0x06000756 RID: 1878 RVA: 0x0002B578 File Offset: 0x00029778
	[Rpc(SendTo.SpecifiedInParams)]
	public void Server_SlideInputRpc(bool value, RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 4107840079U;
			RpcAttribute.RpcAttributeParams attributeParams = default(RpcAttribute.RpcAttributeParams);
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.SpecifiedInParams, RpcDelivery.Reliable);
			fastBufferWriter.WriteValueSafe<bool>(value, default(FastBufferWriter.ForPrimitives));
			base.__endSendRpc(ref fastBufferWriter, 4107840079U, rpcParams, attributeParams, SendTo.SpecifiedInParams, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		this.SlideInput.ServerValue = value;
	}

	// Token: 0x06000757 RID: 1879 RVA: 0x0002B658 File Offset: 0x00029858
	[Rpc(SendTo.Server)]
	public void Client_SprintInputRpc(bool value)
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 2917244568U;
			RpcParams rpcParams2;
			RpcParams rpcParams = rpcParams2;
			RpcAttribute.RpcAttributeParams attributeParams = default(RpcAttribute.RpcAttributeParams);
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
			fastBufferWriter.WriteValueSafe<bool>(value, default(FastBufferWriter.ForPrimitives));
			base.__endSendRpc(ref fastBufferWriter, 2917244568U, rpcParams2, attributeParams, SendTo.Server, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		this.SprintInput.ServerValue = value;
		this.Server_SprintInputRpc(this.SprintInput.ServerValue, base.RpcTarget.NotServer);
	}

	// Token: 0x06000758 RID: 1880 RVA: 0x0002B758 File Offset: 0x00029958
	[Rpc(SendTo.SpecifiedInParams)]
	public void Server_SprintInputRpc(bool value, RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 778340344U;
			RpcAttribute.RpcAttributeParams attributeParams = default(RpcAttribute.RpcAttributeParams);
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.SpecifiedInParams, RpcDelivery.Reliable);
			fastBufferWriter.WriteValueSafe<bool>(value, default(FastBufferWriter.ForPrimitives));
			base.__endSendRpc(ref fastBufferWriter, 778340344U, rpcParams, attributeParams, SendTo.SpecifiedInParams, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		this.SprintInput.ServerValue = value;
	}

	// Token: 0x06000759 RID: 1881 RVA: 0x0002B838 File Offset: 0x00029A38
	[Rpc(SendTo.Server)]
	public void Client_TrackInputRpc(bool value)
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 3765825011U;
			RpcParams rpcParams2;
			RpcParams rpcParams = rpcParams2;
			RpcAttribute.RpcAttributeParams attributeParams = default(RpcAttribute.RpcAttributeParams);
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
			fastBufferWriter.WriteValueSafe<bool>(value, default(FastBufferWriter.ForPrimitives));
			base.__endSendRpc(ref fastBufferWriter, 3765825011U, rpcParams2, attributeParams, SendTo.Server, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		this.TrackInput.ServerValue = value;
		this.Server_TrackInputRpc(this.TrackInput.ServerValue, base.RpcTarget.NotServer);
	}

	// Token: 0x0600075A RID: 1882 RVA: 0x0002B938 File Offset: 0x00029B38
	[Rpc(SendTo.SpecifiedInParams)]
	public void Server_TrackInputRpc(bool value, RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 2722698928U;
			RpcAttribute.RpcAttributeParams attributeParams = default(RpcAttribute.RpcAttributeParams);
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.SpecifiedInParams, RpcDelivery.Reliable);
			fastBufferWriter.WriteValueSafe<bool>(value, default(FastBufferWriter.ForPrimitives));
			base.__endSendRpc(ref fastBufferWriter, 2722698928U, rpcParams, attributeParams, SendTo.SpecifiedInParams, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		this.TrackInput.ServerValue = value;
	}

	// Token: 0x0600075B RID: 1883 RVA: 0x0002BA18 File Offset: 0x00029C18
	[Rpc(SendTo.Server)]
	public void Client_LookInputRpc(bool value)
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 3995092734U;
			RpcParams rpcParams2;
			RpcParams rpcParams = rpcParams2;
			RpcAttribute.RpcAttributeParams attributeParams = default(RpcAttribute.RpcAttributeParams);
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
			fastBufferWriter.WriteValueSafe<bool>(value, default(FastBufferWriter.ForPrimitives));
			base.__endSendRpc(ref fastBufferWriter, 3995092734U, rpcParams2, attributeParams, SendTo.Server, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		this.LookInput.ServerValue = value;
		this.Server_LookInputRpc(this.LookInput.ServerValue, base.RpcTarget.NotServer);
	}

	// Token: 0x0600075C RID: 1884 RVA: 0x0002BB18 File Offset: 0x00029D18
	[Rpc(SendTo.SpecifiedInParams)]
	public void Server_LookInputRpc(bool value, RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 3779091983U;
			RpcAttribute.RpcAttributeParams attributeParams = default(RpcAttribute.RpcAttributeParams);
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.SpecifiedInParams, RpcDelivery.Reliable);
			fastBufferWriter.WriteValueSafe<bool>(value, default(FastBufferWriter.ForPrimitives));
			base.__endSendRpc(ref fastBufferWriter, 3779091983U, rpcParams, attributeParams, SendTo.SpecifiedInParams, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		this.LookInput.ServerValue = value;
	}

	// Token: 0x0600075D RID: 1885 RVA: 0x0002BBF8 File Offset: 0x00029DF8
	[Rpc(SendTo.Server)]
	public void Client_JumpInputRpc()
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 4077849638U;
			RpcParams rpcParams2;
			RpcParams rpcParams = rpcParams2;
			RpcAttribute.RpcAttributeParams attributeParams = default(RpcAttribute.RpcAttributeParams);
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
			base.__endSendRpc(ref fastBufferWriter, 4077849638U, rpcParams2, attributeParams, SendTo.Server, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		if (!this.JumpInput.ShouldChange)
		{
			return;
		}
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Server_OnPlayerJumpInput", new Dictionary<string, object>
		{
			{
				"player",
				this.Player
			}
		});
		this.JumpInput.ServerTick();
	}

	// Token: 0x0600075E RID: 1886 RVA: 0x0002BCF0 File Offset: 0x00029EF0
	[Rpc(SendTo.Server)]
	public void Client_StopInputRpc(bool value)
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 3261073083U;
			RpcParams rpcParams2;
			RpcParams rpcParams = rpcParams2;
			RpcAttribute.RpcAttributeParams attributeParams = default(RpcAttribute.RpcAttributeParams);
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
			fastBufferWriter.WriteValueSafe<bool>(value, default(FastBufferWriter.ForPrimitives));
			base.__endSendRpc(ref fastBufferWriter, 3261073083U, rpcParams2, attributeParams, SendTo.Server, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		this.StopInput.ServerValue = value;
	}

	// Token: 0x0600075F RID: 1887 RVA: 0x0002BDD0 File Offset: 0x00029FD0
	[Rpc(SendTo.Server)]
	public void Client_DashLeftInputRpc()
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 3013974635U;
			RpcParams rpcParams2;
			RpcParams rpcParams = rpcParams2;
			RpcAttribute.RpcAttributeParams attributeParams = default(RpcAttribute.RpcAttributeParams);
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
			base.__endSendRpc(ref fastBufferWriter, 3013974635U, rpcParams2, attributeParams, SendTo.Server, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		if (!this.DashLeftInput.ShouldChange)
		{
			return;
		}
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Server_OnPlayerDashLeftInput", new Dictionary<string, object>
		{
			{
				"player",
				this.Player
			}
		});
		this.DashLeftInput.ServerTick();
	}

	// Token: 0x06000760 RID: 1888 RVA: 0x0002BEC8 File Offset: 0x0002A0C8
	[Rpc(SendTo.Server)]
	public void Client_DashRightInputRpc()
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 341272022U;
			RpcParams rpcParams2;
			RpcParams rpcParams = rpcParams2;
			RpcAttribute.RpcAttributeParams attributeParams = default(RpcAttribute.RpcAttributeParams);
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
			base.__endSendRpc(ref fastBufferWriter, 341272022U, rpcParams2, attributeParams, SendTo.Server, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		if (!this.DashRightInput.ShouldChange)
		{
			return;
		}
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Server_OnPlayerDashRightInput", new Dictionary<string, object>
		{
			{
				"player",
				this.Player
			}
		});
		this.DashRightInput.ServerTick();
	}

	// Token: 0x06000761 RID: 1889 RVA: 0x0002BFC0 File Offset: 0x0002A1C0
	[Rpc(SendTo.Server)]
	public void Client_TwistLeftInputRpc()
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 1583302350U;
			RpcParams rpcParams2;
			RpcParams rpcParams = rpcParams2;
			RpcAttribute.RpcAttributeParams attributeParams = default(RpcAttribute.RpcAttributeParams);
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
			base.__endSendRpc(ref fastBufferWriter, 1583302350U, rpcParams2, attributeParams, SendTo.Server, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		if (!this.TwistLeftInput.ShouldChange)
		{
			return;
		}
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Server_OnPlayerTwistLeftInput", new Dictionary<string, object>
		{
			{
				"player",
				this.Player
			}
		});
		this.TwistLeftInput.ServerTick();
	}

	// Token: 0x06000762 RID: 1890 RVA: 0x0002C0B8 File Offset: 0x0002A2B8
	[Rpc(SendTo.Server)]
	public void Client_TwistRightInputRpc()
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 2380271940U;
			RpcParams rpcParams2;
			RpcParams rpcParams = rpcParams2;
			RpcAttribute.RpcAttributeParams attributeParams = default(RpcAttribute.RpcAttributeParams);
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
			base.__endSendRpc(ref fastBufferWriter, 2380271940U, rpcParams2, attributeParams, SendTo.Server, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		if (!this.TwistRightInput.ShouldChange)
		{
			return;
		}
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Server_OnPlayerTwistRightInput", new Dictionary<string, object>
		{
			{
				"player",
				this.Player
			}
		});
		this.TwistRightInput.ServerTick();
	}

	// Token: 0x06000763 RID: 1891 RVA: 0x0002C1B0 File Offset: 0x0002A3B0
	[Rpc(SendTo.Server)]
	public void Client_ExtendLeftInputRpc(bool value)
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 2897220457U;
			RpcParams rpcParams2;
			RpcParams rpcParams = rpcParams2;
			RpcAttribute.RpcAttributeParams attributeParams = default(RpcAttribute.RpcAttributeParams);
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
			fastBufferWriter.WriteValueSafe<bool>(value, default(FastBufferWriter.ForPrimitives));
			base.__endSendRpc(ref fastBufferWriter, 2897220457U, rpcParams2, attributeParams, SendTo.Server, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		this.ExtendLeftInput.ServerValue = value;
		this.Server_ExtendLeftInputRpc(this.ExtendLeftInput.ServerValue, base.RpcTarget.NotServer);
	}

	// Token: 0x06000764 RID: 1892 RVA: 0x0002C2B0 File Offset: 0x0002A4B0
	[Rpc(SendTo.SpecifiedInParams)]
	public void Server_ExtendLeftInputRpc(bool value, RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 3288109408U;
			RpcAttribute.RpcAttributeParams attributeParams = default(RpcAttribute.RpcAttributeParams);
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.SpecifiedInParams, RpcDelivery.Reliable);
			fastBufferWriter.WriteValueSafe<bool>(value, default(FastBufferWriter.ForPrimitives));
			base.__endSendRpc(ref fastBufferWriter, 3288109408U, rpcParams, attributeParams, SendTo.SpecifiedInParams, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		this.ExtendLeftInput.ServerValue = value;
	}

	// Token: 0x06000765 RID: 1893 RVA: 0x0002C390 File Offset: 0x0002A590
	[Rpc(SendTo.Server)]
	public void Client_ExtendRightInputRpc(bool value)
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 2892078582U;
			RpcParams rpcParams2;
			RpcParams rpcParams = rpcParams2;
			RpcAttribute.RpcAttributeParams attributeParams = default(RpcAttribute.RpcAttributeParams);
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
			fastBufferWriter.WriteValueSafe<bool>(value, default(FastBufferWriter.ForPrimitives));
			base.__endSendRpc(ref fastBufferWriter, 2892078582U, rpcParams2, attributeParams, SendTo.Server, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		this.ExtendRightInput.ServerValue = value;
		this.Server_ExtendRightInputRpc(this.ExtendRightInput.ServerValue, base.RpcTarget.NotServer);
	}

	// Token: 0x06000766 RID: 1894 RVA: 0x0002C490 File Offset: 0x0002A690
	[Rpc(SendTo.SpecifiedInParams)]
	public void Server_ExtendRightInputRpc(bool value, RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 152722375U;
			RpcAttribute.RpcAttributeParams attributeParams = default(RpcAttribute.RpcAttributeParams);
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.SpecifiedInParams, RpcDelivery.Reliable);
			fastBufferWriter.WriteValueSafe<bool>(value, default(FastBufferWriter.ForPrimitives));
			base.__endSendRpc(ref fastBufferWriter, 152722375U, rpcParams, attributeParams, SendTo.SpecifiedInParams, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		this.ExtendRightInput.ServerValue = value;
	}

	// Token: 0x06000767 RID: 1895 RVA: 0x0002C570 File Offset: 0x0002A770
	[Rpc(SendTo.Server)]
	public void Client_LateralLeftInputRpc(bool value)
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 3433706111U;
			RpcParams rpcParams2;
			RpcParams rpcParams = rpcParams2;
			RpcAttribute.RpcAttributeParams attributeParams = default(RpcAttribute.RpcAttributeParams);
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
			fastBufferWriter.WriteValueSafe<bool>(value, default(FastBufferWriter.ForPrimitives));
			base.__endSendRpc(ref fastBufferWriter, 3433706111U, rpcParams2, attributeParams, SendTo.Server, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		this.LateralLeftInput.ServerValue = value;
	}

	// Token: 0x06000768 RID: 1896 RVA: 0x0002C650 File Offset: 0x0002A850
	[Rpc(SendTo.Server)]
	public void Client_LateralRightInputRpc(bool value)
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 1602051713U;
			RpcParams rpcParams2;
			RpcParams rpcParams = rpcParams2;
			RpcAttribute.RpcAttributeParams attributeParams = default(RpcAttribute.RpcAttributeParams);
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
			fastBufferWriter.WriteValueSafe<bool>(value, default(FastBufferWriter.ForPrimitives));
			base.__endSendRpc(ref fastBufferWriter, 1602051713U, rpcParams2, attributeParams, SendTo.Server, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		this.LateralRightInput.ServerValue = value;
	}

	// Token: 0x06000769 RID: 1897 RVA: 0x0002C730 File Offset: 0x0002A930
	[Rpc(SendTo.Server)]
	public void Client_TalkInputRpc(bool value)
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 1234853793U;
			RpcParams rpcParams2;
			RpcParams rpcParams = rpcParams2;
			RpcAttribute.RpcAttributeParams attributeParams = default(RpcAttribute.RpcAttributeParams);
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
			fastBufferWriter.WriteValueSafe<bool>(value, default(FastBufferWriter.ForPrimitives));
			base.__endSendRpc(ref fastBufferWriter, 1234853793U, rpcParams2, attributeParams, SendTo.Server, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		this.TalkInput.ServerValue = value;
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnPlayerTalkInput", new Dictionary<string, object>
		{
			{
				"player",
				this.Player
			},
			{
				"value",
				value
			}
		});
		this.Server_TalkInputRpc(this.TalkInput.ServerValue, base.RpcTarget.NotServer);
	}

	// Token: 0x0600076A RID: 1898 RVA: 0x0002C868 File Offset: 0x0002AA68
	[Rpc(SendTo.SpecifiedInParams)]
	public void Server_TalkInputRpc(bool value, RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 3713736028U;
			RpcAttribute.RpcAttributeParams attributeParams = default(RpcAttribute.RpcAttributeParams);
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.SpecifiedInParams, RpcDelivery.Reliable);
			fastBufferWriter.WriteValueSafe<bool>(value, default(FastBufferWriter.ForPrimitives));
			base.__endSendRpc(ref fastBufferWriter, 3713736028U, rpcParams, attributeParams, SendTo.SpecifiedInParams, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		this.TalkInput.ServerValue = value;
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnPlayerTalkInput", new Dictionary<string, object>
		{
			{
				"player",
				this.Player
			},
			{
				"value",
				value
			}
		});
	}

	// Token: 0x0600076B RID: 1899 RVA: 0x0002C97C File Offset: 0x0002AB7C
	[Rpc(SendTo.Server)]
	public void Client_SleepInputRpc(bool value)
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 1182029238U;
			RpcParams rpcParams2;
			RpcParams rpcParams = rpcParams2;
			RpcAttribute.RpcAttributeParams attributeParams = default(RpcAttribute.RpcAttributeParams);
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
			fastBufferWriter.WriteValueSafe<bool>(value, default(FastBufferWriter.ForPrimitives));
			base.__endSendRpc(ref fastBufferWriter, 1182029238U, rpcParams2, attributeParams, SendTo.Server, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		this.SleepInput.ServerValue = value;
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Server_OnPlayerSleepInput", new Dictionary<string, object>
		{
			{
				"player",
				this.Player
			},
			{
				"value",
				value
			}
		});
	}

	// Token: 0x0600076C RID: 1900 RVA: 0x0002CA90 File Offset: 0x0002AC90
	public void Server_ForceSynchronizeClientId(ulong clientId)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (clientId == 0UL)
		{
			return;
		}
		this.Server_BladeAngleInputRpc(this.BladeAngleInput.ServerValue, base.RpcTarget.Single(clientId, RpcTargetUse.Persistent));
		this.Server_SlideInputRpc(this.SlideInput.ServerValue, base.RpcTarget.Single(clientId, RpcTargetUse.Persistent));
		this.Server_SprintInputRpc(this.SprintInput.ServerValue, base.RpcTarget.Single(clientId, RpcTargetUse.Persistent));
		this.Server_TrackInputRpc(this.TrackInput.ServerValue, base.RpcTarget.Single(clientId, RpcTargetUse.Persistent));
		this.Server_LookInputRpc(this.LookInput.ServerValue, base.RpcTarget.Single(clientId, RpcTargetUse.Persistent));
		this.Server_ExtendLeftInputRpc(this.ExtendLeftInput.ServerValue, base.RpcTarget.Single(clientId, RpcTargetUse.Persistent));
		this.Server_ExtendRightInputRpc(this.ExtendRightInput.ServerValue, base.RpcTarget.Single(clientId, RpcTargetUse.Persistent));
		this.Server_TalkInputRpc(this.TalkInput.ServerValue, base.RpcTarget.Single(clientId, RpcTargetUse.Persistent));
	}

	// Token: 0x0600076D RID: 1901 RVA: 0x0002CBC8 File Offset: 0x0002ADC8
	private void OnInputActionChangeCallback(object obj, InputActionChange change)
	{
		if (!NetworkManager.Singleton || NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.SleepInput.ClientValue = false;
		Tween tween = this.sleepingTween;
		if (tween != null)
		{
			tween.Kill(false);
		}
		this.sleepingTween = DOVirtual.DelayedCall(this.SleepTimeout, delegate
		{
			this.SleepInput.ClientValue = true;
		}, true);
	}

	// Token: 0x06000774 RID: 1908 RVA: 0x0000F5B8 File Offset: 0x0000D7B8
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x06000775 RID: 1909 RVA: 0x0002CF14 File Offset: 0x0002B114
	protected override void __initializeRpcs()
	{
		base.__registerRpc(354985997U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_354985997), "Client_MoveInputRpc");
		base.__registerRpc(2333051307U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_2333051307), "Server_MoveInputRpc");
		base.__registerRpc(3072819325U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_3072819325), "Client_RaycastOriginAngleInputRpc");
		base.__registerRpc(3003669798U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_3003669798), "Server_RaycastOriginAngleInputRpc");
		base.__registerRpc(3839358977U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_3839358977), "Client_LookAngleInputRpc");
		base.__registerRpc(1047632353U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_1047632353), "Server_LookAngleInputRpc");
		base.__registerRpc(2671629003U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_2671629003), "Client_BladeAngleInputRpc");
		base.__registerRpc(817646686U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_817646686), "Server_BladeAngleInputRpc");
		base.__registerRpc(804686296U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_804686296), "Client_SlideInputRpc");
		base.__registerRpc(4107840079U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_4107840079), "Server_SlideInputRpc");
		base.__registerRpc(2917244568U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_2917244568), "Client_SprintInputRpc");
		base.__registerRpc(778340344U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_778340344), "Server_SprintInputRpc");
		base.__registerRpc(3765825011U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_3765825011), "Client_TrackInputRpc");
		base.__registerRpc(2722698928U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_2722698928), "Server_TrackInputRpc");
		base.__registerRpc(3995092734U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_3995092734), "Client_LookInputRpc");
		base.__registerRpc(3779091983U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_3779091983), "Server_LookInputRpc");
		base.__registerRpc(4077849638U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_4077849638), "Client_JumpInputRpc");
		base.__registerRpc(3261073083U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_3261073083), "Client_StopInputRpc");
		base.__registerRpc(3013974635U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_3013974635), "Client_DashLeftInputRpc");
		base.__registerRpc(341272022U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_341272022), "Client_DashRightInputRpc");
		base.__registerRpc(1583302350U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_1583302350), "Client_TwistLeftInputRpc");
		base.__registerRpc(2380271940U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_2380271940), "Client_TwistRightInputRpc");
		base.__registerRpc(2897220457U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_2897220457), "Client_ExtendLeftInputRpc");
		base.__registerRpc(3288109408U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_3288109408), "Server_ExtendLeftInputRpc");
		base.__registerRpc(2892078582U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_2892078582), "Client_ExtendRightInputRpc");
		base.__registerRpc(152722375U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_152722375), "Server_ExtendRightInputRpc");
		base.__registerRpc(3433706111U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_3433706111), "Client_LateralLeftInputRpc");
		base.__registerRpc(1602051713U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_1602051713), "Client_LateralRightInputRpc");
		base.__registerRpc(1234853793U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_1234853793), "Client_TalkInputRpc");
		base.__registerRpc(3713736028U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_3713736028), "Server_TalkInputRpc");
		base.__registerRpc(1182029238U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_1182029238), "Client_SleepInputRpc");
		base.__initializeRpcs();
	}

	// Token: 0x06000776 RID: 1910 RVA: 0x0002D290 File Offset: 0x0002B490
	private static void __rpc_handler_354985997(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		short x;
		ByteUnpacker.ReadValueBitPacked(reader, out x);
		short y;
		ByteUnpacker.ReadValueBitPacked(reader, out y);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((global::PlayerInput)target).Client_MoveInputRpc(x, y);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x06000777 RID: 1911 RVA: 0x0002D304 File Offset: 0x0002B504
	private static void __rpc_handler_2333051307(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		Vector2 value;
		reader.ReadValueSafe(out value);
		RpcParams ext = rpcParams.Ext;
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((global::PlayerInput)target).Server_MoveInputRpc(value, ext);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x06000778 RID: 1912 RVA: 0x0002D374 File Offset: 0x0002B574
	private static void __rpc_handler_3072819325(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		short x;
		ByteUnpacker.ReadValueBitPacked(reader, out x);
		short y;
		ByteUnpacker.ReadValueBitPacked(reader, out y);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((global::PlayerInput)target).Client_RaycastOriginAngleInputRpc(x, y);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x06000779 RID: 1913 RVA: 0x0002D3E8 File Offset: 0x0002B5E8
	private static void __rpc_handler_3003669798(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		Vector2 value;
		reader.ReadValueSafe(out value);
		RpcParams ext = rpcParams.Ext;
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((global::PlayerInput)target).Server_RaycastOriginAngleInputRpc(value, ext);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x0600077A RID: 1914 RVA: 0x0002D458 File Offset: 0x0002B658
	private static void __rpc_handler_3839358977(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		short x;
		ByteUnpacker.ReadValueBitPacked(reader, out x);
		short y;
		ByteUnpacker.ReadValueBitPacked(reader, out y);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((global::PlayerInput)target).Client_LookAngleInputRpc(x, y);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x0600077B RID: 1915 RVA: 0x0002D4CC File Offset: 0x0002B6CC
	private static void __rpc_handler_1047632353(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		short x;
		ByteUnpacker.ReadValueBitPacked(reader, out x);
		short y;
		ByteUnpacker.ReadValueBitPacked(reader, out y);
		RpcParams ext = rpcParams.Ext;
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((global::PlayerInput)target).Server_LookAngleInputRpc(x, y, ext);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x0600077C RID: 1916 RVA: 0x0002D550 File Offset: 0x0002B750
	private static void __rpc_handler_2671629003(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		sbyte value;
		reader.ReadValueSafe<sbyte>(out value, default(FastBufferWriter.ForPrimitives));
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((global::PlayerInput)target).Client_BladeAngleInputRpc(value);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x0600077D RID: 1917 RVA: 0x0002D5C0 File Offset: 0x0002B7C0
	private static void __rpc_handler_817646686(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		sbyte value;
		reader.ReadValueSafe<sbyte>(out value, default(FastBufferWriter.ForPrimitives));
		RpcParams ext = rpcParams.Ext;
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((global::PlayerInput)target).Server_BladeAngleInputRpc(value, ext);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x0600077E RID: 1918 RVA: 0x0002D640 File Offset: 0x0002B840
	private static void __rpc_handler_804686296(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		bool value;
		reader.ReadValueSafe<bool>(out value, default(FastBufferWriter.ForPrimitives));
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((global::PlayerInput)target).Client_SlideInputRpc(value);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x0600077F RID: 1919 RVA: 0x0002D6B0 File Offset: 0x0002B8B0
	private static void __rpc_handler_4107840079(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		bool value;
		reader.ReadValueSafe<bool>(out value, default(FastBufferWriter.ForPrimitives));
		RpcParams ext = rpcParams.Ext;
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((global::PlayerInput)target).Server_SlideInputRpc(value, ext);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x06000780 RID: 1920 RVA: 0x0002D730 File Offset: 0x0002B930
	private static void __rpc_handler_2917244568(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		bool value;
		reader.ReadValueSafe<bool>(out value, default(FastBufferWriter.ForPrimitives));
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((global::PlayerInput)target).Client_SprintInputRpc(value);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x06000781 RID: 1921 RVA: 0x0002D7A0 File Offset: 0x0002B9A0
	private static void __rpc_handler_778340344(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		bool value;
		reader.ReadValueSafe<bool>(out value, default(FastBufferWriter.ForPrimitives));
		RpcParams ext = rpcParams.Ext;
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((global::PlayerInput)target).Server_SprintInputRpc(value, ext);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x06000782 RID: 1922 RVA: 0x0002D820 File Offset: 0x0002BA20
	private static void __rpc_handler_3765825011(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		bool value;
		reader.ReadValueSafe<bool>(out value, default(FastBufferWriter.ForPrimitives));
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((global::PlayerInput)target).Client_TrackInputRpc(value);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x06000783 RID: 1923 RVA: 0x0002D890 File Offset: 0x0002BA90
	private static void __rpc_handler_2722698928(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		bool value;
		reader.ReadValueSafe<bool>(out value, default(FastBufferWriter.ForPrimitives));
		RpcParams ext = rpcParams.Ext;
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((global::PlayerInput)target).Server_TrackInputRpc(value, ext);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x06000784 RID: 1924 RVA: 0x0002D910 File Offset: 0x0002BB10
	private static void __rpc_handler_3995092734(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		bool value;
		reader.ReadValueSafe<bool>(out value, default(FastBufferWriter.ForPrimitives));
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((global::PlayerInput)target).Client_LookInputRpc(value);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x06000785 RID: 1925 RVA: 0x0002D980 File Offset: 0x0002BB80
	private static void __rpc_handler_3779091983(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		bool value;
		reader.ReadValueSafe<bool>(out value, default(FastBufferWriter.ForPrimitives));
		RpcParams ext = rpcParams.Ext;
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((global::PlayerInput)target).Server_LookInputRpc(value, ext);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x06000786 RID: 1926 RVA: 0x0002DA00 File Offset: 0x0002BC00
	private static void __rpc_handler_4077849638(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((global::PlayerInput)target).Client_JumpInputRpc();
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x06000787 RID: 1927 RVA: 0x0002DA54 File Offset: 0x0002BC54
	private static void __rpc_handler_3261073083(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		bool value;
		reader.ReadValueSafe<bool>(out value, default(FastBufferWriter.ForPrimitives));
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((global::PlayerInput)target).Client_StopInputRpc(value);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x06000788 RID: 1928 RVA: 0x0002DAC4 File Offset: 0x0002BCC4
	private static void __rpc_handler_3013974635(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((global::PlayerInput)target).Client_DashLeftInputRpc();
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x06000789 RID: 1929 RVA: 0x0002DB18 File Offset: 0x0002BD18
	private static void __rpc_handler_341272022(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((global::PlayerInput)target).Client_DashRightInputRpc();
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x0600078A RID: 1930 RVA: 0x0002DB6C File Offset: 0x0002BD6C
	private static void __rpc_handler_1583302350(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((global::PlayerInput)target).Client_TwistLeftInputRpc();
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x0600078B RID: 1931 RVA: 0x0002DBC0 File Offset: 0x0002BDC0
	private static void __rpc_handler_2380271940(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((global::PlayerInput)target).Client_TwistRightInputRpc();
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x0600078C RID: 1932 RVA: 0x0002DC14 File Offset: 0x0002BE14
	private static void __rpc_handler_2897220457(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		bool value;
		reader.ReadValueSafe<bool>(out value, default(FastBufferWriter.ForPrimitives));
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((global::PlayerInput)target).Client_ExtendLeftInputRpc(value);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x0600078D RID: 1933 RVA: 0x0002DC84 File Offset: 0x0002BE84
	private static void __rpc_handler_3288109408(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		bool value;
		reader.ReadValueSafe<bool>(out value, default(FastBufferWriter.ForPrimitives));
		RpcParams ext = rpcParams.Ext;
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((global::PlayerInput)target).Server_ExtendLeftInputRpc(value, ext);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x0600078E RID: 1934 RVA: 0x0002DD04 File Offset: 0x0002BF04
	private static void __rpc_handler_2892078582(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		bool value;
		reader.ReadValueSafe<bool>(out value, default(FastBufferWriter.ForPrimitives));
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((global::PlayerInput)target).Client_ExtendRightInputRpc(value);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x0600078F RID: 1935 RVA: 0x0002DD74 File Offset: 0x0002BF74
	private static void __rpc_handler_152722375(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		bool value;
		reader.ReadValueSafe<bool>(out value, default(FastBufferWriter.ForPrimitives));
		RpcParams ext = rpcParams.Ext;
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((global::PlayerInput)target).Server_ExtendRightInputRpc(value, ext);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x06000790 RID: 1936 RVA: 0x0002DDF4 File Offset: 0x0002BFF4
	private static void __rpc_handler_3433706111(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		bool value;
		reader.ReadValueSafe<bool>(out value, default(FastBufferWriter.ForPrimitives));
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((global::PlayerInput)target).Client_LateralLeftInputRpc(value);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x06000791 RID: 1937 RVA: 0x0002DE64 File Offset: 0x0002C064
	private static void __rpc_handler_1602051713(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		bool value;
		reader.ReadValueSafe<bool>(out value, default(FastBufferWriter.ForPrimitives));
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((global::PlayerInput)target).Client_LateralRightInputRpc(value);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x06000792 RID: 1938 RVA: 0x0002DED4 File Offset: 0x0002C0D4
	private static void __rpc_handler_1234853793(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		bool value;
		reader.ReadValueSafe<bool>(out value, default(FastBufferWriter.ForPrimitives));
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((global::PlayerInput)target).Client_TalkInputRpc(value);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x06000793 RID: 1939 RVA: 0x0002DF44 File Offset: 0x0002C144
	private static void __rpc_handler_3713736028(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		bool value;
		reader.ReadValueSafe<bool>(out value, default(FastBufferWriter.ForPrimitives));
		RpcParams ext = rpcParams.Ext;
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((global::PlayerInput)target).Server_TalkInputRpc(value, ext);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x06000794 RID: 1940 RVA: 0x0002DFC4 File Offset: 0x0002C1C4
	private static void __rpc_handler_1182029238(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		bool value;
		reader.ReadValueSafe<bool>(out value, default(FastBufferWriter.ForPrimitives));
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((global::PlayerInput)target).Client_SleepInputRpc(value);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x06000795 RID: 1941 RVA: 0x0000BB42 File Offset: 0x00009D42
	protected internal override string __getTypeName()
	{
		return "PlayerInput";
	}

	// Token: 0x0400043B RID: 1083
	[Header("Settings")]
	[SerializeField]
	private Vector3 initialLookAngle = new Vector3(30f, 0f, 0f);

	// Token: 0x0400043C RID: 1084
	[Space(20f)]
	[SerializeField]
	private Vector2 initialStickRaycastOriginAngle = new Vector2(40f, 80f);

	// Token: 0x0400043D RID: 1085
	[SerializeField]
	private Vector2 minimumStickRaycastOriginAngle = new Vector2(-25f, -92.5f);

	// Token: 0x0400043E RID: 1086
	[SerializeField]
	private Vector2 maximumStickRaycastOriginAngle = new Vector2(80f, 92.5f);

	// Token: 0x0400043F RID: 1087
	[Space(20f)]
	[SerializeField]
	private Vector2 minimumLookAngle = new Vector2(-25f, -135f);

	// Token: 0x04000440 RID: 1088
	[SerializeField]
	private Vector2 maximumLookAngle = new Vector2(75f, 135f);

	// Token: 0x04000441 RID: 1089
	[Space(20f)]
	[SerializeField]
	private int minimumBladeAngle = -4;

	// Token: 0x04000442 RID: 1090
	[SerializeField]
	private int maximumBladeAngle = 4;

	// Token: 0x04000443 RID: 1091
	[Space(20f)]
	[SerializeField]
	private Vector2 debugShootStartAngle = new Vector2(37.5f, 90f);

	// Token: 0x04000444 RID: 1092
	[SerializeField]
	private Vector2 debugShootEndAngle = new Vector2(37.5f, -90f);

	// Token: 0x04000445 RID: 1093
	public NetworkedInput<Vector2> MoveInput = new NetworkedInput<Vector2>(default(Vector2), null, null);

	// Token: 0x04000446 RID: 1094
	public NetworkedInput<Vector2> StickRaycastOriginAngleInput = new NetworkedInput<Vector2>(default(Vector2), (Vector2 lastSentValue, Vector2 clientValue) => Vector2.Distance(lastSentValue, clientValue) > 0.1f, null);

	// Token: 0x04000447 RID: 1095
	public NetworkedInput<Vector2> LookAngleInput = new NetworkedInput<Vector2>(default(Vector2), (Vector2 lastSentValue, Vector2 clientValue) => Vector2.Distance(lastSentValue, clientValue) > 0.1f, null);

	// Token: 0x04000448 RID: 1096
	public NetworkedInput<sbyte> BladeAngleInput = new NetworkedInput<sbyte>(0, null, null);

	// Token: 0x04000449 RID: 1097
	public NetworkedInput<bool> SlideInput = new NetworkedInput<bool>(false, null, null);

	// Token: 0x0400044A RID: 1098
	public NetworkedInput<bool> SprintInput = new NetworkedInput<bool>(false, null, null);

	// Token: 0x0400044B RID: 1099
	public NetworkedInput<bool> TrackInput = new NetworkedInput<bool>(false, null, null);

	// Token: 0x0400044C RID: 1100
	public NetworkedInput<bool> LookInput = new NetworkedInput<bool>(false, null, null);

	// Token: 0x0400044D RID: 1101
	public NetworkedInput<byte> JumpInput = new NetworkedInput<byte>(0, null, (byte lastReceivedValue, double lastReceivedTime, byte serverValue) => Time.timeAsDouble - lastReceivedTime > 0.5);

	// Token: 0x0400044E RID: 1102
	public NetworkedInput<bool> StopInput = new NetworkedInput<bool>(false, null, null);

	// Token: 0x0400044F RID: 1103
	public NetworkedInput<byte> TwistLeftInput = new NetworkedInput<byte>(0, null, (byte lastReceivedValue, double lastReceivedTime, byte serverValue) => Time.timeAsDouble - lastReceivedTime > 0.5);

	// Token: 0x04000450 RID: 1104
	public NetworkedInput<byte> TwistRightInput = new NetworkedInput<byte>(0, null, (byte lastReceivedValue, double lastReceivedTime, byte serverValue) => Time.timeAsDouble - lastReceivedTime > 0.5);

	// Token: 0x04000451 RID: 1105
	public NetworkedInput<byte> DashLeftInput = new NetworkedInput<byte>(0, null, (byte lastReceivedValue, double lastReceivedTime, byte serverValue) => Time.timeAsDouble - lastReceivedTime > 0.25);

	// Token: 0x04000452 RID: 1106
	public NetworkedInput<byte> DashRightInput = new NetworkedInput<byte>(0, null, (byte lastReceivedValue, double lastReceivedTime, byte serverValue) => Time.timeAsDouble - lastReceivedTime > 0.25);

	// Token: 0x04000453 RID: 1107
	public NetworkedInput<bool> ExtendLeftInput = new NetworkedInput<bool>(false, null, null);

	// Token: 0x04000454 RID: 1108
	public NetworkedInput<bool> ExtendRightInput = new NetworkedInput<bool>(false, null, null);

	// Token: 0x04000455 RID: 1109
	public NetworkedInput<bool> LateralLeftInput = new NetworkedInput<bool>(false, null, null);

	// Token: 0x04000456 RID: 1110
	public NetworkedInput<bool> LateralRightInput = new NetworkedInput<bool>(false, null, null);

	// Token: 0x04000457 RID: 1111
	public NetworkedInput<bool> TalkInput = new NetworkedInput<bool>(false, null, null);

	// Token: 0x04000458 RID: 1112
	public NetworkedInput<bool> SleepInput = new NetworkedInput<bool>(false, null, null);

	// Token: 0x04000459 RID: 1113
	[HideInInspector]
	public Player Player;

	// Token: 0x0400045A RID: 1114
	[HideInInspector]
	public int TickRate = 200;

	// Token: 0x0400045B RID: 1115
	[HideInInspector]
	public float SleepTimeout = 60f;

	// Token: 0x0400045C RID: 1116
	private float bladeAngleBuffer;

	// Token: 0x0400045D RID: 1117
	private bool shouldUpdateInputs;

	// Token: 0x0400045E RID: 1118
	private bool shouldTickInputs;

	// Token: 0x0400045F RID: 1119
	private float tickAccumulator;

	// Token: 0x04000460 RID: 1120
	private Tween sleepingTween;

	// Token: 0x04000461 RID: 1121
	private bool debugInputs;

	// Token: 0x04000462 RID: 1122
	private float lastDebugInputUpdateTime;

	// Token: 0x04000463 RID: 1123
	private Tween debugShootTween;
}
