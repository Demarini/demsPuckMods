using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Token: 0x0200001C RID: 28
public class PlayerPosition : NetworkBehaviour
{
	// Token: 0x17000007 RID: 7
	// (get) Token: 0x060000A2 RID: 162 RVA: 0x00003E06 File Offset: 0x00002006
	[HideInInspector]
	public bool IsClaimed
	{
		get
		{
			return this.ClaimedByPlayer != null;
		}
	}

	// Token: 0x060000A3 RID: 163 RVA: 0x00003E14 File Offset: 0x00002014
	protected override void OnNetworkPreSpawn(ref NetworkManager networkManager)
	{
		this.InitializeNetworkVariables(default(NetworkObjectReference));
		base.OnNetworkPreSpawn(ref networkManager);
	}

	// Token: 0x060000A4 RID: 164 RVA: 0x00003E37 File Offset: 0x00002037
	public override void OnNetworkSpawn()
	{
		NetworkVariable<NetworkObjectReference> claimedByPlayerReference = this.ClaimedByPlayerReference;
		claimedByPlayerReference.OnValueChanged = (NetworkVariable<NetworkObjectReference>.OnValueChangedDelegate)Delegate.Combine(claimedByPlayerReference.OnValueChanged, new NetworkVariable<NetworkObjectReference>.OnValueChangedDelegate(this.OnClaimedByReferenceChanged));
		base.OnNetworkSpawn();
	}

	// Token: 0x060000A5 RID: 165 RVA: 0x00003E68 File Offset: 0x00002068
	protected override void OnNetworkPostSpawn()
	{
		EventManager.TriggerEvent("Event_Everyone_OnPlayerPositionSpawned", new Dictionary<string, object>
		{
			{
				"playerPosition",
				this
			}
		});
		if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsConnectedClient)
		{
			this.ProcessInitialNetworkVariableValues();
		}
		base.OnNetworkPostSpawn();
	}

	// Token: 0x060000A6 RID: 166 RVA: 0x00003EB4 File Offset: 0x000020B4
	protected override void OnNetworkSessionSynchronized()
	{
		this.ProcessInitialNetworkVariableValues();
		base.OnNetworkSessionSynchronized();
	}

	// Token: 0x060000A7 RID: 167 RVA: 0x00003EC4 File Offset: 0x000020C4
	public override void OnNetworkDespawn()
	{
		EventManager.TriggerEvent("Event_Everyone_OnPlayerPositionDespawned", new Dictionary<string, object>
		{
			{
				"playerPosition",
				this
			}
		});
		NetworkVariable<NetworkObjectReference> claimedByPlayerReference = this.ClaimedByPlayerReference;
		claimedByPlayerReference.OnValueChanged = (NetworkVariable<NetworkObjectReference>.OnValueChangedDelegate)Delegate.Remove(claimedByPlayerReference.OnValueChanged, new NetworkVariable<NetworkObjectReference>.OnValueChangedDelegate(this.OnClaimedByReferenceChanged));
		base.OnNetworkDespawn();
	}

	// Token: 0x060000A8 RID: 168 RVA: 0x00003F19 File Offset: 0x00002119
	public void InitializeNetworkVariables(NetworkObjectReference claimedByPlayerReference = default(NetworkObjectReference))
	{
		if (this.isNetworkVariablesInitialized)
		{
			return;
		}
		this.isNetworkVariablesInitialized = true;
		this.ClaimedByPlayerReference = new NetworkVariable<NetworkObjectReference>(claimedByPlayerReference, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
	}

	// Token: 0x060000A9 RID: 169 RVA: 0x00003F3C File Offset: 0x0000213C
	private void ProcessInitialNetworkVariableValues()
	{
		this.OnClaimedByReferenceChanged(default(NetworkObjectReference), this.ClaimedByPlayerReference.Value);
	}

	// Token: 0x060000AA RID: 170 RVA: 0x00003F64 File Offset: 0x00002164
	private void OnClaimedByReferenceChanged(NetworkObjectReference oldClaimedByReferece, NetworkObjectReference newClaimedByReferece)
	{
		Player playerFromNetworkObjectReference = NetworkingUtils.GetPlayerFromNetworkObjectReference(oldClaimedByReferece);
		Player playerFromNetworkObjectReference2 = NetworkingUtils.GetPlayerFromNetworkObjectReference(newClaimedByReferece);
		this.ClaimedByPlayer = playerFromNetworkObjectReference2;
		EventManager.TriggerEvent("Event_Everyone_OnPlayerPositionClaimedByPlayerChanged", new Dictionary<string, object>
		{
			{
				"playerPosition",
				this
			},
			{
				"oldClaimedByPlayer",
				playerFromNetworkObjectReference
			},
			{
				"newClaimedByPlayer",
				playerFromNetworkObjectReference2
			}
		});
	}

	// Token: 0x060000AB RID: 171 RVA: 0x00003FB9 File Offset: 0x000021B9
	public void Server_Claim(Player player)
	{
		Debug.Log(string.Format("[PlayerPosition] Position {0} claimed by {1}", this.Name, player.OwnerClientId));
		this.ClaimedByPlayerReference.Value = new NetworkObjectReference(player.NetworkObject);
	}

	// Token: 0x060000AC RID: 172 RVA: 0x00003FF4 File Offset: 0x000021F4
	public void Server_Unclaim()
	{
		Debug.Log("[PlayerPosition] Position " + this.Name + " unclaimed");
		this.ClaimedByPlayerReference.Value = default(NetworkObjectReference);
	}

	// Token: 0x060000AE RID: 174 RVA: 0x00004030 File Offset: 0x00002230
	protected override void __initializeVariables()
	{
		bool flag = this.ClaimedByPlayerReference == null;
		if (flag)
		{
			throw new Exception("PlayerPosition.ClaimedByPlayerReference cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.ClaimedByPlayerReference.Initialize(this);
		base.__nameNetworkVariable(this.ClaimedByPlayerReference, "ClaimedByPlayerReference");
		this.NetworkVariableFields.Add(this.ClaimedByPlayerReference);
		base.__initializeVariables();
	}

	// Token: 0x060000AF RID: 175 RVA: 0x000021BE File Offset: 0x000003BE
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x060000B0 RID: 176 RVA: 0x00004093 File Offset: 0x00002293
	protected internal override string __getTypeName()
	{
		return "PlayerPosition";
	}

	// Token: 0x04000047 RID: 71
	[Header("Settings")]
	public string Name;

	// Token: 0x04000048 RID: 72
	public PlayerTeam Team;

	// Token: 0x04000049 RID: 73
	public PlayerRole Role;

	// Token: 0x0400004A RID: 74
	[HideInInspector]
	public NetworkVariable<NetworkObjectReference> ClaimedByPlayerReference;

	// Token: 0x0400004B RID: 75
	[HideInInspector]
	public Player ClaimedByPlayer;

	// Token: 0x0400004C RID: 76
	private bool isNetworkVariablesInitialized;
}
