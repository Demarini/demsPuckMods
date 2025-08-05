using System;
using System.Collections.Generic;
using SocketIOClient;
using Unity.Netcode;
using UnityEngine;

// Token: 0x02000141 RID: 321
public class UIToastManagerController : MonoBehaviour
{
	// Token: 0x06000B5A RID: 2906 RVA: 0x0000E9B2 File Offset: 0x0000CBB2
	private void Awake()
	{
		this.uiToastManager = base.GetComponent<UIToastManager>();
	}

	// Token: 0x06000B5B RID: 2907 RVA: 0x00040A74 File Offset: 0x0003EC74
	private void Start()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnClientConnected", new Action<Dictionary<string, object>>(this.Event_OnClientConnected));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnGetAuthTicketForWebApi", new Action<Dictionary<string, object>>(this.Event_Client_OnGetAuthTicketForWebApi));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnGotAuthTicketForWebApi", new Action<Dictionary<string, object>>(this.Event_Client_OnGotAuthTicketForWebApi));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnServerStartFailed", new Action<Dictionary<string, object>>(this.Event_Client_OnServerStartFailed));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnClientStarted", new Action<Dictionary<string, object>>(this.Event_Client_OnClientStarted));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnClientStopped", new Action<Dictionary<string, object>>(this.Event_Client_OnClientStopped));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnConnectionRejected", new Action<Dictionary<string, object>>(this.Event_Client_OnConnectionRejected));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnDisconnected", new Action<Dictionary<string, object>>(this.Event_Client_OnDisconnected));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnPendingModsReset", new Action<Dictionary<string, object>>(this.Event_Client_OnPendingModsReset));
		MonoBehaviourSingleton<WebSocketManager>.Instance.AddMessageListener("emit", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnEmit));
		MonoBehaviourSingleton<WebSocketManager>.Instance.AddMessageListener("playerAuthenticateResponse", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnPlayerAuthenticateResponse));
		MonoBehaviourSingleton<WebSocketManager>.Instance.AddMessageListener("playerStartPurchaseResponse", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnPlayerStartPurchaseResponse));
		MonoBehaviourSingleton<WebSocketManager>.Instance.AddMessageListener("playerCompletePurchaseResponse", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnPlayerCompletePurchaseResponse));
		MonoBehaviourSingleton<WebSocketManager>.Instance.AddMessageListener("playerLaunchServerResponse", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnPlayerLaunchServerResponse));
		MonoBehaviourSingleton<WebSocketManager>.Instance.AddMessageListener("playerSetIdentityResponse", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnPlayerSetIdentityResponse));
	}

	// Token: 0x06000B5C RID: 2908 RVA: 0x00040C18 File Offset: 0x0003EE18
	private void OnDestroy()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnClientConnected", new Action<Dictionary<string, object>>(this.Event_OnClientConnected));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnGetAuthTicketForWebApi", new Action<Dictionary<string, object>>(this.Event_Client_OnGetAuthTicketForWebApi));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnGotAuthTicketForWebApi", new Action<Dictionary<string, object>>(this.Event_Client_OnGotAuthTicketForWebApi));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnServerStartFailed", new Action<Dictionary<string, object>>(this.Event_Client_OnServerStartFailed));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnClientStarted", new Action<Dictionary<string, object>>(this.Event_Client_OnClientStarted));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnClientStopped", new Action<Dictionary<string, object>>(this.Event_Client_OnClientStopped));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnConnectionRejected", new Action<Dictionary<string, object>>(this.Event_Client_OnConnectionRejected));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnDisconnected", new Action<Dictionary<string, object>>(this.Event_Client_OnDisconnected));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnPendingModsReset", new Action<Dictionary<string, object>>(this.Event_Client_OnPendingModsReset));
		MonoBehaviourSingleton<WebSocketManager>.Instance.RemoveMessageListener("emit", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnEmit));
		MonoBehaviourSingleton<WebSocketManager>.Instance.RemoveMessageListener("playerAuthenticateResponse", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnPlayerAuthenticateResponse));
		MonoBehaviourSingleton<WebSocketManager>.Instance.RemoveMessageListener("playerStartPurchaseResponse", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnPlayerStartPurchaseResponse));
		MonoBehaviourSingleton<WebSocketManager>.Instance.RemoveMessageListener("playerCompletePurchaseResponse", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnPlayerCompletePurchaseResponse));
		MonoBehaviourSingleton<WebSocketManager>.Instance.RemoveMessageListener("playerLaunchServerResponse", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnPlayerLaunchServerResponse));
		MonoBehaviourSingleton<WebSocketManager>.Instance.RemoveMessageListener("playerSetIdentityResponse", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnPlayerSetIdentityResponse));
	}

	// Token: 0x06000B5D RID: 2909 RVA: 0x00040DBC File Offset: 0x0003EFBC
	private void Event_OnClientConnected(Dictionary<string, object> message)
	{
		ulong num = (ulong)message["clientId"];
		if (NetworkManager.Singleton.LocalClientId != num)
		{
			return;
		}
		this.uiToastManager.HideToast("connectingToServer");
	}

	// Token: 0x06000B5E RID: 2910 RVA: 0x0000E9C0 File Offset: 0x0000CBC0
	private void Event_Client_OnGetAuthTicketForWebApi(Dictionary<string, object> message)
	{
		this.uiToastManager.ShowToast("connectingToSteam", "Connecting to Steam...", float.PositiveInfinity);
	}

	// Token: 0x06000B5F RID: 2911 RVA: 0x0000E9DC File Offset: 0x0000CBDC
	private void Event_Client_OnGotAuthTicketForWebApi(Dictionary<string, object> message)
	{
		this.uiToastManager.HideToast("connectingToSteam");
	}

	// Token: 0x06000B60 RID: 2912 RVA: 0x00040DF8 File Offset: 0x0003EFF8
	private void Event_Client_OnServerStartFailed(Dictionary<string, object> message)
	{
		bool flag = (bool)message["isHost"];
		bool flag2 = (bool)message["isServer"];
		if (flag)
		{
			this.uiToastManager.ShowToast("serverStartFailed", "Host start failure: Transport failure", 3f);
			return;
		}
		if (flag2)
		{
			this.uiToastManager.ShowToast("serverStartFailed", "Server start failure: Transport failure", 3f);
		}
	}

	// Token: 0x06000B61 RID: 2913 RVA: 0x0000E9EE File Offset: 0x0000CBEE
	private void Event_Client_OnClientStarted(Dictionary<string, object> message)
	{
		this.uiToastManager.ShowToast("connectingToServer", "Connecting to server...", float.PositiveInfinity);
	}

	// Token: 0x06000B62 RID: 2914 RVA: 0x0000EA0A File Offset: 0x0000CC0A
	private void Event_Client_OnClientStopped(Dictionary<string, object> message)
	{
		this.uiToastManager.HideToast("connectingToServer");
	}

	// Token: 0x06000B63 RID: 2915 RVA: 0x00040E60 File Offset: 0x0003F060
	private void Event_Client_OnConnectionRejected(Dictionary<string, object> message)
	{
		ConnectionRejection connectionRejection = (ConnectionRejection)message["connectionRejection"];
		if (connectionRejection.code == ConnectionRejectionCode.MissingPassword || connectionRejection.code == ConnectionRejectionCode.MissingMods)
		{
			return;
		}
		this.uiToastManager.HideToast("connectingToServer");
		this.uiToastManager.ShowToast("connectionRejection", "Connection failure: " + Utils.GetConnectionRejectionMessage(connectionRejection.code), 3f);
	}

	// Token: 0x06000B64 RID: 2916 RVA: 0x00040ECC File Offset: 0x0003F0CC
	private void Event_Client_OnDisconnected(Dictionary<string, object> message)
	{
		Disconnection disconnection = (Disconnection)message["disconnection"];
		if (disconnection.code != DisconnectionCode.Disconnected)
		{
			this.uiToastManager.ShowToast("disconnection", "Disconnected: " + Utils.GetDisconnectionMessage(disconnection.code), 3f);
		}
	}

	// Token: 0x06000B65 RID: 2917 RVA: 0x00040F1C File Offset: 0x0003F11C
	private void Event_Client_OnPendingModsReset(Dictionary<string, object> message)
	{
		string text = (string)message["reason"];
		this.uiToastManager.ShowToast("pendingModsReset", text ?? "", 3f);
	}

	// Token: 0x06000B66 RID: 2918 RVA: 0x00040F5C File Offset: 0x0003F15C
	private void WebSocket_Event_OnEmit(Dictionary<string, object> message)
	{
		string a = (string)message["messageName"];
		if (a == "playerAuthenticateRequest")
		{
			this.uiToastManager.ShowToast("playerAuthenticate", "Authenticating with Puck...", float.PositiveInfinity);
			return;
		}
		if (a == "playerStartPurchaseRequest")
		{
			this.uiToastManager.ShowToast("playerStartPurchase", "Starting transaction...", float.PositiveInfinity);
			return;
		}
		if (a == "playerCompletePurchaseRequest")
		{
			this.uiToastManager.ShowToast("playerCompletePurchase", "Completing transaction...", float.PositiveInfinity);
			return;
		}
		if (a == "playerCancelPurchaseRequest")
		{
			this.uiToastManager.ShowToast("playerCancelPurchase", "Transaction cancelled.", 3f);
			return;
		}
		if (a == "playerLaunchServerRequest")
		{
			this.uiToastManager.ShowToast("playerLaunchServer", "Launching server...", float.PositiveInfinity);
			return;
		}
		if (!(a == "playerSetIdentityRequest"))
		{
			return;
		}
		this.uiToastManager.ShowToast("playerSetIdentity", "Setting identity...", float.PositiveInfinity);
	}

	// Token: 0x06000B67 RID: 2919 RVA: 0x00041070 File Offset: 0x0003F270
	private void WebSocket_Event_OnPlayerAuthenticateResponse(Dictionary<string, object> message)
	{
		PlayerAuthenticateResponse value = ((SocketIOResponse)message["response"]).GetValue<PlayerAuthenticateResponse>(0);
		if (value.success)
		{
			this.uiToastManager.HideToast("playerAuthenticate");
			return;
		}
		this.uiToastManager.ShowToast("playerAuthenticate", "Authentication failure: " + value.error, 3f);
	}

	// Token: 0x06000B68 RID: 2920 RVA: 0x000410D4 File Offset: 0x0003F2D4
	private void WebSocket_Event_OnPlayerStartPurchaseResponse(Dictionary<string, object> message)
	{
		PlayerStartPurchaseResponse value = ((SocketIOResponse)message["response"]).GetValue<PlayerStartPurchaseResponse>(0);
		if (value.success)
		{
			this.uiToastManager.HideToast("playerStartPurchase");
			return;
		}
		this.uiToastManager.ShowToast("playerStartPurchase", "Transaction failure: " + value.error, 3f);
	}

	// Token: 0x06000B69 RID: 2921 RVA: 0x00041138 File Offset: 0x0003F338
	private void WebSocket_Event_OnPlayerCompletePurchaseResponse(Dictionary<string, object> message)
	{
		PlayerCompletePurchaseResponse value = ((SocketIOResponse)message["response"]).GetValue<PlayerCompletePurchaseResponse>(0);
		if (value.success)
		{
			this.uiToastManager.HideToast("playerCompletePurchase");
			return;
		}
		this.uiToastManager.ShowToast("playerCompletePurchase", "Transaction failure: " + value.error, 3f);
	}

	// Token: 0x06000B6A RID: 2922 RVA: 0x0004119C File Offset: 0x0003F39C
	private void WebSocket_Event_OnPlayerLaunchServerResponse(Dictionary<string, object> message)
	{
		PlayerLaunchServerResponse value = ((SocketIOResponse)message["response"]).GetValue<PlayerLaunchServerResponse>(0);
		if (value.success)
		{
			this.uiToastManager.ShowToast("playerLaunchServer", "Server launched!", 3f);
			return;
		}
		this.uiToastManager.ShowToast("playerLaunchServer", "Server launch failure: " + value.error, 3f);
	}

	// Token: 0x06000B6B RID: 2923 RVA: 0x00041208 File Offset: 0x0003F408
	private void WebSocket_Event_OnPlayerSetIdentityResponse(Dictionary<string, object> message)
	{
		PlayerSetIdentityResponse value = ((SocketIOResponse)message["response"]).GetValue<PlayerSetIdentityResponse>(0);
		if (value.success)
		{
			this.uiToastManager.ShowToast("playerSetIdentity", "Identity set!", 3f);
			return;
		}
		this.uiToastManager.ShowToast("playerSetIdentity", "Identity set failure: " + value.error, 3f);
	}

	// Token: 0x040006B2 RID: 1714
	private UIToastManager uiToastManager;
}
