using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Token: 0x02000143 RID: 323
public class UIManagerController : MonoBehaviour
{
	// Token: 0x060009A1 RID: 2465 RVA: 0x0002EC00 File Offset: 0x0002CE00
	public void Awake()
	{
		this.uiManager = base.GetComponent<UIManager>();
		EventManager.AddEventListener("Event_OnUserInterfaceScaleChanged", new Action<Dictionary<string, object>>(this.Event_OnUserInterfaceScaleChanged));
		EventManager.AddEventListener("Event_OnUIStateChanged", new Action<Dictionary<string, object>>(this.Event_OnUIStateChanged));
		EventManager.AddEventListener("Event_OnMainMenuClickPlay", new Action<Dictionary<string, object>>(this.Event_OnMainMenuClickPlay));
		EventManager.AddEventListener("Event_OnMainMenuClickPlayer", new Action<Dictionary<string, object>>(this.Event_OnMainMenuClickPlayer));
		EventManager.AddEventListener("Event_OnMainMenuClickSettings", new Action<Dictionary<string, object>>(this.Event_OnMainMenuClickSettings));
		EventManager.AddEventListener("Event_OnMainMenuClickMods", new Action<Dictionary<string, object>>(this.Event_OnMainMenuClickMods));
		EventManager.AddEventListener("Event_OnPlayerMenuClickBack", new Action<Dictionary<string, object>>(this.Event_OnPlayerMenuClickBack));
		EventManager.AddEventListener("Event_OnPlayerMenuClickIdentity", new Action<Dictionary<string, object>>(this.Event_OnPlayerMenuClickIdentity));
		EventManager.AddEventListener("Event_OnPlayerMenuClickAppearance", new Action<Dictionary<string, object>>(this.Event_OnPlayerMenuClickAppearance));
		EventManager.AddEventListener("Event_OnIdentityClickClose", new Action<Dictionary<string, object>>(this.Event_OnIdentityClickClose));
		EventManager.AddEventListener("Event_OnAppearanceClickClose", new Action<Dictionary<string, object>>(this.Event_OnAppearanceClickClose));
		EventManager.AddEventListener("Event_OnPauseMenuClickSettings", new Action<Dictionary<string, object>>(this.Event_OnPauseMenuClickSettings));
		EventManager.AddEventListener("Event_OnPauseMenuClickSelectTeam", new Action<Dictionary<string, object>>(this.Event_OnPauseMenuClickSelectTeam));
		EventManager.AddEventListener("Event_OnPauseMenuClickSelectPosition", new Action<Dictionary<string, object>>(this.Event_OnPauseMenuClickSelectPosition));
		EventManager.AddEventListener("Event_OnPauseMenuClickServerBrowser", new Action<Dictionary<string, object>>(this.Event_OnPauseMenuClickServerBrowser));
		EventManager.AddEventListener("Event_OnServerBrowserClickClose", new Action<Dictionary<string, object>>(this.Event_OnServerBrowserClickClose));
		EventManager.AddEventListener("Event_OnServerBrowserClickEndPoint", new Action<Dictionary<string, object>>(this.Event_OnServerBrowserClickEndPoint));
		EventManager.AddEventListener("Event_OnServerBrowserClickNewServer", new Action<Dictionary<string, object>>(this.Event_OnServerBrowserClickNewServer));
		EventManager.AddEventListener("Event_OnServerBrowserClickFilters", new Action<Dictionary<string, object>>(this.Event_OnServerBrowserClickFilters));
		EventManager.AddEventListener("Event_OnServerBrowserFiltersClickClose", new Action<Dictionary<string, object>>(this.Event_OnServerBrowserFiltersClickClose));
		EventManager.AddEventListener("Event_OnSettingsClickClose", new Action<Dictionary<string, object>>(this.Event_OnSettingsClickClose));
		EventManager.AddEventListener("Event_OnNewServerClickStart", new Action<Dictionary<string, object>>(this.Event_OnNewServerClickStart));
		EventManager.AddEventListener("Event_OnNewServerClickClose", new Action<Dictionary<string, object>>(this.Event_OnNewServerClickClose));
		EventManager.AddEventListener("Event_OnModsClickClose", new Action<Dictionary<string, object>>(this.Event_OnModsClickClose));
		EventManager.AddEventListener("Event_OnFooterClickInvite", new Action<Dictionary<string, object>>(this.Event_OnFooterClickInvite));
		EventManager.AddEventListener("Event_OnFriendsClickClose", new Action<Dictionary<string, object>>(this.Event_OnFriendsClickClose));
		EventManager.AddEventListener("Event_OnPlayClickServerBrowser", new Action<Dictionary<string, object>>(this.Event_OnPlayClickServerBrowser));
		EventManager.AddEventListener("Event_OnPlayClickClose", new Action<Dictionary<string, object>>(this.Event_OnPlayClickClose));
		EventManager.AddEventListener("Event_OnChatMessageAdded", new Action<Dictionary<string, object>>(this.Event_OnChatMessageAdded));
		EventManager.AddEventListener("Event_OnMatchJoinTimeoutTickerStarted", new Action<Dictionary<string, object>>(this.Event_OnMatchJoinTimeoutTickerStarted));
		EventManager.AddEventListener("Event_OnMatchJoinTimeoutTickerTick", new Action<Dictionary<string, object>>(this.Event_OnMatchJoinTimeoutTickerTick));
		EventManager.AddEventListener("Event_Everyone_OnPlayerGameStateChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerGameStateChanged));
	}

	// Token: 0x060009A2 RID: 2466 RVA: 0x0002EED9 File Offset: 0x0002D0D9
	private void Start()
	{
		this.uiManager.SetUIScale(SettingsManager.UserInterfaceScale);
		this.uiManager.ShowPhaseViews(GlobalStateManager.UIState.Phase);
	}

	// Token: 0x060009A3 RID: 2467 RVA: 0x0002EF00 File Offset: 0x0002D100
	private void OnDestroy()
	{
		EventManager.RemoveEventListener("Event_OnUserInterfaceScaleChanged", new Action<Dictionary<string, object>>(this.Event_OnUserInterfaceScaleChanged));
		EventManager.RemoveEventListener("Event_OnUIStateChanged", new Action<Dictionary<string, object>>(this.Event_OnUIStateChanged));
		EventManager.RemoveEventListener("Event_OnMainMenuClickPlay", new Action<Dictionary<string, object>>(this.Event_OnMainMenuClickPlay));
		EventManager.RemoveEventListener("Event_OnMainMenuClickPlayer", new Action<Dictionary<string, object>>(this.Event_OnMainMenuClickPlayer));
		EventManager.RemoveEventListener("Event_OnMainMenuClickSettings", new Action<Dictionary<string, object>>(this.Event_OnMainMenuClickSettings));
		EventManager.RemoveEventListener("Event_OnMainMenuClickMods", new Action<Dictionary<string, object>>(this.Event_OnMainMenuClickMods));
		EventManager.RemoveEventListener("Event_OnPlayerMenuClickBack", new Action<Dictionary<string, object>>(this.Event_OnPlayerMenuClickBack));
		EventManager.RemoveEventListener("Event_OnPlayerMenuClickIdentity", new Action<Dictionary<string, object>>(this.Event_OnPlayerMenuClickIdentity));
		EventManager.RemoveEventListener("Event_OnPlayerMenuClickAppearance", new Action<Dictionary<string, object>>(this.Event_OnPlayerMenuClickAppearance));
		EventManager.RemoveEventListener("Event_OnIdentityClickClose", new Action<Dictionary<string, object>>(this.Event_OnIdentityClickClose));
		EventManager.RemoveEventListener("Event_OnAppearanceClickClose", new Action<Dictionary<string, object>>(this.Event_OnAppearanceClickClose));
		EventManager.RemoveEventListener("Event_OnPauseMenuClickSettings", new Action<Dictionary<string, object>>(this.Event_OnPauseMenuClickSettings));
		EventManager.RemoveEventListener("Event_OnPauseMenuClickSelectTeam", new Action<Dictionary<string, object>>(this.Event_OnPauseMenuClickSelectTeam));
		EventManager.RemoveEventListener("Event_OnPauseMenuClickSelectPosition", new Action<Dictionary<string, object>>(this.Event_OnPauseMenuClickSelectPosition));
		EventManager.RemoveEventListener("Event_OnPauseMenuClickServerBrowser", new Action<Dictionary<string, object>>(this.Event_OnPauseMenuClickServerBrowser));
		EventManager.RemoveEventListener("Event_OnServerBrowserClickClose", new Action<Dictionary<string, object>>(this.Event_OnServerBrowserClickClose));
		EventManager.RemoveEventListener("Event_OnServerBrowserClickEndPoint", new Action<Dictionary<string, object>>(this.Event_OnServerBrowserClickEndPoint));
		EventManager.RemoveEventListener("Event_OnServerBrowserClickNewServer", new Action<Dictionary<string, object>>(this.Event_OnServerBrowserClickNewServer));
		EventManager.RemoveEventListener("Event_OnServerBrowserClickFilters", new Action<Dictionary<string, object>>(this.Event_OnServerBrowserClickFilters));
		EventManager.RemoveEventListener("Event_OnServerBrowserFiltersClickClose", new Action<Dictionary<string, object>>(this.Event_OnServerBrowserFiltersClickClose));
		EventManager.RemoveEventListener("Event_OnSettingsClickClose", new Action<Dictionary<string, object>>(this.Event_OnSettingsClickClose));
		EventManager.RemoveEventListener("Event_OnNewServerClickStart", new Action<Dictionary<string, object>>(this.Event_OnNewServerClickStart));
		EventManager.RemoveEventListener("Event_OnNewServerClickClose", new Action<Dictionary<string, object>>(this.Event_OnNewServerClickClose));
		EventManager.RemoveEventListener("Event_OnModsClickClose", new Action<Dictionary<string, object>>(this.Event_OnModsClickClose));
		EventManager.RemoveEventListener("Event_OnFooterClickInvite", new Action<Dictionary<string, object>>(this.Event_OnFooterClickInvite));
		EventManager.RemoveEventListener("Event_OnFriendsClickClose", new Action<Dictionary<string, object>>(this.Event_OnFriendsClickClose));
		EventManager.RemoveEventListener("Event_OnPlayClickServerBrowser", new Action<Dictionary<string, object>>(this.Event_OnPlayClickServerBrowser));
		EventManager.RemoveEventListener("Event_OnPlayClickClose", new Action<Dictionary<string, object>>(this.Event_OnPlayClickClose));
		EventManager.RemoveEventListener("Event_OnChatMessageAdded", new Action<Dictionary<string, object>>(this.Event_OnChatMessageAdded));
		EventManager.RemoveEventListener("Event_OnMatchJoinTimeoutTickerStarted", new Action<Dictionary<string, object>>(this.Event_OnMatchJoinTimeoutTickerStarted));
		EventManager.RemoveEventListener("Event_OnMatchJoinTimeoutTickerTick", new Action<Dictionary<string, object>>(this.Event_OnMatchJoinTimeoutTickerTick));
		EventManager.RemoveEventListener("Event_Everyone_OnPlayerGameStateChanged", new Action<Dictionary<string, object>>(this.Event_Everyone_OnPlayerGameStateChanged));
	}

	// Token: 0x060009A4 RID: 2468 RVA: 0x0002F1D0 File Offset: 0x0002D3D0
	private void Event_OnUserInterfaceScaleChanged(Dictionary<string, object> message)
	{
		float uiscale = (float)message["value"];
		this.uiManager.SetUIScale(uiscale);
	}

	// Token: 0x060009A5 RID: 2469 RVA: 0x0002F1FC File Offset: 0x0002D3FC
	private void Event_OnUIStateChanged(Dictionary<string, object> message)
	{
		ref UIState ptr = (UIState)message["oldUIState"];
		UIState uistate = (UIState)message["newUIState"];
		if (ptr.Phase == uistate.Phase)
		{
			return;
		}
		this.uiManager.ShowPhaseViews(uistate.Phase);
	}

	// Token: 0x060009A6 RID: 2470 RVA: 0x0002F249 File Offset: 0x0002D449
	private void Event_OnMainMenuClickPlay(Dictionary<string, object> message)
	{
		this.uiManager.Play.Show();
		this.uiManager.MainMenu.Hide();
	}

	// Token: 0x060009A7 RID: 2471 RVA: 0x0002F26D File Offset: 0x0002D46D
	private void Event_OnMainMenuClickPlayer(Dictionary<string, object> message)
	{
		this.uiManager.PlayerMenu.Show();
		this.uiManager.MainMenu.Hide();
	}

	// Token: 0x060009A8 RID: 2472 RVA: 0x0002F291 File Offset: 0x0002D491
	private void Event_OnMainMenuClickSettings(Dictionary<string, object> message)
	{
		this.uiManager.Settings.Show();
		this.uiManager.MainMenu.Hide();
	}

	// Token: 0x060009A9 RID: 2473 RVA: 0x0002F2B5 File Offset: 0x0002D4B5
	private void Event_OnMainMenuClickMods(Dictionary<string, object> message)
	{
		this.uiManager.Mods.Show();
		this.uiManager.MainMenu.Hide();
	}

	// Token: 0x060009AA RID: 2474 RVA: 0x0002F2D9 File Offset: 0x0002D4D9
	private void Event_OnPlayerMenuClickBack(Dictionary<string, object> message)
	{
		this.uiManager.MainMenu.Show();
		this.uiManager.PlayerMenu.Hide();
	}

	// Token: 0x060009AB RID: 2475 RVA: 0x0002F2FD File Offset: 0x0002D4FD
	private void Event_OnPlayerMenuClickIdentity(Dictionary<string, object> message)
	{
		this.uiManager.Identity.Show();
		this.uiManager.PlayerMenu.Hide();
	}

	// Token: 0x060009AC RID: 2476 RVA: 0x0002F321 File Offset: 0x0002D521
	private void Event_OnPlayerMenuClickAppearance(Dictionary<string, object> message)
	{
		this.uiManager.Appearance.Show();
		this.uiManager.PlayerMenu.Hide();
	}

	// Token: 0x060009AD RID: 2477 RVA: 0x0002F345 File Offset: 0x0002D545
	private void Event_OnIdentityClickClose(Dictionary<string, object> message)
	{
		this.uiManager.PlayerMenu.Show();
		this.uiManager.Identity.Hide();
	}

	// Token: 0x060009AE RID: 2478 RVA: 0x0002F369 File Offset: 0x0002D569
	private void Event_OnAppearanceClickClose(Dictionary<string, object> message)
	{
		this.uiManager.PlayerMenu.Show();
		this.uiManager.Appearance.Hide();
	}

	// Token: 0x060009AF RID: 2479 RVA: 0x0002F38D File Offset: 0x0002D58D
	private void Event_OnPauseMenuClickSettings(Dictionary<string, object> message)
	{
		this.uiManager.Settings.Show();
		this.uiManager.PauseMenu.Hide();
	}

	// Token: 0x060009B0 RID: 2480 RVA: 0x0002F3B1 File Offset: 0x0002D5B1
	private void Event_OnPauseMenuClickSelectTeam(Dictionary<string, object> message)
	{
		this.uiManager.PauseMenu.Hide();
	}

	// Token: 0x060009B1 RID: 2481 RVA: 0x0002F3B1 File Offset: 0x0002D5B1
	private void Event_OnPauseMenuClickSelectPosition(Dictionary<string, object> message)
	{
		this.uiManager.PauseMenu.Hide();
	}

	// Token: 0x060009B2 RID: 2482 RVA: 0x0002F3C4 File Offset: 0x0002D5C4
	private void Event_OnPauseMenuClickServerBrowser(Dictionary<string, object> message)
	{
		this.uiManager.ServerBrowser.Show();
		this.uiManager.PauseMenu.Hide();
	}

	// Token: 0x060009B3 RID: 2483 RVA: 0x0002F3E8 File Offset: 0x0002D5E8
	private void Event_OnServerBrowserClickClose(Dictionary<string, object> message)
	{
		UIPhase phase = GlobalStateManager.UIState.Phase;
		if (phase != UIPhase.LockerRoom)
		{
			if (phase == UIPhase.Playing)
			{
				this.uiManager.PauseMenu.Show();
			}
		}
		else
		{
			this.uiManager.Play.Show();
		}
		this.uiManager.ServerBrowser.Hide();
	}

	// Token: 0x060009B4 RID: 2484 RVA: 0x000020D3 File Offset: 0x000002D3
	private void Event_OnServerBrowserClickEndPoint(Dictionary<string, object> message)
	{
	}

	// Token: 0x060009B5 RID: 2485 RVA: 0x0002F43F File Offset: 0x0002D63F
	private void Event_OnServerBrowserClickNewServer(Dictionary<string, object> message)
	{
		this.uiManager.NewServer.Show();
		this.uiManager.ServerBrowser.Hide();
	}

	// Token: 0x060009B6 RID: 2486 RVA: 0x0002F463 File Offset: 0x0002D663
	private void Event_OnServerBrowserClickFilters(Dictionary<string, object> message)
	{
		this.uiManager.ServerBrowser.ShowFilters();
	}

	// Token: 0x060009B7 RID: 2487 RVA: 0x0002F475 File Offset: 0x0002D675
	private void Event_OnServerBrowserFiltersClickClose(Dictionary<string, object> message)
	{
		this.uiManager.ServerBrowser.HideFilters();
	}

	// Token: 0x060009B8 RID: 2488 RVA: 0x0002F488 File Offset: 0x0002D688
	private void Event_OnSettingsClickClose(Dictionary<string, object> message)
	{
		UIPhase phase = GlobalStateManager.UIState.Phase;
		if (phase != UIPhase.LockerRoom)
		{
			if (phase == UIPhase.Playing)
			{
				this.uiManager.PauseMenu.Show();
			}
		}
		else
		{
			this.uiManager.MainMenu.Show();
		}
		this.uiManager.Settings.Hide();
	}

	// Token: 0x060009B9 RID: 2489 RVA: 0x0002F4DF File Offset: 0x0002D6DF
	private void Event_OnNewServerClickStart(Dictionary<string, object> message)
	{
		this.uiManager.ServerBrowser.Show();
		this.uiManager.NewServer.Hide();
	}

	// Token: 0x060009BA RID: 2490 RVA: 0x0002F4DF File Offset: 0x0002D6DF
	private void Event_OnNewServerClickClose(Dictionary<string, object> message)
	{
		this.uiManager.ServerBrowser.Show();
		this.uiManager.NewServer.Hide();
	}

	// Token: 0x060009BB RID: 2491 RVA: 0x0002F503 File Offset: 0x0002D703
	private void Event_OnModsClickClose(Dictionary<string, object> message)
	{
		this.uiManager.MainMenu.Show();
		this.uiManager.Mods.Hide();
	}

	// Token: 0x060009BC RID: 2492 RVA: 0x0002F527 File Offset: 0x0002D727
	private void Event_OnFooterClickInvite(Dictionary<string, object> message)
	{
		this.uiManager.Friends.Show();
	}

	// Token: 0x060009BD RID: 2493 RVA: 0x0002F53A File Offset: 0x0002D73A
	private void Event_OnFriendsClickClose(Dictionary<string, object> message)
	{
		this.uiManager.Friends.Hide();
	}

	// Token: 0x060009BE RID: 2494 RVA: 0x0002F54D File Offset: 0x0002D74D
	private void Event_OnPlayClickServerBrowser(Dictionary<string, object> message)
	{
		this.uiManager.ServerBrowser.Show();
		this.uiManager.Play.Hide();
	}

	// Token: 0x060009BF RID: 2495 RVA: 0x0002F571 File Offset: 0x0002D771
	private void Event_OnPlayClickClose(Dictionary<string, object> message)
	{
		this.uiManager.MainMenu.Show();
		this.uiManager.Play.Hide();
	}

	// Token: 0x060009C0 RID: 2496 RVA: 0x0002F595 File Offset: 0x0002D795
	private void Event_OnChatMessageAdded(Dictionary<string, object> message)
	{
		this.uiManager.PlayNotificationSound();
	}

	// Token: 0x060009C1 RID: 2497 RVA: 0x0002F5A2 File Offset: 0x0002D7A2
	private void Event_OnMatchJoinTimeoutTickerStarted(Dictionary<string, object> message)
	{
		if (!BackendUtils.IsConnectedToMatchEndPoint())
		{
			this.uiManager.PlayWhooshSound();
		}
	}

	// Token: 0x060009C2 RID: 2498 RVA: 0x0002F5B6 File Offset: 0x0002D7B6
	private void Event_OnMatchJoinTimeoutTickerTick(Dictionary<string, object> message)
	{
		if (!BackendUtils.IsConnectedToMatchEndPoint())
		{
			this.uiManager.PlayTickSound();
		}
	}

	// Token: 0x060009C3 RID: 2499 RVA: 0x0002F5CC File Offset: 0x0002D7CC
	private void Event_Everyone_OnPlayerGameStateChanged(Dictionary<string, object> message)
	{
		NetworkBehaviour networkBehaviour = (Player)message["player"];
		PlayerGameState playerGameState = (PlayerGameState)message["oldGameState"];
		PlayerGameState playerGameState2 = (PlayerGameState)message["newGameState"];
		if (!networkBehaviour.IsLocalPlayer)
		{
			return;
		}
		if (playerGameState.Phase == playerGameState2.Phase)
		{
			return;
		}
		this.uiManager.ShowPhaseViews(GlobalStateManager.UIState.Phase);
		switch (playerGameState2.Phase)
		{
		case PlayerPhase.TeamSelect:
			this.uiManager.TeamSelect.Show();
			return;
		case PlayerPhase.PositionSelect:
			this.uiManager.PositionSelect.Show();
			return;
		case PlayerPhase.Play:
			this.uiManager.Hud.Show();
			this.uiManager.Minimap.Show();
			return;
		default:
			return;
		}
	}

	// Token: 0x0400059E RID: 1438
	private UIManager uiManager;
}
