using System;
using System.Collections.Generic;

// Token: 0x0200010E RID: 270
public static class SceneManagerController
{
	// Token: 0x0600075E RID: 1886 RVA: 0x00024714 File Offset: 0x00022914
	public static void Initialize()
	{
		EventManager.AddEventListener("Event_Server_OnServerStarted", new Action<Dictionary<string, object>>(SceneManagerController.Event_Server_OnServerStarted));
		EventManager.AddEventListener("Event_Server_OnServerStopped", new Action<Dictionary<string, object>>(SceneManagerController.Event_Server_OnServerStopped));
		EventManager.AddEventListener("Event_OnClientStopped", new Action<Dictionary<string, object>>(SceneManagerController.Event_OnClientStopped));
		if (!ApplicationManager.IsDedicatedGameServer)
		{
			SceneManager.LoadScene("locker_room");
		}
	}

	// Token: 0x0600075F RID: 1887 RVA: 0x00024774 File Offset: 0x00022974
	public static void Dispose()
	{
		EventManager.RemoveEventListener("Event_Server_OnServerStarted", new Action<Dictionary<string, object>>(SceneManagerController.Event_Server_OnServerStarted));
		EventManager.RemoveEventListener("Event_Server_OnServerStopped", new Action<Dictionary<string, object>>(SceneManagerController.Event_Server_OnServerStopped));
		EventManager.RemoveEventListener("Event_OnClientStopped", new Action<Dictionary<string, object>>(SceneManagerController.Event_OnClientStopped));
	}

	// Token: 0x06000760 RID: 1888 RVA: 0x000247C3 File Offset: 0x000229C3
	private static void Event_Server_OnServerStarted(Dictionary<string, object> message)
	{
		ServerConfig serverConfig = (ServerConfig)message["serverConfig"];
		SceneManager.InitializeServer();
		if (serverConfig.level == "default")
		{
			SceneManager.LoadScene("level_default");
		}
	}

	// Token: 0x06000761 RID: 1889 RVA: 0x000247F5 File Offset: 0x000229F5
	private static void Event_Server_OnServerStopped(Dictionary<string, object> message)
	{
		SceneManager.DisposeServer();
	}

	// Token: 0x06000762 RID: 1890 RVA: 0x000247FC File Offset: 0x000229FC
	private static void Event_OnClientStopped(Dictionary<string, object> message)
	{
		SceneManager.LoadScene("locker_room");
	}
}
