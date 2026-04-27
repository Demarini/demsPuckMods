using System;
using System.Collections.Generic;

// Token: 0x020001A7 RID: 423
public class UINewServerController : UIViewController<UINewServer>
{
	// Token: 0x06000C2C RID: 3116 RVA: 0x000399FD File Offset: 0x00037BFD
	public override void Awake()
	{
		base.Awake();
		this.uiNewServer = base.GetComponent<UINewServer>();
		EventManager.AddEventListener("Event_OnPlayerDataChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerDataChanged));
		WebSocketManager.AddMessageListener("playerGetLocationsResponse", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnPlayerGetLocationsResponse));
	}

	// Token: 0x06000C2D RID: 3117 RVA: 0x00039A3D File Offset: 0x00037C3D
	public override void OnDestroy()
	{
		EventManager.RemoveEventListener("Event_OnPlayerDataChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerDataChanged));
		WebSocketManager.RemoveMessageListener("playerGetLocationsResponse", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnPlayerGetLocationsResponse));
		base.OnDestroy();
	}

	// Token: 0x06000C2E RID: 3118 RVA: 0x00039A74 File Offset: 0x00037C74
	private void Event_OnPlayerDataChanged(Dictionary<string, object> message)
	{
		PlayerData playerData = (PlayerData)message["newPlayerData"];
		if (playerData == null)
		{
			return;
		}
		if (playerData.patreonLevel >= 1)
		{
			this.uiNewServer.HidePatreonOverlay();
			return;
		}
		this.uiNewServer.ShowPatreonOverlay();
	}

	// Token: 0x06000C2F RID: 3119 RVA: 0x00039AB8 File Offset: 0x00037CB8
	private void WebSocket_Event_OnPlayerGetLocationsResponse(Dictionary<string, object> message)
	{
		PlayerGetLocationsResponse data = ((InMessage)message["inMessage"]).GetData<PlayerGetLocationsResponse>();
		this.uiNewServer.SetDedicatedLocations(data.data.locations);
	}

	// Token: 0x04000735 RID: 1845
	private UINewServer uiNewServer;
}
