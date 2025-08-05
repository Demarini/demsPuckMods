using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Token: 0x02000061 RID: 97
internal class NetworkManagerEventEmitter : MonoBehaviour
{
	// Token: 0x060002AE RID: 686 RVA: 0x00017C88 File Offset: 0x00015E88
	private void Start()
	{
		if (NetworkManager.Singleton == null)
		{
			return;
		}
		NetworkManager.Singleton.OnServerStarted += this.Server_OnServerStarted;
		NetworkManager.Singleton.OnServerStopped += this.Server_OnServerStopped;
		NetworkManager.Singleton.OnClientStarted += this.Client_OnClientStarted;
		NetworkManager.Singleton.OnClientStopped += this.Client_OnClientStopped;
		NetworkManager.Singleton.OnClientConnectedCallback += this.OnClientConnected;
		NetworkManager.Singleton.OnClientDisconnectCallback += this.OnClientDisconnected;
		NetworkManager.Singleton.OnTransportFailure += this.OnTransportFailure;
	}

	// Token: 0x060002AF RID: 687 RVA: 0x00017D40 File Offset: 0x00015F40
	private void OnDestroy()
	{
		if (NetworkManager.Singleton == null)
		{
			return;
		}
		NetworkManager.Singleton.OnServerStarted -= this.Server_OnServerStarted;
		NetworkManager.Singleton.OnServerStopped -= this.Server_OnServerStopped;
		NetworkManager.Singleton.OnClientStarted -= this.Client_OnClientStarted;
		NetworkManager.Singleton.OnClientStopped -= this.Client_OnClientStopped;
		NetworkManager.Singleton.OnClientConnectedCallback -= this.OnClientConnected;
		NetworkManager.Singleton.OnClientDisconnectCallback -= this.OnClientDisconnected;
		NetworkManager.Singleton.OnTransportFailure -= this.OnTransportFailure;
	}

	// Token: 0x060002B0 RID: 688 RVA: 0x00017DF8 File Offset: 0x00015FF8
	private void Update()
	{
		if (NetworkManager.Singleton)
		{
			if (this.isClient != NetworkManager.Singleton.IsClient)
			{
				this.isClient = NetworkManager.Singleton.IsClient;
				MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnIsClientChanged", null);
			}
			if (this.isServer != NetworkManager.Singleton.IsServer)
			{
				this.isServer = NetworkManager.Singleton.IsServer;
				MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Server_OnIsServerChanged", null);
			}
		}
	}

	// Token: 0x060002B1 RID: 689 RVA: 0x00017E78 File Offset: 0x00016078
	private void Server_OnServerStarted()
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Server_OnServerStarted", null);
		if (NetworkManager.Singleton == null)
		{
			return;
		}
		NetworkManager.Singleton.SceneManager.OnSynchronizeComplete += this.Server_OnSynchronizeComplete;
		if (NetworkManager.Singleton.IsHost)
		{
			this.Server_OnSynchronizeComplete(0UL);
		}
	}

	// Token: 0x060002B2 RID: 690 RVA: 0x000089B1 File Offset: 0x00006BB1
	private void Server_OnServerStopped(bool wasHost)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Server_OnServerStopped", new Dictionary<string, object>
		{
			{
				"wasHost",
				wasHost
			}
		});
	}

	// Token: 0x060002B3 RID: 691 RVA: 0x000089D8 File Offset: 0x00006BD8
	private void Client_OnClientStarted()
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnClientStarted", null);
	}

	// Token: 0x060002B4 RID: 692 RVA: 0x000089EA File Offset: 0x00006BEA
	private void Client_OnClientStopped(bool wasHost)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnClientStopped", new Dictionary<string, object>
		{
			{
				"wasHost",
				wasHost
			}
		});
	}

	// Token: 0x060002B5 RID: 693 RVA: 0x00008A11 File Offset: 0x00006C11
	private void OnClientConnected(ulong clientId)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnClientConnected", new Dictionary<string, object>
		{
			{
				"clientId",
				clientId
			}
		});
	}

	// Token: 0x060002B6 RID: 694 RVA: 0x00008A38 File Offset: 0x00006C38
	private void OnClientDisconnected(ulong clientId)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnClientDisconnected", new Dictionary<string, object>
		{
			{
				"clientId",
				clientId
			}
		});
	}

	// Token: 0x060002B7 RID: 695 RVA: 0x00008A5F File Offset: 0x00006C5F
	private void OnTransportFailure()
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnTransportFailure", null);
	}

	// Token: 0x060002B8 RID: 696 RVA: 0x00008A71 File Offset: 0x00006C71
	private void Server_OnSynchronizeComplete(ulong clientId)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Server_OnSynchronizeComplete", new Dictionary<string, object>
		{
			{
				"clientId",
				clientId
			}
		});
	}

	// Token: 0x04000190 RID: 400
	private bool isClient;

	// Token: 0x04000191 RID: 401
	private bool isServer;
}
