using System;
using System.Collections.Generic;
using System.Linq;
using Steamworks;
using UnityEngine;

// Token: 0x0200012E RID: 302
public static class SteamIntegrationManager
{
	// Token: 0x170000EB RID: 235
	// (get) Token: 0x060008C9 RID: 2249 RVA: 0x0002A846 File Offset: 0x00028A46
	public static bool IsOverlayEnabled
	{
		get
		{
			return SteamManager.IsInitialized && SteamUtils.IsOverlayEnabled();
		}
	}

	// Token: 0x060008CA RID: 2250 RVA: 0x0002A856 File Offset: 0x00028A56
	public static void Initialize()
	{
		SteamIntegrationManager.RegisterCallbacks();
		SteamIntegrationManagerController.Initialize();
	}

	// Token: 0x060008CB RID: 2251 RVA: 0x0002A862 File Offset: 0x00028A62
	public static void Dispose()
	{
		SteamIntegrationManagerController.Dispose();
		SteamIntegrationManager.UnregisterCallbacks();
		SteamIntegrationManager.joinedLobbyIds.Clear();
	}

	// Token: 0x060008CC RID: 2252 RVA: 0x0002A878 File Offset: 0x00028A78
	private static void RegisterCallbacks()
	{
		if (!SteamManager.IsInitialized)
		{
			return;
		}
		SteamIntegrationManager.GetTicketForWebApiCallback = Callback<GetTicketForWebApiResponse_t>.Create(new Callback<GetTicketForWebApiResponse_t>.DispatchDelegate(SteamIntegrationManager.OnGetTicketForWebApiResponse));
		SteamIntegrationManager.MicroTxnAuthorizationResponse = Callback<MicroTxnAuthorizationResponse_t>.Create(new Callback<MicroTxnAuthorizationResponse_t>.DispatchDelegate(SteamIntegrationManager.OnMicroTxnAuthorizationResponse));
		SteamIntegrationManager.GameRichPresenceJoinRequested = Callback<GameRichPresenceJoinRequested_t>.Create(new Callback<GameRichPresenceJoinRequested_t>.DispatchDelegate(SteamIntegrationManager.OnGameRichPresenceJoinRequested));
		SteamIntegrationManager.NewUrlLaunchParameters = Callback<NewUrlLaunchParameters_t>.Create(new Callback<NewUrlLaunchParameters_t>.DispatchDelegate(SteamIntegrationManager.OnNewUrlLaunchParameters));
		SteamIntegrationManager.LobbyCreatedCallback = Callback<LobbyCreated_t>.Create(new Callback<LobbyCreated_t>.DispatchDelegate(SteamIntegrationManager.OnLobbyCreated));
		SteamIntegrationManager.LobbyEnterCallback = Callback<LobbyEnter_t>.Create(new Callback<LobbyEnter_t>.DispatchDelegate(SteamIntegrationManager.OnLobbyEntered));
		SteamIntegrationManager.LobbyChatUpdateCallback = Callback<LobbyChatUpdate_t>.Create(new Callback<LobbyChatUpdate_t>.DispatchDelegate(SteamIntegrationManager.OnLobbyChatUpdate));
		SteamIntegrationManager.GameLobbyJoinRequestedCallback = Callback<GameLobbyJoinRequested_t>.Create(new Callback<GameLobbyJoinRequested_t>.DispatchDelegate(SteamIntegrationManager.OnGameLobbyJoinRequested));
		SteamIntegrationManager.PersonaStateChangeCallback = Callback<PersonaStateChange_t>.Create(new Callback<PersonaStateChange_t>.DispatchDelegate(SteamIntegrationManager.OnPersonaStateChange));
	}

	// Token: 0x060008CD RID: 2253 RVA: 0x0002A954 File Offset: 0x00028B54
	private static void UnregisterCallbacks()
	{
		if (!SteamManager.IsInitialized)
		{
			return;
		}
		SteamIntegrationManager.GetTicketForWebApiCallback.Unregister();
		SteamIntegrationManager.MicroTxnAuthorizationResponse.Unregister();
		SteamIntegrationManager.GameRichPresenceJoinRequested.Unregister();
		SteamIntegrationManager.NewUrlLaunchParameters.Unregister();
		SteamIntegrationManager.LobbyCreatedCallback.Unregister();
		SteamIntegrationManager.LobbyEnterCallback.Unregister();
		SteamIntegrationManager.LobbyChatUpdateCallback.Unregister();
		SteamIntegrationManager.GameLobbyJoinRequestedCallback.Unregister();
		SteamIntegrationManager.PersonaStateChangeCallback.Unregister();
	}

	// Token: 0x060008CE RID: 2254 RVA: 0x0002A9C3 File Offset: 0x00028BC3
	public static void SetRichPresenceMainMenu()
	{
		if (!SteamManager.IsInitialized)
		{
			return;
		}
		SteamFriends.ClearRichPresence();
		SteamFriends.SetRichPresence("steam_display", "#Status_MainMenu");
		SteamFriends.SetRichPresence("status", "In the changing room");
	}

	// Token: 0x060008CF RID: 2255 RVA: 0x0002A9F4 File Offset: 0x00028BF4
	public static void SetRichPresenceSpectating(Server server, int playerCount)
	{
		if (!SteamManager.IsInitialized)
		{
			return;
		}
		SteamFriends.SetRichPresence("steam_player_group", string.Format("{0}:{1}", server.IpAddress, server.Port));
		SteamFriends.SetRichPresence("steam_player_group_size", string.Format("{0}", playerCount));
		SteamFriends.SetRichPresence("steam_display", "#Status_Spectating");
		SteamFriends.SetRichPresence("status", "Spectating");
		SteamFriends.SetRichPresence("connect", string.Format("+ipAddress {0} +port {1}", server.IpAddress, server.Port));
	}

	// Token: 0x060008D0 RID: 2256 RVA: 0x0002AA9C File Offset: 0x00028C9C
	public static void SetRichPresencePlaying(Server server, int playerCount)
	{
		if (!SteamManager.IsInitialized)
		{
			return;
		}
		SteamFriends.SetRichPresence("steam_player_group", string.Format("{0}:{1}", server.IpAddress, server.Port));
		SteamFriends.SetRichPresence("steam_player_group_size", string.Format("{0}", playerCount));
		SteamFriends.SetRichPresence("steam_display", "#Status_Playing");
		SteamFriends.SetRichPresence("status", "Playing");
		SteamFriends.SetRichPresence("connect", string.Format("+ipAddress {0} +port {1}", server.IpAddress, server.Port));
	}

	// Token: 0x060008D1 RID: 2257 RVA: 0x0002AB44 File Offset: 0x00028D44
	public static void UpdateRichPresenceScore(bool show, int period, int blueScore, int redScore)
	{
		if (!SteamManager.IsInitialized)
		{
			return;
		}
		string pchValue = show ? string.Format(" | P{0} {1} - {2}", period, blueScore, redScore) : " ";
		SteamFriends.SetRichPresence("score", pchValue);
	}

	// Token: 0x060008D2 RID: 2258 RVA: 0x0002AB8C File Offset: 0x00028D8C
	public static void UpdateRichPresenceRole(PlayerRole role)
	{
		if (!SteamManager.IsInitialized)
		{
			return;
		}
		string pchValue = role.ToString().Replace("Attacker", "Skater");
		SteamFriends.SetRichPresence("role", pchValue);
	}

	// Token: 0x060008D3 RID: 2259 RVA: 0x0002ABCC File Offset: 0x00028DCC
	public static void UpdateRichPresenceTeam(PlayerTeam team)
	{
		if (!SteamManager.IsInitialized)
		{
			return;
		}
		string pchValue = team.ToString().Replace("Blue", "Team Blue").Replace("Red", "Team Red");
		SteamFriends.SetRichPresence("team", pchValue);
	}

	// Token: 0x060008D4 RID: 2260 RVA: 0x0002AC1C File Offset: 0x00028E1C
	public static void UpdateRichPresencePhase(GamePhase phase)
	{
		if (!SteamManager.IsInitialized)
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

	// Token: 0x060008D5 RID: 2261 RVA: 0x0002AC4F File Offset: 0x00028E4F
	public static void CreateLobby()
	{
		if (!SteamManager.IsInitialized)
		{
			return;
		}
		Debug.Log("[SteamIntegrationManager] Creating lobby");
		SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 128);
	}

	// Token: 0x060008D6 RID: 2262 RVA: 0x0002AC6F File Offset: 0x00028E6F
	public static void JoinLobby(string lobbyId)
	{
		if (!SteamManager.IsInitialized)
		{
			return;
		}
		if (SteamIntegrationManager.IsInLobby(lobbyId))
		{
			return;
		}
		Debug.Log("[SteamIntegrationManager] Joining lobby " + lobbyId);
		SteamMatchmaking.JoinLobby(new CSteamID(ulong.Parse(lobbyId)));
	}

	// Token: 0x060008D7 RID: 2263 RVA: 0x0002ACA4 File Offset: 0x00028EA4
	public static void LeaveLobby(string lobbyId)
	{
		if (!SteamManager.IsInitialized)
		{
			return;
		}
		if (!SteamIntegrationManager.IsInLobby(lobbyId))
		{
			return;
		}
		Debug.Log("[SteamIntegrationManager] Leaving lobby " + lobbyId);
		SteamMatchmaking.LeaveLobby(new CSteamID(ulong.Parse(lobbyId)));
		SteamIntegrationManager.joinedLobbyIds.Remove(lobbyId);
		EventManager.TriggerEvent("Event_OnLobbyLeft", new Dictionary<string, object>
		{
			{
				"lobbyId",
				lobbyId
			}
		});
	}

	// Token: 0x060008D8 RID: 2264 RVA: 0x0002AD0C File Offset: 0x00028F0C
	public static void LeaveAllLobbies()
	{
		if (!SteamManager.IsInitialized)
		{
			return;
		}
		Debug.Log("[SteamIntegrationManager] Leaving all lobbies");
		foreach (string lobbyId in SteamIntegrationManager.joinedLobbyIds.ToList<string>())
		{
			SteamIntegrationManager.LeaveLobby(lobbyId);
		}
	}

	// Token: 0x060008D9 RID: 2265 RVA: 0x0002AD74 File Offset: 0x00028F74
	public static bool IsInLobby(string lobbyId)
	{
		return SteamIntegrationManager.joinedLobbyIds.Contains(lobbyId);
	}

	// Token: 0x060008DA RID: 2266 RVA: 0x0002AD84 File Offset: 0x00028F84
	public static string GetSteamId()
	{
		if (!SteamManager.IsInitialized)
		{
			return null;
		}
		return SteamUser.GetSteamID().ToString();
	}

	// Token: 0x060008DB RID: 2267 RVA: 0x0002ADAD File Offset: 0x00028FAD
	public static void GetTicketForWebApi()
	{
		if (!SteamManager.IsInitialized)
		{
			return;
		}
		SteamUser.GetAuthTicketForWebApi("*");
	}

	// Token: 0x060008DC RID: 2268 RVA: 0x0002ADC4 File Offset: 0x00028FC4
	public static void GetLaunchCommandLine()
	{
		if (!SteamManager.IsInitialized)
		{
			return;
		}
		string text;
		SteamApps.GetLaunchCommandLine(out text, 256);
		string[] array = text.Split(" ", StringSplitOptions.None);
		if (array.Length != 0)
		{
			Debug.Log(string.Format("[SteamIntegrationManager] GotLaunchCommandLine: {0} ({1})", text, array.Length));
			EventManager.TriggerEvent("Event_OnGotLaunchCommandLine", new Dictionary<string, object>
			{
				{
					"args",
					array
				}
			});
		}
	}

	// Token: 0x060008DD RID: 2269 RVA: 0x0002AE2C File Offset: 0x0002902C
	public static Texture2D GetAvatar(string steamId, AvatarSize size)
	{
		if (!SteamManager.IsInitialized)
		{
			return null;
		}
		CSteamID steamIDFriend = new CSteamID(ulong.Parse(steamId));
		int num;
		switch (size)
		{
		case AvatarSize.Small:
			num = SteamFriends.GetSmallFriendAvatar(steamIDFriend);
			break;
		case AvatarSize.Medium:
			num = SteamFriends.GetMediumFriendAvatar(steamIDFriend);
			break;
		case AvatarSize.Large:
			num = SteamFriends.GetLargeFriendAvatar(steamIDFriend);
			break;
		default:
			num = SteamFriends.GetMediumFriendAvatar(steamIDFriend);
			break;
		}
		int iImage = num;
		uint num2;
		uint num3;
		SteamUtils.GetImageSize(iImage, out num2, out num3);
		byte[] array = new byte[num2 * num3 * 4U];
		bool imageRGBA = SteamUtils.GetImageRGBA(iImage, array, array.Length);
		byte[] array2 = new byte[array.Length];
		int num4 = (int)(num2 * 4U);
		int num5 = 0;
		while ((long)num5 < (long)((ulong)num3))
		{
			Buffer.BlockCopy(array, num5 * num4, array2, (int)((num3 - 1U - (uint)num5) * (uint)num4), num4);
			num5++;
		}
		if (imageRGBA)
		{
			Texture2D texture2D = new Texture2D((int)num2, (int)num3, TextureFormat.RGBA32, false);
			texture2D.LoadRawTextureData(array2);
			texture2D.Apply();
			return texture2D;
		}
		return null;
	}

	// Token: 0x060008DE RID: 2270 RVA: 0x0002AF04 File Offset: 0x00029104
	public static string GetUsername(string steamId)
	{
		if (!SteamManager.IsInitialized)
		{
			return null;
		}
		CSteamID csteamID = new CSteamID(ulong.Parse(steamId));
		if (csteamID == SteamUser.GetSteamID())
		{
			return SteamFriends.GetPersonaName();
		}
		return SteamFriends.GetFriendPersonaName(csteamID);
	}

	// Token: 0x060008DF RID: 2271 RVA: 0x0002AF40 File Offset: 0x00029140
	public static string[] GetFriendSteamIds(bool includeOffline = false)
	{
		if (!SteamManager.IsInitialized)
		{
			return new string[0];
		}
		int friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);
		List<string> list = new List<string>();
		for (int i = 0; i < friendCount; i++)
		{
			string text = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate).ToString();
			if (SteamIntegrationManager.IsFriendOnline(text) || includeOffline)
			{
				list.Add(text);
			}
		}
		return list.ToArray();
	}

	// Token: 0x060008E0 RID: 2272 RVA: 0x0002AFA8 File Offset: 0x000291A8
	public static string GetLobbyOwnerSteamId(string lobbyId)
	{
		if (!SteamManager.IsInitialized)
		{
			return null;
		}
		return SteamMatchmaking.GetLobbyOwner(new CSteamID(ulong.Parse(lobbyId))).ToString();
	}

	// Token: 0x060008E1 RID: 2273 RVA: 0x0002AFDC File Offset: 0x000291DC
	public static string[] GetLobbyMemberSteamIds(string lobbyId)
	{
		if (!SteamManager.IsInitialized)
		{
			return new string[0];
		}
		CSteamID steamIDLobby = new CSteamID(ulong.Parse(lobbyId));
		int numLobbyMembers = SteamMatchmaking.GetNumLobbyMembers(steamIDLobby);
		List<string> list = new List<string>();
		for (int i = 0; i < numLobbyMembers; i++)
		{
			string item = SteamMatchmaking.GetLobbyMemberByIndex(steamIDLobby, i).ToString();
			list.Add(item);
		}
		return list.ToArray();
	}

	// Token: 0x060008E2 RID: 2274 RVA: 0x0002B043 File Offset: 0x00029243
	public static bool IsFriend(string steamId)
	{
		return SteamManager.IsInitialized && SteamFriends.GetFriendRelationship(new CSteamID(ulong.Parse(steamId))) == EFriendRelationship.k_EFriendRelationshipFriend;
	}

	// Token: 0x060008E3 RID: 2275 RVA: 0x0002B061 File Offset: 0x00029261
	public static bool IsFriendOnline(string steamId)
	{
		return SteamManager.IsInitialized && SteamFriends.GetFriendPersonaState(new CSteamID(ulong.Parse(steamId))) > EPersonaState.k_EPersonaStateOffline;
	}

	// Token: 0x060008E4 RID: 2276 RVA: 0x0002B080 File Offset: 0x00029280
	public static void InviteToLobby(string lobbyId, string invitedSteamId)
	{
		if (!SteamManager.IsInitialized)
		{
			return;
		}
		CSteamID steamIDLobby = new CSteamID(ulong.Parse(lobbyId));
		CSteamID steamIDInvitee = new CSteamID(ulong.Parse(invitedSteamId));
		SteamMatchmaking.InviteUserToLobby(steamIDLobby, steamIDInvitee);
	}

	// Token: 0x060008E5 RID: 2277 RVA: 0x0002B0B4 File Offset: 0x000292B4
	private static void OnGetTicketForWebApiResponse(GetTicketForWebApiResponse_t response)
	{
		byte[] rgubTicket = response.m_rgubTicket;
		string value = BitConverter.ToString(rgubTicket, 0, rgubTicket.Length).Replace("-", string.Empty);
		EventManager.TriggerEvent("Event_OnGetTicketForWebApiResponse", new Dictionary<string, object>
		{
			{
				"ticket",
				value
			}
		});
	}

	// Token: 0x060008E6 RID: 2278 RVA: 0x0002B100 File Offset: 0x00029300
	private static void OnMicroTxnAuthorizationResponse(MicroTxnAuthorizationResponse_t response)
	{
		EventManager.TriggerEvent("Event_OnMicroTxnAuthorizationResponse", new Dictionary<string, object>
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

	// Token: 0x060008E7 RID: 2279 RVA: 0x0002B14D File Offset: 0x0002934D
	private static void OnGameRichPresenceJoinRequested(GameRichPresenceJoinRequested_t response)
	{
		EventManager.TriggerEvent("Event_OnGameRichPresenceJoinRequested", new Dictionary<string, object>
		{
			{
				"args",
				response.m_rgchConnect.Split(" ", StringSplitOptions.None)
			}
		});
	}

	// Token: 0x060008E8 RID: 2280 RVA: 0x0002B17B File Offset: 0x0002937B
	private static void OnNewUrlLaunchParameters(NewUrlLaunchParameters_t response)
	{
		SteamIntegrationManager.GetLaunchCommandLine();
	}

	// Token: 0x060008E9 RID: 2281 RVA: 0x0002B184 File Offset: 0x00029384
	private static void OnLobbyCreated(LobbyCreated_t result)
	{
		if (result.m_eResult != EResult.k_EResultOK)
		{
			return;
		}
		string text = result.m_ulSteamIDLobby.ToString();
		Debug.Log("[SteamIntegrationManager] Lobby " + text + " created");
		if (!SteamIntegrationManager.joinedLobbyIds.Contains(text))
		{
			SteamIntegrationManager.joinedLobbyIds.Add(text);
		}
		EventManager.TriggerEvent("Event_OnLobbyCreated", new Dictionary<string, object>
		{
			{
				"lobbyId",
				text
			}
		});
	}

	// Token: 0x060008EA RID: 2282 RVA: 0x0002B1F0 File Offset: 0x000293F0
	private static void OnLobbyEntered(LobbyEnter_t result)
	{
		string text = result.m_ulSteamIDLobby.ToString();
		Debug.Log("[SteamIntegrationManager] Lobby " + text + " entered");
		if (!SteamIntegrationManager.joinedLobbyIds.Contains(text))
		{
			SteamIntegrationManager.joinedLobbyIds.Add(text);
		}
		EventManager.TriggerEvent("Event_OnLobbyEntered", new Dictionary<string, object>
		{
			{
				"lobbyId",
				text
			},
			{
				"ownerSteamId",
				SteamIntegrationManager.GetLobbyOwnerSteamId(text)
			},
			{
				"memberSteamIds",
				SteamIntegrationManager.GetLobbyMemberSteamIds(text)
			}
		});
	}

	// Token: 0x060008EB RID: 2283 RVA: 0x0002B274 File Offset: 0x00029474
	private static void OnLobbyChatUpdate(LobbyChatUpdate_t result)
	{
		string text = result.m_ulSteamIDLobby.ToString();
		Debug.Log("[SteamIntegrationManager] Lobby " + text + " updated");
		EventManager.TriggerEvent("Event_OnLobbyChatUpdate", new Dictionary<string, object>
		{
			{
				"lobbyId",
				text
			},
			{
				"ownerSteamId",
				SteamIntegrationManager.GetLobbyOwnerSteamId(text)
			},
			{
				"memberSteamIds",
				SteamIntegrationManager.GetLobbyMemberSteamIds(text)
			}
		});
	}

	// Token: 0x060008EC RID: 2284 RVA: 0x0002B2E0 File Offset: 0x000294E0
	private static void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t result)
	{
		CSteamID steamIDLobby = result.m_steamIDLobby;
		string text = steamIDLobby.ToString();
		Debug.Log("[SteamIntegrationManager] Lobby " + text + " join requested");
		EventManager.TriggerEvent("Event_OnGameLobbyJoinRequested", new Dictionary<string, object>
		{
			{
				"lobbyId",
				text
			}
		});
	}

	// Token: 0x060008ED RID: 2285 RVA: 0x0002B334 File Offset: 0x00029534
	private static void OnPersonaStateChange(PersonaStateChange_t result)
	{
		string value = result.m_ulSteamID.ToString();
		if ((result.m_nChangeFlags & (EPersonaChange.k_EPersonaChangeName | EPersonaChange.k_EPersonaChangeStatus | EPersonaChange.k_EPersonaChangeAvatar | EPersonaChange.k_EPersonaChangeRelationshipChanged | EPersonaChange.k_EPersonaChangeNickname)) != (EPersonaChange)0)
		{
			EventManager.TriggerEvent("Event_OnPersonaStateChange", new Dictionary<string, object>
			{
				{
					"steamId",
					value
				}
			});
		}
	}

	// Token: 0x0400052B RID: 1323
	private static List<string> joinedLobbyIds = new List<string>();

	// Token: 0x0400052C RID: 1324
	private static Callback<GetTicketForWebApiResponse_t> GetTicketForWebApiCallback;

	// Token: 0x0400052D RID: 1325
	private static Callback<MicroTxnAuthorizationResponse_t> MicroTxnAuthorizationResponse;

	// Token: 0x0400052E RID: 1326
	private static Callback<GameRichPresenceJoinRequested_t> GameRichPresenceJoinRequested;

	// Token: 0x0400052F RID: 1327
	private static Callback<NewUrlLaunchParameters_t> NewUrlLaunchParameters;

	// Token: 0x04000530 RID: 1328
	private static Callback<LobbyCreated_t> LobbyCreatedCallback;

	// Token: 0x04000531 RID: 1329
	private static Callback<LobbyEnter_t> LobbyEnterCallback;

	// Token: 0x04000532 RID: 1330
	private static Callback<LobbyChatUpdate_t> LobbyChatUpdateCallback;

	// Token: 0x04000533 RID: 1331
	private static Callback<GameLobbyJoinRequested_t> GameLobbyJoinRequestedCallback;

	// Token: 0x04000534 RID: 1332
	private static Callback<PersonaStateChange_t> PersonaStateChangeCallback;
}
