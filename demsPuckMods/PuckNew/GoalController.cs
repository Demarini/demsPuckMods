using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Token: 0x0200000A RID: 10
public class GoalController : MonoBehaviour
{
	// Token: 0x06000031 RID: 49 RVA: 0x0000280A File Offset: 0x00000A0A
	private void Awake()
	{
		this.goal = base.GetComponent<Goal>();
		EventManager.AddEventListener("Event_Everyone_OnPuckSpawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPuckSpawned));
		EventManager.AddEventListener("Event_Everyone_OnPuckDespawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPuckDespawned));
	}

	// Token: 0x06000032 RID: 50 RVA: 0x00002844 File Offset: 0x00000A44
	public void OnDestroy()
	{
		EventManager.RemoveEventListener("Event_Everyone_OnPuckSpawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPuckSpawned));
		EventManager.RemoveEventListener("Event_Everyone_OnPuckDespawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPuckDespawned));
	}

	// Token: 0x06000033 RID: 51 RVA: 0x00002874 File Offset: 0x00000A74
	private void Event_Everyone_OnPuckSpawned(Dictionary<string, object> message)
	{
		Puck puck = (Puck)message["puck"];
		if (!NetworkManager.Singleton.IsClient)
		{
			return;
		}
		this.goal.Client_AddNetClothSphereCollider(puck.NetSphereCollider);
	}

	// Token: 0x06000034 RID: 52 RVA: 0x000028B0 File Offset: 0x00000AB0
	private void Event_Everyone_OnPuckDespawned(Dictionary<string, object> message)
	{
		Puck puck = (Puck)message["puck"];
		if (!NetworkManager.Singleton.IsClient)
		{
			return;
		}
		this.goal.Client_RemoveNetClothSphereCollider(puck.NetSphereCollider);
	}

	// Token: 0x0400001A RID: 26
	private Goal goal;
}
