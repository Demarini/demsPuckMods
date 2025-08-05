using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

// Token: 0x0200003E RID: 62
public class GameManagerController : NetworkBehaviour
{
	// Token: 0x060001BD RID: 445 RVA: 0x000080A2 File Offset: 0x000062A2
	private void Awake()
	{
		this.gameManager = base.GetComponent<GameManager>();
	}

	// Token: 0x060001BE RID: 446 RVA: 0x00014168 File Offset: 0x00012368
	private void Start()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Server_OnServerReady", new Action<Dictionary<string, object>>(this.Event_Server_OnServerReady));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Server_OnPuckEnterTeamGoal", new Action<Dictionary<string, object>>(this.Event_Server_OnPuckEnterTeamGoal));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Server_OnVoteSuccess", new Action<Dictionary<string, object>>(this.Event_Server_OnVoteSuccess));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Server_OnChatCommand", new Action<Dictionary<string, object>>(this.Event_Server_OnChatCommand));
	}

	// Token: 0x060001BF RID: 447 RVA: 0x000080B0 File Offset: 0x000062B0
	public override void OnNetworkDespawn()
	{
		base.OnNetworkDespawn();
	}

	// Token: 0x060001C0 RID: 448 RVA: 0x000141E4 File Offset: 0x000123E4
	public override void OnDestroy()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Server_OnServerReady", new Action<Dictionary<string, object>>(this.Event_Server_OnServerReady));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Server_OnPuckEnterTeamGoal", new Action<Dictionary<string, object>>(this.Event_Server_OnPuckEnterTeamGoal));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Server_OnVoteSuccess", new Action<Dictionary<string, object>>(this.Event_Server_OnVoteSuccess));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Server_OnChatCommand", new Action<Dictionary<string, object>>(this.Event_Server_OnChatCommand));
		base.OnDestroy();
	}

	// Token: 0x060001C1 RID: 449 RVA: 0x000080B8 File Offset: 0x000062B8
	private void Event_Server_OnServerReady(Dictionary<string, object> message)
	{
		this.gameManager.Server_StartGame(true, -1);
		if (NetworkBehaviourSingleton<ServerManager>.Instance.ServerConfigurationManager.ServerConfiguration.startPaused)
		{
			this.gameManager.Server_StopGameStateTickCoroutine();
			return;
		}
		this.gameManager.Server_StartGameStateTickCoroutine();
	}

	// Token: 0x060001C2 RID: 450 RVA: 0x00014264 File Offset: 0x00012464
	private void Event_Server_OnPuckEnterTeamGoal(Dictionary<string, object> message)
	{
		PlayerTeam playerTeam = (PlayerTeam)message["team"];
		Puck puck = (Puck)message["puck"];
		if (this.gameManager.Phase != GamePhase.Playing)
		{
			return;
		}
		PlayerTeam team = (playerTeam == PlayerTeam.Blue) ? PlayerTeam.Red : PlayerTeam.Blue;
		List<KeyValuePair<Player, float>> playerCollisions = puck.GetPlayerCollisions();
		List<KeyValuePair<Player, float>> playerCollisionsByTeam = puck.GetPlayerCollisionsByTeam(team);
		Player lastPlayer = null;
		Player goalPlayer = null;
		Player assistPlayer = null;
		Player secondAssistPlayer = null;
		if (playerCollisionsByTeam.Count >= 1)
		{
			List<KeyValuePair<Player, float>> list = playerCollisionsByTeam;
			goalPlayer = list[list.Count - 1].Key;
			if (playerCollisionsByTeam.Count > 1)
			{
				List<KeyValuePair<Player, float>> list2 = playerCollisionsByTeam;
				assistPlayer = list2[list2.Count - 2].Key;
			}
			if (playerCollisionsByTeam.Count > 2)
			{
				List<KeyValuePair<Player, float>> list3 = playerCollisionsByTeam;
				secondAssistPlayer = list3[list3.Count - 3].Key;
			}
		}
		if (playerCollisions.Count >= 1)
		{
			List<KeyValuePair<Player, float>> list4 = playerCollisions;
			lastPlayer = list4[list4.Count - 1].Key;
		}
		this.gameManager.Server_GoalScored(team, lastPlayer, goalPlayer, assistPlayer, secondAssistPlayer, puck);
	}

	// Token: 0x060001C3 RID: 451 RVA: 0x0001436C File Offset: 0x0001256C
	private void Event_Server_OnVoteSuccess(Dictionary<string, object> message)
	{
		Vote vote = (Vote)message["vote"];
		VoteType type = vote.Type;
		if (type == VoteType.Start)
		{
			Debug.Log(string.Format("[GameManagerController] Vote succeeded to start game ({0}/{1})", vote.Votes, vote.VotesNeeded));
			this.gameManager.Server_StartGame(false, 10);
			return;
		}
		if (type != VoteType.Warmup)
		{
			return;
		}
		Debug.Log(string.Format("[GameManagerController] Vote succeeded to start warmup ({0}/{1})", vote.Votes, vote.VotesNeeded));
		this.gameManager.Server_StartGame(true, -1);
	}

	// Token: 0x060001C4 RID: 452 RVA: 0x00014400 File Offset: 0x00012600
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
		if (!(a == "/start"))
		{
			if (!(a == "/warmup"))
			{
				if (!(a == "/pause"))
				{
					if (!(a == "/resume"))
					{
						if (!(a == "/debug"))
						{
							return;
						}
						if (flag)
						{
							if (this.gameManager.IsDebugGameStateCoroutineRunning)
							{
								this.gameManager.Server_StopDebugGameStateCoroutine();
								return;
							}
							this.gameManager.Server_StartDebugGameStateCoroutine(0.1f);
						}
					}
					else if (flag)
					{
						NetworkBehaviourSingleton<UIChat>.Instance.Server_SendSystemChatMessage("<b><color=orange>ADMIN</color></b> resumed the game.");
						this.gameManager.Server_StartGameStateTickCoroutine();
						return;
					}
				}
				else if (flag)
				{
					NetworkBehaviourSingleton<UIChat>.Instance.Server_SendSystemChatMessage("<b><color=orange>ADMIN</color></b> paused the game.");
					this.gameManager.Server_StopGameStateTickCoroutine();
					return;
				}
			}
			else if (flag)
			{
				NetworkBehaviourSingleton<UIChat>.Instance.Server_SendSystemChatMessage("<b><color=orange>ADMIN</color></b> started warmup.");
				this.gameManager.Server_StartGame(true, -1);
				return;
			}
		}
		else if (flag)
		{
			NetworkBehaviourSingleton<UIChat>.Instance.Server_SendSystemChatMessage("<b><color=orange>ADMIN</color></b> started the game.");
			this.gameManager.Server_StartGame(false, 10);
			return;
		}
	}

	// Token: 0x060001C6 RID: 454 RVA: 0x0000F5B8 File Offset: 0x0000D7B8
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x060001C7 RID: 455 RVA: 0x00006C33 File Offset: 0x00004E33
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x060001C8 RID: 456 RVA: 0x000080F4 File Offset: 0x000062F4
	protected internal override string __getTypeName()
	{
		return "GameManagerController";
	}

	// Token: 0x040000ED RID: 237
	private GameManager gameManager;
}
