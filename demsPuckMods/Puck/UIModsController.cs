using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000116 RID: 278
public class UIModsController : MonoBehaviour
{
	// Token: 0x060009E0 RID: 2528 RVA: 0x0000D4DE File Offset: 0x0000B6DE
	private void Awake()
	{
		this.uiMods = base.GetComponent<UIMods>();
	}

	// Token: 0x060009E1 RID: 2529 RVA: 0x0003AFB8 File Offset: 0x000391B8
	private void Start()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnModAdded", new Action<Dictionary<string, object>>(this.Event_Client_OnModAdded));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnModChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnModChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnModRemoved", new Action<Dictionary<string, object>>(this.Event_Client_OnModRemoved));
		this.uiMods.ClearMods();
	}

	// Token: 0x060009E2 RID: 2530 RVA: 0x0003B024 File Offset: 0x00039224
	private void OnDestroy()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnModAdded", new Action<Dictionary<string, object>>(this.Event_Client_OnModAdded));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnModChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnModChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnModRemoved", new Action<Dictionary<string, object>>(this.Event_Client_OnModRemoved));
	}

	// Token: 0x060009E3 RID: 2531 RVA: 0x0003B084 File Offset: 0x00039284
	private void Event_Client_OnModAdded(Dictionary<string, object> message)
	{
		Mod mod = (Mod)message["mod"];
		this.uiMods.AddMod(mod);
	}

	// Token: 0x060009E4 RID: 2532 RVA: 0x0003B0B0 File Offset: 0x000392B0
	private void Event_Client_OnModChanged(Dictionary<string, object> message)
	{
		Mod mod = (Mod)message["mod"];
		Debug.Log(string.Format("[UIModsController] Mod changed, updating mod {0}", mod.InstalledItem.Id));
		this.uiMods.UpdateMod(mod);
	}

	// Token: 0x060009E5 RID: 2533 RVA: 0x0003B0FC File Offset: 0x000392FC
	private void Event_Client_OnModRemoved(Dictionary<string, object> message)
	{
		Mod mod = (Mod)message["mod"];
		this.uiMods.RemoveMod(mod);
	}

	// Token: 0x040005DE RID: 1502
	private UIMods uiMods;
}
