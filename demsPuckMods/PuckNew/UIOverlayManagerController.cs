using System;
using System.Collections.Generic;
using Unity.Netcode;

// Token: 0x020001AB RID: 427
internal class UIOverlayManagerController : UIViewController<UIOverlayManager>
{
	// Token: 0x06000C42 RID: 3138 RVA: 0x0003A074 File Offset: 0x00038274
	public override void Awake()
	{
		base.Awake();
		this.uiOverlay = base.GetComponent<UIOverlayManager>();
		EventManager.AddEventListener("Event_Everyone_OnClientConnected", new Action<Dictionary<string, object>>(this.Event_Everyone_OnClientConnected));
		EventManager.AddEventListener("Event_OnBaseCameraEnabled", new Action<Dictionary<string, object>>(this.Event_OnBaseCameraEnabled));
		EventManager.AddEventListener("Event_OnBaseCameraDisabled", new Action<Dictionary<string, object>>(this.Event_OnBaseCameraDisabled));
		EventManager.AddEventListener("Event_OnClientStarted", new Action<Dictionary<string, object>>(this.Event_OnClientStarted));
		EventManager.AddEventListener("Event_OnClientStopped", new Action<Dictionary<string, object>>(this.Event_OnClientStopped));
		EventManager.AddEventListener("Event_OnPopupShow", new Action<Dictionary<string, object>>(this.Event_OnPopupShow));
		EventManager.AddEventListener("Event_OnPopupHide", new Action<Dictionary<string, object>>(this.Event_OnPopupHide));
		WebSocketManager.AddMessageListener("playerData", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnPlayerData));
	}

	// Token: 0x06000C43 RID: 3139 RVA: 0x0003A143 File Offset: 0x00038343
	private void Start()
	{
		this.uiOverlay.ShowOverlay("loading", true, false, true, 0.25f, false, 0.25f);
	}

	// Token: 0x06000C44 RID: 3140 RVA: 0x0003A164 File Offset: 0x00038364
	public override void OnDestroy()
	{
		EventManager.RemoveEventListener("Event_Everyone_OnClientConnected", new Action<Dictionary<string, object>>(this.Event_Everyone_OnClientConnected));
		EventManager.RemoveEventListener("Event_OnBaseCameraEnabled", new Action<Dictionary<string, object>>(this.Event_OnBaseCameraEnabled));
		EventManager.RemoveEventListener("Event_OnBaseCameraDisabled", new Action<Dictionary<string, object>>(this.Event_OnBaseCameraDisabled));
		EventManager.RemoveEventListener("Event_OnClientStarted", new Action<Dictionary<string, object>>(this.Event_OnClientStarted));
		EventManager.RemoveEventListener("Event_OnClientStopped", new Action<Dictionary<string, object>>(this.Event_OnClientStopped));
		EventManager.RemoveEventListener("Event_OnPopupShow", new Action<Dictionary<string, object>>(this.Event_OnPopupShow));
		EventManager.RemoveEventListener("Event_OnPopupHide", new Action<Dictionary<string, object>>(this.Event_OnPopupHide));
		WebSocketManager.RemoveMessageListener("playerData", new Action<Dictionary<string, object>>(this.WebSocket_Event_OnPlayerData));
		base.OnDestroy();
	}

	// Token: 0x06000C45 RID: 3141 RVA: 0x0003A228 File Offset: 0x00038428
	private void Event_Everyone_OnClientConnected(Dictionary<string, object> message)
	{
		ulong num = (ulong)message["clientId"];
		if (NetworkManager.Singleton.LocalClientId != num)
		{
			return;
		}
		this.uiOverlay.HideOverlay("connecting");
	}

	// Token: 0x06000C46 RID: 3142 RVA: 0x0003A264 File Offset: 0x00038464
	private void Event_OnBaseCameraEnabled(Dictionary<string, object> message)
	{
		this.uiOverlay.HideOverlay("camera");
	}

	// Token: 0x06000C47 RID: 3143 RVA: 0x0003A276 File Offset: 0x00038476
	private void Event_OnBaseCameraDisabled(Dictionary<string, object> message)
	{
		this.uiOverlay.ShowOverlay("camera", false, false, true, 0.25f, false, 0.25f);
	}

	// Token: 0x06000C48 RID: 3144 RVA: 0x0003A296 File Offset: 0x00038496
	private void Event_OnClientStarted(Dictionary<string, object> message)
	{
		this.uiOverlay.ShowOverlay("connecting", true, false, true, 0.25f, false, 0.25f);
	}

	// Token: 0x06000C49 RID: 3145 RVA: 0x0003A2B6 File Offset: 0x000384B6
	private void Event_OnClientStopped(Dictionary<string, object> message)
	{
		this.uiOverlay.HideOverlay("connecting");
	}

	// Token: 0x06000C4A RID: 3146 RVA: 0x0003A2C8 File Offset: 0x000384C8
	private void Event_OnPopupShow(Dictionary<string, object> message)
	{
		string a = (string)message["name"];
		if (a == "missingPassword")
		{
			this.uiOverlay.ShowOverlay("missingPassword", true, false, true, 0.25f, false, 0.25f);
			return;
		}
		if (!(a == "pendingMods"))
		{
			return;
		}
		this.uiOverlay.ShowOverlay("pendingMods", true, false, true, 0.25f, false, 0.25f);
	}

	// Token: 0x06000C4B RID: 3147 RVA: 0x0003A340 File Offset: 0x00038540
	private void Event_OnPopupHide(Dictionary<string, object> message)
	{
		string a = (string)message["name"];
		if (a == "missingPassword")
		{
			this.uiOverlay.HideOverlay("missingPassword");
			return;
		}
		if (!(a == "pendingMods"))
		{
			return;
		}
		this.uiOverlay.HideOverlay("pendingMods");
	}

	// Token: 0x06000C4C RID: 3148 RVA: 0x0003A39A File Offset: 0x0003859A
	private void WebSocket_Event_OnPlayerData(Dictionary<string, object> message)
	{
		this.uiOverlay.HideOverlay("loading");
	}

	// Token: 0x0400074C RID: 1868
	private UIOverlayManager uiOverlay;
}
