using System;
using System.Collections.Generic;
using Unity.Netcode;

// Token: 0x02000072 RID: 114
public class PlayerPositionManagerController : NetworkBehaviour
{
	// Token: 0x06000311 RID: 785 RVA: 0x00008E29 File Offset: 0x00007029
	private void Awake()
	{
		this.playerPositionManager = base.GetComponent<PlayerPositionManager>();
	}

	// Token: 0x06000312 RID: 786 RVA: 0x00008E37 File Offset: 0x00007037
	private void Start()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnLevelStarted", new Action<Dictionary<string, object>>(this.Event_OnLevelStarted));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnPositionSelectClickPosition", new Action<Dictionary<string, object>>(this.Event_Client_OnPositionSelectClickPosition));
	}

	// Token: 0x06000313 RID: 787 RVA: 0x00008E6F File Offset: 0x0000706F
	public override void OnDestroy()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnLevelStarted", new Action<Dictionary<string, object>>(this.Event_OnLevelStarted));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnPositionSelectClickPosition", new Action<Dictionary<string, object>>(this.Event_Client_OnPositionSelectClickPosition));
		base.OnDestroy();
	}

	// Token: 0x06000314 RID: 788 RVA: 0x00018750 File Offset: 0x00016950
	private void Event_OnLevelStarted(Dictionary<string, object> message)
	{
		List<PlayerPosition> bluePositions = (List<PlayerPosition>)message["playerBluePositions"];
		List<PlayerPosition> redPositions = (List<PlayerPosition>)message["playerRedPositions"];
		this.playerPositionManager.SetBluePositions(bluePositions);
		this.playerPositionManager.SetRedPositions(redPositions);
	}

	// Token: 0x06000315 RID: 789 RVA: 0x00018798 File Offset: 0x00016998
	private void Event_Client_OnPositionSelectClickPosition(Dictionary<string, object> message)
	{
		PlayerPosition playerPosition = (PlayerPosition)message["playerPosition"];
		this.playerPositionManager.Client_ClaimPositionRpc(new NetworkObjectReference(playerPosition.NetworkObject), default(RpcParams));
	}

	// Token: 0x06000317 RID: 791 RVA: 0x0000F5B8 File Offset: 0x0000D7B8
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x06000318 RID: 792 RVA: 0x00006C33 File Offset: 0x00004E33
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x06000319 RID: 793 RVA: 0x00008EAD File Offset: 0x000070AD
	protected internal override string __getTypeName()
	{
		return "PlayerPositionManagerController";
	}

	// Token: 0x040001B0 RID: 432
	private PlayerPositionManager playerPositionManager;
}
