using System;
using System.Collections.Generic;
using SocketIOClient;
using UnityEngine;

// Token: 0x020000AB RID: 171
public class StateManagerController : MonoBehaviour
{
	// Token: 0x060004FC RID: 1276 RVA: 0x0000A1F2 File Offset: 0x000083F2
	private void Awake()
	{
		this.stateManager = base.GetComponent<StateManager>();
	}

	// Token: 0x060004FD RID: 1277 RVA: 0x00020990 File Offset: 0x0001EB90
	private void Start()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnIdentityNameChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnIdentityNameChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnIdentityNumberChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnIdentityNumberChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnPopupClickOk", new Action<Dictionary<string, object>>(this.Event_Client_OnPopupClickOk));
		MonoBehaviourSingleton<WebSocketManager>.Instance.AddMessageListener("player", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnPlayer));
	}

	// Token: 0x060004FE RID: 1278 RVA: 0x00020A0C File Offset: 0x0001EC0C
	private void OnDestroy()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnIdentityNameChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnIdentityNameChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnIdentityNumberChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnIdentityNumberChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnPopupClickOk", new Action<Dictionary<string, object>>(this.Event_Client_OnPopupClickOk));
		MonoBehaviourSingleton<WebSocketManager>.Instance.RemoveMessageListener("player", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnPlayer));
	}

	// Token: 0x060004FF RID: 1279 RVA: 0x00020A88 File Offset: 0x0001EC88
	private void Event_Client_OnIdentityNameChanged(Dictionary<string, object> message)
	{
		string text = (string)message["value"];
		this.identityName = text;
	}

	// Token: 0x06000500 RID: 1280 RVA: 0x00020AB0 File Offset: 0x0001ECB0
	private void Event_Client_OnIdentityNumberChanged(Dictionary<string, object> message)
	{
		int num = (int)message["value"];
		this.identityNumber = num;
	}

	// Token: 0x06000501 RID: 1281 RVA: 0x00020AD8 File Offset: 0x0001ECD8
	private void Event_Client_OnPopupClickOk(Dictionary<string, object> message)
	{
		if ((string)message["name"] != "identity")
		{
			return;
		}
		MonoBehaviourSingleton<WebSocketManager>.Instance.Emit("playerSetIdentityRequest", new Dictionary<string, object>
		{
			{
				"username",
				this.identityName
			},
			{
				"number",
				this.identityNumber
			}
		}, "playerSetIdentityResponse");
	}

	// Token: 0x06000502 RID: 1282 RVA: 0x00020B44 File Offset: 0x0001ED44
	private void WebSocket_Event_OnPlayer(Dictionary<string, object> message)
	{
		SocketIOResponse socketIOResponse = (SocketIOResponse)message["response"];
		this.stateManager.PlayerData = socketIOResponse.GetValue<PlayerData>(0);
	}

	// Token: 0x040002CA RID: 714
	private StateManager stateManager;

	// Token: 0x040002CB RID: 715
	private string identityName;

	// Token: 0x040002CC RID: 716
	private int identityNumber;
}
