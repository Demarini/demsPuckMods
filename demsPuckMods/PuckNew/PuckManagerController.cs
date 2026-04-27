using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000F5 RID: 245
public class PuckManagerController : MonoBehaviour
{
	// Token: 0x06000702 RID: 1794 RVA: 0x00022500 File Offset: 0x00020700
	private void Awake()
	{
		this.puckManager = base.GetComponent<PuckManager>();
		EventManager.AddEventListener("Event_Everyone_OnPuckPositionSpawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPuckPositionSpawned));
		EventManager.AddEventListener("Event_Everyone_OnPuckPositionDespawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPuckPositionDespawned));
		EventManager.AddEventListener("Event_Everyone_OnPuckSpawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPuckSpawned));
		EventManager.AddEventListener("Event_Everyone_OnPuckDespawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPuckDespawned));
	}

	// Token: 0x06000703 RID: 1795 RVA: 0x00022574 File Offset: 0x00020774
	private void OnDestroy()
	{
		EventManager.RemoveEventListener("Event_Everyone_OnPuckPositionSpawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPuckPositionSpawned));
		EventManager.RemoveEventListener("Event_Everyone_OnPuckPositionDespawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPuckPositionDespawned));
		EventManager.RemoveEventListener("Event_Everyone_OnPuckSpawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPuckSpawned));
		EventManager.RemoveEventListener("Event_Everyone_OnPuckDespawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPuckDespawned));
	}

	// Token: 0x06000704 RID: 1796 RVA: 0x000225DC File Offset: 0x000207DC
	private void Event_Everyone_OnPuckPositionSpawned(Dictionary<string, object> message)
	{
		PuckPosition puckPosition = (PuckPosition)message["puckPosition"];
		this.puckManager.AddPuckPosition(puckPosition);
	}

	// Token: 0x06000705 RID: 1797 RVA: 0x00022608 File Offset: 0x00020808
	private void Event_Everyone_OnPuckPositionDespawned(Dictionary<string, object> message)
	{
		PuckPosition puckPosition = (PuckPosition)message["puckPosition"];
		this.puckManager.RemovePuckPosition(puckPosition);
	}

	// Token: 0x06000706 RID: 1798 RVA: 0x00022634 File Offset: 0x00020834
	private void Event_Everyone_OnPuckSpawned(Dictionary<string, object> message)
	{
		Puck puck = (Puck)message["puck"];
		this.puckManager.AddPuck(puck);
	}

	// Token: 0x06000707 RID: 1799 RVA: 0x00022660 File Offset: 0x00020860
	private void Event_Everyone_OnPuckDespawned(Dictionary<string, object> message)
	{
		Puck puck = (Puck)message["puck"];
		this.puckManager.RemovePuck(puck);
	}

	// Token: 0x0400042E RID: 1070
	private PuckManager puckManager;
}
