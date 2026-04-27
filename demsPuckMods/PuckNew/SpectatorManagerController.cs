using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200012C RID: 300
public class SpectatorManagerController : MonoBehaviour
{
	// Token: 0x060008C2 RID: 2242 RVA: 0x0002A668 File Offset: 0x00028868
	private void Awake()
	{
		this.spectatorManager = base.GetComponent<SpectatorManager>();
		EventManager.AddEventListener("Event_OnSpectatorPositionSpawned", new Action<Dictionary<string, object>>(this.Event_OnSpectatorPositionSpawned));
		EventManager.AddEventListener("Event_OnSpectatorPositionDespawned", new Action<Dictionary<string, object>>(this.Event_OnSpectatorPositionDespawned));
		EventManager.AddEventListener("Event_Everyone_OnPuckSpawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPuckSpawned));
		EventManager.AddEventListener("Event_Everyone_OnGameStateChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnGameStateChanged));
	}

	// Token: 0x060008C3 RID: 2243 RVA: 0x0002A6DC File Offset: 0x000288DC
	private void OnDestroy()
	{
		EventManager.RemoveEventListener("Event_OnSpectatorPositionSpawned", new Action<Dictionary<string, object>>(this.Event_OnSpectatorPositionSpawned));
		EventManager.RemoveEventListener("Event_OnSpectatorPositionDespawned", new Action<Dictionary<string, object>>(this.Event_OnSpectatorPositionDespawned));
		EventManager.RemoveEventListener("Event_Everyone_OnPuckSpawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPuckSpawned));
		EventManager.RemoveEventListener("Event_Everyone_OnGameStateChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnGameStateChanged));
	}

	// Token: 0x060008C4 RID: 2244 RVA: 0x0002A744 File Offset: 0x00028944
	private void Event_OnSpectatorPositionSpawned(Dictionary<string, object> message)
	{
		SpectatorPosition position = (SpectatorPosition)message["spectatorPosition"];
		if (ApplicationManager.IsDedicatedGameServer)
		{
			return;
		}
		this.spectatorManager.RegisterSpectatorPosition(position);
	}

	// Token: 0x060008C5 RID: 2245 RVA: 0x0002A778 File Offset: 0x00028978
	private void Event_OnSpectatorPositionDespawned(Dictionary<string, object> message)
	{
		SpectatorPosition position = (SpectatorPosition)message["spectatorPosition"];
		if (ApplicationManager.IsDedicatedGameServer)
		{
			return;
		}
		this.spectatorManager.UnregisterSpectatorPosition(position);
	}

	// Token: 0x060008C6 RID: 2246 RVA: 0x0002A7AC File Offset: 0x000289AC
	private void Event_Everyone_OnPuckSpawned(Dictionary<string, object> message)
	{
		Puck puck = (Puck)message["puck"];
		this.spectatorManager.SetSpectatorLookTarget(puck.transform);
	}

	// Token: 0x060008C7 RID: 2247 RVA: 0x0002A7DC File Offset: 0x000289DC
	private void Event_Everyone_OnGameStateChanged(Dictionary<string, object> message)
	{
		ref GameState ptr = (GameState)message["oldGameState"];
		GameState gameState = (GameState)message["newGameState"];
		if (ptr.Phase == gameState.Phase)
		{
			return;
		}
		GamePhase phase = gameState.Phase;
		if (phase - GamePhase.BlueScore <= 1)
		{
			this.spectatorManager.SetSpectatorAnimation("Cheering");
			return;
		}
		this.spectatorManager.SetSpectatorAnimation("Seated");
	}

	// Token: 0x04000526 RID: 1318
	private SpectatorManager spectatorManager;
}
