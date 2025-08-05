using System;
using System.Collections.Generic;
using Unity.Netcode;

// Token: 0x0200007A RID: 122
public class PuckManagerController : NetworkBehaviour
{
	// Token: 0x06000346 RID: 838 RVA: 0x000090E0 File Offset: 0x000072E0
	private void Awake()
	{
		this.puckManager = base.GetComponent<PuckManager>();
	}

	// Token: 0x06000347 RID: 839 RVA: 0x00018CA4 File Offset: 0x00016EA4
	private void Start()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnLevelStarted", new Action<Dictionary<string, object>>(this.Event_OnLevelStarted));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPuckSpawned", new Action<Dictionary<string, object>>(this.Event_OnPuckSpawned));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPuckDespawned", new Action<Dictionary<string, object>>(this.Event_OnPuckDespawned));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnGamePhaseChanged", new Action<Dictionary<string, object>>(this.Event_OnGamePhaseChanged));
	}

	// Token: 0x06000348 RID: 840 RVA: 0x000090EE File Offset: 0x000072EE
	public override void OnNetworkDespawn()
	{
		this.puckManager.ClearPuckPositions();
		base.OnNetworkDespawn();
	}

	// Token: 0x06000349 RID: 841 RVA: 0x00018D20 File Offset: 0x00016F20
	public override void OnDestroy()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnLevelStarted", new Action<Dictionary<string, object>>(this.Event_OnLevelStarted));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPuckSpawned", new Action<Dictionary<string, object>>(this.Event_OnPuckSpawned));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPuckDespawned", new Action<Dictionary<string, object>>(this.Event_OnPuckDespawned));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnGamePhaseChanged", new Action<Dictionary<string, object>>(this.Event_OnGamePhaseChanged));
		base.OnDestroy();
	}

	// Token: 0x0600034A RID: 842 RVA: 0x00018DA0 File Offset: 0x00016FA0
	private void Event_OnLevelStarted(Dictionary<string, object> message)
	{
		List<PuckPosition> puckPositions = (List<PuckPosition>)message["puckPositions"];
		this.puckManager.SetPuckPositions(puckPositions);
	}

	// Token: 0x0600034B RID: 843 RVA: 0x00018DCC File Offset: 0x00016FCC
	private void Event_OnPuckSpawned(Dictionary<string, object> message)
	{
		Puck puck = (Puck)message["puck"];
		this.puckManager.AddPuck(puck);
	}

	// Token: 0x0600034C RID: 844 RVA: 0x00018DF8 File Offset: 0x00016FF8
	private void Event_OnPuckDespawned(Dictionary<string, object> message)
	{
		Puck puck = (Puck)message["puck"];
		this.puckManager.RemovePuck(puck);
	}

	// Token: 0x0600034D RID: 845 RVA: 0x00018E24 File Offset: 0x00017024
	private void Event_OnGamePhaseChanged(Dictionary<string, object> message)
	{
		GamePhase gamePhase = (GamePhase)message["newGamePhase"];
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (gamePhase - GamePhase.Warmup > 1)
		{
			if (gamePhase == GamePhase.Replay)
			{
				this.puckManager.Server_DespawnPucks(false);
			}
		}
		else
		{
			this.puckManager.Server_DespawnPucks(false);
		}
		this.puckManager.Server_SpawnPucksForPhase(gamePhase);
	}

	// Token: 0x0600034F RID: 847 RVA: 0x0000F5B8 File Offset: 0x0000D7B8
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x06000350 RID: 848 RVA: 0x00006C33 File Offset: 0x00004E33
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x06000351 RID: 849 RVA: 0x00009101 File Offset: 0x00007301
	protected internal override string __getTypeName()
	{
		return "PuckManagerController";
	}

	// Token: 0x040001C1 RID: 449
	private PuckManager puckManager;
}
