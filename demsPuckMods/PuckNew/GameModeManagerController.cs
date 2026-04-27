using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Token: 0x020000A7 RID: 167
public class GameModeManagerController : MonoBehaviour
{
	// Token: 0x0600054A RID: 1354 RVA: 0x0001D12C File Offset: 0x0001B32C
	private void Awake()
	{
		this.gameModeManager = base.GetComponent<GameModeManager>();
		EventManager.AddEventListener("Event_Server_OnServerStarted", new Action<Dictionary<string, object>>(this.Event_Server_OnServerStarted));
		EventManager.AddEventListener("Event_Server_OnServerStopped", new Action<Dictionary<string, object>>(this.Event_Server_OnServerStopped));
		EventManager.AddEventListener("Event_Server_OnLoadSceneEventCompleted", new Action<Dictionary<string, object>>(this.Event_Server_OnLoadSceneEventCompleted));
		EventManager.AddEventListener("Event_Everyone_OnLevelSpawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnLevelSpawned));
		EventManager.AddEventListener("Event_Everyone_OnLevelDespawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnLevelDespawned));
	}

	// Token: 0x0600054B RID: 1355 RVA: 0x0001D1B4 File Offset: 0x0001B3B4
	private void OnDestroy()
	{
		EventManager.RemoveEventListener("Event_Server_OnServerStarted", new Action<Dictionary<string, object>>(this.Event_Server_OnServerStarted));
		EventManager.RemoveEventListener("Event_Server_OnServerStopped", new Action<Dictionary<string, object>>(this.Event_Server_OnServerStopped));
		EventManager.RemoveEventListener("Event_Server_OnLoadSceneEventCompleted", new Action<Dictionary<string, object>>(this.Event_Server_OnLoadSceneEventCompleted));
		EventManager.RemoveEventListener("Event_Everyone_OnLevelSpawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnLevelSpawned));
		EventManager.RemoveEventListener("Event_Everyone_OnLevelDespawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnLevelDespawned));
	}

	// Token: 0x0600054C RID: 1356 RVA: 0x0001D230 File Offset: 0x0001B430
	private void Event_Server_OnServerStarted(Dictionary<string, object> message)
	{
		ServerConfig serverConfig = (ServerConfig)message["serverConfig"];
		this.gameModeManager.SelectGameMode(serverConfig.gameMode);
	}

	// Token: 0x0600054D RID: 1357 RVA: 0x0001D25F File Offset: 0x0001B45F
	private void Event_Server_OnServerStopped(Dictionary<string, object> message)
	{
		this.gameModeManager.DisableSelectedGameMode();
		this.gameModeManager.DeselectGameMode();
	}

	// Token: 0x0600054E RID: 1358 RVA: 0x0001D277 File Offset: 0x0001B477
	private void Event_Server_OnLoadSceneEventCompleted(Dictionary<string, object> message)
	{
		this.gameModeManager.EnableSelectedGameMode();
	}

	// Token: 0x0600054F RID: 1359 RVA: 0x0001D284 File Offset: 0x0001B484
	private void Event_Everyone_OnLevelSpawned(Dictionary<string, object> message)
	{
		Level level = (Level)message["level"];
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.gameModeManager.Level = level;
	}

	// Token: 0x06000550 RID: 1360 RVA: 0x0001D2BB File Offset: 0x0001B4BB
	private void Event_Everyone_OnLevelDespawned(Dictionary<string, object> message)
	{
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		this.gameModeManager.Level = null;
	}

	// Token: 0x0400033F RID: 831
	private GameModeManager gameModeManager;
}
