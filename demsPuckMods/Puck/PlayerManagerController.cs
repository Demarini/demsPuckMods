using System;
using System.Collections.Generic;
using Unity.Netcode;

// Token: 0x02000070 RID: 112
public class PlayerManagerController : NetworkBehaviour
{
	// Token: 0x060002FB RID: 763 RVA: 0x00008D6F File Offset: 0x00006F6F
	private void Awake()
	{
		this.playerManager = base.GetComponent<PlayerManager>();
	}

	// Token: 0x060002FC RID: 764 RVA: 0x000183C4 File Offset: 0x000165C4
	private void Start()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnClientConnected", new Action<Dictionary<string, object>>(this.Event_OnClientConnected));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerSpawned", new Action<Dictionary<string, object>>(this.Event_OnPlayerSpawned));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerDespawned", new Action<Dictionary<string, object>>(this.Event_OnPlayerDespawned));
	}

	// Token: 0x060002FD RID: 765 RVA: 0x00018424 File Offset: 0x00016624
	public override void OnDestroy()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnClientConnected", new Action<Dictionary<string, object>>(this.Event_OnClientConnected));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerSpawned", new Action<Dictionary<string, object>>(this.Event_OnPlayerSpawned));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerDespawned", new Action<Dictionary<string, object>>(this.Event_OnPlayerDespawned));
		base.OnDestroy();
	}

	// Token: 0x060002FE RID: 766 RVA: 0x00018488 File Offset: 0x00016688
	private void Event_OnClientConnected(Dictionary<string, object> message)
	{
		ulong clientId = (ulong)message["clientId"];
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.playerManager.Server_SpawnPlayer(clientId, false);
	}

	// Token: 0x060002FF RID: 767 RVA: 0x000184C0 File Offset: 0x000166C0
	private void Event_OnPlayerSpawned(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		this.playerManager.AddPlayer(player);
	}

	// Token: 0x06000300 RID: 768 RVA: 0x000184EC File Offset: 0x000166EC
	private void Event_OnPlayerDespawned(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		this.playerManager.RemovePlayer(player);
	}

	// Token: 0x06000302 RID: 770 RVA: 0x0000F5B8 File Offset: 0x0000D7B8
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x06000303 RID: 771 RVA: 0x00006C33 File Offset: 0x00004E33
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x06000304 RID: 772 RVA: 0x00008D7D File Offset: 0x00006F7D
	protected internal override string __getTypeName()
	{
		return "PlayerManagerController";
	}

	// Token: 0x040001AD RID: 429
	private PlayerManager playerManager;
}
