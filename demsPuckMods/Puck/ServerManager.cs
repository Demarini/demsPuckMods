using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

// Token: 0x0200009D RID: 157
public class ServerManager : NetworkBehaviourSingleton<ServerManager>
{
	// Token: 0x06000411 RID: 1041 RVA: 0x0001B84C File Offset: 0x00019A4C
	public override void Awake()
	{
		base.Awake();
		this.EdgegapManager = base.GetComponent<EdgegapManager>();
		this.ServerConfigurationManager = base.GetComponent<ServerConfigurationManager>();
		this.UdpSocket = new UDPSocket();
		UDPSocket udpSocket = this.UdpSocket;
		udpSocket.OnSocketStarted = (Action<ushort>)Delegate.Combine(udpSocket.OnSocketStarted, new Action<ushort>(delegate(ushort port)
		{
			Debug.Log(string.Format("[ServerManager] UDP socket started on port {0}", port));
		}));
		UDPSocket udpSocket2 = this.UdpSocket;
		udpSocket2.OnSocketFailed = (Action<ushort>)Delegate.Combine(udpSocket2.OnSocketFailed, new Action<ushort>(delegate(ushort port)
		{
			Debug.Log(string.Format("[ServerManager] UDP socket failed on port {0}", port));
		}));
		UDPSocket udpSocket3 = this.UdpSocket;
		udpSocket3.OnSocketStopped = (Action)Delegate.Combine(udpSocket3.OnSocketStopped, new Action(delegate()
		{
			Debug.Log("[ServerManager] UDP socket stopped");
		}));
		UDPSocket udpSocket4 = this.UdpSocket;
		udpSocket4.OnUdpMessageReceived = (Action<string, ushort, string, long>)Delegate.Combine(udpSocket4.OnUdpMessageReceived, new Action<string, ushort, string, long>(delegate(string ipAddress, ushort port, string message, long timestamp)
		{
			if (message == "ping")
			{
				this.UdpSocket.Send(ipAddress, port, "pong");
			}
		}));
		uPnPHelper.DebugMode = true;
		uPnPHelper.LogErrors = true;
	}

	// Token: 0x06000412 RID: 1042 RVA: 0x0000980E File Offset: 0x00007A0E
	private void Start()
	{
		this.UnityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
		NetworkManager.Singleton.ConnectionApprovalCallback = new Action<NetworkManager.ConnectionApprovalRequest, NetworkManager.ConnectionApprovalResponse>(this.Server_ConnectionApproval);
	}

	// Token: 0x06000413 RID: 1043 RVA: 0x00009836 File Offset: 0x00007A36
	private void Update()
	{
		if (!base.IsSpawned)
		{
			return;
		}
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.deltaTimeBuffer.Add(Time.deltaTime);
	}

	// Token: 0x06000414 RID: 1044 RVA: 0x0000985E File Offset: 0x00007A5E
	public void LoadAdminSteamIds()
	{
		this.AdminSteamIds = this.ServerConfigurationManager.ServerConfiguration.adminSteamIds;
	}

	// Token: 0x06000415 RID: 1045 RVA: 0x0001B964 File Offset: 0x00019B64
	public void LoadBannedSteamIds()
	{
		string path = "./banned_steam_ids.json";
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		for (int i = 0; i < commandLineArgs.Length; i++)
		{
			if (commandLineArgs[i] == "--bannedSteamIdsPath")
			{
				path = commandLineArgs[i + 1];
			}
		}
		string text = Uri.UnescapeDataString(new Uri(Path.GetFullPath(path)).AbsolutePath);
		Debug.Log("[ServerManager] Reading banned Steam IDs file from " + text + "...");
		if (!File.Exists(text))
		{
			Debug.Log("[ServerManager] Banned Steam IDs file not found at " + text + ", creating...");
			File.AppendAllText(text, JsonSerializer.Serialize<string[]>(this.BannedSteamIds, new JsonSerializerOptions
			{
				WriteIndented = true
			}));
		}
		string json = File.ReadAllText(text);
		Debug.Log("[ServerManager] Parsing banned Steam IDs...");
		this.BannedSteamIds = JsonSerializer.Deserialize<string[]>(json, null);
		Debug.Log(string.Format("[ServerManager] Loaded {0} banned Steam IDs", this.BannedSteamIds.Length));
	}

	// Token: 0x06000416 RID: 1046 RVA: 0x0001BA48 File Offset: 0x00019C48
	private void AddBannedSteamId(string steamId)
	{
		this.LoadBannedSteamIds();
		if (this.BannedSteamIds.Contains(steamId))
		{
			return;
		}
		this.BannedSteamIds = this.BannedSteamIds.Append(steamId).ToArray<string>();
		string text = Uri.UnescapeDataString(new Uri(Path.GetFullPath(".") + "/banned_steam_ids.json").AbsolutePath);
		Debug.Log("[ServerManager] Writing banned Steam IDs to " + text + "...");
		File.WriteAllText(text, JsonSerializer.Serialize<string[]>(this.BannedSteamIds, new JsonSerializerOptions
		{
			WriteIndented = true
		}));
	}

	// Token: 0x06000417 RID: 1047 RVA: 0x0001BAD8 File Offset: 0x00019CD8
	private void RemoveBannedSteamId(string steamId)
	{
		this.LoadBannedSteamIds();
		if (!this.BannedSteamIds.Contains(steamId))
		{
			return;
		}
		this.BannedSteamIds = (from id in this.BannedSteamIds
		where id != steamId
		select id).ToArray<string>();
		string text = Uri.UnescapeDataString(new Uri(Path.GetFullPath(".") + "/banned_steam_ids.json").AbsolutePath);
		Debug.Log("[ServerManager] Writing banned Steam IDs to " + text + "...");
		File.WriteAllText(text, JsonSerializer.Serialize<string[]>(this.BannedSteamIds, new JsonSerializerOptions
		{
			WriteIndented = true
		}));
	}

	// Token: 0x06000418 RID: 1048 RVA: 0x0001BB84 File Offset: 0x00019D84
	public void Server_Authenticate(string[] connectedSteamIds)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		string environmentVariable = Environment.GetEnvironmentVariable("PUCK_SERVER_TOKEN");
		string environmentVariable2 = Environment.GetEnvironmentVariable("PUCK_SERVER_LAUNCHED_BY_STEAM_ID");
		MonoBehaviourSingleton<WebSocketManager>.Instance.Emit("serverAuthenticateRequest", new Dictionary<string, object>
		{
			{
				"port",
				this.Server.Port
			},
			{
				"pingPort",
				this.Server.PingPort
			},
			{
				"name",
				this.Server.Name.ToString()
			},
			{
				"maxPlayers",
				this.Server.MaxPlayers
			},
			{
				"password",
				this.Server.Password.ToString()
			},
			{
				"isPublic",
				this.Server.IsPublic
			},
			{
				"isDedicated",
				this.Server.IsDedicated
			},
			{
				"isHosted",
				this.Server.IsHosted
			},
			{
				"ownerSteamId",
				this.Server.OwnerSteamId.ToString()
			},
			{
				"token",
				environmentVariable
			},
			{
				"requestId",
				this.EdgegapManager.RequestId
			},
			{
				"launchedBySteamId",
				environmentVariable2
			},
			{
				"connectedSteamIds",
				connectedSteamIds
			}
		}, "serverAuthenticateResponse");
	}

	// Token: 0x06000419 RID: 1049 RVA: 0x00009876 File Offset: 0x00007A76
	public void Server_Unauthenticate()
	{
		MonoBehaviourSingleton<WebSocketManager>.Instance.Emit("serverUnauthenticate", null, null);
	}

	// Token: 0x0600041A RID: 1050 RVA: 0x00009889 File Offset: 0x00007A89
	public void Server_UpdateConnectedSteamIds(string[] steamIds)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		MonoBehaviourSingleton<WebSocketManager>.Instance.Emit("serverUpdateConnectedSteamIds", new Dictionary<string, object>
		{
			{
				"connectedSteamIds",
				steamIds
			}
		}, null);
	}

	// Token: 0x0600041B RID: 1051 RVA: 0x0001BD0C File Offset: 0x00019F0C
	private void Server_ConnectionApproval(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
	{
		ulong clientNetworkId = request.ClientNetworkId;
		if (clientNetworkId == 0UL)
		{
			response.Approved = true;
			Debug.Log(string.Format("[ServerManager] Host connection approved for {0}", clientNetworkId));
		}
		else
		{
			ConnectionData connectionData = JsonSerializer.Deserialize<ConnectionData>(Encoding.ASCII.GetString(request.Payload), null);
			Debug.Log(string.Format("[ServerManager] Connection approval incoming from {0} ({1})", clientNetworkId, connectionData.SteamId));
			string text = this.Server.Password.ToString();
			bool flag = !string.IsNullOrEmpty(text);
			this.Server_VerifyTimeouts();
			bool flag2 = !string.IsNullOrEmpty(connectionData.SocketId);
			bool flag3 = !string.IsNullOrEmpty(connectionData.SteamId);
			bool flag4 = NetworkManager.Singleton.ConnectedClientsList.Count >= this.Server.MaxPlayers;
			bool flag5 = flag3 && this.SteamIdTimeouts.ContainsKey(connectionData.SteamId);
			bool flag6 = flag3 && this.BannedSteamIds.Contains(connectionData.SteamId);
			bool flag7 = string.IsNullOrEmpty(connectionData.Password) && flag;
			bool flag8 = connectionData.Password == text || !flag;
			bool flag9 = this.ServerConfigurationManager.ClientRequiredModIds.Any((ulong modId) => !connectionData.EnabledModIds.Contains(modId));
			response.Approved = (flag2 && flag3 && !flag4 && !flag5 && !flag6 && !flag7 && flag8 && !flag9);
			if (response.Approved)
			{
				if (this.ServerConfigurationManager.ServerConfiguration.usePuckBannedSteamIds)
				{
					response.Pending = true;
					if (this.ConnectionApprovalRequests.ContainsKey(connectionData.SteamId))
					{
						this.ConnectionApprovalRequests.Remove(connectionData.SteamId);
					}
					this.ConnectionApprovalRequests.Add(connectionData.SteamId, response);
					MonoBehaviourSingleton<WebSocketManager>.Instance.Emit("serverConnectionApprovalRequest", new Dictionary<string, object>
					{
						{
							"steamId",
							connectionData.SteamId
						},
						{
							"socketId",
							connectionData.SocketId
						}
					}, "serverConnectionApprovalResponse");
					Debug.Log(string.Format("[ServerManager] Connection approval request sent for {0} ({1})", clientNetworkId, connectionData.SteamId));
				}
				else
				{
					Debug.Log(string.Format("[ServerManager] Connection approved for {0} ({1})", clientNetworkId, connectionData.SteamId));
				}
			}
			else
			{
				ConnectionRejectionCode code = ConnectionRejectionCode.Unreachable;
				ulong[] clientRequiredModIds = this.ServerConfigurationManager.ClientRequiredModIds;
				if (!flag2)
				{
					code = ConnectionRejectionCode.InvalidSocketId;
				}
				else if (!flag3)
				{
					code = ConnectionRejectionCode.InvalidSteamId;
				}
				else if (flag4)
				{
					code = ConnectionRejectionCode.ServerFull;
				}
				else if (flag5)
				{
					code = ConnectionRejectionCode.TimedOut;
				}
				else if (flag6)
				{
					code = ConnectionRejectionCode.Banned;
				}
				else if (flag7)
				{
					code = ConnectionRejectionCode.MissingPassword;
				}
				else if (!flag8)
				{
					code = ConnectionRejectionCode.InvalidPassword;
				}
				else if (flag9)
				{
					code = ConnectionRejectionCode.MissingMods;
				}
				string reason = JsonSerializer.Serialize<ConnectionRejection>(new ConnectionRejection
				{
					code = code,
					clientRequiredModIds = clientRequiredModIds
				}, new JsonSerializerOptions
				{
					WriteIndented = true
				});
				response.Reason = reason;
				Debug.Log(string.Format("[ServerManager] Connection rejected for {0} ({1}): {2}", clientNetworkId, connectionData.SteamId, Utils.GetConnectionRejectionMessage(code)));
			}
		}
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Server_ConnectionApproval", new Dictionary<string, object>
		{
			{
				"clientId",
				clientNetworkId
			},
			{
				"approved",
				response.Approved
			}
		});
	}

	// Token: 0x0600041C RID: 1052 RVA: 0x000098B9 File Offset: 0x00007AB9
	public void Server_StartMetricsCoroutine()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.Server_StopMetricsCoroutine();
		this.serverMetricsCoroutine = this.IMetrics(10f);
		base.StartCoroutine(this.serverMetricsCoroutine);
	}

	// Token: 0x0600041D RID: 1053 RVA: 0x000098EC File Offset: 0x00007AEC
	public void Server_StopMetricsCoroutine()
	{
		if (this.serverMetricsCoroutine != null)
		{
			base.StopCoroutine(this.serverMetricsCoroutine);
		}
	}

	// Token: 0x0600041E RID: 1054 RVA: 0x00009902 File Offset: 0x00007B02
	private IEnumerator IMetrics(float delay)
	{
		yield return new WaitForSeconds(delay);
		if (this.deltaTimeBuffer.Count <= 0)
		{
			this.deltaTimeBuffer.Clear();
			yield return null;
		}
		Debug.Log(string.Format("[ServerManager] FPS: {0} (min: {1}, average: {2}, max: {3})", new object[]
		{
			1f / Time.deltaTime,
			1f / this.deltaTimeBuffer.Max(),
			1f / this.deltaTimeBuffer.Average(),
			1f / this.deltaTimeBuffer.Min()
		}));
		this.deltaTimeBuffer.Clear();
		this.Server_StartMetricsCoroutine();
		yield break;
	}

	// Token: 0x0600041F RID: 1055 RVA: 0x00009918 File Offset: 0x00007B18
	public void Server_StartBannedSteamIdsReloadCoroutine()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.Server_StopBannedSteamIdsReloadCoroutine();
		this.bannedSteamIdsReloadCoroutine = this.IBannedSteamIdsReload(300f);
		base.StartCoroutine(this.bannedSteamIdsReloadCoroutine);
	}

	// Token: 0x06000420 RID: 1056 RVA: 0x0000994B File Offset: 0x00007B4B
	public void Server_StopBannedSteamIdsReloadCoroutine()
	{
		if (this.bannedSteamIdsReloadCoroutine != null)
		{
			base.StopCoroutine(this.bannedSteamIdsReloadCoroutine);
		}
	}

	// Token: 0x06000421 RID: 1057 RVA: 0x00009961 File Offset: 0x00007B61
	private IEnumerator IBannedSteamIdsReload(float delay)
	{
		yield return new WaitForSeconds(delay);
		this.LoadBannedSteamIds();
		this.Server_StartBannedSteamIdsReloadCoroutine();
		yield break;
	}

	// Token: 0x06000422 RID: 1058 RVA: 0x0001C098 File Offset: 0x0001A298
	[Rpc(SendTo.SpecifiedInParams)]
	public void Server_ServerConfigurationRpc(Server server, RpcParams rpcParams = default(RpcParams))
	{
		NetworkManager networkManager = base.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			uint rpcMethodId = 3940886306U;
			RpcAttribute.RpcAttributeParams attributeParams = default(RpcAttribute.RpcAttributeParams);
			FastBufferWriter fastBufferWriter = base.__beginSendRpc(rpcMethodId, rpcParams, attributeParams, SendTo.SpecifiedInParams, RpcDelivery.Reliable);
			fastBufferWriter.WriteValueSafe<Server>(server, default(FastBufferWriter.ForNetworkSerializable));
			base.__endSendRpc(ref fastBufferWriter, 3940886306U, rpcParams, attributeParams, SendTo.SpecifiedInParams, RpcDelivery.Reliable);
		}
		if (this.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Execute)
		{
			return;
		}
		this.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
		this.Server = server;
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnServerConfiguration", new Dictionary<string, object>
		{
			{
				"server",
				server
			}
		});
	}

	// Token: 0x06000423 RID: 1059 RVA: 0x00009977 File Offset: 0x00007B77
	public void Server_TimeoutSteamId(string steamId)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (this.SteamIdTimeouts.ContainsKey(steamId))
		{
			this.SteamIdTimeouts.Remove(steamId);
		}
		this.SteamIdTimeouts.Add(steamId, Time.time);
	}

	// Token: 0x06000424 RID: 1060 RVA: 0x0001C198 File Offset: 0x0001A398
	public void Server_VerifyTimeouts()
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		foreach (string key in this.SteamIdTimeouts.Keys.ToList<string>())
		{
			if (Time.time - this.SteamIdTimeouts[key] > this.ServerConfigurationManager.ServerConfiguration.kickTimeout)
			{
				this.SteamIdTimeouts.Remove(key);
			}
		}
	}

	// Token: 0x06000425 RID: 1061 RVA: 0x0001C22C File Offset: 0x0001A42C
	public void Server_KickPlayer(Player player, DisconnectionCode disconnectionCode = DisconnectionCode.Kicked, bool applyTimeout = true)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		string steamId = player.SteamId.Value.ToString();
		if (player.OwnerClientId == 0UL)
		{
			NetworkManager.Singleton.Shutdown(true);
			return;
		}
		if (applyTimeout)
		{
			this.Server_TimeoutSteamId(steamId);
		}
		string reason = JsonSerializer.Serialize<Disconnection>(new Disconnection
		{
			code = disconnectionCode
		}, null);
		NetworkManager.Singleton.DisconnectClient(player.OwnerClientId, reason);
	}

	// Token: 0x06000426 RID: 1062 RVA: 0x0001C2A4 File Offset: 0x0001A4A4
	public void Server_BanPlayer(Player player)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		string steamId = player.SteamId.Value.ToString();
		this.Server_BanSteamId(steamId);
		this.Server_KickPlayer(player, DisconnectionCode.Banned, false);
	}

	// Token: 0x06000427 RID: 1063 RVA: 0x000099B2 File Offset: 0x00007BB2
	public void Server_BanSteamId(string steamId)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.AddBannedSteamId(steamId);
	}

	// Token: 0x06000428 RID: 1064 RVA: 0x000099C8 File Offset: 0x00007BC8
	public void Server_UnbanSteamId(string steamId)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.RemoveBannedSteamId(steamId);
	}

	// Token: 0x06000429 RID: 1065 RVA: 0x0001C2E8 File Offset: 0x0001A4E8
	public void Client_StartHost(ushort port, string name, int maxPlayers, string password, bool voip, bool isPublic, string ownerSteamId, bool uPnP = false)
	{
		if (NetworkManager.Singleton.IsClient)
		{
			return;
		}
		ServerConfiguration serverConfiguration = new ServerConfiguration
		{
			port = port,
			pingPort = port + 1,
			name = name,
			maxPlayers = maxPlayers,
			password = password,
			voip = voip,
			isPublic = isPublic,
			adminSteamIds = new string[]
			{
				ownerSteamId
			},
			serverTickRate = 240,
			clientTickRate = 240
		};
		this.ServerConfigurationManager.ServerConfiguration = serverConfiguration;
		Debug.Log(string.Concat(new string[]
		{
			"[ServerManager] Starting ",
			isPublic ? "public" : "private",
			" Puck host (",
			Application.version,
			")"
		}));
		this.UnityTransport.SetConnectionData("0.0.0.0", this.ServerConfigurationManager.ServerConfiguration.port, null);
		this.Server = new Server
		{
			Port = this.ServerConfigurationManager.ServerConfiguration.port,
			PingPort = this.ServerConfigurationManager.ServerConfiguration.pingPort,
			Name = this.ServerConfigurationManager.ServerConfiguration.name,
			MaxPlayers = this.ServerConfigurationManager.ServerConfiguration.maxPlayers,
			Password = this.ServerConfigurationManager.ServerConfiguration.password,
			Voip = this.ServerConfigurationManager.ServerConfiguration.voip,
			IsPublic = this.ServerConfigurationManager.ServerConfiguration.isPublic,
			IsDedicated = false,
			IsHosted = true,
			OwnerSteamId = ownerSteamId,
			SleepTimeout = this.ServerConfigurationManager.ServerConfiguration.sleepTimeout,
			ClientTickRate = this.ServerConfigurationManager.ServerConfiguration.clientTickRate,
			ClientRequiredModIds = this.ServerConfigurationManager.ClientRequiredModIds
		};
		if (uPnP)
		{
			Debug.Log(string.Format("[ServerManager] Forwarding port {0} & {1} with uPnP", this.Server.Port, this.Server.PingPort));
			uPnPHelper.Start(uPnPHelper.Protocol.UDP, (int)this.Server.Port, 0, "Puck Port");
			Debug.Log(uPnPHelper.GetDebugMessages());
			Debug.Log(uPnPHelper.GetErrorMessages());
			uPnPHelper.Start(uPnPHelper.Protocol.UDP, (int)this.Server.PingPort, 0, "Puck Ping Port");
			Debug.Log(uPnPHelper.GetDebugMessages());
			Debug.Log(uPnPHelper.GetErrorMessages());
		}
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Server_OnBeforeServerStarted", new Dictionary<string, object>
		{
			{
				"serverConfiguration",
				this.ServerConfigurationManager.ServerConfiguration
			}
		});
		this.IsHostStartInProgress = true;
	}

	// Token: 0x0600042A RID: 1066 RVA: 0x0001C5B0 File Offset: 0x0001A7B0
	public void Client_StartServer(ushort port, bool uPnP = false)
	{
		if (NetworkManager.Singleton.IsServer)
		{
			return;
		}
		Debug.Log("[ServerManager] Starting Puck server (" + Application.version + ")");
		this.UnityTransport.SetConnectionData("0.0.0.0", port, null);
		this.Server = new Server
		{
			Port = (this.EdgegapManager.IsEdgegap ? this.EdgegapManager.ArbitriumPortGamePortExternal : this.ServerConfigurationManager.ServerConfiguration.port),
			PingPort = (this.EdgegapManager.IsEdgegap ? this.EdgegapManager.ArbitriumPortPingPortExternal : this.ServerConfigurationManager.ServerConfiguration.pingPort),
			Name = this.ServerConfigurationManager.ServerConfiguration.name,
			MaxPlayers = this.ServerConfigurationManager.ServerConfiguration.maxPlayers,
			Password = this.ServerConfigurationManager.ServerConfiguration.password,
			Voip = this.ServerConfigurationManager.ServerConfiguration.voip,
			IsPublic = this.ServerConfigurationManager.ServerConfiguration.isPublic,
			IsDedicated = true,
			IsHosted = false,
			SleepTimeout = this.ServerConfigurationManager.ServerConfiguration.sleepTimeout,
			ClientTickRate = this.ServerConfigurationManager.ServerConfiguration.clientTickRate,
			ClientRequiredModIds = this.ServerConfigurationManager.ClientRequiredModIds
		};
		if (uPnP)
		{
			Debug.Log(string.Format("[ServerManager] Forwarding port {0} & {1} with uPnP", this.Server.Port, this.Server.PingPort));
			uPnPHelper.Start(uPnPHelper.Protocol.UDP, (int)this.Server.Port, 0, "Puck Port");
			Debug.Log(uPnPHelper.GetDebugMessages());
			Debug.Log(uPnPHelper.GetErrorMessages());
			uPnPHelper.Start(uPnPHelper.Protocol.UDP, (int)this.Server.PingPort, 0, "Puck Ping Port");
			Debug.Log(uPnPHelper.GetDebugMessages());
			Debug.Log(uPnPHelper.GetErrorMessages());
		}
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Server_OnBeforeServerStarted", new Dictionary<string, object>
		{
			{
				"serverConfiguration",
				this.ServerConfigurationManager.ServerConfiguration
			}
		});
		this.IsServerStartInProgress = true;
	}

	// Token: 0x0600042B RID: 1067 RVA: 0x000099DE File Offset: 0x00007BDE
	private void OnApplicationQuit()
	{
		uPnPHelper.CloseAll();
	}

	// Token: 0x0600042E RID: 1070 RVA: 0x0001C83C File Offset: 0x0001AA3C
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x0600042F RID: 1071 RVA: 0x00009A06 File Offset: 0x00007C06
	protected override void __initializeRpcs()
	{
		base.__registerRpc(3940886306U, new NetworkBehaviour.RpcReceiveHandler(ServerManager.__rpc_handler_3940886306), "Server_ServerConfigurationRpc");
		base.__initializeRpcs();
	}

	// Token: 0x06000430 RID: 1072 RVA: 0x0001C854 File Offset: 0x0001AA54
	private static void __rpc_handler_3940886306(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		Server server;
		reader.ReadValueSafe<Server>(out server, default(FastBufferWriter.ForNetworkSerializable));
		RpcParams ext = rpcParams.Ext;
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Execute;
		((ServerManager)target).Server_ServerConfigurationRpc(server, ext);
		target.__rpc_exec_stage = NetworkBehaviour.__RpcExecStage.Send;
	}

	// Token: 0x06000431 RID: 1073 RVA: 0x00009A2C File Offset: 0x00007C2C
	protected internal override string __getTypeName()
	{
		return "ServerManager";
	}

	// Token: 0x0400025C RID: 604
	private const int SERVER_METRICS_INTERVAL = 10;

	// Token: 0x0400025D RID: 605
	[HideInInspector]
	public UnityTransport UnityTransport;

	// Token: 0x0400025E RID: 606
	[HideInInspector]
	public EdgegapManager EdgegapManager;

	// Token: 0x0400025F RID: 607
	[HideInInspector]
	public ServerConfigurationManager ServerConfigurationManager;

	// Token: 0x04000260 RID: 608
	[HideInInspector]
	public Server Server;

	// Token: 0x04000261 RID: 609
	[HideInInspector]
	public bool IsHostStartInProgress;

	// Token: 0x04000262 RID: 610
	[HideInInspector]
	public bool IsServerStartInProgress;

	// Token: 0x04000263 RID: 611
	[HideInInspector]
	public string[] AdminSteamIds = new string[0];

	// Token: 0x04000264 RID: 612
	[HideInInspector]
	public string[] BannedSteamIds = new string[0];

	// Token: 0x04000265 RID: 613
	[HideInInspector]
	public Dictionary<string, float> SteamIdTimeouts = new Dictionary<string, float>();

	// Token: 0x04000266 RID: 614
	[HideInInspector]
	public Dictionary<string, NetworkManager.ConnectionApprovalResponse> ConnectionApprovalRequests = new Dictionary<string, NetworkManager.ConnectionApprovalResponse>();

	// Token: 0x04000267 RID: 615
	[HideInInspector]
	public UDPSocket UdpSocket;

	// Token: 0x04000268 RID: 616
	private List<float> deltaTimeBuffer = new List<float>();

	// Token: 0x04000269 RID: 617
	private IEnumerator serverMetricsCoroutine;

	// Token: 0x0400026A RID: 618
	private IEnumerator bannedSteamIdsReloadCoroutine;
}
