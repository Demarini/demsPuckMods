using System;

// Token: 0x02000078 RID: 120
public class PublicGameMode<TConfig> : StandardGameMode<!0> where TConfig : PublicGameModeConfig, new()
{
	// Token: 0x06000412 RID: 1042 RVA: 0x00016980 File Offset: 0x00014B80
	public PublicGameMode(string defaultConfigFilePath, string configFilePathCliArgument = null, string configCliArgument = null, string configEnvVariable = null) : base(defaultConfigFilePath, configFilePathCliArgument, configCliArgument, configEnvVariable)
	{
	}

	// Token: 0x06000413 RID: 1043 RVA: 0x0001698D File Offset: 0x00014B8D
	public override bool Initialize(Level level, ServerManager serverManager, GameManager gameManager, PlayerManager playerManager, PuckManager puckManager, ChatManager chatManager, ReplayManager replayManager)
	{
		if (!base.Initialize(level, serverManager, gameManager, playerManager, puckManager, chatManager, replayManager))
		{
			return false;
		}
		this.StartGame(true);
		return true;
	}

	// Token: 0x06000414 RID: 1044 RVA: 0x000169AC File Offset: 0x00014BAC
	protected override void OnWarmupEnded()
	{
		if (this.PlayerManager.GetPlayersByTeam(PlayerTeam.Blue, false).Count == 0 || this.PlayerManager.GetPlayersByTeam(PlayerTeam.Red, false).Count == 0)
		{
			this.GameManager.Server_SetGameState(new GamePhase?(GamePhase.Warmup), new int?(base.Config.phaseDurationMap[GamePhase.Warmup]), null, null, null, null);
			this.ChatManager.Server_BroadcastChatMessage("Not enough players to start the game. Extending warmup...");
			return;
		}
		base.OnWarmupEnded();
	}

	// Token: 0x06000415 RID: 1045 RVA: 0x000164F9 File Offset: 0x000146F9
	protected override void OnPostGameEnded()
	{
		base.OnPostGameEnded();
		this.StartGame(true);
	}

	// Token: 0x06000416 RID: 1046 RVA: 0x00016A48 File Offset: 0x00014C48
	protected override void OnPlayerJoined(Player player)
	{
		base.OnPlayerJoined(player);
		if (base.CanPlayerEnterPhase(player, PlayerPhase.TeamSelect))
		{
			player.Server_SetGameState(new PlayerPhase?(PlayerPhase.TeamSelect), null, null, null);
		}
	}

	// Token: 0x06000417 RID: 1047 RVA: 0x00016A90 File Offset: 0x00014C90
	protected override void OnPlayerRequestTeamSelect(Player player)
	{
		base.OnPlayerRequestTeamSelect(player);
		if (base.CanPlayerEnterPhase(player, PlayerPhase.TeamSelect))
		{
			player.Server_SetGameState(new PlayerPhase?(PlayerPhase.TeamSelect), new PlayerTeam?(PlayerTeam.None), null, null);
		}
	}

	// Token: 0x06000418 RID: 1048 RVA: 0x00016AD4 File Offset: 0x00014CD4
	protected override void OnPlayerRequestTeam(Player player, PlayerTeam team)
	{
		base.OnPlayerRequestTeam(player, team);
		if (team - PlayerTeam.Blue > 1)
		{
			if (team != PlayerTeam.Spectator)
			{
				return;
			}
			PlayerTeam? team2 = new PlayerTeam?(team);
			player.Server_SetGameState(null, team2, null, null);
			if (base.CanPlayerEnterPhase(player, PlayerPhase.Spectate))
			{
				PlayerPhase? phase = new PlayerPhase?(PlayerPhase.Spectate);
				team2 = null;
				player.Server_SetGameState(phase, team2, null, null);
			}
		}
		else
		{
			PlayerTeam? team2 = new PlayerTeam?(team);
			player.Server_SetGameState(null, team2, null, null);
			if (base.CanPlayerEnterPhase(player, PlayerPhase.PositionSelect))
			{
				PlayerPhase? phase2 = new PlayerPhase?(PlayerPhase.PositionSelect);
				team2 = null;
				player.Server_SetGameState(phase2, team2, null, null);
				return;
			}
		}
	}

	// Token: 0x06000419 RID: 1049 RVA: 0x00016BB0 File Offset: 0x00014DB0
	protected override void OnVoteSuccess(Vote vote)
	{
		base.OnVoteSuccess(vote);
		VoteType type = vote.Type;
		if (type == VoteType.Start)
		{
			this.StartGame(false);
			return;
		}
		if (type != VoteType.Warmup)
		{
			return;
		}
		this.StartGame(true);
	}
}
