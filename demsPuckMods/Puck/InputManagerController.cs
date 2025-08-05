using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000043 RID: 67
public class InputManagerController : MonoBehaviour
{
	// Token: 0x060001E4 RID: 484 RVA: 0x00008189 File Offset: 0x00006389
	private void Awake()
	{
		this.inputManager = base.GetComponent<InputManager>();
	}

	// Token: 0x060001E5 RID: 485 RVA: 0x0001580C File Offset: 0x00013A0C
	private void Start()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnSettingsKeyBindClicked", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsKeyBindClicked));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnSettingsKeyBindTypeChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsKeyBindTypeChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnSettingsResetToDefault", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsResetToDefault));
		this.inputManager.LoadKeyBinds();
	}

	// Token: 0x060001E6 RID: 486 RVA: 0x00015878 File Offset: 0x00013A78
	private void OnDestroy()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnSettingsKeyBindClicked", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsKeyBindClicked));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnSettingsKeyBindTypeChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsKeyBindTypeChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnSettingsResetToDefault", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsResetToDefault));
	}

	// Token: 0x060001E7 RID: 487 RVA: 0x000158D8 File Offset: 0x00013AD8
	private void Event_Client_OnSettingsKeyBindClicked(Dictionary<string, object> message)
	{
		string actionName = (string)message["actionName"];
		this.inputManager.RebindButtonInteractively(actionName);
	}

	// Token: 0x060001E8 RID: 488 RVA: 0x00015904 File Offset: 0x00013B04
	private void Event_Client_OnSettingsKeyBindTypeChanged(Dictionary<string, object> message)
	{
		string text = (string)message["actionName"];
		string humanizedInteraction = (string)message["type"];
		if (!this.inputManager.RebindableInputActions.ContainsKey(text))
		{
			return;
		}
		if (this.inputManager.RebindableInputActions[text].bindings[0].effectiveInteractions == Utils.GetInteractionFromHumanizedInteraction(humanizedInteraction))
		{
			return;
		}
		this.inputManager.SetActionInteractions(text, Utils.GetInteractionFromHumanizedInteraction(humanizedInteraction));
		this.inputManager.SaveKeyBinds();
	}

	// Token: 0x060001E9 RID: 489 RVA: 0x00008197 File Offset: 0x00006397
	private void OnApplicationQuit()
	{
		this.inputManager.SaveKeyBinds();
	}

	// Token: 0x060001EA RID: 490 RVA: 0x000081A4 File Offset: 0x000063A4
	private void Event_Client_OnSettingsResetToDefault(Dictionary<string, object> message)
	{
		this.inputManager.ResetToDefault();
	}

	// Token: 0x04000126 RID: 294
	private InputManager inputManager;
}
