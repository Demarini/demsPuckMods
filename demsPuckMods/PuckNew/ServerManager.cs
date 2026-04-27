using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

// Token: 0x02000120 RID: 288
[RequireComponent(typeof(EdgegapManager))]
[RequireComponent(typeof(ConnectionApprovalManager))]
[RequireComponent(typeof(TimeoutManager))]
[RequireComponent(typeof(BanManager))]
[RequireComponent(typeof(AdminManager))]
public class ServerManager : NetworkBehaviourSingleton<ServerManager>
{
	// Token: 0x170000E9 RID: 233
	// (get) Token: 0x060007F3 RID: 2035 RVA: 0x000263EC File Offset: 0x000245EC
	[HideInInspector]
	public ulong[] ClientRequiredModIds
	{
		get
		{
			return (from mod in this.ServerConfig.mods
			where mod.clientRequired
			select mod.id).ToArray<ulong>();
		}
	}

	// Token: 0x170000EA RID: 234
	// (get) Token: 0x060007F4 RID: 2036 RVA: 0x00026454 File Offset: 0x00024654
	[HideInInspector]
	public ulong[] EnabledModIds
	{
		get
		{
			return (from mod in this.ServerConfig.mods
			where mod.enabled
			select mod.id).ToArray<ulong>();
		}
	}

	// Token: 0x060007F5 RID: 2037 RVA: 0x000264BC File Offset: 0x000246BC
	public override void Awake()
	{
		base.Awake();
		if (ApplicationManager.IsDedicatedGameServer)
		{
			this.LoadConfig("./server_config.json", "--serverConfigPath", "--serverConfig", "PUCK_SERVER_CONFIG");
		}
		this.EdgegapManager = base.GetComponent<EdgegapManager>();
		this.ConnectionApprovalManager = base.GetComponent<ConnectionApprovalManager>();
		this.TimeoutManager = base.GetComponent<TimeoutManager>();
		this.BanManager = base.GetComponent<BanManager>();
		this.AdminManager = base.GetComponent<AdminManager>();
		this.WhitelistManager = base.GetComponent<WhitelistManager>();
		uPnPHelper.DebugMode = true;
		uPnPHelper.LogErrors = true;
	}

	// Token: 0x060007F6 RID: 2038 RVA: 0x00026544 File Offset: 0x00024744
	private void Start()
	{
		this.UnityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
		NetworkManager.Singleton.OnServerStarted += this.Server_OnServerStarted;
		NetworkManager.Singleton.OnServerStopped += this.Server_OnServerStopped;
		NetworkManager.Singleton.OnClientConnectedCallback += this.OnClientConnected;
		NetworkManager.Singleton.OnClientDisconnectCallback += this.OnClientDisconnected;
		NetworkManager.Singleton.OnTransportFailure += this.OnTransportFailure;
	}

	// Token: 0x060007F7 RID: 2039 RVA: 0x000265D0 File Offset: 0x000247D0
	protected override void OnNetworkPreSpawn(ref NetworkManager networkManager)
	{
		if (this.Server == null)
		{
			this.Server = new NetworkVariable<Server>(default(Server), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
		}
		if (networkManager.IsServer)
		{
			this.Server.Value = default(Server);
		}
		base.OnNetworkPreSpawn(ref networkManager);
	}

	// Token: 0x060007F8 RID: 2040 RVA: 0x0002661F File Offset: 0x0002481F
	public override void OnNetworkSpawn()
	{
		NetworkVariable<Server> server = this.Server;
		server.OnValueChanged = (NetworkVariable<Server>.OnValueChangedDelegate)Delegate.Combine(server.OnValueChanged, new NetworkVariable<Server>.OnValueChangedDelegate(this.OnServerChanged));
		base.OnNetworkSpawn();
	}

	// Token: 0x060007F9 RID: 2041 RVA: 0x0002664E File Offset: 0x0002484E
	protected override void OnNetworkPostSpawn()
	{
		if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsConnectedClient)
		{
			this.ProcessInitialNetworkVariableValues();
		}
		base.OnNetworkPostSpawn();
	}

	// Token: 0x060007FA RID: 2042 RVA: 0x00026674 File Offset: 0x00024874
	protected override void OnNetworkSessionSynchronized()
	{
		this.ProcessInitialNetworkVariableValues();
		base.OnNetworkSessionSynchronized();
	}

	// Token: 0x060007FB RID: 2043 RVA: 0x00026682 File Offset: 0x00024882
	public override void OnNetworkDespawn()
	{
		NetworkVariable<Server> server = this.Server;
		server.OnValueChanged = (NetworkVariable<Server>.OnValueChangedDelegate)Delegate.Remove(server.OnValueChanged, new NetworkVariable<Server>.OnValueChangedDelegate(this.OnServerChanged));
		base.OnNetworkDespawn();
	}

	// Token: 0x060007FC RID: 2044 RVA: 0x000266B4 File Offset: 0x000248B4
	public override void OnDestroy()
	{
		if (NetworkManager.Singleton != null)
		{
			NetworkManager.Singleton.OnServerStarted -= this.Server_OnServerStarted;
			NetworkManager.Singleton.OnServerStopped -= this.Server_OnServerStopped;
			NetworkManager.Singleton.OnClientConnectedCallback -= this.OnClientConnected;
			NetworkManager.Singleton.OnClientDisconnectCallback -= this.OnClientDisconnected;
			NetworkManager.Singleton.OnTransportFailure -= this.OnTransportFailure;
		}
		uPnPHelper.CloseAll();
		Utils.PrintUPnPLogs();
	}

	// Token: 0x060007FD RID: 2045 RVA: 0x00026748 File Offset: 0x00024948
	private void ProcessInitialNetworkVariableValues()
	{
		this.OnServerChanged(default(Server), this.Server.Value);
	}

	// Token: 0x060007FE RID: 2046 RVA: 0x00026770 File Offset: 0x00024970
	private void LoadConfig(string defaultFilePath, string filePathCliArgument = null, string cliArgument = null, string envVariable = null)
	{
		string environmentVariable = Environment.GetEnvironmentVariable(envVariable);
		string commandLineArgument = Utils.GetCommandLineArgument(cliArgument, null);
		if (!string.IsNullOrEmpty(environmentVariable))
		{
			Debug.Log("[ServerManager] Deserializing server config from environment variable (" + envVariable + ")");
			this.ServerConfig = ConfigUtils.LoadConfigFromSerializedString<ServerConfig>(environmentVariable);
			return;
		}
		if (!string.IsNullOrEmpty(commandLineArgument))
		{
			Debug.Log("[ServerManager] Deserializing server config from CLI argument (" + cliArgument + ")");
			this.ServerConfig = ConfigUtils.LoadConfigFromSerializedString<ServerConfig>(commandLineArgument);
			return;
		}
		string text = Utils.GetCommandLineArgument(filePathCliArgument, null) ?? defaultFilePath;
		Debug.Log("[ServerManager] Deserializing server config from file (" + text + ")");
		this.ServerConfig = ConfigUtils.LoadConfigFromFile<ServerConfig>(text, true);
	}

	// Token: 0x060007FF RID: 2047 RVA: 0x00026814 File Offset: 0x00024A14
	private bool StartListener(ushort port, bool forwardPorts = false)
	{
		if (NetworkManager.Singleton.IsListening)
		{
			return false;
		}
		if (forwardPorts)
		{
			this.StartPortForwarding(port);
		}
		Debug.Log(string.Format("[ServerManager] Starting Puck listener ({0})", ApplicationManager.Version));
		this.UnityTransport.SetConnectionData("0.0.0.0", port, null);
		return true;
	}

	// Token: 0x06000800 RID: 2048 RVA: 0x00026868 File Offset: 0x00024A68
	public void StartHost(ushort port, string name, int maxPlayers, string password, bool isPublic, bool useVoip, bool forwardPorts = false)
	{
		if (!this.StartListener(port, forwardPorts))
		{
			return;
		}
		this.ServerConfig = new ServerConfig
		{
			port = port,
			name = name,
			maxPlayers = maxPlayers,
			password = password,
			isPublic = isPublic,
			useVoip = useVoip
		};
		string s = JsonSerializer.Serialize<ConnectionData>(new ConnectionData
		{
			SteamId = BackendManager.PlayerState.PlayerData.steamId,
			Key = BackendManager.PlayerState.Key,
			Password = password,
			EnabledModIds = MonoBehaviourSingleton<ModManager>.Instance.EnabledModIds,
			Handedness = SettingsManager.Handedness,
			FlagID = SettingsManager.FlagID,
			HeadgearIDBlueAttacker = SettingsManager.HeadgearIDBlueAttacker,
			HeadgearIDRedAttacker = SettingsManager.HeadgearIDRedAttacker,
			HeadgearIDBlueGoalie = SettingsManager.HeadgearIDBlueGoalie,
			HeadgearIDRedGoalie = SettingsManager.HeadgearIDRedGoalie,
			MustacheID = SettingsManager.MustacheID,
			BeardID = SettingsManager.BeardID,
			JerseyIDBlueAttacker = SettingsManager.JerseyIDBlueAttacker,
			JerseyIDRedAttacker = SettingsManager.JerseyIDRedAttacker,
			JerseyIDBlueGoalie = SettingsManager.JerseyIDBlueGoalie,
			JerseyIDRedGoalie = SettingsManager.JerseyIDRedGoalie,
			StickSkinIDBlueAttacker = SettingsManager.StickSkinIDBlueAttacker,
			StickSkinIDRedAttacker = SettingsManager.StickSkinIDRedAttacker,
			StickSkinIDBlueGoalie = SettingsManager.StickSkinIDBlueGoalie,
			StickSkinIDRedGoalie = SettingsManager.StickSkinIDRedGoalie,
			StickShaftTapeIDBlueAttacker = SettingsManager.StickShaftTapeIDBlueAttacker,
			StickShaftTapeIDRedAttacker = SettingsManager.StickShaftTapeIDRedAttacker,
			StickShaftTapeIDBlueGoalie = SettingsManager.StickShaftTapeIDBlueGoalie,
			StickShaftTapeIDRedGoalie = SettingsManager.StickShaftTapeIDRedGoalie,
			StickBladeTapeIDBlueAttacker = SettingsManager.StickBladeTapeIDBlueAttacker,
			StickBladeTapeIDRedAttacker = SettingsManager.StickBladeTapeIDRedAttacker,
			StickBladeTapeIDBlueGoalie = SettingsManager.StickBladeTapeIDBlueGoalie,
			StickBladeTapeIDRedGoalie = SettingsManager.StickBladeTapeIDRedGoalie
		}, null);
		NetworkManager.Singleton.NetworkConfig.ConnectionData = Encoding.UTF8.GetBytes(s);
		this.IsHostStartInProgress = true;
		this.Authenticate();
	}

	// Token: 0x06000801 RID: 2049 RVA: 0x00026A31 File Offset: 0x00024C31
	public void StartServer(ushort port, bool forwardPorts = false)
	{
		if (!this.StartListener(port, forwardPorts))
		{
			return;
		}
		this.IsServerStartInProgress = true;
		this.Authenticate();
	}

	// Token: 0x06000802 RID: 2050 RVA: 0x00026A4B File Offset: 0x00024C4B
	public void StartPortForwarding(ushort port)
	{
		Debug.Log(string.Format("[ServerManager] Starting uPnP port forwarding for TCP & UDP port {0}", port));
		uPnPHelper.Start(uPnPHelper.Protocol.UDP, (int)port, 0, "Puck");
		Utils.PrintUPnPLogs();
		uPnPHelper.Start(uPnPHelper.Protocol.TCP, (int)port, 0, "Puck");
		Utils.PrintUPnPLogs();
	}

	// Token: 0x06000803 RID: 2051 RVA: 0x00026A86 File Offset: 0x00024C86
	public void StopPortForwarding()
	{
		Debug.Log("[ServerManager] Stopping uPnP port forwarding");
		uPnPHelper.CloseAll();
		Utils.PrintUPnPLogs();
	}

	// Token: 0x06000804 RID: 2052 RVA: 0x00026A9C File Offset: 0x00024C9C
	public void Authenticate()
	{
		string value = Environment.GetEnvironmentVariable("PUCK_MATCH_ID") ?? Utils.GetCommandLineArgument("--matchId", null);
		WebSocketManager.Emit("serverAuthenticateRequest", new Dictionary<string, object>
		{
			{
				"port",
				this.EdgegapManager.IsEdgegap ? this.EdgegapManager.ArbitriumPortPuckExternal : this.ServerConfig.port
			},
			{
				"isPublic",
				this.ServerConfig.isPublic
			},
			{
				"requestId",
				this.EdgegapManager.IsEdgegap ? this.EdgegapManager.RequestId : null
			},
			{
				"matchId",
				value
			}
		}, "serverAuthenticateResponse");
	}

	// Token: 0x06000805 RID: 2053 RVA: 0x00026B59 File Offset: 0x00024D59
	public void Unauthenticate()
	{
		WebSocketManager.Emit("serverUnauthenticateRequest", null, "serverUnauthenticateResponse");
	}

	// Token: 0x06000806 RID: 2054 RVA: 0x00026B6B File Offset: 0x00024D6B
	private void OnServerChanged(Server oldServer, Server newServer)
	{
		EventManager.TriggerEvent("Event_Everyone_OnServerChanged", new Dictionary<string, object>
		{
			{
				"oldServer",
				oldServer
			},
			{
				"newServer",
				newServer
			}
		});
	}

	// Token: 0x06000807 RID: 2055 RVA: 0x00026B9E File Offset: 0x00024D9E
	public void StartTcpServer(ushort port)
	{
		this.TcpServer = new TCPServer(port);
		this.TcpServer.OnMessageReceived += delegate(string ipPort, string message)
		{
			try
			{
				if (JsonSerializer.Deserialize<TCPServerMessage>(message, null).type == TCPServerMessageType.PreviewRequest)
				{
					JsonSerializer.Deserialize<TCPServerPreviewRequest>(message, null);
					string message2 = JsonSerializer.Serialize<TCPServerPreviewResponse>(new TCPServerPreviewResponse
					{
						name = this.ServerConfig.name,
						players = NetworkManager.Singleton.ConnectedClientsList.Count,
						maxPlayers = this.ServerConfig.maxPlayers,
						isPasswordProtected = !string.IsNullOrEmpty(this.ServerConfig.password),
						clientRequiredModIds = this.ClientRequiredModIds
					}, null);
					this.TcpServer.SendMessageAsync(ipPort, message2);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("[ServerManager] Error parsing message from " + ipPort + ": " + ex.Message);
			}
		};
		this.TcpServer.StartAsync();
	}

	// Token: 0x06000808 RID: 2056 RVA: 0x00026BCE File Offset: 0x00024DCE
	public void StopTcpServer()
	{
		this.TcpServer.StopAsync();
		this.TcpServer = null;
	}

	// Token: 0x06000809 RID: 2057 RVA: 0x00026BE2 File Offset: 0x00024DE2
	private void Server_OnServerStarted()
	{
		EventManager.TriggerEvent("Event_Server_OnServerStarted", new Dictionary<string, object>
		{
			{
				"serverConfig",
				this.ServerConfig
			}
		});
	}

	// Token: 0x0600080A RID: 2058 RVA: 0x00026C04 File Offset: 0x00024E04
	private void Server_OnServerStopped(bool wasHost)
	{
		EventManager.TriggerEvent("Event_Server_OnServerStopped", new Dictionary<string, object>
		{
			{
				"wasHost",
				wasHost
			}
		});
	}

	// Token: 0x0600080B RID: 2059 RVA: 0x00026C26 File Offset: 0x00024E26
	private void OnClientConnected(ulong clientId)
	{
		if (NetworkManager.Singleton.LocalClientId == clientId)
		{
			EventManager.TriggerEvent("Event_OnClientConnected", null);
		}
		EventManager.TriggerEvent("Event_Everyone_OnClientConnected", new Dictionary<string, object>
		{
			{
				"clientId",
				clientId
			}
		});
	}

	// Token: 0x0600080C RID: 2060 RVA: 0x00026C60 File Offset: 0x00024E60
	private void OnClientDisconnected(ulong clientId)
	{
		EventManager.TriggerEvent("Event_Everyone_OnClientDisconnected", new Dictionary<string, object>
		{
			{
				"clientId",
				clientId
			}
		});
		if (NetworkManager.Singleton.LocalClientId == clientId)
		{
			EventManager.TriggerEvent("Event_OnClientDisconnected", null);
		}
	}

	// Token: 0x0600080D RID: 2061 RVA: 0x00026C9A File Offset: 0x00024E9A
	private void OnTransportFailure()
	{
		EventManager.TriggerEvent("Event_OnTransportFailure", null);
	}

	// Token: 0x0600080E RID: 2062 RVA: 0x00026CA8 File Offset: 0x00024EA8
	public void Server_KickPlayer(Player player, DisconnectionCode disconnectionCode = DisconnectionCode.Kicked, string message = null, bool applyTimeout = true)
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
			this.TimeoutManager.AddSteamIdTimeout(steamId, 60f);
		}
		string reason = JsonSerializer.Serialize<Disconnection>(new Disconnection
		{
			code = disconnectionCode,
			message = message
		}, null);
		NetworkManager.Singleton.DisconnectClient(player.OwnerClientId, reason);
	}

	// Token: 0x0600080F RID: 2063 RVA: 0x00026D30 File Offset: 0x00024F30
	public void Server_BanPlayer(Player player)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		string steamId = player.SteamId.Value.ToString();
		this.Server_BanSteamId(steamId);
		this.Server_KickPlayer(player, DisconnectionCode.Banned, null, false);
	}

	// Token: 0x06000810 RID: 2064 RVA: 0x00026D75 File Offset: 0x00024F75
	public void Server_BanSteamId(string steamId)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.BanManager.AddBannedSteamId(steamId);
	}

	// Token: 0x06000811 RID: 2065 RVA: 0x00026D90 File Offset: 0x00024F90
	public void Server_UnbanSteamId(string steamId)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.BanManager.RemoveBannedSteamId(steamId);
	}

	// Token: 0x06000814 RID: 2068 RVA: 0x00026E7C File Offset: 0x0002507C
	protected override void __initializeVariables()
	{
		bool flag = this.Server == null;
		if (flag)
		{
			throw new Exception("ServerManager.Server cannot be null. All NetworkVariableBase instances must be initialized.");
		}
		this.Server.Initialize(this);
		base.__nameNetworkVariable(this.Server, "Server");
		this.NetworkVariableFields.Add(this.Server);
		base.__initializeVariables();
	}

	// Token: 0x06000815 RID: 2069 RVA: 0x00026EDF File Offset: 0x000250DF
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x06000816 RID: 2070 RVA: 0x00026EE9 File Offset: 0x000250E9
	protected internal override string __getTypeName()
	{
		return "ServerManager";
	}

	// Token: 0x040004C0 RID: 1216
	[HideInInspector]
	public UnityTransport UnityTransport;

	// Token: 0x040004C1 RID: 1217
	[HideInInspector]
	public EdgegapManager EdgegapManager;

	// Token: 0x040004C2 RID: 1218
	[HideInInspector]
	public ConnectionApprovalManager ConnectionApprovalManager;

	// Token: 0x040004C3 RID: 1219
	[HideInInspector]
	public TimeoutManager TimeoutManager;

	// Token: 0x040004C4 RID: 1220
	[HideInInspector]
	public BanManager BanManager;

	// Token: 0x040004C5 RID: 1221
	[HideInInspector]
	public AdminManager AdminManager;

	// Token: 0x040004C6 RID: 1222
	[HideInInspector]
	public WhitelistManager WhitelistManager;

	// Token: 0x040004C7 RID: 1223
	[HideInInspector]
	public NetworkVariable<Server> Server;

	// Token: 0x040004C8 RID: 1224
	[HideInInspector]
	public bool IsHostStartInProgress;

	// Token: 0x040004C9 RID: 1225
	[HideInInspector]
	public bool IsServerStartInProgress;

	// Token: 0x040004CA RID: 1226
	[HideInInspector]
	public TCPServer TcpServer;

	// Token: 0x040004CB RID: 1227
	[HideInInspector]
	public string IpAddress;

	// Token: 0x040004CC RID: 1228
	[HideInInspector]
	public ServerConfig ServerConfig;
}
