using System;
using System.Linq;

// Token: 0x02000073 RID: 115
public class CompetitiveGameMode<TConfig> : MatchableGameMode<!0> where TConfig : CompetitiveGameModeConfig, new()
{
	// Token: 0x060003F4 RID: 1012 RVA: 0x00016341 File Offset: 0x00014541
	public CompetitiveGameMode(string defaultConfigFilePath, string configFilePathCliArgument = null, string configCliArgument = null, string configEnvVariable = null) : base(defaultConfigFilePath, configFilePathCliArgument, configCliArgument, configEnvVariable)
	{
	}

	// Token: 0x060003F5 RID: 1013 RVA: 0x0001634E File Offset: 0x0001454E
	public override bool Initialize(Level level, ServerManager serverManager, GameManager gameManager, PlayerManager playerManager, PuckManager puckManager, ChatManager chatManager, ReplayManager replayManager)
	{
		if (!base.Initialize(level, serverManager, gameManager, playerManager, puckManager, chatManager, replayManager))
		{
			return false;
		}
		this.StartGame(true);
		return true;
	}

	// Token: 0x060003F6 RID: 1014 RVA: 0x00016370 File Offset: 0x00014570
	private PlayerTeam GetAssignedPlayerTeam(Player player)
	{
		foreach (PlayerTeam playerTeam in base.Config.teamAssignments.Keys)
		{
			if (base.Config.teamAssignments[playerTeam].Contains(player.SteamId.Value.ToString()))
			{
				return playerTeam;
			}
		}
		return PlayerTeam.None;
	}

	// Token: 0x060003F7 RID: 1015 RVA: 0x00016408 File Offset: 0x00014608
	protected override void OnPlayerJoined(Player player)
	{
		base.OnPlayerJoined(player);
		PlayerTeam assignedPlayerTeam = this.GetAssignedPlayerTeam(player);
		switch (assignedPlayerTeam)
		{
		case PlayerTeam.None:
		case PlayerTeam.Spectator:
		{
			PlayerTeam? team = new PlayerTeam?(PlayerTeam.Spectator);
			player.Server_SetGameState(null, team, null, null);
			if (base.CanPlayerEnterPhase(player, PlayerPhase.Spectate))
			{
				PlayerPhase? phase = new PlayerPhase?(PlayerPhase.Spectate);
				team = null;
				player.Server_SetGameState(phase, team, null, null);
			}
			break;
		}
		case PlayerTeam.Blue:
		case PlayerTeam.Red:
		{
			PlayerTeam? team = new PlayerTeam?(assignedPlayerTeam);
			player.Server_SetGameState(null, team, null, null);
			if (base.CanPlayerEnterPhase(player, PlayerPhase.PositionSelect))
			{
				PlayerPhase? phase2 = new PlayerPhase?(PlayerPhase.PositionSelect);
				team = null;
				player.Server_SetGameState(phase2, team, null, null);
				return;
			}
			break;
		}
		default:
			return;
		}
	}

	// Token: 0x060003F8 RID: 1016 RVA: 0x000164F9 File Offset: 0x000146F9
	protected override void OnPostGameEnded()
	{
		base.OnPostGameEnded();
		this.StartGame(true);
	}
}
