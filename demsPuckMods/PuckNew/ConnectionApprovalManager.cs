using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using Unity.Netcode;
using UnityEngine;

// Token: 0x02000114 RID: 276
[RequireComponent(typeof(ServerManager))]
[RequireComponent(typeof(TimeoutManager))]
[RequireComponent(typeof(BanManager))]
public class ConnectionApprovalManager : MonoBehaviourSingleton<ConnectionApprovalManager>
{
	// Token: 0x06000793 RID: 1939 RVA: 0x0002506C File Offset: 0x0002326C
	public override void Awake()
	{
		base.Awake();
		this.ServerManager = base.GetComponent<ServerManager>();
		this.TimeoutManager = base.GetComponent<TimeoutManager>();
		this.BanManager = base.GetComponent<BanManager>();
		this.WhitelistManager = base.GetComponent<WhitelistManager>();
	}

	// Token: 0x06000794 RID: 1940 RVA: 0x000250A4 File Offset: 0x000232A4
	private void Start()
	{
		this.bufferConnectionApprovals = true;
		NetworkManager.Singleton.ConnectionApprovalCallback = new Action<NetworkManager.ConnectionApprovalRequest, NetworkManager.ConnectionApprovalResponse>(this.ConnectionApprovalCallback);
	}

	// Token: 0x06000795 RID: 1941 RVA: 0x000250C3 File Offset: 0x000232C3
	public void Dispose()
	{
		this.clientIdConnectionApprovalMap.Clear();
		this.bufferConnectionApprovals = true;
		this.bufferedConnectionApprovals.Clear();
	}

	// Token: 0x06000796 RID: 1942 RVA: 0x000250E2 File Offset: 0x000232E2
	private void ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
	{
		if (this.bufferConnectionApprovals)
		{
			this.bufferedConnectionApprovals.Add(new ValueTuple<NetworkManager.ConnectionApprovalRequest, NetworkManager.ConnectionApprovalResponse>(request, response));
			return;
		}
		this.HandleConnectionApproval(request, response);
	}

	// Token: 0x06000797 RID: 1943 RVA: 0x00025108 File Offset: 0x00023308
	private void HandleConnectionApproval(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
	{
		ulong clientNetworkId = request.ClientNetworkId;
		ConnectionData connectionData = null;
		try
		{
			connectionData = JsonSerializer.Deserialize<ConnectionData>(Encoding.UTF8.GetString(request.Payload), null);
		}
		catch (Exception ex)
		{
			Debug.LogError(string.Format("[ConnectionApprovalManager] Error deserializing connection data for client {0}: {1}", clientNetworkId, ex.Message));
		}
		this.AddConnectionApproval(clientNetworkId, request, response, connectionData);
	}

	// Token: 0x06000798 RID: 1944 RVA: 0x00025170 File Offset: 0x00023370
	private void AddConnectionApproval(ulong clientId, NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response, ConnectionData connectionData)
	{
		if (this.clientIdConnectionApprovalMap.ContainsKey(clientId))
		{
			return;
		}
		ConnectionApproval connectionApproval = new ConnectionApproval
		{
			Request = request,
			Response = response,
			ConnectionData = connectionData,
			IpAddress = ((clientId == 0UL) ? "127.0.0.1" : this.ServerManager.UnityTransport.GetEndpoint(clientId).Address)
		};
		this.clientIdConnectionApprovalMap.Add(clientId, connectionApproval);
		this.OnConnectionApprovalStarted(clientId, connectionApproval);
	}

	// Token: 0x06000799 RID: 1945 RVA: 0x000251E8 File Offset: 0x000233E8
	public void ConsumeBufferedConnectionApprovals(bool stopBuffering = true)
	{
		Debug.Log(string.Format("[ConnectionApprovalManager] Consuming {0} buffered connection approvals", this.bufferedConnectionApprovals.Count));
		this.bufferConnectionApprovals = !stopBuffering;
		foreach (ValueTuple<NetworkManager.ConnectionApprovalRequest, NetworkManager.ConnectionApprovalResponse> valueTuple in this.bufferedConnectionApprovals.ToList<ValueTuple<NetworkManager.ConnectionApprovalRequest, NetworkManager.ConnectionApprovalResponse>>())
		{
			NetworkManager.ConnectionApprovalRequest item = valueTuple.Item1;
			NetworkManager.ConnectionApprovalResponse item2 = valueTuple.Item2;
			this.HandleConnectionApproval(item, item2);
			this.bufferedConnectionApprovals.Remove(new ValueTuple<NetworkManager.ConnectionApprovalRequest, NetworkManager.ConnectionApprovalResponse>(item, item2));
		}
	}

	// Token: 0x0600079A RID: 1946 RVA: 0x0002528C File Offset: 0x0002348C
	public void RemoveConnectionApproval(ulong clientId)
	{
		if (!this.clientIdConnectionApprovalMap.ContainsKey(clientId))
		{
			return;
		}
		this.clientIdConnectionApprovalMap.Remove(clientId);
	}

	// Token: 0x0600079B RID: 1947 RVA: 0x000252AA File Offset: 0x000234AA
	public ConnectionApproval GetConnectionApprovalByClientId(ulong clientId)
	{
		if (!this.clientIdConnectionApprovalMap.ContainsKey(clientId))
		{
			return null;
		}
		return this.clientIdConnectionApprovalMap[clientId];
	}

	// Token: 0x0600079C RID: 1948 RVA: 0x000252C8 File Offset: 0x000234C8
	public ConnectionApproval GetConnectionApprovalBySteamId(string steamId)
	{
		return this.clientIdConnectionApprovalMap.Values.FirstOrDefault((ConnectionApproval approval) => approval.ConnectionData.SteamId == steamId);
	}

	// Token: 0x0600079D RID: 1949 RVA: 0x00025300 File Offset: 0x00023500
	public void ApproveConnection(ulong clientId, PlayerData playerData)
	{
		if (!this.clientIdConnectionApprovalMap.ContainsKey(clientId))
		{
			return;
		}
		ConnectionApproval connectionApproval = this.clientIdConnectionApprovalMap[clientId];
		connectionApproval.Approve(playerData);
		Debug.Log(string.Format("[ConnectionApprovalManager] Approved connection for client {0}", clientId));
		EventManager.TriggerEvent("Event_Server_OnConnectionApproved", new Dictionary<string, object>
		{
			{
				"clientId",
				clientId
			},
			{
				"connectionApproval",
				connectionApproval
			}
		});
	}

	// Token: 0x0600079E RID: 1950 RVA: 0x00025371 File Offset: 0x00023571
	public string GetRejectionReason(ConnectionRejectionCode code, string message = null)
	{
		return JsonSerializer.Serialize<ConnectionRejection>(new ConnectionRejection
		{
			code = code,
			clientRequiredModIds = this.ServerManager.ClientRequiredModIds,
			message = message
		}, new JsonSerializerOptions
		{
			WriteIndented = true
		});
	}

	// Token: 0x0600079F RID: 1951 RVA: 0x000253A8 File Offset: 0x000235A8
	public void RejectConnection(ulong clientId, ConnectionRejectionCode code, string message = null)
	{
		if (!this.clientIdConnectionApprovalMap.ContainsKey(clientId))
		{
			return;
		}
		ConnectionApproval connectionApproval = this.clientIdConnectionApprovalMap[clientId];
		connectionApproval.Reject(this.GetRejectionReason(code, message));
		Debug.Log(string.Format("[ConnectionApprovalManager] Rejected connection for client {0} with reason: {1}", clientId, Utils.GetConnectionRejectionMessage(code, message)));
		EventManager.TriggerEvent("Event_Server_OnConnectionRejected", new Dictionary<string, object>
		{
			{
				"clientId",
				clientId
			},
			{
				"connectionApproval",
				connectionApproval
			},
			{
				"rejectionCode",
				code
			}
		});
		this.RemoveConnectionApproval(clientId);
	}

	// Token: 0x060007A0 RID: 1952 RVA: 0x00025440 File Offset: 0x00023640
	public ConnectionRejectionCode? GetConnectionRejectionCode(ConnectionApproval connectionApproval)
	{
		int num = NetworkManager.Singleton.ConnectedClientsList.Count((NetworkClient c) => c.ClientId != connectionApproval.ClientID);
		bool flag = !string.IsNullOrEmpty(this.ServerManager.ServerConfig.password);
		bool useWhitelist = this.ServerManager.ServerConfig.useWhitelist;
		bool flag2 = connectionApproval.ConnectionData == null;
		bool flag3 = num >= this.ServerManager.ServerConfig.maxPlayers;
		bool flag4 = this.TimeoutManager.IsSteamIdTimedOut(connectionApproval.ConnectionData.SteamId);
		bool flag5 = this.BanManager.IsSteamIdBanned(connectionApproval.ConnectionData.SteamId);
		bool flag6 = this.BanManager.IsIpAddressBanned(connectionApproval.IpAddress);
		bool flag7 = this.WhitelistManager.IsSteamIdWhitelisted(connectionApproval.ConnectionData.SteamId);
		bool flag8 = string.IsNullOrEmpty(connectionApproval.ConnectionData.Password) && flag;
		bool flag9 = connectionApproval.ConnectionData.Password == this.ServerManager.ServerConfig.password || !flag;
		bool flag10 = this.ServerManager.ClientRequiredModIds.Any((ulong modId) => !connectionApproval.ConnectionData.EnabledModIds.Contains(modId));
		if (flag2)
		{
			return new ConnectionRejectionCode?(ConnectionRejectionCode.Unknown);
		}
		if (flag3)
		{
			return new ConnectionRejectionCode?(ConnectionRejectionCode.ServerFull);
		}
		if (flag4)
		{
			return new ConnectionRejectionCode?(ConnectionRejectionCode.TimedOut);
		}
		if (flag5 || flag6)
		{
			return new ConnectionRejectionCode?(ConnectionRejectionCode.Banned);
		}
		if (useWhitelist && !flag7)
		{
			return new ConnectionRejectionCode?(ConnectionRejectionCode.NotWhitelisted);
		}
		if (flag8)
		{
			return new ConnectionRejectionCode?(ConnectionRejectionCode.MissingPassword);
		}
		if (!flag9)
		{
			return new ConnectionRejectionCode?(ConnectionRejectionCode.InvalidPassword);
		}
		if (flag10)
		{
			return new ConnectionRejectionCode?(ConnectionRejectionCode.MissingMods);
		}
		return null;
	}

	// Token: 0x060007A1 RID: 1953 RVA: 0x00025604 File Offset: 0x00023804
	private void OnConnectionApprovalStarted(ulong clientId, ConnectionApproval connectionApproval)
	{
		Debug.Log(string.Format("[ConnectionApprovalManager] Started connection approval for client {0}", clientId));
		ConnectionRejectionCode? connectionRejectionCode = this.GetConnectionRejectionCode(connectionApproval);
		if (connectionRejectionCode == null)
		{
			connectionApproval.Halt();
			WebSocketManager.Emit("serverConnectionApprovalRequest", new Dictionary<string, object>
			{
				{
					"steamId",
					connectionApproval.ConnectionData.SteamId
				},
				{
					"key",
					connectionApproval.ConnectionData.Key
				}
			}, "serverConnectionApprovalResponse");
			return;
		}
		this.RejectConnection(clientId, connectionRejectionCode.Value, null);
	}

	// Token: 0x0400048E RID: 1166
	[HideInInspector]
	public ServerManager ServerManager;

	// Token: 0x0400048F RID: 1167
	[HideInInspector]
	public TimeoutManager TimeoutManager;

	// Token: 0x04000490 RID: 1168
	[HideInInspector]
	public BanManager BanManager;

	// Token: 0x04000491 RID: 1169
	[HideInInspector]
	public WhitelistManager WhitelistManager;

	// Token: 0x04000492 RID: 1170
	private Dictionary<ulong, ConnectionApproval> clientIdConnectionApprovalMap = new Dictionary<ulong, ConnectionApproval>();

	// Token: 0x04000493 RID: 1171
	private bool bufferConnectionApprovals = true;

	// Token: 0x04000494 RID: 1172
	private List<ValueTuple<NetworkManager.ConnectionApprovalRequest, NetworkManager.ConnectionApprovalResponse>> bufferedConnectionApprovals = new List<ValueTuple<NetworkManager.ConnectionApprovalRequest, NetworkManager.ConnectionApprovalResponse>>();
}
