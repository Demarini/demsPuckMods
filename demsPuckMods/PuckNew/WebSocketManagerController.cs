using System;
using System.Collections.Generic;

// Token: 0x02000154 RID: 340
public static class WebSocketManagerController
{
	// Token: 0x06000A0E RID: 2574 RVA: 0x00030C16 File Offset: 0x0002EE16
	public static void Initialize()
	{
		EventManager.AddEventListener("Event_OnSteamConnected", new Action<Dictionary<string, object>>(WebSocketManagerController.Event_OnSteamConnected));
	}

	// Token: 0x06000A0F RID: 2575 RVA: 0x00030C2E File Offset: 0x0002EE2E
	public static void Dispose()
	{
		WebSocketManager.Disconnect();
		EventManager.RemoveEventListener("Event_OnSteamConnected", new Action<Dictionary<string, object>>(WebSocketManagerController.Event_OnSteamConnected));
	}

	// Token: 0x06000A10 RID: 2576 RVA: 0x00030C4C File Offset: 0x0002EE4C
	private static void Event_OnSteamConnected(Dictionary<string, object> message)
	{
		if (WebSocketManager.IsConnected || WebSocketManager.IsConnectionInProgress || WebSocketManager.IsReconnecting)
		{
			return;
		}
		WebSocketManager.Connect("wss://puck2.nasejevs.com");
	}
}
