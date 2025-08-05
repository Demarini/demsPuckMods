using System;
using System.Collections.Generic;
using Unity.Netcode;

// Token: 0x020000F7 RID: 247
public class StickPositionerController : NetworkBehaviour
{
	// Token: 0x0600087C RID: 2172 RVA: 0x0000C2F6 File Offset: 0x0000A4F6
	private void Awake()
	{
		this.stickPositioner = base.GetComponent<StickPositioner>();
	}

	// Token: 0x0600087D RID: 2173 RVA: 0x0000C304 File Offset: 0x0000A504
	public override void OnNetworkSpawn()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnStickSpawned", new Action<Dictionary<string, object>>(this.Event_OnStickSpawned));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerHandednessChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerHandednessChanged));
		base.OnNetworkSpawn();
	}

	// Token: 0x0600087E RID: 2174 RVA: 0x0000C342 File Offset: 0x0000A542
	public override void OnNetworkDespawn()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnStickSpawned", new Action<Dictionary<string, object>>(this.Event_OnStickSpawned));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerHandednessChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerHandednessChanged));
		base.OnNetworkDespawn();
	}

	// Token: 0x0600087F RID: 2175 RVA: 0x00035408 File Offset: 0x00033608
	private void Event_OnStickSpawned(Dictionary<string, object> message)
	{
		Stick stick = (Stick)message["stick"];
		if (base.OwnerClientId == stick.OwnerClientId)
		{
			this.stickPositioner.PrepareShaftTarget(stick);
		}
	}

	// Token: 0x06000880 RID: 2176 RVA: 0x00035440 File Offset: 0x00033640
	private void Event_OnPlayerHandednessChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		PlayerHandedness handedness = (PlayerHandedness)message["newHandedness"];
		if (base.OwnerClientId == player.OwnerClientId)
		{
			this.stickPositioner.Handedness = handedness;
		}
	}

	// Token: 0x06000882 RID: 2178 RVA: 0x0000F5B8 File Offset: 0x0000D7B8
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x06000883 RID: 2179 RVA: 0x00006C33 File Offset: 0x00004E33
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x06000884 RID: 2180 RVA: 0x0000C380 File Offset: 0x0000A580
	protected internal override string __getTypeName()
	{
		return "StickPositionerController";
	}

	// Token: 0x0400051E RID: 1310
	private StickPositioner stickPositioner;
}
