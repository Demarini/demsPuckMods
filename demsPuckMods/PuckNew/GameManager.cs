using System;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;

// Token: 0x020000A3 RID: 163
public class GameManager : NetworkBehaviourSingleton<GameManager>
{
	// Token: 0x1700007C RID: 124
	// (get) Token: 0x06000525 RID: 1317 RVA: 0x0001C823 File Offset: 0x0001AA23
	[HideInInspector]
	public GamePhase Phase
	{
		get
		{
			return this.GameState.Value.Phase;
		}
	}

	// Token: 0x1700007D RID: 125
	// (get) Token: 0x06000526 RID: 1318 RVA: 0x0001C835 File Offset: 0x0001AA35
	[HideInInspector]
	public int Tick
	{
		get
		{
			return this.GameState.Value.Tick;
		}
	}

	// Token: 0x1700007E RID: 126
	// (get) Token: 0x06000527 RID: 1319 RVA: 0x0001C847 File Offset: 0x0001AA47
	[HideInInspector]
	public int Period
	{
		get
		{
			return this.GameState.Value.Period;
		}
	}

	// Token: 0x1700007F RID: 127
	// (get) Token: 0x06000528 RID: 1320 RVA: 0x0001C859 File Offset: 0x0001AA59
	[HideInInspector]
	public int BlueScore
	{
		get
		{
			return this.GameState.Value.BlueScore;
		}
	}

	// Token: 0x17000080 RID: 128
	// (get) Token: 0x06000529 RID: 1321 RVA: 0x0001C86B File Offset: 0x0001AA6B
	[HideInInspector]
	public int RedScore
	{
		get
		{
			return this.GameState.Value.RedScore;
		}
	}

	// Token: 0x17000081 RID: 129
	// (get) Token: 0x0600052A RID: 1322 RVA: 0x0001C87D File Offset: 0x0001AA7D
	[HideInInspector]
	public bool IsOvertime
	{
		get
		{
			return this.GameState.Value.IsOvertime;
		}
	}

	// Token: 0x0600052B RID: 1323 RVA: 0x0001C890 File Offset: 0x0001AA90
	protected override void OnNetworkPreSpawn(ref NetworkManager networkManager)
	{
		if (this.GameState == null)
		{
			this.GameState = new NetworkVariable<GameState>(default(GameState), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
		}
		if (networkManager.IsServer)
		{
			this.GameState.Value = default(GameState);
		}
		base.OnNetworkPreSpawn(ref networkManager);
	}

	// Token: 0x0600052C RID: 1324 RVA: 0x0001C8DF File Offset: 0x0001AADF
	public override void OnNetworkSpawn()
	{
		NetworkVariable<GameState> gameState = this.GameState;
		gameState.OnValueChanged = (NetworkVariable<GameState>.OnValueChangedDelegate)Delegate.Combine(gameState.OnValueChanged, new NetworkVariable<GameState>.OnValueChangedDelegate(this.OnGameStateChanged));
		base.OnNetworkSpawn();
	}

	// Token: 0x0600052D RID: 1325 RVA: 0x0001C90E File Offset: 0x0001AB0E
	protected override void OnNetworkPostSpawn()
	{
		if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsConnectedClient)
		{
			this.ProcessInitialNetworkVariableValues();
		}
		base.OnNetworkPostSpawn();
	}

	// Token: 0x0600052E RID: 1326 RVA: 0x0001C934 File Offset: 0x0001AB34
	protected override void OnNetworkSessionSynchronized()
	{
		this.ProcessInitialNetworkVariableValues();
		base.OnNetworkSessionSynchronized();
	}

	// Token: 0x0600052F RID: 1327 RVA: 0x0001C944 File Offset: 0x0001AB44
	public override void OnNetworkDespawn()
	{
		if (NetworkManager.Singleton.IsServer)
		{
			Tween tween = this.tickTween;
			if (tween != null)
			{
				tween.Kill(false);
			}
		}
		NetworkVariable<GameState> gameState = this.GameState;
		gameState.OnValueChanged = (NetworkVariable<GameState>.OnValueChangedDelegate)Delegate.Remove(gameState.OnValueChanged, new NetworkVariable<GameState>.OnValueChangedDelegate(this.OnGameStateChanged));
		base.OnNetworkDespawn();
	}

	// Token: 0x06000530 RID: 1328 RVA: 0x0001C99C File Offset: 0x0001AB9C
	private void ProcessInitialNetworkVariableValues()
	{
		this.OnGameStateChanged(default(GameState), this.GameState.Value);
	}

	// Token: 0x06000531 RID: 1329 RVA: 0x0001C9C3 File Offset: 0x0001ABC3
	private void OnGameStateChanged(GameState oldGameState, GameState newGameState)
	{
		EventManager.TriggerEvent("Event_Everyone_OnGameStateChanged", new Dictionary<string, object>
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
	}

	// Token: 0x06000532 RID: 1330 RVA: 0x0001C9F8 File Offset: 0x0001ABF8
	private void Server_Tick()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		int? tick = new int?(Mathf.Max(0, this.GameState.Value.Tick - 1));
		this.Server_SetGameState(null, tick, null, null, null, null);
	}

	// Token: 0x06000533 RID: 1331 RVA: 0x0001CA68 File Offset: 0x0001AC68
	public void Server_StartTicking()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		Tween tween = this.tickTween;
		if (tween != null)
		{
			tween.Kill(false);
		}
		this.tickTween = DOVirtual.DelayedCall(1f, new TweenCallback(this.Server_Tick), true).SetLoops(-1);
	}

	// Token: 0x06000534 RID: 1332 RVA: 0x0001CAB7 File Offset: 0x0001ACB7
	public void Server_StopTicking()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		Tween tween = this.tickTween;
		if (tween == null)
		{
			return;
		}
		tween.Kill(false);
	}

	// Token: 0x06000535 RID: 1333 RVA: 0x0001CAD8 File Offset: 0x0001ACD8
	public void Server_SetGameState(GamePhase? phase = null, int? tick = null, int? period = null, int? blueScore = null, int? redScore = null, bool? isOvertime = null)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		GameState value = new GameState
		{
			Phase = (phase ?? this.GameState.Value.Phase),
			Tick = (tick ?? this.GameState.Value.Tick),
			Period = (period ?? this.GameState.Value.Period),
			BlueScore = (blueScore ?? this.GameState.Value.BlueScore),
			RedScore = (redScore ?? this.GameState.Value.RedScore),
			IsOvertime = (isOvertime ?? this.GameState.Value.IsOvertime)
		};
		this.GameState.Value = value;
	}

	// Token: 0x06000536 RID: 1334 RVA: 0x0001CC10 File Offset: 0x0001AE10
	[Rpc(SendTo.Everyone, InvokePermission = RpcInvokePermission.Server, DeferLocal = true)]
	public void Server_NotifyGoalScoredRpc(PlayerTeam byTeam, NetworkObjectReference goalPlayerNetworkObjectReference, NetworkObjectReference assistPlayerNetworkObjectReference, NetworkObjectReference secondAssistPlayerNetworkObjectReference, NetworkObjectReference puckNetworkObjectReference)
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			Debug.LogError("Rpc methods can only be invoked after starting the NetworkManager!");
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 1809670267U;
			RpcParams rpcParams2;
			RpcParams rpcParams = rpcParams2;
			RpcAttribute.RpcAttributeParams attributeParams = new RpcAttribute.RpcAttributeParams
			{
				InvokePermission = RpcInvokePermission.Server,
				DeferLocal = true
			};
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.Everyone, RpcDelivery.Reliable);
			fastBufferWriter.WriteValueSafe<PlayerTeam>(byTeam, default(FastBufferWriter.ForEnums));
			fastBufferWriter.WriteValueSafe<NetworkObjectReference>(goalPlayerNetworkObjectReference, default(FastBufferWriter.ForNetworkSerializable));
			fastBufferWriter.WriteValueSafe<NetworkObjectReference>(assistPlayerNetworkObjectReference, default(FastBufferWriter.ForNetworkSerializable));
			fastBufferWriter.WriteValueSafe<NetworkObjectReference>(secondAssistPlayerNetworkObjectReference, default(FastBufferWriter.ForNetworkSerializable));
			fastBufferWriter.WriteValueSafe<NetworkObjectReference>(puckNetworkObjectReference, default(FastBufferWriter.ForNetworkSerializable));
			base.__endSendRpc(ref fastBufferWriter, 1809670267U, rpcParams2, attributeParams, SendTo.Everyone, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		Player playerFromNetworkObjectReference = NetworkingUtils.GetPlayerFromNetworkObjectReference(goalPlayerNetworkObjectReference);
		Player playerFromNetworkObjectReference2 = NetworkingUtils.GetPlayerFromNetworkObjectReference(assistPlayerNetworkObjectReference);
		Player playerFromNetworkObjectReference3 = NetworkingUtils.GetPlayerFromNetworkObjectReference(secondAssistPlayerNetworkObjectReference);
		Puck puckFromNetworkObjectReference = NetworkingUtils.GetPuckFromNetworkObjectReference(puckNetworkObjectReference);
		EventManager.TriggerEvent("Event_Everyone_OnGoalScored", new Dictionary<string, object>
		{
			{
				"byTeam",
				byTeam
			},
			{
				"goalPlayer",
				playerFromNetworkObjectReference
			},
			{
				"assistPlayer",
				playerFromNetworkObjectReference2
			},
			{
				"secondAssistPlayer",
				playerFromNetworkObjectReference3
			},
			{
				"puck",
				puckFromNetworkObjectReference
			}
		});
	}

	// Token: 0x06000538 RID: 1336 RVA: 0x0001CDE8 File Offset: 0x0001AFE8
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

	// Token: 0x06000539 RID: 1337 RVA: 0x0001CE4B File Offset: 0x0001B04B
	protected override void __initializeRpcs()
	{
		base.__registerRpc(1809670267U, new NetworkBehaviour.RpcReceiveHandler(GameManager.__rpc_handler_1809670267), "Server_NotifyGoalScoredRpc", RpcInvokePermission.Server);
		base.__initializeRpcs();
	}

	// Token: 0x0600053A RID: 1338 RVA: 0x0001CE78 File Offset: 0x0001B078
	private static void __rpc_handler_1809670267(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		PlayerTeam byTeam;
		reader.ReadValueSafe<PlayerTeam>(out byTeam, default(FastBufferWriter.ForEnums));
		NetworkObjectReference goalPlayerNetworkObjectReference;
		reader.ReadValueSafe<NetworkObjectReference>(out goalPlayerNetworkObjectReference, default(FastBufferWriter.ForNetworkSerializable));
		NetworkObjectReference assistPlayerNetworkObjectReference;
		reader.ReadValueSafe<NetworkObjectReference>(out assistPlayerNetworkObjectReference, default(FastBufferWriter.ForNetworkSerializable));
		NetworkObjectReference secondAssistPlayerNetworkObjectReference;
		reader.ReadValueSafe<NetworkObjectReference>(out secondAssistPlayerNetworkObjectReference, default(FastBufferWriter.ForNetworkSerializable));
		NetworkObjectReference puckNetworkObjectReference;
		reader.ReadValueSafe<NetworkObjectReference>(out puckNetworkObjectReference, default(FastBufferWriter.ForNetworkSerializable));
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((GameManager)target).Server_NotifyGoalScoredRpc(byTeam, goalPlayerNetworkObjectReference, assistPlayerNetworkObjectReference, secondAssistPlayerNetworkObjectReference, puckNetworkObjectReference);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x0600053B RID: 1339 RVA: 0x0001CF64 File Offset: 0x0001B164
	protected internal override string __getTypeName()
	{
		return "GameManager";
	}

	// Token: 0x04000333 RID: 819
	[HideInInspector]
	public NetworkVariable<GameState> GameState;

	// Token: 0x04000334 RID: 820
	private Tween tickTween;
}
