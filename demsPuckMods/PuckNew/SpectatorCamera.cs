using System;
using Unity.Netcode;
using UnityEngine;

// Token: 0x02000066 RID: 102
public class SpectatorCamera : BaseCamera
{
	// Token: 0x17000068 RID: 104
	// (get) Token: 0x0600035F RID: 863 RVA: 0x00014004 File Offset: 0x00012204
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

	// Token: 0x06000360 RID: 864 RVA: 0x00014034 File Offset: 0x00012234
	protected override void OnNetworkPreSpawn(ref NetworkManager networkManager)
	{
		this.InitializeNetworkVariables(default(NetworkObjectReference));
		base.OnNetworkPreSpawn(ref networkManager);
	}

	// Token: 0x06000361 RID: 865 RVA: 0x00014057 File Offset: 0x00012257
	public override void OnNetworkSpawn()
	{
		NetworkVariable<NetworkObjectReference> playerReference = this.PlayerReference;
		playerReference.OnValueChanged = (NetworkVariable<NetworkObjectReference>.OnValueChangedDelegate)Delegate.Combine(playerReference.OnValueChanged, new NetworkVariable<NetworkObjectReference>.OnValueChangedDelegate(this.OnPlayerReferenceChanged));
		base.OnNetworkSpawn();
	}

	// Token: 0x06000362 RID: 866 RVA: 0x00014088 File Offset: 0x00012288
	protected override void OnNetworkPostSpawn()
	{
		if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsConnectedClient)
		{
			this.ProcessInitialNetworkVariableValues();
		}
		this.position = base.transform.position;
		this.pitch = base.transform.eulerAngles.x;
		this.yaw = base.transform.eulerAngles.y;
		this.targetPitch = this.pitch;
		this.targetYaw = this.yaw;
		base.OnNetworkPostSpawn();
	}

	// Token: 0x06000363 RID: 867 RVA: 0x0001410E File Offset: 0x0001230E
	protected override void OnNetworkSessionSynchronized()
	{
		this.ProcessInitialNetworkVariableValues();
		base.OnNetworkSessionSynchronized();
	}

	// Token: 0x06000364 RID: 868 RVA: 0x0001411C File Offset: 0x0001231C
	public override void OnNetworkDespawn()
	{
		NetworkVariable<NetworkObjectReference> playerReference = this.PlayerReference;
		playerReference.OnValueChanged = (NetworkVariable<NetworkObjectReference>.OnValueChangedDelegate)Delegate.Remove(playerReference.OnValueChanged, new NetworkVariable<NetworkObjectReference>.OnValueChangedDelegate(this.OnPlayerReferenceChanged));
		base.OnNetworkDespawn();
	}

	// Token: 0x06000365 RID: 869 RVA: 0x0001414B File Offset: 0x0001234B
	public void InitializeNetworkVariables(NetworkObjectReference playerReference = default(NetworkObjectReference))
	{
		if (this.isNetworkVariablesInitialized)
		{
			return;
		}
		this.isNetworkVariablesInitialized = true;
		this.PlayerReference = new NetworkVariable<NetworkObjectReference>(playerReference, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
	}

	// Token: 0x06000366 RID: 870 RVA: 0x0001416C File Offset: 0x0001236C
	private void ProcessInitialNetworkVariableValues()
	{
		this.OnPlayerReferenceChanged(default(NetworkObjectReference), this.PlayerReference.Value);
	}

	// Token: 0x06000367 RID: 871 RVA: 0x00014194 File Offset: 0x00012394
	public override void OnTick(float deltaTime)
	{
		base.OnTick(deltaTime);
		if (!base.IsOwner)
		{
			return;
		}
		float d = (float)((InputManager.TurnRightAction.IsPressed() ? 1 : 0) + (InputManager.TurnLeftAction.IsPressed() ? -1 : 0));
		float d2 = (float)(InputManager.JumpAction.IsPressed() ? 1 : (InputManager.SlideAction.IsPressed() ? -1 : 0));
		float d3 = (float)((InputManager.MoveForwardAction.IsPressed() ? 1 : 0) + (InputManager.MoveBackwardAction.IsPressed() ? -1 : 0));
		bool flag = InputManager.SprintAction.IsPressed();
		Vector2 vector = InputManager.StickAction.ReadValue<Vector2>();
		if (GlobalStateManager.UIState.IsMouseRequired)
		{
			d = 0f;
			d2 = 0f;
			d3 = 0f;
			flag = false;
			vector = Vector2.zero;
		}
		float d4 = flag ? (this.movementSpeed * 2f) : this.movementSpeed;
		this.position += base.transform.right * d * d4 * deltaTime;
		this.position += base.transform.up * d2 * d4 * deltaTime;
		this.position += base.transform.forward * d3 * d4 * deltaTime;
		base.transform.position = Vector3.SmoothDamp(base.transform.position, this.position, ref this.positionVelocity, this.positionSmoothTime, float.PositiveInfinity, deltaTime);
		this.targetPitch -= vector.y * SettingsManager.LookSensitivity;
		this.targetYaw += vector.x * SettingsManager.LookSensitivity;
		this.targetPitch = Mathf.Clamp(this.targetPitch, this.pitchMin, this.pitchMax);
		this.pitch = Mathf.Lerp(this.pitch, this.targetPitch, this.lookSmoothing * deltaTime);
		this.yaw = Mathf.Lerp(this.yaw, this.targetYaw, this.lookSmoothing * deltaTime);
		base.transform.rotation = Quaternion.Euler(this.pitch, this.yaw, 0f);
	}

	// Token: 0x06000368 RID: 872 RVA: 0x000143DF File Offset: 0x000125DF
	private void OnPlayerReferenceChanged(NetworkObjectReference previousValue, NetworkObjectReference newValue)
	{
		if (!this.Player)
		{
			return;
		}
		this.Player.SpectatorCamera = this;
	}

	// Token: 0x0600036A RID: 874 RVA: 0x0001445C File Offset: 0x0001265C
	protected override void __initializeVariables()
	{
		bool flag = this.PlayerReference == null;
		if (flag)
		{
			throw new Exception("SpectatorCamera.PlayerReference cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.PlayerReference.Initialize(this);
		base.__nameNetworkVariable(this.PlayerReference, "PlayerReference");
		this.NetworkVariableFields.Add(this.PlayerReference);
		base.__initializeVariables();
	}

	// Token: 0x0600036B RID: 875 RVA: 0x00002D76 File Offset: 0x00000F76
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x0600036C RID: 876 RVA: 0x000144BF File Offset: 0x000126BF
	protected internal override string __getTypeName()
	{
		return "SpectatorCamera";
	}

	// Token: 0x0400025A RID: 602
	[Header("Movement Settings")]
	[SerializeField]
	private float movementSpeed = 5f;

	// Token: 0x0400025B RID: 603
	[SerializeField]
	private float positionSmoothTime = 0.25f;

	// Token: 0x0400025C RID: 604
	[SerializeField]
	private float lookSmoothing = 10f;

	// Token: 0x0400025D RID: 605
	[SerializeField]
	private float pitchMin = -89f;

	// Token: 0x0400025E RID: 606
	[SerializeField]
	private float pitchMax = 89f;

	// Token: 0x0400025F RID: 607
	[HideInInspector]
	public NetworkVariable<NetworkObjectReference> PlayerReference;

	// Token: 0x04000260 RID: 608
	private Vector3 position = Vector3.zero;

	// Token: 0x04000261 RID: 609
	private Vector3 positionVelocity = Vector3.zero;

	// Token: 0x04000262 RID: 610
	private float pitch;

	// Token: 0x04000263 RID: 611
	private float yaw;

	// Token: 0x04000264 RID: 612
	private float targetPitch;

	// Token: 0x04000265 RID: 613
	private float targetYaw;

	// Token: 0x04000266 RID: 614
	private bool isNetworkVariablesInitialized;
}
