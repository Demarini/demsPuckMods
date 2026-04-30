using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

// Token: 0x0200008C RID: 140
public static class BackendManagerController
{
	// Token: 0x060004A5 RID: 1189 RVA: 0x00019704 File Offset: 0x00017904
	public static void Initialize()
	{
		EventManager.AddEventListener("Event_OnGetTicketForWebApiResponse", new Action<Dictionary<string, object>>(BackendManagerController.Event_OnGetTicketForWebApiResponse));
		EventManager.AddEventListener("Event_OnFooterClickCreateParty", new Action<Dictionary<string, object>>(BackendManagerController.Event_OnFooterClickCreateParty));
		EventManager.AddEventListener("Event_OnFooterClickLeaveParty", new Action<Dictionary<string, object>>(BackendManagerController.Event_OnFooterClickLeaveParty));
		EventManager.AddEventListener("Event_OnFooterClickDisbandParty", new Action<Dictionary<string, object>>(BackendManagerController.Event_OnFooterClickDisbandParty));
		EventManager.AddEventListener("Event_OnLobbyCreated", new Action<Dictionary<string, object>>(BackendManagerController.Event_OnLobbyCreated));
		EventManager.AddEventListener("Event_OnLobbyEntered", new Action<Dictionary<string, object>>(BackendManagerController.Event_OnLobbyEntered));
		EventManager.AddEventListener("Event_OnPlayClickThreeVsThree", new Action<Dictionary<string, object>>(BackendManagerController.Event_OnPlayClickThreeVsThree));
		EventManager.AddEventListener("Event_OnPlayClickFiveVsFive", new Action<Dictionary<string, object>>(BackendManagerController.Event_OnPlayClickFiveVsFive));
		EventManager.AddEventListener("Event_OnMatchmakingMatchingClickClose", new Action<Dictionary<string, object>>(BackendManagerController.Event_OnMatchmakingMatchingClickClose));
		EventManager.AddEventListener("Event_OnPopupClickOk", new Action<Dictionary<string, object>>(BackendManagerController.Event_OnPopupClickOk));
		EventManager.AddEventListener("Event_OnAppearanceClickPurchaseItem", new Action<Dictionary<string, object>>(BackendManagerController.Event_OnAppearanceClickPurchaseItem));
		EventManager.AddEventListener("Event_OnMicroTxnAuthorizationResponse", new Action<Dictionary<string, object>>(BackendManagerController.Event_OnMicroTxnAuthorizationResponse));
		EventManager.AddEventListener("Event_OnNewServerClickStart", new Action<Dictionary<string, object>>(BackendManagerController.Event_OnNewServerClickStart));
		WebSocketManager.AddMessageListener("disconnected", new Action<Dictionary<string, object>>(BackendManagerController.WebSocket_Event_OnDisconnected));
		WebSocketManager.AddMessageListener("playerAuthenticateResponse", new Action<Dictionary<string, object>>(BackendManagerController.WebSocket_Event_OnPlayerAuthenticateResponse));
		WebSocketManager.AddMessageListener("playerData", new Action<Dictionary<string, object>>(BackendManagerController.WebSocket_Event_OnPlayerData));
		WebSocketManager.AddMessageListener("playerPartyData", new Action<Dictionary<string, object>>(BackendManagerController.WebSocket_Event_OnPlayerPartyData));
		WebSocketManager.AddMessageListener("playerGroupData", new Action<Dictionary<string, object>>(BackendManagerController.WebSocket_Event_OnPlayerGroupData));
		WebSocketManager.AddMessageListener("playerMatchData", new Action<Dictionary<string, object>>(BackendManagerController.WebSocket_Event_OnPlayerMatchData));
		WebSocketManager.AddMessageListener("playerStatistics", new Action<Dictionary<string, object>>(BackendManagerController.WebSocket_Event_OnPlayerStatistics));
		WebSocketManager.AddMessageListener("playerKey", new Action<Dictionary<string, object>>(BackendManagerController.WebSocket_Event_OnPlayerKey));
		WebSocketManager.AddMessageListener("playerBeaconRttRequest", new Action<Dictionary<string, object>>(BackendManagerController.WebSocket_Event_OnPlayerBeaconRttRequest));
		WebSocketManager.AddMessageListener("playerStartTransactionResponse", new Action<Dictionary<string, object>>(BackendManagerController.WebSocket_Event_OnPlayerStartTransactionResponse));
		WebSocketManager.AddMessageListener("playerFinalizeTransactionResponse", new Action<Dictionary<string, object>>(BackendManagerController.WebSocket_Event_OnPlayerFinalizeTransactionResponse));
		WebSocketManager.AddMessageListener("serverAuthenticateResponse", new Action<Dictionary<string, object>>(BackendManagerController.WebSocket_Event_OnServerAuthenticateResponse));
		WebSocketManager.AddMessageListener("serverUnauthenticateResponse", new Action<Dictionary<string, object>>(BackendManagerController.WebSocket_Event_OnServerUnauthenticateResponse));
		WebSocketManager.AddMessageListener("serverData", new Action<Dictionary<string, object>>(BackendManagerController.WebSocket_Event_OnServerData));
		WebSocketManager.AddMessageListener("serverMatchData", new Action<Dictionary<string, object>>(BackendManagerController.WebSocket_Event_OnServerMatchData));
	}

	// Token: 0x060004A6 RID: 1190 RVA: 0x0001997C File Offset: 0x00017B7C
	public static void Dispose()
	{
		EventManager.RemoveEventListener("Event_OnGetTicketForWebApiResponse", new Action<Dictionary<string, object>>(BackendManagerController.Event_OnGetTicketForWebApiResponse));
		EventManager.RemoveEventListener("Event_OnFooterClickCreateParty", new Action<Dictionary<string, object>>(BackendManagerController.Event_OnFooterClickCreateParty));
		EventManager.RemoveEventListener("Event_OnFooterClickLeaveParty", new Action<Dictionary<string, object>>(BackendManagerController.Event_OnFooterClickLeaveParty));
		EventManager.RemoveEventListener("Event_OnFooterClickDisbandParty", new Action<Dictionary<string, object>>(BackendManagerController.Event_OnFooterClickDisbandParty));
		EventManager.RemoveEventListener("Event_OnLobbyCreated", new Action<Dictionary<string, object>>(BackendManagerController.Event_OnLobbyCreated));
		EventManager.RemoveEventListener("Event_OnLobbyEntered", new Action<Dictionary<string, object>>(BackendManagerController.Event_OnLobbyEntered));
		EventManager.RemoveEventListener("Event_OnPlayClickThreeVsThree", new Action<Dictionary<string, object>>(BackendManagerController.Event_OnPlayClickThreeVsThree));
		EventManager.RemoveEventListener("Event_OnPlayClickFiveVsFive", new Action<Dictionary<string, object>>(BackendManagerController.Event_OnPlayClickFiveVsFive));
		EventManager.RemoveEventListener("Event_OnMatchmakingMatchingClickClose", new Action<Dictionary<string, object>>(BackendManagerController.Event_OnMatchmakingMatchingClickClose));
		EventManager.RemoveEventListener("Event_OnPopupClickOk", new Action<Dictionary<string, object>>(BackendManagerController.Event_OnPopupClickOk));
		EventManager.RemoveEventListener("Event_OnAppearanceClickPurchaseItem", new Action<Dictionary<string, object>>(BackendManagerController.Event_OnAppearanceClickPurchaseItem));
		EventManager.RemoveEventListener("Event_OnMicroTxnAuthorizationResponse", new Action<Dictionary<string, object>>(BackendManagerController.Event_OnMicroTxnAuthorizationResponse));
		EventManager.RemoveEventListener("Event_OnNewServerClickStart", new Action<Dictionary<string, object>>(BackendManagerController.Event_OnNewServerClickStart));
		WebSocketManager.RemoveMessageListener("disconnected", new Action<Dictionary<string, object>>(BackendManagerController.WebSocket_Event_OnDisconnected));
		WebSocketManager.RemoveMessageListener("playerAuthenticateResponse", new Action<Dictionary<string, object>>(BackendManagerController.WebSocket_Event_OnPlayerAuthenticateResponse));
		WebSocketManager.RemoveMessageListener("playerData", new Action<Dictionary<string, object>>(BackendManagerController.WebSocket_Event_OnPlayerData));
		WebSocketManager.RemoveMessageListener("playerPartyData", new Action<Dictionary<string, object>>(BackendManagerController.WebSocket_Event_OnPlayerPartyData));
		WebSocketManager.RemoveMessageListener("playerGroupData", new Action<Dictionary<string, object>>(BackendManagerController.WebSocket_Event_OnPlayerGroupData));
		WebSocketManager.RemoveMessageListener("playerMatchData", new Action<Dictionary<string, object>>(BackendManagerController.WebSocket_Event_OnPlayerMatchData));
		WebSocketManager.RemoveMessageListener("playerStatistics", new Action<Dictionary<string, object>>(BackendManagerController.WebSocket_Event_OnPlayerStatistics));
		WebSocketManager.RemoveMessageListener("playerKey", new Action<Dictionary<string, object>>(BackendManagerController.WebSocket_Event_OnPlayerKey));
		WebSocketManager.RemoveMessageListener("playerBeaconRttRequest", new Action<Dictionary<string, object>>(BackendManagerController.WebSocket_Event_OnPlayerBeaconRttRequest));
		WebSocketManager.RemoveMessageListener("playerStartTransactionResponse", new Action<Dictionary<string, object>>(BackendManagerController.WebSocket_Event_OnPlayerStartTransactionResponse));
		WebSocketManager.RemoveMessageListener("playerFinalizeTransactionResponse", new Action<Dictionary<string, object>>(BackendManagerController.WebSocket_Event_OnPlayerFinalizeTransactionResponse));
		WebSocketManager.RemoveMessageListener("serverAuthenticateResponse", new Action<Dictionary<string, object>>(BackendManagerController.WebSocket_Event_OnServerAuthenticateResponse));
		WebSocketManager.RemoveMessageListener("serverUnauthenticateResponse", new Action<Dictionary<string, object>>(BackendManagerController.WebSocket_Event_OnServerUnauthenticateResponse));
		WebSocketManager.RemoveMessageListener("serverData", new Action<Dictionary<string, object>>(BackendManagerController.WebSocket_Event_OnServerData));
		WebSocketManager.RemoveMessageListener("serverMatchData", new Action<Dictionary<string, object>>(BackendManagerController.WebSocket_Event_OnServerMatchData));
	}

	// Token: 0x060004A7 RID: 1191 RVA: 0x00019BF4 File Offset: 0x00017DF4
	private static int? PingBeacon(Beacon beacon, int connectTimeout, int responseTimeout)
	{
		EndPoint endPoint = new EndPoint(beacon.host, beacon.tcp_port);
		TCPClient tcpClient = new TCPClient(endPoint, connectTimeout, 1000);
		double pingTimestamp = 0.0;
		int? rtt = null;
		ManualResetEventSlim responseEvent = new ManualResetEventSlim(false);
		tcpClient.OnConnected += delegate()
		{
			tcpClient.SendMessage("ping");
		};
		tcpClient.OnMessageSent += delegate(string message)
		{
			pingTimestamp = Utils.GetTimestamp();
		};
		tcpClient.OnMessageReceived += delegate(string message)
		{
			rtt = new int?((int)(Utils.GetTimestamp() - pingTimestamp));
			responseEvent.Set();
		};
		tcpClient.Connect();
		if (tcpClient.IsConnected)
		{
			responseEvent.Wait(responseTimeout);
			tcpClient.Disconnect();
		}
		return rtt;
	}

	// Token: 0x060004A8 RID: 1192 RVA: 0x00019CD0 File Offset: 0x00017ED0
	private static void Event_OnGetTicketForWebApiResponse(Dictionary<string, object> message)
	{
		string value = (string)message["ticket"];
		BackendManager.SetPlayerState(new Dictionary<string, object>
		{
			{
				"authenticationPhase",
				AuthenticationPhase.Authenticating
			}
		});
		WebSocketManager.Emit("playerAuthenticateRequest", new Dictionary<string, object>
		{
			{
				"ticket",
				value
			}
		}, "playerAuthenticateResponse");
	}

	// Token: 0x060004A9 RID: 1193 RVA: 0x00019D29 File Offset: 0x00017F29
	private static void Event_OnFooterClickCreateParty(Dictionary<string, object> message)
	{
		WebSocketManager.Emit("playerCreatePartyRequest", null, "playerCreatePartyResponse");
	}

	// Token: 0x060004AA RID: 1194 RVA: 0x00019D3B File Offset: 0x00017F3B
	private static void Event_OnFooterClickLeaveParty(Dictionary<string, object> message)
	{
		WebSocketManager.Emit("playerLeavePartyRequest", null, "playerLeavePartyResponse");
	}

	// Token: 0x060004AB RID: 1195 RVA: 0x00019D4D File Offset: 0x00017F4D
	private static void Event_OnFooterClickDisbandParty(Dictionary<string, object> message)
	{
		WebSocketManager.Emit("playerDisbandPartyRequest", null, "playerDisbandPartyResponse");
	}

	// Token: 0x060004AC RID: 1196 RVA: 0x00019D60 File Offset: 0x00017F60
	private static void Event_OnLobbyCreated(Dictionary<string, object> message)
	{
		string value = (string)message["lobbyId"];
		WebSocketManager.Emit("playerUpdatePartyRequest", new Dictionary<string, object>
		{
			{
				"steamLobbyId",
				value
			}
		}, "playerUpdatePartyResponse");
	}

	// Token: 0x060004AD RID: 1197 RVA: 0x00019DA0 File Offset: 0x00017FA0
	private static void Event_OnLobbyEntered(Dictionary<string, object> message)
	{
		string text = (string)message["lobbyId"];
		string text2 = (string)message["ownerSteamId"];
		Debug.Log(string.Concat(new string[]
		{
			"[BackendManagerController] Entered lobby ",
			text,
			" owned by ",
			text2,
			" (player's Steam ID: ",
			BackendManager.PlayerState.PlayerData.steamId,
			")"
		}));
		if (text2 != BackendManager.PlayerState.PlayerData.steamId)
		{
			WebSocketManager.Emit("playerJoinPartyRequest", new Dictionary<string, object>
			{
				{
					"steamLobbyId",
					text
				}
			}, "playerJoinPartyResponse");
		}
	}

	// Token: 0x060004AE RID: 1198 RVA: 0x00019E50 File Offset: 0x00018050
	private static void Event_OnPlayClickThreeVsThree(Dictionary<string, object> message)
	{
		WebSocketManager.Emit("playerStartMatchmakingRequest", new Dictionary<string, object>
		{
			{
				"poolId",
				"3v3"
			},
			{
				"maxRtt",
				SettingsManager.MaxMatchmakingPing
			}
		}, "playerStartMatchmakingResponse");
	}

	// Token: 0x060004AF RID: 1199 RVA: 0x00019E8B File Offset: 0x0001808B
	private static void Event_OnPlayClickFiveVsFive(Dictionary<string, object> message)
	{
		WebSocketManager.Emit("playerStartMatchmakingRequest", new Dictionary<string, object>
		{
			{
				"poolId",
				"5v5"
			},
			{
				"maxRtt",
				SettingsManager.MaxMatchmakingPing
			}
		}, "playerStartMatchmakingResponse");
	}

	// Token: 0x060004B0 RID: 1200 RVA: 0x00019EC6 File Offset: 0x000180C6
	private static void Event_OnMatchmakingMatchingClickClose(Dictionary<string, object> message)
	{
		WebSocketManager.Emit("playerStopMatchmakingRequest", null, "playerStopMatchmakingResponse");
	}

	// Token: 0x060004B1 RID: 1201 RVA: 0x00019ED8 File Offset: 0x000180D8
	private static void Event_OnPopupClickOk(Dictionary<string, object> message)
	{
		if ((string)message["name"] == "identity")
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)((IPopupContent)message["content"]).Data;
			string value = (string)dictionary["username"];
			int num = (int)dictionary["number"];
			WebSocketManager.Emit("playerSetIdentityRequest", new Dictionary<string, object>
			{
				{
					"username",
					value
				},
				{
					"number",
					num
				}
			}, "playerSetIdentityResponse");
		}
	}

	// Token: 0x060004B2 RID: 1202 RVA: 0x00019F70 File Offset: 0x00018170
	private static void Event_OnAppearanceClickPurchaseItem(Dictionary<string, object> message)
	{
		Item item = (Item)message["item"];
		BackendManager.SetTransactionState(new Dictionary<string, object>
		{
			{
				"phase",
				TransactionPhase.Starting
			}
		});
		if (!SteamIntegrationManager.IsOverlayEnabled)
		{
			BackendManager.SetTransactionState(new Dictionary<string, object>
			{
				{
					"phase",
					TransactionPhase.None
				}
			});
			return;
		}
		WebSocketManager.Emit("playerStartTransactionRequest", new Dictionary<string, object>
		{
			{
				"itemId",
				item.id
			}
		}, "playerStartTransactionResponse");
	}

	// Token: 0x060004B3 RID: 1203 RVA: 0x00019FF8 File Offset: 0x000181F8
	private static void Event_OnMicroTxnAuthorizationResponse(Dictionary<string, object> message)
	{
		if ((bool)message["authorized"])
		{
			WebSocketManager.Emit("playerFinalizeTransactionRequest", null, "playerFinalizeTransactionResponse");
			return;
		}
		WebSocketManager.Emit("playerCancelTransaction", null, null);
		BackendManager.SetTransactionState(new Dictionary<string, object>
		{
			{
				"phase",
				TransactionPhase.None
			}
		});
	}

	// Token: 0x060004B4 RID: 1204 RVA: 0x0001A050 File Offset: 0x00018250
	private static void Event_OnNewServerClickStart(Dictionary<string, object> message)
	{
		if ((string)message["type"] != "dedicated")
		{
			return;
		}
		string value = (string)message["name"];
		int num = (int)message["maxPlayers"];
		string value2 = (string)message["password"];
		string value3 = (string)message["locationId"];
		WebSocketManager.Emit("playerDeployServerRequest", new Dictionary<string, object>
		{
			{
				"name",
				value
			},
			{
				"maxPlayers",
				num
			},
			{
				"password",
				value2
			},
			{
				"locationId",
				value3
			}
		}, "playerDeployServerResponse");
	}

	// Token: 0x060004B5 RID: 1205 RVA: 0x0001A108 File Offset: 0x00018308
	private static void WebSocket_Event_OnDisconnected(Dictionary<string, object> message)
	{
		BackendManager.SetPlayerState(new Dictionary<string, object>
		{
			{
				"steamId",
				null
			},
			{
				"playerData",
				null
			},
			{
				"partyData",
				null
			},
			{
				"key",
				null
			},
			{
				"authenticationPhase",
				AuthenticationPhase.None
			}
		});
		BackendManager.SetServerState(new Dictionary<string, object>
		{
			{
				"serverData",
				null
			},
			{
				"matchData",
				null
			},
			{
				"authenticationPhase",
				AuthenticationPhase.None
			}
		});
	}

	// Token: 0x060004B6 RID: 1206 RVA: 0x0001A194 File Offset: 0x00018394
	private static void WebSocket_Event_OnPlayerAuthenticateResponse(Dictionary<string, object> message)
	{
		PlayerAuthenticateResponse data = ((InMessage)message["inMessage"]).GetData<PlayerAuthenticateResponse>();
		BackendManager.SetPlayerState(new Dictionary<string, object>
		{
			{
				"authenticationPhase",
				data.success ? AuthenticationPhase.Authenticated : AuthenticationPhase.None
			}
		});
	}

	// Token: 0x060004B7 RID: 1207 RVA: 0x0001A1E0 File Offset: 0x000183E0
	private static void WebSocket_Event_OnPlayerData(Dictionary<string, object> message)
	{
		InMessage inMessage = (InMessage)message["inMessage"];
		BackendManager.SetPlayerState(new Dictionary<string, object>
		{
			{
				"playerData",
				inMessage.GetData<PlayerData>()
			}
		});
	}

	// Token: 0x060004B8 RID: 1208 RVA: 0x0001A21C File Offset: 0x0001841C
	private static void WebSocket_Event_OnPlayerPartyData(Dictionary<string, object> message)
	{
		InMessage inMessage = (InMessage)message["inMessage"];
		BackendManager.SetPlayerState(new Dictionary<string, object>
		{
			{
				"partyData",
				inMessage.GetData<PlayerPartyData>()
			}
		});
	}

	// Token: 0x060004B9 RID: 1209 RVA: 0x0001A258 File Offset: 0x00018458
	private static void WebSocket_Event_OnPlayerGroupData(Dictionary<string, object> message)
	{
		InMessage inMessage = (InMessage)message["inMessage"];
		BackendManager.SetPlayerState(new Dictionary<string, object>
		{
			{
				"groupData",
				inMessage.GetData<PlayerGroupData>()
			}
		});
	}

	// Token: 0x060004BA RID: 1210 RVA: 0x0001A294 File Offset: 0x00018494
	private static void WebSocket_Event_OnPlayerMatchData(Dictionary<string, object> message)
	{
		InMessage inMessage = (InMessage)message["inMessage"];
		BackendManager.SetPlayerState(new Dictionary<string, object>
		{
			{
				"matchData",
				inMessage.GetData<PlayerMatchData>()
			}
		});
	}

	// Token: 0x060004BB RID: 1211 RVA: 0x0001A2D0 File Offset: 0x000184D0
	private static void WebSocket_Event_OnPlayerStatistics(Dictionary<string, object> message)
	{
		InMessage inMessage = (InMessage)message["inMessage"];
		BackendManager.SetPlayerState(new Dictionary<string, object>
		{
			{
				"playerStatistics",
				inMessage.GetData<PlayerStatistics>()
			}
		});
	}

	// Token: 0x060004BC RID: 1212 RVA: 0x0001A30C File Offset: 0x0001850C
	private static void WebSocket_Event_OnPlayerKey(Dictionary<string, object> message)
	{
		InMessage inMessage = (InMessage)message["inMessage"];
		BackendManager.SetPlayerState(new Dictionary<string, object>
		{
			{
				"key",
				inMessage.GetData<string>()
			}
		});
	}

	// Token: 0x060004BD RID: 1213 RVA: 0x0001A345 File Offset: 0x00018545
	private static void WebSocket_Event_OnPlayerBeaconRttRequest(Dictionary<string, object> message)
	{
		BackendManagerController.<>c__DisplayClass24_0 CS$<>8__locals1 = new BackendManagerController.<>c__DisplayClass24_0();
		CS$<>8__locals1.inMessage = (InMessage)message["inMessage"];
		CS$<>8__locals1.beacons = CS$<>8__locals1.inMessage.GetData<Beacon[]>();
		Task.Run(delegate()
		{
			BackendManagerController.<>c__DisplayClass24_0.<<WebSocket_Event_OnPlayerBeaconRttRequest>b__0>d <<WebSocket_Event_OnPlayerBeaconRttRequest>b__0>d;
			<<WebSocket_Event_OnPlayerBeaconRttRequest>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
			<<WebSocket_Event_OnPlayerBeaconRttRequest>b__0>d.<>4__this = CS$<>8__locals1;
			<<WebSocket_Event_OnPlayerBeaconRttRequest>b__0>d.<>1__state = -1;
			<<WebSocket_Event_OnPlayerBeaconRttRequest>b__0>d.<>t__builder.Start<BackendManagerController.<>c__DisplayClass24_0.<<WebSocket_Event_OnPlayerBeaconRttRequest>b__0>d>(ref <<WebSocket_Event_OnPlayerBeaconRttRequest>b__0>d);
			return <<WebSocket_Event_OnPlayerBeaconRttRequest>b__0>d.<>t__builder.Task;
		});
	}

	// Token: 0x060004BE RID: 1214 RVA: 0x0001A384 File Offset: 0x00018584
	private static void WebSocket_Event_OnPlayerStartTransactionResponse(Dictionary<string, object> message)
	{
		if (((InMessage)message["inMessage"]).GetData<PlayerStartTransactionResponse>().success)
		{
			BackendManager.SetTransactionState(new Dictionary<string, object>
			{
				{
					"phase",
					TransactionPhase.Started
				}
			});
			return;
		}
		BackendManager.SetTransactionState(new Dictionary<string, object>
		{
			{
				"phase",
				TransactionPhase.None
			}
		});
	}

	// Token: 0x060004BF RID: 1215 RVA: 0x0001A3E4 File Offset: 0x000185E4
	private static void WebSocket_Event_OnPlayerFinalizeTransactionResponse(Dictionary<string, object> message)
	{
		((InMessage)message["inMessage"]).GetData<PlayerFinalizeTransactionResponse>();
		BackendManager.SetTransactionState(new Dictionary<string, object>
		{
			{
				"phase",
				TransactionPhase.None
			}
		});
	}

	// Token: 0x060004C0 RID: 1216 RVA: 0x0001A418 File Offset: 0x00018618
	private static void WebSocket_Event_OnServerAuthenticateResponse(Dictionary<string, object> message)
	{
		ServerAuthenticateResponse data = ((InMessage)message["inMessage"]).GetData<ServerAuthenticateResponse>();
		BackendManager.SetServerState(new Dictionary<string, object>
		{
			{
				"authenticationPhase",
				data.success ? AuthenticationPhase.Authenticated : AuthenticationPhase.None
			}
		});
	}

	// Token: 0x060004C1 RID: 1217 RVA: 0x0001A461 File Offset: 0x00018661
	private static void WebSocket_Event_OnServerUnauthenticateResponse(Dictionary<string, object> message)
	{
		BackendManager.SetServerState(new Dictionary<string, object>
		{
			{
				"authenticationPhase",
				AuthenticationPhase.None
			}
		});
	}

	// Token: 0x060004C2 RID: 1218 RVA: 0x0001A480 File Offset: 0x00018680
	private static void WebSocket_Event_OnServerData(Dictionary<string, object> message)
	{
		InMessage inMessage = (InMessage)message["inMessage"];
		BackendManager.SetServerState(new Dictionary<string, object>
		{
			{
				"serverData",
				inMessage.GetData<ServerData>()
			}
		});
	}

	// Token: 0x060004C3 RID: 1219 RVA: 0x0001A4BC File Offset: 0x000186BC
	private static void WebSocket_Event_OnServerMatchData(Dictionary<string, object> message)
	{
		InMessage inMessage = (InMessage)message["inMessage"];
		BackendManager.SetServerState(new Dictionary<string, object>
		{
			{
				"matchData",
				inMessage.GetData<ServerMatchData>()
			}
		});
	}
}
