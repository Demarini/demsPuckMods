using System;
using System.Collections.Generic;

// Token: 0x0200019C RID: 412
public class UIMainMenuController : UIViewController<UIMainMenu>
{
	// Token: 0x06000BBC RID: 3004 RVA: 0x000375A1 File Offset: 0x000357A1
	public override void Awake()
	{
		base.Awake();
		this.uiMainMenu = base.GetComponent<UIMainMenu>();
		EventManager.AddEventListener("Event_OnDebugChanged", new Action<Dictionary<string, object>>(this.Event_OnDebugChanged));
	}

	// Token: 0x06000BBD RID: 3005 RVA: 0x000375CB File Offset: 0x000357CB
	private void Start()
	{
		if (SettingsManager.Debug)
		{
			this.uiMainMenu.ShowDebug();
			return;
		}
		this.uiMainMenu.HideDebug();
	}

	// Token: 0x06000BBE RID: 3006 RVA: 0x000375EB File Offset: 0x000357EB
	public override void OnDestroy()
	{
		EventManager.RemoveEventListener("Event_OnDebugChanged", new Action<Dictionary<string, object>>(this.Event_OnDebugChanged));
		base.OnDestroy();
	}

	// Token: 0x06000BBF RID: 3007 RVA: 0x00037609 File Offset: 0x00035809
	private void Event_OnDebugChanged(Dictionary<string, object> message)
	{
		if ((bool)message["value"])
		{
			this.uiMainMenu.ShowDebug();
			return;
		}
		this.uiMainMenu.HideDebug();
	}

	// Token: 0x040006F6 RID: 1782
	private UIMainMenu uiMainMenu;
}
