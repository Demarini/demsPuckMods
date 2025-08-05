using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000005 RID: 5
public class ChangingRoomPlayerController : MonoBehaviour
{
	// Token: 0x06000029 RID: 41 RVA: 0x00006DF1 File Offset: 0x00004FF1
	private void Awake()
	{
		this.changingRoomPlayer = base.GetComponent<ChangingRoomPlayer>();
	}

	// Token: 0x0600002A RID: 42 RVA: 0x0000FA10 File Offset: 0x0000DC10
	private void Start()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnChangingRoomTeamChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnChangingRoomTeamChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnChangingRoomRoleChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnChangingRoomRoleChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnCountryChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnCountryChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnVisorSkinChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnVisorSkinChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnMustacheChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnMustacheChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnBeardChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnBeardChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnJerseySkinChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnJerseySkinChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnAppearanceCountryChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceCountryChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnAppearanceVisorChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceVisorChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnAppearanceMustacheChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceMustacheChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnAppearanceBeardChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceBeardChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnAppearanceJerseyChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceJerseyChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnAppearanceTabChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceTabChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnAppearanceShow", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceShow));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnAppearanceHide", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceHide));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnIdentityShow", new Action<Dictionary<string, object>>(this.Event_Client_OnIdentityShow));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnIdentityHide", new Action<Dictionary<string, object>>(this.Event_Client_OnIdentityHide));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnPlayerDataChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnPlayerDataChanged));
	}

	// Token: 0x0600002B RID: 43 RVA: 0x0000FC04 File Offset: 0x0000DE04
	private void OnDestroy()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnChangingRoomTeamChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnChangingRoomTeamChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnChangingRoomRoleChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnChangingRoomRoleChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnCountryChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnCountryChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnVisorSkinChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnVisorSkinChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnMustacheChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnMustacheChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnBeardChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnBeardChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnJerseySkinChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnJerseySkinChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnAppearanceCountryChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceCountryChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnAppearanceVisorChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceVisorChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnAppearanceMustacheChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceMustacheChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnAppearanceBeardChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceBeardChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnAppearanceJerseyChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceJerseyChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnAppearanceTabChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceTabChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnAppearanceShow", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceShow));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnAppearanceHide", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceHide));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnIdentityShow", new Action<Dictionary<string, object>>(this.Event_Client_OnIdentityShow));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnIdentityHide", new Action<Dictionary<string, object>>(this.Event_Client_OnIdentityHide));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnPlayerDataChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnPlayerDataChanged));
	}

	// Token: 0x0600002C RID: 44 RVA: 0x0000FDF8 File Offset: 0x0000DFF8
	private void Event_Client_OnChangingRoomTeamChanged(Dictionary<string, object> message)
	{
		PlayerTeam team = (PlayerTeam)message["team"];
		this.changingRoomPlayer.Team = team;
	}

	// Token: 0x0600002D RID: 45 RVA: 0x0000FE24 File Offset: 0x0000E024
	private void Event_Client_OnChangingRoomRoleChanged(Dictionary<string, object> message)
	{
		PlayerRole role = (PlayerRole)message["role"];
		this.changingRoomPlayer.Role = role;
	}

	// Token: 0x0600002E RID: 46 RVA: 0x00006DFF File Offset: 0x00004FFF
	private void Event_Client_OnCountryChanged(Dictionary<string, object> message)
	{
		this.changingRoomPlayer.UpdatePlayerMesh();
	}

	// Token: 0x0600002F RID: 47 RVA: 0x00006DFF File Offset: 0x00004FFF
	private void Event_Client_OnVisorSkinChanged(Dictionary<string, object> message)
	{
		this.changingRoomPlayer.UpdatePlayerMesh();
	}

	// Token: 0x06000030 RID: 48 RVA: 0x00006DFF File Offset: 0x00004FFF
	private void Event_Client_OnMustacheChanged(Dictionary<string, object> message)
	{
		this.changingRoomPlayer.UpdatePlayerMesh();
	}

	// Token: 0x06000031 RID: 49 RVA: 0x00006DFF File Offset: 0x00004FFF
	private void Event_Client_OnBeardChanged(Dictionary<string, object> message)
	{
		this.changingRoomPlayer.UpdatePlayerMesh();
	}

	// Token: 0x06000032 RID: 50 RVA: 0x00006DFF File Offset: 0x00004FFF
	private void Event_Client_OnJerseySkinChanged(Dictionary<string, object> message)
	{
		this.changingRoomPlayer.UpdatePlayerMesh();
	}

	// Token: 0x06000033 RID: 51 RVA: 0x0000FE50 File Offset: 0x0000E050
	private void Event_Client_OnAppearanceCountryChanged(Dictionary<string, object> message)
	{
		string helmetFlag = (string)message["value"];
		this.changingRoomPlayer.PlayerMesh.PlayerHead.SetHelmetFlag(helmetFlag);
	}

	// Token: 0x06000034 RID: 52 RVA: 0x0000FE84 File Offset: 0x0000E084
	private void Event_Client_OnAppearanceVisorChanged(Dictionary<string, object> message)
	{
		string helmetVisor = (string)message["value"];
		this.changingRoomPlayer.PlayerMesh.PlayerHead.SetHelmetVisor(helmetVisor);
	}

	// Token: 0x06000035 RID: 53 RVA: 0x0000FEB8 File Offset: 0x0000E0B8
	private void Event_Client_OnAppearanceMustacheChanged(Dictionary<string, object> message)
	{
		string mustache = (string)message["value"];
		this.changingRoomPlayer.PlayerMesh.PlayerHead.SetMustache(mustache);
	}

	// Token: 0x06000036 RID: 54 RVA: 0x0000FEEC File Offset: 0x0000E0EC
	private void Event_Client_OnAppearanceBeardChanged(Dictionary<string, object> message)
	{
		string beard = (string)message["value"];
		this.changingRoomPlayer.PlayerMesh.PlayerHead.SetBeard(beard);
	}

	// Token: 0x06000037 RID: 55 RVA: 0x0000FF20 File Offset: 0x0000E120
	private void Event_Client_OnAppearanceJerseyChanged(Dictionary<string, object> message)
	{
		string jersey = (string)message["value"];
		PlayerTeam team = (PlayerTeam)message["team"];
		this.changingRoomPlayer.PlayerMesh.SetJersey(team, jersey);
	}

	// Token: 0x06000038 RID: 56 RVA: 0x0000FF64 File Offset: 0x0000E164
	private void Event_Client_OnAppearanceTabChanged(Dictionary<string, object> message)
	{
		string a = (string)message["tabName"];
		if (a == "HeadTab" || a == "BodyTab")
		{
			this.changingRoomPlayer.RotateWithMouse = true;
			return;
		}
		this.changingRoomPlayer.RotateWithMouse = false;
		this.changingRoomPlayer.Client_MovePlayerToDefaultPosition();
	}

	// Token: 0x06000039 RID: 57 RVA: 0x00006E0C File Offset: 0x0000500C
	private void Event_Client_OnAppearanceShow(Dictionary<string, object> message)
	{
		this.changingRoomPlayer.RotateWithMouse = true;
	}

	// Token: 0x0600003A RID: 58 RVA: 0x00006E1A File Offset: 0x0000501A
	private void Event_Client_OnAppearanceHide(Dictionary<string, object> message)
	{
		this.changingRoomPlayer.RotateWithMouse = false;
		this.changingRoomPlayer.Client_MovePlayerToDefaultPosition();
	}

	// Token: 0x0600003B RID: 59 RVA: 0x00006E33 File Offset: 0x00005033
	private void Event_Client_OnIdentityShow(Dictionary<string, object> message)
	{
		this.changingRoomPlayer.RotateWithMouse = true;
		this.changingRoomPlayer.Client_MovePlayerToIdentityPosition();
	}

	// Token: 0x0600003C RID: 60 RVA: 0x00006E1A File Offset: 0x0000501A
	private void Event_Client_OnIdentityHide(Dictionary<string, object> message)
	{
		this.changingRoomPlayer.RotateWithMouse = false;
		this.changingRoomPlayer.Client_MovePlayerToDefaultPosition();
	}

	// Token: 0x0600003D RID: 61 RVA: 0x00006DFF File Offset: 0x00004FFF
	private void Event_Client_OnPlayerDataChanged(Dictionary<string, object> message)
	{
		this.changingRoomPlayer.UpdatePlayerMesh();
	}

	// Token: 0x04000014 RID: 20
	private ChangingRoomPlayer changingRoomPlayer;
}
