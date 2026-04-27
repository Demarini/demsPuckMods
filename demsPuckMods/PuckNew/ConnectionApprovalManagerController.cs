using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Token: 0x02000117 RID: 279
public class ConnectionApprovalManagerController : MonoBehaviour
{
	// Token: 0x060007A8 RID: 1960 RVA: 0x00025700 File Offset: 0x00023900
	private void Awake()
	{
		this.connectionApprovalManager = base.GetComponent<ConnectionApprovalManager>();
		EventManager.AddEventListener("Event_Everyone_OnClientConnected", new Action<Dictionary<string, object>>(this.Event_Everyone_OnClientConnected));
		EventManager.AddEventListener("Event_Everyone_OnClientDisconnected", new Action<Dictionary<string, object>>(this.Event_Everyone_OnClientDisconnected));
		EventManager.AddEventListener("Event_Server_OnServerStopped", new Action<Dictionary<string, object>>(this.Event_Server_OnServerStopped));
		EventManager.AddEventListener("Event_Server_OnLoadSceneEventCompleted", new Action<Dictionary<string, object>>(this.Event_Server_OnLoadSceneEventCompleted));
		EventManager.AddEventListener("Event_Server_OnConnectionApproved", new Action<Dictionary<string, object>>(this.Event_Server_OnConnectionApproved));
		WebSocketManager.AddMessageListener("serverConnectionApprovalResponse", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnServerConnectionApprovalResponse));
	}

	// Token: 0x060007A9 RID: 1961 RVA: 0x000020D3 File Offset: 0x000002D3
	private void Start()
	{
	}

	// Token: 0x060007AA RID: 1962 RVA: 0x000257A0 File Offset: 0x000239A0
	private void OnDestroy()
	{
		EventManager.RemoveEventListener("Event_Everyone_OnClientConnected", new Action<Dictionary<string, object>>(this.Event_Everyone_OnClientConnected));
		EventManager.RemoveEventListener("Event_Everyone_OnClientDisconnected", new Action<Dictionary<string, object>>(this.Event_Everyone_OnClientDisconnected));
		EventManager.RemoveEventListener("Event_Server_OnServerStopped", new Action<Dictionary<string, object>>(this.Event_Server_OnServerStopped));
		EventManager.RemoveEventListener("Event_Server_OnLoadSceneEventCompleted", new Action<Dictionary<string, object>>(this.Event_Server_OnLoadSceneEventCompleted));
		EventManager.RemoveEventListener("Event_Server_OnConnectionApproved", new Action<Dictionary<string, object>>(this.Event_Server_OnConnectionApproved));
		WebSocketManager.RemoveMessageListener("serverConnectionApprovalResponse", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnServerConnectionApprovalResponse));
	}

	// Token: 0x060007AB RID: 1963 RVA: 0x00025834 File Offset: 0x00023A34
	private void Event_Everyone_OnClientConnected(Dictionary<string, object> message)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		ulong num = (ulong)message["clientId"];
		ConnectionApproval connectionApprovalByClientId = this.connectionApprovalManager.GetConnectionApprovalByClientId(num);
		if (connectionApprovalByClientId == null || !connectionApprovalByClientId.IsApproved)
		{
			return;
		}
		if (connectionApprovalByClientId.IsHost)
		{
			return;
		}
		EventManager.TriggerEvent("Event_Server_OnApprovedClientConnected", new Dictionary<string, object>
		{
			{
				"clientId",
				num
			},
			{
				"connectionApproval",
				connectionApprovalByClientId
			}
		});
	}

	// Token: 0x060007AC RID: 1964 RVA: 0x000258B0 File Offset: 0x00023AB0
	private void Event_Everyone_OnClientDisconnected(Dictionary<string, object> message)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		ulong num = (ulong)message["clientId"];
		ConnectionApproval connectionApprovalByClientId = this.connectionApprovalManager.GetConnectionApprovalByClientId(num);
		if (connectionApprovalByClientId == null || !connectionApprovalByClientId.IsApproved)
		{
			return;
		}
		EventManager.TriggerEvent("Event_Server_OnApprovedClientDisconnected", new Dictionary<string, object>
		{
			{
				"clientId",
				num
			},
			{
				"connectionApproval",
				connectionApprovalByClientId
			}
		});
		this.connectionApprovalManager.RemoveConnectionApproval(num);
	}

	// Token: 0x060007AD RID: 1965 RVA: 0x0002592C File Offset: 0x00023B2C
	private void Event_Server_OnServerStopped(Dictionary<string, object> message)
	{
		this.connectionApprovalManager.Dispose();
	}

	// Token: 0x060007AE RID: 1966 RVA: 0x00025939 File Offset: 0x00023B39
	private void Event_Server_OnLoadSceneEventCompleted(Dictionary<string, object> message)
	{
		if ((bool)message["isInitialScene"])
		{
			this.connectionApprovalManager.ConsumeBufferedConnectionApprovals(true);
		}
	}

	// Token: 0x060007AF RID: 1967 RVA: 0x0002595C File Offset: 0x00023B5C
	private void Event_Server_OnConnectionApproved(Dictionary<string, object> message)
	{
		ulong num = (ulong)message["clientId"];
		ConnectionApproval connectionApproval = (ConnectionApproval)message["connectionApproval"];
		if (connectionApproval.IsHost)
		{
			EventManager.TriggerEvent("Event_Server_OnApprovedClientConnected", new Dictionary<string, object>
			{
				{
					"clientId",
					num
				},
				{
					"connectionApproval",
					connectionApproval
				}
			});
		}
	}

	// Token: 0x060007B0 RID: 1968 RVA: 0x000259C0 File Offset: 0x00023BC0
	private void WebSocket_Event_OnServerConnectionApprovalResponse(Dictionary<string, object> message)
	{
		OutMessage outMessage = (OutMessage)message["outMessage"];
		ServerConnectionApprovalResponse data = ((InMessage)message["inMessage"]).GetData<ServerConnectionApprovalResponse>();
		string steamId = (string)outMessage.Data["steamId"];
		ConnectionApproval connectionApprovalBySteamId = this.connectionApprovalManager.GetConnectionApprovalBySteamId(steamId);
		if (connectionApprovalBySteamId == null)
		{
			return;
		}
		ulong clientID = connectionApprovalBySteamId.ClientID;
		if (!data.success)
		{
			this.connectionApprovalManager.RejectConnection(clientID, ConnectionRejectionCode.Unknown, data.errorData.message);
			return;
		}
		ConnectionRejectionCode? connectionRejectionCode = this.connectionApprovalManager.GetConnectionRejectionCode(connectionApprovalBySteamId);
		bool flag = connectionRejectionCode == null;
		if (flag && BackendUtils.GetActivePlayerDataBan(data.data.playerData) != null)
		{
			connectionRejectionCode = new ConnectionRejectionCode?(ConnectionRejectionCode.Banned);
			flag = false;
		}
		if (flag)
		{
			this.connectionApprovalManager.ApproveConnection(clientID, data.data.playerData);
			return;
		}
		this.connectionApprovalManager.RejectConnection(clientID, connectionRejectionCode.Value, null);
	}

	// Token: 0x04000497 RID: 1175
	private ConnectionApprovalManager connectionApprovalManager;
}
