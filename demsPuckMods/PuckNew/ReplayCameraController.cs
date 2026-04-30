using System;
using System.Collections.Generic;

// Token: 0x02000061 RID: 97
public class ReplayCameraController : BaseCameraController
{
	// Token: 0x06000347 RID: 839 RVA: 0x000138B9 File Offset: 0x00011AB9
	public override void Awake()
	{
		base.Awake();
		this.replayCamera = base.GetComponent<ReplayCamera>();
		EventManager.AddEventListener("Event_Everyone_OnPuckSpawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPuckSpawned));
	}

	// Token: 0x06000348 RID: 840 RVA: 0x000138E3 File Offset: 0x00011AE3
	public override void OnDestroy()
	{
		EventManager.RemoveEventListener("Event_Everyone_OnPuckSpawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPuckSpawned));
		base.OnDestroy();
	}

	// Token: 0x06000349 RID: 841 RVA: 0x00013904 File Offset: 0x00011B04
	private void Event_Everyone_OnPuckSpawned(Dictionary<string, object> message)
	{
		Puck puck = (Puck)message["puck"];
		this.replayCamera.Target = puck.transform;
	}

	// Token: 0x04000245 RID: 581
	private ReplayCamera replayCamera;
}
