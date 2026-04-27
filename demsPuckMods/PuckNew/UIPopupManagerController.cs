using System;
using System.Collections.Generic;
using System.Globalization;
using Humanizer;
using UnityEngine;

// Token: 0x020001BA RID: 442
internal class UIPopupManagerController : UIViewController<UIPopupManager>
{
	// Token: 0x06000CAD RID: 3245 RVA: 0x0003B844 File Offset: 0x00039A44
	public override void Awake()
	{
		base.Awake();
		this.uiPopupManager = base.GetComponent<UIPopupManager>();
		EventManager.AddEventListener("Event_OnIdentityClickConfirm", new Action<Dictionary<string, object>>(this.Event_OnIdentityClickConfirm));
		EventManager.AddEventListener("Event_OnMainMenuClickExitGame", new Action<Dictionary<string, object>>(this.Event_OnMainMenuClickExitGame));
		EventManager.AddEventListener("Event_OnPauseMenuClickExitGame", new Action<Dictionary<string, object>>(this.Event_OnPauseMenuClickExitGame));
		EventManager.AddEventListener("Event_OnPlayerBanned", new Action<Dictionary<string, object>>(this.Event_OnPlayerBanned));
		EventManager.AddEventListener("Event_OnPlayerMuted", new Action<Dictionary<string, object>>(this.Event_OnPlayerMuted));
		EventManager.AddEventListener("Event_OnPlayerCooldown", new Action<Dictionary<string, object>>(this.Event_OnPlayerCooldown));
		EventManager.AddEventListener("Event_OnSettingsClickResetToDefault", new Action<Dictionary<string, object>>(this.Event_OnSettingsClickResetToDefault));
		EventManager.AddEventListener("Event_OnBeforePendingModsSet", new Action<Dictionary<string, object>>(this.Event_OnBeforePendingModsSet));
		EventManager.AddEventListener("Event_OnPendingModsReset", new Action<Dictionary<string, object>>(this.Event_OnPendingModsReset));
		EventManager.AddEventListener("Event_OnPendingModsCleared", new Action<Dictionary<string, object>>(this.Event_OnPendingModsCleared));
		EventManager.AddEventListener("Event_OnConnectionRejected", new Action<Dictionary<string, object>>(this.Event_OnConnectionRejected));
		EventManager.AddEventListener("Event_OnPopupClickOk", new Action<Dictionary<string, object>>(this.Event_OnPopupClickOk));
		EventManager.AddEventListener("Event_OnPopupClickClose", new Action<Dictionary<string, object>>(this.Event_OnPopupClickClose));
		EventManager.AddEventListener("Event_OnKeyBindRebindStart", new Action<Dictionary<string, object>>(this.Event_OnKeyBindRebindStart));
		EventManager.AddEventListener("Event_OnKeyBindRebindComplete", new Action<Dictionary<string, object>>(this.Event_OnKeyBindRebindComplete));
		EventManager.AddEventListener("Event_OnKeyBindRebindCancel", new Action<Dictionary<string, object>>(this.Event_OnKeyBindRebindCancel));
	}

	// Token: 0x06000CAE RID: 3246 RVA: 0x0003B9C4 File Offset: 0x00039BC4
	public override void OnDestroy()
	{
		EventManager.RemoveEventListener("Event_OnIdentityClickConfirm", new Action<Dictionary<string, object>>(this.Event_OnIdentityClickConfirm));
		EventManager.RemoveEventListener("Event_OnMainMenuClickExitGame", new Action<Dictionary<string, object>>(this.Event_OnMainMenuClickExitGame));
		EventManager.RemoveEventListener("Event_OnPauseMenuClickExitGame", new Action<Dictionary<string, object>>(this.Event_OnPauseMenuClickExitGame));
		EventManager.RemoveEventListener("Event_OnPlayerBanned", new Action<Dictionary<string, object>>(this.Event_OnPlayerBanned));
		EventManager.RemoveEventListener("Event_OnPlayerMuted", new Action<Dictionary<string, object>>(this.Event_OnPlayerMuted));
		EventManager.RemoveEventListener("Event_OnPlayerCooldown", new Action<Dictionary<string, object>>(this.Event_OnPlayerCooldown));
		EventManager.RemoveEventListener("Event_OnSettingsClickResetToDefault", new Action<Dictionary<string, object>>(this.Event_OnSettingsClickResetToDefault));
		EventManager.RemoveEventListener("Event_OnBeforePendingModsSet", new Action<Dictionary<string, object>>(this.Event_OnBeforePendingModsSet));
		EventManager.RemoveEventListener("Event_OnPendingModsReset", new Action<Dictionary<string, object>>(this.Event_OnPendingModsReset));
		EventManager.RemoveEventListener("Event_OnPendingModsCleared", new Action<Dictionary<string, object>>(this.Event_OnPendingModsCleared));
		EventManager.RemoveEventListener("Event_OnConnectionRejected", new Action<Dictionary<string, object>>(this.Event_OnConnectionRejected));
		EventManager.RemoveEventListener("Event_OnPopupClickOk", new Action<Dictionary<string, object>>(this.Event_OnPopupClickOk));
		EventManager.RemoveEventListener("Event_OnPopupClickClose", new Action<Dictionary<string, object>>(this.Event_OnPopupClickClose));
		EventManager.RemoveEventListener("Event_OnKeyBindRebindStart", new Action<Dictionary<string, object>>(this.Event_OnKeyBindRebindStart));
		EventManager.RemoveEventListener("Event_OnKeyBindRebindComplete", new Action<Dictionary<string, object>>(this.Event_OnKeyBindRebindComplete));
		EventManager.RemoveEventListener("Event_OnKeyBindRebindCancel", new Action<Dictionary<string, object>>(this.Event_OnKeyBindRebindCancel));
		base.OnDestroy();
	}

	// Token: 0x06000CAF RID: 3247 RVA: 0x0003BB38 File Offset: 0x00039D38
	private void Event_OnIdentityClickConfirm(Dictionary<string, object> message)
	{
		string value = (string)message["username"];
		int num = (int)message["number"];
		PopupTextContent content = this.uiPopupManager.CreateTextContent("<align=center>Identity can be changed once every 24 hours.<br>Are you sure you want to continue?", new Dictionary<string, object>
		{
			{
				"username",
				value
			},
			{
				"number",
				num
			}
		});
		this.uiPopupManager.ShowPopup("identity", "IDENTITY", content, true, true);
	}

	// Token: 0x06000CB0 RID: 3248 RVA: 0x0003BBB4 File Offset: 0x00039DB4
	private void Event_OnMainMenuClickExitGame(Dictionary<string, object> message)
	{
		PopupTextContent content = this.uiPopupManager.CreateTextContent("<align=center>Are you sure you want to exit the game?", null);
		this.uiPopupManager.ShowPopup("mainMenuExitGame", "EXIT GAME", content, true, true);
	}

	// Token: 0x06000CB1 RID: 3249 RVA: 0x0003BBEC File Offset: 0x00039DEC
	private void Event_OnPauseMenuClickExitGame(Dictionary<string, object> message)
	{
		PopupTextContent content = this.uiPopupManager.CreateTextContent("<align=center>Are you sure you want to exit the game?", null);
		this.uiPopupManager.ShowPopup("pauseMenuExitGame", "EXIT GAME", content, true, true);
	}

	// Token: 0x06000CB2 RID: 3250 RVA: 0x0003BC24 File Offset: 0x00039E24
	private void Event_OnPlayerBanned(Dictionary<string, object> message)
	{
		string text = (string)message["reason"];
		long num = (long)((double)message["expiresAt"]);
		DateTime utcNow = DateTime.UtcNow;
		TimeSpan timeSpan = DateTimeOffset.FromUnixTimeMilliseconds(num).DateTime.Subtract(utcNow);
		string text2 = "<align=center>Your account has been banned<br>Banned for " + timeSpan.Humanize(2, CultureInfo.InvariantCulture, TimeUnit.Week, TimeUnit.Millisecond, ", ", false);
		if (!string.IsNullOrEmpty(text))
		{
			text2 = text2 + "<br><br><align=left>" + text;
		}
		PopupTextContent content = this.uiPopupManager.CreateTextContent(text2, null);
		this.uiPopupManager.ShowPopup("banned", "BANNED", content, true, true);
	}

	// Token: 0x06000CB3 RID: 3251 RVA: 0x0003BCD4 File Offset: 0x00039ED4
	private void Event_OnPlayerMuted(Dictionary<string, object> message)
	{
		string text = (string)message["reason"];
		long num = (long)((double)message["expiresAt"]);
		DateTime utcNow = DateTime.UtcNow;
		TimeSpan timeSpan = DateTimeOffset.FromUnixTimeMilliseconds(num).DateTime.Subtract(utcNow);
		string text2 = "<align=center>Your account has been muted<br>Muted for " + timeSpan.Humanize(2, CultureInfo.InvariantCulture, TimeUnit.Week, TimeUnit.Millisecond, ", ", false);
		if (!string.IsNullOrEmpty(text))
		{
			text2 = text2 + "<br><br><align=left>" + text;
		}
		PopupTextContent content = this.uiPopupManager.CreateTextContent(text2, null);
		this.uiPopupManager.ShowPopup("muted", "MUTED", content, true, true);
	}

	// Token: 0x06000CB4 RID: 3252 RVA: 0x0003BD84 File Offset: 0x00039F84
	private void Event_OnPlayerCooldown(Dictionary<string, object> message)
	{
		long num = (long)((double)message["expiresAt"]);
		DateTime utcNow = DateTime.UtcNow;
		TimeSpan timeSpan = DateTimeOffset.FromUnixTimeMilliseconds(num).DateTime.Subtract(utcNow);
		string text = "<align=center>Your account has received a matchmaking cooldown<br>Expires in " + timeSpan.Humanize(2, CultureInfo.InvariantCulture, TimeUnit.Week, TimeUnit.Millisecond, ", ", false);
		PopupTextContent content = this.uiPopupManager.CreateTextContent(text, null);
		this.uiPopupManager.ShowPopup("cooldown", "COOLDOWN", content, true, true);
	}

	// Token: 0x06000CB5 RID: 3253 RVA: 0x0003BE08 File Offset: 0x0003A008
	private void Event_OnSettingsClickResetToDefault(Dictionary<string, object> message)
	{
		PopupTextContent content = this.uiPopupManager.CreateTextContent("<align=center>This will reset all settings to their default values, including key binds. Are you sure you want to continue?", null);
		this.uiPopupManager.ShowPopup("settingsResetToDefault", "RESET SETTINGS", content, true, true);
	}

	// Token: 0x06000CB6 RID: 3254 RVA: 0x0003BE40 File Offset: 0x0003A040
	private void Event_OnBeforePendingModsSet(Dictionary<string, object> message)
	{
		ulong[] array = (ulong[])message["ids"];
		PopupTextContent content = this.uiPopupManager.CreateTextContent(string.Format("<align=center>This server requires {0} mods to be installed before connecting. Proceeding with installation...", array.Length), null);
		this.uiPopupManager.ShowPopup("pendingMods", "MODS REQUIRED", content, false, true);
	}

	// Token: 0x06000CB7 RID: 3255 RVA: 0x0003BE95 File Offset: 0x0003A095
	private void Event_OnPendingModsReset(Dictionary<string, object> message)
	{
		this.uiPopupManager.HidePopup("pendingMods");
	}

	// Token: 0x06000CB8 RID: 3256 RVA: 0x0003BE95 File Offset: 0x0003A095
	private void Event_OnPendingModsCleared(Dictionary<string, object> message)
	{
		this.uiPopupManager.HidePopup("pendingMods");
	}

	// Token: 0x06000CB9 RID: 3257 RVA: 0x0003BEA8 File Offset: 0x0003A0A8
	private void Event_OnConnectionRejected(Dictionary<string, object> message)
	{
		if (((ConnectionRejection)message["connectionRejection"]).code != ConnectionRejectionCode.MissingPassword)
		{
			return;
		}
		Debug.Log("Connection rejected due to missing password, showing password popup");
		PopupPasswordContent content = this.uiPopupManager.CreatePasswordContent(null);
		this.uiPopupManager.ShowPopup("missingPassword", "PASSWORD REQUIRED", content, true, true);
	}

	// Token: 0x06000CBA RID: 3258 RVA: 0x0003BF00 File Offset: 0x0003A100
	private void Event_OnPopupClickOk(Dictionary<string, object> message)
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

	// Token: 0x06000CBB RID: 3259 RVA: 0x0003BF50 File Offset: 0x0003A150
	private void Event_OnPopupClickClose(Dictionary<string, object> message)
	{
		string name = (string)message["name"];
		this.uiPopupManager.HidePopup(name);
	}

	// Token: 0x06000CBC RID: 3260 RVA: 0x0003BF7C File Offset: 0x0003A17C
	private void Event_OnKeyBindRebindStart(Dictionary<string, object> message)
	{
		if ((bool)message["isComposite"])
		{
			PopupTextContent content = this.uiPopupManager.CreateTextContent("<align=center>Press a <b>key</b> or combination of <b>modifier + key</b> to rebind", null);
			this.uiPopupManager.ShowPopup("keyBindRebind", "KEY REBIND", content, false, false);
			return;
		}
		PopupTextContent content2 = this.uiPopupManager.CreateTextContent("<align=center>Press a <b>key</b> to rebind", null);
		this.uiPopupManager.ShowPopup("keyBindRebind", "KEY REBIND", content2, false, false);
	}

	// Token: 0x06000CBD RID: 3261 RVA: 0x0003BFF0 File Offset: 0x0003A1F0
	private void Event_OnKeyBindRebindComplete(Dictionary<string, object> message)
	{
		this.uiPopupManager.HidePopup("keyBindRebind");
	}

	// Token: 0x06000CBE RID: 3262 RVA: 0x0003BFF0 File Offset: 0x0003A1F0
	private void Event_OnKeyBindRebindCancel(Dictionary<string, object> message)
	{
		this.uiPopupManager.HidePopup("keyBindRebind");
	}

	// Token: 0x04000788 RID: 1928
	private UIPopupManager uiPopupManager;
}
