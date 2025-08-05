using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Token: 0x02000049 RID: 73
public class LevelManagerController : MonoBehaviour
{
	// Token: 0x06000211 RID: 529 RVA: 0x000083F3 File Offset: 0x000065F3
	private void Awake()
	{
		this.levelManager = base.GetComponent<LevelManager>();
	}

	// Token: 0x06000212 RID: 530 RVA: 0x00015CDC File Offset: 0x00013EDC
	private void Start()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnGamePhaseChanged", new Action<Dictionary<string, object>>(this.Event_OnGamePhaseChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerStateChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerStateChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnClientConnected", new Action<Dictionary<string, object>>(this.Event_OnClientConnected));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Server_OnChatCommand", new Action<Dictionary<string, object>>(this.Event_Server_OnChatCommand));
		this.levelManager.Client_EnableObserverCamera();
		this.levelManager.Client_DeactivateGoalLights();
	}

	// Token: 0x06000213 RID: 531 RVA: 0x00015D6C File Offset: 0x00013F6C
	private void OnDestroy()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnGamePhaseChanged", new Action<Dictionary<string, object>>(this.Event_OnGamePhaseChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerStateChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerStateChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnClientConnected", new Action<Dictionary<string, object>>(this.Event_OnClientConnected));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Server_OnChatCommand", new Action<Dictionary<string, object>>(this.Event_Server_OnChatCommand));
	}

	// Token: 0x06000214 RID: 532 RVA: 0x00015DE8 File Offset: 0x00013FE8
	private void FixedUpdate()
	{
		if (!this.isBlackHoleActive)
		{
			return;
		}
		foreach (Puck puck in NetworkBehaviourSingleton<PuckManager>.Instance.GetPucks(false))
		{
			if (!(puck == null))
			{
				Rigidbody component = puck.GetComponent<Rigidbody>();
				if (!(component == null))
				{
					Vector3 position = puck.transform.position;
					Vector3 normalized = (this.blackHolePosition - position).normalized;
					float num = Vector3.Distance(this.blackHolePosition, position);
					num = Mathf.Max(num, this.blackHoleMinDistance);
					float d = this.blackHoleBasePullStrength / num;
					Vector3 force = normalized * d;
					component.AddForce(force, ForceMode.Force);
				}
			}
		}
	}

	// Token: 0x06000215 RID: 533 RVA: 0x00015EBC File Offset: 0x000140BC
	private void Event_OnPlayerStateChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		if (!player.IsLocalPlayer)
		{
			return;
		}
		switch (player.State.Value)
		{
		case PlayerState.TeamSelect:
			this.levelManager.Client_EnableObserverCamera();
			return;
		case PlayerState.PositionSelectBlue:
			this.levelManager.Client_EnableBluePositionSelectionCamera();
			return;
		case PlayerState.PositionSelectRed:
			this.levelManager.Client_EnableRedPositionSelectionCamera();
			return;
		case PlayerState.Play:
			break;
		case PlayerState.Replay:
			this.levelManager.Client_EnableReplayCamera();
			break;
		default:
			return;
		}
	}

	// Token: 0x06000216 RID: 534 RVA: 0x00015F3C File Offset: 0x0001413C
	private void Event_OnGamePhaseChanged(Dictionary<string, object> message)
	{
		GamePhase gamePhase = (GamePhase)message["newGamePhase"];
		if (NetworkManager.Singleton.IsServer && gamePhase != GamePhase.Warmup)
		{
			this.levelManager.PuckShooter.Server_StopShootingCoroutine();
			this.isBlackHoleActive = false;
		}
		switch (gamePhase)
		{
		case GamePhase.BlueScore:
			this.levelManager.Client_ActivateRedGoalLight();
			this.levelManager.Server_PlayTeamRedGoalSound();
			this.levelManager.Server_PlayCheerSound();
			return;
		case GamePhase.RedScore:
			this.levelManager.Client_ActivateBlueGoalLight();
			this.levelManager.Server_PlayTeamBlueGoalSound();
			this.levelManager.Server_PlayCheerSound();
			return;
		case GamePhase.PeriodOver:
			this.levelManager.Server_PlayPeriodHornSound();
			return;
		case GamePhase.GameOver:
			this.levelManager.Server_PlayPeriodHornSound();
			this.levelManager.Server_PlayCheerSound();
			return;
		}
		this.levelManager.Client_DeactivateGoalLights();
	}

	// Token: 0x06000217 RID: 535 RVA: 0x00016014 File Offset: 0x00014214
	private void Event_OnClientConnected(Dictionary<string, object> message)
	{
		ulong num = (ulong)message["clientId"];
		if (NetworkManager.Singleton.LocalClientId != num)
		{
			return;
		}
		this.levelManager.Client_EnableObserverCamera();
		this.levelManager.Client_DeactivateGoalLights();
	}

	// Token: 0x06000218 RID: 536 RVA: 0x00016058 File Offset: 0x00014258
	private void Event_Server_OnChatCommand(Dictionary<string, object> message)
	{
		ulong clientId = (ulong)message["clientId"];
		string a = (string)message["command"];
		string[] array = (string[])message["args"];
		Player playerByClientId = NetworkBehaviourSingleton<PlayerManager>.Instance.GetPlayerByClientId(clientId);
		GamePhase phase = NetworkBehaviourSingleton<GameManager>.Instance.GameState.Value.Phase;
		if (!(a == "/puckshooter"))
		{
			if (!(a == "/magnet"))
			{
				if (!(a == "/pucks"))
				{
					return;
				}
				if (!Application.isEditor && phase != GamePhase.Warmup)
				{
					return;
				}
				if (!playerByClientId)
				{
					return;
				}
				if (playerByClientId.AdminLevel.Value < 2)
				{
					return;
				}
				if (!playerByClientId.IsCharacterFullySpawned)
				{
					return;
				}
				for (int i = 0; i < 10; i++)
				{
					NetworkBehaviourSingleton<PuckManager>.Instance.Server_SpawnPuck(playerByClientId.Stick.BladeHandlePosition, playerByClientId.Stick.transform.rotation, Vector3.zero, false);
				}
			}
			else
			{
				if (!Application.isEditor && phase != GamePhase.Warmup)
				{
					return;
				}
				if (!playerByClientId)
				{
					return;
				}
				if (playerByClientId.AdminLevel.Value < 2)
				{
					return;
				}
				if (!playerByClientId.IsCharacterFullySpawned)
				{
					return;
				}
				this.isBlackHoleActive = !this.isBlackHoleActive;
				if (this.isBlackHoleActive)
				{
					this.blackHolePosition = playerByClientId.Stick.BladeHandlePosition;
					return;
				}
			}
		}
		else
		{
			if (array.Length < 1)
			{
				this.levelManager.PuckShooter.Server_StartShootingCoroutine();
				return;
			}
			if (!Application.isEditor && phase != GamePhase.Warmup)
			{
				return;
			}
			if (!playerByClientId)
			{
				return;
			}
			if (!playerByClientId.IsCharacterFullySpawned)
			{
				return;
			}
			if (array[0] == "on")
			{
				this.levelManager.PuckShooter.transform.position = playerByClientId.PlayerBody.transform.position;
				this.levelManager.PuckShooter.transform.rotation = playerByClientId.PlayerBody.transform.rotation;
				this.levelManager.PuckShooter.Server_StartShootingCoroutine();
				return;
			}
			if (array[0] == "off")
			{
				this.levelManager.PuckShooter.Server_StopShootingCoroutine();
				return;
			}
		}
	}

	// Token: 0x04000141 RID: 321
	private LevelManager levelManager;

	// Token: 0x04000142 RID: 322
	private bool isBlackHoleActive;

	// Token: 0x04000143 RID: 323
	private Vector3 blackHolePosition;

	// Token: 0x04000144 RID: 324
	private float blackHoleBasePullStrength = 10f;

	// Token: 0x04000145 RID: 325
	private float blackHoleMinDistance = 0.5f;
}
