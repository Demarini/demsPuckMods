using System;
using System.Collections.Generic;
using Unity.Netcode;

// Token: 0x0200000E RID: 14
public class GoalController : NetworkBehaviour
{
	// Token: 0x06000076 RID: 118 RVA: 0x0000708A File Offset: 0x0000528A
	private void Awake()
	{
		this.goal = base.GetComponent<Goal>();
	}

	// Token: 0x06000077 RID: 119 RVA: 0x00010980 File Offset: 0x0000EB80
	private void Start()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPuckSpawned", new Action<Dictionary<string, object>>(this.Event_OnPuckSpawned));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPuckDespawned", new Action<Dictionary<string, object>>(this.Event_OnPuckDespawned));
		NetworkBehaviourSingleton<PuckManager>.Instance.GetPucks(false).ForEach(delegate(Puck puck)
		{
			this.goal.Client_AddNetClothSphereCollider(puck.NetSphereCollider);
		});
	}

	// Token: 0x06000078 RID: 120 RVA: 0x00007098 File Offset: 0x00005298
	public override void OnNetworkSpawn()
	{
		this.goal.NetCloth.enabled = NetworkManager.Singleton.IsClient;
		base.OnNetworkSpawn();
	}

	// Token: 0x06000079 RID: 121 RVA: 0x000070BA File Offset: 0x000052BA
	public override void OnDestroy()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPuckSpawned", new Action<Dictionary<string, object>>(this.Event_OnPuckSpawned));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPuckDespawned", new Action<Dictionary<string, object>>(this.Event_OnPuckDespawned));
		base.OnDestroy();
	}

	// Token: 0x0600007A RID: 122 RVA: 0x000109E0 File Offset: 0x0000EBE0
	private void Event_OnPuckSpawned(Dictionary<string, object> message)
	{
		Puck puck = (Puck)message["puck"];
		if (!NetworkManager.Singleton.IsClient)
		{
			return;
		}
		this.goal.Client_AddNetClothSphereCollider(puck.NetSphereCollider);
	}

	// Token: 0x0600007B RID: 123 RVA: 0x00010A1C File Offset: 0x0000EC1C
	private void Event_OnPuckDespawned(Dictionary<string, object> message)
	{
		Puck puck = (Puck)message["puck"];
		if (!NetworkManager.Singleton.IsClient)
		{
			return;
		}
		this.goal.Client_RemoveNetClothSphereCollider(puck.NetSphereCollider);
	}

	// Token: 0x0600007E RID: 126 RVA: 0x0000F5B8 File Offset: 0x0000D7B8
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x0600007F RID: 127 RVA: 0x00006C33 File Offset: 0x00004E33
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x06000080 RID: 128 RVA: 0x0000710B File Offset: 0x0000530B
	protected internal override string __getTypeName()
	{
		return "GoalController";
	}

	// Token: 0x04000036 RID: 54
	private Goal goal;
}
