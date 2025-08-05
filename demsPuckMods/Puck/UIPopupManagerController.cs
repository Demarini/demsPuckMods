using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000128 RID: 296
internal class UIPopupManagerController : MonoBehaviour
{
	// Token: 0x06000A5D RID: 2653 RVA: 0x0000DA06 File Offset: 0x0000BC06
	private void Awake()
	{
		this.uiPopupManager = base.GetComponent<UIPopupManager>();
	}

	// Token: 0x06000A5E RID: 2654 RVA: 0x0003C3C8 File Offset: 0x0003A5C8
	private void Start()
	{
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnIdentityClickConfirm", new Action<Dictionary<string, object>>(this.Event_Client_OnIdentityClickConfirm));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnMainMenuClickExitGame", new Action<Dictionary<string, object>>(this.Event_Client_OnMainMenuClickExitGame));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnPauseMenuClickExitGame", new Action<Dictionary<string, object>>(this.Event_Client_OnPauseMenuClickExitGame));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnPlayerBanned", new Action<Dictionary<string, object>>(this.Event_Client_OnPlayerBanned));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnPlayerMuted", new Action<Dictionary<string, object>>(this.Event_Client_OnPlayerMuted));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnSettingsClickResetToDefault", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsClickResetToDefault));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnBeforePendingModsSet", new Action<Dictionary<string, object>>(this.Event_Client_OnBeforePendingModsSet));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnPendingModsReset", new Action<Dictionary<string, object>>(this.Event_Client_OnPendingModsReset));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnPendingModsCleared", new Action<Dictionary<string, object>>(this.Event_Client_OnPendingModsCleared));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnConnectionRejected", new Action<Dictionary<string, object>>(this.Event_Client_OnConnectionRejected));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnPopupClickOk", new Action<Dictionary<string, object>>(this.Event_Client_OnPopupClickOk));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnPopupClickClose", new Action<Dictionary<string, object>>(this.Event_Client_OnPopupClickClose));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnKeyBindRebindStart", new Action<Dictionary<string, object>>(this.Event_Client_OnKeyBindRebindStart));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnKeyBindRebindComplete", new Action<Dictionary<string, object>>(this.Event_Client_OnKeyBindRebindComplete));
		MonoBehaviourSingleton<EventManager>.Instance.AddEventListener("Event_Client_OnKeyBindRebindCancel", new Action<Dictionary<string, object>>(this.Event_Client_OnKeyBindRebindCancel));
	}

	// Token: 0x06000A5F RID: 2655 RVA: 0x0003C56C File Offset: 0x0003A76C
	private void OnDestroy()
	{
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnIdentityClickConfirm", new Action<Dictionary<string, object>>(this.Event_Client_OnIdentityClickConfirm));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnMainMenuClickExitGame", new Action<Dictionary<string, object>>(this.Event_Client_OnMainMenuClickExitGame));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnPauseMenuClickExitGame", new Action<Dictionary<string, object>>(this.Event_Client_OnPauseMenuClickExitGame));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnPlayerBanned", new Action<Dictionary<string, object>>(this.Event_Client_OnPlayerBanned));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnPlayerMuted", new Action<Dictionary<string, object>>(this.Event_Client_OnPlayerMuted));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnSettingsClickResetToDefault", new Action<Dictionary<string, object>>(this.Event_Client_OnSettingsClickResetToDefault));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnBeforePendingModsSet", new Action<Dictionary<string, object>>(this.Event_Client_OnBeforePendingModsSet));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnPendingModsReset", new Action<Dictionary<string, object>>(this.Event_Client_OnPendingModsReset));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnPendingModsCleared", new Action<Dictionary<string, object>>(this.Event_Client_OnPendingModsCleared));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnConnectionRejected", new Action<Dictionary<string, object>>(this.Event_Client_OnConnectionRejected));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnPopupClickOk", new Action<Dictionary<string, object>>(this.Event_Client_OnPopupClickOk));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnPopupClickClose", new Action<Dictionary<string, object>>(this.Event_Client_OnPopupClickClose));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnKeyBindRebindStart", new Action<Dictionary<string, object>>(this.Event_Client_OnKeyBindRebindStart));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnKeyBindRebindComplete", new Action<Dictionary<string, object>>(this.Event_Client_OnKeyBindRebindComplete));
		MonoBehaviourSingleton<EventManager>.Instance.RemoveEventListener("Event_Client_OnKeyBindRebindCancel", new Action<Dictionary<string, object>>(this.Event_Client_OnKeyBindRebindCancel));
	}

	// Token: 0x06000A60 RID: 2656 RVA: 0x0003C710 File Offset: 0x0003A910
	private void Event_Client_OnIdentityClickConfirm(Dictionary<string, object> message)
	{
		PopupContentText content = new PopupContentText(this.uiPopupManager.popupContentTextAsset, "<align=center>Your name and number can be changed once every 7 days. Are you sure you want to continue?");
		this.uiPopupManager.ShowPopup("identity", "IDENTITY", content, true, true);
	}

	// Token: 0x06000A61 RID: 2657 RVA: 0x0003C74C File Offset: 0x0003A94C
	private void Event_Client_OnMainMenuClickExitGame(Dictionary<string, object> message)
	{
		PopupContentText content = new PopupContentText(this.uiPopupManager.popupContentTextAsset, "<align=center>Are you sure you want to exit the game?");
		this.uiPopupManager.ShowPopup("mainMenuExitGame", "EXIT GAME", content, true, true);
	}

	// Token: 0x06000A62 RID: 2658 RVA: 0x0003C788 File Offset: 0x0003A988
	private void Event_Client_OnPauseMenuClickExitGame(Dictionary<string, object> message)
	{
		PopupContentText content = new PopupContentText(this.uiPopupManager.popupContentTextAsset, "<align=center>Are you sure you want to exit the game?");
		this.uiPopupManager.ShowPopup("pauseMenuExitGame", "EXIT GAME", content, true, true);
	}

	// Token: 0x06000A63 RID: 2659 RVA: 0x0003C7C4 File Offset: 0x0003A9C4
	private void Event_Client_OnPlayerBanned(Dictionary<string, object> message)
	{
		string str = (string)message["reason"];
		long num = (long)((double)message["until"]);
		DateTime value = DateTime.Now.ToLocalTime();
		TimeSpan timeSpan = DateTimeOffset.FromUnixTimeMilliseconds(num).DateTime.ToLocalTime().Subtract(value);
		string str2 = string.Format("{0} days, {1} hours, {2} minutes", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes);
		PopupContentText content = new PopupContentText(this.uiPopupManager.popupContentTextAsset, "<align=center>Your account has been banned.<br><align=left><br><b>Reason:</b> " + str + "<br><b>Remaining:</b> " + str2);
		this.uiPopupManager.ShowPopup("banned", "BANNED", content, true, true);
	}

	// Token: 0x06000A64 RID: 2660 RVA: 0x0003C890 File Offset: 0x0003AA90
	private void Event_Client_OnPlayerMuted(Dictionary<string, object> message)
	{
		string str = (string)message["reason"];
		long num = (long)((double)message["until"]);
		DateTime value = DateTime.Now.ToLocalTime();
		TimeSpan timeSpan = DateTimeOffset.FromUnixTimeMilliseconds(num).DateTime.ToLocalTime().Subtract(value);
		string str2 = string.Format("{0} days, {1} hours, {2} minutes", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes);
		PopupContentText content = new PopupContentText(this.uiPopupManager.popupContentTextAsset, "<align=center>Your account has been muted.<br><align=left><br><b>Reason:</b> " + str + "<br><b>Remaining:</b> " + str2);
		this.uiPopupManager.ShowPopup("muted", "MUTED", content, true, true);
	}

	// Token: 0x06000A65 RID: 2661 RVA: 0x0003C95C File Offset: 0x0003AB5C
	private void Event_Client_OnSettingsClickResetToDefault(Dictionary<string, object> message)
	{
		PopupContentText content = new PopupContentText(this.uiPopupManager.popupContentTextAsset, "<align=center>This will reset all settings to their default values, including keybinds. Are you sure you want to continue?");
		this.uiPopupManager.ShowPopup("settingsResetToDefault", "RESET SETTINGS", content, true, true);
	}

	// Token: 0x06000A66 RID: 2662 RVA: 0x0003C998 File Offset: 0x0003AB98
	private void Event_Client_OnBeforePendingModsSet(Dictionary<string, object> message)
	{
		ulong[] array = (ulong[])message["ids"];
		PopupContentText content = new PopupContentText(this.uiPopupManager.popupContentTextAsset, string.Format("<align=center>This server requires {0} mods to be installed before connecting. Proceeding with installation...", array.Length));
		this.uiPopupManager.ShowPopup("pendingMods", "MODS REQUIRED", content, false, true);
	}

	// Token: 0x06000A67 RID: 2663 RVA: 0x0000DA14 File Offset: 0x0000BC14
	private void Event_Client_OnPendingModsReset(Dictionary<string, object> message)
	{
		this.uiPopupManager.HidePopup("pendingMods");
	}

	// Token: 0x06000A68 RID: 2664 RVA: 0x0000DA14 File Offset: 0x0000BC14
	private void Event_Client_OnPendingModsCleared(Dictionary<string, object> message)
	{
		this.uiPopupManager.HidePopup("pendingMods");
	}

	// Token: 0x06000A69 RID: 2665 RVA: 0x0003C9F4 File Offset: 0x0003ABF4
	private void Event_Client_OnConnectionRejected(Dictionary<string, object> message)
	{
		if (((ConnectionRejection)message["connectionRejection"]).code != ConnectionRejectionCode.MissingPassword)
		{
			return;
		}
		PopupContentPassword content = new PopupContentPassword(this.uiPopupManager.popupContentPasswordAsset);
		this.uiPopupManager.ShowPopup("missingPassword", "PASSWORD REQUIRED", content, true, true);
	}

	// Token: 0x06000A6A RID: 2666 RVA: 0x0003CA44 File Offset: 0x0003AC44
	private void Event_Client_OnPopupClickOk(Dictionary<string, object> message)
	{
		string text = (string)message["name"];
		this.uiPopupManager.HidePopup(text);
		if (text == "mainMenuExitGame")
		{
			Application.Quit();
			return;
		}
		if (!(text == "pauseMenuExitGame"))
		{
			return;
		}
		Application.Quit();
	}

	// Token: 0x06000A6B RID: 2667 RVA: 0x0003CA94 File Offset: 0x0003AC94
	private void Event_Client_OnPopupClickClose(Dictionary<string, object> message)
	{
		string name = (string)message["name"];
		this.uiPopupManager.HidePopup(name);
	}

	// Token: 0x06000A6C RID: 2668 RVA: 0x0003CAC0 File Offset: 0x0003ACC0
	private void Event_Client_OnKeyBindRebindStart(Dictionary<string, object> message)
	{
		if ((bool)message["isComposite"])
		{
			PopupContentText content = new PopupContentText(this.uiPopupManager.popupContentTextAsset, "<align=center>Press a <b>key</b> or combination of <b>modifier + key</b> to rebind.");
			this.uiPopupManager.ShowPopup("keyBindRebind", "KEY REBIND", content, false, false);
			return;
		}
		PopupContentText content2 = new PopupContentText(this.uiPopupManager.popupContentTextAsset, "<align=center>Press a <b>key</b> to rebind.");
		this.uiPopupManager.ShowPopup("keyBindRebind", "KEY REBIND", content2, false, false);
	}

	// Token: 0x06000A6D RID: 2669 RVA: 0x0000DA26 File Offset: 0x0000BC26
	private void Event_Client_OnKeyBindRebindComplete(Dictionary<string, object> message)
	{
		this.uiPopupManager.HidePopup("keyBindRebind");
	}

	// Token: 0x06000A6E RID: 2670 RVA: 0x0000DA26 File Offset: 0x0000BC26
	private void Event_Client_OnKeyBindRebindCancel(Dictionary<string, object> message)
	{
		this.uiPopupManager.HidePopup("keyBindRebind");
	}

	// Token: 0x04000620 RID: 1568
	private UIPopupManager uiPopupManager;
}
