using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

// Token: 0x02000071 RID: 113
public class PlayerPositionManager : NetworkBehaviourSingleton<PlayerPositionManager>
{
	// Token: 0x1700004B RID: 75
	// (get) Token: 0x06000305 RID: 773 RVA: 0x00008D84 File Offset: 0x00006F84
	[HideInInspector]
	public List<PlayerPosition> BluePositions
	{
		get
		{
			return this.bluePositions;
		}
	}

	// Token: 0x1700004C RID: 76
	// (get) Token: 0x06000306 RID: 774 RVA: 0x00008D8C File Offset: 0x00006F8C
	[HideInInspector]
	public List<PlayerPosition> RedPositions
	{
		get
		{
			return this.redPositions;
		}
	}

	// Token: 0x1700004D RID: 77
	// (get) Token: 0x06000307 RID: 775 RVA: 0x00008D94 File Offset: 0x00006F94
	[HideInInspector]
	public List<PlayerPosition> AllPositions
	{
		get
		{
			return this.bluePositions.Concat(this.redPositions).ToList<PlayerPosition>();
		}
	}

	// Token: 0x06000308 RID: 776 RVA: 0x00008DAC File Offset: 0x00006FAC
	public void SetBluePositions(List<PlayerPosition> bluePositions)
	{
		this.bluePositions = bluePositions;
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnBluePlayerPositionsSet", null);
	}

	// Token: 0x06000309 RID: 777 RVA: 0x00008DC5 File Offset: 0x00006FC5
	public void SetRedPositions(List<PlayerPosition> redPositions)
	{
		this.redPositions = redPositions;
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnRedPlayerPositionsSet", null);
	}

	// Token: 0x0600030A RID: 778 RVA: 0x00018518 File Offset: 0x00016718
	public void Server_UnclaimAllPositions()
	{
		foreach (PlayerPosition playerPosition in this.AllPositions)
		{
			playerPosition.Server_Unclaim();
		}
	}

	// Token: 0x0600030B RID: 779 RVA: 0x00018568 File Offset: 0x00016768
	[Rpc(SendTo.Server)]
	public void Client_ClaimPositionRpc(NetworkObjectReference playerPositionNetworkObjectReference, RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 4027053218U;
			RpcAttribute.RpcAttributeParams attributeParams = default(RpcAttribute.RpcAttributeParams);
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
			fastBufferWriter.WriteValueSafe<NetworkObjectReference>(playerPositionNetworkObjectReference, default(FastBufferWriter.ForNetworkSerializable));
			base.__endSendRpc(ref fastBufferWriter, 4027053218U, rpcParams, attributeParams, SendTo.Server, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		Player playerByClientId = NetworkBehaviourSingleton<PlayerManager>.Instance.GetPlayerByClientId(rpcParams.Receive.SenderClientId);
		PlayerPosition playerPosition = playerByClientId.PlayerPosition;
		NetworkObject networkObject;
		if (!playerPositionNetworkObjectReference.TryGet(out networkObject, null))
		{
			return;
		}
		PlayerPosition component = networkObject.GetComponent<PlayerPosition>();
		if (!playerByClientId)
		{
			return;
		}
		if (!component)
		{
			return;
		}
		if (component.IsClaimed)
		{
			return;
		}
		if (!playerByClientId)
		{
			return;
		}
		if (playerByClientId.Team.Value != component.Team)
		{
			return;
		}
		if (playerPosition)
		{
			playerPosition.Server_Unclaim();
		}
		component.Server_Claim(playerByClientId);
	}

	// Token: 0x0600030D RID: 781 RVA: 0x000186B8 File Offset: 0x000168B8
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x0600030E RID: 782 RVA: 0x00008DFC File Offset: 0x00006FFC
	protected override void __initializeRpcs()
	{
		base.__registerRpc(4027053218U, new NetworkBehaviour.RpcReceiveHandler(PlayerPositionManager.__rpc_handler_4027053218), "Client_ClaimPositionRpc");
		base.__initializeRpcs();
	}

	// Token: 0x0600030F RID: 783 RVA: 0x000186D0 File Offset: 0x000168D0
	private static void __rpc_handler_4027053218(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		NetworkObjectReference playerPositionNetworkObjectReference;
		reader.ReadValueSafe<NetworkObjectReference>(out playerPositionNetworkObjectReference, default(FastBufferWriter.ForNetworkSerializable));
		RpcParams ext = rpcParams.Ext;
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((PlayerPositionManager)target).Client_ClaimPositionRpc(playerPositionNetworkObjectReference, ext);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x06000310 RID: 784 RVA: 0x00008E22 File Offset: 0x00007022
	protected internal override string __getTypeName()
	{
		return "PlayerPositionManager";
	}

	// Token: 0x040001AE RID: 430
	private List<PlayerPosition> bluePositions = new List<PlayerPosition>();

	// Token: 0x040001AF RID: 431
	private List<PlayerPosition> redPositions = new List<PlayerPosition>();
}
