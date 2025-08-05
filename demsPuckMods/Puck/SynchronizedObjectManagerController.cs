using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000BA RID: 186
public class SynchronizedObjectManagerController : MonoBehaviour
{
	// Token: 0x0600058D RID: 1421 RVA: 0x0000A767 File Offset: 0x00008967
	private void Awake()
	{
		this.synchronizedObjectManager = base.GetComponent<SynchronizedObjectManager>();
	}

	// Token: 0x0600058E RID: 1422 RVA: 0x00022EBC File Offset: 0x000210BC
	private void Start()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnSynchronizedObjectSpawned", new Action<Dictionary<string, object>>(this.Event_OnSynchronizedObjectSpawned));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnSynchronizedObjectDespawned", new Action<Dictionary<string, object>>(this.Event_OnSynchronizedObjectDespawned));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerSpawned", new Action<Dictionary<string, object>>(this.Event_OnPlayerSpawned));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerDespawned", new Action<Dictionary<string, object>>(this.Event_OnPlayerDespawned));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Server_OnSynchronizeComplete", new Action<Dictionary<string, object>>(this.Event_Server_OnSynchronizeComplete));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnUseNetworkSmoothingChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnUseNetworkSmoothingChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnNetworkSmoothingStrengthChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnNetworkSmoothingStrengthChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnClientStopped", new Action<Dictionary<string, object>>(this.Event_Client_OnClientStopped));
		this.synchronizedObjectManager.UseNetworkSmoothing = (MonoBehaviourSingleton<SettingsManager>.Instance.UseNetworkSmoothing > 0);
		this.synchronizedObjectManager.NetworkSmoothingStrength = MonoBehaviourSingleton<SettingsManager>.Instance.NetworkSmoothingStrength;
	}

	// Token: 0x0600058F RID: 1423 RVA: 0x00022FD0 File Offset: 0x000211D0
	private void OnDestroy()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnSynchronizedObjectSpawned", new Action<Dictionary<string, object>>(this.Event_OnSynchronizedObjectSpawned));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnSynchronizedObjectDespawned", new Action<Dictionary<string, object>>(this.Event_OnSynchronizedObjectDespawned));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerSpawned", new Action<Dictionary<string, object>>(this.Event_OnPlayerSpawned));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerDespawned", new Action<Dictionary<string, object>>(this.Event_OnPlayerDespawned));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Server_OnSynchronizeComplete", new Action<Dictionary<string, object>>(this.Event_Server_OnSynchronizeComplete));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnUseNetworkSmoothingChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnUseNetworkSmoothingChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnNetworkSmoothingStrengthChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnNetworkSmoothingStrengthChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnClientStopped", new Action<Dictionary<string, object>>(this.Event_Client_OnClientStopped));
		this.synchronizedObjectManager.Dispose();
	}

	// Token: 0x06000590 RID: 1424 RVA: 0x000230C0 File Offset: 0x000212C0
	private void Event_OnSynchronizedObjectSpawned(Dictionary<string, object> message)
	{
		SynchronizedObject synchronizedObject = (SynchronizedObject)message["synchronizedObject"];
		this.synchronizedObjectManager.AddSynchronizedObject(synchronizedObject);
	}

	// Token: 0x06000591 RID: 1425 RVA: 0x000230EC File Offset: 0x000212EC
	private void Event_OnSynchronizedObjectDespawned(Dictionary<string, object> message)
	{
		SynchronizedObject synchronizedObject = (SynchronizedObject)message["synchronizedObject"];
		this.synchronizedObjectManager.RemoveSynchronizedObject(synchronizedObject);
	}

	// Token: 0x06000592 RID: 1426 RVA: 0x00023118 File Offset: 0x00021318
	private void Event_OnPlayerSpawned(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		if (player.OwnerClientId == 0UL)
		{
			return;
		}
		this.synchronizedObjectManager.Server_AddSynchronizedClientId(player.OwnerClientId);
	}

	// Token: 0x06000593 RID: 1427 RVA: 0x00023150 File Offset: 0x00021350
	private void Event_OnPlayerDespawned(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		this.synchronizedObjectManager.Server_RemoveSynchronizedClientId(player.OwnerClientId);
	}

	// Token: 0x06000594 RID: 1428 RVA: 0x00023180 File Offset: 0x00021380
	private void Event_Server_OnSynchronizeComplete(Dictionary<string, object> message)
	{
		ulong clientId = (ulong)message["clientId"];
		this.synchronizedObjectManager.Server_ForceSynchronizeClientId(clientId);
	}

	// Token: 0x06000595 RID: 1429 RVA: 0x000231AC File Offset: 0x000213AC
	private void Event_Client_OnUseNetworkSmoothingChanged(Dictionary<string, object> message)
	{
		bool useNetworkSmoothing = (bool)message["value"];
		this.synchronizedObjectManager.UseNetworkSmoothing = useNetworkSmoothing;
	}

	// Token: 0x06000596 RID: 1430 RVA: 0x000231D8 File Offset: 0x000213D8
	private void Event_Client_OnNetworkSmoothingStrengthChanged(Dictionary<string, object> message)
	{
		float networkSmoothingStrength = (float)message["value"];
		this.synchronizedObjectManager.NetworkSmoothingStrength = networkSmoothingStrength;
	}

	// Token: 0x06000597 RID: 1431 RVA: 0x0000A775 File Offset: 0x00008975
	private void Event_Client_OnClientStopped(Dictionary<string, object> message)
	{
		this.synchronizedObjectManager.Dispose();
	}

	// Token: 0x04000309 RID: 777
	private SynchronizedObjectManager synchronizedObjectManager;
}
