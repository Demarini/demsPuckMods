using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000125 RID: 293
public class TimeoutManagerController : MonoBehaviour
{
	// Token: 0x06000835 RID: 2101 RVA: 0x00027989 File Offset: 0x00025B89
	public void Awake()
	{
		this.timeoutManager = base.GetComponent<TimeoutManager>();
		EventManager.AddEventListener("Event_Server_OnServerStopped", new Action<Dictionary<string, object>>(this.Event_Server_OnServerStopped));
	}

	// Token: 0x06000836 RID: 2102 RVA: 0x000020D3 File Offset: 0x000002D3
	private void Start()
	{
	}

	// Token: 0x06000837 RID: 2103 RVA: 0x000279AD File Offset: 0x00025BAD
	private void OnDestroy()
	{
		EventManager.RemoveEventListener("Event_Server_OnServerStopped", new Action<Dictionary<string, object>>(this.Event_Server_OnServerStopped));
	}

	// Token: 0x06000838 RID: 2104 RVA: 0x000279C5 File Offset: 0x00025BC5
	private void Event_Server_OnServerStopped(Dictionary<string, object> message)
	{
		this.timeoutManager.Dispose();
	}

	// Token: 0x040004D6 RID: 1238
	private TimeoutManager timeoutManager;
}
