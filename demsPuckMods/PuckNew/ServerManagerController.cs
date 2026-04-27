using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

// Token: 0x02000122 RID: 290
public class ServerManagerController : MonoBehaviour
{
	// Token: 0x0600081D RID: 2077 RVA: 0x00026F14 File Offset: 0x00025114
	private void Awake()
	{
		this.serverManager = base.GetComponent<ServerManager>();
		EventManager.AddEventListener("Event_Everyone_OnClientConnected", new Action<Dictionary<string, object>>(this.Event_Everyone_OnClientConnected));
		EventManager.AddEventListener("Event_Everyone_OnClientDisconnected", new Action<Dictionary<string, object>>(this.Event_Everyone_OnClientDisconnected));
		EventManager.AddEventListener("Event_Server_OnServerStarted", new Action<Dictionary<string, object>>(this.Event_Server_OnServerStarted));
		EventManager.AddEventListener("Event_Server_OnServerStopped", new Action<Dictionary<string, object>>(this.Event_Server_OnServerStopped));
		EventManager.AddEventListener("Event_Server_OnVoteSuccess", new Action<Dictionary<string, object>>(this.Event_Server_OnVoteSuccess));
		EventManager.AddEventListener("Event_Server_OnChatCommand", new Action<Dictionary<string, object>>(this.Event_Server_OnChatCommand));
		EventManager.AddEventListener("Event_OnServerStateChanged", new Action<Dictionary<string, object>>(this.Event_OnServerStateChanged));
		EventManager.AddEventListener("Event_OnTransportFailure", new Action<Dictionary<string, object>>(this.Event_OnTransportFailure));
		EventManager.AddEventListener("Event_OnMainMenuClickHostServer", new Action<Dictionary<string, object>>(this.Event_OnMainMenuClickHostServer));
		EventManager.AddEventListener("Event_OnNewServerClickStart", new Action<Dictionary<string, object>>(this.Event_OnNewServerClickStart));
		EventManager.AddEventListener("Event_OnPlayClickPractice", new Action<Dictionary<string, object>>(this.Event_OnPlayClickPractice));
		WebSocketManager.AddMessageListener("connected", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnConnected));
		WebSocketManager.AddMessageListener("serverKickPlayer", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnServerKickPlayer));
	}

	// Token: 0x0600081E RID: 2078 RVA: 0x000020D3 File Offset: 0x000002D3
	private void Start()
	{
	}

	// Token: 0x0600081F RID: 2079 RVA: 0x0002704C File Offset: 0x0002524C
	private void OnDestroy()
	{
		EventManager.RemoveEventListener("Event_Everyone_OnClientConnected", new Action<Dictionary<string, object>>(this.Event_Everyone_OnClientConnected));
		EventManager.RemoveEventListener("Event_Everyone_OnClientDisconnected", new Action<Dictionary<string, object>>(this.Event_Everyone_OnClientDisconnected));
		EventManager.RemoveEventListener("Event_Server_OnServerStarted", new Action<Dictionary<string, object>>(this.Event_Server_OnServerStarted));
		EventManager.RemoveEventListener("Event_Server_OnServerStopped", new Action<Dictionary<string, object>>(this.Event_Server_OnServerStopped));
		EventManager.RemoveEventListener("Event_Server_OnVoteSuccess", new Action<Dictionary<string, object>>(this.Event_Server_OnVoteSuccess));
		EventManager.RemoveEventListener("Event_Server_OnChatCommand", new Action<Dictionary<string, object>>(this.Event_Server_OnChatCommand));
		EventManager.RemoveEventListener("Event_OnServerStateChanged", new Action<Dictionary<string, object>>(this.Event_OnServerStateChanged));
		EventManager.RemoveEventListener("Event_OnTransportFailure", new Action<Dictionary<string, object>>(this.Event_OnTransportFailure));
		EventManager.RemoveEventListener("Event_OnMainMenuClickHostServer", new Action<Dictionary<string, object>>(this.Event_OnMainMenuClickHostServer));
		EventManager.RemoveEventListener("Event_OnNewServerClickStart", new Action<Dictionary<string, object>>(this.Event_OnNewServerClickStart));
		EventManager.RemoveEventListener("Event_OnPlayClickPractice", new Action<Dictionary<string, object>>(this.Event_OnPlayClickPractice));
		WebSocketManager.RemoveMessageListener("connected", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnConnected));
		WebSocketManager.RemoveMessageListener("serverKickPlayer", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnServerKickPlayer));
	}

	// Token: 0x06000820 RID: 2080 RVA: 0x00027178 File Offset: 0x00025378
	private void Event_Everyone_OnClientConnected(Dictionary<string, object> message)
	{
		ulong num = (ulong)message["clientId"];
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		Debug.Log(string.Format("[ServerManagerController] Client connected ({0}) {1}/{2}", num, NetworkManager.Singleton.ConnectedClientsList.Count, this.serverManager.ServerConfig.maxPlayers));
	}

	// Token: 0x06000821 RID: 2081 RVA: 0x000271E4 File Offset: 0x000253E4
	private void Event_Everyone_OnClientDisconnected(Dictionary<string, object> message)
	{
		ulong num = (ulong)message["clientId"];
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		Debug.Log(string.Format("[ServerManagerController] Client disconnected ({0}) {1}/{2}", num, NetworkManager.Singleton.ConnectedClientsList.Count, this.serverManager.ServerConfig.maxPlayers));
	}

	// Token: 0x06000822 RID: 2082 RVA: 0x00027250 File Offset: 0x00025450
	private void Event_OnServerStateChanged(Dictionary<string, object> message)
	{
		ref ServerState ptr = (ServerState)message["oldServerState"];
		ServerState serverState = (ServerState)message["newServerState"];
		if (ptr.AuthenticationPhase == AuthenticationPhase.None && serverState.AuthenticationPhase == AuthenticationPhase.Authenticated)
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
	}

	// Token: 0x06000823 RID: 2083 RVA: 0x000272D6 File Offset: 0x000254D6
	private void Event_OnTransportFailure(Dictionary<string, object> message)
	{
		if (this.serverManager.IsHostStartInProgress)
		{
			this.serverManager.IsHostStartInProgress = false;
		}
		if (this.serverManager.IsServerStartInProgress)
		{
			this.serverManager.IsServerStartInProgress = false;
		}
	}

	// Token: 0x06000824 RID: 2084 RVA: 0x0002730C File Offset: 0x0002550C
	private void Event_Server_OnServerStarted(Dictionary<string, object> message)
	{
		this.serverManager.Server.Value = new Server
		{
			IpAddress = this.serverManager.IpAddress,
			Port = this.serverManager.ServerConfig.port,
			Name = this.serverManager.ServerConfig.name,
			MaxPlayers = this.serverManager.ServerConfig.maxPlayers,
			TickRate = this.serverManager.ServerConfig.tickRate,
			UseVoip = this.serverManager.ServerConfig.useVoip
		};
		this.serverManager.StartTcpServer(this.serverManager.ServerConfig.port);
	}

	// Token: 0x06000825 RID: 2085 RVA: 0x000273DC File Offset: 0x000255DC
	private void Event_Server_OnServerStopped(Dictionary<string, object> message)
	{
		this.serverManager.StopTcpServer();
		this.serverManager.StopPortForwarding();
		this.serverManager.Unauthenticate();
	}

	// Token: 0x06000826 RID: 2086 RVA: 0x00027400 File Offset: 0x00025600
	private void Event_Server_OnVoteSuccess(Dictionary<string, object> message)
	{
		Vote vote = (Vote)message["vote"];
		if (vote.Type == VoteType.Kick)
		{
			FixedString32Bytes steamId = (FixedString32Bytes)vote.Data;
			Player playerBySteamId = MonoBehaviourSingleton<PlayerManager>.Instance.GetPlayerBySteamId(steamId);
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
			this.serverManager.Server_KickPlayer(playerBySteamId, DisconnectionCode.Kicked, null, true);
		}
	}

	// Token: 0x06000827 RID: 2087 RVA: 0x000274AC File Offset: 0x000256AC
	private void Event_Server_OnChatCommand(Dictionary<string, object> message)
	{
		ulong clientId = (ulong)message["clientId"];
		string a = (string)message["command"];
		string[] array = (string[])message["args"];
		Player playerByClientId = MonoBehaviourSingleton<PlayerManager>.Instance.GetPlayerByClientId(clientId);
		if (!playerByClientId)
		{
			return;
		}
		bool flag = this.serverManager.AdminManager.IsSteamIdAdmin(playerByClientId.SteamId.Value.ToString());
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
						NetworkBehaviourSingleton<ChatManager>.Instance.BroadcastMessage("<b><color=orange>ADMIN</color></b> unbanned Steam ID " + text);
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
					NetworkBehaviourSingleton<ChatManager>.Instance.BroadcastMessage("<b><color=orange>ADMIN</color></b> banned Steam ID " + text2);
					return;
				}
			}
			else if (flag)
			{
				if (array.Length < 1)
				{
					return;
				}
				Player player = MonoBehaviourSingleton<PlayerManager>.Instance.GetPlayerByUsername(array[0], false);
				if (!player)
				{
					int number;
					if (int.TryParse(array[0], out number))
					{
						player = MonoBehaviourSingleton<PlayerManager>.Instance.GetPlayerByNumber(number);
					}
					if (!player)
					{
						return;
					}
				}
				NetworkBehaviourSingleton<ChatManager>.Instance.BroadcastMessage(string.Format("<b><color=orange>ADMIN</color></b> banned {0}", player.Username.Value));
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
			Player player2 = MonoBehaviourSingleton<PlayerManager>.Instance.GetPlayerByUsername(array[0], false);
			if (!player2)
			{
				int number2;
				if (int.TryParse(array[0], out number2))
				{
					player2 = MonoBehaviourSingleton<PlayerManager>.Instance.GetPlayerByNumber(number2);
				}
				if (!player2)
				{
					return;
				}
			}
			NetworkBehaviourSingleton<ChatManager>.Instance.BroadcastMessage(string.Format("<b><color=orange>ADMIN</color></b> kicked {0}", player2.Username.Value));
			this.serverManager.Server_KickPlayer(player2, DisconnectionCode.Kicked, null, true);
			return;
		}
	}

	// Token: 0x06000828 RID: 2088 RVA: 0x000276DC File Offset: 0x000258DC
	private void Event_OnMainMenuClickHostServer(Dictionary<string, object> message)
	{
		ushort port = (ushort)message["port"];
		string password = (string)message["password"];
		this.serverManager.StartHost(port, "MY PUCK SERVER", 12, password, true, true, true);
	}

	// Token: 0x06000829 RID: 2089 RVA: 0x00027724 File Offset: 0x00025924
	private void Event_OnNewServerClickStart(Dictionary<string, object> message)
	{
		if ((string)message["type"] != "selfHosted")
		{
			return;
		}
		int num = (int)message["port"];
		string name = (string)message["name"];
		int maxPlayers = (int)message["maxPlayers"];
		string password = (string)message["password"];
		bool useVoip = (bool)message["useVoip"];
		this.serverManager.StartHost((ushort)num, name, maxPlayers, password, true, useVoip, true);
	}

	// Token: 0x0600082A RID: 2090 RVA: 0x000277B8 File Offset: 0x000259B8
	private void Event_OnPlayClickPractice(Dictionary<string, object> message)
	{
		this.serverManager.StartHost(30609, "PRACTICE", 1, null, false, false, false);
	}

	// Token: 0x0600082B RID: 2091 RVA: 0x000277D4 File Offset: 0x000259D4
	private void WebSocket_Event_OnConnected(Dictionary<string, object> message)
	{
		if (ApplicationManager.IsDedicatedGameServer)
		{
			if (NetworkManager.Singleton.IsServer)
			{
				this.serverManager.Authenticate();
				return;
			}
			this.serverManager.StartServer(this.serverManager.ServerConfig.port, true);
		}
	}

	// Token: 0x0600082C RID: 2092 RVA: 0x00027814 File Offset: 0x00025A14
	private void WebSocket_Event_OnServerKickPlayer(Dictionary<string, object> message)
	{
		ServerKickPlayer data = ((InMessage)message["inMessage"]).GetData<ServerKickPlayer>();
		Player playerBySteamId = MonoBehaviourSingleton<PlayerManager>.Instance.GetPlayerBySteamId(data.steamId);
		if (!playerBySteamId)
		{
			return;
		}
		this.serverManager.Server_KickPlayer(playerBySteamId, DisconnectionCode.Kicked, null, false);
	}

	// Token: 0x040004D2 RID: 1234
	private ServerManager serverManager;
}
