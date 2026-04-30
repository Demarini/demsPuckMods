using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x020000CB RID: 203
public class ModManagerController : MonoBehaviour
{
	// Token: 0x06000618 RID: 1560 RVA: 0x00020068 File Offset: 0x0001E268
	public void Awake()
	{
		this.modManager = base.GetComponent<ModManager>();
		EventManager.AddEventListener("Event_OnInstalledItemAdded", new Action<Dictionary<string, object>>(this.Event_OnInstalledItemAdded));
		EventManager.AddEventListener("Event_OnInstalledItemRemoved", new Action<Dictionary<string, object>>(this.Event_OnInstalledItemRemoved));
		EventManager.AddEventListener("Event_OnModAdded", new Action<Dictionary<string, object>>(this.Event_OnModAdded));
		EventManager.AddEventListener("Event_OnModClickEnable", new Action<Dictionary<string, object>>(this.Event_OnModClickEnable));
		EventManager.AddEventListener("Event_OnModClickDisable", new Action<Dictionary<string, object>>(this.Event_OnModClickDisable));
		EventManager.AddEventListener("Event_OnConnectionRejected", new Action<Dictionary<string, object>>(this.Event_OnConnectionRejected));
		EventManager.AddEventListener("Event_OnDisconnected", new Action<Dictionary<string, object>>(this.Event_OnDisconnected));
		EventManager.AddEventListener("Event_OnModEnableSucceeded", new Action<Dictionary<string, object>>(this.Event_OnModEnableSucceeded));
		EventManager.AddEventListener("Event_OnModEnableFailed", new Action<Dictionary<string, object>>(this.Event_OnModEnableFailed));
		EventManager.AddEventListener("Event_OnModDisableSucceeded", new Action<Dictionary<string, object>>(this.Event_OnModDisableSucceeded));
		EventManager.AddEventListener("Event_OnItemSubscribeFailed", new Action<Dictionary<string, object>>(this.Event_OnItemSubscribeFailed));
		EventManager.AddEventListener("Event_OnItemDownloadFailed", new Action<Dictionary<string, object>>(this.Event_OnItemDownloadFailed));
		EventManager.AddEventListener("Event_OnPendingModsSet", new Action<Dictionary<string, object>>(this.Event_OnPendingModsSet));
		EventManager.AddEventListener("Event_OnPopupClickClose", new Action<Dictionary<string, object>>(this.Event_OnPopupClickClose));
		EventManager.AddEventListener("Event_OnItemIntegrityVerified", new Action<Dictionary<string, object>>(this.Event_OnItemIntegrityVerified));
	}

	// Token: 0x06000619 RID: 1561 RVA: 0x000201CB File Offset: 0x0001E3CB
	private void Start()
	{
		this.modManager.LoadModsState();
		this.modManager.LoadPlugins();
	}

	// Token: 0x0600061A RID: 1562 RVA: 0x000201E4 File Offset: 0x0001E3E4
	private void OnDestroy()
	{
		EventManager.RemoveEventListener("Event_OnInstalledItemAdded", new Action<Dictionary<string, object>>(this.Event_OnInstalledItemAdded));
		EventManager.RemoveEventListener("Event_OnInstalledItemRemoved", new Action<Dictionary<string, object>>(this.Event_OnInstalledItemRemoved));
		EventManager.RemoveEventListener("Event_OnModAdded", new Action<Dictionary<string, object>>(this.Event_OnModAdded));
		EventManager.RemoveEventListener("Event_OnModClickEnable", new Action<Dictionary<string, object>>(this.Event_OnModClickEnable));
		EventManager.RemoveEventListener("Event_OnModClickDisable", new Action<Dictionary<string, object>>(this.Event_OnModClickDisable));
		EventManager.RemoveEventListener("Event_OnConnectionRejected", new Action<Dictionary<string, object>>(this.Event_OnConnectionRejected));
		EventManager.RemoveEventListener("Event_OnDisconnected", new Action<Dictionary<string, object>>(this.Event_OnDisconnected));
		EventManager.RemoveEventListener("Event_OnModEnableSucceeded", new Action<Dictionary<string, object>>(this.Event_OnModEnableSucceeded));
		EventManager.RemoveEventListener("Event_OnModEnableFailed", new Action<Dictionary<string, object>>(this.Event_OnModEnableFailed));
		EventManager.RemoveEventListener("Event_OnModDisableSucceeded", new Action<Dictionary<string, object>>(this.Event_OnModDisableSucceeded));
		EventManager.RemoveEventListener("Event_OnItemSubscribeFailed", new Action<Dictionary<string, object>>(this.Event_OnItemSubscribeFailed));
		EventManager.RemoveEventListener("Event_OnItemDownloadFailed", new Action<Dictionary<string, object>>(this.Event_OnItemDownloadFailed));
		EventManager.RemoveEventListener("Event_OnPendingModsSet", new Action<Dictionary<string, object>>(this.Event_OnPendingModsSet));
		EventManager.RemoveEventListener("Event_OnPopupClickClose", new Action<Dictionary<string, object>>(this.Event_OnPopupClickClose));
		EventManager.RemoveEventListener("Event_OnItemIntegrityVerified", new Action<Dictionary<string, object>>(this.Event_OnItemIntegrityVerified));
	}

	// Token: 0x0600061B RID: 1563 RVA: 0x0002033C File Offset: 0x0001E53C
	private void Event_OnInstalledItemAdded(Dictionary<string, object> message)
	{
		InstalledItem installedItem = (InstalledItem)message["installedItem"];
		this.modManager.AddMod(installedItem, false);
	}

	// Token: 0x0600061C RID: 1564 RVA: 0x00020368 File Offset: 0x0001E568
	private void Event_OnInstalledItemRemoved(Dictionary<string, object> message)
	{
		InstalledItem installedItem = (InstalledItem)message["installedItem"];
		this.modManager.RemoveMod(installedItem);
	}

	// Token: 0x0600061D RID: 1565 RVA: 0x00020394 File Offset: 0x0001E594
	private void Event_OnModAdded(Dictionary<string, object> message)
	{
		Mod mod = (Mod)message["mod"];
		if (ApplicationManager.IsDedicatedGameServer)
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

	// Token: 0x0600061E RID: 1566 RVA: 0x00020413 File Offset: 0x0001E613
	private void Event_OnModClickEnable(Dictionary<string, object> message)
	{
		((Mod)message["mod"]).Enable(true);
	}

	// Token: 0x0600061F RID: 1567 RVA: 0x0002042B File Offset: 0x0001E62B
	private void Event_OnModClickDisable(Dictionary<string, object> message)
	{
		((Mod)message["mod"]).Disable(true);
	}

	// Token: 0x06000620 RID: 1568 RVA: 0x00020444 File Offset: 0x0001E644
	private void Event_OnConnectionRejected(Dictionary<string, object> message)
	{
		ConnectionRejection connectionRejection = (ConnectionRejection)message["connectionRejection"];
		if (connectionRejection.code == ConnectionRejectionCode.MissingMods)
		{
			this.modManager.SetPendingMods(connectionRejection.clientRequiredModIds);
			return;
		}
		this.modManager.SetModsToState();
	}

	// Token: 0x06000621 RID: 1569 RVA: 0x00020488 File Offset: 0x0001E688
	private void Event_OnDisconnected(Dictionary<string, object> message)
	{
		this.modManager.SetModsToState();
	}

	// Token: 0x06000622 RID: 1570 RVA: 0x00020498 File Offset: 0x0001E698
	private void Event_OnModEnableSucceeded(Dictionary<string, object> message)
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

	// Token: 0x06000623 RID: 1571 RVA: 0x00020520 File Offset: 0x0001E720
	private void Event_OnModEnableFailed(Dictionary<string, object> message)
	{
		Mod mod = (Mod)message["mod"];
		if (this.modManager.PendingModIds.Contains(mod.InstalledItem.Id))
		{
			this.modManager.ResetPendingMods(string.Format("Installation failed for {0}", mod.InstalledItem.Id));
		}
	}

	// Token: 0x06000624 RID: 1572 RVA: 0x00020580 File Offset: 0x0001E780
	private void Event_OnModDisableSucceeded(Dictionary<string, object> message)
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

	// Token: 0x06000625 RID: 1573 RVA: 0x000205D4 File Offset: 0x0001E7D4
	private void Event_OnItemSubscribeFailed(Dictionary<string, object> message)
	{
		ulong num = (ulong)message["itemId"];
		if (this.modManager.PendingModIds.Contains(num))
		{
			this.modManager.ResetPendingMods(string.Format("Subscription failed for {0}", num));
		}
	}

	// Token: 0x06000626 RID: 1574 RVA: 0x00020620 File Offset: 0x0001E820
	private void Event_OnItemDownloadFailed(Dictionary<string, object> message)
	{
		ulong num = (ulong)message["itemId"];
		if (this.modManager.PendingModIds.Contains(num))
		{
			this.modManager.ResetPendingMods(string.Format("Download failed for {0}", num));
		}
	}

	// Token: 0x06000627 RID: 1575 RVA: 0x0002066C File Offset: 0x0001E86C
	private void Event_OnPendingModsSet(Dictionary<string, object> message)
	{
		foreach (PendingMod pendingMod in (PendingMod[])message["pendingMods"])
		{
			if (pendingMod.Mod != null && !pendingMod.Mod.IsEnabled)
			{
				pendingMod.Mod.Enable(false);
			}
		}
	}

	// Token: 0x06000628 RID: 1576 RVA: 0x000206BD File Offset: 0x0001E8BD
	private void Event_OnPopupClickClose(Dictionary<string, object> message)
	{
		if ((string)message["name"] != "pendingMods")
		{
			return;
		}
		this.modManager.ResetPendingMods("Installation cancelled");
	}

	// Token: 0x06000629 RID: 1577 RVA: 0x000206EC File Offset: 0x0001E8EC
	private void Event_OnItemIntegrityVerified(Dictionary<string, object> message)
	{
		List<InstalledItem> source = (List<InstalledItem>)message["installedItems"];
		this.modManager.VerifyModsState((from item in source
		select item.Id).ToArray<ulong>());
	}

	// Token: 0x040003C4 RID: 964
	private ModManager modManager;
}
