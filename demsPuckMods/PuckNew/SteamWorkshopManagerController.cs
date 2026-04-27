using System;
using System.Collections.Generic;

// Token: 0x02000138 RID: 312
public static class SteamWorkshopManagerController
{
	// Token: 0x06000945 RID: 2373 RVA: 0x0002CB08 File Offset: 0x0002AD08
	public static void Initialize()
	{
		EventManager.AddEventListener("Event_OnSteamConnected", new Action<Dictionary<string, object>>(SteamWorkshopManagerController.Event_OnSteamConnected));
		EventManager.AddEventListener("Event_OnItemDownloadSucceeded", new Action<Dictionary<string, object>>(SteamWorkshopManagerController.Event_OnItemDownloadSucceeded));
		EventManager.AddEventListener("Event_OnPendingModsSet", new Action<Dictionary<string, object>>(SteamWorkshopManagerController.Event_OnPendingModsSet));
		SteamWorkshopManager.VerifyItemIntegrity();
	}

	// Token: 0x06000946 RID: 2374 RVA: 0x0002CB5C File Offset: 0x0002AD5C
	public static void Dispose()
	{
		EventManager.RemoveEventListener("Event_OnSteamConnected", new Action<Dictionary<string, object>>(SteamWorkshopManagerController.Event_OnSteamConnected));
		EventManager.RemoveEventListener("Event_OnItemDownloadSucceeded", new Action<Dictionary<string, object>>(SteamWorkshopManagerController.Event_OnItemDownloadSucceeded));
		EventManager.RemoveEventListener("Event_OnPendingModsSet", new Action<Dictionary<string, object>>(SteamWorkshopManagerController.Event_OnPendingModsSet));
	}

	// Token: 0x06000947 RID: 2375 RVA: 0x0002CBAC File Offset: 0x0002ADAC
	private static void Event_OnSteamConnected(Dictionary<string, object> message)
	{
		if (!ApplicationManager.IsDedicatedGameServer)
		{
			return;
		}
		ulong[] enabledModIds = NetworkBehaviourSingleton<ServerManager>.Instance.EnabledModIds;
		for (int i = 0; i < enabledModIds.Length; i++)
		{
			SteamWorkshopManager.DownloadItem(enabledModIds[i]);
		}
	}

	// Token: 0x06000948 RID: 2376 RVA: 0x0002CBE2 File Offset: 0x0002ADE2
	private static void Event_OnItemDownloadSucceeded(Dictionary<string, object> message)
	{
		SteamWorkshopManager.VerifyItemIntegrity();
	}

	// Token: 0x06000949 RID: 2377 RVA: 0x0002CBEC File Offset: 0x0002ADEC
	private static void Event_OnPendingModsSet(Dictionary<string, object> message)
	{
		foreach (PendingMod pendingMod in (PendingMod[])message["pendingMods"])
		{
			if (pendingMod.Mod == null)
			{
				SteamWorkshopManager.SubscribeItem(pendingMod.Id);
			}
		}
	}
}
