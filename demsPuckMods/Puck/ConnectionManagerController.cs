using System;
using System.Collections.Generic;
using System.Text.Json;
using Unity.Multiplayer.Tools.NetStatsMonitor;
using Unity.Netcode;
using UnityEngine;

// Token: 0x02000060 RID: 96
public class ConnectionManagerController : MonoBehaviour
{
	// Token: 0x0600029F RID: 671 RVA: 0x0000898A File Offset: 0x00006B8A
	private void Awake()
	{
		this.connectionManager = base.GetComponent<ConnectionManager>();
		this.runtimeNetStatsMonitor = base.GetComponent<RuntimeNetStatsMonitor>();
	}

	// Token: 0x060002A0 RID: 672 RVA: 0x0001760C File Offset: 0x0001580C
	private void Start()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnClientConnected", new Action<Dictionary<string, object>>(this.Event_OnClientConnected));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnClientDisconnected", new Action<Dictionary<string, object>>(this.Event_OnClientDisconnected));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnIsClientChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnIsClientChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnMainMenuClickJoinServer", new Action<Dictionary<string, object>>(this.Event_Client_OnMainMenuClickJoinServer));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnPauseMenuClickDisconnect", new Action<Dictionary<string, object>>(this.Event_Client_OnPauseMenuClickDisconnect));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnDebugChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnDebugChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnGotLaunchCommandLine", new Action<Dictionary<string, object>>(this.Event_Client_OnGotLaunchCommandLine));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnGameRichPresenceJoinRequested", new Action<Dictionary<string, object>>(this.Event_Client_OnGameRichPresenceJoinRequested));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnServerBrowserClickServer", new Action<Dictionary<string, object>>(this.Event_Client_OnServerBrowserClickServer));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnPendingModsCleared", new Action<Dictionary<string, object>>(this.Event_Client_OnPendingModsCleared));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnPopupClickOk", new Action<Dictionary<string, object>>(this.Event_Client_OnPopupClickOk));
	}

	// Token: 0x060002A1 RID: 673 RVA: 0x00017744 File Offset: 0x00015944
	private void OnDestroy()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnClientConnected", new Action<Dictionary<string, object>>(this.Event_OnClientConnected));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnClientDisconnected", new Action<Dictionary<string, object>>(this.Event_OnClientDisconnected));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnIsClientChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnIsClientChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnMainMenuClickJoinServer", new Action<Dictionary<string, object>>(this.Event_Client_OnMainMenuClickJoinServer));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnPauseMenuClickDisconnect", new Action<Dictionary<string, object>>(this.Event_Client_OnPauseMenuClickDisconnect));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnDebugChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnDebugChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnGotLaunchCommandLine", new Action<Dictionary<string, object>>(this.Event_Client_OnGotLaunchCommandLine));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnGameRichPresenceJoinRequested", new Action<Dictionary<string, object>>(this.Event_Client_OnGameRichPresenceJoinRequested));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnServerBrowserClickServer", new Action<Dictionary<string, object>>(this.Event_Client_OnServerBrowserClickServer));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnPendingModsCleared", new Action<Dictionary<string, object>>(this.Event_Client_OnPendingModsCleared));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnPopupClickOk", new Action<Dictionary<string, object>>(this.Event_Client_OnPopupClickOk));
	}

	// Token: 0x060002A2 RID: 674 RVA: 0x0001787C File Offset: 0x00015A7C
	private void Event_OnClientConnected(Dictionary<string, object> message)
	{
		ulong num = (ulong)message["clientId"];
		if (NetworkManager.Singleton.LocalClientId != num)
		{
			return;
		}
		this.connectionManager.IsConnecting = false;
		this.connectionManager.PendingConnection = null;
	}

	// Token: 0x060002A3 RID: 675 RVA: 0x000178C0 File Offset: 0x00015AC0
	private void Event_OnClientDisconnected(Dictionary<string, object> message)
	{
		ulong num = (ulong)message["clientId"];
		if (NetworkManager.Singleton.LocalClientId != num)
		{
			return;
		}
		if (this.connectionManager.IsConnecting)
		{
			this.connectionManager.IsConnecting = false;
			ConnectionRejection connectionRejection;
			if (string.IsNullOrEmpty(NetworkManager.Singleton.DisconnectReason))
			{
				connectionRejection = new ConnectionRejection
				{
					code = ConnectionRejectionCode.Unreachable
				};
			}
			else
			{
				connectionRejection = JsonSerializer.Deserialize<ConnectionRejection>(NetworkManager.Singleton.DisconnectReason, null);
			}
			Debug.Log("[ConnectionManagerController] Connection rejected: " + Utils.GetConnectionRejectionMessage(connectionRejection.code));
			MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnConnectionRejected", new Dictionary<string, object>
			{
				{
					"connectionRejection",
					connectionRejection
				}
			});
			return;
		}
		Disconnection disconnection;
		if (string.IsNullOrEmpty(NetworkManager.Singleton.DisconnectReason))
		{
			disconnection = new Disconnection
			{
				code = DisconnectionCode.Disconnected
			};
		}
		else
		{
			disconnection = JsonSerializer.Deserialize<Disconnection>(NetworkManager.Singleton.DisconnectReason, null);
		}
		Debug.Log("[ConnectionManagerController] Disconnected: " + Utils.GetDisconnectionMessage(disconnection.code));
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnDisconnected", new Dictionary<string, object>
		{
			{
				"disconnection",
				disconnection
			}
		});
	}

	// Token: 0x060002A4 RID: 676 RVA: 0x000179DC File Offset: 0x00015BDC
	private void Event_Client_OnIsClientChanged(Dictionary<string, object> message)
	{
		if (NetworkManager.Singleton.IsClient)
		{
			return;
		}
		if (!this.connectionManager.IsPendingConnection)
		{
			return;
		}
		string ipAddress = this.connectionManager.PendingConnection.IpAddress;
		ushort port = this.connectionManager.PendingConnection.Port;
		string password = this.connectionManager.PendingConnection.Password;
		this.connectionManager.PendingConnection = null;
		this.connectionManager.Client_StartClient(ipAddress, port, password);
	}

	// Token: 0x060002A5 RID: 677 RVA: 0x00017A54 File Offset: 0x00015C54
	private void Event_Client_OnMainMenuClickJoinServer(Dictionary<string, object> message)
	{
		string ipAddress = (string)message["ip"];
		ushort port = (ushort)message["port"];
		string password = (string)message["password"];
		this.connectionManager.Client_StartClient(ipAddress, port, password);
	}

	// Token: 0x060002A6 RID: 678 RVA: 0x000089A4 File Offset: 0x00006BA4
	private void Event_Client_OnPauseMenuClickDisconnect(Dictionary<string, object> message)
	{
		this.connectionManager.Client_Disconnect();
	}

	// Token: 0x060002A7 RID: 679 RVA: 0x00017AA4 File Offset: 0x00015CA4
	private void Event_Client_OnDebugChanged(Dictionary<string, object> message)
	{
		int num = (int)message["value"];
		NetworkManager.Singleton.NetworkConfig.NetworkMessageMetrics = (num > 0);
		NetworkManager.Singleton.NetworkConfig.NetworkProfilingMetrics = (num > 0);
		this.runtimeNetStatsMonitor.Visible = (num > 0);
	}

	// Token: 0x060002A8 RID: 680 RVA: 0x00017AF8 File Offset: 0x00015CF8
	private void Event_Client_OnGotLaunchCommandLine(Dictionary<string, object> message)
	{
		string[] args = (string[])message["args"];
		string commandLineArgument = Utils.GetCommandLineArgument("+ipAddress", args);
		ushort num;
		ushort port = ushort.TryParse(Utils.GetCommandLineArgument("+port", args), out num) ? num : 7777;
		string commandLineArgument2 = Utils.GetCommandLineArgument("+password", args);
		if (string.IsNullOrEmpty(commandLineArgument))
		{
			return;
		}
		this.connectionManager.Client_StartClient(commandLineArgument, port, commandLineArgument2);
	}

	// Token: 0x060002A9 RID: 681 RVA: 0x00017AF8 File Offset: 0x00015CF8
	private void Event_Client_OnGameRichPresenceJoinRequested(Dictionary<string, object> message)
	{
		string[] args = (string[])message["args"];
		string commandLineArgument = Utils.GetCommandLineArgument("+ipAddress", args);
		ushort num;
		ushort port = ushort.TryParse(Utils.GetCommandLineArgument("+port", args), out num) ? num : 7777;
		string commandLineArgument2 = Utils.GetCommandLineArgument("+password", args);
		if (string.IsNullOrEmpty(commandLineArgument))
		{
			return;
		}
		this.connectionManager.Client_StartClient(commandLineArgument, port, commandLineArgument2);
	}

	// Token: 0x060002AA RID: 682 RVA: 0x00017B64 File Offset: 0x00015D64
	private void Event_Client_OnServerBrowserClickServer(Dictionary<string, object> message)
	{
		ServerBrowserServer serverBrowserServer = (ServerBrowserServer)message["serverBrowserServer"];
		this.connectionManager.Client_StartClient(serverBrowserServer.ipAddress, serverBrowserServer.port, "");
	}

	// Token: 0x060002AB RID: 683 RVA: 0x00017BA0 File Offset: 0x00015DA0
	private void Event_Client_OnPendingModsCleared(Dictionary<string, object> message)
	{
		Debug.Log(string.Format("[ConnectionManagerController] Pending mods cleared, reconnecting to last connection ({0}:{1})", this.connectionManager.LastConnection.IpAddress, this.connectionManager.LastConnection.Port));
		this.connectionManager.Client_StartClient(this.connectionManager.LastConnection.IpAddress, this.connectionManager.LastConnection.Port, this.connectionManager.LastConnection.Password);
	}

	// Token: 0x060002AC RID: 684 RVA: 0x00017C1C File Offset: 0x00015E1C
	private void Event_Client_OnPopupClickOk(Dictionary<string, object> message)
	{
		if ((string)message["name"] != "missingPassword")
		{
			return;
		}
		string password = ((PopupContentPassword)message["content"]).Password;
		this.connectionManager.Client_StartClient(this.connectionManager.LastConnection.IpAddress, this.connectionManager.LastConnection.Port, password);
	}

	// Token: 0x0400018E RID: 398
	private ConnectionManager connectionManager;

	// Token: 0x0400018F RID: 399
	private RuntimeNetStatsMonitor runtimeNetStatsMonitor;
}
