using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// Token: 0x0200018E RID: 398
public class UIFooter : UIView
{
	// Token: 0x06000B55 RID: 2901 RVA: 0x00035BF0 File Offset: 0x00033DF0
	public void Initialize(VisualElement rootVisualElement)
	{
		base.View = rootVisualElement.Query("FooterView", null);
		this.footer = base.View.Query("Footer", null);
		this.left = this.footer.Query("Left", null);
		this.center = this.footer.Query("Center", null);
		this.right = this.footer.Query("Right", null);
		this.localUserContainer = this.left.Query("LocalUserContainer", null);
		this.mmr = this.left.Query("LocalUserMmr", null).First().Query(null, null);
		this.party = this.right.Query("Party", null);
		this.partyUsers = this.party.Query("Users", null);
		this.createPartyIconButtonInstance = this.party.Query("CreatePartyIconButtonContainer", null);
		this.createPartyIconButton = this.createPartyIconButtonInstance.Query(null, null);
		this.createPartyIconButton.clicked += this.OnClickCreateParty;
		this.inviteIconButtonInstance = this.party.Query("InviteIconButtonContainer", null);
		this.inviteIconButton = this.inviteIconButtonInstance.Query(null, null);
		this.inviteIconButton.clicked += this.OnClickInvite;
		this.leavePartyIconButtonInstance = this.party.Query("LeavePartyIconButtonContainer", null);
		this.leavePartyIconButton = this.leavePartyIconButtonInstance.Query(null, null);
		this.leavePartyIconButton.clicked += this.OnClickLeaveParty;
		this.disbandPartyIconButtonInstance = this.party.Query("DisbandPartyIconButtonContainer", null);
		this.disbandPartyIconButton = this.disbandPartyIconButtonInstance.Query(null, null);
		this.disbandPartyIconButton.clicked += this.OnClickDisbandParty;
		this.ClearLocalUser();
		this.ClearPartyUsers();
		this.SetCreatePartyButtonVisibility(false);
		this.SetInviteButtonVisibility(false);
		this.SetDisbandPartyButtonVisibility(false);
		this.SetLeavePartyButtonVisibility(false);
	}

	// Token: 0x06000B56 RID: 2902 RVA: 0x00035E58 File Offset: 0x00034058
	public void SetLocalUser(string username, Texture2D avatar)
	{
		this.ClearLocalUser();
		TemplateContainer child = this.CreateUser(username, avatar, false, false);
		this.localUserContainer.Add(child);
	}

	// Token: 0x06000B57 RID: 2903 RVA: 0x00035E82 File Offset: 0x00034082
	public void ClearLocalUser()
	{
		this.localUserContainer.Clear();
	}

	// Token: 0x06000B58 RID: 2904 RVA: 0x00035E8F File Offset: 0x0003408F
	public void SetMmr(int value)
	{
		this.mmr.TargetValue = new int?(value);
	}

	// Token: 0x06000B59 RID: 2905 RVA: 0x00035EA4 File Offset: 0x000340A4
	public void AddPartyUser(string steamId, string username, Texture2D texture)
	{
		if (this.partyUserMap.ContainsKey(steamId))
		{
			return;
		}
		TemplateContainer templateContainer = this.CreateUser(username, texture, true, true);
		this.partyUsers.Add(templateContainer);
		this.partyUsers.style.display = DisplayStyle.Flex;
		this.partyUserMap.Add(steamId, templateContainer);
	}

	// Token: 0x06000B5A RID: 2906 RVA: 0x00035EFC File Offset: 0x000340FC
	public void RemovePartyUser(string steamId)
	{
		if (!this.partyUserMap.ContainsKey(steamId))
		{
			return;
		}
		VisualElement element = this.partyUserMap[steamId];
		this.partyUsers.Remove(element);
		this.partyUsers.style.display = ((this.partyUsers.childCount > 0) ? DisplayStyle.Flex : DisplayStyle.None);
		this.partyUserMap.Remove(steamId);
	}

	// Token: 0x06000B5B RID: 2907 RVA: 0x00035F65 File Offset: 0x00034165
	public void ClearPartyUsers()
	{
		this.partyUsers.Clear();
		this.partyUserMap.Clear();
		this.partyUsers.style.display = DisplayStyle.None;
	}

	// Token: 0x06000B5C RID: 2908 RVA: 0x00035F93 File Offset: 0x00034193
	private TemplateContainer CreateUser(string username, Texture2D texture, bool small = false, bool hideUsername = false)
	{
		TemplateContainer templateContainer = this.userAsset.Instantiate();
		User user = templateContainer.Query(null, null);
		user.AvatarTexture = texture;
		user.Username = username;
		templateContainer.EnableInClassList("small", small);
		templateContainer.EnableInClassList("hideUsername", hideUsername);
		return templateContainer;
	}

	// Token: 0x06000B5D RID: 2909 RVA: 0x00035FD3 File Offset: 0x000341D3
	public void SetCreatePartyButtonVisibility(bool show)
	{
		this.createPartyIconButtonInstance.style.display = (show ? DisplayStyle.Flex : DisplayStyle.None);
	}

	// Token: 0x06000B5E RID: 2910 RVA: 0x00035FF1 File Offset: 0x000341F1
	public void SetInviteButtonVisibility(bool show)
	{
		this.inviteIconButtonInstance.style.display = (show ? DisplayStyle.Flex : DisplayStyle.None);
	}

	// Token: 0x06000B5F RID: 2911 RVA: 0x0003600F File Offset: 0x0003420F
	public void SetLeavePartyButtonVisibility(bool show)
	{
		this.leavePartyIconButtonInstance.style.display = (show ? DisplayStyle.Flex : DisplayStyle.None);
	}

	// Token: 0x06000B60 RID: 2912 RVA: 0x0003602D File Offset: 0x0003422D
	public void SetDisbandPartyButtonVisibility(bool show)
	{
		this.disbandPartyIconButtonInstance.style.display = (show ? DisplayStyle.Flex : DisplayStyle.None);
	}

	// Token: 0x06000B61 RID: 2913 RVA: 0x0003604B File Offset: 0x0003424B
	private void OnClickCreateParty()
	{
		EventManager.TriggerEvent("Event_OnFooterClickCreateParty", null);
	}

	// Token: 0x06000B62 RID: 2914 RVA: 0x00036058 File Offset: 0x00034258
	private void OnClickInvite()
	{
		EventManager.TriggerEvent("Event_OnFooterClickInvite", null);
	}

	// Token: 0x06000B63 RID: 2915 RVA: 0x00036065 File Offset: 0x00034265
	private void OnClickLeaveParty()
	{
		EventManager.TriggerEvent("Event_OnFooterClickLeaveParty", null);
	}

	// Token: 0x06000B64 RID: 2916 RVA: 0x00036072 File Offset: 0x00034272
	private void OnClickDisbandParty()
	{
		EventManager.TriggerEvent("Event_OnFooterClickDisbandParty", null);
	}

	// Token: 0x040006B2 RID: 1714
	[Header("References")]
	public VisualTreeAsset userAsset;

	// Token: 0x040006B3 RID: 1715
	private VisualElement footer;

	// Token: 0x040006B4 RID: 1716
	private VisualElement left;

	// Token: 0x040006B5 RID: 1717
	private VisualElement center;

	// Token: 0x040006B6 RID: 1718
	private VisualElement right;

	// Token: 0x040006B7 RID: 1719
	private VisualElement localUserContainer;

	// Token: 0x040006B8 RID: 1720
	private Mmr mmr;

	// Token: 0x040006B9 RID: 1721
	private VisualElement party;

	// Token: 0x040006BA RID: 1722
	private VisualElement partyUsers;

	// Token: 0x040006BB RID: 1723
	private TemplateContainer createPartyIconButtonInstance;

	// Token: 0x040006BC RID: 1724
	private IconButton createPartyIconButton;

	// Token: 0x040006BD RID: 1725
	private TemplateContainer inviteIconButtonInstance;

	// Token: 0x040006BE RID: 1726
	private IconButton inviteIconButton;

	// Token: 0x040006BF RID: 1727
	private TemplateContainer leavePartyIconButtonInstance;

	// Token: 0x040006C0 RID: 1728
	private IconButton leavePartyIconButton;

	// Token: 0x040006C1 RID: 1729
	private TemplateContainer disbandPartyIconButtonInstance;

	// Token: 0x040006C2 RID: 1730
	private IconButton disbandPartyIconButton;

	// Token: 0x040006C3 RID: 1731
	private Dictionary<string, TemplateContainer> partyUserMap = new Dictionary<string, TemplateContainer>();
}
