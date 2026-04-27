using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000110 RID: 272
public class AdminManagerController : MonoBehaviour
{
	// Token: 0x0600076E RID: 1902 RVA: 0x00024A3F File Offset: 0x00022C3F
	public void Awake()
	{
		this.adminManager = base.GetComponent<AdminManager>();
		EventManager.AddEventListener("Event_Server_OnServerStarted", new Action<Dictionary<string, object>>(this.Event_Server_OnServerStarted));
		EventManager.AddEventListener("Event_Server_OnServerStopped", new Action<Dictionary<string, object>>(this.Event_Server_OnServerStopped));
	}

	// Token: 0x0600076F RID: 1903 RVA: 0x000020D3 File Offset: 0x000002D3
	private void Start()
	{
	}

	// Token: 0x06000770 RID: 1904 RVA: 0x00024A79 File Offset: 0x00022C79
	private void OnDestroy()
	{
		EventManager.RemoveEventListener("Event_Server_OnServerStarted", new Action<Dictionary<string, object>>(this.Event_Server_OnServerStarted));
		EventManager.RemoveEventListener("Event_Server_OnServerStopped", new Action<Dictionary<string, object>>(this.Event_Server_OnServerStopped));
	}

	// Token: 0x06000771 RID: 1905 RVA: 0x00024AA7 File Offset: 0x00022CA7
	private void Event_Server_OnServerStarted(Dictionary<string, object> message)
	{
		this.adminManager.LoadAdminSteamIds();
	}

	// Token: 0x06000772 RID: 1906 RVA: 0x00024AB4 File Offset: 0x00022CB4
	private void Event_Server_OnServerStopped(Dictionary<string, object> message)
	{
		this.adminManager.Dispose();
	}

	// Token: 0x0400047F RID: 1151
	private AdminManager adminManager;
}
