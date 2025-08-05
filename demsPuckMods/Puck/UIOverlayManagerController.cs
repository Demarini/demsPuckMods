using System;
using System.Collections.Generic;
using SocketIOClient;
using Unity.Netcode;
using UnityEngine;

// Token: 0x0200011E RID: 286
internal class UIOverlayManagerController : MonoBehaviour
{
	// Token: 0x06000A12 RID: 2578 RVA: 0x0000D67E File Offset: 0x0000B87E
	private void Awake()
	{
		this.uiOverlay = base.GetComponent<UIOverlayManager>();
	}

	// Token: 0x06000A13 RID: 2579 RVA: 0x0003B698 File Offset: 0x00039898
	private void Start()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnClientConnected", new Action<Dictionary<string, object>>(this.Event_OnClientConnected));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnBaseCameraDisabled", new Action<Dictionary<string, object>>(this.Event_Client_OnBaseCameraDisabled));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnClientStarted", new Action<Dictionary<string, object>>(this.Event_Client_OnClientStarted));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnClientStopped", new Action<Dictionary<string, object>>(this.Event_Client_OnClientStopped));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnPopupShow", new Action<Dictionary<string, object>>(this.Event_Client_OnPopupShow));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnPopupHide", new Action<Dictionary<string, object>>(this.Event_Client_OnPopupHide));
		MonoBehaviourSingleton<WebSocketManager>.Instance.AddMessageListener("player", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnPlayer));
		MonoBehaviourSingleton<WebSocketManager>.Instance.AddMessageListener("playerStartPurchaseResponse", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnPlayerStartPurchaseResponse));
		MonoBehaviourSingleton<WebSocketManager>.Instance.AddMessageListener("playerCompletePurchaseResponse", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnPlayerCompletePurchaseResponse));
		MonoBehaviourSingleton<WebSocketManager>.Instance.AddMessageListener("emit", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnEmit));
		this.uiOverlay.ShowOverlay("loading", true, false, false, false);
	}

	// Token: 0x06000A14 RID: 2580 RVA: 0x0003B7C8 File Offset: 0x000399C8
	private void OnDestroy()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnClientConnected", new Action<Dictionary<string, object>>(this.Event_OnClientConnected));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnBaseCameraDisabled", new Action<Dictionary<string, object>>(this.Event_Client_OnBaseCameraDisabled));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnClientStarted", new Action<Dictionary<string, object>>(this.Event_Client_OnClientStarted));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnClientStopped", new Action<Dictionary<string, object>>(this.Event_Client_OnClientStopped));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnPopupShow", new Action<Dictionary<string, object>>(this.Event_Client_OnPopupShow));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnPopupHide", new Action<Dictionary<string, object>>(this.Event_Client_OnPopupHide));
		MonoBehaviourSingleton<WebSocketManager>.Instance.RemoveMessageListener("player", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnPlayer));
		MonoBehaviourSingleton<WebSocketManager>.Instance.RemoveMessageListener("playerStartPurchaseResponse", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnPlayerStartPurchaseResponse));
		MonoBehaviourSingleton<WebSocketManager>.Instance.RemoveMessageListener("playerCompletePurchaseResponse", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnPlayerCompletePurchaseResponse));
		MonoBehaviourSingleton<WebSocketManager>.Instance.RemoveMessageListener("emit", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnEmit));
	}

	// Token: 0x06000A15 RID: 2581 RVA: 0x0003B8E4 File Offset: 0x00039AE4
	private void Event_OnClientConnected(Dictionary<string, object> message)
	{
		ulong num = (ulong)message["clientId"];
		if (NetworkManager.Singleton.LocalClientId != num)
		{
			return;
		}
		this.uiOverlay.HideOverlay("connecting", true);
	}

	// Token: 0x06000A16 RID: 2582 RVA: 0x0000D68C File Offset: 0x0000B88C
	private void Event_Client_OnBaseCameraDisabled(Dictionary<string, object> message)
	{
		this.uiOverlay.ShowOverlay("camera", false, false, true, true);
	}

	// Token: 0x06000A17 RID: 2583 RVA: 0x0000D6A2 File Offset: 0x0000B8A2
	private void Event_Client_OnClientStarted(Dictionary<string, object> message)
	{
		this.uiOverlay.ShowOverlay("connecting", true, false, false, false);
	}

	// Token: 0x06000A18 RID: 2584 RVA: 0x0000D6B8 File Offset: 0x0000B8B8
	private void Event_Client_OnClientStopped(Dictionary<string, object> message)
	{
		this.uiOverlay.HideOverlay("connecting", true);
	}

	// Token: 0x06000A19 RID: 2585 RVA: 0x0003B924 File Offset: 0x00039B24
	private void Event_Client_OnPopupShow(Dictionary<string, object> message)
	{
		string a = (string)message["name"];
		if (a == "missingPassword")
		{
			this.uiOverlay.ShowOverlay("missingPassword", true, false, false, false);
			return;
		}
		if (!(a == "pendingMods"))
		{
			return;
		}
		this.uiOverlay.ShowOverlay("pendingMods", true, false, false, false);
	}

	// Token: 0x06000A1A RID: 2586 RVA: 0x0003B988 File Offset: 0x00039B88
	private void Event_Client_OnPopupHide(Dictionary<string, object> message)
	{
		string a = (string)message["name"];
		if (a == "missingPassword")
		{
			this.uiOverlay.HideOverlay("missingPassword", true);
			return;
		}
		if (!(a == "pendingMods"))
		{
			return;
		}
		this.uiOverlay.HideOverlay("pendingMods", true);
	}

	// Token: 0x06000A1B RID: 2587 RVA: 0x0000D6CB File Offset: 0x0000B8CB
	private void WebSocket_Event_OnPlayer(Dictionary<string, object> message)
	{
		this.uiOverlay.HideOverlay("loading", true);
	}

	// Token: 0x06000A1C RID: 2588 RVA: 0x0000D6DE File Offset: 0x0000B8DE
	private void WebSocket_Event_OnPlayerStartPurchaseResponse(Dictionary<string, object> message)
	{
		if (((SocketIOResponse)message["response"]).GetValue<PlayerStartPurchaseResponse>(0).success)
		{
			this.uiOverlay.ShowOverlay("purchase", false, true, false, false);
		}
	}

	// Token: 0x06000A1D RID: 2589 RVA: 0x0000D711 File Offset: 0x0000B911
	private void WebSocket_Event_OnPlayerCompletePurchaseResponse(Dictionary<string, object> message)
	{
		this.uiOverlay.HideOverlay("purchase", true);
	}

	// Token: 0x06000A1E RID: 2590 RVA: 0x0000D724 File Offset: 0x0000B924
	private void WebSocket_Event_OnEmit(Dictionary<string, object> message)
	{
		if ((string)message["messageName"] == "playerCancelPurchaseRequest")
		{
			this.uiOverlay.HideOverlay("purchase", true);
		}
	}

	// Token: 0x040005FE RID: 1534
	private UIOverlayManager uiOverlay;
}
