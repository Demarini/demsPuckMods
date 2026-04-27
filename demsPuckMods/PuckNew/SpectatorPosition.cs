using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000068 RID: 104
public class SpectatorPosition : MonoBehaviour
{
	// Token: 0x0600036F RID: 879 RVA: 0x000144DA File Offset: 0x000126DA
	private void Start()
	{
		EventManager.TriggerEvent("Event_OnSpectatorPositionSpawned", new Dictionary<string, object>
		{
			{
				"spectatorPosition",
				this
			}
		});
	}

	// Token: 0x06000370 RID: 880 RVA: 0x000144F7 File Offset: 0x000126F7
	private void OnDestroy()
	{
		EventManager.TriggerEvent("Event_OnSpectatorPositionDespawned", new Dictionary<string, object>
		{
			{
				"spectatorPosition",
				this
			}
		});
	}

	// Token: 0x06000371 RID: 881 RVA: 0x00014514 File Offset: 0x00012714
	private void OnDrawGizmos()
	{
		if (!Application.isEditor)
		{
			return;
		}
		Gizmos.color = Color.white;
		Gizmos.DrawLine(base.transform.position, base.transform.position + Vector3.up * 0.5f);
	}
}
