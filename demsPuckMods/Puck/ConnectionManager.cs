using System;
using System.Text;
using System.Text.Json;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

// Token: 0x0200005F RID: 95
public class ConnectionManager : MonoBehaviourSingleton<ConnectionManager>
{
	// Token: 0x17000048 RID: 72
	// (get) Token: 0x0600029A RID: 666 RVA: 0x00008944 File Offset: 0x00006B44
	[HideInInspector]
	public bool IsPendingConnection
	{
		get
		{
			return this.PendingConnection != null;
		}
	}

	// Token: 0x0600029B RID: 667 RVA: 0x000174B8 File Offset: 0x000156B8
	private void Start()
	{
		this.UnityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
		ushort num;
		if (ushort.TryParse(Application.version, out num))
		{
			Debug.Log(string.Format("[ConnectionManager] Setting NetworkConfig protocol version to {0}", num));
			NetworkManager.Singleton.NetworkConfig.ProtocolVersion = num;
		}
	}

	// Token: 0x0600029C RID: 668 RVA: 0x00017508 File Offset: 0x00015708
	public void Client_StartClient(string ipAddress, ushort port, string password = "")
	{
		Debug.Log(string.Format("[ConnectionManager] Starting client {0}:{1}", ipAddress, port));
		if (NetworkManager.Singleton.IsClient)
		{
			this.PendingConnection = new Connection
			{
				IpAddress = ipAddress,
				Port = port,
				Password = password
			};
			Debug.Log("[ConnectionManager] Existing connection detected, disconnecting and setting pending connection");
			this.Client_Disconnect();
			return;
		}
		string s = JsonSerializer.Serialize<ConnectionData>(new ConnectionData
		{
			Password = password,
			SteamId = MonoBehaviourSingleton<StateManager>.Instance.PlayerData.steamId,
			SocketId = MonoBehaviourSingleton<WebSocketManager>.Instance.SocketId,
			EnabledModIds = MonoBehaviourSingleton<ModManagerV2>.Instance.EnabledModIds
		}, null);
		NetworkManager.Singleton.NetworkConfig.ConnectionData = Encoding.ASCII.GetBytes(s);
		this.LastConnection = new Connection
		{
			IpAddress = ipAddress,
			Port = port,
			Password = password
		};
		this.IsConnecting = true;
		this.UnityTransport.SetConnectionData(ipAddress, port, null);
		NetworkManager.Singleton.StartClient();
	}

	// Token: 0x0600029D RID: 669 RVA: 0x0000894F File Offset: 0x00006B4F
	public void Client_Disconnect()
	{
		if (!NetworkManager.Singleton.IsClient)
		{
			return;
		}
		Debug.Log("[ConnectionManager] Puck (" + Application.version + ") network shutdown");
		NetworkManager.Singleton.Shutdown(true);
	}

	// Token: 0x0400018A RID: 394
	[HideInInspector]
	public UnityTransport UnityTransport;

	// Token: 0x0400018B RID: 395
	[HideInInspector]
	public bool IsConnecting;

	// Token: 0x0400018C RID: 396
	[HideInInspector]
	public Connection PendingConnection;

	// Token: 0x0400018D RID: 397
	[HideInInspector]
	public Connection LastConnection;
}
