using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Token: 0x02000039 RID: 57
public class PlayerCamera : BaseCamera
{
	// Token: 0x17000022 RID: 34
	// (get) Token: 0x06000168 RID: 360 RVA: 0x00007C78 File Offset: 0x00005E78
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

	// Token: 0x17000023 RID: 35
	// (get) Token: 0x06000169 RID: 361 RVA: 0x00007CA5 File Offset: 0x00005EA5
	[HideInInspector]
	public PlayerBody PlayerBody
	{
		get
		{
			return this.Player.PlayerBody;
		}
	}

	// Token: 0x0600016A RID: 362 RVA: 0x00007CB4 File Offset: 0x00005EB4
	protected override void OnNetworkPreSpawn(ref NetworkManager networkManager)
	{
		this.InitializeNetworkVariables(default(NetworkObjectReference));
		base.OnNetworkPreSpawn(ref networkManager);
	}

	// Token: 0x0600016B RID: 363 RVA: 0x00007CD7 File Offset: 0x00005ED7
	public override void OnNetworkSpawn()
	{
		NetworkVariable<NetworkObjectReference> playerReference = this.PlayerReference;
		playerReference.OnValueChanged = (NetworkVariable<NetworkObjectReference>.OnValueChangedDelegate)Delegate.Combine(playerReference.OnValueChanged, new NetworkVariable<NetworkObjectReference>.OnValueChangedDelegate(this.OnPlayerReferenceChanged));
		base.OnNetworkSpawn();
	}

	// Token: 0x0600016C RID: 364 RVA: 0x00007D06 File Offset: 0x00005F06
	protected override void OnNetworkPostSpawn()
	{
		if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsConnectedClient)
		{
			this.ProcessInitialNetworkVariableValues();
		}
		base.OnNetworkPostSpawn();
	}

	// Token: 0x0600016D RID: 365 RVA: 0x00007D2C File Offset: 0x00005F2C
	protected override void OnNetworkSessionSynchronized()
	{
		this.ProcessInitialNetworkVariableValues();
		base.OnNetworkSessionSynchronized();
	}

	// Token: 0x0600016E RID: 366 RVA: 0x00007D3A File Offset: 0x00005F3A
	public override void OnNetworkDespawn()
	{
		NetworkVariable<NetworkObjectReference> playerReference = this.PlayerReference;
		playerReference.OnValueChanged = (NetworkVariable<NetworkObjectReference>.OnValueChangedDelegate)Delegate.Remove(playerReference.OnValueChanged, new NetworkVariable<NetworkObjectReference>.OnValueChangedDelegate(this.OnPlayerReferenceChanged));
		base.OnNetworkDespawn();
	}

	// Token: 0x0600016F RID: 367 RVA: 0x00007D69 File Offset: 0x00005F69
	public void InitializeNetworkVariables(NetworkObjectReference playerReference = default(NetworkObjectReference))
	{
		if (this.isNetworkVariablesInitialized)
		{
			return;
		}
		this.isNetworkVariablesInitialized = true;
		this.PlayerReference = new NetworkVariable<NetworkObjectReference>(playerReference, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
	}

	// Token: 0x06000170 RID: 368 RVA: 0x00007D8C File Offset: 0x00005F8C
	private void ProcessInitialNetworkVariableValues()
	{
		this.OnPlayerReferenceChanged(default(NetworkObjectReference), this.PlayerReference.Value);
	}

	// Token: 0x06000171 RID: 369 RVA: 0x00007DB3 File Offset: 0x00005FB3
	public override bool Enable()
	{
		bool flag = base.Enable();
		if (flag)
		{
			EventManager.TriggerEvent("Event_OnPlayerCameraEnabled", new Dictionary<string, object>
			{
				{
					"playerCamera",
					this
				}
			});
		}
		return flag;
	}

	// Token: 0x06000172 RID: 370 RVA: 0x00007DD9 File Offset: 0x00005FD9
	public override bool Disable()
	{
		bool flag = base.Disable();
		if (flag)
		{
			EventManager.TriggerEvent("Event_OnPlayerCameraDisabled", new Dictionary<string, object>
			{
				{
					"playerCamera",
					this
				}
			});
		}
		return flag;
	}

	// Token: 0x06000173 RID: 371 RVA: 0x00007E00 File Offset: 0x00006000
	public override void OnTick(float deltaTime)
	{
		base.OnTick(deltaTime);
		if (!this.Player)
		{
			return;
		}
		PlayerInput playerInput = this.Player.PlayerInput;
		if (!playerInput)
		{
			return;
		}
		playerInput.UpdateLookAngle(deltaTime);
		base.transform.localRotation = Quaternion.Euler(this.Player.IsLocalPlayer ? playerInput.LookAngleInput.ClientValue : playerInput.LookAngleInput.ServerValue);
	}

	// Token: 0x06000174 RID: 372 RVA: 0x00007E78 File Offset: 0x00006078
	private void OnPlayerReferenceChanged(NetworkObjectReference previousValue, NetworkObjectReference newValue)
	{
		if (!this.Player)
		{
			return;
		}
		this.Player.PlayerCamera = this;
	}

	// Token: 0x06000176 RID: 374 RVA: 0x00007E94 File Offset: 0x00006094
	protected override void __initializeVariables()
	{
		bool flag = this.PlayerReference == null;
		if (flag)
		{
			throw new Exception("PlayerCamera.PlayerReference cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.PlayerReference.Initialize(this);
		base.__nameNetworkVariable(this.PlayerReference, "PlayerReference");
		this.NetworkVariableFields.Add(this.PlayerReference);
		base.__initializeVariables();
	}

	// Token: 0x06000177 RID: 375 RVA: 0x00002D76 File Offset: 0x00000F76
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x06000178 RID: 376 RVA: 0x00007EF7 File Offset: 0x000060F7
	protected internal override string __getTypeName()
	{
		return "PlayerCamera";
	}

	// Token: 0x0400011B RID: 283
	[HideInInspector]
	public NetworkVariable<NetworkObjectReference> PlayerReference;

	// Token: 0x0400011C RID: 284
	private bool isNetworkVariablesInitialized;
}
