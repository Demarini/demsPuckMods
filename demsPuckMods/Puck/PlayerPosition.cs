using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Token: 0x02000017 RID: 23
public class PlayerPosition : NetworkBehaviour
{
	// Token: 0x1700000A RID: 10
	// (get) Token: 0x060000A8 RID: 168 RVA: 0x000072DF File Offset: 0x000054DF
	[HideInInspector]
	public bool IsClaimed
	{
		get
		{
			return this.ClaimedBy != null;
		}
	}

	// Token: 0x060000A9 RID: 169 RVA: 0x00010F48 File Offset: 0x0000F148
	public override void OnNetworkSpawn()
	{
		this.ClaimedByReference.Initialize(this);
		NetworkVariable<NetworkObjectReference> claimedByReference = this.ClaimedByReference;
		claimedByReference.OnValueChanged = (NetworkVariable<NetworkObjectReference>.OnValueChangedDelegate)Delegate.Combine(claimedByReference.OnValueChanged, new NetworkVariable<NetworkObjectReference>.OnValueChangedDelegate(this.OnPlayerPositionClaimedByReferenceChanged));
		this.Client_InitializeNetworkVariables();
		base.OnNetworkSpawn();
	}

	// Token: 0x060000AA RID: 170 RVA: 0x000072ED File Offset: 0x000054ED
	protected override void OnNetworkSessionSynchronized()
	{
		this.Client_InitializeNetworkVariables();
		base.OnNetworkSessionSynchronized();
	}

	// Token: 0x060000AB RID: 171 RVA: 0x000072FB File Offset: 0x000054FB
	public override void OnNetworkDespawn()
	{
		NetworkVariable<NetworkObjectReference> claimedByReference = this.ClaimedByReference;
		claimedByReference.OnValueChanged = (NetworkVariable<NetworkObjectReference>.OnValueChangedDelegate)Delegate.Remove(claimedByReference.OnValueChanged, new NetworkVariable<NetworkObjectReference>.OnValueChangedDelegate(this.OnPlayerPositionClaimedByReferenceChanged));
		this.ClaimedByReference.Dispose();
		base.OnNetworkDespawn();
	}

	// Token: 0x060000AC RID: 172 RVA: 0x00010F94 File Offset: 0x0000F194
	private void OnPlayerPositionClaimedByReferenceChanged(NetworkObjectReference oldClaimedByReferece, NetworkObjectReference newClaimedByReferece)
	{
		Player playerFromNetworkObjectReference = NetworkingUtils.GetPlayerFromNetworkObjectReference(oldClaimedByReferece);
		Player playerFromNetworkObjectReference2 = NetworkingUtils.GetPlayerFromNetworkObjectReference(newClaimedByReferece);
		this.ClaimedBy = playerFromNetworkObjectReference2;
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnPlayerPositionClaimedByChanged", new Dictionary<string, object>
		{
			{
				"playerPosition",
				this
			},
			{
				"oldClaimedBy",
				playerFromNetworkObjectReference
			},
			{
				"newClaimedBy",
				playerFromNetworkObjectReference2
			}
		});
	}

	// Token: 0x060000AD RID: 173 RVA: 0x00007335 File Offset: 0x00005535
	public void Server_Claim(Player player)
	{
		this.ClaimedByReference.Value = new NetworkObjectReference(player.NetworkObject);
	}

	// Token: 0x060000AE RID: 174 RVA: 0x00010FF0 File Offset: 0x0000F1F0
	public void Server_Unclaim()
	{
		this.ClaimedByReference.Value = default(NetworkObjectReference);
	}

	// Token: 0x060000AF RID: 175 RVA: 0x0000734D File Offset: 0x0000554D
	public void Client_InitializeNetworkVariables()
	{
		this.OnPlayerPositionClaimedByReferenceChanged(this.ClaimedByReference.Value, this.ClaimedByReference.Value);
	}

	// Token: 0x060000B1 RID: 177 RVA: 0x00011040 File Offset: 0x0000F240
	protected override void __initializeVariables()
	{
		bool flag = this.ClaimedByReference == null;
		if (flag)
		{
			throw new Exception("PlayerPosition.ClaimedByReference cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.ClaimedByReference.Initialize(this);
		base.__nameNetworkVariable(this.ClaimedByReference, "ClaimedByReference");
		this.NetworkVariableFields.Add(this.ClaimedByReference);
		base.__initializeVariables();
	}

	// Token: 0x060000B2 RID: 178 RVA: 0x00006C33 File Offset: 0x00004E33
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x060000B3 RID: 179 RVA: 0x0000736B File Offset: 0x0000556B
	protected internal override string __getTypeName()
	{
		return "PlayerPosition";
	}

	// Token: 0x04000048 RID: 72
	public string Name;

	// Token: 0x04000049 RID: 73
	public PlayerTeam Team;

	// Token: 0x0400004A RID: 74
	public PlayerRole Role;

	// Token: 0x0400004B RID: 75
	[HideInInspector]
	public NetworkVariable<NetworkObjectReference> ClaimedByReference = new NetworkVariable<NetworkObjectReference>(default(NetworkObjectReference), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x0400004C RID: 76
	[HideInInspector]
	public Player ClaimedBy;
}
