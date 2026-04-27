using System;
using System.Net;
using DG.Tweening;
using Steamworks;
using UnityEngine;

// Token: 0x02000130 RID: 304
public static class SteamManager
{
	// Token: 0x06000900 RID: 2304 RVA: 0x0002BAF1 File Offset: 0x00029CF1
	public static void Initialize()
	{
		SteamManagerController.Initialize();
		SteamManager.InitializeSteam();
	}

	// Token: 0x06000901 RID: 2305 RVA: 0x0002BAFD File Offset: 0x00029CFD
	public static void Dispose()
	{
		SteamManager.DisposeSteam();
		SteamManagerController.Dispose();
	}

	// Token: 0x06000902 RID: 2306 RVA: 0x0002BB0C File Offset: 0x00029D0C
	private static void InitializeSteam()
	{
		if (SteamManager.IsInitialized)
		{
			return;
		}
		Debug.Log("[SteamManager] Initializing Steam");
		EventManager.TriggerEvent("Event_OnSteamInitializationStarted", null);
		if (ApplicationManager.IsDedicatedGameServer)
		{
			SteamManager.IsInitialized = GameServer.Init(BitConverter.ToUInt32(IPAddress.Any.GetAddressBytes(), 0), 0, 0, EServerMode.eServerModeNoAuthentication, null);
		}
		else
		{
			SteamManager.IsInitialized = SteamAPI.Init();
		}
		if (!SteamManager.IsInitialized)
		{
			if (ApplicationManager.IsDedicatedGameServer)
			{
				Debug.Log("[SteamManager] Failed to initialize as game server");
			}
			else
			{
				Debug.Log("[SteamManager] Failed to initialize as client");
			}
			EventManager.TriggerEvent("Event_OnSteamInitializationFailed", null);
			Tween tween = SteamManager.steamInitializationRetryTween;
			if (tween != null)
			{
				tween.Kill(false);
			}
			SteamManager.steamInitializationRetryTween = DOVirtual.DelayedCall(5f, delegate
			{
				Debug.Log("[SteamManager] Retrying Steam initialization");
				SteamManager.InitializeSteam();
			}, true);
			return;
		}
		SteamManager.RegisterCallbacks();
		SteamManager.StartCallbackLoop();
		if (ApplicationManager.IsDedicatedGameServer)
		{
			Debug.Log("[SteamManager] Initialized as game server");
			EventManager.TriggerEvent("Event_OnSteamInitialized", null);
			SteamGameServer.LogOnAnonymous();
			return;
		}
		Debug.Log("[SteamManager] Initialized as client");
		EventManager.TriggerEvent("Event_OnSteamInitialized", null);
		SteamManager.OnSteamServersConnected(default(SteamServersConnected_t));
	}

	// Token: 0x06000903 RID: 2307 RVA: 0x0002BC26 File Offset: 0x00029E26
	private static void DisposeSteam()
	{
		if (!SteamManager.IsInitialized)
		{
			return;
		}
		Tween tween = SteamManager.steamInitializationRetryTween;
		if (tween != null)
		{
			tween.Kill(false);
		}
		if (ApplicationManager.IsDedicatedGameServer)
		{
			GameServer.Shutdown();
		}
		else
		{
			SteamAPI.Shutdown();
		}
		SteamManager.StopCallbackLoop();
		SteamManager.UnregisterCallbacks();
		SteamManager.IsInitialized = false;
	}

	// Token: 0x06000904 RID: 2308 RVA: 0x0002BC64 File Offset: 0x00029E64
	private static void RegisterCallbacks()
	{
		if (!SteamManager.IsInitialized)
		{
			return;
		}
		if (ApplicationManager.IsDedicatedGameServer)
		{
			SteamManager.steamServersConnectedCallback = Callback<SteamServersConnected_t>.CreateGameServer(new Callback<SteamServersConnected_t>.DispatchDelegate(SteamManager.OnSteamServersConnected));
			SteamManager.steamServerConnectFailureCallback = Callback<SteamServerConnectFailure_t>.CreateGameServer(new Callback<SteamServerConnectFailure_t>.DispatchDelegate(SteamManager.OnSteamServerConnectFailure));
			SteamManager.steamServersDisconnectedCallback = Callback<SteamServersDisconnected_t>.CreateGameServer(new Callback<SteamServersDisconnected_t>.DispatchDelegate(SteamManager.OnSteamServersDisconnected));
			return;
		}
		SteamManager.steamServersConnectedCallback = Callback<SteamServersConnected_t>.Create(new Callback<SteamServersConnected_t>.DispatchDelegate(SteamManager.OnSteamServersConnected));
		SteamManager.steamServerConnectFailureCallback = Callback<SteamServerConnectFailure_t>.Create(new Callback<SteamServerConnectFailure_t>.DispatchDelegate(SteamManager.OnSteamServerConnectFailure));
		SteamManager.steamServersDisconnectedCallback = Callback<SteamServersDisconnected_t>.Create(new Callback<SteamServersDisconnected_t>.DispatchDelegate(SteamManager.OnSteamServersDisconnected));
	}

	// Token: 0x06000905 RID: 2309 RVA: 0x0002BD05 File Offset: 0x00029F05
	private static void UnregisterCallbacks()
	{
		if (!SteamManager.IsInitialized)
		{
			return;
		}
		SteamManager.steamServersConnectedCallback.Unregister();
		SteamManager.steamServerConnectFailureCallback.Unregister();
		SteamManager.steamServersDisconnectedCallback.Unregister();
	}

	// Token: 0x06000906 RID: 2310 RVA: 0x0002BD30 File Offset: 0x00029F30
	private static void StartCallbackLoop()
	{
		if (!SteamManager.IsInitialized)
		{
			return;
		}
		Tween tween = SteamManager.callbackTween;
		if (tween != null)
		{
			tween.Kill(false);
		}
		SteamManager.callbackTween = DOVirtual.DelayedCall(SteamManager.RunCallbackInterval, delegate
		{
			if (ApplicationManager.IsDedicatedGameServer)
			{
				GameServer.RunCallbacks();
			}
			else
			{
				SteamAPI.RunCallbacks();
			}
			SteamManager.StartCallbackLoop();
		}, true);
	}

	// Token: 0x06000907 RID: 2311 RVA: 0x0002BD85 File Offset: 0x00029F85
	private static void StopCallbackLoop()
	{
		if (!SteamManager.IsInitialized)
		{
			return;
		}
		Tween tween = SteamManager.callbackTween;
		if (tween != null)
		{
			tween.Kill(false);
		}
		SteamManager.callbackTween = null;
	}

	// Token: 0x06000908 RID: 2312 RVA: 0x0002BDA6 File Offset: 0x00029FA6
	private static void OnSteamServersConnected(SteamServersConnected_t callback)
	{
		Debug.Log("[SteamManager] Connected to Steam");
		SteamIntegrationManager.Initialize();
		SteamWorkshopManager.Initialize();
		SteamManager.IsConnected = true;
		EventManager.TriggerEvent("Event_OnSteamConnected", null);
	}

	// Token: 0x06000909 RID: 2313 RVA: 0x0002BDCD File Offset: 0x00029FCD
	private static void OnSteamServerConnectFailure(SteamServerConnectFailure_t callback)
	{
		Debug.LogError(string.Format("[SteamManager] Failed to connect to Steam: {0}", callback.m_eResult));
		EventManager.TriggerEvent("Event_OnSteamConnectionFailed", null);
	}

	// Token: 0x0600090A RID: 2314 RVA: 0x0002BDF4 File Offset: 0x00029FF4
	private static void OnSteamServersDisconnected(SteamServersDisconnected_t callback)
	{
		Debug.LogWarning(string.Format("[SteamManager] Disconnected from Steam: {0}", callback.m_eResult));
		SteamManager.IsConnected = false;
		SteamWorkshopManager.Dispose();
		SteamIntegrationManager.Dispose();
		EventManager.TriggerEvent("Event_OnSteamDisconnected", null);
	}

	// Token: 0x04000535 RID: 1333
	public static bool IsInitialized = false;

	// Token: 0x04000536 RID: 1334
	public static float RunCallbackInterval = 0.033333335f;

	// Token: 0x04000537 RID: 1335
	public static bool IsConnected = false;

	// Token: 0x04000538 RID: 1336
	private static Callback<SteamServersConnected_t> steamServersConnectedCallback;

	// Token: 0x04000539 RID: 1337
	private static Callback<SteamServerConnectFailure_t> steamServerConnectFailureCallback;

	// Token: 0x0400053A RID: 1338
	private static Callback<SteamServersDisconnected_t> steamServersDisconnectedCallback;

	// Token: 0x0400053B RID: 1339
	private static Tween callbackTween;

	// Token: 0x0400053C RID: 1340
	private static Tween steamInitializationRetryTween;
}
