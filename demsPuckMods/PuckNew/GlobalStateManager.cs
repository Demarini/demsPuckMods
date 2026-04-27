using System;
using System.Collections.Generic;

// Token: 0x020000AA RID: 170
public static class GlobalStateManager
{
	// Token: 0x17000084 RID: 132
	// (get) Token: 0x0600055E RID: 1374 RVA: 0x0001D590 File Offset: 0x0001B790
	// (set) Token: 0x0600055F RID: 1375 RVA: 0x0001D597 File Offset: 0x0001B797
	public static UIState UIState
	{
		get
		{
			return GlobalStateManager.uiState;
		}
		set
		{
			if (GlobalStateManager.uiState.Equals(value))
			{
				return;
			}
			UIState oldUIState = GlobalStateManager.uiState;
			GlobalStateManager.uiState = value;
			GlobalStateManager.OnUIStateChanged(oldUIState, GlobalStateManager.uiState);
		}
	}

	// Token: 0x17000085 RID: 133
	// (get) Token: 0x06000560 RID: 1376 RVA: 0x0001D5BC File Offset: 0x0001B7BC
	// (set) Token: 0x06000561 RID: 1377 RVA: 0x0001D5C3 File Offset: 0x0001B7C3
	public static ConnectionState ConnectionState
	{
		get
		{
			return GlobalStateManager.connectionState;
		}
		set
		{
			if (GlobalStateManager.connectionState.Equals(value))
			{
				return;
			}
			ConnectionState oldConnectionState = GlobalStateManager.connectionState;
			GlobalStateManager.connectionState = value;
			GlobalStateManager.OnConnectionStateChanged(oldConnectionState, GlobalStateManager.connectionState);
		}
	}

	// Token: 0x06000562 RID: 1378 RVA: 0x0001D5E8 File Offset: 0x0001B7E8
	public static void Initialize()
	{
		GlobalStateManagerController.Initialize();
	}

	// Token: 0x06000563 RID: 1379 RVA: 0x0001D5EF File Offset: 0x0001B7EF
	public static void Dispose()
	{
		GlobalStateManagerController.Dispose();
	}

	// Token: 0x06000564 RID: 1380 RVA: 0x0001D5F8 File Offset: 0x0001B7F8
	public static void SetUIState(Dictionary<string, object> updates)
	{
		GlobalStateManager.UIState = new UIState
		{
			Phase = (updates.ContainsKey("phase") ? ((UIPhase)updates["phase"]) : GlobalStateManager.UIState.Phase),
			IsMouseRequired = (updates.ContainsKey("isMouseRequired") ? ((bool)updates["isMouseRequired"]) : GlobalStateManager.UIState.IsMouseRequired),
			IsMouseOverUI = (updates.ContainsKey("isMouseOverUI") ? ((bool)updates["isMouseOverUI"]) : GlobalStateManager.UIState.IsMouseOverUI),
			InteractingViews = (updates.ContainsKey("interactingViews") ? ((List<UIView>)updates["interactingViews"]) : GlobalStateManager.UIState.InteractingViews)
		};
	}

	// Token: 0x06000565 RID: 1381 RVA: 0x0001D6D4 File Offset: 0x0001B8D4
	public static void SetConnectionState(Dictionary<string, object> updates)
	{
		GlobalStateManager.ConnectionState = new ConnectionState
		{
			IsConnecting = (updates.ContainsKey("isConnecting") ? ((bool)updates["isConnecting"]) : GlobalStateManager.ConnectionState.IsConnecting),
			IsConnected = (updates.ContainsKey("isConnected") ? ((bool)updates["isConnected"]) : GlobalStateManager.ConnectionState.IsConnected),
			Connection = (updates.ContainsKey("connection") ? ((Connection)updates["connection"]) : GlobalStateManager.ConnectionState.Connection),
			LastConnection = (updates.ContainsKey("lastConnection") ? ((Connection)updates["lastConnection"]) : GlobalStateManager.ConnectionState.LastConnection),
			PendingConnection = (updates.ContainsKey("pendingConnection") ? ((Connection)updates["pendingConnection"]) : GlobalStateManager.ConnectionState.PendingConnection)
		};
	}

	// Token: 0x06000566 RID: 1382 RVA: 0x0001D7DF File Offset: 0x0001B9DF
	private static void OnUIStateChanged(UIState oldUIState, UIState newUIState)
	{
		EventManager.TriggerEvent("Event_OnUIStateChanged", new Dictionary<string, object>
		{
			{
				"oldUIState",
				oldUIState
			},
			{
				"newUIState",
				newUIState
			}
		});
	}

	// Token: 0x06000567 RID: 1383 RVA: 0x0001D812 File Offset: 0x0001BA12
	private static void OnConnectionStateChanged(ConnectionState oldConnectionState, ConnectionState newConnectionState)
	{
		EventManager.TriggerEvent("Event_OnConnectionStateChanged", new Dictionary<string, object>
		{
			{
				"oldConnectionState",
				oldConnectionState
			},
			{
				"newConnectionState",
				newConnectionState
			}
		});
	}

	// Token: 0x04000349 RID: 841
	private static UIState uiState;

	// Token: 0x0400034A RID: 842
	private static ConnectionState connectionState;
}
