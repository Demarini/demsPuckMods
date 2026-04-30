using System;
using System.Collections.Generic;

// Token: 0x020001A3 RID: 419
public class UIModsController : UIViewController<UIMods>
{
	// Token: 0x06000C09 RID: 3081 RVA: 0x00038E38 File Offset: 0x00037038
	public override void Awake()
	{
		base.Awake();
		this.uiMods = base.GetComponent<UIMods>();
		EventManager.AddEventListener("Event_OnModAdded", new Action<Dictionary<string, object>>(this.Event_OnModAdded));
		EventManager.AddEventListener("Event_OnModChanged", new Action<Dictionary<string, object>>(this.Event_OnModChanged));
		EventManager.AddEventListener("Event_OnModRemoved", new Action<Dictionary<string, object>>(this.Event_OnModRemoved));
	}

	// Token: 0x06000C0A RID: 3082 RVA: 0x00038E9C File Offset: 0x0003709C
	public override void OnDestroy()
	{
		EventManager.RemoveEventListener("Event_OnModAdded", new Action<Dictionary<string, object>>(this.Event_OnModAdded));
		EventManager.RemoveEventListener("Event_OnModChanged", new Action<Dictionary<string, object>>(this.Event_OnModChanged));
		EventManager.RemoveEventListener("Event_OnModRemoved", new Action<Dictionary<string, object>>(this.Event_OnModRemoved));
		base.OnDestroy();
	}

	// Token: 0x06000C0B RID: 3083 RVA: 0x00038EF4 File Offset: 0x000370F4
	private void Event_OnModAdded(Dictionary<string, object> message)
	{
		Mod mod = (Mod)message["mod"];
		this.uiMods.AddMod(mod);
	}

	// Token: 0x06000C0C RID: 3084 RVA: 0x00038F20 File Offset: 0x00037120
	private void Event_OnModChanged(Dictionary<string, object> message)
	{
		Mod mod = (Mod)message["mod"];
		this.uiMods.UpdateMod(mod);
	}

	// Token: 0x06000C0D RID: 3085 RVA: 0x00038F4C File Offset: 0x0003714C
	private void Event_OnModRemoved(Dictionary<string, object> message)
	{
		Mod mod = (Mod)message["mod"];
		this.uiMods.RemoveMod(mod);
	}

	// Token: 0x04000715 RID: 1813
	private UIMods uiMods;
}
