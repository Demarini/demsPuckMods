using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000103 RID: 259
public class ReplayManagerController : MonoBehaviour
{
	// Token: 0x0600070F RID: 1807 RVA: 0x0002273C File Offset: 0x0002093C
	private void Awake()
	{
		this.replayManager = base.GetComponent<ReplayManager>();
		EventManager.AddEventListener("Event_Server_OnServerStopped", new Action<Dictionary<string, object>>(this.Event_Server_OnServerStopped));
	}

	// Token: 0x06000710 RID: 1808 RVA: 0x00022760 File Offset: 0x00020960
	private void OnDestroy()
	{
		EventManager.RemoveEventListener("Event_Server_OnServerStopped", new Action<Dictionary<string, object>>(this.Event_Server_OnServerStopped));
	}

	// Token: 0x06000711 RID: 1809 RVA: 0x00022778 File Offset: 0x00020978
	private void Event_Server_OnServerStopped(Dictionary<string, object> message)
	{
		this.replayManager.Server_StopReplaying();
		this.replayManager.Server_StopRecording();
	}

	// Token: 0x04000462 RID: 1122
	private ReplayManager replayManager;
}
