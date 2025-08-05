using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

// Token: 0x0200003B RID: 59
public class GameManager : NetworkBehaviourSingleton<GameManager>
{
	// Token: 0x17000023 RID: 35
	// (get) Token: 0x06000191 RID: 401 RVA: 0x00007EB5 File Offset: 0x000060B5
	public GamePhase Phase
	{
		get
		{
			return this.GameState.Value.Phase;
		}
	}

	// Token: 0x17000024 RID: 36
	// (get) Token: 0x06000192 RID: 402 RVA: 0x00007EC7 File Offset: 0x000060C7
	public bool IsOvertime
	{
		get
		{
			return this.GameState.Value.Period > 3;
		}
	}

	// Token: 0x17000025 RID: 37
	// (get) Token: 0x06000193 RID: 403 RVA: 0x0001312C File Offset: 0x0001132C
	public bool IsFirstFaceOff
	{
		get
		{
			return this.GameState.Value.Period == 1 && this.GameState.Value.Phase == GamePhase.FaceOff && this.GameState.Value.BlueScore == 0 && this.GameState.Value.RedScore == 0;
		}
	}

	// Token: 0x17000026 RID: 38
	// (get) Token: 0x06000194 RID: 404 RVA: 0x00007EDC File Offset: 0x000060DC
	public bool IsDebugGameStateCoroutineRunning
	{
		get
		{
			return this.debugGameStateCoroutine != null;
		}
	}

	// Token: 0x06000195 RID: 405 RVA: 0x00013188 File Offset: 0x00011388
	public override void OnNetworkSpawn()
	{
		MonoBehaviourSingleton<InputManager>.Instance.DebugGameStateAction.performed += this.OnDebugGameStateActionPerofrmed;
		MonoBehaviourSingleton<InputManager>.Instance.DebugShootAction.performed += this.OnDebugShootActionPerformed;
		this.GameState.Initialize(this);
		if (base.IsServer)
		{
			this.GameState.Value = default(GameState);
		}
		NetworkVariable<GameState> gameState = this.GameState;
		gameState.OnValueChanged = (NetworkVariable<GameState>.OnValueChangedDelegate)Delegate.Combine(gameState.OnValueChanged, new NetworkVariable<GameState>.OnValueChangedDelegate(this.OnGameStateChanged));
		this.Client_InitializeNetworkVariables();
		base.OnNetworkSpawn();
	}

	// Token: 0x06000196 RID: 406 RVA: 0x00007EE7 File Offset: 0x000060E7
	protected override void OnNetworkSessionSynchronized()
	{
		this.Client_InitializeNetworkVariables();
		base.OnNetworkSessionSynchronized();
	}

	// Token: 0x06000197 RID: 407 RVA: 0x00013228 File Offset: 0x00011428
	public override void OnNetworkDespawn()
	{
		MonoBehaviourSingleton<InputManager>.Instance.DebugGameStateAction.performed -= this.OnDebugGameStateActionPerofrmed;
		MonoBehaviourSingleton<InputManager>.Instance.DebugShootAction.performed -= this.OnDebugShootActionPerformed;
		NetworkVariable<GameState> gameState = this.GameState;
		gameState.OnValueChanged = (NetworkVariable<GameState>.OnValueChangedDelegate)Delegate.Remove(gameState.OnValueChanged, new NetworkVariable<GameState>.OnValueChangedDelegate(this.OnGameStateChanged));
		this.GameState.Dispose();
		if (NetworkManager.Singleton.IsServer)
		{
			this.Server_StopGameStateTickCoroutine();
		}
		base.OnNetworkDespawn();
	}

	// Token: 0x06000198 RID: 408 RVA: 0x00007EF5 File Offset: 0x000060F5
	private void OnDebugGameStateActionPerofrmed(InputAction.CallbackContext context)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (this.IsDebugGameStateCoroutineRunning)
		{
			Debug.Log("[GameManager] Stopping debug game state coroutine");
			this.Server_StopDebugGameStateCoroutine();
			return;
		}
		Debug.Log("[GameManager] Starting debug game state coroutine");
		this.Server_StartDebugGameStateCoroutine(0.1f);
	}

	// Token: 0x06000199 RID: 409 RVA: 0x000132B8 File Offset: 0x000114B8
	private void OnDebugShootActionPerformed(InputAction.CallbackContext context)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		Player localPlayer = NetworkBehaviourSingleton<PlayerManager>.Instance.GetLocalPlayer();
		if (!localPlayer)
		{
			return;
		}
		PlayerBodyV2 playerBody = localPlayer.PlayerBody;
		if (!playerBody)
		{
			return;
		}
		NetworkBehaviourSingleton<PuckManager>.Instance.Server_SpawnPuck(playerBody.transform.position + playerBody.transform.forward * 2.25f + Vector3.up * 0.1f, Quaternion.identity, Vector3.zero, false);
	}

	// Token: 0x0600019A RID: 410 RVA: 0x00013348 File Offset: 0x00011548
	private void OnGameStateChanged(GameState oldGameState, GameState newGameState)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnGameStateChanged", new Dictionary<string, object>
		{
			{
				"oldGameState",
				oldGameState
			},
			{
				"newGameState",
				newGameState
			}
		});
		if (oldGameState.Phase != newGameState.Phase)
		{
			Debug.Log(string.Format("[GameManager] Game phase changed from {0} to {1}", oldGameState.Phase, newGameState.Phase));
			MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnGamePhaseChanged", new Dictionary<string, object>
			{
				{
					"oldGamePhase",
					oldGameState.Phase
				},
				{
					"newGamePhase",
					newGameState.Phase
				},
				{
					"gameState",
					newGameState
				},
				{
					"period",
					newGameState.Period
				},
				{
					"time",
					newGameState.Time
				},
				{
					"isOvertime",
					this.IsOvertime
				},
				{
					"isFirstFaceOff",
					this.IsFirstFaceOff
				}
			});
			if (newGameState.Phase == GamePhase.GameOver)
			{
				MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnGameOver", new Dictionary<string, object>
				{
					{
						"winningTeam",
						(this.GameState.Value.BlueScore > this.GameState.Value.RedScore) ? PlayerTeam.Blue : PlayerTeam.Red
					},
					{
						"blueScore",
						this.GameState.Value.BlueScore
					},
					{
						"redScore",
						this.GameState.Value.RedScore
					}
				});
			}
		}
	}

	// Token: 0x0600019B RID: 411 RVA: 0x00013500 File Offset: 0x00011700
	public void Server_UpdateGameState(GamePhase? phase = null, int? time = null, int? period = null, int? blueScore = null, int? redScore = null)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.GameState.Value = new GameState
		{
			Phase = ((phase == null) ? this.GameState.Value.Phase : phase.Value),
			Time = ((time == null) ? this.GameState.Value.Time : time.Value),
			Period = ((period == null) ? this.GameState.Value.Period : period.Value),
			BlueScore = ((blueScore == null) ? this.GameState.Value.BlueScore : blueScore.Value),
			RedScore = ((redScore == null) ? this.GameState.Value.RedScore : redScore.Value)
		};
	}

	// Token: 0x0600019C RID: 412 RVA: 0x000135FC File Offset: 0x000117FC
	public void Server_ResetGameState(bool resetPhase = false)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.Server_UpdateGameState(new GamePhase?(resetPhase ? GamePhase.None : this.GameState.Value.Phase), new int?(0), new int?(0), new int?(0), new int?(0));
	}

	// Token: 0x0600019D RID: 413 RVA: 0x00013650 File Offset: 0x00011850
	public void Server_SetPhase(GamePhase phase, int time = -1)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (phase == GamePhase.Playing)
		{
			this.Server_UpdateGameState(new GamePhase?(phase), new int?((time == -1) ? this.remainingPlayTime : time), null, null, null);
			return;
		}
		this.Server_UpdateGameState(new GamePhase?(phase), new int?((time == -1) ? this.PhaseDurationMap[phase] : time), null, null, null);
	}

	// Token: 0x0600019E RID: 414 RVA: 0x000136E8 File Offset: 0x000118E8
	public void Server_StartGame(bool warmup = true, int warmupTime = -1)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.Server_ResetGameState(false);
		int? period = new int?(1);
		int? blueScore = new int?(0);
		int? redScore = new int?(0);
		this.Server_UpdateGameState(null, null, period, blueScore, redScore);
		if (warmup)
		{
			this.Server_SetPhase(GamePhase.Warmup, warmupTime);
		}
		else
		{
			this.Server_SetPhase(GamePhase.FaceOff, -1);
		}
		this.remainingPlayTime = this.PhaseDurationMap[GamePhase.Playing];
	}

	// Token: 0x0600019F RID: 415 RVA: 0x00007F32 File Offset: 0x00006132
	public void Server_GameOver()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.Server_SetPhase(GamePhase.GameOver, -1);
	}

	// Token: 0x060001A0 RID: 416 RVA: 0x00007F49 File Offset: 0x00006149
	public void Server_Pause()
	{
		this.Server_StopGameStateTickCoroutine();
	}

	// Token: 0x060001A1 RID: 417 RVA: 0x00007F51 File Offset: 0x00006151
	public void Server_Resume()
	{
		this.Server_StartGameStateTickCoroutine();
	}

	// Token: 0x060001A2 RID: 418 RVA: 0x00007F59 File Offset: 0x00006159
	public void Server_StartGameStateTickCoroutine()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.Server_StopGameStateTickCoroutine();
		this.gameStateTickCoroutine = this.Server_IGameStateTick();
		base.StartCoroutine(this.gameStateTickCoroutine);
	}

	// Token: 0x060001A3 RID: 419 RVA: 0x00007F87 File Offset: 0x00006187
	public void Server_StopGameStateTickCoroutine()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (this.gameStateTickCoroutine == null)
		{
			return;
		}
		base.StopCoroutine(this.gameStateTickCoroutine);
	}

	// Token: 0x060001A4 RID: 420 RVA: 0x00007FAB File Offset: 0x000061AB
	private IEnumerator Server_IGameStateTick()
	{
		yield return new WaitForSeconds(1f);
		this.Server_OnGameStateTick();
		this.Server_StartGameStateTickCoroutine();
		yield break;
	}

	// Token: 0x060001A5 RID: 421 RVA: 0x00007FBA File Offset: 0x000061BA
	public void Server_StartDebugGameStateCoroutine(float delay)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.Server_StopDebugGameStateCoroutine();
		this.debugGameStateCoroutine = this.Server_IDebugGameState(delay);
		base.StartCoroutine(this.debugGameStateCoroutine);
	}

	// Token: 0x060001A6 RID: 422 RVA: 0x00007FE9 File Offset: 0x000061E9
	public void Server_StopDebugGameStateCoroutine()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (this.debugGameStateCoroutine != null)
		{
			base.StopCoroutine(this.debugGameStateCoroutine);
		}
		this.debugGameStateCoroutine = null;
	}

	// Token: 0x060001A7 RID: 423 RVA: 0x00008013 File Offset: 0x00006213
	private IEnumerator Server_IDebugGameState(float delay)
	{
		this.Server_SetPhase(GamePhase.FaceOff, -1);
		yield return new WaitForSeconds(delay);
		this.Server_SetPhase(GamePhase.Warmup, -1);
		this.debugGameStateCoroutine = this.Server_IDebugGameState(delay);
		base.StartCoroutine(this.debugGameStateCoroutine);
		yield break;
	}

	// Token: 0x060001A8 RID: 424 RVA: 0x00013764 File Offset: 0x00011964
	private void Server_OnGameStateTick()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		int? num;
		if (this.GameState.Value.Time <= 0)
		{
			switch (this.GameState.Value.Phase)
			{
			case GamePhase.Warmup:
				this.Server_SetPhase(GamePhase.FaceOff, -1);
				break;
			case GamePhase.FaceOff:
				this.Server_SetPhase(GamePhase.Playing, -1);
				break;
			case GamePhase.Playing:
				if (this.GameState.Value.Period < 3)
				{
					this.Server_SetPhase(GamePhase.PeriodOver, -1);
					num = new int?(this.GameState.Value.Period + 1);
					this.Server_UpdateGameState(null, null, num, null, null);
				}
				else if (this.GameState.Value.BlueScore == this.GameState.Value.RedScore)
				{
					this.Server_SetPhase(GamePhase.PeriodOver, -1);
					num = new int?(this.GameState.Value.Period + 1);
					this.Server_UpdateGameState(null, null, num, null, null);
				}
				else
				{
					this.Server_GameOver();
				}
				break;
			case GamePhase.BlueScore:
			case GamePhase.RedScore:
				this.Server_SetPhase(GamePhase.Replay, -1);
				break;
			case GamePhase.Replay:
				if (this.IsOvertime)
				{
					this.Server_GameOver();
				}
				else
				{
					this.Server_SetPhase(GamePhase.FaceOff, -1);
				}
				break;
			case GamePhase.PeriodOver:
				this.remainingPlayTime = this.PhaseDurationMap[GamePhase.Playing];
				this.Server_SetPhase(GamePhase.FaceOff, -1);
				break;
			case GamePhase.GameOver:
				this.Server_StartGame(true, NetworkBehaviourSingleton<PlayerManager>.Instance.IsEnoughPlayersForPlaying() ? 60 : -1);
				break;
			}
		}
		num = new int?(this.GameState.Value.Time - 1);
		this.Server_UpdateGameState(null, num, null, null, null);
	}

	// Token: 0x060001A9 RID: 425 RVA: 0x00013964 File Offset: 0x00011B64
	public void Server_GoalScored(PlayerTeam team, Player lastPlayer, Player goalPlayer, Player assistPlayer, Player secondAssistPlayer, Puck puck)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.remainingPlayTime = this.GameState.Value.Time;
		if (team != PlayerTeam.Blue)
		{
			if (team == PlayerTeam.Red)
			{
				int? num = new int?(this.GameState.Value.RedScore + 1);
				this.Server_UpdateGameState(null, null, null, null, num);
				this.Server_SetPhase(GamePhase.RedScore, -1);
			}
		}
		else
		{
			int? num = new int?(this.GameState.Value.BlueScore + 1);
			this.Server_UpdateGameState(null, null, null, num, null);
			this.Server_SetPhase(GamePhase.BlueScore, -1);
		}
		this.Server_GoalScoredRpc(team, lastPlayer != null, lastPlayer ? lastPlayer.OwnerClientId : 0UL, goalPlayer != null, goalPlayer ? goalPlayer.OwnerClientId : 0UL, assistPlayer != null, assistPlayer ? assistPlayer.OwnerClientId : 0UL, secondAssistPlayer != null, secondAssistPlayer ? secondAssistPlayer.OwnerClientId : 0UL, puck.Speed, puck.ShotSpeed, default(RpcParams));
		Debug.Log(string.Format("[GameManager] Goal scored by {0} ({1}-{2})", team, this.GameState.Value.BlueScore, this.GameState.Value.RedScore));
		Debug.Log(string.Format("[GameManager] Goal player: {0} ({1})", (goalPlayer != null) ? new FixedString32Bytes?(goalPlayer.Username.Value) : null, (goalPlayer != null) ? new ulong?(goalPlayer.OwnerClientId) : null));
		Debug.Log(string.Format("[GameManager] Assist player: {0} ({1})", (assistPlayer != null) ? new FixedString32Bytes?(assistPlayer.Username.Value) : null, (assistPlayer != null) ? new ulong?(assistPlayer.OwnerClientId) : null));
		Debug.Log(string.Format("[GameManager] Second assist player: {0} ({1})", (secondAssistPlayer != null) ? new FixedString32Bytes?(secondAssistPlayer.Username.Value) : null, (secondAssistPlayer != null) ? new ulong?(secondAssistPlayer.OwnerClientId) : null));
	}

	// Token: 0x060001AA RID: 426 RVA: 0x00013C00 File Offset: 0x00011E00
	[Rpc(SendTo.Everyone)]
	public void Server_GoalScoredRpc(PlayerTeam team, bool hasLastPlayer, ulong lastPlayerClientId, bool hasGoalPlayer, ulong goalPlayerClientId, bool hasAssistPlayer, ulong assistPlayerClientId, bool hasSecondAssistPlayer, ulong secondAssistPlayerClientId, float speedAcrossLine, float highestSpeedSinceStick, RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 460221987U;
			RpcAttribute.RpcAttributeParams attributeParams = default(RpcAttribute.RpcAttributeParams);
			FastBufferWriter writer = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Everyone, RpcDelivery.Reliable);
			writer.WriteValueSafe<PlayerTeam>(team, default(FastBufferWriter.ForEnums));
			writer.WriteValueSafe<bool>(hasLastPlayer, default(FastBufferWriter.ForPrimitives));
			BytePacker.WriteValueBitPacked(writer, lastPlayerClientId);
			writer.WriteValueSafe<bool>(hasGoalPlayer, default(FastBufferWriter.ForPrimitives));
			BytePacker.WriteValueBitPacked(writer, goalPlayerClientId);
			writer.WriteValueSafe<bool>(hasAssistPlayer, default(FastBufferWriter.ForPrimitives));
			BytePacker.WriteValueBitPacked(writer, assistPlayerClientId);
			writer.WriteValueSafe<bool>(hasSecondAssistPlayer, default(FastBufferWriter.ForPrimitives));
			BytePacker.WriteValueBitPacked(writer, secondAssistPlayerClientId);
			writer.WriteValueSafe<float>(speedAcrossLine, default(FastBufferWriter.ForPrimitives));
			writer.WriteValueSafe<float>(highestSpeedSinceStick, default(FastBufferWriter.ForPrimitives));
			base.__endSendRpc(ref writer, 460221987U, rpcParams, attributeParams, SendTo.Everyone, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnGoalScored", new Dictionary<string, object>
		{
			{
				"team",
				team
			},
			{
				"hasLastPlayer",
				hasLastPlayer
			},
			{
				"lastPlayerClientId",
				lastPlayerClientId
			},
			{
				"hasGoalPlayer",
				hasGoalPlayer
			},
			{
				"goalPlayerClientId",
				goalPlayerClientId
			},
			{
				"hasAssistPlayer",
				hasAssistPlayer
			},
			{
				"assistPlayerClientId",
				assistPlayerClientId
			},
			{
				"hasSecondAssistPlayer",
				hasSecondAssistPlayer
			},
			{
				"secondAssistPlayerClientId",
				secondAssistPlayerClientId
			},
			{
				"speedAcrossLine",
				speedAcrossLine
			},
			{
				"highestSpeedSinceStick",
				highestSpeedSinceStick
			}
		});
	}

	// Token: 0x060001AB RID: 427 RVA: 0x00008029 File Offset: 0x00006229
	public void Client_InitializeNetworkVariables()
	{
		this.OnGameStateChanged(this.GameState.Value, this.GameState.Value);
	}

	// Token: 0x060001AD RID: 429 RVA: 0x00013EB4 File Offset: 0x000120B4
	protected override void __initializeVariables()
	{
		bool flag = this.GameState == null;
		if (flag)
		{
			throw new Exception("GameManager.GameState cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.GameState.Initialize(this);
		base.__nameNetworkVariable(this.GameState, "GameState");
		this.NetworkVariableFields.Add(this.GameState);
		base.__initializeVariables();
	}

	// Token: 0x060001AE RID: 430 RVA: 0x00008047 File Offset: 0x00006247
	protected override void __initializeRpcs()
	{
		base.__registerRpc(460221987U, new NetworkBehaviour.RpcReceiveHandler(GameManager.__rpc_handler_460221987), "Server_GoalScoredRpc");
		base.__initializeRpcs();
	}

	// Token: 0x060001AF RID: 431 RVA: 0x00013F18 File Offset: 0x00012118
	private static void __rpc_handler_460221987(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		PlayerTeam team;
		reader.ReadValueSafe<PlayerTeam>(out team, default(FastBufferWriter.ForEnums));
		bool hasLastPlayer;
		reader.ReadValueSafe<bool>(out hasLastPlayer, default(FastBufferWriter.ForPrimitives));
		ulong lastPlayerClientId;
		ByteUnpacker.ReadValueBitPacked(reader, out lastPlayerClientId);
		bool hasGoalPlayer;
		reader.ReadValueSafe<bool>(out hasGoalPlayer, default(FastBufferWriter.ForPrimitives));
		ulong goalPlayerClientId;
		ByteUnpacker.ReadValueBitPacked(reader, out goalPlayerClientId);
		bool hasAssistPlayer;
		reader.ReadValueSafe<bool>(out hasAssistPlayer, default(FastBufferWriter.ForPrimitives));
		ulong assistPlayerClientId;
		ByteUnpacker.ReadValueBitPacked(reader, out assistPlayerClientId);
		bool hasSecondAssistPlayer;
		reader.ReadValueSafe<bool>(out hasSecondAssistPlayer, default(FastBufferWriter.ForPrimitives));
		ulong secondAssistPlayerClientId;
		ByteUnpacker.ReadValueBitPacked(reader, out secondAssistPlayerClientId);
		float speedAcrossLine;
		reader.ReadValueSafe<float>(out speedAcrossLine, default(FastBufferWriter.ForPrimitives));
		float highestSpeedSinceStick;
		reader.ReadValueSafe<float>(out highestSpeedSinceStick, default(FastBufferWriter.ForPrimitives));
		RpcParams ext = rpcParams.Ext;
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((GameManager)target).Server_GoalScoredRpc(team, hasLastPlayer, lastPlayerClientId, hasGoalPlayer, goalPlayerClientId, hasAssistPlayer, assistPlayerClientId, hasSecondAssistPlayer, secondAssistPlayerClientId, speedAcrossLine, highestSpeedSinceStick, ext);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x060001B0 RID: 432 RVA: 0x0000806D File Offset: 0x0000626D
	protected internal override string __getTypeName()
	{
		return "GameManager";
	}

	// Token: 0x040000E1 RID: 225
	public NetworkVariable<GameState> GameState = new NetworkVariable<GameState>(default(GameState), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	// Token: 0x040000E2 RID: 226
	public Dictionary<GamePhase, int> PhaseDurationMap = new Dictionary<GamePhase, int>();

	// Token: 0x040000E3 RID: 227
	private IEnumerator gameStateTickCoroutine;

	// Token: 0x040000E4 RID: 228
	private IEnumerator debugGameStateCoroutine;

	// Token: 0x040000E5 RID: 229
	private int remainingPlayTime;
}
