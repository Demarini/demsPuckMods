using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening;

// Token: 0x0200008B RID: 139
public static class BackendManager
{
	// Token: 0x17000079 RID: 121
	// (get) Token: 0x06000492 RID: 1170 RVA: 0x00018D81 File Offset: 0x00016F81
	// (set) Token: 0x06000493 RID: 1171 RVA: 0x00018D88 File Offset: 0x00016F88
	public static PlayerState PlayerState
	{
		get
		{
			return BackendManager.playerState;
		}
		set
		{
			if (BackendManager.playerState.Equals(value))
			{
				return;
			}
			PlayerState oldPlayerState = BackendManager.playerState;
			BackendManager.playerState = value;
			BackendManager.OnPlayerStateChanged(oldPlayerState, BackendManager.playerState);
		}
	}

	// Token: 0x1700007A RID: 122
	// (get) Token: 0x06000494 RID: 1172 RVA: 0x00018DAD File Offset: 0x00016FAD
	// (set) Token: 0x06000495 RID: 1173 RVA: 0x00018DB4 File Offset: 0x00016FB4
	public static ServerState ServerState
	{
		get
		{
			return BackendManager.serverState;
		}
		set
		{
			if (BackendManager.serverState.Equals(value))
			{
				return;
			}
			ServerState oldServerState = BackendManager.serverState;
			BackendManager.serverState = value;
			BackendManager.OnServerStateChanged(oldServerState, BackendManager.serverState);
		}
	}

	// Token: 0x1700007B RID: 123
	// (get) Token: 0x06000496 RID: 1174 RVA: 0x00018DD9 File Offset: 0x00016FD9
	// (set) Token: 0x06000497 RID: 1175 RVA: 0x00018DE0 File Offset: 0x00016FE0
	public static TransactionState TransactionState
	{
		get
		{
			return BackendManager.transactionState;
		}
		set
		{
			if (BackendManager.transactionState.Equals(value))
			{
				return;
			}
			TransactionState transactionState = BackendManager.transactionState;
			BackendManager.transactionState = value;
			EventManager.TriggerEvent("Event_OnTransactionStateChanged", new Dictionary<string, object>
			{
				{
					"oldTransactionState",
					transactionState
				},
				{
					"newTransactionState",
					BackendManager.transactionState
				}
			});
		}
	}

	// Token: 0x06000498 RID: 1176 RVA: 0x00018E3C File Offset: 0x0001703C
	public static void Initialize()
	{
		BackendManagerController.Initialize();
	}

	// Token: 0x06000499 RID: 1177 RVA: 0x00018E43 File Offset: 0x00017043
	public static void Dispose()
	{
		BackendManagerController.Dispose();
	}

	// Token: 0x0600049A RID: 1178 RVA: 0x00018E4C File Offset: 0x0001704C
	public static void SetPlayerState(Dictionary<string, object> updates)
	{
		BackendManager.PlayerState = new PlayerState
		{
			AuthenticationPhase = (updates.ContainsKey("authenticationPhase") ? ((AuthenticationPhase)updates["authenticationPhase"]) : BackendManager.PlayerState.AuthenticationPhase),
			PlayerData = (updates.ContainsKey("playerData") ? ((PlayerData)updates["playerData"]) : BackendManager.PlayerState.PlayerData),
			PartyData = (updates.ContainsKey("partyData") ? ((PlayerPartyData)updates["partyData"]) : BackendManager.PlayerState.PartyData),
			GroupData = (updates.ContainsKey("groupData") ? ((PlayerGroupData)updates["groupData"]) : BackendManager.PlayerState.GroupData),
			MatchData = (updates.ContainsKey("matchData") ? ((PlayerMatchData)updates["matchData"]) : BackendManager.PlayerState.MatchData),
			PlayerStatistics = (updates.ContainsKey("playerStatistics") ? ((PlayerStatistics)updates["playerStatistics"]) : BackendManager.PlayerState.PlayerStatistics),
			Key = (updates.ContainsKey("key") ? ((string)updates["key"]) : BackendManager.PlayerState.Key)
		};
	}

	// Token: 0x0600049B RID: 1179 RVA: 0x00018FB8 File Offset: 0x000171B8
	public static void SetServerState(Dictionary<string, object> updates)
	{
		BackendManager.ServerState = new ServerState
		{
			AuthenticationPhase = (updates.ContainsKey("authenticationPhase") ? ((AuthenticationPhase)updates["authenticationPhase"]) : BackendManager.ServerState.AuthenticationPhase),
			ServerData = (updates.ContainsKey("serverData") ? ((ServerData)updates["serverData"]) : BackendManager.ServerState.ServerData),
			MatchData = (updates.ContainsKey("matchData") ? ((ServerMatchData)updates["matchData"]) : BackendManager.ServerState.MatchData)
		};
	}

	// Token: 0x0600049C RID: 1180 RVA: 0x00019064 File Offset: 0x00017264
	public static void SetTransactionState(Dictionary<string, object> updates)
	{
		BackendManager.TransactionState = new TransactionState
		{
			Phase = (updates.ContainsKey("phase") ? ((TransactionPhase)updates["phase"]) : BackendManager.TransactionState.Phase)
		};
	}

	// Token: 0x0600049D RID: 1181 RVA: 0x000190B0 File Offset: 0x000172B0
	private static void OnPlayerStateChanged(PlayerState oldPlayerState, PlayerState newPlayerState)
	{
		EventManager.TriggerEvent("Event_OnPlayerStateChanged", new Dictionary<string, object>
		{
			{
				"oldPlayerState",
				oldPlayerState
			},
			{
				"newPlayerState",
				newPlayerState
			}
		});
		if (oldPlayerState.PlayerData != newPlayerState.PlayerData)
		{
			EventManager.TriggerEvent("Event_OnPlayerDataChanged", new Dictionary<string, object>
			{
				{
					"oldPlayerData",
					oldPlayerState.PlayerData
				},
				{
					"newPlayerData",
					newPlayerState.PlayerData
				}
			});
			if (newPlayerState.PlayerData != null)
			{
				bool flag = BackendUtils.GetActivePlayerDataBan(oldPlayerState.PlayerData) != null;
				bool flag2 = BackendUtils.GetActivePlayerDataMute(oldPlayerState.PlayerData) != null;
				bool flag3 = BackendUtils.GetActivePlayerDataCooldown(oldPlayerState.PlayerData) != null;
				PlayerBan activePlayerDataBan = BackendUtils.GetActivePlayerDataBan(newPlayerState.PlayerData);
				PlayerMute activePlayerDataMute = BackendUtils.GetActivePlayerDataMute(newPlayerState.PlayerData);
				PlayerCooldown activePlayerDataCooldown = BackendUtils.GetActivePlayerDataCooldown(newPlayerState.PlayerData);
				bool flag4 = activePlayerDataBan != null;
				bool flag5 = activePlayerDataMute != null;
				bool flag6 = activePlayerDataCooldown != null;
				if (flag4 && !flag)
				{
					EventManager.TriggerEvent("Event_OnPlayerBanned", new Dictionary<string, object>
					{
						{
							"reason",
							activePlayerDataBan.reason
						},
						{
							"expiresAt",
							activePlayerDataBan.expiresAt
						}
					});
				}
				else if (!flag4 && flag)
				{
					EventManager.TriggerEvent("Event_OnPlayerUnbanned", null);
				}
				if (flag5 && !flag2)
				{
					EventManager.TriggerEvent("Event_OnPlayerMuted", new Dictionary<string, object>
					{
						{
							"reason",
							activePlayerDataMute.reason
						},
						{
							"expiresAt",
							activePlayerDataMute.expiresAt
						}
					});
				}
				else if (!flag5 && flag2)
				{
					EventManager.TriggerEvent("Event_OnPlayerUnmuted", null);
				}
				if (flag6 && !flag3)
				{
					EventManager.TriggerEvent("Event_OnPlayerCooldown", new Dictionary<string, object>
					{
						{
							"expiresAt",
							activePlayerDataCooldown.expiresAt
						}
					});
				}
				else if (!flag6 && flag3)
				{
					EventManager.TriggerEvent("Event_OnPlayerCooldownExpired", null);
				}
			}
		}
		if (oldPlayerState.PartyData != newPlayerState.PartyData)
		{
			EventManager.TriggerEvent("Event_OnPlayerPartyDataChanged", new Dictionary<string, object>
			{
				{
					"oldPlayerPartyData",
					oldPlayerState.PartyData
				},
				{
					"newPlayerPartyData",
					newPlayerState.PartyData
				}
			});
		}
		if (oldPlayerState.GroupData != newPlayerState.GroupData)
		{
			EventManager.TriggerEvent("Event_OnPlayerGroupDataChanged", new Dictionary<string, object>
			{
				{
					"oldPlayerGroupData",
					oldPlayerState.GroupData
				},
				{
					"newPlayerGroupData",
					newPlayerState.GroupData
				}
			});
			bool flag7 = oldPlayerState.GroupData == null && newPlayerState.GroupData != null;
			bool flag8 = oldPlayerState.GroupData != null && newPlayerState.GroupData == null;
			if (flag7)
			{
				BackendManager.StartMatchingTicker(0);
			}
			else if (flag8)
			{
				BackendManager.StopMatchingTicker();
			}
		}
		if (oldPlayerState.MatchData != newPlayerState.MatchData)
		{
			EventManager.TriggerEvent("Event_OnPlayerMatchDataChanged", new Dictionary<string, object>
			{
				{
					"oldPlayerMatchData",
					oldPlayerState.MatchData
				},
				{
					"newPlayerMatchData",
					newPlayerState.MatchData
				}
			});
			PlayerMatchData matchData = oldPlayerState.MatchData;
			bool flag9;
			if (matchData == null || matchData.JoinTimeoutRemainingSeconds == null)
			{
				PlayerMatchData matchData2 = newPlayerState.MatchData;
				flag9 = (matchData2 != null && matchData2.JoinTimeoutRemainingSeconds != null);
			}
			else
			{
				flag9 = false;
			}
			PlayerMatchData matchData3 = oldPlayerState.MatchData;
			bool flag10;
			if (matchData3 != null && matchData3.JoinTimeoutRemainingSeconds != null)
			{
				PlayerMatchData matchData4 = newPlayerState.MatchData;
				flag10 = (matchData4 == null || matchData4.JoinTimeoutRemainingSeconds == null);
			}
			else
			{
				flag10 = false;
			}
			bool flag11 = flag10;
			if (flag9)
			{
				BackendManager.StartMatchJoinTimeoutTicker(newPlayerState.MatchData.JoinTimeoutRemainingSeconds.Value);
			}
			else if (flag11)
			{
				BackendManager.StopMatchJoinTimeoutTicker();
			}
		}
		if (oldPlayerState.PlayerStatistics != newPlayerState.PlayerStatistics)
		{
			EventManager.TriggerEvent("Event_OnPlayerStatisticsChanged", new Dictionary<string, object>
			{
				{
					"oldPlayerStatistics",
					oldPlayerState.PlayerStatistics
				},
				{
					"newPlayerStatistics",
					newPlayerState.PlayerStatistics
				}
			});
		}
		if (oldPlayerState.Key != newPlayerState.Key)
		{
			EventManager.TriggerEvent("Event_OnPlayerKeyChanged", new Dictionary<string, object>
			{
				{
					"oldKey",
					oldPlayerState.Key
				},
				{
					"newKey",
					newPlayerState.Key
				}
			});
		}
	}

	// Token: 0x0600049E RID: 1182 RVA: 0x000194B4 File Offset: 0x000176B4
	private static void OnServerStateChanged(ServerState oldServerState, ServerState newServerState)
	{
		EventManager.TriggerEvent("Event_OnServerStateChanged", new Dictionary<string, object>
		{
			{
				"oldServerState",
				oldServerState
			},
			{
				"newServerState",
				newServerState
			}
		});
		if (oldServerState.ServerData != newServerState.ServerData)
		{
			EventManager.TriggerEvent("Event_OnServerDataChanged", new Dictionary<string, object>
			{
				{
					"oldServerData",
					oldServerState.ServerData
				},
				{
					"newServerData",
					newServerState.ServerData
				}
			});
		}
		if (oldServerState.MatchData != newServerState.MatchData)
		{
			EventManager.TriggerEvent("Event_OnServerMatchDataChanged", new Dictionary<string, object>
			{
				{
					"oldServerMatchData",
					oldServerState.MatchData
				},
				{
					"newServerMatchData",
					newServerState.MatchData
				}
			});
		}
	}

	// Token: 0x0600049F RID: 1183 RVA: 0x00019570 File Offset: 0x00017770
	private static void StartMatchingTicker(int startingTick)
	{
		EventManager.TriggerEvent("Event_OnMatchingTickerStarted", new Dictionary<string, object>
		{
			{
				"startingTick",
				startingTick
			}
		});
		BackendManager.matchingTick = startingTick;
		Tween tween = BackendManager.matchingTickerTween;
		if (tween != null)
		{
			tween.Kill(false);
		}
		BackendManager.matchingTickerTween = DOVirtual.DelayedCall(1f, new TweenCallback(BackendManager.<StartMatchingTicker>g__Tick|23_0), true).SetLoops(-1);
	}

	// Token: 0x060004A0 RID: 1184 RVA: 0x000195D6 File Offset: 0x000177D6
	private static void StopMatchingTicker()
	{
		BackendManager.matchingTick = 0;
		Tween tween = BackendManager.matchingTickerTween;
		if (tween != null)
		{
			tween.Kill(false);
		}
		BackendManager.matchingTickerTween = null;
		EventManager.TriggerEvent("Event_OnMatchingTickerStopped", null);
	}

	// Token: 0x060004A1 RID: 1185 RVA: 0x00019600 File Offset: 0x00017800
	private static void StartMatchJoinTimeoutTicker(int startingTick)
	{
		EventManager.TriggerEvent("Event_OnMatchJoinTimeoutTickerStarted", new Dictionary<string, object>
		{
			{
				"startingTick",
				startingTick
			}
		});
		BackendManager.joinTimeoutTick = startingTick;
		Tween tween = BackendManager.matchJoinTimeoutTickerTween;
		if (tween != null)
		{
			tween.Kill(false);
		}
		BackendManager.matchJoinTimeoutTickerTween = DOVirtual.DelayedCall(1f, new TweenCallback(BackendManager.<StartMatchJoinTimeoutTicker>g__Tick|25_0), true).SetLoops(-1);
	}

	// Token: 0x060004A2 RID: 1186 RVA: 0x00019666 File Offset: 0x00017866
	private static void StopMatchJoinTimeoutTicker()
	{
		BackendManager.joinTimeoutTick = 0;
		Tween tween = BackendManager.matchJoinTimeoutTickerTween;
		if (tween != null)
		{
			tween.Kill(false);
		}
		BackendManager.matchJoinTimeoutTickerTween = null;
		EventManager.TriggerEvent("Event_OnMatchJoinTimeoutTickerStopped", null);
	}

	// Token: 0x060004A3 RID: 1187 RVA: 0x00019690 File Offset: 0x00017890
	[CompilerGenerated]
	internal static void <StartMatchingTicker>g__Tick|23_0()
	{
		BackendManager.matchingTick++;
		EventManager.TriggerEvent("Event_OnMatchingTickerTick", new Dictionary<string, object>
		{
			{
				"tick",
				BackendManager.matchingTick
			}
		});
	}

	// Token: 0x060004A4 RID: 1188 RVA: 0x000196C2 File Offset: 0x000178C2
	[CompilerGenerated]
	internal static void <StartMatchJoinTimeoutTicker>g__Tick|25_0()
	{
		if (BackendManager.joinTimeoutTick <= 0)
		{
			BackendManager.StopMatchJoinTimeoutTicker();
			return;
		}
		BackendManager.joinTimeoutTick--;
		EventManager.TriggerEvent("Event_OnMatchJoinTimeoutTickerTick", new Dictionary<string, object>
		{
			{
				"tick",
				BackendManager.joinTimeoutTick
			}
		});
	}

	// Token: 0x040002D6 RID: 726
	private static PlayerState playerState;

	// Token: 0x040002D7 RID: 727
	private static ServerState serverState;

	// Token: 0x040002D8 RID: 728
	private static TransactionState transactionState;

	// Token: 0x040002D9 RID: 729
	private static Tween matchingTickerTween;

	// Token: 0x040002DA RID: 730
	private static int matchingTick;

	// Token: 0x040002DB RID: 731
	private static Tween matchJoinTimeoutTickerTween;

	// Token: 0x040002DC RID: 732
	private static int joinTimeoutTick;
}
