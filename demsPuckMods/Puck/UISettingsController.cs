using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200013D RID: 317
public class UISettingsController : MonoBehaviour
{
	// Token: 0x06000B3F RID: 2879 RVA: 0x0000E872 File Offset: 0x0000CA72
	private void Awake()
	{
		this.uiSettings = base.GetComponent<UISettings>();
	}

	// Token: 0x06000B40 RID: 2880 RVA: 0x00040784 File Offset: 0x0003E984
	private void Start()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnDisplayIndexChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnDisplayIndexChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnKeyBindsLoaded", new Action<Dictionary<string, object>>(this.Event_Client_OnKeyBindsLoaded));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnSettingsResetToDefault", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsResetToDefault));
		this.uiSettings.ApplySettingsValues();
	}

	// Token: 0x06000B41 RID: 2881 RVA: 0x000407F0 File Offset: 0x0003E9F0
	private void OnDestroy()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnDisplayIndexChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnDisplayIndexChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnKeyBindsLoaded", new Action<Dictionary<string, object>>(this.Event_Client_OnKeyBindsLoaded));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnSettingsResetToDefault", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsResetToDefault));
	}

	// Token: 0x06000B42 RID: 2882 RVA: 0x00040850 File Offset: 0x0003EA50
	private void Event_Client_OnDisplayIndexChanged(Dictionary<string, object> message)
	{
		int index = (int)message["resolutionIndex"];
		string displayStringFromIndex = Utils.GetDisplayStringFromIndex((int)message["displayIndex"]);
		string resolutionStringFromIndex = Utils.GetResolutionStringFromIndex(index);
		this.uiSettings.UpdateDisplayDropdown(displayStringFromIndex);
		this.uiSettings.UpdateResolutionsDropdown(resolutionStringFromIndex);
	}

	// Token: 0x06000B43 RID: 2883 RVA: 0x000408A4 File Offset: 0x0003EAA4
	private void Event_Client_OnKeyBindsLoaded(Dictionary<string, object> message)
	{
		Dictionary<string, KeyBind> keyBinds = (Dictionary<string, KeyBind>)message["keyBinds"];
		this.uiSettings.UpdateKeyBinds(keyBinds);
	}

	// Token: 0x06000B44 RID: 2884 RVA: 0x0000E880 File Offset: 0x0000CA80
	private void Event_Client_OnSettingsResetToDefault(Dictionary<string, object> message)
	{
		this.uiSettings.ApplySettingsValues();
	}

	// Token: 0x040006A4 RID: 1700
	private UISettings uiSettings;
}
