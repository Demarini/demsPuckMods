using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using UnityEngine;

// Token: 0x02000053 RID: 83
public class ModManagerV2 : MonoBehaviourSingleton<ModManagerV2>
{
	// Token: 0x17000039 RID: 57
	// (get) Token: 0x0600025C RID: 604 RVA: 0x000086C1 File Offset: 0x000068C1
	[HideInInspector]
	public ulong[] InstalledModIds
	{
		get
		{
			return (from mod in this.Mods
			select mod.InstalledItem.Id).ToArray<ulong>();
		}
	}

	// Token: 0x1700003A RID: 58
	// (get) Token: 0x0600025D RID: 605 RVA: 0x00016F48 File Offset: 0x00015148
	[HideInInspector]
	public ulong[] EnabledModIds
	{
		get
		{
			return (from mod in this.Mods
			where mod.IsEnabled
			select mod.InstalledItem.Id).ToArray<ulong>();
		}
	}

	// Token: 0x1700003B RID: 59
	// (get) Token: 0x0600025E RID: 606 RVA: 0x00016FA8 File Offset: 0x000151A8
	[HideInInspector]
	public ulong[] DisabledModIds
	{
		get
		{
			return (from mod in this.Mods
			where !mod.IsEnabled
			select mod.InstalledItem.Id).ToArray<ulong>();
		}
	}

	// Token: 0x1700003C RID: 60
	// (get) Token: 0x0600025F RID: 607 RVA: 0x000086F2 File Offset: 0x000068F2
	[HideInInspector]
	public ulong[] PendingModIds
	{
		get
		{
			return (from pm in this.pendingMods
			select pm.Id).ToArray<ulong>();
		}
	}

	// Token: 0x1700003D RID: 61
	// (get) Token: 0x06000260 RID: 608 RVA: 0x00008723 File Offset: 0x00006923
	private string pluginsPath
	{
		get
		{
			return Path.Combine(Path.GetFullPath("."), "Plugins");
		}
	}

	// Token: 0x06000261 RID: 609 RVA: 0x00017008 File Offset: 0x00015208
	public void LoadPlugins()
	{
		Debug.Log("[ModManagerV2] Loading plugins from " + this.pluginsPath);
		if (!Directory.Exists(this.pluginsPath))
		{
			Directory.CreateDirectory(this.pluginsPath);
		}
		foreach (string path in Directory.GetDirectories(this.pluginsPath))
		{
			InstalledItem installedItem = new InstalledItem((ulong)((long)this.Mods.Count), path);
			this.AddMod(installedItem, true);
			string fileName = Path.GetFileName(path);
			installedItem.ItemDetails = new ItemDetails
			{
				Title = fileName
			};
		}
	}

	// Token: 0x06000262 RID: 610 RVA: 0x00017098 File Offset: 0x00015298
	public void AddMod(InstalledItem installedItem, bool isPlugin = false)
	{
		Debug.Log(string.Format("[ModManagerV2] Adding mod {0} from {1}", installedItem.Id, installedItem.Path));
		Mod mod = new Mod(this, installedItem);
		mod.IsPlugin = isPlugin;
		this.Mods.Add(mod);
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnModAdded", new Dictionary<string, object>
		{
			{
				"mod",
				mod
			}
		});
	}

	// Token: 0x06000263 RID: 611 RVA: 0x00017100 File Offset: 0x00015300
	public void RemoveMod(InstalledItem installedItem)
	{
		Debug.Log(string.Format("[ModManagerV2] Removing mod {0}", installedItem.Id));
		Mod modByInstalledItem = this.GetModByInstalledItem(installedItem);
		if (modByInstalledItem == null)
		{
			return;
		}
		modByInstalledItem.Dispose();
		this.Mods.Remove(modByInstalledItem);
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnModRemoved", new Dictionary<string, object>
		{
			{
				"mod",
				modByInstalledItem
			}
		});
	}

	// Token: 0x06000264 RID: 612 RVA: 0x00017168 File Offset: 0x00015368
	public Mod GetModByInstalledItem(InstalledItem installedItem)
	{
		return this.Mods.Find((Mod m) => m.InstalledItem == installedItem);
	}

	// Token: 0x06000265 RID: 613 RVA: 0x0001719C File Offset: 0x0001539C
	public Mod GetModById(ulong id)
	{
		return this.Mods.Find((Mod m) => m.InstalledItem.Id == id);
	}

	// Token: 0x06000266 RID: 614 RVA: 0x000171D0 File Offset: 0x000153D0
	public void SetPendingMods(ulong[] ids)
	{
		Debug.Log("[ModManagerV2] Setting pending mods: " + string.Join<ulong>(", ", ids));
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnBeforePendingModsSet", new Dictionary<string, object>
		{
			{
				"ids",
				ids
			}
		});
		foreach (ulong id in ids)
		{
			Mod modById = this.GetModById(id);
			if (modById == null || !modById.IsEnabled)
			{
				this.pendingMods.Add(new PendingMod(id, modById));
			}
		}
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnPendingModsSet", new Dictionary<string, object>
		{
			{
				"pendingMods",
				this.pendingMods.ToArray()
			}
		});
	}

	// Token: 0x06000267 RID: 615 RVA: 0x00008739 File Offset: 0x00006939
	public void ResetPendingMods(string reason = null)
	{
		this.pendingMods.Clear();
		Debug.Log("[ModManagerV2] Pending mods reset: " + reason);
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnPendingModsReset", new Dictionary<string, object>
		{
			{
				"reason",
				reason
			}
		});
	}

	// Token: 0x06000268 RID: 616 RVA: 0x0001727C File Offset: 0x0001547C
	public void RemovePendingMod(ulong id)
	{
		PendingMod pendingMod = this.pendingMods.Find((PendingMod pm) => pm.Id == id);
		if (pendingMod == null)
		{
			return;
		}
		this.pendingMods.Remove(pendingMod);
		Debug.Log(string.Format("[ModManagerV2] Pending mod {0} removed", id));
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnPendingModRemoved", new Dictionary<string, object>
		{
			{
				"pendingMod",
				pendingMod
			}
		});
		if (this.pendingMods.Count == 0)
		{
			Debug.Log("[ModManagerV2] Pending mods cleared");
			MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnPendingModsCleared", null);
		}
	}

	// Token: 0x06000269 RID: 617 RVA: 0x00017320 File Offset: 0x00015520
	public void LoadModsState()
	{
		string @string = PlayerPrefs.GetString("modsState", null);
		if (string.IsNullOrEmpty(@string))
		{
			this.SaveModsState();
			this.LoadModsState();
			return;
		}
		this.ModsState = JsonSerializer.Deserialize<Dictionary<ulong, bool>>(@string, null);
	}

	// Token: 0x0600026A RID: 618 RVA: 0x0001735C File Offset: 0x0001555C
	public void VerifyModsState(ulong[] installedItemIds)
	{
		(from modState in this.ModsState
		where !installedItemIds.Contains(modState.Key)
		select modState).ToList<KeyValuePair<ulong, bool>>().ForEach(delegate(KeyValuePair<ulong, bool> modState)
		{
			this.ModsState.Remove(modState.Key);
		});
		this.SaveModsState();
	}

	// Token: 0x0600026B RID: 619 RVA: 0x000173B0 File Offset: 0x000155B0
	public void SetModsToState()
	{
		foreach (Mod mod in this.Mods)
		{
			bool modState = this.GetModState(mod.InstalledItem.Id);
			if (!mod.IsPlugin)
			{
				if (modState)
				{
					mod.Enable(false);
				}
				else
				{
					mod.Disable(false);
				}
			}
		}
	}

	// Token: 0x0600026C RID: 620 RVA: 0x0001742C File Offset: 0x0001562C
	public void SaveModsState()
	{
		string value = JsonSerializer.Serialize<Dictionary<ulong, bool>>(this.ModsState, new JsonSerializerOptions
		{
			WriteIndented = true
		});
		PlayerPrefs.SetString("modsState", value);
	}

	// Token: 0x0600026D RID: 621 RVA: 0x00008776 File Offset: 0x00006976
	public void SetModState(ulong id, bool state)
	{
		if (this.ModsState.ContainsKey(id))
		{
			this.ModsState[id] = state;
		}
		else
		{
			this.ModsState.Add(id, state);
		}
		this.SaveModsState();
	}

	// Token: 0x0600026E RID: 622 RVA: 0x000087A8 File Offset: 0x000069A8
	public bool GetModState(ulong id)
	{
		if (!this.ModsState.ContainsKey(id))
		{
			this.ModsState.Add(id, false);
			this.SaveModsState();
		}
		return this.ModsState[id];
	}

	// Token: 0x0600026F RID: 623 RVA: 0x0001745C File Offset: 0x0001565C
	private void OnApplicationQuit()
	{
		foreach (Mod mod in this.Mods)
		{
			mod.Dispose();
		}
		this.Mods.Clear();
	}

	// Token: 0x04000163 RID: 355
	[HideInInspector]
	public List<Mod> Mods = new List<Mod>();

	// Token: 0x04000164 RID: 356
	public Dictionary<ulong, bool> ModsState = new Dictionary<ulong, bool>();

	// Token: 0x04000165 RID: 357
	public List<PendingMod> pendingMods = new List<PendingMod>();
}
