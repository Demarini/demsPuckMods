using System;
using System.Collections.Generic;
using System.Text.Json;
using Unity.Multiplayer.Tools.NetStatsMonitor;
using Unity.Netcode;
using UnityEngine;

// Token: 0x020000D5 RID: 213
public class ConnectionManagerController : MonoBehaviour
{
	// Token: 0x06000686 RID: 1670 RVA: 0x00020D4C File Offset: 0x0001EF4C
	private void Awake()
	{
		this.connectionManager = base.GetComponent<ConnectionManager>();
		this.runtimeNetStatsMonitor = base.GetComponent<RuntimeNetStatsMonitor>();
		EventManager.AddEventListener("Event_Server_OnConnectionRejected", new Action<Dictionary<string, object>>(this.Event_Server_OnConnectionRejected));
		EventManager.AddEventListener("Event_OnClientStarted", new Action<Dictionary<string, object>>(this.Event_OnClientStarted));
		EventManager.AddEventListener("Event_OnClientStopped", new Action<Dictionary<string, object>>(this.Event_OnClientStopped));
		EventManager.AddEventListener("Event_OnClientConnected", new Action<Dictionary<string, object>>(this.Event_OnClientConnected));
		EventManager.AddEventListener("Event_OnMainMenuClickJoinServer", new Action<Dictionary<string, object>>(this.Event_OnMainMenuClickJoinServer));
		EventManager.AddEventListener("Event_OnPauseMenuClickDisconnect", new Action<Dictionary<string, object>>(this.Event_OnPauseMenuClickDisconnect));
		EventManager.AddEventListener("Event_OnDebugChanged", new Action<Dictionary<string, object>>(this.Event_OnDebugChanged));
		EventManager.AddEventListener("Event_OnGotLaunchCommandLine", new Action<Dictionary<string, object>>(this.Event_OnGotLaunchCommandLine));
		EventManager.AddEventListener("Event_OnGameRichPresenceJoinRequested", new Action<Dictionary<string, object>>(this.Event_OnGameRichPresenceJoinRequested));
		EventManager.AddEventListener("Event_OnServerBrowserClickEndPoint", new Action<Dictionary<string, object>>(this.Event_OnServerBrowserClickEndPoint));
		EventManager.AddEventListener("Event_OnPendingModsCleared", new Action<Dictionary<string, object>>(this.Event_OnPendingModsCleared));
		EventManager.AddEventListener("Event_OnPopupClickOk", new Action<Dictionary<string, object>>(this.Event_OnPopupClickOk));
		EventManager.AddEventListener("Event_OnMatchmakingMatchingClickConnect", new Action<Dictionary<string, object>>(this.Event_OnMatchmakingMatchingClickConnect));
		EventManager.AddEventListener("Event_OnConnectionStateChanged", new Action<Dictionary<string, object>>(this.Event_OnConnectionStateChanged));
	}

	// Token: 0x06000687 RID: 1671 RVA: 0x00020EA5 File Offset: 0x0001F0A5
	private void Start()
	{
		NetworkManager.Singleton.NetworkConfig.NetworkMessageMetrics = SettingsManager.Debug;
		NetworkManager.Singleton.NetworkConfig.NetworkProfilingMetrics = SettingsManager.Debug;
		this.UpdateRnsmVisibility();
	}

	// Token: 0x06000688 RID: 1672 RVA: 0x00020ED8 File Offset: 0x0001F0D8
	private void OnDestroy()
	{
		EventManager.RemoveEventListener("Event_Server_OnConnectionRejected", new Action<Dictionary<string, object>>(this.Event_Server_OnConnectionRejected));
		EventManager.RemoveEventListener("Event_OnClientStarted", new Action<Dictionary<string, object>>(this.Event_OnClientStarted));
		EventManager.RemoveEventListener("Event_OnClientStopped", new Action<Dictionary<string, object>>(this.Event_OnClientStopped));
		EventManager.RemoveEventListener("Event_OnClientConnected", new Action<Dictionary<string, object>>(this.Event_OnClientConnected));
		EventManager.RemoveEventListener("Event_OnMainMenuClickJoinServer", new Action<Dictionary<string, object>>(this.Event_OnMainMenuClickJoinServer));
		EventManager.RemoveEventListener("Event_OnPauseMenuClickDisconnect", new Action<Dictionary<string, object>>(this.Event_OnPauseMenuClickDisconnect));
		EventManager.RemoveEventListener("Event_OnDebugChanged", new Action<Dictionary<string, object>>(this.Event_OnDebugChanged));
		EventManager.RemoveEventListener("Event_OnGotLaunchCommandLine", new Action<Dictionary<string, object>>(this.Event_OnGotLaunchCommandLine));
		EventManager.RemoveEventListener("Event_OnGameRichPresenceJoinRequested", new Action<Dictionary<string, object>>(this.Event_OnGameRichPresenceJoinRequested));
		EventManager.RemoveEventListener("Event_OnServerBrowserClickEndPoint", new Action<Dictionary<string, object>>(this.Event_OnServerBrowserClickEndPoint));
		EventManager.RemoveEventListener("Event_OnPendingModsCleared", new Action<Dictionary<string, object>>(this.Event_OnPendingModsCleared));
		EventManager.RemoveEventListener("Event_OnPopupClickOk", new Action<Dictionary<string, object>>(this.Event_OnPopupClickOk));
		EventManager.RemoveEventListener("Event_OnMatchmakingMatchingClickConnect", new Action<Dictionary<string, object>>(this.Event_OnMatchmakingMatchingClickConnect));
		EventManager.RemoveEventListener("Event_OnConnectionStateChanged", new Action<Dictionary<string, object>>(this.Event_OnConnectionStateChanged));
	}

	// Token: 0x06000689 RID: 1673 RVA: 0x00021019 File Offset: 0x0001F219
	private void UpdateRnsmVisibility()
	{
		this.runtimeNetStatsMonitor.Visible = (SettingsManager.Debug && GlobalStateManager.ConnectionState.IsConnected);
	}

	// Token: 0x0600068A RID: 1674 RVA: 0x0002103C File Offset: 0x0001F23C
	private void HandleConnectionRejection(string reason)
	{
		ConnectionRejection value;
		try
		{
			value = JsonSerializer.Deserialize<ConnectionRejection>(reason, null);
		}
		catch
		{
			value = new ConnectionRejection
			{
				code = ConnectionRejectionCode.Unreachable
			};
		}
		EventManager.TriggerEvent("Event_OnConnectionRejected", new Dictionary<string, object>
		{
			{
				"connectionRejection",
				value
			}
		});
	}

	// Token: 0x0600068B RID: 1675 RVA: 0x00021090 File Offset: 0x0001F290
	private void HandleDisconnection(string reason)
	{
		Disconnection value;
		try
		{
			value = JsonSerializer.Deserialize<Disconnection>(reason, null);
		}
		catch
		{
			if (NetworkManager.Singleton.DisconnectEvent == NetworkTransport.DisconnectEvents.TransportShutdown)
			{
				value = new Disconnection
				{
					code = DisconnectionCode.Disconnected
				};
			}
			else
			{
				value = new Disconnection
				{
					code = DisconnectionCode.ConnectionLost
				};
			}
		}
		EventManager.TriggerEvent("Event_OnDisconnected", new Dictionary<string, object>
		{
			{
				"disconnection",
				value
			}
		});
	}

	// Token: 0x0600068C RID: 1676 RVA: 0x00021100 File Offset: 0x0001F300
	private void Event_Server_OnConnectionRejected(Dictionary<string, object> message)
	{
		ConnectionApproval connectionApproval = (ConnectionApproval)message["connectionApproval"];
		if (connectionApproval.IsHost)
		{
			this.connectionManager.Client_Disconnect();
			this.HandleConnectionRejection(connectionApproval.Response.Reason);
		}
	}

	// Token: 0x0600068D RID: 1677 RVA: 0x00021142 File Offset: 0x0001F342
	private void Event_OnClientStarted(Dictionary<string, object> message)
	{
		GlobalStateManager.SetConnectionState(new Dictionary<string, object>
		{
			{
				"isConnecting",
				true
			},
			{
				"isConnected",
				false
			}
		});
	}

	// Token: 0x0600068E RID: 1678 RVA: 0x00021170 File Offset: 0x0001F370
	private void Event_OnClientStopped(Dictionary<string, object> message)
	{
		bool isConnecting = GlobalStateManager.ConnectionState.IsConnecting;
		GlobalStateManager.SetConnectionState(new Dictionary<string, object>
		{
			{
				"isConnecting",
				false
			},
			{
				"isConnected",
				false
			},
			{
				"connection",
				null
			},
			{
				"lastConnection",
				GlobalStateManager.ConnectionState.Connection
			}
		});
		if (isConnecting)
		{
			this.HandleConnectionRejection(NetworkManager.Singleton.DisconnectReason);
		}
		else
		{
			this.HandleDisconnection(NetworkManager.Singleton.DisconnectReason);
		}
		Connection pendingConnection = GlobalStateManager.ConnectionState.PendingConnection;
		if (pendingConnection != null)
		{
			this.connectionManager.Client_StartClient(pendingConnection.EndPoint.ipAddress, pendingConnection.EndPoint.port, pendingConnection.Password);
		}
	}

	// Token: 0x0600068F RID: 1679 RVA: 0x0002122D File Offset: 0x0001F42D
	private void Event_OnClientConnected(Dictionary<string, object> message)
	{
		GlobalStateManager.SetConnectionState(new Dictionary<string, object>
		{
			{
				"isConnecting",
				false
			},
			{
				"isConnected",
				true
			}
		});
	}

	// Token: 0x06000690 RID: 1680 RVA: 0x0002125C File Offset: 0x0001F45C
	private void Event_OnMainMenuClickJoinServer(Dictionary<string, object> message)
	{
		string ipAddress = (string)message["ipAddress"];
		ushort port = (ushort)message["port"];
		string password = (string)message["password"];
		this.connectionManager.Client_StartClient(ipAddress, port, password);
	}

	// Token: 0x06000691 RID: 1681 RVA: 0x000212AA File Offset: 0x0001F4AA
	private void Event_OnPauseMenuClickDisconnect(Dictionary<string, object> message)
	{
		this.connectionManager.Client_Disconnect();
	}

	// Token: 0x06000692 RID: 1682 RVA: 0x000212B8 File Offset: 0x0001F4B8
	private void Event_OnDebugChanged(Dictionary<string, object> message)
	{
		bool flag = (bool)message["value"];
		NetworkManager.Singleton.NetworkConfig.NetworkMessageMetrics = flag;
		NetworkManager.Singleton.NetworkConfig.NetworkProfilingMetrics = flag;
		this.UpdateRnsmVisibility();
	}

	// Token: 0x06000693 RID: 1683 RVA: 0x000212FC File Offset: 0x0001F4FC
	private void Event_OnGotLaunchCommandLine(Dictionary<string, object> message)
	{
		string[] args = (string[])message["args"];
		string commandLineArgument = Utils.GetCommandLineArgument("+ipAddress", args);
		ushort num;
		ushort port = ushort.TryParse(Utils.GetCommandLineArgument("+port", args), out num) ? num : 30609;
		string commandLineArgument2 = Utils.GetCommandLineArgument("+password", args);
		if (string.IsNullOrEmpty(commandLineArgument))
		{
			return;
		}
		this.connectionManager.Client_StartClient(commandLineArgument, port, commandLineArgument2);
	}

	// Token: 0x06000694 RID: 1684 RVA: 0x00021368 File Offset: 0x0001F568
	private void Event_OnGameRichPresenceJoinRequested(Dictionary<string, object> message)
	{
		string[] args = (string[])message["args"];
		string commandLineArgument = Utils.GetCommandLineArgument("+ipAddress", args);
		ushort num;
		ushort port = ushort.TryParse(Utils.GetCommandLineArgument("+port", args), out num) ? num : 30609;
		string commandLineArgument2 = Utils.GetCommandLineArgument("+password", args);
		if (string.IsNullOrEmpty(commandLineArgument))
		{
			return;
		}
		this.connectionManager.Client_StartClient(commandLineArgument, port, commandLineArgument2);
	}

	// Token: 0x06000695 RID: 1685 RVA: 0x000213D4 File Offset: 0x0001F5D4
	private void Event_OnServerBrowserClickEndPoint(Dictionary<string, object> message)
	{
		EndPoint endPoint = (EndPoint)message["endPoint"];
		this.connectionManager.Client_StartClient(endPoint.ipAddress, endPoint.port, null);
	}

	// Token: 0x06000696 RID: 1686 RVA: 0x0002140C File Offset: 0x0001F60C
	private void Event_OnPendingModsCleared(Dictionary<string, object> message)
	{
		Connection lastConnection = GlobalStateManager.ConnectionState.LastConnection;
		if (lastConnection == null)
		{
			return;
		}
		this.connectionManager.Client_StartClient(lastConnection.EndPoint.ipAddress, lastConnection.EndPoint.port, lastConnection.Password);
	}

	// Token: 0x06000697 RID: 1687 RVA: 0x00021450 File Offset: 0x0001F650
	private void Event_OnPopupClickOk(Dictionary<string, object> message)
	{
		if ((string)message["name"] != "missingPassword")
		{
			return;
		}
		Connection lastConnection = GlobalStateManager.ConnectionState.LastConnection;
		if (lastConnection == null)
		{
			return;
		}
		string password = ((PopupPasswordContent)message["content"]).Password;
		this.connectionManager.Client_StartClient(lastConnection.EndPoint.ipAddress, lastConnection.EndPoint.port, password);
	}

	// Token: 0x06000698 RID: 1688 RVA: 0x000214C4 File Offset: 0x0001F6C4
	private void Event_OnMatchmakingMatchingClickConnect(Dictionary<string, object> message)
	{
		EndPoint endPoint = BackendManager.PlayerState.MatchData.endPoint;
		if (endPoint == null)
		{
			return;
		}
		this.connectionManager.Client_StartClient(endPoint.ipAddress, endPoint.port, null);
	}

	// Token: 0x06000699 RID: 1689 RVA: 0x00021503 File Offset: 0x0001F703
	private void Event_OnConnectionStateChanged(Dictionary<string, object> message)
	{
		this.UpdateRnsmVisibility();
	}

	// Token: 0x040003FD RID: 1021
	private ConnectionManager connectionManager;

	// Token: 0x040003FE RID: 1022
	private RuntimeNetStatsMonitor runtimeNetStatsMonitor;
}
