using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

// Token: 0x0200007C RID: 124
public class StandardGameMode<TConfig> : BaseGameMode<!0> where TConfig : StandardGameModeConfig, new()
{
	// Token: 0x0600042F RID: 1071 RVA: 0x00016D25 File Offset: 0x00014F25
	public StandardGameMode(string defaultConfigFilePath, string configFilePathCliArgument = null, string configCliArgument = null, string configEnvVariable = null) : base(defaultConfigFilePath, configFilePathCliArgument, configCliArgument, configEnvVariable)
	{
	}

	// Token: 0x06000430 RID: 1072 RVA: 0x00016D40 File Offset: 0x00014F40
	protected bool CanPlayerEnterPhase(Player player, PlayerPhase phase)
	{
		if (player.Phase == phase)
		{
			return false;
		}
		switch (phase)
		{
		case PlayerPhase.TeamSelect:
			return true;
		case PlayerPhase.PositionSelect:
			return player.Team == PlayerTeam.Blue || player.Team == PlayerTeam.Red;
		case PlayerPhase.Play:
		case PlayerPhase.Replay:
			return (player.Team == PlayerTeam.Blue || player.Team == PlayerTeam.Red) && (player.Role == PlayerRole.Attacker || player.Role == PlayerRole.Goalie) && player.PlayerPosition != null;
		case PlayerPhase.Spectate:
			return player.Team == PlayerTeam.Spectator;
		default:
			return false;
		}
	}

	// Token: 0x06000431 RID: 1073 RVA: 0x00016DCC File Offset: 0x00014FCC
	protected virtual void PreparePlayersForGamePhase(GamePhase gamePhase)
	{
		foreach (Player player in this.PlayerManager.GetPlayers(false))
		{
			this.PreparePlayerForGamePhase(player, gamePhase);
		}
	}

	// Token: 0x06000432 RID: 1074 RVA: 0x00016E28 File Offset: 0x00015028
	protected virtual void PreparePlayerForGamePhase(Player player, GamePhase gamePhase)
	{
		player.Server_CancelDelayedGameState();
		switch (gamePhase)
		{
		case GamePhase.Warmup:
			if (this.CanPlayerEnterPhase(player, PlayerPhase.Play))
			{
				PlayerPhase? phase = new PlayerPhase?(PlayerPhase.Play);
				PlayerTeam? team = null;
				PlayerRole? role = null;
				float? delay = null;
				player.Server_SetGameState(phase, team, role, delay);
				return;
			}
			break;
		case GamePhase.PreGame:
			if (this.CanPlayerEnterPhase(player, PlayerPhase.PositionSelect))
			{
				PlayerPhase? phase2 = new PlayerPhase?(PlayerPhase.PositionSelect);
				PlayerTeam? team2 = null;
				PlayerRole? role2 = null;
				float? delay = null;
				player.Server_SetGameState(phase2, team2, role2, delay);
				return;
			}
			break;
		case GamePhase.FaceOff:
			if (this.CanPlayerEnterPhase(player, PlayerPhase.Play))
			{
				PlayerPhase? phase3 = new PlayerPhase?(PlayerPhase.Play);
				PlayerTeam? team3 = null;
				PlayerRole? role3 = null;
				float? delay = null;
				player.Server_SetGameState(phase3, team3, role3, delay);
			}
			else if (player.Phase == PlayerPhase.Play && player.IsCharacterSpawned)
			{
				player.PlayerBody.Server_Teleport(player.PlayerPosition.transform.position, player.PlayerPosition.transform.rotation);
			}
			if (player.IsCharacterSpawned)
			{
				player.PlayerBody.Server_Freeze();
				return;
			}
			break;
		case GamePhase.Play:
			if (this.CanPlayerEnterPhase(player, PlayerPhase.Play))
			{
				PlayerPhase? phase4 = new PlayerPhase?(PlayerPhase.Play);
				float? delay = new float?(base.Config.spawnDelay);
				player.Server_SetGameState(phase4, null, null, delay);
				this.ChatManager.Server_SendChatMessageToClients(string.Format("Spawning in {0} seconds...", base.Config.spawnDelay), new ulong[]
				{
					player.OwnerClientId
				});
			}
			if (player.IsCharacterSpawned)
			{
				player.PlayerBody.Server_Unfreeze();
				return;
			}
			break;
		case GamePhase.BlueScore:
		case GamePhase.RedScore:
			break;
		case GamePhase.Replay:
			if (this.CanPlayerEnterPhase(player, PlayerPhase.Replay))
			{
				PlayerPhase? phase5 = new PlayerPhase?(PlayerPhase.Replay);
				PlayerTeam? team4 = null;
				PlayerRole? role4 = null;
				float? delay = null;
				player.Server_SetGameState(phase5, team4, role4, delay);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06000433 RID: 1075 RVA: 0x00017010 File Offset: 0x00015210
	protected virtual void StartGame(bool warmup = false)
	{
		this.PlayerManager.GetPlayers(false).ForEach(delegate(Player player)
		{
			player.Server_ResetPoints();
			if (this.CanPlayerEnterPhase(player, PlayerPhase.PositionSelect))
			{
				player.Server_SetGameState(new PlayerPhase?(PlayerPhase.PositionSelect), null, null, null);
				if (player.PlayerPosition != null)
				{
					player.PlayerPosition.Server_Unclaim();
				}
			}
		});
		GamePhase gamePhase = warmup ? GamePhase.Warmup : GamePhase.PreGame;
		this.GameManager.Server_SetGameState(new GamePhase?(gamePhase), new int?(base.Config.phaseDurationMap[gamePhase]), new int?(1), new int?(0), new int?(0), new bool?(false));
		this.GameManager.Server_StartTicking();
		this.OnGameStarted();
	}

	// Token: 0x06000434 RID: 1076 RVA: 0x00017097 File Offset: 0x00015297
	protected virtual void StopGame()
	{
		this.GameManager.Server_StopTicking();
		this.OnGameStopped();
	}

	// Token: 0x06000435 RID: 1077 RVA: 0x000170AA File Offset: 0x000152AA
	protected virtual void ClearGameResult()
	{
		this.gameResult = new GameResult();
	}

	// Token: 0x06000436 RID: 1078 RVA: 0x000170B8 File Offset: 0x000152B8
	protected virtual void UpdateGameResult()
	{
		this.gameResult.winningTeam = ((this.GameManager.BlueScore > this.GameManager.RedScore) ? PlayerTeam.Blue : ((this.GameManager.BlueScore < this.GameManager.RedScore) ? PlayerTeam.Red : PlayerTeam.None));
		this.gameResult.blueScore = this.GameManager.BlueScore;
		this.gameResult.redScore = this.GameManager.RedScore;
		foreach (Player player in this.PlayerManager.GetPlayers(false))
		{
			string key = player.SteamId.Value.ToString();
			if (!this.gameResult.playerResults.ContainsKey(key))
			{
				PlayerResult playerResult = new PlayerResult();
				playerResult.goals = player.Goals.Value;
				playerResult.assists = player.Assists.Value;
				this.gameResult.playerResults.Add(key, playerResult);
			}
			else
			{
				PlayerResult playerResult2 = this.gameResult.playerResults[key];
				playerResult2.goals = player.Goals.Value;
				playerResult2.assists = player.Assists.Value;
			}
		}
	}

	// Token: 0x06000437 RID: 1079 RVA: 0x00017220 File Offset: 0x00015420
	protected virtual void OnGameStarted()
	{
		this.ClearGameResult();
		this.UpdateGameResult();
	}

	// Token: 0x06000438 RID: 1080 RVA: 0x000020D3 File Offset: 0x000002D3
	protected virtual void OnGameStopped()
	{
	}

	// Token: 0x06000439 RID: 1081 RVA: 0x00017230 File Offset: 0x00015430
	protected override void OnPlayerJoined(Player player)
	{
		base.OnPlayerJoined(player);
		string key = player.SteamId.Value.ToString();
		if (this.gameResult.playerResults.ContainsKey(key))
		{
			PlayerResult playerResult = this.gameResult.playerResults[key];
			player.Goals.Value = playerResult.goals;
			player.Assists.Value = playerResult.assists;
		}
	}

	// Token: 0x0600043A RID: 1082 RVA: 0x000172A5 File Offset: 0x000154A5
	protected override void OnGamePhaseStarted(GamePhase gamePhase)
	{
		base.OnGamePhaseStarted(gamePhase);
		this.PreparePlayersForGamePhase(gamePhase);
	}

	// Token: 0x0600043B RID: 1083 RVA: 0x000172B5 File Offset: 0x000154B5
	protected override void OnWarmupStarted()
	{
		base.OnWarmupStarted();
		this.PuckManager.Server_DespawnPucks(false);
		this.PuckManager.Server_SpawnPucksForPhase(GamePhase.Warmup);
	}

	// Token: 0x0600043C RID: 1084 RVA: 0x000172D5 File Offset: 0x000154D5
	protected override void OnPreGameStarted()
	{
		base.OnPreGameStarted();
		this.tickRemainder = base.Config.phaseDurationMap[GamePhase.Play];
	}

	// Token: 0x0600043D RID: 1085 RVA: 0x000172F9 File Offset: 0x000154F9
	protected override void OnFaceOffStarted()
	{
		base.OnFaceOffStarted();
		this.PuckManager.Server_DespawnPucks(false);
		this.ReplayManager.Server_StartRecording();
	}

	// Token: 0x0600043E RID: 1086 RVA: 0x00017318 File Offset: 0x00015518
	protected override void OnPlayStarted()
	{
		base.OnPlayStarted();
		this.PuckManager.Server_SpawnPucksForPhase(GamePhase.Play);
	}

	// Token: 0x0600043F RID: 1087 RVA: 0x0001732C File Offset: 0x0001552C
	protected override void OnBlueScoreStarted()
	{
		base.OnBlueScoreStarted();
		this.Level.Server_PlayerCheerSound((float)(base.Config.phaseDurationMap[GamePhase.BlueScore] + base.Config.phaseDurationMap[GamePhase.Replay]));
		this.Level.Server_PlayRedGoalSound();
	}

	// Token: 0x06000440 RID: 1088 RVA: 0x00017384 File Offset: 0x00015584
	protected override void OnRedScoreStarted()
	{
		base.OnRedScoreStarted();
		this.Level.Server_PlayerCheerSound((float)(base.Config.phaseDurationMap[GamePhase.RedScore] + base.Config.phaseDurationMap[GamePhase.Replay]));
		this.Level.Server_PlayBlueGoalSound();
	}

	// Token: 0x06000441 RID: 1089 RVA: 0x000173DB File Offset: 0x000155DB
	protected override void OnIntermissionStarted()
	{
		base.OnIntermissionStarted();
		this.Level.Server_PlayHornSound();
	}

	// Token: 0x06000442 RID: 1090 RVA: 0x000173EE File Offset: 0x000155EE
	protected override void OnReplayStarted()
	{
		base.OnReplayStarted();
		this.PuckManager.Server_DespawnPucks(false);
		this.ReplayManager.Server_StartReplaying((float)base.Config.phaseDurationMap[GamePhase.Replay]);
	}

	// Token: 0x06000443 RID: 1091 RVA: 0x00017424 File Offset: 0x00015624
	protected override void OnGameOverStarted()
	{
		base.OnGameOverStarted();
		PlayerTeam playerTeam = (this.GameManager.BlueScore > this.GameManager.RedScore) ? PlayerTeam.Blue : PlayerTeam.Red;
		int num = Mathf.Max(this.GameManager.BlueScore, this.GameManager.RedScore);
		int num2 = Mathf.Min(this.GameManager.BlueScore, this.GameManager.RedScore);
		this.ChatManager.Server_BroadcastChatMessage(string.Format("Game Over! {0} team wins with a score of {1} to {2}!", playerTeam, num, num2));
		this.Level.Server_PlayBlueGoalSound();
		this.Level.Server_PlayRedGoalSound();
		this.Level.Server_PlayHornSound();
		this.UpdateGameResult();
	}

	// Token: 0x06000444 RID: 1092 RVA: 0x000174DC File Offset: 0x000156DC
	protected override void OnWarmupEnded()
	{
		base.OnWarmupEnded();
		this.GameManager.Server_SetGameState(new GamePhase?(GamePhase.PreGame), new int?(base.Config.phaseDurationMap[GamePhase.PreGame]), null, null, null, null);
	}

	// Token: 0x06000445 RID: 1093 RVA: 0x00017540 File Offset: 0x00015740
	protected override void OnPreGameEnded()
	{
		base.OnPreGameEnded();
		this.GameManager.Server_SetGameState(new GamePhase?(GamePhase.FaceOff), new int?(base.Config.phaseDurationMap[GamePhase.FaceOff]), null, null, null, null);
	}

	// Token: 0x06000446 RID: 1094 RVA: 0x000175A4 File Offset: 0x000157A4
	protected override void OnFaceOffEnded()
	{
		base.OnFaceOffEnded();
		this.GameManager.Server_SetGameState(new GamePhase?(GamePhase.Play), new int?(this.tickRemainder), null, null, null, null);
	}

	// Token: 0x06000447 RID: 1095 RVA: 0x000175F8 File Offset: 0x000157F8
	protected override void OnBlueScoreEnded()
	{
		base.OnBlueScoreEnded();
		this.ReplayManager.Server_StopRecording();
		this.GameManager.Server_SetGameState(new GamePhase?(GamePhase.Replay), new int?(base.Config.phaseDurationMap[GamePhase.Replay]), null, null, null, null);
	}

	// Token: 0x06000448 RID: 1096 RVA: 0x00017668 File Offset: 0x00015868
	protected override void OnRedScoreEnded()
	{
		base.OnRedScoreEnded();
		this.ReplayManager.Server_StopRecording();
		this.GameManager.Server_SetGameState(new GamePhase?(GamePhase.Replay), new int?(base.Config.phaseDurationMap[GamePhase.Replay]), null, null, null, null);
	}

	// Token: 0x06000449 RID: 1097 RVA: 0x000176D8 File Offset: 0x000158D8
	protected override void OnReplayEnded()
	{
		base.OnReplayEnded();
		this.ReplayManager.Server_StopReplaying();
		if (this.GameManager.IsOvertime)
		{
			this.GameManager.Server_SetGameState(new GamePhase?(GamePhase.GameOver), new int?(base.Config.phaseDurationMap[GamePhase.GameOver]), null, null, null, null);
			return;
		}
		this.GameManager.Server_SetGameState(new GamePhase?(GamePhase.FaceOff), new int?(base.Config.phaseDurationMap[GamePhase.FaceOff]), null, null, null, null);
	}

	// Token: 0x0600044A RID: 1098 RVA: 0x000177A8 File Offset: 0x000159A8
	protected override void OnIntermissionEnded()
	{
		base.OnIntermissionEnded();
		this.GameManager.Server_SetGameState(new GamePhase?(GamePhase.FaceOff), new int?(base.Config.phaseDurationMap[GamePhase.FaceOff]), null, null, null, null);
	}

	// Token: 0x0600044B RID: 1099 RVA: 0x0001780C File Offset: 0x00015A0C
	protected override void OnPlayEnded()
	{
		base.OnPlayEnded();
		bool? isOvertime;
		if (this.GameManager.Period < base.Config.maxPeriods)
		{
			this.tickRemainder = base.Config.phaseDurationMap[GamePhase.Play];
			GameManager gameManager = this.GameManager;
			GamePhase? phase = new GamePhase?(GamePhase.Intermission);
			int? tick = new int?(base.Config.phaseDurationMap[GamePhase.Intermission]);
			int? period = new int?(this.GameManager.Period + 1);
			int? blueScore = null;
			int? redScore = null;
			isOvertime = null;
			gameManager.Server_SetGameState(phase, tick, period, blueScore, redScore, isOvertime);
			return;
		}
		if (this.GameManager.BlueScore == this.GameManager.RedScore)
		{
			this.tickRemainder = base.Config.phaseDurationMap[GamePhase.Play];
			GameManager gameManager2 = this.GameManager;
			GamePhase? phase2 = new GamePhase?(GamePhase.Intermission);
			int? tick2 = new int?(base.Config.phaseDurationMap[GamePhase.Intermission]);
			int? period2 = new int?(this.GameManager.Period + 1);
			isOvertime = new bool?(true);
			gameManager2.Server_SetGameState(phase2, tick2, period2, null, null, isOvertime);
			return;
		}
		GameManager gameManager3 = this.GameManager;
		GamePhase? phase3 = new GamePhase?(GamePhase.GameOver);
		int? tick3 = new int?(base.Config.phaseDurationMap[GamePhase.GameOver]);
		int? period3 = null;
		int? blueScore2 = null;
		int? redScore2 = null;
		isOvertime = null;
		gameManager3.Server_SetGameState(phase3, tick3, period3, blueScore2, redScore2, isOvertime);
	}

	// Token: 0x0600044C RID: 1100 RVA: 0x00017994 File Offset: 0x00015B94
	protected override void OnGameOverEnded()
	{
		base.OnGameOverEnded();
		this.GameManager.Server_SetGameState(new GamePhase?(GamePhase.PostGame), new int?(base.Config.phaseDurationMap[GamePhase.PostGame]), null, null, null, null);
	}

	// Token: 0x0600044D RID: 1101 RVA: 0x000179F9 File Offset: 0x00015BF9
	protected override void OnPlayerLeft(Player player)
	{
		base.OnPlayerLeft(player);
		if (player.PlayerPosition != null)
		{
			player.PlayerPosition.Server_Unclaim();
		}
	}

	// Token: 0x0600044E RID: 1102 RVA: 0x00017A1C File Offset: 0x00015C1C
	protected override void OnPlayerPhaseChanged(Player player, PlayerPhase oldPhase, PlayerPhase newPhase)
	{
		base.OnPlayerPhaseChanged(player, oldPhase, newPhase);
		switch (newPhase)
		{
		case PlayerPhase.PositionSelect:
			if (player.IsCharacterSpawned)
			{
				player.Server_DespawnCharacter();
				return;
			}
			break;
		case PlayerPhase.Play:
			player.Server_SpawnCharacter(player.PlayerPosition.transform.position, player.PlayerPosition.transform.rotation, player.Role);
			return;
		case PlayerPhase.Replay:
			if (player.IsCharacterSpawned)
			{
				player.Server_DespawnCharacter();
				return;
			}
			break;
		case PlayerPhase.Spectate:
			player.Server_SpawnSpectatorCamera(Vector3.zero, Quaternion.identity);
			return;
		default:
			if (player.IsCharacterSpawned)
			{
				player.Server_DespawnCharacter();
			}
			if (player.PlayerPosition != null)
			{
				player.PlayerPosition.Server_Unclaim();
			}
			break;
		}
	}

	// Token: 0x0600044F RID: 1103 RVA: 0x00017AD0 File Offset: 0x00015CD0
	protected override void OnPlayerPositionChanged(Player player, PlayerPosition oldPlayerPosition, PlayerPosition newPlayerPosition)
	{
		base.OnPlayerPositionChanged(player, oldPlayerPosition, newPlayerPosition);
		PlayerRole? role;
		if (newPlayerPosition == null)
		{
			role = new PlayerRole?(PlayerRole.None);
			player.Server_SetGameState(null, null, role, null);
			return;
		}
		role = new PlayerRole?(newPlayerPosition.Role);
		player.Server_SetGameState(null, null, role, null);
		this.PreparePlayerForGamePhase(player, this.GameManager.Phase);
	}

	// Token: 0x06000450 RID: 1104 RVA: 0x00017B5C File Offset: 0x00015D5C
	protected override void OnGoalScored(PlayerTeam byTeam, Player goalPlayer, Player assistPlayer, Player secondAssistPlayer, Puck puck)
	{
		base.OnGoalScored(byTeam, goalPlayer, assistPlayer, secondAssistPlayer, puck);
		this.tickRemainder = this.GameManager.Tick;
		if (goalPlayer)
		{
			goalPlayer.Server_GoalScored();
		}
		if (assistPlayer)
		{
			assistPlayer.Server_AssistScored();
		}
		if (secondAssistPlayer)
		{
			secondAssistPlayer.Server_AssistScored();
		}
		if (byTeam != PlayerTeam.Blue)
		{
			if (byTeam == PlayerTeam.Red)
			{
				GameManager gameManager = this.GameManager;
				GamePhase? phase = new GamePhase?(GamePhase.RedScore);
				int? tick = new int?(base.Config.phaseDurationMap[GamePhase.RedScore]);
				int? num = new int?(this.GameManager.GameState.Value.RedScore + 1);
				gameManager.Server_SetGameState(phase, tick, null, null, num, null);
			}
		}
		else
		{
			GameManager gameManager2 = this.GameManager;
			GamePhase? phase2 = new GamePhase?(GamePhase.BlueScore);
			int? tick2 = new int?(base.Config.phaseDurationMap[GamePhase.BlueScore]);
			int? num = new int?(this.GameManager.GameState.Value.BlueScore + 1);
			gameManager2.Server_SetGameState(phase2, tick2, null, num, null, null);
		}
		if (puck)
		{
			string text = puck.Speed.ToString(CultureInfo.InvariantCulture);
			string text2 = puck.ShotSpeed.ToString(CultureInfo.InvariantCulture);
			this.ChatManager.Server_BroadcastChatMessage(string.Concat(new string[]
			{
				"Goal scored! <b><united>",
				text,
				"</united> &units</b> across the line, <b><united>",
				text2,
				"</united> &units</b> from the stick."
			}));
		}
		else
		{
			this.ChatManager.Server_BroadcastChatMessage("Goal scored!");
		}
		this.UpdateGameResult();
	}

	// Token: 0x06000451 RID: 1105 RVA: 0x00017D10 File Offset: 0x00015F10
	protected override void OnPuckEnterGoal(PlayerTeam team, Puck puck)
	{
		base.OnPuckEnterGoal(team, puck);
		if (this.GameManager.Phase != GamePhase.Play)
		{
			return;
		}
		PlayerTeam playerTeam = (team == PlayerTeam.Blue) ? PlayerTeam.Red : PlayerTeam.Blue;
		List<KeyValuePair<Player, float>> playerCollisionsByTeam = puck.GetPlayerCollisionsByTeam(playerTeam);
		Player goalPlayer = null;
		Player assistPlayer = null;
		Player secondAssistPlayer = null;
		if (playerCollisionsByTeam.Count >= 1)
		{
			List<KeyValuePair<Player, float>> list = playerCollisionsByTeam;
			goalPlayer = list[list.Count - 1].Key;
			if (playerCollisionsByTeam.Count >= 2)
			{
				List<KeyValuePair<Player, float>> list2 = playerCollisionsByTeam;
				assistPlayer = list2[list2.Count - 2].Key;
			}
			if (playerCollisionsByTeam.Count >= 3)
			{
				List<KeyValuePair<Player, float>> list3 = playerCollisionsByTeam;
				secondAssistPlayer = list3[list3.Count - 3].Key;
			}
		}
		this.ScoreGoal(playerTeam, goalPlayer, assistPlayer, secondAssistPlayer, puck);
	}

	// Token: 0x06000452 RID: 1106 RVA: 0x00017DBC File Offset: 0x00015FBC
	protected override void OnPlayerRequestPositionSelect(Player player)
	{
		base.OnPlayerRequestPositionSelect(player);
		if (this.CanPlayerEnterPhase(player, PlayerPhase.PositionSelect))
		{
			player.Server_SetGameState(new PlayerPhase?(PlayerPhase.PositionSelect), null, null, null);
			if (player.PlayerPosition != null)
			{
				player.PlayerPosition.Server_Unclaim();
			}
		}
	}

	// Token: 0x06000453 RID: 1107 RVA: 0x00017E1C File Offset: 0x0001601C
	protected override void OnPlayerRequestPosition(Player player, PlayerPosition position)
	{
		base.OnPlayerRequestPosition(player, position);
		if (position.IsClaimed)
		{
			if (player == position.ClaimedByPlayer)
			{
				position.Server_Unclaim();
				return;
			}
		}
		else if (position.Team == player.Team)
		{
			PlayerPosition playerPosition = player.PlayerPosition;
			position.Server_Claim(player);
			if (playerPosition != null)
			{
				playerPosition.Server_Unclaim();
			}
		}
	}

	// Token: 0x040002AC RID: 684
	protected int tickRemainder;

	// Token: 0x040002AD RID: 685
	protected GameResult gameResult = new GameResult();
}
