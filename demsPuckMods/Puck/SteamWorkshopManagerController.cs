using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000B4 RID: 180
public class SteamWorkshopManagerController : MonoBehaviour
{
	// Token: 0x0600055D RID: 1373 RVA: 0x0000A632 File Offset: 0x00008832
	private void Awake()
	{
		this.steamWorkshopManager = base.GetComponent<SteamWorkshopManager>();
	}

	// Token: 0x0600055E RID: 1374 RVA: 0x00022098 File Offset: 0x00020298
	private void Start()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnSteamServersConnected", new Action<Dictionary<string, object>>(this.Event_Client_OnSteamServersConnected));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnItemDownloadSucceeded", new Action<Dictionary<string, object>>(this.Event_Client_OnItemDownloadSucceeded));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnPendingModsSet", new Action<Dictionary<string, object>>(this.Event_Client_OnPendingModsSet));
		base.StartCoroutine(this.LateStart());
	}

	// Token: 0x0600055F RID: 1375 RVA: 0x0000A640 File Offset: 0x00008840
	private IEnumerator LateStart()
	{
		yield return new WaitForEndOfFrame();
		this.steamWorkshopManager.VerifyItemIntegrity();
		yield break;
	}

	// Token: 0x06000560 RID: 1376 RVA: 0x00022104 File Offset: 0x00020304
	private void OnDestroy()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnSteamServersConnected", new Action<Dictionary<string, object>>(this.Event_Client_OnSteamServersConnected));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnItemDownloadSucceeded", new Action<Dictionary<string, object>>(this.Event_Client_OnItemDownloadSucceeded));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnPendingModsSet", new Action<Dictionary<string, object>>(this.Event_Client_OnPendingModsSet));
	}

	// Token: 0x06000561 RID: 1377 RVA: 0x00022164 File Offset: 0x00020364
	private void Event_Client_OnSteamServersConnected(Dictionary<string, object> message)
	{
		foreach (ulong itemId in NetworkBehaviourSingleton<ServerManager>.Instance.ServerConfigurationManager.EnabledModIds)
		{
			this.steamWorkshopManager.DownloadItem(itemId);
		}
	}

	// Token: 0x06000562 RID: 1378 RVA: 0x0000A64F File Offset: 0x0000884F
	private void Event_Client_OnItemDownloadSucceeded(Dictionary<string, object> message)
	{
		this.steamWorkshopManager.VerifyItemIntegrity();
	}

	// Token: 0x06000563 RID: 1379 RVA: 0x000221A0 File Offset: 0x000203A0
	private void Event_Client_OnPendingModsSet(Dictionary<string, object> message)
	{
		foreach (PendingMod pendingMod in (PendingMod[])message["pendingMods"])
		{
			if (pendingMod.Mod == null)
			{
				this.steamWorkshopManager.SubscribeItem(pendingMod.Id);
			}
		}
	}

	// Token: 0x040002E8 RID: 744
	private SteamWorkshopManager steamWorkshopManager;
}
