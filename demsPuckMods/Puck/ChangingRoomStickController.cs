using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000007 RID: 7
public class ChangingRoomStickController : MonoBehaviour
{
	// Token: 0x0600004C RID: 76 RVA: 0x00006F52 File Offset: 0x00005152
	private void Awake()
	{
		this.changingRoomStick = base.GetComponent<ChangingRoomStick>();
	}

	// Token: 0x0600004D RID: 77 RVA: 0x0001011C File Offset: 0x0000E31C
	private void Start()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnChangingRoomTeamChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnChangingRoomTeamChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnChangingRoomRoleChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnChangingRoomRoleChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnStickSkinChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnStickSkinChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnStickShaftTapeSkinChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnStickShaftTapeSkinChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnStickBladeTapeSkinChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnStickBladeTapeSkinChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnAppearanceStickSkinChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceStickSkinChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnAppearanceStickShaftTapeSkinChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceStickShaftTapeSkinChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnAppearanceStickBladeTapeSkinChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceStickBladeTapeSkinChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnAppearanceHide", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceHide));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnAppearanceTabChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceTabChanged));
	}

	// Token: 0x0600004E RID: 78 RVA: 0x00010238 File Offset: 0x0000E438
	private void OnDestroy()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnChangingRoomTeamChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnChangingRoomTeamChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnChangingRoomRoleChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnChangingRoomRoleChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnStickSkinChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnStickSkinChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnStickShaftTapeSkinChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnStickShaftTapeSkinChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnStickBladeTapeSkinChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnStickBladeTapeSkinChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnAppearanceStickSkinChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceStickSkinChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnAppearanceStickShaftTapeSkinChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceStickShaftTapeSkinChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnAppearanceStickBladeTapeSkinChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceStickBladeTapeSkinChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnAppearanceHide", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceHide));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnAppearanceTabChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceTabChanged));
	}

	// Token: 0x0600004F RID: 79 RVA: 0x00010354 File Offset: 0x0000E554
	private void Event_Client_OnChangingRoomTeamChanged(Dictionary<string, object> message)
	{
		PlayerTeam team = (PlayerTeam)message["team"];
		this.changingRoomStick.Team = team;
	}

	// Token: 0x06000050 RID: 80 RVA: 0x00010380 File Offset: 0x0000E580
	private void Event_Client_OnChangingRoomRoleChanged(Dictionary<string, object> message)
	{
		PlayerRole playerRole = (PlayerRole)message["role"];
		if (this.changingRoomStick.Role != playerRole)
		{
			this.changingRoomStick.Hide();
			return;
		}
		this.changingRoomStick.Show();
	}

	// Token: 0x06000051 RID: 81 RVA: 0x00006F60 File Offset: 0x00005160
	private void Event_Client_OnStickSkinChanged(Dictionary<string, object> message)
	{
		this.changingRoomStick.UpdateStickMesh();
	}

	// Token: 0x06000052 RID: 82 RVA: 0x00006F60 File Offset: 0x00005160
	private void Event_Client_OnStickShaftTapeSkinChanged(Dictionary<string, object> message)
	{
		this.changingRoomStick.UpdateStickMesh();
	}

	// Token: 0x06000053 RID: 83 RVA: 0x00006F60 File Offset: 0x00005160
	private void Event_Client_OnStickBladeTapeSkinChanged(Dictionary<string, object> message)
	{
		this.changingRoomStick.UpdateStickMesh();
	}

	// Token: 0x06000054 RID: 84 RVA: 0x000103C4 File Offset: 0x0000E5C4
	private void Event_Client_OnAppearanceStickSkinChanged(Dictionary<string, object> message)
	{
		PlayerTeam team = (PlayerTeam)message["team"];
		string skinName = (string)message["value"];
		this.changingRoomStick.StickMesh.SetSkin(team, skinName);
	}

	// Token: 0x06000055 RID: 85 RVA: 0x00010408 File Offset: 0x0000E608
	private void Event_Client_OnAppearanceStickShaftTapeSkinChanged(Dictionary<string, object> message)
	{
		string shaftTape = (string)message["value"];
		this.changingRoomStick.StickMesh.SetShaftTape(shaftTape);
	}

	// Token: 0x06000056 RID: 86 RVA: 0x00010438 File Offset: 0x0000E638
	private void Event_Client_OnAppearanceStickBladeTapeSkinChanged(Dictionary<string, object> message)
	{
		string bladeTape = (string)message["value"];
		this.changingRoomStick.StickMesh.SetBladeTape(bladeTape);
	}

	// Token: 0x06000057 RID: 87 RVA: 0x00010468 File Offset: 0x0000E668
	private void Event_Client_OnAppearanceTabChanged(Dictionary<string, object> message)
	{
		if ((string)message["tabName"] == "StickTab")
		{
			this.changingRoomStick.Client_MoveStickToAppearanceStickPosition();
			this.changingRoomStick.RotateWithMouse = true;
			return;
		}
		this.changingRoomStick.RotateWithMouse = false;
		this.changingRoomStick.Client_MoveStickToDefaultPosition();
	}

	// Token: 0x06000058 RID: 88 RVA: 0x00006F6D File Offset: 0x0000516D
	private void Event_Client_OnAppearanceHide(Dictionary<string, object> message)
	{
		this.changingRoomStick.RotateWithMouse = false;
		this.changingRoomStick.Client_MoveStickToDefaultPosition();
	}

	// Token: 0x04000020 RID: 32
	private ChangingRoomStick changingRoomStick;
}
