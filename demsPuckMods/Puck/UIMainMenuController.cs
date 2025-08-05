using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000111 RID: 273
public class UIMainMenuController : MonoBehaviour
{
	// Token: 0x0600099F RID: 2463 RVA: 0x0000D1CE File Offset: 0x0000B3CE
	private void Awake()
	{
		this.uiMainMenu = base.GetComponent<UIMainMenu>();
	}

	// Token: 0x060009A0 RID: 2464 RVA: 0x0000D1DC File Offset: 0x0000B3DC
	private void Start()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnDebugChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnDebugChanged));
	}

	// Token: 0x060009A1 RID: 2465 RVA: 0x0000D1F9 File Offset: 0x0000B3F9
	private void OnDestroy()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnDebugChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnDebugChanged));
	}

	// Token: 0x060009A2 RID: 2466 RVA: 0x0000D216 File Offset: 0x0000B416
	private void Event_Client_OnDebugChanged(Dictionary<string, object> message)
	{
		if ((int)message["value"] > 0)
		{
			this.uiMainMenu.ShowDebugTools();
			return;
		}
		this.uiMainMenu.HideDebugTools();
	}

	// Token: 0x040005C6 RID: 1478
	private UIMainMenu uiMainMenu;
}
