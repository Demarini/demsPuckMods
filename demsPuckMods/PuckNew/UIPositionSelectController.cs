using System;
using System.Collections.Generic;
using Unity.Netcode;

// Token: 0x020001BD RID: 445
internal class UIPositionSelectController : UIViewController<UIPositionSelect>
{
	// Token: 0x06000CCD RID: 3277 RVA: 0x0003C444 File Offset: 0x0003A644
	public override void Awake()
	{
		base.Awake();
		this.uiPositionSelect = base.GetComponent<UIPositionSelect>();
		EventManager.AddEventListener("Event_Everyone_OnPlayerPositionSpawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerPositionSpawned));
		EventManager.AddEventListener("Event_Everyone_OnPlayerPositionDespawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerPositionDespawned));
		EventManager.AddEventListener("Event_Everyone_OnPlayerPositionClaimedByPlayerChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerPositionClaimedByPlayerChanged));
		EventManager.AddEventListener("Event_Everyone_OnPlayerGameStateChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerGameStateChanged));
	}

	// Token: 0x06000CCE RID: 3278 RVA: 0x0003C4BC File Offset: 0x0003A6BC
	public override void OnDestroy()
	{
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerPositionSpawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerPositionSpawned));
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerPositionDespawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerPositionDespawned));
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerPositionClaimedByPlayerChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerPositionClaimedByPlayerChanged));
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerGameStateChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerGameStateChanged));
		base.OnDestroy();
	}

	// Token: 0x06000CCF RID: 3279 RVA: 0x0003C528 File Offset: 0x0003A728
	private void Event_Everyone_OnPlayerPositionSpawned(Dictionary<string, object> message)
	{
		PlayerPosition playerPosition = (PlayerPosition)message["playerPosition"];
		this.uiPositionSelect.AddPosition(playerPosition);
	}

	// Token: 0x06000CD0 RID: 3280 RVA: 0x0003C554 File Offset: 0x0003A754
	private void Event_Everyone_OnPlayerPositionDespawned(Dictionary<string, object> message)
	{
		PlayerPosition playerPosition = (PlayerPosition)message["playerPosition"];
		this.uiPositionSelect.RemovePosition(playerPosition);
	}

	// Token: 0x06000CD1 RID: 3281 RVA: 0x0003C580 File Offset: 0x0003A780
	private void Event_Everyone_OnPlayerPositionClaimedByPlayerChanged(Dictionary<string, object> message)
	{
		PlayerPosition playerPosition = (PlayerPosition)message["playerPosition"];
		this.uiPositionSelect.StylePosition(playerPosition);
	}

	// Token: 0x06000CD2 RID: 3282 RVA: 0x0003C5AC File Offset: 0x0003A7AC
	private void Event_Everyone_OnPlayerGameStateChanged(Dictionary<string, object> message)
	{
		NetworkBehaviour networkBehaviour = (Player)message["player"];
		PlayerGameState playerGameState = (PlayerGameState)message["oldGameState"];
		PlayerGameState playerGameState2 = (PlayerGameState)message["newGameState"];
		if (!networkBehaviour.IsLocalPlayer)
		{
			return;
		}
		if (playerGameState.Team == playerGameState2.Team)
		{
			return;
		}
		this.uiPositionSelect.Team = playerGameState2.Team;
	}

	// Token: 0x04000791 RID: 1937
	private UIPositionSelect uiPositionSelect;
}
