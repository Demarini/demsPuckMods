using System;
using System.Collections.Generic;
using Sirenix.Utilities;
using UnityEngine;

// Token: 0x02000194 RID: 404
public class UIFriendsController : UIViewController<UIFriends>
{
	// Token: 0x06000B7D RID: 2941 RVA: 0x000365F8 File Offset: 0x000347F8
	public override void Awake()
	{
		base.Awake();
		this.uiFriends = base.GetComponent<UIFriends>();
		EventManager.AddEventListener("Event_OnSteamConnected", new Action<Dictionary<string, object>>(this.Event_OnSteamConnected));
		EventManager.AddEventListener("Event_OnPersonaStateChange", new Action<Dictionary<string, object>>(this.Event_OnPersonaStateChange));
	}

	// Token: 0x06000B7E RID: 2942 RVA: 0x00036638 File Offset: 0x00034838
	public override void OnDestroy()
	{
		EventManager.RemoveEventListener("Event_OnSteamConnected", new Action<Dictionary<string, object>>(this.Event_OnSteamConnected));
		EventManager.RemoveEventListener("Event_OnPersonaStateChange", new Action<Dictionary<string, object>>(this.Event_OnPersonaStateChange));
		base.OnDestroy();
	}

	// Token: 0x06000B7F RID: 2943 RVA: 0x0003666C File Offset: 0x0003486C
	private void ParseSteamId(string steamId)
	{
		bool flag = this.uiFriends.IsFriendListed(steamId);
		bool flag2 = SteamIntegrationManager.IsFriend(steamId);
		bool flag3 = SteamIntegrationManager.IsFriendOnline(steamId);
		bool flag4 = !flag && flag2 && flag3;
		bool flag5 = flag && flag2 && flag3;
		bool flag6 = flag && (!flag2 || !flag3);
		if (flag4)
		{
			string username = SteamIntegrationManager.GetUsername(steamId);
			Texture2D avatar = SteamIntegrationManager.GetAvatar(steamId, AvatarSize.Medium);
			this.uiFriends.AddFriend(steamId, username, avatar);
			return;
		}
		if (flag5)
		{
			string username2 = SteamIntegrationManager.GetUsername(steamId);
			Texture2D avatar2 = SteamIntegrationManager.GetAvatar(steamId, AvatarSize.Medium);
			this.uiFriends.UpdateFriend(steamId, username2, avatar2);
			return;
		}
		if (flag6)
		{
			this.uiFriends.RemoveFriend(steamId);
		}
	}

	// Token: 0x06000B80 RID: 2944 RVA: 0x0003670D File Offset: 0x0003490D
	private void Event_OnSteamConnected(Dictionary<string, object> message)
	{
		if (ApplicationManager.IsDedicatedGameServer)
		{
			return;
		}
		SteamIntegrationManager.GetFriendSteamIds(false).ForEach(delegate(string steamId)
		{
			this.ParseSteamId(steamId);
		});
	}

	// Token: 0x06000B81 RID: 2945 RVA: 0x00036730 File Offset: 0x00034930
	private void Event_OnPersonaStateChange(Dictionary<string, object> message)
	{
		string steamId = (string)message["steamId"];
		if (ApplicationManager.IsDedicatedGameServer)
		{
			return;
		}
		this.ParseSteamId(steamId);
	}

	// Token: 0x040006D0 RID: 1744
	private UIFriends uiFriends;
}
