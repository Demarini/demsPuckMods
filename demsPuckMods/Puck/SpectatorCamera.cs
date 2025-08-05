using System;
using Unity.Netcode;
using UnityEngine;

// Token: 0x0200002C RID: 44
public class SpectatorCamera : BaseCamera
{
	// Token: 0x06000137 RID: 311 RVA: 0x000120C8 File Offset: 0x000102C8
	protected override void OnNetworkPostSpawn()
	{
		NetworkObject networkObject;
		if (this.PlayerReference.Value.TryGet(out networkObject, null))
		{
			this.Player = networkObject.GetComponent<Player>();
		}
		if (this.Player)
		{
			this.Player.SpectatorCamera = this;
		}
		if (this.Player && this.Player.IsLocalPlayer)
		{
			this.Enable();
		}
		base.OnNetworkPostSpawn();
	}

	// Token: 0x06000138 RID: 312 RVA: 0x00012138 File Offset: 0x00010338
	public override void OnTick(float deltaTime)
	{
		base.OnTick(deltaTime);
		if (!base.IsOwner)
		{
			return;
		}
		if (NetworkBehaviourSingleton<UIManager>.Instance.isMouseActive)
		{
			return;
		}
		Vector3 vector = new Vector3((float)((MonoBehaviourSingleton<InputManager>.Instance.TurnRightAction.IsPressed() ? 1 : 0) + (MonoBehaviourSingleton<InputManager>.Instance.TurnLeftAction.IsPressed() ? -1 : 0)), (float)((MonoBehaviourSingleton<InputManager>.Instance.MoveForwardAction.IsPressed() ? 1 : 0) + (MonoBehaviourSingleton<InputManager>.Instance.MoveBackwardAction.IsPressed() ? -1 : 0)), (float)(MonoBehaviourSingleton<InputManager>.Instance.JumpAction.IsPressed() ? 1 : (MonoBehaviourSingleton<InputManager>.Instance.SlideAction.IsPressed() ? -1 : 0)));
		float d = MonoBehaviourSingleton<InputManager>.Instance.SprintAction.IsPressed() ? (this.freeLookMovementSpeed * 2f) : this.freeLookMovementSpeed;
		this.freeLookPosition += base.transform.right * vector.x * deltaTime * d;
		this.freeLookPosition += base.transform.forward * vector.y * deltaTime * d;
		this.freeLookPosition += base.transform.up * vector.z * deltaTime * d;
		Vector2 vector2 = MonoBehaviourSingleton<InputManager>.Instance.StickAction.ReadValue<Vector2>();
		float lookSensitivity = MonoBehaviourSingleton<SettingsManager>.Instance.LookSensitivity;
		this.freeLookAngle += new Vector3(-vector2.y * lookSensitivity, vector2.x * lookSensitivity, -this.freeLookRotation.eulerAngles.z);
		this.freeLookAngle.x = Mathf.Clamp(this.freeLookAngle.x, -80f, 80f);
		this.freeLookRotation = Quaternion.Euler(Utils.WrapEulerAngles(this.freeLookAngle));
		base.transform.position = Vector3.Lerp(base.transform.position, this.freeLookPosition, deltaTime / this.freeLookPositionSmoothing);
		base.transform.rotation = Quaternion.Lerp(base.transform.rotation, this.freeLookRotation, deltaTime / this.freeLookRotationSmoothing);
	}

	// Token: 0x0600013A RID: 314 RVA: 0x00012404 File Offset: 0x00010604
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

	// Token: 0x0600013B RID: 315 RVA: 0x00007917 File Offset: 0x00005B17
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x0600013C RID: 316 RVA: 0x00007A96 File Offset: 0x00005C96
	protected internal override string __getTypeName()
	{
		return "SpectatorCamera";
	}

	// Token: 0x040000A3 RID: 163
	[Header("Settings")]
	[SerializeField]
	private float freeLookMovementSpeed = 5f;

	// Token: 0x040000A4 RID: 164
	[SerializeField]
	private float freeLookPositionSmoothing = 1f;

	// Token: 0x040000A5 RID: 165
	[SerializeField]
	private float freeLookRotationSmoothing = 1f;

	// Token: 0x040000A6 RID: 166
	private Vector3 freeLookPosition = Vector3.up * 2f;

	// Token: 0x040000A7 RID: 167
	private Vector3 freeLookAngle = Vector3.zero;

	// Token: 0x040000A8 RID: 168
	private Quaternion freeLookRotation = Quaternion.identity;

	// Token: 0x040000A9 RID: 169
	[HideInInspector]
	public NetworkVariable<NetworkObjectReference> PlayerReference = new NetworkVariable<NetworkObjectReference>(default(NetworkObjectReference), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x040000AA RID: 170
	[HideInInspector]
	public Player Player;
}
