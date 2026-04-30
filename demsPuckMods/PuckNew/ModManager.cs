using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using UnityEngine;

// Token: 0x020000C5 RID: 197
public class ModManager : MonoBehaviourSingleton<ModManager>
{
	// Token: 0x170000A5 RID: 165
	// (get) Token: 0x060005F2 RID: 1522 RVA: 0x0001F93E File Offset: 0x0001DB3E
	[HideInInspector]
	public ulong[] InstalledModIds
	{
		get
		{
			return (from mod in this.Mods
			select mod.InstalledItem.Id).ToArray<ulong>();
		}
	}

	// Token: 0x170000A6 RID: 166
	// (get) Token: 0x060005F3 RID: 1523 RVA: 0x0001F970 File Offset: 0x0001DB70
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

	// Token: 0x170000A7 RID: 167
	// (get) Token: 0x060005F4 RID: 1524 RVA: 0x0001F9D0 File Offset: 0x0001DBD0
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

	// Token: 0x170000A8 RID: 168
	// (get) Token: 0x060005F5 RID: 1525 RVA: 0x0001FA30 File Offset: 0x0001DC30
	[HideInInspector]
	public ulong[] PendingModIds
	{
		get
		{
			return (from pm in this.pendingMods
			select pm.Id).ToArray<ulong>();
		}
	}

	// Token: 0x170000A9 RID: 169
	// (get) Token: 0x060005F6 RID: 1526 RVA: 0x0001FA61 File Offset: 0x0001DC61
	private string pluginsPath
	{
		get
		{
			return Path.Combine(Path.GetFullPath("."), "Plugins");
		}
	}

	// Token: 0x060005F7 RID: 1527 RVA: 0x0001FA78 File Offset: 0x0001DC78
	private void OnDestroy()
	{
		foreach (Mod mod in this.Mods)
		{
			mod.Dispose();
		}
		this.Mods.Clear();
	}

	// Token: 0x060005F8 RID: 1528 RVA: 0x0001FAD4 File Offset: 0x0001DCD4
	public void LoadPlugins()
	{
		Debug.Log("[ModManager] Loading plugins from " + this.pluginsPath);
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

	// Token: 0x060005F9 RID: 1529 RVA: 0x0001FB64 File Offset: 0x0001DD64
	public void AddMod(InstalledItem installedItem, bool isPlugin = false)
	{
		Debug.Log(string.Format("[ModManager] Adding mod {0} from {1}", installedItem.Id, installedItem.Path));
		Mod mod = new Mod(this, installedItem);
		mod.IsPlugin = isPlugin;
		this.Mods.Add(mod);
		EventManager.TriggerEvent("Event_OnModAdded", new Dictionary<string, object>
		{
			{
				"mod",
				mod
			}
		});
	}

	// Token: 0x060005FA RID: 1530 RVA: 0x0001FBC8 File Offset: 0x0001DDC8
	public void RemoveMod(InstalledItem installedItem)
	{
		Debug.Log(string.Format("[ModManager] Removing mod {0}", installedItem.Id));
		Mod modByInstalledItem = this.GetModByInstalledItem(installedItem);
		if (modByInstalledItem == null)
		{
			return;
		}
		modByInstalledItem.Dispose();
		this.Mods.Remove(modByInstalledItem);
		EventManager.TriggerEvent("Event_OnModRemoved", new Dictionary<string, object>
		{
			{
				"mod",
				modByInstalledItem
			}
		});
	}

	// Token: 0x060005FB RID: 1531 RVA: 0x0001FC2C File Offset: 0x0001DE2C
	public Mod GetModByInstalledItem(InstalledItem installedItem)
	{
		return this.Mods.Find((Mod m) => m.InstalledItem == installedItem);
	}

	// Token: 0x060005FC RID: 1532 RVA: 0x0001FC60 File Offset: 0x0001DE60
	public Mod GetModById(ulong id)
	{
		return this.Mods.Find((Mod m) => m.InstalledItem.Id == id);
	}

	// Token: 0x060005FD RID: 1533 RVA: 0x0001FC94 File Offset: 0x0001DE94
	public void SetPendingMods(ulong[] ids)
	{
		Debug.Log("[ModManager] Setting pending mods: " + string.Join<ulong>(", ", ids));
		EventManager.TriggerEvent("Event_OnBeforePendingModsSet", new Dictionary<string, object>
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
		EventManager.TriggerEvent("Event_OnPendingModsSet", new Dictionary<string, object>
		{
			{
				"pendingMods",
				this.pendingMods.ToArray()
			}
		});
	}

	// Token: 0x060005FE RID: 1534 RVA: 0x0001FD34 File Offset: 0x0001DF34
	public void ResetPendingMods(string reason = null)
	{
		this.pendingMods.Clear();
		Debug.Log("[ModManager] Pending mods reset: " + reason);
		EventManager.TriggerEvent("Event_OnPendingModsReset", new Dictionary<string, object>
		{
			{
				"reason",
				reason
			}
		});
	}

	// Token: 0x060005FF RID: 1535 RVA: 0x0001FD6C File Offset: 0x0001DF6C
	public void RemovePendingMod(ulong id)
	{
		PendingMod pendingMod = this.pendingMods.Find((PendingMod pm) => pm.Id == id);
		if (pendingMod == null)
		{
			return;
		}
		this.pendingMods.Remove(pendingMod);
		Debug.Log(string.Format("[ModManager] Pending mod {0} removed", id));
		EventManager.TriggerEvent("Event_OnPendingModRemoved", new Dictionary<string, object>
		{
			{
				"pendingMod",
				pendingMod
			}
		});
		if (this.pendingMods.Count == 0)
		{
			Debug.Log("[ModManager] Pending mods cleared");
			EventManager.TriggerEvent("Event_OnPendingModsCleared", null);
		}
	}

	// Token: 0x06000600 RID: 1536 RVA: 0x0001FE08 File Offset: 0x0001E008
	public void LoadModsState()
	{
		string @string = SaveManager.GetString("modsState", null);
		if (string.IsNullOrEmpty(@string))
		{
			this.SaveModsState();
			this.LoadModsState();
			return;
		}
		this.ModsState = JsonSerializer.Deserialize<Dictionary<ulong, bool>>(@string, null);
	}

	// Token: 0x06000601 RID: 1537 RVA: 0x0001FE44 File Offset: 0x0001E044
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

	// Token: 0x06000602 RID: 1538 RVA: 0x0001FE98 File Offset: 0x0001E098
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

	// Token: 0x06000603 RID: 1539 RVA: 0x0001FF14 File Offset: 0x0001E114
	public void SaveModsState()
	{
		string value = JsonSerializer.Serialize<Dictionary<ulong, bool>>(this.ModsState, new JsonSerializerOptions
		{
			WriteIndented = true
		});
		SaveManager.SetString("modsState", value);
	}

	// Token: 0x06000604 RID: 1540 RVA: 0x0001FF44 File Offset: 0x0001E144
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

	// Token: 0x06000605 RID: 1541 RVA: 0x0001FF76 File Offset: 0x0001E176
	public bool GetModState(ulong id)
	{
		if (!this.ModsState.ContainsKey(id))
		{
			this.ModsState.Add(id, false);
			this.SaveModsState();
		}
		return this.ModsState[id];
	}

	// Token: 0x040003B5 RID: 949
	[HideInInspector]
	public List<Mod> Mods = new List<Mod>();

	// Token: 0x040003B6 RID: 950
	public Dictionary<ulong, bool> ModsState = new Dictionary<ulong, bool>();

	// Token: 0x040003B7 RID: 951
	public List<PendingMod> pendingMods = new List<PendingMod>();
}
