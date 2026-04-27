using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

// Token: 0x020000E3 RID: 227
public class PlayerManager : MonoBehaviourSingleton<PlayerManager>
{
	// Token: 0x060006B3 RID: 1715 RVA: 0x00021864 File Offset: 0x0001FA64
	public void AddPlayer(Player player)
	{
		this.players.Add(player);
		EventManager.TriggerEvent("Event_Everyone_OnPlayerAdded", new Dictionary<string, object>
		{
			{
				"player",
				player
			}
		});
	}

	// Token: 0x060006B4 RID: 1716 RVA: 0x0002188D File Offset: 0x0001FA8D
	public void RemovePlayer(Player player)
	{
		this.players.Remove(player);
		EventManager.TriggerEvent("Event_Everyone_OnPlayerRemoved", new Dictionary<string, object>
		{
			{
				"player",
				player
			}
		});
	}

	// Token: 0x060006B5 RID: 1717 RVA: 0x000218B8 File Offset: 0x0001FAB8
	public List<Player> GetPlayers(bool includeReplay = false)
	{
		this.players.RemoveAll((Player player) => !player || !player.NetworkObject.IsSpawned);
		if (includeReplay)
		{
			return this.players;
		}
		return (from player in this.players
		where !player.IsReplay.Value
		select player).ToList<Player>();
	}

	// Token: 0x060006B6 RID: 1718 RVA: 0x0002192C File Offset: 0x0001FB2C
	public List<Player> GetPlayersByPhase(PlayerPhase phase, bool includeReplay = false)
	{
		return (from player in this.GetPlayers(includeReplay)
		where player.Phase == phase
		select player).ToList<Player>();
	}

	// Token: 0x060006B7 RID: 1719 RVA: 0x00021964 File Offset: 0x0001FB64
	public List<Player> GetPlayersByPhases(PlayerPhase[] phases, bool includeReplay = false)
	{
		return (from player in this.GetPlayers(includeReplay)
		where phases.Contains(player.Phase)
		select player).ToList<Player>();
	}

	// Token: 0x060006B8 RID: 1720 RVA: 0x0002199C File Offset: 0x0001FB9C
	public List<Player> GetPlayersByTeam(PlayerTeam team, bool includeReplay = false)
	{
		return (from player in this.GetPlayers(includeReplay)
		where player.Team == team
		select player).ToList<Player>();
	}

	// Token: 0x060006B9 RID: 1721 RVA: 0x000219D4 File Offset: 0x0001FBD4
	public List<Player> GetPlayersByTeams(PlayerTeam[] team, bool includeReplay = false)
	{
		return (from player in this.GetPlayers(includeReplay)
		where team.Contains(player.Team)
		select player).ToList<Player>();
	}

	// Token: 0x060006BA RID: 1722 RVA: 0x00021A0C File Offset: 0x0001FC0C
	public Player GetPlayerByClientId(ulong clientId)
	{
		return this.GetPlayers(false).Find((Player player) => player.OwnerClientId == clientId);
	}

	// Token: 0x060006BB RID: 1723 RVA: 0x00021A40 File Offset: 0x0001FC40
	public Player GetPlayerByUsername(FixedString32Bytes username, bool caseSensitive = true)
	{
		return this.GetPlayers(false).Find((Player player) => (caseSensitive ? player.Username.Value.ToString() : player.Username.Value.ToString().ToUpper()) == (caseSensitive ? username.ToString() : username.ToString().ToUpper()));
	}

	// Token: 0x060006BC RID: 1724 RVA: 0x00021A7C File Offset: 0x0001FC7C
	public Player GetPlayerByNumber(int number)
	{
		return this.GetPlayers(false).Find((Player player) => player.Number.Value == number);
	}

	// Token: 0x060006BD RID: 1725 RVA: 0x00021AB0 File Offset: 0x0001FCB0
	public Player GetPlayerBySteamId(FixedString32Bytes steamId)
	{
		return this.GetPlayers(false).Find(delegate(Player player)
		{
			FixedString32Bytes value = player.SteamId.Value;
			return value == steamId;
		});
	}

	// Token: 0x060006BE RID: 1726 RVA: 0x00021AE2 File Offset: 0x0001FCE2
	public List<Player> GetReplayPlayers()
	{
		return (from player in this.GetPlayers(true)
		where player.IsReplay.Value
		select player).ToList<Player>();
	}

	// Token: 0x060006BF RID: 1727 RVA: 0x00021B14 File Offset: 0x0001FD14
	public Player GetReplayPlayerByClientId(ulong clientId)
	{
		return this.GetReplayPlayers().Find((Player player) => player.OwnerClientId == clientId + 1337UL);
	}

	// Token: 0x060006C0 RID: 1728 RVA: 0x00021B45 File Offset: 0x0001FD45
	public Player GetLocalPlayer()
	{
		return this.GetPlayers(false).Find((Player player) => player.IsLocalPlayer);
	}

	// Token: 0x060006C1 RID: 1729 RVA: 0x00021B72 File Offset: 0x0001FD72
	public List<Player> GetSpawnedPlayers(bool includeReplay = false)
	{
		return this.GetPlayers(includeReplay).FindAll((Player player) => player.IsCharacterSpawned);
	}

	// Token: 0x060006C2 RID: 1730 RVA: 0x00021BA0 File Offset: 0x0001FDA0
	public List<Player> GetSpawnedPlayersByTeam(PlayerTeam team, bool includeReplay = false)
	{
		return (from player in this.GetSpawnedPlayers(includeReplay)
		where player.Team == team
		select player).ToList<Player>();
	}

	// Token: 0x060006C3 RID: 1731 RVA: 0x00021BD8 File Offset: 0x0001FDD8
	public void Server_SpawnPlayer(ulong clientId, PlayerGameState gameState, PlayerCustomizationState customizationState, PlayerHandedness handedness, string steamID, string username, int number, int patreonLevel, int adminLevel, bool isMuted = false, bool isReplay = false)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		Player player = Object.Instantiate<Player>(this.playerPrefab);
		Player player2 = player;
		FixedString32Bytes steamID2 = steamID;
		FixedString32Bytes username2 = username;
		player2.InitializeNetworkVariables(gameState, customizationState, handedness, steamID2, username2, number, patreonLevel, adminLevel, 0, 0, 0UL, default(NetworkObjectReference), isMuted, isReplay);
		if (isReplay)
		{
			player.NetworkObject.SpawnWithOwnership(1337UL + clientId, false);
			Debug.Log(string.Format("[PlayerManager] Spawned replay player ({0})", clientId));
			return;
		}
		player.NetworkObject.SpawnAsPlayerObject(clientId, false);
		Debug.Log(string.Format("[PlayerManager] Spawned player ({0})", clientId));
	}

	// Token: 0x0400040F RID: 1039
	[Header("Prefabs")]
	[SerializeField]
	private Player playerPrefab;

	// Token: 0x04000410 RID: 1040
	private List<Player> players = new List<Player>();
}
