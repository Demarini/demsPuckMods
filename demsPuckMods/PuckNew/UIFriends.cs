using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// Token: 0x02000190 RID: 400
public class UIFriends : UIView
{
	// Token: 0x06000B6C RID: 2924 RVA: 0x000362F0 File Offset: 0x000344F0
	public void Initialize(VisualElement rootVisualElement)
	{
		base.View = rootVisualElement.Query("FriendsView", null);
		this.friends = base.View.Query("Friends", null);
		this.friendsList = this.friends.Query("FriendsList", null);
		this.closeIconButton = this.friends.Query("CloseIconButtonContainer", null).First().Query(null, null);
		this.closeIconButton.clicked += this.OnCloseIconButtonClicked;
		this.friendsList.Clear();
		this.friendsMap.Clear();
	}

	// Token: 0x06000B6D RID: 2925 RVA: 0x000363A4 File Offset: 0x000345A4
	public void AddFriend(string steamId, string username, Texture2D avatar)
	{
		if (this.friendsMap.ContainsKey(steamId))
		{
			return;
		}
		TemplateContainer templateContainer = this.CreateFriend(steamId, username, avatar);
		this.friendsList.Add(templateContainer);
		this.friendsMap.Add(steamId, templateContainer);
		this.SortFriends();
	}

	// Token: 0x06000B6E RID: 2926 RVA: 0x000363EC File Offset: 0x000345EC
	public void UpdateFriend(string steamId, string username, Texture2D texture)
	{
		if (!this.friendsMap.ContainsKey(steamId))
		{
			return;
		}
		Friend friend = this.friendsMap[steamId].Query("Friend", null);
		friend.Texture = texture;
		friend.Username = username;
		friend.InviteButtonClicked = delegate()
		{
			this.OnFriendInviteButtonClicked(steamId);
		};
		this.SortFriends();
	}

	// Token: 0x06000B6F RID: 2927 RVA: 0x00036468 File Offset: 0x00034668
	public void RemoveFriend(string steamId)
	{
		if (!this.friendsMap.ContainsKey(steamId))
		{
			return;
		}
		TemplateContainer element = this.friendsMap[steamId];
		this.friendsList.Remove(element);
		this.friendsMap.Remove(steamId);
	}

	// Token: 0x06000B70 RID: 2928 RVA: 0x000364AA File Offset: 0x000346AA
	public bool IsFriendListed(string steamId)
	{
		return this.friendsMap.ContainsKey(steamId);
	}

	// Token: 0x06000B71 RID: 2929 RVA: 0x000364B8 File Offset: 0x000346B8
	private TemplateContainer CreateFriend(string steamId, string username, Texture2D texture)
	{
		TemplateContainer templateContainer = this.friendAsset.Instantiate();
		Friend friend = templateContainer.Query("Friend", null);
		friend.Texture = texture;
		friend.Username = username;
		friend.InviteButtonClicked = delegate()
		{
			this.OnFriendInviteButtonClicked(steamId);
		};
		return templateContainer;
	}

	// Token: 0x06000B72 RID: 2930 RVA: 0x00036514 File Offset: 0x00034714
	private void SortFriends()
	{
		this.friendsList.Sort(delegate(VisualElement a, VisualElement b)
		{
			Friend friend = a.Query("Friend", null).First();
			Friend friend2 = b.Query("Friend", null).First();
			return string.Compare(friend.Username, friend2.Username);
		});
	}

	// Token: 0x06000B73 RID: 2931 RVA: 0x00036540 File Offset: 0x00034740
	private void OnCloseIconButtonClicked()
	{
		EventManager.TriggerEvent("Event_OnFriendsClickClose", null);
	}

	// Token: 0x06000B74 RID: 2932 RVA: 0x0003654D File Offset: 0x0003474D
	private void OnFriendInviteButtonClicked(string steamId)
	{
		EventManager.TriggerEvent("Event_OnFriendInviteButtonClicked", new Dictionary<string, object>
		{
			{
				"steamId",
				steamId
			}
		});
	}

	// Token: 0x040006C5 RID: 1733
	[Header("References")]
	public VisualTreeAsset friendAsset;

	// Token: 0x040006C6 RID: 1734
	private VisualElement friends;

	// Token: 0x040006C7 RID: 1735
	private VisualElement friendsList;

	// Token: 0x040006C8 RID: 1736
	private IconButton closeIconButton;

	// Token: 0x040006C9 RID: 1737
	private Dictionary<string, TemplateContainer> friendsMap = new Dictionary<string, TemplateContainer>();
}
