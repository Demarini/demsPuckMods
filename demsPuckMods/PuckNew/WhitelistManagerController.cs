using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000127 RID: 295
public class WhitelistManagerController : MonoBehaviour
{
	// Token: 0x06000847 RID: 2119 RVA: 0x00027CEA File Offset: 0x00025EEA
	public void Awake()
	{
		this.whitelistManager = base.GetComponent<WhitelistManager>();
		EventManager.AddEventListener("Event_Server_OnServerStarted", new Action<Dictionary<string, object>>(this.Event_Server_OnServerStarted));
		EventManager.AddEventListener("Event_Server_OnServerStopped", new Action<Dictionary<string, object>>(this.Event_Server_OnServerStopped));
	}

	// Token: 0x06000848 RID: 2120 RVA: 0x000020D3 File Offset: 0x000002D3
	private void Start()
	{
	}

	// Token: 0x06000849 RID: 2121 RVA: 0x00027D24 File Offset: 0x00025F24
	private void OnDestroy()
	{
		EventManager.RemoveEventListener("Event_Server_OnServerStarted", new Action<Dictionary<string, object>>(this.Event_Server_OnServerStarted));
		EventManager.RemoveEventListener("Event_Server_OnServerStopped", new Action<Dictionary<string, object>>(this.Event_Server_OnServerStopped));
	}

	// Token: 0x0600084A RID: 2122 RVA: 0x00027D52 File Offset: 0x00025F52
	private void Event_Server_OnServerStarted(Dictionary<string, object> message)
	{
		this.whitelistManager.LoadWhitelistedSteamIds();
	}

	// Token: 0x0600084B RID: 2123 RVA: 0x00027D5F File Offset: 0x00025F5F
	private void Event_Server_OnServerStopped(Dictionary<string, object> message)
	{
		this.whitelistManager.Dispose();
	}

	// Token: 0x040004DA RID: 1242
	private WhitelistManager whitelistManager;
}
