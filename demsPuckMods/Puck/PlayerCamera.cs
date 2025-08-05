using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Token: 0x020000E3 RID: 227
public class PlayerCamera : BaseCamera
{
	// Token: 0x170000AC RID: 172
	// (get) Token: 0x06000720 RID: 1824 RVA: 0x0000B8D2 File Offset: 0x00009AD2
	[HideInInspector]
	public PlayerBodyV2 PlayerBody
	{
		get
		{
			return this.Player.PlayerBody;
		}
	}

	// Token: 0x06000721 RID: 1825 RVA: 0x00029B54 File Offset: 0x00027D54
	protected override void OnNetworkPostSpawn()
	{
		NetworkObject networkObject;
		if (this.PlayerReference.Value.TryGet(out networkObject, null))
		{
			this.Player = networkObject.GetComponent<Player>();
		}
		if (this.Player)
		{
			this.Player.PlayerCamera = this;
			if (this.Player.IsLocalPlayer)
			{
				this.Enable();
			}
		}
		base.OnNetworkPostSpawn();
	}

	// Token: 0x06000722 RID: 1826 RVA: 0x0000B8DF File Offset: 0x00009ADF
	public override void OnNetworkDespawn()
	{
		if (base.IsEnabled)
		{
			this.Disable();
		}
		base.OnNetworkDespawn();
	}

	// Token: 0x06000723 RID: 1827 RVA: 0x0000B8F5 File Offset: 0x00009AF5
	public override void OnDestroy()
	{
		if (base.IsEnabled)
		{
			this.Disable();
		}
		base.OnDestroy();
	}

	// Token: 0x06000724 RID: 1828 RVA: 0x0000B90B File Offset: 0x00009B0B
	public override void Enable()
	{
		if (!base.IsEnabled)
		{
			MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnPlayerCameraEnabled", new Dictionary<string, object>
			{
				{
					"playerCamera",
					this
				}
			});
		}
		base.Enable();
	}

	// Token: 0x06000725 RID: 1829 RVA: 0x0000B93B File Offset: 0x00009B3B
	public override void Disable()
	{
		if (base.IsEnabled)
		{
			MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnPlayerCameraDisabled", new Dictionary<string, object>
			{
				{
					"playerCamera",
					this
				}
			});
		}
		base.Disable();
	}

	// Token: 0x06000726 RID: 1830 RVA: 0x00029BB8 File Offset: 0x00027DB8
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

	// Token: 0x06000728 RID: 1832 RVA: 0x00029C5C File Offset: 0x00027E5C
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

	// Token: 0x06000729 RID: 1833 RVA: 0x00007917 File Offset: 0x00005B17
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x0600072A RID: 1834 RVA: 0x0000B96B File Offset: 0x00009B6B
	protected internal override string __getTypeName()
	{
		return "PlayerCamera";
	}

	// Token: 0x04000438 RID: 1080
	[HideInInspector]
	public NetworkVariable<NetworkObjectReference> PlayerReference = new NetworkVariable<NetworkObjectReference>(default(NetworkObjectReference), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x04000439 RID: 1081
	[HideInInspector]
	public Player Player;
}
