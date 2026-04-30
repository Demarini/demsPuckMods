using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200013D RID: 317
public class SynchronizedObjectManagerController : MonoBehaviour
{
	// Token: 0x0600096C RID: 2412 RVA: 0x0002D9BC File Offset: 0x0002BBBC
	private void Awake()
	{
		this.synchronizedObjectManager = base.GetComponent<SynchronizedObjectManager>();
		EventManager.AddEventListener("Event_OnUseNetworkSmoothingChanged", new Action<Dictionary<string, object>>(this.Event_OnUseNetworkSmoothingChanged));
		EventManager.AddEventListener("Event_OnNetworkSmoothingStrengthChanged", new Action<Dictionary<string, object>>(this.Event_OnNetworkSmoothingStrengthChanged));
		EventManager.AddEventListener("Event_OnClientStopped", new Action<Dictionary<string, object>>(this.Event_OnClientStopped));
		EventManager.AddEventListener("Event_Everyone_OnSynchronizedObjectSpawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnSynchronizedObjectSpawned));
		EventManager.AddEventListener("Event_Everyone_OnSynchronizedObjectDespawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnSynchronizedObjectDespawned));
		EventManager.AddEventListener("Event_Everyone_OnPlayerSpawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerSpawned));
		EventManager.AddEventListener("Event_Everyone_OnPlayerDespawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerDespawned));
		EventManager.AddEventListener("Event_Server_OnServerStarted", new Action<Dictionary<string, object>>(this.Event_Server_OnServerStarted));
		EventManager.AddEventListener("Event_Server_OnClientSceneSynchronizeComplete", new Action<Dictionary<string, object>>(this.Event_Server_OnClientSceneSynchronizeComplete));
	}

	// Token: 0x0600096D RID: 2413 RVA: 0x0002DA9B File Offset: 0x0002BC9B
	private void Start()
	{
		this.synchronizedObjectManager.UseNetworkSmoothing = SettingsManager.UseNetworkSmoothing;
		this.synchronizedObjectManager.NetworkSmoothingStrength = SettingsManager.NetworkSmoothingStrength;
	}

	// Token: 0x0600096E RID: 2414 RVA: 0x0002DAC0 File Offset: 0x0002BCC0
	private void OnDestroy()
	{
		EventManager.RemoveEventListener("Event_OnUseNetworkSmoothingChanged", new Action<Dictionary<string, object>>(this.Event_OnUseNetworkSmoothingChanged));
		EventManager.RemoveEventListener("Event_OnNetworkSmoothingStrengthChanged", new Action<Dictionary<string, object>>(this.Event_OnNetworkSmoothingStrengthChanged));
		EventManager.RemoveEventListener("Event_OnClientStopped", new Action<Dictionary<string, object>>(this.Event_OnClientStopped));
		EventManager.RemoveEventListener("Event_Everyone_OnSynchronizedObjectSpawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnSynchronizedObjectSpawned));
		EventManager.RemoveEventListener("Event_Everyone_OnSynchronizedObjectDespawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnSynchronizedObjectDespawned));
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerSpawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerSpawned));
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerDespawned", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerDespawned));
		EventManager.RemoveEventListener("Event_Server_OnServerStarted", new Action<Dictionary<string, object>>(this.Event_Server_OnServerStarted));
		EventManager.RemoveEventListener("Event_Server_OnClientSceneSynchronizeComplete", new Action<Dictionary<string, object>>(this.Event_Server_OnClientSceneSynchronizeComplete));
	}

	// Token: 0x0600096F RID: 2415 RVA: 0x0002DB94 File Offset: 0x0002BD94
	private void Event_OnUseNetworkSmoothingChanged(Dictionary<string, object> message)
	{
		bool useNetworkSmoothing = (bool)message["value"];
		this.synchronizedObjectManager.UseNetworkSmoothing = useNetworkSmoothing;
	}

	// Token: 0x06000970 RID: 2416 RVA: 0x0002DBC0 File Offset: 0x0002BDC0
	private void Event_OnNetworkSmoothingStrengthChanged(Dictionary<string, object> message)
	{
		int networkSmoothingStrength = (int)message["value"];
		this.synchronizedObjectManager.NetworkSmoothingStrength = networkSmoothingStrength;
	}

	// Token: 0x06000971 RID: 2417 RVA: 0x0002DBEA File Offset: 0x0002BDEA
	private void Event_OnClientStopped(Dictionary<string, object> message)
	{
		this.synchronizedObjectManager.Dispose();
	}

	// Token: 0x06000972 RID: 2418 RVA: 0x0002DBF8 File Offset: 0x0002BDF8
	private void Event_Everyone_OnSynchronizedObjectSpawned(Dictionary<string, object> message)
	{
		SynchronizedObject synchronizedObject = (SynchronizedObject)message["synchronizedObject"];
		this.synchronizedObjectManager.AddSynchronizedObject(synchronizedObject);
	}

	// Token: 0x06000973 RID: 2419 RVA: 0x0002DC24 File Offset: 0x0002BE24
	private void Event_Everyone_OnSynchronizedObjectDespawned(Dictionary<string, object> message)
	{
		SynchronizedObject synchronizedObject = (SynchronizedObject)message["synchronizedObject"];
		this.synchronizedObjectManager.RemoveSynchronizedObject(synchronizedObject);
	}

	// Token: 0x06000974 RID: 2420 RVA: 0x0002DC50 File Offset: 0x0002BE50
	private void Event_Everyone_OnPlayerSpawned(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		if (player.IsReplay.Value)
		{
			return;
		}
		if (player.OwnerClientId == 0UL)
		{
			return;
		}
		this.synchronizedObjectManager.Server_AddSynchronizedClientId(player.OwnerClientId);
	}

	// Token: 0x06000975 RID: 2421 RVA: 0x0002DC98 File Offset: 0x0002BE98
	private void Event_Everyone_OnPlayerDespawned(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		if (player.IsReplay.Value)
		{
			return;
		}
		if (player.OwnerClientId == 0UL)
		{
			return;
		}
		this.synchronizedObjectManager.Server_RemoveSynchronizedClientId(player.OwnerClientId);
	}

	// Token: 0x06000976 RID: 2422 RVA: 0x0002DCE0 File Offset: 0x0002BEE0
	private void Event_Server_OnServerStarted(Dictionary<string, object> message)
	{
		ServerConfig serverConfig = (ServerConfig)message["serverConfig"];
		this.synchronizedObjectManager.TickRate = serverConfig.tickRate;
	}

	// Token: 0x06000977 RID: 2423 RVA: 0x0002DD10 File Offset: 0x0002BF10
	private void Event_Server_OnClientSceneSynchronizeComplete(Dictionary<string, object> message)
	{
		ulong num = (ulong)message["clientId"];
		if (num == 0UL)
		{
			return;
		}
		this.synchronizedObjectManager.Server_ForceSynchronizeClientId(num);
	}

	// Token: 0x04000570 RID: 1392
	private SynchronizedObjectManager synchronizedObjectManager;
}
