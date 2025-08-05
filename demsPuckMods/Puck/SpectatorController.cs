using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000032 RID: 50
public class SpectatorController : MonoBehaviour
{
	// Token: 0x0600015D RID: 349 RVA: 0x00007C0F File Offset: 0x00005E0F
	private void Awake()
	{
		this.spectator = base.GetComponent<Spectator>();
	}

	// Token: 0x0600015E RID: 350 RVA: 0x00007C1D File Offset: 0x00005E1D
	private void Start()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPuckSpawned", new Action<Dictionary<string, object>>(this.Event_OnPuckSpawned));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnGamePhaseChanged", new Action<Dictionary<string, object>>(this.Event_OnGamePhaseChanged));
	}

	// Token: 0x0600015F RID: 351 RVA: 0x00007C55 File Offset: 0x00005E55
	private void OnDestroy()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPuckSpawned", new Action<Dictionary<string, object>>(this.Event_OnPuckSpawned));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnGamePhaseChanged", new Action<Dictionary<string, object>>(this.Event_OnGamePhaseChanged));
	}

	// Token: 0x06000160 RID: 352 RVA: 0x00012888 File Offset: 0x00010A88
	private void Event_OnPuckSpawned(Dictionary<string, object> message)
	{
		Puck puck = (Puck)message["puck"];
		this.spectator.LookTarget = puck.transform;
	}

	// Token: 0x06000161 RID: 353 RVA: 0x000128B8 File Offset: 0x00010AB8
	private void Event_OnGamePhaseChanged(Dictionary<string, object> message)
	{
		GamePhase gamePhase = (GamePhase)message["newGamePhase"];
		if (gamePhase - GamePhase.BlueScore <= 1)
		{
			this.spectator.PlayAnimation("Cheering", UnityEngine.Random.Range(0f, 0.25f));
			return;
		}
		if (gamePhase != GamePhase.GameOver)
		{
			this.spectator.PlayAnimation("Seated", UnityEngine.Random.Range(0f, 0.25f));
			return;
		}
		this.spectator.PlayAnimation("Cheering", UnityEngine.Random.Range(0f, 0.25f));
	}

	// Token: 0x040000BE RID: 190
	private Spectator spectator;
}
