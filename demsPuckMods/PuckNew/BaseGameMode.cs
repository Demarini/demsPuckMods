using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Token: 0x02000071 RID: 113
public abstract class BaseGameMode<TConfig> : IGameMode where TConfig : BaseGameModeConfig, new()
{
	// Token: 0x17000069 RID: 105
	// (get) Token: 0x060003B2 RID: 946 RVA: 0x0001561F File Offset: 0x0001381F
	// (set) Token: 0x060003B3 RID: 947 RVA: 0x00015627 File Offset: 0x00013827
	public bool IsInitialized { get; set; }

	// Token: 0x1700006A RID: 106
	// (get) Token: 0x060003B4 RID: 948 RVA: 0x00015630 File Offset: 0x00013830
	// (set) Token: 0x060003B5 RID: 949 RVA: 0x00015638 File Offset: 0x00013838
	public TConfig Config { get; private set; }

	// Token: 0x060003B6 RID: 950 RVA: 0x00015641 File Offset: 0x00013841
	public BaseGameMode(string defaultConfigFilePath, string configFilePathCliArgument = null, string configCliArgument = null, string configEnvVariable = null)
	{
		this.defaultConfigFilePath = defaultConfigFilePath;
		this.configFilePathCliArgument = configFilePathCliArgument;
		this.configCliArgument = configCliArgument;
		this.configEnvVariable = configEnvVariable;
	}

	// Token: 0x060003B7 RID: 951 RVA: 0x00015668 File Offset: 0x00013868
	public virtual bool Initialize(Level level, ServerManager serverManager, GameManager gameManager, PlayerManager playerManager, PuckManager puckManager, ChatManager chatManager, ReplayManager replayManager)
	{
		if (this.IsInitialized)
		{
			return false;
		}
		this.IsInitialized = true;
		this.Level = level;
		this.ServerManager = serverManager;
		this.GameManager = gameManager;
		this.PlayerManager = playerManager;
		this.PuckManager = puckManager;
		this.ChatManager = chatManager;
		this.ReplayManager = replayManager;
		this.Config = this.LoadConfig(this.defaultConfigFilePath, this.configFilePathCliArgument, this.configCliArgument, this.configEnvVariable);
		this.SubscribeEvents();
		return true;
	}

	// Token: 0x060003B8 RID: 952 RVA: 0x000156E8 File Offset: 0x000138E8
	public virtual bool Dispose()
	{
		if (!this.IsInitialized)
		{
			return false;
		}
		this.IsInitialized = false;
		this.Level = null;
		this.GameManager = null;
		this.PlayerManager = null;
		this.PuckManager = null;
		this.ChatManager = null;
		this.ReplayManager = null;
		this.Config = default(!0);
		this.UnsubscribeEvents();
		return true;
	}

	// Token: 0x060003B9 RID: 953 RVA: 0x00015748 File Offset: 0x00013948
	protected virtual void SubscribeEvents()
	{
		EventManager.AddEventListener("Event_Everyone_OnGameStateChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnGameStateChanged));
		EventManager.AddEventListener("Event_Everyone_OnPlayerAdded", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerAdded));
		EventManager.AddEventListener("Event_Everyone_OnPlayerRemoved", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerRemoved));
		EventManager.AddEventListener("Event_Everyone_OnPlayerGameStateChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerGameStateChanged));
		EventManager.AddEventListener("Event_Everyone_OnPlayerPositionChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerPositionChanged));
		EventManager.AddEventListener("Event_Everyone_OnGoalScored", new Action<Dictionary<string, object>>(this.Event_Everyone_OnGoalScored));
		EventManager.AddEventListener("Event_Server_OnPuckEnterGoal", new Action<Dictionary<string, object>>(this.Event_Server_OnPuckEnterGoal));
		EventManager.AddEventListener("Event_Server_OnPlayerRequestTeamSelect", new Action<Dictionary<string, object>>(this.Event_Server_OnPlayerRequestTeamSelect));
		EventManager.AddEventListener("Event_Server_OnPlayerRequestTeam", new Action<Dictionary<string, object>>(this.Event_Server_OnPlayerRequestTeam));
		EventManager.AddEventListener("Event_Server_OnPlayerRequestPositionSelect", new Action<Dictionary<string, object>>(this.Event_Server_OnPlayerRequestPositionSelect));
		EventManager.AddEventListener("Event_Server_OnPlayerRequestPosition", new Action<Dictionary<string, object>>(this.Event_Server_OnPlayerRequestPosition));
		EventManager.AddEventListener("Event_Server_OnPlayerRequestHandedness", new Action<Dictionary<string, object>>(this.Event_Server_OnPlayerRequestHandedness));
		EventManager.AddEventListener("Event_Server_OnVoteSuccess", new Action<Dictionary<string, object>>(this.Event_Server_OnVoteSuccess));
	}

	// Token: 0x060003BA RID: 954 RVA: 0x00015874 File Offset: 0x00013A74
	protected virtual void UnsubscribeEvents()
	{
		EventManager.RemoveEventListener("Event_Everyone_OnGameStateChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnGameStateChanged));
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerAdded", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerAdded));
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerRemoved", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerRemoved));
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerGameStateChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerGameStateChanged));
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerPositionChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerPositionChanged));
		EventManager.RemoveEventListener("Event_Everyone_OnGoalScored", new Action<Dictionary<string, object>>(this.Event_Everyone_OnGoalScored));
		EventManager.RemoveEventListener("Event_Server_OnPuckEnterGoal", new Action<Dictionary<string, object>>(this.Event_Server_OnPuckEnterGoal));
		EventManager.RemoveEventListener("Event_Server_OnPlayerRequestTeamSelect", new Action<Dictionary<string, object>>(this.Event_Server_OnPlayerRequestTeamSelect));
		EventManager.RemoveEventListener("Event_Server_OnPlayerRequestTeam", new Action<Dictionary<string, object>>(this.Event_Server_OnPlayerRequestTeam));
		EventManager.RemoveEventListener("Event_Server_OnPlayerRequestPositionSelect", new Action<Dictionary<string, object>>(this.Event_Server_OnPlayerRequestPositionSelect));
		EventManager.RemoveEventListener("Event_Server_OnPlayerRequestPosition", new Action<Dictionary<string, object>>(this.Event_Server_OnPlayerRequestPosition));
		EventManager.RemoveEventListener("Event_Server_OnPlayerRequestHandedness", new Action<Dictionary<string, object>>(this.Event_Server_OnPlayerRequestHandedness));
		EventManager.RemoveEventListener("Event_Server_OnVoteSuccess", new Action<Dictionary<string, object>>(this.Event_Server_OnVoteSuccess));
	}

	// Token: 0x060003BB RID: 955 RVA: 0x000159A0 File Offset: 0x00013BA0
	private TConfig LoadConfig(string defaultFilePath, string filePathCliArgument = null, string cliArgument = null, string envVariable = null)
	{
		string environmentVariable = Environment.GetEnvironmentVariable(envVariable);
		string commandLineArgument = Utils.GetCommandLineArgument(cliArgument, null);
		if (!string.IsNullOrEmpty(commandLineArgument))
		{
			Debug.Log(string.Concat(new string[]
			{
				"[",
				base.GetType().Name,
				"] Deserializing config from CLI argument (",
				cliArgument,
				")"
			}));
			return ConfigUtils.LoadConfigFromSerializedString<TConfig>(commandLineArgument);
		}
		if (!string.IsNullOrEmpty(environmentVariable))
		{
			Debug.Log(string.Concat(new string[]
			{
				"[",
				base.GetType().Name,
				"] Deserializing config from environment variable (",
				envVariable,
				")"
			}));
			return ConfigUtils.LoadConfigFromSerializedString<TConfig>(environmentVariable);
		}
		string text = Utils.GetCommandLineArgument(filePathCliArgument, null) ?? defaultFilePath;
		Debug.Log(string.Concat(new string[]
		{
			"[",
			base.GetType().Name,
			"] Deserializing config from file (",
			text,
			")"
		}));
		return ConfigUtils.LoadConfigFromFile<TConfig>(text, true);
	}

	// Token: 0x060003BC RID: 956 RVA: 0x00015AA0 File Offset: 0x00013CA0
	protected void SendServerState()
	{
		Dictionary<string, object> value = new Dictionary<string, object>
		{
			{
				"phase",
				this.GameManager.GameState.Value.Phase
			},
			{
				"tick",
				this.GameManager.GameState.Value.Tick
			},
			{
				"period",
				this.GameManager.GameState.Value.Period
			},
			{
				"blueScore",
				this.GameManager.GameState.Value.BlueScore
			},
			{
				"redScore",
				this.GameManager.GameState.Value.RedScore
			},
			{
				"isOvertime",
				this.GameManager.GameState.Value.IsOvertime
			}
		};
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		foreach (Player player in this.PlayerManager.GetPlayers(false))
		{
			string text = player.SteamId.Value.ToString();
			Dictionary<string, object> dictionary2 = dictionary;
			string key = text;
			Dictionary<string, object> dictionary3 = new Dictionary<string, object>();
			dictionary3.Add("team", player.Team);
			dictionary3.Add("role", player.Role);
			string key2 = "position";
			PlayerPosition playerPosition = player.PlayerPosition;
			dictionary3.Add(key2, ((playerPosition != null) ? playerPosition.Name : null) ?? null);
			dictionary3.Add("goals", player.Goals.Value);
			dictionary3.Add("assists", player.Assists.Value);
			dictionary2[key] = dictionary3;
		}
		WebSocketManager.Emit("serverState", new Dictionary<string, object>
		{
			{
				"gameState",
				value
			},
			{
				"playerState",
				dictionary
			}
		}, null);
	}

	// Token: 0x060003BD RID: 957 RVA: 0x00015CB8 File Offset: 0x00013EB8
	protected virtual void ScoreGoal(PlayerTeam byTeam, Player goalPlayer, Player assistPlayer, Player secondAssistPlayer, Puck puck)
	{
		NetworkObjectReference goalPlayerNetworkObjectReference = new NetworkObjectReference((goalPlayer != null) ? goalPlayer.NetworkObject : null);
		NetworkObjectReference assistPlayerNetworkObjectReference = new NetworkObjectReference((assistPlayer != null) ? assistPlayer.NetworkObject : null);
		NetworkObjectReference secondAssistPlayerNetworkObjectReference = new NetworkObjectReference((secondAssistPlayer != null) ? secondAssistPlayer.NetworkObject : null);
		NetworkObjectReference puckNetworkObjectReference = new NetworkObjectReference((puck != null) ? puck.NetworkObject : null);
		this.GameManager.Server_NotifyGoalScoredRpc(byTeam, goalPlayerNetworkObjectReference, assistPlayerNetworkObjectReference, secondAssistPlayerNetworkObjectReference, puckNetworkObjectReference);
	}

	// Token: 0x060003BE RID: 958 RVA: 0x00015D28 File Offset: 0x00013F28
	protected virtual void OnGameStateChanged(GameState oldGameState, GameState newGameState)
	{
		if (oldGameState.Phase != newGameState.Phase)
		{
			this.OnGamePhaseStarted(newGameState.Phase);
			if (newGameState.Tick == 0)
			{
				this.OnGamePhaseEnded(newGameState.Phase);
			}
		}
		if (oldGameState.Tick != newGameState.Tick && newGameState.Tick == 0)
		{
			this.OnGamePhaseEnded(newGameState.Phase);
		}
	}

	// Token: 0x060003BF RID: 959 RVA: 0x00015D88 File Offset: 0x00013F88
	protected virtual void OnGamePhaseStarted(GamePhase gamePhase)
	{
		switch (gamePhase)
		{
		case GamePhase.Warmup:
			this.OnWarmupStarted();
			return;
		case GamePhase.PreGame:
			this.OnPreGameStarted();
			return;
		case GamePhase.FaceOff:
			this.OnFaceOffStarted();
			return;
		case GamePhase.Play:
			this.OnPlayStarted();
			return;
		case GamePhase.BlueScore:
			this.OnBlueScoreStarted();
			return;
		case GamePhase.RedScore:
			this.OnRedScoreStarted();
			return;
		case GamePhase.Replay:
			this.OnReplayStarted();
			return;
		case GamePhase.Intermission:
			this.OnIntermissionStarted();
			return;
		case GamePhase.GameOver:
			this.OnGameOverStarted();
			return;
		case GamePhase.PostGame:
			this.OnPostGameStarted();
			return;
		default:
			return;
		}
	}

	// Token: 0x060003C0 RID: 960 RVA: 0x000020D3 File Offset: 0x000002D3
	protected virtual void OnWarmupStarted()
	{
	}

	// Token: 0x060003C1 RID: 961 RVA: 0x000020D3 File Offset: 0x000002D3
	protected virtual void OnPreGameStarted()
	{
	}

	// Token: 0x060003C2 RID: 962 RVA: 0x000020D3 File Offset: 0x000002D3
	protected virtual void OnFaceOffStarted()
	{
	}

	// Token: 0x060003C3 RID: 963 RVA: 0x000020D3 File Offset: 0x000002D3
	protected virtual void OnBlueScoreStarted()
	{
	}

	// Token: 0x060003C4 RID: 964 RVA: 0x000020D3 File Offset: 0x000002D3
	protected virtual void OnRedScoreStarted()
	{
	}

	// Token: 0x060003C5 RID: 965 RVA: 0x000020D3 File Offset: 0x000002D3
	protected virtual void OnReplayStarted()
	{
	}

	// Token: 0x060003C6 RID: 966 RVA: 0x000020D3 File Offset: 0x000002D3
	protected virtual void OnIntermissionStarted()
	{
	}

	// Token: 0x060003C7 RID: 967 RVA: 0x000020D3 File Offset: 0x000002D3
	protected virtual void OnPlayStarted()
	{
	}

	// Token: 0x060003C8 RID: 968 RVA: 0x000020D3 File Offset: 0x000002D3
	protected virtual void OnGameOverStarted()
	{
	}

	// Token: 0x060003C9 RID: 969 RVA: 0x000020D3 File Offset: 0x000002D3
	protected virtual void OnPostGameStarted()
	{
	}

	// Token: 0x060003CA RID: 970 RVA: 0x00015E0C File Offset: 0x0001400C
	protected virtual void OnGamePhaseEnded(GamePhase gamePhase)
	{
		switch (gamePhase)
		{
		case GamePhase.Warmup:
			this.OnWarmupEnded();
			return;
		case GamePhase.PreGame:
			this.OnPreGameEnded();
			return;
		case GamePhase.FaceOff:
			this.OnFaceOffEnded();
			return;
		case GamePhase.Play:
			this.OnPlayEnded();
			return;
		case GamePhase.BlueScore:
			this.OnBlueScoreEnded();
			return;
		case GamePhase.RedScore:
			this.OnRedScoreEnded();
			return;
		case GamePhase.Replay:
			this.OnReplayEnded();
			return;
		case GamePhase.Intermission:
			this.OnIntermissionEnded();
			return;
		case GamePhase.GameOver:
			this.OnGameOverEnded();
			return;
		case GamePhase.PostGame:
			this.OnPostGameEnded();
			return;
		default:
			return;
		}
	}

	// Token: 0x060003CB RID: 971 RVA: 0x000020D3 File Offset: 0x000002D3
	protected virtual void OnWarmupEnded()
	{
	}

	// Token: 0x060003CC RID: 972 RVA: 0x000020D3 File Offset: 0x000002D3
	protected virtual void OnPreGameEnded()
	{
	}

	// Token: 0x060003CD RID: 973 RVA: 0x000020D3 File Offset: 0x000002D3
	protected virtual void OnFaceOffEnded()
	{
	}

	// Token: 0x060003CE RID: 974 RVA: 0x000020D3 File Offset: 0x000002D3
	protected virtual void OnBlueScoreEnded()
	{
	}

	// Token: 0x060003CF RID: 975 RVA: 0x000020D3 File Offset: 0x000002D3
	protected virtual void OnRedScoreEnded()
	{
	}

	// Token: 0x060003D0 RID: 976 RVA: 0x000020D3 File Offset: 0x000002D3
	protected virtual void OnReplayEnded()
	{
	}

	// Token: 0x060003D1 RID: 977 RVA: 0x000020D3 File Offset: 0x000002D3
	protected virtual void OnIntermissionEnded()
	{
	}

	// Token: 0x060003D2 RID: 978 RVA: 0x000020D3 File Offset: 0x000002D3
	protected virtual void OnPlayEnded()
	{
	}

	// Token: 0x060003D3 RID: 979 RVA: 0x000020D3 File Offset: 0x000002D3
	protected virtual void OnGameOverEnded()
	{
	}

	// Token: 0x060003D4 RID: 980 RVA: 0x000020D3 File Offset: 0x000002D3
	protected virtual void OnPostGameEnded()
	{
	}

	// Token: 0x060003D5 RID: 981 RVA: 0x00015E90 File Offset: 0x00014090
	protected virtual void OnPlayerJoined(Player player)
	{
		string str = StringUtils.WrapInTeamColor(player.Username.Value.ToString(), player.Team);
		this.ChatManager.Server_BroadcastChatMessage(str + " has joined the server");
		this.ChatManager.Server_SendChatMessageToClients("Welcome to Puck! Use the <b>/help</b> command to display available server chat commands.", new ulong[]
		{
			player.OwnerClientId
		});
	}

	// Token: 0x060003D6 RID: 982 RVA: 0x00015EF8 File Offset: 0x000140F8
	protected virtual void OnPlayerLeft(Player player)
	{
		string str = StringUtils.WrapInTeamColor(player.Username.Value.ToString(), player.Team);
		this.ChatManager.Server_BroadcastChatMessage(str + " has left the server");
	}

	// Token: 0x060003D7 RID: 983 RVA: 0x00015F40 File Offset: 0x00014140
	protected virtual void OnPlayerGameStateChanged(Player player, PlayerGameState oldGameState, PlayerGameState newGameState)
	{
		if (oldGameState.Phase != newGameState.Phase)
		{
			this.OnPlayerPhaseChanged(player, oldGameState.Phase, newGameState.Phase);
		}
		if (oldGameState.Team != newGameState.Team)
		{
			this.OnPlayerTeamChanged(player, oldGameState.Team, newGameState.Team);
		}
		if (oldGameState.Role != newGameState.Role)
		{
			this.OnPlayerRoleChanged(player, oldGameState.Role, newGameState.Role);
		}
	}

	// Token: 0x060003D8 RID: 984 RVA: 0x000020D3 File Offset: 0x000002D3
	protected virtual void OnPlayerPhaseChanged(Player player, PlayerPhase oldPlayerPhase, PlayerPhase newPlayerPhase)
	{
	}

	// Token: 0x060003D9 RID: 985 RVA: 0x000020D3 File Offset: 0x000002D3
	protected virtual void OnPlayerTeamChanged(Player player, PlayerTeam oldPlayerTeam, PlayerTeam newPlayerTeam)
	{
	}

	// Token: 0x060003DA RID: 986 RVA: 0x000020D3 File Offset: 0x000002D3
	protected virtual void OnPlayerRoleChanged(Player player, PlayerRole oldPlayerRole, PlayerRole newPlayerRole)
	{
	}

	// Token: 0x060003DB RID: 987 RVA: 0x000020D3 File Offset: 0x000002D3
	protected virtual void OnPlayerPositionChanged(Player player, PlayerPosition oldPlayerPosition, PlayerPosition newPlayerPosition)
	{
	}

	// Token: 0x060003DC RID: 988 RVA: 0x000020D3 File Offset: 0x000002D3
	protected virtual void OnGoalScored(PlayerTeam byTeam, Player goalPlayer, Player assistPlayer, Player secondAssistPlayer, Puck puck)
	{
	}

	// Token: 0x060003DD RID: 989 RVA: 0x000020D3 File Offset: 0x000002D3
	protected virtual void OnPuckEnterGoal(PlayerTeam team, Puck puck)
	{
	}

	// Token: 0x060003DE RID: 990 RVA: 0x000020D3 File Offset: 0x000002D3
	protected virtual void OnPlayerRequestTeamSelect(Player player)
	{
	}

	// Token: 0x060003DF RID: 991 RVA: 0x000020D3 File Offset: 0x000002D3
	protected virtual void OnPlayerRequestTeam(Player player, PlayerTeam team)
	{
	}

	// Token: 0x060003E0 RID: 992 RVA: 0x000020D3 File Offset: 0x000002D3
	protected virtual void OnPlayerRequestPositionSelect(Player player)
	{
	}

	// Token: 0x060003E1 RID: 993 RVA: 0x000020D3 File Offset: 0x000002D3
	protected virtual void OnPlayerRequestPosition(Player player, PlayerPosition position)
	{
	}

	// Token: 0x060003E2 RID: 994 RVA: 0x00015FB0 File Offset: 0x000141B0
	protected virtual void OnPlayerRequestHandedness(Player player, PlayerHandedness handedness)
	{
		player.Handedness.Value = handedness;
	}

	// Token: 0x060003E3 RID: 995 RVA: 0x000020D3 File Offset: 0x000002D3
	protected virtual void OnVoteSuccess(Vote vote)
	{
	}

	// Token: 0x060003E4 RID: 996 RVA: 0x00015FC0 File Offset: 0x000141C0
	private void Event_Everyone_OnGameStateChanged(Dictionary<string, object> message)
	{
		GameState oldGameState = (GameState)message["oldGameState"];
		GameState newGameState = (GameState)message["newGameState"];
		this.OnGameStateChanged(oldGameState, newGameState);
	}

	// Token: 0x060003E5 RID: 997 RVA: 0x00015FF8 File Offset: 0x000141F8
	private void Event_Everyone_OnPlayerAdded(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		if (player.IsReplay.Value)
		{
			return;
		}
		this.OnPlayerJoined(player);
	}

	// Token: 0x060003E6 RID: 998 RVA: 0x0001602C File Offset: 0x0001422C
	private void Event_Everyone_OnPlayerRemoved(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		if (player.IsReplay.Value)
		{
			return;
		}
		this.OnPlayerLeft(player);
	}

	// Token: 0x060003E7 RID: 999 RVA: 0x00016060 File Offset: 0x00014260
	private void Event_Everyone_OnPlayerGameStateChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		PlayerGameState oldGameState = (PlayerGameState)message["oldGameState"];
		PlayerGameState newGameState = (PlayerGameState)message["newGameState"];
		if (player.IsReplay.Value)
		{
			return;
		}
		this.OnPlayerGameStateChanged(player, oldGameState, newGameState);
	}

	// Token: 0x060003E8 RID: 1000 RVA: 0x000160B8 File Offset: 0x000142B8
	private void Event_Everyone_OnPlayerPositionChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		PlayerPosition oldPlayerPosition = (PlayerPosition)message["oldPlayerPosition"];
		PlayerPosition newPlayerPosition = (PlayerPosition)message["newPlayerPosition"];
		if (player.IsReplay.Value)
		{
			return;
		}
		this.OnPlayerPositionChanged(player, oldPlayerPosition, newPlayerPosition);
	}

	// Token: 0x060003E9 RID: 1001 RVA: 0x00016110 File Offset: 0x00014310
	private void Event_Everyone_OnGoalScored(Dictionary<string, object> message)
	{
		PlayerTeam byTeam = (PlayerTeam)message["byTeam"];
		Player goalPlayer = (Player)message["goalPlayer"];
		Player assistPlayer = (Player)message["assistPlayer"];
		Player secondAssistPlayer = (Player)message["secondAssistPlayer"];
		Puck puck = (Puck)message["puck"];
		this.OnGoalScored(byTeam, goalPlayer, assistPlayer, secondAssistPlayer, puck);
	}

	// Token: 0x060003EA RID: 1002 RVA: 0x00016180 File Offset: 0x00014380
	private void Event_Server_OnPuckEnterGoal(Dictionary<string, object> message)
	{
		PlayerTeam team = (PlayerTeam)message["team"];
		Puck puck = (Puck)message["puck"];
		this.OnPuckEnterGoal(team, puck);
	}

	// Token: 0x060003EB RID: 1003 RVA: 0x000161B8 File Offset: 0x000143B8
	private void Event_Server_OnPlayerRequestTeamSelect(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		if (player.IsReplay.Value)
		{
			return;
		}
		this.OnPlayerRequestTeamSelect(player);
	}

	// Token: 0x060003EC RID: 1004 RVA: 0x000161EC File Offset: 0x000143EC
	private void Event_Server_OnPlayerRequestTeam(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		PlayerTeam team = (PlayerTeam)message["team"];
		if (player.IsReplay.Value)
		{
			return;
		}
		this.OnPlayerRequestTeam(player, team);
	}

	// Token: 0x060003ED RID: 1005 RVA: 0x00016234 File Offset: 0x00014434
	private void Event_Server_OnPlayerRequestPositionSelect(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		if (player.IsReplay.Value)
		{
			return;
		}
		this.OnPlayerRequestPositionSelect(player);
	}

	// Token: 0x060003EE RID: 1006 RVA: 0x00016268 File Offset: 0x00014468
	private void Event_Server_OnPlayerRequestPosition(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		PlayerPosition position = (PlayerPosition)message["playerPosition"];
		if (player.IsReplay.Value)
		{
			return;
		}
		this.OnPlayerRequestPosition(player, position);
	}

	// Token: 0x060003EF RID: 1007 RVA: 0x000162B0 File Offset: 0x000144B0
	private void Event_Server_OnPlayerRequestHandedness(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		PlayerHandedness handedness = (PlayerHandedness)message["handedness"];
		if (player.IsReplay.Value)
		{
			return;
		}
		this.OnPlayerRequestHandedness(player, handedness);
	}

	// Token: 0x060003F0 RID: 1008 RVA: 0x000162F8 File Offset: 0x000144F8
	private void Event_Server_OnVoteSuccess(Dictionary<string, object> message)
	{
		Vote vote = (Vote)message["vote"];
		this.OnVoteSuccess(vote);
	}

	// Token: 0x04000290 RID: 656
	public Level Level;

	// Token: 0x04000291 RID: 657
	public ServerManager ServerManager;

	// Token: 0x04000292 RID: 658
	public GameManager GameManager;

	// Token: 0x04000293 RID: 659
	public PlayerManager PlayerManager;

	// Token: 0x04000294 RID: 660
	public PuckManager PuckManager;

	// Token: 0x04000295 RID: 661
	public ChatManager ChatManager;

	// Token: 0x04000296 RID: 662
	public ReplayManager ReplayManager;

	// Token: 0x04000298 RID: 664
	private string defaultConfigFilePath;

	// Token: 0x04000299 RID: 665
	private string configFilePathCliArgument;

	// Token: 0x0400029A RID: 666
	private string configCliArgument;

	// Token: 0x0400029B RID: 667
	private string configEnvVariable;
}
