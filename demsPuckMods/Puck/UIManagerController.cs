using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000C1 RID: 193
public class UIManagerController : NetworkBehaviourSingleton<UIManagerController>
{
	// Token: 0x060005C3 RID: 1475 RVA: 0x0000A8D3 File Offset: 0x00008AD3
	public override void Awake()
	{
		base.Awake();
		this.uiManager = base.GetComponent<UIManager>();
	}

	// Token: 0x060005C4 RID: 1476 RVA: 0x00023BAC File Offset: 0x00021DAC
	private void Start()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnSocialClickDiscord", new Action<Dictionary<string, object>>(this.Event_Client_OnSocialClickDiscord));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnSocialClickPatreon", new Action<Dictionary<string, object>>(this.Event_Client_OnSocialClickPatreon));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnTeamSelectClickTeamBlue", new Action<Dictionary<string, object>>(this.Event_Client_OnTeamSelectClickTeamBlue));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnTeamSelectClickTeamRed", new Action<Dictionary<string, object>>(this.Event_Client_OnTeamSelectClickTeamRed));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnTeamSelectClickTeamSpectator", new Action<Dictionary<string, object>>(this.Event_Client_OnTeamSelectClickTeamSpectator));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnUserInterfaceScaleChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnUserInterfaceScaleChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnUserInterfaceSelect", new Action<Dictionary<string, object>>(this.Event_Client_OnUserInterfaceSelect));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnUserInterfaceClick", new Action<Dictionary<string, object>>(this.Event_Client_OnUserInterfaceClick));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnUserInterfaceNotification", new Action<Dictionary<string, object>>(this.Event_Client_OnUserInterfaceNotification));
	}

	// Token: 0x060005C5 RID: 1477 RVA: 0x00023CAC File Offset: 0x00021EAC
	public override void OnDestroy()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnSocialClickDiscord", new Action<Dictionary<string, object>>(this.Event_Client_OnSocialClickDiscord));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnSocialClickPatreon", new Action<Dictionary<string, object>>(this.Event_Client_OnSocialClickPatreon));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnTeamSelectClickTeamBlue", new Action<Dictionary<string, object>>(this.Event_Client_OnTeamSelectClickTeamBlue));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnTeamSelectClickTeamRed", new Action<Dictionary<string, object>>(this.Event_Client_OnTeamSelectClickTeamRed));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnTeamSelectClickTeamSpectator", new Action<Dictionary<string, object>>(this.Event_Client_OnTeamSelectClickTeamSpectator));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnUserInterfaceScaleChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnUserInterfaceScaleChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnUserInterfaceSelect", new Action<Dictionary<string, object>>(this.Event_Client_OnUserInterfaceSelect));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnUserInterfaceClick", new Action<Dictionary<string, object>>(this.Event_Client_OnUserInterfaceClick));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnUserInterfaceNotification", new Action<Dictionary<string, object>>(this.Event_Client_OnUserInterfaceNotification));
		base.OnDestroy();
	}

	// Token: 0x060005C6 RID: 1478 RVA: 0x0000A8E7 File Offset: 0x00008AE7
	private void Event_Client_OnSocialClickDiscord(Dictionary<string, object> message)
	{
		Application.OpenURL("https://discord.gg/AZDBj6XsGg");
	}

	// Token: 0x060005C7 RID: 1479 RVA: 0x0000A8F3 File Offset: 0x00008AF3
	private void Event_Client_OnSocialClickPatreon(Dictionary<string, object> message)
	{
		Application.OpenURL("https://www.patreon.com/c/PuckGame");
	}

	// Token: 0x060005C8 RID: 1480 RVA: 0x00023DB4 File Offset: 0x00021FB4
	private void Event_Client_OnTeamSelectClickTeamBlue(Dictionary<string, object> message)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnPlayerSelectTeam", new Dictionary<string, object>
		{
			{
				"clientId",
				base.NetworkManager.LocalClientId
			},
			{
				"team",
				PlayerTeam.Blue
			}
		});
	}

	// Token: 0x060005C9 RID: 1481 RVA: 0x00023E04 File Offset: 0x00022004
	private void Event_Client_OnTeamSelectClickTeamRed(Dictionary<string, object> message)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnPlayerSelectTeam", new Dictionary<string, object>
		{
			{
				"clientId",
				base.NetworkManager.LocalClientId
			},
			{
				"team",
				PlayerTeam.Red
			}
		});
	}

	// Token: 0x060005CA RID: 1482 RVA: 0x00023E54 File Offset: 0x00022054
	private void Event_Client_OnTeamSelectClickTeamSpectator(Dictionary<string, object> message)
	{
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnPlayerSelectTeam", new Dictionary<string, object>
		{
			{
				"clientId",
				base.NetworkManager.LocalClientId
			},
			{
				"team",
				PlayerTeam.Spectator
			}
		});
	}

	// Token: 0x060005CB RID: 1483 RVA: 0x00023EA4 File Offset: 0x000220A4
	private void Event_Client_OnUserInterfaceScaleChanged(Dictionary<string, object> message)
	{
		float uiScale = (float)message["value"];
		this.uiManager.SetUiScale(uiScale);
	}

	// Token: 0x060005CC RID: 1484 RVA: 0x0000A8FF File Offset: 0x00008AFF
	private void Event_Client_OnUserInterfaceSelect(Dictionary<string, object> message)
	{
		this.uiManager.PlayerSelectSound();
	}

	// Token: 0x060005CD RID: 1485 RVA: 0x0000A90C File Offset: 0x00008B0C
	private void Event_Client_OnUserInterfaceClick(Dictionary<string, object> message)
	{
		this.uiManager.PlayerClickSound();
	}

	// Token: 0x060005CE RID: 1486 RVA: 0x0000A919 File Offset: 0x00008B19
	private void Event_Client_OnUserInterfaceNotification(Dictionary<string, object> message)
	{
		this.uiManager.PlayerNotificationSound();
	}

	// Token: 0x060005D0 RID: 1488 RVA: 0x00023ED0 File Offset: 0x000220D0
	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	// Token: 0x060005D1 RID: 1489 RVA: 0x0000A92E File Offset: 0x00008B2E
	protected override void __initializeRpcs()
	{
		base.__initializeRpcs();
	}

	// Token: 0x060005D2 RID: 1490 RVA: 0x0000A938 File Offset: 0x00008B38
	protected internal override string __getTypeName()
	{
		return "UIManagerController";
	}

	// Token: 0x04000338 RID: 824
	private UIManager uiManager;
}
