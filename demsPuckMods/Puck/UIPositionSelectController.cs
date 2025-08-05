using System;
using System.Collections.Generic;
using Unity.Netcode;

// Token: 0x0200012B RID: 299
internal class UIPositionSelectController : NetworkBehaviour
{
	// Token: 0x06000A80 RID: 2688 RVA: 0x0000DB1B File Offset: 0x0000BD1B
	private void Awake()
	{
		this.uiPositionSelect = base.GetComponent<UIPositionSelect>();
	}

	// Token: 0x06000A81 RID: 2689 RVA: 0x0000DB29 File Offset: 0x0000BD29
	private void Start()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerTeamChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerTeamChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerPositionClaimedByChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerPositionClaimedByChanged));
	}

	// Token: 0x06000A82 RID: 2690 RVA: 0x0000DB61 File Offset: 0x0000BD61
	public override void OnNetworkDespawn()
	{
		this.uiPositionSelect.ClearPositions();
		base.OnNetworkDespawn();
	}

	// Token: 0x06000A83 RID: 2691 RVA: 0x0000DB74 File Offset: 0x0000BD74
	public override void OnDestroy()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerTeamChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerTeamChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerPositionClaimedByChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerPositionClaimedByChanged));
	}

	// Token: 0x06000A84 RID: 2692 RVA: 0x0003CEC4 File Offset: 0x0003B0C4
	private void Event_OnPlayerTeamChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		if (!player.IsLocalPlayer)
		{
			return;
		}
		this.uiPositionSelect.ClearPositions();
		PlayerTeam value = player.Team.Value;
		if (value == PlayerTeam.Blue)
		{
			NetworkBehaviourSingleton<PlayerPositionManager>.Instance.BluePositions.ForEach(delegate(PlayerPosition playerPosition)
			{
				this.uiPositionSelect.AddPosition(playerPosition);
			});
			return;
		}
		if (value != PlayerTeam.Red)
		{
			return;
		}
		NetworkBehaviourSingleton<PlayerPositionManager>.Instance.RedPositions.ForEach(delegate(PlayerPosition playerPosition)
		{
			this.uiPositionSelect.AddPosition(playerPosition);
		});
	}

	// Token: 0x06000A85 RID: 2693 RVA: 0x0003CF44 File Offset: 0x0003B144
	private void Event_OnPlayerPositionClaimedByChanged(Dictionary<string, object> message)
	{
		PlayerPosition playerPosition = (PlayerPosition)message["playerPosition"];
		this.uiPositionSelect.UpdatePosition(playerPosition);
	}

	// Token: 0x06000A89 RID: 2697 RVA: 0x0000F5B8 File Offset: 0x0000D7B8
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x06000A8A RID: 2698 RVA: 0x00006C33 File Offset: 0x00004E33
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x06000A8B RID: 2699 RVA: 0x0000DBBA File Offset: 0x0000BDBA
	protected internal override string __getTypeName()
	{
		return "UIPositionSelectController";
	}

	// Token: 0x04000627 RID: 1575
	private UIPositionSelect uiPositionSelect;
}
