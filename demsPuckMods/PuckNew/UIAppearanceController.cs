using System;
using System.Collections.Generic;

// Token: 0x02000184 RID: 388
public class UIAppearanceController : UIViewController<UIAppearance>
{
	// Token: 0x06000AF8 RID: 2808 RVA: 0x000344EC File Offset: 0x000326EC
	public override void Awake()
	{
		base.Awake();
		this.uiAppearance = base.GetComponent<UIAppearance>();
		EventManager.AddEventListener("Event_OnTeamChanged", new Action<Dictionary<string, object>>(this.Event_OnTeamChanged));
		EventManager.AddEventListener("Event_OnRoleChanged", new Action<Dictionary<string, object>>(this.Event_OnRoleChanged));
		EventManager.AddEventListener("Event_OnApplyForBothTeamsChanged", new Action<Dictionary<string, object>>(this.Event_OnApplyForBothTeamsChanged));
		EventManager.AddEventListener("Event_OnFlagIDChanged", new Action<Dictionary<string, object>>(this.Event_OnFlagIDChanged));
		EventManager.AddEventListener("Event_OnHeadgearIDChanged", new Action<Dictionary<string, object>>(this.Event_OnHeadgearIDChanged));
		EventManager.AddEventListener("Event_OnMustacheIDChanged", new Action<Dictionary<string, object>>(this.Event_OnMustacheIDChanged));
		EventManager.AddEventListener("Event_OnBeardIDChanged", new Action<Dictionary<string, object>>(this.Event_OnBeardIDChanged));
		EventManager.AddEventListener("Event_OnJerseyIDChanged", new Action<Dictionary<string, object>>(this.Event_OnJerseyIDChanged));
		EventManager.AddEventListener("Event_OnStickSkinIDChanged", new Action<Dictionary<string, object>>(this.Event_OnStickSkinIDChanged));
		EventManager.AddEventListener("Event_OnStickShaftTapeIDChanged", new Action<Dictionary<string, object>>(this.Event_OnStickShaftTapeIDChanged));
		EventManager.AddEventListener("Event_OnStickBladeTapeIDChanged", new Action<Dictionary<string, object>>(this.Event_OnStickBladeTapeIDChanged));
		EventManager.AddEventListener("Event_OnPlayerDataChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerDataChanged));
		EventManager.AddEventListener("Event_OnAppearanceHide", new Action<Dictionary<string, object>>(this.Event_OnAppearanceHide));
	}

	// Token: 0x06000AF9 RID: 2809 RVA: 0x0003462C File Offset: 0x0003282C
	private void Start()
	{
		this.uiAppearance.SetTeam(SettingsManager.Team);
		this.uiAppearance.SetRole(SettingsManager.Role);
		this.uiAppearance.SetApplyForBothTeams(SettingsManager.ApplyForBothTeams);
		this.uiAppearance.SetFlagID(SettingsManager.FlagID);
		this.uiAppearance.SetHeadgearID(PlayerTeam.Blue, PlayerRole.Attacker, SettingsManager.HeadgearIDBlueAttacker);
		this.uiAppearance.SetHeadgearID(PlayerTeam.Blue, PlayerRole.Goalie, SettingsManager.HeadgearIDBlueGoalie);
		this.uiAppearance.SetHeadgearID(PlayerTeam.Red, PlayerRole.Attacker, SettingsManager.HeadgearIDRedAttacker);
		this.uiAppearance.SetHeadgearID(PlayerTeam.Red, PlayerRole.Goalie, SettingsManager.HeadgearIDRedGoalie);
		this.uiAppearance.SetMustacheID(SettingsManager.MustacheID);
		this.uiAppearance.SetBeardID(SettingsManager.BeardID);
		this.uiAppearance.SetJerseyID(PlayerTeam.Blue, PlayerRole.Attacker, SettingsManager.JerseyIDBlueAttacker);
		this.uiAppearance.SetJerseyID(PlayerTeam.Blue, PlayerRole.Goalie, SettingsManager.JerseyIDBlueGoalie);
		this.uiAppearance.SetJerseyID(PlayerTeam.Red, PlayerRole.Attacker, SettingsManager.JerseyIDRedAttacker);
		this.uiAppearance.SetJerseyID(PlayerTeam.Red, PlayerRole.Goalie, SettingsManager.JerseyIDRedGoalie);
		this.uiAppearance.SetStickSkinID(PlayerTeam.Blue, PlayerRole.Attacker, SettingsManager.StickSkinIDBlueAttacker);
		this.uiAppearance.SetStickSkinID(PlayerTeam.Blue, PlayerRole.Goalie, SettingsManager.StickSkinIDBlueGoalie);
		this.uiAppearance.SetStickSkinID(PlayerTeam.Red, PlayerRole.Attacker, SettingsManager.StickSkinIDRedAttacker);
		this.uiAppearance.SetStickSkinID(PlayerTeam.Red, PlayerRole.Goalie, SettingsManager.StickSkinIDRedGoalie);
		this.uiAppearance.SetStickShaftTapeID(PlayerTeam.Blue, PlayerRole.Attacker, SettingsManager.StickShaftTapeIDBlueAttacker);
		this.uiAppearance.SetStickShaftTapeID(PlayerTeam.Blue, PlayerRole.Goalie, SettingsManager.StickShaftTapeIDBlueGoalie);
		this.uiAppearance.SetStickShaftTapeID(PlayerTeam.Red, PlayerRole.Attacker, SettingsManager.StickShaftTapeIDRedAttacker);
		this.uiAppearance.SetStickShaftTapeID(PlayerTeam.Red, PlayerRole.Goalie, SettingsManager.StickShaftTapeIDRedGoalie);
		this.uiAppearance.SetStickBladeTapeID(PlayerTeam.Blue, PlayerRole.Attacker, SettingsManager.StickBladeTapeIDBlueAttacker);
		this.uiAppearance.SetStickBladeTapeID(PlayerTeam.Blue, PlayerRole.Goalie, SettingsManager.StickBladeTapeIDBlueGoalie);
		this.uiAppearance.SetStickBladeTapeID(PlayerTeam.Red, PlayerRole.Attacker, SettingsManager.StickBladeTapeIDRedAttacker);
		this.uiAppearance.SetStickBladeTapeID(PlayerTeam.Red, PlayerRole.Goalie, SettingsManager.StickBladeTapeIDRedGoalie);
	}

	// Token: 0x06000AFA RID: 2810 RVA: 0x00034804 File Offset: 0x00032A04
	public override void OnDestroy()
	{
		EventManager.RemoveEventListener("Event_OnTeamChanged", new Action<Dictionary<string, object>>(this.Event_OnTeamChanged));
		EventManager.RemoveEventListener("Event_OnRoleChanged", new Action<Dictionary<string, object>>(this.Event_OnRoleChanged));
		EventManager.RemoveEventListener("Event_OnApplyForBothTeamsChanged", new Action<Dictionary<string, object>>(this.Event_OnApplyForBothTeamsChanged));
		EventManager.RemoveEventListener("Event_OnFlagIDChanged", new Action<Dictionary<string, object>>(this.Event_OnFlagIDChanged));
		EventManager.RemoveEventListener("Event_OnHeadgearIDChanged", new Action<Dictionary<string, object>>(this.Event_OnHeadgearIDChanged));
		EventManager.RemoveEventListener("Event_OnMustacheIDChanged", new Action<Dictionary<string, object>>(this.Event_OnMustacheIDChanged));
		EventManager.RemoveEventListener("Event_OnBeardIDChanged", new Action<Dictionary<string, object>>(this.Event_OnBeardIDChanged));
		EventManager.RemoveEventListener("Event_OnJerseyIDChanged", new Action<Dictionary<string, object>>(this.Event_OnJerseyIDChanged));
		EventManager.RemoveEventListener("Event_OnStickSkinIDChanged", new Action<Dictionary<string, object>>(this.Event_OnStickSkinIDChanged));
		EventManager.RemoveEventListener("Event_OnStickShaftTapeIDChanged", new Action<Dictionary<string, object>>(this.Event_OnStickShaftTapeIDChanged));
		EventManager.RemoveEventListener("Event_OnStickBladeTapeIDChanged", new Action<Dictionary<string, object>>(this.Event_OnStickBladeTapeIDChanged));
		EventManager.RemoveEventListener("Event_OnPlayerDataChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerDataChanged));
		EventManager.RemoveEventListener("Event_OnAppearanceHide", new Action<Dictionary<string, object>>(this.Event_OnAppearanceHide));
		base.OnDestroy();
	}

	// Token: 0x06000AFB RID: 2811 RVA: 0x00034938 File Offset: 0x00032B38
	private void Event_OnTeamChanged(Dictionary<string, object> message)
	{
		PlayerTeam team = (PlayerTeam)message["value"];
		this.uiAppearance.SetTeam(team);
	}

	// Token: 0x06000AFC RID: 2812 RVA: 0x00034964 File Offset: 0x00032B64
	private void Event_OnRoleChanged(Dictionary<string, object> message)
	{
		PlayerRole role = (PlayerRole)message["value"];
		this.uiAppearance.SetRole(role);
	}

	// Token: 0x06000AFD RID: 2813 RVA: 0x00034990 File Offset: 0x00032B90
	private void Event_OnApplyForBothTeamsChanged(Dictionary<string, object> message)
	{
		bool applyForBothTeams = (bool)message["value"];
		this.uiAppearance.SetApplyForBothTeams(applyForBothTeams);
	}

	// Token: 0x06000AFE RID: 2814 RVA: 0x000349BC File Offset: 0x00032BBC
	private void Event_OnFlagIDChanged(Dictionary<string, object> message)
	{
		int flagID = (int)message["value"];
		this.uiAppearance.SetFlagID(flagID);
	}

	// Token: 0x06000AFF RID: 2815 RVA: 0x000349E8 File Offset: 0x00032BE8
	private void Event_OnHeadgearIDChanged(Dictionary<string, object> message)
	{
		PlayerTeam team = (PlayerTeam)message["team"];
		PlayerRole role = (PlayerRole)message["role"];
		int value = (int)message["value"];
		this.uiAppearance.SetHeadgearID(team, role, value);
	}

	// Token: 0x06000B00 RID: 2816 RVA: 0x00034A38 File Offset: 0x00032C38
	private void Event_OnMustacheIDChanged(Dictionary<string, object> message)
	{
		int mustacheID = (int)message["value"];
		this.uiAppearance.SetMustacheID(mustacheID);
	}

	// Token: 0x06000B01 RID: 2817 RVA: 0x00034A64 File Offset: 0x00032C64
	private void Event_OnBeardIDChanged(Dictionary<string, object> message)
	{
		int beardID = (int)message["value"];
		this.uiAppearance.SetBeardID(beardID);
	}

	// Token: 0x06000B02 RID: 2818 RVA: 0x00034A90 File Offset: 0x00032C90
	private void Event_OnJerseyIDChanged(Dictionary<string, object> message)
	{
		PlayerTeam team = (PlayerTeam)message["team"];
		PlayerRole role = (PlayerRole)message["role"];
		int value = (int)message["value"];
		this.uiAppearance.SetJerseyID(team, role, value);
	}

	// Token: 0x06000B03 RID: 2819 RVA: 0x00034AE0 File Offset: 0x00032CE0
	private void Event_OnStickSkinIDChanged(Dictionary<string, object> message)
	{
		PlayerTeam team = (PlayerTeam)message["team"];
		PlayerRole role = (PlayerRole)message["role"];
		int value = (int)message["value"];
		this.uiAppearance.SetStickSkinID(team, role, value);
	}

	// Token: 0x06000B04 RID: 2820 RVA: 0x00034B30 File Offset: 0x00032D30
	private void Event_OnStickShaftTapeIDChanged(Dictionary<string, object> message)
	{
		PlayerTeam team = (PlayerTeam)message["team"];
		PlayerRole role = (PlayerRole)message["role"];
		int value = (int)message["value"];
		this.uiAppearance.SetStickShaftTapeID(team, role, value);
	}

	// Token: 0x06000B05 RID: 2821 RVA: 0x00034B80 File Offset: 0x00032D80
	private void Event_OnStickBladeTapeIDChanged(Dictionary<string, object> message)
	{
		PlayerTeam team = (PlayerTeam)message["team"];
		PlayerRole role = (PlayerRole)message["role"];
		int value = (int)message["value"];
		this.uiAppearance.SetStickBladeTapeID(team, role, value);
	}

	// Token: 0x06000B06 RID: 2822 RVA: 0x00034BCE File Offset: 0x00032DCE
	private void Event_OnPlayerDataChanged(Dictionary<string, object> message)
	{
		if ((PlayerData)message["newPlayerData"] == null)
		{
			return;
		}
		this.uiAppearance.StyleRadioButtonGroups();
	}

	// Token: 0x06000B07 RID: 2823 RVA: 0x00034BEE File Offset: 0x00032DEE
	private void Event_OnAppearanceHide(Dictionary<string, object> message)
	{
		this.uiAppearance.UpdateRadioButtons();
	}

	// Token: 0x04000691 RID: 1681
	private UIAppearance uiAppearance;
}
