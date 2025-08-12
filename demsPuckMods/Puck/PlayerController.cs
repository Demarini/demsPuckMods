using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

// Token: 0x020000F1 RID: 241
public class PlayerController : NetworkBehaviour
{
	// Token: 0x06000821 RID: 2081 RVA: 0x0000BF79 File Offset: 0x0000A179
	private void Awake()
	{
		this.player = base.GetComponent<Player>();
	}

	// Token: 0x06000822 RID: 2082 RVA: 0x000334F0 File Offset: 0x000316F0
	public override void OnNetworkSpawn()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerTeamChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerTeamChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerPositionClaimedByChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerPositionClaimedByChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerPositionChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerPositionChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnGoalScored", new Action<Dictionary<string, object>>(this.Event_OnGoalScored));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnGamePhaseChanged", new Action<Dictionary<string, object>>(this.Event_OnGamePhaseChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Server_OnChatCommand", new Action<Dictionary<string, object>>(this.Event_Server_OnChatCommand));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Server_OnPlayerSubscription", new Action<Dictionary<string, object>>(this.Event_Server_OnPlayerSubscription));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnPlayerSelectTeam", new Action<Dictionary<string, object>>(this.Event_Client_OnPlayerSelectTeam));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnHandednessChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnHandednessChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnPauseMenuClickSwitchTeam", new Action<Dictionary<string, object>>(this.Event_Client_OnPauseMenuClickSwitchTeam));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnPlayerRequestPositionSelect", new Action<Dictionary<string, object>>(this.Event_Client_OnPlayerRequestPositionSelect));
		if (NetworkManager.Singleton.IsServer)
		{
			this.pingIntervalCoroutine = this.IPingInterval();
			base.StartCoroutine(this.pingIntervalCoroutine);
		}
		base.OnNetworkSpawn();
	}

	// Token: 0x06000823 RID: 2083 RVA: 0x00033654 File Offset: 0x00031854
	public override void OnNetworkDespawn()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerTeamChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerTeamChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerPositionClaimedByChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerPositionClaimedByChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerPositionChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerPositionChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnGoalScored", new Action<Dictionary<string, object>>(this.Event_OnGoalScored));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnGamePhaseChanged", new Action<Dictionary<string, object>>(this.Event_OnGamePhaseChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Server_OnChatCommand", new Action<Dictionary<string, object>>(this.Event_Server_OnChatCommand));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Server_OnPlayerSubscription", new Action<Dictionary<string, object>>(this.Event_Server_OnPlayerSubscription));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnPlayerSelectTeam", new Action<Dictionary<string, object>>(this.Event_Client_OnPlayerSelectTeam));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnHandednessChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnHandednessChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnPauseMenuClickSwitchTeam", new Action<Dictionary<string, object>>(this.Event_Client_OnPauseMenuClickSwitchTeam));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnPlayerRequestPositionSelect", new Action<Dictionary<string, object>>(this.Event_Client_OnPlayerRequestPositionSelect));
		if (NetworkManager.Singleton.IsServer && this.pingIntervalCoroutine != null)
		{
			base.StopCoroutine(this.pingIntervalCoroutine);
		}
		base.OnNetworkDespawn();
	}

	// Token: 0x06000824 RID: 2084 RVA: 0x0000BF87 File Offset: 0x0000A187
	private IEnumerator IPingInterval()
	{
		yield return new WaitForSeconds(10f);
		this.player.Server_UpdatePing();
		this.pingIntervalCoroutine = this.IPingInterval();
		base.StartCoroutine(this.pingIntervalCoroutine);
		yield break;
	}

	// Token: 0x06000825 RID: 2085 RVA: 0x000337B0 File Offset: 0x000319B0
	private void Event_OnPlayerTeamChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (base.OwnerClientId != player.OwnerClientId)
		{
			return;
		}
		Debug.Log(string.Format("[PlayerController] Player {0} ({1}) changed team to {2}", player.Username.Value, player.OwnerClientId, player.Team.Value));
		switch (player.Team.Value)
		{
		case PlayerTeam.Spectator:
			player.Client_SetPlayerStateRpc(PlayerState.Spectate, 0f);
			return;
		case PlayerTeam.Blue:
			player.Client_SetPlayerStateRpc(PlayerState.PositionSelectBlue, 0f);
			return;
		case PlayerTeam.Red:
			player.Client_SetPlayerStateRpc(PlayerState.PositionSelectRed, 0f);
			return;
		default:
			return;
		}
	}

	// Token: 0x06000826 RID: 2086 RVA: 0x0003386C File Offset: 0x00031A6C
	private void Event_OnPlayerPositionClaimedByChanged(Dictionary<string, object> message)
	{
		PlayerPosition playerPosition = (PlayerPosition)message["playerPosition"];
		Player player = (Player)message["oldClaimedBy"];
		Player player2 = (Player)message["newClaimedBy"];
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (player && player.OwnerClientId == base.OwnerClientId)
		{
			Debug.Log(string.Format("[PlayerController] Player {0} ({1}) was unassigned position {2}", this.player.Username.Value, this.player.OwnerClientId, playerPosition.Name));
			Debug.Log(default(NetworkObjectReference));
			this.player.PlayerPositionReference.Value = default(NetworkObjectReference);
		}
		if (player2 && player2.OwnerClientId == base.OwnerClientId)
		{
			Debug.Log(string.Format("[PlayerController] Player {0} ({1}) was assigned {2}", this.player.Username.Value, this.player.OwnerClientId, playerPosition.Name));
			this.player.PlayerPositionReference.Value = new NetworkObjectReference(playerPosition.NetworkObject);
		}
	}

	// Token: 0x06000827 RID: 2087 RVA: 0x00033990 File Offset: 0x00031B90
	private void Event_OnPlayerPositionChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (base.OwnerClientId != player.OwnerClientId)
		{
			return;
		}
		if (!player.PlayerPosition)
		{
			player.Role.Value = PlayerRole.None;
			return;
		}
		player.Role.Value = player.PlayerPosition.Role;
		GamePhase phase = NetworkBehaviourSingleton<GameManager>.Instance.GameState.Value.Phase;
		if (phase - GamePhase.Warmup <= 1)
		{
			player.Client_SetPlayerStateRpc(PlayerState.Play, 0f);
			return;
		}
		if (phase != GamePhase.Playing)
		{
			return;
		}
		player.Client_SetPlayerStateRpc(PlayerState.Play, NetworkBehaviourSingleton<ServerManager>.Instance.ServerConfigurationManager.ServerConfiguration.joinMidMatchDelay);
	}

	// Token: 0x06000828 RID: 2088 RVA: 0x00033A44 File Offset: 0x00031C44
	public void Event_OnGoalScored(Dictionary<string, object> message)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		bool flag = (bool)message["hasGoalPlayer"];
		ulong num = (ulong)message["goalPlayerClientId"];
		bool flag2 = (bool)message["hasAssistPlayer"];
		ulong num2 = (ulong)message["assistPlayerClientId"];
		bool flag3 = (bool)message["hasSecondAssistPlayer"];
		ulong num3 = (ulong)message["secondAssistPlayerClientId"];
		if (flag && num == base.OwnerClientId)
		{
			NetworkVariable<int> goals = this.player.Goals;
			int value = goals.Value;
			goals.Value = value + 1;
		}
		if (flag2 && num2 == base.OwnerClientId)
		{
			NetworkVariable<int> assists = this.player.Assists;
			int value = assists.Value;
			assists.Value = value + 1;
		}
		if (flag3 && num3 == base.OwnerClientId)
		{
			NetworkVariable<int> assists2 = this.player.Assists;
			int value = assists2.Value;
			assists2.Value = value + 1;
		}
	}

	// Token: 0x06000829 RID: 2089 RVA: 0x00033B3C File Offset: 0x00031D3C
	private void Event_OnGamePhaseChanged(Dictionary<string, object> message)
	{
		bool flag = (bool)message["isFirstFaceOff"];
		GamePhase gamePhase = (GamePhase)message["newGamePhase"];
		GamePhase gamePhase2 = (GamePhase)message["oldGamePhase"];
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (flag)
		{
			this.player.Server_ResetPoints();
		}
		if (this.player.Team.Value == PlayerTeam.None || this.player.Team.Value == PlayerTeam.Spectator)
		{
			return;
		}
		if (!this.player.PlayerPosition)
		{
			return;
		}
		if (gamePhase <= GamePhase.FaceOff)
		{
			if (gamePhase != GamePhase.Warmup)
			{
				if (gamePhase != GamePhase.FaceOff)
				{
					return;
				}
			}
			else
			{
				if (gamePhase2 == GamePhase.GameOver)
				{
					this.player.Client_SetPlayerStateRpc((this.player.Team.Value == PlayerTeam.Blue) ? PlayerState.PositionSelectBlue : PlayerState.PositionSelectRed, 0f);
					return;
				}
				this.player.Client_SetPlayerStateRpc(PlayerState.Play, 0f);
				return;
			}
		}
		else
		{
			if (gamePhase == GamePhase.Replay)
			{
				this.player.Client_SetPlayerStateRpc(PlayerState.Replay, 0f);
				return;
			}
			if (gamePhase != GamePhase.GameOver)
			{
				return;
			}
		}
		this.player.Client_SetPlayerStateRpc(PlayerState.Play, 0f);
	}

	// Token: 0x0600082A RID: 2090 RVA: 0x00033C48 File Offset: 0x00031E48
	private void Event_Server_OnChatCommand(Dictionary<string, object> message)
	{
		ulong clientId = (ulong)message["clientId"];
		string a = (string)message["command"];
		string[] array = (string[])message["args"];
		Player playerByClientId = NetworkBehaviourSingleton<PlayerManager>.Instance.GetPlayerByClientId(clientId);
		if (!playerByClientId)
		{
			return;
		}
		bool flag = NetworkBehaviourSingleton<ServerManager>.Instance.AdminSteamIds.Contains(playerByClientId.SteamId.Value.ToString());
		if (!(a == "/username"))
		{
			if (!(a == "/number"))
			{
				return;
			}
			if (flag)
			{
				if (array.Length < 1)
				{
					return;
				}
				int number;
				if (!int.TryParse(array[0], out number))
				{
					return;
				}
				playerByClientId.Client_SetPlayerNumberRpc(number);
			}
		}
		else if (flag)
		{
			if (array.Length < 1)
			{
				return;
			}
			playerByClientId.Client_SetPlayerUsernameRpc(array[0]);
			return;
		}
	}

	// Token: 0x0600082B RID: 2091 RVA: 0x00033D20 File Offset: 0x00031F20
	private void Event_Server_OnPlayerSubscription(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		if (base.OwnerClientId != player.OwnerClientId)
		{
			return;
		}
		player.Client_SetPlayerStateRpc(PlayerState.TeamSelect, 0f);
	}

	// Token: 0x0600082C RID: 2092 RVA: 0x00033D5C File Offset: 0x00031F5C
	private void Event_Client_OnPlayerSelectTeam(Dictionary<string, object> message)
	{
		if (!base.IsOwner)
		{
			return;
		}
		PlayerTeam team = (PlayerTeam)message["team"];
		this.player.Client_SetPlayerTeamRpc(team);
	}

	// Token: 0x0600082D RID: 2093 RVA: 0x00033D90 File Offset: 0x00031F90
	private void Event_Client_OnHandednessChanged(Dictionary<string, object> message)
	{
		if (!base.IsOwner)
		{
			return;
		}
		string a = (string)message["value"];
		this.player.Client_SetPlayerHandednessRpc((a == "LEFT") ? PlayerHandedness.Left : PlayerHandedness.Right);
	}

	// Token: 0x0600082E RID: 2094 RVA: 0x0000BF96 File Offset: 0x0000A196
	private void Event_Client_OnPauseMenuClickSwitchTeam(Dictionary<string, object> message)
	{
		if (!base.IsOwner)
		{
			return;
		}
		this.player.Client_SetPlayerStateRpc(PlayerState.TeamSelect, 0f);
	}

	// Token: 0x0600082F RID: 2095 RVA: 0x0000BFB2 File Offset: 0x0000A1B2
	private void Event_Client_OnPlayerRequestPositionSelect(Dictionary<string, object> message)
	{
		if (!base.IsOwner)
		{
			return;
		}
		this.player.Client_SetPlayerStateRpc((this.player.Team.Value == PlayerTeam.Blue) ? PlayerState.PositionSelectBlue : PlayerState.PositionSelectRed, 0f);
	}

	// Token: 0x06000831 RID: 2097 RVA: 0x0000F5B8 File Offset: 0x0000D7B8
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x06000832 RID: 2098 RVA: 0x00006C33 File Offset: 0x00004E33
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x06000833 RID: 2099 RVA: 0x0000BFE4 File Offset: 0x0000A1E4
	protected internal override string __getTypeName()
	{
		return "PlayerController";
	}

	// Token: 0x040004C7 RID: 1223
	private Player player;

	// Token: 0x040004C8 RID: 1224
	private IEnumerator pingIntervalCoroutine;
}
