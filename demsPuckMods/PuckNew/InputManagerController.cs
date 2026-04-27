using System;
using System.Collections.Generic;

// Token: 0x020000B6 RID: 182
public static class InputManagerController
{
	// Token: 0x06000593 RID: 1427 RVA: 0x0001EAE8 File Offset: 0x0001CCE8
	public static void Initialize()
	{
		if (!ApplicationManager.IsDedicatedGameServer)
		{
			InputManager.LoadKeyBinds();
		}
		InputManagerController.AddSettingsEventListeners();
	}

	// Token: 0x06000594 RID: 1428 RVA: 0x0001EAFB File Offset: 0x0001CCFB
	public static void Dispose()
	{
		InputManagerController.RemoveSettingsEventListeners();
		if (!ApplicationManager.IsDedicatedGameServer)
		{
			InputManager.SaveKeyBinds();
		}
	}

	// Token: 0x06000595 RID: 1429 RVA: 0x0001EB10 File Offset: 0x0001CD10
	private static void AddSettingsEventListeners()
	{
		EventManager.AddEventListener("Event_OnSettingsKeyBindInputClicked", new Action<Dictionary<string, object>>(InputManagerController.Event_OnSettingsKeyBindInputClicked));
		EventManager.AddEventListener("Event_OnSettingsKeyBindInputInteractionChanged", new Action<Dictionary<string, object>>(InputManagerController.Event_OnSettingsKeyBindInputInteractionChanged));
		EventManager.AddEventListener("Event_OnPopupClickOk", new Action<Dictionary<string, object>>(InputManagerController.Event_OnPopupClickOk));
	}

	// Token: 0x06000596 RID: 1430 RVA: 0x0001EB60 File Offset: 0x0001CD60
	private static void RemoveSettingsEventListeners()
	{
		EventManager.RemoveEventListener("Event_OnSettingsKeyBindInputClicked", new Action<Dictionary<string, object>>(InputManagerController.Event_OnSettingsKeyBindInputClicked));
		EventManager.RemoveEventListener("Event_OnSettingsKeyBindInputInteractionChanged", new Action<Dictionary<string, object>>(InputManagerController.Event_OnSettingsKeyBindInputInteractionChanged));
		EventManager.RemoveEventListener("Event_OnPopupClickOk", new Action<Dictionary<string, object>>(InputManagerController.Event_OnPopupClickOk));
	}

	// Token: 0x06000597 RID: 1431 RVA: 0x0001EBAF File Offset: 0x0001CDAF
	private static void Event_OnSettingsKeyBindInputClicked(Dictionary<string, object> message)
	{
		InputManager.RebindButtonInteractively((string)message["actionName"]);
	}

	// Token: 0x06000598 RID: 1432 RVA: 0x0001EBC8 File Offset: 0x0001CDC8
	private static void Event_OnSettingsKeyBindInputInteractionChanged(Dictionary<string, object> message)
	{
		string actionName = (string)message["actionName"];
		KeyBindInteraction keyBindInteraction = (KeyBindInteraction)message["interaction"];
		InputManager.SetActionInteractions(actionName, Utils.GetInteractionFromKeyBindInteraction(keyBindInteraction));
	}

	// Token: 0x06000599 RID: 1433 RVA: 0x0001EC01 File Offset: 0x0001CE01
	private static void Event_OnPopupClickOk(Dictionary<string, object> message)
	{
		if ((string)message["name"] == "settingsResetToDefault")
		{
			InputManager.ResetToDefault();
		}
	}
}
