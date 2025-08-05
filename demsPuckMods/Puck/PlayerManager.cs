using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

// Token: 0x02000064 RID: 100
public class PlayerManager : NetworkBehaviourSingleton<PlayerManager>
{
	// Token: 0x060002C3 RID: 707 RVA: 0x00008AE7 File Offset: 0x00006CE7
	public void AddPlayer(Player player)
	{
		this.players.Add(player);
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnPlayerAdded", new Dictionary<string, object>
		{
			{
				"player",
				player
			}
		});
	}

	// Token: 0x060002C4 RID: 708 RVA: 0x00008B15 File Offset: 0x00006D15
	public void RemovePlayer(Player player)
	{
		this.players.Remove(player);
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnPlayerRemoved", new Dictionary<string, object>
		{
			{
				"player",
				player
			}
		});
	}

	// Token: 0x060002C5 RID: 709 RVA: 0x00017F34 File Offset: 0x00016134
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

	// Token: 0x060002C6 RID: 710 RVA: 0x00017FA8 File Offset: 0x000161A8
	public List<Player> GetPlayersByTeam(PlayerTeam team, bool includeReplay = false)
	{
		return (from player in this.GetPlayers(includeReplay)
		where player.Team.Value == team
		select player).ToList<Player>();
	}

	// Token: 0x060002C7 RID: 711 RVA: 0x00017FE0 File Offset: 0x000161E0
	public List<Player> GetReplayPlayers()
	{
		this.players.RemoveAll((Player player) => !player || !player.NetworkObject.IsSpawned);
		return (from player in this.players
		where player.IsReplay.Value
		select player).ToList<Player>();
	}

	// Token: 0x060002C8 RID: 712 RVA: 0x00018048 File Offset: 0x00016248
	public Player GetPlayerByClientId(ulong clientId)
	{
		return this.GetPlayers(false).Find((Player player) => player.OwnerClientId == clientId);
	}

	// Token: 0x060002C9 RID: 713 RVA: 0x0001807C File Offset: 0x0001627C
	public Player GetPlayerByUsername(FixedString32Bytes username, bool caseSensitive = true)
	{
		return this.GetPlayers(false).Find((Player player) => (caseSensitive ? player.Username.Value.ToString() : player.Username.Value.ToString().ToUpper()) == (caseSensitive ? username.ToString() : username.ToString().ToUpper()));
	}

	// Token: 0x060002CA RID: 714 RVA: 0x000180B8 File Offset: 0x000162B8
	public Player GetPlayerByNumber(int number)
	{
		return this.GetPlayers(false).Find((Player player) => player.Number.Value == number);
	}

	// Token: 0x060002CB RID: 715 RVA: 0x000180EC File Offset: 0x000162EC
	public Player GetPlayerBySteamId(FixedString32Bytes steamId)
	{
		return this.GetPlayers(false).Find(delegate(Player player)
		{
			FixedString32Bytes value = player.SteamId.Value;
			return value == steamId;
		});
	}

	// Token: 0x060002CC RID: 716 RVA: 0x00018120 File Offset: 0x00016320
	public Player GetReplayPlayerByClientId(ulong clientId)
	{
		return this.GetReplayPlayers().Find((Player player) => player.OwnerClientId == 1337UL + clientId);
	}

	// Token: 0x060002CD RID: 717 RVA: 0x00008B44 File Offset: 0x00006D44
	public Player GetLocalPlayer()
	{
		return this.GetPlayers(false).Find((Player player) => player.IsLocalPlayer);
	}

	// Token: 0x060002CE RID: 718 RVA: 0x00008B71 File Offset: 0x00006D71
	public List<Player> GetSpawnedPlayers(bool includeReplay = false)
	{
		return this.GetPlayers(includeReplay).FindAll((Player player) => player.IsCharacterPartiallySpawned);
	}

	// Token: 0x060002CF RID: 719 RVA: 0x00018154 File Offset: 0x00016354
	public List<Player> GetSpawnedPlayersByTeam(PlayerTeam team, bool includeReplay = false)
	{
		return (from player in this.GetSpawnedPlayers(includeReplay)
		where player.Team.Value == team
		select player).ToList<Player>();
	}

	// Token: 0x060002D0 RID: 720 RVA: 0x00008B9E File Offset: 0x00006D9E
	public Player GetSpawnedFirstPlayer(bool includeReplay = false)
	{
		List<Player> spawnedPlayers = this.GetSpawnedPlayers(includeReplay);
		spawnedPlayers.Sort((Player a, Player b) => a.OwnerClientId.CompareTo(b.OwnerClientId));
		return spawnedPlayers.FirstOrDefault<Player>();
	}

	// Token: 0x060002D1 RID: 721 RVA: 0x00008BD1 File Offset: 0x00006DD1
	public Player GetSpawnedLastPlayer(bool includeReplay = false)
	{
		List<Player> spawnedPlayers = this.GetSpawnedPlayers(includeReplay);
		spawnedPlayers.Sort((Player a, Player b) => a.OwnerClientId.CompareTo(b.OwnerClientId));
		return spawnedPlayers.LastOrDefault<Player>();
	}

	// Token: 0x060002D2 RID: 722 RVA: 0x0001818C File Offset: 0x0001638C
	public Player GetSpawnedNextPlayerByClientId(ulong clientId, bool includeReplay = false)
	{
		return this.GetSpawnedPlayers(includeReplay).Find((Player player) => player.OwnerClientId > clientId);
	}

	// Token: 0x060002D3 RID: 723 RVA: 0x000181C0 File Offset: 0x000163C0
	public Player GetSpawnedPreviousPlayerByClientId(ulong clientId, bool includeReplay = false)
	{
		return this.GetSpawnedPlayers(includeReplay).Find((Player player) => player.OwnerClientId < clientId);
	}

	// Token: 0x060002D4 RID: 724 RVA: 0x000181F4 File Offset: 0x000163F4
	public PlayerBodyV2 GetPlayerBodyByNetworkObjectId(ulong networkObjectId, bool includeReplay = false)
	{
		Player player2 = this.GetSpawnedPlayers(includeReplay).Find((Player player) => player.PlayerBody.NetworkObjectId == networkObjectId);
		if (player2)
		{
			return player2.PlayerBody;
		}
		return null;
	}

	// Token: 0x060002D5 RID: 725 RVA: 0x00008C04 File Offset: 0x00006E04
	public string[] GetPlayerSteamIds()
	{
		return (from player in this.GetPlayers(false)
		select player.SteamId.Value.ToString()).ToArray<string>();
	}

	// Token: 0x060002D6 RID: 726 RVA: 0x00008C36 File Offset: 0x00006E36
	public bool IsEnoughPlayersForPlaying()
	{
		return this.GetPlayersByTeam(PlayerTeam.Blue, false).Count >= 1 && this.GetPlayersByTeam(PlayerTeam.Red, false).Count >= 1;
	}

	// Token: 0x060002D7 RID: 727 RVA: 0x00018238 File Offset: 0x00016438
	public void Server_SpawnPlayer(ulong clientId, bool isReplay = false)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		Player player = UnityEngine.Object.Instantiate<Player>(this.playerPrefab);
		player.IsReplay.Value = isReplay;
		if (isReplay)
		{
			player.NetworkObject.SpawnWithOwnership(clientId, false);
			Debug.Log(string.Format("[PlayerManager] Spawned replay player ({0})", clientId));
			return;
		}
		player.NetworkObject.SpawnAsPlayerObject(clientId, false);
		Debug.Log(string.Format("[PlayerManager] Spawned player ({0})", clientId));
	}

	// Token: 0x060002D9 RID: 729 RVA: 0x000182B4 File Offset: 0x000164B4
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x060002DA RID: 730 RVA: 0x00008C70 File Offset: 0x00006E70
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x060002DB RID: 731 RVA: 0x00008C7A File Offset: 0x00006E7A
	protected internal override string __getTypeName()
	{
		return "PlayerManager";
	}

	// Token: 0x04000196 RID: 406
	[Header("Prefabs")]
	[SerializeField]
	private Player playerPrefab;

	// Token: 0x04000197 RID: 407
	private List<Player> players = new List<Player>();
}
