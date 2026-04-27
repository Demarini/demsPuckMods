using System;
using System.Collections.Generic;

// Token: 0x020001CC RID: 460
public class UIServerBrowserController : UIViewController<UIServerBrowser>
{
	// Token: 0x06000D28 RID: 3368 RVA: 0x0003E45C File Offset: 0x0003C65C
	public override void Awake()
	{
		base.Awake();
		this.uiServerBrowser = base.GetComponent<UIServerBrowser>();
		EventManager.AddEventListener("Event_OnServerBrowserShow", new Action<Dictionary<string, object>>(this.Event_OnServerBrowserShow));
		EventManager.AddEventListener("Event_OnServerBrowserClickRefresh", new Action<Dictionary<string, object>>(this.Event_OnServerBrowserClickRefresh));
		WebSocketManager.AddMessageListener("playerGetServerBrowserEndPointsResponse", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnPlayerGetServerBrowserEndPointsResponse));
	}

	// Token: 0x06000D29 RID: 3369 RVA: 0x0003E4C0 File Offset: 0x0003C6C0
	public override void OnDestroy()
	{
		EventManager.RemoveEventListener("Event_OnServerBrowserShow", new Action<Dictionary<string, object>>(this.Event_OnServerBrowserShow));
		EventManager.RemoveEventListener("Event_OnServerBrowserClickRefresh", new Action<Dictionary<string, object>>(this.Event_OnServerBrowserClickRefresh));
		WebSocketManager.RemoveMessageListener("playerGetServerBrowserEndPointsResponse", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnPlayerGetServerBrowserEndPointsResponse));
		base.OnDestroy();
	}

	// Token: 0x06000D2A RID: 3370 RVA: 0x0003E515 File Offset: 0x0003C715
	private void Event_OnServerBrowserShow(Dictionary<string, object> message)
	{
		this.uiServerBrowser.HideFilters();
		if (this.uiServerBrowser.ServerCount == 0)
		{
			this.uiServerBrowser.Refresh();
		}
	}

	// Token: 0x06000D2B RID: 3371 RVA: 0x0003E53A File Offset: 0x0003C73A
	private void Event_OnServerBrowserClickRefresh(Dictionary<string, object> message)
	{
		this.uiServerBrowser.Refresh();
	}

	// Token: 0x06000D2C RID: 3372 RVA: 0x0003E548 File Offset: 0x0003C748
	private void WebSocket_Event_OnPlayerGetServerBrowserEndPointsResponse(Dictionary<string, object> message)
	{
		ServerBrowserEndPointsResponse data = ((InMessage)message["inMessage"]).GetData<ServerBrowserEndPointsResponse>();
		List<EndPoint> list = new List<EndPoint>(data.data.endPoints);
		if (data.success)
		{
			this.uiServerBrowser.UpdateEndPoints(list.ToArray());
		}
	}

	// Token: 0x040007D8 RID: 2008
	private UIServerBrowser uiServerBrowser;
}
