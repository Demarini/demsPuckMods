using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

// Token: 0x02000081 RID: 129
public static class ApplicationManagerController
{
	// Token: 0x06000467 RID: 1127 RVA: 0x000182AC File Offset: 0x000164AC
	public static Task Initialize()
	{
		ApplicationManagerController.<Initialize>d__0 <Initialize>d__;
		<Initialize>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<Initialize>d__.<>1__state = -1;
		<Initialize>d__.<>t__builder.Start<ApplicationManagerController.<Initialize>d__0>(ref <Initialize>d__);
		return <Initialize>d__.<>t__builder.Task;
	}

	// Token: 0x06000468 RID: 1128 RVA: 0x000182E8 File Offset: 0x000164E8
	public static void Dispose()
	{
		EventManager.RemoveEventListener("Event_OnFullScreenModeChanged", new Action<Dictionary<string, object>>(ApplicationManagerController.Event_OnFullScreenModeChanged));
		EventManager.RemoveEventListener("Event_OnDisplayIndexChanged", new Action<Dictionary<string, object>>(ApplicationManagerController.Event_OnDisplayIndexChanged));
		EventManager.RemoveEventListener("Event_OnResolutionIndexChanged", new Action<Dictionary<string, object>>(ApplicationManagerController.Event_OnResolutionIndexChanged));
		EventManager.RemoveEventListener("Event_OnVSyncChanged", new Action<Dictionary<string, object>>(ApplicationManagerController.Event_OnVSyncChanged));
		EventManager.RemoveEventListener("Event_OnFpsLimitChanged", new Action<Dictionary<string, object>>(ApplicationManagerController.Event_OnFpsLimitChanged));
		EventManager.RemoveEventListener("Event_OnQualityChanged", new Action<Dictionary<string, object>>(ApplicationManagerController.Event_OnQualityChanged));
		EventManager.RemoveEventListener("Event_OnUIStateChanged", new Action<Dictionary<string, object>>(ApplicationManagerController.Event_OnUIStateChanged));
		EventManager.RemoveEventListener("Event_OnSocialClickDiscord", new Action<Dictionary<string, object>>(ApplicationManagerController.Event_OnSocialClickDiscord));
		EventManager.RemoveEventListener("Event_OnSocialClickPatreon", new Action<Dictionary<string, object>>(ApplicationManagerController.Event_OnSocialClickPatreon));
		EventManager.RemoveEventListener("Event_Server_OnServerStarted", new Action<Dictionary<string, object>>(ApplicationManagerController.Event_Server_OnServerStarted));
	}

	// Token: 0x06000469 RID: 1129 RVA: 0x000183D4 File Offset: 0x000165D4
	private static void Event_OnFullScreenModeChanged(Dictionary<string, object> message)
	{
		FullScreenMode fullScreenMode = (FullScreenMode)message["value"];
		if (ApplicationManager.IsDedicatedGameServer)
		{
			return;
		}
		ApplicationManager.SetFullScreenMode(fullScreenMode);
	}

	// Token: 0x0600046A RID: 1130 RVA: 0x00018400 File Offset: 0x00016600
	private static void Event_OnDisplayIndexChanged(Dictionary<string, object> message)
	{
		ApplicationManagerController.<Event_OnDisplayIndexChanged>d__3 <Event_OnDisplayIndexChanged>d__;
		<Event_OnDisplayIndexChanged>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<Event_OnDisplayIndexChanged>d__.message = message;
		<Event_OnDisplayIndexChanged>d__.<>1__state = -1;
		<Event_OnDisplayIndexChanged>d__.<>t__builder.Start<ApplicationManagerController.<Event_OnDisplayIndexChanged>d__3>(ref <Event_OnDisplayIndexChanged>d__);
	}

	// Token: 0x0600046B RID: 1131 RVA: 0x00018438 File Offset: 0x00016638
	private static void Event_OnResolutionIndexChanged(Dictionary<string, object> message)
	{
		int resolution = (int)message["value"];
		if (ApplicationManager.IsDedicatedGameServer)
		{
			return;
		}
		ApplicationManager.SetResolution(resolution);
	}

	// Token: 0x0600046C RID: 1132 RVA: 0x00018464 File Offset: 0x00016664
	private static void Event_OnVSyncChanged(Dictionary<string, object> message)
	{
		bool vsync = (bool)message["value"];
		if (ApplicationManager.IsDedicatedGameServer)
		{
			return;
		}
		ApplicationManager.SetVSync(vsync);
	}

	// Token: 0x0600046D RID: 1133 RVA: 0x00018490 File Offset: 0x00016690
	private static void Event_OnFpsLimitChanged(Dictionary<string, object> message)
	{
		int targetFrameRate = (int)message["value"];
		if (ApplicationManager.IsDedicatedGameServer)
		{
			return;
		}
		ApplicationManager.SetTargetFrameRate(targetFrameRate);
	}

	// Token: 0x0600046E RID: 1134 RVA: 0x000184BC File Offset: 0x000166BC
	private static void Event_OnQualityChanged(Dictionary<string, object> message)
	{
		ApplicationQuality quality = (ApplicationQuality)message["value"];
		if (ApplicationManager.IsDedicatedGameServer)
		{
			return;
		}
		ApplicationManager.SetQuality(quality);
	}

	// Token: 0x0600046F RID: 1135 RVA: 0x000184E8 File Offset: 0x000166E8
	private static void Event_OnUIStateChanged(Dictionary<string, object> message)
	{
		UIState uistate = (UIState)message["oldUIState"];
		UIState uistate2 = (UIState)message["newUIState"];
		if (ApplicationManager.IsDedicatedGameServer)
		{
			return;
		}
		if (uistate.IsMouseRequired == uistate2.IsMouseRequired)
		{
			return;
		}
		ApplicationManager.SetMouseVisibility(uistate2.IsMouseRequired);
	}

	// Token: 0x06000470 RID: 1136 RVA: 0x00018539 File Offset: 0x00016739
	private static void Event_OnSocialClickDiscord(Dictionary<string, object> message)
	{
		if (ApplicationManager.IsDedicatedGameServer)
		{
			return;
		}
		Application.OpenURL("https://discord.gg/AZDBj6XsGg");
	}

	// Token: 0x06000471 RID: 1137 RVA: 0x0001854D File Offset: 0x0001674D
	private static void Event_OnSocialClickPatreon(Dictionary<string, object> message)
	{
		if (ApplicationManager.IsDedicatedGameServer)
		{
			return;
		}
		Application.OpenURL("https://www.patreon.com/c/PuckGame");
	}

	// Token: 0x06000472 RID: 1138 RVA: 0x00018564 File Offset: 0x00016764
	private static void Event_Server_OnServerStarted(Dictionary<string, object> message)
	{
		ServerConfig serverConfig = (ServerConfig)message["serverConfig"];
		if (ApplicationManager.IsDedicatedGameServer)
		{
			ApplicationManager.SetTargetFrameRate(serverConfig.tickRate);
		}
	}
}
