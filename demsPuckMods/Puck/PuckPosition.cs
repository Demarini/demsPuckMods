using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200001B RID: 27
public class PuckPosition : MonoBehaviour
{
	// Token: 0x060000C6 RID: 198 RVA: 0x0000750B File Offset: 0x0000570B
	private void Start()
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnPuckPositionSpawned", new Dictionary<string, object>
		{
			{
				"puckPosition",
				this
			}
		});
	}

	// Token: 0x060000C7 RID: 199 RVA: 0x0000752D File Offset: 0x0000572D
	private void OnDestroy()
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_OnPuckPositionDespawned", new Dictionary<string, object>
		{
			{
				"puckPosition",
				this
			}
		});
	}

	// Token: 0x04000056 RID: 86
	public GamePhase Phase;
}
