using System;
using System.Collections.Generic;
using SocketIOClient;
using UnityEngine;

// Token: 0x0200013A RID: 314
public class UIServerLauncherController : MonoBehaviour
{
	// Token: 0x06000B02 RID: 2818 RVA: 0x0000E110 File Offset: 0x0000C310
	private void Awake()
	{
		this.uIServerLauncher = base.GetComponent<UIServerLauncher>();
	}

	// Token: 0x06000B03 RID: 2819 RVA: 0x0000E11E File Offset: 0x0000C31E
	private void Start()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnPlayerDataChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnPlayerDataChanged));
		MonoBehaviourSingleton<WebSocketManager>.Instance.AddMessageListener("playerGetServerLauncherLocationsResponse", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnPlayerGetServerLauncherLocationsResponse));
	}

	// Token: 0x06000B04 RID: 2820 RVA: 0x0000E156 File Offset: 0x0000C356
	private void OnDestroy()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnPlayerDataChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnPlayerDataChanged));
		MonoBehaviourSingleton<WebSocketManager>.Instance.RemoveMessageListener("playerGetServerLauncherLocationsResponse", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnPlayerGetServerLauncherLocationsResponse));
	}

	// Token: 0x06000B05 RID: 2821 RVA: 0x0000E18E File Offset: 0x0000C38E
	private void Event_Client_OnPlayerDataChanged(Dictionary<string, object> message)
	{
		if (((PlayerData)message["newPlayerData"]).patreonLevel >= 1)
		{
			this.uIServerLauncher.ShowDedicatedPasswordProtection();
			return;
		}
		this.uIServerLauncher.HideDedicatedPasswordProtection();
	}

	// Token: 0x06000B06 RID: 2822 RVA: 0x0003F08C File Offset: 0x0003D28C
	private void WebSocket_Event_OnPlayerGetServerLauncherLocationsResponse(Dictionary<string, object> message)
	{
		ServerLauncherLocationsResponse value = ((SocketIOResponse)message["response"]).GetValue<ServerLauncherLocationsResponse>(0);
		this.uIServerLauncher.SetDedicatedLocations(value.locations);
	}

	// Token: 0x04000679 RID: 1657
	private UIServerLauncher uIServerLauncher;
}
