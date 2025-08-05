using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000035 RID: 53
public class ChangingRoomManagerController : MonoBehaviour
{
	// Token: 0x06000177 RID: 375 RVA: 0x00007DC8 File Offset: 0x00005FC8
	private void Awake()
	{
		this.changingRoomManager = base.GetComponent<ChangingRoomManager>();
	}

	// Token: 0x06000178 RID: 376 RVA: 0x00012C60 File Offset: 0x00010E60
	private void Start()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnChangingRoomReady", new Action<Dictionary<string, object>>(this.Event_Client_OnChangingRoomReady));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnMainMenuShow", new Action<Dictionary<string, object>>(this.Event_Client_OnMainMenuShow));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnPlayerMenuShow", new Action<Dictionary<string, object>>(this.Event_Client_OnPlayerMenuShow));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnIdentityShow", new Action<Dictionary<string, object>>(this.Event_Client_OnIdentityShow));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnAppearanceShow", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceShow));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnAppearanceTabChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceTabChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnAppearanceTeamChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceTeamChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnAppearanceRoleChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceRoleChanged));
	}

	// Token: 0x06000179 RID: 377 RVA: 0x00012D48 File Offset: 0x00010F48
	private void OnDestroy()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnChangingRoomReady", new Action<Dictionary<string, object>>(this.Event_Client_OnChangingRoomReady));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnMainMenuShow", new Action<Dictionary<string, object>>(this.Event_Client_OnMainMenuShow));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnPlayerMenuShow", new Action<Dictionary<string, object>>(this.Event_Client_OnPlayerMenuShow));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnIdentityShow", new Action<Dictionary<string, object>>(this.Event_Client_OnIdentityShow));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnAppearanceShow", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceShow));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnAppearanceTabChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceTabChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnAppearanceTeamChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceTeamChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnAppearanceRoleChanged", new Action<Dictionary<string, object>>(this.Event_Client_OnAppearanceRoleChanged));
	}

	// Token: 0x0600017A RID: 378 RVA: 0x00007DD6 File Offset: 0x00005FD6
	private void Event_Client_OnChangingRoomReady(Dictionary<string, object> message)
	{
		this.changingRoomManager.Client_EnableMainCamera();
		this.changingRoomManager.Team = PlayerTeam.Blue;
		this.changingRoomManager.Role = PlayerRole.Attacker;
	}

	// Token: 0x0600017B RID: 379 RVA: 0x00007DFB File Offset: 0x00005FFB
	private void Event_Client_OnMainMenuShow(Dictionary<string, object> message)
	{
		this.changingRoomManager.Client_MoveCameraToDefaultPosition();
	}

	// Token: 0x0600017C RID: 380 RVA: 0x00007E08 File Offset: 0x00006008
	private void Event_Client_OnPlayerMenuShow(Dictionary<string, object> message)
	{
		this.changingRoomManager.Client_MoveCameraToPlayerPosition();
	}

	// Token: 0x0600017D RID: 381 RVA: 0x00007E15 File Offset: 0x00006015
	private void Event_Client_OnIdentityShow(Dictionary<string, object> message)
	{
		this.changingRoomManager.Client_MoveCameraToIdentityPosition();
	}

	// Token: 0x0600017E RID: 382 RVA: 0x00007E22 File Offset: 0x00006022
	private void Event_Client_OnAppearanceShow(Dictionary<string, object> message)
	{
		this.changingRoomManager.Client_MoveCameraToAppearanceDefaultPosition();
	}

	// Token: 0x0600017F RID: 383 RVA: 0x00007E2F File Offset: 0x0000602F
	private void Event_Client_OnAppearanceTabChanged(Dictionary<string, object> message)
	{
		if ((string)message["tabName"] == "HeadTab")
		{
			this.changingRoomManager.Client_MoveCameraToAppearanceHeadPosition();
			return;
		}
		this.changingRoomManager.Client_MoveCameraToAppearanceDefaultPosition();
	}

	// Token: 0x06000180 RID: 384 RVA: 0x00012E30 File Offset: 0x00011030
	private void Event_Client_OnAppearanceTeamChanged(Dictionary<string, object> message)
	{
		PlayerTeam team = (PlayerTeam)message["team"];
		this.changingRoomManager.Team = team;
	}

	// Token: 0x06000181 RID: 385 RVA: 0x00012E5C File Offset: 0x0001105C
	private void Event_Client_OnAppearanceRoleChanged(Dictionary<string, object> message)
	{
		PlayerRole role = (PlayerRole)message["role"];
		this.changingRoomManager.Role = role;
	}

	// Token: 0x040000CE RID: 206
	private ChangingRoomManager changingRoomManager;
}
