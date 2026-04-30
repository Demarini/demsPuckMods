using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

// Token: 0x02000074 RID: 116
public class MatchableGameMode<TConfig> : StandardGameMode<!0> where TConfig : StandardGameModeConfig, new()
{
	// Token: 0x1700006C RID: 108
	// (get) Token: 0x060003F9 RID: 1017 RVA: 0x00016508 File Offset: 0x00014708
	public bool IsMatch
	{
		get
		{
			return BackendManager.ServerState.MatchData != null;
		}
	}

	// Token: 0x060003FA RID: 1018 RVA: 0x00016517 File Offset: 0x00014717
	public MatchableGameMode(string defaultConfigFilePath, string configFilePathCliArgument = null, string configCliArgument = null, string configEnvVariable = null) : base(defaultConfigFilePath, configFilePathCliArgument, configCliArgument, configEnvVariable)
	{
	}

	// Token: 0x060003FB RID: 1019 RVA: 0x00016530 File Offset: 0x00014730
	public override bool Initialize(Level level, ServerManager serverManager, GameManager gameManager, PlayerManager playerManager, PuckManager puckManager, ChatManager chatManager, ReplayManager replayManager)
	{
		if (!base.Initialize(level, serverManager, gameManager, playerManager, puckManager, chatManager, replayManager))
		{
			return false;
		}
		if (this.IsMatch)
		{
			List<string> steamIds = (from p in BackendManager.ServerState.MatchData.Players
			select p.steamId).ToList<string>();
			this.ServerManager.WhitelistManager.AddWhitelistedSteamIds(steamIds);
			this.StartMatch();
		}
		return true;
	}

	// Token: 0x060003FC RID: 1020 RVA: 0x000165AB File Offset: 0x000147AB
	private void StartMatch()
	{
		if (!this.IsMatch)
		{
			return;
		}
		WebSocketManager.Emit("serverMatchStart", null, null);
		this.OnMatchStarted();
	}

	// Token: 0x060003FD RID: 1021 RVA: 0x000165C8 File Offset: 0x000147C8
	private void EndMatch(GameResult gameResult = null)
	{
		if (!this.IsMatch)
		{
			return;
		}
		WebSocketManager.Emit("serverMatchEnd", new Dictionary<string, object>
		{
			{
				"gameResult",
				gameResult ?? this.gameResult
			}
		}, null);
		this.OnMatchEnded(gameResult ?? this.gameResult);
	}

	// Token: 0x060003FE RID: 1022 RVA: 0x00016615 File Offset: 0x00014815
	private void CancelMatch()
	{
		if (!this.IsMatch)
		{
			return;
		}
		WebSocketManager.Emit("serverMatchCancel", null, null);
		this.OnMatchCancelled();
	}

	// Token: 0x060003FF RID: 1023 RVA: 0x00016634 File Offset: 0x00014834
	private void IssueCooldown(string steamId)
	{
		if (!this.IsMatch)
		{
			return;
		}
		MatchPlayer matchPlayerBySteamId = BackendManager.ServerState.MatchData.GetMatchPlayerBySteamId(steamId);
		if (matchPlayerBySteamId == null)
		{
			return;
		}
		this.ChatManager.Server_BroadcastChatMessage(matchPlayerBySteamId.username + " has been issued a matchmaking cooldown");
		WebSocketManager.Emit("serverMatchIssueCooldown", new Dictionary<string, object>
		{
			{
				"steamId",
				steamId
			}
		}, null);
	}

	// Token: 0x06000400 RID: 1024 RVA: 0x00016698 File Offset: 0x00014898
	private void StartAbandonmentTimeout(string steamId, float timeout)
	{
		if (this.steamIdAbandonmentTweenMap.ContainsKey(steamId))
		{
			return;
		}
		Tween value = DOVirtual.DelayedCall(timeout, delegate
		{
			this.steamIdAbandonmentTweenMap.Remove(steamId);
			this.OnSteamIdAbandonedMatch(steamId);
		}, true);
		this.steamIdAbandonmentTweenMap.Add(steamId, value);
	}

	// Token: 0x06000401 RID: 1025 RVA: 0x000166F3 File Offset: 0x000148F3
	private void StopAbandonmentTimeout(string steamId)
	{
		if (!this.steamIdAbandonmentTweenMap.ContainsKey(steamId))
		{
			return;
		}
		this.steamIdAbandonmentTweenMap[steamId].Kill(false);
		this.steamIdAbandonmentTweenMap.Remove(steamId);
	}

	// Token: 0x06000402 RID: 1026 RVA: 0x00016724 File Offset: 0x00014924
	private void StopAllAbandonmentTimeouts()
	{
		foreach (Tween t in this.steamIdAbandonmentTweenMap.Values)
		{
			t.Kill(false);
		}
		this.steamIdAbandonmentTweenMap.Clear();
	}

	// Token: 0x06000403 RID: 1027 RVA: 0x000020D3 File Offset: 0x000002D3
	protected virtual void OnMatchStarted()
	{
	}

	// Token: 0x06000404 RID: 1028 RVA: 0x00016788 File Offset: 0x00014988
	protected virtual void OnMatchEnded(GameResult gameResult)
	{
		this.StopAllAbandonmentTimeouts();
	}

	// Token: 0x06000405 RID: 1029 RVA: 0x00016788 File Offset: 0x00014988
	protected virtual void OnMatchCancelled()
	{
		this.StopAllAbandonmentTimeouts();
	}

	// Token: 0x06000406 RID: 1030 RVA: 0x00016790 File Offset: 0x00014990
	protected virtual void OnSteamIdAbandonedMatch(string steamId)
	{
		this.IssueCooldown(steamId);
	}

	// Token: 0x06000407 RID: 1031 RVA: 0x0001679C File Offset: 0x0001499C
	protected override void OnWarmupEnded()
	{
		if (this.IsMatch)
		{
			string[] second = (from p in this.PlayerManager.GetPlayers(false)
			select p.SteamId.Value.ToString()).ToArray<string>();
			string[] array = BackendManager.ServerState.MatchData.SteamIds.Except(second).ToArray<string>();
			if (array.Length != 0)
			{
				this.ChatManager.Server_BroadcastChatMessage("Cancelling match due to players failing to join the match");
				foreach (string steamId in array)
				{
					this.IssueCooldown(steamId);
				}
				this.CancelMatch();
				return;
			}
		}
		base.OnWarmupEnded();
	}

	// Token: 0x06000408 RID: 1032 RVA: 0x00016843 File Offset: 0x00014A43
	protected override void OnPostGameStarted()
	{
		base.OnPostGameStarted();
		if (this.IsMatch)
		{
			this.EndMatch(null);
		}
	}

	// Token: 0x06000409 RID: 1033 RVA: 0x0001685C File Offset: 0x00014A5C
	protected override void OnPlayerJoined(Player player)
	{
		base.OnPlayerJoined(player);
		if (this.IsMatch)
		{
			this.StopAbandonmentTimeout(player.SteamId.Value.ToString());
		}
	}

	// Token: 0x0600040A RID: 1034 RVA: 0x00016898 File Offset: 0x00014A98
	protected override void OnPlayerLeft(Player player)
	{
		base.OnPlayerLeft(player);
		if (this.IsMatch)
		{
			if (this.GameManager.Phase == GamePhase.Warmup)
			{
				return;
			}
			this.StartAbandonmentTimeout(player.SteamId.Value.ToString(), 120f);
			this.ChatManager.Server_BroadcastChatMessage(string.Format("{0} has {1} seconds to reconnect before receiving a matchmaking cooldown", player.Username.Value, 120));
		}
	}

	// Token: 0x0400029D RID: 669
	private Dictionary<string, Tween> steamIdAbandonmentTweenMap = new Dictionary<string, Tween>();
}
