using System;
using System.Collections.Generic;

// Token: 0x0200019E RID: 414
public class UIMatchmakingController : UIViewController<UIMatchmaking>
{
	// Token: 0x06000BCB RID: 3019 RVA: 0x00037868 File Offset: 0x00035A68
	public override void Awake()
	{
		base.Awake();
		this.uiMatchmaking = base.GetComponent<UIMatchmaking>();
		EventManager.AddEventListener("Event_OnPlayerGroupDataChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerGroupDataChanged));
		EventManager.AddEventListener("Event_OnPlayerMatchDataChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerMatchDataChanged));
		EventManager.AddEventListener("Event_OnConnectionStateChanged", new Action<Dictionary<string, object>>(this.Event_OnConnectionStateChanged));
		EventManager.AddEventListener("Event_OnMatchingTickerStarted", new Action<Dictionary<string, object>>(this.Event_OnMatchingTickerStarted));
		EventManager.AddEventListener("Event_OnMatchingTickerTick", new Action<Dictionary<string, object>>(this.Event_OnMatchingTickerTick));
		EventManager.AddEventListener("Event_OnMatchingTickerStopped", new Action<Dictionary<string, object>>(this.Event_OnMatchingTickerStopped));
		EventManager.AddEventListener("Event_OnMatchJoinTimeoutTickerStarted", new Action<Dictionary<string, object>>(this.Event_OnMatchJoinTimeoutTickerStarted));
		EventManager.AddEventListener("Event_OnMatchJoinTimeoutTickerTick", new Action<Dictionary<string, object>>(this.Event_OnMatchJoinTimeoutTickerTick));
		EventManager.AddEventListener("Event_OnMatchJoinTimeoutTickerStopped", new Action<Dictionary<string, object>>(this.Event_OnMatchJoinTimeoutTickerStopped));
	}

	// Token: 0x06000BCC RID: 3020 RVA: 0x00037950 File Offset: 0x00035B50
	private void Start()
	{
		this.uiMatchmaking.SetMatchingVisibility(false);
		this.uiMatchmaking.SetMatchingPhaseText(string.Empty);
		this.uiMatchmaking.SetMatchingTimeVisibility(false);
		this.uiMatchmaking.SetMatchingTimeText(0);
		this.uiMatchmaking.SetMatchingConnectButtonVisibility(false);
		this.uiMatchmaking.SetMatchingCloseButtonVisibility(false);
	}

	// Token: 0x06000BCD RID: 3021 RVA: 0x000379AC File Offset: 0x00035BAC
	public override void OnDestroy()
	{
		base.OnDestroy();
		EventManager.RemoveEventListener("Event_OnPlayerGroupDataChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerGroupDataChanged));
		EventManager.RemoveEventListener("Event_OnPlayerMatchDataChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerMatchDataChanged));
		EventManager.RemoveEventListener("Event_OnConnectionStateChanged", new Action<Dictionary<string, object>>(this.Event_OnConnectionStateChanged));
		EventManager.RemoveEventListener("Event_OnMatchingTickerStarted", new Action<Dictionary<string, object>>(this.Event_OnMatchingTickerStarted));
		EventManager.RemoveEventListener("Event_OnMatchingTickerTick", new Action<Dictionary<string, object>>(this.Event_OnMatchingTickerTick));
		EventManager.RemoveEventListener("Event_OnMatchingTickerStopped", new Action<Dictionary<string, object>>(this.Event_OnMatchingTickerStopped));
		EventManager.RemoveEventListener("Event_OnMatchJoinTimeoutTickerStarted", new Action<Dictionary<string, object>>(this.Event_OnMatchJoinTimeoutTickerStarted));
		EventManager.RemoveEventListener("Event_OnMatchJoinTimeoutTickerTick", new Action<Dictionary<string, object>>(this.Event_OnMatchJoinTimeoutTickerTick));
		EventManager.RemoveEventListener("Event_OnMatchJoinTimeoutTickerStopped", new Action<Dictionary<string, object>>(this.Event_OnMatchJoinTimeoutTickerStopped));
	}

	// Token: 0x06000BCE RID: 3022 RVA: 0x00037A88 File Offset: 0x00035C88
	private void UpdateMatching()
	{
		bool groupData = BackendManager.PlayerState.GroupData != null;
		PlayerMatchData matchData = BackendManager.PlayerState.MatchData;
		if (groupData)
		{
			this.uiMatchmaking.SetMatchingVisibility(true);
			this.uiMatchmaking.SetMatchingPhaseText("LOOKING FOR A MATCH...");
			this.uiMatchmaking.SetMatchingConnectButtonVisibility(false);
			this.uiMatchmaking.SetMatchingCloseButtonVisibility(true);
			return;
		}
		if (matchData == null || BackendUtils.IsConnectedToMatchEndPoint())
		{
			this.uiMatchmaking.SetMatchingVisibility(false);
			this.uiMatchmaking.SetMatchingPhaseText(string.Empty);
			this.uiMatchmaking.SetMatchingConnectButtonVisibility(false);
			this.uiMatchmaking.SetMatchingCloseButtonVisibility(false);
			return;
		}
		this.uiMatchmaking.SetMatchingVisibility(true);
		this.uiMatchmaking.SetMatchingCloseButtonVisibility(false);
		if (matchData.endPoint == null)
		{
			this.uiMatchmaking.SetMatchingPhaseText("MATCH FOUND! DEPLOYING MATCH SERVER...");
			this.uiMatchmaking.SetMatchingConnectButtonVisibility(false);
			return;
		}
		this.uiMatchmaking.SetMatchingPhaseText("MATCH READY!");
		this.uiMatchmaking.SetMatchingConnectButtonVisibility(true);
	}

	// Token: 0x06000BCF RID: 3023 RVA: 0x00037B7F File Offset: 0x00035D7F
	private void Event_OnPlayerGroupDataChanged(Dictionary<string, object> message)
	{
		this.UpdateMatching();
	}

	// Token: 0x06000BD0 RID: 3024 RVA: 0x00037B7F File Offset: 0x00035D7F
	private void Event_OnPlayerMatchDataChanged(Dictionary<string, object> message)
	{
		this.UpdateMatching();
	}

	// Token: 0x06000BD1 RID: 3025 RVA: 0x00037B7F File Offset: 0x00035D7F
	private void Event_OnConnectionStateChanged(Dictionary<string, object> message)
	{
		this.UpdateMatching();
	}

	// Token: 0x06000BD2 RID: 3026 RVA: 0x00037B88 File Offset: 0x00035D88
	private void Event_OnMatchingTickerStarted(Dictionary<string, object> message)
	{
		int matchingTimeText = (int)message["startingTick"];
		this.uiMatchmaking.SetMatchingTimeVisibility(true);
		this.uiMatchmaking.SetMatchingTimeText(matchingTimeText);
	}

	// Token: 0x06000BD3 RID: 3027 RVA: 0x00037BC0 File Offset: 0x00035DC0
	private void Event_OnMatchingTickerTick(Dictionary<string, object> message)
	{
		int matchingTimeText = (int)message["tick"];
		this.uiMatchmaking.SetMatchingTimeText(matchingTimeText);
	}

	// Token: 0x06000BD4 RID: 3028 RVA: 0x00037BEA File Offset: 0x00035DEA
	private void Event_OnMatchingTickerStopped(Dictionary<string, object> message)
	{
		this.uiMatchmaking.SetMatchingTimeVisibility(false);
		this.uiMatchmaking.SetMatchingTimeText(0);
	}

	// Token: 0x06000BD5 RID: 3029 RVA: 0x00037C04 File Offset: 0x00035E04
	private void Event_OnMatchJoinTimeoutTickerStarted(Dictionary<string, object> message)
	{
		int matchingTimeText = (int)message["startingTick"];
		this.uiMatchmaking.SetMatchingTimeVisibility(true);
		this.uiMatchmaking.SetMatchingTimeText(matchingTimeText);
	}

	// Token: 0x06000BD6 RID: 3030 RVA: 0x00037C3C File Offset: 0x00035E3C
	private void Event_OnMatchJoinTimeoutTickerTick(Dictionary<string, object> message)
	{
		int matchingTimeText = (int)message["tick"];
		this.uiMatchmaking.SetMatchingTimeText(matchingTimeText);
	}

	// Token: 0x06000BD7 RID: 3031 RVA: 0x00037BEA File Offset: 0x00035DEA
	private void Event_OnMatchJoinTimeoutTickerStopped(Dictionary<string, object> message)
	{
		this.uiMatchmaking.SetMatchingTimeVisibility(false);
		this.uiMatchmaking.SetMatchingTimeText(0);
	}

	// Token: 0x040006FD RID: 1789
	private UIMatchmaking uiMatchmaking;
}
