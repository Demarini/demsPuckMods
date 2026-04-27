using System;
using System.Collections.Generic;
using Unity.Netcode;

// Token: 0x020001D6 RID: 470
public class UIToastManagerController : UIViewController<UIToastManager>
{
	// Token: 0x06000DCB RID: 3531 RVA: 0x00041490 File Offset: 0x0003F690
	public override void Awake()
	{
		base.Awake();
		this.uiToastManager = base.GetComponent<UIToastManager>();
		EventManager.AddEventListener("Event_Everyone_OnClientConnected", new Action<Dictionary<string, object>>(this.Event_Everyone_OnClientConnected));
		EventManager.AddEventListener("Event_OnSteamInitializationStarted", new Action<Dictionary<string, object>>(this.Event_OnSteamInitializationStarted));
		EventManager.AddEventListener("Event_OnSteamInitializationFailed", new Action<Dictionary<string, object>>(this.Event_OnSteamInitializationFailed));
		EventManager.AddEventListener("Event_OnSteamInitialized", new Action<Dictionary<string, object>>(this.Event_OnSteamInitialized));
		EventManager.AddEventListener("Event_OnSteamConnectionFailed", new Action<Dictionary<string, object>>(this.Event_OnSteamConnectionFailed));
		EventManager.AddEventListener("Event_OnSteamConnected", new Action<Dictionary<string, object>>(this.Event_OnSteamConnected));
		EventManager.AddEventListener("Event_OnSteamDisconnected", new Action<Dictionary<string, object>>(this.Event_OnSteamDisconnected));
		EventManager.AddEventListener("Event_OnPlayerStateChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerStateChanged));
		EventManager.AddEventListener("Event_OnTransportFailure", new Action<Dictionary<string, object>>(this.Event_OnTransportFailure));
		EventManager.AddEventListener("Event_OnClientStarted", new Action<Dictionary<string, object>>(this.Event_OnClientStarted));
		EventManager.AddEventListener("Event_OnClientStopped", new Action<Dictionary<string, object>>(this.Event_OnClientStopped));
		EventManager.AddEventListener("Event_OnConnectionRejected", new Action<Dictionary<string, object>>(this.Event_OnConnectionRejected));
		EventManager.AddEventListener("Event_OnDisconnected", new Action<Dictionary<string, object>>(this.Event_OnDisconnected));
		EventManager.AddEventListener("Event_OnPendingModsReset", new Action<Dictionary<string, object>>(this.Event_OnPendingModsReset));
		WebSocketManager.AddMessageListener("emit", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnEmit));
		WebSocketManager.AddMessageListener("connecting", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnConnecting));
		WebSocketManager.AddMessageListener("connected", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnConnected));
		WebSocketManager.AddMessageListener("disconnected", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnDisconnected));
		WebSocketManager.AddMessageListener("PlayerStartTransactionResponse", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnPlayerStartTransactionResponse));
		WebSocketManager.AddMessageListener("playerDeployServerResponse", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnPlayerDeployServerResponse));
		WebSocketManager.AddMessageListener("playerSetIdentityResponse", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnPlayerSetIdentityResponse));
		WebSocketManager.AddMessageListener("playerJoinPartyResponse", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnPlayerJoinPartyResponse));
		WebSocketManager.AddMessageListener("playerStartMatchmakingResponse", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnPlayerStartMatchmakingResponse));
	}

	// Token: 0x06000DCC RID: 3532 RVA: 0x000416AC File Offset: 0x0003F8AC
	public override void OnDestroy()
	{
		EventManager.RemoveEventListener("Event_Everyone_OnClientConnected", new Action<Dictionary<string, object>>(this.Event_Everyone_OnClientConnected));
		EventManager.RemoveEventListener("Event_OnSteamInitializationStarted", new Action<Dictionary<string, object>>(this.Event_OnSteamInitializationStarted));
		EventManager.RemoveEventListener("Event_OnSteamInitializationFailed", new Action<Dictionary<string, object>>(this.Event_OnSteamInitializationFailed));
		EventManager.RemoveEventListener("Event_OnSteamInitialized", new Action<Dictionary<string, object>>(this.Event_OnSteamInitialized));
		EventManager.RemoveEventListener("Event_OnSteamConnectionFailed", new Action<Dictionary<string, object>>(this.Event_OnSteamConnectionFailed));
		EventManager.RemoveEventListener("Event_OnSteamConnected", new Action<Dictionary<string, object>>(this.Event_OnSteamConnected));
		EventManager.RemoveEventListener("Event_OnSteamDisconnected", new Action<Dictionary<string, object>>(this.Event_OnSteamDisconnected));
		EventManager.RemoveEventListener("Event_OnPlayerStateChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerStateChanged));
		EventManager.RemoveEventListener("Event_OnTransportFailure", new Action<Dictionary<string, object>>(this.Event_OnTransportFailure));
		EventManager.RemoveEventListener("Event_OnClientStarted", new Action<Dictionary<string, object>>(this.Event_OnClientStarted));
		EventManager.RemoveEventListener("Event_OnClientStopped", new Action<Dictionary<string, object>>(this.Event_OnClientStopped));
		EventManager.RemoveEventListener("Event_OnConnectionRejected", new Action<Dictionary<string, object>>(this.Event_OnConnectionRejected));
		EventManager.RemoveEventListener("Event_OnDisconnected", new Action<Dictionary<string, object>>(this.Event_OnDisconnected));
		EventManager.RemoveEventListener("Event_OnPendingModsReset", new Action<Dictionary<string, object>>(this.Event_OnPendingModsReset));
		WebSocketManager.RemoveMessageListener("emit", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnEmit));
		WebSocketManager.RemoveMessageListener("connecting", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnConnecting));
		WebSocketManager.RemoveMessageListener("connected", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnConnected));
		WebSocketManager.RemoveMessageListener("disconnected", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnDisconnected));
		WebSocketManager.RemoveMessageListener("PlayerStartTransactionResponse", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnPlayerStartTransactionResponse));
		WebSocketManager.RemoveMessageListener("playerDeployServerResponse", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnPlayerDeployServerResponse));
		WebSocketManager.RemoveMessageListener("playerSetIdentityResponse", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnPlayerSetIdentityResponse));
		WebSocketManager.RemoveMessageListener("playerJoinPartyResponse", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnPlayerJoinPartyResponse));
		WebSocketManager.RemoveMessageListener("playerStartMatchmakingResponse", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnPlayerStartMatchmakingResponse));
		base.OnDestroy();
	}

	// Token: 0x06000DCD RID: 3533 RVA: 0x000418BC File Offset: 0x0003FABC
	private void Event_Everyone_OnClientConnected(Dictionary<string, object> message)
	{
		ulong num = (ulong)message["clientId"];
		if (NetworkManager.Singleton.LocalClientId != num)
		{
			return;
		}
		this.uiToastManager.HideToast("serverConnection");
	}

	// Token: 0x06000DCE RID: 3534 RVA: 0x000418F8 File Offset: 0x0003FAF8
	private void Event_OnSteamInitializationStarted(Dictionary<string, object> message)
	{
		this.uiToastManager.ShowToast("steamInitialization", "Initializing Steam...", float.PositiveInfinity);
	}

	// Token: 0x06000DCF RID: 3535 RVA: 0x00041914 File Offset: 0x0003FB14
	private void Event_OnSteamInitializationFailed(Dictionary<string, object> message)
	{
		this.uiToastManager.ShowToast("steamInitialization", "Failed to initialize Steam, retrying...", float.PositiveInfinity);
	}

	// Token: 0x06000DD0 RID: 3536 RVA: 0x00041930 File Offset: 0x0003FB30
	private void Event_OnSteamInitialized(Dictionary<string, object> message)
	{
		this.uiToastManager.HideToast("steamInitialization");
		this.uiToastManager.ShowToast("steamConnection", "Connecting to Steam...", float.PositiveInfinity);
	}

	// Token: 0x06000DD1 RID: 3537 RVA: 0x0004195C File Offset: 0x0003FB5C
	private void Event_OnSteamConnectionFailed(Dictionary<string, object> message)
	{
		this.uiToastManager.ShowToast("steamConnection", "Failed to connect to Steam, retrying...", float.PositiveInfinity);
	}

	// Token: 0x06000DD2 RID: 3538 RVA: 0x00041978 File Offset: 0x0003FB78
	private void Event_OnSteamConnected(Dictionary<string, object> message)
	{
		this.uiToastManager.HideToast("steamConnection");
	}

	// Token: 0x06000DD3 RID: 3539 RVA: 0x0004198A File Offset: 0x0003FB8A
	private void Event_OnSteamDisconnected(Dictionary<string, object> message)
	{
		this.uiToastManager.ShowToast("steamConnection", "Disconnected from Steam, reconnecting...", float.PositiveInfinity);
	}

	// Token: 0x06000DD4 RID: 3540 RVA: 0x000419A8 File Offset: 0x0003FBA8
	private void Event_OnPlayerStateChanged(Dictionary<string, object> message)
	{
		if (((PlayerState)message["newPlayerState"]).AuthenticationPhase == AuthenticationPhase.Authenticating)
		{
			this.uiToastManager.ShowToast("playerAuthentication", "Authenticating...", float.PositiveInfinity);
			return;
		}
		this.uiToastManager.HideToast("playerAuthentication");
	}

	// Token: 0x06000DD5 RID: 3541 RVA: 0x000419F8 File Offset: 0x0003FBF8
	private void Event_OnTransportFailure(Dictionary<string, object> message)
	{
		this.uiToastManager.ShowToast("transportFailure", "Network transport failure", 3f);
	}

	// Token: 0x06000DD6 RID: 3542 RVA: 0x00041A14 File Offset: 0x0003FC14
	private void Event_OnClientStarted(Dictionary<string, object> message)
	{
		this.uiToastManager.ShowToast("serverConnection", "Connecting to server...", float.PositiveInfinity);
	}

	// Token: 0x06000DD7 RID: 3543 RVA: 0x00041A30 File Offset: 0x0003FC30
	private void Event_OnClientStopped(Dictionary<string, object> message)
	{
		this.uiToastManager.HideToast("serverConnection");
	}

	// Token: 0x06000DD8 RID: 3544 RVA: 0x00041A44 File Offset: 0x0003FC44
	private void Event_OnConnectionRejected(Dictionary<string, object> message)
	{
		ConnectionRejection connectionRejection = (ConnectionRejection)message["connectionRejection"];
		if (connectionRejection.code == ConnectionRejectionCode.MissingPassword || connectionRejection.code == ConnectionRejectionCode.MissingMods)
		{
			return;
		}
		this.uiToastManager.ShowToast("connectionRejected", "Connection rejected: " + Utils.GetConnectionRejectionMessage(connectionRejection.code, connectionRejection.message), 3f);
	}

	// Token: 0x06000DD9 RID: 3545 RVA: 0x00041AA8 File Offset: 0x0003FCA8
	private void Event_OnDisconnected(Dictionary<string, object> message)
	{
		Disconnection disconnection = (Disconnection)message["disconnection"];
		if (disconnection.code != DisconnectionCode.Disconnected)
		{
			this.uiToastManager.ShowToast("disconnected", "Disconnected: " + Utils.GetDisconnectionMessage(disconnection.code, disconnection.message), 3f);
		}
	}

	// Token: 0x06000DDA RID: 3546 RVA: 0x00041B00 File Offset: 0x0003FD00
	private void Event_OnPendingModsReset(Dictionary<string, object> message)
	{
		string text = (string)message["reason"];
		this.uiToastManager.ShowToast("pendingModsReset", text ?? "", 3f);
	}

	// Token: 0x06000DDB RID: 3547 RVA: 0x00041B3D File Offset: 0x0003FD3D
	private void WebSocket_Event_OnEmit(Dictionary<string, object> message)
	{
		if ((string)message["messageName"] == "playerDeployServerRequest")
		{
			this.uiToastManager.ShowToast("playerDeployServer", "Deploying server...", float.PositiveInfinity);
		}
	}

	// Token: 0x06000DDC RID: 3548 RVA: 0x00041B75 File Offset: 0x0003FD75
	private void WebSocket_Event_OnConnecting(Dictionary<string, object> message)
	{
		this.uiToastManager.ShowToast("webSocketConnection", "Connecting to Puck backend...", float.PositiveInfinity);
	}

	// Token: 0x06000DDD RID: 3549 RVA: 0x00041B91 File Offset: 0x0003FD91
	private void WebSocket_Event_OnConnected(Dictionary<string, object> message)
	{
		this.uiToastManager.HideToast("webSocketConnection");
	}

	// Token: 0x06000DDE RID: 3550 RVA: 0x00041BA3 File Offset: 0x0003FDA3
	private void WebSocket_Event_OnDisconnected(Dictionary<string, object> message)
	{
		this.uiToastManager.ShowToast("webSocketConnection", "Disconnected from Puck backend, reconnecting...", float.PositiveInfinity);
	}

	// Token: 0x06000DDF RID: 3551 RVA: 0x00041BC0 File Offset: 0x0003FDC0
	private void WebSocket_Event_OnPlayerStartTransactionResponse(Dictionary<string, object> message)
	{
		PlayerStartTransactionResponse data = ((InMessage)message["inMessage"]).GetData<PlayerStartTransactionResponse>();
		if (!data.success)
		{
			this.uiToastManager.ShowToast("playerStartTransaction", "Failed to start transaction: " + data.errorData.message, 3f);
		}
	}

	// Token: 0x06000DE0 RID: 3552 RVA: 0x00041C18 File Offset: 0x0003FE18
	private void WebSocket_Event_OnPlayerDeployServerResponse(Dictionary<string, object> message)
	{
		PlayerDeployServerResponse data = ((InMessage)message["inMessage"]).GetData<PlayerDeployServerResponse>();
		if (data.success)
		{
			this.uiToastManager.ShowToast("playerDeployServer", "Server deployed!", 3f);
			return;
		}
		this.uiToastManager.ShowToast("playerDeployServer", "Failed to deploy server: " + data.errorData.message, 3f);
	}

	// Token: 0x06000DE1 RID: 3553 RVA: 0x00041C88 File Offset: 0x0003FE88
	private void WebSocket_Event_OnPlayerSetIdentityResponse(Dictionary<string, object> message)
	{
		PlayerSetIdentityResponse data = ((InMessage)message["inMessage"]).GetData<PlayerSetIdentityResponse>();
		if (!data.success)
		{
			this.uiToastManager.ShowToast("playerSetIdentity", "Failed to set identity: " + data.errorData.message, 3f);
		}
	}

	// Token: 0x06000DE2 RID: 3554 RVA: 0x00041CE0 File Offset: 0x0003FEE0
	private void WebSocket_Event_OnPlayerJoinPartyResponse(Dictionary<string, object> message)
	{
		PlayerJoinPartyResponse data = ((InMessage)message["inMessage"]).GetData<PlayerJoinPartyResponse>();
		if (!data.success)
		{
			this.uiToastManager.ShowToast("playerJoinParty", "Failed to join party: " + data.errorData.message, 3f);
		}
	}

	// Token: 0x06000DE3 RID: 3555 RVA: 0x00041D38 File Offset: 0x0003FF38
	private void WebSocket_Event_OnPlayerStartMatchmakingResponse(Dictionary<string, object> message)
	{
		PlayerStartMatchmakingResponse data = ((InMessage)message["inMessage"]).GetData<PlayerStartMatchmakingResponse>();
		if (!data.success)
		{
			this.uiToastManager.ShowToast("playerStartMatchmaking", "Failed to start matchmaking: " + data.errorData.message, 3f);
		}
	}

	// Token: 0x0400081A RID: 2074
	private UIToastManager uiToastManager;
}
