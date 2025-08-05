using System;
using System.Collections.Generic;
using SocketIOClient;
using Steamworks;
using Unity.Netcode;
using UnityEngine;

// Token: 0x020000AD RID: 173
public class SteamIntegrationManagerController : MonoBehaviour
{
	// Token: 0x06000516 RID: 1302 RVA: 0x0000A289 File Offset: 0x00008489
	private void Awake()
	{
		this.steamIntegrationManager = base.GetComponent<SteamIntegrationManager>();
	}

	// Token: 0x06000517 RID: 1303 RVA: 0x00021098 File Offset: 0x0001F298
	private void Start()
	{
		if (Application.isBatchMode)
		{
			return;
		}
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnClientDisconnected", new Action<Dictionary<string, object>>(this.Event_OnClientDisconnected));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnGamePhaseChanged", new Action<Dictionary<string, object>>(this.Event_OnGamePhaseChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_OnPlayerRoleChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerRoleChanged));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnGotAuthTicketForWebApi", new Action<Dictionary<string, object>>(this.Event_Client_OnGotAuthTicketForWebApi));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnPlayerDataReady", new Action<Dictionary<string, object>>(this.Event_Client_OnPlayerDataReady));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnScoreboardClickPlayer", new Action<Dictionary<string, object>>(this.Event_Client_OnScoreboardClickPlayer));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnServerConfiguration", new Action<Dictionary<string, object>>(this.Event_Client_OnServerConfiguration));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnModsClickFindMods", new Action<Dictionary<string, object>>(this.Event_Client_OnModsClickFindMods));
		MonoBehaviourSingleton<WebSocketManager>.Instance.AddMessageListener("connect", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnConnect));
		MonoBehaviourSingleton<WebSocketManager>.Instance.AddMessageListener("playerAuthenticateResponse", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnPlayerAuthenticateResponse));
	}

	// Token: 0x06000518 RID: 1304 RVA: 0x000211BC File Offset: 0x0001F3BC
	private void OnDestroy()
	{
		if (Application.isBatchMode)
		{
			return;
		}
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnClientDisconnected", new Action<Dictionary<string, object>>(this.Event_OnClientDisconnected));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnGamePhaseChanged", new Action<Dictionary<string, object>>(this.Event_OnGamePhaseChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_OnPlayerRoleChanged", new Action<Dictionary<string, object>>(this.Event_OnPlayerRoleChanged));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnGotAuthTicketForWebApi", new Action<Dictionary<string, object>>(this.Event_Client_OnGotAuthTicketForWebApi));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnPlayerDataReady", new Action<Dictionary<string, object>>(this.Event_Client_OnPlayerDataReady));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnScoreboardClickPlayer", new Action<Dictionary<string, object>>(this.Event_Client_OnScoreboardClickPlayer));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnServerConfiguration", new Action<Dictionary<string, object>>(this.Event_Client_OnServerConfiguration));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnModsClickFindMods", new Action<Dictionary<string, object>>(this.Event_Client_OnModsClickFindMods));
		MonoBehaviourSingleton<WebSocketManager>.Instance.RemoveMessageListener("connect", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnConnect));
		MonoBehaviourSingleton<WebSocketManager>.Instance.RemoveMessageListener("playerAuthenticateResponse", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnPlayerAuthenticateResponse));
	}

	// Token: 0x06000519 RID: 1305 RVA: 0x000212E0 File Offset: 0x0001F4E0
	private void Event_OnClientDisconnected(Dictionary<string, object> message)
	{
		ulong num = (ulong)message["clientId"];
		if (NetworkManager.Singleton.LocalClientId != num)
		{
			return;
		}
		this.steamIntegrationManager.SetRichPresenceMainMenu();
	}

	// Token: 0x0600051A RID: 1306 RVA: 0x00021318 File Offset: 0x0001F518
	private void Event_OnGamePhaseChanged(Dictionary<string, object> message)
	{
		GameState gameState = (GameState)message["gameState"];
		this.steamIntegrationManager.UpdateRichPresencePhase(gameState.Phase);
		this.steamIntegrationManager.UpdateRichPresenceScore(gameState.Phase != GamePhase.Warmup, gameState.Period, gameState.BlueScore, gameState.RedScore);
	}

	// Token: 0x0600051B RID: 1307 RVA: 0x00021370 File Offset: 0x0001F570
	private void Event_OnPlayerRoleChanged(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		Server server = NetworkBehaviourSingleton<ServerManager>.Instance.Server;
		GameState value = NetworkBehaviourSingleton<GameManager>.Instance.GameState.Value;
		if (!player.IsLocalPlayer)
		{
			return;
		}
		PlayerTeam value2 = player.Team.Value;
		if (value2 - PlayerTeam.Blue <= 1)
		{
			this.steamIntegrationManager.UpdateRichPresencePhase(value.Phase);
			this.steamIntegrationManager.UpdateRichPresenceTeam(player.Team.Value);
			this.steamIntegrationManager.UpdateRichPresenceRole(player.Role.Value);
			this.steamIntegrationManager.UpdateRichPresenceScore(value.Phase != GamePhase.Warmup, value.Period, value.BlueScore, value.RedScore);
			this.steamIntegrationManager.SetRichPresencePlaying(server, NetworkBehaviourSingleton<PlayerManager>.Instance.GetPlayers(false).Count);
			return;
		}
		this.steamIntegrationManager.UpdateRichPresenceScore(value.Phase != GamePhase.Warmup, value.Period, value.BlueScore, value.RedScore);
		this.steamIntegrationManager.SetRichPresenceSpectating(server, NetworkBehaviourSingleton<PlayerManager>.Instance.GetPlayers(false).Count);
	}

	// Token: 0x0600051C RID: 1308 RVA: 0x00021490 File Offset: 0x0001F690
	private void Event_Client_OnGotAuthTicketForWebApi(Dictionary<string, object> message)
	{
		string value = (string)message["ticket"];
		MonoBehaviourSingleton<WebSocketManager>.Instance.Emit("playerAuthenticateRequest", new Dictionary<string, object>
		{
			{
				"ticket",
				value
			}
		}, "playerAuthenticateResponse");
	}

	// Token: 0x0600051D RID: 1309 RVA: 0x0000A297 File Offset: 0x00008497
	private void Event_Client_OnPlayerDataReady(Dictionary<string, object> message)
	{
		this.steamIntegrationManager.GetLaunchCommandLine();
	}

	// Token: 0x0600051E RID: 1310 RVA: 0x000214D4 File Offset: 0x0001F6D4
	private void Event_Client_OnScoreboardClickPlayer(Dictionary<string, object> message)
	{
		Player player = (Player)message["player"];
		SteamFriends.ActivateGameOverlayToUser("steamID", new CSteamID(ulong.Parse(player.SteamId.Value.ToString())));
	}

	// Token: 0x0600051F RID: 1311 RVA: 0x00021520 File Offset: 0x0001F720
	private void Event_Client_OnServerConfiguration(Dictionary<string, object> message)
	{
		Server server = (Server)message["server"];
		this.steamIntegrationManager.UpdateRichPresenceScore(false, 0, 0, 0);
		this.steamIntegrationManager.SetRichPresenceSpectating(server, 1);
	}

	// Token: 0x06000520 RID: 1312 RVA: 0x0000A2A4 File Offset: 0x000084A4
	private void Event_Client_OnModsClickFindMods(Dictionary<string, object> message)
	{
		SteamFriends.ActivateGameOverlayToWebPage("https://steamcommunity.com/app/2994020/workshop/", EActivateGameOverlayToWebPageMode.k_EActivateGameOverlayToWebPageMode_Default);
	}

	// Token: 0x06000521 RID: 1313 RVA: 0x0000A2B1 File Offset: 0x000084B1
	private void WebSocket_Event_OnConnect(Dictionary<string, object> message)
	{
		this.steamIntegrationManager.GetAuthTicketForWebApi();
	}

	// Token: 0x06000522 RID: 1314 RVA: 0x0000A2BE File Offset: 0x000084BE
	private void WebSocket_Event_OnPlayerAuthenticateResponse(Dictionary<string, object> message)
	{
		if (((SocketIOResponse)message["response"]).GetValue<PlayerAuthenticateResponse>(0).success)
		{
			this.steamIntegrationManager.SetRichPresenceMainMenu();
		}
	}

	// Token: 0x040002D1 RID: 721
	private SteamIntegrationManager steamIntegrationManager;
}
