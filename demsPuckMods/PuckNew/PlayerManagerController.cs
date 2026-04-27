using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000EF RID: 239
public class PlayerManagerController : MonoBehaviour
{
	// Token: 0x060006E0 RID: 1760 RVA: 0x00021E34 File Offset: 0x00020034
	private void Awake()
	{
		this.playerManager = base.GetComponent<PlayerManager>();
		EventManager.AddEventListener("Event_Everyone_OnPlayerSpawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerSpawned));
		EventManager.AddEventListener("Event_Everyone_OnPlayerDespawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerDespawned));
		EventManager.AddEventListener("Event_Server_OnApprovedClientConnected", new Action<Dictionary<string, object>>(this.Event_Server_OnApprovedClientConnected));
	}

	// Token: 0x060006E1 RID: 1761 RVA: 0x000020D3 File Offset: 0x000002D3
	private void Start()
	{
	}

	// Token: 0x060006E2 RID: 1762 RVA: 0x00021E90 File Offset: 0x00020090
	private void OnDestroy()
	{
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerSpawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerSpawned));
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerDespawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerDespawned));
		EventManager.RemoveEventListener("Event_Server_OnApprovedClientConnected", new Action<Dictionary<string, object>>(this.Event_Server_OnApprovedClientConnected));
	}

	// Token: 0x060006E3 RID: 1763 RVA: 0x00021EE0 File Offset: 0x000200E0
	private void Event_Everyone_OnPlayerSpawned(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		this.playerManager.AddPlayer(player);
	}

	// Token: 0x060006E4 RID: 1764 RVA: 0x00021F0C File Offset: 0x0002010C
	private void Event_Everyone_OnPlayerDespawned(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		this.playerManager.RemovePlayer(player);
	}

	// Token: 0x060006E5 RID: 1765 RVA: 0x00021F38 File Offset: 0x00020138
	private void Event_Server_OnApprovedClientConnected(Dictionary<string, object> message)
	{
		Debug.Log("[PlayerManagerController] Event_Server_OnApprovedClientConnected");
		ulong clientId = (ulong)message["clientId"];
		ConnectionApproval connectionApproval = (ConnectionApproval)message["connectionApproval"];
		ConnectionData connectionData = connectionApproval.ConnectionData;
		PlayerData playerData = connectionApproval.PlayerData;
		PlayerGameState gameState = new PlayerGameState
		{
			Phase = PlayerPhase.TeamSelect,
			Team = PlayerTeam.None,
			Role = PlayerRole.None
		};
		PlayerCustomizationState customizationState = new PlayerCustomizationState
		{
			FlagID = connectionData.FlagID,
			HeadgearIDBlueAttacker = connectionData.HeadgearIDBlueAttacker,
			HeadgearIDRedAttacker = connectionData.HeadgearIDRedAttacker,
			HeadgearIDBlueGoalie = connectionData.HeadgearIDBlueGoalie,
			HeadgearIDRedGoalie = connectionData.HeadgearIDRedGoalie,
			MustacheID = connectionData.MustacheID,
			BeardID = connectionData.BeardID,
			JerseyIDBlueAttacker = connectionData.JerseyIDBlueAttacker,
			JerseyIDRedAttacker = connectionData.JerseyIDRedAttacker,
			JerseyIDBlueGoalie = connectionData.JerseyIDBlueGoalie,
			JerseyIDRedGoalie = connectionData.JerseyIDRedGoalie,
			StickSkinIDBlueAttacker = connectionData.StickSkinIDBlueAttacker,
			StickSkinIDRedAttacker = connectionData.StickSkinIDRedAttacker,
			StickSkinIDBlueGoalie = connectionData.StickSkinIDBlueGoalie,
			StickSkinIDRedGoalie = connectionData.StickSkinIDRedGoalie,
			StickShaftTapeIDBlueAttacker = connectionData.StickShaftTapeIDBlueAttacker,
			StickShaftTapeIDRedAttacker = connectionData.StickShaftTapeIDRedAttacker,
			StickShaftTapeIDBlueGoalie = connectionData.StickShaftTapeIDBlueGoalie,
			StickShaftTapeIDRedGoalie = connectionData.StickShaftTapeIDRedGoalie,
			StickBladeTapeIDBlueAttacker = connectionData.StickBladeTapeIDBlueAttacker,
			StickBladeTapeIDRedAttacker = connectionData.StickBladeTapeIDRedAttacker,
			StickBladeTapeIDBlueGoalie = connectionData.StickBladeTapeIDBlueGoalie,
			StickBladeTapeIDRedGoalie = connectionData.StickBladeTapeIDRedGoalie
		};
		bool isMuted = BackendUtils.GetActivePlayerDataMute(playerData) != null;
		this.playerManager.Server_SpawnPlayer(clientId, gameState, customizationState, connectionData.Handedness, playerData.steamId, playerData.username, playerData.number, playerData.patreonLevel, playerData.adminLevel, isMuted, false);
	}

	// Token: 0x04000422 RID: 1058
	private PlayerManager playerManager;
}
