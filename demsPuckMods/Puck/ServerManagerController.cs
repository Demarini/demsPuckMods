using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using SocketIOClient;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

// Token: 0x020000A3 RID: 163
public class ServerManagerController : NetworkBehaviour
{
	// Token: 0x06000447 RID: 1095 RVA: 0x00009ACB File Offset: 0x00007CCB
	private void Awake()
	{
		this.serverManager = base.GetComponent<ServerManager>();
	}

	// Token: 0x06000448 RID: 1096 RVA: 0x0001CA3C File Offset: 0x0001AC3C
	private void Start()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnClientConnected", new Action<Dictionary<string, object>>(this.Event_OnClientConnected));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnClientDisconnected", new Action<Dictionary<string, object>>(this.Event_OnClientDisconnected));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerRemoved", new Action<Dictionary<string, object>>(this.Event_OnPlayerRemoved));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Server_OnServerStarted", new Action<Dictionary<string, object>>(this.Event_Server_OnServerStarted));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Server_OnServerReady", new Action<Dictionary<string, object>>(this.Event_Server_OnServerReady));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Server_OnServerStopped", new Action<Dictionary<string, object>>(this.Event_Server_OnServerStopped));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Server_ConnectionApproval", new Action<Dictionary<string, object>>(this.Event_Server_ConnectionApproval));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Server_OnVoteSuccess", new Action<Dictionary<string, object>>(this.Event_Server_OnVoteSuccess));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Server_OnPlayerSubscription", new Action<Dictionary<string, object>>(this.Event_Server_OnPlayerSubscription));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Server_OnChatCommand", new Action<Dictionary<string, object>>(this.Event_Server_OnChatCommand));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Server_OnPlayerSleepInput", new Action<Dictionary<string, object>>(this.Event_Server_OnPlayerSleepInput));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnTransportFailure", new Action<Dictionary<string, object>>(this.Event_Client_OnTransportFailure));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnLevelReady", new Action<Dictionary<string, object>>(this.Event_Client_OnLevelReady));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnMainMenuClickHostServer", new Action<Dictionary<string, object>>(this.Event_Client_OnMainMenuClickHostServer));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnMainMenuClickPractice", new Action<Dictionary<string, object>>(this.Event_Client_OnMainMenuClickPractice));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnServerLauncherClickStartSelfHostedServer", new Action<Dictionary<string, object>>(this.Event_Client_OnServerLauncherClickStartSelfHostedServer));
		MonoBehaviourSingleton<WebSocketManager>.Instance.AddMessageListener("connect", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnConnect));
		MonoBehaviourSingleton<WebSocketManager>.Instance.AddMessageListener("serverAuthenticateResponse", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnServerAuthenticateResponse));
		MonoBehaviourSingleton<WebSocketManager>.Instance.AddMessageListener("serverConnectionApprovalResponse", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnServerConnectionApprovalResponse));
		MonoBehaviourSingleton<WebSocketManager>.Instance.AddMessageListener("serverKickPlayer", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnServerKickPlayer));
	}

	// Token: 0x06000449 RID: 1097 RVA: 0x00009AD9 File Offset: 0x00007CD9
	public void HelloWorld()
	{
		Debug.Log("[ServerManagerController] Hello World");
	}

	// Token: 0x0600044A RID: 1098 RVA: 0x0001CC68 File Offset: 0x0001AE68
	public override void OnDestroy()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnClientConnected", new Action<Dictionary<string, object>>(this.Event_OnClientConnected));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnClientDisconnected", new Action<Dictionary<string, object>>(this.Event_OnClientDisconnected));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerRemoved", new Action<Dictionary<string, object>>(this.Event_OnPlayerRemoved));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Server_OnServerStarted", new Action<Dictionary<string, object>>(this.Event_Server_OnServerStarted));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Server_OnServerReady", new Action<Dictionary<string, object>>(this.Event_Server_OnServerReady));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Server_OnServerStopped", new Action<Dictionary<string, object>>(this.Event_Server_OnServerStopped));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Server_ConnectionApproval", new Action<Dictionary<string, object>>(this.Event_Server_ConnectionApproval));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Server_OnVoteSuccess", new Action<Dictionary<string, object>>(this.Event_Server_OnVoteSuccess));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Server_OnPlayerSubscription", new Action<Dictionary<string, object>>(this.Event_Server_OnPlayerSubscription));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Server_OnChatCommand", new Action<Dictionary<string, object>>(this.Event_Server_OnChatCommand));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Server_OnPlayerSleepInput", new Action<Dictionary<string, object>>(this.Event_Server_OnPlayerSleepInput));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnTransportFailure", new Action<Dictionary<string, object>>(this.Event_Client_OnTransportFailure));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnLevelReady", new Action<Dictionary<string, object>>(this.Event_Client_OnLevelReady));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnMainMenuClickHostServer", new Action<Dictionary<string, object>>(this.Event_Client_OnMainMenuClickHostServer));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnMainMenuClickPractice", new Action<Dictionary<string, object>>(this.Event_Client_OnMainMenuClickPractice));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnServerLauncherClickStartSelfHostedServer", new Action<Dictionary<string, object>>(this.Event_Client_OnServerLauncherClickStartSelfHostedServer));
		MonoBehaviourSingleton<WebSocketManager>.Instance.RemoveMessageListener("connect", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnConnect));
		MonoBehaviourSingleton<WebSocketManager>.Instance.RemoveMessageListener("serverAuthenticateResponse", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnServerAuthenticateResponse));
		MonoBehaviourSingleton<WebSocketManager>.Instance.RemoveMessageListener("serverConnectionApprovalResponse", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnServerConnectionApprovalResponse));
		MonoBehaviourSingleton<WebSocketManager>.Instance.RemoveMessageListener("serverKickPlayer", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnServerKickPlayer));
		base.OnDestroy();
	}

	// Token: 0x0600044B RID: 1099 RVA: 0x0001CE98 File Offset: 0x0001B098
	private void Event_OnClientConnected(Dictionary<string, object> message)
	{
		ulong num = (ulong)message["clientId"];
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		Debug.Log(string.Format("[ServerManagerController] Client connected ({0}) {1}/{2}", num, NetworkManager.Singleton.ConnectedClientsList.Count, this.serverManager.Server.MaxPlayers));
		if (this.serverManager.EdgegapManager.IsEdgegap)
		{
			this.serverManager.EdgegapManager.StopDeleteDeploymentCoroutine();
		}
	}

	// Token: 0x0600044C RID: 1100 RVA: 0x0001CF24 File Offset: 0x0001B124
	private void Event_OnClientDisconnected(Dictionary<string, object> message)
	{
		ulong num = (ulong)message["clientId"];
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		Debug.Log(string.Format("[ServerManagerController] Client disconnected ({0}) {1}/{2}", num, NetworkManager.Singleton.ConnectedClientsList.Count, this.serverManager.Server.MaxPlayers));
		if (this.approvedClients.Contains(num))
		{
			this.approvedClients.Remove(num);
			if (this.serverManager.EdgegapManager.IsEdgegap && this.approvedClients.Count == 0)
			{
				this.serverManager.EdgegapManager.DeleteDeployment();
			}
		}
	}

	// Token: 0x0600044D RID: 1101 RVA: 0x0001CFD8 File Offset: 0x0001B1D8
	private void Event_Client_OnTransportFailure(Dictionary<string, object> message)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnServerStartFailed", new Dictionary<string, object>
		{
			{
				"isHost",
				this.serverManager.IsHostStartInProgress
			},
			{
				"isServer",
				this.serverManager.IsServerStartInProgress
			}
		});
	}

	// Token: 0x0600044E RID: 1102 RVA: 0x0001D030 File Offset: 0x0001B230
	private void Event_Client_OnLevelReady(Dictionary<string, object> message)
	{
		if (this.serverManager.IsServerStartInProgress)
		{
			NetworkManager.Singleton.StartServer();
			this.serverManager.IsServerStartInProgress = false;
		}
		if (this.serverManager.IsHostStartInProgress)
		{
			NetworkManager.Singleton.StartHost();
			this.serverManager.IsHostStartInProgress = false;
		}
	}

	// Token: 0x0600044F RID: 1103 RVA: 0x0001D088 File Offset: 0x0001B288
	private void Event_OnPlayerRemoved(Dictionary<string, object> message)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		string[] playerSteamIds = NetworkBehaviourSingleton<PlayerManager>.Instance.GetPlayerSteamIds();
		this.serverManager.Server_UpdateConnectedSteamIds(playerSteamIds);
	}

	// Token: 0x06000450 RID: 1104 RVA: 0x0001D0BC File Offset: 0x0001B2BC
	private void Event_Server_OnServerStarted(Dictionary<string, object> message)
	{
		this.serverManager.UdpSocket.StartSocket(this.serverManager.ServerConfigurationManager.ServerConfiguration.pingPort);
		NetworkBehaviourSingleton<SynchronizedObjectManager>.Instance.TickRate = this.serverManager.ServerConfigurationManager.ServerConfiguration.serverTickRate;
		NetworkBehaviourSingleton<GameManager>.Instance.PhaseDurationMap = this.serverManager.ServerConfigurationManager.ServerConfiguration.phaseDurationMap;
		this.serverManager.LoadAdminSteamIds();
		this.serverManager.LoadBannedSteamIds();
		if (this.serverManager.ServerConfigurationManager.ServerConfiguration.printMetrics)
		{
			this.serverManager.Server_StartMetricsCoroutine();
		}
		if (this.serverManager.ServerConfigurationManager.ServerConfiguration.reloadBannedSteamIds)
		{
			this.serverManager.Server_StartBannedSteamIdsReloadCoroutine();
		}
		if (Application.isBatchMode)
		{
			Application.targetFrameRate = this.serverManager.ServerConfigurationManager.ServerConfiguration.targetFrameRate;
		}
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Server_OnServerReady", null);
	}

	// Token: 0x06000451 RID: 1105 RVA: 0x00009AE5 File Offset: 0x00007CE5
	private void Event_Server_OnServerReady(Dictionary<string, object> message)
	{
		this.serverManager.Server_Authenticate(new string[0]);
	}

	// Token: 0x06000452 RID: 1106 RVA: 0x0001D1B8 File Offset: 0x0001B3B8
	private void Event_Server_OnServerStopped(Dictionary<string, object> message)
	{
		this.serverManager.Server_StopMetricsCoroutine();
		this.serverManager.Server_StopBannedSteamIdsReloadCoroutine();
		this.serverManager.Server_Unauthenticate();
		this.serverManager.UdpSocket.StopSocket();
		this.serverManager.ConnectionApprovalRequests.Clear();
		uPnPHelper.CloseAll();
	}

	// Token: 0x06000453 RID: 1107 RVA: 0x0001D20C File Offset: 0x0001B40C
	private void Event_Server_ConnectionApproval(Dictionary<string, object> message)
	{
		ulong item = (ulong)message["clientId"];
		if (!(bool)message["approved"])
		{
			return;
		}
		this.approvedClients.Add(item);
	}

	// Token: 0x06000454 RID: 1108 RVA: 0x0001D24C File Offset: 0x0001B44C
	private void Event_Server_OnVoteSuccess(Dictionary<string, object> message)
	{
		Vote vote = (Vote)message["vote"];
		if (vote.Type == VoteType.Kick)
		{
			FixedString32Bytes steamId = (FixedString32Bytes)vote.Data;
			Player playerBySteamId = NetworkBehaviourSingleton<PlayerManager>.Instance.GetPlayerBySteamId(steamId);
			if (!playerBySteamId)
			{
				return;
			}
			Debug.Log(string.Format("[ServerManagerController] Vote succeeded to kick player {0} ({1}) ({2}/{3})", new object[]
			{
				playerBySteamId.Username.Value,
				playerBySteamId.OwnerClientId,
				vote.Votes,
				vote.VotesNeeded
			}));
			this.serverManager.Server_KickPlayer(playerBySteamId, DisconnectionCode.Kicked, true);
		}
	}

	// Token: 0x06000455 RID: 1109 RVA: 0x0001D2F8 File Offset: 0x0001B4F8
	private void Event_Server_OnPlayerSubscription(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		if (player.IsReplay.Value)
		{
			return;
		}
		string[] playerSteamIds = NetworkBehaviourSingleton<PlayerManager>.Instance.GetPlayerSteamIds();
		this.serverManager.Server_UpdateConnectedSteamIds(playerSteamIds);
		if (player.OwnerClientId != 0UL)
		{
			this.serverManager.Server_ServerConfigurationRpc(this.serverManager.Server, base.RpcTarget.Single(player.OwnerClientId, RpcTargetUse.Temp));
		}
	}

	// Token: 0x06000456 RID: 1110 RVA: 0x0001D370 File Offset: 0x0001B570
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
		bool flag = this.serverManager.AdminSteamIds.Contains(playerByClientId.SteamId.Value.ToString());
		if (!(a == "/kick"))
		{
			if (!(a == "/ban"))
			{
				if (!(a == "/bansteamid"))
				{
					if (!(a == "/unbansteamid"))
					{
						return;
					}
					if (flag)
					{
						if (array.Length < 1)
						{
							return;
						}
						string text = array[0];
						this.serverManager.Server_UnbanSteamId(text);
						NetworkBehaviourSingleton<UIChat>.Instance.Server_SendSystemChatMessage("<b><color=orange>ADMIN</color></b> unbanned Steam ID " + text + ".");
					}
				}
				else if (flag)
				{
					if (array.Length < 1)
					{
						return;
					}
					string text2 = array[0];
					this.serverManager.Server_BanSteamId(text2);
					NetworkBehaviourSingleton<UIChat>.Instance.Server_SendSystemChatMessage("<b><color=orange>ADMIN</color></b> banned Steam ID " + text2 + ".");
					return;
				}
			}
			else if (flag)
			{
				if (array.Length < 1)
				{
					return;
				}
				Player player = NetworkBehaviourSingleton<PlayerManager>.Instance.GetPlayerByUsername(array[0], false);
				if (!player)
				{
					int number;
					if (int.TryParse(array[0], out number))
					{
						player = NetworkBehaviourSingleton<PlayerManager>.Instance.GetPlayerByNumber(number);
					}
					if (!player)
					{
						return;
					}
				}
				NetworkBehaviourSingleton<UIChat>.Instance.Server_SendSystemChatMessage(string.Format("<b><color=orange>ADMIN</color></b> banned {0}.", player.Username.Value));
				this.serverManager.Server_BanPlayer(player);
				return;
			}
		}
		else if (flag)
		{
			if (array.Length < 1)
			{
				return;
			}
			Player player2 = NetworkBehaviourSingleton<PlayerManager>.Instance.GetPlayerByUsername(array[0], false);
			if (!player2)
			{
				int number2;
				if (int.TryParse(array[0], out number2))
				{
					player2 = NetworkBehaviourSingleton<PlayerManager>.Instance.GetPlayerByNumber(number2);
				}
				if (!player2)
				{
					return;
				}
			}
			NetworkBehaviourSingleton<UIChat>.Instance.Server_SendSystemChatMessage(string.Format("<b><color=orange>ADMIN</color></b> kicked {0}.", player2.Username.Value));
			this.serverManager.Server_KickPlayer(player2, DisconnectionCode.Kicked, true);
			return;
		}
	}

	// Token: 0x06000457 RID: 1111 RVA: 0x0001D5A8 File Offset: 0x0001B7A8
	private void Event_Server_OnPlayerSleepInput(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		if ((bool)message["value"])
		{
			this.serverManager.Server_KickPlayer(player, DisconnectionCode.Kicked, false);
		}
	}

	// Token: 0x06000458 RID: 1112 RVA: 0x0001D5E8 File Offset: 0x0001B7E8
	private void Event_Client_OnMainMenuClickHostServer(Dictionary<string, object> message)
	{
		ushort port = (ushort)message["port"];
		string password = (string)message["password"];
		this.serverManager.Client_StartHost(port, "MY PUCK SERVER", 12, password, true, false, MonoBehaviourSingleton<StateManager>.Instance.PlayerData.steamId, true);
	}

	// Token: 0x06000459 RID: 1113 RVA: 0x0001D640 File Offset: 0x0001B840
	private void Event_Client_OnMainMenuClickPractice(Dictionary<string, object> message)
	{
		this.serverManager.Client_StartHost(7777, "PRACTICE", 1, "", false, false, MonoBehaviourSingleton<StateManager>.Instance.PlayerData.steamId, false);
	}

	// Token: 0x0600045A RID: 1114 RVA: 0x0001D67C File Offset: 0x0001B87C
	private void Event_Client_OnServerLauncherClickStartSelfHostedServer(Dictionary<string, object> message)
	{
		int num = (int)message["port"];
		string name = (string)message["name"];
		int maxPlayers = (int)message["maxPlayers"];
		string password = (string)message["password"];
		bool voip = (bool)message["voip"];
		this.serverManager.Client_StartHost((ushort)num, name, maxPlayers, password, voip, true, MonoBehaviourSingleton<StateManager>.Instance.PlayerData.steamId, true);
	}

	// Token: 0x0600045B RID: 1115 RVA: 0x0001D704 File Offset: 0x0001B904
	private void WebSocket_Event_OnConnect(Dictionary<string, object> message)
	{
		if (NetworkManager.Singleton)
		{
			if (Application.isBatchMode && !NetworkManager.Singleton.IsServer)
			{
				this.serverManager.Client_StartServer(this.serverManager.ServerConfigurationManager.ServerConfiguration.port, true);
			}
			if (NetworkManager.Singleton.IsServer)
			{
				this.serverManager.Server_Authenticate(NetworkBehaviourSingleton<PlayerManager>.Instance.GetPlayerSteamIds());
				return;
			}
		}
		else if (Application.isBatchMode)
		{
			this.serverManager.Client_StartServer(this.serverManager.ServerConfigurationManager.ServerConfiguration.port, true);
		}
	}

	// Token: 0x0600045C RID: 1116 RVA: 0x0001D79C File Offset: 0x0001B99C
	private void WebSocket_Event_OnServerAuthenticateResponse(Dictionary<string, object> message)
	{
		ServerAuthenticateResponse value = ((SocketIOResponse)message["response"]).GetValue<ServerAuthenticateResponse>(0);
		if (value.success)
		{
			this.serverManager.Server.IpAddress = value.ipAddress;
			this.serverManager.Server.IsAuthenticated = value.isAuthenticated;
			Debug.Log(string.Format("[ServerManagerController] Server authenticated {0} {1}", this.serverManager.Server.IpAddress, this.serverManager.Server.IsAuthenticated));
			if (NetworkManager.Singleton.IsHost)
			{
				this.serverManager.Server_ServerConfigurationRpc(this.serverManager.Server, base.RpcTarget.Single(0UL, RpcTargetUse.Temp));
			}
		}
	}

	// Token: 0x0600045D RID: 1117 RVA: 0x0001D86C File Offset: 0x0001BA6C
	private void WebSocket_Event_OnServerConnectionApprovalResponse(Dictionary<string, object> message)
	{
		string steamId = ((SocketIOResponse)message["response"]).GetValue<ServerConnectionApprovalResponse>(0).steamId;
		if (!this.serverManager.ConnectionApprovalRequests.ContainsKey(steamId))
		{
			return;
		}
		NetworkManager.ConnectionApprovalResponse connectionApprovalResponse = this.serverManager.ConnectionApprovalRequests[steamId];
		bool flag = NetworkManager.Singleton.ConnectedClientsList.Count >= this.serverManager.Server.MaxPlayers;
		connectionApprovalResponse.Approved = !flag;
		if (connectionApprovalResponse.Approved)
		{
			connectionApprovalResponse.Pending = false;
			Debug.Log("[ServerManager] Connection approved for (" + steamId + ")");
		}
		else
		{
			ConnectionRejectionCode code = ConnectionRejectionCode.Unreachable;
			if (flag)
			{
				code = ConnectionRejectionCode.ServerFull;
			}
			string reason = JsonSerializer.Serialize<ConnectionRejection>(new ConnectionRejection
			{
				code = code,
				clientRequiredModIds = this.serverManager.ServerConfigurationManager.ClientRequiredModIds
			}, new JsonSerializerOptions
			{
				WriteIndented = true
			});
			connectionApprovalResponse.Pending = false;
			connectionApprovalResponse.Reason = reason;
			Debug.Log("[ServerManager] Connection rejected for (" + steamId + "): " + Utils.GetConnectionRejectionMessage(code));
		}
		this.serverManager.ConnectionApprovalRequests.Remove(steamId);
	}

	// Token: 0x0600045E RID: 1118 RVA: 0x0001D988 File Offset: 0x0001BB88
	private void WebSocket_Event_OnServerKickPlayer(Dictionary<string, object> message)
	{
		ServerKickPlayer value = ((SocketIOResponse)message["response"]).GetValue<ServerKickPlayer>(0);
		Player playerBySteamId = NetworkBehaviourSingleton<PlayerManager>.Instance.GetPlayerBySteamId(value.steamId);
		if (!playerBySteamId)
		{
			return;
		}
		this.serverManager.Server_KickPlayer(playerBySteamId, DisconnectionCode.Kicked, false);
	}

	// Token: 0x06000460 RID: 1120 RVA: 0x0000F5B8 File Offset: 0x0000D7B8
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x06000461 RID: 1121 RVA: 0x00006C33 File Offset: 0x00004E33
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x06000462 RID: 1122 RVA: 0x00009B0B File Offset: 0x00007D0B
	protected internal override string __getTypeName()
	{
		return "ServerManagerController";
	}

	// Token: 0x04000279 RID: 633
	private ServerManager serverManager;

	// Token: 0x0400027A RID: 634
	private List<ulong> approvedClients = new List<ulong>();
}
