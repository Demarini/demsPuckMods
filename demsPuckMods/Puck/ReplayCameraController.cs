using System;
using System.Collections.Generic;

// Token: 0x02000028 RID: 40
public class ReplayCameraController : BaseCameraController
{
	// Token: 0x0600011F RID: 287 RVA: 0x00007928 File Offset: 0x00005B28
	public override void Awake()
	{
		base.Awake();
		this.replayCamera = base.GetComponent<ReplayCamera>();
	}

	// Token: 0x06000120 RID: 288 RVA: 0x0000793C File Offset: 0x00005B3C
	public override void Start()
	{
		base.Start();
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPuckSpawned", new Action<Dictionary<string, object>>(this.Event_OnPuckSpawned));
	}

	// Token: 0x06000121 RID: 289 RVA: 0x0000795F File Offset: 0x00005B5F
	public override void OnDestroy()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPuckSpawned", new Action<Dictionary<string, object>>(this.Event_OnPuckSpawned));
		base.OnDestroy();
	}

	// Token: 0x06000122 RID: 290 RVA: 0x00011CD0 File Offset: 0x0000FED0
	private void Event_OnPuckSpawned(Dictionary<string, object> message)
	{
		Puck puck = (Puck)message["puck"];
		this.replayCamera.SetTarget(puck.transform);
	}

	// Token: 0x06000124 RID: 292 RVA: 0x00011D00 File Offset: 0x0000FF00
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x06000125 RID: 293 RVA: 0x0000798A File Offset: 0x00005B8A
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x06000126 RID: 294 RVA: 0x00007994 File Offset: 0x00005B94
	protected internal override string __getTypeName()
	{
		return "ReplayCameraController";
	}

	// Token: 0x04000095 RID: 149
	private ReplayCamera replayCamera;
}
