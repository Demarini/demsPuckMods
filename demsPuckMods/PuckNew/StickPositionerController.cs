using System;
using System.Collections.Generic;
using Unity.Netcode;

// Token: 0x02000053 RID: 83
public class StickPositionerController : NetworkBehaviour
{
	// Token: 0x060002DC RID: 732 RVA: 0x0001251E File Offset: 0x0001071E
	private void Awake()
	{
		this.stickPositioner = base.GetComponent<StickPositioner>();
	}

	// Token: 0x060002DD RID: 733 RVA: 0x0001252C File Offset: 0x0001072C
	public override void OnNetworkSpawn()
	{
		EventManager.AddEventListener("Event_Everyone_OnStickSpawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnStickSpawned));
		EventManager.AddEventListener("Event_Everyone_OnPlayerHandednessChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerHandednessChanged));
		base.OnNetworkSpawn();
	}

	// Token: 0x060002DE RID: 734 RVA: 0x00012560 File Offset: 0x00010760
	public override void OnNetworkDespawn()
	{
		EventManager.RemoveEventListener("Event_Everyone_OnStickSpawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnStickSpawned));
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerHandednessChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerHandednessChanged));
		base.OnNetworkDespawn();
	}

	// Token: 0x060002DF RID: 735 RVA: 0x00012594 File Offset: 0x00010794
	private void Event_Everyone_OnStickSpawned(Dictionary<string, object> message)
	{
		Stick stick = (Stick)message["stick"];
		if (base.OwnerClientId == stick.OwnerClientId)
		{
			this.stickPositioner.PrepareShaftTarget(stick);
		}
	}

	// Token: 0x060002E0 RID: 736 RVA: 0x000125CC File Offset: 0x000107CC
	private void Event_Everyone_OnPlayerHandednessChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		if (base.OwnerClientId == player.OwnerClientId)
		{
			this.stickPositioner.Handedness = player.Handedness.Value;
		}
	}

	// Token: 0x060002E2 RID: 738 RVA: 0x00012610 File Offset: 0x00010810
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x060002E3 RID: 739 RVA: 0x000021BE File Offset: 0x000003BE
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x060002E4 RID: 740 RVA: 0x00012626 File Offset: 0x00010826
	protected internal override string __getTypeName()
	{
		return "StickPositionerController";
	}

	// Token: 0x04000202 RID: 514
	private StickPositioner stickPositioner;
}
