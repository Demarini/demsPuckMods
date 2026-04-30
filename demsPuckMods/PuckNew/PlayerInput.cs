using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

// Token: 0x0200003B RID: 59
public class PlayerInput : NetworkBehaviour
{
	// Token: 0x17000024 RID: 36
	// (get) Token: 0x0600017B RID: 379 RVA: 0x00007F12 File Offset: 0x00006112
	[HideInInspector]
	public Vector2 MinimumStickRaycastOriginAngle
	{
		get
		{
			return this.minimumStickRaycastOriginAngle;
		}
	}

	// Token: 0x17000025 RID: 37
	// (get) Token: 0x0600017C RID: 380 RVA: 0x00007F1A File Offset: 0x0000611A
	[HideInInspector]
	public Vector2 MaximumStickRaycastOriginAngle
	{
		get
		{
			return this.maximumStickRaycastOriginAngle;
		}
	}

	// Token: 0x17000026 RID: 38
	// (get) Token: 0x0600017D RID: 381 RVA: 0x00007F22 File Offset: 0x00006122
	[HideInInspector]
	public Vector2 MinimumLookAngle
	{
		get
		{
			return this.minimumLookAngle;
		}
	}

	// Token: 0x17000027 RID: 39
	// (get) Token: 0x0600017E RID: 382 RVA: 0x00007F2A File Offset: 0x0000612A
	[HideInInspector]
	public Vector2 MaximumLookAngle
	{
		get
		{
			return this.maximumLookAngle;
		}
	}

	// Token: 0x17000028 RID: 40
	// (get) Token: 0x0600017F RID: 383 RVA: 0x00007F32 File Offset: 0x00006132
	[HideInInspector]
	public int MinimumBladeAngle
	{
		get
		{
			return this.minimumBladeAngle;
		}
	}

	// Token: 0x17000029 RID: 41
	// (get) Token: 0x06000180 RID: 384 RVA: 0x00007F3A File Offset: 0x0000613A
	[HideInInspector]
	public int MaximumBladeAngle
	{
		get
		{
			return this.maximumBladeAngle;
		}
	}

	// Token: 0x1700002A RID: 42
	// (get) Token: 0x06000181 RID: 385 RVA: 0x00007F42 File Offset: 0x00006142
	// (set) Token: 0x06000182 RID: 386 RVA: 0x00007F4F File Offset: 0x0000614F
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

	// Token: 0x06000183 RID: 387 RVA: 0x00007F67 File Offset: 0x00006167
	private void Awake()
	{
		this.Player = base.GetComponent<Player>();
	}

	// Token: 0x06000184 RID: 388 RVA: 0x00007F78 File Offset: 0x00006178
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
		InputManager.BladeAngleUpAction.performed += this.OnBladeAngleUpActionPerformed;
		InputManager.BladeAngleDownAction.performed += this.OnBladeAngleDownActionPerformed;
		InputManager.JumpAction.performed += this.OnJumpActionPerformed;
		InputManager.TwistLeftAction.performed += this.OnTwistLeftActionPerformed;
		InputManager.TwistRightAction.performed += this.OnTwistRightActionPerformed;
		InputManager.DashLeftAction.performed += this.OnDashLeftActionPerformed;
		InputManager.DashRightAction.performed += this.OnDashRightActionPerformed;
		this.shouldUpdateInputs = true;
		this.shouldTickInputs = true;
		base.OnNetworkSpawn();
	}

	// Token: 0x06000185 RID: 389 RVA: 0x00008058 File Offset: 0x00006258
	public override void OnNetworkDespawn()
	{
		if (!base.IsOwner)
		{
			return;
		}
		this.shouldUpdateInputs = false;
		this.shouldTickInputs = false;
		InputManager.BladeAngleUpAction.performed -= this.OnBladeAngleUpActionPerformed;
		InputManager.BladeAngleDownAction.performed -= this.OnBladeAngleDownActionPerformed;
		InputManager.JumpAction.performed -= this.OnJumpActionPerformed;
		InputManager.TwistLeftAction.performed -= this.OnTwistLeftActionPerformed;
		InputManager.TwistRightAction.performed -= this.OnTwistRightActionPerformed;
		InputManager.DashLeftAction.performed -= this.OnDashLeftActionPerformed;
		InputManager.DashRightAction.performed -= this.OnDashRightActionPerformed;
		base.OnNetworkDespawn();
	}

	// Token: 0x06000186 RID: 390 RVA: 0x0000811C File Offset: 0x0000631C
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

	// Token: 0x06000187 RID: 391 RVA: 0x00008190 File Offset: 0x00006390
	private void UpdateInputs()
	{
		if (GlobalStateManager.UIState.IsMouseRequired)
		{
			return;
		}
		this.MoveInput.ClientValue = new Vector2((float)((InputManager.TurnRightAction.IsInProgress() ? 1 : 0) + (InputManager.TurnLeftAction.IsInProgress() ? -1 : 0)), (float)((InputManager.MoveForwardAction.IsInProgress() ? 1 : 0) + (InputManager.MoveBackwardAction.IsInProgress() ? -1 : 0)));
		if (!this.LookInput.ClientValue)
		{
			Vector2 vector = InputManager.StickAction.ReadValue<Vector2>();
			Vector2 b = new Vector2(-vector.y * (SettingsManager.GlobalStickSensitivity / 2f) * SettingsManager.VerticalStickSensitivity, vector.x * (SettingsManager.GlobalStickSensitivity / 2f) * SettingsManager.HorizontalStickSensitivity);
			this.StickRaycastOriginAngleInput.ClientValue = Utils.Vector2Clamp(this.StickRaycastOriginAngleInput.ClientValue + b, this.minimumStickRaycastOriginAngle, this.maximumStickRaycastOriginAngle);
		}
		this.SlideInput.ClientValue = InputManager.SlideAction.IsInProgress();
		this.SprintInput.ClientValue = InputManager.SprintAction.IsInProgress();
		this.TrackInput.ClientValue = InputManager.TrackAction.IsInProgress();
		this.LookInput.ClientValue = InputManager.LookAction.IsInProgress();
		this.ExtendLeftInput.ClientValue = InputManager.ExtendLeftAction.IsInProgress();
		this.ExtendRightInput.ClientValue = InputManager.ExtendRightAction.IsInProgress();
		this.TalkInput.ClientValue = InputManager.TalkAction.IsInProgress();
		this.StopInput.ClientValue = InputManager.StopAction.IsInProgress();
	}

	// Token: 0x06000188 RID: 392 RVA: 0x00008328 File Offset: 0x00006528
	public void UpdateLookAngle(float deltaTime)
	{
		if (this.TrackInput.ClientValue && !this.LookInput.ClientValue)
		{
			Puck puck = MonoBehaviourSingleton<PuckManager>.Instance.GetPlayerPuck(base.OwnerClientId);
			if (!puck)
			{
				puck = MonoBehaviourSingleton<PuckManager>.Instance.GetPuck(false);
			}
			PlayerCamera playerCamera = this.Player.PlayerCamera;
			PlayerBody playerBody = this.Player.PlayerBody;
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
			Vector2 vector2 = InputManager.StickAction.ReadValue<Vector2>();
			Vector2 b = new Vector2(-vector2.y * (SettingsManager.LookSensitivity / 2f), vector2.x * (SettingsManager.LookSensitivity / 2f));
			this.LookAngleInput.ClientValue = Utils.Vector2Clamp(this.LookAngleInput.ClientValue + b, this.minimumLookAngle, this.maximumLookAngle);
			return;
		}
		if (!this.TrackInput.ClientValue)
		{
			this.LookAngleInput.ClientValue = Vector3.Lerp(this.LookAngleInput.ClientValue, this.initialLookAngle, deltaTime * 10f);
		}
	}

	// Token: 0x06000189 RID: 393 RVA: 0x000084FC File Offset: 0x000066FC
	public void ResetInputs(PlayerHandedness handedness)
	{
		this.MoveInput.ClientValue = Vector2.zero;
		this.LookAngleInput.ClientValue = this.initialLookAngle;
		this.StickRaycastOriginAngleInput.ClientValue = new Vector2(this.initialStickRaycastOriginAngle.x, (handedness == PlayerHandedness.Left) ? (-this.initialStickRaycastOriginAngle.y) : this.initialStickRaycastOriginAngle.y);
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
	}

	// Token: 0x0600018A RID: 394 RVA: 0x000085F4 File Offset: 0x000067F4
	private void OnBladeAngleUpActionPerformed(InputAction.CallbackContext context)
	{
		if (GlobalStateManager.UIState.IsMouseRequired)
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

	// Token: 0x0600018B RID: 395 RVA: 0x00008668 File Offset: 0x00006868
	private void OnBladeAngleDownActionPerformed(InputAction.CallbackContext context)
	{
		if (GlobalStateManager.UIState.IsMouseRequired)
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

	// Token: 0x0600018C RID: 396 RVA: 0x000086DA File Offset: 0x000068DA
	private void OnJumpActionPerformed(InputAction.CallbackContext context)
	{
		if (GlobalStateManager.UIState.IsMouseRequired)
		{
			return;
		}
		NetworkedInput<byte> jumpInput = this.JumpInput;
		jumpInput.ClientValue += 1;
	}

	// Token: 0x0600018D RID: 397 RVA: 0x000086FD File Offset: 0x000068FD
	private void OnTwistLeftActionPerformed(InputAction.CallbackContext context)
	{
		if (GlobalStateManager.UIState.IsMouseRequired)
		{
			return;
		}
		NetworkedInput<byte> twistLeftInput = this.TwistLeftInput;
		twistLeftInput.ClientValue += 1;
	}

	// Token: 0x0600018E RID: 398 RVA: 0x00008720 File Offset: 0x00006920
	private void OnTwistRightActionPerformed(InputAction.CallbackContext context)
	{
		if (GlobalStateManager.UIState.IsMouseRequired)
		{
			return;
		}
		NetworkedInput<byte> twistRightInput = this.TwistRightInput;
		twistRightInput.ClientValue += 1;
	}

	// Token: 0x0600018F RID: 399 RVA: 0x00008743 File Offset: 0x00006943
	private void OnDashLeftActionPerformed(InputAction.CallbackContext context)
	{
		if (GlobalStateManager.UIState.IsMouseRequired)
		{
			return;
		}
		NetworkedInput<byte> dashLeftInput = this.DashLeftInput;
		dashLeftInput.ClientValue += 1;
	}

	// Token: 0x06000190 RID: 400 RVA: 0x00008766 File Offset: 0x00006966
	private void OnDashRightActionPerformed(InputAction.CallbackContext context)
	{
		if (GlobalStateManager.UIState.IsMouseRequired)
		{
			return;
		}
		NetworkedInput<byte> dashRightInput = this.DashRightInput;
		dashRightInput.ClientValue += 1;
	}

	// Token: 0x06000191 RID: 401 RVA: 0x0000878C File Offset: 0x0000698C
	private void ClientTick()
	{
		if (this.MoveInput.HasChanged)
		{
			this.Client_MoveInputRpc(NetworkingUtils.CompressFloatToShort(this.MoveInput.ClientValue.x, -1f, 1f), NetworkingUtils.CompressFloatToShort(this.MoveInput.ClientValue.y, -1f, 1f), default(RpcParams));
			this.MoveInput.ClientTick();
		}
		if (this.StickRaycastOriginAngleInput.HasChanged)
		{
			this.Client_RaycastOriginAngleInputRpc(NetworkingUtils.CompressFloatToShort(this.StickRaycastOriginAngleInput.ClientValue.x, this.minimumStickRaycastOriginAngle.x, this.maximumStickRaycastOriginAngle.x), NetworkingUtils.CompressFloatToShort(this.StickRaycastOriginAngleInput.ClientValue.y, this.minimumStickRaycastOriginAngle.y, this.maximumStickRaycastOriginAngle.y), default(RpcParams));
			this.StickRaycastOriginAngleInput.ClientTick();
		}
		if (this.LookAngleInput.HasChanged)
		{
			this.Client_LookAngleInputRpc(NetworkingUtils.CompressFloatToShort(this.LookAngleInput.ClientValue.x, this.minimumLookAngle.x, this.maximumLookAngle.x), NetworkingUtils.CompressFloatToShort(this.LookAngleInput.ClientValue.y, this.minimumLookAngle.y, this.maximumLookAngle.y), default(RpcParams));
			this.LookAngleInput.ClientTick();
		}
		if (this.BladeAngleInput.HasChanged)
		{
			this.Client_BladeAngleInputRpc(this.BladeAngleInput.ClientValue, default(RpcParams));
			this.BladeAngleInput.ClientTick();
		}
		if (this.SlideInput.HasChanged)
		{
			this.Client_SlideInputRpc(this.SlideInput.ClientValue, default(RpcParams));
			this.SlideInput.ClientTick();
		}
		if (this.SprintInput.HasChanged)
		{
			this.Client_SprintInputRpc(this.SprintInput.ClientValue, default(RpcParams));
			this.SprintInput.ClientTick();
		}
		if (this.TrackInput.HasChanged)
		{
			this.Client_TrackInputRpc(this.TrackInput.ClientValue, default(RpcParams));
			this.TrackInput.ClientTick();
		}
		if (this.LookInput.HasChanged)
		{
			this.Client_LookInputRpc(this.LookInput.ClientValue, default(RpcParams));
			this.LookInput.ClientTick();
		}
		if (this.JumpInput.HasChanged)
		{
			this.Client_JumpInputRpc(default(RpcParams));
			this.JumpInput.ClientTick();
		}
		if (this.StopInput.HasChanged)
		{
			this.Client_StopInputRpc(this.StopInput.ClientValue, default(RpcParams));
			this.StopInput.ClientTick();
		}
		if (this.TwistLeftInput.HasChanged)
		{
			this.Client_TwistLeftInputRpc(default(RpcParams));
			this.TwistLeftInput.ClientTick();
		}
		if (this.TwistRightInput.HasChanged)
		{
			this.Client_TwistRightInputRpc(default(RpcParams));
			this.TwistRightInput.ClientTick();
		}
		if (this.DashLeftInput.HasChanged)
		{
			this.Client_DashLeftInputRpc(default(RpcParams));
			this.DashLeftInput.ClientTick();
		}
		if (this.DashRightInput.HasChanged)
		{
			this.Client_DashRightInputRpc(default(RpcParams));
			this.DashRightInput.ClientTick();
		}
		if (this.ExtendLeftInput.HasChanged)
		{
			this.Client_ExtendLeftInputRpc(this.ExtendLeftInput.ClientValue, default(RpcParams));
			this.ExtendLeftInput.ClientTick();
		}
		if (this.ExtendRightInput.HasChanged)
		{
			this.Client_ExtendRightInputRpc(this.ExtendRightInput.ClientValue, default(RpcParams));
			this.ExtendRightInput.ClientTick();
		}
		if (this.LateralLeftInput.HasChanged)
		{
			this.Client_LateralLeftInputRpc(this.LateralLeftInput.ClientValue, default(RpcParams));
			this.LateralLeftInput.ClientTick();
		}
		if (this.LateralRightInput.HasChanged)
		{
			this.Client_LateralRightInputRpc(this.LateralRightInput.ClientValue, default(RpcParams));
			this.LateralRightInput.ClientTick();
		}
		if (this.TalkInput.HasChanged)
		{
			this.Client_TalkInputRpc(this.TalkInput.ClientValue, default(RpcParams));
			this.TalkInput.ClientTick();
		}
	}

	// Token: 0x06000192 RID: 402 RVA: 0x00008BE4 File Offset: 0x00006DE4
	[Rpc(SendTo.Server, DeferLocal = true)]
	public void Client_MoveInputRpc(short x, short y, RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			Debug.LogError("Rpc methods can only be invoked after starting the NetworkManager!");
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 2880114289U;
			RpcAttribute.RpcAttributeParams attributeParams = new RpcAttribute.RpcAttributeParams
			{
				DeferLocal = true
			};
			FastBufferWriter writer = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
			BytePacker.WriteValueBitPacked(writer, x);
			BytePacker.WriteValueBitPacked(writer, y);
			base.__endSendRpc(ref writer, 2880114289U, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		if (rpcParams.Receive.SenderClientId != base.OwnerClientId && rpcParams.Receive.SenderClientId != 0UL)
		{
			return;
		}
		this.Server_MoveInputRpc(x, y, base.RpcTarget.Everyone);
	}

	// Token: 0x06000193 RID: 403 RVA: 0x00008D04 File Offset: 0x00006F04
	[Rpc(SendTo.SpecifiedInParams, InvokePermission = RpcInvokePermission.Server, DeferLocal = true)]
	public void Server_MoveInputRpc(short x, short y, RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			Debug.LogError("Rpc methods can only be invoked after starting the NetworkManager!");
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 2371540155U;
			RpcAttribute.RpcAttributeParams attributeParams = new RpcAttribute.RpcAttributeParams
			{
				InvokePermission = RpcInvokePermission.Server,
				DeferLocal = true
			};
			FastBufferWriter writer = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.SpecifiedInParams, RpcDelivery.Reliable);
			BytePacker.WriteValueBitPacked(writer, x);
			BytePacker.WriteValueBitPacked(writer, y);
			base.__endSendRpc(ref writer, 2371540155U, rpcParams, attributeParams, SendTo.SpecifiedInParams, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		Vector2 vector = new Vector2(NetworkingUtils.DecompressShortToFloat(x, -1f, 1f), NetworkingUtils.DecompressShortToFloat(y, -1f, 1f));
		vector = Utils.Vector2Clamp(vector, -Vector2.one, Vector2.one);
		this.MoveInput.ServerValue = vector;
	}

	// Token: 0x06000194 RID: 404 RVA: 0x00008E44 File Offset: 0x00007044
	[Rpc(SendTo.Server, DeferLocal = true, Delivery = RpcDelivery.Unreliable)]
	public void Client_RaycastOriginAngleInputRpc(short x, short y, RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			Debug.LogError("Rpc methods can only be invoked after starting the NetworkManager!");
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 4145643342U;
			RpcAttribute.RpcAttributeParams attributeParams = new RpcAttribute.RpcAttributeParams
			{
				DeferLocal = true,
				Delivery = RpcDelivery.Unreliable
			};
			FastBufferWriter writer = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Unreliable);
			BytePacker.WriteValueBitPacked(writer, x);
			BytePacker.WriteValueBitPacked(writer, y);
			base.__endSendRpc(ref writer, 4145643342U, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Unreliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		if (rpcParams.Receive.SenderClientId != base.OwnerClientId && rpcParams.Receive.SenderClientId != 0UL)
		{
			return;
		}
		this.Server_RaycastOriginAngleInputRpc(x, y, base.RpcTarget.Everyone);
	}

	// Token: 0x06000195 RID: 405 RVA: 0x00008F70 File Offset: 0x00007170
	[Rpc(SendTo.SpecifiedInParams, InvokePermission = RpcInvokePermission.Server, DeferLocal = true, Delivery = RpcDelivery.Unreliable)]
	public void Server_RaycastOriginAngleInputRpc(short x, short y, RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			Debug.LogError("Rpc methods can only be invoked after starting the NetworkManager!");
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 3031376689U;
			RpcAttribute.RpcAttributeParams attributeParams = new RpcAttribute.RpcAttributeParams
			{
				InvokePermission = RpcInvokePermission.Server,
				DeferLocal = true,
				Delivery = RpcDelivery.Unreliable
			};
			FastBufferWriter writer = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.SpecifiedInParams, RpcDelivery.Unreliable);
			BytePacker.WriteValueBitPacked(writer, x);
			BytePacker.WriteValueBitPacked(writer, y);
			base.__endSendRpc(ref writer, 3031376689U, rpcParams, attributeParams, SendTo.SpecifiedInParams, RpcDelivery.Unreliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		Vector2 vector = new Vector2(NetworkingUtils.DecompressShortToFloat(x, this.minimumStickRaycastOriginAngle.x, this.maximumStickRaycastOriginAngle.x), NetworkingUtils.DecompressShortToFloat(y, this.minimumStickRaycastOriginAngle.y, this.maximumStickRaycastOriginAngle.y));
		vector = Utils.Vector2Clamp(vector, this.minimumStickRaycastOriginAngle, this.maximumStickRaycastOriginAngle);
		this.StickRaycastOriginAngleInput.ServerValue = vector;
	}

	// Token: 0x06000196 RID: 406 RVA: 0x000090D4 File Offset: 0x000072D4
	[Rpc(SendTo.Server, DeferLocal = true, Delivery = RpcDelivery.Unreliable)]
	public void Client_LookAngleInputRpc(short x, short y, RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			Debug.LogError("Rpc methods can only be invoked after starting the NetworkManager!");
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 2301322626U;
			RpcAttribute.RpcAttributeParams attributeParams = new RpcAttribute.RpcAttributeParams
			{
				DeferLocal = true,
				Delivery = RpcDelivery.Unreliable
			};
			FastBufferWriter writer = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Unreliable);
			BytePacker.WriteValueBitPacked(writer, x);
			BytePacker.WriteValueBitPacked(writer, y);
			base.__endSendRpc(ref writer, 2301322626U, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Unreliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		if (rpcParams.Receive.SenderClientId != base.OwnerClientId && rpcParams.Receive.SenderClientId != 0UL)
		{
			return;
		}
		this.Server_LookAngleInputRpc(x, y, base.RpcTarget.Everyone);
	}

	// Token: 0x06000197 RID: 407 RVA: 0x00009200 File Offset: 0x00007400
	[Rpc(SendTo.SpecifiedInParams, InvokePermission = RpcInvokePermission.Server, DeferLocal = true, Delivery = RpcDelivery.Unreliable)]
	public void Server_LookAngleInputRpc(short x, short y, RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			Debug.LogError("Rpc methods can only be invoked after starting the NetworkManager!");
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 1047632353U;
			RpcAttribute.RpcAttributeParams attributeParams = new RpcAttribute.RpcAttributeParams
			{
				InvokePermission = RpcInvokePermission.Server,
				DeferLocal = true,
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
		Vector2 vector = new Vector2(NetworkingUtils.DecompressShortToFloat(x, this.minimumLookAngle.x, this.maximumLookAngle.x), NetworkingUtils.DecompressShortToFloat(y, this.minimumLookAngle.y, this.maximumLookAngle.y));
		vector = Utils.Vector2Clamp(vector, this.minimumLookAngle, this.maximumLookAngle);
		this.LookAngleInput.ServerValue = vector;
	}

	// Token: 0x06000198 RID: 408 RVA: 0x00009364 File Offset: 0x00007564
	[Rpc(SendTo.Server, DeferLocal = true)]
	public void Client_BladeAngleInputRpc(sbyte value, RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			Debug.LogError("Rpc methods can only be invoked after starting the NetworkManager!");
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 4018011136U;
			RpcAttribute.RpcAttributeParams attributeParams = new RpcAttribute.RpcAttributeParams
			{
				DeferLocal = true
			};
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
			fastBufferWriter.WriteValueSafe<sbyte>(value, default(FastBufferWriter.ForPrimitives));
			base.__endSendRpc(ref fastBufferWriter, 4018011136U, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		if (rpcParams.Receive.SenderClientId != base.OwnerClientId && rpcParams.Receive.SenderClientId != 0UL)
		{
			return;
		}
		this.Server_BladeAngleInputRpc(value, base.RpcTarget.Everyone);
	}

	// Token: 0x06000199 RID: 409 RVA: 0x00009484 File Offset: 0x00007684
	[Rpc(SendTo.SpecifiedInParams, InvokePermission = RpcInvokePermission.Server, DeferLocal = true)]
	public void Server_BladeAngleInputRpc(sbyte value, RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			Debug.LogError("Rpc methods can only be invoked after starting the NetworkManager!");
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 817646686U;
			RpcAttribute.RpcAttributeParams attributeParams = new RpcAttribute.RpcAttributeParams
			{
				InvokePermission = RpcInvokePermission.Server,
				DeferLocal = true
			};
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.SpecifiedInParams, RpcDelivery.Reliable);
			fastBufferWriter.WriteValueSafe<sbyte>(value, default(FastBufferWriter.ForPrimitives));
			base.__endSendRpc(ref fastBufferWriter, 817646686U, rpcParams, attributeParams, SendTo.SpecifiedInParams, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		this.BladeAngleInput.ServerValue = (sbyte)Mathf.Clamp((int)value, this.minimumBladeAngle, this.maximumBladeAngle);
	}

	// Token: 0x0600019A RID: 410 RVA: 0x00009598 File Offset: 0x00007798
	[Rpc(SendTo.Server, DeferLocal = true)]
	public void Client_SlideInputRpc(bool value, RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			Debug.LogError("Rpc methods can only be invoked after starting the NetworkManager!");
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 3775351339U;
			RpcAttribute.RpcAttributeParams attributeParams = new RpcAttribute.RpcAttributeParams
			{
				DeferLocal = true
			};
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
			fastBufferWriter.WriteValueSafe<bool>(value, default(FastBufferWriter.ForPrimitives));
			base.__endSendRpc(ref fastBufferWriter, 3775351339U, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		if (rpcParams.Receive.SenderClientId != base.OwnerClientId && rpcParams.Receive.SenderClientId != 0UL)
		{
			return;
		}
		this.Server_SlideInputRpc(value, base.RpcTarget.Everyone);
	}

	// Token: 0x0600019B RID: 411 RVA: 0x000096B8 File Offset: 0x000078B8
	[Rpc(SendTo.SpecifiedInParams, InvokePermission = RpcInvokePermission.Server, DeferLocal = true)]
	public void Server_SlideInputRpc(bool value, RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			Debug.LogError("Rpc methods can only be invoked after starting the NetworkManager!");
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 4107840079U;
			RpcAttribute.RpcAttributeParams attributeParams = new RpcAttribute.RpcAttributeParams
			{
				InvokePermission = RpcInvokePermission.Server,
				DeferLocal = true
			};
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

	// Token: 0x0600019C RID: 412 RVA: 0x000097B8 File Offset: 0x000079B8
	[Rpc(SendTo.Server, DeferLocal = true)]
	public void Client_SprintInputRpc(bool value, RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			Debug.LogError("Rpc methods can only be invoked after starting the NetworkManager!");
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 3297803930U;
			RpcAttribute.RpcAttributeParams attributeParams = new RpcAttribute.RpcAttributeParams
			{
				DeferLocal = true
			};
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
			fastBufferWriter.WriteValueSafe<bool>(value, default(FastBufferWriter.ForPrimitives));
			base.__endSendRpc(ref fastBufferWriter, 3297803930U, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		if (rpcParams.Receive.SenderClientId != base.OwnerClientId && rpcParams.Receive.SenderClientId != 0UL)
		{
			return;
		}
		this.Server_SprintInputRpc(value, base.RpcTarget.Everyone);
	}

	// Token: 0x0600019D RID: 413 RVA: 0x000098D8 File Offset: 0x00007AD8
	[Rpc(SendTo.SpecifiedInParams, InvokePermission = RpcInvokePermission.Server, DeferLocal = true)]
	public void Server_SprintInputRpc(bool value, RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			Debug.LogError("Rpc methods can only be invoked after starting the NetworkManager!");
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 778340344U;
			RpcAttribute.RpcAttributeParams attributeParams = new RpcAttribute.RpcAttributeParams
			{
				InvokePermission = RpcInvokePermission.Server,
				DeferLocal = true
			};
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

	// Token: 0x0600019E RID: 414 RVA: 0x000099D8 File Offset: 0x00007BD8
	[Rpc(SendTo.Server, DeferLocal = true)]
	public void Client_TrackInputRpc(bool value, RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			Debug.LogError("Rpc methods can only be invoked after starting the NetworkManager!");
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 3231418942U;
			RpcAttribute.RpcAttributeParams attributeParams = new RpcAttribute.RpcAttributeParams
			{
				DeferLocal = true
			};
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
			fastBufferWriter.WriteValueSafe<bool>(value, default(FastBufferWriter.ForPrimitives));
			base.__endSendRpc(ref fastBufferWriter, 3231418942U, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		if (rpcParams.Receive.SenderClientId != base.OwnerClientId && rpcParams.Receive.SenderClientId != 0UL)
		{
			return;
		}
		this.Server_TrackInputRpc(value, base.RpcTarget.Everyone);
	}

	// Token: 0x0600019F RID: 415 RVA: 0x00009AF8 File Offset: 0x00007CF8
	[Rpc(SendTo.SpecifiedInParams, InvokePermission = RpcInvokePermission.Server, DeferLocal = true)]
	public void Server_TrackInputRpc(bool value, RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			Debug.LogError("Rpc methods can only be invoked after starting the NetworkManager!");
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 2722698928U;
			RpcAttribute.RpcAttributeParams attributeParams = new RpcAttribute.RpcAttributeParams
			{
				InvokePermission = RpcInvokePermission.Server,
				DeferLocal = true
			};
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

	// Token: 0x060001A0 RID: 416 RVA: 0x00009BF8 File Offset: 0x00007DF8
	[Rpc(SendTo.Server, DeferLocal = true)]
	public void Client_LookInputRpc(bool value, RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			Debug.LogError("Rpc methods can only be invoked after starting the NetworkManager!");
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 894150284U;
			RpcAttribute.RpcAttributeParams attributeParams = new RpcAttribute.RpcAttributeParams
			{
				DeferLocal = true
			};
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
			fastBufferWriter.WriteValueSafe<bool>(value, default(FastBufferWriter.ForPrimitives));
			base.__endSendRpc(ref fastBufferWriter, 894150284U, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		if (rpcParams.Receive.SenderClientId != base.OwnerClientId && rpcParams.Receive.SenderClientId != 0UL)
		{
			return;
		}
		this.Server_LookInputRpc(value, base.RpcTarget.Everyone);
	}

	// Token: 0x060001A1 RID: 417 RVA: 0x00009D18 File Offset: 0x00007F18
	[Rpc(SendTo.SpecifiedInParams, InvokePermission = RpcInvokePermission.Server, DeferLocal = true)]
	public void Server_LookInputRpc(bool value, RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			Debug.LogError("Rpc methods can only be invoked after starting the NetworkManager!");
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 3779091983U;
			RpcAttribute.RpcAttributeParams attributeParams = new RpcAttribute.RpcAttributeParams
			{
				InvokePermission = RpcInvokePermission.Server,
				DeferLocal = true
			};
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

	// Token: 0x060001A2 RID: 418 RVA: 0x00009E18 File Offset: 0x00008018
	[Rpc(SendTo.Server, DeferLocal = true)]
	public void Client_JumpInputRpc(RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			Debug.LogError("Rpc methods can only be invoked after starting the NetworkManager!");
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 566720222U;
			RpcAttribute.RpcAttributeParams attributeParams = new RpcAttribute.RpcAttributeParams
			{
				DeferLocal = true
			};
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
			base.__endSendRpc(ref fastBufferWriter, 566720222U, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		if (rpcParams.Receive.SenderClientId != base.OwnerClientId && rpcParams.Receive.SenderClientId != 0UL)
		{
			return;
		}
		if (!this.JumpInput.ShouldChange)
		{
			return;
		}
		EventManager.TriggerEvent("Event_Server_OnPlayerJumpInput", new Dictionary<string, object>
		{
			{
				"player",
				this.Player
			}
		});
		this.JumpInput.ServerTick();
	}

	// Token: 0x060001A3 RID: 419 RVA: 0x00009F3C File Offset: 0x0000813C
	[Rpc(SendTo.Server, DeferLocal = true)]
	public void Client_StopInputRpc(bool value, RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			Debug.LogError("Rpc methods can only be invoked after starting the NetworkManager!");
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 212770831U;
			RpcAttribute.RpcAttributeParams attributeParams = new RpcAttribute.RpcAttributeParams
			{
				DeferLocal = true
			};
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
			fastBufferWriter.WriteValueSafe<bool>(value, default(FastBufferWriter.ForPrimitives));
			base.__endSendRpc(ref fastBufferWriter, 212770831U, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		if (rpcParams.Receive.SenderClientId != base.OwnerClientId && rpcParams.Receive.SenderClientId != 0UL)
		{
			return;
		}
		this.StopInput.ServerValue = value;
	}

	// Token: 0x060001A4 RID: 420 RVA: 0x0000A050 File Offset: 0x00008250
	[Rpc(SendTo.Server, DeferLocal = true)]
	public void Client_DashLeftInputRpc(RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			Debug.LogError("Rpc methods can only be invoked after starting the NetworkManager!");
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 1929006103U;
			RpcAttribute.RpcAttributeParams attributeParams = new RpcAttribute.RpcAttributeParams
			{
				DeferLocal = true
			};
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
			base.__endSendRpc(ref fastBufferWriter, 1929006103U, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		if (rpcParams.Receive.SenderClientId != base.OwnerClientId && rpcParams.Receive.SenderClientId != 0UL)
		{
			return;
		}
		if (!this.DashLeftInput.ShouldChange)
		{
			return;
		}
		EventManager.TriggerEvent("Event_Server_OnPlayerDashLeftInput", new Dictionary<string, object>
		{
			{
				"player",
				this.Player
			}
		});
		this.DashLeftInput.ServerTick();
	}

	// Token: 0x060001A5 RID: 421 RVA: 0x0000A174 File Offset: 0x00008374
	[Rpc(SendTo.Server, DeferLocal = true)]
	public void Client_DashRightInputRpc(RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			Debug.LogError("Rpc methods can only be invoked after starting the NetworkManager!");
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 3135613427U;
			RpcAttribute.RpcAttributeParams attributeParams = new RpcAttribute.RpcAttributeParams
			{
				DeferLocal = true
			};
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
			base.__endSendRpc(ref fastBufferWriter, 3135613427U, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		if (rpcParams.Receive.SenderClientId != base.OwnerClientId && rpcParams.Receive.SenderClientId != 0UL)
		{
			return;
		}
		if (!this.DashRightInput.ShouldChange)
		{
			return;
		}
		EventManager.TriggerEvent("Event_Server_OnPlayerDashRightInput", new Dictionary<string, object>
		{
			{
				"player",
				this.Player
			}
		});
		this.DashRightInput.ServerTick();
	}

	// Token: 0x060001A6 RID: 422 RVA: 0x0000A298 File Offset: 0x00008498
	[Rpc(SendTo.Server, DeferLocal = true)]
	public void Client_TwistLeftInputRpc(RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			Debug.LogError("Rpc methods can only be invoked after starting the NetworkManager!");
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 4104804754U;
			RpcAttribute.RpcAttributeParams attributeParams = new RpcAttribute.RpcAttributeParams
			{
				DeferLocal = true
			};
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
			base.__endSendRpc(ref fastBufferWriter, 4104804754U, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		if (rpcParams.Receive.SenderClientId != base.OwnerClientId && rpcParams.Receive.SenderClientId != 0UL)
		{
			return;
		}
		if (!this.TwistLeftInput.ShouldChange)
		{
			return;
		}
		EventManager.TriggerEvent("Event_Server_OnPlayerTwistLeftInput", new Dictionary<string, object>
		{
			{
				"player",
				this.Player
			}
		});
		this.TwistLeftInput.ServerTick();
	}

	// Token: 0x060001A7 RID: 423 RVA: 0x0000A3BC File Offset: 0x000085BC
	[Rpc(SendTo.Server, DeferLocal = true)]
	public void Client_TwistRightInputRpc(RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			Debug.LogError("Rpc methods can only be invoked after starting the NetworkManager!");
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 2735818857U;
			RpcAttribute.RpcAttributeParams attributeParams = new RpcAttribute.RpcAttributeParams
			{
				DeferLocal = true
			};
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
			base.__endSendRpc(ref fastBufferWriter, 2735818857U, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		if (rpcParams.Receive.SenderClientId != base.OwnerClientId && rpcParams.Receive.SenderClientId != 0UL)
		{
			return;
		}
		if (!this.TwistRightInput.ShouldChange)
		{
			return;
		}
		EventManager.TriggerEvent("Event_Server_OnPlayerTwistRightInput", new Dictionary<string, object>
		{
			{
				"player",
				this.Player
			}
		});
		this.TwistRightInput.ServerTick();
	}

	// Token: 0x060001A8 RID: 424 RVA: 0x0000A4E0 File Offset: 0x000086E0
	[Rpc(SendTo.Server, DeferLocal = true)]
	public void Client_ExtendLeftInputRpc(bool value, RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			Debug.LogError("Rpc methods can only be invoked after starting the NetworkManager!");
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 537498773U;
			RpcAttribute.RpcAttributeParams attributeParams = new RpcAttribute.RpcAttributeParams
			{
				DeferLocal = true
			};
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
			fastBufferWriter.WriteValueSafe<bool>(value, default(FastBufferWriter.ForPrimitives));
			base.__endSendRpc(ref fastBufferWriter, 537498773U, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		if (rpcParams.Receive.SenderClientId != base.OwnerClientId && rpcParams.Receive.SenderClientId != 0UL)
		{
			return;
		}
		this.Server_ExtendLeftInputRpc(value, base.RpcTarget.Everyone);
	}

	// Token: 0x060001A9 RID: 425 RVA: 0x0000A600 File Offset: 0x00008800
	[Rpc(SendTo.SpecifiedInParams, InvokePermission = RpcInvokePermission.Server, DeferLocal = true)]
	public void Server_ExtendLeftInputRpc(bool value, RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			Debug.LogError("Rpc methods can only be invoked after starting the NetworkManager!");
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 3288109408U;
			RpcAttribute.RpcAttributeParams attributeParams = new RpcAttribute.RpcAttributeParams
			{
				InvokePermission = RpcInvokePermission.Server,
				DeferLocal = true
			};
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

	// Token: 0x060001AA RID: 426 RVA: 0x0000A700 File Offset: 0x00008900
	[Rpc(SendTo.Server, DeferLocal = true)]
	public void Client_ExtendRightInputRpc(bool value, RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			Debug.LogError("Rpc methods can only be invoked after starting the NetworkManager!");
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 4044541524U;
			RpcAttribute.RpcAttributeParams attributeParams = new RpcAttribute.RpcAttributeParams
			{
				DeferLocal = true
			};
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
			fastBufferWriter.WriteValueSafe<bool>(value, default(FastBufferWriter.ForPrimitives));
			base.__endSendRpc(ref fastBufferWriter, 4044541524U, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		if (rpcParams.Receive.SenderClientId != base.OwnerClientId && rpcParams.Receive.SenderClientId != 0UL)
		{
			return;
		}
		this.Server_ExtendRightInputRpc(value, base.RpcTarget.Everyone);
	}

	// Token: 0x060001AB RID: 427 RVA: 0x0000A820 File Offset: 0x00008A20
	[Rpc(SendTo.SpecifiedInParams, InvokePermission = RpcInvokePermission.Server, DeferLocal = true)]
	public void Server_ExtendRightInputRpc(bool value, RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			Debug.LogError("Rpc methods can only be invoked after starting the NetworkManager!");
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 152722375U;
			RpcAttribute.RpcAttributeParams attributeParams = new RpcAttribute.RpcAttributeParams
			{
				InvokePermission = RpcInvokePermission.Server,
				DeferLocal = true
			};
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

	// Token: 0x060001AC RID: 428 RVA: 0x0000A920 File Offset: 0x00008B20
	[Rpc(SendTo.Server, DeferLocal = true)]
	public void Client_LateralLeftInputRpc(bool value, RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			Debug.LogError("Rpc methods can only be invoked after starting the NetworkManager!");
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 867760499U;
			RpcAttribute.RpcAttributeParams attributeParams = new RpcAttribute.RpcAttributeParams
			{
				DeferLocal = true
			};
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
			fastBufferWriter.WriteValueSafe<bool>(value, default(FastBufferWriter.ForPrimitives));
			base.__endSendRpc(ref fastBufferWriter, 867760499U, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		if (rpcParams.Receive.SenderClientId != base.OwnerClientId && rpcParams.Receive.SenderClientId != 0UL)
		{
			return;
		}
		this.Server_LateralLeftInputRpc(value, base.RpcTarget.Everyone);
	}

	// Token: 0x060001AD RID: 429 RVA: 0x0000AA40 File Offset: 0x00008C40
	[Rpc(SendTo.SpecifiedInParams, InvokePermission = RpcInvokePermission.Server, DeferLocal = true)]
	public void Server_LateralLeftInputRpc(bool value, RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			Debug.LogError("Rpc methods can only be invoked after starting the NetworkManager!");
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 1623507309U;
			RpcAttribute.RpcAttributeParams attributeParams = new RpcAttribute.RpcAttributeParams
			{
				InvokePermission = RpcInvokePermission.Server,
				DeferLocal = true
			};
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.SpecifiedInParams, RpcDelivery.Reliable);
			fastBufferWriter.WriteValueSafe<bool>(value, default(FastBufferWriter.ForPrimitives));
			base.__endSendRpc(ref fastBufferWriter, 1623507309U, rpcParams, attributeParams, SendTo.SpecifiedInParams, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		this.LateralLeftInput.ServerValue = value;
	}

	// Token: 0x060001AE RID: 430 RVA: 0x0000AB40 File Offset: 0x00008D40
	[Rpc(SendTo.Server, DeferLocal = true)]
	public void Client_LateralRightInputRpc(bool value, RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			Debug.LogError("Rpc methods can only be invoked after starting the NetworkManager!");
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 2139362476U;
			RpcAttribute.RpcAttributeParams attributeParams = new RpcAttribute.RpcAttributeParams
			{
				DeferLocal = true
			};
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
			fastBufferWriter.WriteValueSafe<bool>(value, default(FastBufferWriter.ForPrimitives));
			base.__endSendRpc(ref fastBufferWriter, 2139362476U, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		if (rpcParams.Receive.SenderClientId != base.OwnerClientId && rpcParams.Receive.SenderClientId != 0UL)
		{
			return;
		}
		this.Server_LateralRightInputRpc(value, base.RpcTarget.Everyone);
	}

	// Token: 0x060001AF RID: 431 RVA: 0x0000AC60 File Offset: 0x00008E60
	[Rpc(SendTo.SpecifiedInParams, InvokePermission = RpcInvokePermission.Server, DeferLocal = true)]
	public void Server_LateralRightInputRpc(bool value, RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			Debug.LogError("Rpc methods can only be invoked after starting the NetworkManager!");
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 3221925618U;
			RpcAttribute.RpcAttributeParams attributeParams = new RpcAttribute.RpcAttributeParams
			{
				InvokePermission = RpcInvokePermission.Server,
				DeferLocal = true
			};
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.SpecifiedInParams, RpcDelivery.Reliable);
			fastBufferWriter.WriteValueSafe<bool>(value, default(FastBufferWriter.ForPrimitives));
			base.__endSendRpc(ref fastBufferWriter, 3221925618U, rpcParams, attributeParams, SendTo.SpecifiedInParams, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		this.LateralRightInput.ServerValue = value;
	}

	// Token: 0x060001B0 RID: 432 RVA: 0x0000AD60 File Offset: 0x00008F60
	[Rpc(SendTo.Server, DeferLocal = true)]
	public void Client_TalkInputRpc(bool value, RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			Debug.LogError("Rpc methods can only be invoked after starting the NetworkManager!");
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 1563095812U;
			RpcAttribute.RpcAttributeParams attributeParams = new RpcAttribute.RpcAttributeParams
			{
				DeferLocal = true
			};
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
			fastBufferWriter.WriteValueSafe<bool>(value, default(FastBufferWriter.ForPrimitives));
			base.__endSendRpc(ref fastBufferWriter, 1563095812U, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		if (rpcParams.Receive.SenderClientId != base.OwnerClientId && rpcParams.Receive.SenderClientId != 0UL)
		{
			return;
		}
		this.Server_TalkInputRpc(value, base.RpcTarget.Everyone);
	}

	// Token: 0x060001B1 RID: 433 RVA: 0x0000AE80 File Offset: 0x00009080
	[Rpc(SendTo.SpecifiedInParams, InvokePermission = RpcInvokePermission.Server, DeferLocal = true)]
	public void Server_TalkInputRpc(bool value, RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			Debug.LogError("Rpc methods can only be invoked after starting the NetworkManager!");
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 3713736028U;
			RpcAttribute.RpcAttributeParams attributeParams = new RpcAttribute.RpcAttributeParams
			{
				InvokePermission = RpcInvokePermission.Server,
				DeferLocal = true
			};
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
		EventManager.TriggerEvent("Event_Everyone_OnPlayerTalkInput", new Dictionary<string, object>
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

	// Token: 0x060001B2 RID: 434 RVA: 0x0000AFB0 File Offset: 0x000091B0
	public void Server_ForceSynchronizeClientId(ulong clientId)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.Server_MoveInputRpc(NetworkingUtils.CompressFloatToShort(this.MoveInput.ServerValue.x, -1f, 1f), NetworkingUtils.CompressFloatToShort(this.MoveInput.ServerValue.y, -1f, 1f), base.RpcTarget.Single(clientId, RpcTargetUse.Persistent));
		this.Server_RaycastOriginAngleInputRpc(NetworkingUtils.CompressFloatToShort(this.StickRaycastOriginAngleInput.ServerValue.x, this.minimumStickRaycastOriginAngle.x, this.maximumStickRaycastOriginAngle.x), NetworkingUtils.CompressFloatToShort(this.StickRaycastOriginAngleInput.ServerValue.y, this.minimumStickRaycastOriginAngle.y, this.maximumStickRaycastOriginAngle.y), base.RpcTarget.Single(clientId, RpcTargetUse.Persistent));
		this.Server_LookAngleInputRpc(NetworkingUtils.CompressFloatToShort(this.LookAngleInput.ServerValue.x, this.minimumLookAngle.x, this.maximumLookAngle.x), NetworkingUtils.CompressFloatToShort(this.LookAngleInput.ServerValue.y, this.minimumLookAngle.y, this.maximumLookAngle.y), base.RpcTarget.Single(clientId, RpcTargetUse.Persistent));
		this.Server_BladeAngleInputRpc(this.BladeAngleInput.ServerValue, base.RpcTarget.Single(clientId, RpcTargetUse.Persistent));
		this.Server_SlideInputRpc(this.SlideInput.ServerValue, base.RpcTarget.Single(clientId, RpcTargetUse.Persistent));
		this.Server_SprintInputRpc(this.SprintInput.ServerValue, base.RpcTarget.Single(clientId, RpcTargetUse.Persistent));
		this.Server_TrackInputRpc(this.TrackInput.ServerValue, base.RpcTarget.Single(clientId, RpcTargetUse.Persistent));
		this.Server_LookInputRpc(this.LookInput.ServerValue, base.RpcTarget.Single(clientId, RpcTargetUse.Persistent));
		this.Server_ExtendLeftInputRpc(this.ExtendLeftInput.ServerValue, base.RpcTarget.Single(clientId, RpcTargetUse.Persistent));
		this.Server_ExtendRightInputRpc(this.ExtendRightInput.ServerValue, base.RpcTarget.Single(clientId, RpcTargetUse.Persistent));
		this.Server_LateralLeftInputRpc(this.LateralLeftInput.ServerValue, base.RpcTarget.Single(clientId, RpcTargetUse.Persistent));
		this.Server_LateralRightInputRpc(this.LateralRightInput.ServerValue, base.RpcTarget.Single(clientId, RpcTargetUse.Persistent));
		this.Server_TalkInputRpc(this.TalkInput.ServerValue, base.RpcTarget.Single(clientId, RpcTargetUse.Persistent));
	}

	// Token: 0x060001B4 RID: 436 RVA: 0x0000B500 File Offset: 0x00009700
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x060001B5 RID: 437 RVA: 0x0000B518 File Offset: 0x00009718
	protected override void __initializeRpcs()
	{
		base.__registerRpc(2880114289U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_2880114289), "Client_MoveInputRpc", RpcInvokePermission.Everyone);
		base.__registerRpc(2371540155U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_2371540155), "Server_MoveInputRpc", RpcInvokePermission.Server);
		base.__registerRpc(4145643342U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_4145643342), "Client_RaycastOriginAngleInputRpc", RpcInvokePermission.Everyone);
		base.__registerRpc(3031376689U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_3031376689), "Server_RaycastOriginAngleInputRpc", RpcInvokePermission.Server);
		base.__registerRpc(2301322626U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_2301322626), "Client_LookAngleInputRpc", RpcInvokePermission.Everyone);
		base.__registerRpc(1047632353U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_1047632353), "Server_LookAngleInputRpc", RpcInvokePermission.Server);
		base.__registerRpc(4018011136U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_4018011136), "Client_BladeAngleInputRpc", RpcInvokePermission.Everyone);
		base.__registerRpc(817646686U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_817646686), "Server_BladeAngleInputRpc", RpcInvokePermission.Server);
		base.__registerRpc(3775351339U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_3775351339), "Client_SlideInputRpc", RpcInvokePermission.Everyone);
		base.__registerRpc(4107840079U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_4107840079), "Server_SlideInputRpc", RpcInvokePermission.Server);
		base.__registerRpc(3297803930U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_3297803930), "Client_SprintInputRpc", RpcInvokePermission.Everyone);
		base.__registerRpc(778340344U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_778340344), "Server_SprintInputRpc", RpcInvokePermission.Server);
		base.__registerRpc(3231418942U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_3231418942), "Client_TrackInputRpc", RpcInvokePermission.Everyone);
		base.__registerRpc(2722698928U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_2722698928), "Server_TrackInputRpc", RpcInvokePermission.Server);
		base.__registerRpc(894150284U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_894150284), "Client_LookInputRpc", RpcInvokePermission.Everyone);
		base.__registerRpc(3779091983U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_3779091983), "Server_LookInputRpc", RpcInvokePermission.Server);
		base.__registerRpc(566720222U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_566720222), "Client_JumpInputRpc", RpcInvokePermission.Everyone);
		base.__registerRpc(212770831U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_212770831), "Client_StopInputRpc", RpcInvokePermission.Everyone);
		base.__registerRpc(1929006103U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_1929006103), "Client_DashLeftInputRpc", RpcInvokePermission.Everyone);
		base.__registerRpc(3135613427U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_3135613427), "Client_DashRightInputRpc", RpcInvokePermission.Everyone);
		base.__registerRpc(4104804754U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_4104804754), "Client_TwistLeftInputRpc", RpcInvokePermission.Everyone);
		base.__registerRpc(2735818857U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_2735818857), "Client_TwistRightInputRpc", RpcInvokePermission.Everyone);
		base.__registerRpc(537498773U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_537498773), "Client_ExtendLeftInputRpc", RpcInvokePermission.Everyone);
		base.__registerRpc(3288109408U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_3288109408), "Server_ExtendLeftInputRpc", RpcInvokePermission.Server);
		base.__registerRpc(4044541524U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_4044541524), "Client_ExtendRightInputRpc", RpcInvokePermission.Everyone);
		base.__registerRpc(152722375U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_152722375), "Server_ExtendRightInputRpc", RpcInvokePermission.Server);
		base.__registerRpc(867760499U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_867760499), "Client_LateralLeftInputRpc", RpcInvokePermission.Everyone);
		base.__registerRpc(1623507309U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_1623507309), "Server_LateralLeftInputRpc", RpcInvokePermission.Server);
		base.__registerRpc(2139362476U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_2139362476), "Client_LateralRightInputRpc", RpcInvokePermission.Everyone);
		base.__registerRpc(3221925618U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_3221925618), "Server_LateralRightInputRpc", RpcInvokePermission.Server);
		base.__registerRpc(1563095812U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_1563095812), "Client_TalkInputRpc", RpcInvokePermission.Everyone);
		base.__registerRpc(3713736028U, new NetworkBehaviour.RpcReceiveHandler(global::PlayerInput.__rpc_handler_3713736028), "Server_TalkInputRpc", RpcInvokePermission.Server);
		base.__initializeRpcs();
	}

	// Token: 0x060001B6 RID: 438 RVA: 0x0000B950 File Offset: 0x00009B50
	private static void __rpc_handler_2880114289(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
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
		((global::PlayerInput)target).Client_MoveInputRpc(x, y, ext);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x060001B7 RID: 439 RVA: 0x0000B9D4 File Offset: 0x00009BD4
	private static void __rpc_handler_2371540155(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
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
		((global::PlayerInput)target).Server_MoveInputRpc(x, y, ext);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x060001B8 RID: 440 RVA: 0x0000BA58 File Offset: 0x00009C58
	private static void __rpc_handler_4145643342(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
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
		((global::PlayerInput)target).Client_RaycastOriginAngleInputRpc(x, y, ext);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x060001B9 RID: 441 RVA: 0x0000BADC File Offset: 0x00009CDC
	private static void __rpc_handler_3031376689(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
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
		((global::PlayerInput)target).Server_RaycastOriginAngleInputRpc(x, y, ext);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x060001BA RID: 442 RVA: 0x0000BB60 File Offset: 0x00009D60
	private static void __rpc_handler_2301322626(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
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
		((global::PlayerInput)target).Client_LookAngleInputRpc(x, y, ext);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x060001BB RID: 443 RVA: 0x0000BBE4 File Offset: 0x00009DE4
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

	// Token: 0x060001BC RID: 444 RVA: 0x0000BC68 File Offset: 0x00009E68
	private static void __rpc_handler_4018011136(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
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
		((global::PlayerInput)target).Client_BladeAngleInputRpc(value, ext);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x060001BD RID: 445 RVA: 0x0000BCE8 File Offset: 0x00009EE8
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

	// Token: 0x060001BE RID: 446 RVA: 0x0000BD68 File Offset: 0x00009F68
	private static void __rpc_handler_3775351339(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
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
		((global::PlayerInput)target).Client_SlideInputRpc(value, ext);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x060001BF RID: 447 RVA: 0x0000BDE8 File Offset: 0x00009FE8
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

	// Token: 0x060001C0 RID: 448 RVA: 0x0000BE68 File Offset: 0x0000A068
	private static void __rpc_handler_3297803930(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
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
		((global::PlayerInput)target).Client_SprintInputRpc(value, ext);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x060001C1 RID: 449 RVA: 0x0000BEE8 File Offset: 0x0000A0E8
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

	// Token: 0x060001C2 RID: 450 RVA: 0x0000BF68 File Offset: 0x0000A168
	private static void __rpc_handler_3231418942(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
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
		((global::PlayerInput)target).Client_TrackInputRpc(value, ext);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x060001C3 RID: 451 RVA: 0x0000BFE8 File Offset: 0x0000A1E8
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

	// Token: 0x060001C4 RID: 452 RVA: 0x0000C068 File Offset: 0x0000A268
	private static void __rpc_handler_894150284(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
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
		((global::PlayerInput)target).Client_LookInputRpc(value, ext);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x060001C5 RID: 453 RVA: 0x0000C0E8 File Offset: 0x0000A2E8
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

	// Token: 0x060001C6 RID: 454 RVA: 0x0000C168 File Offset: 0x0000A368
	private static void __rpc_handler_566720222(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		RpcParams ext = rpcParams.Ext;
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((global::PlayerInput)target).Client_JumpInputRpc(ext);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x060001C7 RID: 455 RVA: 0x0000C1C8 File Offset: 0x0000A3C8
	private static void __rpc_handler_212770831(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
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
		((global::PlayerInput)target).Client_StopInputRpc(value, ext);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x060001C8 RID: 456 RVA: 0x0000C248 File Offset: 0x0000A448
	private static void __rpc_handler_1929006103(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		RpcParams ext = rpcParams.Ext;
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((global::PlayerInput)target).Client_DashLeftInputRpc(ext);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x060001C9 RID: 457 RVA: 0x0000C2A8 File Offset: 0x0000A4A8
	private static void __rpc_handler_3135613427(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		RpcParams ext = rpcParams.Ext;
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((global::PlayerInput)target).Client_DashRightInputRpc(ext);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x060001CA RID: 458 RVA: 0x0000C308 File Offset: 0x0000A508
	private static void __rpc_handler_4104804754(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		RpcParams ext = rpcParams.Ext;
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((global::PlayerInput)target).Client_TwistLeftInputRpc(ext);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x060001CB RID: 459 RVA: 0x0000C368 File Offset: 0x0000A568
	private static void __rpc_handler_2735818857(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		RpcParams ext = rpcParams.Ext;
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((global::PlayerInput)target).Client_TwistRightInputRpc(ext);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x060001CC RID: 460 RVA: 0x0000C3C8 File Offset: 0x0000A5C8
	private static void __rpc_handler_537498773(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
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
		((global::PlayerInput)target).Client_ExtendLeftInputRpc(value, ext);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x060001CD RID: 461 RVA: 0x0000C448 File Offset: 0x0000A648
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

	// Token: 0x060001CE RID: 462 RVA: 0x0000C4C8 File Offset: 0x0000A6C8
	private static void __rpc_handler_4044541524(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
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
		((global::PlayerInput)target).Client_ExtendRightInputRpc(value, ext);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x060001CF RID: 463 RVA: 0x0000C548 File Offset: 0x0000A748
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

	// Token: 0x060001D0 RID: 464 RVA: 0x0000C5C8 File Offset: 0x0000A7C8
	private static void __rpc_handler_867760499(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
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
		((global::PlayerInput)target).Client_LateralLeftInputRpc(value, ext);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x060001D1 RID: 465 RVA: 0x0000C648 File Offset: 0x0000A848
	private static void __rpc_handler_1623507309(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
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
		((global::PlayerInput)target).Server_LateralLeftInputRpc(value, ext);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x060001D2 RID: 466 RVA: 0x0000C6C8 File Offset: 0x0000A8C8
	private static void __rpc_handler_2139362476(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
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
		((global::PlayerInput)target).Client_LateralRightInputRpc(value, ext);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x060001D3 RID: 467 RVA: 0x0000C748 File Offset: 0x0000A948
	private static void __rpc_handler_3221925618(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
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
		((global::PlayerInput)target).Server_LateralRightInputRpc(value, ext);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x060001D4 RID: 468 RVA: 0x0000C7C8 File Offset: 0x0000A9C8
	private static void __rpc_handler_1563095812(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
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
		((global::PlayerInput)target).Client_TalkInputRpc(value, ext);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x060001D5 RID: 469 RVA: 0x0000C848 File Offset: 0x0000AA48
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

	// Token: 0x060001D6 RID: 470 RVA: 0x0000C8C6 File Offset: 0x0000AAC6
	protected internal override string __getTypeName()
	{
		return "PlayerInput";
	}

	// Token: 0x0400011E RID: 286
	[Header("Settings")]
	[SerializeField]
	private Vector3 initialLookAngle = new Vector3(30f, 0f, 0f);

	// Token: 0x0400011F RID: 287
	[Space(20f)]
	[SerializeField]
	private Vector2 initialStickRaycastOriginAngle = new Vector2(40f, 80f);

	// Token: 0x04000120 RID: 288
	[SerializeField]
	private Vector2 minimumStickRaycastOriginAngle = new Vector2(-25f, -92.5f);

	// Token: 0x04000121 RID: 289
	[SerializeField]
	private Vector2 maximumStickRaycastOriginAngle = new Vector2(80f, 92.5f);

	// Token: 0x04000122 RID: 290
	[Space(20f)]
	[SerializeField]
	private Vector2 minimumLookAngle = new Vector2(-25f, -135f);

	// Token: 0x04000123 RID: 291
	[SerializeField]
	private Vector2 maximumLookAngle = new Vector2(75f, 135f);

	// Token: 0x04000124 RID: 292
	[Space(20f)]
	[SerializeField]
	private int minimumBladeAngle = -4;

	// Token: 0x04000125 RID: 293
	[SerializeField]
	private int maximumBladeAngle = 4;

	// Token: 0x04000126 RID: 294
	public NetworkedInput<Vector2> MoveInput = new NetworkedInput<Vector2>(default(Vector2), null, null);

	// Token: 0x04000127 RID: 295
	public NetworkedInput<Vector2> StickRaycastOriginAngleInput = new NetworkedInput<Vector2>(default(Vector2), (Vector2 lastSentValue, Vector2 clientValue) => Vector2.Distance(lastSentValue, clientValue) > 0.1f, null);

	// Token: 0x04000128 RID: 296
	public NetworkedInput<Vector2> LookAngleInput = new NetworkedInput<Vector2>(default(Vector2), (Vector2 lastSentValue, Vector2 clientValue) => Vector2.Distance(lastSentValue, clientValue) > 0.1f, null);

	// Token: 0x04000129 RID: 297
	public NetworkedInput<sbyte> BladeAngleInput = new NetworkedInput<sbyte>(0, null, null);

	// Token: 0x0400012A RID: 298
	public NetworkedInput<bool> SlideInput = new NetworkedInput<bool>(false, null, null);

	// Token: 0x0400012B RID: 299
	public NetworkedInput<bool> SprintInput = new NetworkedInput<bool>(false, null, null);

	// Token: 0x0400012C RID: 300
	public NetworkedInput<bool> TrackInput = new NetworkedInput<bool>(false, null, null);

	// Token: 0x0400012D RID: 301
	public NetworkedInput<bool> LookInput = new NetworkedInput<bool>(false, null, null);

	// Token: 0x0400012E RID: 302
	public NetworkedInput<byte> JumpInput = new NetworkedInput<byte>(0, null, (byte lastReceivedValue, double lastReceivedTime, byte serverValue) => Time.timeAsDouble - lastReceivedTime > 0.5);

	// Token: 0x0400012F RID: 303
	public NetworkedInput<bool> StopInput = new NetworkedInput<bool>(false, null, null);

	// Token: 0x04000130 RID: 304
	public NetworkedInput<byte> TwistLeftInput = new NetworkedInput<byte>(0, null, (byte lastReceivedValue, double lastReceivedTime, byte serverValue) => Time.timeAsDouble - lastReceivedTime > 0.5);

	// Token: 0x04000131 RID: 305
	public NetworkedInput<byte> TwistRightInput = new NetworkedInput<byte>(0, null, (byte lastReceivedValue, double lastReceivedTime, byte serverValue) => Time.timeAsDouble - lastReceivedTime > 0.5);

	// Token: 0x04000132 RID: 306
	public NetworkedInput<byte> DashLeftInput = new NetworkedInput<byte>(0, null, (byte lastReceivedValue, double lastReceivedTime, byte serverValue) => Time.timeAsDouble - lastReceivedTime > 0.25);

	// Token: 0x04000133 RID: 307
	public NetworkedInput<byte> DashRightInput = new NetworkedInput<byte>(0, null, (byte lastReceivedValue, double lastReceivedTime, byte serverValue) => Time.timeAsDouble - lastReceivedTime > 0.25);

	// Token: 0x04000134 RID: 308
	public NetworkedInput<bool> ExtendLeftInput = new NetworkedInput<bool>(false, null, null);

	// Token: 0x04000135 RID: 309
	public NetworkedInput<bool> ExtendRightInput = new NetworkedInput<bool>(false, null, null);

	// Token: 0x04000136 RID: 310
	public NetworkedInput<bool> LateralLeftInput = new NetworkedInput<bool>(false, null, null);

	// Token: 0x04000137 RID: 311
	public NetworkedInput<bool> LateralRightInput = new NetworkedInput<bool>(false, null, null);

	// Token: 0x04000138 RID: 312
	public NetworkedInput<bool> TalkInput = new NetworkedInput<bool>(false, null, null);

	// Token: 0x04000139 RID: 313
	[HideInInspector]
	public Player Player;

	// Token: 0x0400013A RID: 314
	[HideInInspector]
	public int TickRate = 200;

	// Token: 0x0400013B RID: 315
	private float bladeAngleBuffer;

	// Token: 0x0400013C RID: 316
	private bool shouldUpdateInputs;

	// Token: 0x0400013D RID: 317
	private bool shouldTickInputs;

	// Token: 0x0400013E RID: 318
	private float tickAccumulator;
}
