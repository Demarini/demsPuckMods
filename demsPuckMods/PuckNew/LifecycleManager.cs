using System;
using UnityEngine;

// Token: 0x020000BD RID: 189
public static class LifecycleManager
{
	// Token: 0x060005C7 RID: 1479 RVA: 0x0001EFB7 File Offset: 0x0001D1B7
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private static void SubsystemRegistration()
	{
		LogManager.Initialize();
		Application.quitting += LifecycleManager.Dispose;
	}

	// Token: 0x060005C8 RID: 1480 RVA: 0x0001EFCF File Offset: 0x0001D1CF
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void BeforeSceneLoad()
	{
		PatchManager.Initialize();
		EventManager.Initialize();
		GlobalStateManager.Initialize();
		SaveManager.Initialize();
		InputManager.Initialize();
		SettingsManager.Initialize();
		ApplicationManager.Initialize();
		BackendManager.Initialize();
		ItemManager.Initialize();
		CameraManager.Initialize();
	}

	// Token: 0x060005C9 RID: 1481 RVA: 0x0001F003 File Offset: 0x0001D203
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	private static void AfterSceneLoad()
	{
		SceneManager.Initialize();
		WebSocketManager.Initialize();
		SteamManager.Initialize();
	}

	// Token: 0x060005CA RID: 1482 RVA: 0x0001F014 File Offset: 0x0001D214
	public static void Dispose()
	{
		SteamManager.Dispose();
		WebSocketManager.Dispose();
		SceneManager.Dispose();
		CameraManager.Dispose();
		ItemManager.Dispose();
		BackendManager.Dispose();
		ApplicationManager.Dispose();
		SettingsManager.Dispose();
		InputManager.Dispose();
		SaveManager.Dispose();
		GlobalStateManager.Dispose();
		EventManager.Dispose();
		PatchManager.Dispose();
		LogManager.Dispose();
	}
}
