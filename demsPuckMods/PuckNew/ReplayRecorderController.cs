using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Token: 0x0200010A RID: 266
public class ReplayRecorderController : MonoBehaviour
{
	// Token: 0x06000734 RID: 1844 RVA: 0x00023EF0 File Offset: 0x000220F0
	private void Awake()
	{
		this.replayRecorder = base.GetComponent<ReplayRecorder>();
		EventManager.AddEventListener("Event_Everyone_OnPlayerSpawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerSpawned));
		EventManager.AddEventListener("Event_Everyone_OnPlayerDespawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerDespawned));
		EventManager.AddEventListener("Event_Everyone_OnPlayerBodySpawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerBodySpawned));
		EventManager.AddEventListener("Event_Everyone_OnPlayerBodyDespawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerBodyDespawned));
		EventManager.AddEventListener("Event_Everyone_OnStickSpawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnStickSpawned));
		EventManager.AddEventListener("Event_Everyone_OnStickDespawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnStickDespawned));
		EventManager.AddEventListener("Event_Everyone_OnPuckSpawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPuckSpawned));
		EventManager.AddEventListener("Event_Everyone_OnPuckDespawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPuckDespawned));
	}

	// Token: 0x06000735 RID: 1845 RVA: 0x000020D3 File Offset: 0x000002D3
	private void Start()
	{
	}

	// Token: 0x06000736 RID: 1846 RVA: 0x00023FBC File Offset: 0x000221BC
	private void OnDestroy()
	{
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerSpawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerSpawned));
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerDespawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerDespawned));
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerBodySpawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerBodySpawned));
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerBodyDespawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerBodyDespawned));
		EventManager.RemoveEventListener("Event_Everyone_OnStickSpawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnStickSpawned));
		EventManager.RemoveEventListener("Event_Everyone_OnStickDespawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnStickDespawned));
		EventManager.RemoveEventListener("Event_Everyone_OnPuckSpawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPuckSpawned));
		EventManager.RemoveEventListener("Event_Everyone_OnPuckDespawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPuckDespawned));
	}

	// Token: 0x06000737 RID: 1847 RVA: 0x0002407C File Offset: 0x0002227C
	private void Event_Everyone_OnPlayerSpawned(Dictionary<string, object> message)
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

	// Token: 0x06000738 RID: 1848 RVA: 0x000240C4 File Offset: 0x000222C4
	private void Event_Everyone_OnPlayerDespawned(Dictionary<string, object> message)
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

	// Token: 0x06000739 RID: 1849 RVA: 0x0002410C File Offset: 0x0002230C
	private void Event_Everyone_OnPlayerBodySpawned(Dictionary<string, object> message)
	{
		PlayerBody playerBody = (PlayerBody)message["playerBody"];
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (playerBody.Player.IsReplay.Value)
		{
			return;
		}
		this.replayRecorder.Server_AddPlayerBodySpawnedEvent(playerBody);
	}

	// Token: 0x0600073A RID: 1850 RVA: 0x00024158 File Offset: 0x00022358
	private void Event_Everyone_OnPlayerBodyDespawned(Dictionary<string, object> message)
	{
		PlayerBody playerBody = (PlayerBody)message["playerBody"];
		if (!NetworkManager.Singleton.IsServer)
		{
			return;
		}
		if (playerBody.Player.IsReplay.Value)
		{
			return;
		}
		this.replayRecorder.Server_AddPlayerBodyDespawnedEvent(playerBody);
	}

	// Token: 0x0600073B RID: 1851 RVA: 0x000241A4 File Offset: 0x000223A4
	private void Event_Everyone_OnStickSpawned(Dictionary<string, object> message)
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

	// Token: 0x0600073C RID: 1852 RVA: 0x000241F0 File Offset: 0x000223F0
	private void Event_Everyone_OnStickDespawned(Dictionary<string, object> message)
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

	// Token: 0x0600073D RID: 1853 RVA: 0x0002423C File Offset: 0x0002243C
	private void Event_Everyone_OnPuckSpawned(Dictionary<string, object> message)
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

	// Token: 0x0600073E RID: 1854 RVA: 0x00024284 File Offset: 0x00022484
	private void Event_Everyone_OnPuckDespawned(Dictionary<string, object> message)
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

	// Token: 0x04000476 RID: 1142
	private ReplayRecorder replayRecorder;
}
