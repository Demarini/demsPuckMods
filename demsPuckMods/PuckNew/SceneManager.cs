using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x0200010D RID: 269
public static class SceneManager
{
	// Token: 0x170000D2 RID: 210
	// (get) Token: 0x06000750 RID: 1872 RVA: 0x00024404 File Offset: 0x00022604
	public static bool IsNetworkSceneManagerAvailable
	{
		get
		{
			return NetworkManager.Singleton != null && NetworkManager.Singleton.SceneManager != null && NetworkManager.Singleton.IsServer;
		}
	}

	// Token: 0x06000751 RID: 1873 RVA: 0x0002442B File Offset: 0x0002262B
	public static void Initialize()
	{
		UnityEngine.SceneManagement.SceneManager.sceneLoaded += global::SceneManager.OnSceneLoaded;
		UnityEngine.SceneManagement.SceneManager.sceneUnloaded += global::SceneManager.OnSceneUnloaded;
		SceneManagerController.Initialize();
	}

	// Token: 0x06000752 RID: 1874 RVA: 0x00024454 File Offset: 0x00022654
	public static void Dispose()
	{
		UnityEngine.SceneManagement.SceneManager.sceneLoaded -= global::SceneManager.OnSceneLoaded;
		UnityEngine.SceneManagement.SceneManager.sceneUnloaded -= global::SceneManager.OnSceneUnloaded;
		SceneManagerController.Dispose();
	}

	// Token: 0x06000753 RID: 1875 RVA: 0x0002447D File Offset: 0x0002267D
	public static void InitializeServer()
	{
		if (!global::SceneManager.IsNetworkSceneManagerAvailable)
		{
			return;
		}
		global::SceneManager.IsSceneLoadInProgress = false;
		global::SceneManager.IsInitialSceneLoaded = false;
		NetworkManager.Singleton.SceneManager.OnSceneEvent += global::SceneManager.Server_OnSceneEvent;
	}

	// Token: 0x06000754 RID: 1876 RVA: 0x000244AE File Offset: 0x000226AE
	public static void DisposeServer()
	{
		if (!global::SceneManager.IsNetworkSceneManagerAvailable)
		{
			return;
		}
		NetworkManager.Singleton.SceneManager.OnSceneEvent -= global::SceneManager.Server_OnSceneEvent;
		global::SceneManager.IsSceneLoadInProgress = false;
		global::SceneManager.IsInitialSceneLoaded = false;
	}

	// Token: 0x06000755 RID: 1877 RVA: 0x000244E0 File Offset: 0x000226E0
	public static void LoadScene(string sceneName)
	{
		if (global::SceneManager.IsNetworkSceneManagerAvailable)
		{
			Debug.Log("[SceneManager] Loading server scene " + sceneName);
			NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
			return;
		}
		Debug.Log("[SceneManager] Loading scene " + sceneName);
		UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
	}

	// Token: 0x06000756 RID: 1878 RVA: 0x0002452D File Offset: 0x0002272D
	private static void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
	{
		EventManager.TriggerEvent("Event_OnSceneLoaded", new Dictionary<string, object>
		{
			{
				"scene",
				scene
			}
		});
	}

	// Token: 0x06000757 RID: 1879 RVA: 0x0002454F File Offset: 0x0002274F
	private static void OnSceneUnloaded(Scene scene)
	{
		EventManager.TriggerEvent("Event_OnSceneUnloaded", new Dictionary<string, object>
		{
			{
				"scene",
				scene
			}
		});
	}

	// Token: 0x06000758 RID: 1880 RVA: 0x00024574 File Offset: 0x00022774
	private static void Server_OnSceneEvent(SceneEvent sceneEvent)
	{
		switch (sceneEvent.SceneEventType)
		{
		case SceneEventType.Load:
			global::SceneManager.Server_OnLoadScene();
			return;
		case SceneEventType.Unload:
		case SceneEventType.Synchronize:
		case SceneEventType.ReSynchronize:
		case SceneEventType.UnloadEventCompleted:
			break;
		case SceneEventType.LoadEventCompleted:
			global::SceneManager.Server_OnLoadSceneEventCompleted(sceneEvent.ClientsThatCompleted, sceneEvent.ClientsThatTimedOut);
			break;
		case SceneEventType.LoadComplete:
			global::SceneManager.Server_OnClientSceneLoadComplete(sceneEvent.ClientId);
			return;
		case SceneEventType.UnloadComplete:
			global::SceneManager.Server_OnClientSceneUnloadComplete(sceneEvent.ClientId);
			return;
		case SceneEventType.SynchronizeComplete:
			global::SceneManager.Server_OnClientSceneSynchronizeComplete(sceneEvent.ClientId);
			return;
		default:
			return;
		}
	}

	// Token: 0x06000759 RID: 1881 RVA: 0x000245EE File Offset: 0x000227EE
	private static void Server_OnLoadScene()
	{
		global::SceneManager.IsSceneLoadInProgress = true;
		Debug.Log("[SceneManager] Server started loading scene");
		EventManager.TriggerEvent("Event_Server_OnLoadScene", null);
	}

	// Token: 0x0600075A RID: 1882 RVA: 0x0002460B File Offset: 0x0002280B
	private static void Server_OnClientSceneLoadComplete(ulong clientId)
	{
		Debug.Log(string.Format("[SceneManager] Client {0} completed scene load", clientId));
		EventManager.TriggerEvent("Event_Server_OnClientSceneLoadComplete", new Dictionary<string, object>
		{
			{
				"clientId",
				clientId
			}
		});
	}

	// Token: 0x0600075B RID: 1883 RVA: 0x00024642 File Offset: 0x00022842
	private static void Server_OnClientSceneUnloadComplete(ulong clientId)
	{
		Debug.Log(string.Format("[SceneManager] Client {0} completed scene unload", clientId));
		EventManager.TriggerEvent("Event_Server_OnClientSceneUnloadComplete", new Dictionary<string, object>
		{
			{
				"clientId",
				clientId
			}
		});
	}

	// Token: 0x0600075C RID: 1884 RVA: 0x00024679 File Offset: 0x00022879
	private static void Server_OnClientSceneSynchronizeComplete(ulong clientId)
	{
		Debug.Log(string.Format("[SceneManager] Client {0} completed scene synchronization", clientId));
		EventManager.TriggerEvent("Event_Server_OnClientSceneSynchronizeComplete", new Dictionary<string, object>
		{
			{
				"clientId",
				clientId
			}
		});
	}

	// Token: 0x0600075D RID: 1885 RVA: 0x000246B0 File Offset: 0x000228B0
	private static void Server_OnLoadSceneEventCompleted(List<ulong> clientsThatCompleted, List<ulong> clientsThatTimedOut)
	{
		Debug.Log("[SceneManager] Scene load event completed on server");
		global::SceneManager.IsSceneLoadInProgress = false;
		bool flag = !global::SceneManager.IsInitialSceneLoaded;
		global::SceneManager.IsInitialSceneLoaded = true;
		EventManager.TriggerEvent("Event_Server_OnLoadSceneEventCompleted", new Dictionary<string, object>
		{
			{
				"clientsThatCompleted",
				clientsThatCompleted
			},
			{
				"clientsThatTimedOut",
				clientsThatTimedOut
			},
			{
				"isInitialScene",
				flag
			}
		});
	}

	// Token: 0x0400047A RID: 1146
	public static bool IsSceneLoadInProgress;

	// Token: 0x0400047B RID: 1147
	public static bool IsInitialSceneLoaded;
}
