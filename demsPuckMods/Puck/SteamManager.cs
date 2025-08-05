using System;
using System.Net;
using Steamworks;
using UnityEngine;

// Token: 0x020000AE RID: 174
public class SteamManager : MonoBehaviourSingleton<SteamManager>
{
	// Token: 0x06000524 RID: 1316 RVA: 0x0002155C File Offset: 0x0001F75C
	public override void Awake()
	{
		base.Awake();
		if (Application.isBatchMode)
		{
			this.IsInitialized = GameServer.Init(BitConverter.ToUInt32(IPAddress.Any.GetAddressBytes(), 0), 0, 0, EServerMode.eServerModeNoAuthentication, null);
			if (this.IsInitialized)
			{
				SteamGameServer.LogOnAnonymous();
			}
		}
		else
		{
			this.IsInitialized = SteamAPI.Init();
		}
		if (this.IsInitialized)
		{
			if (Application.isBatchMode)
			{
				Debug.Log("[SteamManager] Initialized as GameServer");
			}
			else
			{
				Debug.Log("[SteamManager] Initialized as SteamClient");
			}
			this.RegisterCallbacks();
			return;
		}
		if (Application.isBatchMode)
		{
			Debug.LogError("[SteamManager] Failed to initialize GameServer");
			return;
		}
		Debug.LogError("[SteamManager] Failed to initialize SteamClient");
	}

	// Token: 0x06000525 RID: 1317 RVA: 0x000215F8 File Offset: 0x0001F7F8
	private void RegisterCallbacks()
	{
		if (!this.IsInitialized)
		{
			return;
		}
		if (Application.isBatchMode)
		{
			SteamManager.steamServersConnectedCallback = Callback<SteamServersConnected_t>.CreateGameServer(new Callback<SteamServersConnected_t>.DispatchDelegate(SteamManager.OnSteamServersConnected));
			SteamManager.steamServerConnectFailureCallback = Callback<SteamServerConnectFailure_t>.CreateGameServer(new Callback<SteamServerConnectFailure_t>.DispatchDelegate(SteamManager.OnSteamServerConnectFailure));
			SteamManager.steamServersDisconnectedCallback = Callback<SteamServersDisconnected_t>.CreateGameServer(new Callback<SteamServersDisconnected_t>.DispatchDelegate(SteamManager.OnSteamServersDisconnected));
		}
		Debug.Log("[SteamManager] Registered callbacks");
	}

	// Token: 0x06000526 RID: 1318 RVA: 0x0000A2E8 File Offset: 0x000084E8
	private void OnDestroy()
	{
		if (!this.IsInitialized)
		{
			return;
		}
		if (Application.isBatchMode)
		{
			GameServer.Shutdown();
			return;
		}
		SteamAPI.Shutdown();
	}

	// Token: 0x06000527 RID: 1319 RVA: 0x0000A2E8 File Offset: 0x000084E8
	private void OnApplicationQuit()
	{
		if (!this.IsInitialized)
		{
			return;
		}
		if (Application.isBatchMode)
		{
			GameServer.Shutdown();
			return;
		}
		SteamAPI.Shutdown();
	}

	// Token: 0x06000528 RID: 1320 RVA: 0x0000A305 File Offset: 0x00008505
	private void Update()
	{
		if (!this.IsInitialized)
		{
			return;
		}
		if (Application.isBatchMode)
		{
			GameServer.RunCallbacks();
			return;
		}
		SteamAPI.RunCallbacks();
	}

	// Token: 0x06000529 RID: 1321 RVA: 0x0000A322 File Offset: 0x00008522
	private static void OnSteamServersConnected(SteamServersConnected_t callback)
	{
		Debug.Log("[SteamManager] Connected to Steam");
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnSteamServersConnected", null);
	}

	// Token: 0x0600052A RID: 1322 RVA: 0x0000A33E File Offset: 0x0000853E
	private static void OnSteamServerConnectFailure(SteamServerConnectFailure_t callback)
	{
		Debug.Log(string.Format("[SteamManager] Failed to connect to Steam: {0}", callback.m_eResult));
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnSteamServerConnectFailure", null);
	}

	// Token: 0x0600052B RID: 1323 RVA: 0x0000A36A File Offset: 0x0000856A
	private static void OnSteamServersDisconnected(SteamServersDisconnected_t callback)
	{
		Debug.Log(string.Format("[SteamManager] Disconnected from Steam: {0}", callback.m_eResult));
		MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnSteamServersDisconnected", null);
	}

	// Token: 0x040002D2 RID: 722
	[HideInInspector]
	public bool IsInitialized;

	// Token: 0x040002D3 RID: 723
	private static Callback<SteamServersConnected_t> steamServersConnectedCallback;

	// Token: 0x040002D4 RID: 724
	private static Callback<SteamServerConnectFailure_t> steamServerConnectFailureCallback;

	// Token: 0x040002D5 RID: 725
	private static Callback<SteamServersDisconnected_t> steamServersDisconnectedCallback;
}
