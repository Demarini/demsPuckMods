using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000015 RID: 21
public class LockerRoomStickController : MonoBehaviour
{
	// Token: 0x0600007B RID: 123 RVA: 0x00003708 File Offset: 0x00001908
	private void Awake()
	{
		this.lockerRoomStick = base.GetComponent<LockerRoomStick>();
		EventManager.AddEventListener("Event_OnTeamChanged", new Action<Dictionary<string, object>>(this.Event_OnTeamChanged));
		EventManager.AddEventListener("Event_OnRoleChanged", new Action<Dictionary<string, object>>(this.Event_OnRoleChanged));
		EventManager.AddEventListener("Event_OnAppearanceClickItem", new Action<Dictionary<string, object>>(this.Event_OnAppearanceClickItem));
		EventManager.AddEventListener("Event_OnAppearanceHide", new Action<Dictionary<string, object>>(this.Event_OnAppearanceHide));
	}

	// Token: 0x0600007C RID: 124 RVA: 0x00003779 File Offset: 0x00001979
	private void Start()
	{
		this.ApplySettings();
	}

	// Token: 0x0600007D RID: 125 RVA: 0x00003784 File Offset: 0x00001984
	private void OnDestroy()
	{
		EventManager.RemoveEventListener("Event_OnTeamChanged", new Action<Dictionary<string, object>>(this.Event_OnTeamChanged));
		EventManager.RemoveEventListener("Event_OnRoleChanged", new Action<Dictionary<string, object>>(this.Event_OnRoleChanged));
		EventManager.RemoveEventListener("Event_OnAppearanceClickItem", new Action<Dictionary<string, object>>(this.Event_OnAppearanceClickItem));
		EventManager.RemoveEventListener("Event_OnAppearanceHide", new Action<Dictionary<string, object>>(this.Event_OnAppearanceHide));
	}

	// Token: 0x0600007E RID: 126 RVA: 0x000037EC File Offset: 0x000019EC
	private void ApplySettings()
	{
		int stickSkinID = SettingsManager.GetStickSkinID(SettingsManager.Team, SettingsManager.Role);
		int stickShaftTapeID = SettingsManager.GetStickShaftTapeID(SettingsManager.Team, SettingsManager.Role);
		int stickBladeTapeID = SettingsManager.GetStickBladeTapeID(SettingsManager.Team, SettingsManager.Role);
		this.lockerRoomStick.ShowRoleStick(SettingsManager.Role);
		this.lockerRoomStick.SetSkinID(stickSkinID, SettingsManager.Team, SettingsManager.Role);
		this.lockerRoomStick.SetShaftTapeID(stickShaftTapeID, SettingsManager.Role);
		this.lockerRoomStick.SetBladeTapeID(stickBladeTapeID, SettingsManager.Role);
	}

	// Token: 0x0600007F RID: 127 RVA: 0x00003779 File Offset: 0x00001979
	private void Event_OnTeamChanged(Dictionary<string, object> message)
	{
		this.ApplySettings();
	}

	// Token: 0x06000080 RID: 128 RVA: 0x00003779 File Offset: 0x00001979
	private void Event_OnRoleChanged(Dictionary<string, object> message)
	{
		this.ApplySettings();
	}

	// Token: 0x06000081 RID: 129 RVA: 0x00003874 File Offset: 0x00001A74
	private void Event_OnAppearanceClickItem(Dictionary<string, object> message)
	{
		Item item = message["item"] as Item;
		AppearanceCategory appearanceCategory = (AppearanceCategory)message["category"];
		AppearanceSubcategory appearanceSubcategory = (AppearanceSubcategory)message["subcategory"];
		PlayerTeam team = (PlayerTeam)message["team"];
		PlayerRole role = (PlayerRole)message["role"];
		switch (appearanceSubcategory)
		{
		case AppearanceSubcategory.StickSkins:
			this.lockerRoomStick.SetSkinID(item.id, team, role);
			return;
		case AppearanceSubcategory.StickShaftTapes:
			this.lockerRoomStick.SetShaftTapeID(item.id, role);
			return;
		case AppearanceSubcategory.StickBladeTapes:
			this.lockerRoomStick.SetBladeTapeID(item.id, role);
			return;
		default:
			return;
		}
	}

	// Token: 0x06000082 RID: 130 RVA: 0x00003779 File Offset: 0x00001979
	private void Event_OnAppearanceHide(Dictionary<string, object> message)
	{
		this.ApplySettings();
	}

	// Token: 0x04000039 RID: 57
	private LockerRoomStick lockerRoomStick;
}
