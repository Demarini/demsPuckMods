using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000112 RID: 274
public class BanManagerController : MonoBehaviour
{
	// Token: 0x06000787 RID: 1927 RVA: 0x00024F35 File Offset: 0x00023135
	public void Awake()
	{
		this.banManager = base.GetComponent<BanManager>();
		EventManager.AddEventListener("Event_Server_OnServerStarted", new Action<Dictionary<string, object>>(this.Event_Server_OnServerStarted));
		EventManager.AddEventListener("Event_Server_OnServerStopped", new Action<Dictionary<string, object>>(this.Event_Server_OnServerStopped));
	}

	// Token: 0x06000788 RID: 1928 RVA: 0x000020D3 File Offset: 0x000002D3
	private void Start()
	{
	}

	// Token: 0x06000789 RID: 1929 RVA: 0x00024F6F File Offset: 0x0002316F
	private void OnDestroy()
	{
		EventManager.RemoveEventListener("Event_Server_OnServerStarted", new Action<Dictionary<string, object>>(this.Event_Server_OnServerStarted));
		EventManager.RemoveEventListener("Event_Server_OnServerStopped", new Action<Dictionary<string, object>>(this.Event_Server_OnServerStopped));
	}

	// Token: 0x0600078A RID: 1930 RVA: 0x00024F9D File Offset: 0x0002319D
	private void Event_Server_OnServerStarted(Dictionary<string, object> message)
	{
		this.banManager.LoadBannedSteamIds();
		this.banManager.LoadBannedIpAddresses();
	}

	// Token: 0x0600078B RID: 1931 RVA: 0x00024FB5 File Offset: 0x000231B5
	private void Event_Server_OnServerStopped(Dictionary<string, object> message)
	{
		this.banManager.Dispose();
	}

	// Token: 0x04000486 RID: 1158
	private BanManager banManager;
}
