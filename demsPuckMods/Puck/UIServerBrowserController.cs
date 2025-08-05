using System;
using System.Collections.Generic;
using System.Linq;
using SocketIOClient;
using UnityEngine;

// Token: 0x02000136 RID: 310
public class UIServerBrowserController : MonoBehaviour
{
	// Token: 0x06000ADA RID: 2778 RVA: 0x0000DE6B File Offset: 0x0000C06B
	private void Awake()
	{
		this.uiServerBrowser = base.GetComponent<UIServerBrowser>();
	}

	// Token: 0x06000ADB RID: 2779 RVA: 0x0000DE79 File Offset: 0x0000C079
	private void Start()
	{
		MonoBehaviourSingleton<WebSocketManager>.Instance.AddMessageListener("playerGetServerBrowserServersResponse", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnPlayerGetServerBrowserServersResponse));
	}

	// Token: 0x06000ADC RID: 2780 RVA: 0x0000DE96 File Offset: 0x0000C096
	private void OnDestroy()
	{
		MonoBehaviourSingleton<WebSocketManager>.Instance.RemoveMessageListener("playerGetServerBrowserServersResponse", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnPlayerGetServerBrowserServersResponse));
	}

	// Token: 0x06000ADD RID: 2781 RVA: 0x0003E720 File Offset: 0x0003C920
	private void WebSocket_Event_OnPlayerGetServerBrowserServersResponse(Dictionary<string, object> message)
	{
		ServerBrowserServersResponse value = ((SocketIOResponse)message["response"]).GetValue<ServerBrowserServersResponse>(0);
		this.uiServerBrowser.ClearServers();
		this.uiServerBrowser.UpdateServers(value.servers.ToList<ServerBrowserServer>());
	}

	// Token: 0x0400065A RID: 1626
	private UIServerBrowser uiServerBrowser;
}
