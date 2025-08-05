using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000FE RID: 254
public class UIAppearanceController : MonoBehaviour
{
	// Token: 0x060008C4 RID: 2244 RVA: 0x0000C66C File Offset: 0x0000A86C
	private void Awake()
	{
		this.uiAppearance = base.GetComponent<UIAppearance>();
	}

	// Token: 0x060008C5 RID: 2245 RVA: 0x00036D4C File Offset: 0x00034F4C
	private void Start()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnChangingRoomTeamChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnChangingRoomTeamChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnChangingRoomRoleChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnChangingRoomRoleChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnAppearanceClickClose", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceClickClose));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnOwnedItemIdsUpdated", new Action<Dictionary<string, object>>(this.Event_Client_OnOwnedItemIdsUpdated));
		this.uiAppearance.ApplyAppearanceValues();
	}

	// Token: 0x060008C6 RID: 2246 RVA: 0x00036DD0 File Offset: 0x00034FD0
	private void OnDestroy()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnChangingRoomTeamChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnChangingRoomTeamChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnChangingRoomRoleChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnChangingRoomRoleChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnAppearanceClickClose", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceClickClose));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnOwnedItemIdsUpdated", new Action<Dictionary<string, object>>(this.Event_Client_OnOwnedItemIdsUpdated));
	}

	// Token: 0x060008C7 RID: 2247 RVA: 0x00036E4C File Offset: 0x0003504C
	private void Event_Client_OnChangingRoomTeamChanged(Dictionary<string, object> message)
	{
		PlayerTeam team = (PlayerTeam)message["team"];
		this.uiAppearance.Team = team;
		this.uiAppearance.Reload();
		this.uiAppearance.ApplyAppearanceValues();
	}

	// Token: 0x060008C8 RID: 2248 RVA: 0x00036E8C File Offset: 0x0003508C
	private void Event_Client_OnChangingRoomRoleChanged(Dictionary<string, object> message)
	{
		PlayerRole role = (PlayerRole)message["role"];
		this.uiAppearance.Role = role;
		this.uiAppearance.Reload();
		this.uiAppearance.ApplyAppearanceValues();
	}

	// Token: 0x060008C9 RID: 2249 RVA: 0x0000C67A File Offset: 0x0000A87A
	private void Event_Client_OnAppearanceClickClose(Dictionary<string, object> message)
	{
		this.uiAppearance.Reload();
		this.uiAppearance.ApplyAppearanceValues();
	}

	// Token: 0x060008CA RID: 2250 RVA: 0x00036ECC File Offset: 0x000350CC
	private void Event_Client_OnOwnedItemIdsUpdated(Dictionary<string, object> message)
	{
		int[] ownedItemIds = (int[])message["ownedItemIds"];
		this.uiAppearance.SetOwnedItemIds(ownedItemIds);
	}

	// Token: 0x04000560 RID: 1376
	private UIAppearance uiAppearance;
}
