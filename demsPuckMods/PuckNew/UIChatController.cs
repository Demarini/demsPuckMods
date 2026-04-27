using System;
using System.Collections.Generic;

// Token: 0x0200018B RID: 395
public class UIChatController : UIViewController<UIChat>
{
	// Token: 0x06000B3E RID: 2878 RVA: 0x000357A4 File Offset: 0x000339A4
	public override void Awake()
	{
		base.Awake();
		this.uiChat = base.GetComponent<UIChat>();
		EventManager.AddEventListener("Event_OnChatMessageAdded", new Action<Dictionary<string, object>>(this.Event_OnChatMessageAdded));
		EventManager.AddEventListener("Event_OnChatMessageRemoved", new Action<Dictionary<string, object>>(this.Event_OnChatMessageRemoved));
		EventManager.AddEventListener("Event_OnChatMessagesCleared", new Action<Dictionary<string, object>>(this.Event_OnChatMessagesCleared));
		EventManager.AddEventListener("Event_OnQuickChatEnabled", new Action<Dictionary<string, object>>(this.Event_OnQuickChatEnabled));
		EventManager.AddEventListener("Event_OnQuickChatDisabled", new Action<Dictionary<string, object>>(this.Event_OnQuickChatDisabled));
		EventManager.AddEventListener("Event_OnShowGameUserInterfaceChanged", new Action<Dictionary<string, object>>(this.Event_OnShowGameUserInterfaceChanged));
		EventManager.AddEventListener("Event_OnChatOpacityChanged", new Action<Dictionary<string, object>>(this.Event_OnChatOpacityChanged));
		EventManager.AddEventListener("Event_OnChatScaleChanged", new Action<Dictionary<string, object>>(this.Event_OnChatScaleChanged));
		EventManager.AddEventListener("Event_OnClientStopped", new Action<Dictionary<string, object>>(this.Event_OnClientStopped));
	}

	// Token: 0x06000B3F RID: 2879 RVA: 0x00035889 File Offset: 0x00033A89
	private void Start()
	{
		this.uiChat.SetOpacity(SettingsManager.ChatOpacity);
		this.uiChat.SetScale(SettingsManager.ChatScale);
	}

	// Token: 0x06000B40 RID: 2880 RVA: 0x000358AC File Offset: 0x00033AAC
	public override void OnDestroy()
	{
		EventManager.RemoveEventListener("Event_OnChatMessageAdded", new Action<Dictionary<string, object>>(this.Event_OnChatMessageAdded));
		EventManager.RemoveEventListener("Event_OnChatMessageRemoved", new Action<Dictionary<string, object>>(this.Event_OnChatMessageRemoved));
		EventManager.RemoveEventListener("Event_OnChatMessagesCleared", new Action<Dictionary<string, object>>(this.Event_OnChatMessagesCleared));
		EventManager.RemoveEventListener("Event_OnQuickChatEnabled", new Action<Dictionary<string, object>>(this.Event_OnQuickChatEnabled));
		EventManager.RemoveEventListener("Event_OnQuickChatDisabled", new Action<Dictionary<string, object>>(this.Event_OnQuickChatDisabled));
		EventManager.RemoveEventListener("Event_OnShowGameUserInterfaceChanged", new Action<Dictionary<string, object>>(this.Event_OnShowGameUserInterfaceChanged));
		EventManager.RemoveEventListener("Event_OnChatOpacityChanged", new Action<Dictionary<string, object>>(this.Event_OnChatOpacityChanged));
		EventManager.RemoveEventListener("Event_OnChatScaleChanged", new Action<Dictionary<string, object>>(this.Event_OnChatScaleChanged));
		EventManager.RemoveEventListener("Event_OnClientStopped", new Action<Dictionary<string, object>>(this.Event_OnClientStopped));
		base.OnDestroy();
	}

	// Token: 0x06000B41 RID: 2881 RVA: 0x00035988 File Offset: 0x00033B88
	private void Event_OnChatMessageAdded(Dictionary<string, object> message)
	{
		ChatMessage chatMessage = (ChatMessage)message["chatMessage"];
		this.uiChat.AddChatMessage(chatMessage, SettingsManager.Units, SettingsManager.FilterChatProfanity);
	}

	// Token: 0x06000B42 RID: 2882 RVA: 0x000359BC File Offset: 0x00033BBC
	private void Event_OnChatMessageRemoved(Dictionary<string, object> message)
	{
		ChatMessage chatMessage = (ChatMessage)message["chatMessage"];
		this.uiChat.RemoveChatMessage(chatMessage);
	}

	// Token: 0x06000B43 RID: 2883 RVA: 0x000359E6 File Offset: 0x00033BE6
	private void Event_OnChatMessagesCleared(Dictionary<string, object> message)
	{
		this.uiChat.ClearChatMessages();
	}

	// Token: 0x06000B44 RID: 2884 RVA: 0x000359F4 File Offset: 0x00033BF4
	private void Event_OnQuickChatEnabled(Dictionary<string, object> message)
	{
		QuickChatCategory category = (QuickChatCategory)message["category"];
		QuickChat[] quickChats = (QuickChat[])message["quickChats"];
		this.uiChat.ShowQuickChat(category, quickChats);
	}

	// Token: 0x06000B45 RID: 2885 RVA: 0x00035A30 File Offset: 0x00033C30
	private void Event_OnQuickChatDisabled(Dictionary<string, object> message)
	{
		this.uiChat.HideQuickChat();
	}

	// Token: 0x06000B46 RID: 2886 RVA: 0x00035A3D File Offset: 0x00033C3D
	private void Event_OnShowGameUserInterfaceChanged(Dictionary<string, object> message)
	{
		if (GlobalStateManager.UIState.Phase == UIPhase.LockerRoom)
		{
			return;
		}
		if ((bool)message["value"])
		{
			this.uiChat.Show();
			return;
		}
		this.uiChat.Hide();
	}

	// Token: 0x06000B47 RID: 2887 RVA: 0x00035A78 File Offset: 0x00033C78
	private void Event_OnChatOpacityChanged(Dictionary<string, object> message)
	{
		float opacity = (float)message["value"];
		this.uiChat.SetOpacity(opacity);
	}

	// Token: 0x06000B48 RID: 2888 RVA: 0x00035AA4 File Offset: 0x00033CA4
	private void Event_OnChatScaleChanged(Dictionary<string, object> message)
	{
		float scale = (float)message["value"];
		this.uiChat.SetScale(scale);
	}

	// Token: 0x06000B49 RID: 2889 RVA: 0x00035A30 File Offset: 0x00033C30
	private void Event_OnClientStopped(Dictionary<string, object> message)
	{
		this.uiChat.HideQuickChat();
	}

	// Token: 0x040006AF RID: 1711
	private UIChat uiChat;
}
