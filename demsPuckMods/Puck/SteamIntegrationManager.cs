using System;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

// Token: 0x020000AC RID: 172
public class SteamIntegrationManager : MonoBehaviourSingleton<SteamIntegrationManager>
{
	// Token: 0x06000504 RID: 1284 RVA: 0x0000A200 File Offset: 0x00008400
	private void Start()
	{
		this.RegisterCallbacks();
	}

	// Token: 0x06000505 RID: 1285 RVA: 0x00020B74 File Offset: 0x0001ED74
	private void RegisterCallbacks()
	{
		if (!MonoBehaviourSingleton<SteamManager>.Instance.IsInitialized)
		{
			return;
		}
		this.GetTicketForWebApiCallback = Callback<GetTicketForWebApiResponse_t>.Create(new Callback<GetTicketForWebApiResponse_t>.DispatchDelegate(this.OnGotAuthTicketForWebApi));
		this.MicroTxnAuthorizationResponse = Callback<MicroTxnAuthorizationResponse_t>.Create(new Callback<MicroTxnAuthorizationResponse_t>.DispatchDelegate(this.OnMicroTxnAuthorizationResponse));
		this.GameRichPresenceJoinRequested = Callback<GameRichPresenceJoinRequested_t>.Create(new Callback<GameRichPresenceJoinRequested_t>.DispatchDelegate(this.OnGameRichPresenceJoinRequested));
		this.NewUrlLaunchParameters = Callback<NewUrlLaunchParameters_t>.Create(new Callback<NewUrlLaunchParameters_t>.DispatchDelegate(this.OnNewUrlLaunchParameters));
	}

	// Token: 0x06000506 RID: 1286 RVA: 0x00020BEC File Offset: 0x0001EDEC
	public void SubscribeItem(ulong itemId)
	{
		if (!MonoBehaviourSingleton<SteamManager>.Instance.IsInitialized)
		{
			return;
		}
		SteamUGC.SubscribeItem(new PublishedFileId_t
		{
			m_PublishedFileId = itemId
		});
	}

	// Token: 0x06000507 RID: 1287 RVA: 0x00020C20 File Offset: 0x0001EE20
	public void DownloadItem(ulong itemId)
	{
		if (!MonoBehaviourSingleton<SteamManager>.Instance.IsInitialized)
		{
			return;
		}
		SteamUGC.DownloadItem(new PublishedFileId_t
		{
			m_PublishedFileId = itemId
		}, true);
	}

	// Token: 0x06000508 RID: 1288 RVA: 0x0000A208 File Offset: 0x00008408
	public void SetRichPresenceMainMenu()
	{
		if (!MonoBehaviourSingleton<SteamManager>.Instance.IsInitialized)
		{
			return;
		}
		SteamFriends.ClearRichPresence();
		SteamFriends.SetRichPresence("steam_display", "#Status_MainMenu");
		SteamFriends.SetRichPresence("status", "In the changing room");
	}

	// Token: 0x06000509 RID: 1289 RVA: 0x00020C54 File Offset: 0x0001EE54
	public void SetRichPresenceSpectating(Server server, int playerCount)
	{
		if (!MonoBehaviourSingleton<SteamManager>.Instance.IsInitialized)
		{
			return;
		}
		SteamFriends.SetRichPresence("steam_player_group", string.Format("{0}:{1}", server.IpAddress, server.Port));
		SteamFriends.SetRichPresence("steam_player_group_size", string.Format("{0}", playerCount));
		SteamFriends.SetRichPresence("steam_display", "#Status_Spectating");
		SteamFriends.SetRichPresence("status", "Spectating");
		SteamFriends.SetRichPresence("connect", string.Format("+ipAddress {0} +port {1}", server.IpAddress, server.Port));
	}

	// Token: 0x0600050A RID: 1290 RVA: 0x00020D00 File Offset: 0x0001EF00
	public void SetRichPresencePlaying(Server server, int playerCount)
	{
		if (!MonoBehaviourSingleton<SteamManager>.Instance.IsInitialized)
		{
			return;
		}
		SteamFriends.SetRichPresence("steam_player_group", string.Format("{0}:{1}", server.IpAddress, server.Port));
		SteamFriends.SetRichPresence("steam_player_group_size", string.Format("{0}", playerCount));
		SteamFriends.SetRichPresence("steam_display", "#Status_Playing");
		SteamFriends.SetRichPresence("status", "Playing");
		SteamFriends.SetRichPresence("connect", string.Format("+ipAddress {0} +port {1}", server.IpAddress, server.Port));
	}

	// Token: 0x0600050B RID: 1291 RVA: 0x00020DAC File Offset: 0x0001EFAC
	public void UpdateRichPresenceScore(bool show, int period, int blueScore, int redScore)
	{
		if (!MonoBehaviourSingleton<SteamManager>.Instance.IsInitialized)
		{
			return;
		}
		string pchValue = show ? string.Format(" | P{0} {1} - {2}", period, blueScore, redScore) : " ";
		SteamFriends.SetRichPresence("score", pchValue);
	}

	// Token: 0x0600050C RID: 1292 RVA: 0x00020DFC File Offset: 0x0001EFFC
	public void UpdateRichPresenceRole(PlayerRole role)
	{
		if (!MonoBehaviourSingleton<SteamManager>.Instance.IsInitialized)
		{
			return;
		}
		string pchValue = role.ToString().Replace("Attacker", "Skater");
		SteamFriends.SetRichPresence("role", pchValue);
	}

	// Token: 0x0600050D RID: 1293 RVA: 0x00020E40 File Offset: 0x0001F040
	public void UpdateRichPresenceTeam(PlayerTeam team)
	{
		if (!MonoBehaviourSingleton<SteamManager>.Instance.IsInitialized)
		{
			return;
		}
		string pchValue = team.ToString().Replace("Blue", "Team Blue").Replace("Red", "Team Red");
		SteamFriends.SetRichPresence("team", pchValue);
	}

	// Token: 0x0600050E RID: 1294 RVA: 0x00020E94 File Offset: 0x0001F094
	public void UpdateRichPresencePhase(GamePhase phase)
	{
		if (!MonoBehaviourSingleton<SteamManager>.Instance.IsInitialized)
		{
			return;
		}
		string pchValue;
		if (phase == GamePhase.Warmup)
		{
			pchValue = "Warming up";
		}
		else
		{
			pchValue = "Playing";
		}
		SteamFriends.SetRichPresence("phase", pchValue);
	}

	// Token: 0x0600050F RID: 1295 RVA: 0x0000A23C File Offset: 0x0000843C
	public void GetAuthTicketForWebApi()
	{
		if (!MonoBehaviourSingleton<SteamManager>.Instance.IsInitialized)
		{
			return;
		}
		Debug.Log("[SteamIntegrationManager] Getting Steam Auth Ticket for Web API");
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnGetAuthTicketForWebApi", null);
		SteamUser.GetAuthTicketForWebApi(null);
	}

	// Token: 0x06000510 RID: 1296 RVA: 0x00020ECC File Offset: 0x0001F0CC
	public void GetLaunchCommandLine()
	{
		if (!MonoBehaviourSingleton<SteamManager>.Instance.IsInitialized)
		{
			return;
		}
		string text;
		SteamApps.GetLaunchCommandLine(out text, 256);
		string[] array = text.Split(" ", StringSplitOptions.None);
		if (array.Length != 0)
		{
			Debug.Log(string.Format("[SteamIntegrationManager] GotLaunchCommandLine: {0} ({1})", text, array.Length));
			MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnGotLaunchCommandLine", new Dictionary<string, object>
			{
				{
					"args",
					array
				}
			});
		}
	}

	// Token: 0x06000511 RID: 1297 RVA: 0x00020F3C File Offset: 0x0001F13C
	private void OnGotAuthTicketForWebApi(GetTicketForWebApiResponse_t response)
	{
		if (!MonoBehaviourSingleton<SteamManager>.Instance.IsInitialized)
		{
			return;
		}
		byte[] rgubTicket = response.m_rgubTicket;
		string value = BitConverter.ToString(rgubTicket, 0, rgubTicket.Length).Replace("-", string.Empty);
		Debug.Log("[SteamIntegrationManager] GotAuthTicketForWebApi");
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnGotAuthTicketForWebApi", new Dictionary<string, object>
		{
			{
				"ticket",
				value
			}
		});
	}

	// Token: 0x06000512 RID: 1298 RVA: 0x00020FA4 File Offset: 0x0001F1A4
	private void OnMicroTxnAuthorizationResponse(MicroTxnAuthorizationResponse_t response)
	{
		if (!MonoBehaviourSingleton<SteamManager>.Instance.IsInitialized)
		{
			return;
		}
		Debug.Log(string.Format("[SteamIntegrationManager] MicroTxnAuthorizationResponse: {0} {1} {2}", response.m_unAppID, response.m_ulOrderID, response.m_bAuthorized));
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnMicroTxnAuthorizationResponse", new Dictionary<string, object>
		{
			{
				"orderId",
				response.m_ulOrderID
			},
			{
				"authorized",
				Convert.ToBoolean(response.m_bAuthorized)
			}
		});
	}

	// Token: 0x06000513 RID: 1299 RVA: 0x00021034 File Offset: 0x0001F234
	private void OnGameRichPresenceJoinRequested(GameRichPresenceJoinRequested_t response)
	{
		if (!MonoBehaviourSingleton<SteamManager>.Instance.IsInitialized)
		{
			return;
		}
		Debug.Log("[SteamIntegrationManager] GameRichPresenceJoinRequested: " + response.m_rgchConnect);
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnGameRichPresenceJoinRequested", new Dictionary<string, object>
		{
			{
				"args",
				response.m_rgchConnect.Split(" ", StringSplitOptions.None)
			}
		});
	}

	// Token: 0x06000514 RID: 1300 RVA: 0x0000A26C File Offset: 0x0000846C
	private void OnNewUrlLaunchParameters(NewUrlLaunchParameters_t response)
	{
		if (!MonoBehaviourSingleton<SteamManager>.Instance.IsInitialized)
		{
			return;
		}
		this.GetLaunchCommandLine();
	}

	// Token: 0x040002CD RID: 717
	private Callback<GetTicketForWebApiResponse_t> GetTicketForWebApiCallback;

	// Token: 0x040002CE RID: 718
	private Callback<MicroTxnAuthorizationResponse_t> MicroTxnAuthorizationResponse;

	// Token: 0x040002CF RID: 719
	private Callback<GameRichPresenceJoinRequested_t> GameRichPresenceJoinRequested;

	// Token: 0x040002D0 RID: 720
	private Callback<NewUrlLaunchParameters_t> NewUrlLaunchParameters;
}
