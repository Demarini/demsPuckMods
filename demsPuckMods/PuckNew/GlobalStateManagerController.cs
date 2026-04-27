using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

// Token: 0x020000AB RID: 171
public static class GlobalStateManagerController
{
	// Token: 0x06000568 RID: 1384 RVA: 0x0001D845 File Offset: 0x0001BA45
	public static void Initialize()
	{
		EventManager.AddEventListener("Event_OnSceneLoaded", new Action<Dictionary<string, object>>(GlobalStateManagerController.Event_OnSceneLoaded));
	}

	// Token: 0x06000569 RID: 1385 RVA: 0x0001D85D File Offset: 0x0001BA5D
	public static void Dispose()
	{
		EventManager.RemoveEventListener("Event_OnSceneLoaded", new Action<Dictionary<string, object>>(GlobalStateManagerController.Event_OnSceneLoaded));
	}

	// Token: 0x0600056A RID: 1386 RVA: 0x0001D878 File Offset: 0x0001BA78
	private static void Event_OnSceneLoaded(Dictionary<string, object> message)
	{
		if (((Scene)message["scene"]).name == "level_default")
		{
			GlobalStateManager.SetUIState(new Dictionary<string, object>
			{
				{
					"phase",
					UIPhase.Playing
				}
			});
			return;
		}
		GlobalStateManager.SetUIState(new Dictionary<string, object>
		{
			{
				"phase",
				UIPhase.LockerRoom
			}
		});
	}
}
