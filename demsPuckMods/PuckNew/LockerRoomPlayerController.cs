using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000013 RID: 19
public class LockerRoomPlayerController : MonoBehaviour
{
	// Token: 0x06000069 RID: 105 RVA: 0x000032A4 File Offset: 0x000014A4
	private void Awake()
	{
		this.lockerRoomPlayer = base.GetComponent<LockerRoomPlayer>();
		EventManager.AddEventListener("Event_OnTeamChanged", new Action<Dictionary<string, object>>(this.Event_OnTeamChanged));
		EventManager.AddEventListener("Event_OnRoleChanged", new Action<Dictionary<string, object>>(this.Event_OnRoleChanged));
		EventManager.AddEventListener("Event_OnAppearanceClickItem", new Action<Dictionary<string, object>>(this.Event_OnAppearanceClickItem));
		EventManager.AddEventListener("Event_OnAppearanceShow", new Action<Dictionary<string, object>>(this.Event_OnAppearanceShow));
		EventManager.AddEventListener("Event_OnAppearanceHide", new Action<Dictionary<string, object>>(this.Event_OnAppearanceHide));
		EventManager.AddEventListener("Event_OnIdentityShow", new Action<Dictionary<string, object>>(this.Event_OnIdentityShow));
		EventManager.AddEventListener("Event_OnIdentityHide", new Action<Dictionary<string, object>>(this.Event_OnIdentityHide));
		EventManager.AddEventListener("Event_OnPlayerDataChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerDataChanged));
	}

	// Token: 0x0600006A RID: 106 RVA: 0x0000336D File Offset: 0x0000156D
	private void Start()
	{
		this.ApplySettings();
	}

	// Token: 0x0600006B RID: 107 RVA: 0x00003378 File Offset: 0x00001578
	private void OnDestroy()
	{
		EventManager.RemoveEventListener("Event_OnTeamChanged", new Action<Dictionary<string, object>>(this.Event_OnTeamChanged));
		EventManager.RemoveEventListener("Event_OnRoleChanged", new Action<Dictionary<string, object>>(this.Event_OnRoleChanged));
		EventManager.RemoveEventListener("Event_OnAppearanceClickItem", new Action<Dictionary<string, object>>(this.Event_OnAppearanceClickItem));
		EventManager.RemoveEventListener("Event_OnAppearanceShow", new Action<Dictionary<string, object>>(this.Event_OnAppearanceShow));
		EventManager.RemoveEventListener("Event_OnAppearanceHide", new Action<Dictionary<string, object>>(this.Event_OnAppearanceHide));
		EventManager.RemoveEventListener("Event_OnIdentityShow", new Action<Dictionary<string, object>>(this.Event_OnIdentityShow));
		EventManager.RemoveEventListener("Event_OnIdentityHide", new Action<Dictionary<string, object>>(this.Event_OnIdentityHide));
		EventManager.RemoveEventListener("Event_OnPlayerDataChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerDataChanged));
	}

	// Token: 0x0600006C RID: 108 RVA: 0x00003438 File Offset: 0x00001638
	private void ApplySettings()
	{
		int flagID = SettingsManager.FlagID;
		int headgearID = SettingsManager.GetHeadgearID(SettingsManager.Team, SettingsManager.Role);
		int mustacheID = SettingsManager.MustacheID;
		int beardID = SettingsManager.BeardID;
		int jerseyID = SettingsManager.GetJerseyID(SettingsManager.Team, SettingsManager.Role);
		if (BackendManager.PlayerState.PlayerData != null)
		{
			this.lockerRoomPlayer.SetUsername(BackendManager.PlayerState.PlayerData.username);
			this.lockerRoomPlayer.SetNumber(BackendManager.PlayerState.PlayerData.number.ToString());
		}
		this.lockerRoomPlayer.SetLegsPadsActive(SettingsManager.Role == PlayerRole.Goalie);
		this.lockerRoomPlayer.SetFlagID(flagID);
		this.lockerRoomPlayer.SetHeadgearID(headgearID, SettingsManager.Role);
		this.lockerRoomPlayer.SetMustacheID(mustacheID);
		this.lockerRoomPlayer.SetBeardID(beardID);
		this.lockerRoomPlayer.SetJerseyID(jerseyID, SettingsManager.Team);
	}

	// Token: 0x0600006D RID: 109 RVA: 0x0000336D File Offset: 0x0000156D
	private void Event_OnTeamChanged(Dictionary<string, object> message)
	{
		this.ApplySettings();
	}

	// Token: 0x0600006E RID: 110 RVA: 0x0000336D File Offset: 0x0000156D
	private void Event_OnRoleChanged(Dictionary<string, object> message)
	{
		this.ApplySettings();
	}

	// Token: 0x0600006F RID: 111 RVA: 0x0000351C File Offset: 0x0000171C
	private void Event_OnAppearanceClickItem(Dictionary<string, object> message)
	{
		Item item = message["item"] as Item;
		AppearanceCategory appearanceCategory = (AppearanceCategory)message["category"];
		AppearanceSubcategory appearanceSubcategory = (AppearanceSubcategory)message["subcategory"];
		PlayerTeam team = (PlayerTeam)message["team"];
		PlayerRole role = (PlayerRole)message["role"];
		switch (appearanceSubcategory)
		{
		case AppearanceSubcategory.Headgear:
			this.lockerRoomPlayer.SetHeadgearID(item.id, role);
			return;
		case AppearanceSubcategory.Flags:
			this.lockerRoomPlayer.SetFlagID(item.id);
			return;
		case AppearanceSubcategory.Mustaches:
			this.lockerRoomPlayer.SetMustacheID(item.id);
			return;
		case AppearanceSubcategory.Beards:
			this.lockerRoomPlayer.SetBeardID(item.id);
			return;
		case AppearanceSubcategory.Jerseys:
			this.lockerRoomPlayer.SetJerseyID(item.id, team);
			return;
		default:
			return;
		}
	}

	// Token: 0x06000070 RID: 112 RVA: 0x000035F4 File Offset: 0x000017F4
	private void Event_OnAppearanceShow(Dictionary<string, object> message)
	{
		this.lockerRoomPlayer.AllowRotation = true;
		this.lockerRoomPlayer.SetRotationFromPreset("front");
	}

	// Token: 0x06000071 RID: 113 RVA: 0x00003612 File Offset: 0x00001812
	private void Event_OnAppearanceHide(Dictionary<string, object> message)
	{
		this.ApplySettings();
		this.lockerRoomPlayer.AllowRotation = false;
		this.lockerRoomPlayer.SetRotationFromPreset("front");
	}

	// Token: 0x06000072 RID: 114 RVA: 0x00003636 File Offset: 0x00001836
	private void Event_OnIdentityShow(Dictionary<string, object> message)
	{
		this.lockerRoomPlayer.AllowRotation = true;
		this.lockerRoomPlayer.SetRotationFromPreset("back");
	}

	// Token: 0x06000073 RID: 115 RVA: 0x00003654 File Offset: 0x00001854
	private void Event_OnIdentityHide(Dictionary<string, object> message)
	{
		this.lockerRoomPlayer.AllowRotation = false;
		this.lockerRoomPlayer.SetRotationFromPreset("front");
	}

	// Token: 0x06000074 RID: 116 RVA: 0x00003672 File Offset: 0x00001872
	private void Event_OnPlayerDataChanged(Dictionary<string, object> message)
	{
		if ((PlayerData)message["newPlayerData"] == null)
		{
			return;
		}
		this.ApplySettings();
	}

	// Token: 0x04000036 RID: 54
	private LockerRoomPlayer lockerRoomPlayer;
}
