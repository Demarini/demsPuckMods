using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x0200004B RID: 75
public class ModManagerControllerV2 : MonoBehaviour
{
	// Token: 0x0600021F RID: 543 RVA: 0x00008494 File Offset: 0x00006694
	public void Awake()
	{
		this.modManager = base.GetComponent<ModManagerV2>();
	}

	// Token: 0x06000220 RID: 544 RVA: 0x000162DC File Offset: 0x000144DC
	private void Start()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnInstalledItemAdded", new Action<Dictionary<string, object>>(this.Event_Client_OnInstalledItemAdded));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnInstalledItemRemoved", new Action<Dictionary<string, object>>(this.Event_Client_OnInstalledItemRemoved));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnModAdded", new Action<Dictionary<string, object>>(this.Event_Client_OnModAdded));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnModClickEnable", new Action<Dictionary<string, object>>(this.Event_Client_OnModClickEnable));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnModClickDisable", new Action<Dictionary<string, object>>(this.Event_Client_OnModClickDisable));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnConnectionRejected", new Action<Dictionary<string, object>>(this.Event_Client_OnConnectionRejected));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnDisconnected", new Action<Dictionary<string, object>>(this.Event_Client_OnDisconnected));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnModEnableSucceeded", new Action<Dictionary<string, object>>(this.Event_Client_OnModEnableSucceeded));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnModEnableFailed", new Action<Dictionary<string, object>>(this.Event_Client_OnModEnableFailed));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnModDisableSucceeded", new Action<Dictionary<string, object>>(this.Event_Client_OnModDisableSucceeded));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnItemSubscribeFailed", new Action<Dictionary<string, object>>(this.Event_Client_OnItemSubscribeFailed));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnItemDownloadFailed", new Action<Dictionary<string, object>>(this.Event_Client_OnItemDownloadFailed));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnPendingModsSet", new Action<Dictionary<string, object>>(this.Event_Client_OnPendingModsSet));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnPopupClickClose", new Action<Dictionary<string, object>>(this.Event_Client_OnPopupClickClose));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnItemIntegrityVerified", new Action<Dictionary<string, object>>(this.Event_Client_OnItemIntegrityVerified));
		this.modManager.LoadModsState();
		base.StartCoroutine(this.LateStart());
	}

	// Token: 0x06000221 RID: 545 RVA: 0x000084A2 File Offset: 0x000066A2
	private IEnumerator LateStart()
	{
		yield return new WaitForEndOfFrame();
		this.modManager.LoadPlugins();
		yield break;
	}

	// Token: 0x06000222 RID: 546 RVA: 0x00016498 File Offset: 0x00014698
	private void OnDestroy()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnInstalledItemAdded", new Action<Dictionary<string, object>>(this.Event_Client_OnInstalledItemAdded));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnInstalledItemRemoved", new Action<Dictionary<string, object>>(this.Event_Client_OnInstalledItemRemoved));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnModAdded", new Action<Dictionary<string, object>>(this.Event_Client_OnModAdded));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnModClickEnable", new Action<Dictionary<string, object>>(this.Event_Client_OnModClickEnable));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnModClickDisable", new Action<Dictionary<string, object>>(this.Event_Client_OnModClickDisable));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnConnectionRejected", new Action<Dictionary<string, object>>(this.Event_Client_OnConnectionRejected));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnDisconnected", new Action<Dictionary<string, object>>(this.Event_Client_OnDisconnected));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnModEnableSucceeded", new Action<Dictionary<string, object>>(this.Event_Client_OnModEnableSucceeded));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnModEnableFailed", new Action<Dictionary<string, object>>(this.Event_Client_OnModEnableFailed));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnModDisableSucceeded", new Action<Dictionary<string, object>>(this.Event_Client_OnModDisableSucceeded));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnItemSubscribeFailed", new Action<Dictionary<string, object>>(this.Event_Client_OnItemSubscribeFailed));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnItemDownloadFailed", new Action<Dictionary<string, object>>(this.Event_Client_OnItemDownloadFailed));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnPendingModsSet", new Action<Dictionary<string, object>>(this.Event_Client_OnPendingModsSet));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnPopupClickClose", new Action<Dictionary<string, object>>(this.Event_Client_OnPopupClickClose));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnItemIntegrityVerified", new Action<Dictionary<string, object>>(this.Event_Client_OnItemIntegrityVerified));
	}

	// Token: 0x06000223 RID: 547 RVA: 0x0001663C File Offset: 0x0001483C
	private void Event_Client_OnInstalledItemAdded(Dictionary<string, object> message)
	{
		InstalledItem installedItem = (InstalledItem)message["installedItem"];
		this.modManager.AddMod(installedItem, false);
	}

	// Token: 0x06000224 RID: 548 RVA: 0x00016668 File Offset: 0x00014868
	private void Event_Client_OnInstalledItemRemoved(Dictionary<string, object> message)
	{
		InstalledItem installedItem = (InstalledItem)message["installedItem"];
		this.modManager.RemoveMod(installedItem);
	}

	// Token: 0x06000225 RID: 549 RVA: 0x00016694 File Offset: 0x00014894
	private void Event_Client_OnModAdded(Dictionary<string, object> message)
	{
		Mod mod = (Mod)message["mod"];
		if (Application.isBatchMode)
		{
			mod.Enable(false);
			return;
		}
		if (mod.IsPlugin)
		{
			mod.Enable(false);
		}
		if (this.modManager.PendingModIds.Contains(mod.InstalledItem.Id))
		{
			mod.Enable(false);
		}
		if (this.modManager.GetModState(mod.InstalledItem.Id))
		{
			mod.Enable(false);
		}
	}

	// Token: 0x06000226 RID: 550 RVA: 0x000084B1 File Offset: 0x000066B1
	private void Event_Client_OnModClickEnable(Dictionary<string, object> message)
	{
		((Mod)message["mod"]).Enable(true);
	}

	// Token: 0x06000227 RID: 551 RVA: 0x000084C9 File Offset: 0x000066C9
	private void Event_Client_OnModClickDisable(Dictionary<string, object> message)
	{
		((Mod)message["mod"]).Disable(true);
	}

	// Token: 0x06000228 RID: 552 RVA: 0x00016714 File Offset: 0x00014914
	private void Event_Client_OnConnectionRejected(Dictionary<string, object> message)
	{
		ConnectionRejection connectionRejection = (ConnectionRejection)message["connectionRejection"];
		if (connectionRejection.code == ConnectionRejectionCode.MissingMods)
		{
			this.modManager.SetPendingMods(connectionRejection.clientRequiredModIds);
			return;
		}
		this.modManager.SetModsToState();
	}

	// Token: 0x06000229 RID: 553 RVA: 0x000084E1 File Offset: 0x000066E1
	private void Event_Client_OnDisconnected(Dictionary<string, object> message)
	{
		this.modManager.SetModsToState();
	}

	// Token: 0x0600022A RID: 554 RVA: 0x00016758 File Offset: 0x00014958
	private void Event_Client_OnModEnableSucceeded(Dictionary<string, object> message)
	{
		Mod mod = (Mod)message["mod"];
		bool flag = (bool)message["isManual"];
		if (mod.IsPlugin)
		{
			return;
		}
		if (flag)
		{
			this.modManager.SetModState(mod.InstalledItem.Id, true);
		}
		if (this.modManager.PendingModIds.Contains(mod.InstalledItem.Id))
		{
			this.modManager.RemovePendingMod(mod.InstalledItem.Id);
		}
	}

	// Token: 0x0600022B RID: 555 RVA: 0x000167E0 File Offset: 0x000149E0
	private void Event_Client_OnModEnableFailed(Dictionary<string, object> message)
	{
		Mod mod = (Mod)message["mod"];
		if (this.modManager.PendingModIds.Contains(mod.InstalledItem.Id))
		{
			this.modManager.ResetPendingMods(string.Format("Installation failed for {0}", mod.InstalledItem.Id));
		}
	}

	// Token: 0x0600022C RID: 556 RVA: 0x00016840 File Offset: 0x00014A40
	private void Event_Client_OnModDisableSucceeded(Dictionary<string, object> message)
	{
		Mod mod = (Mod)message["mod"];
		bool flag = (bool)message["isManual"];
		if (mod.IsPlugin)
		{
			return;
		}
		if (flag)
		{
			this.modManager.SetModState(mod.InstalledItem.Id, false);
		}
	}

	// Token: 0x0600022D RID: 557 RVA: 0x00016894 File Offset: 0x00014A94
	private void Event_Client_OnItemSubscribeFailed(Dictionary<string, object> message)
	{
		ulong num = (ulong)message["itemId"];
		if (this.modManager.PendingModIds.Contains(num))
		{
			this.modManager.ResetPendingMods(string.Format("Subscription failed for {0}", num));
		}
	}

	// Token: 0x0600022E RID: 558 RVA: 0x000168E0 File Offset: 0x00014AE0
	private void Event_Client_OnItemDownloadFailed(Dictionary<string, object> message)
	{
		ulong num = (ulong)message["itemId"];
		if (this.modManager.PendingModIds.Contains(num))
		{
			this.modManager.ResetPendingMods(string.Format("Download failed for {0}", num));
		}
	}

	// Token: 0x0600022F RID: 559 RVA: 0x0001692C File Offset: 0x00014B2C
	private void Event_Client_OnPendingModsSet(Dictionary<string, object> message)
	{
		foreach (PendingMod pendingMod in (PendingMod[])message["pendingMods"])
		{
			if (pendingMod.Mod != null && !pendingMod.Mod.IsEnabled)
			{
				pendingMod.Mod.Enable(false);
			}
		}
	}

	// Token: 0x06000230 RID: 560 RVA: 0x000084EE File Offset: 0x000066EE
	private void Event_Client_OnPopupClickClose(Dictionary<string, object> message)
	{
		if ((string)message["name"] != "pendingMods")
		{
			return;
		}
		this.modManager.ResetPendingMods("Installation cancelled");
	}

	// Token: 0x06000231 RID: 561 RVA: 0x00016980 File Offset: 0x00014B80
	private void Event_Client_OnItemIntegrityVerified(Dictionary<string, object> message)
	{
		List<InstalledItem> source = (List<InstalledItem>)message["installedItems"];
		this.modManager.VerifyModsState((from item in source
		select item.Id).ToArray<ulong>());
	}

	// Token: 0x04000147 RID: 327
	private ModManagerV2 modManager;
}
