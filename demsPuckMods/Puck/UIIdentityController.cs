using System;
using System.Collections.Generic;
using SocketIOClient;
using UnityEngine;

// Token: 0x0200010F RID: 271
public class UIIdentityController : MonoBehaviour
{
	// Token: 0x06000982 RID: 2434 RVA: 0x0000CF4E File Offset: 0x0000B14E
	private void Awake()
	{
		this.uiIdentity = base.GetComponent<UIIdentity>();
	}

	// Token: 0x06000983 RID: 2435 RVA: 0x0000CF5C File Offset: 0x0000B15C
	private void Start()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnPlayerDataChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnPlayerDataChanged));
		MonoBehaviourSingleton<WebSocketManager>.Instance.AddMessageListener("playerSetIdentityResponse", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnPlayerSetIdentityResponse));
	}

	// Token: 0x06000984 RID: 2436 RVA: 0x0000CF94 File Offset: 0x0000B194
	private void OnDestroy()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnPlayerDataChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnPlayerDataChanged));
		MonoBehaviourSingleton<WebSocketManager>.Instance.RemoveMessageListener("playerSetIdentityResponse", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnPlayerSetIdentityResponse));
	}

	// Token: 0x06000985 RID: 2437 RVA: 0x0000CFCC File Offset: 0x0000B1CC
	private void Event_Client_OnPlayerDataChanged(Dictionary<string, object> message)
	{
		this.uiIdentity.ApplyIdentityValues();
	}

	// Token: 0x06000986 RID: 2438 RVA: 0x0000CFD9 File Offset: 0x0000B1D9
	private void WebSocket_Event_OnPlayerSetIdentityResponse(Dictionary<string, object> message)
	{
		if (!((SocketIOResponse)message["response"]).GetValue<PlayerSetIdentityResponse>(0).success)
		{
			this.uiIdentity.ApplyIdentityValues();
		}
	}

	// Token: 0x040005B3 RID: 1459
	private UIIdentity uiIdentity;
}
