using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Humanizer;

// Token: 0x020001AF RID: 431
public class UIPlayController : UIViewController<UIPlay>
{
	// Token: 0x06000C65 RID: 3173 RVA: 0x0003A7F0 File Offset: 0x000389F0
	public override void Awake()
	{
		base.Awake();
		this.uiPlay = base.GetComponent<UIPlay>();
		EventManager.AddEventListener("Event_OnPlayerDataChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerDataChanged));
		EventManager.AddEventListener("Event_OnPlayerPartyDataChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerPartyDataChanged));
		EventManager.AddEventListener("Event_OnPlayerGroupDataChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerGroupDataChanged));
		EventManager.AddEventListener("Event_OnPlayerMatchDataChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerMatchDataChanged));
		EventManager.AddEventListener("Event_OnPlayerStatisticsChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerStatisticsChanged));
	}

	// Token: 0x06000C66 RID: 3174 RVA: 0x0003A87D File Offset: 0x00038A7D
	private void Start()
	{
		this.uiPlay.SetThreeVsThreeButtonEnabled(false);
		this.uiPlay.SetFiveVsFiveButtonEnabled(false);
	}

	// Token: 0x06000C67 RID: 3175 RVA: 0x0003A898 File Offset: 0x00038A98
	public override void OnDestroy()
	{
		EventManager.RemoveEventListener("Event_OnPlayerDataChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerDataChanged));
		EventManager.RemoveEventListener("Event_OnPlayerPartyDataChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerPartyDataChanged));
		EventManager.RemoveEventListener("Event_OnPlayerGroupDataChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerGroupDataChanged));
		EventManager.RemoveEventListener("Event_OnPlayerMatchDataChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerMatchDataChanged));
		EventManager.RemoveEventListener("Event_OnPlayerStatisticsChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerStatisticsChanged));
		base.OnDestroy();
	}

	// Token: 0x06000C68 RID: 3176 RVA: 0x0003A91C File Offset: 0x00038B1C
	private void UpdateMatchmakingPlayerButtonState()
	{
		PlayerData playerData = BackendManager.PlayerState.PlayerData;
		PlayerGroupData groupData = BackendManager.PlayerState.GroupData;
		PlayerMatchData matchData = BackendManager.PlayerState.MatchData;
		PlayerPartyData partyData = BackendManager.PlayerState.PartyData;
		bool flag = playerData != null && (partyData == null || (partyData != null && partyData.ownerSteamId == playerData.steamId)) && groupData == null && matchData == null;
		this.uiPlay.SetThreeVsThreeButtonEnabled(flag);
		this.uiPlay.SetFiveVsFiveButtonEnabled(flag);
	}

	// Token: 0x06000C69 RID: 3177 RVA: 0x0003A998 File Offset: 0x00038B98
	private void Event_OnPlayerStatisticsChanged(Dictionary<string, object> message)
	{
		PlayerStatistics playerStatistics = (PlayerStatistics)message["newPlayerStatistics"];
		PoolStatistics poolStatistics = playerStatistics.matchmakingManager.pools.FirstOrDefault((PoolStatistics pool) => pool.id == "3v3");
		PoolStatistics poolStatistics2 = playerStatistics.matchmakingManager.pools.FirstOrDefault((PoolStatistics pool) => pool.id == "5v5");
		if (poolStatistics != null)
		{
			string arg = "NO DATA";
			if (poolStatistics.averageMatchingDuration != null)
			{
				arg = TimeSpan.FromMilliseconds(poolStatistics.averageMatchingDuration.Value).Humanize(2, CultureInfo.InvariantCulture, TimeUnit.Week, TimeUnit.Second, ", ", false);
			}
			this.uiPlay.SetThreeVsThreeButtonDescription(string.Format("IN QUEUE: {0}<br>EST. MATCHING TIME: {1}", poolStatistics.groupPlayers, arg));
		}
		if (poolStatistics2 != null)
		{
			string arg2 = "NO DATA";
			if (poolStatistics2.averageMatchingDuration != null)
			{
				arg2 = TimeSpan.FromMilliseconds(poolStatistics2.averageMatchingDuration.Value).Humanize(2, CultureInfo.InvariantCulture, TimeUnit.Week, TimeUnit.Second, ", ", false);
			}
			this.uiPlay.SetFiveVsFiveButtonDescription(string.Format("IN QUEUE: {0}<br>EST. MATCHING TIME: {1}", poolStatistics2.groupPlayers, arg2));
		}
		this.uiPlay.SetStatistics(playerStatistics.playerManager.players);
	}

	// Token: 0x06000C6A RID: 3178 RVA: 0x0003AAF5 File Offset: 0x00038CF5
	private void Event_OnPlayerDataChanged(Dictionary<string, object> message)
	{
		this.UpdateMatchmakingPlayerButtonState();
	}

	// Token: 0x06000C6B RID: 3179 RVA: 0x0003AAF5 File Offset: 0x00038CF5
	private void Event_OnPlayerPartyDataChanged(Dictionary<string, object> message)
	{
		this.UpdateMatchmakingPlayerButtonState();
	}

	// Token: 0x06000C6C RID: 3180 RVA: 0x0003AAF5 File Offset: 0x00038CF5
	private void Event_OnPlayerGroupDataChanged(Dictionary<string, object> message)
	{
		this.UpdateMatchmakingPlayerButtonState();
	}

	// Token: 0x06000C6D RID: 3181 RVA: 0x0003AAF5 File Offset: 0x00038CF5
	private void Event_OnPlayerMatchDataChanged(Dictionary<string, object> message)
	{
		this.UpdateMatchmakingPlayerButtonState();
	}

	// Token: 0x0400075D RID: 1885
	private UIPlay uiPlay;
}
