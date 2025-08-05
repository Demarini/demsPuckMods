using System;
using System.Collections.Generic;
using Unity.Netcode;

// Token: 0x02000091 RID: 145
public class ReplayRecorderController : NetworkBehaviour
{
	// Token: 0x06000395 RID: 917 RVA: 0x0000941A File Offset: 0x0000761A
	private void Awake()
	{
		this.replayRecorder = base.GetComponent<ReplayRecorder>();
	}

	// Token: 0x06000396 RID: 918 RVA: 0x0001A958 File Offset: 0x00018B58
	private void Start()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerSpawned", new Action<Dictionary<string, object>>(this.Event_OnPlayerSpawned));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerDespawned", new Action<Dictionary<string, object>>(this.Event_OnPlayerDespawned));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerBodySpawned", new Action<Dictionary<string, object>>(this.Event_OnPlayerBodySpawned));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerBodyDespawned", new Action<Dictionary<string, object>>(this.Event_OnPlayerBodyDespawned));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnStickSpawned", new Action<Dictionary<string, object>>(this.Event_OnStickSpawned));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnStickDespawned", new Action<Dictionary<string, object>>(this.Event_OnStickDespawned));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPuckSpawned", new Action<Dictionary<string, object>>(this.Event_OnPuckSpawned));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPuckDespawned", new Action<Dictionary<string, object>>(this.Event_OnPuckDespawned));
	}

	// Token: 0x06000397 RID: 919 RVA: 0x0001AA40 File Offset: 0x00018C40
	public override void OnDestroy()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerSpawned", new Action<Dictionary<string, object>>(this.Event_OnPlayerSpawned));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerDespawned", new Action<Dictionary<string, object>>(this.Event_OnPlayerDespawned));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerBodySpawned", new Action<Dictionary<string, object>>(this.Event_OnPlayerBodySpawned));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerBodyDespawned", new Action<Dictionary<string, object>>(this.Event_OnPlayerBodyDespawned));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnStickSpawned", new Action<Dictionary<string, object>>(this.Event_OnStickSpawned));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnStickDespawned", new Action<Dictionary<string, object>>(this.Event_OnStickDespawned));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPuckSpawned", new Action<Dictionary<string, object>>(this.Event_OnPuckSpawned));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPuckDespawned", new Action<Dictionary<string, object>>(this.Event_OnPuckDespawned));
		base.OnDestroy();
	}

	// Token: 0x06000398 RID: 920 RVA: 0x0001AB2C File Offset: 0x00018D2C
	private void Event_OnPlayerSpawned(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (player.IsReplay.Value)
		{
			return;
		}
		this.replayRecorder.Server_AddPlayerSpawnedEvent(player);
	}

	// Token: 0x06000399 RID: 921 RVA: 0x0001AB74 File Offset: 0x00018D74
	private void Event_OnPlayerDespawned(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (player.IsReplay.Value)
		{
			return;
		}
		this.replayRecorder.Server_AddPlayerDespawnedEvent(player);
	}

	// Token: 0x0600039A RID: 922 RVA: 0x0001ABBC File Offset: 0x00018DBC
	private void Event_OnPlayerBodySpawned(Dictionary<string, object> message)
	{
		PlayerBodyV2 playerBodyV = (PlayerBodyV2)message["playerBody"];
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (!playerBodyV)
		{
			return;
		}
		if (playerBodyV.Player.IsReplay.Value)
		{
			return;
		}
		this.replayRecorder.Server_AddPlayerBodySpawnedEvent(playerBodyV);
	}

	// Token: 0x0600039B RID: 923 RVA: 0x0001AC10 File Offset: 0x00018E10
	private void Event_OnPlayerBodyDespawned(Dictionary<string, object> message)
	{
		PlayerBodyV2 playerBodyV = (PlayerBodyV2)message["playerBody"];
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (playerBodyV.Player.IsReplay.Value)
		{
			return;
		}
		this.replayRecorder.Server_AddPlayerBodyDespawnedEvent(playerBodyV);
	}

	// Token: 0x0600039C RID: 924 RVA: 0x0001AC5C File Offset: 0x00018E5C
	private void Event_OnStickSpawned(Dictionary<string, object> message)
	{
		Stick stick = (Stick)message["stick"];
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (stick.Player.IsReplay.Value)
		{
			return;
		}
		this.replayRecorder.Server_AddStickSpawnedEvent(stick);
	}

	// Token: 0x0600039D RID: 925 RVA: 0x0001ACA8 File Offset: 0x00018EA8
	private void Event_OnStickDespawned(Dictionary<string, object> message)
	{
		Stick stick = (Stick)message["stick"];
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (stick.Player.IsReplay.Value)
		{
			return;
		}
		this.replayRecorder.Server_AddStickDespawnedEvent(stick);
	}

	// Token: 0x0600039E RID: 926 RVA: 0x0001ACF4 File Offset: 0x00018EF4
	private void Event_OnPuckSpawned(Dictionary<string, object> message)
	{
		Puck puck = (Puck)message["puck"];
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (puck.IsReplay.Value)
		{
			return;
		}
		this.replayRecorder.Server_AddPuckSpawnedEvent(puck);
	}

	// Token: 0x0600039F RID: 927 RVA: 0x0001AD3C File Offset: 0x00018F3C
	private void Event_OnPuckDespawned(Dictionary<string, object> message)
	{
		Puck puck = (Puck)message["puck"];
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (puck.IsReplay.Value)
		{
			return;
		}
		this.replayRecorder.Server_AddPuckDespawnedEvent(puck);
	}

	// Token: 0x060003A1 RID: 929 RVA: 0x0000F5B8 File Offset: 0x0000D7B8
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x060003A2 RID: 930 RVA: 0x00006C33 File Offset: 0x00004E33
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x060003A3 RID: 931 RVA: 0x00009428 File Offset: 0x00007628
	protected internal override string __getTypeName()
	{
		return "ReplayRecorderController";
	}

	// Token: 0x04000218 RID: 536
	private ReplayRecorder replayRecorder;
}
